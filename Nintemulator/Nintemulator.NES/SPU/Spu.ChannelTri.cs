using System;

namespace Nintemulator.FC.SPU
{
    public partial class Spu
    {
        public class ChannelTri : Channel
        {
            private static readonly byte[] Pyramid =
            {
                0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF,
                0xF, 0xE, 0xD, 0xC, 0xB, 0xA, 0x9, 0x8, 0x7, 0x6, 0x5, 0x4, 0x3, 0x2, 0x1, 0x0,
            };

            private bool control;
            private bool halt;
            private int counter;
            private int refresh;
            private int step;

            public Duration Duration = new Duration( );

            public override bool Enabled
            {
                get { return Duration.Counter != 0; }
                set { Duration.SetEnabled( value ); }
            }

            public ChannelTri( Famicom console, Timing.System system )
                : base( console, system )
            {
                timing = new Timing( system.Period, system.Apu );
                timing.Cycles =
                timing.Single = ( frequency + 1 ) * PHASE;
            }

            public void ClockLinearCounter( )
            {
                if ( halt )
                {
                    counter = refresh;
                }
                else
                {
                    if ( counter != 0 )
                        counter--;
                }

                halt &= control;
            }

            public override void PokeReg1( uint address, byte data )
            {
                control = ( data & 0x80 ) != 0;
                refresh = ( data & 0x7F );

                Duration.Halted = ( data & 0x80 ) != 0;
            }
            public override void PokeReg3( uint address, byte data )
            {
                frequency = ( frequency & 0x700 ) | ( data << 0 & 0x0FF );
                timing.Single = ( frequency + 1 ) * PHASE;
            }
            public override void PokeReg4( uint address, byte data )
            {
                frequency = ( frequency & 0x0FF ) | ( data << 8 & 0x700 );
                timing.Single = ( frequency + 1 ) * PHASE;

                Duration.SetCounter( data );
                halt = true;
            }
            public override byte Render( )
            {
                int sum = timing.Cycles;
                timing.Cycles -= DELAY;

                if ( Duration.Counter != 0 && counter != 0 && frequency > 2 && timing.Cycles < 0 )
                {
                    sum *= Pyramid[ step ];

                    for ( ; timing.Cycles < 0; timing.Cycles += timing.Single )
                        sum += Math.Min( -timing.Cycles, timing.Single ) * Pyramid[ step = ( step + 1 ) & 0x1F ];

                    return ( byte )( sum / DELAY );
                }
                else
                {
                    timing.Cycles += ( ( ~timing.Cycles + timing.Single ) / timing.Single ) * timing.Single;

                    return Pyramid[ step ];
                }
            }
        }
    }
}