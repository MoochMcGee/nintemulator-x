using Nintemulator.FC.GPU;

namespace Nintemulator.FC.Boards.Camerica
{
    public class CamericaBF9093 : Board
    {
        public CamericaBF9093(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            prgPage = new uint[2u];
        }

        private void Poke8000(uint address, byte data)
        {
            switch ((data >> 4) & 1u)
            {
            case 0u: gpu.SwitchNametables(Mirroring.Mode1ScA); break;
            case 1u: gpu.SwitchNametables(Mirroring.Mode1ScB); break;
            }
        }
        private void PokeC000(uint address, byte data)
        {
            prgPage[0u] = (data & 0xffu) << 14;
        }

        protected override uint DecodePrg(uint address)
        {
            return prgPage[(address >> 14) & 1u] | (address & 0x3fffu);
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();

            prgPage[0u] = (0u << 14);
            prgPage[1u] = (~0u << 14);

            cpu.Hook(0x8000u, 0xbfffu, Poke8000);
            cpu.Hook(0xc000u, 0xffffu, PokeC000);
        }
    }
}