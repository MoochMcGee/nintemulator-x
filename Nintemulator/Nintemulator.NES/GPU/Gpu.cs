using Nintemulator.FC.PAD;
using Nintemulator.Shared;

namespace Nintemulator.FC.GPU
{
    public partial class Gpu : Famicom.Processor
    {
        public Fetch fetch = new Fetch( );
        public Scroll scroll = new Scroll( );
        public Synthesizer bkg = new Synthesizer( 256 + 16 );
        public Synthesizer spr = new Synthesizer( 256 );
        public bool odd_toggler;
        public uint nmi_enabled;
        public bool spr_overrun;
        public bool spr_zerohit;
        public uint vbl_flag;
        public uint vbl_hold;
        public byte chr;
        public uint hclock;
        public uint vclock;
        public uint clipping;
        public uint emphasis;

        public int[] raster;
        public int[][] screen = Utility.CreateArray<int>( 240, 256 );

        public bool Rendering
        {
            get { return ( bkg.enabled || spr.enabled ) && vclock < 240; }
        }

        public Gpu( Famicom console, Timing.System system )
            : base( console, system )
        {
            Single = system.Ppu;

            vclock = 261u;
            raster = screen[ 0U ];

            EvaluationReset( );
        }

        private byte Peek____( uint address ) { return 0; }
        private byte Peek2002( uint address )
        {
            byte data = 0;

            if ( vbl_flag != 0u ) data |= 0x80;
            if ( spr_zerohit    ) data |= 0x40;
            if ( spr_overrun    ) data |= 0x20;

            vbl_hold = 0u;
            vbl_flag = 0u;
            scroll.swap = false;

            cpu.Nmi( vbl_flag & nmi_enabled );

            return data;
        }
        private byte Peek2004( uint address )
        {
            if ( ( bkg.enabled || spr.enabled ) && vclock < 240 )
                return spr_latch;

            return oam[ oam_address ];
        }
        private byte Peek2007( uint address )
        {
            byte tmp;

            if ( ( scroll.addr & 0x3F00 ) == 0x3F00 )
            {
                tmp = PeekByte( scroll.addr );
                chr = PeekByte( scroll.addr & 0x2FFF );
            }
            else
            {
                tmp = chr;
                chr = PeekByte( scroll.addr );
            }

            if ( Rendering )
            {
                scroll.ClockY( );
            }
            else
            {
                scroll.addr = ( scroll.addr + scroll.step ) & 0x7FFF;
            }

            console.Board.GpuAddressUpdate( scroll.addr );

            return tmp;
        }
        private void Poke____( uint address, byte data ) { }
        private void Poke2000( uint address, byte data )
        {
            scroll.temp = ( scroll.temp & 0x73FF ) | ( uint )( data << 10 & 0x0C00 );
            scroll.step = ( data & 0x04u ) != 0 ? 0x0020U : 0x0001U;
            spr.address = ( data & 0x08u ) != 0 ? 0x1000U : 0x0000U;
            bkg.address = ( data & 0x10u ) != 0 ? 0x1000U : 0x0000U;
            spr.rasters = ( data & 0x20u ) != 0 ? 0x0010U : 0x0008U;
            nmi_enabled = ( data & 0x80u ) >> 7;

            cpu.Nmi( vbl_flag & nmi_enabled );
        }
        private void Poke2001( uint address, byte data )
        {
            bkg.clipped = ( data & 0x02U ) == 0;
            spr.clipped = ( data & 0x04U ) == 0;
            bkg.enabled = ( data & 0x08U ) != 0;
            spr.enabled = ( data & 0x10U ) != 0;

            clipping = ( data & 0x01U ) != 0 ? 0x30U : 0x3FU;
            emphasis = ( data & 0xE0U ) << 1;
        }
        private void Poke2003( uint address, byte data )
        {
            oam_address = data;
            oam_address_latch = data;
        }
        private void Poke2004( uint address, byte data )
        {
            if ( ( oam_address & 0x3 ) == 0x2 )
                data &= 0xE3;

            oam[ oam_address++ ] = data;
        }
        private void Poke2005( uint address, byte data )
        {
            if ( scroll.swap = !scroll.swap )
            {
                scroll.temp = ( scroll.temp & ~0x001FU ) | ( ( data & 0xF8U ) >> 0x3 );
                scroll.fine = ( data & 0x07U );
            }
            else
            {
                scroll.temp = ( scroll.temp & ~0x73E0U ) | ( ( data & 0x07U ) << 0xC ) | ( ( data & 0xF8U ) << 0x2 );
            }
        }
        private void Poke2006( uint address, byte data )
        {
            if ( scroll.swap = !scroll.swap )
            {
                scroll.temp = ( scroll.temp & ~0xFF00U ) | ( ( data & 0x3FU ) << 0x8 );
            }
            else
            {
                scroll.temp = ( scroll.temp & ~0x00FFU ) | ( ( data & 0xFFU ) << 0x0 );
                scroll.addr = ( scroll.temp );

                console.Board.GpuAddressUpdate( scroll.addr );
            }
        }
        private void Poke2007( uint address, byte data )
        {
            PokeByte( scroll.addr, data );

            if ( Rendering )
            {
                scroll.ClockY( );
            }
            else
            {
                scroll.addr = ( scroll.addr + scroll.step ) & 0x7FFF;
            }

            console.Board.GpuAddressUpdate( scroll.addr );
        }

