using Nintemulator.Shared;

namespace Nintemulator.SFC.GPU
{
    public partial class Gpu
    {
        public abstract class Layer : SuperFamicom.Component
        {
            private uint w1_reg;
            private uint w2_reg;

            public bool wn_dirty;
            public bool wm;
            public bool ws;
            public uint sm;
            public uint ss;
            public uint w1;
            public uint w2;
            public uint w1_inv;
            public uint w2_inv;
            public uint wn_log;
            public bool[] enable = new bool[ 256 ];
            public uint[] raster = new uint[ 256 ];
            public uint[] window = new uint[ 256 ];
            public uint[] priority = new uint[ 256 ];
            public uint[] priorities;

            public Layer( SuperFamicom console, int priorities )
                : base( console )
            {
                this.priorities = new uint[ priorities ];
                this.window.Initialize<uint>( 0xffffffffu );
            }

            public virtual uint GetColorM( uint index, uint priority )
            {
                var color = raster[ index ] & sm;

                if ( wm ) color &= window[ index ];

                return color;
            }
            public virtual uint GetColorS( uint index, uint priority )
            {
                var color = raster[ index ] & ss;

                if ( ws ) color &= window[ index ];

                return color;
            }

            public void PokeWindow1( byte value )
            {
                if ( w1_reg != ( value & 15u ) )
                {
                    w1_reg   =  ( ( value & 0xfu ) >> 0 );
                    w1_inv   =  ( ( value & 0x1u ) >> 0 );
                    w1       = ~( ( value & 0x2u ) >> 1 ) + 1u;
                    w2_inv   =  ( ( value & 0x4u ) >> 2 );
                    w2       = ~( ( value & 0x8u ) >> 3 ) + 1u;
                    wn_dirty = true;
                }
            }
            public void PokeWindow2( byte value )
            {
                if ( w2_reg != ( value & 3u ) )
                {
                    w2_reg   =  ( ( value & 0x3u ) >> 0 );
                    wn_log   =  ( ( value & 0x3u ) >> 0 );
                    wn_dirty = true;
                }
            }

            public void UpdateWindow( )
            {
                var w1_buffer = gpu.w1.mask_buffer;
                var w2_buffer = gpu.w2.mask_buffer;

                for ( int i = 0; i < 256; i++ )
                {
                    var w1_mask = ( w1_buffer[ i ] ^ w1_inv ) & w1;
                    var w2_mask = ( w2_buffer[ i ] ^ w2_inv ) & w2;

                    switch ( wn_log & w1 & w2 )
                    {
                    case 0: window[ i ] = ( ( w1_mask | w2_mask ) ^ 0u ) - 1u; break; // or
                    case 1: window[ i ] = ( ( w1_mask & w2_mask ) ^ 0u ) - 1u; break; // and
                    case 2: window[ i ] = ( ( w1_mask ^ w2_mask ) ^ 0u ) - 1u; break; // xor
                    case 3: window[ i ] = ( ( w1_mask ^ w2_mask ) ^ 1u ) - 1u; break; // xnor
                    }
                }
            }
        }
    }
}