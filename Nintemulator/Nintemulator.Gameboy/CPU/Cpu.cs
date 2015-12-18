using Nintemulator.Shared;
using System.Runtime.InteropServices;

namespace Nintemulator.GB.CPU
{
    public class Cpu : Gameboy.Processor
    {
        internal Status sr;
        internal Interrupt interrupt;
        internal Registers registers;
        internal bool halt;
        internal bool stop;
        internal uint code;

        public Cpu( Gameboy gameboy, Timing.System system )
            : base( gameboy, system )
        {
            Single = system.Cpu;
        }

        private int CarryBits( int a, int b, int r )
        {
            return ( a & b ) | ( ( a ^ b ) & ~r );
        }

        private bool Flag( )
        {
            switch ( ( code >> 3 ) & 3u )
            {
            default: return !sr.z.b;
            case 1u: return  sr.z.b;
            case 2u: return !sr.c.b;
            case 3u: return  sr.c.b;
            }
        }
        private byte Operand( uint code )
        {
            switch ( code & 7u )
            {
            default: return registers.b;
            case 1u: return registers.c;
            case 2u: return registers.d;
            case 3u: return registers.e;
            case 4u: return registers.h;
            case 5u: return registers.l;
            case 6u: return console.PeekByte( registers.hl );
            case 7u: return registers.a;
            }
        }
        private void Operand( uint code, byte data )
        {
            switch ( code & 7u )
            {
            default: registers.b = data; break;
            case 1u: registers.c = data; break;
            case 2u: registers.d = data; break;
            case 3u: registers.e = data; break;
            case 4u: registers.h = data; break;
            case 5u: registers.l = data; break;
            case 6u: console.PokeByte( registers.hl, data ); break;
            case 7u: registers.a = data; break;
            }
        }
        private void ExtCode( )
        {
            var op = Operand( code = console.PeekByte( registers.pc++ ) );

            switch ( code >> 3 )
            {
            case 0x00: Operand( code,  Shl( op, op >> 7 ) ); break; // rlc
            case 0x01: Operand( code,  Shr( op, op & 1  ) ); break; // rrc
            case 0x02: Operand( code,  Shl( op, sr.c.i  ) ); break; // rl
            case 0x03: Operand( code,  Shr( op, sr.c.i  ) ); break; // rr
            case 0x04: Operand( code,  Shl( op          ) ); break; // sla
            case 0x05: Operand( code,  Shr( op, op >> 7 ) ); break; // sra
            case 0x06: Operand( code, Swap( op          ) ); break; // swap
            case 0x07: Operand( code,  Shr( op          ) ); break; // srl
            case 0x08: case 0x09: case 0x0a: case 0x0b: case 0x0c: case 0x0d: case 0x0e: case 0x0f:                Bit( op );   break; // bit
            case 0x10: case 0x11: case 0x12: case 0x13: case 0x14: case 0x15: case 0x16: case 0x17: Operand( code, Res( op ) ); break; // res
            case 0x18: case 0x19: case 0x1a: case 0x1b: case 0x1c: case 0x1d: case 0x1e: case 0x1f: Operand( code, Set( op ) ); break; // set
            }
        }
        private void StdCode( )
        {
            switch ( code = console.PeekByte( registers.pc++ ) )
            {
            case 0x00: break;
            case 0x10: /*stop = true;
                while ( stop ) console.Dispatch( );*/ break; // todo: find out why gb cpu test rom uses this without joypad interrupts enabled.
            case 0x76:   halt = true;
                while ( halt ) console.Dispatch( );   break;
            case 0xcb: ExtCode( ); break;
            case 0xf3: interrupt.ff1.i = 0; break;
            case 0xfb: interrupt.ff1.i = 1; break;

            case 0x07: Rol( registers.a >> 7 ); break;
            case 0x0f: Ror( registers.a & 1 ); break;
            case 0x17: Rol( sr.c.i ); break;
            case 0x1f: Ror( sr.c.i ); break;
            case 0x27: Daa( ); break;
            case 0x2f: Cpl( ); break;
            case 0x37: Scf( ); break;
            case 0x3f: Ccf( ); break;

            case 0xcd: case 0xc4: case 0xcc: case 0xd4: case 0xdc: Call( ); break;
            case 0xc9: case 0xc0: case 0xc8: case 0xd0: case 0xd8:  Ret( ); break;
            case 0xc3: case 0xc2: case 0xca: case 0xd2: case 0xda:   Jp( ); break;
            case 0x18: case 0x20: case 0x28: case 0x30: case 0x38:   Jr( ); break;

            case 0x01: Ld( ref registers.bc ); break;
            case 0x11: Ld( ref registers.de ); break;
            case 0x21: Ld( ref registers.hl ); break;
            case 0x31: Ld( ref registers.sp ); break;

            case 0x02: console.PokeByte( registers.bc, registers.a ); break;
            case 0x12: console.PokeByte( registers.de, registers.a ); break;
            case 0x22: console.PokeByte( registers.hl, registers.a ); registers.hl++; break;
            case 0x32: console.PokeByte( registers.hl, registers.a ); registers.hl--; break;

            case 0x0a: registers.a = console.PeekByte( registers.bc ); break;
            case 0x1a: registers.a = console.PeekByte( registers.de ); break;
            case 0x2a: registers.a = console.PeekByte( registers.hl ); registers.hl++; break;
            case 0x3a: registers.a = console.PeekByte( registers.hl ); registers.hl--; break;

            case 0xe0: console.PokeByte( 0xff00u + console.PeekByte( registers.pc++ ), registers.a ); break;
            case 0xe2: console.PokeByte( 0xff00u + registers.c, registers.a ); break;
            case 0xf0: registers.a = console.PeekByte( 0xff00u + console.PeekByte( registers.pc++ ) ); break;
            case 0xf2: registers.a = console.PeekByte( 0xff00u + registers.c ); break;

            case 0xea: // ld ($nnnn),a
                registers.aal = console.PeekByte( registers.pc++ );
                registers.aah = console.PeekByte( registers.pc++ );

                console.PokeByte( registers.aa, registers.a );
                break;

            case 0xfa: // ld a,($nnnn)
                registers.aal = console.PeekByte( registers.pc++ );
                registers.aah = console.PeekByte( registers.pc++ );

                registers.a = console.PeekByte( registers.aa );
                break;

            case 0x08: // ld ($nnnn),sp
                registers.aal = console.PeekByte( registers.pc++ );
                registers.aah = console.PeekByte( registers.pc++ );

                console.PokeByte( registers.aa, registers.spl ); registers.aa++;
                console.PokeByte( registers.aa, registers.sph );
                break;

            case 0xd9: Reti( ); break;

            case 0xe8: // add sp,sp,#$nn
                {
                    var data = console.PeekByte( registers.pc++ );
                    var temp = ( ushort )( registers.sp + ( sbyte )data );
                    var bits = CarryBits( registers.sp, data, temp );

                    sr.z.i = 0;
                    sr.n.i = 0;
                    sr.h.i = ( bits >> 3 ) & 1;
                    sr.c.i = ( bits >> 7 ) & 1;

                    registers.sp = temp;

                    console.Dispatch( );
                    console.Dispatch( );
                }
                break;

            case 0xf8: // add hl,sp,#$nn
                {
                    var data = console.PeekByte( registers.pc++ );
                    var temp = ( ushort )( registers.sp + ( sbyte )data );
                    var bits = CarryBits( registers.sp, data, temp );

                    sr.z.i = 0;
                    sr.n.i = 0;
                    sr.h.i = ( bits >> 3 ) & 1;
                    sr.c.i = ( bits >> 7 ) & 1;

                    registers.hl = temp;

                    console.Dispatch( );
                }
                break;

            case 0xe9: /*                */ registers.pc = registers.hl; break; // ld pc,hl
            case 0xf9: console.Dispatch( ); registers.sp = registers.hl; break; // ld sp,hl

            case 0x03: console.Dispatch( ); registers.bc++; break;
            case 0x13: console.Dispatch( ); registers.de++; break;
            case 0x23: console.Dispatch( ); registers.hl++; break;
            case 0x33: console.Dispatch( ); registers.sp++; break;

            case 0x0b: console.Dispatch( ); registers.bc--; break;
            case 0x1b: console.Dispatch( ); registers.de--; break;
            case 0x2b: console.Dispatch( ); registers.hl--; break;
            case 0x3b: console.Dispatch( ); registers.sp--; break;

            case 0x09: Add( ref registers.bc ); break;
            case 0x19: Add( ref registers.de ); break;
            case 0x29: Add( ref registers.hl ); break;
            case 0x39: Add( ref registers.sp ); break;

            case 0x04: case 0x0c: case 0x14: case 0x1c: case 0x24: case 0x2c: case 0x34: case 0x3c: Operand( code >> 3, Inc( Operand( code >> 3 ) ) ); break;
            case 0x05: case 0x0d: case 0x15: case 0x1d: case 0x25: case 0x2d: case 0x35: case 0x3d: Operand( code >> 3, Dec( Operand( code >> 3 ) ) ); break;
            case 0x06: case 0x0e: case 0x16: case 0x1e: case 0x26: case 0x2e: case 0x36: case 0x3e: Operand( code >> 3, console.PeekByte( registers.pc++ ) ); break;

            case 0x40: case 0x41: case 0x42: case 0x43: case 0x44: case 0x45: case 0x46: case 0x47:  Ld( ); break;
            case 0x48: case 0x49: case 0x4a: case 0x4b: case 0x4c: case 0x4d: case 0x4e: case 0x4f:  Ld( ); break;
            case 0x50: case 0x51: case 0x52: case 0x53: case 0x54: case 0x55: case 0x56: case 0x57:  Ld( ); break;
            case 0x58: case 0x59: case 0x5a: case 0x5b: case 0x5c: case 0x5d: case 0x5e: case 0x5f:  Ld( ); break;
            case 0x60: case 0x61: case 0x62: case 0x63: case 0x64: case 0x65: case 0x66: case 0x67:  Ld( ); break;
            case 0x68: case 0x69: case 0x6a: case 0x6b: case 0x6c: case 0x6d: case 0x6e: case 0x6f:  Ld( ); break;
            case 0x70: case 0x71: case 0x72: case 0x73: case 0x74: case 0x75: /*  76  */ case 0x77:  Ld( ); break;
            case 0x78: case 0x79: case 0x7a: case 0x7b: case 0x7c: case 0x7d: case 0x7e: case 0x7f:  Ld( ); break;
            case 0x80: case 0x81: case 0x82: case 0x83: case 0x84: case 0x85: case 0x86: case 0x87: Add( Operand( code ) ); break;
            case 0x88: case 0x89: case 0x8a: case 0x8b: case 0x8c: case 0x8d: case 0x8e: case 0x8f: Add( Operand( code ), sr.c.i ); break;
            case 0x90: case 0x91: case 0x92: case 0x93: case 0x94: case 0x95: case 0x96: case 0x97: Sub( Operand( code ) ); break;
            case 0x98: case 0x99: case 0x9a: case 0x9b: case 0x9c: case 0x9d: case 0x9e: case 0x9f: Sub( Operand( code ), sr.c.i ); break;
            case 0xa0: case 0xa1: case 0xa2: case 0xa3: case 0xa4: case 0xa5: case 0xa6: case 0xa7: And( Operand( code ) ); break;
            case 0xa8: case 0xa9: case 0xaa: case 0xab: case 0xac: case 0xad: case 0xae: case 0xaf: Xor( Operand( code ) ); break;
            case 0xb0: case 0xb1: case 0xb2: case 0xb3: case 0xb4: case 0xb5: case 0xb6: case 0xb7:  Or( Operand( code ) ); break;
            case 0xb8: case 0xb9: case 0xba: case 0xbb: case 0xbc: case 0xbd: case 0xbe: case 0xbf:  Cp( Operand( code ) ); break;
            case 0xc7: Rst( 0x00 ); break;
            case 0xcf: Rst( 0x08 ); break;
            case 0xd7: Rst( 0x10 ); break;
            case 0xdf: Rst( 0x18 ); break;
            case 0xe7: Rst( 0x20 ); break;
            case 0xef: Rst( 0x28 ); break;
            case 0xf7: Rst( 0x30 ); break;
            case 0xff: Rst( 0x38 ); break;

            case 0xc1: Pop( ref registers.bc ); /*                   */ break;
            case 0xd1: Pop( ref registers.de ); /*                   */ break;
            case 0xe1: Pop( ref registers.hl ); /*                   */ break;
            case 0xf1: Pop( ref registers.af ); sr.Load( registers.f ); break;

            case 0xc5: /*                     */ Push( ref registers.bc ); break;
            case 0xd5: /*                     */ Push( ref registers.de ); break;
            case 0xe5: /*                     */ Push( ref registers.hl ); break;
            case 0xf5: registers.f = sr.Save( ); Push( ref registers.af ); break;

            case 0xc6: Add( console.PeekByte( registers.pc++ ) ); break;
            case 0xce: Add( console.PeekByte( registers.pc++ ), sr.c.i ); break;
            case 0xd6: Sub( console.PeekByte( registers.pc++ ) ); break;
            case 0xde: Sub( console.PeekByte( registers.pc++ ), sr.c.i ); break;
            case 0xe6: And( console.PeekByte( registers.pc++ ) ); break;
            case 0xee: Xor( console.PeekByte( registers.pc++ ) ); break;
            case 0xf6:  Or( console.PeekByte( registers.pc++ ) ); break;
            case 0xfe:  Cp( console.PeekByte( registers.pc++ ) ); break;
                
            case 0xd3: case 0xdb: case 0xdd: case 0xe3: case 0xe4: case 0xeb: case 0xec: case 0xed: case 0xf4: case 0xfc: case 0xfd: Jam( ); break;
            }
        }

