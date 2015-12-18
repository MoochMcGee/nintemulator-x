#pragma warning disable 1717

using Nintemulator.Shared;

namespace Nintemulator.GBC.CPU
{
    public partial class Cpu
    {
        // --- 8-bit instructions ---
        private void add( byte data, int carry = 0 )
        {
            var temp = ( registers.a + data ) + carry;
            var bits = GetCarryBits( registers.a, data, temp );

            z = ( temp & 0xFF ) == 0;
            n = false;
            h = ( bits & 0x08 ) != 0;
            c = ( bits & 0x80 ) != 0;

            registers.a = ( byte )temp;
        }
        private void sub( byte data, int carry = 0 )
        {
            var temp = ( registers.a - data ) - carry;
            var bits = GetCarryBits( registers.a, ~data, temp );

            z = ( temp & 0xFF ) == 0;
            n = true;
            h = ( bits & 0x08 ) == 0;
            c = ( bits & 0x80 ) == 0;

            registers.a = ( byte )temp;
        }

        private void and( byte data )
        {
            z = ( registers.a &= data ) == 0;
            n = false;
            h = true;
            c = false;
        }
        private void call( bool flag = true )
        {
            var lo = console.Peek( registers.pc++ );
            var hi = console.Peek( registers.pc++ );

            if ( flag )
            {
                console.Dispatch( );

                console.Poke( --registers.sp, registers.pch ); registers.pch = hi;
                console.Poke( --registers.sp, registers.pcl ); registers.pcl = lo;
            }
        }
        private void ccf( )
        {
            n = false;
            h = false;
            c = !c;
        }
        private void cp( byte data )
        {
            var temp = ( registers.a - data );
            var bits = GetCarryBits( registers.a, ~data, temp );

            z = ( temp & 0xFF ) == 0;
            n = true;
            h = ( bits & 0x08 ) == 0;
            c = ( bits & 0x80 ) == 0;
        }
        private void cpl( )
        {
            registers.a ^= 0xFF;
            n = true;
            h = true;
        }
        private void daa( )
        {
            if ( n )
            {
                if ( c ) registers.a -= 0x60;
                if ( h ) registers.a -= 0x06;
            }
            else
            {
                if ( c || ( registers.a & 0xFF ) > 0x99 ) { registers.a += 0x60; c = true; }
                if ( h || ( registers.a & 0x0F ) > 0x09 ) { registers.a += 0x06; }
            }

            z = registers.a == 0;
            h = false;
        }
        private byte dec( byte data )
        {
            data--;

            z = ( data & 0xFF ) == 0x00;
            n = true;
            h = ( data & 0x0F ) == 0x0F;

            return data;
        }
        private void halt( )
        {
            interrupt.halt = true;

            while ( interrupt.halt ) console.Dispatch( );
        }
        private byte inc( byte data )
        {
            data++;

            z = ( data & 0xFF ) == 0x00;
            n = false;
            h = ( data & 0x0F ) == 0x00;

            return data;
        }
        private void jam( byte code )
        {
            global::System.Windows.Forms.MessageBox.Show( "Invalid instruction $" + code.ToString( "X2" ) );
            registers.pc--;
        }
        private void jp( bool flag = true )
        {
            var lo = console.Peek( registers.pc++ );
            var hi = console.Peek( registers.pc++ );

            if ( flag )
            {
                console.Dispatch( );

                registers.pcl = lo;
                registers.pch = hi;
            }
        }
        private void jr( bool flag = true )
        {
            var data = console.Peek( registers.pc++ );

            if ( flag )
            {
                console.Dispatch( );
                registers.pc = ( ushort )( registers.pc + ( sbyte )data );
            }
        }
        private void or( byte data )
        {
            z = ( registers.a |= data ) == 0;
            n = false;
            h = false;
            c = false;
        }
        private void ret( )
        {
            console.Dispatch( );

            registers.pcl = console.Peek( registers.sp++ );
            registers.pch = console.Peek( registers.sp++ );
        }
        private void ret( bool flag )
        {
            console.Dispatch( );

            if ( flag )
            {
                console.Dispatch( );

                registers.pcl = console.Peek( registers.sp++ );
                registers.pch = console.Peek( registers.sp++ );
            }
        }
        private void reti( )
        {
            registers.pcl = console.Peek( registers.sp++ );
            registers.pch = console.Peek( registers.sp++ );
            interrupt.ff2.i = 1;

            console.Dispatch( );
        }
        private void rla( )
        {
            var flag = ( registers.a & 0x80 ) != 0;

            registers.a = ( byte )( ( registers.a << 1 ) | ( c ? 0x01 : 0x00 ) );

            z = false;
            n = false;
            h = false;
            c = flag;
        }
        private void rlca( )
        {
            c = ( registers.a & 0x80 ) != 0;

            registers.a = ( byte )( ( registers.a << 1 ) | ( registers.a >> 7 ) );

            z = false;
            n = false;
            h = false;
        }
        private void rra( )
        {
            var flag = ( registers.a & 0x01 ) != 0;

            registers.a = ( byte )( ( registers.a >> 1 ) | ( c ? 0x80 : 0x00 ) );

            z = false;
            n = false;
            h = false;
            c = flag;
        }
        private void rrca( )
        {
            c = ( registers.a & 0x01 ) != 0;

            registers.a = ( byte )( ( registers.a >> 1 ) | ( registers.a << 7 ) );

            z = false;
            n = false;
            h = false;
        }
        private void rst( byte data )
        {
            console.Dispatch( );

            console.Poke( --registers.sp, registers.pch );
            console.Poke( --registers.sp, registers.pcl );

            registers.pcl = data;
            registers.pch = 0;
        }
        private void scf( )
        {
            n = false;
            h = false;
            c = true;
        }
        private void stop( )
        {
            if ( swap_speed )
            {
                ClockShift ^= 1;
                swap_speed = false;
                return;
            }

            interrupt.stop = true;

            while ( interrupt.stop ) console.Dispatch( );
        }
        private void xor( byte data )
        {
            z = ( registers.a ^= data ) == 0;
            n = false;
            h = false;
            c = false;
        }

