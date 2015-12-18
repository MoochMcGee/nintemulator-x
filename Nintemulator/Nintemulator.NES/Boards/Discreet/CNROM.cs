namespace Nintemulator.FC.Boards.Discreet
{
    public class CNROM : Board
    {
        public CNROM(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            chrPage = new uint[1U];
        }

        protected override uint DecodeChr(uint address)
        {
            return (address & 0x1FFF) | chrPage[0U];
        }
        protected override void PokePrg(uint address, byte data)
        {
            chrPage[0U] = (data & 0xFFU) << 13;
        }
    }
}