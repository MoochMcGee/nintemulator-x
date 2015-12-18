using Nintemulator.Shared;
using half = System.UInt16;
using word = System.UInt32;

namespace Nintemulator.GBA.CPU
{
    public partial class Cpu : GameboyAdvance.Processor
    {
        public Flags cpsr = new Flags( );
        public Flags spsr;
        public Flags spsr_abt = new Flags( );
        public Flags spsr_fiq = new Flags( );
        public Flags spsr_irq = new Flags( );
        public Flags spsr_svc = new Flags( );
        public Flags spsr_und = new Flags( );
        public Pipeline pipeline = new Pipeline( );
        public Register sp;
        public Register lr;
        public Register pc;
        public Register r14_abt, r13_abt;
        public Register r14_fiq, r13_fiq, r12_fiq, r11_fiq, r10_fiq, r09_fiq, r08_fiq;
        public Register r14_irq, r13_irq;
        public Register r14_svc, r13_svc;
        public Register r14_und, r13_und;
        public Register r14_usr, r13_usr, r12_usr, r11_usr, r10_usr, r09_usr, r08_usr;
        public Register[] registers = new Register[ 16 ];
        public Register16 ief, irf;
        public bool halt;
        public bool ime;
        public bool irqline;
        public uint code;

        public Cpu( GameboyAdvance console, Timing.System system )
            : base( console, system )
        {
            r13_abt = new Register( );
            r14_abt = new Register( );

            r08_fiq = new Register( );
            r09_fiq = new Register( );
            r10_fiq = new Register( );
            r11_fiq = new Register( );
            r12_fiq = new Register( );
            r13_fiq = new Register( );
            r14_fiq = new Register( );

            r13_irq = new Register( );
            r14_irq = new Register( );

            r13_svc = new Register( );
            r14_svc = new Register( );

            r13_und = new Register( );
            r14_und = new Register( );

            r08_usr = new Register( );
            r09_usr = new Register( );
            r10_usr = new Register( );
            r11_usr = new Register( );
            r12_usr = new Register( );
            r13_usr = new Register( );
            r14_usr = new Register( );

            registers[ 0U ] = new Register( );
            registers[ 1U ] = new Register( );
            registers[ 2U ] = new Register( );
            registers[ 3U ] = new Register( );
            registers[ 4U ] = new Register( );
            registers[ 5U ] = new Register( );
            registers[ 6U ] = new Register( );
            registers[ 7U ] = new Register( );
            registers[ 15U ] = new Register( );

            Isr( Mode.SVC, Vector.RST );
        }

        private void OverflowCarryAdd( uint a, uint b, uint r )
        {
            var overflow = ~( a ^ b ) & ( a ^ r );

            cpsr.n = r >> 31;
            cpsr.z = r == 0 ? 1U : 0U;
            cpsr.c = ( overflow ^ a ^ b ^ r ) >> 31;
            cpsr.v = ( overflow ) >> 31;
        }
        private void OverflowCarrySub( uint a, uint b, uint r )
        {
            OverflowCarryAdd( a, ~b, r );
        }

        public void Dispatch( )
        {
            if ( console.Dma0.Request ) { console.Dma0.Request = false; console.Dma0.Transfer( ); }
            if ( console.Dma1.Request ) { console.Dma1.Request = false; console.Dma1.Transfer( ); }
            if ( console.Dma2.Request ) { console.Dma2.Request = false; console.Dma2.Transfer( ); }
            if ( console.Dma3.Request ) { console.Dma3.Request = false; console.Dma3.Transfer( ); }

            gpu.Update( Cycles );
            spu.Update( Cycles );

            if ( console.Timer0.Enabled ) console.Timer0.Update( Cycles );
            if ( console.Timer1.Enabled ) console.Timer1.Update( Cycles );
            if ( console.Timer2.Enabled ) console.Timer2.Update( Cycles );
            if ( console.Timer3.Enabled ) console.Timer3.Update( Cycles );
        }