        private void RenderPixel( )
        {
            uint bkgPixel = bkg.pixels[ hclock + scroll.fine ];
            uint sprPixel = spr.pixels[ hclock ];
            uint pixel;

            if ( !bkg.enabled || ( bkg.clipped && hclock < 8 ) )
                bkgPixel = 0x3F00;

            if ( !spr.enabled || ( spr.clipped && hclock < 8 ) || hclock == 255 )
                sprPixel = 0x3F00;

            if ( ( bkgPixel & 0x03 ) == 0 )
            {
                pixel = sprPixel;
            }
            else if ( ( sprPixel & 0x03 ) == 0 )
            {
                pixel = bkgPixel;
            }
            else
            {
                if ( ( sprPixel & 0x8000 ) != 0 )
                    pixel = bkgPixel;
                else
                    pixel = sprPixel;

                if ( ( sprPixel & 0x4000 ) != 0 )
                    spr_zerohit = true;
            }

            raster[ hclock ] = Palette.NTSC[ ( PeekByte( pixel | 0x3F00U ) & clipping ) | emphasis ];
        }

        protected override void OnInitialize( )
        {
            for ( uint i = 0x2000; i <= 0x3FFF; i += 8 )
            {
                cpu.Hook( i + 0, Peek____, Poke2000 );
                cpu.Hook( i + 1, Peek____, Poke2001 );
                cpu.Hook( i + 2, Peek2002, Poke____ );
                cpu.Hook( i + 3, Peek____, Poke2003 );
                cpu.Hook( i + 4, Peek2004, Poke2004 );
                cpu.Hook( i + 5, Peek____, Poke2005 );
                cpu.Hook( i + 6, Peek____, Poke2006 );
                cpu.Hook( i + 7, Peek2007, Poke2007 );
            }

            Poke2000( 0x2000, 0x00 );
            Poke2001( 0x2001, 0x00 );
            //Poke2002(0x2002, 0x00); // $2002: Unimplemented/Invalid
            Poke2003( 0x2003, 0x00 );
            //Poke2004(0x2004, 0x00); // $2004: ORAM Data Port (Writing will modify internal registers in an undesired manner)
            Poke2005( 0x2005, 0x00 );
            Poke2006( 0x2006, 0x00 );
            //Poke2007(0x2007, 0x00); // $2007: VRAM Data Port (Writing will modify internal registers in an undesired manner)

            InitializeMemory( );
            InitializeSprite( );
        }

