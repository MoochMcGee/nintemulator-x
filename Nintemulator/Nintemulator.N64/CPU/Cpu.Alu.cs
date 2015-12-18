using s08 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using s64 = System.Int64;
using u01 = System.Boolean;
using u08 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;

namespace Nintemulator.N64.CPU
{
    public partial class Cpu
    {
        private void AluAdd (u32 a, u32 b, ref u32 result)
        {
            u32 temp = (a + b);
            u32 bits = (a & b) | ((a ^ b) & ~temp);
            u32 flow = (bits ^ (bits << 1)) & 0x80000000U;

            if (flow != 0U)
            {
                // integer overflow exception
                return;
            }

            result = temp;
        }
        private void AluAddu(u32 a, u32 b, ref u32 result) { result = (a + b); }
        private void AluSub (u32 a, u32 b, ref u32 result)
        {
            AluAdd(a, ~b + 1U, ref result);
        }
        private void AluSubu(u32 a, u32 b, ref u32 result) { result = (a - b); }

        // ------------------------------------
        // 64-bit x 64-bit = 128-bit Multiplier
        // ------------------------------------
        private void AluMult(u64 a, u64 b)
        {
            u64 prod0 = ((a >>  0) & 0xFFFFFFFFUL) * ((b >>  0) & 0xFFFFFFFFUL);
            u64 prod1 = ((a >>  0) & 0xFFFFFFFFUL) * ((b >> 32) & 0xFFFFFFFFUL);
            u64 prod2 = ((a >> 32) & 0xFFFFFFFFUL) * ((b >>  0) & 0xFFFFFFFFUL);
            u64 prod3 = ((a >> 32) & 0xFFFFFFFFUL) * ((b >> 32) & 0xFFFFFFFFUL);

            mh = prod3 + (prod1 >> 32) + (prod2 >> 32);
            ml = prod0;

            if ((ml += (prod1 <<= 32)) < prod1) mh++;
            if ((ml += (prod2 <<= 32)) < prod2) mh++;
        }
        private void AluMult(s64 a, s64 b)
        {
            AluMult(
                (u64)(a < 0L ? -a : a),
                (u64)(b < 0L ? -b : b));

            if ((a < 0) ^ (b < 0))
            {
                ml = ~ml;
                mh = ~mh;

                if (++ml == 0)
                    ++mh;
            }
        }

        // -----------------------------------
        // 32-bit x 32-bit = 64-bit Multiplier
        // -----------------------------------
        private void AluMult(u32 a, u32 b)
        {
            u64 result = a * b;

            ml = result;
            mh = 0;
        }
        private void AluMult(s32 a, s32 b)
        {
            s64 result = a * b;

            ml = (u64)(result);
            mh = (u64)(result >> 64);
        }
    }
}