using Nintemulator.FC.GPU;

namespace Nintemulator.FC.Boards.Nintendo
{
    public class NintendoMMC1 : Board
    {
        private uint chrMode = 0U;
        private uint prgMode = 3U;
        private uint shift;
        private uint value;

        public NintendoMMC1(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            chrPage = new uint[2U];
            prgPage = new uint[1U];
        }

        private void Poke8000()
        {
            switch (value & 3U)
            {
            case 0U: gpu.SwitchNametables(Mirroring.Mode1ScA); break;
            case 1U: gpu.SwitchNametables(Mirroring.Mode1ScB); break;
            case 2U: gpu.SwitchNametables(Mirroring.ModeVert); break;
            case 3U: gpu.SwitchNametables(Mirroring.ModeHorz); break;
            }

            prgMode = (value & 0x0CU) >> 2;
            chrMode = (value & 0x10U) >> 4;
        }
        private void PokeA000() { chrPage[0U] = (value & 0x1FU) << 12; }
        private void PokeC000() { chrPage[1U] = (value & 0x1FU) << 12; }
        private void PokeE000() { prgPage[0U] = (value & 0x0FU) << 14; }

        protected override uint DecodeChr(uint address)
        {
            switch (chrMode)
            {
            case 0U: return (address & 0x1FFFU) | (chrPage[0] & ~0x1FFFU);
            case 1U:
                switch (address & 0x1000U)
                {
                case 0x0000U: return (address & 0x0FFFU) | chrPage[0U];
                case 0x1000U: return (address & 0x0FFFU) | chrPage[1U];
                }
                break;
            }

            return base.DecodeChr(address);
        }
        protected override uint DecodePrg(uint address)
        {
            switch (prgMode)
            {
            case 0U:
            case 1U: return (address & 0x7FFFU) | (prgPage[0U] & ~0x7FFFU);
            case 2U:
                switch (address & 0xC000U)
                {
                case 0x8000U: return (address & 0x3FFFU) | 0x00U << 14;
                case 0xC000U: return (address & 0x3FFFU) | prgPage[0U];
                }
                break;
            case 3U:
                switch (address & 0xC000U)
                {
                case 0x8000U: return (address & 0x3FFFU) | prgPage[0U];
                case 0xC000U: return (address & 0x3FFFU) | 0x0FU << 14;
                }
                break;
            }

            return base.DecodePrg(address);
        }
        protected override void PokePrg(uint address, byte data)
        {
            if (cpu.Edge == 0) // ignore multiple writes
                return;

            if (data >= 0x80)
            {
                prgMode = 3U;
                shift = 0U;
                value = 0U;
            }
            else
            {
                value |= (data & 1U) << (int)shift;

                if (++shift == 5U)
                {
                    switch (address & 0xE000U)
                    {
                    case 0x8000U: Poke8000(); break;
                    case 0xA000U: PokeA000(); break;
                    case 0xC000U: PokeC000(); break;
                    case 0xE000U: PokeE000(); break;
                    }

                    value = 0U;
                    shift = 0U;
                }
            }
        }
    }
}