        public override void Update( )
        {
            if ( bkg.enabled || spr.enabled )
            {
                if ( vclock < 240 )
                {
                    #region V-Active
                    if ( hclock < 256 )
                    {
                        switch ( hclock & 7 )
                        {
                        case 0: PointBgName( ); RenderPixel( ); SpriteEvaluation0( ); break;
                        case 1: FetchBgName( ); RenderPixel( ); SpriteEvaluation1( ); break;
                        case 2: PointBgAttr( ); RenderPixel( ); SpriteEvaluation0( ); break;
                        case 3: FetchBgAttr( ); RenderPixel( ); SpriteEvaluation1( ); break;
                        case 4: PointBgBit0( ); RenderPixel( ); SpriteEvaluation0( ); break;
                        case 5: FetchBgBit0( ); RenderPixel( ); SpriteEvaluation1( ); break;
                        case 6: PointBgBit1( ); RenderPixel( ); SpriteEvaluation0( ); break;
                        case 7: FetchBgBit1( ); RenderPixel( ); SpriteEvaluation1( ); SynthesizeBg( );

                            if ( hclock != 0xFF )
                                scroll.ClockX( );
                            else
                                scroll.ClockY( );

                            break;
                        }

                        if ( hclock == 0x3F ) EvaluationBegin( );
                        if ( hclock == 0xFF ) EvaluationReset( );
                    }
                    else if ( hclock < 320 )
                    {
                        if ( hclock == 0x101 )
                            scroll.ResetX( );

                        switch ( hclock & 7 )
                        {
                        case 0: PointBgName( ); break;
                        case 1: FetchBgName( ); break;
                        case 2: PointBgAttr( ); break;
                        case 3: FetchBgAttr( ); break;
                        case 4: PointSpBit0( ); break;
                        case 5: FetchSpBit0( ); break;
                        case 6: PointSpBit1( ); break;
                        case 7: FetchSpBit1( ); SynthesizeSp( ); break;
                        }
                    }
                    else if ( hclock < 336 )
                    {
                        switch ( hclock & 7 )
                        {
                        case 0: PointBgName( ); break;
                        case 1: FetchBgName( ); break;
                        case 2: PointBgAttr( ); break;
                        case 3: FetchBgAttr( ); break;
                        case 4: PointBgBit0( ); break;
                        case 5: FetchBgBit0( ); break;
                        case 6: PointBgBit1( ); break;
                        case 7: FetchBgBit1( ); SynthesizeBg( ); scroll.ClockX( ); break;
                        }
                    }
                    else if ( hclock < 340 )
                    {
                        switch ( hclock & 1 )
                        {
                        case 0: PointBgName( ); break;
                        case 1: FetchBgName( ); break;
                        }
                    }
                    #endregion
                }
                else if ( vclock == 261 )
                {
                    #region V-Buffer
                    if ( hclock < 256 )
                    {
                        switch ( hclock & 7 )
                        {
                        case 0: console.Board.GpuAddressUpdate( 0x2000 ); break;
                        case 2: console.Board.GpuAddressUpdate( 0x23C0 ); break;
                        case 4: console.Board.GpuAddressUpdate( bkg.address ); break;
                        case 6: console.Board.GpuAddressUpdate( bkg.address ); break;
                        case 7:
                            if ( hclock != 0xFF )
                                scroll.ClockX( );
                            else
                                scroll.ClockY( );
                            break;
                        }

                        bkg.pixels[ hclock ] = 0x0000;
                        spr.pixels[ hclock ] = 0x0000;
                    }
                    else if ( hclock < 320 )
                    {
                        if ( hclock == 0x101 )
                            scroll.ResetX( );

                        switch ( hclock & 7 )
                        {
                        case 0: console.Board.GpuAddressUpdate( 0x2000 ); break;
                        case 2: console.Board.GpuAddressUpdate( 0x23C0 ); break;
                        case 4: console.Board.GpuAddressUpdate( spr.address ); break;
                        case 6: console.Board.GpuAddressUpdate( spr.address ); break;
                        }
                    }
                    else if ( hclock < 336 )
                    {
                        switch ( hclock & 7 )
                        {
                        case 0: PointBgName( ); break;
                        case 1: FetchBgName( ); break;
                        case 2: PointBgAttr( ); break;
                        case 3: FetchBgAttr( ); scroll.ClockX( ); break;
                        case 4: PointBgBit0( ); break;
                        case 5: FetchBgBit0( ); break;
                        case 6: PointBgBit1( ); break;
                        case 7: FetchBgBit1( ); SynthesizeBg( ); break;
                        }
                    }
                    else if ( hclock < 340 )
                    {
                        switch ( hclock )
                        {
                        case 336: console.Board.GpuAddressUpdate( 0x2000 ); break;
                        case 338: console.Board.GpuAddressUpdate( 0x2000 ); break;
                        }
                    }
                    #endregion

                    if ( hclock == 304 )
                        scroll.addr = scroll.temp;

                    if ( hclock == 337 && odd_toggler )
                        tick( );
                }
            }
            else
            {
                if ( vclock < 240 && hclock < 256 )
                {
                    byte color;

                    if ( ( scroll.addr & 0x3F00U ) == 0x3F00U )
                        color = PeekByte( scroll.addr );
                    else
                        color = PeekByte( 0x3F00U );

                    raster[ hclock ] = Palette.NTSC[ ( color & clipping ) | emphasis ];
                }
            }

            tick( );

            if ( hclock == 341 )
            {
                hclock = 0;
                vclock++;

                if ( vclock == 261 )
                {
                    odd_toggler = !odd_toggler;
                }
                else if ( vclock == 262 )
                {
                    vclock = 0;

                    Pad.AutofireState = !Pad.AutofireState;

                    console.Jpa.Update( );
                    console.Jpb.Update( );

                    console.Video.Render( screen );
                }

                if ( vclock < 240 )
                    raster = screen[ vclock ];
            }
        }

