using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nintemulator.GB.Boards
{
    public class NintendoMBC2 : Board
    {
        private uint romPage = (1U << 14);

        public NintendoMBC2(Gameboy gameboy, byte[] rom)
            : base(gameboy, rom)
        {
            base.ram = new byte[0x200];
            base.ramMask = 0x1FF;
        }

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
            return ram[address & 0x1FF];
        }
        private void Poke_0000_1FFF(uint address, byte data)
        {
            if ((address & 0x100) == 0)
            {
            }
        }
        private void Poke_2000_3FFF(uint address, byte data)
        {
            if ((address & 0x100) != 0)
            {
                romPage = (data & 0x1FU) << 14;

                if (romPage == 0)
                    romPage += (1 << 14);
            }
        }
        private void Poke_4000_5FFF(uint address, byte data) { }
        private void Poke_6000_7FFF(uint address, byte data) { }
        private void Poke_A000_BFFF(uint address, byte data)
        {
            ram[address & 0x1FF] = data &= 0x0F;
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
            console.Hook(0x2000, 0x3FFF, Peek_0000_3FFF, Poke_2000_3FFF);
            console.Hook(0x4000, 0x5FFF, Peek_4000_7FFF, Poke_4000_5FFF);
            console.Hook(0x6000, 0x7FFF, Peek_4000_7FFF, Poke_6000_7FFF);
        }
    }
}
