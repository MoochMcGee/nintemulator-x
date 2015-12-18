using Nintemulator.Shared;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Nintemulator.FC
{
    [XmlRoot("database")]
    public class FamicomDatabase
    {
        [XmlAttribute("version")]     public string Version     { get; set; }
        [XmlAttribute("conformance")] public string Conformance { get; set; }
        [XmlAttribute("author")]      public string Author      { get; set; }
        [XmlAttribute("agent")]       public string Agent       { get; set; }
        [XmlAttribute("timestamp")]   public string Timestamp   { get; set; }

        [XmlElement("game")] public List<Game> Games { get; set; }

        [XmlIgnore]
        private static FamicomDatabase singleton;

        public static void Load( )
        {
            var fileInfo = new FileInfo( "FC/Database.xml" );

            if ( !fileInfo.Exists )
            {
                MessageBox.Show( "Famicom ROM Database couldn't be found. Please follow the setup instructions." );
                return;
            }

            using ( var reader = new StreamReader( fileInfo.OpenRead( ), Encoding.UTF8 ) )
            {
                singleton = new XmlSerializer( typeof( FamicomDatabase ) ).Deserialize( reader ) as FamicomDatabase;
            }
        }

        public static Game.Cartridge.Board Find( byte[] filedata )
        {
            var hash = Crc32.Compute( filedata, 16 );

            foreach ( var game in singleton.Games )
            {
                foreach ( var cart in game.cartridges )
                {
                    if (cart.crc == hash)
                    {
                        return cart.boards[0];
                    }
                }
            }

            return null;
        }

        public class Game
        {
            [XmlAttribute("name")]          public string name          { get; set; }
            [XmlAttribute("altname")]       public string altName       { get; set; }
            [XmlAttribute("class")]         public string Class         { get; set; }
            [XmlAttribute("subclass")]      public string subclass      { get; set; }
            [XmlAttribute("catalog")]       public string catalog       { get; set; }
            [XmlAttribute("publisher")]     public string publisher     { get; set; }
            [XmlAttribute("developer")]     public string developer     { get; set; }
            [XmlAttribute("portdeveloper")] public string portDeveloper { get; set; }
            [XmlAttribute("players")]       public uint   players       { get; set; }
            [XmlAttribute("date")]          public string date          { get; set; }

            [XmlElement("cartridge")] public List<Cartridge> cartridges { get; set; }

            public class Cartridge
            {
                [XmlIgnore]
                public uint crc { get; set; }

                [XmlAttribute("system")]     public string system     { get; set; }
                [XmlAttribute("revision")]   public string revision   { get; set; }
                [XmlAttribute("prototype")]  public string prototype  { get; set; }
                [XmlAttribute("dumper")]     public string dumper     { get; set; }
                [XmlAttribute("datedumped")] public string dateDumped { get; set; }
                [XmlAttribute("dump")]       public string dump       { get; set; }
                [XmlAttribute("crc")]        public string crcString
                {
                    get { return crc.ToString("X8"); }
                    set { crc = uint.Parse(value, NumberStyles.HexNumber); }
                }
                [XmlAttribute("sha1")]       public string sha1       { get; set; }

                [XmlElement("board")] public List<Board> boards { get; set; }

                public class Board
                {
                    [XmlAttribute("type")]   public string type   { get; set; }
                    [XmlAttribute("pcb")]    public string name   { get; set; }
                    [XmlAttribute("mapper")] public uint   mapper { get; set; }

                    [XmlElement("prg")]  public List<Rom>  prg  { get; set; }
                    [XmlElement("chr")]  public List<Rom>  chr  { get; set; }
                    [XmlElement("wram")] public List<Ram>  wram { get; set; }
                    [XmlElement("vram")] public List<Ram>  vram { get; set; }
                    [XmlElement("chip")] public List<Chip> chip { get; set; }
                    [XmlElement("cic")]  public List<Cic>  cic  { get; set; }
                    [XmlElement("pad")]  public Pad pad      { get; set; }

                    public class IC
                    {
                        [XmlElement("pin")] public List<Pin> pin { get; set; }

                        public class Pin
                        {
                            [XmlAttribute("number")]   public uint   number   { get; set; }
                            [XmlAttribute("function")] public string function { get; set; }
                        }
                    }
                    public class Chip : IC
                    {
                        [XmlAttribute("type")]    public string type    { get; set; }
                        [XmlAttribute("battery")] public bool   battery { get; set; }
                    }
                    public class Cic
                    {
                        [XmlAttribute("type")] public string type { get; set; }
                    }
                    public class Pad
                    {
                        [XmlAttribute("h")] public uint h { get; set; }
                        [XmlAttribute("v")] public uint v { get; set; }
                    }
                    public class Rom : IC
                    {
                        [XmlIgnore]
                        public int size { get; set; }

                        [XmlAttribute("id")]   public uint   id   { get; set; }
                        [XmlAttribute("name")] public string name { get; set; }
                        [XmlAttribute("size")] public string sizeString
                        {
                            get { return (size / 1024) + "k"; }
                            set { size = int.Parse(value.Substring(0, value.Length - 1)) * 1024; }
                        }
                        [XmlAttribute("file")] public string file { get; set; }
                        [XmlAttribute("crc")]  public string crc  { get; set; }
                        [XmlAttribute("sha1")] public string sha1 { get; set; }
                    }
                    public class Ram : Rom
                    {
                        [XmlAttribute("battery")] public bool battery { get; set; }
                    }
                }
            }
        }
    }
}