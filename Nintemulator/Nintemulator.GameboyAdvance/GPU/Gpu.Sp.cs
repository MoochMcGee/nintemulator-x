using System.Drawing;

namespace Nintemulator.GBA.GPU
{
    public partial class Gpu
    {
        public class Sp : Layer
        {
            private const int Count = 128;

            public static int MosaicH;
            public static int MosaicV;
            public static int[][] XSizeLut;
            public static int[][] YSizeLut;

            public bool mapping;
            public int[] Raster0 = new int[ 240 ];
            public int[] Raster1 = new int[ 240 ];
            public int[] Raster2 = new int[ 240 ];
            public int[] Raster3 = new int[ 240 ];

            static Sp( )
            {
                XSizeLut = new int[ 4 ][]
                {
                    new int[4] {  8, 16, 32, 64 }, // square
                    new int[4] { 16, 32, 32, 64 }, // wide
                    new int[4] {  8,  8, 16, 32 }, // tall
                    new int[4] {  8,  8,  8,  8 }
                };

                YSizeLut = new int[ 4 ][]
                {
                    new int[4] {  8, 16, 32, 64 }, // square
                    new int[4] {  8,  8, 16, 32 }, // wide
                    new int[4] { 16, 32, 32, 64 }, // tall
                    new int[4] {  8,  8,  8,  8 }
                };
            }
            public Sp( GameboyAdvance console )
                : base( console ) { Index = 4; }

            private void Render4bpp( int[] raster, ushort attr0, ushort attr1, ushort attr2 ) { }
            private void Render8bpp( int[] raster, ushort attr0, ushort attr1, ushort attr2 ) { }
            private void Render4bppAffine( int[] raster, ushort attr0, ushort attr1, ushort attr2, short pa, short pb, short pc, short pd ) { }
            private void Render8bppAffine( int[] raster, ushort attr0, ushort attr1, ushort attr2, short pa, short pb, short pc, short pd ) { }

            public void Render( )
            {
                var oram16 = console.oram.h;

                for ( int i = Count - 1; i >= 0; i-- )
                {
                    var attr0 = oram16[ ( i << 2 ) | 0 ];
                    var attr1 = oram16[ ( i << 2 ) | 1 ];
                    var attr2 = oram16[ ( i << 2 ) | 2 ];

                    switch ( ( attr0 >> 10 ) & 3 )
                    {
                    case 0: break;    // normal
                    case 1: continue; // semi-transparent
                    case 2: continue; // window
                    case 3: continue; // prohibited
                    }

                    var x = attr1 & 511U;
                    var y = attr0 & 255U;

                    if ( ( attr0 & ( 1 << 8 ) ) != 0 )
                    {
                        var param = ( attr1 & 0x3E00 ) >> 5;

                        var pa = ( short )oram16[ param | 0x3 ];
                        var pb = ( short )oram16[ param | 0x7 ];
                        var pc = ( short )oram16[ param | 0xB ];
                        var pd = ( short )oram16[ param | 0xF ];

                        if ( ( attr0 & ( 1U << 13 ) ) != 0 )
                        {
                            switch ( ( attr2 >> 10 ) & 0x03 )
                            {
                            case 0: Render8bppAffine( Raster0, attr0, attr1, attr2, pa, pb, pc, pd ); break;
                            case 1: Render8bppAffine( Raster1, attr0, attr1, attr2, pa, pb, pc, pd ); break;
                            case 2: Render8bppAffine( Raster2, attr0, attr1, attr2, pa, pb, pc, pd ); break;
                            case 3: Render8bppAffine( Raster3, attr0, attr1, attr2, pa, pb, pc, pd ); break;
                            }
                        }
                        else
                        {
                            switch ( ( attr2 >> 10 ) & 0x03 )
                            {
                            case 0: Render4bppAffine( Raster0, attr0, attr1, attr2, pa, pb, pc, pd ); break;
                            case 1: Render4bppAffine( Raster1, attr0, attr1, attr2, pa, pb, pc, pd ); break;
                            case 2: Render4bppAffine( Raster2, attr0, attr1, attr2, pa, pb, pc, pd ); break;
                            case 3: Render4bppAffine( Raster3, attr0, attr1, attr2, pa, pb, pc, pd ); break;
                            }
                        }
                    }
                    else
                    {
                        if ( ( attr0 & ( 1U << 9 ) ) != 0 )
                            continue; // disabled

                        if ( ( attr0 & ( 1U << 13 ) ) != 0 )
                        {
                            switch ( ( attr2 >> 10 ) & 0x03 )
                            {
                            case 0: Render8bpp( Raster0, attr0, attr1, attr2 ); break;
                            case 1: Render8bpp( Raster1, attr0, attr1, attr2 ); break;
                            case 2: Render8bpp( Raster2, attr0, attr1, attr2 ); break;
                            case 3: Render8bpp( Raster3, attr0, attr1, attr2 ); break;
                            }
                        }
                        else
                        {
                            switch ( ( attr2 >> 10 ) & 0x03 )
                            {
                            case 0: Render4bpp( Raster0, attr0, attr1, attr2 ); break;
                            case 1: Render4bpp( Raster1, attr0, attr1, attr2 ); break;
                            case 2: Render4bpp( Raster2, attr0, attr1, attr2 ); break;
                            case 3: Render4bpp( Raster3, attr0, attr1, attr2 ); break;
                            }
                        }
                    }
                }
            }
            //public void RenderSprites(int priority)
            //{
            //    var oram = this.console.ORam.HalfBuffer;
            //    var pram = this.console.PRam.HalfBuffer;
            //    var vram = this.console.VRam.ByteBuffer;

