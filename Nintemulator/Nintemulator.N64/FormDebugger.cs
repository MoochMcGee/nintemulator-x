using Nintemulator.N64.CPU.Debugging;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Nintemulator.N64
{
    public partial class FormDebugger : Form
    {
        private Nintendo64 console;

        public FormDebugger(Nintendo64 console)
        {
            InitializeComponent();

            this.console = console;
        }

        private void UpdateForm()
        {
            panelDisassembly.Invalidate();

            labelIC.Text = string.Format("IC: ${0:x8}", console.Cpu.pipeline.ic.data);
            labelRF.Text = string.Format("RF: ${0:x8}", console.Cpu.pipeline.rf.data);
            labelEX.Text = string.Format("EX: ${0:x8}", console.Cpu.pipeline.ex.data);
            labelDC.Text = string.Format("DC: ${0:x8}", console.Cpu.pipeline.dc.data);
            labelWB.Text = string.Format("WB: ${0:x8}", console.Cpu.pipeline.wb.data);

            labelPC.Text  = string.Format("${0:x16}", console.Cpu.pc);
            labelR0.Text  = string.Format("${0:x16}", console.Cpu.registers[ 0U].uq0);
            labelR1.Text  = string.Format("${0:x16}", console.Cpu.registers[ 1U].uq0);
            labelR2.Text  = string.Format("${0:x16}", console.Cpu.registers[ 2U].uq0);
            labelR3.Text  = string.Format("${0:x16}", console.Cpu.registers[ 3U].uq0);
            labelR4.Text  = string.Format("${0:x16}", console.Cpu.registers[ 4U].uq0);
            labelR5.Text  = string.Format("${0:x16}", console.Cpu.registers[ 5U].uq0);
            labelR6.Text  = string.Format("${0:x16}", console.Cpu.registers[ 6U].uq0);
            labelR7.Text  = string.Format("${0:x16}", console.Cpu.registers[ 7U].uq0);
            labelR8.Text  = string.Format("${0:x16}", console.Cpu.registers[ 8U].uq0);
            labelR9.Text  = string.Format("${0:x16}", console.Cpu.registers[ 9U].uq0);
            labelR10.Text = string.Format("${0:x16}", console.Cpu.registers[10U].uq0);
            labelR11.Text = string.Format("${0:x16}", console.Cpu.registers[11U].uq0);
            labelR12.Text = string.Format("${0:x16}", console.Cpu.registers[12U].uq0);
            labelR13.Text = string.Format("${0:x16}", console.Cpu.registers[13U].uq0);
            labelR14.Text = string.Format("${0:x16}", console.Cpu.registers[14U].uq0);
            labelR15.Text = string.Format("${0:x16}", console.Cpu.registers[15U].uq0);
            labelR16.Text = string.Format("${0:x16}", console.Cpu.registers[16U].uq0);
            labelR17.Text = string.Format("${0:x16}", console.Cpu.registers[17U].uq0);
            labelR18.Text = string.Format("${0:x16}", console.Cpu.registers[18U].uq0);
            labelR19.Text = string.Format("${0:x16}", console.Cpu.registers[19U].uq0);
            labelR20.Text = string.Format("${0:x16}", console.Cpu.registers[20U].uq0);
            labelR21.Text = string.Format("${0:x16}", console.Cpu.registers[21U].uq0);
            labelR22.Text = string.Format("${0:x16}", console.Cpu.registers[22U].uq0);
            labelR23.Text = string.Format("${0:x16}", console.Cpu.registers[23U].uq0);
            labelR24.Text = string.Format("${0:x16}", console.Cpu.registers[24U].uq0);
            labelR25.Text = string.Format("${0:x16}", console.Cpu.registers[25U].uq0);
            labelR26.Text = string.Format("${0:x16}", console.Cpu.registers[26U].uq0);
            labelR27.Text = string.Format("${0:x16}", console.Cpu.registers[27U].uq0);
            labelR28.Text = string.Format("${0:x16}", console.Cpu.registers[28U].uq0);
            labelR29.Text = string.Format("${0:x16}", console.Cpu.registers[29U].uq0);
            labelR30.Text = string.Format("${0:x16}", console.Cpu.registers[30U].uq0);
            labelR31.Text = string.Format("${0:x16}", console.Cpu.registers[31U].uq0);
        }

        private void buttonStep_Click(object sender, EventArgs e)
        {
            console.Cpu.Update();
            UpdateForm();
        }

        private void panelDisassembly_Paint(object sender, PaintEventArgs e)
        {
            var pc = console.Cpu.pc - 19*4;

            for (int i = 0; i < 39; i++)
            {
                Brush bColor = Brushes.White;
                Brush fColor = Brushes.Black;

                if (pc == console.Cpu.pipeline.ic.addr) { bColor = new SolidBrush(Color.FromArgb(0x55, 0x55, 0xFF)); fColor = Brushes.White; }
                if (pc == console.Cpu.pipeline.rf.addr) { bColor = new SolidBrush(Color.FromArgb(0x44, 0x44, 0xCC)); fColor = Brushes.White; }
                if (pc == console.Cpu.pipeline.ex.addr) { bColor = new SolidBrush(Color.FromArgb(0x33, 0x33, 0x99)); fColor = Brushes.White; }
                if (pc == console.Cpu.pipeline.dc.addr) { bColor = new SolidBrush(Color.FromArgb(0x22, 0x22, 0x66)); fColor = Brushes.White; }
                if (pc == console.Cpu.pipeline.wb.addr) { bColor = new SolidBrush(Color.FromArgb(0x11, 0x11, 0x33)); fColor = Brushes.White; }

                var addr = pc; pc += 4U;
                var code = console.bus.PeekU32(addr);

                e.Graphics.FillRectangle(bColor, 0, i * Font.Height, panelDisassembly.Width, Font.Height);
                e.Graphics.DrawString(string.Format("[${0:x8}] ${1:x8} {2}", addr, code, Disassembler.Disassemble(pc, code)),
                    Font,
                    fColor,
                    0,
                    i * Font.Height);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            UpdateForm();

            base.OnLoad(e);
        }
    }
}