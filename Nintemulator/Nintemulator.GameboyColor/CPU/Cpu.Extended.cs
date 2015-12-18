namespace Nintemulator.GBC.CPU
{
    public partial class Cpu
    {
        private void bit ( byte value )
        {
            var mask = 1u << ( int )( ( code >> 3 ) & 7u );

            z = ( value & mask ) == 0;
            n = false;
            h = true;
        }
        private byte res ( byte value )
        {
            var mask = 1u << ( int )( ( code >> 3 ) & 7u );

            return ( byte )( value & ~mask );
        }
        private byte set ( byte value )
        {
            var mask = 1u << ( int )( ( code >> 3 ) & 7u );

            return ( byte )( value | mask );
        }
        private byte shl ( byte value, int carry )
        {
            c = ( value & 0x80 ) != 0;

            value = ( byte )( ( value << 1 ) | ( carry << 0 ) );

            z = ( value & 0xff ) == 0;
            n = false;
            h = false;

            return value;
        }
        private byte shr ( byte value, int carry )
        {
            c = ( value & 0x01 ) != 0;

            value = ( byte )( ( value >> 1 ) | ( carry << 7 ) );

            z = ( value & 0xff ) == 0;
            n = false;
            h = false;

            return value;
        }
        private byte swap( byte value )
        {
            value = ( byte )( ( value >> 4 ) | ( value << 4 ) );

            z = ( value & 0xff ) == 0;
            n = false;
            h = false;
            c = false;

            return value;
        }

        private void ExtendedCode( )
        {
            code = console.Peek( registers.pc++ );

            var value = operand( 0 );

            switch ( code >> 3 )
            {
            case 0x00: operand( 0, shl ( value, value >> 7 ) ); break; // rlc
            case 0x01: operand( 0, shr ( value, value & 1  ) ); break; // rrc
            case 0x02: operand( 0, shl ( value, c ? 1 : 0  ) ); break; // rl
            case 0x03: operand( 0, shr ( value, c ? 1 : 0  ) ); break; // rr
            case 0x04: operand( 0, shl ( value, 0          ) ); break; // sla
            case 0x05: operand( 0, shr ( value, value >> 7 ) ); break; // sra
            case 0x06: operand( 0, swap( value             ) ); break; // swap
            case 0x07: operand( 0, shr ( value, 0          ) ); break; // srl
            case 0x08: case 0x09: case 0x0a: case 0x0b: case 0x0c: case 0x0d: case 0x0e: case 0x0f:             bit( value );   break; // bit n
            case 0x10: case 0x11: case 0x12: case 0x13: case 0x14: case 0x15: case 0x16: case 0x17: operand( 0, res( value ) ); break; // res n
            case 0x18: case 0x19: case 0x1a: case 0x1b: case 0x1c: case 0x1d: case 0x1e: case 0x1f: operand( 0, set( value ) ); break; // set n
            }
        }
    }
}