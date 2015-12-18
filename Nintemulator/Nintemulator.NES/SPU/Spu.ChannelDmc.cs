using Nintemulator.FC.CPU;

namespace Nintemulator.FC.SPU
{
    public partial class Spu
    {
        public class ChannelDmc : Channel
        {
            private static readonly int[][] PeriodTable =
            {
                new int[] { 0x1AC, 0x17C, 0x154, 0x140, 0x11E, 0x0FE, 0x0E2, 0x0D6, 0x0BE, 0x0A0, 0x08E, 0x080, 0x06A, 0x054, 0x048, 0x036 },
                new int[] { 0x18E, 0x162, 0x13C, 0x12A, 0x114, 0x0EC, 0x0D2, 0x0C6, 0x0B0, 0x094, 0x084, 0x076, 0x062, 0x04E, 0x042, 0x032 }
            };

            private Output output;
            private Reader reader;
            private Register regs;

            public bool IrqPending;

            public override bool Enabled
            {
                get { return reader.size != 0; }
                set
                {
                    if ( !value )
                    {
                        reader.size = 0;
                    }
                    else if ( reader.size == 0 )
                    {
                        reader.addr = regs.addr;
                        reader.size = regs.size;

                        if ( !reader.buffered )
                            DoDMA( );
                    }
                }
            }

            public ChannelDmc( Famicom console, Timing.System system )
                : base( console, system )
            {
                timing = new Timing( );
                timing.Cycles =
                timing.Period = PeriodTable[ system.Serial ][ 0 ] * PHASE;
                timing.Single = 2 * PHASE;
            }

            private void ClockOutput( )
            {
                if ( output.active )
                {
                    var next = ( ( ( output.buffer << 2 & 0x4 ) - 2 ) + output.dac ) & 0xFFFF;
                    output.buffer >>= 1;

                    if ( next <= 0x7F )
                        output.dac = next;
                }
            }
            private void ClockReader( )
            {
                if ( output.shifter != 0 )
                {
                    output.shifter--;
                }
                else
                {
                    output.shifter = 7;
                    output.active = reader.buffered;

                    if ( output.active )
                    {
                        output.buffer = reader.buffer;
                        reader.buffered = false;

                        if ( reader.size != 0 )
                            DoDMA( );
                    }
                }
            }
            private void DoDMA( )
            {
                reader.buffer = cpu.PeekByte( reader.addr );
                reader.buffered = true;

                reader.addr = ( ++reader.addr & 0x7FFF ) | 0x8000;
                reader.size = ( --reader.size );

                if ( reader.size == 0 )
                {
                    if ( ( regs.ctrl & 0x40 ) != 0 )
                    {
                        reader.addr = regs.addr;
                        reader.size = regs.size;
                    }
                    else if ( ( regs.ctrl & 0x80 ) != 0 )
                    {
                        IrqPending = true;
                        cpu.Irq( 1u );
                    }
                }
            }

            public void Clock( )
            {
                timing.Cycles -= timing.Single;

                if ( timing.Cycles == 0 )
                {
                    timing.Cycles += timing.Period;

                    ClockOutput( );
                    ClockReader( );
                }
            }

            public override void PokeReg1( uint address, byte data )
            {
                regs.ctrl = data;
                timing.Period = PeriodTable[ system.Serial ][ regs.ctrl & 0xF ] * PHASE;

                if ( ( regs.ctrl & 0x80 ) == 0 )
                {
                    IrqPending = false;
                    cpu.Irq( 0u );
                }
            }
            public override void PokeReg2( uint address, byte data ) { output.dac = ( data &= 0x7F ); }
            public override void PokeReg3( uint address, byte data ) { regs.addr = ( uint )( data << 6 ) | 0xC000; }
            public override void PokeReg4( uint address, byte data ) { regs.size = ( uint )( data << 4 ) | 0x0001; }
            public override byte Render( )
            {
                return ( byte )output.dac;
            }

            struct Output
            {
                public bool active;
                public int buffer;
                public int dac;
                public int shifter;
            }
            struct Reader
            {
                public bool buffered;
                public uint addr;
                public uint size;
                public int buffer;
            }
            struct Register
            {
                public uint addr;
                public uint size;
                public int ctrl;
            }
        }
    }
}