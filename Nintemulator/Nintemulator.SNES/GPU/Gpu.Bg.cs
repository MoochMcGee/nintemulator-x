using Nintemulator.Shared;

namespace Nintemulator.SFC.GPU
{
    public partial class Gpu
    {
        public Bg bg0;
        public Bg bg1;
        public Bg bg2;
        public Bg bg3;

        public sealed class Bg : Gpu.Layer
        {
            public const int BPP2 = 2;
            public const int BPP4 = 4;
            public const int BPP8 = 8;

            public static bool Priority;
            public static uint OffsetLatch;
            public static uint Mode;
            public static uint MosaicSize;

            public static byte M7Control;
            public static byte M7Latch;
            public static Register16 M7A;
            public static Register16 M7B;
            public static Register16 M7C;
            public static Register16 M7D;
            public static Register16 M7X;
            public static Register16 M7Y;
            public static Register16 M7HOffset;
            public static Register16 M7VOffset;

            public uint h_offset;
            public uint v_offset;

            public bool mosaic;

            public uint name_base;
            public uint name_size;
            public uint char_base;
            public uint char_size;

            public uint index;

            public Bg( SuperFamicom console, uint index )
                : base( console, 2 )
            {
                this.index = index;
            }

            public void WriteHOffset( byte data )
            {
                h_offset = ( ( data & 0xFFU ) << 8 ) | ( OffsetLatch & ~7U ) | ( ( h_offset >> 8 ) & 7U );
                OffsetLatch = data;
            }
            public void WriteVOffset( byte data )
            {
                v_offset = ( ( data & 0xFFU ) << 8 ) | OffsetLatch;
                OffsetLatch = data;
            }

            public void Render( int depth )
            {
                //if (char_size == 16U)
                //    RenderLarge(depth);
                //else
                RenderSmall( depth );
            }
            public void RenderLarge( int depth ) { }
            public void RenderSmall( int depth )
            {
                var x_line = ( h_offset ) & 7u;
                var x_tile = ( h_offset ) >> 3;
                var y_line = ( v_offset + gpu.vclock ) & 7u;
                var y_tile = ( v_offset + gpu.vclock ) >> 3;
                int x_move;
                int y_move;

                int fine = ( int )( h_offset & 7u );
                int offset = 0;

                switch ( name_size )
                {
                default:
                case 0U: { y_move = 31; x_move = 31; } break;
                case 1U: { y_move = 31; x_move =  5; } break;
                case 2U: { y_move =  5; x_move = 31; } break;
                case 3U: { y_move =  6; x_move =  5; } break;
                }

                var bits = new byte[ depth ];

                for ( uint i = 0; i < 33; i++, x_tile++ )
                {
                    var name_address = name_base +
                        ( ( y_tile & 31U ) << 5 ) +
                        ( ( x_tile & 31U ) << 0 ) +
                        ( ( y_tile & 32U ) << y_move ) +
                        ( ( x_tile & 32U ) << x_move );

                    var name = gpu.vram[ name_address & 0x7fffu ].w;
                    var char_address = char_base + ( ( name & 0x3ffu ) * depth * 4 ) + y_line;

                    var priority = priorities[ ( name >> 13 ) & 1u ];

                    if ( ( name & 0x8000u ) != 0U )
                    {
                        char_address ^= 0x0007u; // vflip
                    }

                    if ( ( name & 0x4000u ) == 0U ) // hflip
                    {
                        for ( uint j = 0; j < depth; j += 2 )
                        {
                            Register16 data = gpu.vram[ ( char_address & 0x7fffu ) | ( j << 2 ) ];
                            bits[ j | 0u ] = ReverseLookup[ data.l ];
                            bits[ j | 1u ] = ReverseLookup[ data.h ];
                        }
                    }
                    else
                    {
                        for ( uint j = 0; j < depth; j += 2 )
                        {
                            Register16 data = gpu.vram[ ( char_address & 0x7fffu ) | ( j << 2 ) ];
                            bits[ j | 0u ] = data.l;
                            bits[ j | 1u ] = data.h;
                        }
                    }

                    var palette = ( ( name & 0x1c00u ) >> 10 ) << depth;

                    switch ( Mode )
                    {
                    case 0U: palette += index << 5; break;
                    case 3U:
                    case 4U: palette *= index; break;
                    }

                    for ( int k = 0; k < depth; k++ )
                    {
                        bits[ k ] >>= fine;
                    }

                    for ( int j = fine; j < 8 && offset < 256; j++, offset++ )
                    {
                        var colour = 0u;

                        for ( int k = 0; k < depth; k++ )
                        {
                            colour |= ( bits[ k ] & 1u ) << k;
                            bits[ k ] >>= 1;
                        }

                        if ( colour != 0 )
                        {
                            this.enable[ offset ] = true;
                            this.raster[ offset ] = palette + colour;
                            this.priority[ offset ] = priority;
                        }
                        else
                        {
                            this.enable[ offset ] = false;
                        }
                    }

                    fine = 0;
                }
            }

            public void RenderAffine( )
            {
                int a = ( short )M7A.w;
                int b = ( short )M7B.w;
                int c = ( short )M7C.w;
                int d = ( short )M7D.w;

                var cx = ( ( M7X.w & 0x1FFF ) ^ 0x1000 ) - 0x1000;
                var cy = ( ( M7Y.w & 0x1FFF ) ^ 0x1000 ) - 0x1000;

                var hoffs = ( ( M7HOffset.w & 0x1FFF ) ^ 0x1000 ) - 0x1000;
                var voffs = ( ( M7VOffset.w & 0x1FFF ) ^ 0x1000 ) - 0x1000;

                var h = hoffs - cx;
                var v = voffs - cy;
                var y = ( int )gpu.vclock;

                h = ( h & 0x2000 ) != 0 ? ( h | ~0x3FF ) : ( h & 0x3FF );
                v = ( v & 0x2000 ) != 0 ? ( v | ~0x3FF ) : ( v & 0x3FF );

                if ( ( M7Control & 0x02 ) != 0 ) y ^= 255;

                int tx = ( ( a * h ) & ~63 ) + ( ( b * v ) & ~63 ) + ( ( b * y ) & ~63 ) + ( cx << 8 );
                int ty = ( ( c * h ) & ~63 ) + ( ( d * v ) & ~63 ) + ( ( d * y ) & ~63 ) + ( cy << 8 );
                int dx = a;
                int dy = c;

                if ( ( M7Control & 0x01 ) != 0 )
                {
                    tx += ( dx * 255 );
                    ty += ( dy * 255 );
                    dx = -dx;
                    dy = -dy;
                }

                for ( int x = 0; x < 256; x++, tx += dx, ty += dy )
                {
                    var rx = ( tx >> 8 ) & 0x3FF;
                    var ry = ( ty >> 8 ) & 0x3FF;

                    var tile = gpu.vram[ ( ( ry & ~7 ) << 4 ) + ( rx >> 3 ) ].l; // ..yy yyyy yxxx xxxx
                    var data = gpu.vram[ ( tile << 6 ) + ( ( ry & 7 ) << 3 ) + ( rx & 7 ) ].h; // ..dd dddd ddyy yxxx

                    enable[ x ] = true;
                    raster[ x ] = data;
                    priority[ x ] = priorities[ 0 ];
                }
            }
        }
    }
}