        public void ChangeMode( uint mode )
        {
            ChangeRegisters( mode );

            if ( spsr != null )
                spsr.Load( cpsr.Save( ) );
        }
        private void ChangeRegisters( uint mode )
        {
            if ( mode == Mode.FIQ )
            {
                registers[  8 ] = r08_fiq;
                registers[  9 ] = r09_fiq;
                registers[ 10 ] = r10_fiq;
                registers[ 11 ] = r11_fiq;
                registers[ 12 ] = r12_fiq;
            }
            else
            {
                registers[  8 ] = r08_usr;
                registers[  9 ] = r09_usr;
                registers[ 10 ] = r10_usr;
                registers[ 11 ] = r11_usr;
                registers[ 12 ] = r12_usr;
            }

            switch ( mode )
            {
            case Mode.ABT: registers[ 13 ] = r13_abt; registers[ 14 ] = r14_abt; spsr = spsr_abt; break;
            case Mode.FIQ: registers[ 13 ] = r13_fiq; registers[ 14 ] = r14_fiq; spsr = spsr_fiq; break;
            case Mode.IRQ: registers[ 13 ] = r13_irq; registers[ 14 ] = r14_irq; spsr = spsr_irq; break;
            case Mode.SVC: registers[ 13 ] = r13_svc; registers[ 14 ] = r14_svc; spsr = spsr_svc; break;
            case Mode.SYS: registers[ 13 ] = r13_usr; registers[ 14 ] = r14_usr; spsr = null; break;
            case Mode.UND: registers[ 13 ] = r13_und; registers[ 14 ] = r14_und; spsr = spsr_und; break;
            case Mode.USR: registers[ 13 ] = r13_usr; registers[ 14 ] = r14_usr; spsr = null; break;
            }

            sp = registers[ 13 ];
            lr = registers[ 14 ];
            pc = registers[ 15 ];
        }
        public bool GetCondition( uint code )
        {
            switch ( code & 15U )
            {
            default:
            case Condition.EQ: return cpsr.z != 0U;
            case Condition.NE: return cpsr.z == 0U;
            case Condition.CS: return cpsr.c != 0U;
            case Condition.CC: return cpsr.c == 0U;
            case Condition.MI: return cpsr.n != 0U;
            case Condition.PL: return cpsr.n == 0U;
            case Condition.VS: return cpsr.v != 0U;
            case Condition.VC: return cpsr.v == 0U;
            case Condition.HI: return cpsr.c != 0U && cpsr.z == 0U;
            case Condition.LS: return cpsr.c == 0U || cpsr.z != 0U;
            case Condition.GE: return cpsr.n == cpsr.v;
            case Condition.LT: return cpsr.n != cpsr.v;
            case Condition.GT: return cpsr.n == cpsr.v && cpsr.z == 0;
            case Condition.LE: return cpsr.n != cpsr.v || cpsr.z != 0;
            case Condition.AL: return true;
            case Condition.NV: return false;
            }
        }
        public void Interrupt( half irq )
        {
            irf.w |= irq;
        }
        public void Isr( uint mode, uint vector )
        {
            ChangeMode( mode );

            cpsr.t  = 0u;
            cpsr.i  = 1u;
            cpsr.f |= ( vector == Vector.FIQ || vector == Vector.RST ) ? 1u : 0u;
            cpsr.m  = mode;

            lr.value = pipeline.decode.addr;
            pc.value = vector;
            pipeline.refresh = true;
        }

        //public bool PrivilegeMode( ) { return cpsr.m != Mode.USR; }
        //public bool ExceptionMode( ) { return cpsr.m != Mode.USR && cpsr.m != Mode.SYS; }

        public override void Update( )
        {
            irqline = ( ( ief.w & irf.w ) != 0 ) && ime;

            if ( halt )
            {
                Cycles += 1;
            }
            else
            {
                switch ( cpsr.t )
                {
                case 0U: this.Armv4Execute( ); break;
                case 1U: this.ThumbExecute( ); break;
                }
            }

            Dispatch( );
            Cycles = 0;
        }

