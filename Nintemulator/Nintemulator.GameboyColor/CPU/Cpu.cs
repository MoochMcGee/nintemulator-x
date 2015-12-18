using Nintemulator.Shared;
using System.Runtime.InteropServices;

namespace Nintemulator.GBC.CPU
{
    public partial class Cpu : GameboyColor.Processor
    {
        internal Registers registers = new Registers( );
        internal Interrupt interrupt = new Interrupt( );
        internal bool z;
        internal bool n;
        internal bool h;
        internal bool c;
        internal bool swap_speed;
        internal uint code;

        public int ClockShift;

        public Cpu( GameboyColor console, Timing.System system )
            : base( console, system )
        {
            Single = system.Cpu;
        }

        private static int GetCarryBits( int a, int b, int r )
        {
            return ( a & b ) | ( ( a ^ b ) & ~r );
        }

        private byte operand( int shift )
        {
            switch ( ( code >> shift ) & 7u )
            {
            default: return registers.b;
            case 1u: return registers.c;
            case 2u: return registers.d;
            case 3u: return registers.e;
            case 4u: return registers.h;
            case 5u: return registers.l;
            case 6u: return console.Peek( registers.hl );
            case 7u: return registers.a;
            }
        }
        private void operand( int shift, byte value )
        {
            switch ( ( code >> shift ) & 7u )
            {
            default: registers.b = value; break;
            case 1u: registers.c = value; break;
            case 2u: registers.d = value; break;
            case 3u: registers.e = value; break;
            case 4u: registers.h = value; break;
            case 5u: registers.l = value; break;
            case 6u: console.Poke( registers.hl, value ); break;
            case 7u: registers.a = value; break;
            }
        }

        private byte PeekFF4D( uint address )
        {
            return ( byte )( ClockShift << 7 );
        }
        private void PokeFF4D( uint address, byte data )
        {
            swap_speed = ( data & 0x01 ) != 0;
        }

        #region (H)DMA

        private Register16 dma_dst;
        private Register16 dma_src;
        private bool dma_hbl;
        private uint dma_len;

        private byte PeekFF55( uint address ) { return ( byte )( ( dma_hbl ? 0x00u : 0x80u ) | dma_len ); }
        private void PokeFF55( uint address, byte data )
        {
            dma_len = ( data & 0x7fu );
            dma_hbl = ( data & 0x80u ) != 0;

            if ( !dma_hbl )
            {
                do
                {
                    DmaUpdate( );
                }
                while ( dma_len != 0x7fu );
            }
        }

        #endregion

        private void DmaUpdate( )
        {
            var dst = dma_dst.w;
            var src = dma_src.w;

            console.Poke( dst +  0u, console.Peek( src +  0u ) );
            console.Poke( dst +  1u, console.Peek( src +  1u ) );
            console.Poke( dst +  2u, console.Peek( src +  2u ) );
            console.Poke( dst +  3u, console.Peek( src +  3u ) );
            console.Poke( dst +  4u, console.Peek( src +  4u ) );
            console.Poke( dst +  5u, console.Peek( src +  5u ) );
            console.Poke( dst +  6u, console.Peek( src +  6u ) );
            console.Poke( dst +  7u, console.Peek( src +  7u ) );
            console.Poke( dst +  8u, console.Peek( src +  8u ) );
            console.Poke( dst +  9u, console.Peek( src +  9u ) );
            console.Poke( dst + 10u, console.Peek( src + 10u ) );
            console.Poke( dst + 11u, console.Peek( src + 11u ) );
            console.Poke( dst + 12u, console.Peek( src + 12u ) );
            console.Poke( dst + 13u, console.Peek( src + 13u ) );
            console.Poke( dst + 14u, console.Peek( src + 14u ) );
            console.Poke( dst + 15u, console.Peek( src + 15u ) );

            dma_len = ( --dma_len ) & 0x7fu;
            dma_dst.w += 16;
            dma_src.w += 16;
        }
        private void LoadFlags( )
        {
            z = ( registers.f & 0x80 ) != 0;
            n = ( registers.f & 0x40 ) != 0;
            h = ( registers.f & 0x20 ) != 0;
            c = ( registers.f & 0x10 ) != 0;
        }
        private void SaveFlags( )
        {
            registers.f = ( byte )(
                ( z ? 0x80 : 0 ) |
                ( n ? 0x40 : 0 ) |
                ( h ? 0x20 : 0 ) |
                ( c ? 0x10 : 0 ) );
        }

