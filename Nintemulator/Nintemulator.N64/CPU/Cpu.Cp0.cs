using Nintemulator.Shared;
using s08 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using s64 = System.Int64;
using u01 = System.Boolean;
using u08 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;

namespace Nintemulator.N64.CPU
{
    public partial class Cpu
    {
        // MCU
        public class Cp0 : Nintendo64.Component
        {
            #region Constants

            private const int Index = 0x00;
            private const int Random = 0x01;
            private const int EntryLo0 = 0x02;
            private const int EntryLo1 = 0x03;
            private const int Context = 0x04;
            private const int PageMask = 0x05;
            private const int Wired = 0x06;
            private const int RESERVED_0 = 0x07;
            private const int BadVAddr = 0x08;
            private const int Count = 0x09;
            private const int EntryHi = 0x0A;
            private const int Compare = 0x0B;
            private const int Status = 0x0C;
            private const int Cause = 0x0D;
            private const int EPC = 0x0E;
            private const int PRevID = 0x0F;
            private const int Config = 0x10;
            private const int LLAddr = 0x11;
            private const int WatchLo = 0x12;
            private const int WatchHi = 0x13;
            private const int XContext = 0x14;
            private const int RESERVED_1 = 0x15;
            private const int RESERVED_2 = 0x16;
            private const int RESERVED_3 = 0x17;
            private const int RESERVED_4 = 0x18;
            private const int RESERVED_5 = 0x19;
            private const int PErr = 0x1A;
            private const int CacheErr = 0x1B;
            private const int TagLo = 0x1C;
            private const int TagHi = 0x1D;
            private const int ErrorEPC = 0x1E;
            private const int RESERVED_7 = 0x1F;

            private const int EX_INT   =  0; // Interrupt
            private const int EX_MOD   =  1; // TLB Modification
            private const int EX_TLBL  =  2; // TLB Load
            private const int EX_TLBS  =  3; // TLB Store
            private const int EX_ADEL  =  4; // Address Load
            private const int EX_ADES  =  5; // Address Store
            private const int EX_IBE   =  6; // Bus Error (Instruction Fetch)
            private const int EX_DBE   =  7; // Bus Error (Data Load / Store)
            private const int EX_SYS   =  8; // Syscall Exception
            private const int EX_BRK   =  9; // Breakpoint Exception
            private const int EX_RES   = 10; // Reserved Instruction Exception
            private const int EX_CPU   = 11; // Coprocessor Unusable Exception
            private const int EX_OVF   = 12; // Arithmetic Overflow Exception
            private const int EX_TRP   = 13; // Trap Exception
            private const int EX_VCEI  = 14; // Virtual Coherency Exception Instruction
            private const int EX_FPE   = 15; // Floating Point Exception
            private const int EX_WATCH = 23; // Reference to WatchHi/WatchLo
            private const int EX_VCED  = 31; // Virtual Coherency Exception Data

            #endregion

            private TLBEntry[] itlb = new TLBEntry[0x02];
            private TLBEntry[]  tlb = new TLBEntry[0x30];
            private CacheEntry[] datacache = new CacheEntry[0x2000 / (4 * sizeof(u32))]; // 4 words per line
            private CacheEntry[] instcache = new CacheEntry[0x4000 / (8 * sizeof(u32))]; // 8 words per line
            private u64[] registers = new u64[32];

            public Cp0(Nintendo64 console)
                : base(console)
            {
                for (int i = 0; i < itlb.Length; i++)
                    itlb[i] = new TLBEntry();
                for (int i = 0; i < tlb.Length; i++)
                    tlb[i] = new TLBEntry();

                for (int i = 0; i < datacache.Length; i++)
                    datacache[i] = new CacheEntry(4); // 4-word lines
                for (int i = 0; i < instcache.Length; i++)
                    instcache[i] = new CacheEntry(8); // 8-word lines
            }

            private u32 DCachePeek(u64 address) { return 0U; }
            private u32 ICachePeek(u64 address) { return 0U; }

