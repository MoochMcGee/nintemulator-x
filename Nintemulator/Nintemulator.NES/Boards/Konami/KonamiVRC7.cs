using Nintemulator.FC.GPU;
using Nintemulator.FC.SPU;
using Nintemulator.Shared;
using System;
using System.Linq;

namespace Nintemulator.FC.Boards.Konami
{
    public class KonamiVRC7 : Board
    {
        private IRQ irq;
        private Sound sound;

        public KonamiVRC7(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            prgPage = new uint[4U];
            chrPage = new uint[8U];

            irq = new IRQ();
            sound = new Sound(console);
        }

        private void Poke8000(uint address, byte data) { prgPage[0U] = (data & 0xFFU) << 13; }
        private void Poke8010(uint address, byte data) { prgPage[1U] = (data & 0xFFU) << 13; }
        private void Poke9000(uint address, byte data) { prgPage[2U] = (data & 0xFFU) << 13; }
        private void Poke9010(uint address, byte data) { sound.WriteAddr(data); }
        private void Poke9030(uint address, byte data) { sound.WriteData(data); }
        private void PokeA000(uint address, byte data) { chrPage[0U] = (data & 0xFFU) << 10; }
        private void PokeA010(uint address, byte data) { chrPage[1U] = (data & 0xFFU) << 10; }
        private void PokeB000(uint address, byte data) { chrPage[2U] = (data & 0xFFU) << 10; }
        private void PokeB010(uint address, byte data) { chrPage[3U] = (data & 0xFFU) << 10; }
        private void PokeC000(uint address, byte data) { chrPage[4U] = (data & 0xFFU) << 10; }
        private void PokeC010(uint address, byte data) { chrPage[5U] = (data & 0xFFU) << 10; }
        private void PokeD000(uint address, byte data) { chrPage[6U] = (data & 0xFFU) << 10; }
        private void PokeD010(uint address, byte data) { chrPage[7U] = (data & 0xFFU) << 10; }
        private void PokeE000(uint address, byte data)
        {
            switch (data & 0x03U)
            {
            case 0U: gpu.SwitchNametables(Mirroring.ModeVert); break; // Vert
            case 1U: gpu.SwitchNametables(Mirroring.ModeHorz); break; // Horz
            case 2U: gpu.SwitchNametables(Mirroring.Mode1ScA); break; // 1ScA
            case 3U: gpu.SwitchNametables(Mirroring.Mode1ScB); break; // 1ScB
            }
        }
        private void PokeE010(uint address, byte data)
        {
            irq.refresh = data;
        }
        private void PokeF000(uint address, byte data)
        {
            irq.mode = (data & 0x04U) != 0;
            irq.enabled = (data & 0x02U) != 0;
            irq.enabledRefresh = (data & 0x01U) != 0;
            irq.scaler = 341;

            if (irq.enabled)
                irq.counter = irq.refresh;

            cpu.Irq(0u);
        }
        private void PokeF010(uint address, byte data)
        {
            irq.enabled = irq.enabledRefresh;
            cpu.Irq(0u);
        }

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

            prgPage[0U] = 0x00000000U << 13;
            prgPage[1U] = 0x00000000U << 13;
            prgPage[2U] = 0x00000000U << 13;
            prgPage[3U] = 0xFFFFFFFFU << 13; // -1

            uint pin11 = 1U << int.Parse(GetPin("VRC7", 0x11).Replace("PRG A", ""));

