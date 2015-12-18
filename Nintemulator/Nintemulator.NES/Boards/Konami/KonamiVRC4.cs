using Nintemulator.FC.GPU;

namespace Nintemulator.FC.Boards.Konami
{
    public class KonamiVRC4 : Board
    {
        private IRQ irq;
        private uint prgMode;

        public KonamiVRC4(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            prgPage = new uint[4U];
            chrPage = new uint[8U];

            irq = new IRQ();
        }

        private void Poke8000(uint address, byte data) { prgPage[0U] = (data & 0x1FU) << 13; }
        private void Poke9000(uint address, byte data)
        {
            switch (data & 0x03U)
            {
            case 0U: gpu.SwitchNametables(Mirroring.ModeVert); break;
            case 1U: gpu.SwitchNametables(Mirroring.ModeHorz); break;
            case 2U: gpu.SwitchNametables(Mirroring.Mode1ScA); break;
            case 3U: gpu.SwitchNametables(Mirroring.Mode1ScB); break;
            }
        }
        private void Poke9002(uint address, byte data) { prgMode = (data & 0x02U) << 13; }
        private void PokeA000(uint address, byte data) { prgPage[1U] = (data & 0x1FU) << 13; }
        private void PokeB000(uint address, byte data) { chrPage[0U] = (chrPage[0U] & ~(0x0FU << 10)) | ((data & 0x0FU) << 10); }
        private void PokeB001(uint address, byte data) { chrPage[0U] = (chrPage[0U] & ~(0xF0U << 10)) | ((data & 0x0FU) << 14); }
        private void PokeB002(uint address, byte data) { chrPage[1U] = (chrPage[1U] & ~(0x0FU << 10)) | ((data & 0x0FU) << 10); }
        private void PokeB003(uint address, byte data) { chrPage[1U] = (chrPage[1U] & ~(0xF0U << 10)) | ((data & 0x0FU) << 14); }
        private void PokeC000(uint address, byte data) { chrPage[2U] = (chrPage[2U] & ~(0x0FU << 10)) | ((data & 0x0FU) << 10); }
        private void PokeC001(uint address, byte data) { chrPage[2U] = (chrPage[2U] & ~(0xF0U << 10)) | ((data & 0x0FU) << 14); }
        private void PokeC002(uint address, byte data) { chrPage[3U] = (chrPage[3U] & ~(0x0FU << 10)) | ((data & 0x0FU) << 10); }
        private void PokeC003(uint address, byte data) { chrPage[3U] = (chrPage[3U] & ~(0xF0U << 10)) | ((data & 0x0FU) << 14); }
        private void PokeD000(uint address, byte data) { chrPage[4U] = (chrPage[4U] & ~(0x0FU << 10)) | ((data & 0x0FU) << 10); }
        private void PokeD001(uint address, byte data) { chrPage[4U] = (chrPage[4U] & ~(0xF0U << 10)) | ((data & 0x0FU) << 14); }
        private void PokeD002(uint address, byte data) { chrPage[5U] = (chrPage[5U] & ~(0x0FU << 10)) | ((data & 0x0FU) << 10); }
        private void PokeD003(uint address, byte data) { chrPage[5U] = (chrPage[5U] & ~(0xF0U << 10)) | ((data & 0x0FU) << 14); }
        private void PokeE000(uint address, byte data) { chrPage[6U] = (chrPage[6U] & ~(0x0FU << 10)) | ((data & 0x0FU) << 10); }
        private void PokeE001(uint address, byte data) { chrPage[6U] = (chrPage[6U] & ~(0xF0U << 10)) | ((data & 0x0FU) << 14); }
        private void PokeE002(uint address, byte data) { chrPage[7U] = (chrPage[7U] & ~(0x0FU << 10)) | ((data & 0x0FU) << 10); }
        private void PokeE003(uint address, byte data) { chrPage[7U] = (chrPage[7U] & ~(0xF0U << 10)) | ((data & 0x0FU) << 14); }
        private void PokeF000(uint address, byte data)
        {
            irq.refresh = (irq.refresh & ~0x0F) | ((data & 0x0F) << 0);
        }
        private void PokeF001(uint address, byte data)
        {
            irq.refresh = (irq.refresh & ~0xF0) | ((data & 0x0F) << 4);
        }
        private void PokeF002(uint address, byte data)
        {
            irq.mode = (data & 0x04U) != 0;
            irq.enabled = (data & 0x02U) != 0;
            irq.enabledRefresh = (data & 0x01U) != 0;
            irq.scaler = 341;

            if (irq.enabled)
                irq.counter = irq.refresh;

            cpu.Irq(0u);
        }
        private void PokeF003(uint address, byte data)
        {
            irq.enabled = irq.enabledRefresh;
            cpu.Irq(0u);
        }

