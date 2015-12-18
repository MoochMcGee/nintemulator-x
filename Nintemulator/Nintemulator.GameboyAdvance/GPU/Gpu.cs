using Nintemulator.GBA.CPU;
using Nintemulator.Shared;
using System;
using word = System.UInt32;

namespace Nintemulator.GBA.GPU
{
    public partial class Gpu : GameboyAdvance.Processor
    {
        private Bg bg0;
        private Bg bg1;
        private Bg bg2;
        private Bg bg3;
        private Sp spr;
        private Window window_0 = new Window( );
        private Window window_1 = new Window( );
        private bool DisplayFrameSelect;
        private bool forcedBlank;
        private bool hblankIntervalFree;
        private bool hblank, hblankIrq;
        private bool vblank, vblankIrq;
        private bool vmatch, vmatchIrq;
        private bool windowObjEnabled;
        private byte vcheck;
        private byte windowObjFlags;
        private byte windowOutFlags;
        private byte[] registers = new byte[ 256 ];
        private ushort hclock;
        private ushort vclock;
        private int bgMode;
        private int eva;
        private int evb;
        private int evy;
        private int blendTarget1;
        private int blendTarget2;
        private int blendType;
        private int[] lastraster;
        private int[] raster;
        private int[][] lastscreen = Utility.CreateArray<int>( 160, 240 );
        private int[][] screen = Utility.CreateArray<int>( 160, 240 );

        static Gpu( )
        {
            for ( int i = 0; i < 32768; i++ )
            {
                int r = ( i << 0x3 ) & 0xF8;
                int g = ( i >> 0x2 ) & 0xF8;
                int b = ( i >> 0x7 ) & 0xF8;

                r |= ( r >> 5 );
                g |= ( g >> 5 );
                b |= ( b >> 5 );

                int colour = ( r << 16 ) | ( g << 8 ) | ( b << 0 );

                ColourLut[ i ] = colour;
            }
        }
        public Gpu( GameboyAdvance console, Timing.System system )
            : base( console, system )
        {
            Single = system.Ppu;

            bg0 = new Bg( console );
            bg1 = new Bg( console );
            bg2 = new Bg( console );
            bg3 = new Bg( console );
            spr = new Sp( console );

            lastraster = lastscreen[ 0 ];
            raster = screen[ 0 ];
        }

        private void EnterHBlank( )
        {
            hblank = true;

            bg2.ClockAffine( );
            bg3.ClockAffine( );

            if ( hblankIrq )
                cpu.Interrupt( Cpu.Source.HBlank );

            if ( vclock < 160 )
            {
                if ( console.Dma0.Enabled && console.Dma0.Type == Dma.Type2 ) console.Dma0.Request = true;
                if ( console.Dma1.Enabled && console.Dma1.Type == Dma.Type2 ) console.Dma1.Request = true;
                if ( console.Dma2.Enabled && console.Dma2.Type == Dma.Type2 ) console.Dma2.Request = true;
                if ( console.Dma3.Enabled && console.Dma3.Type == Dma.Type2 ) console.Dma3.Request = true;
            }
        }
        private void EnterVBlank( )
        {
            vblank = true;

            if ( vblankIrq )
                cpu.Interrupt( Cpu.Source.VBlank );

            if ( console.Dma0.Enabled && console.Dma0.Type == Dma.Type1 ) console.Dma0.Request = true;
            if ( console.Dma1.Enabled && console.Dma1.Type == Dma.Type1 ) console.Dma1.Request = true;
            if ( console.Dma2.Enabled && console.Dma2.Type == Dma.Type1 ) console.Dma2.Request = true;
            if ( console.Dma3.Enabled && console.Dma3.Type == Dma.Type1 ) console.Dma3.Request = true;
        }
        private void LeaveHBlank( )
        {
            hblank = false;
        }
        private void LeaveVBlank( )
        {
            vblank = false;

            bg2.ResetAffine( );
            bg3.ResetAffine( );
            
            Pad.AutofireState = !Pad.AutofireState;

            console.Pad.Update( );
            console.Video.Render( screen );
        }
        private void RenderScanline( )
        {
            if ( forcedBlank )
            {
                // output white, don't render anything
                for ( int i = 0; i < 240; i++ )
                    raster[ i ] = 0xFFFFFF;
            }
            else
            {
                if ( window_0.Enabled || window_1.Enabled || windowObjEnabled )
                {
                    for ( int i = 0; i < 240; i++ )
                    {
                        window[ i ] = windowOutFlags;

                        spr.enable[ i ] = false;
                        spr.raster[ i ] = 0;
                        spr.priority[ i ] = 5;
                    }

                    if ( windowObjEnabled ) RenderSpriteWindow( );
                    if ( window_1.Enabled ) window_1.Calculate( window, vclock );
                    if ( window_0.Enabled ) window_0.Calculate( window, vclock );
                }
                else
                {
                    for ( int i = 0; i < 240; i++ )
                    {
                        window[ i ] = 0xFF;

                        spr.enable[ i ] = false;
                        spr.raster[ i ] = 0;
                        spr.priority[ i ] = 5;
                    }
                }

                switch ( bgMode )
                {
                case 0: this.RenderMode0( ); break;
                case 1: this.RenderMode1( ); break;
                case 2: this.RenderMode2( ); break;
                case 3: this.RenderMode3( ); break;
                case 4: this.RenderMode4( ); break;
                case 5: this.RenderMode5( ); break;
                }
            }
        }

