using Nintemulator.Shared;
using System;
using System.IO;
using System.Windows.Forms;

namespace Nintemulator.FC
{
    public partial class FormGame : FormHost
    {
        private FormDebugger debugger;

        public FormGame( string filename )
        {
            InitializeComponent( );

            console = new Famicom( base.Handle, filename );
            console.ResetHard( );

            SetSize( 256 * 2, 240 * 2 );
        }

        protected override void OnFormClosing( FormClosingEventArgs e )
        {
#if !DEBUG
            AbortThread( );
#endif
            base.OnFormClosing( e );
        }
        protected override void OnLoad( EventArgs e )
        {
#if !DEBUG
            StartThread( );
#endif
            base.OnLoad( e );
        }
        protected override void OnShown( EventArgs e )
        {
#if DEBUG
            ShowDebugger( );
#endif
            base.OnShown( e );
        }

        public override void ShowDebugger( )
        {
            debugger = new FormDebugger( this, (Famicom)console );
            debugger.Show( );
        }
        public override void Step()
        {
            ((Famicom)console).Cpu.Update();
        }

        public override bool CanPlayGame( )
        {
            if ( !File.Exists( "fc/database.xml" ) )
            {
                MessageBox.Show( "ROM database not found, please follow the setup instructions." );
                return false;
            }

            return true;
        }
    }
}