        protected override uint DecodeChr(uint address)
        {
            return (address & 0x03FFU) | chrPage[(address >> 10) & 7U];
        }
        protected override uint DecodePrg(uint address)
        {
            address ^= (~address << 1) & prgMode;

            return (address & 0x1FFFU) | prgPage[(address >> 13) & 3U];
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();

            prgPage[0U] = 0x00000000U << 13;
            prgPage[1U] = 0x00000000U << 13;
            prgPage[2U] = 0xFFFFFFFEU << 13; // -2
            prgPage[3U] = 0xFFFFFFFFU << 13; // -1

            uint pin3 = 1U << int.Parse(GetPin("VRC4", 3).Replace("PRG A", ""));
            uint pin4 = 1U << int.Parse(GetPin("VRC4", 4).Replace("PRG A", ""));
            uint pins = pin3 | pin4;

            for (uint i = 0x0000U; i < 0x1FFFU; i++)
            {
                if ((i & pins) == 0U)
                {
                    cpu.Hook(0x8000U | i, Poke8000);
                    cpu.Hook(0x9000U | i, Poke9000);
                    cpu.Hook(0xA000U | i, PokeA000);
                    cpu.Hook(0xB000U | i, PokeB000);
                    cpu.Hook(0xC000U | i, PokeC000);
                    cpu.Hook(0xD000U | i, PokeD000);
                    cpu.Hook(0xE000U | i, PokeE000);
                    cpu.Hook(0xF000U | i, PokeF000);
                }
                else if ((i & pins) == pin4)
                {
                    cpu.Hook(0x8000U | i, Poke8000);
                    cpu.Hook(0x9000U | i, Poke9000);
                    cpu.Hook(0xA000U | i, PokeA000);
                    cpu.Hook(0xB000U | i, PokeB001);
                    cpu.Hook(0xC000U | i, PokeC001);
                    cpu.Hook(0xD000U | i, PokeD001);
                    cpu.Hook(0xE000U | i, PokeE001);
                    cpu.Hook(0xF000U | i, PokeF001);
                }
                else if ((i & pins) == pin3)
                {
                    cpu.Hook(0x8000U | i, Poke8000);
                    cpu.Hook(0x9000U | i, Poke9002);
                    cpu.Hook(0xA000U | i, PokeA000);
                    cpu.Hook(0xB000U | i, PokeB002);
                    cpu.Hook(0xC000U | i, PokeC002);
                    cpu.Hook(0xD000U | i, PokeD002);
                    cpu.Hook(0xE000U | i, PokeE002);
                    cpu.Hook(0xF000U | i, PokeF002);
                }
                else if ((i & pins) == pins)
                {
                    cpu.Hook(0x8000U | i, Poke8000);
                    cpu.Hook(0x9000U | i, Poke9002);
                    cpu.Hook(0xA000U | i, PokeA000);
                    cpu.Hook(0xB000U | i, PokeB003);
                    cpu.Hook(0xC000U | i, PokeC003);
                    cpu.Hook(0xD000U | i, PokeD003);
                    cpu.Hook(0xE000U | i, PokeE003);
                    cpu.Hook(0xF000U | i, PokeF003);
                }
            }
        }

        public override void Clock()
        {
            if (!irq.enabled)
                return;

            if (irq.mode)
            {
                if (irq.Clock())
                    cpu.Irq(1u);
            }
            else
            {
                irq.scaler -= 3;

                if (irq.scaler <= 0)
                {
                    irq.scaler += 341;

                    if (irq.Clock())
                        cpu.Irq(1u);
                }
            }
        }

        public class IRQ
        {
            public bool mode;
            public bool enabled;
            public bool enabledRefresh;
            public int counter;
            public int refresh;
            public int scaler;

            public bool Clock()
            {
                if (counter == 0xFF)
                {
                    counter = refresh;
                    return true;
                }
                else
                {
                    counter++;
                    return false;
                }
            }
        }
    }
}