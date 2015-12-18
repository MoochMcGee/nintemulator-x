using Nintemulator.Shared;

namespace Nintemulator.GBC.SPU
{
    public partial class Spu
    {
        private const int DELAY = 8192;
        private const int PHASE = 375;

        public class Channel : GameboyColor.Component
        {
            private static readonly byte[][] RegisterTable =
            {
                new byte[] { 0x80, 0x3F, 0x00, 0xFF, 0xBF },
                new byte[] { 0xFF, 0x3F, 0x00, 0xFF, 0xBF },
                new byte[] { 0x7F, 0xFF, 0x9F, 0xFF, 0xBF },
                new byte[] { 0xFF, 0xFF, 0x00, 0x00, 0xBF }
            };

            protected Duration duration = new Duration();
            protected Envelope envelope = new Envelope();
            protected Timing timing;
            protected Timing.System system;
            protected bool active;
            protected bool power;
            protected byte[] emptyBits;
            protected byte[] registers = new byte[5];
            protected int frequency;

            public bool Enabled { get { return active; } }

            public Channel(GameboyColor console, Timing.System system, int channel)
                : base(console)
            {
                this.system = system;
                this.emptyBits = RegisterTable[channel];
            }

            protected virtual void OnPokeReg1(byte data) { }
            protected virtual void OnPokeReg2(byte data) { }
            protected virtual void OnPokeReg3(byte data) { }
            protected virtual void OnPokeReg4(byte data) { }
            protected virtual void OnPokeReg5(byte data) { }

            public void ClockDuration()
            {
                if (duration.Clock())
                {
                    active = false;
                }
            }

            public void PowerOff()
            {
                power = true;

                PokeReg1(0x0000, 0x00);
                PokeReg2(0x0000, 0x00);
                PokeReg3(0x0000, 0x00);
                PokeReg4(0x0000, 0x00);
                PokeReg5(0x0000, 0x00);

                active = false;
                power = false;
            }
            public void PowerOn()
            {
                power = true;
            }

            public virtual byte PeekReg1(uint address) { return (byte)(registers[0] | emptyBits[0]); }
            public virtual byte PeekReg2(uint address) { return (byte)(registers[1] | emptyBits[1]); }
            public virtual byte PeekReg3(uint address) { return (byte)(registers[2] | emptyBits[2]); }
            public virtual byte PeekReg4(uint address) { return (byte)(registers[3] | emptyBits[3]); }
            public virtual byte PeekReg5(uint address) { return (byte)(registers[4] | emptyBits[4]); }
            public virtual void PokeReg1(uint address, byte data)
            {
                if (power)
                {
                    OnPokeReg1(registers[0] = data);
                }
            }
            public virtual void PokeReg2(uint address, byte data)
            {
                if (power)
                {
                    OnPokeReg2(registers[1] = data);
                }
            }
            public virtual void PokeReg3(uint address, byte data)
            {
                if (power)
                {
                    OnPokeReg3(registers[2] = data);
                }
            }
            public virtual void PokeReg4(uint address, byte data)
            {
                if (power)
                {
                    OnPokeReg4(registers[3] = data);
                }
            }
            public virtual void PokeReg5(uint address, byte data)
            {
                if (power)
                {
                    OnPokeReg5(registers[4] = data);
                }
            }

            public virtual void Initialize(uint address)
            {
                Initialize();

                console.Hook(address + 0U, PeekReg1, PokeReg1);
                console.Hook(address + 1U, PeekReg2, PokeReg2);
                console.Hook(address + 2U, PeekReg3, PokeReg3);
                console.Hook(address + 3U, PeekReg4, PokeReg4);
                console.Hook(address + 4U, PeekReg5, PokeReg5);
            }
            public virtual byte Sample() { return 0; }
        }
    }
}