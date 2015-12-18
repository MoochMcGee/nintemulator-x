using Nintemulator.Shared;
using System;
using System.Text;

namespace Nintemulator.FC.CPU.Debugging
{
    public static class Disassembler
    {
        private delegate string AddressingMode( ref ushort pc, Cpu cpu );

        private static AddressingMode[] modes = new AddressingMode[ 256 ]
        {
        //    0    1    2    3    4    5    6    7    8    9    A    B    C    D    E    F
            imm, inx, imp, inx, zpg, zpg, zpg, zpg, acc, imm, imp, imm, abs, abs, abs, abs, // 0
            rel, iny, imp, iny, zpx, zpx, zpx, zpx, imp, aby, imp, aby, abx, abx, abx, abx, // 1
            abs, inx, imp, inx, zpg, zpg, zpg, zpg, acc, imm, imp, imm, abs, abs, abs, abs, // 2
            rel, iny, imp, iny, zpx, zpx, zpx, zpx, imp, aby, imp, aby, abx, abx, abx, abx, // 3
            imp, inx, imp, inx, zpg, zpg, zpg, zpg, acc, imm, imp, imm, abs, abs, abs, abs, // 4
            rel, iny, imp, iny, zpx, zpx, zpx, zpx, imp, aby, imp, aby, abx, abx, abx, abx, // 5
            imp, inx, imp, inx, zpg, zpg, zpg, zpg, acc, imm, imp, imm, ind, abs, abs, abs, // 6
            rel, iny, imp, iny, zpx, zpx, zpx, zpx, imp, aby, imp, aby, abx, abx, abx, abx, // 7
            imm, inx, imm, inx, zpg, zpg, zpg, zpg, imp, imm, imp, imm, abs, abs, abs, abs, // 8
            rel, iny, imp, iny, zpx, zpx, zpy, zpy, imp, aby, imp, aby, abs, abx, abs, aby, // 9
            imm, inx, imm, inx, zpg, zpg, zpg, zpg, imp, imm, imp, imm, abs, abs, abs, abs, // A
            rel, iny, imp, iny, zpx, zpx, zpy, zpy, imp, aby, imp, aby, abx, abx, aby, aby, // B
            imm, inx, imm, inx, zpg, zpg, zpg, zpg, imp, imm, imp, imm, abs, abs, abs, abs, // C
            rel, iny, imp, iny, zpx, zpx, zpx, zpx, imp, aby, imp, aby, abx, abx, abx, abx, // D
            imm, inx, imm, inx, zpg, zpg, zpg, zpg, imp, imm, imp, imm, abs, abs, abs, abs, // E
            rel, iny, imp, iny, zpx, zpx, zpx, zpx, imp, aby, imp, aby, abx, abx, abx, abx, // F
        };
        private static string[] codes = new string[ 256 ]
        {
            //  0      1      2      3      4      5      6      7      8      9      A      B      C      D      E      F
            "brk", "ora", "jam", "slo", "dop", "ora", "asl", "slo", "php", "ora", "asl", "anc", "top", "ora", "asl", "slo", // 0
            "bpl", "ora", "jam", "slo", "dop", "ora", "asl", "slo", "clc", "ora", "nop", "slo", "top", "ora", "asl", "slo", // 1
            "jsr", "and", "jam", "rla", "bit", "and", "rol", "rla", "plp", "and", "rol", "anc", "bit", "and", "rol", "rla", // 2
            "bmi", "and", "jam", "rla", "dop", "and", "rol", "rla", "sec", "and", "nop", "rla", "top", "and", "rol", "rla", // 3
            "rti", "eor", "jam", "sre", "dop", "eor", "lsr", "sre", "pha", "eor", "lsr", "asr", "jmp", "eor", "lsr", "sre", // 4
            "bvc", "eor", "jam", "sre", "dop", "eor", "lsr", "sre", "cli", "eor", "nop", "sre", "top", "eor", "lsr", "sre", // 5
            "rts", "adc", "jam", "rra", "dop", "adc", "ror", "rra", "pla", "adc", "ror", "arr", "jmp", "adc", "ror", "rra", // 6
            "bvs", "adc", "jam", "rra", "dop", "adc", "ror", "rra", "sei", "adc", "nop", "rra", "top", "adc", "ror", "rra", // 7
            "dop", "sta", "dop", "aax", "sty", "sta", "stx", "aax", "dey", "dop", "txa", "xaa", "sty", "sta", "stx", "aax", // 8
            "bcc", "sta", "jam", "axa", "sty", "sta", "stx", "aax", "tya", "sta", "txs", "xas", "sya", "sta", "sxa", "axa", // 9
            "ldy", "lda", "ldx", "lax", "ldy", "lda", "ldx", "lax", "tay", "lda", "tax", "lax", "ldy", "lda", "ldx", "lax", // a
            "bcs", "lda", "jam", "lax", "ldy", "lda", "ldx", "lax", "clv", "lda", "tsx", "lar", "ldy", "lda", "ldx", "lax", // b
            "cpy", "cmp", "dop", "dcp", "cpy", "cmp", "dec", "dcp", "iny", "cmp", "dex", "axs", "cpy", "cmp", "dec", "dcp", // c
            "bne", "cmp", "jam", "dcp", "dop", "cmp", "dec", "dcp", "cld", "cmp", "nop", "dcp", "top", "cmp", "dec", "dcp", // d
            "cpx", "sbc", "dop", "isc", "cpx", "sbc", "inc", "isc", "inx", "sbc", "nop", "sbc", "cpx", "sbc", "inc", "isc", // e
            "beq", "sbc", "jam", "isc", "dop", "sbc", "inc", "isc", "sed", "sbc", "nop", "isc", "top", "sbc", "inc", "isc", // f
        };

