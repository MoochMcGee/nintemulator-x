using System;

namespace Nintemulator.GBC.Boards
{
    public class Board : GameboyColor.Component
    {
        protected bool ramEnabled;
        protected byte[] ram;
        protected byte[] rom;
        protected uint ramMask;
        protected uint ramPage;
        protected uint romMask;
        protected uint romPage = 1u << 14;

        public Board( GameboyColor console, byte[] rom )
            : base( console )
        {
            this.rom = rom.Clone( ) as byte[];
            this.SetRomSize( this.rom[ 0x148 ] );
            this.SetRamSize( this.rom[ 0x149 ] );
        }

        protected virtual byte PeekRam( uint address )
        {
            if ( !ramEnabled || ram == null )
                return 0xff;

            return ram[ DecodeRam( address ) & ramMask ];
        }
        protected virtual byte PeekRom( uint address )
        {
            return rom[ DecodeRom( address ) & romMask ];
        }
        protected virtual void PokeRam( uint address, byte data )
        {
            if ( !ramEnabled || ram == null )
                return;

            ram[ DecodeRam( address ) & ramMask ] = data;
        }
        protected virtual void PokeRom( uint address, byte data ) { }

        protected virtual uint DecodeRam( uint address ) { return address; }
        protected virtual uint DecodeRom( uint address ) { return address; }
        protected virtual void SetRamSize( byte value )
        {
            switch ( value )
            {
            case 0x00: this.ram = null; break;
            case 0x01: this.ram = new byte[ 0x0800 ]; this.ramMask = 0x07ff; break;
            case 0x02: this.ram = new byte[ 0x2000 ]; this.ramMask = 0x1fff; break;
            case 0x03: this.ram = new byte[ 0x8000 ]; this.ramMask = 0x1fff; break;
            }
        }
        protected virtual void SetRomSize( byte value )
        {
            switch ( value )
            {
            case 0x00: this.romMask = 0x7fffu; break;   // 00h -  32KByte (no ROM banking)
            case 0x01: this.romMask = 0xffffu; break;   // 01h -  64KByte (4 banks)
            case 0x02: this.romMask = 0x1ffffu; break;  // 02h - 128KByte (8 banks)
            case 0x03: this.romMask = 0x3ffffu; break;  // 03h - 256KByte (16 banks)
            case 0x04: this.romMask = 0x7ffffu; break;  // 04h - 512KByte (32 banks)
            case 0x05: this.romMask = 0xfffffu; break;  // 05h -   1MByte (64 banks)  - only 63 banks used by MBC1
            case 0x06: this.romMask = 0x1fffffu; break; // 06h -   2MByte (128 banks) - only 125 banks used by MBC1
            case 0x07: this.romMask = 0x3fffffu; break; // 07h -   4MByte (256 banks)
            case 0x52: throw new NotSupportedException( "Multi-chip ROMs aren't supported yet." ); // 52h - 1.1MByte (72 banks)
            case 0x53: throw new NotSupportedException( "Multi-chip ROMs aren't supported yet." ); // 53h - 1.2MByte (80 banks)
            case 0x54: throw new NotSupportedException( "Multi-chip ROMs aren't supported yet." ); // 54h - 1.5MByte (96 banks)
            }
        }

        protected override void OnInitialize( )
        {
            if ( rom != null )
            {
                HookRom( );
            }

            if ( ram != null )
            {
                HookRam( );
            }

            console.Hook( 0xff50, DisableBios );
        }

        protected virtual void DisableBios( uint address, byte data )
        {
            console.Hook( 0x0000, 0x00ff, PeekRom, PokeRom );
            console.Hook( 0x0200, 0x08ff, PeekRom, PokeRom );
        }
        protected virtual void HookRam( ) { console.Hook( 0xa000u, 0xbfffu, PeekRam, PokeRam ); }
        protected virtual void HookRom( ) { console.Hook( 0x0000u, 0x7fffu, PeekRom, PokeRom ); }
    }
}