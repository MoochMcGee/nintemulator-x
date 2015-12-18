namespace Nintemulator.FC.Boards.Unlicensed
{
    public class ColorDreams74x377 : Board
    {
        public ColorDreams74x377(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            chrPage = new uint[1U];
            prgPage = new uint[1U];
        }

        protected override uint DecodeChr(uint address) { return (address & 0x1FFFU) | chrPage[0U]; }
        protected override uint DecodePrg(uint address) { return (address & 0x7FFFU) | prgPage[0U]; }
        protected override void PokePrg(uint address, byte data)
        {
            chrPage[0U] = (data & 0xF0U) << 0x9;
            prgPage[0U] = (data & 0x03U) << 0xF;
        }
    }
}