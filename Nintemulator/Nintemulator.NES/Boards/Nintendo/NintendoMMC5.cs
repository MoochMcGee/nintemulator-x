using Nintemulator.FC.SPU;
using Nintemulator.Shared;
using System;

namespace Nintemulator.FC.Boards.Nintendo
{
    // TODO List:
    // - Fully Implement PRG regs (ROM/RAM select)
    // - Implement Split Screen

    // Writable Regs:
    //   $5114-5117:  PRG regs [Partial Implementation]
    //   $5200:       Split Screen control
    //   $5201:       Split Screen V Scroll
    //   $5202:       Split Screen CHR Page

    // file://E:\Emulation\Documents\NES\Mappers\005.txt
    public class NintendoMMC5 : Board
    {
        private Register16 product;
        private Sound sound;
        private byte multiplierA;
        private byte multiplierB;

        private byte[] eram = new byte[1024U];
        private byte[] nmta;
        private byte[] nmtb;
        private uint[] nmt_indice = new uint[4U];
        private uint chrMode;
        private uint chrHigh;
        private uint chrSelect;
        private uint extMode;
        private uint prgMode;
        private uint ramLock;
        private byte fill_tile;
        private byte fill_attr;

        private bool irq_enabled;
        private bool irq_inframe;
        private bool irq_pending;
        private uint irq_compare;
        private uint irq_counter;

        public NintendoMMC5(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            chrPage = new uint[12U];
            prgPage = new uint[4U];
            ramPage = new uint[1U];

            sound = new Sound(console, Famicom.NTSC);
        }

        private byte PeekERam(uint address)
        {
            switch (extMode)
            {
            default:
            case 0U:
            case 1U: return cpu.Open;
            case 2U:
            case 3U: return eram[address & 0x3FFU];
            }
        }
        private byte PeekName(uint address)
        {
            if (extMode == 1 && (address & 0x23C0U) == 0x23C0U)
            {
                return (byte)(((eram[gpu.scroll.addr & 0x3FFU] & 0xC0U) >> 6) * 0x55U);
            }

            switch (nmt_indice[(address >> 10) & 3U])
            {
            default:
            case 0U: return nmta[address & 0x3FFU];
            case 1U: return nmtb[address & 0x3FFU];
            case 2U:
                switch (extMode)
                {
                default:
                case 0U: return eram[address & 0x3FFU];
                case 1U: return eram[address & 0x3FFU];
                case 2U: return 0x00;
                case 3U: return 0x00;
                }

            case 3U:
                if ((address & 0x23C0U) == 0x23C0U)
                    return (byte)(fill_attr * 0x55U);
                else
                    return (byte)(fill_tile);
            }
        }
        private void PokeERam(uint address, byte data)
        {
            switch (extMode)
            {
            case 0U:
            case 1U: eram[address & 0x3FFU] = (byte)(gpu.Rendering ? data : 0x00); break;
            case 2U: eram[address & 0x3FFU] = data; break;
            case 3U: break;
            }
        }
        private void PokeName(uint address, byte data)
        {
            switch (nmt_indice[(address >> 10) & 3U])
            {
            default:
            case 0U: nmta[address & 0x3FFU] = data; break;
            case 1U: nmtb[address & 0x3FFU] = data; break;
            case 2U: eram[address & 0x3FFU] = data; break;
            case 3U: break;
            }
        }

