using Nintemulator.Shared;
using System;
using System.Runtime.InteropServices;
using word = System.UInt16;

namespace Nintemulator.PKM.CPU
{
    public partial class Cpu : PokemonMini.Processor
    {
        private Flags f;
        private Register16 ba;
        private Register16 hl;
        private Register16 rx;
        private Register16 ry;
        private Register16 sp;
        private Register32 pc;
        private byte code;
        private byte e;
        private byte i; // data index
        private byte n; // immediate hi-byte
        private byte u; // pc index delay
        private byte xi; // x index
        private byte yi; // y index

        private byte a { get { return ba.l; } set { ba.l = value; } }
        private byte b { get { return ba.h; } set { ba.h = value; } }
        private byte l { get { return hl.l; } set { hl.l = value; } }
        private byte h { get { return hl.h; } set { hl.h = value; } }

        public Cpu( PokemonMini console, Timing.System system )
            : base( console, system ) { }

        private byte data_ib( )
        {
            var d = console.Peek( addr_pc( ) ); pc.uw0++;
            return d;
        }
        private word data_iw( )
        {
            var l = console.Peek( addr_pc( ) ); pc.uw0++;
            var h = console.Peek( addr_pc( ) ); pc.uw0++;
            return ( word )( l | ( h << 8 ) );
        }

        private void Vector( uint interrupt )
        {
            console.Poke( --sp.w, pc.ub2 );
            console.Poke( --sp.w, pc.ub1 );
            console.Poke( --sp.w, pc.ub0 );
            console.Poke( --sp.w, f );

            f.i = true;

            pc.ub0 = console.Peek( ( interrupt << 1 ) | 0u );
            pc.ub1 = console.Peek( ( interrupt << 1 ) | 1u );
        }

        private uint addr_pc( ) { return ( pc.uw0 < 0x8000u ) ? pc.uw0 : pc.ud0; }
        private uint addr_hl( ) { return ( uint )( (  i << 16 ) | hl.w ); }
        private uint addr_ib( ) { return ( uint )( (  i << 16 ) | data_ib( ) | ( n << 8 ) ); }
        private uint addr_iw( ) { return ( uint )( (  i << 16 ) | data_iw( ) ); }
        private uint addr_rx( ) { return ( uint )( ( xi << 16 ) | rx.w ); }
        private uint addr_ry( ) { return ( uint )( ( yi << 16 ) | ry.w ); }

        protected override void OnInitialize( )
        {
            base.OnInitialize( );

            Vector( 0 ); // rst
        }

        public override void Update( )
        {
            op_std( );
        }

        private byte alu( )
        {
            switch ( code & 7u )
            {
            default:
            case 0u: return a; // a
            case 1u: return b; // b
            case 2u: return data_ib( ); // $nn
            case 3u: return console.Peek( addr_hl( ) ); // [hl]
            case 4u: return console.Peek( addr_ib( ) ); // [n+#nn]
            case 5u: return console.Peek( addr_iw( ) ); // [#nnnn]
            case 6u: return console.Peek( addr_rx( ) ); // [x]
            case 7u: return console.Peek( addr_ry( ) ); // [y]
            }
        }
        private byte mov( )
        {
            switch ( code & 7u )
            {
            default:
            case 0u: return a; // a
            case 1u: return b; // b
            case 2u: return l; // l
            case 3u: return h; // h
            case 4u: return console.Peek( addr_ib( ) ); // [n+#nn]
            case 5u: return console.Peek( addr_hl( ) ); // [hl]
            case 6u: return console.Peek( addr_rx( ) ); // [x]
            case 7u: return console.Peek( addr_ry( ) ); // [y]
            }
        }