        protected override void OnInitialize( )
        {
            base.OnInitialize( );

            console.Hook( 0xFF0F, ( a ) => interrupt.rf, ( a, d ) => interrupt.rf = d );
            console.Hook( 0xFFFF, ( a ) => interrupt.ef, ( a, d ) => interrupt.ef = d );

            console.Hook( 0xFF4D, PeekFF4D, PokeFF4D );
            console.Hook( 0xFF51, ( a ) => dma_src.h, ( a, d ) => dma_src.h = d );
            console.Hook( 0xFF52, ( a ) => dma_src.l, ( a, d ) => dma_src.l = d );
            console.Hook( 0xFF53, ( a ) => dma_dst.h, ( a, d ) => dma_dst.h = d );
            console.Hook( 0xFF54, ( a ) => dma_dst.l, ( a, d ) => dma_dst.l = d );
            console.Hook( 0xFF55, PeekFF55, PokeFF55 );
        }

        public override void Update( )
        {
            StandardCode( );

            var flags = ( interrupt.rf & interrupt.ef ) & -interrupt.ff1.i;

            if ( flags != 0 )
            {
                interrupt.ff1.i = 0;

                if ( ( flags & 0x01 ) != 0 ) { interrupt.rf ^= 0x01; rst( 0x40 ); }
                if ( ( flags & 0x02 ) != 0 ) { interrupt.rf ^= 0x02; rst( 0x48 ); }
                if ( ( flags & 0x04 ) != 0 ) { interrupt.rf ^= 0x04; rst( 0x50 ); }
                if ( ( flags & 0x08 ) != 0 ) { interrupt.rf ^= 0x08; rst( 0x58 ); }
                if ( ( flags & 0x10 ) != 0 ) { interrupt.rf ^= 0x10; rst( 0x60 ); }
            }
        }

        public void EnterHBlank( )
        {
            if ( dma_hbl )
            {
                DmaUpdate( );

                if ( dma_len == 0x7f )
                    dma_hbl = false;
            }
        }
        public void EnterVBlank( ) { }
        public void RequestInterrupt( byte flag )
        {
            interrupt.rf |= flag;

            if ( ( interrupt.ef & flag ) != 0 )
            {
                interrupt.halt = false;

                if ( flag == Interrupt.Joypad ) interrupt.stop = false;
            }
        }

        [StructLayout( LayoutKind.Explicit )]
        public class Registers
        {
            [FieldOffset(  1 )] public byte b;
            [FieldOffset(  0 )] public byte c;
            [FieldOffset(  3 )] public byte d;
            [FieldOffset(  2 )] public byte e;
            [FieldOffset(  5 )] public byte h;
            [FieldOffset(  4 )] public byte l;
            [FieldOffset(  7 )] public byte a;
            [FieldOffset(  6 )] public byte f;
            [FieldOffset(  9 )] public byte sph;
            [FieldOffset(  8 )] public byte spl;
            [FieldOffset( 11 )] public byte pch;
            [FieldOffset( 10 )] public byte pcl;
            [FieldOffset( 13 )] public byte aah;
            [FieldOffset( 12 )] public byte aal;

            [FieldOffset(  0 )] public ushort bc;
            [FieldOffset(  2 )] public ushort de;
            [FieldOffset(  4 )] public ushort hl;
            [FieldOffset(  6 )] public ushort af;
            [FieldOffset(  8 )] public ushort sp;
            [FieldOffset( 10 )] public ushort pc;
            [FieldOffset( 12 )] public ushort aa;
        }
        public class Interrupt
        {
            public const byte VBlank = ( 1 << 0 );
            public const byte Status = ( 1 << 1 );
            public const byte Elapse = ( 1 << 2 );
            public const byte Serial = ( 1 << 3 );
            public const byte Joypad = ( 1 << 4 );

            public Flag ff1;
            public Flag ff2;
            public bool halt;
            public bool stop;
            public byte ef;
            public byte rf;
        }
    }
}