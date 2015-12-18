using Nintemulator.Shared;

namespace Nintemulator.GBA.GPU
{
    public partial class Gpu
    {
        private static int[] ColourLut = new int[ 32768 ];

        private int[] window = new int[ 240 ];

        // I'd be dishonest if I tried to pass off the "blend" methods as my own.
        // Credit for these goes to higan.

        private void blend( )
        {
            int backdrop = console.pram.h[ 0 ];

            for ( int i = 0; i < 240; i++ )
            {
                int c1 = backdrop;
                int c2 = backdrop;
                int m1 = 0x20;
                int m2 = 0x20;

                for ( int p = 3; p >= 0; p-- )
                {
                    if ( bg3.enabled && p == bg3.priority      && bg3.enable[ i ] ) { m2 = m1; c2 = c1; m1 = 0x08; c1 = bg3.raster[ i ]; }
                    if ( bg2.enabled && p == bg2.priority      && bg2.enable[ i ] ) { m2 = m1; c2 = c1; m1 = 0x04; c1 = bg2.raster[ i ]; }
                    if ( bg1.enabled && p == bg1.priority      && bg1.enable[ i ] ) { m2 = m1; c2 = c1; m1 = 0x02; c1 = bg1.raster[ i ]; }
                    if ( bg0.enabled && p == bg0.priority      && bg0.enable[ i ] ) { m2 = m1; c2 = c1; m1 = 0x01; c1 = bg0.raster[ i ]; }
                    if ( spr.enabled && p == spr.priority[ i ] && spr.enable[ i ] ) { m2 = m1; c2 = c1; m1 = 0x10; c1 = spr.raster[ i ]; }
                }

                if ( ( window[ i ] & 0x20 ) != 0 )
                {
                    var b1 = ( blendTarget1 & m1 ) != 0;
                    var b2 = ( blendTarget2 & m2 ) != 0;

                    if ( c1 > 0x7fff && b2 ) { c1 = blend( c1, eva, c2, evb ); }
                    else if ( blendType == 1 && b1 && b2 ) { c1 = blend( c1, eva, c2, evb ); }
                    else if ( blendType == 2 && b1 ) { c1 = blend( c1, 16 - evy, 0x7fff, evy ); }
                    else if ( blendType == 3 && b1 ) { c1 = blend( c1, 16 - evy, 0x0000, evy ); }
                }

                c1 = ( c1 & 0x7fff );

                raster[ i ] = ColourLut[ c1 ];
            }
        }
        private int blend( int color1, int eva, int color2, int evb )
        {
            int r1 = ( color1 >> 0 ) & 31, g1 = ( color1 >> 5 ) & 31, b1 = ( color1 >> 10 ) & 31;
            int r2 = ( color2 >> 0 ) & 31, g2 = ( color2 >> 5 ) & 31, b2 = ( color2 >> 10 ) & 31;

            int r = ( r1 * eva + r2 * evb ) >> 4;
            int g = ( g1 * eva + g2 * evb ) >> 4;
            int b = ( b1 * eva + b2 * evb ) >> 4;

            if ( r > 31 ) r = 31;
            if ( g > 31 ) g = 31;
            if ( b > 31 ) b = 31;

            return ( r << 0 ) | ( g << 5 ) | ( b << 10 );
        }

