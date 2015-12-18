using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using word = System.UInt16;

namespace Nintemulator.SFC.SPU
{
    public partial class Spu : SuperFamicom.Processor
    {
        private static readonly byte[] bootROM = new byte[]
        {
            0xcd, 0xef, 0xbd, 0xe8, 0x00, 0xc6, 0x1d, 0xd0, 0xfc, 0x8f, 0xaa, 0xf4, 0x8f, 0xbb, 0xf5, 0x78,
            0xcc, 0xf4, 0xd0, 0xfb, 0x2f, 0x19, 0xeb, 0xf4, 0xd0, 0xfc, 0x7e, 0xf4, 0xd0, 0x0b, 0xe4, 0xf5,
            0xcb, 0xf4, 0xd7, 0x00, 0xfc, 0xd0, 0xf3, 0xab, 0x01, 0x10, 0xef, 0x7e, 0xf4, 0x10, 0xeb, 0xba,
            0xf6, 0xda, 0x00, 0xba, 0xf4, 0xc4, 0xf4, 0xdd, 0x5d, 0xd0, 0xdb, 0x1f, 0x00, 0x00, 0xc0, 0xff,
        };

        public Registers registers;
        public Action[] codeTable;
        public Spu.Dsp DSP;
        public Timer[] Timers = new Timer[3];
        public readonly byte[] wram;
        public readonly byte[] port;
        public bool bootROMEnabled;
        public bool flagC;
        public bool flagZ;
        public bool flagH;
        public int  flagP;
        public bool flagV;
        public bool flagN;
        public int timerCycles1;
        public int timerCycles2;

        public Spu(SuperFamicom console, Timing.System system)
            : base(console, system)
        {
            base.Single = 1;

            this.codeTable = new Action[]
            {
                    Op00, Op01, Op02, Op03, Op04, Op05, Op06, Op07, Op08, Op09, Op0A, Op0B, Op0C, Op0D, Op0E, Op0F,
                    Op10, Op11, Op12, Op13, Op14, Op15, Op16, Op17, Op18, Op19, Op1A, Op1B, Op1C, Op1D, Op1E, Op1F,
                    Op20, Op21, Op22, Op23, Op24, Op25, Op26, Op27, Op28, Op29, Op2A, Op2B, Op2C, Op2D, Op2E, Op2F,
                    Op30, Op31, Op32, Op33, Op34, Op35, Op36, Op37, Op38, Op39, Op3A, Op3B, Op3C, Op3D, Op3E, Op3F,
                    Op40, Op41, Op42, Op43, Op44, Op45, Op46, Op47, Op48, Op49, Op4A, Op4B, Op4C, Op4D, Op4E, Op4F,
                    Op50, Op51, Op52, Op53, Op54, Op55, Op56, Op57, Op58, Op59, Op5A, Op5B, Op5C, Op5D, Op5E, Op5F,
                    Op60, Op61, Op62, Op63, Op64, Op65, Op66, Op67, Op68, Op69, Op6A, Op6B, Op6C, Op6D, Op6E, Op6F,
                    Op70, Op71, Op72, Op73, Op74, Op75, Op76, Op77, Op78, Op79, Op7A, Op7B, Op7C, Op7D, Op7E, Op7F,
                    Op80, Op81, Op82, Op83, Op84, Op85, Op86, Op87, Op88, Op89, Op8A, Op8B, Op8C, Op8D, Op8E, Op8F,
                    Op90, Op91, Op92, Op93, Op94, Op95, Op96, Op97, Op98, Op99, Op9A, Op9B, Op9C, Op9D, Op9E, Op9F,
                    OpA0, OpA1, OpA2, OpA3, OpA4, OpA5, OpA6, OpA7, OpA8, OpA9, OpAA, OpAB, OpAC, OpAD, OpAE, OpAF,
                    OpB0, OpB1, OpB2, OpB3, OpB4, OpB5, OpB6, OpB7, OpB8, OpB9, OpBA, OpBB, OpBC, OpBD, OpBE, OpBF,
                    OpC0, OpC1, OpC2, OpC3, OpC4, OpC5, OpC6, OpC7, OpC8, OpC9, OpCA, OpCB, OpCC, OpCD, OpCE, OpCF,
                    OpD0, OpD1, OpD2, OpD3, OpD4, OpD5, OpD6, OpD7, OpD8, OpD9, OpDA, OpDB, OpDC, OpDD, OpDE, OpDF,
                    OpE0, OpE1, OpE2, OpE3, OpE4, OpE5, OpE6, OpE7, OpE8, OpE9, OpEA, OpEB, OpEC, OpED, OpEE, OpEF,
                    OpF0, OpF1, OpF2, OpF3, OpF4, OpF5, OpF6, OpF7, OpF8, OpF9, OpFA, OpFB, OpFC, OpFD, OpFE, OpFF
            };
            this.wram = new byte[65536];
            this.port = new byte[4];
            this.DSP = new Spu.Dsp(console, system, this.wram);

            this.registers.sph = 1;
        }

        public static byte ReadBootROM(int address)
        {
            return bootROM[address & 0x3f];
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            //Arrays.fill(this.wram, (byte)0x55);
            //Arrays.fill(this.port, (byte)0x00);

            this.wram[0xf0] = 0x0a;
            this.wram[0xf1] = 0xb0;
            this.bootROMEnabled = true;
            this.registers.a = 0;
            this.registers.x = 0;
            this.registers.y = 0;
            this.registers.sp = 0x100;
            this.registers.pc = this.ReadFullWord(0xfffe);
            this.flagP = 0;
            this.flagC = false;
            this.flagZ = false;
            this.flagH = false;
            this.flagV = false;
            this.flagN = false;
            this.timerCycles1 = 0;
            this.timerCycles2 = 0;
            this.Timers[0].Enabled = false;
            this.Timers[1].Enabled = false;
            this.Timers[2].Enabled = false;
            this.Timers[0].Compare = 255;
            this.Timers[1].Compare = 255;
            this.Timers[2].Compare = 255;
            this.Timers[0].Stage1 = 0;
            this.Timers[1].Stage1 = 0;
            this.Timers[2].Stage1 = 0;
            this.Timers[0].Stage2 = 0;
            this.Timers[1].Stage2 = 0;
            this.Timers[2].Stage2 = 0;
            this.DSP.Initialize();
        }

        private byte Shl(byte value, int carry = 0)
        {
            this.flagC = (value & 0x80) != 0;
            value = (byte)((value << 1) | carry);
            SetZNByte(value);
            return value;
        }
        private byte Shr(byte value, int carry = 0)
        {
            this.flagC = (value & 0x01) != 0;
            value = (byte)((value >> 1) | carry);
            SetZNByte(value);
            return value;
        }

