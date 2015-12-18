using Nintemulator.FC.GPU;

namespace Nintemulator.FC.Boards.Nintendo
{
    public class NintendoMMC3 : Board
    {
        private bool ram_enabled;
        private bool ram_protect;
        private bool irq_enabled;
        private uint irq_counter;
        private uint irq_refresh;
        private uint irq_latch;
        private uint irq_timer;

        private uint address;
        private uint chrMode;
        private uint prgMode;

        public NintendoMMC3(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            chrPage = new uint[6U];
            prgPage = new uint[4U];
        }

        private void Poke8000(uint address, byte data)
        {
            this.chrMode = (data & 0x80U) << 5; // $0000/$1000
            this.prgMode = (data & 0x40U) << 8; // $0000/$4000
            this.address = (data & 0x07U) << 0;
        }
        private void Poke8001(uint address, byte data)
        {
            switch (this.address)
            {
            case 0: chrPage[0U] = (data & 0xFEU) << 10; break;
            case 1: chrPage[1U] = (data & 0xFEU) << 10; break;
            case 2: chrPage[2U] = (data & 0xFFU) << 10; break;
            case 3: chrPage[3U] = (data & 0xFFU) << 10; break;
            case 4: chrPage[4U] = (data & 0xFFU) << 10; break;
            case 5: chrPage[5U] = (data & 0xFFU) << 10; break;
            case 6: prgPage[0U] = (data & 0xFFU) << 13; break;
            case 7: prgPage[1U] = (data & 0xFFU) << 13; break;
            }
        }
        private void PokeA000(uint address, byte data)
        {
            switch (data & 1U)
            {
            case 0U: gpu.SwitchNametables(Mirroring.ModeVert); break;
            case 1U: gpu.SwitchNametables(Mirroring.ModeHorz); break;
            }
        }
        private void PokeA001(uint address, byte data)
        {
            ram_enabled = (data & 0x80U) != 0;
            ram_protect = (data & 0x40U) != 0;
        }
        private void PokeC000(uint address, byte data) { irq_refresh = data; }
        private void PokeC001(uint address, byte data) { irq_counter = 0x0U; }
        private void PokeE000(uint address, byte data)
        {
            irq_enabled = false;
            cpu.Irq(0u);
        }
        private void PokeE001(uint address, byte data) { irq_enabled = true; }

        protected override uint DecodeChr(uint address)
        {
            address ^= chrMode;

            switch (address & 0x1C00U)
            {
            default:
            case 0x0000U:
            case 0x0400U: return (address & 0x07FFU) | chrPage[0U];
            case 0x0800U:
            case 0x0C00U: return (address & 0x07FFU) | chrPage[1U];
            case 0x1000U: return (address & 0x03FFU) | chrPage[2U];
            case 0x1400U: return (address & 0x03FFU) | chrPage[3U];
            case 0x1800U: return (address & 0x03FFU) | chrPage[4U];
            case 0x1C00U: return (address & 0x03FFU) | chrPage[5U];
            }
        }
        protected override uint DecodePrg(uint address)
        {
            address ^= prgMode & ~(address << 1);

            switch (address & 0xE000U)
            {
            default:
            case 0x8000U: return (address & 0x1FFFU) | prgPage[0U];
            case 0xA000U: return (address & 0x1FFFU) | prgPage[1U];
            case 0xC000U: return (address & 0x1FFFU) | prgPage[2U];
            case 0xE000U: return (address & 0x1FFFU) | prgPage[3U];
            }
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();

            prgPage[0U] = 0x00000000U << 13;
            prgPage[1U] = 0x00000000U << 13;
            prgPage[2U] = 0xFFFFFFFEU << 13; // -2
            prgPage[3U] = 0xFFFFFFFFU << 13; // -1

            for (uint i = 0x8000U; i <= 0xFFFFU; i++)
            {
                switch (i & 0xE001U)
                {
                case 0x8000: cpu.Hook(i, Poke8000); break;
                case 0x8001: cpu.Hook(i, Poke8001); break;
                case 0xA000: cpu.Hook(i, PokeA000); break;
                case 0xA001: cpu.Hook(i, PokeA001); break;
                case 0xC000: cpu.Hook(i, PokeC000); break;
                case 0xC001: cpu.Hook(i, PokeC001); break;
                case 0xE000: cpu.Hook(i, PokeE000); break;
                case 0xE001: cpu.Hook(i, PokeE001); break;
                }
            }

            Poke8000(0x8000U, 0x00);
            Poke8000(0x8001U, 0x00);
            Poke8000(0xA000U, 0x00);
            Poke8000(0xA001U, 0x00);
            Poke8000(0xC000U, 0x00);
            Poke8000(0xC001U, 0x00);
            Poke8000(0xE000U, 0x00);
            Poke8000(0xE001U, 0x00);
        }

        protected override byte PeekRam(uint address)
        {
            if (ram_enabled)
                return base.PeekRam(address);

            return cpu.Open;
        }
        protected override void PokeRam(uint address, byte data)
        {
            if (ram_enabled && !ram_protect)
                base.PokeRam(address, data);
        }

        public override void Clock()
        {
            // emulate phi2 filtering
            irq_timer++;
        }
        public override void GpuAddressUpdate(uint address)
        {
            if (irq_latch < (address & 0x1000U)) // rising edge
            {
                if (irq_timer >= 5)
                {
                    if (irq_counter == 0)
                    {
                        irq_counter = irq_refresh;
                    }
                    else
                    {
                        irq_counter--;
                    }

                    if (irq_counter == 0U && irq_enabled)
                        cpu.Irq(1u);
                }

                irq_timer = 0U;
            }

            irq_latch = (address & 0x1000U);
        }
    }
}