        private void RenderMode0( )
        {
            if ( spr.enabled ) RenderSprite( );
            if ( bg0.enabled ) RenderLinear( bg0 );
            if ( bg1.enabled ) RenderLinear( bg1 );
            if ( bg2.enabled ) RenderLinear( bg2 );
            if ( bg3.enabled ) RenderLinear( bg3 );

            blend( );
        }
        private void RenderMode1( )
        {
            if ( spr.enabled ) RenderSprite( );
            if ( bg0.enabled ) RenderLinear( bg0 );
            if ( bg1.enabled ) RenderLinear( bg1 );
            if ( bg2.enabled ) RenderAffine( bg2 );

            blend( );
        }
        private void RenderMode2( )
        {
            if ( spr.enabled ) RenderSprite( );
            if ( bg2.enabled ) RenderAffine( bg2 );
            if ( bg3.enabled ) RenderAffine( bg3 );

            blend( );
        }
        private void RenderMode3( )
        {
            var vram = console.vram.h;

            if ( spr.enabled ) RenderSprite( );
            if ( bg2.enabled )
            {
                int dx = bg2.Dx;
                int dy = bg2.Dy;
                int rx = bg2.Rx;
                int ry = bg2.Ry;

                for ( int i = 0; i < 240; i++, rx += dx, ry += dy )
                {
                    if ( ( window[ i ] & 0x04 ) == 0 )
                    {
                        bg2.enable[ i ] = false;
                        continue;
                    }

                    int ax = rx >> 8;
                    int ay = ry >> 8;

                    if ( ax >= 0 && ax < 240 && ay >= 0 && ay < 160 )
                    {
                        bg2.enable[ i ] = true;
                        bg2.raster[ i ] = vram[ ( ay * 240 ) + ax ];
                    }
                    else
                    {
                        bg2.enable[ i ] = false;
                    }
                }
            }

            blend( );
        }
        private void RenderMode4( )
        {
            var pram = this.console.pram.h;
            var vram = this.console.vram.b;

            if ( spr.enabled ) RenderSprite( );
            if ( bg2.enabled )
            {
                int address_base = DisplayFrameSelect ? 0xa000 : 0x0000;
                int dx = bg2.Dx;
                int dy = bg2.Dy;
                int rx = bg2.Rx;
                int ry = bg2.Ry;

                for ( int i = 0; i < 240; i++, rx += dx, ry += dy )
                {
                    if ( ( window[ i ] & 0x04 ) == 0 )
                    {
                        bg2.enable[ i ] = false;
                        continue;
                    }

                    int ax = rx >> 8;
                    int ay = ry >> 8;

                    if ( ax >= 0 && ax < 240 && ay >= 0 && ay < 160 )
                    {
                        int colour = vram[ address_base + ( ay * 240 ) + ax ];

                        if ( colour != 0 )
                        {
                            bg2.enable[ i ] = true;
                            bg2.raster[ i ] = pram[ colour ];
                        }
                        else
                        {
                            bg2.enable[ i ] = false;
                        }
                    }
                }
            }

            blend( );
        }
        private void RenderMode5( )
        {
            var vram = this.console.vram.h;

            if ( spr.enabled ) RenderSprite( );
            if ( bg2.enabled )
            {
                int address_base = DisplayFrameSelect ? 0xa000 : 0x0000;
                int dx = bg2.Dx;
                int dy = bg2.Dy;
                int rx = bg2.Rx;
                int ry = bg2.Ry;

                for ( int i = 0; i < 240; i++, rx += dx, ry += dy )
                {
                    if ( ( window[ i ] & 0x04 ) == 0 )
                    {
                        bg2.enable[ i ] = false;
                        continue;
                    }

                    int ax = rx >> 8;
                    int ay = ry >> 8;

                    if ( ax >= 0 && ax < 160 && ay >= 0 && ay < 128 )
                    {
                        bg2.enable[ i ] = true;
                        bg2.raster[ i ] = vram[ address_base + ( ay * 160 ) + ax ];
                    }
                    else
                    {
                        bg2.enable[ i ] = false;
                    }
                }
            }

            blend( );
        }

