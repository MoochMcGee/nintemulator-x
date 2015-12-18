using Nintemulator.Shared;

namespace Nintemulator.GB
{
    public class Lcd : Gameboy.Component
    {
        private int[][] output = Utility.CreateArray<int>( 144, 160 );

        public Lcd( Gameboy console )
            : base( console ) { }

        public void Output( int[][] screen )
        {
            for ( int v = 0; v < 144; v++ )
            {
                for ( int h = 0; h < 160; h++ )
                {
                    output[ v ][ h ] = screen[ v ][ h ];// *0x010101;
                }
            }
        }
        public void Render( )
        {
            console.Video.Render( output );
        }
    }
}