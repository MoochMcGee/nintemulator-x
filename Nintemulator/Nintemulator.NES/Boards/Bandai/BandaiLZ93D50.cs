using Nintemulator.FC.GPU;

namespace Nintemulator.FC.Boards.Bandai
{
    public class BandaiLZ93D50 : Board
    {
        private EepromBase eeprom;
        private bool irq_enabled;
        private uint irq_counter;

        public BandaiLZ93D50(Famicom console, byte[] cart, FamicomDatabase.Game.Cartridge.Board board)
            : base(console, cart, board)
        {
            chrPage = new uint[8U];
            prgPage = new uint[2U];
        }

        private byte Peek800D(uint address)
        {
            if (eeprom != null)
                return eeprom.Peek();

            return cpu.Open;
        }
        private void Poke8000(uint address, byte data) { chrPage[0U] = (data & 0xFFU) << 10; }
        private void Poke8001(uint address, byte data) { chrPage[1U] = (data & 0xFFU) << 10; }
        private void Poke8002(uint address, byte data) { chrPage[2U] = (data & 0xFFU) << 10; }
        private void Poke8003(uint address, byte data) { chrPage[3U] = (data & 0xFFU) << 10; }
        private void Poke8004(uint address, byte data) { chrPage[4U] = (data & 0xFFU) << 10; }
        private void Poke8005(uint address, byte data) { chrPage[5U] = (data & 0xFFU) << 10; }
        private void Poke8006(uint address, byte data) { chrPage[6U] = (data & 0xFFU) << 10; }
        private void Poke8007(uint address, byte data) { chrPage[7U] = (data & 0xFFU) << 10; }
        private void Poke8008(uint address, byte data) { prgPage[0U] = (data & 0xFFU) << 14; }
        private void Poke8009(uint address, byte data)
        {
            switch (data & 0x03U)
            {
            case 0U: gpu.SwitchNametables(Mirroring.ModeVert); break;
            case 1U: gpu.SwitchNametables(Mirroring.ModeHorz); break;
            case 2U: gpu.SwitchNametables(Mirroring.Mode1ScA); break;
            case 3U: gpu.SwitchNametables(Mirroring.Mode1ScB); break;
            }
        }
        private void Poke800A(uint address, byte data)
        {
            irq_enabled = (data & 0x01U) != 0;
            cpu.Irq(0u);
        }
        private void Poke800B(uint address, byte data) { irq_counter = (irq_counter & ~0x00FFU) | ((data & 0xFFU) << 0); }
        private void Poke800C(uint address, byte data) { irq_counter = (irq_counter & ~0xFF00U) | ((data & 0xFFU) << 8); }
        private void Poke800D(uint address, byte data)
        {
            if (eeprom != null)
                eeprom.Poke(data & 0x20U, data & 0x40U);
        }

