﻿using Nintemulator.Shared;
using System;

namespace Nintemulator.GBA.SPU
{
    public partial class Spu
    {
        public class ChannelRam : Channel
        {
            private static readonly int[] VolumeTable =
            {
                0x4,
                0x0,
                0x1,
                0x2
            };

            private byte[][] amp = Utility.CreateArray<byte>(2, 32);
            private byte[][] ram = Utility.CreateArray<byte>(2, 16);
            private int bank;
            private int count;
            private int dimension;
            private int shift = VolumeTable[0];

            public ChannelRam(GameboyAdvance console, Timing timing)
                : base(console, timing)
            {
                this.timing.Cycles =
                this.timing.Period = (2048 - frequency) * 8 * timing.Single;
                this.timing.Single = timing.Single;
            }

            public byte Peek(uint address)
            {
                return ram[bank ^ 1][address & 0x0F];
            }
            public void Poke(uint address, byte data)
            {
                ram[bank ^ 1][address & 0x0F] = data;

                address = (address << 1) & 0x1E;

                amp[bank ^ 1][address | 0x00] = (byte)(data >> 4 & 0xF);
                amp[bank ^ 1][address | 0x01] = (byte)(data >> 0 & 0xF);
            }

            protected override byte PeekRegister1(uint address)
            {
                byte data = 0;

                if (dimension != 0) data |= 0x20;
                if (bank != 0) data |= 0x40;

                return data;
            }
            protected override void PokeRegister1(uint address, byte data)
            {
                base.PokeRegister1(address, data);

                dimension = (data >> 5) & 1;
                bank = (data >> 6) & 1;

                if ((data & 0x80) == 0)
                    active = false;
            }
            protected override void PokeRegister2(uint address, byte data) { }
            protected override void PokeRegister3(uint address, byte data)
            {
                base.PokeRegister3(address, data);

                duration.Refresh = data;
                duration.Counter = 256 - duration.Refresh;
            }
            protected override void PokeRegister4(uint address, byte data)
            {
                base.PokeRegister4(address, data);

                shift = VolumeTable[data >> 5 & 0x3];
            }
            protected override void PokeRegister5(uint address, byte data)
            {
                base.PokeRegister5(address, data);

                frequency = (frequency & ~0x0FF) | (data << 0 & 0x0FF);
                timing.Period = (2048 - frequency) * 8 * timing.Single;
            }
            protected override void PokeRegister6(uint address, byte data)
            {
                base.PokeRegister6(address, data);

                frequency = (frequency & ~0x700) | (data << 8 & 0x700);
                timing.Period = (2048 - frequency) * 8 * timing.Single;

                if ((data & 0x80) != 0)
                {
                    active = true;
                    timing.Cycles = timing.Single;

                    duration.Counter = 256 - duration.Refresh;

                    count = 0;
                }

                duration.Enabled = (data & 0x40) != 0;
            }
            protected override void PokeRegister7(uint address, byte data) { }
            protected override void PokeRegister8(uint address, byte data) { }

            public override int Render(int cycles)
            {
                if ((registers[0] & 0x80) != 0)
                {
                    var sum = timing.Cycles;
                    timing.Cycles -= cycles;

                    if (active)
                    {
                        if (timing.Cycles < 0)
                        {
                            sum *= amp[bank][count] >> shift;

                            for (; timing.Cycles < 0; timing.Cycles += timing.Period)
                            {
                                count = (count + 1) & 0x1F;

                                if (count == 0)
                                    bank ^= dimension;

                                sum += Math.Min(-timing.Cycles, timing.Period) * amp[bank][count] >> shift;
                            }

                            return (byte)(sum / cycles);
                        }
                    }
                    else if (timing.Cycles < 0)
                    {
                        var c = (~timing.Cycles + timing.Single) / timing.Single;
                        timing.Cycles += (c * timing.Single);
                    }
                }

                return (byte)(amp[bank][count] >> shift);
            }
        }
    }
}