        #region PRG-ROM Mode
        private void Poke5100(uint address, byte data) { prgMode = (data & 0x03U); }
        #endregion
        #region CHR-ROM Mode
        private void Poke5101(uint address, byte data) { chrMode = (data & 0x03U); }
        #endregion
        #region PRG-RAM Lock
        private void Poke5102(uint address, byte data) { ramLock = (ramLock & ~0x3U) | ((data & 0x3U) << 0); }
        private void Poke5103(uint address, byte data) { ramLock = (ramLock & ~0xCU) | ((data & 0x3U) << 2); }
        #endregion
        #region EXT-RAM Mode
        private void Poke5104(uint address, byte data) { extMode = (data & 0x03U); }
        #endregion
        private void Poke5105(uint address, byte data)
        {
            nmt_indice[0U] = (data & 0x03U) >> 0;
            nmt_indice[1U] = (data & 0x0CU) >> 2;
            nmt_indice[2U] = (data & 0x30U) >> 4;
            nmt_indice[3U] = (data & 0xC0U) >> 6;
        }
        private void Poke5106(uint address, byte data) { fill_tile = (byte)(data); }
        private void Poke5107(uint address, byte data) { fill_attr = (byte)(data & 0x03U); }
        #region PRG-RAM Bank
        private void Poke5113(uint address, byte data)
        {
            SelectRam((address & 0x04U) >> 2);

            ramPage[0U] = (data & 0x03U) << 13;
        }
        #endregion
        #region PRG-ROM Bank
        private void Poke5114(uint address, byte data) { prgPage[0x0U] = (data & 0xFFU) << 13; }
        private void Poke5115(uint address, byte data) { prgPage[0x1U] = (data & 0xFFU) << 13; }
        private void Poke5116(uint address, byte data) { prgPage[0x2U] = (data & 0xFFU) << 13; }
        private void Poke5117(uint address, byte data) { prgPage[0x3U] = (data & 0x7FU) << 13; }
        #endregion
        #region CHR-ROM Bank
        private void Poke5120(uint address, byte data) { chrPage[0x0U] = (data & 0xFFU) | chrHigh; chrSelect = 'A'; }
        private void Poke5121(uint address, byte data) { chrPage[0x1U] = (data & 0xFFU) | chrHigh; chrSelect = 'A'; }
        private void Poke5122(uint address, byte data) { chrPage[0x2U] = (data & 0xFFU) | chrHigh; chrSelect = 'A'; }
        private void Poke5123(uint address, byte data) { chrPage[0x3U] = (data & 0xFFU) | chrHigh; chrSelect = 'A'; }
        private void Poke5124(uint address, byte data) { chrPage[0x4U] = (data & 0xFFU) | chrHigh; chrSelect = 'A'; }
        private void Poke5125(uint address, byte data) { chrPage[0x5U] = (data & 0xFFU) | chrHigh; chrSelect = 'A'; }
        private void Poke5126(uint address, byte data) { chrPage[0x6U] = (data & 0xFFU) | chrHigh; chrSelect = 'A'; }
        private void Poke5127(uint address, byte data) { chrPage[0x7U] = (data & 0xFFU) | chrHigh; chrSelect = 'A'; }
        private void Poke5128(uint address, byte data) { chrPage[0x8U] = (data & 0xFFU) | chrHigh; chrSelect = 'B'; }
        private void Poke5129(uint address, byte data) { chrPage[0x9U] = (data & 0xFFU) | chrHigh; chrSelect = 'B'; }
        private void Poke512A(uint address, byte data) { chrPage[0xAU] = (data & 0xFFU) | chrHigh; chrSelect = 'B'; }
        private void Poke512B(uint address, byte data) { chrPage[0xBU] = (data & 0xFFU) | chrHigh; chrSelect = 'B'; }
        private void Poke5130(uint address, byte data) { chrHigh = (data & 0x03U) << 8; }
        #endregion

        private byte Peek5204(uint address)
        {
            byte data = 0;

            if (irq_pending) data |= 0x80;
            if (irq_inframe) data |= 0x40;

            irq_pending = false;
            cpu.Irq(0u);

            return data;
        }
        private byte Peek5205(uint address) { return product.l; }
        private byte Peek5206(uint address) { return product.h; }
        private void Poke5200(uint address, byte data) { }
        private void Poke5201(uint address, byte data) { }
        private void Poke5202(uint address, byte data) { }
        private void Poke5203(uint address, byte data)
        {
            irq_compare = data;
        }
        private void Poke5204(uint address, byte data)
        {
            irq_enabled = (data & 0x80U) != 0;
        }
        private void Poke5205(uint address, byte data) { multiplierA = data; product.w = (ushort)(multiplierA * multiplierB); }
        private void Poke5206(uint address, byte data) { multiplierB = data; product.w = (ushort)(multiplierA * multiplierB); }