        public byte ImmediateByte()
        {
            return this.ReadCode(this.registers.pc++);
        }
        public word AbsoluteAddress()
        {
            byte l = ImmediateByte();
            byte h = ImmediateByte();

            return (word)((h << 8) | l);
        }
        public word AbsoluteAddressX()
        {
            return (word)(this.AbsoluteAddress() + this.registers.x);
        }
        public word AbsoluteAddressY()
        {
            return (word)(this.AbsoluteAddress() + this.registers.y);
        }
        public word DirectPageAddress()
        {
            return (word)(this.flagP | this.ImmediateByte());
        }
        public word DirectPageAddressIndirect()
        {
            return this.ReadDataWord(this.DirectPageAddress());
        }
        public word DirectPageAddressX()
        {
            return (word)(this.DirectPageAddress() + this.registers.x);
        }
        public word DirectPageAddressY()
        {
            return (word)(this.DirectPageAddress() + this.registers.y);
        }
        public word DirectPageAddressXIndirect()
        {
            return this.ReadDataWord(this.DirectPageAddressX());
        }
        public word DirectPageAddressYIndirect()
        {
            return (word)(this.DirectPageAddressIndirect() + this.registers.y);
        }
        public word DirectPageXAddress()
        {
            return (word)(this.flagP | this.registers.x);
        }
        public word DirectPageXAddressIncrement()
        {
            return (word)(this.flagP | (this.registers.x++));
        }
        public word DirectPageYAddress()
        {
            return (word)(this.flagP | this.registers.y);
        }
        public void UpdateTimers()
        {
            Timers[0].Update(this.timerCycles1 >> 7);
            Timers[1].Update(this.timerCycles1 >> 7);
            Timers[2].Update(this.timerCycles2 >> 4);

            this.timerCycles1 &= 127;
            this.timerCycles2 &= 15;
        }
        public void TakeBranch()
        {
            word offset = (ushort)(sbyte)this.ImmediateByte();
            this.registers.pc += offset;
            this.AddCycles(4);
        }
        public byte ReadFull(int address)
        {
            if (address >= 0xffc0 && this.bootROMEnabled)
            {
                return ReadBootROM(address);
            }

            return  this.ReadData(address);
        }
        public byte ReadData(int address)
        {
            if ((address & 0xfff0) != 0x00f0)
            {
                return this.ReadWRAM(address);
            }
            else
            {
                byte result;

                switch (address)
                {
                case 0x00f3: return this.DSP.Peek();
                case 0x00fd: this.UpdateTimers(); result = (byte)this.Timers[0].Stage2; this.Timers[0].Stage2 = 0; break;
                case 0x00fe: this.UpdateTimers(); result = (byte)this.Timers[1].Stage2; this.Timers[1].Stage2 = 0; break;
                case 0x00ff: this.UpdateTimers(); result = (byte)this.Timers[2].Stage2; this.Timers[2].Stage2 = 0; break;
                default: return this.ReadWRAM(address);
                }

                return result;
            }
        }
        public byte ReadCode(int address)
        {
            if (address >= 0xffc0 && this.bootROMEnabled)
            {
                return ReadBootROM(address);
            }

            return this.ReadWRAM(address);
        }
        public byte ReadWRAM(int address)
        {
            return this.wram[address];
        }
        public void WriteWRAM(int address, int data)
        {
            this.wram[address] = (byte)data;
        }
        public word ReadFullWord(int address)
        {
            byte l = this.ReadFull(address);
            byte h = this.ReadFull((address + 1) & 0xffff);

            return (word)(l | (h << 8));
        }
        public word ReadDataWord(int address)
        {
            byte l = this.ReadData(address);
            byte h = this.ReadData((address + 1) & 0xffff);

            return (word)(l | (h << 8));
        }
        public override void Update(int cycles)
        {
            for (; this.Cycles < cycles; this.codeTable[this.ImmediateByte()]())
            {
            }

            this.Cycles -= cycles;
        }
        public void AddCycles(int amount)
        {
            this.Cycles += amount * 21;
            this.timerCycles1 += amount;
            this.timerCycles2 += amount;
            this.DSP.Update(amount);
        }
        public void SetZNByte(int value)
        {
            this.flagZ = (value & 0xff) == 0;
            this.flagN = (value & 0x80) != 0;
        }
        public void SetZNWord(word value)
        {
            this.flagZ = (value & 0xffff) == 0;
            this.flagN = (value & 0x8000) != 0;
        }
        public byte PullByte()
        {
            this.registers.spl++;
            return this.ReadWRAM(this.registers.sp);
        }
        public word PullWord()
        {
            byte l = PullByte();
            byte h = PullByte();

            return (word)((h << 8) | l);
        }
        public void PushByte(byte value)
        {
            this.WriteWRAM(this.registers.sp, value);
            this.registers.spl--;
        }
        public void PushWord(word value)
        {
            this.PushByte((byte)(value >> 8));
            this.PushByte((byte)(value >> 0));
        }
        public void LoadFlags(byte value)
        {
            this.flagC = (value & 0x01) != 0;
            this.flagZ = (value & 0x02) != 0;
            this.flagH = (value & 0x08) != 0;
            this.flagP = (value & 0x20) != 0 ? 256 : 0;
            this.flagV = (value & 0x40) != 0;
            this.flagN = (value & 0x80) != 0;
        }
        public byte ReadPort(int number, int timestamp)
        {
            this.Update(timestamp);
            return this.port[number];
        }
        public void WritePort(int number, int data, int timestamp)
        {
            this.Update(timestamp);
            this.wram[number + 0xf4] = (byte)data;
        }
        public void WriteByte(word address, int data)
        {
            if ((address & 0xfff0) == 0x00f0)
            {
                switch (address)
                {
                case 0xf1:
                    this.UpdateTimers();

                    if (!this.Timers[0].Enabled && (data & 0x01) != 0)
                    {
                        this.Timers[0].Stage1 = 0;
                        this.Timers[0].Stage2 = 0;
                    }

                    this.Timers[0].Enabled = (data & 0x01) != 0;

                    if (!this.Timers[1].Enabled && (data & 0x02) != 0)
                    {
                        this.Timers[1].Stage1 = 0;
                        this.Timers[1].Stage2 = 0;
                    }

                    this.Timers[1].Enabled = (data & 0x02) != 0;

                    if (!this.Timers[2].Enabled && (data & 0x04) != 0)
                    {
                        this.Timers[2].Stage1 = 0;
                        this.Timers[2].Stage2 = 0;
                    }

                    this.Timers[2].Enabled = (data & 0x04) != 0;

                    if ((data & 0x10) != 0)
                    {
                        this.wram[0xf4] = 0;
                        this.wram[0xf5] = 0;
                    }

                    if ((data & 0x20) != 0)
                    {
                        this.wram[0xf6] = 0;
                        this.wram[0xf7] = 0;
                    }

                    this.bootROMEnabled = (data & 0x80) != 0;
                    break;

                case 0xf2:
                case 0xf8:
                case 0xf9:
                default: break;
                case 0xf3: this.DSP.Poke((byte)data); break;
                case 0xf4: this.port[0] = (byte)data; return;
                case 0xf5: this.port[1] = (byte)data; return;
                case 0xf6: this.port[2] = (byte)data; return;
                case 0xf7: this.port[3] = (byte)data; return;
                case 0xfa: this.UpdateTimers(); this.Timers[1 - 1].Compare = data; break;
                case 0xfb: this.UpdateTimers(); this.Timers[2 - 1].Compare = data; break;
                case 0xfc: this.UpdateTimers(); this.Timers[3 - 1].Compare = data; break;
                case 0xfd: return;
                case 0xfe: return;
                case 0xff: return;
                }
            }

            this.WriteWRAM(address, data);
        }
        public void WriteWord(word address, word data)
        {
            this.WriteByte(address, (byte)(data >> 0));
            address++;
            this.WriteByte(address, (byte)(data >> 8));
        }
        public void PushFlags()
        {
            byte flags = 0;

            if (this.flagC) { flags |= 0x01; }
            if (this.flagZ) { flags |= 0x02; }
            if (this.flagH) { flags |= 0x08; }
            if (this.flagP != 0) { flags |= 0x20; }
            if (this.flagV) { flags |= 0x40; }
            if (this.flagN) { flags |= 0x80; }

            this.PushByte(flags);
        }
        public void Branch(bool flag)
        {
            if (flag)
            {
                this.TakeBranch();
            }
            else
            {
                this.registers.pc++;
                this.AddCycles(2);
            }
        }
        public byte And(byte value) { return this.registers.a &= value; }
        public byte Eor(byte value) { return this.registers.a ^= value; }
        public byte Ora(byte value) { return this.registers.a |= value; }
        public byte Asl(byte value) { return Shl(value, 0); }
        public byte Lsr(byte value) { return Shr(value, 0); }
        public byte Rol(byte value) { return Shl(value, flagC ? 0x01 : 0); }
        public byte Ror(byte value) { return Shr(value, flagC ? 0x80 : 0); }
        public byte Adc(byte left, byte right)
        {
            int temporary = left + right + (this.flagC ? 1 : 0);
            this.flagC = (temporary > 0xff);
            temporary &= 0xff;
            this.flagV = (~(left ^ right) & (right ^ temporary) & 0x80) != 0;
            this.flagH = ((left ^ right ^ temporary) & 0x10) != 0;
            this.SetZNByte(temporary);
            return (byte)temporary;
        }
        public word Add(word left, word right)
        {
            int temporary = left + right;
            this.flagC = (temporary > 0xffff);
            temporary &= 0xffff;
            this.flagV = (~(left ^ right) & (right ^ temporary) & 0x8000) != 0;
            this.flagH = ((left ^ right ^ temporary) & 0x100) != 0;
            this.SetZNWord((word)temporary);
            return (word)temporary;
        }
        public byte Sbc(byte left, byte right)
        {
            int temporary = left - right - (this.flagC ? 0 : 1);
            this.flagC = (temporary & 256) == 0;
            temporary &= 255;
            this.flagV = (~(left ^ right) & (right ^ temporary) & 128) != 0;
            this.flagH = ((left ^ right ^ temporary) & 16) == 0;
            this.SetZNByte(temporary);
            return (byte)temporary;
        }
        public word Sub(word left, word right)
        {
            int temporary = left - right;
            this.flagC = (temporary & 65536) == 0;
            temporary &= 0xffff;
            this.flagV = (~(left ^ right) & (right ^ temporary) & 0x8000) != 0;
            this.flagH = ((left ^ right ^ temporary) & 256) == 0;
            this.SetZNWord((word)temporary);
            return (word)temporary;
        }
        public void CmpByte(byte left, byte right)
        {
            this.flagC = left >= right;
            left -= right;
            this.SetZNByte(left);
        }
        public void CmpWord(word left, word right)
        {
            this.flagC = left >= right;
            left -= right;
            this.SetZNWord(left);
        }

