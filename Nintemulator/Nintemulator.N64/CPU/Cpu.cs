using Nintemulator.Shared;
using s32 = System.Int32;
using u01 = System.Boolean;
using u32 = System.UInt32;
using u64 = System.UInt64;

namespace Nintemulator.N64.CPU
{
    // r4300i
    public partial class Cpu : Nintendo64.Processor
    {
        #region Constants

        // 00h = r0/reg0     08h = t0/reg8     10h = s0/reg16    18h = t8/reg24
        // 01h = at/reg1     09h = t1/reg9     11h = s1/reg17    19h = t9/reg25
        // 02h = v0/reg2     0Ah = t2/reg10    12h = s2/reg18    1Ah = k0/reg26
        // 03h = v1/reg3     0Bh = t3/reg11    13h = s3/reg19    1Bh = k1/reg27
        // 04h = a0/reg4     0Ch = t4/reg12    14h = s4/reg20    1Ch = gp/reg28
        // 05h = a1/reg5     0Dh = t5/reg13    15h = s5/reg21    1Dh = sp/reg29
        // 06h = a2/reg6     0Eh = t6/reg14    16h = s6/reg22    1Eh = s8/reg30
        // 07h = a3/reg7     0Fh = t7/reg15    17h = s7/reg23    1Fh = ra/reg31

        private const int R0 = 0x00;
        private const int AT = 0x01;
        private const int V0 = 0x02;
        private const int V1 = 0x03;
        private const int A0 = 0x04;
        private const int A1 = 0x05;
        private const int A2 = 0x06;
        private const int A3 = 0x07;
        private const int T0 = 0x08;
        private const int T1 = 0x09;
        private const int T2 = 0x0A;
        private const int T3 = 0x0B;
        private const int T4 = 0x0C;
        private const int T5 = 0x0D;
        private const int T6 = 0x0E;
        private const int T7 = 0x0F;
        private const int S0 = 0x10;
        private const int S1 = 0x11;
        private const int S2 = 0x12;
        private const int S3 = 0x13;
        private const int S4 = 0x14;
        private const int S5 = 0x15;
        private const int S6 = 0x16;
        private const int S7 = 0x17;        
        private const int T8 = 0x18;
        private const int T9 = 0x19;
        private const int K0 = 0x1A;
        private const int K1 = 0x1B;
        private const int GP = 0x1C;
        private const int SP = 0x1D;
        private const int S8 = 0x1E;
        private const int RA = 0x1F;

        #endregion

        internal Cp0 cp0;
        internal Cp1 cp1;
        internal Pipeline pipeline;
        internal Register64[] registers = new Register64[32];
        internal u01 ll;
        internal u64 mh;
        internal u64 ml;
        internal u64 pc;

        public Cpu(Nintendo64 console, Timing.System system)
            : base(console, system)
        {
            cp0 = new Cp0(console);
            cp1 = new Cp1();
        }

        private void Branch(bool condition, s32 offset, bool likely = false, bool link = false)
        {
            if (condition)
            {
                if (link)
                    registers[RA].uq0 = pipeline.ic.addr;

                pc = pipeline.rf.addr + (u64)(offset << 2);
            }
            else if (likely)
            {
                // nullify delay slot (turn to sll r0,r0,0x0)
                pipeline.rf = default(Pipeline.Stage);
            }
        }