        #region Extended Opcodes

        private void  Bit( byte data )
        {
            int shift = ( int )( ( code >> 3 ) & 7 );

            sr.z.b = ( data & ( 1 << shift ) ) == 0;
            sr.n.i = 0;
            sr.h.i = 1;
        }
        private byte  Shl( byte data, int carry = 0 )
        {
            sr.c.i = ( data >> 7 );

            data = ( byte )( ( data << 1 ) | ( carry ) );

            sr.z.b = ( data & 0xff ) == 0;
            sr.n.i = 0;
            sr.h.i = 0;

            return ( data );
        }
        private byte  Shr( byte data, int carry = 0 )
        {
            sr.c.i = ( data & 0x01 );

            data = ( byte )( ( data >> 1 ) | ( carry << 7 ) );

            sr.z.b = ( data & 0xff ) == 0;
            sr.n.i = 0;
            sr.h.i = 0;

            return ( data );
        }
        private byte  Res( byte data )
        {
            var mask = ( 1 << ( int )( ( code >> 3 ) & 7 ) );

            return data &= ( byte )( mask ^ 0xff );
        }
        private byte  Set( byte data )
        {
            var mask = ( 1 << ( int )( ( code >> 3 ) & 7 ) );

            return data |= ( byte )( mask );
        }
        private byte Swap( byte data )
        {
            data = ( byte )( ( data >> 4 ) | ( data << 4 ) );

            sr.z.b = data == 0;
            sr.n.i = 0;
            sr.h.i = 0;
            sr.c.i = 0;

            return data;
        }

