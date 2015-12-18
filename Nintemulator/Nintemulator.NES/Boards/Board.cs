using Nintemulator.Shared;
using System.IO;

namespace Nintemulator.FC.Boards
{
    public class Board : Famicom.Component
    {
        private FamicomDatabase.Game.Cartridge.Board.Chip[] chips;

        protected MemoryChip[] prgChips;
        protected MemoryChip[] chrChips;
        protected MemoryChip[] ramChips;

        protected MemoryChip chr;
        protected MemoryChip prg;
        protected MemoryChip ram;
        protected uint[] chrPage;
        protected uint[] prgPage;
        protected uint[] ramPage;
        protected string type;

        public Board( Famicom console, byte[] binary, FamicomDatabase.Game.Cartridge.Board board )
            : base( console )
        {
            var stream = new MemoryStream( binary );
            var reader = new BinaryReader( stream );

            reader.BaseStream.Seek( 16L, SeekOrigin.Begin ); // skip shitty ines header

            prgChips = new MemoryChip[ board.prg.Count ];
            chrChips = new MemoryChip[ board.chr.Count + board.vram.Count ];
            ramChips = new MemoryChip[ board.wram.Count ];

            for ( int i = 0; i < board.prg.Count;  i++ ) { prgChips[ i                   ] = new MemoryChip( reader.ReadBytes( board.prg[ i ].size ) ); }
            for ( int i = 0; i < board.chr.Count;  i++ ) { chrChips[ i                   ] = new MemoryChip( reader.ReadBytes( board.chr[ i ].size ) ); }
            for ( int i = 0; i < board.vram.Count; i++ ) { chrChips[ i + board.chr.Count ] = new MemoryChip( ( uint )board.vram[ i ].size ); }
            for ( int i = 0; i < board.wram.Count; i++ ) { ramChips[ i                   ] = new MemoryChip( ( uint )board.wram[ i ].size ); }

            type = board.type;
            chips = board.chip.ToArray( );

            SelectPrg( );
            SelectChr( );
            SelectRam( );
        }

        protected void SelectPrg( uint chip = 0U ) { prg = ( chip < prgChips.Length ) ? prgChips[ chip ] : null; }
        protected void SelectChr( uint chip = 0U ) { chr = ( chip < chrChips.Length ) ? chrChips[ chip ] : null; }
        protected void SelectRam( uint chip = 0U ) { ram = ( chip < ramChips.Length ) ? ramChips[ chip ] : null; }

        protected string GetPin( string chip, uint number )
        {
            foreach ( var c in chips )
            {
                if ( c.type == chip )
                {
                    foreach ( var p in c.pin )
                    {
                        if ( p.number == number )
                            return p.function;
                    }
                }
            }

            return null;
        }

        protected override void OnInitialize( )
        {
            cpu.Hook( 0x8000, 0xffff, PeekPrg, PokePrg );
            gpu.Hook( 0x0000, 0x1fff, PeekChr, PokeChr );

            if ( ram == null ) return;

            cpu.Hook( 0x6000, 0x7fff, PeekRam, PokeRam );
        }

        protected virtual uint DecodeChr( uint address ) { return address; }
        protected virtual uint DecodePrg( uint address ) { return address; }
        protected virtual uint DecodeRam( uint address ) { return address; }

        protected virtual byte PeekChr( uint address ) { return chr.b[ DecodeChr( address ) & chr.mask ]; }
        protected virtual byte PeekPrg( uint address ) { return prg.b[ DecodePrg( address ) & prg.mask ]; }
        protected virtual byte PeekRam( uint address ) { return ram.b[ DecodeRam( address ) & ram.mask ]; }
        protected virtual void PokeChr( uint address, byte data )
        {
            if ( chr.writable )
                chr.b[ DecodeChr( address ) & chr.mask ] = data;
        }
        protected virtual void PokePrg( uint address, byte data ) { }
        protected virtual void PokeRam( uint address, byte data )
        {
            if ( ram.writable )
                ram.b[ DecodeRam( address ) & ram.mask ] = data;
        }

        public virtual void Clock( ) { }
        public virtual void CpuAddressUpdate( uint address ) { }
        public virtual void GpuAddressUpdate( uint address ) { }
    }
}