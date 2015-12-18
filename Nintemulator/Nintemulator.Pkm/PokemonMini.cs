using Nintemulator.PKM.CPU;
using Nintemulator.PKM.GPU;
using Nintemulator.PKM.SPU;
using Nintemulator.Shared;
using System;

namespace Nintemulator.PKM
{
    public partial class PokemonMini : Console<PokemonMini, Cpu, Gpu, Spu>
    {
        private static Timing.System NTSC = new Timing.System( 0, 4194304, 4, 4, 4 );

        public PokemonMini( IntPtr handle, string filename )
        {
            cpu = new Cpu( this, NTSC );
            ppu = new Gpu( this, NTSC );
            apu = new Spu( this, NTSC );
            audio = new AudioRenderer( handle, new AudioRenderer.Params( 48000, 2, 1f ) );
            video = new VideoRenderer( handle, new VideoRenderer.Params( 96, 64 ) );

            Initialize( filename );
        }

        private void Initialize( string filename )
        {
            audio.Initialize( );
            video.Initialize( );

            InitializeMemory( filename );

            cpu.Initialize( );
            ppu.Initialize( );
            apu.Initialize( );
        }
    }
}