        #region Modes

        private static string abs( ref ushort pc, Cpu cpu )
        {
            Register16 ea = default( Register16 );
            ea.l = cpu.PeekByteDebugger( pc++ );
            ea.h = cpu.PeekByteDebugger( pc++ );

            return string.Format( " ${0:x4}", ea.w );
        }
        private static string abx( ref ushort pc, Cpu cpu )
        {
            Register16 ea = default( Register16 );
            ea.l = cpu.PeekByteDebugger( pc++ );
            ea.h = cpu.PeekByteDebugger( pc++ );

            return string.Format( " ${0:x4},x\t[${1:x4}]", ea.w, ( ea.w + cpu.x ) & 0xffff );
        }
        private static string aby( ref ushort pc, Cpu cpu )
        {
            Register16 ea = default( Register16 );
            ea.l = cpu.PeekByteDebugger( pc++ );
            ea.h = cpu.PeekByteDebugger( pc++ );

            return string.Format( " ${0:x4},y\t[${1:x4}]", ea.w, ( ea.w + cpu.y ) & 0xffff );
        }
        private static string acc( ref ushort pc, Cpu cpu ) { return " a"; }
        private static string imm( ref ushort pc, Cpu cpu )
        {
            return string.Format( " #${0:x2}", cpu.PeekByteDebugger( pc++ ) );
        }
        private static string imp( ref ushort pc, Cpu cpu ) { return " "; }
        private static string ind( ref ushort pc, Cpu cpu )
        {
            Register16 ea = default( Register16 );
            ea.l = cpu.PeekByteDebugger( pc++ );
            ea.h = cpu.PeekByteDebugger( pc++ );

            var l = cpu.PeekByteDebugger( ea.w ); ea.l++;
            var h = cpu.PeekByteDebugger( ea.w ); ea.l--;

            return string.Format( " ${0:x4},y\t[${1:x2}{2:x2}]", ea.w, h, l );
        }
        private static string inx( ref ushort pc, Cpu cpu )
        {
            byte pointer = cpu.PeekByteDebugger( pc++ );

            Register16 ea = default( Register16 );
            ea.l = cpu.PeekByteDebugger( ( pointer + 0u + cpu.x ) & 0xff );
            ea.h = cpu.PeekByteDebugger( ( pointer + 1u + cpu.x ) & 0xff );

            return string.Format( " (${0:x2},x)\t[${1:x4}]", pointer, ea.w );
        }
        private static string iny( ref ushort pc, Cpu cpu )
        {
            byte pointer = cpu.PeekByteDebugger( pc++ );

            Register16 ea = default( Register16 );
            ea.l = cpu.PeekByteDebugger( ( pointer + 0u ) & 0xff );
            ea.h = cpu.PeekByteDebugger( ( pointer + 1u ) & 0xff );
            ea.w += cpu.y;

            return string.Format( " (${0:x2}),y\t[${1:x4}]", pointer, ea.w );
        }
        private static string rel( ref ushort pc, Cpu cpu )
        {
            var offset = cpu.PeekByteDebugger( pc++ );

            return string.Format( " ${0:x4}", ( pc + ( ushort )( sbyte )offset ) & 0xffff );
        }
        private static string zpg( ref ushort pc, Cpu cpu )
        {
            Register16 ea = default( Register16 );
            ea.l = cpu.PeekByteDebugger( pc++ );
            ea.h = 0;

            return string.Format( " ${0:x2}", ea.l );
        }
        private static string zpx( ref ushort pc, Cpu cpu )
        {
            Register16 ea = default( Register16 );
            ea.l = cpu.PeekByteDebugger( pc++ );
            ea.h = 0;

            return string.Format( " ${0:x2}\t[${0:x4}]", ea.l, ( ea.l + cpu.x ) & 0xff );
        }
        private static string zpy( ref ushort pc, Cpu cpu )
        {
            Register16 ea = default( Register16 );
            ea.l = cpu.PeekByteDebugger( pc++ );
            ea.h = 0;

            return string.Format( " ${0:x2}\t[${0:x4}]", ea.l, ( ea.l + cpu.y ) & 0xff );
        }

        #endregion

        public static string Disassemble( ref ushort pc, Cpu cpu )
        {
            switch ( pc & 0xfffeu )
            {
            case 0xfffau: return string.Format( "nmi: .dw ${1:x2}{0:x2}", cpu.PeekByteDebugger( 0xfffau ), cpu.PeekByteDebugger( 0xfffbu ) );
            case 0xfffcu: return string.Format( "rst: .dw ${1:x2}{0:x2}", cpu.PeekByteDebugger( 0xfffcu ), cpu.PeekByteDebugger( 0xfffdu ) );
            case 0xfffeu: return string.Format( "irq: .dw ${1:x2}{0:x2}", cpu.PeekByteDebugger( 0xfffeu ), cpu.PeekByteDebugger( 0xffffu ) );
            }

            var data = cpu.PeekByteDebugger( pc++ );
            var code = codes[ data ];
            var mode = modes[ data ]( ref pc, cpu );

            return string.Format( "{0}{1}", code, mode );
        }
    }
}