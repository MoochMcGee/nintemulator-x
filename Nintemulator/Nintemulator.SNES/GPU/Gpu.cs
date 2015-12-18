using Nintemulator.Shared;

namespace Nintemulator.SFC.GPU
{
    public sealed partial class Gpu : SuperFamicom.Processor
    {
        static uint[][][] priorityLut =
        {
            new uint[][] // mode 0
            {
                new uint[] {  8, 11 },
                new uint[] {  7, 10 },
                new uint[] {  2,  5 },
                new uint[] {  1,  4 },
                new uint[] {  3,  6,  9, 12 }
            },
            new uint[][] // mode 1
            {
                new uint[] { 6, 9 },
                new uint[] { 5, 8 },
                new uint[] { 1, 3 },
                new uint[] { 0, 0 },
                new uint[] { 2, 4, 7, 10 }
            },
            new uint[][]
            {
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0, 0, 0 }
            },
            new uint[][]
            {
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0, 0, 0 }
            },
            new uint[][]
            {
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0, 0, 0 }
            },
            new uint[][]
            {
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0, 0, 0 }
            },
            new uint[][]
            {
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0, 0, 0 }
            },
            new uint[][]
            {
                new uint[] { 2, 3 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 0, 0 },
                new uint[] { 1, 4, 5, 6 }
            },
            new uint[][] // mode 1 priority
            {
                new uint[] { 5, 8 },
                new uint[] { 4, 7 },
                new uint[] { 1, 10 },
                new uint[] { 0, 0 },
                new uint[] { 2, 3, 6, 9 }
            }
        };

        internal Register32 h_latch;
        internal Register32 v_latch;
        internal Register32 product;
        internal bool forceblank;
        internal bool interlace;
        internal bool overscan;
        internal bool pseudohi;
        internal bool[] math_enable = new bool[ 6 ];
        internal byte ppu1_open;
        internal byte ppu2_open;
        internal byte ppu1_stat = 1;
        internal byte ppu2_stat = 2;
        internal byte vram_latch;
        internal uint forcemaintoblack;
        internal uint fixedcolor;
        internal uint brightness;
        internal uint hclock;
        internal uint vclock;
        internal int[] colors = ColourLookup[ 0 ];
        internal int[] raster;
        internal int[][] screen = Utility.CreateArray<int>( 240, 256 );
        private uint colormathenabled;

        static Gpu( )
        {
            for ( int brightness = 0; brightness < 16; brightness++ )
            {
                for ( int colour = 0; colour < 32768; colour++ )
                {
                    var r = ( colour << 3 ) & 0xf8;
                    var g = ( colour >> 2 ) & 0xf8;
                    var b = ( colour >> 7 ) & 0xf8;

                    // apply gradient to lower bits (this will make black=$000000 and white=$ffffff)
                    r |= ( r >> 5 );
                    g |= ( r >> 5 );
                    b |= ( r >> 5 );

                    r = ( r * brightness ) / 0xf;
                    g = ( g * brightness ) / 0xf;
                    b = ( b * brightness ) / 0xf;

                    ColourLookup[ brightness ][ colour ] = ( r << 16 ) | ( g << 8 ) | b;
                }
            }
        }
        public Gpu( SuperFamicom console, Timing.System system )
            : base( console, system )
        {
            Single = system.Ppu;

            raster = screen[ 0U ];

            bg0 = new Bg( console, 0 );
            bg1 = new Bg( console, 1 );
            bg2 = new Bg( console, 2 );
            bg3 = new Bg( console, 3 );
            clr = new Cg( console );
            spr = new Sp( console );
        }

        private void ShowMessage( string register, uint address, byte data )
        {
            //global::System.Windows.Forms.MessageBox.Show( string.Format( "${0:X2}->${1:X4} - {2}", data, address, register ) );
        }

        internal byte Peek2134( uint address )
        {
            product.sd0 = ( short )Bg.M7A.w * ( sbyte )Bg.M7B.h;

            return ppu1_open = product.ub0;
        }
        internal byte Peek2135( uint address )
        {
            product.sd0 = ( short )Bg.M7A.w * ( sbyte )Bg.M7B.h;

            return ppu1_open = product.ub1;
        }
        internal byte Peek2136( uint address )
        {
            product.sd0 = ( short )Bg.M7A.w * ( sbyte )Bg.M7B.h;

            return ppu1_open = product.ub2;
        }
        internal byte Peek2137( uint address )
        {
            if ( cpu.latch_a )
                LatchCounters( );

            return 0;
        }
        internal byte Peek213C( uint address )
        {
            return ( h_latch.ub2 ^= 1 ) != 0 ?
                ( ppu2_open = h_latch.ub0 ) :
                ( ppu2_open = h_latch.ub1 );
        }
        internal byte Peek213D( uint address )
        {
            return ( v_latch.ub2 ^= 1 ) != 0 ?
                ( ppu2_open = v_latch.ub0 ) :
                ( ppu2_open = v_latch.ub1 );
        }
        internal byte Peek213E( uint address )
        {
            return ppu1_stat;
        }
        internal byte Peek213F( uint address )
        {
            byte data = ppu2_stat;

            if ( cpu.latch_a )
            {
                ppu2_stat &= 0xBF; // clear latch flag
            }

            h_latch.ub2 = 0;
            v_latch.ub2 = 0;

            return data;
        }
        internal void Poke2100( uint address, byte data )
        {
            forceblank = ( data & 0x80U ) != 0;
            brightness = ( data & 0x0FU );

            colors = ColourLookup[ brightness ];
        }
        internal void Poke2101( uint address, byte data )
        {
            spr.addr = ( data & 0x07U ) << 13;
            spr.name = ( data & 0x18U ) << 9;
            spr.name += 0x1000;
            spr.size = Sp.size_lut[ ( data & 0xE0U ) >> 5 ];
        }
        internal void Poke2105( uint address, byte data )
        {
            Bg.Mode = ( data & 0x07U );
            Bg.Priority = ( data & 0x08U ) != 0;
            bg0.char_size = ( data & 0x10U ) != 0 ? 16U : 8U;
            bg1.char_size = ( data & 0x20U ) != 0 ? 16U : 8U;
            bg2.char_size = ( data & 0x40U ) != 0 ? 16U : 8U;
            bg3.char_size = ( data & 0x80U ) != 0 ? 16U : 8U;

            var table = priorityLut[ Bg.Mode ];

            if ( Bg.Mode == 1 && Bg.Priority )
                table = priorityLut[ 8 ];

            bg0.priorities = table[ 0 ];
            bg1.priorities = table[ 1 ];
            bg2.priorities = table[ 2 ];
            bg3.priorities = table[ 3 ];
            spr.priorities = table[ 4 ];
        }
        internal void Poke2106( uint address, byte data )
        {
            bg0.mosaic = ( data & 0x01U ) != 0;
            bg1.mosaic = ( data & 0x02U ) != 0;
            bg2.mosaic = ( data & 0x04U ) != 0;
            bg3.mosaic = ( data & 0x08U ) != 0;

            Bg.MosaicSize = ( data & 0xF0U ) >> 4;
        }
        internal void Poke2107( uint address, byte data )
        {
            bg0.name_size = ( data & 0x03U );
            bg0.name_base = ( data & 0x7CU ) << 8;
        }
        internal void Poke2108( uint address, byte data )
        {
            bg1.name_size = ( data & 0x03U );
            bg1.name_base = ( data & 0x7CU ) << 8;
        }
        internal void Poke2109( uint address, byte data )
        {
            bg2.name_size = ( data & 0x03U );
            bg2.name_base = ( data & 0x7CU ) << 8;
        }
        internal void Poke210A( uint address, byte data )
        {
            bg3.name_size = ( data & 0x03U );
            bg3.name_base = ( data & 0x7CU ) << 8;
        }
        internal void Poke210B( uint address, byte data )
        {
            bg0.char_base = ( data & 0x07U ) << 12;
            bg1.char_base = ( data & 0x70U ) << 8;
        }
        internal void Poke210C( uint address, byte data )
        {
            bg2.char_base = ( data & 0x07U ) << 12;
            bg3.char_base = ( data & 0x70U ) << 8;
        }
        internal void Poke210D( uint address, byte data )
        {
            bg0.WriteHOffset( data );

            Bg.M7HOffset.l = Bg.M7Latch;
            Bg.M7HOffset.h = data;
            Bg.M7Latch = data;
        }
        internal void Poke210E( uint address, byte data )
        {
            bg0.WriteVOffset( data );

            Bg.M7VOffset.l = Bg.M7Latch;
            Bg.M7VOffset.h = data;
            Bg.M7Latch = data;
        }
        internal void Poke210F( uint address, byte data ) { bg1.WriteHOffset( data ); }
        internal void Poke2110( uint address, byte data ) { bg1.WriteVOffset( data ); }
        internal void Poke2111( uint address, byte data ) { bg2.WriteHOffset( data ); }
        internal void Poke2112( uint address, byte data ) { bg2.WriteVOffset( data ); }
        internal void Poke2113( uint address, byte data ) { bg3.WriteHOffset( data ); }
        internal void Poke2114( uint address, byte data ) { bg3.WriteVOffset( data ); }
        internal void Poke2115( uint address, byte data )
        {
            vram_ctrl = data;

            switch ( vram_ctrl & 3U )
            {
            default: vram_step = 0x01; break;
            case 1U: vram_step = 0x20; break;
            case 2U: vram_step = 0x80; break;
            case 3U: vram_step = 0x80; break;
            }
        }
        internal void Poke211A( uint address, byte data )
        {
            Bg.M7Control = data;
        }
        internal void Poke211B( uint address, byte data )
        {
            Bg.M7A.l = Bg.M7Latch;
            Bg.M7A.h = data;
            Bg.M7Latch = data;
        }
        internal void Poke211C( uint address, byte data )
        {
            Bg.M7B.l = Bg.M7Latch;
            Bg.M7B.h = data;
            Bg.M7Latch = data;
        }
        internal void Poke211D( uint address, byte data )
        {
            Bg.M7C.l = Bg.M7Latch;
            Bg.M7C.h = data;
            Bg.M7Latch = data;
        }
        internal void Poke211E( uint address, byte data )
        {
            Bg.M7D.l = Bg.M7Latch;
            Bg.M7D.h = data;
            Bg.M7Latch = data;
        }
        internal void Poke211F( uint address, byte data )
        {
            Bg.M7X.l = Bg.M7Latch;
            Bg.M7X.h = data;
            Bg.M7Latch = data;
        }
        internal void Poke2120( uint address, byte data )
        {
            Bg.M7Y.l = Bg.M7Latch;
            Bg.M7Y.h = data;
            Bg.M7Latch = data;
        }
        internal void Poke2123( uint address, byte data )
        {
            bg0.PokeWindow1( data ); data >>= 4;
            bg1.PokeWindow1( data );
        }
        internal void Poke2124( uint address, byte data )
        {
            bg2.PokeWindow1( data ); data >>= 4;
            bg3.PokeWindow1( data );
        }
        internal void Poke2125( uint address, byte data )
        {
            spr.PokeWindow1( data ); data >>= 4;
            clr.PokeWindow1( data );
        }
        internal void Poke2126( uint address, byte data )
        {
            if ( w1.l != data )
            {
                w1.l = data;
                w1.dirty = true;
            }
        }
        internal void Poke2127( uint address, byte data )
        {
            if ( w1.r != data )
            {
                w1.r = data;
                w1.dirty = true;
            }
        }
        internal void Poke2128( uint address, byte data )
        {
            if ( w2.l != data )
            {
                w2.l = data;
                w2.dirty = true;
            }
        }
        internal void Poke2129( uint address, byte data )
        {
            if ( w2.r != data )
            {
                w2.r = data;
                w2.dirty = true;
            }
        }
        internal void Poke212A( uint address, byte data )
        {
            bg0.PokeWindow2( data ); data >>= 2;
            bg1.PokeWindow2( data ); data >>= 2;
            bg2.PokeWindow2( data ); data >>= 2;
            bg3.PokeWindow2( data );
        }
        internal void Poke212B( uint address, byte data )
        {
            spr.PokeWindow2( data ); data >>= 2;
            clr.PokeWindow2( data );
        }
        internal void Poke212C( uint address, byte data )
        {
            bg0.sm = ( data & 0x01U ) != 0 ? ~0u : 0u;
            bg1.sm = ( data & 0x02U ) != 0 ? ~0u : 0u;
            bg2.sm = ( data & 0x04U ) != 0 ? ~0u : 0u;
            bg3.sm = ( data & 0x08U ) != 0 ? ~0u : 0u;
            spr.sm = ( data & 0x10U ) != 0 ? ~0u : 0u;
        }
        internal void Poke212D( uint address, byte data )
        {
            bg0.ss = ( data & 0x01U ) != 0 ? ~0u : 0u;
            bg1.ss = ( data & 0x02U ) != 0 ? ~0u : 0u;
            bg2.ss = ( data & 0x04U ) != 0 ? ~0u : 0u;
            bg3.ss = ( data & 0x08U ) != 0 ? ~0u : 0u;
            spr.ss = ( data & 0x10U ) != 0 ? ~0u : 0u;
        }
        internal void Poke212E( uint address, byte data )
        {
            bg0.wm = ( data & 0x01U ) != 0;
            bg1.wm = ( data & 0x02U ) != 0;
            bg2.wm = ( data & 0x04U ) != 0;
            bg3.wm = ( data & 0x08U ) != 0;
            spr.wm = ( data & 0x10U ) != 0;
        }
        internal void Poke212F( uint address, byte data )
        {
            bg0.ws = ( data & 0x01U ) != 0;
            bg1.ws = ( data & 0x02U ) != 0;
            bg2.ws = ( data & 0x04U ) != 0;
            bg3.ws = ( data & 0x08U ) != 0;
            spr.ws = ( data & 0x10U ) != 0;
        }
        internal void Poke2130( uint address, byte data )
        {
            ShowMessage( "CGWSEL ", 0x2130, data );

            forcemaintoblack = ( data & 0xc0u ) >> 6;
            colormathenabled = ( data & 0x30u ) >> 4;
        }
        internal void Poke2131( uint address, byte data )
        {
            ShowMessage( "CGADSUB", 0x2131, data );

            math_enable[ 0 ] = ( data & 0x01 ) != 0;
            math_enable[ 1 ] = ( data & 0x02 ) != 0;
            math_enable[ 2 ] = ( data & 0x04 ) != 0;
            math_enable[ 3 ] = ( data & 0x08 ) != 0;
            math_enable[ 4 ] = ( data & 0x10 ) != 0;
            math_enable[ 5 ] = ( data & 0x20 ) != 0;
        }
        internal void Poke2132( uint address, byte data )
        {
            if ( ( data & 0x80u ) != 0u ) { fixedcolor = ( fixedcolor & ~0x7c00u ) | ( ( data & 0x1fu ) << 10 ); }
            if ( ( data & 0x40u ) != 0u ) { fixedcolor = ( fixedcolor & ~0x03e0u ) | ( ( data & 0x1fu ) <<  5 ); }
            if ( ( data & 0x20u ) != 0u ) { fixedcolor = ( fixedcolor & ~0x001fu ) | ( ( data & 0x1fu ) <<  0 ); }
        }
        internal void Poke2133( uint address, byte data )
        {
            // todo: mode7 extbg bit
            pseudohi = ( data & 0x08u ) != 0;
            overscan = ( data & 0x04u ) != 0;
            spr.interlace = ( data & 0x02u ) != 0;
            interlace = ( data & 0x01u ) != 0;
        }

        protected sealed override void OnInitialize( )
        {
            base.OnInitialize( );

            bg0.Initialize( );
            bg1.Initialize( );
            bg2.Initialize( );
            bg3.Initialize( );
            clr.Initialize( );
            spr.Initialize( );
        }

        public sealed override void Update( )
        {
            if ( hclock == 0x143 ) { Cycles -= 2; }
            if ( hclock == 0x147 ) { Cycles -= 2; }

            hclock++;

            if ( hclock == 0x001 ) { cpu.LeaveHBlank( ); }
            if ( hclock == 0x112 ) { cpu.EnterHBlank( ); RenderScanline( ); }
            if ( hclock == 0x154 )
            {
                hclock = 0;
                vclock++;

                if ( vclock == ( overscan ? 0x0F1 : 0xE1 ) )
                {
                    cpu.EnterVBlank( );

                    ppu2_stat ^= 0x80; // toggle field flag every vblank
                }

                if ( vclock == 0x105 )
                {
                    vclock = 0;

                    ppu1_stat &= 0x3F; // reset time and range flags

                    cpu.LeaveVBlank( );

                    console.Video.Render( screen );
                }

                if ( vclock < 240 )
                    raster = screen[ vclock ];
            }

            if ( cpu.hv_select != 0 )
            {
                switch ( cpu.hv_select )
                {
                case 0: cpu.irq = false; break;
                case 1: cpu.irq = ( cpu.h_check.w & 0x1FFU ) == hclock; break;
                case 2: cpu.irq = ( cpu.v_check.w & 0x1FFU ) == vclock; break;
                case 3: cpu.irq = ( cpu.h_check.w & 0x1FFU ) == hclock && ( cpu.v_check.w & 0x1FFU ) == vclock; break;
                }

                cpu.reg4211 = cpu.irq ? ( byte )0x80 : ( byte )0x00;
            }
        }

        public void LatchCounters( )
        {
            h_latch.ud0 = hclock;
            v_latch.ud0 = vclock;

            ppu2_stat |= 0x40;
        }
        public void RenderScanline( )
        {
            for ( int i = 0; i < 256; i++ )
            {
                bg0.enable[ i ] = false;
                bg1.enable[ i ] = false;
                bg2.enable[ i ] = false;
                bg3.enable[ i ] = false;
                spr.enable[ i ] = false;
                raster[ i ] = 0;
            }

            if ( forceblank || vclock > ( overscan ? 0xEF : 0xDF ) )
                return;

            if ( w1.dirty )
            {
                w1.dirty = false;
                w1.Update( );

                // invalidate all layers using window 1
                bg0.wn_dirty |= bg0.w1 != 0;
                bg1.wn_dirty |= bg1.w1 != 0;
                bg2.wn_dirty |= bg2.w1 != 0;
                bg3.wn_dirty |= bg3.w1 != 0;
                spr.wn_dirty |= spr.w1 != 0;
                clr.wn_dirty |= clr.w1 != 0;
            }

            if ( w2.dirty )
            {
                w2.dirty = false;
                w2.Update( );

                // invalidate all layers using window 2
                bg0.wn_dirty |= bg0.w2 != 0;
                bg1.wn_dirty |= bg1.w2 != 0;
                bg2.wn_dirty |= bg2.w2 != 0;
                bg3.wn_dirty |= bg3.w2 != 0;
                spr.wn_dirty |= spr.w2 != 0;
                clr.wn_dirty |= clr.w2 != 0;
            }

            if ( bg0.wn_dirty ) { bg0.wn_dirty = false; bg0.UpdateWindow( ); }
            if ( bg1.wn_dirty ) { bg1.wn_dirty = false; bg1.UpdateWindow( ); }
            if ( bg2.wn_dirty ) { bg2.wn_dirty = false; bg2.UpdateWindow( ); }
            if ( bg3.wn_dirty ) { bg3.wn_dirty = false; bg3.UpdateWindow( ); }
            if ( spr.wn_dirty ) { spr.wn_dirty = false; spr.UpdateWindow( ); }
            if ( clr.wn_dirty ) { clr.wn_dirty = false; clr.UpdateWindow( ); }

            switch ( Bg.Mode )
            {
            case 0U: RenderMode0( ); break;
            case 1U: RenderMode1( ); break;
            case 2U: RenderMode2( ); break;
            case 3U: RenderMode3( ); break;
            case 4U: RenderMode4( ); break;
            case 5U: RenderMode5( ); break;
            case 6U: RenderMode6( ); break;
            case 7U: RenderMode7( ); break; // affine render
            }
        }
    }
}