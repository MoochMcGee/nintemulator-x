using Nintemulator.FC.GPU;

namespace Nintemulator.FC.Boards.Unlicensed
{
    public class MltAction52 : Board
    {
        private uint prgMode;
        private uint ram0;
        private uint ram1;
        private uint ram2;
        private uint ram3;

        public MltAction52(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            chrPage = new uint[1U];
            prgPage = new uint[1U];
        }

        private byte PeekRam0(uint address) { return (byte)ram0; }
        private byte PeekRam1(uint address) { return (byte)ram1; }
        private byte PeekRam2(uint address) { return (byte)ram2; }
        private byte PeekRam3(uint address) { return (byte)ram3; }
        private void PokeRam0(uint address, byte data) { ram0 = (data & 0x0FU); }
        private void PokeRam1(uint address, byte data) { ram1 = (data & 0x0FU); }
        private void PokeRam2(uint address, byte data) { ram2 = (data & 0x0FU); }
        private void PokeRam3(uint address, byte data) { ram3 = (data & 0x0FU); }

        protected override uint DecodeChr(uint address)
        {
            return (address & 0x1FFFU) | chrPage[0U];
        }
        protected override uint DecodePrg(uint address)
        {
            switch (prgMode)
            {
            case 0U: return (address & 0x7FFFU) | (prgPage[0U] & ~0x7FFFU);
            case 1U: return (address & 0x3FFFU) | (prgPage[0U] & ~0x3FFFU);
            }

            return base.DecodePrg(address);
        }
        protected override byte PeekPrg(uint address)
        {
            if (prg == null)
                return cpu.Open;

            return prg.b[DecodePrg(address) & prg.mask];
        }
        protected override void PokePrg(uint address, byte data)
        {
            switch ((address >> 11) & 0x03U)
            {
            case 0U: SelectPrg(0x00000000U); break;
            case 1U: SelectPrg(0x00000001U); break;
            case 2U: SelectPrg(0xFFFFFFFFU); break;
            case 3U: SelectPrg(0x00000002U); break;
            }

            switch ((address >> 13) & 0x01U)
            {
            case 0U: gpu.SwitchNametables(Mirroring.ModeVert); break;
            case 1U: gpu.SwitchNametables(Mirroring.ModeHorz); break;
            }

            chrPage[0U] = ((address & 0x000FU) << 0xF) | ((data & 0x03U) << 0xD);
            prgPage[0U] = ((address & 0x07C0U) << 0x8);
            prgMode = ((address & 0x0020U) >> 0x5);
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();

            for (uint address = 0x4200U; address < 0x5FFFU; address++)
            {
                switch (address & 0x0003U)
                {
                case 0U: cpu.Hook(address, PeekRam0, PokeRam0); break;
                case 1U: cpu.Hook(address, PeekRam1, PokeRam1); break;
                case 2U: cpu.Hook(address, PeekRam2, PokeRam2); break;
                case 3U: cpu.Hook(address, PeekRam3, PokeRam3); break;
                }
            }
        }
    }
}