using System;

namespace Nintemulator.GB.Boards
{
    public class Board : Gameboy.Component
    {
        protected byte[] ram;
        protected byte[] rom;
        protected int ramMask;
        protected int romMask = 0x7FFF;

        public Board(Gameboy gameboy, byte[] rom)
            : base(gameboy)
        {
            this.rom = rom.Clone() as byte[];
            this.SetRomSize(this.rom[0x148]);
            this.SetRamSize(this.rom[0x149]);
        }

        private byte PeekRam(uint address) { return ram[address & ramMask]; }
        private byte PeekRom(uint address) { return rom[address & romMask]; }
        private void PokeRam(uint address, byte data) { ram[address & ramMask] = data; }
        private void PokeRom(uint address, byte data) { }

        protected virtual void SetRamSize(byte value)
        {
            switch (value)
            {
            case 0x00: this.ram = null; break;
            case 0x01: this.ram = new byte[0x0800]; this.ramMask = 0x07FF; break;
            case 0x02: this.ram = new byte[0x2000]; this.ramMask = 0x1FFF; break;
            case 0x03: this.ram = new byte[0x8000]; this.ramMask = 0x1FFF; break;
            }
        }
        protected virtual void SetRomSize(byte value)
        {
            switch (value)
            {
            case 0x00: this.romMask = 0x7FFF; break;   // 00h -  32KByte (no ROM banking)
            case 0x01: this.romMask = 0xFFFF; break;   // 01h -  64KByte (4 banks)
            case 0x02: this.romMask = 0x1FFFF; break;  // 02h - 128KByte (8 banks)
            case 0x03: this.romMask = 0x3FFFF; break;  // 03h - 256KByte (16 banks)
            case 0x04: this.romMask = 0x7FFFF; break;  // 04h - 512KByte (32 banks)
            case 0x05: this.romMask = 0xFFFFF; break;  // 05h -   1MByte (64 banks)  - only 63 banks used by MBC1
            case 0x06: this.romMask = 0x1FFFFF; break; // 06h -   2MByte (128 banks) - only 125 banks used by MBC1
            case 0x07: this.romMask = 0x3FFFFF; break; // 07h -   4MByte (256 banks)
            case 0x52: throw new NotSupportedException("Multi-chip ROMs aren't supported yet."); // 52h - 1.1MByte (72 banks)
            case 0x53: throw new NotSupportedException("Multi-chip ROMs aren't supported yet."); // 53h - 1.2MByte (80 banks)
            case 0x54: throw new NotSupportedException("Multi-chip ROMs aren't supported yet."); // 54h - 1.5MByte (96 banks)
            }
        }

        protected override void OnInitialize()
        {
            if (rom != null)
            {
                HookRom();
            }

            if (ram != null)
            {
                HookRam();
            }

            console.Hook(0xFF50, DisableBios);
        }

        protected virtual void DisableBios(uint address, byte data)
        {
            console.Hook(0x0000, 0x00FF, PeekRom, PokeRom);
        }
        protected virtual void HookRam() { console.Hook(0xA000, 0xBFFF, PeekRam, PokeRam); }
        protected virtual void HookRom() { console.Hook(0x0000, 0x7FFF, PeekRom, PokeRom); }
    }
}