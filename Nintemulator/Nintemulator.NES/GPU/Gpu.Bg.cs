namespace Nintemulator.FC.GPU
{
    public partial class Gpu
    {
        private void FetchBgName( ) { fetch.name = PeekByte( fetch.addr ); }
        private void FetchBgAttr( ) { fetch.attr = ( uint )PeekByte( fetch.addr ) >> ( int )( ( scroll.addr >> 4 & 4u ) | ( scroll.addr & 2u ) ); }
        private void FetchBgBit0( ) { fetch.bit0 = PeekByte( fetch.addr ); }
        private void FetchBgBit1( ) { fetch.bit1 = PeekByte( fetch.addr ); }

        private void PointBgName( )
        {
            fetch.addr = 0x2000u | ( scroll.addr & 0xfffu );
            console.Board.GpuAddressUpdate( fetch.addr );
        }
        private void PointBgAttr( )
        {
            fetch.addr = 0x23c0u | ( scroll.addr & 0xc00u ) | ( scroll.addr >> 4 & 0x38u ) | ( scroll.addr >> 2 & 0x7u );
            console.Board.GpuAddressUpdate( fetch.addr );
        }
        private void PointBgBit0( )
        {
            fetch.addr = bkg.address | ( fetch.name << 4 ) | 0u | ( ( scroll.addr >> 12 ) & 7u );
            console.Board.GpuAddressUpdate( fetch.addr );
        }
        private void PointBgBit1( )
        {
            fetch.addr = bkg.address | ( fetch.name << 4 ) | 8u | ( ( scroll.addr >> 12 ) & 7u );
            console.Board.GpuAddressUpdate( fetch.addr );
        }

        private void SynthesizeBg( )
        {
            var offset = ( hclock + 9 ) % 336;

            for ( int i = 0; i < 8; i++, offset++, fetch.bit0 <<= 1, fetch.bit1 <<= 1 )
                bkg.pixels[ offset ] = 0x3F00U | ( fetch.attr << 2 & 0xC ) | ( fetch.bit0 >> 7 & 0x1 ) | ( fetch.bit1 >> 6 & 0x2 );
        }
    }
}