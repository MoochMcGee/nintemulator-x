using Nintemulator.Shared;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Nintemulator.GBC
{
    public partial class FormGame : FormHost
    {
        public FormGame( string filename )
        {
            InitializeComponent( );

            console = new GameboyColor( base.Handle, filename );
            console.ResetHard( );

            SetSize( 160 * 4, 144 * 4 );
        }

        protected override void OnFormClosing( FormClosingEventArgs e )
        {
            AbortThread( );

            base.OnFormClosing( e );
        }
        protected override void OnLoad( EventArgs e )
        {
            StartThread( );

            base.OnLoad( e );
        }

        public override bool CanPlayGame( )
        {
            var boot = new FileInfo( "gbc/boot.rom" );

            if ( !boot.Exists )
            {
                MessageBox.Show( "Could not find required file 'gbc/boot.rom'.", "File not found" );
                return false;
            }

            if ( boot.Length != 0x900 )
            {
                MessageBox.Show( "Could not confirm 'gbc/boot.rom' is the proper binary.", "Invalid file size" );
                return false;
            }

            var data = File.ReadAllBytes(boot.FullName);

            if ( data.Sum(o => o) != 0x2f3ea )
            {
                MessageBox.Show( "Could not confirm 'gbc/boot.rom' is the proper binary.", "Checksum failed" );
                return false;
            }

            return true;
        }
        public override void Step()
        {
            ((GameboyColor)console).Cpu.Update();
        }
    }
}