        private uint DecodeChrA(uint address)
        {
            uint page = ((address >> 10) & 7U);

            switch (chrMode)
            {
            default:
            case 0U: return (address & 0x1FFFU) | (chrPage[page | 7U] << 13); //                                           $5127
            case 1U: return (address & 0x0FFFU) | (chrPage[page | 3U] << 12); //                   $5123,                  $5127
            case 2U: return (address & 0x07FFU) | (chrPage[page | 1U] << 11); //       $5121,      $5123,      $5125,      $5127
            case 3U: return (address & 0x03FFU) | (chrPage[page | 0U] << 10); // $5120,$5121,$5122,$5123,$5124,$5125,$5126,$5127
            }
        }
        private uint DecodeChrB(uint address)
        {
            uint page = ((address >> 10) & 3U) | 0x8U;

            switch (chrMode)
            {
            default:
            case 0U: return (address & 0x0FFFU) | (chrPage[page | 3U] << 13); //                   $512B
            case 1U: return (address & 0x0FFFU) | (chrPage[page | 3U] << 12); //                   $512B
            case 2U: return (address & 0x07FFU) | (chrPage[page | 1U] << 11); //       $5129,      $512B
            case 3U: return (address & 0x03FFU) | (chrPage[page | 0U] << 10); // $5128,$5129,$512A,$512B
            }
        }

        protected override uint DecodeChr(uint address)
        {
            if (gpu.spr.rasters == 16)
            {
                if (gpu.hclock < 256U) return DecodeChrB(address);
                if (gpu.hclock < 320U) return DecodeChrA(address);
                if (gpu.hclock < 341U) return DecodeChrB(address);

                throw new global::System.NotSupportedException("CHR Access at an invalid time");
            }
            else
            {
                switch (chrSelect)
                {
                default:
                case 'A': return DecodeChrA(address);
                case 'B': return DecodeChrB(address);
                }
            }
        }
        protected override uint DecodePrg(uint address)
        {
            switch (prgMode)
            {
            case 0U:
                switch (address & 0xE000U)
                {
                case 0x8000U: return (address & 0x7FFFU) | (prgPage[3U] & ~0x7FFFU);
                case 0xA000U: return (address & 0x7FFFU) | (prgPage[3U] & ~0x7FFFU);
                case 0xC000U: return (address & 0x7FFFU) | (prgPage[3U] & ~0x7FFFU);
                case 0xE000U: return (address & 0x7FFFU) | (prgPage[3U] & ~0x7FFFU);
                }
                break;

            case 1U:
                switch (address & 0xE000U)
                {
                case 0x8000U: return (address & 0x3FFFU) | (prgPage[1U] & ~0x3FFFU);
                case 0xA000U: return (address & 0x3FFFU) | (prgPage[1U] & ~0x3FFFU);
                case 0xC000U: return (address & 0x3FFFU) | (prgPage[3U] & ~0x3FFFU);
                case 0xE000U: return (address & 0x3FFFU) | (prgPage[3U] & ~0x3FFFU);
                }
                break;

            case 2U:
                switch (address & 0xE000U)
                {
                case 0x8000U: return (address & 0x3FFFU) | (prgPage[1U] & ~0x3FFFU);
                case 0xA000U: return (address & 0x3FFFU) | (prgPage[1U] & ~0x3FFFU);
                case 0xC000U: return (address & 0x1FFFU) | (prgPage[2U]);
                case 0xE000U: return (address & 0x1FFFU) | (prgPage[3U]);
                }
                break;

            case 3U:
                switch (address & 0xE000U)
                {
                case 0x8000U: return (address & 0x1FFFU) | (prgPage[0U]);
                case 0xA000U: return (address & 0x1FFFU) | (prgPage[1U]);
                case 0xC000U: return (address & 0x1FFFU) | (prgPage[2U]);
                case 0xE000U: return (address & 0x1FFFU) | (prgPage[3U]);
                }
                break;
            }

            return base.DecodePrg(address);
        }
        protected override uint DecodeRam(uint address)
        {
            return (address & 0x1FFFU) | ramPage[0U];
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();

            nmta = gpu.nmt[0U];
            nmtb = gpu.nmt[1U];

            Poke5100(0x5100U, 0x03);
            Poke5117(0x5117U, 0x7F);

            gpu.Hook(0x2000U, 0x3EFFU, PeekName, PokeName);

            cpu.Hook(0x5100U, Poke5100); // PRG-ROM Mode
            cpu.Hook(0x5101U, Poke5101); // CHR-ROM Mode
            cpu.Hook(0x5102U, Poke5102); // PRG-RAM Lock
            cpu.Hook(0x5103U, Poke5103); // PRG-RAM Lock
            cpu.Hook(0x5104U, Poke5104); // EXT-RAM Mode
            cpu.Hook(0x5105U, Poke5105); // NMT-RAM Banking
            cpu.Hook(0x5106U, Poke5106); // Fill Tile
            cpu.Hook(0x5107U, Poke5107); // Fill Attr

            #region PRG-RAM Banking
            cpu.Hook(0x5113U, Poke5113);
            #endregion
            #region PRG-ROM Banking
            cpu.Hook(0x5114U, Poke5114);
            cpu.Hook(0x5115U, Poke5115);
            cpu.Hook(0x5116U, Poke5116);
            cpu.Hook(0x5117U, Poke5117);
            #endregion
            #region CHR-ROM Banking
            cpu.Hook(0x5120U, Poke5120);
            cpu.Hook(0x5121U, Poke5121);
            cpu.Hook(0x5122U, Poke5122);
            cpu.Hook(0x5123U, Poke5123);
            cpu.Hook(0x5124U, Poke5124);
            cpu.Hook(0x5125U, Poke5125);
            cpu.Hook(0x5126U, Poke5126);
            cpu.Hook(0x5127U, Poke5127);
            cpu.Hook(0x5128U, Poke5128);
            cpu.Hook(0x5129U, Poke5129);
            cpu.Hook(0x512AU, Poke512A);
            cpu.Hook(0x512BU, Poke512B);
            cpu.Hook(0x5130U, Poke5130);
            #endregion
            #region Split Screen
            cpu.Hook(0x5200U, Poke5200); // Split Screen Control
            cpu.Hook(0x5201U, Poke5201); // Split Screen V Scroll
            cpu.Hook(0x5202U, Poke5202); // Split Screen CHR Page
            #endregion
            #region IRQ
            cpu.Hook(0x5203U, Poke5203);
            cpu.Hook(0x5204U, Peek5204, Poke5204);
            #endregion
            // 8x8:16 Multiplier
            cpu.Hook(0x5205U, Peek5205, Poke5205);
            cpu.Hook(0x5206U, Peek5206, Poke5206);
            cpu.Hook(0x5C00U, 0x5FFFU, PeekERam, PokeERam);

            sound.Initialize();

            spu.Hook(sound);
        }

