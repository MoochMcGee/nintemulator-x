using Nintemulator.N64.CPU;
using Nintemulator.N64.GPU;
using Nintemulator.N64.SPU;
using Nintemulator.Shared;
using System;
using System.IO;
using s08 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using s64 = System.Int64;
using u08 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;

namespace Nintemulator.N64
{
    public partial class Nintendo64 : Console<Nintendo64, Cpu, Gpu, Spu>
    {
        public Bus bus;

        public Nintendo64( IntPtr handle, string filename )
        {
            bus = new Bus( );
            bus.cart = File.ReadAllBytes( filename );

            cpu = new Cpu( this, default( Timing.System ) );
            ppu = new Gpu( this, default( Timing.System ) );
            apu = new Spu( this, default( Timing.System ) );

            bus.boot_rom = File.ReadAllBytes( "N64/boot_ntsc.rom" );

            audio = new AudioRenderer( handle, new AudioRenderer.Params( 48000, 2, 1F ) );
            video = new VideoRenderer( handle, new VideoRenderer.Params( 640, 480 ) );

            Initialize( );
        }


        protected void Initialize( )
        {
            audio.Initialize( );
            video.Initialize( );

            cpu.Initialize( );
            ppu.Initialize( );
            apu.Initialize( );
        }
    }
}