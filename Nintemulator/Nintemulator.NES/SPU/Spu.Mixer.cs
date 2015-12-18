namespace Nintemulator.FC.SPU
{
    public partial class Spu
    {
        public static class Mixer
        {
            private static short[][][][][] mix_table;
            private static int accum;
            private static int prev_x;
            private static int prev_y;

            static Mixer( )
            {
                mix_table = new short[ 16 ][][][][];

                for ( int sq1 = 0; sq1 < 16; sq1++ )
                {
                    mix_table[ sq1 ] = new short[ 16 ][][][];

                    for ( int sq2 = 0; sq2 < 16; sq2++ )
                    {
                        mix_table[ sq1 ][ sq2 ] = new short[ 16 ][][];

                        for ( int tri = 0; tri < 16; tri++ )
                        {
                            mix_table[ sq1 ][ sq2 ][ tri ] = new short[ 16 ][];

                            for ( int noi = 0; noi < 16; noi++ )
                            {
                                mix_table[ sq1 ][ sq2 ][ tri ][ noi ] = new short[ 128 ];

                                for ( int dmc = 0; dmc < 128; dmc++ )
                                {
                                    var sqr = ( 95.88 / ( 8128.0 / ( sq1 + sq2 ) + 100 ) );
                                    var tnd = ( 159.79 / ( 1.0 / ( tri / 8227.0 + noi / 12241.0 + dmc / 22638.0 ) + 100 ) );

                                    mix_table[ sq1 ][ sq2 ][ tri ][ noi ][ dmc ] = ( short )( ( sqr + tnd ) * 32767 );
                                }
                            }
                        }
                    }
                }
            }

            private static short Filter( int value )
            {
                const int POLE = ( int )( 32767 * ( 1.0 - 0.9999 ) );

                accum -= prev_x;
                prev_x = value << 15;
                accum += prev_x - prev_y * POLE;
                prev_y = accum >> 15;

                return ( short )prev_y;
            }

            public static short MixSamples( byte sq1, byte sq2, byte tri, byte noi, byte dmc )
            {
                return Filter( mix_table[ sq1 ][ sq2 ][ tri ][ noi ][ dmc ] );
            }
            public static short MixSamples( byte sq1, byte sq2, byte tri, byte noi, byte dmc, short ext )
            {
                return Filter(
                    ( mix_table[ sq1 ][ sq2 ][ tri ][ noi ][ dmc ] + ext ) >> 1 );
            }
        }
    }
}