        #region Codes

        private void Op00()
        {
            AddCycles(2);
        }
        private void Op01()
        {
            var var1 = ReadFullWord(0xffde);
            PushWord(this.registers.pc);
            this.registers.pc = (var1);
            AddCycles(8);
        }
        private void Op02()
        {
            var var1 = DirectPageAddress();
            var var2 = ReadFull(var1) | 1;
            WriteByte(var1, var2);
            AddCycles(4);
        }
        private void Op03()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 1) != 0);
        }
        private void Op04()
        {
            SetZNByte(Ora(ReadData(this.DirectPageAddress())));
            AddCycles(3);
        }
        private void Op05()
        {
            SetZNByte(Ora(ReadFull(this.AbsoluteAddress())));
            AddCycles(4);
        }
        private void Op06()
        {
            SetZNByte(Ora(ReadData(this.DirectPageXAddress())));
            AddCycles(3);
        }
        private void Op07()
        {
            SetZNByte(Ora(ReadFull(this.DirectPageAddressXIndirect())));
            AddCycles(6);
        }
        private void Op08()
        {
            SetZNByte(Ora(ImmediateByte()));
            AddCycles(2);
        }
        private void Op09()
        {
            var var1 = ReadData(this.DirectPageAddress());
            var var2 = DirectPageAddress();
            var1 |= ReadFull(var2);
            WriteByte(var2, var1);
            SetZNByte(var1);
            AddCycles(6);
        }
        private void Op0A()
        {
            var var1 = AbsoluteAddress();
            var var2 = 1 << (var1 >> 13);
            var1 &= 8191;
            if ((ReadFull(var1) & var2) != 0)
            {
                flagC = (true);
            }
            AddCycles(5);
        }
        private void Op0B()
        {
            var address = DirectPageAddress();
            WriteByte(address, Asl(ReadFull(address)));
            AddCycles(4);
        }
        private void Op0C()
        {
            var address = AbsoluteAddress();
            WriteByte(address, Asl(ReadFull(address)));
            AddCycles(5);
        }
        private void Op0D()
        {
            PushFlags();
            AddCycles(4);
        }
        private void Op0E()
        {
            var address = AbsoluteAddress();
            var data = ReadFull(address);
            SetZNByte(this.registers.a - data & 255);
            data |= this.registers.a;
            WriteByte(address, data);
            AddCycles(6);
        }
        private void Op0F()
        {
            var var1 = ReadFullWord(0xfffe);
            PushWord(this.registers.pc);
            PushFlags();
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void Op10()
        {
            Branch(!this.flagN);
        }
        private void Op11()
        {
            var var1 = ReadFullWord(0xffdc);
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void Op12()
        {
            var address = DirectPageAddress();
            var var2 = ReadFull(address) & ~1;
            WriteByte(address, var2);
            AddCycles(4);
        }
        private void Op13()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 1) == 0);
        }
        private void Op14()
        {
            SetZNByte(Ora(ReadData(this.DirectPageAddressX())));
            AddCycles(4);
        }
        private void Op15()
        {
            SetZNByte(Ora(ReadFull(this.AbsoluteAddressX())));
            AddCycles(5);
        }
        private void Op16()
        {
            SetZNByte(Ora(ReadFull(this.AbsoluteAddressY())));
            AddCycles(5);
        }
        private void Op17()
        {
            SetZNByte(Ora(ReadFull(this.DirectPageAddressYIndirect())));
            AddCycles(6);
        }
        private void Op18()
        {
            var data = ImmediateByte();
            var address = DirectPageAddress();
            data |= ReadFull(address);
            WriteByte(address, data);
            SetZNByte(data);
            AddCycles(5);
        }
        private void Op19()
        {
            var data = ReadData(this.flagP | this.registers.y);
            var address = DirectPageXAddress();
            data |= ReadFull(address);
            WriteByte(address, data);
            SetZNByte(data);
            AddCycles(5);
        }
        private void Op1A()
        {
            var address = DirectPageAddress();
            var data = ReadFullWord(address);
            WriteWord(address, --data);
            SetZNWord(data);
            AddCycles(6);
        }
        private void Op1B()
        {
            var address = DirectPageAddressX();
            WriteByte(address, Asl(ReadFull(address)));
            AddCycles(5);
        }
        private void Op1C()
        {
            this.registers.a = Asl(this.registers.a);
            AddCycles(2);
        }
        private void Op1D()
        {
            SetZNByte(--this.registers.x);
            AddCycles(2);
        }
        private void Op1E()
        {
            CmpByte(this.registers.x, ReadFull(this.AbsoluteAddress()));
            AddCycles(4);
        }
        private void Op1F()
        {
            this.registers.pc = (ReadFullWord(this.AbsoluteAddressX()));
            AddCycles(6);
        }
        private void Op20()
        {
            flagP = (0);
            AddCycles(2);
        }
        private void Op21()
        {
            var var1 = ReadFullWord(0xffda);
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void Op22()
        {
            var address = DirectPageAddress();
            var data = ReadFull(address) | 2;
            WriteByte(address, data);
            AddCycles(4);
        }
        private void Op23()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 2) != 0);
        }
        private void Op24()
        {
            SetZNByte(And(ReadData(this.DirectPageAddress())));
            AddCycles(3);
        }
        private void Op25()
        {
            SetZNByte(And(ReadFull(this.AbsoluteAddress())));
            AddCycles(4);
        }
        private void Op26()
        {
            SetZNByte(And(ReadData(this.DirectPageXAddress())));
            AddCycles(3);
        }
        private void Op27()
        {
            SetZNByte(And(ReadFull(this.DirectPageAddressXIndirect())));
            AddCycles(6);
        }
        private void Op28()
        {
            SetZNByte(And(ImmediateByte()));
            AddCycles(2);
        }
        private void Op29()
        {
            var data = ReadData(this.DirectPageAddress());
            var address = DirectPageAddress();
            data &= ReadFull(address);
            WriteByte(address, data);
            SetZNByte(data);
            AddCycles(6);
        }
        private void Op2A()
        {
            var var1 = AbsoluteAddress();
            var var2 = 1 << (var1 >> 13);
            var1 &= 8191;
            if ((ReadFull(var1) & var2) == 0)
            {
                flagC = (true);
            }
            AddCycles(5);
        }
        private void Op2B()
        {
            var address = DirectPageAddress();
            WriteByte(address, Rol(ReadFull(address)));
            AddCycles(4);
        }
        private void Op2C()
        {
            var address = AbsoluteAddress();
            WriteByte(address, Rol(ReadFull(address)));
            AddCycles(5);
        }
        private void Op2D()
        {
            PushByte(this.registers.a);
            AddCycles(4);
        }
        private void Op2E()
        {
            AddCycles(3);
            Branch(ReadData(this.DirectPageAddress()) != this.registers.a);
        }
        private void Op2F()
        {
            this.TakeBranch();
        }
        private void Op30()
        {
            Branch(this.flagN);
        }
        private void Op31()
        {
            var var1 = ReadFullWord(0xffd8);
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void Op32()
        {
            var address = DirectPageAddress();
            var data = ReadFull(address) & ~2;
            WriteByte(address, data);
            AddCycles(4);
        }
        private void Op33()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 2) == 0);
        }
        private void Op34()
        {
            SetZNByte(And(ReadData(this.DirectPageAddressX())));
            AddCycles(4);
        }
        private void Op35()
        {
            SetZNByte(And(ReadFull(this.AbsoluteAddressX())));
            AddCycles(5);
        }
        private void Op36()
        {
            SetZNByte(And(ReadFull(this.AbsoluteAddressY())));
            AddCycles(5);
        }
        private void Op37()
        {
            SetZNByte(And(ReadFull(this.DirectPageAddressYIndirect())));
            AddCycles(6);
        }
        private void Op38()
        {
            var data = ImmediateByte();
            var address = DirectPageAddress();
            data &= ReadFull(address);
            WriteByte(address, data);
            SetZNByte(data);
            AddCycles(5);
        }
        private void Op39()
        {
            var data = ReadData(DirectPageYAddress());
            var address = DirectPageXAddress();
            data &= ReadFull(address);
            WriteByte(address, data);
            SetZNByte(data);
            AddCycles(5);
        }
        private void Op3A()
        {
            var address = DirectPageAddress();
            var data = ReadFullWord(address);
            WriteWord(address, ++data);
            SetZNWord(data);
            AddCycles(6);
        }
        private void Op3B()
        {
            var address = DirectPageAddressX();
            WriteByte(address, Rol(ReadFull(address)));
            AddCycles(5);
        }
        private void Op3C()
        {
            this.registers.a = (Rol(this.registers.a));
            AddCycles(2);
        }
        private void Op3D()
        {
            SetZNByte(++this.registers.x);
            AddCycles(2);
        }
        private void Op3E()
        {
            CmpByte(this.registers.x, ReadData(this.DirectPageAddress()));
            AddCycles(3);
        }
        private void Op3F()
        {
            var var1 = AbsoluteAddress();
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void Op40()
        {
            flagP = (256);
            AddCycles(2);
        }
        private void Op41()
        {
            var var1 = ReadFullWord(0xffd6);
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void Op42()
        {
            var address = DirectPageAddress();
            var data = ReadFull(address) | 4;
            WriteByte(address, data);
            AddCycles(4);
        }
        private void Op43()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 4) != 0);
        }
        private void Op44()
        {
            SetZNByte(Eor(ReadData(this.DirectPageAddress())));
            AddCycles(3);
        }
        private void Op45()
        {
            SetZNByte(Eor(ReadFull(this.AbsoluteAddress())));
            AddCycles(4);
        }
        private void Op46()
        {
            SetZNByte(Eor(ReadData(this.DirectPageXAddress())));
            AddCycles(3);
        }
        private void Op47()
        {
            SetZNByte(Eor(ReadFull(this.DirectPageAddressXIndirect())));
            AddCycles(6);
        }
        private void Op48()
        {
            SetZNByte(Eor(ImmediateByte()));
            AddCycles(2);
        }
        private void Op49()
        {
            var data = ReadData(this.DirectPageAddress());
            var address = DirectPageAddress();
            data ^= ReadFull(address);
            WriteByte(address, data);
            SetZNByte(data);
            AddCycles(6);
        }
        private void Op4A()
        {
            var var1 = AbsoluteAddress();
            var var2 = 1 << (var1 >> 13);
            var1 &= 8191;
            flagC = (this.flagC && (ReadFull(var1) & var2) != 0);
            AddCycles(4);
        }
        private void Op4B()
        {
            var address = DirectPageAddress();
            WriteByte(address, Lsr(ReadFull(address)));
            AddCycles(4);
        }
        private void Op4C()
        {
            var address = AbsoluteAddress();
            WriteByte(address, Lsr(ReadFull(address)));
            AddCycles(5);
        }
        private void Op4D()
        {
            PushByte(this.registers.x);
            AddCycles(4);
        }
        private void Op4E()
        {
            var address = AbsoluteAddress();
            int data = ReadFull(address);
            SetZNByte(this.registers.a - data & 255);
            data &= ~this.registers.a;
            WriteByte(address, data);
            AddCycles(6);
        }
        private void Op4F()
        {
            var var1 = (word)(0xff00 | ImmediateByte());
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(6);
        }
        private void Op50()
        {
            Branch(!this.flagV);
        }
        private void Op51()
        {
            var var1 = ReadFullWord(0xffd4);
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void Op52()
        {
            var var1 = DirectPageAddress();
            var var2 = ReadFull(var1) & -5;
            WriteByte(var1, var2);
            AddCycles(4);
        }
        private void Op53()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 4) == 0);
        }
        private void Op54()
        {
            SetZNByte(Eor(ReadData(this.DirectPageAddressX())));
            AddCycles(4);
        }
        private void Op55()
        {
            SetZNByte(Eor(ReadFull(this.AbsoluteAddressX())));
            AddCycles(5);
        }
        private void Op56()
        {
            SetZNByte(Eor(ReadFull(this.AbsoluteAddressY())));
            AddCycles(5);
        }
        private void Op57()
        {
            SetZNByte(Eor(ReadFull(this.DirectPageAddressYIndirect())));
            AddCycles(6);
        }
        private void Op58()
        {
            var var1 = ImmediateByte();
            var var2 = DirectPageAddress();
            var1 ^= ReadFull(var2);
            WriteByte(var2, var1);
            SetZNByte(var1);
            AddCycles(5);
        }
        private void Op59()
        {
            var var1 = ReadData(this.flagP | this.registers.y);
            var var2 = DirectPageXAddress();
            var1 ^= ReadFull(var2);
            WriteByte(var2, var1);
            SetZNByte(var1);
            AddCycles(5);
        }
        private void Op5A()
        {
            word var1 = DirectPageAddressIndirect();
            CmpWord(registers.ya, var1);
            AddCycles(4);
        }
        private void Op5B()
        {
            var var1 = DirectPageAddressX();
            WriteByte(var1, Lsr(ReadFull(var1)));
            AddCycles(5);
        }
        private void Op5C()
        {
            this.registers.a = (Lsr(this.registers.a));
            AddCycles(2);
        }
        private void Op5D()
        {
            SetZNByte(this.registers.x = this.registers.a);
            AddCycles(2);
        }
        private void Op5E()
        {
            CmpByte(this.registers.y, ReadFull(this.AbsoluteAddress()));
            AddCycles(4);
        }
        private void Op5F()
        {
            this.registers.pc = (AbsoluteAddress());
            AddCycles(3);
        }
        private void Op60()
        {
            flagC = (false);
            AddCycles(2);
        }
        private void Op61()
        {
            var var1 = ReadFullWord(0xffd2);
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void Op62()
        {
            var var1 = DirectPageAddress();
            var var2 = ReadFull(var1) | 8;
            WriteByte(var1, var2);
            AddCycles(4);
        }
        private void Op63()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 8) != 0);
        }
        private void Op64()
        {
            CmpByte(this.registers.a, ReadData(this.DirectPageAddress()));
            AddCycles(3);
        }
        private void Op65()
        {
            CmpByte(this.registers.a, ReadFull(this.AbsoluteAddress()));
            AddCycles(4);
        }
        private void Op66()
        {
            CmpByte(this.registers.a, ReadData(this.DirectPageXAddress()));
            AddCycles(3);
        }
        private void Op67()
        {
            CmpByte(this.registers.a, ReadFull(this.DirectPageAddressXIndirect()));
            AddCycles(6);
        }
        private void Op68()
        {
            CmpByte(this.registers.a, ImmediateByte());
            AddCycles(2);
        }
        private void Op69()
        {
            byte var1 = ReadData(this.DirectPageAddress());
            CmpByte(ReadData(this.DirectPageAddress()), var1);
            AddCycles(6);
        }
        private void Op6A()
        {
            var var1 = AbsoluteAddress();
            var var2 = 1 << (var1 >> 13);
            var1 &= 8191;
            flagC = (this.flagC && (ReadFull(var1) & var2) == 0);
            AddCycles(4);
        }
        private void Op6B()
        {
            var var1 = DirectPageAddress();
            WriteByte(var1, Ror(ReadFull(var1)));
            AddCycles(4);
        }
        private void Op6C()
        {
            var var1 = AbsoluteAddress();
            WriteByte(var1, Ror(ReadFull(var1)));
            AddCycles(5);
        }
        private void Op6D()
        {
            PushByte(this.registers.y);
            AddCycles(4);
        }
        private void Op6E()
        {
            var var1 = DirectPageAddress();
            var var2 = ReadFull(var1) - 1 & 255;
            WriteByte(var1, var2);
            Branch(var2 != 0);
        }
        private void Op6F()
        {
            this.registers.pc = (PullWord());
            AddCycles(5);
        }
        private void Op70()
        {
            Branch(this.flagV);
        }
        private void Op71()
        {
            var var1 = ReadFullWord(0xffd0);
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void Op72()
        {
            var var1 = DirectPageAddress();
            var var2 = ReadFull(var1) & -9;
            WriteByte(var1, var2);
            AddCycles(4);
        }
        private void Op73()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 8) == 0);
        }
        private void Op74()
        {
            CmpByte(this.registers.a, ReadData(this.DirectPageAddressX()));
            AddCycles(4);
        }
        private void Op75()
        {
            CmpByte(this.registers.a, ReadFull(this.AbsoluteAddressX()));
            AddCycles(5);
        }
        private void Op76()
        {
            CmpByte(this.registers.a, ReadFull(this.AbsoluteAddressY()));
            AddCycles(5);
        }
        private void Op77()
        {
            CmpByte(this.registers.a, ReadFull(this.DirectPageAddressYIndirect()));
            AddCycles(6);
        }
        private void Op78()
        {
            byte var1 = ImmediateByte();
            CmpByte(ReadData(this.DirectPageAddress()), var1);
            AddCycles(5);
        }
        private void Op79()
        {
            byte var1 = ReadData(this.flagP | this.registers.y);
            CmpByte(ReadData(this.DirectPageXAddress()), var1);
            AddCycles(5);
        }
        private void Op7A()
        {
            registers.ya = Add(registers.ya, DirectPageAddressIndirect());
            AddCycles(5);
        }
        private void Op7B()
        {
            var var1 = DirectPageAddressX();
            WriteByte(var1, Ror(ReadFull(var1)));
            AddCycles(5);
        }
        private void Op7C()
        {
            this.registers.a = (Ror(this.registers.a));
            AddCycles(2);
        }
        private void Op7D()
        {
            SetZNByte(this.registers.a = (this.registers.x));
            AddCycles(2);
        }
        private void Op7E()
        {
            CmpByte(this.registers.y, ReadData(this.DirectPageAddress()));
            AddCycles(3);
        }
        private void Op7F()
        {
            LoadFlags(this.PullByte());
            this.registers.pc = (PullWord());
            AddCycles(6);
        }
        private void Op80()
        {
            flagC = (true);
            AddCycles(2);
        }
        private void Op81()
        {
            var var1 = ReadFullWord(0xffce);
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void Op82()
        {
            var var1 = DirectPageAddress();
            var var2 = ReadFull(var1) | 16;
            WriteByte(var1, var2);
            AddCycles(4);
        }
        private void Op83()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 16) != 0);
        }
        private void Op84()
        {
            this.registers.a = Adc(this.registers.a, ReadData(this.DirectPageAddress()));
            AddCycles(3);
        }
        private void Op85()
        {
            this.registers.a = (Adc(this.registers.a, ReadFull(this.AbsoluteAddress())));
            AddCycles(4);
        }
        private void Op86()
        {
            this.registers.a = (Adc(this.registers.a, ReadData(this.DirectPageXAddress())));
            AddCycles(3);
        }
        private void Op87()
        {
            this.registers.a = (Adc(this.registers.a, ReadFull(this.DirectPageAddressXIndirect())));
            AddCycles(6);
        }
        private void Op88()
        {
            this.registers.a = (Adc(this.registers.a, ImmediateByte()));
            AddCycles(2);
        }
        private void Op89()
        {
            byte var1 = ReadData(this.DirectPageAddress());
            var var2 = DirectPageAddress();
            byte var3 = ReadFull(var2);
            var3 = Adc(var3, var1);
            WriteByte(var2, var3);
            AddCycles(6);
        }
        private void Op8A()
        {
            var var1 = AbsoluteAddress();
            var var2 = 1 << (var1 >> 13);
            var1 &= 8191;
            flagC = (this.flagC ^ (ReadFull(var1) & var2) != 0);
            AddCycles(5);
        }
        private void Op8B()
        {
            var var1 = DirectPageAddress();
            var var2 = ReadFull(var1) - 1 & 255;
            WriteByte(var1, var2);
            SetZNByte(var2);
            AddCycles(4);
        }
        private void Op8C()
        {
            var var1 = AbsoluteAddress();
            var var2 = ReadFull(var1) - 1 & 255;
            WriteByte(var1, var2);
            SetZNByte(var2);
            AddCycles(5);
        }
        private void Op8D()
        {
            SetZNByte(this.registers.y = (ImmediateByte()));
            AddCycles(2);
        }
        private void Op8E()
        {
            LoadFlags(this.PullByte());
            AddCycles(4);
        }
        private void Op8F()
        {
            var var1 = ImmediateByte();
            WriteByte(DirectPageAddress(), var1);
            AddCycles(5);
        }
        private void Op90()
        {
            Branch(!this.flagC);
        }
        private void Op91()
        {
            var var1 = ReadFullWord(0xffcc);
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void Op92()
        {
            var var1 = DirectPageAddress();
            var var2 = ReadFull(var1) & -17;
            WriteByte(var1, var2);
            AddCycles(4);
        }
        private void Op93()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 16) == 0);
        }
        private void Op94()
        {
            this.registers.a = (Adc(this.registers.a, ReadData(this.DirectPageAddressX())));
            AddCycles(4);
        }
        private void Op95()
        {
            this.registers.a = (Adc(this.registers.a, ReadFull(this.AbsoluteAddressX())));
            AddCycles(5);
        }
        private void Op96()
        {
            this.registers.a = (Adc(this.registers.a, ReadFull(this.AbsoluteAddressY())));
            AddCycles(5);
        }
        private void Op97()
        {
            this.registers.a = (Adc(this.registers.a, ReadFull(this.DirectPageAddressYIndirect())));
            AddCycles(6);
        }
        private void Op98()
        {
            var var1 = ImmediateByte();
            var var2 = DirectPageAddress();
            var var3 = ReadFull(var2);
            var3 = Adc(var3, var1);
            WriteByte(var2, var3);
            AddCycles(5);
        }
        private void Op99()
        {
            var var1 = ReadData(this.flagP | this.registers.y);
            var var2 = DirectPageXAddress();
            var var3 = ReadFull(var2);
            var3 = Adc(var3, var1);
            WriteByte(var2, var3);
            AddCycles(5);
        }
        private void Op9A()
        {
            registers.ya = Sub(registers.ya, DirectPageAddressIndirect());
            AddCycles(5);
        }
        private void Op9B()
        {
            var var1 = DirectPageAddressX();
            var var2 = ReadFull(var1) - 1 & 255;
            WriteByte(var1, var2);
            SetZNByte(var2);
            AddCycles(5);
        }
        private void Op9C()
        {
            SetZNByte(--this.registers.a);
            AddCycles(2);
        }
        private void Op9D()
        {
            SetZNByte(this.registers.x = this.registers.spl);
            AddCycles(2);
        }
        private void Op9E()
        {
            flagH = (this.registers.x & 0xf) < (this.registers.y & 0xf);

            int ya = registers.ya;
            int x = (this.registers.x << 9);

            for (int i = 0; i < 9; i++)
            {
                ya = ((ya << 1) | (ya >> 16)) & 0x1ffff;

                if (ya > x)
                {
                    ya ^= 1;
                }

                if ((ya & 1) != 0)
                {
                    ya -= x;
                }
            }

            flagV = ((ya & 0x00100) != 0);

            this.registers.a = (byte)((ya & 0x000ff) >> 0);
            this.registers.y = (byte)((ya & 0x1fe00) >> 9);

            SetZNByte(registers.a);
            AddCycles(12);
        }
        private void Op9F()
        {
            registers.a = (byte)((registers.a << 4) | (registers.a >> 4));
            SetZNByte(registers.a);
            AddCycles(5);
        }
        private void OpA0()
        {
            AddCycles(3);
        }
        private void OpA1()
        {
            var var1 = ReadFullWord(0xffca);
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void OpA2()
        {
            var var1 = DirectPageAddress();
            var var2 = ReadFull(var1) | 32;
            WriteByte(var1, var2);
            AddCycles(4);
        }
        private void OpA3()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 32) != 0);
        }
        private void OpA4()
        {
            this.registers.a = (Sbc(this.registers.a, ReadData(this.DirectPageAddress())));
            AddCycles(3);
        }
        private void OpA5()
        {
            this.registers.a = (Sbc(this.registers.a, ReadFull(this.AbsoluteAddress())));
            AddCycles(4);
        }
        private void OpA6()
        {
            this.registers.a = (Sbc(this.registers.a, ReadData(this.DirectPageXAddress())));
            AddCycles(3);
        }
        private void OpA7()
        {
            this.registers.a = (Sbc(this.registers.a, ReadFull(this.DirectPageAddressXIndirect())));
            AddCycles(6);
        }
        private void OpA8()
        {
            this.registers.a = (Sbc(this.registers.a, ImmediateByte()));
            AddCycles(2);
        }
        private void OpA9()
        {
            var var1 = ReadData(this.DirectPageAddress());
            var var2 = DirectPageAddress();
            var var3 = ReadFull(var2);
            var3 = Sbc(var3, var1);
            WriteByte(var2, var3);
            AddCycles(6);
        }
        private void OpAA()
        {
            var var1 = AbsoluteAddress();
            var var2 = 1 << (var1 >> 13);
            var1 &= 8191;
            flagC = ((ReadFull(var1) & var2) != 0);
            AddCycles(4);
        }
        private void OpAB()
        {
            var var1 = DirectPageAddress();
            var var2 = ReadFull(var1) + 1 & 255;
            WriteByte(var1, var2);
            SetZNByte(var2);
            AddCycles(4);
        }
        private void OpAC()
        {
            var var1 = AbsoluteAddress();
            var var2 = ReadFull(var1) + 1 & 255;
            WriteByte(var1, var2);
            SetZNByte(var2);
            AddCycles(5);
        }
        private void OpAD()
        {
            CmpByte(this.registers.y, ImmediateByte());
            AddCycles(2);
        }
        private void OpAE()
        {
            this.registers.a = (PullByte());
            AddCycles(4);
        }
        private void OpAF()
        {
            var var1 = this.registers.a;
            WriteByte(DirectPageXAddressIncrement(), var1);
            AddCycles(4);
        }
        private void OpB0()
        {
            Branch(this.flagC);
        }
        private void OpB1()
        {
            var var1 = ReadFullWord(0xffc8);
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void OpB2()
        {
            var var1 = DirectPageAddress();
            var var2 = ReadFull(var1) & -33;
            WriteByte(var1, var2);
            AddCycles(4);
        }
        private void OpB3()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 32) == 0);
        }
        private void OpB4()
        {
            this.registers.a = (Sbc(this.registers.a, ReadData(this.DirectPageAddressX())));
            AddCycles(4);
        }
        private void OpB5()
        {
            this.registers.a = (Sbc(this.registers.a, ReadFull(this.AbsoluteAddressX())));
            AddCycles(5);
        }
        private void OpB6()
        {
            this.registers.a = (Sbc(this.registers.a, ReadFull(this.AbsoluteAddressY())));
            AddCycles(5);
        }
        private void OpB7()
        {
            this.registers.a = (Sbc(this.registers.a, ReadFull(this.DirectPageAddressYIndirect())));
            AddCycles(6);
        }
        private void OpB8()
        {
            var var1 = ImmediateByte();
            var var2 = DirectPageAddress();
            var var3 = ReadFull(var2);
            var3 = Sbc(var3, var1);
            WriteByte(var2, var3);
            AddCycles(5);
        }
        private void OpB9()
        {
            var var1 = ReadData(this.flagP | this.registers.y);
            var var2 = DirectPageXAddress();
            var var3 = ReadFull(var2);
            var3 = Sbc(var3, var1);
            WriteByte(var2, var3);
            AddCycles(5);
        }
        private void OpBA()
        {
            word var1 = DirectPageAddressIndirect();
            registers.ya = var1;
            SetZNWord(var1);
            AddCycles(5);
        }
        private void OpBB()
        {
            var var1 = DirectPageAddressX();
            var var2 = ReadFull(var1) + 1 & 255;
            WriteByte(var1, var2);
            SetZNByte(var2);
            AddCycles(5);
        }
        private void OpBC()
        {
            SetZNByte(++this.registers.a);
            AddCycles(2);
        }
        private void OpBD()
        {
            this.registers.spl = this.registers.x;
            AddCycles(2);
        }
        private void OpBE()
        {
            var var1 = this.registers.a;
            if (!this.flagC || var1 > 153)
            {
                var1 -= 96;
                flagC = (false);
            }
            if (!this.flagH || (var1 & 15) > 9)
            {
                var1 -= 6;
            }
            var1 &= 255;
            this.registers.a = (byte)(var1);
            SetZNByte(var1);
            AddCycles(3);
        }
        private void OpBF()
        {
            SetZNByte(this.registers.a = (ReadData(this.DirectPageXAddressIncrement())));
            AddCycles(4);
        }
        private void OpC0()
        {
            AddCycles(3);
        }
        private void OpC1()
        {
            var var1 = ReadFullWord(0xffc6);
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void OpC2()
        {
            var var1 = DirectPageAddress();
            var var2 = ReadFull(var1) | 64;
            WriteByte(var1, var2);
            AddCycles(4);
        }
        private void OpC3()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 64) != 0);
        }
        private void OpC4()
        {
            var var1 = this.registers.a;
            WriteByte(DirectPageAddress(), var1);
            AddCycles(4);
        }
        private void OpC5()
        {
            var var1 = this.registers.a;
            WriteByte(AbsoluteAddress(), var1);
            AddCycles(5);
        }
        private void OpC6()
        {
            var var1 = this.registers.a;
            WriteByte(DirectPageXAddress(), var1);
            AddCycles(4);
        }
        private void OpC7()
        {
            var var1 = this.registers.a;
            WriteByte(DirectPageAddressXIndirect(), var1);
            AddCycles(7);
        }
        private void OpC8()
        {
            CmpByte(this.registers.x, ImmediateByte());
            AddCycles(2);
        }
        private void OpC9()
        {
            var var1 = this.registers.x;
            WriteByte(AbsoluteAddress(), var1);
            AddCycles(5);
        }
        private void OpCA()
        {
            var var1 = AbsoluteAddress();
            var var2 = 1 << (var1 >> 13);
            var1 &= 8191;
            var2 = ReadFull(var1) & ~var2 | (this.flagC ? var2 : 0);
            WriteByte(var1, var2);
            AddCycles(6);
        }
        private void OpCB()
        {
            var var1 = this.registers.y;
            WriteByte(DirectPageAddress(), var1);
            AddCycles(4);
        }
        private void OpCC()
        {
            var var1 = this.registers.y;
            WriteByte(AbsoluteAddress(), var1);
            AddCycles(5);
        }
        private void OpCD()
        {
            SetZNByte(this.registers.x = (ImmediateByte()));
            AddCycles(2);
        }
        private void OpCE()
        {
            this.registers.x = (PullByte());
            AddCycles(4);
        }
        private void OpCF()
        {
            registers.ya = (word)(this.registers.y * this.registers.a);
            SetZNByte(this.registers.y);
            AddCycles(9);
        }
        private void OpD0()
        {
            Branch(!this.flagZ);
        }
        private void OpD1()
        {
            var var1 = ReadFullWord(0xffc4);
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void OpD2()
        {
            var var1 = DirectPageAddress();
            var var2 = ReadFull(var1) & -65;
            WriteByte(var1, var2);
            AddCycles(4);
        }
        private void OpD3()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 64) == 0);
        }
        private void OpD4()
        {
            var var1 = this.registers.a;
            WriteByte(DirectPageAddressX(), var1);
            AddCycles(5);
        }
        private void OpD5()
        {
            var var1 = this.registers.a;
            WriteByte(AbsoluteAddressX(), var1);
            AddCycles(6);
        }
        private void OpD6()
        {
            var var1 = this.registers.a;
            WriteByte(AbsoluteAddressY(), var1);
            AddCycles(6);
        }
        private void OpD7()
        {
            var var1 = this.registers.a;
            WriteByte(DirectPageAddressYIndirect(), var1);
            AddCycles(7);
        }
        private void OpD8()
        {
            var var1 = this.registers.x;
            WriteByte(DirectPageAddress(), var1);
            AddCycles(4);
        }
        private void OpD9()
        {
            var var1 = this.registers.x;
            WriteByte(DirectPageAddressY(), var1);
            AddCycles(5);
        }
        private void OpDA()
        {
            WriteWord(this.DirectPageAddress(), registers.ya);
            AddCycles(5);
        }
        private void OpDB()
        {
            var var1 = this.registers.y;
            WriteByte(DirectPageAddressX(), var1);
            AddCycles(5);
        }
        private void OpDC()
        {
            SetZNByte(--this.registers.y);
            AddCycles(2);
        }
        private void OpDD()
        {
            SetZNByte(this.registers.a = (this.registers.y));
            AddCycles(2);
        }
        private void OpDE()
        {
            AddCycles(4);
            Branch(ReadData(this.DirectPageAddressX()) != this.registers.a);
        }
        private void OpDF()
        {
            var var1 = this.registers.a;
            if (this.flagC || var1 > 153)
            {
                var1 += 96;
                flagC = (true);
            }
            if (this.flagH || (var1 & 15) > 9)
            {
                var1 += 6;
            }
            var1 &= 255;
            this.registers.a = (byte)(var1);
            SetZNByte(var1);
            AddCycles(3);
        }
        private void OpE0()
        {
            flagV = (false);
            flagH = (false);
            AddCycles(2);
        }
        private void OpE1()
        {
            var var1 = ReadFullWord(0xffc2);
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void OpE2()
        {
            var var1 = DirectPageAddress();
            var var2 = ReadFull(var1) | 128;
            WriteByte(var1, var2);
            AddCycles(4);
        }
        private void OpE3()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 128) != 0);
        }
        private void OpE4()
        {
            SetZNByte(this.registers.a = (ReadData(this.DirectPageAddress())));
            AddCycles(3);
        }
        private void OpE5()
        {
            SetZNByte(this.registers.a = (ReadFull(this.AbsoluteAddress())));
            AddCycles(4);
        }
        private void OpE6()
        {
            SetZNByte(this.registers.a = (ReadData(this.DirectPageXAddress())));
            AddCycles(3);
        }
        private void OpE7()
        {
            SetZNByte(this.registers.a = (ReadFull(this.DirectPageAddressXIndirect())));
            AddCycles(6);
        }
        private void OpE8()
        {
            SetZNByte(this.registers.a = (ImmediateByte()));
            AddCycles(2);
        }
        private void OpE9()
        {
            SetZNByte(this.registers.x = (ReadFull(this.AbsoluteAddress())));
            AddCycles(4);
        }
        private void OpEA()
        {
            var var1 = AbsoluteAddress();
            var var2 = 1 << (var1 >> 13);
            var1 &= 8191;
            WriteByte(var1, ReadFull(var1) ^ var2);
            AddCycles(5);
        }
        private void OpEB()
        {
            SetZNByte(this.registers.y = (ReadData(this.DirectPageAddress())));
            AddCycles(3);
        }
        private void OpEC()
        {
            SetZNByte(this.registers.y = (ReadFull(this.AbsoluteAddress())));
            AddCycles(4);
        }
        private void OpED()
        {
            flagC = (!this.flagC);
            AddCycles(3);
        }
        private void OpEE()
        {
            this.registers.y = (PullByte());
            AddCycles(4);
        }
        private void OpEF()
        {
            AddCycles(3);
            this.registers.pc--;
        }
        private void OpF0()
        {
            Branch(this.flagZ);
        }
        private void OpF1()
        {
            var var1 = ReadFullWord(0xffc0);
            PushWord(this.registers.pc);
            this.registers.pc = var1;
            AddCycles(8);
        }
        private void OpF2()
        {
            var var1 = DirectPageAddress();
            var var2 = ReadFull(var1) & -129;
            WriteByte(var1, var2);
            AddCycles(4);
        }
        private void OpF3()
        {
            AddCycles(3);
            Branch((ReadData(this.DirectPageAddress()) & 128) == 0);
        }
        private void OpF4()
        {
            SetZNByte(this.registers.a = (ReadData(this.DirectPageAddressX())));
            AddCycles(4);
        }
        private void OpF5()
        {
            SetZNByte(this.registers.a = (ReadFull(this.AbsoluteAddressX())));
            AddCycles(5);
        }
        private void OpF6()
        {
            SetZNByte(this.registers.a = (ReadFull(this.AbsoluteAddressY())));
            AddCycles(5);
        }
        private void OpF7()
        {
            SetZNByte(this.registers.a = (ReadFull(this.DirectPageAddressYIndirect())));
            AddCycles(6);
        }
        private void OpF8()
        {
            SetZNByte(this.registers.x = (ReadData(this.DirectPageAddress())));
            AddCycles(3);
        }
        private void OpF9()
        {
            SetZNByte(this.registers.x = (ReadData(this.DirectPageAddressY())));
            AddCycles(4);
        }
        private void OpFA()
        {
            var var1 = ReadData(this.DirectPageAddress());
            WriteByte(DirectPageAddress(), var1);
            AddCycles(5);
        }
        private void OpFB()
        {
            SetZNByte(this.registers.y = (ReadData(this.DirectPageAddressX())));
            AddCycles(4);
        }
        private void OpFC()
        {
            SetZNByte(++this.registers.y);
            AddCycles(2);
        }
        private void OpFD()
        {
            SetZNByte(this.registers.y = this.registers.a);
            AddCycles(2);
        }
        private void OpFE()
        {
            this.registers.y--;
            Branch(this.registers.y != 0);
        }
        private void OpFF()
        {
            AddCycles(3);
            this.registers.pc--;
        }

        #endregion

        public struct Timer
        {
            public bool Enabled;
            public int Compare;
            public int Stage1;
            public int Stage2;

            public void Update(int clocks)
            {
                if (Enabled)
                {
                    for (int i = 0; i < clocks; i++)
                    {
                        Stage1 = (Stage1 + 1) & 0xff;

                        if (Stage1 == Compare)
                        {
                            Stage1 = 0;
                            Stage2++;
                        }
                    }

                    Stage2 &= 15;
                }
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct Registers
        {
            [FieldOffset(0)] public word sp;
            [FieldOffset(2)] public word pc;
            [FieldOffset(4)] public byte a;
            [FieldOffset(5)] public byte y;
            [FieldOffset(6)] public byte x;

            [FieldOffset(4)] public word ya;

            #region Byte Accessors

            [FieldOffset(0)] public byte spl;
            [FieldOffset(1)] public byte sph;
            [FieldOffset(2)] public byte pcl;
            [FieldOffset(3)] public byte pch;

            #endregion
        }
    }
}
