using Nintemulator.SFC.CPU.Debugging;
using Nintemulator.Shared;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using word = System.UInt16;

namespace Nintemulator.SFC.CPU
{
    public partial class Cpu : SuperFamicom.Processor
    {
#if DEBUG
        internal StringBuilder stringBuilder = new StringBuilder(4096);
        internal byte[] buffer = new byte[4];
#endif
        internal Action[] codes;
        internal Action[] modes;
        internal Div div;
        internal Mul mul;
        internal Flags flags;
        internal Registers regs;
        internal Register16 h_check;
        internal Register16 v_check;
        internal Register24 pc;
        internal Register24 ea;
        internal bool irq;
        internal bool nmi, nmi_enabled;
        internal bool latch_a;
        internal bool latch_b;
        internal byte io;
        internal byte reg4210 = 2;
        internal byte reg4211;
        internal byte reg4212;
        internal uint hv_select;

        public Cpu(SuperFamicom console, Timing.System system)
            : base(console, system)
        {
            this.dma0 = new Dma(console);
            this.dma1 = new Dma(console);
            this.dma2 = new Dma(console);
            this.dma3 = new Dma(console);
            this.dma4 = new Dma(console);
            this.dma5 = new Dma(console);
            this.dma6 = new Dma(console);
            this.dma7 = new Dma(console);

            this.modes = new Action[256]
            {
                am_imm_a, am_inx_a, am_imm_a, am_spr_a, am_dpg_a, am_dpg_a, am_dpg_a, am_ind_l, am_imp_a, am_imm_m, am_imp_a, am_imp_a, am_abs_a, am_abs_a, am_abs_a, am_abs_l, // $00
                am_imm_a, am_iny_a, am_ind_a, am_spy_a, am_dpg_a, am_dpx_a, am_dpx_a, am_iny_l, am_imp_a, am_aby_a, am_imp_a, am_imp_a, am_abs_a, am_abx_a, am_abx_a, am_abx_l, // $10
                am_abs_a, am_inx_a, am______, am_spr_a, am_dpg_a, am_dpg_a, am_dpg_a, am_ind_l, am_imp_a, am_imm_m, am_imp_a, am_imp_a, am_abs_a, am_abs_a, am_abs_a, am_abs_l, // $20
                am_imm_a, am_iny_a, am_ind_a, am_spy_a, am_dpx_a, am_dpx_a, am_dpx_a, am_iny_l, am_imp_a, am_aby_a, am_imp_a, am_imp_a, am_abx_a, am_abx_a, am_abx_a, am_abx_l, // $30
                am_imp_a, am_inx_a, am_imp_a, am_spr_a, am______, am_dpg_a, am_dpg_a, am_ind_l, am_imp_a, am_imm_m, am_imp_a, am_imp_a, am_abs_a, am_abs_a, am_abs_a, am_abs_l, // $40
                am_imm_a, am_iny_a, am_ind_a, am_spy_a, am______, am_dpx_a, am_dpx_a, am_iny_l, am_imp_a, am_aby_a, am_imp_a, am_imp_a, am_abs_l, am_abx_a, am_abx_a, am_abx_l, // $50
                am_imp_a, am_inx_a, am_rel_l, am_spr_a, am_dpg_a, am_dpg_a, am_dpg_a, am_ind_l, am_imp_a, am_imm_m, am_imp_a, am_imp_a, am_abs_a, am_abs_a, am_abs_a, am_abs_l, // $60
                am_imm_a, am_iny_a, am_ind_a, am_spy_a, am_dpx_a, am_dpx_a, am_dpx_a, am_iny_l, am_imp_a, am_aby_a, am_imp_a, am_imp_a, am_abx_i, am_abx_a, am_abx_a, am_abx_l, // $70
                am_imm_a, am_inx_a, am_rel_l, am_spr_a, am_dpg_a, am_dpg_a, am_dpg_a, am_ind_l, am_imp_a, am_imm_m, am_imp_a, am_imp_a, am_abs_a, am_abs_a, am_abs_a, am_abs_l, // $80
                am_imm_a, am_iny_a, am_ind_a, am_spy_a, am_dpx_a, am_dpx_a, am_dpy_a, am_iny_l, am_imp_a, am_aby_a, am_imp_a, am_imp_a, am_abs_a, am_abx_a, am_abx_a, am_abx_l, // $90
                am_imm_x, am_inx_a, am_imm_x, am_spr_a, am_dpg_a, am_dpg_a, am_dpg_a, am_ind_l, am_imp_a, am_imm_m, am_imp_a, am_imp_a, am_abs_a, am_abs_a, am_abs_a, am_abs_l, // $a0
                am_imm_a, am_iny_a, am_ind_a, am_spy_a, am_dpx_a, am_dpx_a, am_dpy_a, am_iny_l, am_imp_a, am_aby_a, am_imp_a, am_imp_a, am_abx_a, am_abx_a, am_aby_a, am_abx_l, // $b0
                am_imm_x, am_inx_a, am_imm_a, am_spr_a, am_dpg_a, am_dpg_a, am_dpg_a, am_ind_l, am_imp_a, am_imm_m, am_imp_a, am_imp_a, am_abs_a, am_abs_a, am_abs_a, am_abs_l, // $c0
                am_imm_a, am_iny_a, am_ind_a, am_spy_a, am_ind_a, am_dpx_a, am_dpx_a, am_iny_l, am_imp_a, am_aby_a, am_imp_a, am_imp_a, am_abs_a, am_abx_a, am_abx_a, am_abx_l, // $d0
                am_imm_x, am_inx_a, am_imm_a, am_spr_a, am_dpg_a, am_dpg_a, am_dpg_a, am_ind_l, am_imp_a, am_imm_m, am_imp_a, am_imp_a, am_abs_a, am_abs_a, am_abs_a, am_abs_l, // $e0
                am_imm_a, am_iny_a, am_ind_a, am_spy_a, am_abs_a, am_dpx_a, am_dpx_a, am_iny_l, am_imp_a, am_aby_a, am_imp_a, am_imp_a, am______, am_abx_a, am_abx_a, am_abx_l  // $f0
            };

            this.codes = new Action[256]
            {
                op_brk_i, op_ora_m, op_cop_i, op_ora_m, op_tsb_m, op_ora_m, op_asl_m, op_ora_m, op_php_i, op_ora_m, op_asl_a, op_phd_i, op_tsb_m, op_ora_m, op_asl_m, op_ora_m, // $00
                op_bpl_m, op_ora_m, op_ora_m, op_ora_m, op_trb_m, op_ora_m, op_asl_m, op_ora_m, op_clc_i, op_ora_m, op_inc_a, op_tcs_i, op_trb_m, op_ora_m, op_asl_m, op_ora_m, // $10
                op_jsr_m, op_and_m, op_jsl_m, op_and_m, op_bit_m, op_and_m, op_rol_m, op_and_m, op_plp_i, op_and_m, op_rol_a, op_pld_i, op_bit_m, op_and_m, op_rol_m, op_and_m, // $20
                op_bmi_m, op_and_m, op_and_m, op_and_m, op_bit_m, op_and_m, op_rol_m, op_and_m, op_sec_i, op_and_m, op_dec_a, op_tsc_i, op_bit_m, op_and_m, op_rol_m, op_and_m, // $30
                op_rti_i, op_eor_m, op_wdm_i, op_eor_m, op_mvp_i, op_eor_m, op_lsr_m, op_eor_m, op_pha_i, op_eor_m, op_lsr_a, op_phk_i, op_jmp_m, op_eor_m, op_lsr_m, op_eor_m, // $40
                op_bvc_m, op_eor_m, op_eor_m, op_eor_m, op_mvn_i, op_eor_m, op_lsr_m, op_eor_m, op_cli_i, op_eor_m, op_phy_i, op_tcd_i, op_jmp_l, op_eor_m, op_lsr_m, op_eor_m, // $50
                op_rts_i, op_adc_m, op_pea_m, op_adc_m, op_stz_m, op_adc_m, op_ror_m, op_adc_m, op_pla_i, op_adc_m, op_ror_a, op_rtl_i, op_jmi_m, op_adc_m, op_ror_m, op_adc_m, // $60
                op_bvs_m, op_adc_m, op_adc_m, op_adc_m, op_stz_m, op_adc_m, op_ror_m, op_adc_m, op_sei_i, op_adc_m, op_ply_i, op_tdc_i, op_jmi_m, op_adc_m, op_ror_m, op_adc_m, // $70
                op_bra_m, op_sta_m, op_brl_m, op_sta_m, op_sty_m, op_sta_m, op_stx_m, op_sta_m, op_dey_i, op_tst_m, op_txa_i, op_phb_i, op_sty_m, op_sta_m, op_stx_m, op_sta_m, // $80
                op_bcc_m, op_sta_m, op_sta_m, op_sta_m, op_sty_m, op_sta_m, op_stx_m, op_sta_m, op_tya_i, op_sta_m, op_txs_i, op_txy_i, op_stz_m, op_sta_m, op_stz_m, op_sta_m, // $90
                op_ldy_m, op_lda_m, op_ldx_m, op_lda_m, op_ldy_m, op_lda_m, op_ldx_m, op_lda_m, op_tay_i, op_lda_m, op_tax_i, op_plb_i, op_ldy_m, op_lda_m, op_ldx_m, op_lda_m, // $a0
                op_bcs_m, op_lda_m, op_lda_m, op_lda_m, op_ldy_m, op_lda_m, op_ldx_m, op_lda_m, op_clv_i, op_lda_m, op_tsx_i, op_tyx_i, op_ldy_m, op_lda_m, op_ldx_m, op_lda_m, // $b0
                op_cpy_m, op_cmp_m, op_rep_m, op_cmp_m, op_cpy_m, op_cmp_m, op_dec_m, op_cmp_m, op_iny_i, op_cmp_m, op_dex_i, op_wai_i, op_cpy_m, op_cmp_m, op_dec_m, op_cmp_m, // $c0
                op_bne_m, op_cmp_m, op_cmp_m, op_cmp_m, op_pea_m, op_cmp_m, op_dec_m, op_cmp_m, op_cld_i, op_cmp_m, op_phx_i, op_stp_i, op_jml_m, op_cmp_m, op_dec_m, op_cmp_m, // $d0
                op_cpx_m, op_sbc_m, op_sep_m, op_sbc_m, op_cpx_m, op_sbc_m, op_inc_m, op_sbc_m, op_inx_i, op_sbc_m, op_nop_i, op_xba_i, op_cpx_m, op_sbc_m, op_inc_m, op_sbc_m, // $e0
                op_beq_m, op_sbc_m, op_sbc_m, op_sbc_m, op_pea_m, op_sbc_m, op_inc_m, op_sbc_m, op_sed_i, op_sbc_m, op_plx_i, op_xce_i, op_jsi_m, op_sbc_m, op_inc_m, op_sbc_m  // $f0
            };
        }