            for (uint i = 0x0000U; i <= 0x1FFFU; i++)
            {
                if ((i & pin11) == 0U)
                {
                    cpu.Hook(0x8000U | i, Poke8000);
                    cpu.Hook(0x9000U | i, Poke9000);
                    cpu.Hook(0xA000U | i, PokeA000);
                    cpu.Hook(0xB000U | i, PokeB000);
                    cpu.Hook(0xC000U | i, PokeC000);
                    cpu.Hook(0xD000U | i, PokeD000);
                    cpu.Hook(0xE000U | i, PokeE000);
                    cpu.Hook(0xF000U | i, PokeF000);
                }
                else
                {
                    cpu.Hook(0x8000U | i, Poke8010);
                    cpu.Hook(0x9000U | i, Poke9010);
                    cpu.Hook(0xA000U | i, PokeA010);
                    cpu.Hook(0xB000U | i, PokeB010);
                    cpu.Hook(0xC000U | i, PokeC010);
                    cpu.Hook(0xD000U | i, PokeD010);
                    cpu.Hook(0xE000U | i, PokeE010);
                    cpu.Hook(0xF000U | i, PokeF010);
                }

                cpu.Hook(0x9030U, Poke9030); // external audio data port for lagrange point, not sure how it's wired in
            }

            spu.Hook(sound);
        }

        public override void Clock()
        {
            if (!irq.enabled)
                return;

            if (irq.mode)
            {
                if (irq.Clock())
                    cpu.Irq(1u);
            }
            else
            {
                irq.scaler -= 3;

                if (irq.scaler <= 0)
                {
                    irq.scaler += 341;

                    if (irq.Clock())
                        cpu.Irq(1u);
                }
            }
        }

        public class IRQ
        {
            public bool mode;
            public bool enabled;
            public bool enabledRefresh;
            public int counter;
            public int refresh;
            public int scaler;

            public bool Clock()
            {
                if (counter == 0xFFU)
                {
                    counter = refresh;
                    return true;
                }
                else
                {
                    counter++;
                    return false;
                }
            }
        }
        public class Sound : Spu.ChannelExt
        {
            private const int CLOCK = 3579545;
            private const int CLOCK_DIV = CLOCK / 72 + 1;
            private const int RATE = 48000;
            private const int AM_DELTA = 37 * 65536 / CLOCK_DIV / 10; // ~3.7hz
            private const int PM_DELTA = 64 * 65536 / CLOCK_DIV / 10; // ~6.4hz

            private static readonly byte[][] Instruments = new byte[][]
            {
                new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 },
                new byte[] { 0x03, 0x21, 0x05, 0x06, 0xB8, 0x82, 0x42, 0x27 },
                new byte[] { 0x13, 0x41, 0x13, 0x0D, 0xD8, 0xD6, 0x23, 0x12 },
                new byte[] { 0x31, 0x11, 0x08, 0x08, 0xFA, 0x9A, 0x22, 0x02 },
                new byte[] { 0x31, 0x61, 0x18, 0x07, 0x78, 0x64, 0x30, 0x27 },
                new byte[] { 0x22, 0x21, 0x1E, 0x06, 0xF0, 0x76, 0x08, 0x28 },
                new byte[] { 0x02, 0x01, 0x06, 0x00, 0xF0, 0xF2, 0x03, 0xF5 },
                new byte[] { 0x21, 0x61, 0x1D, 0x07, 0x82, 0x81, 0x16, 0x07 },
                new byte[] { 0x23, 0x21, 0x1A, 0x17, 0xCF, 0x72, 0x25, 0x17 },
                new byte[] { 0x15, 0x11, 0x25, 0x00, 0x4F, 0x71, 0x00, 0x11 },
                new byte[] { 0x85, 0x01, 0x12, 0x0F, 0x99, 0xA2, 0x40, 0x02 },
                new byte[] { 0x07, 0xC1, 0x69, 0x07, 0xF3, 0xF5, 0xA7, 0x12 },
                new byte[] { 0x71, 0x23, 0x0D, 0x06, 0x66, 0x75, 0x23, 0x16 },
                new byte[] { 0x01, 0x02, 0xD3, 0x05, 0xA3, 0x92, 0xF7, 0x52 },
                new byte[] { 0x61, 0x63, 0x0C, 0x00, 0x94, 0xAF, 0x34, 0x06 },
                new byte[] { 0x21, 0x62, 0x0D, 0x00, 0xB1, 0xA0, 0x54, 0x17 }
		    };