        #endregion
        #region Standard Opcodes

        // -- 8-bit instructions --
        private void Call( )
        {
            var lo = console.PeekByte( registers.pc++ );
            var hi = console.PeekByte( registers.pc++ );

            if ( code == 0xcd || Flag( ) )
            {
                console.Dispatch( );

                console.PokeByte( --registers.sp, registers.pch );
                console.PokeByte( --registers.sp, registers.pcl );

                registers.pcl = lo;
                registers.pch = hi;
            }
        }
        private void  Ccf( )
        {
            sr.n.i = 0;
            sr.h.i = 0;
            sr.c.i ^= 1;
        }
        private void  Cpl( )
        {
            registers.a ^= 0xff;
            sr.n.i = 1;
            sr.h.i = 1;
        }
        private void  Daa( )
        {
            if ( sr.n.b )
            {
                if ( sr.c.b ) registers.a -= 0x60;
                if ( sr.h.b ) registers.a -= 0x06;
            }
            else
            {
                if ( sr.c.b || ( registers.a & 0xff ) > 0x99 ) { registers.a += 0x60; sr.c.i = 1; }
                if ( sr.h.b || ( registers.a & 0x0f ) > 0x09 ) { registers.a += 0x06; }
            }

            sr.z.b = registers.a == 0;
            sr.h.i = 0;
        }
        private void  Jam( )
        {
            global::System.Windows.Forms.MessageBox.Show( "Invalid instruction $" + code.ToString( "X2" ) );
            registers.pc--;
        }
        private void   Jp( )
        {
            var lo = console.PeekByte( registers.pc++ );
            var hi = console.PeekByte( registers.pc++ );

            if ( code == 0xc3 || Flag( ) )
            {
                console.Dispatch( );

                registers.pcl = lo;
                registers.pch = hi;
            }
        }
        private void   Jr( )
        {
            var data = console.PeekByte( registers.pc++ );

            if ( code == 0x18 || Flag( ) )
            {
                console.Dispatch( );
                registers.pc = ( ushort )( registers.pc + ( sbyte )data );
            }
        }
        private void   Ld( )
        {
            Operand( code >> 3, Operand( code ) );
        }
        private void  Ret( )
        {
            console.Dispatch( );

            if ( code == 0xc9 || Flag( ) )
            {
                if ( code != 0xc9 ) console.Dispatch( );
                registers.pcl = console.PeekByte( registers.sp++ );
                registers.pch = console.PeekByte( registers.sp++ );
            }
        }
        private void Reti( )
        {
            registers.pcl = console.PeekByte( registers.sp++ );
            registers.pch = console.PeekByte( registers.sp++ );

            console.Dispatch( );

            interrupt.ff1.i = 1;
        }
        private void  Scf( )
        {
            sr.n.i = 0;
            sr.h.i = 0;
            sr.c.i = 1;
        }
        private void  And( byte data )
        {
            sr.z.b = ( registers.a &= data ) == 0;
            sr.n.i = 0;
            sr.h.i = 1;
            sr.c.i = 0;
        }
        private void   Cp( byte data )
        {
            var temp = ( registers.a + ~data + 1 );
            var bits = ~CarryBits( registers.a, ~data, temp );

            sr.z.b = ( temp & 0xff ) == 0;
            sr.n.i = 1;
            sr.h.i = ( bits >> 3 ) & 1;
            sr.c.i = ( bits >> 7 ) & 1;
        }
        private byte  Dec( byte data )
        {
            sr.z.b = ( --data & 0xff ) == 0x00;
            sr.n.i = 1;
            sr.h.b = ( data & 0x0f ) == 0x0f;

            return data;
        }
        private byte  Inc( byte data )
        {
            sr.z.b = ( ++data & 0xff ) == 0x00;
            sr.n.i = 0;
            sr.h.b = ( data & 0x0f ) == 0x00;

            return data;
        }
        private void   Or( byte data )
        {
            sr.z.b = ( registers.a |= data ) == 0;
            sr.n.i = 0;
            sr.h.i = 0;
            sr.c.i = 0;
        }
        private void  Rol( int carry )
        {
            sr.z.i = 0;
            sr.n.i = 0;
            sr.h.i = 0;
            sr.c.i = ( registers.a >> 7 );

            registers.a = ( byte )( ( registers.a << 1 ) | ( carry ) );
        }
        private void  Ror( int carry )
        {
            sr.z.i = 0;
            sr.n.i = 0;
            sr.h.i = 0;
            sr.c.i = ( registers.a & 0x01 );

            registers.a = ( byte )( ( registers.a >> 1 ) | ( carry << 7 ) );
        }
        private void  Rst( byte addr )
        {
            console.Dispatch( );

            console.PokeByte( --registers.sp, registers.pch );
            console.PokeByte( --registers.sp, registers.pcl );

            registers.pcl = addr;
            registers.pch = 0;
        }
        private void  Xor( byte data )
        {
            sr.z.b = ( registers.a ^= data ) == 0;
            sr.n.i = 0;
            sr.h.i = 0;
            sr.c.i = 0;
        }
        private void  Add( byte data, int carry = 0 )
        {
            var temp = ( registers.a + data ) + carry;
            var bits = CarryBits( registers.a, data, temp );

            sr.z.b = ( temp & 0xff ) == 0;
            sr.n.i = 0;
            sr.h.i = ( bits >> 3 ) & 1;
            sr.c.i = ( bits >> 7 ) & 1;

            registers.a = ( byte )temp;
        }
        private void  Sub( byte data, int carry = 0 )
        {
            var temp = ( registers.a - data ) - carry;
            var bits = ~CarryBits( registers.a, ~data, temp );

            sr.z.b = ( temp & 0xff ) == 0;
            sr.n.i = 1;
            sr.h.i = ( bits >> 3 ) & 1;
            sr.c.i = ( bits >> 7 ) & 1;

            registers.a = ( byte )temp;
        }

