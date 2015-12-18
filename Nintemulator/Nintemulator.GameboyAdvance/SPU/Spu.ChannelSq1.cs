﻿using System;

namespace Nintemulator.GBA.SPU
{
    public partial class Spu
    {
        public class ChannelSq1 : Channel
        {
            private static readonly byte[][] DutyTable =
            {
                new byte[] { 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x00 },
                new byte[] { 0x00, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x00 },
                new byte[] { 0x00, 0x1F, 0x1F, 0x1F, 0x1F, 0x00, 0x00, 0x00 },
                new byte[] { 0x1F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F }
            };

            private int form;
            private int step = 7;

            private Timing sweepTiming;
            private bool sweepEnable;
            private int sweepDelta = 1;
            private int sweepShift;
            private int sweepShadow;

            public override bool Enabled
            {
                get { return active; }
            }

            public ChannelSq1(GameboyAdvance console, Timing timing)
                : base(console, timing)
            {
                this.timing.Cycles =
                this.timing.Period = (2048 - frequency) * 16 * timing.Single;
                this.timing.Single = timing.Single;

                this.sweepTiming.Single = 1;
            }
            
            protected override void PokeRegister1(uint address, byte data)
            {
                base.PokeRegister1(address, data);

                sweepTiming.Period = (data >> 4 & 0x7);
                sweepDelta = 1 - (data >> 2 & 0x2);
                sweepShift = (data >> 0 & 0x7);
            }
            protected override void PokeRegister2(uint address, byte data) { base.PokeRegister2(address, data); }
            protected override void PokeRegister3(uint address, byte data)
            {
                base.PokeRegister3(address, data);

                form = data >> 6;
                duration.Refresh = (data & 0x3F);
                duration.Counter = 64 - duration.Refresh;
            }
            protected override void PokeRegister4(uint address, byte data)
            {
                base.PokeRegister4(address, data);

                envelope.Level = (data >> 4 & 0xF);
                envelope.Delta = (data >> 2 & 0x2) - 1;
                envelope.Timing.Period = (data & 0x7);
            }
            protected override void PokeRegister5(uint address, byte data)
            {
                base.PokeRegister5(address, data);

                frequency = (frequency & 0x700) | (data << 0 & 0x0FF);
                timing.Period = (2048 - frequency) * 16 * timing.Single;
            }
            protected override void PokeRegister6(uint address, byte data)
            {
                base.PokeRegister6(address, data);

                frequency = (frequency & 0x0FF) | (data << 8 & 0x700);
                timing.Period = (2048 - frequency) * 16 * timing.Single;

                if ((data & 0x80) != 0)
                {
                    active = true;
                    timing.Cycles = timing.Period;

                    duration.Counter = 64 - duration.Refresh;
                    envelope.Timing.Cycles = envelope.Timing.Period;
                    envelope.CanUpdate = true;

                    sweepShadow = frequency;
                    sweepTiming.Cycles = sweepTiming.Period;
                    sweepEnable = (sweepShift != 0 || sweepTiming.Period != 0);

                    step = 7;
                }

                duration.Enabled = (data & 0x40) != 0;

                if ((registers[3] & 0xF8) == 0)
                    active = false;
            }

            public void ClockEnvelope()
            {
                envelope.Clock();
            }
            public void ClockSweep()
            {
                if (sweepTiming.ClockDown() && sweepEnable && sweepTiming.Period != 0)
                {
                    var result = sweepShadow + ((sweepShadow >> sweepShift) * sweepDelta);

                    if (result > 0x7FF)
                    {
                        active = false;
                    }
                    else if (sweepShift != 0)
                    {
                        sweepShadow = result;
                        timing.Period = (2048 - sweepShadow) * 16 * timing.Single;
                    }
                }
            }

            public override int Render(int cycles)
            {
                int sum = timing.Cycles;
                timing.Cycles -= cycles;

                if (active)
                {
                    if (timing.Cycles >= 0)
                    {
                        return (byte)(envelope.Level >> DutyTable[form][step]);
                    }
                    else
                    {
                        sum >>= DutyTable[form][step];

                        for (; timing.Cycles < 0; timing.Cycles += timing.Period)
                            sum += Math.Min(-timing.Cycles, timing.Period) >> DutyTable[form][step = (step - 1) & 0x7];

                        return (byte)((sum * envelope.Level) / cycles);
                    }
                }
                else
                {
                    for (; timing.Cycles < 0; timing.Cycles += timing.Period)
                        step = (step - 1) & 0x7;

                    return 0;
                }
            }
        }
    }
}