            private static short[] adjustTable = new short[128];
            private static short[] db2linear = new short[512 * 2];
            private static short[,] waveform = new short[2, 512];
            private static int[] amtable = new int[256];
            private static int[] pmtable = new int[256];
            private static int[,] ar = new int[16, 16];
            private static int[,] dr = new int[16, 16];
            private static int[, ,] deltaTable = new int[512, 8, 16];
            private static int[, ,] slTable = new int[2, 8, 2];
            private static int[, , ,] tlTable = new int[16, 8, 64, 4];

            private Channel[] channels = new Channel[6];
            private Generator am = new Generator();
            private Generator pm = new Generator();
            private int addr;

            public Sound(Famicom console)
                : base(console)
            {
                for (int i = 0; i < 6; i++)
                {
                    channels[i] = new Channel();
                }

                MakeTables();

                Reset();
            }

            private static short linear2db(double linear)
            {
                return (short)((linear == 0) ? 255 : Math.Min(-(int)(20.0 * Math.Log10(linear) / 0.1875), 255));
            }
            private static void makeAdjustTable()
            {
                adjustTable[0] = 128;

                for (int i = 1; i < 128; i++)
                {
                    adjustTable[i] = (short)(128 - 1 - 128 * Math.Log(i) / Math.Log(128));
                }
            }
            private static void makeDB2LinTable()
            {
                for (int i = 0; i < 512; i++)
                {
                    db2linear[i] = (short)(Math.Pow(10, -i * 0.1875 / 20) * 255);

                    if (i >= 256)
                        db2linear[i] = 0;

                    db2linear[i + 512] = (short)(-db2linear[i]);
                }
            }
            private static void makeSinTable()
            {
                for (int i = 0x000; i < 0x080; i++) waveform[0, i] = linear2db(Math.Sin(MathHelper.Tau * i / 512));
                for (int i = 0x000; i < 0x080; i++) waveform[0, 255 - i] = (short)(waveform[0, i]);
                for (int i = 0x000; i < 0x100; i++) waveform[0, 256 + i] = (short)(waveform[0, i] + 512);
                for (int i = 0x000; i < 0x100; i++) waveform[1, i] = waveform[0, i];
                for (int i = 0x100; i < 0x200; i++) waveform[1, i] = waveform[0, 0];
            }
            private static void makeAmTable()
            {
                for (int i = 0; i < 256; i++)
                    amtable[i] = (int)(4.875 / 2 / 0.1875 * (1 + Math.Sin(MathHelper.Tau * i / 256)));
            }
            private static void makePmTable()
            {
                for (int i = 0; i < 256; i++)
                    pmtable[i] = (int)(256 * Math.Pow(2, Math.Sin(MathHelper.Tau * i / 256) * 13.75 / 1200));
            }
            private static void makeDeltaTable()
            {
                var lut = new int[]
                {
                    0x01, 0x02, 0x04, 0x06,
                    0x08, 0x0A, 0x0C, 0x0E,
                    0x10, 0x12, 0x14, 0x14,
                    0x18, 0x18, 0x1E, 0x1E
                };

                for (int f = 0; f < 512; f++)
                {
                    for (int b = 0; b < 8; b++)
                    {
                        for (int m = 0; m < 16; m++)
                        {
                            deltaTable[f, b, m] = (((f * lut[m]) << b) >> 2) * CLOCK_DIV / RATE;
                        }
                    }
                }
            }
            private static void makeRateTable()
            {
                for (int i = 0; i < 16; i++)
                {
                    for (int j = 0; j < 16; j++)
                    {
                        int rm = i + (j >> 2);
                        int rl = j & 3;

                        if (rm > 15)
                            rm = 15;

                        if (i == 0)
                        {
                            ar[i, j] = 0;
                            dr[i, j] = 0;
                        }
                        else if (i == 15)
                        {
                            ar[i, j] = 0;
                            dr[i, j] = (int)(((uint)(1 * (rl + 4) << (rm - 1)) * CLOCK_DIV) / RATE);
                        }
                        else
                        {
                            ar[i, j] = (int)(((uint)(3 * (rl + 4) << (rm + 1)) * CLOCK_DIV) / RATE);
                            dr[i, j] = (int)(((uint)(1 * (rl + 4) << (rm - 1)) * CLOCK_DIV) / RATE);
                        }
                    }
                }
            }
            private static void makeSlTable()
            {
                for (int f = 0; f < 2; f++)
                {
                    for (int b = 0; b < 8; b++)
                    {
                        slTable[f, b, 0] = (b >> 1);
                        slTable[f, b, 1] = (b << 1) + f;
                    }
                }
            }
            private static void makeTlTable()
            {
                var lut = new int[]
                {
				        0, 18000, 24000, 27750,
                    30000, 32250, 33750, 35250,
				    36000, 37500, 38250, 39000,
                    39750, 40500, 41250, 42000
			    };

                for (int f = 0; f < 16; f++)
                {
                    for (int b = 0; b < 8; b++)
                    {
                        for (int t = 0; t < 64; t++)
                        {
                            tlTable[f, b, t, 0] = (t * 2);

                            for (int k = 1; k < 4; k++)
                            {
                                var tmp = (lut[f] - 6000 * (b ^ 7)) / 1000;

                                if (tmp > 0)
                                    tlTable[f, b, t, k] = (t * 2) + ((tmp >> (k ^ 3)) * 8 / 3);
                                else
                                    tlTable[f, b, t, k] = (t * 2);
                            }
                        }
                    }
                }
            }
            private static void MakeTables()
            {
                makeAmTable();
                makePmTable();
                makeTlTable();
                makeSlTable();
                makeSinTable();
                makeRateTable();
                makeDeltaTable();
                makeAdjustTable();
                makeDB2LinTable();
            }

