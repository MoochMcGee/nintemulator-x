using Nintemulator.Shared;

namespace Nintemulator.N64.CPU.Debugging
{
    public class Disassembler
    {
        // SPECIAL
        private static string Disassemble_00(ulong pc, uint code)
        {
            uint rs = (code >> 21) & 31U;
            uint rt = (code >> 16) & 31U;
            uint rd = (code >> 11) & 31U;
            uint nn = (code >>  6) & 31U;

            switch (code >> 0 & 63U)
            {
            case  0U: return string.Format("sll r{0},r{1},0x{2:x}", rd, rt, nn); // SLL
            case  1U: break; //
            case  2U: return string.Format("srl r{0},r{1},0x{2:x}", rd, rt, nn); // SRL
            case  3U: return string.Format("sra r{0},r{1},0x{2:x}", rd, rt, nn); // SRA
            case  4U: return string.Format("sll r{0},r{1},r{2}", rd, rt, rs); // SLLV
            case  5U: break; //
            case  6U: return string.Format("sll r{0},r{1},r{2}", rd, rt, rs); // SRLV
            case  7U: return string.Format("sll r{0},r{1},r{2}", rd, rt, rs); // SRAV
            case  8U: break; // JR
            case  9U: break; // JALR
            case 10U: break; //
            case 11U: break; //
            case 12U: break; // SYSCALL
            case 13U: break; // BREAK
            case 14U: break; //
            case 15U: break; // SYNC
            case 16U: break; // MFHI
            case 17U: break; // MTHI
            case 18U: break; // MFLO
            case 19U: break; // MTLO
            case 20U: break; // DSLLV
            case 21U: break; //
            case 22U: break; // DSRLV
            case 23U: break; // DSRAV
            case 24U: break; // MULT
            case 25U: break; // MULTU
            case 26U: break; // DIV
            case 27U: break; // DIVU
            case 28U: break; // DMULT
            case 29U: break; // DMULTU
            case 30U: break; // DDIV
            case 31U: break; // DDIVU
            case 32U: return string.Format("add r{0},r{1},r{2}", rd, rs, rt); // ADD
            case 33U: return string.Format("addu r{0},r{1},r{2}", rd, rs, rt); // ADDU
            case 34U: return string.Format("sub r{0},r{1},r{2}", rd, rs, rt); // SUB
            case 35U: return string.Format("subu r{0},r{1},r{2}", rd, rs, rt); // SUBU
            case 36U: return string.Format("and r{0},r{1},r{2}", rd, rs, rt); // AND
            case 37U: return string.Format("or r{0},r{1},r{2}", rd, rs, rt); // OR
            case 38U: return string.Format("xor r{0},r{1},r{2}", rd, rs, rt); // XOR
            case 39U: return string.Format("nor r{0},r{1},r{2}", rd, rs, rt); // NOR
            case 40U: break; //
            case 41U: break; //
            case 42U: return string.Format("slt r{0},r{1},r{2}", rd, rs, rt); // SLT
            case 43U: return string.Format("sltu r{0},r{1},r{2}", rd, rs, rt); // SLTU
            case 44U: return string.Format("dadd r{0},r{1},0x{2:x}", rd, rs, rt);  // DADD
            case 45U: return string.Format("daddu r{0},r{1},0x{2:x}", rd, rs, rt);  // DADDU
            case 46U: return string.Format("dsub r{0},r{1},0x{2:x}", rd, rs, rt);  // DSUB
            case 47U: return string.Format("dsubu r{0},r{1},0x{2:x}", rd, rs, rt);  // DSUBU
            case 48U: return string.Format("tge r{0},r{1}", rs, rt); // TGE
            case 49U: return string.Format("tgeu r{0},r{1}", rs, rt); // TGEU
            case 50U: return string.Format("tlt r{0},r{1}", rs, rt); // TLT
            case 51U: return string.Format("tltu r{0},r{1}", rs, rt); // TLTU
            case 52U: return string.Format("teq r{0},r{1}", rs, rt); // TEQ
            case 53U: break; //
            case 54U: return string.Format("tne r{0},r{1}", rs, rt); // TNE
            case 55U: break; //
            case 56U: return string.Format("dsll r{0},r{1},0x{2:x}", rd, rt, nn); // DSLL
            case 57U: break; //
            case 58U: return string.Format("dsrl r{0},r{1},0x{2:x}", rd, rt, nn); // DSRL
            case 59U: return string.Format("dsra r{0},r{1},0x{2:x}", rd, rt, nn); // DSRA
            case 60U: return string.Format("dsll r{0},r{1},0x{2:x}", rd, rt, nn+32); // DSLL32
            case 61U: break; //
            case 62U: return string.Format("dsrl r{0},r{1},0x{2:x}", rd, rt, nn+32);// DSRL32
            case 63U: return string.Format("dsra r{0},r{1},0x{2:x}", rd, rt, nn+32);// DSRA32
            }

            return "undefined";
        }
        // REGIMM
        private static string Disassemble_01(ulong pc, uint code)
        {
            uint rs = (code >> 21) & 31U;
            uint rt = (code >> 16) & 31U;
            uint nn = (code >>  0) & 65535U;

            switch (code >> 16 & 31U)
            {
            case  0U: return string.Format("bltz r{0},0x{1:x}", rs, pc + (ulong)((short)nn << 2)); // BLTZ
            case  1U: return string.Format("bgez r{0},0x{1:x}", rs, pc + (ulong)((short)nn << 2)); // BGEZ
            case  2U: return string.Format("bltzl r{0},0x{1:x}", rs, pc + (ulong)((short)nn << 2)); // BLTZL
            case  3U: return string.Format("bgezl r{0},0x{1:x}", rs, pc + (ulong)((short)nn << 2)); // BGEZL
            case  4U: break;
            case  5U: break;
            case  6U: break;
            case  7U: break;
            case  8U: return string.Format("tge r{0},0x{1:x}", rs, nn); // TGEI
            case  9U: return string.Format("tgeu r{0},0x{1:x}", rs, nn); // TGEIU
            case 10U: return string.Format("tlt r{0},0x{1:x}", rs, nn); // TLTI
            case 11U: return string.Format("tltu r{0},0x{1:x}", rs, nn); // TLTIU
            case 12U: return string.Format("teq r{0},0x{1:x}", rs, nn); // TEQI
            case 13U: break;
            case 14U: return string.Format("tne r{0},0x{1:x}", rs, nn); // TNEI
            case 15U: break;
            case 16U: return string.Format("bltzal r{0},0x{1:x}", rs, pc + (ulong)((short)nn << 2)); // BLTZAL
            case 17U: return string.Format("bgezal r{0},0x{1:x}", rs, pc + (ulong)((short)nn << 2)); // BGEZAL
            case 18U: return string.Format("bltzall r{0},0x{1:x}", rs, pc + (ulong)((short)nn << 2)); // BLTZALL
            case 19U: return string.Format("bgezall r{0},0x{1:x}", rs, pc + (ulong)((short)nn << 2)); // BGEZALL
            case 20U: break;
            case 21U: break;
            case 22U: break;
            case 23U: break;
            case 24U: break;
            case 25U: break;
            case 26U: break;
            case 27U: break;
            case 28U: break;
            case 29U: break;
            case 30U: break;
            case 31U: break;
            }

            return "undefined";
        }

