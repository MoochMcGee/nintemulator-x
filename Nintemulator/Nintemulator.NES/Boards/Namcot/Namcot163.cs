using Nintemulator.FC.SPU;
using Nintemulator.Shared;

namespace Nintemulator.FC.Boards.Namcot
{
    public class Namcot163 : Board
    {
        private Sound sound;
        private bool irq_enabled;
        private uint irq_counter;
        private byte[] nmt_a;
        private byte[] nmt_b;
        private byte[] nmt_c;
        private byte[] nmt_d;
        private uint[] nmtPage;

        public Namcot163(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            chrPage = new uint[8U];
            nmtPage = new uint[4U];
            prgPage = new uint[4U];

            sound = new Sound(console);
        }

        private byte PeekNmtA(uint address) { return nmt_a[(address & 0x03FFU) | nmtPage[0U]]; }
        private byte PeekNmtB(uint address) { return nmt_b[(address & 0x03FFU) | nmtPage[1U]]; }
        private byte PeekNmtC(uint address) { return nmt_c[(address & 0x03FFU) | nmtPage[2U]]; }
        private byte PeekNmtD(uint address) { return nmt_d[(address & 0x03FFU) | nmtPage[3U]]; }

        private void PokeNmtA(uint address, byte data) { nmt_a[address & 0x03FFU] = data; }
        private void PokeNmtB(uint address, byte data) { nmt_b[address & 0x03FFU] = data; }
        private void PokeNmtC(uint address, byte data) { nmt_c[address & 0x03FFU] = data; }
        private void PokeNmtD(uint address, byte data) { nmt_d[address & 0x03FFU] = data; }

        private byte Peek4800(uint address) { return sound.Peek4800(address); }
        private byte Peek5000(uint address) { cpu.Irq(0u); return (byte)(irq_counter >> 0); }
        private byte Peek5800(uint address) { cpu.Irq(0u); return (byte)(irq_counter >> 8); }

        private void Poke4800(uint address, byte data) { sound.Poke4800(address, data); }
        private void Poke5000(uint address, byte data)
        {
            cpu.Irq(0u);
            irq_counter = (irq_counter & ~0x00FFU) | ((data & 0xFFU) << 0);
        }
        private void Poke5800(uint address, byte data)
        {
            cpu.Irq(0u);
            irq_counter = (irq_counter & ~0x7F00U) | ((data & 0x7FU) << 8);
            irq_enabled = (data & 0x80) != 0;
        }
        private void Poke8000(uint address, byte data) { chrPage[0U] = ((data & 0xFFU) << 10); }
        private void Poke8800(uint address, byte data) { chrPage[1U] = ((data & 0xFFU) << 10); }
        private void Poke9000(uint address, byte data) { chrPage[2U] = ((data & 0xFFU) << 10); }
        private void Poke9800(uint address, byte data) { chrPage[3U] = ((data & 0xFFU) << 10); }
        private void PokeA000(uint address, byte data) { chrPage[4U] = ((data & 0xFFU) << 10); }
        private void PokeA800(uint address, byte data) { chrPage[5U] = ((data & 0xFFU) << 10); }
        private void PokeB000(uint address, byte data) { chrPage[6U] = ((data & 0xFFU) << 10); }
        private void PokeB800(uint address, byte data) { chrPage[7U] = ((data & 0xFFU) << 10); }
        private void PokeC000(uint address, byte data)
        {
            if (data < 0xE0)
            {
                nmt_a = chr.b;
                nmtPage[0] = (uint)(data << 10);
            }
            else
            {
                nmt_a = gpu.nmt[data & 1];
                nmtPage[0] = 0;
            }
        }
        private void PokeC800(uint address, byte data)
        {
            if (data < 0xE0)
            {
                nmt_b = chr.b;
                nmtPage[1] = (uint)(data << 10);
            }
            else
            {
                nmt_b = gpu.nmt[data & 1];
                nmtPage[1] = 0;
            }
        }
        private void PokeD000(uint address, byte data)
        {
            if (data < 0xE0)
            {
                nmt_c = chr.b;
                nmtPage[2] = (uint)(data << 10);
            }
            else
            {
                nmt_c = gpu.nmt[data & 1];
                nmtPage[2] = 0;
            }
        }
        private void PokeD800(uint address, byte data)
        {
            if (data < 0xE0)
            {
                nmt_d = chr.b;
                nmtPage[3] = (uint)(data << 10);
            }
            else
            {
                nmt_d = gpu.nmt[data & 1];
                nmtPage[3] = 0;
            }
        }
        private void PokeE000(uint address, byte data) { prgPage[0U] = ((data & 0x3FU) << 13); }
        private void PokeE800(uint address, byte data) { prgPage[1U] = ((data & 0x3FU) << 13); }
        private void PokeF000(uint address, byte data) { prgPage[2U] = ((data & 0x3FU) << 13); }
        private void PokeF800(uint address, byte data) { sound.PokeF800(address, data); }

