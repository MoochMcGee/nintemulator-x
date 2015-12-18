namespace Nintemulator.GBA.CPU
{
    public partial class Cpu
    {
        private uint Add(uint a, uint b, uint carry = 0U)
        {
            var r = (a + b + carry);

            if (cpsr.t != 0U || (code & (1U << 20)) != 0)
            {
                var overflow = ~(a ^ b) & (a ^ r);

                cpsr.n = r >> 31;
                cpsr.z = r == 0 ? 1U : 0U;
                cpsr.c = (overflow ^ a ^ b ^ r) >> 31;
                cpsr.v = (overflow) >> 31;
            }

            return r;
        }
        private uint Sub(uint a, uint b, uint carry = 1U)
        {
            return Add(a, ~b, carry);
        }
        private uint Mul(uint a, uint b, uint c)
        {
            Cycles += MultiplyCycleCalculation(b);

            a += b * c;

            if (cpsr.t != 0U || (code & (1 << 20)) != 0)
            {
                cpsr.n = a >> 31;
                cpsr.z = a == 0U ? 1U : 0U;
            }

            return a;
        }

        private uint Mov(uint value)
        {
            if (cpsr.t != 0U || (code & (1U << 20)) != 0)
            {
                cpsr.n = value >> 31;
                cpsr.z = value == 0U ? 1U : 0U;
                cpsr.c = carryout;
            }

            return value;
        }
        private uint Lsl(uint value, uint shift)
        {
            shift = (shift & 255U);

            carryout = cpsr.c;
            if (shift == 0) return value;

            carryout = shift > 32 ? 0 : (value >> (int)(32 - shift)) & 1U;
            value    = shift > 31 ? 0 : (value << (int)shift);
            return value;
        }
        private uint Lsr(uint value, uint shift)
        {
            shift = (shift & 255U);

            carryout = cpsr.c;
            if (shift == 0) return value;

            carryout = shift > 32 ? 0 : (value >> (int)(shift - 1)) & 1U;
            value    = shift > 31 ? 0 : (value >> (int)shift);
            return value;
        }
        private uint Asr(uint value, uint shift)
        {
            shift = (shift & 255U);

            carryout = cpsr.c;
            if (shift == 0) return value;

            carryout = shift > 32 ? (value >> 31) : (value >> (int)(shift - 1)) & 1U;
            value    = shift > 31 ? (uint)((int)value >> 31) : (uint)((int)value >> (int)shift);
            return value;
        }
        private uint Ror(uint value, uint shift)
        {
            shift = (shift & 255U);

            carryout = cpsr.c;
            if (shift == 0) return value;

            if ((shift &= 31) != 0)
                value = (value >> (int)shift) | (value << (int)(32 - shift));

            carryout = (value >> 31);
            return value;
        }
        private uint Rrx(uint value)
        {
            carryout = value & 1U;
            return (value >> 1) | (cpsr.c << 31);
        }
    }
}