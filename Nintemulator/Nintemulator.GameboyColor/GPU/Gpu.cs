using Nintemulator.GBC.CPU;
using Nintemulator.Shared;

namespace Nintemulator.GBC.GPU
{
    public class Gpu : GameboyColor.Processor
    {
        private const int PriorityClr = 0;
        private const int PriorityBkg = 1;
        private const int PrioritySpr = 2;
        private const int PriorityOvr = 3;

        private const int Mode_0 = 204;
        private const int Mode_1 = ( Mode_2 + Mode_3 + Mode_0 ) * 10;
        private const int Mode_2 = 80;
        private const int Mode_3 = 172;

        private const int Sprite_Seq = 0x2;
        private const int Active_Seq = 0x3;
        private const int HBlank_Seq = 0x0;
        private const int VBlank_Seq = 0x1;

        private const int VCheck_Int = 0x40;
        private const int Sprite_Int = 0x20;
        private const int VBlank_Int = 0x10;
        private const int HBlank_Int = 0x08;

        private static readonly byte[] ReverseLookup = new byte[]
        { 
            0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 0x10, 0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0,
            0x08, 0x88, 0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8, 0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8, 0x78, 0xF8,
            0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4, 0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4,
            0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC, 0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C, 0xFC,
            0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2, 0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2,
            0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 0x1A, 0x9A, 0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA,
            0x06, 0x86, 0x46, 0xC6, 0x26, 0xA6, 0x66, 0xE6, 0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6, 0x76, 0xF6,
            0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE, 0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE,
            0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1, 0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1,
            0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9, 0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9,
            0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 0x15, 0x95, 0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5,
            0x0D, 0x8D, 0x4D, 0xCD, 0x2D, 0xAD, 0x6D, 0xED, 0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD, 0x7D, 0xFD,
            0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3, 0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3,
            0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB, 0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB,
            0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7, 0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7,
            0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 0x1F, 0x9F, 0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF
        };

        private static readonly int[] TimingTable =
        {
            Mode_0,
            Mode_1,
            Mode_2,
            Mode_3
        };

        private static int[] PaletteLut = new int[ 32768 ];

        private Register16[][] bg_palettes = Utility.CreateArray<Register16>( 8, 4 );
        private Register16[][] sp_palettes = Utility.CreateArray<Register16>( 8, 4 );
        private uint bg_palette_index, bg_palette_index_step;
        private uint sp_palette_index, sp_palette_index_step;

        private bool bkg_enabled;
        private bool lcd_enabled;
        private bool spr_enabled;
        private bool wnd_enabled;
        private byte scrollx, scrolly;
        private byte windowx, windowy;
        private byte vramPage;
        private int bkg_tile_address = 0x1000;
        private int bkg_name_address = 0x1800;
        private int wnd_name_address = 0x1800;
        private int spr_rasters = 8;
        private int control;
        private int hclock;
        private int vclock;
        private int vcheck;
        private int render_cycles;
        private int sequence = Sprite_Seq;
        private int sequenceDelay = TimingTable[ Sprite_Seq ];
        private int sequenceTimer;
        private int[] priority = new int[ 160 ];
        private int[] raster;
        private int[][] screen = Utility.CreateArray<int>( 144, 160 );

        private byte[] registers = new byte[ 12 ];
        private byte[] oram = new byte[ 160 ];
        private byte[][] vram = Utility.CreateArray<byte>( 2, 0x2000 );

        static Gpu( )
        {
            for ( int i = 0; i < PaletteLut.Length; i++ )
            {
                int r = ( i & 0x001f ) >>  0;
                int g = ( i & 0x03e0 ) >>  5;
                int b = ( i & 0x7c00 ) >> 10;

                int rp = ( ( r * 13 ) + ( g * 2 ) + ( b *  1 ) ) >> 1;
                int gp = ( ( r *  0 ) + ( g * 3 ) + ( b *  1 ) ) << 1;
                int bp = ( ( r *  3 ) + ( g * 2 ) + ( b * 11 ) ) >> 1;

                PaletteLut[ i ] = ( rp << 16 ) | ( gp << 8 ) | ( bp );
            }
        }
        public Gpu( GameboyColor console, Timing.System system )
            : base( console, system )
        {
            Single = system.Ppu;
        }

