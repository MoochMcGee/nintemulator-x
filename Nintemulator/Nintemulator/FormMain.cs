using Nintemulator.FC;
using Nintemulator.GB;
using Nintemulator.GBA;
using Nintemulator.GBC;
using Nintemulator.N64;
using Nintemulator.NDS;
using Nintemulator.PKM;
using Nintemulator.SFC;
using Nintemulator.Shared;
using System;
using System.IO;
using System.Windows.Forms;

namespace Nintemulator
{
    public partial class FormMain : Form
    {
        private FormHost gameHost;

        public FormMain( )
        {
            InitializeComponent( );
        }

        private void CreateGameHost<T>( )
        {
            if ( gameHost != null )
            {
                gameHost.FormClosed -= gameHost_FormClosed;
                gameHost.Close( );
            }

            var file = new FileInfo( openFileDialog.FileName );

            if ( file.Extension.Equals( ".zip", StringComparison.OrdinalIgnoreCase ) )
            {
                // todo: show archive form
                MessageBox.Show( "ZIP Archives not fully supported yet!" );
                return;
            }

            var type = typeof( T );
            gameHost = Activator.CreateInstance( type.Assembly.GetType( type.Namespace + ".FormGame" ), openFileDialog.FileName ) as FormHost;

            if ( gameHost != null && gameHost.CanPlayGame( ) )
            {
                gameHost.FormClosed += gameHost_FormClosed;
                gameHost.Show( );
            }
        }
        private void gameHost_FormClosed( object sender, FormClosedEventArgs e )
        {
            if ( gameHost != null )
            {
                gameHost.Dispose( );
                gameHost = null;
            }
        }
        
        // consoles
        private void fcToolStripMenuItem_Click( object sender, EventArgs e )
        {
            openFileDialog.Filter = "Famicom Image (*.nes;*.zip)|*.nes;*.zip";

            if ( openFileDialog.ShowDialog( this ) == System.Windows.Forms.DialogResult.OK )
            {
                CreateGameHost<Famicom>( );
            }
        }
        private void sfcToolStripMenuItem_Click( object sender, EventArgs e )
        {
            openFileDialog.Filter = "Super Famicom Image (*.sfc;*.zip)|*.sfc;*.zip";

            if ( openFileDialog.ShowDialog( this ) == System.Windows.Forms.DialogResult.OK )
            {
                CreateGameHost<SuperFamicom>( );
            }
        }
        private void n64ToolStripMenuItem_Click( object sender, EventArgs e )
        {
            openFileDialog.Filter = "Nintendo 64 Image (*.z64;*.zip)|*.z64;*.zip";

            if ( openFileDialog.ShowDialog( this ) == System.Windows.Forms.DialogResult.OK )
            {
                CreateGameHost<Nintendo64>( );
            }
        }
        // handhelds
        private void gbToolStripMenuItem_Click( object sender, EventArgs e )
        {
            openFileDialog.Filter = "Game Boy Image (*.gb;*.zip)|*.gb;*.zip";

            if ( openFileDialog.ShowDialog( this ) == System.Windows.Forms.DialogResult.OK )
            {
                CreateGameHost<Gameboy>( );
            }
        }
        private void gbcToolStripMenuItem_Click( object sender, EventArgs e )
        {
            openFileDialog.Filter = "Game Boy Color Image (*.gbc;*.zip)|*.gbc;*.zip";

            if ( openFileDialog.ShowDialog( this ) == System.Windows.Forms.DialogResult.OK )
            {
                CreateGameHost<GameboyColor>( );
            }
        }
        private void gbaToolStripMenuItem_Click( object sender, EventArgs e )
        {
            openFileDialog.Filter = "Game Boy Advance Image (*.gba;*.zip)|*.gba;*.zip";

            if ( openFileDialog.ShowDialog( this ) == System.Windows.Forms.DialogResult.OK )
            {
                CreateGameHost<GameboyAdvance>( );
            }
        }
        private void ndsToolStripMenuItem_Click( object sender, EventArgs e )
        {
            openFileDialog.Filter = "Dual Screen Image (*.nds;*.zip)|*.nds;*.zip";

            if ( openFileDialog.ShowDialog( this ) == System.Windows.Forms.DialogResult.OK )
            {
                CreateGameHost<DualScreen>( );
            }
        }
        private void pkmToolStripMenuItem_Click( object sender, EventArgs e )
        {
            openFileDialog.Filter = "Pokemon Mini Image (*.min;*.zip)|*.min;*.zip";

            if ( openFileDialog.ShowDialog( this ) == System.Windows.Forms.DialogResult.OK )
            {
                CreateGameHost<PokemonMini>( );
            }
        }

        protected override void OnFormClosing( FormClosingEventArgs e )
        {
            if ( gameHost != null )
            {
                gameHost.Close( );
                gameHost = null;
            }

            base.OnFormClosing( e );
        }
    }
}