        protected override uint DecodeChr(uint address)
        {
            return (address & 0x03FFU) | chrPage[(address >> 10) & 7U];
        }
        protected override uint DecodePrg(uint address)
        {
            return (address & 0x1FFFU) | prgPage[(address >> 13) & 3U];
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();

            nmt_a = chr.b;
            nmt_b = chr.b;
            nmt_c = chr.b;
            nmt_d = chr.b;

            prgPage[0U] = 0x00000000U << 13;
            prgPage[1U] = 0x00000000U << 13;
            prgPage[2U] = 0x00000000U << 13;
            prgPage[3U] = 0xFFFFFFFFU << 13;

            for (uint i = 0x4800U; i <= 0xFFFFU; i++)
            {
                switch (i & 0xF800U)
                {
                case 0x4800: cpu.Hook(i, Peek4800, Poke4800); break;
                case 0x5000: cpu.Hook(i, Peek5000, Poke5000); break;
                case 0x5800: cpu.Hook(i, Peek5800, Poke5800); break;
                case 0x6000: break;
                case 0x6800: break;
                case 0x7000: break;
                case 0x7800: break;
                case 0x8000: cpu.Hook(i, Poke8000); break;
                case 0x8800: cpu.Hook(i, Poke8800); break;
                case 0x9000: cpu.Hook(i, Poke9000); break;
                case 0x9800: cpu.Hook(i, Poke9800); break;
                case 0xA000: cpu.Hook(i, PokeA000); break;
                case 0xA800: cpu.Hook(i, PokeA800); break;
                case 0xB000: cpu.Hook(i, PokeB000); break;
                case 0xB800: cpu.Hook(i, PokeB800); break;
                case 0xC000: cpu.Hook(i, PokeC000); break;
                case 0xC800: cpu.Hook(i, PokeC800); break;
                case 0xD000: cpu.Hook(i, PokeD000); break;
                case 0xD800: cpu.Hook(i, PokeD800); break;
                case 0xE000: cpu.Hook(i, PokeE000); break;
                case 0xE800: cpu.Hook(i, PokeE800); break; //  $E800:  [HLPP PPPP]
                case 0xF000: cpu.Hook(i, PokeF000); break;
                case 0xF800: cpu.Hook(i, PokeF800); break;
                }
            }

            gpu.Hook(0x2000U, 0x23FFU, PeekNmtA, PokeNmtA);
            gpu.Hook(0x2400U, 0x27FFU, PeekNmtB, PokeNmtB);
            gpu.Hook(0x2800U, 0x2BFFU, PeekNmtC, PokeNmtC);
            gpu.Hook(0x2C00U, 0x2FFFU, PeekNmtD, PokeNmtD);

            gpu.Hook(0x3000U, 0x33FFU, PeekNmtA, PokeNmtA);
            gpu.Hook(0x3400U, 0x37FFU, PeekNmtB, PokeNmtB);
            gpu.Hook(0x3800U, 0x3BFFU, PeekNmtC, PokeNmtC);
            gpu.Hook(0x3C00U, 0x3EFFU, PeekNmtD, PokeNmtD);

            spu.Hook(sound);

            sound.Initialize();
        }