        private void RenderAffine( Bg bg )
        {
            var pram = this.console.pram.h;
            var vram = this.console.vram.b;

            int size = 0;
            int mask = 0;
            int exp = 0;

            switch ( bg.Size )
            {
            case 0: mask = ( size = 1 << 0x7 ) - 1; exp = 4; break;
            case 1: mask = ( size = 1 << 0x8 ) - 1; exp = 5; break;
            case 2: mask = ( size = 1 << 0x9 ) - 1; exp = 6; break;
            case 3: mask = ( size = 1 << 0xA ) - 1; exp = 7; break;
            }

            int nmt_base = ( bg.NmtBase << 11 ); // $0000-$F800
            int chr_base = ( bg.ChrBase << 14 ); // $0000-$C000
            int dx = bg.Dx;
            int dy = bg.Dy;
            int rx = bg.Rx;
            int ry = bg.Ry;

            int window_mask = ( 1 << bg.Index );

            for ( int i = 0; i < 240; i++, rx += dx, ry += dy )
            {
                if ( ( window[ i ] & window_mask ) == 0 )
                {
                    bg.enable[ i ] = false;
                    continue;
                }

                int ax = rx >> 8;
                int ay = ry >> 8;

                if ( ( ax >= 0 && ax < size && ay >= 0 && ay < size ) || bg.Wrap )
                {
                    int address = nmt_base + ( ( ( ay & mask ) / 8 ) << exp ) + ( ( ax & mask ) / 8 );
                    int colour = vram[ chr_base + ( vram[ address ] << 0x6 ) + ( ( ay & 7 ) * 8 ) + ( ax & 7 ) ];

                    if ( colour != 0 )
                    {
                        bg.enable[ i ] = true;
                        bg.raster[ i ] = pram[ colour ];
                    }
                    else
                    {
                        bg.enable[ i ] = false;
                    }
                }
            }
        }
        private void RenderLinear( Bg bg )
        {
            var pram16 = this.console.pram.h;
            var vram16 = this.console.vram.h;
            var vram_8 = this.console.vram.b;

            int x_mask = ( bg.Size & 0x01 ) << 8;
            int y_mask = ( bg.Size & 0x02 ) << 7;

            int chr_base = ( bg.ChrBase << 14 );
            int nmt_base = ( bg.NmtBase << 10 );
            int x_scroll = ( bg.Scx );
            int y_scroll = ( bg.Scy + this.vclock );

            int base_addr = nmt_base + ( ( y_scroll & 0xF8 ) << 2 );

            int x_toggle;
            int y_toggle;

            switch ( bg.Size )
            {
            default:
            case 0: x_toggle = 0x000; y_toggle = 0x000; break; // 32x32
            case 1: x_toggle = 0x400; y_toggle = 0x000; break; // 64x32
            case 2: x_toggle = 0x000; y_toggle = 0x400; break; // 32x64
            case 3: x_toggle = 0x400; y_toggle = 0x800; break; // 64x64
            }

            if ( ( y_scroll & y_mask ) != 0 )
                base_addr += y_toggle;

            int window_mask = ( 1 << bg.Index );

            if ( bg.Depth )
            {
                // 8bpp
                for ( int i = 0; i < 240; i++, x_scroll++ )
                {
                    if ( ( window[ i ] & window_mask ) == 0 )
                    {
                        bg.enable[ i ] = false;
                        continue;
                    }

                    int tile_addr = base_addr + ( ( x_scroll & 0xF8 ) >> 3 );

                    if ( ( x_scroll & x_mask ) != 0 )
                        tile_addr += x_toggle;

                    int tile = vram16[ tile_addr ];
                    int x = x_scroll & 7;
                    int y = y_scroll & 7;

                    if ( ( tile & 0x400 ) != 0 ) x ^= 7;
                    if ( ( tile & 0x800 ) != 0 ) y ^= 7;

                    int colour = vram_8[ chr_base + ( ( tile & 0x3FF ) << 6 ) + ( y << 3 ) + ( x >> 0 ) ];

                    if ( colour != 0 )
                    {
                        bg.enable[ i ] = true;
                        bg.raster[ i ] = pram16[ colour ];
                    }
                    else
                    {
                        bg.enable[ i ] = false;
                    }
                }
            }
            else
            {
                // 4bpp
                for ( int i = 0; i < 240; i++, x_scroll++ )
                {
                    if ( ( window[ i ] & window_mask ) == 0 )
                    {
                        bg.enable[ i ] = false;
                        continue;
                    }

                    int tile_addr = base_addr + ( ( x_scroll & 0xF8 ) >> 3 );

                    if ( ( x_scroll & x_mask ) != 0 )
                        tile_addr += x_toggle;

                    int tile = vram16[ tile_addr ];
                    int x = x_scroll & 7;
                    int y = y_scroll & 7;

                    if ( ( tile & 0x400 ) != 0 ) x ^= 7;
                    if ( ( tile & 0x800 ) != 0 ) y ^= 7;

                    int colour = vram_8[ chr_base + ( ( tile & 0x3FF ) << 5 ) + ( y << 2 ) + ( x >> 1 ) ];

                    if ( ( x & 1 ) == 0 )
                        colour &= 15;
                    else
                        colour >>= 4;

                    if ( colour != 0 )
                    {
                        bg.enable[ i ] = true;
                        bg.raster[ i ] = pram16[ ( ( tile >> 8 ) & 0xF0 ) + colour ];
                    }
                    else
                    {
                        bg.enable[ i ] = false;
                    }
                }
            }
        }