        private void op_std( )
        {
            switch ( code = data_ib( ) )
            {
            default: throw new NotImplementedException( );

            case 0x00: case 0x01: case 0x02: case 0x03: case 0x04: case 0x05: case 0x06: case 0x07: a = Add( a, alu( ) ); break;
            case 0x08: case 0x09: case 0x0a: case 0x0b: case 0x0c: case 0x0d: case 0x0e: case 0x0f: a = Add( a, alu( ), f.c ? 1u : 0u ); break;
            case 0x10: case 0x11: case 0x12: case 0x13: case 0x14: case 0x15: case 0x16: case 0x17: a = Sub( a, alu( ) ); break;
            case 0x18: case 0x19: case 0x1a: case 0x1b: case 0x1c: case 0x1d: case 0x1e: case 0x1f: a = Sub( a, alu( ), f.c ? 1u : 0u ); break;
            case 0x20: case 0x21: case 0x22: case 0x23: case 0x24: case 0x25: case 0x26: case 0x27: a = And( a, alu( ) ); break;
            case 0x28: case 0x29: case 0x2a: case 0x2b: case 0x2c: case 0x2d: case 0x2e: case 0x2f: a = Orr( a, alu( ) ); break;
            case 0x30: case 0x31: case 0x32: case 0x33: case 0x34: case 0x35: case 0x36: case 0x37:     Sub( a, alu( ) ); break;
            case 0x38: case 0x39: case 0x3a: case 0x3b: case 0x3c: case 0x3d: case 0x3e: case 0x3f: a = Xor( a, alu( ) ); break;

            case 0x40: case 0x41: case 0x42: case 0x43: case 0x44: case 0x45: case 0x46: case 0x47: a = mov( ); break;
            case 0x48: case 0x49: case 0x4a: case 0x4b: case 0x4c: case 0x4d: case 0x4e: case 0x4f: b = mov( ); break;
            case 0x50: case 0x51: case 0x52: case 0x53: case 0x54: case 0x55: case 0x56: case 0x57: l = mov( ); break;
            case 0x58: case 0x59: case 0x5a: case 0x5b: case 0x5c: case 0x5d: case 0x5e: case 0x5f: h = mov( ); break;
            case 0x60: case 0x61: case 0x62: case 0x63: case 0x64: case 0x65: case 0x66: case 0x67: console.Poke( addr_rx( ), mov( ) ); break;
            case 0x68: case 0x69: case 0x6a: case 0x6b: case 0x6c: case 0x6d: case 0x6e: case 0x6f: console.Poke( addr_hl( ), mov( ) ); break;
            case 0x70: case 0x71: case 0x72: case 0x73: case 0x74: case 0x75: case 0x76: case 0x77: console.Poke( addr_ry( ), mov( ) ); break;
            case 0x78: case 0x79: case 0x7a: case 0x7b: case 0x7c: case 0x7d: case 0x7e: case 0x7f: console.Poke( addr_ib( ), mov( ) ); break;

            case 0x9a: // dec x
                f.z = ( --rx.w & 0xffffu ) == 0u;
                break;
            case 0x9f: // mov f,#$nn
                f = data_ib( );
                break;
            case 0xa0: // push ba
                console.Poke( --sp.w, ba.h );
                console.Poke( --sp.w, ba.l );
                break;
            case 0xa2: // push x
                console.Poke( --sp.w, rx.h );
                console.Poke( --sp.w, rx.l );
                break;
            case 0xa8: // pop ba
                ba.l = console.Peek( sp.w++ );
                ba.h = console.Peek( sp.w++ );
                break;
            case 0xaa: // pop x
                rx.l = console.Peek( sp.w++ );
                rx.h = console.Peek( sp.w++ );
                break;
            case 0xb1: // mov b,#$nn
                b = data_ib( );
                break;
            case 0xb3: // mov h,#$nn
                h = data_ib( );
                break;
            case 0xb4: // mov n,#$nn
                n = data_ib( );
                break;
            case 0xbe: // mov $nnnn,x
                {
                    var addr = addr_iw( );
                    console.Poke( addr + 0u, rx.l );
                    console.Poke( addr + 1u, rx.l );
                }
                break;
            case 0xce: // extension op $ce
                op_ext_ce( );
                break;
            case 0xcf: // extension op $cf
                op_ext_cf( );
                break;
            case 0xc5: // mov hl,$nnnn
                hl.w = data_iw( );
                break;
            case 0xc6: // mov x,$nnnn
                rx.w = data_iw( );
                break;

            case 0xd8: // and $nn,#$nn
                {
                    var addr = addr_ib( );
                    console.Poke( addr, ( byte )( console.Peek( addr ) & data_ib( ) ) );
                }
                break;
            case 0xd9: // or $nn,#$nn
                {
                    var addr = addr_ib( );
                    console.Poke( addr, ( byte )( console.Peek( addr ) | data_ib( ) ) );
                }
                break;
            case 0xdd: console.Poke( addr_ib( ), data_ib( ) ); break; // mov $nn,#$nn
            case 0xe7: // jnzb #$nn
                {
                    var data = data_ib( );

                    if ( !f.z )
                    {
                        pc.uw0 += ( ushort )( sbyte )data;
                        pc.uw0--;
                    }
                }
                break;
            case 0xf1: // jmpb #$nn
                {
                    var offset = data_ib( ); pc.uw0--;

                    pc.uw0 += offset;
                    pc.ub2 = u;
                }
                break;
            case 0xf2: // callw #$nnnn
                {
                    var offset = data_iw( );

                    console.Poke( --sp.w, pc.ub2 );
                    console.Poke( --sp.w, pc.ub1 );
                    console.Poke( --sp.w, pc.ub0 ); pc.uw0--;

                    pc.uw0 += offset;
                    pc.ub2 = u;
                }
                break;

            case 0xf5:
                {
                    // todo: add cycle?
                    f.z = ( --b & 0xff ) == 0;

                    var offset = data_iw( );

                    if ( !f.z )
                    {
                        pc.uw0--;
                        pc.uw0 += ( ushort )( sbyte )offset;
                    }
                }
                break;

            case 0xf8: // ret
                pc.ub0 = console.Peek( sp.w++ );
                pc.ub1 = console.Peek( sp.w++ );
                pc.ub2 = console.Peek( sp.w++ );
                break;

            case 0xff: break; // nop
            }
        }
        private void op_ext_ce( )
        {
            switch ( code = data_ib( ) )
            {
            default: throw new NotImplementedException( );

            case 0xbc: Sub( b, data_ib( ) ); break; // cmp b,#$nn
            case 0xbd: Sub( l, data_ib( ) ); break; // cmp l,#$nn
            case 0xbe: Sub( h, data_ib( ) ); break; // cmp h,#$nn
            case 0xbf: Sub( n, data_ib( ) ); break; // cmp n,#$nn

            case 0xc4: // mov u,#$nn
                u = data_ib( );
                break;
            case 0xc5: // mov i,#$nn
                i = data_ib( );
                break;
            case 0xcd: // mov i,a
                i = a;
                break;
            case 0xce: // mov xi,a
                xi = a;
                break;
            case 0xcf: // mov yi,a
                yi = a;
                break;
            case 0xd0: // mov a,$nnnn
                a = console.Peek( addr_iw( ) );
                break;
            case 0xd4: // mov $nnnn,a
                console.Poke( addr_iw( ), a );
                break;
            }
        }
        private void op_ext_cf( )
        {
            switch ( code = data_ib( ) )
            {
            default: throw new NotImplementedException( );
            case 0x6e: // mov sp,#$nnnn
                sp.w = data_iw( );
                break;
            case 0xe4: // mov hl,ba
                hl.w = ba.w;
                break;
            case 0xe8: // mov x,ba
                rx.w = ba.w;
                break;
            case 0xec: // mov y,ba
                ry.w = ba.w;
                break;
            }
        }

        public struct Flags
        {
            public bool b;
            public bool i;
            public bool m;
            public bool d;
            public bool n;
            public bool v;
            public bool c;
            public bool z;

            public static implicit operator byte( Flags value )
            {
                return ( byte )(
                    ( value.b ? 0x80 : 0x00 ) |
                    ( value.i ? 0x40 : 0x00 ) |
                    ( value.m ? 0x20 : 0x00 ) |
                    ( value.d ? 0x10 : 0x00 ) |
                    ( value.n ? 0x08 : 0x00 ) |
                    ( value.v ? 0x04 : 0x00 ) |
                    ( value.c ? 0x02 : 0x00 ) |
                    ( value.z ? 0x01 : 0x00 ) );
            }
            public static implicit operator Flags( byte value )
            {
                Flags flags;
                flags.b = ( value & 0x80 ) != 0;
                flags.i = ( value & 0x40 ) != 0;
                flags.m = ( value & 0x20 ) != 0;
                flags.d = ( value & 0x10 ) != 0;
                flags.n = ( value & 0x08 ) != 0;
                flags.v = ( value & 0x04 ) != 0;
                flags.c = ( value & 0x02 ) != 0;
                flags.z = ( value & 0x01 ) != 0;
                return flags;
            }
        }
    }
}