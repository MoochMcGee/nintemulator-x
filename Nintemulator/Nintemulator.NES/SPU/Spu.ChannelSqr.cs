using System;

namespace Nintemulator.FC.SPU
{
    public partial class Spu
    {
        public class ChannelSqr : Channel
        {
            private static readonly byte[][] DutyTable =
            {
                new byte[] { 0x1F, 0x00, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F },
                new byte[] { 0x1F, 0x00, 0x00, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F },
                new byte[] { 0x1F, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x1F, 0x1F },
                new byte[] { 0x00, 0x1F, 0x1F, 0x00, 0x00, 0x00, 0x00, 0x00 }
            };

            private bool sweepReload;
            private bool validFrequency;
            private int form;
            private int step;
            private int sweepTimer;
            private int sweepIncrease;
            private int sweepDelay;
            private int sweepShift;

            public Duration Duration = new Duration( );
            public Envelope Envelope = new Envelope( );

            public override bool Enabled
            {
                get { return Duration.Counter != 0; }
                set { Duration.SetEnabled( value ); }
            }

            public ChannelSqr( Famicom console, Timing.System system )
                : base( console, system )
            {
                timing.Cycles =
                timing.Single = ( frequency + 1 ) * PHASE * 2;
            }

            private void UpdateFrequency( )
            {
                if ( ( frequency >= 0x8 ) && ( ( frequency + ( sweepIncrease & ( frequency >> sweepShift ) ) ) <= 0x7FF ) )
                {
                    timing.Single = ( frequency + 1 ) * PHASE * 2;
                    validFrequency = true;
                }
                else
                {
                    validFrequency = false;
                }
            }

            public void ClockSweep( int complement )
            {
                if ( ( sweepDelay != 0x0 ) && ( --sweepTimer == 0x0 ) )
                {
                    sweepTimer = sweepDelay;

                    if ( frequency >= 0x8 )
                    {
                        int num = frequency >> sweepShift;

                        if ( sweepIncrease == 0x0 )
                        {
                            frequency -= num - complement;
                            UpdateFrequency( );
                        }
                        else if ( ( frequency + num ) <= 0x7ff )
                        {
                            frequency += num;
                            UpdateFrequency( );
                        }
                    }
                }

                if ( sweepReload )
                {
                    sweepReload = false;
                    sweepTimer = sweepDelay;
                }
            }

            public override void PokeReg1( uint address, byte data )
            {
                form = ( data >> 6 );
                Envelope.Write( data );
                Duration.Halted = ( data & 0x20 ) != 0;
            }
            public override void PokeReg2( uint address, byte data )
            {
                sweepIncrease = ( ( data & 0x8 ) != 0x0 ) ? 0 : ~0;
                sweepShift = data & 0x7;
                sweepDelay = 0;

                if ( ( data & 0x87 ) > 0x80 )
                {
                    sweepDelay = ( ( data >> 0x4 ) & 0x7 ) + 0x1;
                    sweepReload = true;
                }

                UpdateFrequency( );
            }
            public override void PokeReg3( uint address, byte data )
            {
                frequency = ( frequency & 0x700 ) | ( data << 0 & 0x0FF );
                //timing.single = (frequency + 1) * PHASE * 2;

                UpdateFrequency( );
            }
            public override void PokeReg4( uint address, byte data )
            {
                frequency = ( frequency & 0x0FF ) | ( data << 8 & 0x700 );
                //timing.single = (frequency + 1) * PHASE * 2;

                Duration.SetCounter( data );
                Envelope.Reset = true;
                step = 0;

                UpdateFrequency( );
            }
            public override byte Render( )
            {
                int sum = timing.Cycles;
                timing.Cycles -= DELAY;

                if ( Duration.Counter != 0 && Envelope.Level != 0 && validFrequency )
                {
                    if ( timing.Cycles >= 0 )
                    {
                        return ( byte )( Envelope.Level >> DutyTable[ form ][ step ] );
                    }
                    else
                    {
                        sum >>= DutyTable[ form ][ step ];

                        for ( ; timing.Cycles < 0; timing.Cycles += timing.Single )
                            sum += Math.Min( -timing.Cycles, timing.Single ) >> DutyTable[ form ][ step = ( step - 1 ) & 0x7 ];

                        return ( byte )( ( sum * Envelope.Level ) / DELAY );
                    }
                }
                else
                {
                    var count = ( ~timing.Cycles + timing.Single ) / timing.Single;

                    step = ( step - count ) & 0x7;
                    timing.Cycles += ( count * timing.Single );
                }

                return 0;
            }
        }
    }
}