        public override void Clock()
        {
            if (irq_enabled)
            {
                if (irq_counter == 0x7FFF)
                    cpu.Irq(1u);
                else
                    irq_counter++;
            }
        }

        public class Sound : Spu.ChannelExt
        {
            private Channel[] channels = new Channel[8U];
            private Timing timing;
            private bool step;
            private byte[] wave = new byte[256U];
            private byte[] wram = new byte[128U];
            private uint addr;
            private uint curr;

            public Sound(Famicom console)
                : base(console)
            {
                timing.Cycles =
                timing.Single = Spu.PHASE * 15;
                timing.Period = Spu.DELAY;

                for (uint i = 0U; i < 8U; i++)
                    channels[i] = new Channel(i);
            }

            protected override void OnInitialize()
            {
                base.OnInitialize();

                PokeF800(0xF800U, 0x80); // address: 0, auto-increment on

                for (int i = 0x00; i < 0x80; i++)
                    Poke4800(0x4800U, 0x00); // clear wram, and initialize sound channels

                PokeF800(0xF800U, 0x00); // address: 0, auto-increment off
            }

            public byte Peek4800(uint address)
            {
                var data = wram[addr];

                if (step)
                    addr = (++addr & 0x7FU);

                return data;
            }
            public void Poke4800(uint address, byte data)
            {
                wave[(addr << 1) | 0U] = (byte)(data & 15);
                wave[(addr << 1) | 1U] = (byte)(data >> 4);

                wram[addr] = data;

                if (addr > 0x3FU)
                {
                    var c = channels[(addr >> 3) & 7U];

                    switch (addr & 0x07)
                    {
                    case 0: c.rd.ub0 = (byte)(data); break;
                    case 1: c.rp.ub0 = (byte)(data); break;
                    case 2: c.rd.ub1 = (byte)(data); break;
                    case 3: c.rp.ub1 = (byte)(data); break;
                    case 4: c.rd.ub2 = (byte)(data & 0x03); c.count = 16777216U - ((data & 0xFCU) << 16); break;
                    case 5: c.rp.ub2 = (byte)(data); break;
                    case 6: c.index = (byte)(data); break;
                    case 7: c.level = (byte)(data & 0x0F); break;
                    }
                }

                if (step)
                    addr = (++addr & 0x7FU);
            }
            public void PokeF800(uint address, byte data)
            {
                addr = (data & 0x7FU);
                step = (data & 0x80U) != 0;
            }

            public override short Render()
            {
                var enable = (wram[0x7F] & 0x70U) >> 4;
                var output = 0;

                for (timing.Cycles -= timing.Period; timing.Cycles < 0; timing.Cycles += timing.Single)
                {
                    channels[7U - curr].Update(wram, wave);

                    if (++curr > enable)
                        curr = 0;
                }

                for (uint i = 7U - enable; i <= 7U; i++)
                    output += channels[i].Render();

                return (short)(((output * 32767) / 225) / (enable + 1));
            }

            public class Channel
            {
                public Register32 rd;
                public Register32 rp;
                public byte index;
                public byte level;
                public byte start;
                public byte value;
                public uint count;

                public Channel(uint ordinal)
                {
                    start = (byte)(0x40U | (ordinal << 3));
                }

                public byte Render()
                {
                    return (byte)(value * level);
                }
                public void Update(byte[] wram, byte[] wave)
                {
                    // -- Clock phase counter --
                    rp.ud0 += rd.ud0;
                    rp.ud0 %= count;

                    // -- Get next sample --
                    value = wave[(rp.ub2 + index) & 0xFFU];

                    // -- Write phase back into ram (This is detectable by games via $4800) --
                    wram[start | 1U] = rp.ub0;
                    wram[start | 3U] = rp.ub1;
                    wram[start | 5U] = rp.ub2;
                }
            }
        }
    }
}