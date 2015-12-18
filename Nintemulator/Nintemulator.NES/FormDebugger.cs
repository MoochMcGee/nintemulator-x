using Nintemulator.FC.CPU;
using Nintemulator.FC.GPU;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Nintemulator.FC
{
    public partial class FormDebugger : Form
    {
        private Bitmap bgImage;
        private Bitmap spImage;
        private FormGame parent;
        private Cpu cpu;
        private Gpu gpu;

        public FormDebugger( FormGame parent, Famicom console )
        {
            InitializeComponent( );

            this.parent = parent;
            this.cpu = console.Cpu;
            this.gpu = console.Ppu;
        }

        private void buttonRun_Click(object sender, EventArgs e) { }
        private void buttonStep_Click( object sender, EventArgs e )
        {
            cpu.Update( );
            UpdateForm( );

            this.panelDisassembly.Invalidate( );
        }
        private void FormDebugger_Load( object sender, EventArgs e )
        {
            for ( int i = 1; i <= 64; i++ )
            {
                comboBoxSp.Items.Add( "Sprite #" + i );
            }

            bgImage = new Bitmap( 256, 240 );
            spImage = new Bitmap( 120, 240 );

            UpdateForm( );
        }

        #region APU

        #endregion
        #region CPU

        private void panelDisassembly_Paint( object sender, PaintEventArgs e )
        {
            var buffer = new byte[ 3 ];
            var pc = cpu.pc.w;

            for ( int i = 0; i <= 30; i++ )
            {
                var line = string.Format( "[${0:x4}] {1}", pc, CPU.Debugging.Disassembler.Disassemble( ref pc, cpu ) );

                if ( i == 0 )
                {
                    e.Graphics.FillRectangle( Brushes.Black, 0, i * Font.Height, panelDisassembly.Width, Font.Height );
                    e.Graphics.DrawString( line, Font, Brushes.White, 0, i * Font.Height );
                }
                else
                {
                    switch ( i & 1 )
                    {
                    case 0: e.Graphics.FillRectangle( Brushes.SteelBlue, 0, i * Font.Height, panelDisassembly.Width, Font.Height ); break;
                    case 1: e.Graphics.FillRectangle( Brushes.LightSteelBlue, 0, i * Font.Height, panelDisassembly.Width, Font.Height ); break;
                    }

                    e.Graphics.DrawString( line, Font, Brushes.Black, 0, i * Font.Height );
                }
            }
        }

        #endregion
        #region PPU

        private void comboBoxBg_SelectedIndexChanged( object sender, EventArgs e )
        {
            byte[] bg = null;

            switch ( comboBoxBg.SelectedIndex )
            {
            case 0: bg = gpu.nta; break;
            case 1: bg = gpu.ntb; break;
            case 2: bg = gpu.ntc; break;
            case 3: bg = gpu.ntd; break;
            }

            for ( int y = 0; y < 240; y++ )
            {
                for ( int x = 0; x < 256; )
                {
                    uint attr = bg[ 0x3c0 | ( ( y & 0xe0 ) >> 2 ) | ( ( x & 0xe0 ) >> 5 ) ];
                    uint tile = bg[ 0x000 | ( ( y & 0xf8 ) << 2 ) | ( ( x & 0xf8 ) >> 3 ) ];
                    uint bit0 = gpu.PeekByte( gpu.bkg.address | ( tile << 4 ) | 0u | ( ( uint )y & 7u ) );
                    uint bit1 = gpu.PeekByte( gpu.bkg.address | ( tile << 4 ) | 8u | ( ( uint )y & 7u ) );

                    attr = ( attr ) >> ( ( ( y & 0x10 ) >> 2 ) | ( ( x & 0x10 ) >> 3 ) );

                    for ( int i = 0; i < 8; i++, x++ )
                    {
                        var colour = ( ( attr & 0x03u ) << 2 ) | ( ( bit0 & 0x80u ) >> 7 ) | ( ( bit1 & 0x80u ) >> 6 );
                        bit0 <<= 1;
                        bit1 <<= 1;

                        bgImage.SetPixel( x, y, Color.FromArgb( ( 0xff << 24 ) | Palette.NTSC[ gpu.pal[ 0x00 + colour ] ] ) );
                    }
                }
            }

            panelBg.Invalidate( );
        }
        private void comboBoxSp_SelectedIndexChanged( object sender, EventArgs e )
        {
            var sp = comboBoxSp.SelectedIndex << 2;

            uint ypos = gpu.oam[ sp | 0 ];
            uint tile = gpu.oam[ sp | 1 ];
            uint attr = gpu.oam[ sp | 2 ];
            uint xpos = gpu.oam[ sp | 3 ];

            var palette = ( int )( ( attr & 0x03 ) << 2 );

            using ( var graphics = Graphics.FromImage( spImage ) )
            {
                graphics.Clear( Color.Transparent );

                for ( uint y = 0; y < gpu.spr.rasters; y++ )
                {
                    if ( gpu.spr.rasters == 8u )
                    {
                        var bit0 = gpu.PeekByte( gpu.spr.address | ( tile << 4 ) | 0u | ( y & 7 ) );
                        var bit1 = gpu.PeekByte( gpu.spr.address | ( tile << 4 ) | 8u | ( y & 7 ) );

                        for ( uint x = 0; x < 8; x++ )
                        {
                            var colour = ( ( bit0 & 0x80 ) >> 7 ) | ( ( bit1 & 0x80 ) >> 6 );
                            bit0 <<= 1;
                            bit1 <<= 1;

                            if ( colour != 0 )
                            {
                                using ( var brush = new SolidBrush( Color.FromArgb( ( 0xff << 24 ) + Palette.NTSC[ gpu.pal[ 0x10 + palette + colour ] ] ) ) )
                                {
                                    graphics.FillRectangle( brush, x * 15, y * 15, 15, 15 );
                                }
                            }
                        }
                    }
                    else
                    {
                        var bit0 = gpu.PeekByte( ( ( tile & 0x01u ) << 12 ) | ( ( tile & 0xfeu ) << 4 ) | ( ( y & 8u ) << 1 ) | 0u | ( y & 7 ) );
                        var bit1 = gpu.PeekByte( ( ( tile & 0x01u ) << 12 ) | ( ( tile & 0xfeu ) << 4 ) | ( ( y & 8u ) << 1 ) | 8u | ( y & 7 ) );

                        for ( uint x = 0; x < 8; x++ )
                        {
                            var colour = ( ( bit0 & 0x80 ) >> 7 ) | ( ( bit1 & 0x80 ) >> 6 );
                            bit0 <<= 1;
                            bit1 <<= 1;

                            if ( colour != 0 )
                            {
                                using ( var brush = new SolidBrush( Color.FromArgb( ( 0xff << 24 ) + Palette.NTSC[ gpu.pal[ 0x10 + palette + colour ] ] ) ) )
                                {
                                    graphics.FillRectangle( brush, x * 15, y * 15, 15, 15 );
                                }
                            }
                        }
                    }
                }
            }

            labelSpX.Text = string.Format( "X: ${0:x2}", xpos );
            labelSpY.Text = string.Format( "Y: ${0:x2}", ypos );
            labelSpTile.Text = string.Format( "Tile: ${0:x2}", tile );
            labelSpAttr.Text = string.Format( "Attr: ${0:x2}", attr );

            panelSp.Invalidate( );
        }
        private void panelBg_Paint( object sender, PaintEventArgs e ) { e.Graphics.DrawImage( bgImage, 0, 0 ); }
        private void panelSp_Paint( object sender, PaintEventArgs e ) { e.Graphics.DrawImage( spImage, 0, 0 ); }
        private void panelBgPalette_Paint( object sender, PaintEventArgs e )
        {
            for ( int i = 0; i < 16; i++ )
            {
                var x = ( i >> 0 ) & 3;
                var y = ( i >> 2 ) & 3;

                using ( var brush = new SolidBrush( Color.FromArgb( ( 0xff << 24 ) | Palette.NTSC[ gpu.pal[ i + 0x00 ] ] ) ) )
                {
                    e.Graphics.FillRectangle( brush, x * 24, y * 24, 24, 24 );
                }
            }
        }
        private void panelSpPalette_Paint( object sender, PaintEventArgs e )
        {
            for ( int i = 0; i < 16; i++ )
            {
                var x = ( i >> 0 ) & 3;
                var y = ( i >> 2 ) & 3;

                using ( var brush = new SolidBrush( Color.FromArgb( ( 0xff << 24 ) | Palette.NTSC[ gpu.pal[ i + 0x10 ] ] ) ) )
                {
                    e.Graphics.FillRectangle( brush, x * 24, y * 24, 24, 24 );
                }

                if ( x == 0 )
                {
                    e.Graphics.DrawLine( Pens.Red, x * 24, ( y + 1 ) * 24, ( x + 1 ) * 24, y * 24 );
                }
            }
        }

        #endregion

        private void UpdateForm( )
        {
            #region CPU Update
            this.labelA.Text = string.Format( "${0:x2}", cpu.a );
            this.labelX.Text = string.Format( "${0:x2}", cpu.x );
            this.labelY.Text = string.Format( "${0:x2}", cpu.y );

            this.labelPC.Text = string.Format( "${0:x4}", cpu.pc.w );
            this.labelSP.Text = string.Format( "${0:x4}", cpu.sp.w );

            this.checkBoxN.Checked = cpu.p.n.b;
            this.checkBoxV.Checked = cpu.p.v.b;
            this.checkBoxD.Checked = cpu.p.d.b;
            this.checkBoxI.Checked = cpu.p.i.b;
            this.checkBoxZ.Checked = cpu.p.z.b;
            this.checkBoxC.Checked = cpu.p.c.b;

            this.checkBoxNmi.Checked = cpu.interrupts.nmi != 0u;
            this.checkBoxRst.Checked = cpu.interrupts.rst != 0u;
            this.checkBoxIrq.Checked = cpu.interrupts.irq != 0u;
            #endregion

            panelDisassembly.Invalidate( );

            this.labelH.Text = string.Format( "H-Counter: {0, 3}", gpu.hclock );
            this.labelV.Text = string.Format( "V-Counter: {0, 3}", gpu.vclock );

            panelBgPalette.Invalidate( );
            panelSpPalette.Invalidate( );
        }
    }
}