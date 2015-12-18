namespace Nintemulator.SFC.GPU
{
    public partial class Gpu
    {
        private Wn w1 = new Wn( );
        private Wn w2 = new Wn( );

        public sealed class Wn
        {
            public bool dirty;
            public byte l;
            public byte r;
            public uint[] mask_buffer = new uint[ 256 ];

            public void Update( )
            {
                if ( l > r )
                {
                    for ( int i = 0; i < 256; i++ ) mask_buffer[ i ] = 0u;
                }
                else
                {
                    for ( int i = 0; i <   l; i++ ) mask_buffer[ i ] = 0u;
                    for ( int i = l; i <   r; i++ ) mask_buffer[ i ] = 1u;
                    for ( int i = r; i < 256; i++ ) mask_buffer[ i ] = 0u;
                }
            }
        }
    }
}