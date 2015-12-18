using Nintemulator.FC.Boards;
using Nintemulator.FC.CPU;
using Nintemulator.FC.GPU;
using Nintemulator.FC.PAD;
using Nintemulator.FC.SPU;
using Nintemulator.Shared;
using System;
using System.IO;

namespace Nintemulator.FC
{
    public class Famicom : Console<Famicom, Cpu, Gpu, Spu>
    {
        public const int MODEL_NTSC = 0x0;
        public const int MODEL_PALB = 0x1;

        public static readonly Timing.System NTSC = new Timing.System( MODEL_NTSC, 236250000, 12, 4, 24 );
        public static readonly Timing.System PALB = new Timing.System( MODEL_PALB, 212813700, 16, 5, 32 );

        private uint pad_latch;

        public Board Board;
        public Pad Jpa;
        public Pad Jpb;

        static Famicom( )
        {
            FamicomDatabase.Load( );
        }
        public Famicom( IntPtr handle, string filename )
        {
            const float RATIO_NTSC = 5360490F / ( 236250000F / 44F );
            const float RATIO_PALB = 5319600F / ( 212813700F / 40F );

            var filedata = File.ReadAllBytes( filename );
            var game = FamicomDatabase.Find( filedata );

            if ( game == null )
                throw new FileNotFoundException( "Unable to find file in database." );

            cpu = new Cpu( this, NTSC );
            ppu = new Gpu( this, NTSC );
            apu = new Spu( this, NTSC );
            Jpa = new StandardController( this, 0 );
            Jpb = new StandardController( this, 1 );

            Board = Activator.CreateInstance( BoardManager.GetBoard( game.type ), this, filedata, game ) as Board;
            audio = new AudioRenderer( handle, new AudioRenderer.Params( 48000, 1, RATIO_NTSC ) );
            video = new VideoRenderer( handle, new VideoRenderer.Params( 256, 240 ) );

            Initialize( );

            Cpu.Hook( 0x4016, Peek4016, Poke4016 );
            Cpu.Hook( 0x4017, Peek4017 );

            if ( game.pad != null )
            {
                if ( game.pad.h == 0 && game.pad.v == 1 ) { Ppu.SwitchNametables( Mirroring.ModeHorz ); }
                if ( game.pad.h == 1 && game.pad.v == 0 ) { Ppu.SwitchNametables( Mirroring.ModeVert ); }
            }
        }

        private byte Peek4016( uint address ) { return Jpa.GetData( ); }
        private byte Peek4017( uint address ) { return Jpb.GetData( ); }
        private void Poke4016( uint address, byte data )
        {
            if ( pad_latch > ( data & 0x01U ) ) // falling edge
            {
                Jpa.SetData( );
                Jpb.SetData( );
            }

            pad_latch = ( data & 0x01U );
        }

        protected void Initialize( )
        {
            cpu.Initialize( );
            ppu.Initialize( );
            apu.Initialize( );

            Jpa.Initialize( );
            Jpb.Initialize( );

            Audio.Initialize( );
            Video.Initialize( );
            Board.Initialize( );
        }

        public override void ResetHard( )
        {
            cpu.ResetHard( );
            ppu.ResetHard( );
            apu.ResetHard( );

            Jpa.ResetHard( );
            Jpb.ResetHard( );

            Board.ResetHard( );
        }
        public override void ResetSoft( )
        {
            cpu.ResetSoft( );
            ppu.ResetSoft( );
            apu.ResetSoft( );

            Jpa.ResetSoft( );
            Jpb.ResetSoft( );

            Board.ResetSoft( );
        }
    }
}