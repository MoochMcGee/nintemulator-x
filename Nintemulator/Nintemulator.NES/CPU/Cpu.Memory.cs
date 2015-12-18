using Nintemulator.Shared;
using word = System.UInt32;

namespace Nintemulator.FC.CPU
{
    public partial class Cpu
    {
        private const int BUS_BITS = 16;
        private const int BUS_SIZE = 1 << BUS_BITS;

        private Peek[] peeks = new Peek[ BUS_SIZE ];
        private Poke[] pokes = new Poke[ BUS_SIZE ];
        private bool dma;
        private uint dma_addr;
        private byte open;
        private byte[] wram = new byte[ 2048 ];

        private uint rw, rw_old;

        public byte Open
        {
            get { return open; }
        }
        public uint Edge
        {
            get { return rw ^ rw_old; }
        }

        private byte PeekOpen( uint address ) { return open; }
        private byte PeekWRam( uint address ) { return wram[ address & 0x07ffu ]; }
        private void PokeOpen( uint address, byte data ) { open = data; }
        private void PokeWRam( uint address, byte data ) { wram[ address & 0x07ffu ] = data; }
        private void Poke4014( uint address, byte data )
        {
            dma = true;
            dma_addr = ( uint )( data << 8 );
        }

        protected void InitializeMemory( )
        {
            Hook( 0x0000U, 0x1fffu, PeekWRam, PokeWRam );
            Hook( 0x4014u, /*              */ Poke4014 );
        }

        public void Hook( word address, Peek peek ) { peeks[ address ] = peek; }
        public void Hook( word address, Poke poke ) { pokes[ address ] = poke; }
        public void Hook( word address, Peek peek, Poke poke )
        {
            Hook( address, peek );
            Hook( address, poke );
        }
        public void Hook( word address, word last, Peek peek )
        {
            for ( ; address <= last; address++ )
                Hook( address, peek );
        }
        public void Hook( word address, word last, Poke poke )
        {
            for ( ; address <= last; address++ )
                Hook( address, poke );
        }
        public void Hook( word address, word last, Peek peek, Poke poke )
        {
            for ( ; address <= last; address++ )
            {
                Hook( address, peek );
                Hook( address, poke );
            }
        }

        public byte PeekByteDebugger( uint address )
        {
            // todo: add more oopsie addresses
            if ( address == 0x2002 ) return 0xff;
            if ( address == 0x2007 ) return 0xff;
            if ( address == 0x4015 ) return 0xff;

            return peeks[ address & 0xffffu ]( address );
        }

        public byte PeekByte( uint address, bool last = false )
        {
            rw_old = rw;
            rw = 0u;

            if ( last ) interrupts.Poll( p.i.u );

            spu.Update( Single );
            gpu.Update( Single );

            console.Board.Clock( );

            return open = peeks[ address ]( address );
        }
        public void PokeByte( uint address, byte data, bool last = false )
        {
            rw_old = rw;
            rw = 1u;

            if ( last ) interrupts.Poll( p.i.u );

            spu.Update( Single );
            gpu.Update( Single );

            console.Board.Clock( );

            pokes[ address ]( address, open = data );
        }
    }
}