        // -- 16-bit instructions --
        private void  Add( ref ushort data )
        {
            var temp = ( ushort )( registers.hl + data );
            var bits = CarryBits( registers.hl, data, temp );

            sr.n.i = 0;
            sr.h.i = ( bits >> 11 ) & 1;
            sr.c.i = ( bits >> 15 ) & 1;

            registers.hl = temp;

            console.Dispatch( );
        }
        private void   Ld( ref ushort data )
        {
            var l = console.PeekByte( registers.pc++ );
            var h = console.PeekByte( registers.pc++ );
            data = ( ushort )( ( h << 8 ) | l );
        }
        private void  Pop( ref ushort data )
        {
            var l = console.PeekByte( registers.sp++ );
            var h = console.PeekByte( registers.sp++ );
            data = ( ushort )( ( h << 8 ) | l );
        }
        private void Push( ref ushort data )
        {
            console.Dispatch( );

            console.PokeByte( --registers.sp, ( byte )( data >> 8 ) );
            console.PokeByte( --registers.sp, ( byte )( data >> 0 ) );
        }

        #endregion
        
        protected override void OnInitialize( )
        {
            base.OnInitialize( );

            console.Hook( 0xff0f, ( a ) => interrupt.rf, ( a, d ) => interrupt.rf = d );
            console.Hook( 0xffff, ( a ) => interrupt.ef, ( a, d ) => interrupt.ef = d );
        }

