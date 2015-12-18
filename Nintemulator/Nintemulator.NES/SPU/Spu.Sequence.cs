namespace Nintemulator.FC.SPU
{
    public partial class Spu
    {
        private static readonly int[][][] TimingTable =
        {
            new int[][]
            {
                new int[] { 3729, 3728, 3729, 3729 },
                new int[] { 3729, 3728, 3729, 3729 + 3726 }
            },
            new int[][]
            {
                new int[] { 4157, 4157, 4156, 4157 },
                new int[] { 4157, 4157, 4156, 4157 + 4156 }
            }
        };

        private Timing frameTimer;

        partial void InitializeSequence( )
        {
            frameTimer.Cycles = TimingTable[ System.Serial ][ mode ][ 0 ];
            frameTimer.Single = 1;
        }

        private void ClockSequence( )
        {
            if ( frameTimer.Cycles != 0 && --frameTimer.Cycles == 0 )
            {
                switch ( step )
                {
                case 0: ClockQuad( ); break;
                case 1: ClockQuad( ); ClockHalf( ); break;
                case 2: ClockQuad( ); break;
                case 3: ClockQuad( ); ClockHalf( );

                    if ( mode == 0 && irq_enabled )
                    {
                        irq_pending = true;
                        cpu.Irq( 1u );
                    }

                    break;
                }

                step = ( step + 1 ) & 0x3;
                frameTimer.Cycles += TimingTable[ System.Serial ][ mode ][ step ];
            }
        }
    }
}