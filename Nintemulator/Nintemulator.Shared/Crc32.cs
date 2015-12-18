namespace Nintemulator.Shared
{
    public class Crc32
    {
        private readonly static uint[] table = new uint[ 256U ];

        static Crc32( )
        {
            ComputeTable( 0xedb88320u );
        }

        public static uint Compute( byte[] buffer, int start )
        {
            uint hash = 0xffffffffu;

            for ( int i = start; i < buffer.Length; i++ )
            {
                hash = table[ ( hash ^ buffer[ i ] ) & 255U ] ^ ( hash >> 8 );
            }

            return ~hash;
        }
        public static void ComputeTable( uint polynomial )
        {
            for ( uint i = 0; i < 256; i++ )
            {
                uint c = i;

                for ( int j = 0; j < 8; j++ )
                {
                    switch ( c & 1u )
                    {
                    case 0u: c = ( c >> 1 ); break;
                    case 1u: c = ( c >> 1 ) ^ polynomial; break;
                    }
                }

                table[ i ] = c;
            }
        }
    }
}