            //    for (int oamNum = 127; oamNum >= 0; oamNum--)
            //    {
            //        ushort attr0 = oram[(oamNum << 2) | 0U];
            //        ushort attr1 = oram[(oamNum << 2) | 1U];
            //        ushort attr2 = oram[(oamNum << 2) | 2U];

            //        if ((attr2 >> 10 & 0x3) != priority)
            //            continue;

            //        int x = attr1 & 511;
            //        int y = attr0 & 255;

            //        switch ((attr0 >> 10) & 3)
            //        {
            //        case 1: break; // Semi-transparent
            //        case 2: continue; // Obj window
            //        case 3: continue;
            //        }

            //        int w = Sp.XSizeLut[(attr0 >> 14) & 3][(attr1 >> 14) & 3],
            //            h = Sp.YSizeLut[(attr0 >> 14) & 3][(attr1 >> 14) & 3];

            //        if (w == -1 || h == -1)
            //            continue;

            //        int rw = w,
            //            rh = h;

            //        switch (attr0 & 0x300)
            //        {
            //        case 0x000: break; // Rot-scale off, sprite displayed
            //        case 0x100: break; // Rot-scale on, normal size
            //        case 0x200: // Rot-scale off, sprite hidden
            //            continue;

            //        case 0x300: // Rot-scale on, double size
            //            rw *= 2;
            //            rh *= 2;
            //            break;
            //        }

            //        int line = (this.gpu.vclock - y) & 0xFF;

            //        if (line >= rh)
            //            continue;

            //        if ((attr0 & 0x100) == 0)
            //        {
            //            if ((attr1 & 0x2000) != 0) line = (h - 1) - line;

            //            if ((attr0 & 0x2000) != 0)
            //            {
            //                int baseSprite;

            //                if (this.Mapping)
            //                {
            //                    // 1 dimensional
            //                    baseSprite = (attr2 & 0x3FF) + ((line / 8) * (w / 8)) * 2;
            //                }
            //                else
            //                {
            //                    // 2 dimensional
            //                    baseSprite = (attr2 & 0x3FF) + ((line / 8) * 0x20);
            //                }

            //                int baseInc = 2;

            //                if ((attr1 & 0x1000) != 0)
            //                {
            //                    baseSprite += ((w / 8) - 1) * 2;
            //                    baseInc = -2;
            //                }

            //                // 256 colors
            //                for (int i = x; i < x + w; i++)
            //                {
            //                    if ((i & 0x1FF) < 240 && (window[(i & 0x1FF)] & 0x10) != 0)
            //                    {
            //                        int tx = (i - x) & 7;

            //                        if ((attr1 & 0x1000) != 0)
            //                            tx ^= 7;

            //                        int address = (baseSprite << 5) + ((line & 7) << 3) + (tx >> 0);
            //                        int colour = vram[0x10000 + address];

            //                        if (colour != 0)
            //                            raster[i & 0x1FF] = GbaTo32(pram[0x100 + colour]);
            //                    }

            //                    if (((i - x) & 7) == 7) baseSprite += baseInc;
            //                }
            //            }
            //            else
            //            {
            //                int baseSprite;