        #region Registers
        private byte PeekFF40( uint address ) { return registers[ 0 ]; }
        private byte PeekFF41( uint address )
        {
            return ( byte )( control | ( vclock == vcheck ? 0x04 : 0x00 ) | ( sequence & 0x03 ) | 0x80 );
        }
        private byte PeekFF42( uint address ) { return ( byte )( 0x00 | scrolly ); }
        private byte PeekFF43( uint address ) { return ( byte )( 0x00 | scrollx ); }
        private byte PeekFF44( uint address ) { return ( byte )( 0x00 | vclock ); }
        private byte PeekFF45( uint address ) { return ( byte )( 0x00 | vcheck ); }
        private byte PeekFF46( uint address ) { return registers[ 6 ]; }
        private byte PeekFF47( uint address ) { return registers[ 7 ]; }
        private byte PeekFF48( uint address ) { return registers[ 8 ]; }
        private byte PeekFF49( uint address ) { return registers[ 9 ]; }
        private byte PeekFF4A( uint address ) { return ( byte )( 0x00 | windowy ); }
        private byte PeekFF4B( uint address ) { return ( byte )( 0x00 | windowx ); }
        private byte PeekFF4F( uint address ) { return ( byte )( 0xfe | vramPage ); }
        private byte PeekFF68( uint address )
        {
            return ( byte )( ( bg_palette_index_step << 7 ) | 0x40 | bg_palette_index );
        }
        private byte PeekFF69( uint address )
        {
            var index = bg_palette_index >> 1;

            switch ( bg_palette_index & 1U )
            {
            default:
            case 0U: return bg_palettes[ index >> 2 ][ index & 0x03U ].l;
            case 1U: return bg_palettes[ index >> 2 ][ index & 0x03U ].h;
            }
        }
        private byte PeekFF6A( uint address )
        {
            return ( byte )( ( sp_palette_index_step << 7 ) | 0x40 | sp_palette_index );
        }
        private byte PeekFF6B( uint address )
        {
            var index = sp_palette_index >> 1;

            switch ( sp_palette_index & 1U )
            {
            default:
            case 0U: return sp_palettes[ index >> 2 ][ index & 0x03U ].l;
            case 1U: return sp_palettes[ index >> 2 ][ index & 0x03U ].h;
            }
        }
        private void PokeFF40( uint address, byte data )
        {
            if ( ( registers[ 0 ] & 0x80 ) < ( data & 0x80 ) )
            {
                // lcd turning on
                hclock = 4;
                vclock = 0;

                sequence = Sprite_Seq;
                sequenceTimer = 4;
                sequenceDelay = TimingTable[ Sprite_Seq ];
            }

            registers[ 0 ] = data;

            lcd_enabled = ( data & 0x80 ) != 0; // Bit 7 - LCD Display Enable - (0=Off, 1=On)
            wnd_enabled = ( data & 0x20 ) != 0; // Bit 5 - Wnd Display Enable - (0=Off, 1=On)
            spr_rasters = ( data & 0x04 ) != 0 ? 16 : 8;
            spr_enabled = ( data & 0x02 ) != 0; // Bit 1 - Spr Display Enable - (0=Off, 1=On)
            bkg_enabled = ( data & 0x01 ) != 0; // Bit 0 - Bkg Display Enable - (0=Off, 1=On)

            wnd_name_address = 0x1800 | ( ( data & 0x40 ) << 4 ); // Bit 6 - Wnd Tile Map Display Select    (0=9800-9BFF, 1=9C00-9FFF)
            bkg_tile_address = 0x1000 ^ ( ( data & 0x10 ) << 8 ); // Bit 4 - Bkg & Wnd Tile Data Select     (0=8800-97FF, 1=8000-8FFF)
            bkg_name_address = 0x1800 | ( ( data & 0x08 ) << 7 ); // Bit 3 - Bkg Tile Map Display Select    (0=9800-9BFF, 1=9C00-9FFF)
        }
        private void PokeFF41( uint address, byte data )
        {
            control = ( data & 0x78 );

            if ( vclock == vcheck && ( control & VCheck_Int ) != 0 )
                cpu.RequestInterrupt( Cpu.Interrupt.Status );
        }
        private void PokeFF42( uint address, byte data ) { scrolly = data; }
        private void PokeFF43( uint address, byte data ) { scrollx = data; }
        private void PokeFF44( uint address, byte data )
        {
            hclock = 0x00;
            vclock = 0x00;

            ChangeSequence( Sprite_Seq );

            sequenceTimer = 0;
            sequenceDelay = TimingTable[ sequence ];
        }
        private void PokeFF45( uint address, byte data )
        {
            vcheck = data;

            if ( vclock == vcheck && ( control & VCheck_Int ) != 0 )
                cpu.RequestInterrupt( Cpu.Interrupt.Status );
        }
        private void PokeFF46( uint address, byte data )
        {
            registers[ 6 ] = data;

            uint addr = ( uint )( data << 8 );

            for ( int i = 0; i < 160; i++ )
                oram[ i ] = console.PeekByteFree( addr++ );
        }
        private void PokeFF47( uint address, byte data ) { registers[ 7 ] = data; }
        private void PokeFF48( uint address, byte data ) { registers[ 8 ] = data; }
        private void PokeFF49( uint address, byte data ) { registers[ 9 ] = data; }
        private void PokeFF4A( uint address, byte data ) { windowy = data; }
        private void PokeFF4B( uint address, byte data ) { windowx = data; }
        private void PokeFF4F( uint address, byte data )
        {
            vramPage = data &= 1;
        }
        private void PokeFF68( uint address, byte data )
        {
            bg_palette_index_step = ( data & 0x80U ) >> 7;
            bg_palette_index = ( data & 0x3FU );
        }
        private void PokeFF69( uint address, byte data )
        {
            var index = bg_palette_index >> 1;

            switch ( bg_palette_index & 0x01U )
            {
            default:
            case 0U: bg_palettes[ index >> 2 ][ index & 0x03U ].l = data &= 0xFF; break;
            case 1U: bg_palettes[ index >> 2 ][ index & 0x03U ].h = data &= 0x7F; break;
            }

            bg_palette_index = ( bg_palette_index + bg_palette_index_step ) & 0x3FU;
        }
        private void PokeFF6A( uint address, byte data )
        {
            sp_palette_index_step = ( data & 0x80U ) >> 7;
            sp_palette_index = ( data & 0x3FU );
        }
        private void PokeFF6B( uint address, byte data )
        {
            var index = sp_palette_index >> 1;

            switch ( sp_palette_index & 0x01U )
            {
            default:
            case 0U: sp_palettes[ index >> 2 ][ index & 0x03U ].l = data &= 0xFF; break;
            case 1U: sp_palettes[ index >> 2 ][ index & 0x03U ].h = data &= 0x7F; break;
            }

            sp_palette_index = ( sp_palette_index + sp_palette_index_step ) & 0x3FU;
        }
        #endregion
        
