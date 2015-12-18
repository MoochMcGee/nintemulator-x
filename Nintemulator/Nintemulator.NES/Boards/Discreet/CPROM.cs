namespace Nintemulator.FC.Boards.Discreet
{
    public class CPROM : Board
    {
        private byte[] chr0;
        private byte[] chr1;

        public CPROM(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            chrPage = new uint[2];
        }

        protected override uint DecodeChr(uint address)
        {
            return (address & 0x0FFFU) | chrPage[(address >> 12) & 1U];
        }

        protected override byte PeekChr(uint address)
        {
            address = DecodeChr(address);

            switch (address & 0x2000U)
            {
            default:
            case 0x0000U: return chr0[address & 0x1FFFU];
            case 0x2000U: return chr1[address & 0x1FFFU];
            }
        }
        protected override void PokeChr(uint address, byte data)
        {
            address = DecodeChr(address);

            switch (address & 0x2000U)
            {
            default:
            case 0x0000U: chr0[address & 0x1FFFU] = data; break;
            case 0x2000U: chr1[address & 0x1FFFU] = data; break;
            }
        }
        protected override void PokePrg(uint address, byte data)
        {
            chrPage[1U] = (data & 0x03U) << 12;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            chr0 = chrChips[0U].b;
            chr1 = chrChips[1U].b;
        }
    }
}