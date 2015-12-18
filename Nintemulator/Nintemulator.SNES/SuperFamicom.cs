using Nintemulator.SFC.CPU;
using Nintemulator.SFC.GPU;
using Nintemulator.SFC.PAD;
using Nintemulator.SFC.SPU;
using Nintemulator.Shared;
using System;

namespace Nintemulator.SFC
{
    public class SuperFamicom : Console<SuperFamicom, Cpu, Gpu, Spu>
    {
        public const int MODEL_NTSC = 0x0;
        public const int MODEL_PALB = 0x1;

        public static readonly Timing.System NTSC = new Timing.System( MODEL_NTSC, 236250000, 8, 4, 21 );
        public static readonly Timing.System PALB = new Timing.System( MODEL_PALB, 0, 0, 0, 0 ); // todo: pal-b timing

        public Pad jp1;
        public Pad jp2;

        public SuperFamicom( IntPtr handle, string filename )
        {
            cpu = new Cpu( this, NTSC );
            ppu = new Gpu( this, NTSC );
            apu = new Spu( this, NTSC );
            jp1 = new Pad( this, 0 );
            jp2 = new Pad( this, 1 );

            cpu.LoadCart( filename );

            audio = new AudioRenderer( handle, new AudioRenderer.Params( 32000, 2, 1F ) );
            video = new VideoRenderer( handle, new VideoRenderer.Params( 256, 240 ) );

            Initialize( );
        }

        protected void Initialize( )
        {
            audio.Initialize( );
            video.Initialize( );

            jp1.Initialize( );
            jp2.Initialize( );

            cpu.Initialize( );
            ppu.Initialize( );
            apu.Initialize( );
        }
    }
}