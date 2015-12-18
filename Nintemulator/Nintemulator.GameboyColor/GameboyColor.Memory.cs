using Nintemulator.Shared;
using System;
using System.IO;

namespace Nintemulator.GBC
{
    public partial class GameboyColor
    {
        private Peek[] peeks = new Peek[ 1 << 16 ];
        private Poke[] pokes = new Poke[ 1 << 16 ];
        private byte[] boot;
        private byte[] hram = new byte[ 0x007f ];
        private byte[] wram0;
        private byte[] wramX;
        private byte[][] wram = Utility.CreateArray< byte >( 8, 4096 );
        private uint wramPage;

        private byte PeekOpen( uint address )
        {
#if DEBUG
            global::System.Windows.Forms.MessageBox.Show( "Unmapped read: $" + address.ToString( "X4" ) );
#endif
            return 0xff;
        }
        private byte PeekBoot( uint address )
        {
            return boot[ address ];
        }
        private byte PeekHRam( uint address )
        {
            return hram[ address & 0x7fu ];
        }
        private byte PeekWRam( uint address )
        {
            switch ( ( address >> 12 ) & 1u )
            {
            default: return wram0[ address & 0xfffu ];
            case 1u: return wramX[ address & 0xfffu ];
            }
        }
        private void PokeOpen( uint address, byte data )
        {
#if DEBUG
            global::System.Windows.Forms.MessageBox.Show( "Unmapped write: $" + address.ToString( "X4" ) + ", $" + data.ToString( "X2" ) );
#endif
        }
        private void PokeHRam( uint address, byte data )
        {
            hram[ address & 0x7fu ] = data;
        }
        private void PokeWRam( uint address, byte data )
        {
            switch ( ( address >> 12 ) & 1u )
            {
            default:
            case 0u: wram0[ address & 0xfffu ] = data; break;
            case 1u: wramX[ address & 0xfffu ] = data; break;
            }
        }

        private byte PeekFF70( uint address )
        {
            return ( byte )( 0xf8u | wramPage );
        }
        private void PokeFF70( uint address, byte data )
        {
            wramPage = ( data & 7u );

            wramX = wram[ ( wramPage == 0u ) ? 1u : wramPage ];
        }

        public void InitializeMemory( )
        {
            boot = File.ReadAllBytes( "gbc/boot.rom" );
            wram0 = wram[ 0u ];
            wramX = wram[ 1u ];
            wramPage = 1u;

            Hook( 0x0000u, 0x00ffu, PeekBoot );
            Hook( 0x0200u, 0x08ffu, PeekBoot );
            Hook( 0xc000u, 0xfdffu, PeekWRam, PokeWRam );
            Hook( 0xff80u, 0xfffeu, PeekHRam, PokeHRam );

            // Registers
            Hook( 0xff70u, PeekFF70, PokeFF70 );
        }

        public void Hook( uint address, Peek peek )
        {
            peeks[ address ] = peek;
        }
        public void Hook( uint address, Poke poke )
        {
            pokes[ address ] = poke;
        }
        public void Hook( uint address, Peek peek, Poke poke )
        {
            Hook( address, peek );
            Hook( address, poke );
        }
        public void Hook( uint address, uint last, Peek peek )
        {
            for ( ; address <= last; address++ )
                Hook( address, peek );
        }
        public void Hook( uint address, uint last, Poke poke )
        {
            for ( ; address <= last; address++ )
                Hook( address, poke );
        }
        public void Hook( uint address, uint last, Peek peek, Poke poke )
        {
            for ( ; address <= last; address++ )
            {
                Hook( address, peek );
                Hook( address, poke );
            }
        }

        public void Dispatch( )
        {
            if ( cpu.interrupt.ff2.b )
            {
                cpu.interrupt.ff2.i = 0;
                cpu.interrupt.ff1.i = 1;
            }

            ppu.Update( cpu.Single >> cpu.ClockShift );
            apu.Update( cpu.Single >> cpu.ClockShift );
            tma.Update( );
        }

        public byte Peek( uint address )
        {
            Dispatch( );

            return peeks[ address ]( address );
        }
        public void Poke( uint address, byte data )
        {
            Dispatch( );

            pokes[ address ]( address, data );
        }

        public byte PeekByteFree( uint address )
        {
            return peeks[ address ]( address );
        }
        public void PokeByteFree( uint address, byte data )
        {
            pokes[ address ]( address, data );
        }
    }
}