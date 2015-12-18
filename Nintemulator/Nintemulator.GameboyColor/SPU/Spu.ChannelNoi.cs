using System;

namespace Nintemulator.GBC.SPU
{
    public partial class Spu
    {
        //       Noise
        //     FF1F ---- ---- Not used
        //NR41 FF20 --LL LLLL Length load (64-L)
        //NR42 FF21 VVVV APPP Starting volume, Envelope add mode, period
        //NR43 FF22 SSSS WDDD Clock shift, Width mode of LFSR, Divisor code
        //NR44 FF23 TL-- ---- Trigger, Length enable

        public class ChannelNoi : Channel
        {
            private static readonly int[] DivisorTable =
            {
                0x08, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70
            };

            private int shift = 14;
            private int value = 0x0001;

            public ChannelNoi(GameboyColor console, Timing.System system)
                : base(console, system, 3)
            {
                timing.Cycles =
                timing.Single = DivisorTable[0] * PHASE;
                timing.Period = DELAY;
            }

            protected override void OnPokeReg2(byte data)
            {
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
                shift = 14 - (data & 0x08);
                timing.Single = (DivisorTable[data & 0x7] << (data >> 4)) * PHASE / 4;
            }
            protected override void OnPokeReg5(byte data)
            {
                if ((data & 0x80) != 0)
                {
                    active = true;
                    timing.Cycles = timing.Single;

                    if (duration.Counter == 0)
                        duration.Counter = 64;

                    envelope.Timing.Cycles = envelope.Timing.Period;
                    envelope.CanUpdate = true;

                    value = 0x7FFF;
                }

                duration.Enabled = (data & 0x40) != 0;

                if ((registers[2] & 0xF8) == 0)
                    active = false;
            }

            public override byte Sample()
            {
                var sum = timing.Cycles;
                timing.Cycles -= timing.Period;

                if ((registers[2] & 0xF8) != 0 && active)
                {
                    if (timing.Cycles >= 0)
                    {
                        if ((value & 0x1) == 0)
                            return (byte)envelope.Level;
                    }
                    else
                    {
                        if ((value & 0x1) != 0)
                            sum = 0;

                        for (; timing.Cycles < 0; timing.Cycles += timing.Single)
                        {
                            int feedback = (value ^ (value >> 1)) & 0x1;
                            value = ((value >> 1) | (feedback << shift));

                            if ((value & 0x1) == 0)
                                sum += Math.Min(-timing.Cycles, timing.Single);
                        }

                        return (byte)((sum * envelope.Level) / timing.Period);
                    }
                }
                else
                {
                    for (; timing.Cycles < 0; timing.Cycles += timing.Single)
                    {
                        int feedback = (value ^ (value >> 1)) & 0x1;
                        value = ((value >> 1) | (feedback << shift));
                    }
                }

                return 0;
            }

            public void ClockEnvelope()
            {
                envelope.Clock();
            }
        }
    }
}