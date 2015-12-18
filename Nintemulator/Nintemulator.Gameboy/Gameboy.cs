using System;
using System.IO;
using Nintemulator.Shared;
using Nintemulator.GB.Boards;
using Nintemulator.GB.CPU;
using Nintemulator.GB.GPU;
using Nintemulator.GB.SPU;

namespace Nintemulator.GB
{
    public partial class Gameboy : Console<Gameboy, Cpu, Gpu, Spu>
    {
        public const int MODEL_DMG = 0x0;
        public const int MODEL_GBC = 0x1;
        public const int MODEL_SGB = 0x2;

        public static readonly Timing.System DMG = new Timing.System( MODEL_DMG, 4194304, 4, 1, 4 );
        public static readonly Timing.System GBC = new Timing.System( MODEL_GBC, 4194304, 4, 1, 4 );
        public static readonly Timing.System SGB = new Timing.System( MODEL_SGB, 47250000, 44, 11, 44 );

        private Board board;
        private Lcd lcd;
        private Pad pad;
        private Tma tma;

        public Board Board { get { return board; } }
        public Lcd Lcd { get { return lcd; } }
        public Pad Pad { get { return pad; } }
        public Tma Tma { get { return tma; } }

        public Gameboy( IntPtr handle, string filename )
        {
            Timing.System system;
            Model model;

            switch ( Path.GetExtension( filename ) )
            {
            default:
            case ".gb": system = DMG; model = Model.Gmb; break;
            case ".gbc": system = GBC; model = Model.Gbc; break;
            }

            cpu = new Cpu( this, system );
            ppu = new Gpu( this, system );
            apu = new Spu( this, system );
            lcd = new Lcd( this );
            pad = new Pad( this );
            tma = new Tma( this );

            var rom = File.ReadAllBytes( filename );
            var mbc = BoardManager.GetBoard( rom );

            const float RATIO = ( 4213440F / 4194304F );

            board = Activator.CreateInstance( mbc, this, rom ) as Board;
            audio = new AudioRenderer( handle, new AudioRenderer.Params( 48000, 2, RATIO ) );
            video = new VideoRenderer( handle, new VideoRenderer.Params( 160, 144 ) );

            Hook( 0x0000, 0xFFFF,
                delegate { return 0; },
                delegate { } );

            Initialize( model );
        }

        protected void Initialize( Model model )
        {
            board.Initialize( );
            audio.Initialize( );
            video.Initialize( );

            InitializeMemory( model );

            cpu.Initialize( );
            ppu.Initialize( );
            apu.Initialize( );
            pad.Initialize( );
            tma.Initialize( );
        }

        public enum Model
        {
            Gmb,
            Gbc,
            Sgb
        }
    }
}