        private byte PeekReg( word address ) { return registers[ address & 0xFF ]; }
        private byte Peek004( word address )
        {
            byte data = registers[ 0x04 ];

            if ( vblank ) data |= 0x01;
            if ( hblank ) data |= 0x02;
            if ( vmatch ) data |= 0x04;

            return data;
        }
        private byte Peek005( word address ) { return vcheck; }
        private byte Peek006( word address ) { return ( byte )( vclock >> 0 ); }
        private byte Peek007( word address ) { return ( byte )( vclock >> 8 ); }

        private void Poke000( word address, byte data )
        {
            registers[ 0x00 ] = data &= 0xFF;

            bgMode = ( data & 0x07 );
            //CgbMode = (data & 0x08) != 0; // gbc mode is really pointless here since we have a gbc emulator in development.
            DisplayFrameSelect = ( data & 0x10 ) != 0;
            hblankIntervalFree = ( data & 0x20 ) != 0;
            spr.mapping = ( data & 0x40 ) != 0;
            forcedBlank = ( data & 0x80 ) != 0;
        }
        private void Poke001( word address, byte data )
        {
            registers[ 0x01 ] = data &= 0xFF;

            bg0.enabled = ( data & 0x01 ) != 0;
            bg1.enabled = ( data & 0x02 ) != 0;
            bg2.enabled = ( data & 0x04 ) != 0;
            bg3.enabled = ( data & 0x08 ) != 0;
            spr.enabled = ( data & 0x10 ) != 0;
            window_0.Enabled = ( data & 0x20 ) != 0;
            window_1.Enabled = ( data & 0x40 ) != 0;
            windowObjEnabled = ( data & 0x80 ) != 0;
        }
        private void Poke002( word address, byte data ) { registers[ 0x02 ] = data &= 0x01; }
        private void Poke003( word address, byte data ) { registers[ 0x03 ] = data &= 0x00; }
        private void Poke004( word address, byte data )
        {
            registers[ 0x04 ] = data &= 0x38;

            vblankIrq = ( data & 0x08 ) != 0;
            hblankIrq = ( data & 0x10 ) != 0;
            vmatchIrq = ( data & 0x20 ) != 0;
        }
        private void Poke005( word address, byte data )
        {
            vcheck = data;
        }
        //           006-04B
        private void Poke04C( word address, byte data )
        {
            Bg.MosaicH = ( data >> 0 ) & 15;
            Bg.MosaicV = ( data >> 4 ) & 15;
        }
        private void Poke04D( word address, byte data )
        {
            Sp.MosaicH = ( data >> 0 ) & 15;
            Sp.MosaicV = ( data >> 4 ) & 15;
        }
        //           04E-04F
        private void Poke050( word address, byte data )
        {
            registers[ 0x50 ] = data &= 0xFF;

            blendTarget1 = ( data & 0x3F );
            blendType = ( data & 0xC0 ) >> 6;
        }
        private void Poke051( word address, byte data )
        {
            registers[ 0x51 ] = data &= 0x3F;

            blendTarget2 = ( data & 0x3F );
        }
        private void Poke052( word address, byte data )
        {
            registers[ 0x52 ] = data;

            this.eva = Math.Min( data & 31, 16 );
        }
        private void Poke053( word address, byte data )
        {
            registers[ 0x53 ] = data;

            this.evb = Math.Min( data & 31, 16 );
        }
        private void Poke054( word address, byte data )
        {
            registers[ 0x54 ] = data;

            this.evy = Math.Min( data & 31, 16 );
        }
        //           055-05F