        private void RenderSprite( )
        {
            var oram = this.console.oram.h;
            var pram = this.console.pram.h;
            var vram = this.console.vram.b;

            for ( int sprite = 512 - 4; sprite >= 0; sprite -= 4 )
            {
                ushort attr0 = oram[ sprite | 0 ];
                ushort attr1 = oram[ sprite | 1 ];
                ushort attr2 = oram[ sprite | 2 ];

                int priority = ( ( attr2 >> 10 ) & 3 );
                int semitransparent = 0;

                int x = attr1 & 0x1ff;
                int y = attr0 & 0x0ff;

                switch ( ( attr0 >> 10 ) & 3 )
                {
                case 0: break;
                case 1: semitransparent = 0x8000; break; // Semi-transparent
                case 2: continue; // Obj window
                case 3: continue;
                }

                int w = Sp.XSizeLut[ ( attr0 >> 14 ) & 3 ][ ( attr1 >> 14 ) & 3 ],
                    h = Sp.YSizeLut[ ( attr0 >> 14 ) & 3 ][ ( attr1 >> 14 ) & 3 ];

                int rw = w,
                    rh = h;

                switch ( attr0 & 0x300 )
                {
                case 0x000: break;    // Rot-scale off, sprite displayed
                case 0x100: break;    // Rot-scale on, normal size
                case 0x200: continue; // Rot-scale off, sprite hidden
                case 0x300: // Rot-scale on, double size
                    rw *= 2;
                    rh *= 2;
                    break;
                }

                int line = ( this.vclock - y ) & 0xFF;

                if ( line >= rh )
                    continue;

                if ( ( attr0 & 0x100 ) == 0 )
                {
                    if ( ( attr1 & 0x2000 ) != 0 ) line = ( h - 1 ) - line;

                    if ( ( attr0 & 0x2000 ) != 0 )
                    {
                        RenderSprite8bppLinear( attr1, attr2, priority, semitransparent, x, w, line );
                    }
                    else
                    {
                        RenderSprite4bppLinear( attr1, attr2, priority, semitransparent, x, w, line );
                    }
                }
                else
                {
                    int scale = ( attr0 & 0x2000 ) != 0 ? 2 : 1;
                    int param = ( attr1 & 0x3E00 ) >> 5;

                    short dx  = ( short )oram[ param | 0x3 ];
                    short dmx = ( short )oram[ param | 0x7 ];
                    short dy  = ( short )oram[ param | 0xB ];
                    short dmy = ( short )oram[ param | 0xF ];

                    int baseSprite = attr2 & 0x3FF;
                    int pitch;

                    if ( this.spr.mapping )
                    {
                        // 1 dimensional
                        pitch = ( w / 8 ) * scale;
                    }
                    else
                    {
                        // 2 dimensional
                        pitch = 0x20;
                    }

                    int rx = ( dmx * ( line - ( rh / 2 ) ) ) - ( ( rw / 2 ) * dx ) + ( w << 7 );
                    int ry = ( dmy * ( line - ( rh / 2 ) ) ) - ( ( rw / 2 ) * dy ) + ( h << 7 );

                    // Draw a rot/scale sprite
                    if ( ( attr0 & ( 1 << 13 ) ) != 0 )
                    {
                        RenderSprite8bppAffine( attr2, priority, semitransparent, x, w, h, rw, scale, dx, dy, baseSprite, pitch, rx, ry );
                    }
                    else
                    {
                        RenderSprite4bppAffine( attr2, priority, semitransparent, x, w, h, rw, scale, dx, dy, baseSprite, pitch, rx, ry );
                    }
                }
            }
        }
        private void RenderSpriteWindow( )
        {
            var oram = this.console.oram.h;
            var pram = this.console.pram.h;
            var vram = this.console.vram.b;

            for ( uint sprite = 512u - 4u; sprite < 512u; sprite -= 4u )
            {
                ushort attr0 = oram[ sprite | 0u ];
                ushort attr1 = oram[ sprite | 1u ];
                ushort attr2 = oram[ sprite | 2u ];

                int x = attr1 & 0x1FF;
                int y = attr0 & 0xFF;

                switch ( ( attr0 >> 10 ) & 3 )
                {
                case 0: continue;
                case 1: continue; // Semi-transparent
                case 2: break; // Obj window
                case 3: continue;
                }

                int w = Sp.XSizeLut[ ( attr0 >> 14 ) & 3 ][ ( attr1 >> 14 ) & 3 ],
                    h = Sp.YSizeLut[ ( attr0 >> 14 ) & 3 ][ ( attr1 >> 14 ) & 3 ];

                int rw = w,
                    rh = h;

                switch ( attr0 & 0x300 )
                {
                case 0x000: break; // Rot-scale off, sprite displayed
                case 0x100: break; // Rot-scale on, normal size
                case 0x200: // Rot-scale off, sprite hidden
                    continue;

                case 0x300: // Rot-scale on, double size
                    rw *= 2;
                    rh *= 2;
                    break;
                }

                int line = ( this.vclock - y ) & 0xff;

                if ( line >= rh )
                    continue;

                if ( ( attr0 & 0x100 ) == 0 )
                {
                    if ( ( attr1 & 0x2000 ) != 0 ) line = ( h - 1 ) - line;

                    if ( ( attr0 & 0x2000 ) != 0 )
                    {
                        int baseSprite;

                        if ( this.spr.mapping )
                        {
                            // 1 dimensional
                            baseSprite = ( attr2 & 0x3FF ) + ( ( line / 8 ) * ( w / 8 ) ) * 2;
                        }
                        else
                        {
                            // 2 dimensional
                            baseSprite = ( attr2 & 0x3FF ) + ( ( line / 8 ) * 0x20 );
                        }

                        int baseInc = 2;

                        if ( ( attr1 & 0x1000 ) != 0 )
                        {
                            baseSprite += ( ( w / 8 ) - 1 ) * 2;
                            baseInc = -2;
                        }

                        // 256 colors
                        for ( int i = x; i < x + w; i++ )
                        {
                            if ( ( i & 0x1FF ) < 240 )
                            {
                                int tx = ( i - x ) & 7;

                                if ( ( attr1 & 0x1000 ) != 0 )
                                    tx ^= 7;

                                int address = ( baseSprite << 5 ) + ( ( line & 7 ) << 3 ) + ( tx >> 0 );
                                int colour = vram[ 0x10000 + address ];

                                if ( colour != 0 )
                                {
                                    window[ ( i & 0x1ff ) ] = windowObjFlags;
                                }
                            }

                            if ( ( ( i - x ) & 7 ) == 7 ) baseSprite += baseInc;
                        }
                    }
                    else
                    {
                        int baseSprite;

                        if ( this.spr.mapping )
                        {
                            // 1 dimensional
                            baseSprite = ( attr2 & 0x3FF ) + ( ( line / 8 ) * ( w / 8 ) ) * 1;
                        }
                        else
                        {
                            // 2 dimensional
                            baseSprite = ( attr2 & 0x3FF ) + ( ( line / 8 ) * 0x20 );
                        }

                        int baseInc = 1;

                        if ( ( attr1 & 0x1000 ) != 0 )
                        {
                            baseSprite += ( ( w / 8 ) - 1 ) * 1;
                            baseInc = -baseInc;
                        }

                        for ( int i = x; i < x + w; i++ )
                        {
                            if ( ( i & 0x1FF ) < 240 )
                            {
                                int tx = ( i - x ) & 7;

                                if ( ( attr1 & ( 1 << 12 ) ) != 0 )
                                    tx ^= 7;

                                int address = ( baseSprite << 5 ) + ( ( line & 7 ) << 2 ) + ( tx >> 1 );
                                int colour = vram[ 0x10000 + address ];

                                if ( ( tx & 1 ) == 0 )
                                {
                                    colour &= 15;
                                }
                                else
                                {
                                    colour >>= 4;
                                }

                                if ( colour != 0 )
                                {
                                    window[ ( i & 0x1ff ) ] = windowObjFlags;
                                }
                            }

                            if ( ( ( i - x ) & 7 ) == 7 ) baseSprite += baseInc;
                        }
                    }
                }
                else
                {
                    int scale = ( attr0 & 0x2000 ) != 0 ? 2 : 1;
                    int param = ( attr1 & 0x3E00 ) >> 5;

                    short dx = ( short )oram[ param | 0x3 ];
                    short dmx = ( short )oram[ param | 0x7 ];
                    short dy = ( short )oram[ param | 0xB ];
                    short dmy = ( short )oram[ param | 0xF ];

                    int cx = rw / 2;
                    int cy = rh / 2;

                    int baseSprite = attr2 & 0x3FF;
                    int pitch;

                    if ( this.spr.mapping )
                    {
                        // 1 dimensional
                        pitch = ( w / 8 ) * scale;
                    }
                    else
                    {
                        // 2 dimensional
                        pitch = 0x20;
                    }

                    int rx = ( int )( ( dmx * ( line - cy ) ) - ( cx * dx ) + ( w << 7 ) );
                    int ry = ( int )( ( dmy * ( line - cy ) ) - ( cx * dy ) + ( h << 7 ) );

                    // Draw a rot/scale sprite
                    if ( ( attr0 & ( 1 << 13 ) ) != 0 )
                    {
                        // 256 colors
                        for ( int i = x; i < x + rw; i++, rx += dx, ry += dy )
                        {
                            int tx = rx >> 8;
                            int ty = ry >> 8;

                            if ( ( i & 0x1FF ) < 240 && tx >= 0 && tx < w && ty >= 0 && ty < h )
                            {
                                int address = ( baseSprite + ( ( ty / 8 ) * pitch ) + ( ( tx / 8 ) * scale ) ) * 32 + ( ( ty & 7 ) * 8 ) + ( tx & 7 );
                                int colour = vram[ 0x10000 + address ];

                                if ( colour != 0 )
                                {
                                    window[ ( i & 0x1ff ) ] = windowObjFlags;
                                }
                            }
                        }
                    }
                    else
                    {
                        // 16 colors
                        for ( int i = x; i < x + rw; i++, rx += dx, ry += dy )
                        {
                            int tx = rx >> 8;
                            int ty = ry >> 8;

                            if ( ( i & 0x1FF ) < 240 && tx >= 0 && tx < w && ty >= 0 && ty < h )
                            {
                                int address = ( baseSprite + ( ( ty / 8 ) * pitch ) + ( ( tx / 8 ) * scale ) ) * 32 + ( ( ty & 7 ) * 4 ) + ( ( tx & 7 ) / 2 );
                                int colour = vram[ 0x10000 + address ];

                                if ( ( tx & 1 ) == 0 )
                                {
                                    colour &= 15;
                                }
                                else
                                {
                                    colour >>= 4;
                                }

                                if ( colour != 0 )
                                {
                                    window[ ( i & 0x1ff ) ] = windowObjFlags;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void RenderSprite4bppAffine( ushort attr2, int priority, int semitransparent, int x, int w, int h, int rw, int scale, short dx, short dy, int baseSprite, int pitch, int rx, int ry )
        {
            var pram = console.pram.h;
            var vram = console.vram.b;

            int palette = 0x100 + ( ( attr2 >> 8 ) & 0xf0 );

            // 16 colors
            for ( int i = x; i < x + rw; i++ )
            {
                int tx = rx >> 8;
                int ty = ry >> 8;

                if ( ( i & 0x1FF ) < 240 && tx >= 0 && tx < w && ty >= 0 && ty < h )
                {
                    if ( ( window[ i & 0x1FF ] & 0x10 ) == 0 )
                        continue;

                    int address = ( baseSprite + ( ( ty / 8 ) * pitch ) + ( ( tx / 8 ) * scale ) ) * 32 + ( ( ty & 7 ) * 4 ) + ( ( tx & 7 ) / 2 );
                    int colour = vram[ 0x10000 + address ];

                    if ( ( tx & 1 ) == 0 )
                    {
                        colour &= 15;
                    }
                    else
                    {
                        colour >>= 4;
                    }

                    if ( colour != 0 && spr.priority[ i & 0x1ff ] >= priority )
                    {
                        spr.enable[ i & 0x1ff ] = true;
                        spr.raster[ i & 0x1ff ] = pram[ palette + colour ] | semitransparent;
                        spr.priority[ i & 0x1ff ] = priority;
                    }
                }

                rx += dx;
                ry += dy;
            }
        }
        private void RenderSprite8bppAffine( ushort attr2, int priority, int semitransparent, int x, int w, int h, int rw, int scale, short dx, short dy, int baseSprite, int pitch, int rx, int ry )
        {
            var pram = console.pram.h;
            var vram = console.vram.b;

            // 256 colors
            for ( int i = x; i < x + rw; i++ )
            {
                int tx = rx >> 8;
                int ty = ry >> 8;

                if ( ( i & 0x1FF ) < 240 && tx >= 0 && tx < w && ty >= 0 && ty < h )
                {
                    if ( ( window[ i & 0x1FF ] & 0x10 ) == 0 )
                        continue;

                    int address = ( baseSprite + ( ( ty / 8 ) * pitch ) + ( ( tx / 8 ) * scale ) ) * 32 + ( ( ty & 7 ) * 8 ) + ( tx & 7 );
                    int colour = vram[ 0x10000 + address ];

                    if ( colour != 0 && spr.priority[ i & 0x1ff ] >= priority )
                    {
                        spr.enable[ i & 0x1ff ] = true;
                        spr.raster[ i & 0x1ff ] = pram[ 0x100 + colour ] | semitransparent;
                        spr.priority[ i & 0x1ff ] = priority;
                    }
                }

                rx += dx;
                ry += dy;
            }
        }
        private void RenderSprite4bppLinear( ushort attr1, ushort attr2, int priority, int semitransparent, int x, int w, int line )
        {
            var pram = console.pram.h;
            var vram = console.vram.b;

            int baseSprite;

            if ( this.spr.mapping )
            {
                // 1 dimensional
                baseSprite = ( attr2 & 0x3FF ) + ( ( line / 8 ) * ( w / 8 ) ) * 1;
            }
            else
            {
                // 2 dimensional
                baseSprite = ( attr2 & 0x3FF ) + ( ( line / 8 ) * 0x20 );
            }

            int baseInc = 1;

            if ( ( attr1 & 0x1000 ) != 0 )
            {
                baseSprite += ( ( w / 8 ) - 1 ) * 1;
                baseInc = -baseInc;
            }

            // 16 colors
            int palette = 0x100 + ( ( attr2 >> 8 ) & 0xF0 );

            for ( int i = x; i < x + w; i++ )
            {
                if ( ( i & 0x1FF ) < 240 && ( window[ ( i & 0x1FF ) ] & 0x10 ) != 0 )
                {
                    int tx = ( i - x ) & 7;

                    if ( ( attr1 & ( 1 << 12 ) ) != 0 )
                        tx ^= 7;

                    int address = ( baseSprite << 5 ) + ( ( line & 7 ) << 2 ) + ( tx >> 1 );
                    int colour = vram[ 0x10000 + ( address & 0x7fff ) ];

                    if ( ( tx & 1 ) == 0 )
                    {
                        colour &= 15;
                    }
                    else
                    {
                        colour >>= 4;
                    }

                    if ( colour != 0 && spr.priority[ i & 0x1ff ] >= priority )
                    {
                        spr.enable[ i & 0x1ff ] = true;
                        spr.raster[ i & 0x1ff ] = pram[ palette + colour ] | semitransparent;
                        spr.priority[ i & 0x1ff ] = priority;
                    }
                }

                if ( ( ( i - x ) & 7 ) == 7 ) baseSprite += baseInc;
            }
        }
        private void RenderSprite8bppLinear( ushort attr1, ushort attr2, int priority, int semitransparent, int x, int w, int line )
        {
            var pram = console.pram.h;
            var vram = console.vram.b;

            int baseSprite;

            if ( this.spr.mapping )
            {
                // 1 dimensional
                baseSprite = ( attr2 & 0x3FF ) + ( ( line / 8 ) * ( w / 8 ) ) * 2;
            }
            else
            {
                // 2 dimensional
                baseSprite = ( attr2 & 0x3FF ) + ( ( line / 8 ) * 0x20 );
            }

            int baseInc = 2;

            if ( ( attr1 & 0x1000 ) != 0 )
            {
                baseSprite += ( ( w / 8 ) - 1 ) * 2;
                baseInc = -2;
            }

            // 256 colors
            for ( int i = x; i < x + w; i++ )
            {
                if ( ( i & 0x1FF ) < 240 && ( window[ ( i & 0x1FF ) ] & 0x10 ) != 0 )
                {
                    int tx = ( i - x ) & 7;

                    if ( ( attr1 & 0x1000 ) != 0 )
                        tx ^= 7;

                    int address = ( baseSprite << 5 ) + ( ( line & 7 ) << 3 ) + ( tx >> 0 );
                    int colour = vram[ 0x10000 + address ];

                    if ( colour != 0 && spr.priority[ i & 0x1ff ] >= priority )
                    {
                        spr.enable[ i & 0x1ff ] = true;
                        spr.raster[ i & 0x1ff ] = pram[ 0x100 + colour ] | semitransparent;
                        spr.priority[ i & 0x1ff ] = priority;
                    }
                }

                if ( ( ( i - x ) & 7 ) == 7 ) baseSprite += baseInc;
            }
        }
    }
}