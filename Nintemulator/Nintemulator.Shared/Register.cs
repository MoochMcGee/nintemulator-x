using System.Runtime.InteropServices;
using half = System.UInt16;

namespace Nintemulator.Shared
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Register16
    {
        [field: FieldOffset(0)] public byte l;
        [field: FieldOffset(1)] public byte h;
        [field: FieldOffset(0)] public half w;

        public override string ToString() { return string.Format("0x{0:x4}", w); }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Register24
    {
        [field: FieldOffset(0)] public byte l;
        [field: FieldOffset(1)] public byte h;
        [field: FieldOffset(2)] public byte b;

        [field: FieldOffset(0)] public half w;
        [field: FieldOffset(0)] public uint d;

        public override string ToString() { return string.Format("0x{0:x6}", d); }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Register32
    {
        // unsigned
        [field: FieldOffset(0)] public byte ub0;
        [field: FieldOffset(1)] public byte ub1;
        [field: FieldOffset(2)] public byte ub2;
        [field: FieldOffset(3)] public byte ub3;
        [field: FieldOffset(0)] public half uw0;
        [field: FieldOffset(2)] public half uw1;
        [field: FieldOffset(0)] public uint ud0;

        // signed
        [field: FieldOffset(0)] public sbyte sb0;
        [field: FieldOffset(1)] public sbyte sb1;
        [field: FieldOffset(2)] public sbyte sb2;
        [field: FieldOffset(3)] public sbyte sb3;
        [field: FieldOffset(0)] public short sw0;
        [field: FieldOffset(2)] public short sw1;
        [field: FieldOffset(0)] public int sd0;

        public override string ToString() { return string.Format("0x{0:x8}", ud0); }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Register64
    {
        // unsigned
        [field: FieldOffset(0)] public byte ub0;
        [field: FieldOffset(1)] public byte ub1;
        [field: FieldOffset(2)] public byte ub2;
        [field: FieldOffset(3)] public byte ub3;
        [field: FieldOffset(4)] public byte ub4;
        [field: FieldOffset(5)] public byte ub5;
        [field: FieldOffset(6)] public byte ub6;
        [field: FieldOffset(7)] public byte ub7;
        [field: FieldOffset(0)] public ushort uw0;
        [field: FieldOffset(2)] public ushort uw1;
        [field: FieldOffset(4)] public ushort uw2;
        [field: FieldOffset(6)] public ushort uw3;
        [field: FieldOffset(0)] public uint ud0;
        [field: FieldOffset(4)] public uint ud1;
        [field: FieldOffset(0)] public ulong uq0;

        // signed
        [field: FieldOffset(0)] public sbyte sb0;
        [field: FieldOffset(1)] public sbyte sb1;
        [field: FieldOffset(2)] public sbyte sb2;
        [field: FieldOffset(3)] public sbyte sb3;
        [field: FieldOffset(4)] public sbyte sb4;
        [field: FieldOffset(5)] public sbyte sb5;
        [field: FieldOffset(6)] public sbyte sb6;
        [field: FieldOffset(7)] public sbyte sb7;
        [field: FieldOffset(0)] public short sw0;
        [field: FieldOffset(2)] public short sw1;
        [field: FieldOffset(4)] public short sw2;
        [field: FieldOffset(6)] public short sw3;
        [field: FieldOffset(0)] public int sd0;
        [field: FieldOffset(4)] public int sd1;
        [field: FieldOffset(0)] public long sq0;

        public override string ToString() { return string.Format("0x{0:x16}", uq0); }
    }
}