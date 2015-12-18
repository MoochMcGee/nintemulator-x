using Nintemulator.Shared;
using s08 = System.SByte;
using s16 = System.Int16;
using s32 = System.Int32;
using s64 = System.Int64;
using u08 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;

namespace Nintemulator.N64.CPU
{
    public partial class Cpu
    {
        // FPU
        public class Cp1
        {
            private Register64[] fpr = new Register64[32];
            private u32 fcr00;
            private u32 fcr31;
        }
    }
}