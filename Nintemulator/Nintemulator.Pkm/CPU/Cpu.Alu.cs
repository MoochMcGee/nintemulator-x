using System;

namespace Nintemulator.PKM.CPU
{
    public partial class Cpu
    {
        // 8-bit

        private byte Dec( byte a )
        {
            f.z = ( --a ) == 0;
            return a;
        }
        private byte Inc( byte a )
        {
            f.z = ( ++a ) == 0;
            return a;
        }
        private byte Shl( byte a, int c = 0 )
        {
            f.c = ( a & 0x80 ) != 0;

            a = ( byte )( ( a << 1 ) | c );

            f.n = ( a & 0x80 ) != 0;
            f.z = ( a & 0xff ) == 0;
            return a;
        }
        private byte Shr( byte a, int c = 0 )
        {
            f.c = ( a & 0x01 ) != 0;

            a = ( byte )( ( a >> 1 ) | ( c << 7 ) );

            f.n = ( a & 0x80 ) != 0;
            f.z = ( a & 0xff ) == 0;
            return a;
        }

        private byte Neg( byte a ) { throw new NotImplementedException( ); }
        private byte Not( byte a ) { throw new NotImplementedException( ); }

        private byte And( byte a, byte b )
        {
            a &= b;
            f.n = ( a & 0x80 ) != 0;
            f.z = ( a & 0xff ) == 0;
            return a;
        }
        private byte Orr( byte a, byte b )
        {
            a |= b;
            f.n = ( a & 0x80 ) != 0;
            f.z = ( a & 0xff ) == 0;
            return a;
        }
        private byte Xor( byte a, byte b )
        {
            a ^= b;
            f.n = ( a & 0x80 ) != 0;
            f.z = ( a & 0xff ) == 0;
            return a;
        }
        private byte Add( byte a, byte b, uint c = 0u )
        {
            var sign = f.m ? 0x8 : 0x80;
            var mask = f.m ? 0xf : 0xff;
            var r = ( uint )( a + b ) + c;

            if ( f.d )
            {
                f.n = false;
                f.v = false;
                f.c = false;

                if ( ( r & 0x0f ) > 0x09 ) { r += 0x06; f.c |=  f.m; }
                if ( ( r & 0xf0 ) > 0x90 ) { r += 0x60; f.c |= !f.m; }

                f.z = ( r & mask ) == 0;
            }
            else
            {
                var v = ( a ^ r ) & ~( a ^ b );

                f.n = ( ( r ) & sign ) != 0;
                f.z = ( ( r ) & mask ) == 0;
                f.v = ( ( v ) & sign ) != 0;
                f.c = ( ( v ^ a ^ b ^ r ) & sign ) != 0;
            }

            return ( byte )( r & mask );
        }
        private byte Sub( byte a, byte b, uint c = 1u )
        {
            b ^= 0xff;
            return Add( a, b, c );
        }

        // 16-bit

        private ushort Dec( ushort a )
        {
            f.z = ( --a ) == 0;
            return a;
        }
        private ushort Inc( ushort a )
        {
            f.z = ( ++a ) == 0;
            return a;
        }
        private ushort Add( ushort a, ushort b, uint c = 0u ) { throw new NotImplementedException( ); }
        private ushort Sub( ushort a, ushort b, uint c = 0u ) { throw new NotImplementedException( ); }
    }
}