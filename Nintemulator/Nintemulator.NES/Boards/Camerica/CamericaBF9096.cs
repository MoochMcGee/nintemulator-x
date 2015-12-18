namespace Nintemulator.FC.Boards.Camerica
{
    public class CamericaBF9096 : Board
    {
        public CamericaBF9096(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            prgPage = new uint[2u];
        }

        private void Poke8000(uint address, byte data)
        {
            prgPage[0u] = (prgPage[0u] & ~0x30000u) | ((data & 0x18u) << 13);
            prgPage[1u] = (prgPage[1u] & ~0x30000u) | ((data & 0x18u) << 13);
        }
        private void PokeC000(uint address, byte data)
        {
            prgPage[0u] = (prgPage[0u] & ~0x0c000u) | ((data & 0x03u) << 14);
        }

        protected override uint DecodePrg(uint address)
        {
            return prgPage[(address >> 14) & 1u] | (address & 0x3fffu);
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();

            prgPage[0u] = 0x00000000u << 14;
            prgPage[1u] = 0x00000003u << 14;

            cpu.Hook(0x8000u, 0xbfffu, Poke8000);
            cpu.Hook(0xc000u, 0xffffu, PokeC000);
        }
    }
}