        private void ChangeSequence( int sequence )
        {
            if ( this.sequence == sequence )
                return;

            switch ( sequence )
            {
            case HBlank_Seq:

                if ( ( control & HBlank_Int ) != 0 )
                    cpu.RequestInterrupt( Cpu.Interrupt.Status );

                if ( vclock < 144 )
                    cpu.EnterHBlank( );

                break;

            case VBlank_Seq:

                if ( ( control & VBlank_Int ) != 0 )
                    cpu.RequestInterrupt( Cpu.Interrupt.Status );

                break;

            case Sprite_Seq:

                if ( ( control & Sprite_Int ) != 0 )
                    cpu.RequestInterrupt( Cpu.Interrupt.Status );

                break;
            }

            this.sequence = sequence;
        }
        private void ClockSequencer( )
        {
            sequenceTimer += Single;

            if ( sequenceTimer == sequenceDelay )
            {
                // the sections labelled below represent the END of that section
                // example, when the case label for "Sprite" is hit, the controller is technically at the beginning of "Active"
                switch ( sequence )
                {
                case HBlank_Seq: ChangeSequence( vclock == 144 ? VBlank_Seq : Sprite_Seq ); break;
                case VBlank_Seq: ChangeSequence( Sprite_Seq ); break;
                case Sprite_Seq: ChangeSequence( Active_Seq ); break;
                case Active_Seq: ChangeSequence( HBlank_Seq ); break;
                }

                sequenceTimer = 0;
                sequenceDelay = TimingTable[ sequence ];
            }
        }
        private void RenderScanline( )
        {
            RenderBkg( );

            if ( spr_enabled )
                RenderSpr( );
        }
        private void RenderBkg( )
        {
            int xPos = ( byte )( scrollx );
            int yPos = ( byte )( scrolly + vclock );
            int fine = xPos & 7;

            int ntaddr = bkg_name_address | ( ( yPos & ~7 ) << 2 ) | ( ( xPos & ~7 ) >> 3 );
            int wndcmp = ( vclock - windowy );
            int px = 0;

            for ( int tx = 0; tx < 21; tx++ )
            {
                if ( wnd_enabled && tx == ( windowx / 8 ) && wndcmp >= 0 )
                {
                    xPos = windowx;
                    yPos = wndcmp;
                    fine = 0;

                    // restart rendering engine for window
                    ntaddr = wnd_name_address | ( ( yPos & ~7 ) << 2 );
                }

                var name = vram[ 0u ][ ntaddr ];
                var attr = vram[ 1u ][ ntaddr ];

                var chaddr = bkg_tile_address | ( name << 4 ) | ( ( yPos & 7 ) << 1 );

                if ( ( name & 0x80 ) != 0 ) chaddr ^= bkg_tile_address;
                if ( ( attr & 0x40 ) != 0 ) chaddr ^= 0xe;

                var bit0 = vram[ ( attr >> 3 ) & 1 ][ chaddr | 0 ];
                var bit1 = vram[ ( attr >> 3 ) & 1 ][ chaddr | 1 ];

                if ( ( attr & 0x20 ) != 0 )
                {
                    bit0 = ReverseLookup[ bit0 ];
                    bit1 = ReverseLookup[ bit1 ];
                }

                var palette = bg_palettes[ attr & 7 ];

                for ( int i = 0; i < 8; i++ )
                {
                    var color = ( ( bit0 >> 7 ) & 1 ) | ( ( bit1 >> 6 ) & 2 );
                    bit0 <<= 1;
                    bit1 <<= 1;

                    if ( fine != 0 )
                    {
                        fine--; // stall lcd causing pixels to drop out
                        continue;
                    }

                    if ( px < 160 )
                    {
                        priority[ px ] = ( attr & 0x80 ) != 0 ? PriorityOvr : ( color != 0 ) ? PriorityBkg : PriorityClr;
                        raster[ px++ ] = PaletteLut[ palette[ color ].w ];
                    }
                }

                if ( px >= 160 )
                    break;

                // align nametable pointer to next tile
                ntaddr = ( ntaddr & 0xffe0 ) | ( ( ntaddr + 1 ) & 0x1f );
            }
        }
        private void RenderSpr( )
        {
            int count = 0;

            for ( int i = 0; i < 160 && count < 10; i += 4 )
            {
                var yPos = oram[ 0x00 + i ] - 16;
                var xPos = oram[ 0x01 + i ] - 8;
                var name = oram[ 0x02 + i ];
                var attr = oram[ 0x03 + i ];

                var line = ( vclock - yPos ) & 0xFFFF;

                if ( line < spr_rasters )
                {
                    count++;

                    if ( ( attr & 0x40 ) != 0 )
                        line ^= 0x0F;

                    if ( spr_rasters == 16 )
                    {
                        name &= 0xFE;

                        if ( line >= 8 )
                            name |= 0x01;
                    }

                    var addr = ( name << 4 ) | ( line << 1 & 0x000E );
                    var bit0 = vram[ ( attr >> 3 ) & 1 ][ addr | 0 ];
                    var bit1 = vram[ ( attr >> 3 ) & 1 ][ addr | 1 ];

                    if ( ( attr & 0x20 ) != 0 )
                    {
                        bit0 = ReverseLookup[ bit0 ];
                        bit1 = ReverseLookup[ bit1 ];
                    }

                    var palette = sp_palettes[ attr & 0x07 ];

                    for ( int x = 0; x < 8 && xPos < 160; x++, xPos++, bit0 <<= 1, bit1 <<= 1 )
                    {
                        if ( xPos < 0 || priority[ xPos ] >= PrioritySpr )
                            continue;

                        var color = ( bit0 >> 7 & 0x1 ) | ( bit1 >> 6 & 0x2 );

                        if ( color != 0 )
                        {
                            if ( ( attr & 0x80 ) != 0 )
                            {
                                if ( priority[ xPos ] == PriorityClr )
                                {
                                    priority[ xPos ] = PrioritySpr;
                                    raster[ xPos ] = PaletteLut[ palette[ color ].w ];
                                }
                            }
                            else
                            {
                                priority[ xPos ] = PrioritySpr;
                                raster[ xPos ] = PaletteLut[ palette[ color ].w ];
                            }
                        }
                    }
                }
            }
        }

