using System;
using System.Runtime.InteropServices;

namespace Nintemulator.Shared.Processors
{
    public abstract class SPC700
    {
        public Bus bus;
        public Registers registers;

        protected abstract void tick(); // internal operation
        protected abstract void peek(); // memory access, read
        protected abstract void poke(); // memory access, write

        public struct Alu
        {
            public static byte operand1;
            public static byte operand2;
            public static byte result;

            public static int v; // overflow out
            public static int h; // half carry out
            public static int c; // carry out

            public static void add(int carry = 0)
            {
                result = (byte)((operand1 + operand2) + carry);

                var vbits = (~(operand1 ^ operand2) & (operand1 ^ result)); // 1 and 2 have the same sign, 1 and result don't.
                var cbits = ((operand1 & operand2) | ((operand1 ^ operand2) & ~result)); // 1 and 2 have the same bit7, or one of them has bit7 set and the result doesn't.

                v = (vbits >> 7);
                h = (cbits >> 3) & 1;
                c = (cbits >> 7);
            }
            public static void sub(int carry = 1)
            {
                operand2 ^= 0xff;
                add(carry);
                operand2 ^= 0xff;
            }
            public static void shl(int carry = 0)
            {
                c = (operand1 >> 7);
                operand1 = (byte)((operand1 << 1) | (carry >> 0));
            }
            public static void shr(int carry = 0)
            {
                c = (operand1 & 1);
                operand1 = (byte)((operand1 >> 1) | (carry << 7));
            }
        }

        public struct Bus
        {
            public ushort address;
            public byte data;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct Registers
        {
            [FieldOffset(0)] public byte a;
            [FieldOffset(1)] public byte y;
            [FieldOffset(2)] public byte x;
            // skip field offset 3, to re-align the rest of the fields to their natural boundaries

            [FieldOffset(4)] public byte eal;
            [FieldOffset(5)] public byte eah;

            [FieldOffset(6)] public byte spl;
            [FieldOffset(7)] public byte sph;

            [FieldOffset(8)] public byte pcl;
            [FieldOffset(9)] public byte pch;

            [FieldOffset(0)] public ushort ya;
            [FieldOffset(4)] public ushort ea;
            [FieldOffset(6)] public ushort sp;
            [FieldOffset(8)] public ushort pc;
        }
    }
}