        private uint addr_pc()
        {
            return (uint)(pc.b << 16) | pc.w++;
        }

        private byte Pull()
        {
            if (flags.e.b) regs.spl++;
            else /*    */ regs.sp++;

            return PeekByte(regs.sp);
        }
        private void Push(byte value)
        {
            PokeByte(regs.sp, value);

            if (flags.e.b) regs.spl--;
            else /*    */ regs.sp--;
        }

        private void Branch(bool flag)
        {
            var data = operand_byte();

            if (flag)
            {
                if (flags.e.b)
                {
                    op_io();
                    pc.l = alu.add(pc.l, data);

                    switch (data >> 7)
                    {
                    default: if (alu.c == 1) { op_io(); pc.h += 0x01; } break;
                    case 1: if (alu.c == 0) { op_io(); pc.h += 0xff; } break;
                    }
                }
                else
                {
                    op_io();
                    pc.w += (word)(sbyte)(data);
                }
            }
        }
        private void op_io()
        {
            Cycles += SPEED_FAST;
        }
        private void isr(uint vector)
        {
            op_io();
            op_io();

            if (flags.e.i == 0)
                Push(pc.b);

            Push(pc.h);
            Push(pc.l);
            Push(flags.Save());

            flags.d.i = 0;
            flags.i.i = 1;

            pc.l = PeekByte(vector + 0u);
            pc.h = PeekByte(vector + 1u);
            pc.b = 0;
        }
        private void UpdateMode()
        {
            if (flags.e.b)
            {
                regs.sph = 1;
                flags.m.i = 1;
                flags.x.i = 1;
            }

            if (flags.x.b)
            {
                regs.xh = 0;
                regs.yh = 0;
            }
        }

