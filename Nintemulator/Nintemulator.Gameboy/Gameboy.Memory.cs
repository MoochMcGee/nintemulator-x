using System;
using System.IO;
using Nintemulator.Shared;

namespace Nintemulator.GB
{
    public partial class Gameboy
    {
        private Peek[] peeks = new Peek[ 1 << 16 ];
        private Poke[] pokes = new Poke[ 1 << 16 ];
        private byte[] bios;
        private byte[] hram = new byte[ 0x007F ];
        private byte[] wram = new byte[ 0x2000 ];

        private byte PeekBios( uint address ) { return bios[ address & 0x00FFU ]; }
        private byte PeekHRam( uint address ) { return hram[ address & 0x007FU ]; }
        private byte PeekWRam( uint address ) { return wram[ address & 0x1FFFU ]; }
        private void PokeHRam( uint address, byte data ) { hram[ address & 0x007FU ] = data; }
        private void PokeWRam( uint address, byte data ) { wram[ address & 0x1FFFU ] = data; }

        public void InitializeMemory( Model model )
        {
            bios = File.ReadAllBytes( "gb/boot.rom" );

            Hook( 0x0000, 0x00FF, PeekBios );
            Hook( 0xC000, 0xFDFF, PeekWRam, PokeWRam );
            Hook( 0xFF80, 0xFFFE, PeekHRam, PokeHRam );
        }

        public void Hook( uint address, Peek peek ) { peeks[ address ] = peek; }
        public void Hook( uint address, Poke poke ) { pokes[ address ] = poke; }
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

            ppu.Update( cpu.Single );
            apu.Update( cpu.Single );
            tma.Update( );
        }

        public byte PeekByte( uint address )
        {
            Dispatch( );

            return peeks[ address ]( address );
        }
        public void PokeByte( uint address, byte data )
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