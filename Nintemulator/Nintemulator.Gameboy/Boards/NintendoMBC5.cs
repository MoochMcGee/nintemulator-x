using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nintemulator.GB.Boards
{
    public class NintendoMBC5 : Board
    {
        private bool ramEnabled;
        private uint ramPage;
        private uint romPage;

        public NintendoMBC5(Gameboy gameboy, byte[] rom)
            : base(gameboy, rom) { }

        private byte Peek_0000_3FFF(uint address)
        {
            return rom[address & 0x3FFF];
        }
        private byte Peek_4000_7FFF(uint address)
        {
            return rom[((address & 0x3FFF) | romPage) & romMask];
        }
        private byte Peek_A000_BFFF(uint address)
        {
            if (!ramEnabled)
                return 0;

            return ram[((address & 0x1FFF) | ramPage) & ramMask];
        }
        private void Poke_0000_1FFF(uint address, byte data)
        {
            ramEnabled = (data == 0x0AU);
        }
        private void Poke_2000_2FFF(uint address, byte data)
        {
            romPage = (data & 0xFFU) << 14;
        }
        private void Poke_3000_3FFF(uint address, byte data)
        {
            romPage |= (data & 0x01U) << 22;
        }
        private void Poke_4000_5FFF(uint address, byte data)
        {
            ramPage = (data & 0x0FU) << 13;
        }
        private void Poke_6000_7FFF(uint address, byte data) { }
        private void Poke_A000_BFFF(uint address, byte data)
        {
            if (!ramEnabled)
                return;

            ram[((address & 0x1FFF) | ramPage) & ramMask] = data;
        }

        protected override void SetRamSize(byte value)
        {
            switch (value)
            {
            case 0: ram = null; ramMask = 0; break;
            case 1: ram = new byte[0x02000]; ramMask = 0x01FFF; break;
            case 2: ram = new byte[0x08000]; ramMask = 0x07FFF; break;
            case 3: ram = new byte[0x20000]; ramMask = 0x1FFFF; break;
            }
        }
        protected override void SetRomSize(byte value)
        {
            base.SetRomSize(value);
        }

        protected override void DisableBios(uint address, byte data)
        {
            console.Hook(0x0000, 0x00FF, Peek_0000_3FFF, Poke_0000_1FFF);
            console.Hook(0x0200, 0x08FF, Peek_0000_3FFF, Poke_0000_1FFF);
        }
        protected override void HookRam()
        {
            console.Hook(0xA000, 0xBFFF, Peek_A000_BFFF, Poke_A000_BFFF);
        }
        protected override void HookRom()
        {
            console.Hook(0x0000, 0x1FFF, Peek_0000_3FFF, Poke_0000_1FFF);
            console.Hook(0x2000, 0x2FFF, Peek_0000_3FFF, Poke_2000_2FFF);
            console.Hook(0x3000, 0x3FFF, Peek_0000_3FFF, Poke_3000_3FFF);
            console.Hook(0x4000, 0x5FFF, Peek_4000_7FFF, Poke_4000_5FFF);
            console.Hook(0x6000, 0x7FFF, Peek_4000_7FFF, Poke_6000_7FFF);
        }
    }
}