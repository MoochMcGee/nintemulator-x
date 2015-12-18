using System;

namespace Nintemulator.GBC.SPU
{
    public partial class Spu
    {
        //        Square 2
        //      FF15 ---- ---- Not used
        // NR21 FF16 DDLL LLLL Duty, Length load (64-L)
        // NR22 FF17 VVVV APPP Starting volume, Envelope add mode, period
        // NR23 FF18 FFFF FFFF Frequency LSB
        // NR24 FF19 TL-- -FFF Trigger, Length enable, Frequency MSB

        public class ChannelSq2 : Channel
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

            public ChannelSq2(GameboyColor console, Timing.System system)
                : base(console, system, 1)
            {
                timing.Cycles =
                timing.Single = PHASE * 2048;
                timing.Period = DELAY;
            }

            protected override void OnPokeReg2(byte data)
            {
                form = data >> 6;
                duration.Refresh = (data & 0x3F);
                duration.Counter = 64 - duration.Refresh;
            }
            protected override void OnPokeReg3(byte data)
            {
                envelope.Level = (data >> 4 & 0xF);
                envelope.Delta = (data >> 2 & 0x2) - 1;
                envelope.Timing.Period = (data & 0x7);

                if ((registers[2] & 0xF8) == 0)
                    active = false;
            }
            protected override void OnPokeReg4(byte data)
            {
                frequency = (frequency & 0x700) | (data << 0 & 0x0FF);
                timing.Single = (2048 - frequency) * PHASE;
            }
            protected override void OnPokeReg5(byte data)
            {
                frequency = (frequency & 0x0FF) | (data << 8 & 0x700);
                timing.Single = (2048 - frequency) * PHASE;

                if ((data & 0x80) != 0)
                {
                    active = true;
                    timing.Cycles = timing.Single;

                    if (duration.Counter == 0)
                        duration.Counter = 64;

                    envelope.Timing.Cycles = envelope.Timing.Period;
                    envelope.CanUpdate = true;

                    step = 7;
                }

                duration.Enabled = (data & 0x40) != 0;

                if ((registers[2] & 0xF8) == 0)
                    active = false;
            }

            public override byte Sample()
            {
                int sum = timing.Cycles;
                timing.Cycles -= timing.Period;

                if ((registers[2] & 0xF8) != 0 && active)
                {
                    if (timing.Cycles >= 0)
                    {
                        return (byte)(envelope.Level >> DutyTable[form][step]);
                    }
                    else
                    {
                        sum >>= DutyTable[form][step];

                        for (; timing.Cycles < 0; timing.Cycles += timing.Single)
                            sum += Math.Min(-timing.Cycles, timing.Single) >> DutyTable[form][step = (step - 1) & 0x7];

                        return (byte)((sum * envelope.Level) / timing.Period);
                    }
                }
                else
                {
                    var count = (~timing.Cycles + timing.Single) / timing.Single;

                    step = (step - count) & 0x7;
                    timing.Cycles += (count * timing.Single);

                    return 0;
                }
            }

            public void ClockEnvelope()
            {
                envelope.Clock();
            }
        }
    }
}