        private void IC()
        {
            pipeline.ic.addr = pc; pc += 4U;
            pipeline.ic.data = cp0.PeekInstU32(pipeline.ic.addr);
        }
        private void RF()
        {
            var rf = Decode(pipeline.rf.data);
            var dc = pipeline.dc.code;

            if (dc.rd != 0)
            {
                // code relies on data that isn't concurrent, need to slip dc passed wb
                if (dc.rd == rf.r1 || dc.rd == rf.r2) { global::System.Windows.Forms.MessageBox.Show("[RF] Slip Condition"); }
            }

            rf.reg1 = registers[rf.r1];
            rf.reg2 = registers[rf.r2];

            pipeline.rf.code = rf;
        }
        private void EX()
        {
            var code = pipeline.ex.code;

            switch (code.type)
            {
            case 0x00U:
                switch (code.func)
                {
                case 0x00U: code.regd.ud0 =  (code.reg2.ud0 << code.nn); break; // sll
                case 0x02U: code.regd.ud0 =  (code.reg2.ud0 >> code.nn); break; // srl
                case 0x03U: code.regd.sd0 =  (code.reg2.sd0 >> code.nn); break; // sra
                case 0x04U: code.regd.ud0 =  (code.reg2.ud0 << (code.reg1.sd0 & 31)); break; // sllv
                case 0x06U: code.regd.ud0 =  (code.reg2.ud0 >> (code.reg1.sd0 & 31)); break; // srlv
                case 0x07U: code.regd.sd0 =  (code.reg2.sd0 >> (code.reg1.sd0 & 31)); break; // srav
                case 0x08U: break; // jr
                case 0x09U: break; // jalr
                case 0x0CU: break; // syscall
                case 0x0DU: break; // break
                case 0x0FU: break; // sync
                case 0x10U: code.regd.uq0 = mh; break; // mfhi
                case 0x11U: mh = code.reg1.uq0; break; // mthi
                case 0x12U: code.regd.uq0 = ml; break; // mflo
                case 0x13U: ml = code.reg1.uq0; break; // mtlo
                case 0x14U: code.regd.uq0 =  (code.reg2.uq0 << (code.reg1.sd0 & 63)); break;  // dsllv
                case 0x16U: code.regd.uq0 =  (code.reg2.uq0 >> (code.reg1.sd0 & 63)); break;  // dsrlv
                case 0x17U: code.regd.sq0 =  (code.reg2.sq0 >> (code.reg1.sd0 & 63)); break;  // dsrav
                case 0x18U: code.regd.sd0 =  (code.reg1.sd0 * code.reg2.sd0); break; // mult
                case 0x19U: code.regd.ud0 =  (code.reg1.ud0 * code.reg2.ud0); break; // multu
                case 0x1AU: code.regd.sd0 =  (code.reg1.sd0 / code.reg2.sd0); break; // div
                case 0x1BU: code.regd.ud0 =  (code.reg1.ud0 / code.reg2.ud0); break; // divu
                case 0x1CU: code.regd.sq0 =  (code.reg1.sq0 * code.reg2.sq0); break; // dmult
                case 0x1DU: code.regd.uq0 =  (code.reg1.uq0 * code.reg2.uq0); break; // dmultu
                case 0x1EU: code.regd.sq0 =  (code.reg1.sq0 / code.reg2.sq0); break; // ddiv
                case 0x1FU: code.regd.uq0 =  (code.reg1.uq0 / code.reg2.uq0); break; // ddivu
                case 0x20U: code.regd.ud0 =  (code.reg1.ud0 + code.reg2.ud0); /* Trap on overflow */ break; // add
                case 0x21U: code.regd.ud0 =  (code.reg1.ud0 + code.reg2.ud0); break; // addu
                case 0x22U: code.regd.ud0 =  (code.reg1.ud0 - code.reg2.ud0); /* Trap on overflow */ break; // sub
                case 0x23U: code.regd.ud0 =  (code.reg1.ud0 - code.reg2.ud0); break; // subu
                case 0x24U: code.regd.ud0 =  (code.reg1.ud0 & code.reg2.ud0); break; // and
                case 0x25U: code.regd.ud0 =  (code.reg1.ud0 | code.reg2.ud0); break; // or
                case 0x26U: code.regd.ud0 =  (code.reg1.ud0 ^ code.reg2.ud0); break; // xor
                case 0x27U: code.regd.ud0 = ~(code.reg1.ud0 | code.reg2.ud0); break; // nor
                case 0x2AU: code.regd.ud0 =  (code.reg1.sd0 < code.reg2.sd0) ? 1U : 0U; break; // slt
                case 0x2BU: code.regd.ud0 =  (code.reg1.ud0 < code.reg2.ud0) ? 1U : 0U; break; // sltu
                case 0x2CU: code.regd.uq0 =  (code.reg1.uq0 + code.reg2.uq0); /* Trap on overflow */ break; // dadd
                case 0x2DU: code.regd.uq0 =  (code.reg1.uq0 + code.reg2.uq0); break; // daddu
                case 0x2EU: code.regd.uq0 =  (code.reg1.uq0 - code.reg2.uq0); /* Trap on overflow */ break; // dsub
                case 0x2FU: code.regd.uq0 =  (code.reg1.uq0 - code.reg2.uq0); break; // dsubu
                case 0x30U: break; // tge
                case 0x31U: break; // tgeu
                case 0x32U: break; // tlt
                case 0x33U: break; // tltu
                case 0x34U: break; // teq
                case 0x36U: break; // tne
                case 0x38U: code.regd.uq0 = code.reg2.uq0 << code.nn; break; // dsll
                case 0x3AU: code.regd.uq0 = code.reg2.uq0 >> code.nn; break; // dsrl
                case 0x3BU: code.regd.sq0 = code.reg2.sq0 >> code.nn; break; // dsra
                case 0x3CU: code.regd.uq0 = code.reg2.uq0 << (32 + code.nn); break; // dsll32
                case 0x3EU: code.regd.uq0 = code.reg2.uq0 >> (32 + code.nn); break; // dsrl32
                case 0x3FU: code.regd.sq0 = code.reg2.sq0 >> (32 + code.nn); break; // dsra32
                }
                break;

            case 0x01U:
                switch (0U)
                {
                case 0x00U: Branch(code.reg1.sd0 <  0, code.nn); break; // bltz
                case 0x01U: Branch(code.reg1.sd0 >= 0, code.nn); break; // bgez
                case 0x02U: Branch(code.reg1.sd0 <  0, code.nn, likely: true); break; // bltzl
                case 0x03U: Branch(code.reg1.sd0 >= 0, code.nn, likely: true); break; // bgezl
                // 04 - 07
                case 0x08U: break; // tgei
                case 0x09U: break; // tgeiu
                case 0x0AU: break; // tlti
                case 0x0BU: break; // tltiu
                case 0x0CU: break; // teqi
                // 0D
                case 0x0EU: break; // tnei
                // 0F
                case 0x10U: Branch(code.reg1.sd0 <  0, code.nn, link: true); break; // bltzal
                case 0x11U: Branch(code.reg1.sd0 >= 0, code.nn, link: true); break; // bgezal
                case 0x12U: Branch(code.reg1.sd0 <  0, code.nn, link: true, likely: true); break; // bltzall
                case 0x13U: Branch(code.reg1.sd0 >= 0, code.nn, link: true, likely: true); break; // bgezall
                // 14 - 17
                }
                break;

            case 0x02U: break; // j
            case 0x03U: break; // jal
            case 0x04U: Branch(code.reg1.ud0 == code.reg2.ud0, code.nn); break; // beq
            case 0x05U: Branch(code.reg1.ud0 != code.reg2.ud0, code.nn); break; // bne
            case 0x06U: Branch(code.reg1.sd0 <= 0, code.nn); break; // blez
            case 0x07U: Branch(code.reg1.sd0 >  0, code.nn); break; // bgtz
            case 0x08U: code.regd.sd0 = code.reg1.sd0 + code.nn; /* Trap on overflow */ break; // addi
            case 0x09U: code.regd.sd0 = code.reg1.sd0 + code.nn; break; // addiu
            case 0x0AU: code.regd.ud0 = code.reg1.sd0 < code.nn ? 1U : 0U; break; // slti
            case 0x0BU: code.regd.ud0 = code.reg1.ud0 < (u32)code.nn ? 1U : 0U; break; // sltiu
            case 0x0CU: code.regd.sd0 = code.reg1.sd0 & code.nn; break; // andi
            case 0x0DU: code.regd.sd0 = code.reg1.sd0 | code.nn; break; // ori
            case 0x0EU: code.regd.sd0 = code.reg1.sd0 ^ code.nn; break; // xori
            case 0x0FU: code.regd.sd0 = code.nn << 16; break; // lui
            case 0x10U: break;
            case 0x11U: break;
            case 0x12U: break;
            case 0x13U: break;
            case 0x14U: Branch(code.reg1.ud0 == code.reg2.ud0, code.nn, likely: true); break; // beql
            case 0x15U: Branch(code.reg1.ud0 != code.reg2.ud0, code.nn, likely: true); break; // bnel
            case 0x16U: Branch(code.reg1.sd0 <= 0, code.nn, likely: true); break; // blezl
            case 0x17U: Branch(code.reg1.sd0 >  0, code.nn, likely: true); break; // bgtzl
            case 0x18U: break; // daddi
            case 0x19U: break; // daddiu
            case 0x1AU: code.address = (u64)(code.reg1.ud0 + code.nn); break; // ldl
            case 0x1BU: code.address = (u64)(code.reg1.ud0 + code.nn); break; // ldr
            case 0x1CU: break;
            case 0x1DU: break;
            case 0x1EU: break;
            case 0x1FU: break;
            case 0x20U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // lb
            case 0x21U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // lh
            case 0x22U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // lwl
            case 0x23U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // lw
            case 0x24U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // lbu
            case 0x25U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // lhu
            case 0x26U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // lwr
            case 0x27U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // lwu
            case 0x28U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // sb
            case 0x29U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // sh
            case 0x2AU: code.address = (u64)(code.reg1.sd0 + code.nn); break; // swl
            case 0x2BU: code.address = (u64)(code.reg1.sd0 + code.nn); break; // sw
            case 0x2CU: code.address = (u64)(code.reg1.sd0 + code.nn); break; // sdl
            case 0x2DU: code.address = (u64)(code.reg1.sd0 + code.nn); break; // sdr
            case 0x2EU: code.address = (u64)(code.reg1.sd0 + code.nn); break; // swr
            case 0x2FU: break; // cache
            case 0x30U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // ll
            case 0x31U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // lwc1
            case 0x32U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // lwc2
            case 0x33U: break;
            case 0x34U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // lld
            case 0x35U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // ldc1
            case 0x36U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // ldc2
            case 0x37U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // ld
            case 0x38U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // sc
            case 0x39U: code.address = (u64)(code.reg1.sd0 + code.nn); break; // swc1
            case 0x3AU: code.address = (u64)(code.reg1.sd0 + code.nn); break; // swc2
            case 0x3BU: break;
            case 0x3CU: code.address = (u64)(code.reg1.sd0 + code.nn); break; // scd
            case 0x3DU: code.address = (u64)(code.reg1.sd0 + code.nn); break; // sdc1
            case 0x3EU: code.address = (u64)(code.reg1.sd0 + code.nn); break; // sdc2
            case 0x3FU: code.address = (u64)(code.reg1.sd0 + code.nn); break; // sd
            }

            pipeline.ex.code = code;
        }
        private void DC() { }
        private void WB()
        {
            var wb = pipeline.wb.code;

            registers[wb.rd] = wb.regd;
            registers[0].uq0 = 0;
        }

        public override void Update()
        {
            pipeline.wb = pipeline.dc;
            pipeline.dc = pipeline.ex;
            pipeline.ex = pipeline.rf;
            pipeline.rf = pipeline.ic;

            WB(); DC(); EX(); RF(); IC();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            pc = 0xFFFFFFFFBFC00000U;
        }

        public struct Instruction
        {
            public u32 type;
            public u32 func;

            public Register64 regd;
            public Register64 reg1;
            public Register64 reg2;

            public u32 rd;
            public u32 r1;
            public u32 r2;
            public s32 nn;
            public u64 address; // for load/store instructions
        }
        public struct Pipeline
        {
            public Stage wb;
            public Stage dc;
            public Stage ex;
            public Stage rf;
            public Stage ic;

            public struct Stage
            {
                public u64 addr;
                public u32 data;
                public Instruction code;
            }
        }
    }
}