            public u64 FindAddress(u64 address)
            {
                var asid = 0U;
                var page = (address >> 12) & 0xFFFFFU;

                // pagewalk
                foreach (var entry in tlb)
                {
                    if (entry.vpage == page && (entry.vasid == asid || entry.g))
                    {
                        // tlb hit
                        return (entry.ppage << 12) | (address & 0xFFFU);
                    }
                }

                // tlb miss
                return 0U;
            }
            public u32 PeekDataU32(u64 address)
            {
                if (address >= 0xA0000000U && address <= 0xBFFFFFFFU)
                {
                    return console.bus.PeekU32(address & 0x1FFFFFFFU);
                }
                if (address >= 0xC0000000U && address <= 0xFFFFFFFFU)
                {
                    throw new global::System.NotImplementedException("KSEG2 Read: $" + address.ToString("x8"));
                }

                // ---i iiii iiii ss--
                u32 tag = (u32)(address & 0xFFFFF000U) >> 12; // $00000-$FFFFF
                u32 idx = (u32)(address & 0x00001FF0U) >>  4; // $00000-$001FF
                u32 sub = (u32)(address & 0x0000000CU) >>  2; // $00000-$00007
                var itm = datacache[idx];

                if (itm.dirty) { /* writeback */ itm.dirty = false; }

                if (!itm.valid || itm.tag != tag) // todo: flush data if it's valid but tags don't match
                {
                    itm.valid = true;
                    itm.tag = tag;

                    // translate virtual address into physical address and fill the cache line
                    address = FindAddress(address) & ~0x0FU;

                    itm.data[0U] = console.bus.PeekU32(address |  0U);
                    itm.data[1U] = console.bus.PeekU32(address |  4U);
                    itm.data[2U] = console.bus.PeekU32(address |  8U);
                    itm.data[3U] = console.bus.PeekU32(address | 12U);
                }

                return itm.data[sub];
            }
            public u32 PeekInstU32(u64 address)
            {
                if (address >= 0xFFFFFFFF80000000U && address <= 0xFFFFFFFF9FFFFFFFU) { throw new global::System.NotImplementedException("KSEG0 Read: $" + address.ToString("x8")); }
                if (address >= 0xFFFFFFFFA0000000U && address <= 0xFFFFFFFFBFFFFFFFU) { return console.bus.PeekU32(address & 0x1FFFFFFFU); }
                if (address >= 0xFFFFFFFFC0000000U && address <= 0xFFFFFFFFFFFFFFFFU) { throw new global::System.NotImplementedException("KSEG2 Read: $" + address.ToString("x8")); }

                // --ii iiii iiis ss--
                u32 tag = (u32)(address & 0xFFFFF000U) >> 12; // $00000-$FFFFF
                u32 idx = (u32)(address & 0x00003FE0U) >>  5; // $00000-$001FF
                u32 sub = (u32)(address & 0x0000001CU) >>  2; // $00000-$00007
                var itm = instcache[idx];

                if (!itm.valid || itm.tag != tag)
                {
                    itm.valid = true;
                    itm.tag = tag;

                    // translate virtual address into physical address and fill the cache line
                    address = FindAddress(address) & ~0x1FU;

                    itm.data[0U] = console.bus.PeekU32(address |  0U);
                    itm.data[1U] = console.bus.PeekU32(address |  4U);
                    itm.data[2U] = console.bus.PeekU32(address |  8U);
                    itm.data[3U] = console.bus.PeekU32(address | 12U);
                    itm.data[4U] = console.bus.PeekU32(address | 16U);
                    itm.data[5U] = console.bus.PeekU32(address | 20U);
                    itm.data[6U] = console.bus.PeekU32(address | 24U);
                    itm.data[7U] = console.bus.PeekU32(address | 28U);
                }

                return itm.data[sub];
            }

            public void EnterException()
            {
                registers[EPC] = cpu.pc;
            }

            public void Nmi()
            {
                // Set BEV, SR and ERL
                // Clear TS
                registers[ErrorEPC] = cpu.pc; // todo: PC-4 if delayed
                registers[Status] &= ~0x0070004U;
                registers[Status] |=  0x0050004U;
                cpu.pc = 0xBFC00000U;
            }
            public void Rst()
            {
                //T: undefined
                //Config ¬ CM || EC || EP || SB || SS || SW || EW || SC || SM || BE || EM || EB || 0 || IC || DC || undefined6
                registers[Random  ] = 0x2F;
                registers[Wired   ] = 0x00;
                registers[Config  ] = 0x00;
                registers[ErrorEPC] = 0x00000000U;

                // Set BEV and ERL
                // Clear TS and SR
                registers[Status] &= ~0x0070004U;
                registers[Status] |=  0x0040004U;
                cpu.pc = 0xBFC00000U; // kseg1: $1FC0:0000U
            }

            public class TLBEntry
            {
                public u01 g;       //  1-bit: global
                public u08 vasid;   //  8-bit: virtual address space identifier
                public u32 vpage;   // 20-bit: virtual page number
                public u08 pasid;   //  8-bit: physical address space identifier
                public u32 ppage;   // 20-bit: physical page number
            }
            public class CacheEntry
            {
                public u01 dirty;
                public u01 valid;
                public u32 tag;
                public u32[] data;

                public CacheEntry(u32 capacity)
                {
                    data = new u32[capacity];
                }
            }
        }
    }
}