        protected override void OnInitialize( )
        {
            base.OnInitialize( );

            Armv4Initialize( );
            ThumbInitialize( );

            pipeline.refresh = true;
        }

        public class Flags
        {
            public uint n, z, c, v;
            public uint r;
            public uint i, f, t, m;

            public void Load( uint value )
            {
                n = ( value >> 31 ) & 1U;
                z = ( value >> 30 ) & 1U;
                c = ( value >> 29 ) & 1U;
                v = ( value >> 28 ) & 1U;
                r = ( value >>  8 ) & 0xFFFFF;
                i = ( value >>  7 ) & 1U;
                f = ( value >>  6 ) & 1U;
                t = ( value >>  5 ) & 1U;
                m = ( value >>  0 ) & 31U;
            }
            public uint Save( )
            {
                return
                    ( n << 31 ) | ( z << 30 ) | ( c << 29 ) | ( v << 28 ) |
                    ( r <<  8 ) |
                    ( i <<  7 ) | ( f <<  6 ) | ( t <<  5 ) | ( m <<  0 );
            }
        }
        public class Pipeline
        {
            public bool refresh;
            public Stage execute;
            public Stage decode;
            public Stage fetch;

            public struct Stage
            {
                public word addr;
                public word data;
            }
        }
        public class Register
        {
            public uint value;
        }
        public static class Condition
        {
            public const int EQ = 0x0; //  Z
            public const int NE = 0x1; // ~Z
            public const int CS = 0x2; //  C
            public const int CC = 0x3; // ~C
            public const int MI = 0x4; //  N
            public const int PL = 0x5; // ~N
            public const int VS = 0x6; //  V
            public const int VC = 0x7; // ~V
            public const int HI = 0x8; //  C & ~Z
            public const int LS = 0x9; // ~C |  Z
            public const int GE = 0xA; //  N ~^ V
            public const int LT = 0xB; //  N  ^ V
            public const int GT = 0xC; // ~Z & (N ~^ V)
            public const int LE = 0xD; //  Z | (N  ^ V)
            public const int AL = 0xE; //  1
            public const int NV = 0xF; //  0
        }
        public static class Mode
        {
            public const word USR = 0x10;
            public const word FIQ = 0x11;
            public const word IRQ = 0x12;
            public const word SVC = 0x13;
            public const word ABT = 0x17;
            public const word UND = 0x1B;
            public const word SYS = 0x1F;
        }
        public static class Source
        {
            public const half None   = 0;
            public const half VBlank = 1 << 0x0; // 0 - LCD V-Blank
            public const half HBlank = 1 << 0x1; // 1 - LCD H-Blank
            public const half VCheck = 1 << 0x2; // 2 - LCD V-Counter Match
            public const half Timer0 = 1 << 0x3; // 3 - Timer 0 Overflow
            public const half Timer1 = 1 << 0x4; // 4 - Timer 1 Overflow
            public const half Timer2 = 1 << 0x5; // 5 - Timer 2 Overflow
            public const half Timer3 = 1 << 0x6; // 6 - Timer 3 Overflow
            public const half Serial = 1 << 0x7; // 7 - Serial Communication
            public const half Dma0   = 1 << 0x8; // 8 - DMA 0
            public const half Dma1   = 1 << 0x9; // 9 - DMA 1
            public const half Dma2   = 1 << 0xA; // a - DMA 2
            public const half Dma3   = 1 << 0xB; // b - DMA 3
            public const half Joypad = 1 << 0xC; // c - Keypad
            public const half Cart   = 1 << 0xD; // d - Game Pak
            public const half Res0   = 1 << 0xE; // e - Not used
            public const half Res1   = 1 << 0xF; // f - Not used
        }
        public static class Vector
        {
            public const word RST = 0x00U;
            public const word UND = 0x04U;
            public const word SWI = 0x08U;
            public const word PAB = 0x0CU;
            public const word DAB = 0x10U;
            public const word RES = 0x14U;
            public const word IRQ = 0x18U;
            public const word FIQ = 0x1CU;
        }
    }
}