        #region Registers

        internal byte Peek4210(uint address)
        {
            var data = (byte)((reg4210 & 0x8F) | (open & 0x70));

            reg4210 &= 0x7F;
            nmi = false;

            return data;
        }
        internal byte Peek4211(uint address)
        {
            var data = (byte)((reg4211 & 0x80) | (open & 0x7F));

            reg4211 &= 0x7F;
            irq = false;

            return data;
        }
        internal byte Peek4212(uint address) { return reg4212; }
        internal byte Peek4213(uint address) { return io; }
        internal byte Peek4214(uint address) { return div.result.l; }
        internal byte Peek4215(uint address) { return div.result.h; }
        internal byte Peek4216(uint address) { return mul.result.l; }
        internal byte Peek4217(uint address) { return mul.result.h; }
        internal byte Peek4218(uint address) { return console.jp1.latch.l; }
        internal byte Peek4219(uint address) { return console.jp1.latch.h; }
        internal byte Peek421A(uint address) { return console.jp2.latch.l; }
        internal byte Peek421B(uint address) { return console.jp2.latch.h; }
        internal byte Peek421C(uint address) { return console.jp1.latch.l; }
        internal byte Peek421D(uint address) { return console.jp1.latch.h; }
        internal byte Peek421E(uint address) { return console.jp2.latch.l; }
        internal byte Peek421F(uint address) { return console.jp2.latch.h; }
        internal void Poke4200(uint address, byte data)
        {
            nmi_enabled = (data & 0x80U) != 0;
            hv_select = (data & 0x30U) >> 4;
        }
        internal void Poke4201(uint address, byte data)
        {
            if (!latch_a && (data & 0x80U) != 0)
            {
                gpu.LatchCounters();
            }

            latch_a = (data & 0x80U) != 0;
            latch_b = (data & 0x40U) != 0;

            io = data;
        }
        internal void Poke4202(uint address, byte data) { mul.multiplicand = data; } // Multiplicand
        internal void Poke4203(uint address, byte data) { div.timer = 0; mul.timer = 24; mul.multiplier = data; } // Multiplier
        internal void Poke4204(uint address, byte data) { div.dividend.l = data; } // Dividend
        internal void Poke4205(uint address, byte data) { div.dividend.h = data; } // Dividend
        internal void Poke4206(uint address, byte data) { div.timer = 48; mul.timer = 0; div.divisor = data; } // Divisor
        internal void Poke4207(uint address, byte data) { h_check.l = data; }
        internal void Poke4208(uint address, byte data) { h_check.h = data; }
        internal void Poke4209(uint address, byte data) { v_check.l = data; }
        internal void Poke420A(uint address, byte data) { v_check.h = data; }
        internal void Poke420B(uint address, byte data)
        {
            if ((data & 0x01) != 0) { dma0.Transfer(); }
            if ((data & 0x02) != 0) { dma1.Transfer(); }
            if ((data & 0x04) != 0) { dma2.Transfer(); }
            if ((data & 0x08) != 0) { dma3.Transfer(); }
            if ((data & 0x10) != 0) { dma4.Transfer(); }
            if ((data & 0x20) != 0) { dma5.Transfer(); }
            if ((data & 0x40) != 0) { dma6.Transfer(); }
            if ((data & 0x80) != 0) { dma7.Transfer(); }
        }
        internal void Poke420C(uint address, byte data) { }
        internal void Poke420D(uint address, byte data) { cart_time = (data & 0x01U) != 0 ? SPEED_FAST : SPEED_NORM; }

