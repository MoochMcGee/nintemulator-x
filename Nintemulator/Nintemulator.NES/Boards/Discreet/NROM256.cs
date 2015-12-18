namespace Nintemulator.FC.Boards.Discreet
{
    public class NROM256 : Board
    {
        public NROM256(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board) { }

        protected override uint DecodePrg(uint address)
        {
            return (address & 0x7FFFU);
        }
    }
}