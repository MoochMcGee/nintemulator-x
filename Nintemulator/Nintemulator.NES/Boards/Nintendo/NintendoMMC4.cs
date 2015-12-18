using Nintemulator.FC.GPU;

namespace Nintemulator.FC.Boards.Nintendo
{
    public class NintendoMMC4 : Board
    {
        private uint chrTimer;
        private uint chr0, chr0Latch;
        private uint chr1, chr1Latch;

        public NintendoMMC4(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            chrPage = new uint[4U];
            prgPage = new uint[2U];
        }

        private void PokeA000(uint address, byte data) { prgPage[0U] = (data & 0xFFU) << 14; }
        private void PokeB000(uint address, byte data) { chrPage[0U] = (data & 0xFFU) << 12; }
        private void PokeC000(uint address, byte data) { chrPage[1U] = (data & 0xFFU) << 12; }
        private void PokeD000(uint address, byte data) { chrPage[2U] = (data & 0xFFU) << 12; }
        private void PokeE000(uint address, byte data) { chrPage[3U] = (data & 0xFFU) << 12; }
        private void PokeF000(uint address, byte data)
        {
            switch (data & 1U)
            {
            case 0U: gpu.SwitchNametables(Mirroring.ModeVert); break;
            case 1U: gpu.SwitchNametables(Mirroring.ModeHorz); break;
            }
        }

        protected override uint DecodeChr(uint address)
        {
            switch ((address >> 12) & 1U)
            {
            default:
            case 0U: return chrPage[chr0] | (address & 0x0FFFU);
            case 1U: return chrPage[chr1] | (address & 0x0FFFU);
            }
        }
        protected override uint DecodePrg(uint address)
        {
            return prgPage[(address >> 14) & 1U] | (address & 0x3FFFU);
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();

            prgPage[0U] = 0x00000000U << 14;
            prgPage[1U] = 0xFFFFFFFFU << 14;

            chr0 = chr0Latch = 0U;
            chr1 = chr1Latch = 2U;

            cpu.Hook(0xA000U, 0xAFFFU, PokeA000);
            cpu.Hook(0xB000U, 0xBFFFU, PokeB000);
            cpu.Hook(0xC000U, 0xCFFFU, PokeC000);
            cpu.Hook(0xD000U, 0xDFFFU, PokeD000);
            cpu.Hook(0xE000U, 0xEFFFU, PokeE000);
            cpu.Hook(0xF000U, 0xFFFFU, PokeF000);
        }

        public override void GpuAddressUpdate(uint address)
        {
            if (chrTimer != 0 && --chrTimer == 0)
            {
                chr0 = chr0Latch | 0U;
                chr1 = chr1Latch | 2U;
            }

            switch (address & 0x1FF0U)
            {
            case 0x0FD0: chr0Latch = 0U; chrTimer = 2U; break;
            case 0x0FE0: chr0Latch = 1U; chrTimer = 2U; break;
            case 0x1FD0: chr1Latch = 0U; chrTimer = 2U; break;
            case 0x1FE0: chr1Latch = 1U; chrTimer = 2U; break;
            }
        }
    }
}