        #endregion

        protected override void OnInitialize()
        {
            base.OnInitialize();

            wram.Initialize<byte>(0x55);
            sram.Initialize<byte>(0xff);

            Hook(0x0000u, 0x1fffu, PeekWRam, PokeWRam);
            Hook(0x2000u, 0xffffu, PeekOpen, PokeOpen); // components will map their registers in later

            dma0.Initialize(0x4300u);
            dma1.Initialize(0x4310u);
            dma2.Initialize(0x4320u);
            dma3.Initialize(0x4330u);
            dma4.Initialize(0x4340u);
            dma5.Initialize(0x4350u);
            dma6.Initialize(0x4360u);
            dma7.Initialize(0x4370u);

            Hook(0x4200u, Poke4200);
            Hook(0x4201u, Poke4201);
            Hook(0x4202u, Poke4202);
            Hook(0x4203u, Poke4203);
            Hook(0x4204u, Poke4204);
            Hook(0x4205u, Poke4205);
            Hook(0x4206u, Poke4206);
            Hook(0x4207u, Poke4207);
            Hook(0x4208u, Poke4208);
            Hook(0x4209u, Poke4209);
            Hook(0x420au, Poke420A);
            Hook(0x420bu, Poke420B);
            Hook(0x420cu, Poke420C);
            Hook(0x420du, Poke420D);
            //     $420e
            //     $420f
            Hook(0x4210u, Peek4210);
            Hook(0x4211u, Peek4211);
            Hook(0x4212u, Peek4212);
            Hook(0x4213u, Peek4213);
            Hook(0x4214u, Peek4214);
            Hook(0x4215u, Peek4215);
            Hook(0x4216u, Peek4216);
            Hook(0x4217u, Peek4217);
            Hook(0x4218u, Peek4218);
            Hook(0x4219u, Peek4219);
            Hook(0x421au, Peek421A);
            Hook(0x421bu, Peek421B);
            Hook(0x421cu, Peek421C);
            Hook(0x421du, Peek421D);
            Hook(0x421eu, Peek421E);
            Hook(0x421fu, Peek421F);
        }
        protected override void OnResetHard()
        {
            base.OnResetHard();

            flags.e.i = 1;
            flags.d.i = 0;
            flags.i.i = 1;

            regs.spl = 0xff;
            regs.sph = 0x01;

            pc.l = PeekByte(0xfffcu);
            pc.h = PeekByte(0xfffdu);
            pc.b = 0;

            UpdateMode();
        }