        protected override void OnInitialize( )
        {
            base.OnInitialize( );

            console.Hook( 0x8000U, 0x9FFFU, PeekVRam, PokeVRam );
            console.Hook( 0xFE00U, 0xFE9FU, PeekORam, PokeORam );

            console.Hook( 0xff40u, PeekFF40, PokeFF40 );
            console.Hook( 0xff41u, PeekFF41, PokeFF41 );
            console.Hook( 0xff42u, PeekFF42, PokeFF42 );
            console.Hook( 0xff43u, PeekFF43, PokeFF43 );
            console.Hook( 0xff44u, PeekFF44, PokeFF44 );
            console.Hook( 0xff45u, PeekFF45, PokeFF45 );
            console.Hook( 0xff46u, PeekFF46, PokeFF46 );
            console.Hook( 0xff47u, PeekFF47, PokeFF47 );
            console.Hook( 0xff48u, PeekFF48, PokeFF48 );
            console.Hook( 0xff49u, PeekFF49, PokeFF49 );
            console.Hook( 0xff4au, PeekFF4A, PokeFF4A );
            console.Hook( 0xff4bu, PeekFF4B, PokeFF4B );
            // ff4c
            // ff4d
            // ff4e
            console.Hook( 0xff4fu, PeekFF4F, PokeFF4F );

            console.Hook( 0xff68u, PeekFF68, PokeFF68 );
            console.Hook( 0xff69u, PeekFF69, PokeFF69 );
            console.Hook( 0xff6au, PeekFF6A, PokeFF6A );
            console.Hook( 0xff6bu, PeekFF6B, PokeFF6B );
        }

