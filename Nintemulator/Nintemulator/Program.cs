using Nintemulator.FC;
using Nintemulator.GB;
using Nintemulator.GBA;
using Nintemulator.GBC;
using Nintemulator.N64;
using Nintemulator.Properties;
using Nintemulator.SFC;
using System;
using System.IO;
using System.Windows.Forms;

namespace Nintemulator
{
    static class Program
    {
        public static Settings Settings;

        [STAThread]
        static void Main( )
        {
            Settings = new Settings( );
            Settings.Reload( );

            CreateSystemDirectory(  "fc" );
            CreateSystemDirectory( "sfc" );
            CreateSystemDirectory( "n64" );
            CreateSystemDirectory( "gb"  );
            CreateSystemDirectory( "gba" );
            CreateSystemDirectory( "gbc" );
            CreateSystemDirectory( "nds" );
            CreateSystemDirectory( "pkm" );

            // nes
            if ( !File.Exists( "fc/database.xml" ) ) { File.Create( "fc/put database.xml here.txt" ); }

            // gb
            if ( !File.Exists( "gb/boot.rom" ) ) { File.Create( "gb/put boot.rom here.txt" ); }

            // gbc
            if ( !File.Exists( "gbc/boot.rom" ) ) { File.Create( "gbc/put boot.rom here.txt" ); }

            // gba
            if ( !File.Exists( "gba/boot.rom" ) ) { File.Create( "gba/put boot.rom here.txt" ); }

            // nds
            if ( !File.Exists( "nds/boot7.rom" ) ) { File.Create( "nds/put boot7.rom here.txt" ); }
            if ( !File.Exists( "nds/boot9.rom" ) ) { File.Create( "nds/put boot9.rom here.txt" ); }
            if ( !File.Exists( "nds/firmware.rom" ) ) { File.Create( "nds/put firmware.rom here.txt" ); }

            // pkm
            if ( !File.Exists( "pkm/boot.rom" ) ) { File.Create( "pkm/put boot.rom here.txt" ); }

            Application.EnableVisualStyles( );
            Application.SetCompatibleTextRenderingDefault( false );
            Application.Run( new FormMain( ) );

            Settings.Save( );
        }

        static void CreateSystemDirectory( string name )
        {
            Directory.CreateDirectory( name );
            Directory.CreateDirectory( name + "/battery" );
            Directory.CreateDirectory( name + "/states" );
        }
    }
}