            public void WriteAddr(byte data) { addr = data; }
            public void WriteData(byte data)
            {
                if (addr < 8)
                    Instruments[0][addr] = data;

                switch (addr)
                {
                case 0x00: foreach (var channel in channels.Where(o => o.patch == 0)) { channel.Update(0); } break;
                case 0x01: foreach (var channel in channels.Where(o => o.patch == 0)) { channel.Update(1); } break;
                case 0x02: foreach (var channel in channels.Where(o => o.patch == 0)) { channel.UpdateTL(0); } break;
                case 0x03: break;
                case 0x04: foreach (var channel in channels.Where(o => o.patch == 0)) { channel.UpdateEg(0); } break;
                case 0x05: foreach (var channel in channels.Where(o => o.patch == 0)) { channel.UpdateEg(1); } break;
                case 0x06: foreach (var channel in channels.Where(o => o.patch == 0)) { channel.UpdateEg(0); } break;
                case 0x07: foreach (var channel in channels.Where(o => o.patch == 0)) { channel.UpdateEg(1); } break;

                case 0x10:
                case 0x11:
                case 0x12:
                case 0x13:
                case 0x14:
                case 0x15: channels[addr & 7].WriteReg0(data); break;
                case 0x20:
                case 0x21:
                case 0x22:
                case 0x23:
                case 0x24:
                case 0x25: channels[addr & 7].WriteReg1(data); break;
                case 0x30:
                case 0x31:
                case 0x32:
                case 0x33:
                case 0x34:
                case 0x35: channels[addr & 7].WriteReg2(data); break;
                }
            }

            public override short Render()
            {
                am.count = (int)(am.count + AM_DELTA) & 0xFFFF;
                pm.count = (int)(pm.count + PM_DELTA) & 0xFFFF;
                am.level = amtable[am.count >> 8];
                pm.level = pmtable[pm.count >> 8];

                int output = 0;

                foreach (var channel in channels)
                {
                    channel.CalculatePg(pm.level);
                    channel.CalculateEg(am.level);

                    if (channel.slots[1].eg_state != State.Finish)
                        output += channel.GetSample();
                }

                return (short)(output << 4);
            }