        public static string Disassemble(ulong pc, uint code)
        {
            uint rs = (code >> 21) & 31U;
            uint rt = (code >> 16) & 31U;
            uint nn = (code >>  0) & 65535U;

            switch (code >> 26 & 63U)
            {
            case  0U: return Disassemble_00(pc, code); // SPECIAL
            case  1U: return Disassemble_01(pc, code); // REGIMM
            case  2U: return string.Format("j 0x{0:x}", (pc & 0xFFFFFFFFF0000000U) | ((code & 0x3FFFFFFU) << 2)); // J
            case  3U: return string.Format("jal 0x{0:x}", (pc & 0xFFFFFFFFF0000000U) | ((code & 0x3FFFFFFU) << 2)); // JAL
            case  4U: return string.Format("beq r{0},r{1},0x{2:x}", rt, rs, pc + (ulong)((short)nn << 2)); // BEQ
            case  5U: return string.Format("bne r{0},r{1},0x{2:x}", rt, rs, pc + (ulong)((short)nn << 2)); // BNE
            case  6U: return string.Format("blez r{0},0x{1:x}", rs, pc + (ulong)((short)nn << 2)); // BLEZ
            case  7U: return string.Format("bgtz r{0},0x{1:x}", rs, pc + (ulong)((short)nn << 2)); // BGTZ
            case  8U: return string.Format("add r{0},r{1},0x{2:x}", rt, rs, nn); // ADDI
            case  9U: return string.Format("addu r{0},r{1},0x{2:x}", rt, rs, nn); // ADDIU
            case 10U: return string.Format("slt r{0},r{1},0x{2:x}", rt, rs, nn); // SLTI
            case 11U: return string.Format("sltu r{0},r{1},0x{2:x}", rt, rs, nn); // SLTIU
            case 12U: return string.Format("and r{0},r{1},0x{2:x}", rt, rs, nn); // ANDI
            case 13U: return string.Format("or r{0},r{1},0x{2:x}", rt, rs, nn); // ORI
            case 14U: return string.Format("xor r{0},r{1},0x{2:x}", rt, rs, nn); // XORI
            case 15U: return string.Format("lu r{0},0x{2:x}", rt, rs, nn); // LUI
            case 16U: break; // COP0
            case 17U: break; // COP1
            case 18U: break; // COP2
            case 19U: break;
            case 20U: return string.Format("beql r{0},r{1},0x{2:x}", rt, rs, pc + (ulong)((short)nn << 2)); // BEQL
            case 21U: return string.Format("bnel r{0},r{1},0x{2:x}", rt, rs, pc + (ulong)((short)nn << 2)); // BNEL
            case 22U: return string.Format("blezl r{0},0x{1:x}", rs, pc + (ulong)((short)nn << 2)); // BLEZL
            case 23U: return string.Format("bgtzl r{0},0x{1:x}", rs, pc + (ulong)((short)nn << 2)); // BGTZL
            case 24U: return string.Format("dadd r{0},r{1},0x{2:x}", rt, rs, nn); // DADDI
            case 25U: return string.Format("daddu r{0},r{1},0x{2:x}", rt, rs, nn); // DADDIU
            case 26U: return string.Format("ldl r{0},[r{1}+0x{2:x}]", rt, rs, nn); // LDL
            case 27U: return string.Format("ldr r{0},[r{1}+0x{2:x}]", rt, rs, nn); // LDR
            case 28U: break;
            case 29U: break;
            case 30U: break;
            case 31U: break;
            case 32U: return string.Format("lb r{0},[r{1}+0x{2:x}]", rt, rs, nn); // LB
            case 33U: return string.Format("lh r{0},[r{1}+0x{2:x}]", rt, rs, nn); // LH
            case 34U: return string.Format("lwl r{0},[r{1}+0x{2:x}]", rt, rs, nn); // LWL
            case 35U: return string.Format("lw r{0},[r{1}+0x{2:x}]", rt, rs, nn); // LW
            case 36U: return string.Format("lbu r{0},[r{1}+0x{2:x}]", rt, rs, nn); // LBU
            case 37U: return string.Format("lhu r{0},[r{1}+0x{2:x}]", rt, rs, nn); // LHU
            case 38U: return string.Format("lwr r{0},[r{1}+0x{2:x}]", rt, rs, nn); // LWR
            case 39U: return string.Format("lwu r{0},[r{1}+0x{2:x}]", rt, rs, nn); // LWU
            case 40U: return string.Format("sb r{0},[r{1}+0x{2:x}]", rt, rs, nn); // SB
            case 41U: return string.Format("sh r{0},[r{1}+0x{2:x}]", rt, rs, nn); // SH
            case 42U: return string.Format("swl r{0},[r{1}+0x{2:x}]", rt, rs, nn); // SWL
            case 43U: return string.Format("sw r{0},[r{1}+0x{2:x}]", rt, rs, nn); // SW
            case 44U: return string.Format("sdl r{0},[r{1}+0x{2:x}]", rt, rs, nn); // SDL
            case 45U: return string.Format("sdr r{0},[r{1}+0x{2:x}]", rt, rs, nn); // SDR
            case 46U: return string.Format("swr r{0},[r{1}+0x{2:x}]", rt, rs, nn); // SWR
            case 47U: break; // CACHE
            case 48U: break; // LL
            case 49U: break; // LWC1
            case 50U: break; // LWC2
            case 51U: break;
            case 52U: break; // LLD
            case 53U: break; // LDC1
            case 54U: break; // LDC2
            case 55U: return string.Format("ld r{0},[r{1}+0x{2:x4}]", rt, rs, nn); // LD
            case 56U: break; // SC
            case 57U: break; // SWC1
            case 58U: break; // SWC2
            case 59U: break;
            case 60U: break; // SCD
            case 61U: break; // SDC1
            case 62U: break; // SDC2
            case 63U: return string.Format("sd r{0},[r{1}+0x{2:x4}]", rt, rs, nn); // SD
            }

            return "undefined";
        }
    }
}