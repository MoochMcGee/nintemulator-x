namespace Nintemulator.FC.GPU
{
    public partial class Gpu
    {
        private static byte[] reverseLookup = new byte[]
        { 
            0x00, 0x80, 0x40, 0xc0, 0x20, 0xa0, 0x60, 0xe0, 0x10, 0x90, 0x50, 0xd0, 0x30, 0xb0, 0x70, 0xf0,
            0x08, 0x88, 0x48, 0xc8, 0x28, 0xa8, 0x68, 0xe8, 0x18, 0x98, 0x58, 0xd8, 0x38, 0xb8, 0x78, 0xf8,
            0x04, 0x84, 0x44, 0xc4, 0x24, 0xa4, 0x64, 0xe4, 0x14, 0x94, 0x54, 0xd4, 0x34, 0xb4, 0x74, 0xf4,
            0x0c, 0x8c, 0x4c, 0xcc, 0x2c, 0xac, 0x6c, 0xec, 0x1c, 0x9c, 0x5c, 0xdc, 0x3c, 0xbc, 0x7c, 0xfc,
            0x02, 0x82, 0x42, 0xc2, 0x22, 0xa2, 0x62, 0xe2, 0x12, 0x92, 0x52, 0xd2, 0x32, 0xb2, 0x72, 0xf2,
            0x0a, 0x8a, 0x4a, 0xca, 0x2a, 0xaa, 0x6a, 0xea, 0x1a, 0x9a, 0x5a, 0xda, 0x3a, 0xba, 0x7a, 0xfa,
            0x06, 0x86, 0x46, 0xc6, 0x26, 0xa6, 0x66, 0xe6, 0x16, 0x96, 0x56, 0xd6, 0x36, 0xb6, 0x76, 0xf6,
            0x0e, 0x8e, 0x4e, 0xce, 0x2e, 0xae, 0x6e, 0xee, 0x1e, 0x9e, 0x5e, 0xde, 0x3e, 0xbe, 0x7e, 0xfe,
            0x01, 0x81, 0x41, 0xc1, 0x21, 0xa1, 0x61, 0xe1, 0x11, 0x91, 0x51, 0xd1, 0x31, 0xb1, 0x71, 0xf1,
            0x09, 0x89, 0x49, 0xc9, 0x29, 0xa9, 0x69, 0xe9, 0x19, 0x99, 0x59, 0xd9, 0x39, 0xb9, 0x79, 0xf9,
            0x05, 0x85, 0x45, 0xc5, 0x25, 0xa5, 0x65, 0xe5, 0x15, 0x95, 0x55, 0xd5, 0x35, 0xb5, 0x75, 0xf5,
            0x0d, 0x8d, 0x4d, 0xcd, 0x2d, 0xad, 0x6d, 0xed, 0x1d, 0x9d, 0x5d, 0xdd, 0x3d, 0xbd, 0x7d, 0xfd,
            0x03, 0x83, 0x43, 0xc3, 0x23, 0xa3, 0x63, 0xe3, 0x13, 0x93, 0x53, 0xd3, 0x33, 0xb3, 0x73, 0xf3,
            0x0b, 0x8b, 0x4b, 0xcb, 0x2b, 0xab, 0x6b, 0xeb, 0x1b, 0x9b, 0x5b, 0xdb, 0x3b, 0xbb, 0x7b, 0xfb,
            0x07, 0x87, 0x47, 0xc7, 0x27, 0xa7, 0x67, 0xe7, 0x17, 0x97, 0x57, 0xd7, 0x37, 0xb7, 0x77, 0xf7,
            0x0f, 0x8f, 0x4f, 0xcf, 0x2f, 0xaf, 0x6f, 0xef, 0x1f, 0x9f, 0x5f, 0xdf, 0x3f, 0xbf, 0x7f, 0xff
        };

        private Sprite[] spr_found = new Sprite[ 8u ];
        private byte spr_latch;
        private int spr_count;
        private int spr_index;
        private int spr_phase;

        private void SynthesizeSp( )
        {
            if ( vclock == 261 )
                return;

            var sprite = spr_found[ hclock >> 3 & 7 ];
            var offset = sprite.xpos;

            for ( int i = 0; i < 8 && offset < 256; i++, offset++, fetch.bit0 <<= 1, fetch.bit1 <<= 1 )
            {
                var color = ( fetch.bit0 >> 7 & 1 ) | ( fetch.bit1 >> 6 & 2 );

                if ( ( spr.pixels[ offset ] & 0x03 ) == 0 && color != 0 )
                    spr.pixels[ offset ] = 0x3F10U | ( sprite.attr << 10 & 0xC000 ) | ( sprite.attr << 2 & 12 ) | color;
            }
        }

        private void SpriteEvaluation0( )
        {
            //if (vclock == 261)
            //    return;

            if ( hclock < 64 )
            {
                spr_latch = 0xff;
            }
            else
            {
                spr_latch = oam[ oam_address ];
            }
        }
        private void SpriteEvaluation1( )
        {
            if ( vclock == 261 )
                return;

            if ( hclock < 64 )
            {
                switch ( hclock >> 1 & 3 )
                {
                case 0: spr_found[ ( hclock >> 3 ) & 7 ].ypos = spr_latch; break;
                case 1: spr_found[ ( hclock >> 3 ) & 7 ].name = spr_latch; break;
                case 2: spr_found[ ( hclock >> 3 ) & 7 ].attr = spr_latch & 0xe3u; break;
                case 3: spr_found[ ( hclock >> 3 ) & 7 ].xpos = spr_latch; break;
                }
            }
            else
            {
                switch ( spr_phase )
                {
                case 0:
                    {
                        spr_count++;

                        var raster = ( vclock - spr_latch ) & 0x1ff;

                        if ( raster < spr.rasters )
                        {
                            oam_address++;
                            spr_found[ spr_index ].ypos = spr_latch;
                            spr_phase++;
                        }
                        else
                        {
                            if ( spr_count != 64 )
                            {
                                oam_address += 4;
                            }
                            else
                            {
                                oam_address = 0;
                                spr_phase = 8;
                            }
                        }
                    }
                    break;

                case 1:
                    oam_address++;
                    spr_found[ spr_index ].name = spr_latch;
                    spr_phase++;
                    break;

                case 2:
                    oam_address++;
                    spr_found[ spr_index ].attr = spr_latch & 0xe3u;
                    spr_phase++;

                    if ( spr_count == 1 )
                        spr_found[ spr_index ].attr |= Sprite.SPR_ZERO;
                    break;

                case 3:
                    spr_found[ spr_index ].xpos = spr_latch;
                    spr_index++;

                    if ( spr_count != 64 )
                    {
                        spr_phase = ( spr_index != 8 ? 0 : 4 );
                        oam_address++;
                    }
                    else
                    {
                        spr_phase = 8;
                        oam_address = 0;
                    }
                    break;

                case 4:
                    {
                        var raster = ( vclock - spr_latch ) & 0x1ff;

                        if ( raster < spr.rasters )
                        {
                            spr_overrun = true;
                            spr_phase++;
                            oam_address++;
                        }
                        else
                        {
                            oam_address = ( byte )( ( ( oam_address + 4 ) & 0xFC ) + ( ( oam_address + 1 ) & 0x03 ) );

                            if ( oam_address <= 5 )
                            {
                                spr_phase = 8;
                                oam_address &= 0xFC;
                            }
                        }
                    }
                    break;

                case 5:
                    spr_phase = 6;
                    oam_address++;
                    break;

                case 6:
                    spr_phase = 7;
                    oam_address++;
                    break;

                case 7:
                    spr_phase = 8;
                    oam_address++;
                    break;

                case 8:
                    oam_address += 4;
                    break;
                }
            }
        }

        private void EvaluationBegin( )
        {
            oam_address = 0x0;

            spr_count = 0x0;
            spr_index = 0x0;
            spr_phase = 0x0;
        }
        private void EvaluationReset( )
        {
            EvaluationBegin( );

            for ( int i = 0x0; i < 0x100; i++ )
                spr.pixels[ i ] = 0x3F00;
        }

        private void PointSpBit0( )
        {
            var sprite = spr_found[ hclock >> 3 & 7 ];
            var raster = vclock - sprite.ypos;

            if ( ( sprite.attr & Sprite.V_FLIP ) != 0 )
                raster ^= 0xF;

            if ( spr.rasters == 8 )
            {
                fetch.addr = ( sprite.name << 4 ) | ( raster & 0x7 ) | spr.address;
            }
            else
            {
                sprite.name = ( byte )( ( sprite.name >> 1 ) | ( sprite.name << 7 ) );

                fetch.addr = ( sprite.name << 5 ) | ( raster & 0x7 ) | ( raster << 1 & 0x10 );
            }

            fetch.addr |= 0u;
            console.Board.GpuAddressUpdate( fetch.addr );
        }
        private void PointSpBit1( )
        {
            fetch.addr |= 8u;
            console.Board.GpuAddressUpdate( fetch.addr );
        }
        private void FetchSpBit0( )
        {
            var sprite = spr_found[ hclock >> 3 & 7 ];

            fetch.bit0 = PeekByte( fetch.addr );

            if ( sprite.xpos == 255 || sprite.ypos == 255 )
            {
                fetch.bit0 = 0;
            }
            else if ( ( sprite.attr & Sprite.H_FLIP ) != 0 )
            {
                fetch.bit0 = reverseLookup[ fetch.bit0 ];
            }
        }
        private void FetchSpBit1( )
        {
            var sprite = spr_found[ hclock >> 3 & 7 ];

            fetch.bit1 = PeekByte( fetch.addr );

            if ( sprite.xpos == 255 || sprite.ypos == 255 )
            {
                fetch.bit1 = 0;
            }
            else if ( ( sprite.attr & Sprite.H_FLIP ) != 0 )
            {
                fetch.bit1 = reverseLookup[ fetch.bit1 ];
            }
        }

        private void InitializeSprite( )
        {
            for ( int i = 0; i < 8; i++ )
                spr_found[ i ] = new Sprite( );

            spr_latch = 0;
            spr_count = 0;
            spr_index = 0;
            spr_phase = 0;
        }
        private void ResetSprite( )
        {
            spr_latch = 0;
            spr_count = 0;
            spr_index = 0;
            spr_phase = 0;

            oam_address = 0;
        }

        public class Sprite
        {
            public const int V_FLIP = 0x80;
            public const int H_FLIP = 0x40;
            public const int PRIORITY = 0x20;
            public const int SPR_ZERO = 0x10;

            public uint ypos = 0xFF;
            public uint name = 0xFF;
            public uint attr = 0xE3;
            public uint xpos = 0xFF;
        }
    }
}