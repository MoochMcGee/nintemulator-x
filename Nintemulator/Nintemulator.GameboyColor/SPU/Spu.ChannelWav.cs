using System;

namespace Nintemulator.GBC.SPU
{
    public partial class Spu
    {
        //        Wave
        // NR30 FF1A E--- ---- DAC power
        // NR31 FF1B LLLL LLLL Length load (256-L)
        // NR32 FF1C -VV- ---- Volume code (00=0%, 01=100%, 10=50%, 11=25%)
        // NR33 FF1D FFFF FFFF Frequency LSB
        // NR34 FF1E TL-- -FFF Trigger, Length enable, Frequency MSB

        public class ChannelWav : Channel
        {
            private static readonly int[] VolumeTable =
            {
                0x4,
                0x0,
                0x1,
                0x2
            };

            private byte[] amp = new byte[32];
            private byte[] ram = new byte[16];
            private int count;
            private int shift = VolumeTable[0];

            public ChannelWav(GameboyColor console, Timing.System system)
                : base(console, system, 2)
            {
                timing.Cycles =
                timing.Single = 2048 * PHASE / 2;
                timing.Period = DELAY;
            }

            protected override void OnPokeReg1(byte data)
            {
                if ((data & 0x80) == 0)
                    active = false;
            }
            protected override void OnPokeReg2(byte data)
            {
                duration.Refresh = data;
                duration.Counter = 256 - duration.Refresh;
            }
            protected override void OnPokeReg3(byte data)
            {
                shift = VolumeTable[data >> 5 & 0x3];
            }
            protected override void OnPokeReg4(byte data)
            {
                frequency = (frequency & ~0x0FF) | (data << 0 & 0x0FF);
                timing.Single = (2048 - frequency) * PHASE / 2;
            }
            protected override void OnPokeReg5(byte data)
            {
                frequency = (frequency & ~0x700) | (data << 8 & 0x700);
                timing.Single = (2048 - frequency) * PHASE / 2;

                if ((data & 0x80) != 0)
                {
                    active = true;
                    timing.Cycles = timing.Single;

                    if (duration.Counter == 0)
                        duration.Counter = 256;

                    count = 0;
                }

                duration.Enabled = (data & 0x40) != 0;

                if ((registers[0] & 0xF8) == 0)
                    active = false;
            }

            public byte Peek(uint address)
            {
                return ram[address & 0x0F];
            }
            public void Poke(uint address, byte data)
            {
                ram[address & 0x0F] = data;

                address = (address << 1) & 0x1E;

                amp[address | 0x00] = (byte)(data >> 4 & 0xF);
                amp[address | 0x01] = (byte)(data >> 0 & 0xF);
            }

            public override byte Sample()
            {
                if (active)
                {
                    var sum = timing.Cycles;
                    timing.Cycles -= timing.Period;

                    if (active)
                    {
                        if (timing.Cycles < 0)
                        {
                            sum *= amp[count] >> shift;

                            for (; timing.Cycles < 0; timing.Cycles += timing.Single)
                                sum += Math.Min(-timing.Cycles, timing.Single) * amp[count = (count + 1) & 0x1F] >> shift;

                            return (byte)(sum / timing.Period);
                        }
                    }
                    else if (timing.Cycles < 0)
                    {
                        var c = (~timing.Cycles + timing.Single) / timing.Single;
                        timing.Cycles += (c * timing.Single);
                    }
                }

                return (byte)(amp[count] >> shift);
            }
        }
    }
}