using Nintemulator.Shared;
using System;

namespace Nintemulator.FC
{
    public static class Palette
    {
        public static readonly int[] NTSC = Generator.Generate( );
        public static readonly int[] PALB; // todo: pal-b color generation

        public static class Generator
        {
            private const double ATTEN = 0.746F;
            private const double BLACK = 0.519F;
            private const double WHITE = 1.962F;

            private const double M00 = +0.9468822171F, M10 = +0.6235565820F;
            private const double M01 = -0.2747876463F, M11 = -0.6356910792F;
            private const double M02 = -1.1085450346F, M12 = +1.7090069284F;

            private static readonly double[] levels_lo = { 0.350F, 0.519F, 0.962F, 1.550F }; // $0D, $1D, $2D, $3D
            private static readonly double[] levels_hi = { 1.094F, 1.506F, 1.962F, 1.962F }; // $00, $10, $20, $30

            private static bool InPhase( int phase )
            {
                return ( phase % 12 ) < 6;
            }

            private static double GenerateSignal( int pixel, int phase )
            {
                var color = ( pixel & 0x0f );
                var level = ( pixel & 0x30 ) >> 4;

                if ( color >= 0xe ) { level = 1; }

                var lo = levels_lo[ level ];
                var hi = levels_hi[ level ];

                if ( color == 0x0 ) { lo = hi; }
                if ( color >= 0xd ) { hi = lo; }

                var signal = InPhase( phase + color ) ? hi : lo;

                if ( ( ( pixel & 0x040 ) != 0 && InPhase( phase + 0 ) ) ||
                     ( ( pixel & 0x080 ) != 0 && InPhase( phase + 4 ) ) ||
                     ( ( pixel & 0x100 ) != 0 && InPhase( phase + 8 ) ) ) signal *= ATTEN;

                return signal;
            }
            private static int ModulateSignal( int pixel )
            {
                var y = 0.0;
                var i = 0.0;
                var q = 0.0;

                for ( int phase = 0; phase < 12; phase++ )
                {
                    var v = ( GenerateSignal( pixel, phase ) - BLACK ) / ( WHITE - BLACK );

                    y += v;
                    i += v * Math.Cos( MathHelper.Tau * ( ( phase + 4 ) / 12.0 ) );
                    q += v * Math.Sin( MathHelper.Tau * ( ( phase + 4 ) / 12.0 ) );
                }

                y /= 12.0;
                i /= 12.0;
                q /= 12.0;

                int r = ( int )( ( y + M00 * i + M10 * q ) * byte.MaxValue );
                int g = ( int )( ( y + M01 * i + M11 * q ) * byte.MaxValue );
                int b = ( int )( ( y + M02 * i + M12 * q ) * byte.MaxValue );

                return
                    ( MathHelper.Clamp( r, byte.MinValue, byte.MaxValue ) << 0x10 ) |
                    ( MathHelper.Clamp( g, byte.MinValue, byte.MaxValue ) << 0x08 ) |
                    ( MathHelper.Clamp( b, byte.MinValue, byte.MaxValue ) << 0x00 );
            }

            public static int[] Generate( )
            {
                var palette = new int[ 512 ];

                for ( int pixel = 0; pixel < 512; pixel++ )
                    palette[ pixel ] = ModulateSignal( pixel );

                return palette;
            }
        }
    }
}