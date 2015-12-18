using Nintemulator.Shared;
using System;
using System.Windows.Forms;

namespace Nintemulator.GBA
{
    public partial class FormGame : FormHost
    {
        public FormGame(string filename)
        {
            InitializeComponent();

            console = new GameboyAdvance(base.Handle, filename);
            console.ResetHard();

            SetSize(240 * 2, 160 * 2);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            AbortThread();

            base.OnFormClosing(e);
        }
        protected override void OnLoad(EventArgs e)
        {
            StartThread();

            base.OnLoad(e);
        }

        public override void Step()
        {
            ((GameboyAdvance)console).Cpu.Update();
        }
    }
}