using Nintemulator.Shared;

namespace Nintemulator.GBA
{
    public class Dma : GameboyAdvance.Component
    {
        public const int Type0 = 0x0000;
        public const int Type1 = 0x1000;
        public const int Type2 = 0x2000;
        public const int Type3 = 0x3000;

        private static uint[] StepLut = { 1u, ~0u, 0u, 1u };

        protected Register16 controlRegister;
        protected Register16 counterRegister;
        protected Register32 dstAddrRegister;
        protected Register32 srcAddrRegister;
        protected bool refresh;
        protected uint lenMask;
        protected uint dstAddr, dstMask, dstStep;
        protected uint srcAddr, srcMask, srcStep;
        protected ushort interruptType;

        public bool Request;

        public bool Enabled { get { return ( controlRegister.w & 0x8000U ) != 0; } }
        public uint Type    { get { return ( controlRegister.w & 0x3000U ); } }

        public Dma( GameboyAdvance console, ushort interruptType, int lenBits, int dstBits, int srcBits )
            : base( console )
        {
            this.interruptType = interruptType;
            this.lenMask = ( 1u << lenBits ) - 1u;
            this.dstMask = ( 1u << dstBits ) - 1u;
            this.srcMask = ( 1u << srcBits ) - 1u;
        }

        private uint TransferHalf( uint count )
        {
            dstStep <<= 1;
            srcStep <<= 1;

            do
            {
                console.PokeHalf( dstAddr, ( ushort )console.PeekHalf( srcAddr ) );
                dstAddr = ( dstAddr + dstStep ) & dstMask;
                srcAddr = ( srcAddr + srcStep ) & srcMask;
            }
            while ( ( --count & lenMask ) != 0 );

            return count;
        }
        private uint TransferWord( uint count )
        {
            dstStep <<= 2;
            srcStep <<= 2;

            do
            {
                console.PokeWord( dstAddr, console.PeekWord( srcAddr ) );
                dstAddr = ( dstAddr + dstStep ) & dstMask;
                srcAddr = ( srcAddr + srcStep ) & srcMask;
            }
            while ( ( --count & lenMask ) != 0 );

            return count;
        }

        #region Registers

        private byte PeekControl_0( uint address ) { return controlRegister.l; }
        private byte PeekControl_1( uint address ) { return controlRegister.h; }
        private void PokeControl_0( uint address, byte data ) { controlRegister.l = data; }
        private void PokeControl_1( uint address, byte data )
        {
            if ( controlRegister.h < ( data & 0x80 ) )
            {
                dstAddr = dstAddrRegister.ud0;
                srcAddr = srcAddrRegister.ud0;

                if ( ( data & 0x30 ) == 0x00 )
                    Request = true;
            }

            controlRegister.h = data;
        }
        private void PokeCounter_0( uint address, byte data ) { counterRegister.l = data; }
        private void PokeCounter_1( uint address, byte data ) { counterRegister.h = data; }
        private void PokeDstAddr_0( uint address, byte data ) { dstAddrRegister.ub0 = data; }
        private void PokeDstAddr_1( uint address, byte data ) { dstAddrRegister.ub1 = data; }
        private void PokeDstAddr_2( uint address, byte data ) { dstAddrRegister.ub2 = data; }
        private void PokeDstAddr_3( uint address, byte data ) { dstAddrRegister.ub3 = data; }
        private void PokeSrcAddr_0( uint address, byte data ) { srcAddrRegister.ub0 = data; }
        private void PokeSrcAddr_1( uint address, byte data ) { srcAddrRegister.ub1 = data; }
        private void PokeSrcAddr_2( uint address, byte data ) { srcAddrRegister.ub2 = data; }
        private void PokeSrcAddr_3( uint address, byte data ) { srcAddrRegister.ub3 = data; }

        #endregion

        public void Initialize( uint address )
        {
            base.Initialize( );

            // $40000B0 - 32 - DMA0SAD
            console.Hook( address + 0x0U, PokeSrcAddr_0 );
            console.Hook( address + 0x1U, PokeSrcAddr_1 );
            console.Hook( address + 0x2U, PokeSrcAddr_2 );
            console.Hook( address + 0x3U, PokeSrcAddr_3 );
            // $40000B4 - 32 - DMA0DAD
            console.Hook( address + 0x4U, PokeDstAddr_0 );
            console.Hook( address + 0x5U, PokeDstAddr_1 );
            console.Hook( address + 0x6U, PokeDstAddr_2 );
            console.Hook( address + 0x7U, PokeDstAddr_3 );
            // $40000B8 - 16 - DMA0CNT_L
            console.Hook( address + 0x8U, PokeCounter_0 );
            console.Hook( address + 0x9U, PokeCounter_1 );
            // $40000BA - 16 - DMA0CNT_H
            console.Hook( address + 0xAU, PeekControl_0, PokeControl_0 );
            console.Hook( address + 0xBU, PeekControl_1, PokeControl_1 );
        }
        public void Transfer( )
        {
            dstStep = StepLut[ ( controlRegister.w >> 5 ) & 3U ];
            srcStep = StepLut[ ( controlRegister.w >> 7 ) & 3U ];

            var count = ( counterRegister.w & lenMask );
            var width = ( controlRegister.w & 0x400 ) != 0;

            if ( ( controlRegister.w & 0x3000 ) == 0x3000 )
            {
                dstStep = 0U;
                srcStep = 1U;
                count = 4U;
                width = true;
            }

            count = width ? TransferWord( count ) : TransferHalf( count );

            if ( ( controlRegister.w & 0x0200 ) == 0 )
            {
                controlRegister.w &= 0x7FFF;
            }
            else if ( ( controlRegister.w & 0x0060 ) == 0x0060 )
            {
                dstAddr = dstAddrRegister.ud0;
            }

            if ( ( controlRegister.w & 0x4000 ) != 0 )
                cpu.Interrupt( interruptType );
        }
    }
}