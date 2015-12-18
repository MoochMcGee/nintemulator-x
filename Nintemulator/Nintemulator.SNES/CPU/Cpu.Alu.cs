using word = System.UInt16;

namespace Nintemulator.SFC.CPU
{
    public partial class Cpu
    {
        private void and(byte data) { mov(regs.al &= data); }
        private void and(word data) { mov(regs.a &= data); }
        private void bit(byte data)
        {
            flags.n.i = (data >> 7) & 1;
            flags.v.i = (data >> 6) & 1;
            flags.z.b = (data & regs.al) == 0;
        }
        private void bit(word data)
        {
            flags.n.i = (data >> 15) & 1;
            flags.v.i = (data >> 14) & 1;
            flags.z.b = (data & regs.a) == 0;
        }
        private void eor(byte data) { mov(regs.al ^= data); }
        private void eor(word data) { mov(regs.a ^= data); }
        private byte mov(byte data)
        {
            flags.n.i = (data & 0x80) >> 7;
            flags.z.b = (data & 0xff) == 0;
            return data;
        }
        private word mov(word data)
        {
            flags.n.i = (data & 0x8000) >> 15;
            flags.z.b = (data & 0xffff) == 0;
            return (data);
        }
        private void ora(byte data) { mov(regs.al |= data); }
        private void ora(word data) { mov(regs.a |= data); }
        private byte trb(byte data)
        {
            flags.z.b = (data & regs.al) == 0;
            return (byte)(data & ~regs.al);
        }
        private word trb(word data)
        {
            flags.z.b = (data & regs.a) == 0;
            return (word)(data & ~regs.a);
        }
        private byte tsb(byte data)
        {
            flags.z.b = (data & regs.al) == 0;
            return (byte)(data | regs.al);
        }
        private word tsb(word data)
        {
            flags.z.b = (data & regs.a) == 0;
            return (word)(data | regs.a);
        }
        private byte shl(byte data, int carry = 0)
        {
            flags.c.i = (data >> 7);
            data = (byte)((data << 1) | (carry << 0));

            return mov(data);
        }
        private word shl(word data, int carry = 0)
        {
            flags.c.i = (data >> 15);
            data = (word)((data << 1) | (carry << 0));

            return mov(data);
        }
        private byte shr(byte data, int carry = 0)
        {
            flags.c.i = (data & 1);
            data = (byte)((data >> 1) | (carry << 7));

            return mov(data);
        }
        private word shr(word data, int carry = 0)
        {
            flags.c.i = (data & 1);
            data = (word)((data >> 1) | (carry << 15));

            return mov(data);
        }