        public override void Update()
        {
#if DEBUG
            uint cursor = (uint)((pc.b << 16) | pc.w);

            buffer[0] = PeekBusA(cursor + 0);
            buffer[1] = PeekBusA(cursor + 1);
            buffer[2] = PeekBusA(cursor + 2);
            buffer[3] = PeekBusA(cursor + 3);

            stringBuilder.AppendLine(Disassembler.Trace(this, buffer));

            if (stringBuilder.Length > 4096)
            {
                File.AppendAllText("C:\\Users\\Adam\\Downloads\\log.txt", stringBuilder.ToString());
                stringBuilder.Clear();
            }
#endif
            byte code = PeekByte(addr_pc());

            modes[code]();
            codes[code]();

            if (nmi)
            {
                nmi = false;
                isr(flags.e.b ? 0xfffau : 0xffeau);
            }
            else if (irq && flags.i.i == 0)
            {
                irq = false;
                isr(flags.e.b ? 0xfffeu : 0xffeeu);
            }

            if (div.timer != 0 && (div.timer -= Cycles) <= 0)
            {
                div.timer = 0;

                if (div.divisor == 0)
                {
                    div.result.w = 0xffff; // quotient of 0xffff
                    mul.result.w = div.dividend.w;
                }
                else
                {
                    div.result.w = (word)(div.dividend.w / div.divisor);
                    mul.result.w = (word)(div.dividend.w % div.divisor);
                }
            }

            if (mul.timer != 0 && (mul.timer -= Cycles) <= 0)
            {
                mul.timer = 0;
                mul.result.w = (word)(mul.multiplicand * mul.multiplier);
            }

            gpu.Update(Cycles);
            spu.Update(Cycles);

            Cycles = 0;
        }

