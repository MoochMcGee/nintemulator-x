using Nintemulator.SFC.CPU;
using Nintemulator.SFC.GPU;
using Nintemulator.SFC.SPU;
using Nintemulator.Shared;
using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace Nintemulator.SFC
{
    public partial class FormDebugger : Form
    {
        private SuperFamicom sfc;
        private Cpu cpu;
        private Gpu gpu;
        private Spu spu;
        private uint lastpc;

        public FormDebugger( SuperFamicom console )
        {
            InitializeComponent( );
            this.sfc = console;
            this.cpu = console.Cpu;
            this.gpu = console.Ppu;
            this.spu = console.Apu;
        }

        private void buttonStep_Click( object sender, EventArgs e )
        {
            do
            {
                cpu.Update( );
            }
            while ( lastpc == cpu.pc.w );

            lastpc = cpu.pc.w; // skip passed mvn/mvp and other deadlocks

            UpdateForm( );

            this.panelCpuDisassembly.Invalidate( );
            this.panelSpuDisassembly.Invalidate( );
        }

        private void UpdateForm( )
        {
            #region CPU Update
            this.labelCpuA.Text = string.Format( "${0:x4}", cpu.regs.a );
            this.labelCpuX.Text = string.Format( "${0:x4}", cpu.regs.x );
            this.labelCpuY.Text = string.Format( "${0:x4}", cpu.regs.y );

            this.labelCpuPC.Text = string.Format( "${0:x4}", cpu.pc.w );
            this.labelCpuSP.Text = string.Format( "${0:x4}", cpu.regs.sp );
            this.labelCpuDP.Text = string.Format( "${0:x4}", cpu.regs.d );
            this.labelCpuDB.Text = string.Format( "${0:x2}", cpu.regs.db );
            this.labelCpuPB.Text = string.Format( "${0:x2}", cpu.pc.b );

            this.checkBoxCpuE.Checked = cpu.flags.e.b;
            this.checkBoxCpuN.Checked = cpu.flags.n.b;
            this.checkBoxCpuV.Checked = cpu.flags.v.b;
            this.checkBoxCpuM.Checked = cpu.flags.m.b;
            this.checkBoxCpuX.Checked = cpu.flags.x.b;
            this.checkBoxCpuD.Checked = cpu.flags.d.b;
            this.checkBoxCpuI.Checked = cpu.flags.i.b;
            this.checkBoxCpuZ.Checked = cpu.flags.z.b;
            this.checkBoxCpuC.Checked = cpu.flags.c.b;
            #endregion
            #region SPU Update
            //this.labelSpuA.Text = string.Format( "${0:x2}", spu.registers.a );
            //this.labelSpuX.Text = string.Format( "${0:x2}", spu.registers.x );
            //this.labelSpuY.Text = string.Format( "${0:x2}", spu.registers.y );

            //this.labelSpuSp.Text = string.Format( "${0:x4}", spu.sp.w );
            //this.labelSpuPc.Text = string.Format( "${0:x4}", spu.pc.w );

            //this.checkBoxSpuN.Checked = spu.sr.n != 0;
            //this.checkBoxSpuV.Checked = spu.sr.v != 0;
            //this.checkBoxSpuP.Checked = spu.sr.p != 0;
            //this.checkBoxSpuB.Checked = spu.sr.b != 0;
            //this.checkBoxSpuH.Checked = spu.sr.h != 0;
            //this.checkBoxSpuI.Checked = spu.sr.i != 0;
            //this.checkBoxSpuZ.Checked = spu.sr.z;
            //this.checkBoxSpuC.Checked = spu.sr.c != 0;
            #endregion

            this.labelGpuHCounter.Text = string.Format( "H-Counter: {0, 3}", gpu.hclock );
            this.labelGpuVCounter.Text = string.Format( "V-Counter: {0, 3}", gpu.vclock );
        }

        private void panelCpuDisassembly_Paint( object sender, PaintEventArgs e )
        {
            byte[] buffer = new byte[ 4 ];

            bool fm = cpu.flags.m.b;
            bool fx = cpu.flags.x.b;
            uint pc = cpu.pc.d;

            Brush fColor;

            for ( int i = 0; i < 28; i++ )
            {
                buffer[ 0 ] = cpu.PeekBusA( pc + 0U );
                buffer[ 1 ] = cpu.PeekBusA( pc + 1U );
                buffer[ 2 ] = cpu.PeekBusA( pc + 2U );
                buffer[ 3 ] = cpu.PeekBusA( pc + 3U );

                if ( pc == cpu.pc.d )
                {
                    e.Graphics.FillRectangle( Brushes.Black, 0, i * Font.Height, panelCpuDisassembly.Width, Font.Height );
                    fColor = Brushes.White;
                }
                else
                {
                    fColor = Brushes.Black;
                }

                var line = string.Format( "[${0:x6}] {1}", pc, CPU.Debugging.Disassembler.Disassemble( ref pc, ref fm, ref fx, buffer ) );
                e.Graphics.DrawString( line, Font, fColor, 0, i * Font.Height );
            }
        }
        private void panelSpuDisassembly_Paint( object sender, PaintEventArgs e )
        {
            //byte[] buffer = new byte[ 3 ];

            //bool fm = cpu.flags.m.b;
            //bool fx = cpu.flags.x.b;
            //uint pc = spu.pc.w;

            //Brush fColor;

            //for ( int i = 0; i < 28; i++ )
            //{
            //    buffer[ 0 ] = spu.Peek( pc + 0U );
            //    buffer[ 1 ] = spu.Peek( pc + 1U );
            //    buffer[ 2 ] = spu.Peek( pc + 2U );

            //    if ( pc == spu.pc.w )
            //    {
            //        e.Graphics.FillRectangle( Brushes.Black, 0, i * Font.Height, panelCpuDisassembly.Width, Font.Height );
            //        fColor = Brushes.White;
            //    }
            //    else
            //    {
            //        fColor = Brushes.Black;
            //    }

            //    var line = string.Format( "[${0:x4}] {1}", pc, SPU.Debugging.Disassembler.Disassemble( ref pc, buffer ) );
            //    e.Graphics.DrawString( line, Font, fColor, 0, i * Font.Height );
            //}
        }

        private void FormDebugger_Load( object sender, EventArgs e )
        {
            UpdateForm( );
        }

        private Image RenderBg( Gpu.Bg bg )
        {
            var bitmap = new Bitmap(
                256 + ( int )( ( bg.name_size & 1 ) << 8 ),
                256 + ( int )( ( bg.name_size & 2 ) << 7 ) );

            for ( uint y = 0; y < bitmap.Height; y++ )
            {
                for ( uint x = 0; x < bitmap.Width / 8; x++ )
                {
                    var ytile = y >> 3;
                    var xtile = x;

                    var name_address = bg.name_base +
                        ( ( ytile & 31U ) << 5 ) +
                        ( ( xtile & 31U ) << 0 );

                    if ( bitmap.Width == 512 )
                    {
                        name_address += ( x & 0x100 ) << 2;

                        if ( bitmap.Height == 512 )
                            name_address += ( y & 0x100 ) << 3;
                    }
                    else if ( bitmap.Height == 512 )
                    {
                        name_address += ( y & 0x100 ) << 2;
                    }

                    var name = gpu.vram[ name_address & 0x7fffu ].w;

                    var char_address = bg.char_base + ( ( name & 0x3FFU ) << 3 ) + ( y & 7 );

                    if ( ( name & 0x8000U ) != 0U )
                    {
                        char_address ^= 0x0007U; // vflip
                    }

                    var bits = gpu.vram[ char_address & 0x7fffu ];
                    byte bit0;
                    byte bit1;

                    if ( ( name & 0x4000U ) == 0U ) // hflip
                    {
                        bit0 = Gpu.ReverseLookup[ bits.l ];
                        bit1 = Gpu.ReverseLookup[ bits.h ];
                    }
                    else
                    {
                        bit0 = bits.l;
                        bit1 = bits.h;
                    }

                    for ( uint j = 0; j < 8; j++ )
                    {
                        uint color = ( bit0 & 1U ) | ( ( bit1 & 1U ) << 1 );

                        bit0 >>= 1;
                        bit1 >>= 1;

                        if ( color != 0 )
                        {
                            var value = gpu.cram.h[ ( bg.index << 5 ) | ( ( name & 0x1C00U ) >> 8 ) | color ];

                            var r = ( value & 0x001F ) << 3;
                            var g = ( value & 0x03E0 ) >> 2;
                            var b = ( value & 0x7C00 ) >> 7;

                            bitmap.SetPixel(
                                ( int )( x << 3 | j ),
                                ( int )( y ),
                                Color.FromArgb( r, g, b ) );
                        }
                        else
                        {
                            var value = gpu.cram.h[ 0 ];

                            var r = ( value & 0x001F ) << 3;
                            var g = ( value & 0x03E0 ) >> 2;
                            var b = ( value & 0x7C00 ) >> 7;

                            bitmap.SetPixel(
                                ( int )( x << 3 | j ),
                                ( int )( y ),
                                Color.FromArgb( r, g, b ) );
                        }
                    }
                }
            }

            return bitmap;
        }

        private void radioButtonGpuBg0_CheckedChanged( object sender, EventArgs e )
        {
            pictureBoxGpu.Image = RenderBg( gpu.bg0 );
        }
        private void radioButtonGpuBg1_CheckedChanged( object sender, EventArgs e )
        {
            pictureBoxGpu.Image = RenderBg( gpu.bg1 );
        }
        private void radioButtonGpuBg2_CheckedChanged( object sender, EventArgs e )
        {
            pictureBoxGpu.Image = RenderBg( gpu.bg2 );
        }
        private void radioButtonGpuBg3_CheckedChanged( object sender, EventArgs e )
        {
            pictureBoxGpu.Image = RenderBg( gpu.bg3 );
        }

        private void buttonRun_Click( object sender, EventArgs e )
        {
            var breakpoint = uint.Parse( textBoxBreakpoint.Text, NumberStyles.HexNumber );

            while ( cpu.pc.d != breakpoint )
            {
                cpu.Update( );
            }

            UpdateForm( );

            this.panelCpuDisassembly.Invalidate( );
            this.panelSpuDisassembly.Invalidate( );
        }

        private void panelCRam_Paint( object sender, PaintEventArgs e )
        {
            for ( int i = 0; i < 256; i++ )
            {
                int color = gpu.cram.h[ i ];

                int r = ( ( color & 0x7C00 ) >> 7 );
                int g = ( ( color & 0x03E0 ) >> 2 );
                int b = ( ( color & 0x001F ) << 3 );

                using ( SolidBrush brush = new SolidBrush( Color.FromArgb( r, g, b ) ) )
                {
                    e.Graphics.FillRectangle(
                        brush,
                        ( i % 16 ) * 8,
                        ( i / 16 ) * 8,
                        8,
                        8 );
                }
            }
        }
    }
}