using Nintemulator.Shared;

namespace Nintemulator.SFC.CPU
{
    public partial class Cpu
    {
        public Dma dma0;
        public Dma dma1;
        public Dma dma2;
        public Dma dma3;
        public Dma dma4;
        public Dma dma5;
        public Dma dma6;
        public Dma dma7;

        public class Dma : SuperFamicom.Component
        {
            private Register16 length;
            private Register24 addr_a;
            private Register24 addr_b;
            private byte[] regs = new byte[ 12 ];

            public Dma( SuperFamicom console )
                : base( console ) { }
            
            public byte Peek0( uint address ) { return regs[ 0x0u ]; }
            public byte Peek1( uint address ) { return regs[ 0x1u ]; }
            public byte Peek2( uint address ) { return regs[ 0x2u ]; }
            public byte Peek3( uint address ) { return regs[ 0x3u ]; }
            public byte Peek4( uint address ) { return regs[ 0x4u ]; }
            public byte Peek5( uint address ) { return regs[ 0x5u ]; }
            public byte Peek6( uint address ) { return regs[ 0x6u ]; }
            public byte Peek7( uint address ) { return regs[ 0x7u ]; }
            public byte Peek8( uint address ) { return regs[ 0x8u ]; }
            public byte Peek9( uint address ) { return regs[ 0x9u ]; }
            public byte PeekA( uint address ) { return regs[ 0xau ]; }
            public byte Peekx( uint address ) { return regs[ 0xbu ]; }
            public void Poke0( uint address, byte data ) { regs[ 0x0u ] = data; }
            public void Poke1( uint address, byte data ) { regs[ 0x1u ] = data; }
            public void Poke2( uint address, byte data ) { regs[ 0x2u ] = data; }
            public void Poke3( uint address, byte data ) { regs[ 0x3u ] = data; }
            public void Poke4( uint address, byte data ) { regs[ 0x4u ] = data; }
            public void Poke5( uint address, byte data ) { regs[ 0x5u ] = data; }
            public void Poke6( uint address, byte data ) { regs[ 0x6u ] = data; }
            public void Poke7( uint address, byte data ) { regs[ 0x7u ] = data; }
            public void Poke8( uint address, byte data ) { regs[ 0x8u ] = data; }
            public void Poke9( uint address, byte data ) { regs[ 0x9u ] = data; }
            public void PokeA( uint address, byte data ) { regs[ 0xau ] = data; }
            public void Pokex( uint address, byte data ) { regs[ 0xbu ] = data; }

            private void ClockAddrA( )
            {
                switch ( ( regs[ 0u ] & 0x18u ) >> 3 )
                {
                default: addr_a.w++; break;
                case 1u: break;
                case 2u: addr_a.w--; break;
                case 3u: break;
                }
            }
            private void ClockAddrB( uint index )
            {
                switch ( ( regs[ 0u ] & 0x07u ) >> 0 )
                {
                default: addr_b.l = ( byte )( regs[ 1u ]                           ); break;
                case 1u: addr_b.l = ( byte )( regs[ 1u ] + ( ( index >> 0 ) & 1u ) ); break;
                case 2u: addr_b.l = ( byte )( regs[ 1u ]                           ); break;
                case 3u: addr_b.l = ( byte )( regs[ 1u ] + ( ( index >> 1 ) & 1u ) ); break;
                case 4u: addr_b.l = ( byte )( regs[ 1u ] + ( ( index >> 0 ) & 3u ) ); break;
                case 5u: addr_b.l = ( byte )( regs[ 1u ] + ( ( index >> 0 ) & 1u ) ); break;
                case 6u: addr_b.l = ( byte )( regs[ 1u ]                           ); break;
                case 7u: addr_b.l = ( byte )( regs[ 1u ] + ( ( index >> 1 ) & 1u ) ); break;
                }
            }

            public void Initialize( uint address )
            {
                base.Initialize( );

                regs[ 0x0u ] = 0xff;
                regs[ 0x1u ] = 0xff;
                regs[ 0x2u ] = 0xff;
                regs[ 0x3u ] = 0xff;
                regs[ 0x4u ] = 0xff;
                regs[ 0x5u ] = 0xff;
                regs[ 0x6u ] = 0xff;
                regs[ 0x7u ] = 0xff;
                regs[ 0x8u ] = 0xff;
                regs[ 0x9u ] = 0xff;
                regs[ 0xau ] = 0xff;
                regs[ 0xbu ] = 0xff;

                cpu.Hook( address + 0x0u, Peek0, Poke0 );
                cpu.Hook( address + 0x1u, Peek1, Poke1 );
                cpu.Hook( address + 0x2u, Peek2, Poke2 );
                cpu.Hook( address + 0x3u, Peek3, Poke3 );
                cpu.Hook( address + 0x4u, Peek4, Poke4 );
                cpu.Hook( address + 0x5u, Peek5, Poke5 );
                cpu.Hook( address + 0x6u, Peek6, Poke6 );
                cpu.Hook( address + 0x7u, Peek7, Poke7 );
                cpu.Hook( address + 0x8u, Peek8, Poke8 );
                cpu.Hook( address + 0x9u, Peek9, Poke9 );
                cpu.Hook( address + 0xau, PeekA, PokeA );
                cpu.Hook( address + 0xbu, Peekx, Pokex );
                cpu.Hook( address + 0xfu, Peekx, Pokex );
            }

            public void Transfer( )
            {
                var index = 0U;

                addr_b.l = regs[ 1u ];
                addr_b.h = 0x21;
                addr_b.b = 0x00;
                addr_a.l = regs[ 2u ];
                addr_a.h = regs[ 3u ];
                addr_a.b = regs[ 4u ];
                length.l = regs[ 5u ];
                length.h = regs[ 6u ];

                do
                {
                    // DMA takes advantage of the 2 address buses, and shared data bus.
                    switch ( ( regs[ 0u ] & 0x80u ) >> 7 )
                    {
                    default: cpu.PokeBusB( addr_b.d, cpu.PeekBusA( addr_a.d ) ); break; // CPU->PPU
                    case 1u: cpu.PokeBusA( addr_a.d, cpu.PeekBusB( addr_b.d ) ); break; // PPU->CPU
                    }

                    cpu.Cycles += Cpu.SPEED_NORM; // always 8 master cycles per byte

                    ClockAddrA( );
                    ClockAddrB( ++index );
                }
                while ( --length.w != 0 );

                regs[ 1u ] = addr_b.l; // update addrb
                regs[ 2u ] = addr_a.l; // update addra
                regs[ 3u ] = addr_a.h;
                regs[ 4u ] = addr_a.b;
                regs[ 5u ] = length.l; // update count
                regs[ 6u ] = length.h;
            }

            public void VBlank( ) { }
            public void HBlank( ) { }
        }
    }
}