        protected override uint DecodeChr(uint address)
        {
            return (address & 0x03FFU) | chrPage[(address >> 10) & 7U];
        }
        protected override uint DecodePrg(uint address)
        {
            return (address & 0x3FFFU) | prgPage[(address >> 14) & 1U];
        }
        protected override void OnInitialize()
        {
            base.OnInitialize();

            prgPage[0U] = 0x00000000U << 14;
            prgPage[1U] = 0xFFFFFFFFU << 14;

            for (uint i = 0x6000U; i <= 0x7FFFU; i++)
            {
                switch (i & 0x000FU)
                {
                case 0x0000U: cpu.Hook(i, Peek800D, Poke8000); break;
                case 0x0001U: cpu.Hook(i, Peek800D, Poke8001); break;
                case 0x0002U: cpu.Hook(i, Peek800D, Poke8002); break;
                case 0x0003U: cpu.Hook(i, Peek800D, Poke8003); break;
                case 0x0004U: cpu.Hook(i, Peek800D, Poke8004); break;
                case 0x0005U: cpu.Hook(i, Peek800D, Poke8005); break;
                case 0x0006U: cpu.Hook(i, Peek800D, Poke8006); break;
                case 0x0007U: cpu.Hook(i, Peek800D, Poke8007); break;
                case 0x0008U: cpu.Hook(i, Peek800D, Poke8008); break;
                case 0x0009U: cpu.Hook(i, Peek800D, Poke8009); break;
                case 0x000AU: cpu.Hook(i, Peek800D, Poke800A); break;
                case 0x000BU: cpu.Hook(i, Peek800D, Poke800B); break;
                case 0x000CU: cpu.Hook(i, Peek800D, Poke800C); break;
                case 0x000DU: cpu.Hook(i, Peek800D, Poke800D); break;
                case 0x000EU: break; // open bus?
                case 0x000FU: break; // open bus?
                }
            }

            for (uint i = 0x8000U; i <= 0xFFFFU; i++)
            {
                switch (i & 0x000FU)
                {
                case 0x0000U: cpu.Hook(i, Poke8000); break;
                case 0x0001U: cpu.Hook(i, Poke8001); break;
                case 0x0002U: cpu.Hook(i, Poke8002); break;
                case 0x0003U: cpu.Hook(i, Poke8003); break;
                case 0x0004U: cpu.Hook(i, Poke8004); break;
                case 0x0005U: cpu.Hook(i, Poke8005); break;
                case 0x0006U: cpu.Hook(i, Poke8006); break;
                case 0x0007U: cpu.Hook(i, Poke8007); break;
                case 0x0008U: cpu.Hook(i, Poke8008); break;
                case 0x0009U: cpu.Hook(i, Poke8009); break;
                case 0x000AU: cpu.Hook(i, Poke800A); break;
                case 0x000BU: cpu.Hook(i, Poke800B); break;
                case 0x000CU: cpu.Hook(i, Poke800C); break;
                case 0x000DU: cpu.Hook(i, Poke800D); break;
                case 0x000EU: break; // open bus?
                case 0x000FU: break; // open bus?
                }
            }

            switch (type)
            {
            case "BANDAI-LZ93D50+24C01": eeprom = new Eeprom128(); break; // Xicor 24C01P
            case "BANDAI-LZ93D50+24C02": eeprom = new Eeprom256(); break; // Xicor 24C02P
            }
        }

        public override void Clock()
        {
            if (irq_enabled)
            {
                irq_counter = (--irq_counter & 0xFFFFU);

                if (irq_counter == 0x0000U)
                    cpu.Irq(1u);
            }
        }

        public class EepromBase
        {
            protected Mode mode;
            protected Mode next;
            protected byte output;
            protected byte latch_data;
            protected byte[] mem = new byte[256];
            protected uint latch_addr;
            protected uint latch_bit;
            protected uint scl_line;
            protected uint sda_line;

            protected virtual void Fall() { }
            protected virtual void Idle()
            {
                mode = Mode.Idle;
                output = 0x10;
            }
            protected virtual void Open() { }
            protected virtual void Rise(uint bit) { }

            public byte Peek()
            {
                return output;
            }
            public void Poke(uint scl, uint sda)
            {
                if (sda_line > sda && scl_line != 0) { Open(); goto update; } // SCL: 1, SDA: 1->0
                if (sda_line < sda && scl_line != 0) { Idle(); goto update; } // SCL: 1, SDA: 0->1
                if (scl_line > scl) { Fall(); goto update; } // SCL: 1->0
                if (scl_line < scl) { Rise(sda >> 6); goto update; } // SCL: 0->1

            update:
                scl_line = scl;
                sda_line = sda;
            }

