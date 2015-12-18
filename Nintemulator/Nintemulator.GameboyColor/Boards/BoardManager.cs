using System;

namespace Nintemulator.GBC.Boards
{
    public static class BoardManager
    {
        public static Type GetBoard(byte[] rom)
        {
            switch (rom[0x0147])
            {
            case 0x00: return typeof(Board); // ROM ONLY
            case 0x01:
            case 0x02:
            case 0x03: return typeof(NintendoMBC1);
            case 0x05:
            case 0x06: return typeof(NintendoMBC2);
            case 0x08: throw new NotSupportedException(); // ROM+RAM
            case 0x09: throw new NotSupportedException(); // ROM+RAM+BATTERY
            case 0x0B: throw new NotSupportedException(); // MMM01
            case 0x0C: throw new NotSupportedException(); // MMM01+RAM
            case 0x0D: throw new NotSupportedException(); // MMM01+RAM+BATTERY
            case 0x0F:
            case 0x10:
            case 0x11:
            case 0x12:
            case 0x13: return typeof(NintendoMBC3);
            case 0x15: throw new NotSupportedException(); // MBC4
            case 0x16: throw new NotSupportedException(); // MBC4+RAM
            case 0x17: throw new NotSupportedException(); // MBC4+RAM+BATTERY
            case 0x19:
            case 0x1A:
            case 0x1B:
            case 0x1C:
            case 0x1D:
            case 0x1E: return typeof(NintendoMBC5);
            case 0xFC: throw new NotSupportedException(); // POCKET CAMERA
            case 0xFD: throw new NotSupportedException(); // BANDAI TAMA5
            case 0xFE: throw new NotSupportedException(); // HuC3
            case 0xFF: throw new NotSupportedException(); // HuC1+RAM+BATTERY
            }

            return null;
        }
    }
}