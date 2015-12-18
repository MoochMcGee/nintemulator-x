namespace Nintemulator.SFC.SPU.Debugging
{
    public static class Disassembler
    {
        private delegate string Mode(ref uint pc);

        static string[] codes = new string[256]
        {
            "nop", "___", "___", "___", "or ", "or ", "or ", "or ", "or ", "or ", "___", "asl", "asl", "php", "___", "brk",  // $00
            "bpl", "___", "___", "___", "or ", "or ", "or ", "or ", "or ", "or ", "dew", "asl", "asl", "dex", "cpx", "jmp",  // $10
            "clp", "___", "___", "___", "and", "and", "and", "and", "and", "and", "___", "rol", "rol", "pha", "___", "bra",  // $20
            "bmi", "___", "___", "___", "and", "and", "and", "and", "and", "and", "inw", "rol", "rol", "inx", "cpx", "___",  // $30
            "sep", "___", "___", "___", "eor", "eor", "eor", "eor", "eor", "eor", "___", "lsr", "lsr", "phx", "___", "___",  // $40
            "bvc", "___", "___", "___", "eor", "eor", "eor", "eor", "eor", "eor", "cpw", "lsr", "lsr", "tax", "cpy", "jmp",  // $50
            "clc", "___", "___", "___", "cmp", "cmp", "cmp", "cmp", "cmp", "cmp", "___", "ror", "ror", "phy", "___", "ret",  // $60
            "bvs", "___", "___", "___", "cmp", "cmp", "cmp", "cmp", "cmp", "cmp", "add", "ror", "ror", "txa", "cpy", "rti",  // $70
            "sec", "___", "___", "___", "adc", "adc", "adc", "adc", "adc", "adc", "___", "dec", "dec", "ldy", "plp", "mov",  // $80
            "bcc", "___", "___", "___", "adc", "adc", "adc", "adc", "adc", "adc", "sub", "dec", "dec", "tsx", "div", "xcn",  // $90
            "cli", "___", "___", "___", "sbc", "sbc", "sbc", "sbc", "sbc", "sbc", "___", "inc", "inc", "cpy", "pla", "sta",  // $A0
            "bcs", "___", "___", "___", "sbc", "sbc", "sbc", "sbc", "sbc", "sbc", "ldw", "inc", "inc", "txs", "das", "lda",  // $B0
            "sei", "___", "___", "___", "sta", "sta", "sta", "sta", "cpx", "stx", "___", "sty", "sty", "ldx", "plx", "mul",  // $C0
            "bne", "___", "___", "___", "sta", "sta", "sta", "sta", "stx", "stx", "stw", "sty", "dey", "tya", "___", "daa",  // $D0
            "clv", "___", "___", "___", "lda", "lda", "lda", "lda", "lda", "ldx", "___", "ldy", "ldy", "ccf", "ply", "___",  // $E0
            "beq", "___", "___", "___", "lda", "lda", "lda", "lda", "ldx", "ldx", "mov", "ldy", "iny", "tay", "___", "___"   // $F0
        };
        static Mode[] modes = new Mode[256]
        {
            AmImp, Am___, Am___, Am___, AmDpg, AmAbs, AmXdp, AmInx, AmImm, AmDdp, Am___, AmDpg, AmAbs, AmImp, Am___, AmImp, // $00
            AmImm, Am___, Am___, Am___, AmDpx, AmAbx, AmAby, AmIny, AmDim, AmYtx, AmDpg, AmDpx, AmAcc, AmImp, AmAbs, AmInd, // $10
            AmImp, Am___, Am___, Am___, AmDpg, AmAbs, AmXdp, AmInx, AmImm, AmDdp, Am___, AmDpg, AmAbs, AmImp, Am___, AmImm, // $20
            AmImm, Am___, Am___, Am___, AmDpx, AmAbx, AmAby, AmIny, AmDim, AmYtx, AmDpg, AmDpx, AmAcc, AmImp, AmDpg, Am___, // $30
            AmImp, Am___, Am___, Am___, AmDpg, AmAbs, AmXdp, AmInx, AmImm, AmDdp, Am___, AmDpg, AmAbs, AmImp, Am___, Am___, // $40
            AmImm, Am___, Am___, Am___, AmDpx, AmAbx, AmAby, AmIny, AmDim, AmYtx, AmDpg, AmDpx, AmAcc, AmImp, AmAbs, AmAbs, // $50
            AmImp, Am___, Am___, Am___, AmDpg, AmAbs, AmXdp, AmInx, AmImm, AmDdp, Am___, AmDpg, AmAbs, AmImp, Am___, AmImp, // $60
            AmImm, Am___, Am___, Am___, AmDpx, AmAbx, AmAby, AmIny, AmDim, AmYtx, AmDpg, AmDpx, AmAcc, AmImp, AmDpg, AmImp, // $70
            AmImp, Am___, Am___, Am___, AmDpg, AmAbs, AmXdp, AmInx, AmImm, AmDdp, Am___, AmDpg, AmAbs, AmImm, AmImp, AmDim, // $80
            AmImm, Am___, Am___, Am___, AmDpx, AmAbx, AmAby, AmIny, AmDim, AmYtx, AmDpg, AmDpx, AmAcc, AmImp, AmImp, AmImp, // $90
            AmImp, Am___, Am___, Am___, AmDpg, AmAbs, AmXdp, AmInx, AmImm, AmDdp, Am___, AmDpg, AmAbs, AmImm, AmImp, AmXdi, // $A0
            AmImm, Am___, Am___, Am___, AmDpx, AmAbx, AmAby, AmIny, AmDim, AmYtx, AmDpg, AmDpx, AmAcc, AmImp, AmImp, AmXdi, // $B0
            AmImp, Am___, Am___, Am___, AmDpg, AmAbs, AmXdp, AmInx, AmImm, AmAbs, Am___, AmDpg, AmAbs, AmImm, AmImp, AmImp, // $C0
            AmImm, Am___, Am___, Am___, AmDpx, AmAbx, AmAby, AmIny, AmDpg, AmDpy, AmDpg, AmDpx, AmImp, AmImp, Am___, AmImp, // $D0
            AmImp, Am___, Am___, Am___, AmDpg, AmAbs, AmXdp, AmInx, AmImm, AmAbs, Am___, AmDpg, AmAbs, AmImp, AmImp, Am___, // $E0
            AmImm, Am___, Am___, Am___, AmDpx, AmAbx, AmAby, AmIny, AmDpg, AmDpy, AmDdp, AmDpx, AmImp, AmImp, Am___, Am___  // $F0
        };

        private static string Am___(ref uint pc) { pc += 1; return ""; }
        private static string AmAbs(ref uint pc) { pc += 3; return "${1:x2}{0:x2}"; }
        private static string AmAbx(ref uint pc) { pc += 3; return "${1:x2}{0:x2},x"; }
        private static string AmAby(ref uint pc) { pc += 3; return "${1:x2}{0:x2},y"; }
        private static string AmAcc(ref uint pc) { pc += 1; return "a"; }
        private static string AmDdp(ref uint pc) { pc += 3; return "${1:x2}, ${0:x2}"; }
        private static string AmDim(ref uint pc) { pc += 3; return "${1:x2}, #${0:x2}"; }
        private static string AmDpg(ref uint pc) { pc += 2; return "${0:x2}"; }
        private static string AmDpx(ref uint pc) { pc += 2; return "${0:x2},x"; }
        private static string AmDpy(ref uint pc) { pc += 2; return "${0:x2},y"; }
        private static string AmImm(ref uint pc) { pc += 2; return "#${0:x2}"; }
        private static string AmImp(ref uint pc) { pc += 1; return ""; }
        private static string AmInd(ref uint pc) { pc += 3; return "(${1:x2}{0:x2},x)"; }
        private static string AmInx(ref uint pc) { pc += 2; return "(${0:x2},x)"; }
        private static string AmIny(ref uint pc) { pc += 2; return "(${0:x2}),y"; }
        private static string AmXdi(ref uint pc) { pc += 1; return "(x)+"; }
        private static string AmXdp(ref uint pc) { pc += 1; return "(x)"; }
        private static string AmYdp(ref uint pc) { pc += 1; return "(y)"; }
        private static string AmYtx(ref uint pc) { pc += 1; return "(x), (y)"; }

        //------------------------------
        //Mnemonic            Code Bytes
        //------------------------------
        //  AND1  C, /m.b      6A    3  
        //  AND1  C, m.b       4A    3  

        //  BBC   d.0, r       13    3  
        //  BBC   d.1, r       33    3  
        //  BBC   d.2, r       53    3  
        //  BBC   d.3, r       73    3  
        //  BBC   d.4, r       93    3  
        //  BBC   d.5, r       B3    3  
        //  BBC   d.6, r       D3    3  
        //  BBC   d.7, r       F3    3  

        //  BBS   d.0, r       03    3  
        //  BBS   d.1, r       23    3  
        //  BBS   d.2, r       43    3  
        //  BBS   d.3, r       63    3  
        //  BBS   d.4, r       83    3  
        //  BBS   d.5, r       A3    3  
        //  BBS   d.6, r       C3    3  
        //  BBS   d.7, r       E3    3  

        //  CALL  !a           3F    3  

        //  CBNE  d+X, r       DE    3  
        //  CBNE  d, r         2E    3  

        //  CLR1  d.0          12    2  
        //  CLR1  d.1          32    2  
        //  CLR1  d.2          52    2  
        //  CLR1  d.3          72    2  
        //  CLR1  d.4          92    2  
        //  CLR1  d.5          B2    2  
        //  CLR1  d.6          D2    2  
        //  CLR1  d.7          F2    2  

        //  DBNZ  Y, r         FE    2  
        //  DBNZ  d, r         6E    3  

        //  EOR1  C, m.b       8A    3  

        //  MOV1  C, m.b       AA    3  
        //  MOV1  m.b, C       CA    3  

        //  NOT1  m.b          EA    3  

        //  OR1   C, /m.b      2A    3  
        //  OR1   C, m.b       0A    3  

        //  PCALL u            4F    2  

        //  SET1  d.0          02    2  
        //  SET1  d.1          22    2  
        //  SET1  d.2          42    2  
        //  SET1  d.3          62    2  
        //  SET1  d.4          82    2  
        //  SET1  d.5          A2    2  
        //  SET1  d.6          C2    2  
        //  SET1  d.7          E2    2  

        //  SLEEP              EF    1  
        //  STOP               FF    1  

        //  TCALL 0            01    1  
        //  TCALL 1            11    1  
        //  TCALL 2            21    1  
        //  TCALL 3            31    1  
        //  TCALL 4            41    1  
        //  TCALL 5            51    1  
        //  TCALL 6            61    1  
        //  TCALL 7            71    1  
        //  TCALL 8            81    1  
        //  TCALL 9            91    1  
        //  TCALL 10           A1    1  
        //  TCALL 11           B1    1  
        //  TCALL 12           C1    1  
        //  TCALL 13           D1    1  
        //  TCALL 14           E1    1  
        //  TCALL 15           F1    1  

        //  TCLR1 !a           4E    3  
        //  TSET1 !a           0E    3  

        public static string Disassemble(ref uint pc, byte[] buffer)
        {
            var code = buffer[0U];
            var op_l = buffer[1U];
            var op_h = buffer[2U];

            var code_string = codes[code];
            var mode_string = modes[code](ref pc);
            
            return string.Format("{0} {1}", code_string, string.Format(mode_string, op_l, op_h));
        }
    }
}