        protected override byte PeekChr(uint address)
        {
            if (gpu.hclock >= 256U && gpu.hclock < 320U)
                return base.PeekChr(address);

            if (extMode == 1)
            {
                address = (chrHigh << 10) | ((eram[gpu.scroll.addr & 0x3FFU] & 0x3FU) << 12) | (address & 0xFFFU);

                return chr.b[address & chr.mask];
            }

            return base.PeekChr(address);
        }
        protected override void PokeRam(uint address, byte data)
        {
            if (ramLock != 0x6U)
                return;

            base.PokeRam(address, data);
        }

        public override void Clock()
        {
            if (gpu.Rendering)
            {
                if (gpu.hclock < 3U)
                {
                    if (irq_inframe)
                    {
                        irq_counter++;

                        if (irq_counter == irq_compare)
                        {
                            irq_pending = true;

                            if (irq_enabled)
                                cpu.Irq(1u);
                        }
                    }
                    else
                    {
                        irq_counter = 0U;
                        irq_inframe = true;
                        irq_pending = false;
                        cpu.Irq(0u);
                    }
                }
            }
            else
            {
                irq_inframe = false;
            }
        }

        public class Sound : Spu.ChannelExt
        {
            public ChannelPcm Pcm;
            public ChannelSqr Sq1;
            public ChannelSqr Sq2;

            public Sound(Famicom console, Timing.System system)
                : base(console)
            {
                Pcm = new ChannelPcm(console, system);
                Sq1 = new ChannelSqr(console, system);
                Sq2 = new ChannelSqr(console, system);
            }