        public override void Update( )
        {
            StdCode( );

            var flags = ( interrupt.rf & interrupt.ef ) & -interrupt.ff1.i;

            if ( flags != 0 )
            {
                console.Dispatch( );
                interrupt.ff1.i = 0;

                if ( ( flags & 0x01 ) != 0 ) { interrupt.rf ^= 0x01; Rst( 0x40 ); return; }
                if ( ( flags & 0x02 ) != 0 ) { interrupt.rf ^= 0x02; Rst( 0x48 ); return; }
                if ( ( flags & 0x04 ) != 0 ) { interrupt.rf ^= 0x04; Rst( 0x50 ); return; }
                if ( ( flags & 0x08 ) != 0 ) { interrupt.rf ^= 0x08; Rst( 0x58 ); return; }
                if ( ( flags & 0x10 ) != 0 ) { interrupt.rf ^= 0x10; Rst( 0x60 ); return; }
            }
        }

        public void RequestInterrupt( byte flag )
        {
            interrupt.rf |= flag;

            if ( ( interrupt.ef & flag ) != 0 )
            {
                halt = false;

                if ( flag == Interrupt.Joypad )
                    stop = false;
            }
        }

        public struct Status
        {
            public Flag z;
            public Flag n;
            public Flag h;
            public Flag c;

