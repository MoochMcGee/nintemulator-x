using Nintemulator.FC.GPU;

namespace Nintemulator.FC.Boards.Konami
{
    public class KonamiVRC1 : Board
    {
        public KonamiVRC1(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            chrPage = new uint[2U];
            prgPage = new uint[4U];
        }

        private void Poke8000(uint address, byte data) { prgPage[0U] = (data & 0x0FU) << 13; }
        private void Poke9000(uint address, byte data)
        {
            switch (data & 1U)
            {
            case 0U: gpu.SwitchNametables(Mirroring.ModeVert); break;
            case 1U: gpu.SwitchNametables(Mirroring.ModeHorz); break;
            }

            chrPage[0U] = (chrPage[0U] & ~0x10000U) | ((data & 0x02U) << 15);
            chrPage[1U] = (chrPage[1U] & ~0x10000U) | ((data & 0x04U) << 14);
        }
        private void PokeA000(uint address, byte data) { prgPage[1U] = (data & 0x0FU) << 13; }
        // $B000
        private void PokeC000(uint address, byte data) { prgPage[2U] = (data & 0x0FU) << 13; }
        // $D000
        private void PokeE000(uint address, byte data) { chrPage[0U] = (chrPage[0U] & ~0xF000U) | ((data & 0x0FU) << 12); }
        private void PokeF000(uint address, byte data) { chrPage[1U] = (chrPage[1U] & ~0xF000U) | ((data & 0x0FU) << 12); }

        protected override uint DecodeChr(uint address)
        {
            return (address & 0x0FFFU) | chrPage[(address >> 12) & 1U];
        }
        protected override uint DecodePrg(uint address)
        {
            return (address & 0x1FFFU) | prgPage[(address >> 13) & 3U];
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();

            prgPage[0U] = 0x00000000U << 13;
            prgPage[1U] = 0x00000000U << 13;
            prgPage[2U] = 0x00000000U << 13;
            prgPage[3U] = 0xFFFFFFFFU << 13; // -1

            cpu.Hook(0x8000U, 0x8FFF, Poke8000);
            cpu.Hook(0x9000U, 0x9FFF, Poke9000);
            cpu.Hook(0xA000U, 0xAFFF, PokeA000);
            // $B000
            cpu.Hook(0xC000U, 0xCFFF, PokeC000);
            // $D000
            cpu.Hook(0xE000U, 0xEFFF, PokeE000);
            cpu.Hook(0xF000U, 0xFFFF, PokeF000);
        }
    }
}