        private byte dec(byte data) { return mov(--data); }
        private word dec(word data) { return mov(--data); }
        private byte inc(byte data) { return mov(++data); }
        private word inc(word data) { return mov(++data); }
        private void adc(byte data)
        {
            int temp = 0;

            if (!flags.d.b)
            {
                temp = regs.al + data + flags.c.i;
            }
            else
            {
                temp = (regs.al & 0x0f) + (data & 0x0f) + (flags.c.i << 0) + (temp & 0x00); if (temp > 0x09) temp += 0x06; flags.c.b = temp > 0x0f;
                temp = (regs.al & 0xf0) + (data & 0xf0) + (flags.c.i << 4) + (temp & 0x0f);
            }

            flags.v.i = (~(regs.al ^ data) & (regs.al ^ temp) & 0x80) >> 7;
            if (flags.d.b && temp > 0x9f) temp += 0x60;
            flags.c.b = (temp > 0xff);

            regs.al = mov((byte)(temp));
        }
        private void adc(word data)
        {
            int temp = 0;

            if (!flags.d.b)
            {
                temp = regs.a + data + flags.c.i;
            }
            else
            {
                temp = (regs.a & 0x000f) + (data & 0x000f) + (flags.c.i << 0) + (temp & 0x0000); if (temp > 0x0009) temp += 0x0006; flags.c.b = temp > 0x000f;
                temp = (regs.a & 0x00f0) + (data & 0x00f0) + (flags.c.i << 4) + (temp & 0x000f); if (temp > 0x009f) temp += 0x0060; flags.c.b = temp > 0x00ff;
                temp = (regs.a & 0x0f00) + (data & 0x0f00) + (flags.c.i << 8) + (temp & 0x00ff); if (temp > 0x09ff) temp += 0x0600; flags.c.b = temp > 0x0fff;
                temp = (regs.a & 0xf000) + (data & 0xf000) + (flags.c.i << 12) + (temp & 0x0fff);
            }

            flags.v.i = (~(regs.a ^ data) & (regs.a ^ temp) & 0x8000) >> 15;
            if (flags.d.b && temp > 0x9fff) temp += 0x6000;
            flags.c.b = (temp > 0xffff);

            regs.a = mov((word)(temp));
        }
        private void sbc(byte data)
        {
            int temp = 0;
            data ^= 0xff;

            if (!flags.d.b)
            {
                temp = regs.al + data + flags.c.i;
            }
            else
            {
                temp = (regs.al & 0x0f) + (data & 0x0f) + (flags.c.i << 0) + (temp & 0x00); if (temp <= 0x0f) temp -= 0x06; flags.c.b = temp > 0x0f;
                temp = (regs.al & 0xf0) + (data & 0xf0) + (flags.c.i << 4) + (temp & 0x0f);
            }

            flags.v.i = (~(regs.al ^ data) & (regs.al ^ temp) & 0x80) >> 7;
            if (flags.d.b && temp <= 0xff) temp -= 0x60;
            flags.c.b = (temp > 0xff);

            regs.al = mov((byte)(temp));
        }
        private void sbc(word data)
        {
            int temp = 0;
            data ^= 0xffff;

            if (!flags.d.b)
            {
                temp = regs.a + data + flags.c.i;
            }
            else
            {
                temp = (regs.a & 0x000f) + (data & 0x000f) + (flags.c.i << 0) + (temp & 0x0000); if (temp <= 0x000f) temp -= 0x0006; flags.c.b = temp > 0x000f;
                temp = (regs.a & 0x00f0) + (data & 0x00f0) + (flags.c.i << 4) + (temp & 0x000f); if (temp <= 0x00ff) temp -= 0x0060; flags.c.b = temp > 0x00ff;
                temp = (regs.a & 0x0f00) + (data & 0x0f00) + (flags.c.i << 8) + (temp & 0x00ff); if (temp <= 0x0fff) temp -= 0x0600; flags.c.b = temp > 0x0fff;
                temp = (regs.a & 0xf000) + (data & 0xf000) + (flags.c.i << 12) + (temp & 0x0fff);
            }

            flags.v.i = (~(regs.a ^ data) & (regs.a ^ temp) & 0x8000) >> 15;
            if (flags.d.b && temp <= 0xffff) temp -= 0x6000;
            flags.c.b = (temp > 0xffff);

            regs.a = mov((word)(temp));
        }
        private void cmp(byte data) { mov(alu.sub(regs.al, data)); flags.c.i = alu.c; }
        private void cmp(word data) { mov(alu.sub(regs.a, data)); flags.c.i = alu.c; }
        private void cpx(byte data) { mov(alu.sub(regs.xl, data)); flags.c.i = alu.c; }
        private void cpx(word data) { mov(alu.sub(regs.x, data)); flags.c.i = alu.c; }
        private void cpy(byte data) { mov(alu.sub(regs.yl, data)); flags.c.i = alu.c; }
        private void cpy(word data) { mov(alu.sub(regs.y, data)); flags.c.i = alu.c; }

        public static class alu
        {
            public static int c;
            public static int v;

            public static byte add(byte a, byte b, int carry = 0)
            {
                byte temp = (byte)((a + b) + carry);
                byte bits = (byte)((a ^ temp) & ~(a ^ b));

                v = (bits) >> 7;
                c = (bits ^ temp ^ a ^ b) >> 7;

                return temp;
            }
            public static word add(word a, word b, int carry = 0)
            {
                word temp = (word)((a + b) + carry);
                word bits = (word)((a ^ temp) & ~(a ^ b));

                v = (bits) >> 15;
                c = (bits ^ temp ^ a ^ b) >> 15;

                return temp;
            }
            public static byte sub(byte a, byte b, int carry = 1)
            {
                b ^= 0xff;
                return add(a, b, carry);
            }
            public static word sub(word a, word b, int carry = 1)
            {
                b ^= 0xffff;
                return add(a, b, carry);
            }
        }
    }
}