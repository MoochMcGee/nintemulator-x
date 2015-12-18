namespace Nintemulator.FC.Boards.Discreet
{
    public class NROM128 : Board
    {
        public NROM128(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board) { }

        protected override uint DecodePrg(uint address)
        {
            return (address & 0x3FFFU);
        }
    }
}