        public void EnterHBlank()
        {
            reg4212 |= 0x40;
        }
        public void EnterVBlank()
        {
            reg4210 |= 0x80;
            reg4212 |= 0x80;

            console.jp1.Update();
            console.jp2.Update();

            nmi = nmi_enabled;
        }
        public void LeaveHBlank()
        {
            reg4212 &= 0xbf;

            Cycles += 40;
        }
        public void LeaveVBlank()
        {
            reg4210 &= 0x7f;
            reg4212 &= 0x7f;
        }

        public struct Flags
        {
            public Flag e;

            public Flag n;
            public Flag v;
            public Flag m;
            public Flag x;
            public Flag d;
            public Flag i;
            public Flag z;
            public Flag c;

            public void Load(byte data)
            {
                n.i = ((data >> 7) & 1);
                v.i = ((data >> 6) & 1);
                m.i = ((data >> 5) & 1) | e.i;
                x.i = ((data >> 4) & 1) | e.i;
                d.i = ((data >> 3) & 1);
                i.i = ((data >> 2) & 1);
                z.i = ((data >> 1) & 1);
                c.i = ((data >> 0) & 1);
            }
            public byte Save()
            {
                m.i |= e.i;
                x.i |= e.i;

                return (byte)(
                    (n.i << 7) |
                    (v.i << 6) |
                    (m.i << 5) |
                    (x.i << 4) |
                    (d.i << 3) |
                    (i.i << 2) |
                    (z.i << 1) |
                    (c.i << 0));
            }
        }
        public struct Div
        {
            public Register16 result;
            public Register16 dividend;
            public byte divisor;

            public int timer;
        }
        public struct Mul
        {
            public Register16 result;
            public byte multiplicand;
            public byte multiplier;

            public int timer;
        }
        [StructLayout(LayoutKind.Explicit)]
        public struct Registers
        {
            [field: FieldOffset( 0)] public byte al;
            [field: FieldOffset( 1)] public byte ah;
            [field: FieldOffset( 2)] public byte xl;
            [field: FieldOffset( 3)] public byte xh;
            [field: FieldOffset( 4)] public byte yl;
            [field: FieldOffset( 5)] public byte yh;
            [field: FieldOffset( 6)] public byte dl;
            [field: FieldOffset( 7)] public byte dh;
            [field: FieldOffset( 8)] public byte spl;
            [field: FieldOffset( 9)] public byte sph;
            [field: FieldOffset(10)] public byte db;

            [field: FieldOffset( 0)] public word a;
            [field: FieldOffset( 2)] public word x;
            [field: FieldOffset( 4)] public word y;
            [field: FieldOffset( 6)] public word d;
            [field: FieldOffset( 8)] public word sp;
        }
    }
}