            public void Reset()
            {
                am.count = 0;
                pm.count = 0;

                foreach (var channel in channels)
                {
                    channel.slots[0] = new Slot();
                    channel.slots[1] = new Slot();
                }

                for (addr = 0; addr < 0x40; addr++)
                {
                    WriteData(0);
                }

                addr = 0;
            }

            public class Channel
            {
                public Slot[] slots = new Slot[2];
                public bool sustain;
                public byte[] instrument;
                public int block;
                public int feedback;
                public int frequency;
                public int key;
                public int patch;
                public int sound;

                public Channel()
                {
                    slots[0] = new Slot();
                    slots[1] = new Slot();

                    instrument = Instruments[0];
                }

                public void CalculateEg(int lfo)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        var slot = slots[i];

                        int egout;

                        switch (slot.eg_state)
                        {
                        case State.A:
                            egout = adjustTable[slot.eg.count >> 15];
                            slot.eg.count += slot.eg.delta;

                            if ((slot.eg.count & (1 << 22)) != 0 || ((instrument[4 + i] >> 4) == 15))
                            {
                                egout = 0;
                                slot.eg.count = 0;
                                slot.eg_state = State.D;
                                UpdateEg(i);
                            }
                            break;

                        case State.D:
                            egout = slot.eg.count >> 15;
                            slot.eg.count += slot.eg.delta;

                            int sl = (instrument[6 + i] >> 4) << 18;

                            if (slot.eg.count >= sl)
                            {
                                slot.eg.count = sl;
                                slot.eg_state = (instrument[0 + i] & 0x20) != 0 ? State.H : State.S;
                                UpdateEg(i);
                            }
                            break;

                        case State.H:
                            egout = slot.eg.count >> 15;

                            if ((instrument[0 + i] & 0x20) == 0)
                            {
                                slot.eg_state = State.S;
                                UpdateEg(i);
                            }
                            break;

                        case State.S:
                        case State.R:
                            egout = slot.eg.count >> 15;
                            slot.eg.count += slot.eg.delta;

                            if (egout >= (1 << 7))
                            {
                                slot.eg_state = State.Finish;
                                egout = (1 << 7) - 1;
                            }
                            break;

                        case State.Settle:
                            egout = slot.eg.count >> 15;
                            slot.eg.count += slot.eg.delta;

                            if (egout >= (1 << 7))
                            {
                                slot.eg_state = State.A;
                                egout = (1 << 7) - 1;
                                UpdateEg(i);
                            }
                            break;

                        default: egout = 0x7F; break;
                        }

                        if ((instrument[0 + i] & 0x80) != 0)
                            egout = ((egout + slot.tl) * 2) + lfo;
                        else
                            egout = ((egout + slot.tl) * 2);

                        if (egout > 0xFF)
                            egout = 0xFF;