            private byte Peek5015(uint address)
            {
                byte output = 0;

                if (Sq1.Enabled) output |= 0x01;
                if (Sq2.Enabled) output |= 0x02;

                return output;
            }
            private void Poke5015(uint address, byte data)
            {
                Sq1.Enabled = (data & 0x01) != 0;
                Sq2.Enabled = (data & 0x02) != 0;
            }

            protected override void OnInitialize()
            {
                base.OnInitialize();

                Sq1.Initialize(0x5000U);
                Sq2.Initialize(0x5004U);
                Pcm.Initialize(0x5010U);

                cpu.Hook(0x5015U, Peek5015, Poke5015);
            }

            public override void ClockHalf()
            {
                Sq1.Duration.Clock();
                Sq2.Duration.Clock();
            }
            public override void ClockQuad()
            {
                Sq1.Envelope.Clock();
                Sq2.Envelope.Clock();
            }
            public override short Render()
            {
                var output = 0;

                output += (Pcm.Render() * 32767) / 255;
                output += (Sq1.Render() * 32767) / 15;
                output += (Sq2.Render() * 32767) / 15;

                return (short)(output / 3);
            }

            public class ChannelPcm : Spu.Channel
            {
                private byte output;

                public ChannelPcm(Famicom console, Timing.System system)
                    : base(console, system)
                {
                }

                public override void PokeReg1(uint address, byte data) { }
                public override void PokeReg2(uint address, byte data) { output = data; }
                public override void PokeReg3(uint address, byte data) { }
                public override void PokeReg4(uint address, byte data) { }

                public override byte Render()
                {
                    return output;
                }
            }
            public class ChannelSqr : Spu.Channel
            {
                private static readonly byte[][] DutyTable =
                {
                    new byte[] { 0x1F, 0x00, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F },
                    new byte[] { 0x1F, 0x00, 0x00, 0x1F, 0x1F, 0x1F, 0x1F, 0x1F },
                    new byte[] { 0x1F, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x1F, 0x1F },
                    new byte[] { 0x00, 0x1F, 0x1F, 0x00, 0x00, 0x00, 0x00, 0x00 }
                };

                private int form;
                private int step;

                public Spu.Duration Duration = new Spu.Duration();
                public Spu.Envelope Envelope = new Spu.Envelope();

                public override bool Enabled
                {
                    get { return Duration.Counter != 0; }
                    set { Duration.SetEnabled(true); }
                }

                public ChannelSqr(Famicom console, Timing.System system)
                    : base(console, system)
                {
                    timing.Cycles =
                    timing.Single = (frequency + 1) * Spu.PHASE * 2;
                }

                public override void PokeReg1(uint address, byte data)
                {
                    form = (data >> 6);
                    Envelope.Write(data);
                    Duration.Halted = (data & 0x20) != 0;
                }
                public override void PokeReg2(uint address, byte data) { }
                public override void PokeReg3(uint address, byte data)
                {
                    frequency = (frequency & 0x700) | (data << 0 & 0x0FF);
                    timing.Single = (frequency + 1) * Spu.PHASE * 2;
                }
                public override void PokeReg4(uint address, byte data)
                {
                    frequency = (frequency & 0x0FF) | (data << 8 & 0x700);
                    timing.Single = (frequency + 1) * Spu.PHASE * 2;

                    Duration.SetCounter(data);
                    Envelope.Reset = true;
                    step = 0;
                }

                public override byte Render()
                {
                    int sum = timing.Cycles;
                    timing.Cycles -= Spu.DELAY;

                    if (Duration.Counter != 0 && Envelope.Level != 0)
                    {
                        if (timing.Cycles >= 0)
                        {
                            return (byte)(Envelope.Level >> DutyTable[form][step]);
                        }
                        else
                        {
                            sum >>= DutyTable[form][step];

                            for (; timing.Cycles < 0; timing.Cycles += timing.Single)
                                sum += Math.Min(-timing.Cycles, timing.Single) >> DutyTable[form][step = (step - 1) & 0x7];

                            return (byte)((sum * Envelope.Level) / Spu.DELAY);
                        }
                    }
                    else
                    {
                        var count = (~timing.Cycles + timing.Single) / timing.Single;

                        step = (step - count) & 0x7;
                        timing.Cycles += (count * timing.Single);
                    }

                    return 0;
                }
            }
        }
    }
}