            //                if (this.Mapping)
            //                {
            //                    // 1 dimensional
            //                    baseSprite = (attr2 & 0x3FF) + ((line / 8) * (w / 8)) * 1;
            //                }
            //                else
            //                {
            //                    // 2 dimensional
            //                    baseSprite = (attr2 & 0x3FF) + ((line / 8) * 0x20);
            //                }

            //                int baseInc = 1;

            //                if ((attr1 & 0x1000) != 0)
            //                {
            //                    baseSprite += ((w / 8) - 1) * 1;
            //                    baseInc = -baseInc;
            //                }

            //                // 16 colors
            //                int palette = 0x100 + ((attr2 >> 8) & 0xF0);

            //                for (int i = x; i < x + w; i++)
            //                {
            //                    if ((i & 0x1FF) < 240 && (window[(i & 0x1FF)] & 0x10) != 0)
            //                    {
            //                        int tx = (i - x) & 7;

            //                        if ((attr1 & (1 << 12)) != 0)
            //                            tx ^= 7;

            //                        int address = (baseSprite << 5) + ((line & 7) << 2) + (tx >> 1);
            //                        int colour = vram[0x10000 + address];

            //                        if ((tx & 1) == 0)
            //                        {
            //                            colour &= 15;
            //                        }
            //                        else
            //                        {
            //                            colour >>= 4;
            //                        }

            //                        if (colour != 0)
            //                            raster[(i & 0x1FF)] = GbaTo32(pram[palette + colour]);
            //                    }

            //                    if (((i - x) & 7) == 7) baseSprite += baseInc;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            int scale = (attr0 & 0x2000) != 0 ? 2 : 1;
            //            int param = (attr1 & 0x3E00) >> 5;

            //            short dx = (short)oram[param | 0x3];
            //            short dmx = (short)oram[param | 0x7];
            //            short dy = (short)oram[param | 0xB];
            //            short dmy = (short)oram[param | 0xF];

            //            int cx = rw / 2;
            //            int cy = rh / 2;

            //            int baseSprite = attr2 & 0x3FF;
            //            int pitch;

            //            if (this.Mapping)
            //            {
            //                // 1 dimensional
            //                pitch = (w / 8) * scale;
            //            }
            //            else
            //            {
            //                // 2 dimensional
            //                pitch = 0x20;
            //            }

            //            int rx = (int)((dmx * (line - cy)) - (cx * dx) + (w << 7));
            //            int ry = (int)((dmy * (line - cy)) - (cx * dy) + (h << 7));

            //            // Draw a rot/scale sprite
            //            if ((attr0 & (1 << 13)) != 0)
            //            {
            //                // 256 colors
            //                for (int i = x; i < x + rw; i++, rx += dx, ry += dy)
            //                {
            //                    int tx = rx >> 8;
            //                    int ty = ry >> 8;

            //                    if ((i & 0x1FF) < 240 && tx >= 0 && tx < w && ty >= 0 && ty < h)
            //                    {
            //                        if ((window[(i & 0x1FF)] & 0x10) == 0)
            //                            continue;

            //                        int address = (baseSprite + ((ty / 8) * pitch) + ((tx / 8) * scale)) * 32 + ((ty & 7) * 8) + (tx & 7);
            //                        int colour = vram[0x10000 + address];

            //                        if (colour != 0)
            //                            raster[(i & 0x1FF)] = GbaTo32(pram[0x100 + colour]);
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                int palette = 0x100 + (((attr2 >> 12) & 0xF) * 16);

            //                // 16 colors
            //                for (int i = x; i < x + rw; i++, rx += dx, ry += dy)
            //                {
            //                    int tx = rx >> 8;
            //                    int ty = ry >> 8;

            //                    if ((i & 0x1FF) < 240 && tx >= 0 && tx < w && ty >= 0 && ty < h)
            //                    {
            //                        if ((window[(i & 0x1FF)] & 0x10) == 0)
            //                            continue;

            //                        int address = (baseSprite + ((ty / 8) * pitch) + ((tx / 8) * scale)) * 32 + ((ty & 7) * 4) + ((tx & 7) / 2);
            //                        int colour = vram[0x10000 + address];

            //                        if ((tx & 1) == 0)
            //                        {
            //                            colour &= 15;
            //                        }
            //                        else
            //                        {
            //                            colour >>= 4;
            //                        }

            //                        if (colour != 0)
            //                            raster[(i & 0x1FF)] = GbaTo32(pram[palette + (colour & 0xF)]);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}
        }
    }
}