                        slot.eg.level = egout;
                    }
                }
                public void CalculatePg(int lfo)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        var slot = slots[i];

                        if ((instrument[0 + i] & 0x40) != 0)
                            slot.pg.count += (slot.pg.delta * lfo) >> 8;
                        else
                            slot.pg.count += (slot.pg.delta);

                        slot.pg.count &= 0x3FFFF;
                        slot.pg.level = slot.pg.count >> 9;
                    }
                }
                public int GetSample()
                {
                    var slot = slots[0];

                    if (slot.eg.level >= 0xFF)
                    {
                        slot.output[0] = 0;
                    }
                    else if ((instrument[3] & 7) != 0)
                    {
                        int fm = (feedback << 2) >> (7 - (instrument[3] & 7));
                        slot.output[0] = db2linear[waveform[(instrument[3] >> 3) & 1, (slot.pg.level + fm) & 0x1FF] + slot.eg.level];
                    }
                    else
                    {
                        slot.output[0] = db2linear[waveform[(instrument[3] >> 3) & 1, (slot.pg.level)] + slot.eg.level];
                    }

                    feedback = (slot.output[1] + slot.output[0]) / 2;
                    slot.output[1] = slot.output[0];

                    slot = slots[1];

                    if (slot.eg.level >= 0xFF)
                    {
                        slot.output[0] = 0;
                    }
                    else
                    {
                        slot.output[0] = db2linear[waveform[(instrument[3] >> 4) & 1, (slot.pg.level + (feedback << 3)) & 0x1FF] + slot.eg.level];
                    }

                    return slot.output[1] = (slot.output[1] + slot.output[0]) / 2;
                }
                public void UpdatePg(int i)
                {
                    slots[i].pg.delta = deltaTable[frequency, block, instrument[0 + i] & 15];
                }
                public void UpdateTL(int i)
                {
                    slots[i].tl = tlTable[frequency >> 5, block, i == 0 ? (instrument[2] & 63) : sound, (instrument[2 + i] >> 6) & 3];
                }
                public void UpdateSL(int i)
                {
                    slots[i].sl = slTable[frequency >> 8, block, (instrument[0 + i] >> 4) & 1];
                }
                public void UpdateEg(int i)
                {
                    var slot = slots[i];

                    switch (slot.eg_state)
                    {
                    case State.A: slot.eg.delta = ar[instrument[4 + i] >> 4, slot.sl]; break;
                    case State.D: slot.eg.delta = dr[instrument[4 + i] & 15, slot.sl]; break;
                    case State.H: slot.eg.delta = 0; break;
                    case State.S: slot.eg.delta = dr[instrument[6 + i] & 15, slot.sl]; break;
                    case State.R:
                        if (sustain)
                            slot.eg.delta = dr[5, slot.sl];
                        else if ((instrument[0 + i] & 0x20) != 0)
                            slot.eg.delta = dr[instrument[6 + i] & 15, slot.sl];
                        else
                            slot.eg.delta = dr[7, slot.sl];
                        break;

                    case State.Settle: slot.eg.delta = dr[15, 0]; break;
                    default: slot.eg.delta = 0; break;
                    }
                }
                public void Update()
                {
                    Update(0);
                    Update(1);
                }
                public void Update(int s)
                {
                    UpdatePg(s);
                    UpdateTL(s);
                    UpdateSL(s);
                    UpdateEg(s); // EG should be updated last
                }
                public void WriteReg0(byte data)
                {
                    frequency = (frequency & 0x100) | (data << 0 & 0x0FF);

                    Update();
                }
                public void WriteReg1(byte data)
                {
                    frequency = (frequency & 0x0FF) | (data << 8 & 0x100);
                    block = (data & 0x0E) >> 1;
                    sustain = (data & 0x20) != 0;

                    if (key != (data & 0x10))
                    {
                        key = (data & 0x10);

                        if (key != 0)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                slots[i].eg_state = State.A;
                                slots[i].eg.count = 0;
                                slots[i].pg.count = 0;

                                UpdateEg(i);
                            }
                        }
                        else
                        {
                            if (slots[1].eg_state == State.A)
                                slots[1].eg.count = adjustTable[slots[1].eg.count >> 15] << 15;

                            slots[1].eg_state = State.R;

                            UpdateEg(1);
                        }
                    }

                    Update();
                }
                public void WriteReg2(byte data)
                {
                    patch = (data >> 4) & 0x0F;
                    sound = (data << 2) & 0x3C;

                    instrument = Instruments[patch];

                    Update();
                }
            }
            public class Generator
            {
                public int count;
                public int delta;
                public int level;
            }
            public class Slot
            {
                public Generator eg = new Generator() { count = 1 << 22 };
                public Generator pg = new Generator();
                public State eg_state; // Current state
                public int sl;
                public int tl;
                public int[] output = new int[2]; // Output value of slot
            }

            public enum State { Finish, A, D, H, S, R, Settle }
        }
    }
}