            public void Load( byte value )
            {
                z.i = ( value >> 7 ) & 1;
                n.i = ( value >> 6 ) & 1;
                h.i = ( value >> 5 ) & 1;
                c.i = ( value >> 4 ) & 1;
            }
            public byte Save( )
            {
                return ( byte )(
                    ( z.i << 7 ) |
                    ( n.i << 6 ) |
                    ( h.i << 5 ) |
                    ( c.i << 4 ) );
            }
        }
        public struct Interrupt
        {
            public const byte VBlank = ( 1 << 0 );
            public const byte Status = ( 1 << 1 );
            public const byte Elapse = ( 1 << 2 );
            public const byte Serial = ( 1 << 3 );
            public const byte Joypad = ( 1 << 4 );

            public Flag ff1;
            public Flag ff2;
            public byte ef;
            public byte rf;
        }
        [StructLayout( LayoutKind.Explicit )]
        public struct Registers
        {
            [FieldOffset(  1 )] public byte b;
            [FieldOffset(  0 )] public byte c;
            [FieldOffset(  3 )] public byte d;
            [FieldOffset(  2 )] public byte e;
            [FieldOffset(  5 )] public byte h;
            [FieldOffset(  4 )] public byte l;
            [FieldOffset(  7 )] public byte a;
            [FieldOffset(  6 )] public byte f;
            //
            [FieldOffset(  8 )] public byte spl;
            [FieldOffset(  9 )] public byte sph;
            [FieldOffset( 10 )] public byte pcl;
            [FieldOffset( 11 )] public byte pch;
            [FieldOffset( 12 )] public byte aal;
            [FieldOffset( 13 )] public byte aah;

            [FieldOffset(  0 )] public ushort bc;
            [FieldOffset(  2 )] public ushort de;
            [FieldOffset(  4 )] public ushort hl;
            [FieldOffset(  6 )] public ushort af;
            [FieldOffset(  8 )] public ushort sp;
            [FieldOffset( 10 )] public ushort pc;
            [FieldOffset( 12 )] public ushort aa;
        }
    }
}