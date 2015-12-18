using Nintemulator.Shared;
using half = System.UInt16;
using word = System.UInt32;

namespace Nintemulator.GBA
{
    public partial class GameboyAdvance
    {
        #region Constants

        private const uint ERamSize = ( 1U << 18 ), ERamMask = ( ERamSize - 1U );
        private const uint IRamSize = ( 1U << 15 ), IRamMask = ( IRamSize - 1U );
        private const uint ORamSize = ( 1U << 10 ), ORamMask = ( ORamSize - 1U );
        private const uint PRamSize = ( 1U << 10 ), PRamMask = ( PRamSize - 1U );
        private const uint RegsSize = ( 1U << 10 ), RegsMask = ( RegsSize - 1U );
        private const uint SRamSize = ( 1U << 16 ), SRamMask = ( SRamSize - 1U );
        private const uint VRamSize = ( 3U << 15 ); // VRamMask isn't defined, since it's useless ($17FFF)

        #endregion

        //                                   0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F
        private static int[] BusTiming08 = { 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        private static int[] BusTiming16 = { 1, 1, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
        private static int[] BusTiming32 = { 1, 1, 6, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        private Peek[] peek = new Peek[ RegsSize ];
        private Poke[] poke = new Poke[ RegsSize ];
        private Register32 latch;
        private uint bios_open;

        public MemoryChip bios;
        public MemoryChip eram = new MemoryChip( 256 * 1024 ); // 256KB
        public MemoryChip iram = new MemoryChip(  32 * 1024 ); // 32KB
        public MemoryChip oram = new MemoryChip(   1 * 1024 ); // 1KB
        public MemoryChip pram = new MemoryChip(   1 * 1024 ); // 1KB
        public MemoryChip sram = new MemoryChip(  64 * 1024 ); // 64KB
        public MemoryChip vram = new MemoryChip(  96 * 1024 ); // 64KB + 32KB

        private word PeekBios( word size, word address )
        {
            if ( cpu.pc.value < 0x4000u )
            {
                if ( size == 2 ) bios_open = bios.w[ address >> 2 ];
                if ( size == 1 ) bios_open = bios.h[ address >> 1 ];
                if ( size == 0 ) bios_open = bios.b[ address >> 0 ];
            }

            return bios_open;
        }

        public byte DebuggerPeekByte( word address )
        {
            switch ( ( address >> 24 ) & 15U )
            {
            case 0x0U:
            case 0x1U: return bios.b[ address & 0x3ffff ];
            case 0x2U: return eram.b[ address & ERamMask ];
            case 0x3U: return iram.b[ address & IRamMask ];
            case 0x4U:
                if ( ( address & 0xFFFFFFU ) < RegsSize )
                {
                    return peek[ address & RegsMask ]( address );
                }
                break;

            case 0x5U: return pram.b[ address & PRamMask ];
            case 0x6U:
                switch ( ( address >> 16 ) & 0x01U )
                {
                case 0U: return vram.b[ address & 0x0FFFFU ];
                case 1U: return vram.b[ address & 0x17FFFU ];
                }
                break;

            case 0x7U: return oram.b[ address & ORamMask ];
            }

            return 0;
        }
        public half DebuggerPeekHalf( word address )
        {
            switch ( ( address >> 24 ) & 15U )
            {
            case 0x0:
            case 0x1: return bios.h[ ( address & 0x3ffff ) >> 1 ];
            case 0x2: return eram.h[ ( address & ERamMask ) >> 1 ];
            case 0x3: return iram.h[ ( address & IRamMask ) >> 1 ];
            case 0x4:
                if ( ( address & 0xFFFFFEU ) < RegsSize )
                {
                    address &= RegsMask & ~1U;

                    latch.ub0 = peek[ address | 0U ]( address | 0U );
                    latch.ub1 = peek[ address | 1U ]( address | 1U );

                    return latch.uw0;
                }
                break;

            case 0x5: return pram.h[ ( address & PRamMask ) >> 1 ];
            case 0x6:
                switch ( ( address >> 16 ) & 0x01U )
                {
                case 0U: return vram.h[ ( address & 0x0FFFFU ) >> 1 ];
                case 1U: return vram.h[ ( address & 0x17FFFU ) >> 1 ];
                }
                break;

            case 0x7: return oram.h[ ( address & ORamMask ) >> 1 ];
            }

            return 0;
        }
        public word DebuggerPeekWord( word address )
        {
            switch ( ( address >> 24 ) & 15U )
            {
            case 0x0:
            case 0x1: return bios.w[ ( address & 0x3ffff ) >> 2 ];
            case 0x2: return eram.w[ ( address & ERamMask ) >> 2 ];
            case 0x3: return iram.w[ ( address & IRamMask ) >> 2 ];
            case 0x4:
                if ( ( address & 0xFFFFFCU ) < RegsSize )
                {
                    address &= RegsMask & ~3U;

                    latch.ub0 = peek[ address | 0U ]( address | 0U );
                    latch.ub1 = peek[ address | 1U ]( address | 1U );
                    latch.ub2 = peek[ address | 2U ]( address | 2U );
                    latch.ub3 = peek[ address | 3U ]( address | 3U );

                    return latch.ud0;
                }
                break;

            case 0x5: return pram.w[ ( address & PRamMask ) >> 2 ];
            case 0x6:
                switch ( ( address >> 16 ) & 0x01U )
                {
                case 0U: return vram.w[ ( address & 0x0FFFFU ) >> 2 ];
                case 1U: return vram.w[ ( address & 0x17FFFU ) >> 2 ];
                }
                break;

            case 0x7: return oram.w[ ( address & ORamMask ) >> 2 ];
            }

            return 0;
        }

        public word PeekByte( word address )
        {
            var area = ( address >> 24 ) & 15U;

            cpu.Cycles += BusTiming08[ area ];

            switch ( area )
            {
            case 0x0U:
            case 0x1U: return PeekBios( 0u, address );
            case 0x2U: return eram.b[ address & ERamMask ];
            case 0x3U: return iram.b[ address & IRamMask ];
            case 0x4U:
                if ( ( address & 0xFFFFFFU ) < RegsSize )
                {
                    return peek[ address & RegsMask ]( address );
                }
                break;

            case 0x5U: return pram.b[ address & PRamMask ];
            case 0x6U:
                switch ( ( address >> 16 ) & 0x01U )
                {
                case 0U: return vram.b[ address & 0x0FFFFU ];
                case 1U: return vram.b[ address & 0x17FFFU ];
                }
                break;

            case 0x7U: return oram.b[ address & ORamMask ];
            case 0x8U: return Cart.PeekByte( address );
            case 0x9U: return Cart.PeekByte( address );
            case 0xAU: return Cart.PeekByte( address );
            case 0xBU: return Cart.PeekByte( address );
            case 0xCU: return Cart.PeekByte( address );
            case 0xDU: return Cart.PeekByte( address );
            case 0xEU: cpu.Cycles += Cart.WRamAccess; return sram.PeekByte( address & 0xFFFF );
            case 0xFU: cpu.Cycles += Cart.WRamAccess; return sram.PeekByte( address & 0xFFFF );
            }

            return 0;
        }
        public word PeekHalf( word address )
        {
            var area = ( address >> 24 ) & 15U;

            cpu.Cycles += BusTiming16[ area ];

            switch ( area )
            {
            case 0x0:
            case 0x1: return PeekBios( 1u, address );
            case 0x2: return eram.h[ ( address & ERamMask ) >> 1 ];
            case 0x3: return iram.h[ ( address & IRamMask ) >> 1 ];
            case 0x4:
                if ( ( address & 0xFFFFFEU ) < RegsSize )
                {
                    address &= RegsMask & ~1U;

                    latch.ub0 = peek[ address | 0U ]( address | 0U );
                    latch.ub1 = peek[ address | 1U ]( address | 1U );

                    return latch.uw0;
                }
                break;

            case 0x5: return pram.h[ ( address & PRamMask ) >> 1 ];
            case 0x6:
                switch ( ( address >> 16 ) & 0x01U )
                {
                case 0U: return vram.h[ ( address & 0x0FFFFU ) >> 1 ];
                case 1U: return vram.h[ ( address & 0x17FFFU ) >> 1 ];
                }
                break;

            case 0x7: return oram.h[ ( address & ORamMask ) >> 1 ];
            case 0x8: return Cart.PeekHalf( address );
            case 0x9: return Cart.PeekHalf( address );
            case 0xA: return Cart.PeekHalf( address );
            case 0xB: return Cart.PeekHalf( address );
            case 0xC: return Cart.PeekHalf( address );
            case 0xD: return Cart.PeekHalf( address );
            case 0xE: cpu.Cycles += Cart.WRamAccess; break;
            case 0xF: cpu.Cycles += Cart.WRamAccess; break;
            }

            return 0;
        }
        public word PeekWord( word address )
        {
            var area = ( address >> 24 ) & 15U;

            cpu.Cycles += BusTiming32[ area ];

            switch ( area )
            {
            case 0x0:
            case 0x1: return PeekBios( 2u, address );
            case 0x2: return eram.w[ ( address & ERamMask ) >> 2 ];
            case 0x3: return iram.w[ ( address & IRamMask ) >> 2 ];
            case 0x4:
                if ( ( address & 0xFFFFFCU ) < RegsSize )
                {
                    address &= RegsMask & ~3U;

                    latch.ub0 = peek[ address | 0U ]( address | 0U );
                    latch.ub1 = peek[ address | 1U ]( address | 1U );
                    latch.ub2 = peek[ address | 2U ]( address | 2U );
                    latch.ub3 = peek[ address | 3U ]( address | 3U );

                    return latch.ud0;
                }
                break;

            case 0x5: return pram.w[ ( address & PRamMask ) >> 2 ];
            case 0x6:
                switch ( ( address >> 16 ) & 0x01U )
                {
                case 0U: return vram.w[ ( address & 0x0FFFFU ) >> 2 ];
                case 1U: return vram.w[ ( address & 0x17FFFU ) >> 2 ];
                }
                break;

            case 0x7: return oram.w[ ( address & ORamMask ) >> 2 ];
            case 0x8: return Cart.PeekWord( address );
            case 0x9: return Cart.PeekWord( address );
            case 0xA: return Cart.PeekWord( address );
            case 0xB: return Cart.PeekWord( address );
            case 0xC: return Cart.PeekWord( address );
            case 0xD: return Cart.PeekWord( address );
            case 0xE: cpu.Cycles += Cart.WRamAccess; break;
            case 0xF: cpu.Cycles += Cart.WRamAccess; break;
            }

            return 0;
        }
        public void PokeByte( word address, byte data )
        {
            var area = ( address >> 24 ) & 15U;

            cpu.Cycles += BusTiming08[ area ];

            switch ( area )
            {
            case 0x2: eram.b[ ( address & ERamMask ) >> 0 ] = data; break;
            case 0x3: iram.b[ ( address & IRamMask ) >> 0 ] = data; break;
            case 0x4:
                if ( ( address & 0xFFFFFFU ) < RegsSize )
                {
                    poke[ address & RegsMask ]( address, data );
                }
                break;

            case 0x5:
                address &= ( PRamMask & ~1U );
                pram.b[ address | 0U ] = data;
                pram.b[ address | 1U ] = data &= 0x7f;
                break;

            case 0x6:
                switch ( ( address >> 16 ) & 0x01U )
                {
                case 0U: address &= 0x0FFFEU; break;
                case 1U: address &= 0x17FFEU; break;
                }

                vram.b[ address | 0U ] = data;
                vram.b[ address | 1U ] = data;
                break;

            case 0x7:
                address &= ( ORamMask & ~1U );
                oram.b[ address | 0U ] = data;
                oram.b[ address | 1U ] = data;
                break;

            case 0xE: cpu.Cycles += Cart.WRamAccess; sram.PokeByte( address & SRamMask, data ); break;
            case 0xF: cpu.Cycles += Cart.WRamAccess; sram.PokeByte( address & SRamMask, data ); break;
            }
        }
        public void PokeHalf( word address, half data )
        {
            var area = ( address >> 24 ) & 15U;

            cpu.Cycles += BusTiming16[ area ];

            switch ( area )
            {
            case 0x2: eram.h[ ( address & ERamMask ) >> 1 ] = data; break;
            case 0x3: iram.h[ ( address & IRamMask ) >> 1 ] = data; break;
            case 0x4:
                if ( ( address & 0xFFFFFEU ) < RegsSize )
                {
                    address &= RegsMask & ~1U;
                    latch.uw0 = data;

                    poke[ address | 0U ]( address | 0U, latch.ub0 );
                    poke[ address | 1U ]( address | 1U, latch.ub1 );
                }
                break;

            case 0x5: pram.h[ ( address & PRamMask ) >> 1 ] = data &= 0x7fff; break;
            case 0x6:
                switch ( ( address >> 16 ) & 0x01U )
                {
                case 0U: vram.h[ ( address & 0x0FFFFU ) >> 1 ] = data; break;
                case 1U: vram.h[ ( address & 0x17FFFU ) >> 1 ] = data; break;
                }
                break;

            case 0x7: oram.h[ ( address & ORamMask ) >> 1 ] = data; break;
            case 0xE: cpu.Cycles += Cart.WRamAccess; break;
            case 0xF: cpu.Cycles += Cart.WRamAccess; break;
            }
        }
        public void PokeWord( word address, word data )
        {
            var area = ( address >> 24 ) & 15U;

            cpu.Cycles += BusTiming32[ area ];

            switch ( area )
            {
            case 0x2: eram.w[ ( address & ERamMask ) >> 2 ] = data; break;
            case 0x3: iram.w[ ( address & IRamMask ) >> 2 ] = data; break;
            case 0x4:
                if ( ( address & 0xFFFFFCU ) < RegsSize )
                {
                    address &= ( RegsMask & ~3U );
                    latch.ud0 = data;

                    poke[ address | 0U ]( address | 0U, latch.ub0 );
                    poke[ address | 1U ]( address | 1U, latch.ub1 );
                    poke[ address | 2U ]( address | 2U, latch.ub2 );
                    poke[ address | 3U ]( address | 3U, latch.ub3 );
                }
                break;

            case 0x5: pram.w[ ( address & PRamMask ) >> 2 ] = data &= 0x7fff7fff; break;
            case 0x6:
                switch ( ( address >> 16 ) & 0x01U )
                {
                case 0U: vram.w[ ( address & 0x0FFFFU ) >> 2 ] = data; break;
                case 1U: vram.w[ ( address & 0x17FFFU ) >> 2 ] = data; break;
                }
                break;

            case 0x7: oram.w[ ( address & ORamMask ) >> 2 ] = data; break;
            case 0xE: cpu.Cycles += Cart.WRamAccess; break;
            case 0xF: cpu.Cycles += Cart.WRamAccess; break;
            }
        }

        public word LoadWord( word address )
        {
            var data = PeekWord( address & ~3U );

            switch ( address & 0x03U )
            {
            default:
            case 0U: return data;
            case 1U: return ( data >> 0x08 ) | ( data << 0x18 );
            case 2U: return ( data >> 0x10 ) | ( data << 0x10 );
            case 3U: return ( data >> 0x18 ) | ( data << 0x08 );
            }
        }

        public void Hook( word address, Peek peek ) { this.peek[ address ] = peek; }
        public void Hook( word address, Poke poke ) { this.poke[ address ] = poke; }
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
                Hook( address, peek, poke );
        }
    }
}