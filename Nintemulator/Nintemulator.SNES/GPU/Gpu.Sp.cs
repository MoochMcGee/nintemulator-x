using Nintemulator.Shared;

namespace Nintemulator.SFC.GPU
{
    public partial class Gpu
    {
        internal Sp spr;

        public sealed class Sp : Gpu.Layer
        {
            public static Register16[][] size_lut =
            {
                new Register16[] { new Register16 { l =  8, h =  8 }, new Register16 { l = 16, h = 16 } }, // 000 =  8x8  and 16x16 sprites
                new Register16[] { new Register16 { l =  8, h =  8 }, new Register16 { l = 32, h = 32 } }, // 001 =  8x8  and 32x32 sprites
                new Register16[] { new Register16 { l =  8, h =  8 }, new Register16 { l = 64, h = 64 } }, // 010 =  8x8  and 64x64 sprites
                new Register16[] { new Register16 { l = 16, h = 16 }, new Register16 { l = 32, h = 32 } }, // 011 = 16x16 and 32x32 sprites
                new Register16[] { new Register16 { l = 16, h = 16 }, new Register16 { l = 64, h = 64 } }, // 100 = 16x16 and 64x64 sprites
                new Register16[] { new Register16 { l = 32, h = 32 }, new Register16 { l = 64, h = 64 } }, // 101 = 32x32 and 64x64 sprites
                new Register16[] { new Register16 { l = 16, h = 32 }, new Register16 { l = 32, h = 64 } }, // 110 = 16x32 and 32x64 sprites
                new Register16[] { new Register16 { l = 16, h = 32 }, new Register16 { l = 32, h = 32 } }, // 111 = 16x32 and 32x32 sprites
            };

            public Register16[] size = size_lut[ 0 ];
            public bool interlace;
            public uint addr;
            public uint name;

            public Sp( SuperFamicom console )
                : base( console, 4 ) { }

            public void Render( )
            {
                int t = 0;
                int r = 0;

                for ( int i = 127; i >= 0; i-- )
                {
                    uint exta = ( uint )gpu.oram.h[ ( i >> 3 ) | 0x100U ] >> ( ( i & 7 ) << 1 );
                    uint xpos = ( uint )gpu.oram.b[ ( i << 2 ) | 0x000u ];
                    uint ypos = ( uint )gpu.oram.b[ ( i << 2 ) | 0x001u ] + 1u;
                    uint tile = ( uint )gpu.oram.b[ ( i << 2 ) | 0x002u ];
                    uint attr = ( uint )gpu.oram.b[ ( i << 2 ) | 0x003u ];

                    uint line = ( gpu.vclock - ypos );

                    xpos |= ( exta & 1U ) << 8;

                    uint x_size = size[ ( exta & 2U ) >> 1 ].l;
                    uint y_size = size[ ( exta & 2U ) >> 1 ].h;

                    if ( interlace ) line <<= 1;

                    if ( line >= y_size )
                        continue;

                    r++;

                    var palette = 0x80U + ( ( attr & 0x0EU ) << 3 );

                    if ( ( attr & 0x80U ) != 0U )
                    {
                        line ^= ( y_size - 1U );
                    }

                    var char_address = addr + ( tile << 4 ) + ( ( line & 0x38U ) << 5 ) + ( line & 7U );
                    var char_step = 0x00000010U;

                    if ( ( attr & 0x40U ) != 0U )
                    {
                        char_address += ( x_size - 8 ) << 1;
                        char_step = 0xFFFFFFF0U;
                    }

                    if ( ( attr & 0x01U ) != 0U )
                    {
                        char_address = ( char_address + name ) & 0x7FFFU;
                    }

                    var priority = this.priorities[ ( attr >> 4 ) & 3u ];

                    for ( int column = 0; column < x_size; column += 8 )
                    {
                        t++;

                        var bit0 = gpu.vram[ char_address + 0u ].l;
                        var bit1 = gpu.vram[ char_address + 0u ].h;
                        var bit2 = gpu.vram[ char_address + 8u ].l;
                        var bit3 = gpu.vram[ char_address + 8u ].h;

                        if ( ( attr & 0x40U ) == 0U )
                        {
                            bit0 = ReverseLookup[ bit0 ];
                            bit1 = ReverseLookup[ bit1 ];
                            bit2 = ReverseLookup[ bit2 ];
                            bit3 = ReverseLookup[ bit3 ];
                        }

                        for ( int x = 0; x < 8; x++, xpos = ( xpos + 1 ) & 0x1ff )
                        {
                            var colour = ( ( bit0 & 1U ) << 0 ) | ( ( bit1 & 1U ) << 1 ) | ( ( bit2 & 1U ) << 2 ) | ( ( bit3 & 1U ) << 3 );

                            bit0 >>= 1;
                            bit1 >>= 1;
                            bit2 >>= 1;
                            bit3 >>= 1;

                            if ( colour != 0 && xpos < 256 )
                            {
                                this.enable[ xpos ] = true;
                                this.raster[ xpos ] = palette + colour;
                                this.priority[ xpos ] = priority;
                            }
                        }

                        char_address = ( char_address + char_step ) & 0x7FFFU;
                    }
                }

                if ( t > 34 ) gpu.ppu1_stat |= 0x80;
                if ( r > 32 ) gpu.ppu1_stat |= 0x40;
            }
        }
    }
}