        private void tick( )
        {
            if ( vclock == 240 && hclock == 340 ) { vbl_hold = 1; }
            if ( vclock == 241 && hclock ==   0 ) { vbl_flag = vbl_hold; }
            if ( vclock == 241 && hclock ==   2 ) { cpu.Nmi( vbl_flag & nmi_enabled ); }

            if ( vclock == 260 && hclock == 340 ) { spr_overrun = false; spr_zerohit = false; }
            if ( vclock == 260 && hclock == 340 ) { vbl_hold = 0; }
            if ( vclock == 261 && hclock ==   0 ) { vbl_flag = vbl_hold; }
            if ( vclock == 261 && hclock ==   2 ) { cpu.Nmi( vbl_flag & nmi_enabled ); }

            hclock++;
        }

        public class Fetch
        {
            public uint attr;
            public uint bit0;
            public uint bit1;
            public uint name;
            public uint addr;
        }
        public class Scroll
        {
            public bool swap;
            public uint fine;
            public uint step = 1U;
            public uint addr;
            public uint temp;

            public void ClockX( )
            {
                if ( ( addr & 0x001F ) == 0x001F )
                    addr ^= 0x041F;
                else
                    addr += 0x0001;
            }
            public void ClockY( )
            {
                if ( ( addr & 0x7000 ) != 0x7000 )
                {
                    addr += 0x1000;
                }
                else
                {
                    switch ( addr & 0x3E0 )
                    {
                    case 0x3A0: addr ^= 0x7BA0; break;
                    case 0x3E0: addr ^= 0x73E0; break;
                    default: addr += 0x1020; break;
                    }
                }
            }
            public void ResetX( )
            {
                addr = ( addr & ~0x041FU ) | ( temp & 0x041FU );
            }
            public void ResetY( )
            {
                addr = ( addr & ~0x7BE0U ) | ( temp & 0x7BE0U );
            }
        }
        public class Synthesizer
        {
            public bool clipped;
            public bool enabled;
            public uint address;
            public uint rasters = 8U;
            public uint[] pixels;

            public Synthesizer( int capacity )
            {
                pixels = new uint[ capacity ];
            }
        }
    }
}