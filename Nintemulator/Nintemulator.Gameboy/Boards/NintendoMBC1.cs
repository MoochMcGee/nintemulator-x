using System;

namespace Nintemulator.GB.Boards
{
    public class NintendoMBC1 : Board
    {
        private bool romMode;
        private uint ramPage;
        private uint romPage = (1U << 14);

        public NintendoMBC1(Gameboy gameboy, byte[] rom)
            : base(gameboy, rom) { }

        private byte Peek_0000_3FFF(uint address)
        {
            return rom[address & 0x3FFF];
        }
        private byte Peek_4000_7FFF(uint address)
        {
            if (romMode)
            {
                return rom[((address & 0x3FFF) | romPage | (ramPage << 6)) & romMask];
            }
            else
            {
                return rom[((address & 0x3FFF) | romPage) & romMask];
            }
        }
        private void Poke_0000_1FFF(uint address, byte data) { }
        private void Poke_2000_3FFF(uint address, byte data)
        {
            romPage = (data & 0x1FU) << 14;

            if (romPage == 0)
                romPage += (1U << 14);
        }
        private void Poke_4000_5FFF(uint address, byte data)
        {
            ramPage = (data & 0x03U) << 13;
        }
        private void Poke_6000_7FFF(uint address, byte data)
        {
            romMode = (data & 0x01U) == 0;
        }

        protected override void DisableBios(uint address, byte data)
        {
            console.Hook(0x0000, 0x00FF, Peek_0000_3FFF, Poke_0000_1FFF);
            console.Hook(0x0200, 0x08FF, Peek_0000_3FFF, Poke_0000_1FFF);
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