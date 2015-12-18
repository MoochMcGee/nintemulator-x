using System;

namespace Nintemulator.FC.SPU
{
    public partial class Spu
    {
        public class ChannelNoi : Channel
        {
            private static readonly int[][] PeriodTable =
            {
                new int[] { 4, 8, 16, 32, 64, 96, 128, 160, 202, 254, 380, 508, 762, 1016, 2034, 4068 },
                new int[] { 4, 8, 14, 30, 60, 88, 118, 148, 188, 236, 354, 472, 708,  944, 1890, 3778 }
            };

            private int shift = 1;
            private int value = 13;

            public Duration Duration = new Duration( );
            public Envelope Envelope = new Envelope( );

            public override bool Enabled
            {
                get { return Duration.Counter != 0; }
                set { Duration.SetEnabled( value ); }
            }

            public ChannelNoi( Famicom console, Timing.System system )
                : base( console, system )
            {
                timing = new Timing( system.Period, system.Apu );
                timing.Cycles =
                timing.Single = PeriodTable[ system.Serial ][ 0 ] * PHASE;
            }

            public override void PokeReg1( uint address, byte data )
            {
                Envelope.Write( data );
                Duration.Halted = ( data & 0x20 ) != 0;
            }
            public override void PokeReg2( uint address, byte data ) { }
            public override void PokeReg3( uint address, byte data )
            {
                shift = ( data & 0x80 ) != 0 ? 8 : 13;
                timing.Single = PeriodTable[ system.Serial ][ data & 0xF ] * PHASE;
            }
            public override void PokeReg4( uint address, byte data )
            {
                Duration.SetCounter( data );
                Envelope.Reset = true;
            }
            public override byte Render( )
            {
                int sum = timing.Cycles;
                timing.Cycles -= DELAY;

                if ( Duration.Counter != 0 && Envelope.Level != 0 )
                {
                    if ( timing.Cycles >= 0 )
                    {
                        if ( ( value & 0x0001 ) == 0 )
                            return Envelope.Level;
                    }
                    else
                    {
                        if ( ( value & 0x0001 ) != 0 )
                            sum = 0;

                        for ( ; timing.Cycles < 0; timing.Cycles += timing.Single )
                        {
                            value = ( ( value << 14 ^ value << shift ) & 0x4000 ) | ( value >> 1 );

                            if ( ( value & 0x0001 ) == 0 )
                                sum += Math.Min( -timing.Cycles, timing.Single );
                        }

                        return ( byte )( ( sum * Envelope.Level ) / DELAY );
                    }
                }
                else
                {
                    for ( ; timing.Cycles < 0; timing.Cycles += timing.Single )
                        value = ( ( value << 14 ^ value << shift ) & 0x4000 ) | ( value >> 1 );
                }

                return 0;
            }
        }
    }
}