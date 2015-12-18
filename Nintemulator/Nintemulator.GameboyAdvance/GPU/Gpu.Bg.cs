using Nintemulator.Shared;

namespace Nintemulator.GBA.GPU
{
    public partial class Gpu
    {
        public class Bg : Layer
        {
            public static int MosaicH;
            public static int MosaicV;

            private Register16 controlRegister;
            private Register16 offsetXRegister;
            private Register16 offsetYRegister;
            // Affine Registers (Bg2, Bg3)
            private Register16 paRegister;
            private Register16 pbRegister;
            private Register16 pcRegister;
            private Register16 pdRegister;
            private Register32 rxRegister;
            private Register32 ryRegister;

            public bool Depth;
            public bool Mosaic;
            public bool Wrap;
            public int ChrBase;
            public int NmtBase;
            public new int priority;
            public int Size;
            public int Rx;
            public int Ry;

            public short Scx { get { return ( short )( offsetXRegister.w & 0x1FF ); } }
            public short Scy { get { return ( short )( offsetYRegister.w & 0x1FF ); } }

            public short Dx { get { return ( short )paRegister.w; } }
            public short Dmx { get { return ( short )pbRegister.w; } }
            public short Dy { get { return ( short )pcRegister.w; } }
            public short Dmy { get { return ( short )pdRegister.w; } }

            public Bg( GameboyAdvance console )
                : base( console ) { }

            #region Registers

            private byte PeekControl_0( uint address ) { return controlRegister.l; }
            private byte PeekControl_1( uint address ) { return controlRegister.h; }
            private void PokeControl_0( uint address, byte data )
            {
                controlRegister.l = data &= 0xCF;

                priority = ( data & 0x03 );
                ChrBase = ( data & 0x0C ) >> 2;
                Mosaic = ( data & 0x40 ) != 0;
                Depth = ( data & 0x80 ) != 0;
            }
            private void PokeControl_1( uint address, byte data )
            {
                controlRegister.h = data &= 0xFF;

                NmtBase = ( data & 0x1F );
                Wrap = ( data & 0x20 ) != 0;
                Size = ( data & 0xC0 ) >> 6;
            }
            private void PokeScrollX_0( uint address, byte data ) { offsetXRegister.l = data; }
            private void PokeScrollX_1( uint address, byte data ) { offsetXRegister.h = data &= 0x01; }
            private void PokeScrollY_0( uint address, byte data ) { offsetYRegister.l = data; }
            private void PokeScrollY_1( uint address, byte data ) { offsetYRegister.h = data &= 0x01; }

            // Affine Registers (Bg2, Bg3)
            private void PokePA_0( uint address, byte data ) { paRegister.l = data; }
            private void PokePA_1( uint address, byte data ) { paRegister.h = data; }
            private void PokePB_0( uint address, byte data ) { pbRegister.l = data; }
            private void PokePB_1( uint address, byte data ) { pbRegister.h = data; }
            private void PokePC_0( uint address, byte data ) { pcRegister.l = data; }
            private void PokePC_1( uint address, byte data ) { pcRegister.h = data; }
            private void PokePD_0( uint address, byte data ) { pdRegister.l = data; }
            private void PokePD_1( uint address, byte data ) { pdRegister.h = data; }
            private void PokeRX_0( uint address, byte data ) { rxRegister.ub0 = data; }
            private void PokeRX_1( uint address, byte data ) { rxRegister.ub1 = data; }
            private void PokeRX_2( uint address, byte data ) { rxRegister.ub2 = data; }
            private void PokeRX_3( uint address, byte data ) { rxRegister.ub3 = data; Rx = ( int )rxRegister.ud0; }
            private void PokeRY_0( uint address, byte data ) { ryRegister.ub0 = data; }
            private void PokeRY_1( uint address, byte data ) { ryRegister.ub1 = data; }
            private void PokeRY_2( uint address, byte data ) { ryRegister.ub2 = data; }
            private void PokeRY_3( uint address, byte data ) { ryRegister.ub3 = data; Ry = ( int )ryRegister.ud0; }

            #endregion

            public void Initialize( uint index )
            {
                base.Initialize( );

                this.Index = ( int )index;

                base.console.Hook( 0x008 + ( index * 2U ), PeekControl_0, PokeControl_0 );
                base.console.Hook( 0x009 + ( index * 2U ), PeekControl_1, PokeControl_1 );
                base.console.Hook( 0x010 + ( index * 4U ), /*          */ PokeScrollX_0 );
                base.console.Hook( 0x011 + ( index * 4U ), /*          */ PokeScrollX_1 );
                base.console.Hook( 0x012 + ( index * 4U ), /*          */ PokeScrollY_0 );
                base.console.Hook( 0x013 + ( index * 4U ), /*          */ PokeScrollY_1 );

                if ( index >= 2U )
                {
                    base.console.Hook( 0x020 + ( ( index - 2U ) * 16U ), PokePA_0 );
                    base.console.Hook( 0x021 + ( ( index - 2U ) * 16U ), PokePA_1 );
                    base.console.Hook( 0x022 + ( ( index - 2U ) * 16U ), PokePB_0 );
                    base.console.Hook( 0x023 + ( ( index - 2U ) * 16U ), PokePB_1 );
                    base.console.Hook( 0x024 + ( ( index - 2U ) * 16U ), PokePC_0 );
                    base.console.Hook( 0x025 + ( ( index - 2U ) * 16U ), PokePC_1 );
                    base.console.Hook( 0x026 + ( ( index - 2U ) * 16U ), PokePD_0 );
                    base.console.Hook( 0x027 + ( ( index - 2U ) * 16U ), PokePD_1 );
                    base.console.Hook( 0x028 + ( ( index - 2U ) * 16U ), PokeRX_0 );
                    base.console.Hook( 0x029 + ( ( index - 2U ) * 16U ), PokeRX_1 );
                    base.console.Hook( 0x02A + ( ( index - 2U ) * 16U ), PokeRX_2 );
                    base.console.Hook( 0x02B + ( ( index - 2U ) * 16U ), PokeRX_3 );
                    base.console.Hook( 0x02C + ( ( index - 2U ) * 16U ), PokeRY_0 );
                    base.console.Hook( 0x02D + ( ( index - 2U ) * 16U ), PokeRY_1 );
                    base.console.Hook( 0x02E + ( ( index - 2U ) * 16U ), PokeRY_2 );
                    base.console.Hook( 0x02F + ( ( index - 2U ) * 16U ), PokeRY_3 );
                }
            }

            public void ClockAffine( )
            {
                Rx += Dmx;
                Ry += Dmy;
            }
            public void ResetAffine( )
            {
                Rx = ( int )rxRegister.ud0;
                Ry = ( int )ryRegister.ud0;
            }
        }
    }
}