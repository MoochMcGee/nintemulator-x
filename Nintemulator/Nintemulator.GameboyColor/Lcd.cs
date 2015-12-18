using Nintemulator.Shared;

namespace Nintemulator.GBC
{
    public class Lcd : GameboyColor.Component
    {
        private int[][] output = Utility.CreateArray<int>( 144, 160 );

        public Lcd( GameboyColor console )
            : base( console ) { }

        public void Output( int[][] screen )
        {
            for ( int v = 0; v < 144; v++ )
            {
                for ( int h = 0; h < 160; h++ )
                {
                    output[ v ][ h ] = screen[ v ][ h ];
                }
            }
        }
        public void Render( )
        {
            console.Video.Render( output );
        }
    }
}