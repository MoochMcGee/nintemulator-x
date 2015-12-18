using Nintemulator.Shared;
using System.IO;

namespace Nintemulator.PKM
{
    public partial class PokemonMini
    {
        // Start    End      Size  Description
        // $000000  $000FFF   4KB  Internal BIOS
        // $001000  $001FFF   4KB  PM RAM
        // $002000  $0020FF  256B  Hardware Registers
        // $002100  $1FFFFF  ~2MB  Cartridge Memory

        private Peek[] peek;
        private Poke[] poke;
        private byte open;
        private byte[] bios;
        private byte[] cart;
        private byte[] wram;
        private uint cart_mask;

        private byte PeekBios( uint address )
        {
            return open = bios[ address & 0xfffu ];
        }
        private byte PeekCart( uint address )
        {
            return open = cart[ address & cart_mask ];
        }
        private byte PeekOpen( uint address )
        {
#if DEBUG
            System.Windows.Forms.MessageBox.Show( "Unmapped read: $" + address.ToString( "x4" ) );
#endif
            return open;
        }
        private byte PeekWram( uint address )
        {
            return open = wram[ address & 0xfffu ];
        }
        private void PokeOpen( uint address, byte data )
        {
#if DEBUG
            System.Windows.Forms.MessageBox.Show( "Unmapped write: $" + address.ToString( "x4" ) + ", $" + data.ToString( "x2" ) );
#endif
            open = data;
        }
        private void PokeWram( uint address, byte data )
        {
            wram[ address & 0xfffu ] = open = data;
        }

        protected void InitializeMemory( string filename )
        {
            peek = new Peek[ 1u << 21 ];
            poke = new Poke[ 1u << 21 ];
            bios = File.ReadAllBytes( "pkm/boot.rom" );
            cart = File.ReadAllBytes( filename );
            wram = new byte[ 4096u ];

            cart_mask = MathHelper.NextPowerOfTwo( ( uint )( cart.Length ) ) - 1u;

            Hook( 0x000000u, 0x000fffu, PeekBios, PokeOpen );
            Hook( 0x001000u, 0x001fffu, PeekWram, PokeWram );
            Hook( 0x002000u, 0x0020ffu, PeekOpen, PokeOpen );
            Hook( 0x002100u, 0x1fffffu, PeekCart, PokeOpen );
        }

        public void Hook( uint address, Peek peek )
        {
            this.peek[ address & 0x1fffffu ] = peek;
        }
        public void Hook( uint address, Poke poke )
        {
            this.poke[ address & 0x1fffffu ] = poke;
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
                Hook( address, peek, poke );
        }

        public byte Peek( uint address )
        {
            cpu.Cycles += 4;

            if ( address < 0x200000u )
            {
                return peek[ address ]( address );
            }
            else
            {
                return PeekCart( address );
            }
        }
        public void Poke( uint address, byte data )
        {
            cpu.Cycles += 4;

            if ( address < 0x200000u )
            {
                poke[ address ]( address, data );
            }
            else
            {
                PokeOpen( address, data );
            }
        }
    }
}