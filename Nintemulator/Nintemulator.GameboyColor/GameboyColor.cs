using Nintemulator.GBC.Boards;
using Nintemulator.GBC.CPU;
using Nintemulator.GBC.GPU;
using Nintemulator.GBC.SPU;
using Nintemulator.Shared;
using System;
using System.IO;

namespace Nintemulator.GBC
{
    public partial class GameboyColor : Console<GameboyColor, Cpu, Gpu, Spu>
    {
        private Board board;
        private Lcd lcd;
        private Pad pad;
        private Sio sio;
        private Tma tma;

        public Board Board { get { return board; } }
        public Lcd Lcd { get { return lcd; } }
        public Tma Tma { get { return tma; } }

        public GameboyColor( IntPtr handle, string filename )
        {
            Timing.System system = new Timing.System( 0x0, 4194304, 4, 1, 4 );

            cpu = new Cpu( this, system );
            ppu = new Gpu( this, system );
            apu = new Spu( this, system );
            lcd = new Lcd( this );
            pad = new Pad( this );
            sio = new Sio( this );
            tma = new Tma( this );

            var rom = File.ReadAllBytes( filename );
            var mbc = BoardManager.GetBoard( rom );

            const float RATIO = ( 4213440F / 4194304F );

            board = Activator.CreateInstance( mbc, this, rom ) as Board;
            audio = new AudioRenderer( handle, new AudioRenderer.Params( 48000, 2, RATIO ) );
            video = new VideoRenderer( handle, new VideoRenderer.Params( 160, 144 ) );

            Hook( 0x0000u, 0xffffu, PeekOpen, PokeOpen );

            Initialize( );
        }

        protected void Initialize( )
        {
            board.Initialize( );
            audio.Initialize( );
            video.Initialize( );

            InitializeMemory( );

            cpu.Initialize( );
            ppu.Initialize( );
            apu.Initialize( );
            pad.Initialize( );
            sio.Initialize( );
            tma.Initialize( );
        }
    }
}