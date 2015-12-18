using s08 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using s64 = System.Int64;
using u01 = System.Boolean;
using u08 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;

namespace Nintemulator.N64
{
    public class Bus
    {
        public u08[] boot_rom;
        public u08[] boot_ram = new u08[ 64 ];
        public u08[] dram = new u08[ 4 * 1024 ];
        public u08[] iram = new u08[ 4 * 1024 ];
        public u08[] cart;
        public u08[] wram = new u08[ 4 * 1024 * 1024 ]; // 4MB RDRAM

        public s08 PeekS08( u64 address ) { return 0; }
        public s16 PeekS16( u64 address ) { return 0; }
        public s32 PeekS32( u64 address ) { return 0; }
        public s64 PeekS64( u64 address ) { return 0; }

        public u08 PeekU08( u64 address )
        {
            address &= 0x1FFFFFFFU;

            if ( address >= 0x00000000U && address <= 0x003FFFFFU ) { return wram[ address ]; } // wram
            if ( address >= 0x00400000U && address <= 0x03EFFFFFU ) { return 0; } // todo: open bus?
            if ( address >= 0x03F00000U && address <= 0x03FFFFFFU ) // wram registers
            {
                if ( address <= 0x03F00003U ) { return 0; } // 0x03F0 0000 to 0x03F0 0003  RDRAM_CONFIG_REG or RDRAM_DEVICE_TYPE_REG
                if ( address <= 0x03F00007U ) { return 0; } // 0x03F0 0004 to 0x03F0 0007  RDRAM_DEVICE_ID_REG
                if ( address <= 0x03F0000BU ) { return 0; } // 0x03F0 0008 to 0x03F0 000B  RDRAM_DELAY_REG
                if ( address <= 0x03F0000FU ) { return 0; } // 0x03F0 000C to 0x03F0 000F  RDRAM_MODE_REG
                if ( address <= 0x03F00013U ) { return 0; } // 0x03F0 0010 to 0x03F0 0013  RDRAM_REF_INTERVAL_REG
                if ( address <= 0x03F00017U ) { return 0; } // 0x03F0 0014 to 0x03F0 0017  RDRAM_REF_ROW_REG
                if ( address <= 0x03F0001BU ) { return 0; } // 0x03F0 0018 to 0x03F0 001B  RDRAM_RAS_INTERVAL_REG
                if ( address <= 0x03F0001FU ) { return 0; } // 0x03F0 001C to 0x03F0 001F  RDRAM_MIN_INTERVAL_REG
                if ( address <= 0x03F00023U ) { return 0; } // 0x03F0 0020 to 0x03F0 0023  RDRAM_ADDR_SELECT_REG
                if ( address <= 0x03F00027U ) { return 0; } // 0x03F0 0024 to 0x03F0 0027  RDRAM_DEVICE_MANUF_REG

                return 0; // todo: open bus?
            }

            if ( address >= 0x04000000U && address <= 0x040FFFFFU ) // sp registers
            {
                if ( address <= 0x04000FFFU ) { return 0; } // SP_DMEM
                if ( address <= 0x04001FFFU ) { return 0; } // SP_IMEM
                if ( address <= 0x0403FFFFU ) { return 0; }
                if ( address <= 0x04040003U ) { return 0; } // SP_MEM_ADDR_REG
                if ( address <= 0x04040007U ) { return 0; } // SP_DRAM_ADDR_REG
                if ( address <= 0x0404000BU ) { return 0; } // SP_RD_LEN_REG
                if ( address <= 0x0404000FU ) { return 0; } // SP_WR_LEN_REG
                if ( address <= 0x04040013U ) { return 0; } // SP_STATUS_REG
                if ( address <= 0x04040017U ) { return 0; } // SP_DMA_FULL_REG
                if ( address <= 0x0404001BU ) { return 0; } // SP_DMA_BUSY_REG
                if ( address <= 0x0404001FU ) { return 0; } // SP_SEMAPHORE_REG
                if ( address <= 0x0407FFFFU ) { return 0; }
                if ( address <= 0x04080003U ) { return 0; } // SP_PC_REG
                if ( address <= 0x04080007U ) { return 0; } // SP_IBIST_REG

                return 0; // todo: open bus?
            }

            if ( address >= 0x04100000U && address <= 0x041FFFFFU ) // dp registers
            {
                if ( address <= 0x04100003U ) { return 0; } // DPC_START_REG
                if ( address <= 0x04100007U ) { return 0; } // DPC_END_REG
                if ( address <= 0x0410000BU ) { return 0; } // DPC_CURRENT_REG
                if ( address <= 0x0410000FU ) { return 0; } // DPC_STATUS_REG
                if ( address <= 0x04100013U ) { return 0; } // DPC_CLOCK_REG
                if ( address <= 0x04100017U ) { return 0; } // DPC_BUFBUSY_REG
                if ( address <= 0x0410001BU ) { return 0; } // DPC_PIPEBUSY_REG
                if ( address <= 0x0410001FU ) { return 0; } // DPC_TMEM_REG

                return 0; // todo: open bus?
            }

            if ( address >= 0x04200000U && address <= 0x042FFFFFU ) // dp registers
            {
                if ( address <= 0x04200003U ) { return 0; } // DPS_TBIST_REG
                if ( address <= 0x04200007U ) { return 0; } // DPS_TEST_MODE_REG
                if ( address <= 0x0420000BU ) { return 0; } // DPS_BUFTEST_ADDR_REG
                if ( address <= 0x0420000FU ) { return 0; } // DPS_BUFTEST_DATA_REG

                return 0; // todo: open bus?
            }

            if ( address >= 0x04300000U && address <= 0x043FFFFFU ) // mi registers
            {
                if ( address <= 0x04300003U ) { return 0; } // MI_MODE_REG
                if ( address <= 0x04300007U ) { return 0; } // MI_VERSION_REG
                if ( address <= 0x0430000BU ) { return 0; } // MI_INTR_REG
                if ( address <= 0x0430000FU ) { return 0; } // MI_INTR_MASK_REG

                return 0; // todo: open bus?
            }

            if ( address >= 0x04400000U && address <= 0x044FFFFFU ) // vi registers
            {
                if ( address <= 0x04400003U ) { return 0; } // VI_CONTROL_REG
                if ( address <= 0x04400007U ) { return 0; } // VI_DRAM_ADDR_REG
                if ( address <= 0x0440000BU ) { return 0; } // VI_H_WIDTH_REG
                if ( address <= 0x0440000FU ) { return 0; } // VI_V_INTR_REG
                if ( address <= 0x04400013U ) { return 0; } // VI_V_CURRENT_LINE_REG
                if ( address <= 0x04400017U ) { return 0; } // VI_TIMING_REG
                if ( address <= 0x0440001BU ) { return 0; } // VI_V_SYNC_REG
                if ( address <= 0x0440001FU ) { return 0; } // VI_H_SYNC_REG
                if ( address <= 0x04400023U ) { return 0; } // VI_H_SYNC_LEAP_REG
                if ( address <= 0x04400027U ) { return 0; } // VI_H_VIDEO_REG
                if ( address <= 0x0440002BU ) { return 0; } // VI_V_VIDEO_REG
                if ( address <= 0x0440002FU ) { return 0; } // VI_V_BURST_REG
                if ( address <= 0x04400033U ) { return 0; } // VI_X_SCALE_REG
                if ( address <= 0x04400037U ) { return 0; } // VI_Y_SCALE_REG

                return 0; // todo: open bus?
            }

            if ( address >= 0x04500000U && address <= 0x045FFFFFU ) // ai registers
            {
                if ( address <= 0x04500003U ) { return 0; } // AI_DRAM_ADDR_REG
                if ( address <= 0x04500007U ) { return 0; } // AI_LEN_REG
                if ( address <= 0x0450000BU ) { return 0; } // AI_CONTROL_REG
                if ( address <= 0x0450000FU ) { return 0; } // AI_STATUS_REG
                if ( address <= 0x04500013U ) { return 0; } // AI_DACRATE_REG
                if ( address <= 0x04500017U ) { return 0; } // AI_BITRATE_REG

                return 0; // todo: open bus?
            }

            if ( address >= 0x04600000U && address <= 0x046FFFFFU ) // pi registers
            {
                if ( address <= 0x04600003U ) { return 0; } // PI_DRAM_ADDR_REG
                if ( address <= 0x04600007U ) { return 0; } // PI_CART_ADDR_REG
                if ( address <= 0x0460000BU ) { return 0; } // PI_RD_LEN_REG
                if ( address <= 0x0460000FU ) { return 0; } // PI_WR_LEN_REG
                if ( address <= 0x04600013U ) { return 0; } // PI_STATUS_REG
                if ( address <= 0x04600017U ) { return 0; } // PI_BSD_DOM1_LAT_REG
                if ( address <= 0x0460001BU ) { return 0; } // PI_BSD_DOM1_PWD_REG
                if ( address <= 0x0460001FU ) { return 0; } // PI_BSD_DOM1_PGS_REG
                if ( address <= 0x04600023U ) { return 0; } // PI_BSD_DOM1_RLS_REG
                if ( address <= 0x04600027U ) { return 0; } // PI_BSD_DOM2_LAT_REG
                if ( address <= 0x0460002BU ) { return 0; } // PI_BSD_DOM2_PWD_REG
                if ( address <= 0x0460002FU ) { return 0; } // PI_BSD_DOM2_PGS_REG
                if ( address <= 0x04600033U ) { return 0; } // PI_BSD_DOM2_RLS_REG

                return 0; // todo: open bus?
            }

            if ( address >= 0x04700000U && address <= 0x047FFFFFU ) // ri registers
            {
                if ( address <= 0x04700003U ) { return 0; } // RI_MODE_REG
                if ( address <= 0x04700007U ) { return 0; } // RI_CONFIG_REG
                if ( address <= 0x0470000BU ) { return 0; } // RI_CURRENT_LOAD_REG
                if ( address <= 0x0470000FU ) { return 0; } // RI_SELECT_REG
                if ( address <= 0x04700013U ) { return 0; } // RI_REFRESH_REG
                if ( address <= 0x04700017U ) { return 0; } // RI_LATENCY_REG
                if ( address <= 0x0470001BU ) { return 0; } // RI_RERROR_REG
                if ( address <= 0x0470001FU ) { return 0; } // RI_WERROR_REG

                return 0; // todo: open bus?
            }

            if ( address >= 0x04800000U && address <= 0x048FFFFFU ) // si registers
            {
                if ( address <= 0x04600003U ) { return 0; } // SI_DRAM_ADDR_REG
                if ( address <= 0x04600007U ) { return 0; } // SI_PIF_ADDR_RD64B_REG
                if ( address <= 0x0460000BU ) { return 0; } // Reserved
                if ( address <= 0x0460000FU ) { return 0; } // ?
                if ( address <= 0x04600013U ) { return 0; } // SI_PIF_ADDR_WR64B_REG
                if ( address <= 0x04600017U ) { return 0; } // Reserved
                if ( address <= 0x0460001BU ) { return 0; } // SI_STATUS_REG

                return 0; // todo: open bus?
            }

            if ( address >= 0x04900000U && address <= 0x04FFFFFFU ) { } // unused
            if ( address >= 0x05000000U && address <= 0x05FFFFFFU ) { } // cartridge domain 2
            if ( address >= 0x06000000U && address <= 0x07FFFFFFU ) { } // cartridge domain 1
            if ( address >= 0x08000000U && address <= 0x0FFFFFFFU ) { } // cartridge domain 2 (sram)
            if ( address >= 0x10000000U && address <= 0x1F39FFFFU ) { return cart[ address & 0x01FFFFFFU ]; } // rom
            if ( address >= 0x1F3A0000U && address <= 0x1FBFFFFFU ) { } // blah
            if ( address >= 0x1FC00000U && address <= 0x1FC007BFU ) { return boot_rom[ address & 0x000007FFU ]; } // boot rom
            if ( address >= 0x1FC007C0U && address <= 0x1FC007FFU ) { return boot_ram[ address & 0x0000003FU ]; } // boot ram
            if ( address >= 0x1FC00800U && address <= 0x1FCFFFFFU ) { } // reserved
            if ( address >= 0x1FD00000U && address <= 0x7FFFFFFFU ) { } // reserved

            return 0;
        }
        public u16 PeekU16( u64 address )
        {
            u16 b0 = PeekU08( address | 1U );
            u16 b1 = PeekU08( address & ~1U );

            return ( u16 )( b0 | ( b1 << 8 ) );
        }
        public u32 PeekU32( u64 address )
        {
            u32 h0 = PeekU16( address | 2U );
            u32 h1 = PeekU16( address & ~2U );

            return ( u32 )( h0 | ( h1 << 16 ) );
        }
        public u64 PeekU64( u64 address )
        {
            u64 w0 = PeekU32( address | 4U );
            u64 w1 = PeekU32( address & ~4U );

            return ( u64 )( w0 | ( w1 << 32 ) );
        }

        public void PokeU08( u64 address, u08 data ) { }
        public void PokeU16( u64 address, u16 data ) { }
        public void PokeU32( u64 address, u32 data ) { }
        public void PokeU64( u64 address, u64 data ) { }
    }
}