        // --- 16-bit instructions ---
        private void add( ushort data )
        {
            var temp = ( ushort )( registers.hl + data );
            var bits = GetCarryBits( registers.hl, data, temp );

            n = false;
            h = ( bits & 0x0800 ) != 0;
            c = ( bits & 0x8000 ) != 0;

            registers.hl = temp;

            console.Dispatch( );
        }
        private void ld( ref ushort data )
        {
            var l = console.Peek( registers.pc++ );
            var h = console.Peek( registers.pc++ );
            data = ( ushort )( ( h << 8 ) | l );
        }
        private void pop( ref ushort data )
        {
            var l = console.Peek( registers.sp++ );
            var h = console.Peek( registers.sp++ );
            data = ( ushort )( ( h << 8 ) | l );
        }
        private void push( ushort data )
        {
            console.Dispatch( );

            console.Poke( --registers.sp, ( byte )( data >> 8 ) );
            console.Poke( --registers.sp, ( byte )( data >> 0 ) );
        }

        private void StandardCode( )
        {
            switch ( code = console.Peek( registers.pc++ ) )
            {
            case 0x00: break;
            case 0x10: stop( ); break;
            case 0x76: halt( ); break;
            case 0xCB: ExtendedCode( ); break;
            case 0xF3: interrupt.ff1.i = 0; break;
            case 0xFB: interrupt.ff2.i = 1; break;

            case 0x07: rlca( ); break;
            case 0x0F: rrca( ); break;
            case 0x17: rla( ); break;
            case 0x1F: rra( ); break;

            case 0xCD: call( ); break;
            case 0xC4: call( !z ); break;
            case 0xCC: call(  z ); break;
            case 0xD4: call( !c ); break;
            case 0xDC: call(  c ); break;

            case 0xC3: jp( ); break;
            case 0xC2: jp( !z ); break;
            case 0xCA: jp(  z ); break;
            case 0xD2: jp( !c ); break;
            case 0xDA: jp(  c ); break;

            case 0x18: jr( ); break;
            case 0x20: jr( !z ); break;
            case 0x28: jr(  z ); break;
            case 0x30: jr( !c ); break;
            case 0x38: jr(  c ); break;

            case 0xC9: ret( ); break;
            case 0xC0: ret( !z ); break;
            case 0xC8: ret(  z ); break;
            case 0xD0: ret( !c ); break;
            case 0xD8: ret(  c ); break;

            case 0x27: daa( ); break;
            case 0x2F: cpl( ); break;
            case 0x37: scf( ); break;
            case 0x3F: ccf( ); break;

            case 0x01: ld( ref registers.bc ); break;
            case 0x11: ld( ref registers.de ); break;
            case 0x21: ld( ref registers.hl ); break;
            case 0x31: ld( ref registers.sp ); break;

            case 0x02: console.Poke( registers.bc, registers.a ); break;
            case 0x12: console.Poke( registers.de, registers.a ); break;
            case 0x22: console.Poke( registers.hl, registers.a ); registers.hl++; break;
            case 0x32: console.Poke( registers.hl, registers.a ); registers.hl--; break;

            case 0x0A: registers.a = console.Peek( registers.bc ); break;
            case 0x1A: registers.a = console.Peek( registers.de ); break;
            case 0x2A: registers.a = console.Peek( registers.hl ); registers.hl++; break;
            case 0x3A: registers.a = console.Peek( registers.hl ); registers.hl--; break;

            case 0xE0: console.Poke( 0xff00u + console.Peek( registers.pc++ ), registers.a ); break;
            case 0xE2: console.Poke( 0xff00u + registers.c, registers.a ); break;
            case 0xF0: registers.a = console.Peek( 0xff00u + console.Peek( registers.pc++ ) ); break;
            case 0xF2: registers.a = console.Peek( 0xff00u + registers.c ); break;

            case 0xEA: // ld ($nnnn),a
                registers.aal = console.Peek( registers.pc++ );
                registers.aah = console.Peek( registers.pc++ );

                console.Poke( registers.aa, registers.a );
                break;

            case 0xFA: // ld a,($nnnn)
                registers.aal = console.Peek( registers.pc++ );
                registers.aah = console.Peek( registers.pc++ );

                registers.a = console.Peek( registers.aa );
                break;

            case 0x08: // ld ($nnnn),sp
                registers.aal = console.Peek( registers.pc++ );
                registers.aah = console.Peek( registers.pc++ );

                console.Poke( registers.aa++, registers.spl );
                console.Poke( registers.aa++, registers.sph );
                break;

            case 0xD9: reti( ); break;

            case 0xE8:
                {
                    var data = console.Peek( registers.pc++ );
                    var temp = ( ushort )( registers.sp + ( sbyte )data );
                    var bits = GetCarryBits( registers.sp, data, temp );

                    z = false;
                    n = false;
                    h = ( bits & 0x08 ) != 0;
                    c = ( bits & 0x80 ) != 0;

                    registers.sp = temp;

                    console.Dispatch( );
                    console.Dispatch( );
                }
                break;

            case 0xF8:
                {
                    var data = console.Peek( registers.pc++ );
                    var temp = ( ushort )( registers.sp + ( sbyte )data );
                    var bits = GetCarryBits( registers.sp, data, temp );

                    z = false;
                    n = false;
                    h = ( bits & 0x08 ) != 0;
                    c = ( bits & 0x80 ) != 0;

                    registers.hl = temp;

                    console.Dispatch( );
                }
                break;

            case 0xE9: /*                */ registers.pc = registers.hl; break; // ld pc,hl
            case 0xF9: console.Dispatch( ); registers.sp = registers.hl; break; // ld sp,hl

            case 0x03: console.Dispatch( ); registers.bc++; break;
            case 0x13: console.Dispatch( ); registers.de++; break;
            case 0x23: console.Dispatch( ); registers.hl++; break;
            case 0x33: console.Dispatch( ); registers.sp++; break;

            case 0x0B: console.Dispatch( ); registers.bc--; break;
            case 0x1B: console.Dispatch( ); registers.de--; break;
            case 0x2B: console.Dispatch( ); registers.hl--; break;
            case 0x3B: console.Dispatch( ); registers.sp--; break;

            case 0x09: add( registers.bc ); break;
            case 0x19: add( registers.de ); break;
            case 0x29: add( registers.hl ); break;
            case 0x39: add( registers.sp ); break;

            case 0x04: case 0x0C: case 0x14: case 0x1C: case 0x24: case 0x2C: case 0x34: case 0x3C: operand( 3, inc( operand( 3 ) ) ); break;
            case 0x05: case 0x0D: case 0x15: case 0x1D: case 0x25: case 0x2D: case 0x35: case 0x3D: operand( 3, dec( operand( 3 ) ) ); break;
            case 0x06: case 0x0E: case 0x16: case 0x1E: case 0x26: case 0x2E: case 0x36: case 0x3E: operand( 3, console.Peek( registers.pc++ ) ); break;

            case 0x40: case 0x41: case 0x42: case 0x43: case 0x44: case 0x45: case 0x46: case 0x47:
            case 0x48: case 0x49: case 0x4A: case 0x4B: case 0x4C: case 0x4D: case 0x4E: case 0x4F:
            case 0x50: case 0x51: case 0x52: case 0x53: case 0x54: case 0x55: case 0x56: case 0x57:
            case 0x58: case 0x59: case 0x5A: case 0x5B: case 0x5C: case 0x5D: case 0x5E: case 0x5F:
            case 0x60: case 0x61: case 0x62: case 0x63: case 0x64: case 0x65: case 0x66: case 0x67:
            case 0x68: case 0x69: case 0x6A: case 0x6B: case 0x6C: case 0x6D: case 0x6E: case 0x6F:
            case 0x70: case 0x71: case 0x72: case 0x73: case 0x74: case 0x75: /*  76  */ case 0x77:
            case 0x78: case 0x79: case 0x7A: case 0x7B: case 0x7C: case 0x7D: case 0x7E: case 0x7F: operand( 3, operand( 0 ) ); break;

            case 0x80: case 0x81: case 0x82: case 0x83: case 0x84: case 0x85: case 0x86: case 0x87: add( operand( 0 ) ); break;
            case 0x88: case 0x89: case 0x8A: case 0x8B: case 0x8C: case 0x8D: case 0x8E: case 0x8F: add( operand( 0 ), c ? 1 : 0 ); break;
            case 0x90: case 0x91: case 0x92: case 0x93: case 0x94: case 0x95: case 0x96: case 0x97: sub( operand( 0 ) ); break;
            case 0x98: case 0x99: case 0x9A: case 0x9B: case 0x9C: case 0x9D: case 0x9E: case 0x9F: sub( operand( 0 ), c ? 1 : 0 ); break;
            case 0xA0: case 0xA1: case 0xA2: case 0xA3: case 0xA4: case 0xA5: case 0xA6: case 0xA7: and( operand( 0 ) ); break;
            case 0xA8: case 0xA9: case 0xAA: case 0xAB: case 0xAC: case 0xAD: case 0xAE: case 0xAF: xor( operand( 0 ) ); break;
            case 0xB0: case 0xB1: case 0xB2: case 0xB3: case 0xB4: case 0xB5: case 0xB6: case 0xB7:  or( operand( 0 ) ); break;
            case 0xB8: case 0xB9: case 0xBA: case 0xBB: case 0xBC: case 0xBD: case 0xBE: case 0xBF:  cp( operand( 0 ) ); break;

            case 0xC1: pop( ref registers.bc ); /*         */ break;
            case 0xD1: pop( ref registers.de ); /*         */ break;
            case 0xE1: pop( ref registers.hl ); /*         */ break;
            case 0xF1: pop( ref registers.af ); LoadFlags( ); break;

            case 0xC5: /*         */ push( registers.bc ); break;
            case 0xD5: /*         */ push( registers.de ); break;
            case 0xE5: /*         */ push( registers.hl ); break;
            case 0xF5: SaveFlags( ); push( registers.af ); break;

            case 0xC6: add( console.Peek( registers.pc++ ) ); break;
            case 0xCE: add( console.Peek( registers.pc++ ), c ? 1 : 0 ); break;
            case 0xD6: sub( console.Peek( registers.pc++ ) ); break;
            case 0xDE: sub( console.Peek( registers.pc++ ), c ? 1 : 0 ); break;
            case 0xE6: and( console.Peek( registers.pc++ ) ); break;
            case 0xEE: xor( console.Peek( registers.pc++ ) ); break;
            case 0xF6:  or( console.Peek( registers.pc++ ) ); break;
            case 0xFE:  cp( console.Peek( registers.pc++ ) ); break;

            case 0xC7: rst( 0x00 ); break;
            case 0xCF: rst( 0x08 ); break;
            case 0xD7: rst( 0x10 ); break;
            case 0xDF: rst( 0x18 ); break;
            case 0xE7: rst( 0x20 ); break;
            case 0xEF: rst( 0x28 ); break;
            case 0xF7: rst( 0x30 ); break;
            case 0xFF: rst( 0x38 ); break;

            case 0xD3: jam( 0xD3 ); break;
            case 0xDB: jam( 0xDB ); break;
            case 0xDD: jam( 0xDD ); break;
            case 0xE3: jam( 0xE3 ); break;
            case 0xE4: jam( 0xE4 ); break;
            case 0xEB: jam( 0xEB ); break;
            case 0xEC: jam( 0xEC ); break;
            case 0xED: jam( 0xED ); break;
            case 0xF4: jam( 0xF4 ); break;
            case 0xFC: jam( 0xFC ); break;
            case 0xFD: jam( 0xFD ); break;
            }
        }
    }
}
// 666 lines

#pragma warning restore 1717