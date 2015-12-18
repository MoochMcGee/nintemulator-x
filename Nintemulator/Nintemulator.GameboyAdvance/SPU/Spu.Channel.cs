using System;

namespace Nintemulator.GBA.SPU
{
    public partial class Spu
    {
        public abstract class Channel : GameboyAdvance.Component
        {
            public static readonly int[] LevelLut =
            {
                 0x00, // $0
                 0x10, // $1
                 0x20, // $2
                 0x30, // $3
                 0x40, // $4
                 0x50, // $5
                 0x60, // $6
                 0x70, // $7
                -0x80, // $8
                -0x90, // $9
                -0xA0, // $a
                -0xB0, // $b
                -0xC0, // $c
                -0xD0, // $d
                -0xE0, // $e
                -0xF0, // $f
            };

            protected Duration duration = new Duration();
            protected Envelope envelope = new Envelope();
            protected Timing timing;
            protected bool active;
            protected byte[] registers = new byte[8];
            protected int frequency;

            public bool lenable;
            public bool renable;

            public virtual bool Enabled
            {
                get { return active; }
                set { active = value; }
            }

            public Channel(GameboyAdvance console, Timing timing)
                : base(console) { }

            protected virtual byte PeekRegister1(uint address) { return this.registers[0]; }
            protected virtual byte PeekRegister2(uint address) { return this.registers[1]; }
            protected virtual byte PeekRegister3(uint address) { return this.registers[2]; }
            protected virtual byte PeekRegister4(uint address) { return this.registers[3]; }
            protected virtual byte PeekRegister5(uint address) { return this.registers[4]; }
            protected virtual byte PeekRegister6(uint address) { return this.registers[5]; }
            protected virtual byte PeekRegister7(uint address) { return this.registers[6]; }
            protected virtual byte PeekRegister8(uint address) { return this.registers[7]; }
            protected virtual void PokeRegister1(uint address, byte data) { this.registers[0] = data; }
            protected virtual void PokeRegister2(uint address, byte data) { this.registers[1] = data; }
            protected virtual void PokeRegister3(uint address, byte data) { this.registers[2] = data; }
            protected virtual void PokeRegister4(uint address, byte data) { this.registers[3] = data; }
            protected virtual void PokeRegister5(uint address, byte data) { this.registers[4] = data; }
            protected virtual void PokeRegister6(uint address, byte data) { this.registers[5] = data; }
            protected virtual void PokeRegister7(uint address, byte data) { this.registers[6] = data; }
            protected virtual void PokeRegister8(uint address, byte data) { this.registers[7] = data; }

            public void ClockDuration()
            {
                if (duration.Clock())
                {
                    active = false;
                }
            }

            public virtual void Initialize(uint address)
            {
                this.Initialize();

                base.console.Hook(address + 0U, PeekRegister1, PokeRegister1);
                base.console.Hook(address + 1U, PeekRegister2, PokeRegister2);
                base.console.Hook(address + 2U, PeekRegister3, PokeRegister3);
                base.console.Hook(address + 3U, PeekRegister4, PokeRegister4);
                base.console.Hook(address + 4U, PeekRegister5, PokeRegister5);
                base.console.Hook(address + 5U, PeekRegister6, PokeRegister6);
                base.console.Hook(address + 6U, PeekRegister7, PokeRegister7);
                base.console.Hook(address + 7U, PeekRegister8, PokeRegister8);
            }
            public virtual int Render(int cycles) { return 0; }
        }
    }
}