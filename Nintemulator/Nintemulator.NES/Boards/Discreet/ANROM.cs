using Nintemulator.FC.GPU;

namespace Nintemulator.FC.Boards.Discreet
{
    public class ANROM : Board
    {
        public ANROM(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            prgPage = new uint[1U];
        }

        protected override uint DecodePrg(uint address)
        {
            return (address & 0x7FFF) | prgPage[0U];
        }
        protected override void PokePrg(uint address, byte data)
        {
            switch ((data >> 4) & 1U)
            {
            case 0U: gpu.SwitchNametables(Mirroring.Mode1ScA); break;
            case 1U: gpu.SwitchNametables(Mirroring.Mode1ScB); break;
            }

            prgPage[0U] = (data & 0x03U) << 15;
        }
    }
}