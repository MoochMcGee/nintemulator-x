using Nintemulator.GBA.CPU;
using Nintemulator.GBA.GPU;
using Nintemulator.GBA.SPU;
using Nintemulator.Shared;
using System;
using System.IO;
using half = System.UInt16;
using word = System.UInt32;

namespace Nintemulator.GBA
{
    public partial class GameboyAdvance : Console<GameboyAdvance, Cpu, Gpu, Spu>
    {
        private static readonly Timing.System NTSC = new Timing.System(0, 16777216, 1, 4, 1);

        private byte[] io_memory = new byte[1024];

        public Cart Cart;
        public Dma Dma0;
        public Dma Dma1;
        public Dma Dma2;
        public Dma Dma3;
        public Pad Pad;
        public Timer Timer0;
        public Timer Timer1;
        public Timer Timer2;
        public Timer Timer3;

        public GameboyAdvance(IntPtr handle, string filename)
        {
            cpu = new Cpu(this, NTSC);
            ppu = new Gpu(this, NTSC);
            apu = new Spu(this, NTSC);

            const float RATIO = (16853760F / 16777216F);

            audio = new AudioRenderer(handle, new AudioRenderer.Params(48000, 2, RATIO));
            video = new VideoRenderer(handle, new VideoRenderer.Params(240, 160));

            bios = new MemoryChip(File.ReadAllBytes("gba/boot.rom"));
            Cart = new Cart(this, filename);

            Dma0 = new Dma(this, CPU.Cpu.Source.Dma0, 14, 27, 27);
            Dma1 = new Dma(this, CPU.Cpu.Source.Dma1, 14, 27, 28);
            Dma2 = new Dma(this, CPU.Cpu.Source.Dma2, 14, 27, 28);
            Dma3 = new Dma(this, CPU.Cpu.Source.Dma3, 16, 28, 28);

            Pad = new Pad(this);

            Timer0 = new Timer(this, CPU.Cpu.Source.Timer0, 0);
            Timer1 = new Timer(this, CPU.Cpu.Source.Timer1, 1);
            Timer2 = new Timer(this, CPU.Cpu.Source.Timer2, 2);
            Timer3 = new Timer(this, CPU.Cpu.Source.Timer3, 3);

            Timer0.NextTimer = Timer1;
            Timer1.NextTimer = Timer2;
            Timer2.NextTimer = Timer3;
            Timer3.NextTimer = null;

            Initialize();
        }

        private byte PeekOpenBus(word address) { return io_memory[address & 0x3FF]; }
        private void PokeOpenBus(word address, byte data) { io_memory[address & 0x3FF] = data; }

        private byte Peek200(word address) { return cpu.ief.l; }
        private byte Peek201(word address) { return cpu.ief.h; }
        private byte Peek202(word address) { return cpu.irf.l; }
        private byte Peek203(word address) { return cpu.irf.h; }
        private byte Peek208(word address)
        {
            if (cpu.ime)
                return 1;

            return 0;
        }
        private byte Peek209(word address) { return 0; }
        private void Poke200(word address, byte data) { cpu.ief.l = data; }
        private void Poke201(word address, byte data) { cpu.ief.h = data; }
        private void Poke202(word address, byte data) { cpu.irf.l &= (byte)~data; }
        private void Poke203(word address, byte data) { cpu.irf.h &= (byte)~data; }
        private void Poke204(word address, byte data)
        {
            io_memory[0x204] = data;

            switch ((data >> 0) & 3) //  0-1  SRAM Wait Control          (0..3 = 4,3,2,8 cycles)
            {
            case 0: Cart.WRamAccess = 4; break;
            case 1: Cart.WRamAccess = 3; break;
            case 2: Cart.WRamAccess = 2; break;
            case 3: Cart.WRamAccess = 8; break;
            }

            switch ((data >> 2) & 3) //  2-3  Wait State 0 First Access  (0..3 = 4,3,2,8 cycles)
            {
            case 0: Cart.Access1[0] = 4; break;
            case 1: Cart.Access1[0] = 3; break;
            case 2: Cart.Access1[0] = 2; break;
            case 3: Cart.Access1[0] = 8; break;
            }

            switch ((data >> 4) & 1) //  4    Wait State 0 Second Access (0..1 = 2,1 cycles)
            {
            case 0: Cart.Access2[0] = 2; break;
            case 1: Cart.Access2[0] = 1; break;
            }

            switch ((data >> 5) & 3) //  5-6  Wait State 1 First Access  (0..3 = 4,3,2,8 cycles)
            {
            case 0: Cart.Access1[1] = 4; break;
            case 1: Cart.Access1[1] = 3; break;
            case 2: Cart.Access1[1] = 2; break;
            case 3: Cart.Access1[1] = 8; break;
            }

            switch ((data >> 7) & 1) //  7    Wait State 1 Second Access (0..1 = 4,1 cycles)
            {
            case 0: Cart.Access2[1] = 4; break;
            case 1: Cart.Access2[1] = 1; break;
            }
        }
        private void Poke205(word address, byte data)
        {
            io_memory[0x205] = data;

            switch ((data >> 0) & 3) //  0-1  Wait State 2 First Access  (0..3 = 4,3,2,8 cycles)
            {
            case 0: Cart.Access1[2] = 4; break;
            case 1: Cart.Access1[2] = 3; break;
            case 2: Cart.Access1[2] = 2; break;
            case 3: Cart.Access1[2] = 8; break;
            }

            switch ((data >> 2) & 1) //  2    Wait State 2 Second Access (0..1 = 8,1 cycles; unlike above WS0,WS1)
            {
            case 0: Cart.Access2[2] = 8; break;
            case 1: Cart.Access2[2] = 1; break;
            }

            //  3-4  PHI Terminal Output        (0..3 = Disable, 4.19MHz, 8.38MHz, 16.78MHz)
            //  5    Not used
            //  6    Game Pak Prefetch Buffer (Pipe) (0=Disable, 1=Enable)
            //  7    Game Pak Type Flag  (Read Only) (0=GBA, 1=CGB) (IN35 signal)
        }
        private void Poke208(word address, byte data)
        {
            cpu.ime = (data & 1) != 0;
        }
        private void Poke301(word address, byte data)
        {
            if (cpu.ief.w == 0)
                return;

            cpu.halt = true;
        }

        protected void Initialize()
        {
            Hook(0x000, RegsMask, PeekOpenBus, PokeOpenBus);

            audio.Initialize();
            video.Initialize();

            cpu.Initialize();
            ppu.Initialize();
            apu.Initialize();

            Cart.Initialize();

            Dma0.Initialize(0x0B0);
            Dma1.Initialize(0x0BC);
            Dma2.Initialize(0x0C8);
            Dma3.Initialize(0x0D4);

            Pad.Initialize();

            Timer0.Initialize(0x100);
            Timer1.Initialize(0x104);
            Timer2.Initialize(0x108);
            Timer3.Initialize(0x10C);

            Hook(0x200, Peek200,     Poke200);
            Hook(0x201, Peek201,     Poke201);
            Hook(0x202, Peek202,     Poke202);
            Hook(0x203, Peek203,     Poke203);
            Hook(0x204,              Poke204);
            Hook(0x205,              Poke205);
            Hook(0x206, PeekOpenBus, PokeOpenBus);
            Hook(0x207, PeekOpenBus, PokeOpenBus);
            Hook(0x208, Peek208,     Poke208);
            Hook(0x209, Peek209,     PokeOpenBus);
            // 20a - 20f
        }
    }
}