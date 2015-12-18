using System;
using System.Collections.Generic;

namespace Nintemulator.GBA.SPU
{
    public partial class Spu
    {
        public class ChannelDsd : Channel
        {
            private Dma channel;
            private sbyte[] array;
            private int count;
            private int index;
            private int write;

            public int level;
            public int shift;
            public int timer;

            public ChannelDsd( GameboyAdvance console, Timing timing )
                : base( console, timing )
            {
                array = new sbyte[ 32 ];
            }

            private void PokeFifo( uint address, byte data )
            {
                if ( count < 32 )
                {
                    array[ write ] = ( sbyte )data;
                    write = ( write + 1 ) & 31;
                    count = ( count + 1 );
                }
            }

            public void Initialize( Dma dma, uint address )
            {
                base.Initialize( );

                this.channel = dma;

                base.console.Hook( address + 0U, PokeFifo );
                base.console.Hook( address + 1U, PokeFifo );
                base.console.Hook( address + 2U, PokeFifo );
                base.console.Hook( address + 3U, PokeFifo );
            }

            public void Clear( )
            {
                for ( int i = 0; i < 32; i++ )
                    array[ i ] = 0;

                count = 0;
                index = 0;
                write = 0;
            }
            public void Clock( )
            {
                if ( count > 0 )
                {
                    level = array[ index ] << 1;
                    index = ( index + 1 ) & 31;
                    count = ( count - 1 );
                }

                if ( count > 16 ) return;

                if ( channel.Enabled && channel.Type == Dma.Type3 )
                    channel.Request = true;
            }
        }
    }
}