using Nintemulator.Shared;
using System;
using System.Windows.Forms;

namespace Nintemulator.PKM
{
    public partial class FormGame : FormHost
    {
        public FormGame(string filename)
        {
            InitializeComponent();

            console = new PokemonMini(base.Handle, filename);
            console.ResetHard();

            SetSize(96 * 4, 64 * 4);
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
            ((PokemonMini)console).Cpu.Update();
        }
    }
}