namespace Nintemulator.FC.CPU
{
    public partial class Cpu
    {
        private byte mov( byte data )
        {
            p.n.i = ( data >> 7 );
            p.z.b = ( data == 0 );
            return data;
        }
        private void adc( byte data )
        {
            var temp = ( byte )( ( a + data ) + p.c.i );
            var bits = ( byte )( ( a ^ temp ) & ~( a ^ data ) );

            p.v.i = ( bits ) >> 7;
            p.c.i = ( bits ^ a ^ data ^ temp ) >> 7;

            a = mov( temp );
        }
        private void sbc( byte data )
        {
            adc( data ^= 0xff );
        }
        private byte cmp( byte left, byte data )
        {
            var temp = ( left - data );

            p.c.i = ( ~temp >> 8 ) & 1;

            return mov( ( byte )( temp ) );
        }

        private void and( byte data ) { mov( a &= data ); }
        private void eor( byte data ) { mov( a ^= data ); }
        private void ora( byte data ) { mov( a |= data ); }
        private byte shl( byte data, int carry )
        {
            p.c.i = ( data >> 7 );

            data = ( byte )( ( data << 1 ) | ( carry << 0 ) );

            return mov( data );
        }
        private byte shr( byte data, int carry )
        {
            p.c.i = ( data & 1 );

            data = ( byte )( ( data >> 1 ) | ( carry << 7 ) );

            return mov( data );
        }

        public static class alu
        {
            public static int c;
            public static int v;

            public static byte add( byte a, byte b, int carry = 0 )
            {
                var temp = ( byte )( ( a + b ) + carry );
                var bits = ( byte )( ( a ^ temp ) & ~( a ^ b ) );

                c = ( bits ^ a ^ b ^ temp ) >> 7;
                v = ( bits ) >> 7;

                return temp;
            }
            public static byte sub( byte a, byte b, int carry = 1 )
            {
                b ^= 0xff;
                return add( a, b, carry );
            }
        }
    }
}