using Nintemulator.Shared;
using System;
using System.Windows.Forms;

namespace Nintemulator.SFC
{
    public partial class FormGame : FormHost
    {
        private FormDebugger debugger;

        public FormGame(string filename)
        {
            InitializeComponent();

            console = new SuperFamicom(base.Handle, filename);
            console.ResetHard();

            SetSize(256 * 2, 240 * 2);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
#if !DEBUG
            AbortThread();
#endif
            base.OnFormClosing(e);
        }
        protected override void OnLoad(EventArgs e)
        {
#if !DEBUG
            StartThread();
#endif
            base.OnLoad(e);
        }
        protected override void OnShown(EventArgs e)
        {
#if DEBUG
            ShowDebugger( );
#endif
            base.OnShown(e);
        }

        public override void ShowDebugger()
        {
            debugger = new FormDebugger((SuperFamicom)console);
            debugger.Show();
        }
        public override void Step()
        {
            ((SuperFamicom)console).Cpu.Update();
        }
    }
}