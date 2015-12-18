namespace Nintemulator.FC.SPU
{
    public partial class Spu
    {
        public class Envelope
        {
            private int[] regs = new int[ 2 ];

            public bool Reset;
            public byte Count;
            public byte Level;

            private void UpdateLevel( )
            {
                Level = ( byte )( regs[ regs[ 1 ] >> 4 & 1 ] & 0xF );
            }

            public void Clock( )
            {
                if ( Reset )
                {
                    Reset = false;
                    regs[ 0 ] = 0xF;
                }
                else
                {
                    if ( Count != 0 )
                    {
                        Count--;
                        return;
                    }

                    if ( regs[ 0 ] != 0 || ( regs[ 1 ] & 0x20 ) != 0 )
                        regs[ 0 ] = ( regs[ 0 ] - 1 ) & 0xF;
                }

                Count = ( byte )( regs[ 1 ] & 0xF );
                UpdateLevel( );
            }
            public void Write( byte data )
            {
                regs[ 1 ] = data;
                UpdateLevel( );
            }
        }
    }
}