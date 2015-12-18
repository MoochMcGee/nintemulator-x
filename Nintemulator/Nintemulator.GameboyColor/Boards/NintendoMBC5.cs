namespace Nintemulator.GBC.Boards
{
    public class NintendoMBC5 : Board
    {
        public NintendoMBC5( GameboyColor console, byte[] rom )
            : base( console, rom ) { }

        private void Poke0000( uint address, byte data )
        {
            ramEnabled = ( data == 0x0au );
        }
        private void Poke2000( uint address, byte data )
        {
            romPage &= ~0x3fc000u;
            romPage |= ( data & 0xffu ) << 14;
        }
        private void Poke3000( uint address, byte data )
        {
            romPage &= ~0x400000u;
            romPage |= ( data & 0x01u ) << 22;
        }
        private void Poke4000( uint address, byte data )
        {
            ramPage = ( data & 0x0fu ) << 13;
        }

        protected override uint DecodeRam( uint address )
        {
            return ( address & 0x1fffu ) | ramPage;
        }
        protected override uint DecodeRom( uint address )
        {
            switch ( ( address >> 14 ) & 1u )
            {
            default: return ( address & 0x3fffu );
            case 1u: return ( address & 0x3fffu ) | romPage;
            }
        }
        protected override void OnInitialize( )
        {
            base.OnInitialize( );

            console.Hook( 0x0000U, 0x1fffu, Poke0000 );
            console.Hook( 0x2000U, 0x2fffu, Poke2000 );
            console.Hook( 0x3000U, 0x3fffu, Poke3000 );
            console.Hook( 0x4000U, 0x5fffu, Poke4000 );
        }
        protected override void SetRamSize( byte value )
        {
            switch ( value )
            {
            case 0: ram = null; ramMask = 0x00000U; break;
            case 1: ram = new byte[ 0x02000 ]; ramMask = 0x01fffu; break;
            case 2: ram = new byte[ 0x08000 ]; ramMask = 0x07fffu; break;
            case 3: ram = new byte[ 0x20000 ]; ramMask = 0x1ffffu; break;
            }
        }
    }
}