            protected enum Mode
            {
                Idle,
                Data,
                Addr,
                Peek,
                Poke,
                Ack,
                NotAck,
                AckWait,
                Max
            }
        }
        public class Eeprom128 : EepromBase
        {
            protected override void Fall()
            {
                switch (mode)
                {
                case Mode.Addr:

                    if (latch_bit == 8)
                    {
                        mode = Mode.Ack;
                        output = 0x10;
                    }
                    break;

                case Mode.Ack:

                    mode = next;
                    latch_bit = 0;
                    output = 0x10;
                    break;

                case Mode.Peek:

                    if (latch_bit == 8)
                    {
                        mode = Mode.AckWait;
                        latch_addr = (latch_addr + 1) & 0x7F;
                    }
                    break;

                case Mode.Poke:

                    if (latch_bit == 8)
                    {
                        mode = Mode.Ack;
                        next = Mode.Idle;
                        mem[latch_addr] = latch_data;
                        latch_addr = (latch_addr + 1) & 0x7F;
                    }
                    break;
                }
            }
            protected override void Open()
            {
                mode = Mode.Addr;
                output = 0x10;
                latch_bit = 0;
                latch_addr = 0;
            }
            protected override void Rise(uint bit)
            {
                switch (mode)
                {
                case Mode.Addr:

                    if (latch_bit < 7)
                    {
                        latch_addr &= ~(1U << (int)latch_bit);
                        latch_addr |= (bit << (int)latch_bit++);
                    }
                    else if (latch_bit < 8)
                    {
                        latch_bit = 8;

                        if (bit != 0)
                        {
                            next = Mode.Peek;
                            latch_data = mem[latch_addr];
                        }
                        else
                        {
                            next = Mode.Poke;
                        }
                    }
                    break;

                case Mode.Ack:

                    output = 0x00;
                    break;

                case Mode.Peek:

                    if (latch_bit < 8)
                        output = (byte)((latch_data & (1U << (int)latch_bit++)) != 0U ? 0x10 : 0x00);

                    break;

                case Mode.Poke:

                    if (latch_bit < 8)
                    {
                        latch_data &= (byte)~(1U << (int)latch_bit);
                        latch_data |= (byte)(bit << (int)latch_bit++);
                    }
                    break;

                case Mode.AckWait:

                    if (bit == 0)
                        next = Mode.Idle;

                    break;
                }
            }
        }
        public class Eeprom256 : EepromBase
        {
            private uint rw;

            protected override void Fall()
            {
                switch (mode)
                {
                case Mode.Data:
                    if (latch_bit == 8)
                    {
                        if ((latch_data & 0xA0) == 0xA0)
                        {
                            latch_bit = 0;
                            mode = Mode.Ack;
                            rw = latch_data & 0x01U;
                            output = 0x10;

                            if (rw != 0U)
                            {
                                next = Mode.Peek;
                                latch_data = mem[latch_addr];
                            }
                            else
                            {
                                next = Mode.Addr;
                            }
                        }
                        else
                        {
                            mode = Mode.NotAck;
                            next = Mode.Idle;
                            output = 0x10;
                        }
                    }
                    break;

                case Mode.Addr:
                    if (latch_bit == 8)
                    {
                        latch_bit = 0;
                        mode = Mode.Ack;
                        next = (rw != 0U ? Mode.Idle : Mode.Poke);
                        output = 0x10;
                    }
                    break;

                case Mode.Peek:
                    if (latch_bit == 8)
                    {
                        mode = Mode.AckWait;
                        latch_addr = (latch_addr + 1) & 0xFF;
                    }
                    break;

                case Mode.Poke:
                    if (latch_bit == 8)
                    {
                        latch_bit = 0;
                        mode = Mode.Ack;
                        next = Mode.Poke;
                        mem[latch_addr] = latch_data;
                        latch_addr = (latch_addr + 1U) & 0xFFU;
                    }
                    break;

                case Mode.NotAck:
                    mode = Mode.Idle;
                    latch_bit = 0;
                    output = 0x10;
                    break;

                case Mode.Ack:
                case Mode.AckWait:
                    mode = next;
                    latch_bit = 0;
                    output = 0x10;
                    break;
                }
            }
            protected override void Open()
            {
                mode = Mode.Data;
                output = 0x10;
                latch_bit = 0;
            }
            protected override void Rise(uint bit)
            {
                switch (mode)
                {
                case Mode.Data:
                case Mode.Poke:
                    if (latch_bit < 8)
                    {
                        latch_data &= (byte)~(1U << (int)(7U - latch_bit));
                        latch_data |= (byte)(bit << (int)(7U - latch_bit++));
                    }
                    break;

                case Mode.Addr:
                    if (latch_bit < 8)
                    {
                        latch_addr &= (byte)~(1U << (int)(7 - latch_bit));
                        latch_addr |= (byte)(bit << (int)(7 - latch_bit++));
                    }
                    break;

                case Mode.Peek:
                    if (latch_bit < 8)
                        output = (byte)((latch_data & (1U << (int)(7U - latch_bit++))) != 0 ? 0x10 : 0x00);
                    break;

                case Mode.NotAck: output = 0x10; break;
                case Mode.Ack: output = 0x00; break;
                case Mode.AckWait:
                    if (bit == 0)
                    {
                        next = Mode.Peek;
                        latch_data = mem[latch_addr];
                    }
                    break;
                }
            }
        }
    }
}