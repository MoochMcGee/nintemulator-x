namespace Nintemulator.SFC.CPU.Debugging
{
    public static class Disassembler
    {
        //   ┌───────┬───────────┬──────────┬─────────────┬─────────┬─────────┬─────────┬───────────┬───────┬─────────┬───────┬───────┬───────────┬─────────┬─────────┬──────────┐
        //   │   0   │     1     │    2     │      3      │    4    │    5    │    6    │     7     │   8   │    9    │   A   │   B   │     C     │    D    │    E    │     F    │
        //   ├───────┼───────────┼──────────┼─────────────┼─────────┼─────────┼─────────┼───────────┼───────┼─────────┼───────┼───────┼───────────┼─────────┼─────────┼──────────┤
        // 0 │ BRK s │ ORA (d,x) │ COP s    │ ORA d,s     │ TSB d   │ ORA d   │ ASL d   │ ORA [d]   │ PHP s │ ORA #   │ ASL A │ PHD s │ TSB a     │ ORA a   │ ASL a   │ ORA al   │
        // 1 │ BPL r │ ORA (d),y │ ORA (d)  │ ORA (d,s),y │ TRB d   │ ORA d,x │ ASL d,x │ ORA [d],y │ CLC i │ ORA a,y │ INC A │ TCS i │ TRB a     │ ORA a,x │ ASL a,x │ ORA al,x │
        // 2 │ JSR a │ AND (d,x) │ JSL al   │ AND d,s     │ BIT d   │ AND d   │ ROL d   │ AND [d]   │ PLP s │ AND #   │ ROL A │ PLD s │ BIT a     │ AND a   │ ROL a   │ AND al   │
        // 3 │ BMI r │ AND (d),y │ AND (d)  │ AND (d,s),y │ BIT d,x │ AND d,x │ ROL d,x │ AND [d],y │ SEC i │ AND a,y │ DEC A │ TSC i │ BIT a,x   │ AND a,x │ ROL a,x │ AND al,x │
        // 4 │ RTI s │ EOR (d,x) │ reserved │ EOR d,s     │ MVP xya │ EOR d   │ LSR d   │ EOR [d]   │ PHA s │ EOR #   │ LSR A │ PHK s │ JMP a     │ EOR a   │ LSR a   │ EOR al   │
        // 5 │ BVC r │ EOR (d),y │ EOR (d)  │ EOR (d,s),y │ MVN xya │ EOR d,x │ LSR d,x │ EOR [d],y │ CLI i │ EOR a,y │ PHY s │ TCD i │ JMP al    │ EOR a,x │ LSR a,x │ EOR al,x │
        // 6 │ RTS s │ ADC (d,x) │ PER s    │ ADC d,s     │ STZ d   │ ADC d   │ ROR d   │ ADC [d]   │ PLA s │ ADC #   │ ROR A │ RTL s │ JMP (a)   │ ADC a   │ ROR a   │ ADC al   │
        // 7 │ BVS r │ ADC (d),y │ ADC (d)  │ ADC (d,s),y │ STZ d,x │ ADC d,x │ ROR d,x │ ADC [d],y │ SEI i │ ADC a,y │ PLY s │ TDC i │ JMP (a,x) │ ADC a,x │ ROR a,x │ ADC al,x │
        // 8 │ BRA r │ STA (d,x) │ BRL rl   │ STA d,s     │ STY d   │ STA d   │ STX d   │ STA [d]   │ DEY i │ BIT #   │ TXA i │ PHB s │ STY a     │ STA a   │ STX a   │ STA al   │
        // 9 │ BCC r │ STA (d),y │ STA (d)  │ STA (d,s),y │ STY d,x │ STA d,x │ STX d,y │ STA [d],y │ TYA i │ STA a,y │ TXS i │ TXY i │ STZ a     │ STA a,x │ STZ a,x │ STA al,x │
        // A │ LDY # │ LDA (d,x) │ LDX #    │ LDA d,s     │ LDY d   │ LDA d   │ LDX d   │ LDA [d]   │ TAY i │ LDA #   │ TAX i │ PLB s │ LDY a     │ LDA a   │ LDX a   │ LDA al   │
        // B │ BCS r │ LDA (d),y │ LDA (d)  │ LDA (d,s),y │ LDY d,x │ LDA d,x │ LDX d,y │ LDA [d],y │ CLV i │ LDA a,y │ TSX i │ TYX i │ LDY a,x   │ LDA a,x │ LDX a,y │ LDA al,x │
        // C │ CPY # │ CMP (d,x) │ REP #    │ CMP d,s     │ CPY d   │ CMP d   │ DEC d   │ CMP [d]   │ INY i │ CMP #   │ DEX i │ WAI i │ CPY a     │ CMP a   │ DEC a   │ CMP al   │
        // D │ BNE r │ CMP (d),y │ CMP (d)  │ CMP (d,s),y │ PEI s   │ CMP d,x │ DEC d,x │ CMP [d],y │ CLD i │ CMP a,y │ PHX s │ STP i │ JML (a)   │ CMP a,x │ DEC a,x │ CMP al,x │
        // E │ CPX # │ SBC (d,x) │ SEP #    │ SBC d,s     │ CPX d   │ SBC d   │ INC d   │ SBC [d]   │ INX i │ SBC #   │ NOP i │ XBA i │ CPX a     │ SBC a   │ INC a   │ SBC al   │
        // F │ BEQ r │ SBC (d),y │ SBC (d)  │ SBC (d,s),y │ PEA s   │ SBC d,x │ INC d,x │ SBC [d],y │ SED i │ SBC a,y │ PLX s │ XCE i │ JSR (a,x) │ SBC a,x │ INC a,x │ SBC al,x │
        //   └───────┴───────────┴──────────┴─────────────┴─────────┴─────────┴─────────┴───────────┴───────┴─────────┴───────┴───────┴───────────┴─────────┴─────────┴──────────┘

        private static int[] sizes =
        {
             2, 2,  2, 2, 2, 2, 2, 2, 1, -1, 1, 1, 3, 3, 3, 4,
             2, 2,  2, 2, 2, 2, 2, 2, 1,  3, 1, 1, 3, 3, 3, 4,
             3, 2,  4, 2, 2, 2, 2, 2, 1, -1, 1, 1, 3, 3, 3, 4,
             2, 2,  2, 2, 2, 2, 2, 2, 1,  3, 1, 1, 3, 3, 3, 4,
             1, 2,  1, 2, 3, 2, 2, 2, 1, -1, 1, 1, 3, 3, 3, 4,
             2, 2,  2, 2, 3, 2, 2, 2, 1,  3, 1, 1, 4, 3, 3, 4,
             1, 2,  3, 2, 2, 2, 2, 2, 1, -1, 1, 1, 3, 3, 3, 4,
             2, 2,  2, 2, 2, 2, 2, 2, 1,  3, 1, 1, 3, 3, 3, 4,
             2, 2,  3, 2, 2, 2, 2, 2, 1, -1, 1, 1, 3, 3, 3, 4,
             2, 2,  2, 2, 2, 2, 2, 2, 1,  3, 1, 1, 3, 3, 3, 4,
            -2, 2, -2, 2, 2, 2, 2, 2, 1, -1, 1, 1, 3, 3, 3, 4,
             2, 2,  2, 2, 2, 2, 2, 2, 1,  3, 1, 1, 3, 3, 3, 4,
            -2, 2,  2, 2, 2, 2, 2, 2, 1, -1, 1, 1, 3, 3, 3, 4,
             2, 2,  2, 2, 2, 2, 2, 2, 1,  3, 1, 1, 3, 3, 3, 4,
            -2, 2,  2, 2, 2, 2, 2, 2, 1, -1, 1, 1, 3, 3, 3, 4,
             2, 2,  2, 2, 3, 2, 2, 2, 1,  3, 1, 1, 3, 3, 3, 4,
        };

        private static string[] modes =
        {
            AmImm_b, AmInx, AmImm_b, AmSpr, AmDpg, AmDpg, AmDpg, AmInd_l, AmImp, "   ", AmAcc, AmImp, AmAbs,   AmAbs, AmAbs, AmAbs_l,
            AmRel,   AmIny, AmInd,   AmSpy, AmDpg, AmDpx, AmDpx, AmIny_l, AmImp, AmAby, AmAcc, AmImp, AmAbs,   AmAbx, AmAbx, AmAbx_l,
            AmAbs,   AmInx, AmAbs_l, AmSpr, AmDpg, AmDpg, AmDpg, AmInd_l, AmImp, "   ", AmAcc, AmImp, AmAbs,   AmAbs, AmAbs, AmAbs_l,
            AmRel,   AmIny, AmInd,   AmSpy, AmDpx, AmDpx, AmDpx, AmIny_l, AmImp, AmAby, AmAcc, AmImp, AmAbx,   AmAbx, AmAbx, AmAbx_l,
            AmImp,   AmInx, AmImp,   AmSpr, "xya", AmDpg, AmDpg, AmInd_l, AmImp, "   ", AmAcc, AmImp, AmAbs,   AmAbs, AmAbs, AmAbs_l,
            AmRel,   AmIny, AmInd,   AmSpy, "xya", AmDpx, AmDpx, AmIny_l, AmImp, AmAby, AmImp, AmImp, AmAbs_l, AmAbx, AmAbx, AmAbx_l,
            AmImp,   AmInx, AmImp,   AmSpr, AmDpg, AmDpg, AmDpg, AmInd_l, AmImp, "   ", AmAcc, AmImp, AmAbs_i, AmAbs, AmAbs, AmAbs_l,
            AmRel,   AmIny, AmInd,   AmSpy, AmDpx, AmDpx, AmDpx, AmIny_l, AmImp, AmAby, AmImp, AmImp, AmAbx_i, AmAbx, AmAbx, AmAbx_l,
            AmRel,   AmInx, AmRel_l, AmSpr, AmDpg, AmDpg, AmDpg, AmInd_l, AmImp, "   ", AmImp, AmImp, AmAbs,   AmAbs, AmAbs, AmAbs_l,
            AmRel,   AmIny, AmInd,   AmSpy, AmDpx, AmDpx, AmDpy, AmIny_l, AmImp, AmAby, AmImp, AmImp, AmAbs,   AmAbx, AmAbx, AmAbx_l,
            "   ",   AmInx, "   ",   AmSpr, AmDpg, AmDpg, AmDpg, AmInd_l, AmImp, "   ", AmImp, AmImp, AmAbs,   AmAbs, AmAbs, AmAbs_l,
            AmRel,   AmIny, AmInd,   AmSpy, AmDpx, AmDpx, AmDpy, AmIny_l, AmImp, AmAby, AmImp, AmImp, AmAbx,   AmAbx, AmAby, AmAbx_l,
            "   ",   AmInx, AmImm_b, AmSpr, AmDpg, AmDpg, AmDpg, AmInd_l, AmImp, "   ", AmImp, AmImp, AmAbs,   AmAbs, AmAbs, AmAbs_l,
            AmRel,   AmIny, AmInd,   AmSpy, AmImp, AmDpx, AmDpx, AmIny_l, AmImp, AmAby, AmImp, AmImp, AmAbs_i, AmAbx, AmAbx, AmAbx_l,
            "   ",   AmInx, AmImm_b, AmSpr, AmDpg, AmDpg, AmDpg, AmInd_l, AmImp, "   ", AmImp, AmImp, AmAbs,   AmAbs, AmAbs, AmAbs_l,
            AmRel,   AmIny, AmInd,   AmSpy, AmImp, AmDpx, AmDpx, AmIny_l, AmImp, AmAby, AmImp, AmImp, AmAbx_i, AmAbx, AmAbx, AmAbx_l
        };
        private static string[] codes =
        {
            "brk", "ora", "cop", "ora", "tsb", "ora", "asl", "ora", "php", "ora", "asl", "phd", "tsb", "ora", "asl", "ora",
            "bpl", "ora", "ora", "ora", "trb", "ora", "asl", "ora", "clc", "ora", "inc", "tcs", "trb", "ora", "asl", "ora",
            "jsr", "and", "jsl", "and", "bit", "and", "rol", "and", "plp", "and", "rol", "pld", "bit", "and", "rol", "and",
            "bmi", "and", "and", "and", "bit", "and", "rol", "and", "sec", "and", "dec", "tsc", "bit", "and", "rol", "and",
            "rti", "eor", "wdm", "eor", "mvp", "eor", "lsr", "eor", "pha", "eor", "lsr", "phk", "jmp", "eor", "lsr", "eor",
            "bvc", "eor", "eor", "eor", "mvn", "eor", "lsr", "eor", "cli", "eor", "phy", "tcd", "jml", "eor", "lsr", "eor",
            "rts", "adc", "per", "adc", "stz", "adc", "ror", "adc", "pla", "adc", "ror", "rtl", "jmp", "adc", "ror", "adc",
            "bvs", "adc", "adc", "adc", "stz", "adc", "ror", "adc", "sei", "adc", "ply", "tdc", "jmp", "adc", "ror", "adc",
            "bra", "sta", "brl", "sta", "sty", "sta", "stx", "sta", "dey", "bit", "txa", "phb", "sty", "sta", "stx", "sta",
            "bcc", "sta", "sta", "sta", "sty", "sta", "stx", "sta", "tya", "sta", "txs", "txy", "stz", "sta", "stz", "sta",
            "ldy", "lda", "ldx", "lda", "ldy", "lda", "ldx", "lda", "tay", "lda", "tax", "plb", "ldy", "lda", "ldx", "lda",
            "bcs", "lda", "lda", "lda", "ldy", "lda", "ldx", "lda", "clv", "lda", "tsx", "tyx", "ldy", "lda", "ldx", "lda",
            "cpy", "cmp", "rep", "cmp", "cpy", "cmp", "dec", "cmp", "iny", "cmp", "dex", "wai", "cpy", "cmp", "dec", "cmp",
            "bne", "cmp", "cmp", "cmp", "pei", "cmp", "dec", "cmp", "cld", "cmp", "phx", "stp", "jml", "cmp", "dec", "cmp",
            "cpx", "sbc", "sep", "sbc", "cpx", "sbc", "inc", "sbc", "inx", "sbc", "nop", "xba", "cpx", "sbc", "inc", "sbc",
            "beq", "sbc", "sbc", "sbc", "pea", "sbc", "inc", "sbc", "sed", "sbc", "plx", "xce", "jsr", "sbc", "inc", "sbc"
        };

        #region Modes

        private const string AmAbs   = "${1:x2}{0:x2}";
        private const string AmAbx   = "${1:x2}{0:x2},x";
        private const string AmAby   = "${1:x2}{0:x2},y";

        private const string AmAbs_i = "(${1:x2}{0:x2})";
        private const string AmAbx_i = "(${1:x2}{0:x2},x)";

        private const string AmAbs_l = "${2:x2}{1:x2}{0:x2}";
        private const string AmAbx_l = "${2:x2}{1:x2}{0:x2},x";
        private const string AmAby_l = "${2:x2}{1:x2}{0:x2},y";

        private const string AmDpg   = "${0:x2}";
        private const string AmDpx   = "${0:x2},x";
        private const string AmDpy   = "${0:x2},y";

        private const string AmRel   = "${0:x2}";
        private const string AmRel_l = "${1:x2}{0:x2}";

        private const string AmInd   = "(${0:x2})";
        private const string AmInx   = "(${0:x2},x)";
        private const string AmIny   = "(${0:x2}),y";

        private const string AmInd_l = "[${0:x2}]";
        private const string AmIny_l = "[${0:x2}],y";

        private const string AmSpr   = "${0:x2},s";
        private const string AmSpy   = "(${0:x2},s),y";

        private const string AmAcc   = "a";
        private const string AmImp   = "";
        private const string AmImm_b = "#${0:x2}";
        private const string AmImm_h = "#${1:x2}{0:x2}";

        #endregion

        public static string Trace(Cpu cpu, byte[] buffer)
        {
            var code = buffer[0];
            var op_l = buffer[1];
            var op_h = buffer[2];
            var op_b = buffer[3];

            var code_string = codes[code];
            var mode_string = modes[code];

            switch (sizes[code])
            {
            case -1: mode_string = cpu.flags.m.b ? AmImm_b : AmImm_h; break;
            case -2: mode_string = cpu.flags.x.b ? AmImm_b : AmImm_h; break;
            }

            var p = "";

            if (cpu.flags.n.b) p += "N"; else p += "n";
            if (cpu.flags.v.b) p += "V"; else p += "v";

            if (cpu.flags.e.b)
            {
                p += "1B";
            }
            else
            {
                if (cpu.flags.m.b) p += "M"; else p += "m";
                if (cpu.flags.x.b) p += "X"; else p += "x";
            }

            if (cpu.flags.d.b) p += "D"; else p += "d";
            if (cpu.flags.i.b) p += "I"; else p += "i";
            if (cpu.flags.z.b) p += "Z"; else p += "z";
            if (cpu.flags.c.b) p += "C"; else p += "c";

            return string.Format("{0:x2}{1:x4} {2} {3, -18} A:{4:x4} X:{5:x4} Y:{6:x4} S:{7:x4} D:{8:x4} DB:{9:x2} {10}",
                cpu.pc.b,
                cpu.pc.w,
                code_string,
                string.Format(mode_string, buffer[1], buffer[2], buffer[3]),
                cpu.regs.a,
                cpu.regs.x,
                cpu.regs.y,
                cpu.regs.sp,
                cpu.regs.d,
                cpu.regs.db,
                p);
        }
        public static string Disassemble(ref uint pc, ref bool m, ref bool x, byte[] buffer)
        {
            var code = buffer[0U];
            var op_l = buffer[1U];
            var op_h = buffer[2U];
            var op_b = buffer[3U];

            var code_string = codes[code];
            var mode_string = modes[code];
            var size = sizes[code];

            switch (code_string)
            {
            case "rep":
                if ((op_l & 0x20) != 0) m = false;
                if ((op_l & 0x10) != 0) x = false;
                break;

            case "sep":
                if ((op_l & 0x20) != 0) m = true;
                if ((op_l & 0x10) != 0) x = true;
                break;
            }

            switch (size)
            {
            default: pc += (uint)size; break;
            case -1:
                if (m)
                {
                    pc += 2;
                    mode_string = AmImm_b;
                }
                else
                {
                    pc += 3;
                    mode_string = AmImm_h;
                }
                break;

            case -2:
                if (x)
                {
                    pc += 2;
                    mode_string = AmImm_b;
                }
                else
                {
                    pc += 3;
                    mode_string = AmImm_h;
                }
                break;
            }

            return string.Format("{0} {1}", code_string, string.Format(mode_string, op_l, op_h, op_b));
        }
    }
}