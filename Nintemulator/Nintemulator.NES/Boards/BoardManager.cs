using Nintemulator.FC.Boards.Bandai;
using Nintemulator.FC.Boards.Camerica;
using Nintemulator.FC.Boards.Discreet;
using Nintemulator.FC.Boards.Konami;
using Nintemulator.FC.Boards.Namcot;
using Nintemulator.FC.Boards.Nintendo;
using Nintemulator.FC.Boards.Unlicensed;
using System;

namespace Nintemulator.FC.Boards
{
    public static class BoardManager
    {
        public static Type GetBoard( string name )
        {
            switch ( name )
            {
            case "NES-NROM-128": return typeof( NROM128 );
            case "NES-NROM-256": return typeof( NROM256 );

            case "NES-ANROM": return typeof( ANROM );
            case "NES-AOROM": return typeof( AOROM );
            case "NES-CNROM": return typeof( CNROM );
            case "NES-CPROM": return typeof( CPROM );

            case "HVC-EKROM":
            case "HVC-ELROM":
            case "HVC-ETROM":
            case "HVC-EWROM":
            case "NES-EKROM":
            case "NES-ELROM":
            case "NES-ETROM":
            case "NES-EWROM": return typeof( NintendoMMC5 );

            case "NES-FxROM": return typeof( NintendoMMC4 );
            case "NES-PxROM": return typeof( NintendoMMC2 );
            case "NES-SGROM":
            case "NES-SKROM":
            case "NES-SLROM":
            case "NES-SUROM":
            case "NES-SxROM": return typeof( NintendoMMC1 );
            case "KONAMI-TLROM":
            case "NES-TGROM":
            case "NES-TKROM":
            case "NES-TKEPROM":
            case "HVC-TLROM":
            case "NES-TLROM":
            case "NES-TSROM":
            case "NES-TxROM": return typeof( NintendoMMC3 );
            case "NES-UNROM": return typeof( UNROM );
            case "NES-UOROM": return typeof( UOROM );

            // -- Bandai Boards --

            case "BANDAI-LZ93D50":
            case "BANDAI-LZ93D50+24C01":
            case "BANDAI-LZ93D50+24C02": return typeof( BandaiLZ93D50 );

            // -- Konami Boards --

            case "KONAMI-VRC-1": return typeof( KonamiVRC1 );
            case "KONAMI-VRC-2": return typeof( KonamiVRC2 );
            case "KONAMI-VRC-3": return typeof( KonamiVRC3 );
            case "KONAMI-VRC-4": return typeof( KonamiVRC4 );
            case "KONAMI-VRC-6": return typeof( KonamiVRC6 );
            case "KONAMI-VRC-7": return typeof( KonamiVRC7 );

            // -- Namcot Boards --

            case "NAMCOT-163": return typeof( Namcot163 );

            // -- Camerica Boards --

            case "CAMERICA-BF9093": return typeof( CamericaBF9093 );
            case "CAMERICA-BF9096": return typeof( CamericaBF9096 );

            // -- Unlicensed Boards --

            case "COLORDREAMS-74*377": return typeof( ColorDreams74x377 );
            case "MLT-ACTION52": return typeof( MltAction52 );
            }

            throw new NotSupportedException( "Mapper '" + name + "' isn't supported" );
        }
    }
}