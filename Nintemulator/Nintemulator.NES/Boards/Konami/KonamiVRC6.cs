using Nintemulator.FC.GPU;
using Nintemulator.FC.SPU;
using System;

namespace Nintemulator.FC.Boards.Konami
{
    public class KonamiVRC6 : Board
    {
        private IRQ irq;
        private Sound sound;

        public KonamiVRC6(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            chrPage = new uint[8U];
            prgPage = new uint[3U];

            irq = new IRQ();
            sound = new Sound(console, Famicom.NTSC);
        }

        private void Poke8000(uint address, byte data) { prgPage[0U] = (data & 0xFFU) << 14; }
        private void PokeB003(uint address, byte data)
        {
            switch ((data >> 2) & 0x03U)
            {
            case 0U: gpu.SwitchNametables(Mirroring.ModeVert); break;
            case 1U: gpu.SwitchNametables(Mirroring.ModeHorz); break;
            case 2U: gpu.SwitchNametables(Mirroring.Mode1ScA); break;
            case 3U: gpu.SwitchNametables(Mirroring.Mode1ScB); break;
            }
        }
        private void PokeC000(uint address, byte data) { prgPage[1U] = (data & 0xFFU) << 13; }
        private void PokeD000(uint address, byte data) { chrPage[0U] = (data & 0xFFU) << 10; }
        private void PokeD001(uint address, byte data) { chrPage[1U] = (data & 0xFFU) << 10; }
        private void PokeD002(uint address, byte data) { chrPage[2U] = (data & 0xFFU) << 10; }
        private void PokeD003(uint address, byte data) { chrPage[3U] = (data & 0xFFU) << 10; }
        private void PokeE000(uint address, byte data) { chrPage[4U] = (data & 0xFFU) << 10; }
        private void PokeE001(uint address, byte data) { chrPage[5U] = (data & 0xFFU) << 10; }
        private void PokeE002(uint address, byte data) { chrPage[6U] = (data & 0xFFU) << 10; }
        private void PokeE003(uint address, byte data) { chrPage[7U] = (data & 0xFFU) << 10; }
        private void PokeF000(uint address, byte data)
        {
            irq.refresh = data;
        }
        private void PokeF001(uint address, byte data)
        {
            irq.mode = (data & 0x04U) != 0;
            irq.enabled = (data & 0x02U) != 0;
            irq.enabledRefresh = (data & 0x01U) != 0;
            irq.scaler = 341;

            if (irq.enabled)
                irq.counter = irq.refresh;

            cpu.Irq(0u);
        }
        private void PokeF002(uint address, byte data)
        {
            irq.enabled = irq.enabledRefresh;
            cpu.Irq(0u);
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

        protected override uint DecodeChr(uint address)
        {
            return (address & 0x03FFU) | chrPage[(address >> 10) & 7U];
        }
        protected override uint DecodePrg(uint address)
        {
            switch (address & 0xE000U)
            {
            case 0x8000U:
            case 0xA000U: return (address & 0x3FFFU) | prgPage[0U];
            case 0xC000U: return (address & 0x1FFFU) | prgPage[1U];
            case 0xE000U: return (address & 0x1FFFU) | prgPage[2U];
            }

            return base.DecodePrg(address);
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();

            prgPage[0U] = 0x00000000U << 13;
            prgPage[1U] = 0x00000000U << 13;
            prgPage[2U] = 0xFFFFFFFFU << 13; // -1

            uint pin9 = 1U << int.Parse(GetPin("VRC6", 0x9).Replace("PRG A", ""));
            uint pinA = 1U << int.Parse(GetPin("VRC6", 0xA).Replace("PRG A", ""));
            uint pins = pin9 | pinA;

            for (uint i = 0x0000U; i <= 0x1FFFU; i++)
            {
                if ((i & pins) == 0x0U)
                {
                    cpu.Hook(0x8000U | i, Poke8000);
                    cpu.Hook(0x9000U | i, sound.Sq1.PokeReg1);
                    cpu.Hook(0xA000U | i, sound.Sq2.PokeReg1);
                    cpu.Hook(0xB000U | i, sound.Saw.PokeReg1);
                    cpu.Hook(0xC000U | i, PokeC000);
                    cpu.Hook(0xD000U | i, PokeD000);
                    cpu.Hook(0xE000U | i, PokeE000);
                    cpu.Hook(0xF000U | i, PokeF000);
                }
                else if ((i & pins) == pinA)
                {
                    cpu.Hook(0x8000U | i, Poke8000);
                    cpu.Hook(0x9000U | i, sound.Sq1.PokeReg2);
                    cpu.Hook(0xA000U | i, sound.Sq2.PokeReg2);
                    cpu.Hook(0xB000U | i, sound.Saw.PokeReg2);
                    cpu.Hook(0xC000U | i, PokeC000);
                    cpu.Hook(0xD000U | i, PokeD001);
                    cpu.Hook(0xE000U | i, PokeE001);
                    cpu.Hook(0xF000U | i, PokeF001);
                }
                else if ((i & pins) == pin9)
                {
                    cpu.Hook(0x8000U | i, Poke8000);
                    cpu.Hook(0x9000U | i, sound.Sq1.PokeReg3);
                    cpu.Hook(0xA000U | i, sound.Sq2.PokeReg3);
                    cpu.Hook(0xB000U | i, sound.Saw.PokeReg3);
                    cpu.Hook(0xC000U | i, PokeC000);
                    cpu.Hook(0xD000U | i, PokeD002);
                    cpu.Hook(0xE000U | i, PokeE002);
                    cpu.Hook(0xF000U | i, PokeF002);
                }
                else if ((i & pins) == pins)
                {
                    cpu.Hook(0x8000U | i, Poke8000);
                    // $9000
                    // $A000
                    cpu.Hook(0xB000U | i, PokeB003);
                    cpu.Hook(0xC000U | i, PokeC000);
                    cpu.Hook(0xD000U | i, PokeD003);
                    cpu.Hook(0xE000U | i, PokeE003);
                    // $F000
                }
            }

            spu.Hook(sound);
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
            public SqrChannel Sq1;
            public SqrChannel Sq2;
            public SawChannel Saw;

            public Sound(Famicom console, Timing.System system)
                : base(console)
            {
                Sq1 = new SqrChannel(console, system);
                Sq2 = new SqrChannel(console, system);
                Saw = new SawChannel(console, system);
            }

            public override short Render()
            {
                var output = 0;

                output += (Sq1.Render() * 32767) / 15;
                output += (Sq2.Render() * 32767) / 15;
                output += (Saw.Render() * 32767) / 31;

                return (short)(output / 3);
            }

            public class SqrChannel : Spu.Channel
            {
                private static int[][] DutyTable =
                {
                    new int[] { 0x00, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, },
                    new int[] { 0x00, 0x00, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, },
                    new int[] { 0x00, 0x00, 0x00, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, },
                    new int[] { 0x00, 0x00, 0x00, 0x00, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, },
                    new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, },
                    new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, },
                    new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, },
                    new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, },
                    //-- 'digital' mode
                    new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, },
                    new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, },
                    new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, },
                    new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, },
                    new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, },
                    new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, },
                    new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, },
                    new int[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, },
                };

                private bool enabled;
                private int form;
                private int step;
                private int level;

                public SqrChannel(Famicom console, Timing.System system)
                    : base(console, system)
                {
                    timing.Cycles =
                    timing.Single = Spu.PHASE;
                    timing.Period = Spu.DELAY;
                }

                public override void PokeReg1(uint address, byte data)
                {
                    form = data >> 4 & 0xF;
                    level = data & 0xF;
                }
                public override void PokeReg2(uint address, byte data)
                {
                    frequency = (frequency & ~0x0FF) | (data << 0 & 0x0FF);
                    timing.Single = (frequency + 1) * Spu.PHASE;
                }
                public override void PokeReg3(uint address, byte data)
                {
                    frequency = (frequency & ~0xF00) | (data << 8 & 0xF00);
                    timing.Single = (frequency + 1) * Spu.PHASE;

                    enabled = (data & 0x80) != 0;
                }
                public override byte Render()
                {
                    var sum = timing.Cycles;
                    timing.Cycles -= timing.Period;

                    if (enabled)
                    {
                        if (timing.Cycles >= 0)
                        {
                            return (byte)(level >> DutyTable[form][step]);
                        }
                        else
                        {
                            sum >>= DutyTable[form][step];

                            for (; timing.Cycles < 0; timing.Cycles += timing.Single)
                                sum += Math.Min(-timing.Cycles, timing.Single) >> DutyTable[form][step = (step + 1) & 0xF];

                            return (byte)((sum * level) / timing.Period);
                        }
                    }
                    else
                    {
                        var count = (~timing.Cycles + timing.Single) / timing.Single;

                        step = (step + count) & 0xF;

                        timing.Cycles += count * timing.Single;
                    }

                    return 0;
                }
            }
            public class SawChannel : Spu.Channel
            {
                private bool enabled;
                private int accum;
                private int rate;
                private int step;

                public SawChannel(Famicom console, Timing.System system)
                    : base(console, system)
                {
                    timing.Cycles =
                    timing.Single = Spu.PHASE;
                    timing.Period = Spu.DELAY;
                }

                public override void PokeReg1(uint address, byte data)
                {
                    rate = data & 0x3F;
                }
                public override void PokeReg2(uint address, byte data)
                {
                    frequency = (frequency & ~0x0FF) | (data << 0 & 0x0FF);
                    timing.Single = (frequency + 1) * Spu.PHASE;
                }
                public override void PokeReg3(uint address, byte data)
                {
                    frequency = (frequency & ~0xF00) | (data << 8 & 0xF00);
                    timing.Single = (frequency + 1) * Spu.PHASE;

                    enabled = (data & 0x80) != 0;
                }
                public override byte Render()
                {
                    var sum = timing.Cycles;
                    timing.Cycles -= timing.Period;

                    if (timing.Cycles >= 0)
                    {
                        return (byte)(accum >> 3 & 0x1F);
                    }
                    else
                    {
                        sum *= (accum >> 3);

                        for (; timing.Cycles < 0; timing.Cycles += timing.Single)
                        {
                            step = (step + 1) % 14;
                            accum = (rate * (step >> 1));

                            sum += Math.Min(-timing.Cycles, timing.Single) * (accum >> 3 & 0x1F);
                        }
                    }

                    if (enabled)
                        return (byte)(sum / timing.Period);

                    return 0;
                }
            }
        }
    }
}