        public override void Update( )
        {
            hclock++;
            render_cycles++;

            if ( lcd_enabled )
            {
                if ( hclock == 252 && vclock < 144 )
                {
                    raster = screen[ vclock ];

                    if ( lcd_enabled )
                    {
                        RenderScanline( );
                    }
                    else
                    {
                        for ( int i = 0; i < 160; i++ )
                        {
                            raster[ i ] = PaletteLut[ 0x7FFF ];
                        }
                    }
                }

                if ( hclock == 456 )
                {
                    hclock = 0;
                    vclock++;

                    if ( vclock == 144 )
                        cpu.RequestInterrupt( Cpu.Interrupt.VBlank );

                    if ( vclock == 154 )
                    {
                        vclock = 0;
                        console.Lcd.Output( screen );
                    }

                    if ( vclock == vcheck && ( control & VCheck_Int ) != 0 )
                        cpu.RequestInterrupt( Cpu.Interrupt.Status );
                }

                ClockSequencer( );
            }

            if ( render_cycles == 70224 )
            {
                render_cycles = 0;
                console.Lcd.Render( );
            }
        }

        public byte PeekORam( uint address )
        {
            if ( lcd_enabled && sequence >= Sprite_Seq )
                return 0xff;

            return oram[ address ^ 0xFE00 ];
        }
        public byte PeekVRam( uint address )
        {
            if ( lcd_enabled && sequence == Active_Seq )
                return 0xff;

            return vram[ vramPage ][ address & 0x1FFF ];
        }
        public void PokeORam( uint address, byte data )
        {
            if ( lcd_enabled && sequence >= Sprite_Seq )
                return;

            oram[ address ^ 0xFE00 ] = data;
        }
        public void PokeVRam( uint address, byte data )
        {
            if ( lcd_enabled && sequence == Active_Seq )
                return;

            vram[ vramPage ][ address & 0x1FFF ] = data;
        }
    }
}