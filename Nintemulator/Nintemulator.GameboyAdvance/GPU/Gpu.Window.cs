using System;

namespace Nintemulator.GBA.GPU
{
    public partial class Gpu
    {
        public class Window
        {
            public bool Enabled;
            public byte Flags;

            public byte X1;
            public byte X2;
            public byte Y1;
            public byte Y2;

            public void PokeX1( uint address, byte data ) { X1 = data; }
            public void PokeX2( uint address, byte data ) { X2 = data; }
            public void PokeY1( uint address, byte data ) { Y1 = data; }
            public void PokeY2( uint address, byte data ) { Y2 = data; }

            public void Calculate( int[] buffer, int vclock )
            {
                if ( Y1 > Y2 ) // edge case behavior
                {
                    if ( vclock < Y1 && vclock >= Y2 )
                        return;
                }
                else
                {
                    if ( vclock < Y1 || vclock >= Y2 )
                        return;
                }

                for ( byte i = X1; i != X2; i++ )
                {
                    if ( i < 240 )
                    {
                        buffer[ i ] = Flags;
                    }
                }
            }
        }
    }
}