        protected override void OnInitialize( )
        {
            base.OnInitialize( );

            bg0.Initialize( 0U );
            bg1.Initialize( 1U );
            bg2.Initialize( 2U );
            bg3.Initialize( 3U );

            console.Hook( 0x000, PeekReg, Poke000 );
            console.Hook( 0x001, PeekReg, Poke001 );
            console.Hook( 0x002, PeekReg, Poke002 );
            console.Hook( 0x003, PeekReg, Poke003 );
            console.Hook( 0x004, Peek004, Poke004 );
            console.Hook( 0x005, Peek005, Poke005 );
            // Vertical Counter
            console.Hook( 0x006, Peek006 );
            console.Hook( 0x007, Peek007 );
            // Window Feature
            console.Hook( 0x040, window_0.PokeX2 );
            console.Hook( 0x041, window_0.PokeX1 );
            console.Hook( 0x042, window_1.PokeX2 );
            console.Hook( 0x043, window_1.PokeX1 );
            console.Hook( 0x044, window_0.PokeY2 );
            console.Hook( 0x045, window_0.PokeY1 );
            console.Hook( 0x046, window_1.PokeY2 );
            console.Hook( 0x047, window_1.PokeY1 );
            console.Hook( 0x048, ( a ) => window_0.Flags, ( a, data ) => window_0.Flags = data );
            console.Hook( 0x049, ( a ) => window_1.Flags, ( a, data ) => window_1.Flags = data );
            console.Hook( 0x04A, ( a ) => windowOutFlags, ( a, data ) => windowOutFlags = data );
            console.Hook( 0x04B, ( a ) => windowObjFlags, ( a, data ) => windowObjFlags = data );
            console.Hook( 0x04C, PeekReg, Poke04C );
            console.Hook( 0x04D, PeekReg, Poke04D );
            // 04E - 04F
            console.Hook( 0x050, PeekReg, Poke050 );
            console.Hook( 0x051, PeekReg, Poke051 );
            console.Hook( 0x052, PeekReg, Poke052 );
            console.Hook( 0x053, PeekReg, Poke053 );
            console.Hook( 0x054, PeekReg, Poke054 );
            // 054 - 05F
        }

        public override void Update( )
        {
            hclock++;

            if ( hclock == 240 ) // start of hblank
            {
                if ( vclock < 160 )
                {
                    lastraster = lastscreen[ vclock ];
                    raster = screen[ vclock ];

                    RenderScanline( );
                }

                EnterHBlank( );
            }
            else if ( hclock == 308 ) // end of hblank, next line
            {
                LeaveHBlank( );
                hclock = 0;

                switch ( ++vclock )
                {
                case 160: EnterVBlank( ); break;
                case 228: LeaveVBlank( ); vclock = 0; break;
                }

                vmatch = ( vclock == vcheck );

                if ( vmatch && vmatchIrq )
                    cpu.Interrupt( Cpu.Source.VCheck );
            }
        }
    }
}