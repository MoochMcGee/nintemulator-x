namespace Nintemulator.FC.Boards.Konami
{
    public class KonamiVRC3 : Board
    {
        private IRQ irq;

        public KonamiVRC3(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            prgPage = new uint[2U];

            irq = new IRQ();
        }

        private void Poke8000(uint address, byte data) { irq.refresh = (irq.refresh & ~0x000FU) | ((data & 0x0FU) << 0x0); }
        private void Poke9000(uint address, byte data) { irq.refresh = (irq.refresh & ~0x00F0U) | ((data & 0x0FU) << 0x4); }
        private void PokeA000(uint address, byte data) { irq.refresh = (irq.refresh & ~0x0F00U) | ((data & 0x0FU) << 0x8); }
        private void PokeB000(uint address, byte data) { irq.refresh = (irq.refresh & ~0xF000U) | ((data & 0x0FU) << 0xC); }
        private void PokeC000(uint address, byte data)
        {
            irq.mode = (data & 0x04U) != 0;
            irq.enabled = (data & 0x02U) != 0;
            irq.enabledRefresh = (data & 0x01U) != 0;

            if (irq.enabled)
                irq.counter = irq.refresh;

            cpu.Irq(0u);
        }
        private void PokeD000(uint address, byte data)
        {
            irq.enabled = irq.enabledRefresh;
            cpu.Irq(0u);
        }
        private void PokeE000(uint address, byte data) { }
        private void PokeF000(uint address, byte data) { prgPage[0U] = (data & 0x0FU) << 14; }

        protected override uint DecodePrg(uint address)
        {
            return (address & 0x3FFFU) | prgPage[(address >> 14) & 1U];
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();

            prgPage[0U] = 0x00000000U << 14;
            prgPage[1U] = 0xFFFFFFFFU << 14; // -1

            cpu.Hook(0x8000U, 0x8FFFU, Poke8000);
            cpu.Hook(0x9000U, 0x9FFFU, Poke9000);
            cpu.Hook(0xA000U, 0xAFFFU, PokeA000);
            cpu.Hook(0xB000U, 0xBFFFU, PokeB000);
            cpu.Hook(0xC000U, 0xCFFFU, PokeC000);
            cpu.Hook(0xD000U, 0xDFFFU, PokeD000);
            cpu.Hook(0xE000U, 0xEFFFU, PokeE000);
            cpu.Hook(0xF000U, 0xFFFFU, PokeF000);
        }

        public override void Clock()
        {
            if (!irq.enabled)
                return;

            if (irq.Clock())
                cpu.Irq(1u);
        }

        public class IRQ
        {
            public bool mode;
            public bool enabled;
            public bool enabledRefresh;
            public uint counter;
            public uint refresh;

            public bool Clock()
            {
                if (mode)
                {
                    if ((counter & 0x00FFU) == 0x00FFU)
                    {
                        counter = (counter & ~0x00FFU) | (refresh & 0x00FFU);
                        return true;
                    }
                    else
                    {
                        counter = (counter & ~0x00FFU) | ((counter + 1U) & 0x00FFU);
                        return false;
                    }
                }
                else
                {
                    if ((counter & 0xFFFFU) == 0xFFFFU)
                    {
                        counter = (counter & ~0xFFFFU) | (refresh & 0xFFFFU);
                        return true;
                    }
                    else
                    {
                        counter = (counter & ~0xFFFFU) | ((counter + 1U) & 0xFFFFU);
                        return false;
                    }
                }
            }
        }
    }
}