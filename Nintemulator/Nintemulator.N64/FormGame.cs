using Nintemulator.Shared;
using System;
using System.Windows.Forms;

namespace Nintemulator.N64
{
    public partial class FormGame : FormHost
    {
#if DEBUG
        private FormDebugger debugger;
#endif
        public FormGame( string filename )
        {
            InitializeComponent( );

            console = new Nintendo64( base.Handle, filename );
            console.ResetHard( );

            SetSize( 256 * 2, 224 * 2 );
        }

        protected override void OnShown( EventArgs e )
        {
            base.OnShown( e );
#if DEBUG
            debugger.Show( );
#endif
        }
        protected override void OnLoad( EventArgs e )
        {
#if DEBUG
            debugger = new FormDebugger( console );
#else
            StartThread();
#endif
            base.OnLoad( e );
        }
        protected override void OnFormClosed( FormClosedEventArgs e )
        {
#if DEBUG
            debugger.Close( );
            debugger = null;
#else
            AbortThread();
#endif
            base.OnFormClosed( e );
        }

        public override void Step()
        {
            ((Nintendo64)console).Cpu.Update();
        }
    }
}