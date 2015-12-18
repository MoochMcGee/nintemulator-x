using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        //    CPU: Instructions encoded by opcode field.
        //    31---------26---------------------------------------------------0
        //    |  opcode   |                                                   |
        //    ------6----------------------------------------------------------
        //    |--000--|--001--|--010--|--011--|--100--|--101--|--110--|--111--| lo
        //000 | *1    | *2    | J     | JAL   | BEQ   | BNE   | BLEZ  | BGTZ  |
        //001 | ADDI  | ADDIU | SLTI  | SLTIU | ANDI  | ORI   | XORI  | LUI   |
        //010 | *3    | *4    | *5    |       | BEQL  | BNEL  | BLEZL | BGTZL |
        //011 | DADDI |DADDIU |  LDL  |  LDR  |       |       |       |       |
        //100 | LB    | LH    | LWL   | LW    | LBU   | LHU   | LWR   | LWU   |
        //101 | SB    | SH    | SWL   | SW    | SDL   | SDR   | SWR   | CACHE |
        //110 | LL    | LWC1  | LWC2  |       | LLD   | LDC1  | LDC2  | LD    |
        //111 | SC    | SWC1  | SWC2  |       | SCD   | SDC1  | SDC2  | SD    |
        // hi |-------|-------|-------|-------|-------|-------|-------|-------|
        //     *1 = SPECIAL, see SPECIAL list      *2 = REGIMM, see REGIMM list
        //     *3 = COP0                  *4 = COP1                   *5 = COP2

        //    SPECIAL: Instr. encoded by function field when opcode field = SPECIAL.
        //    31---------26------------------------------------------5--------0
        //    | = SPECIAL |                                         | function|
        //    ------6----------------------------------------------------6-----
        //    |--000--|--001--|--010--|--011--|--100--|--101--|--110--|--111--| lo
        //000 | SLL   |       | SRL   | SRA   | SLLV  |       | SRLV  | SRAV  |
        //001 | JR    | JALR  |       |       |SYSCALL| BREAK |       | SYNC  |
        //010 | MFHI  | MTHI  | MFLO  | MTLO  | DSLLV |       | DSRLV | DSRAV |
        //011 | MULT  | MULTU | DIV   | DIVU  | DMULT | DMULTU| DDIV  | DDIVU |
        //100 | ADD   | ADDU  | SUB   | SUBU  | AND   | OR    | XOR   | NOR   |
        //101 |       |       | SLT   | SLTU  | DADD  | DADDU | DSUB  | DSUBU |
        //110 | TGE   | TGEU  | TLT   | TLTU  | TEQ   |       | TNE   |       |
        //111 | DSLL  |       | DSRL  | DSRA  |DSLL32 |       |DSRL32 |DSRA32 |
        // hi |-------|-------|-------|-------|-------|-------|-------|-------|
        
        //   REGIMM: Instructions encoded by the rt field when opcode field = REGIMM.
        //   31---------26----------20-------16------------------------------0
        //   | = REGIMM  |          |   rt    |                              |
        //   ------6---------------------5------------------------------------
        //   |--000--|--001--|--010--|--011--|--100--|--101--|--110--|--111--| lo
        //00 | BLTZ  | BGEZ  | BLTZL | BGEZL |       |       |       |       |
        //01 | TGEI  | TGEIU | TLTI  | TLTIU | TEQI  |       | TNEI  |       |
        //10 | BLTZAL| BGEZAL|BLTZALL|BGEZALL|       |       |       |       |
        //11 |       |       |       |       |       |       |       |       |
        //hi |-------|-------|-------|-------|-------|-------|-------|-------|

        public Instruction Decode(uint code)
        {
            Instruction instr = default(Instruction);

            instr.type = (code >> 26) & 63U;
            
            if (instr.type == 0U)
            {
                // special instruction
                switch ((code >> 0) & 63U)
                {
                case 0x00: instr.rd = (code >> 11) & 31; instr.r1 = (code >> 16) & 31; instr.nn = (s32)(code >>  6) & 31; break; // | 000000 | 00000 |  rt   |  rd   |  sa   | 000000 (0)  | sll rd,rt,sa
                case 0x02: instr.rd = (code >> 11) & 31; instr.r1 = (code >> 16) & 31; instr.nn = (s32)(code >>  6) & 31; break; // | 000000 | 00000 |  rt   |  rd   |  sa   | 000010 (2)  | srl rd,rt,sa
                case 0x03: instr.rd = (code >> 11) & 31; instr.r1 = (code >> 16) & 31; instr.nn = (s32)(code >>  6) & 31; break; // | 000000 | 00000 |  rt   |  rd   |  sa   | 000011 (3)  | sra rd,rt,sa
                case 0x04: instr.rd = (code >> 11) & 31; instr.r1 = (code >> 16) & 31; instr.r2 =      (code >> 21) & 31; break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 000100 (4)  | sll rd,rt,rs
                case 0x06: instr.rd = (code >> 11) & 31; instr.r1 = (code >> 16) & 31; instr.r2 =      (code >> 21) & 31; break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 000110 (6)  | srl rd,rt,rs
                case 0x07: instr.rd = (code >> 11) & 31; instr.r1 = (code >> 16) & 31; instr.r2 =      (code >> 21) & 31; break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 000111 (7)  | sra rd,rt,rs
                case 0x08: break; // | 000000 |  rs   | 00000 | 00000 | 00000 | 001000 (8)  | jr rs
                case 0x09: break; // | 000000 |  rs   | 00000 |  rd   | 00000 | 001001 (9)  | jalr rs,rd
                case 0x0C: break; // | 000000 |             code              | 001100 (12) | syscall code
                case 0x0D: break; // | 000000 |             code              | 001101 (13) | break code
                case 0x0F: break; // | 000000 | 00000 | 00000 | 00000 | stype | 001111 (15) | sync stype
                case 0x10: break; // | 000000 | 00000 | 00000 |  rd   | 00000 | 010000 (16) | mfhi rd
                case 0x11: break; // | 000000 |  rs   | 00000 | 00000 | 00000 | 010001 (17) | mthi rs
                case 0x12: break; // | 000000 | 00000 | 00000 |  rd   | 00000 | 010010 (18) | mflo rd
                case 0x13: break; // | 000000 |  rs   | 00000 | 00000 | 00000 | 010011 (19) | mtlo rs
                case 0x14: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 010100 (20) | dsll rd,rt,rs
                case 0x16: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 010110 (22) | dsrl rd,rt,rs
                case 0x17: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 010111 (23) | dsra rd,rt,rs
                case 0x18: break; // | 000000 |  rs   |  rt   | 00000 | 00000 | 011000 (24) | mult rs,rt
                case 0x19: break; // | 000000 |  rs   |  rt   | 00000 | 00000 | 011001 (25) | multu rs,rt
                case 0x1A: break; // | 000000 |  rs   |  rt   | 00000 | 00000 | 011010 (26) | div rs,rt
                case 0x1B: break; // | 000000 |  rs   |  rt   | 00000 | 00000 | 011011 (27) | divu rs,rt
                case 0x1C: break; // | 000000 |  rs   |  rt   | 00000 | 00000 | 011100 (28) | dmult rs,rt
                case 0x1D: break; // | 000000 |  rs   |  rt   | 00000 | 00000 | 011101 (29) | dmultu rs,rt
                case 0x1E: break; // | 000000 |  rs   |  rt   | 00000 | 00000 | 011110 (30) | ddiv rs,rt
                case 0x1F: break; // | 000000 |  rs   |  rt   | 00000 | 00000 | 011111 (31) | ddivu rs,rt
                case 0x20: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 100000 (32) |
                case 0x21: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 100001 (33) |
                case 0x22: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 100010 (34) |
                case 0x23: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 100011 (35) |
                case 0x24: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 100100 (36) |
                case 0x25: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 100101 (37) |
                case 0x26: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 100110 (38) |
                case 0x27: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 100111 (39) |
                case 0x2A: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 101010 (42) |
                case 0x2B: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 101011 (43) |
                case 0x2C: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 101100 (44) |
                case 0x2D: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 101101 (45) |
                case 0x2E: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 101110 (46) |
                case 0x2F: break; // | 000000 |  rs   |  rt   |  rd   | 00000 | 101111 (47) |
                case 0x30: break; // | 000000 |  rs   |  rt   |     code      | 110000 (48) |
                case 0x31: break; // | 000000 |  rs   |  rt   |     code      | 110001 (49) |
                case 0x32: break; // | 000000 |  rs   |  rt   |     code      | 110010 (50) |
                case 0x33: break; // | 000000 |  rs   |  rt   |     code      | 110011 (51) |
                case 0x34: break; // | 000000 |  rs   |  rt   |     code      | 110100 (52) |
                case 0x36: break; // | 000000 |  rs   |  rt   |     code      | 110110 (54) |
                case 0x38: instr.rd = (code >> 11) & 31; instr.r1 = (code >> 16) & 31; instr.nn = (s32)(code >> 6) & 31; break; // | 000000 | 00000 |  rt   |  rd   |  sa   | 111000 (56) |
                case 0x3A: instr.rd = (code >> 11) & 31; instr.r1 = (code >> 16) & 31; instr.nn = (s32)(code >> 6) & 31; break; // | 000000 | 00000 |  rt   |  rd   |  sa   | 111010 (58) |
                case 0x3B: instr.rd = (code >> 11) & 31; instr.r1 = (code >> 16) & 31; instr.nn = (s32)(code >> 6) & 31; break; // | 000000 | 00000 |  rt   |  rd   |  sa   | 111011 (59) |
                case 0x3C: instr.rd = (code >> 11) & 31; instr.r1 = (code >> 16) & 31; instr.nn = (s32)(code >> 6) & 31; break; // | 000000 | 00000 |  rt   |  rd   |  sa   | 111100 (60) |
                case 0x3E: instr.rd = (code >> 11) & 31; instr.r1 = (code >> 16) & 31; instr.nn = (s32)(code >> 6) & 31; break; // | 000000 | 00000 |  rt   |  rd   |  sa   | 111110 (62) |
                case 0x3F: instr.rd = (code >> 11) & 31; instr.r1 = (code >> 16) & 31; instr.nn = (s32)(code >> 6) & 31; break; // | 000000 | 00000 |  rt   |  rd   |  sa   | 111111 (63) |
                default: throw new global::System.NotSupportedException("Unsupported Instruction: $" + code.ToString("x8"));
                }
                return instr;
            }

            if (instr.type == 1U)
            {
                // regimm instruction
                switch ((code >> 16) & 31U)
                {
                case 0x00: instr.rd = 0; instr.r1 = (code >> 21) & 31; instr.nn = (s32)(code >> 0) & 0xFFFF; break; // | 000001 |  rs   | 00000 |           offset            |
                case 0x01: instr.rd = 0; instr.r1 = (code >> 21) & 31; instr.nn = (s32)(code >> 0) & 0xFFFF; break; // | 000001 |  rs   | 00001 |           offset            |
                case 0x02: instr.rd = 0; instr.r1 = (code >> 21) & 31; instr.nn = (s32)(code >> 0) & 0xFFFF; break; // | 000001 |  rs   | 00010 |           offset            |
                case 0x03: instr.rd = 0; instr.r1 = (code >> 21) & 31; instr.nn = (s32)(code >> 0) & 0xFFFF; break; // | 000001 |  rs   | 00011 |           offset            |
                case 0x08: instr.rd = 0; instr.r1 = (code >> 21) & 31; instr.nn = (s32)(code >> 0) & 0xFFFF; break; // | 000001 |  rs   | 01000 |           immediate         |
                case 0x09: instr.rd = 0; instr.r1 = (code >> 21) & 31; instr.nn = (s32)(code >> 0) & 0xFFFF; break; // | 000001 |  rs   | 01001 |           immediate         |
                case 0x0A: instr.rd = 0; instr.r1 = (code >> 21) & 31; instr.nn = (s32)(code >> 0) & 0xFFFF; break; // | 000001 |  rs   | 01010 |           immediate         |
                case 0x0B: instr.rd = 0; instr.r1 = (code >> 21) & 31; instr.nn = (s32)(code >> 0) & 0xFFFF; break; // | 000001 |  rs   | 01011 |           immediate         |
                case 0x0C: instr.rd = 0; instr.r1 = (code >> 21) & 31; instr.nn = (s32)(code >> 0) & 0xFFFF; break; // | 000001 |  rs   | 01100 |           immediate         |
                case 0x0E: instr.rd = 0; instr.r1 = (code >> 21) & 31; instr.nn = (s32)(code >> 0) & 0xFFFF; break; // | 000001 |  rs   | 01110 |           immediate         |
                case 0x10: instr.rd = 0; instr.r1 = (code >> 21) & 31; instr.nn = (s32)(code >> 0) & 0xFFFF; break; // | 000001 |  rs   | 10000 |           offset            |
                case 0x11: instr.rd = 0; instr.r1 = (code >> 21) & 31; instr.nn = (s32)(code >> 0) & 0xFFFF; break; // | 000001 |  rs   | 10001 |           offset            |
                case 0x12: instr.rd = 0; instr.r1 = (code >> 21) & 31; instr.nn = (s32)(code >> 0) & 0xFFFF; break; // | 000001 |  rs   | 10010 |           offset            |
                case 0x13: instr.rd = 0; instr.r1 = (code >> 21) & 31; instr.nn = (s32)(code >> 0) & 0xFFFF; break; // | 000001 |  rs   | 10011 |           offset            |
                default: throw new global::System.NotSupportedException("Unsupported Instruction: $" + code.ToString("x8"));
                }
                return instr;
            }

            return instr;
        }
    }
}