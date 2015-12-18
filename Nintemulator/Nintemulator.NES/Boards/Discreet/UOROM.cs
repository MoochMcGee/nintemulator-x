namespace Nintemulator.FC.Boards.Discreet
{
    public class UOROM : Board
    {
        public UOROM(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            prgPage = new uint[2U];
            prgPage[0U] = 0x00000000U << 14;
            prgPage[1U] = 0x0000000FU << 14;
        }

        protected override uint DecodePrg(uint address)
        {
            return (address & 0x3FFFU) | prgPage[(address >> 14) & 1U];
        }
        protected override void PokePrg(uint address, byte data)
        {
            prgPage[0U] = (data & 0x0FU) << 14;
        }
    }
}