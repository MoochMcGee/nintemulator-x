using Nintemulator.Shared;

namespace Nintemulator.FC.GPU
{
    public partial class Gpu
    {
        private const int BUS_BITS = 14;
        private const int BUS_SIZE = 1 << BUS_BITS;

        private Peek[] peeks = new Peek[ BUS_SIZE ];
        private Poke[] pokes = new Poke[ BUS_SIZE ];

        public byte oam_address;
        public byte oam_address_latch;
        public byte[] oam = new byte[ 256 ];
        public byte[] nta;
        public byte[] ntb;
        public byte[] ntc;
        public byte[] ntd;
        public byte[] pal = new byte[ 32 ];
        public byte[][] nmt = new byte[ 2 ][]
        {
            new byte[ 1024u ],
            new byte[ 1024u ]
        };

        public byte PeekNmtA( uint address ) { return nta[ address & 0x03FF ]; }
        public byte PeekNmtB( uint address ) { return ntb[ address & 0x03FF ]; }
        public byte PeekNmtC( uint address ) { return ntc[ address & 0x03FF ]; }
        public byte PeekNmtD( uint address ) { return ntd[ address & 0x03FF ]; }
        public byte PeekPal0( uint address ) { return pal[ address & 0x000C ]; }
        public byte PeekPalN( uint address ) { return pal[ address & 0x001F ]; }

        public void PokeNmtA( uint address, byte data ) { nta[ address & 0x03ff ] = data; }
        public void PokeNmtB( uint address, byte data ) { ntb[ address & 0x03ff ] = data; }
        public void PokeNmtC( uint address, byte data ) { ntc[ address & 0x03ff ] = data; }
        public void PokeNmtD( uint address, byte data ) { ntd[ address & 0x03ff ] = data; }
        public void PokePal0( uint address, byte data ) { pal[ address & 0x000c ] = data &= 0x3f; }
        public void PokePalN( uint address, byte data ) { pal[ address & 0x001f ] = data &= 0x3f; }

        protected void InitializeMemory( )
        {
            SwitchNametables( Mirroring.ModeVert );

            nta.Initialize<byte>( 0xff );
            ntb.Initialize<byte>( 0xff );
            pal.Initialize<byte>( 0x3f );

            for ( uint i = 0x2000u; i <= 0x3fffu; i += 0x1000u )
            {
                Hook( i + 0x000u, i + 0x3ffu, PeekNmtA, PokeNmtA );
                Hook( i + 0x400u, i + 0x7ffu, PeekNmtB, PokeNmtB );
                Hook( i + 0x800u, i + 0xbffu, PeekNmtC, PokeNmtC );
                Hook( i + 0xc00u, i + 0xfffu, PeekNmtD, PokeNmtD );
            }

            for ( uint i = 0x3f00u; i <= 0x3fffu; i += 0x0004u )
            {
                Hook( i + 0u, PeekPal0, PokePal0 );
                Hook( i + 1u, PeekPalN, PokePalN );
                Hook( i + 2u, PeekPalN, PokePalN );
                Hook( i + 3u, PeekPalN, PokePalN );
            }
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

        public byte PeekByte( uint address )
        {
            return peeks[ address &= 0x3fffu ]( address );
        }
        public void PokeByte( uint address, byte data )
        {
            pokes[ address &= 0x3fffu ]( address, data );
        }

        public void SwitchNametables( int mirroring )
        {
            nta = nmt[ ( mirroring >> 3 ) & 1 ];
            ntb = nmt[ ( mirroring >> 2 ) & 1 ];
            ntc = nmt[ ( mirroring >> 1 ) & 1 ];
            ntd = nmt[ ( mirroring >> 0 ) & 1 ];
        }
    }

    public static class Mirroring
    {
        public const int ModeHorz = 0x3;
        public const int ModeVert = 0x5;
        public const int Mode1ScA = 0x0;
        public const int Mode1ScB = 0xF;
    }
}