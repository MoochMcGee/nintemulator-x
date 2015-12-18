using System;

namespace Nintemulator.SFC.CPU
{
    public partial class Cpu
    {
        private void op_asl_a() { if (flags.m.b) { regs.al = shl(regs.al); } else { regs.a = shl(regs.a); } }
        private void op_dec_a() { if (flags.m.b) { regs.al = dec(regs.al); } else { regs.a = dec(regs.a); } }
        private void op_inc_a() { if (flags.m.b) { regs.al = inc(regs.al); } else { regs.a = inc(regs.a); } }
        private void op_lsr_a() { if (flags.m.b) { regs.al = shr(regs.al); } else { regs.a = shr(regs.a); } }
        private void op_rol_a() { if (flags.m.b) { regs.al = shl(regs.al, flags.c.i); } else { regs.a = shl(regs.a, flags.c.i); } }
        private void op_ror_a() { if (flags.m.b) { regs.al = shr(regs.al, flags.c.i); } else { regs.a = shr(regs.a, flags.c.i); } }

        private void op_adc_m() { if (flags.m.b) { adc(operand_byte()); } else { adc(operand_word()); } }
        private void op_and_m() { if (flags.m.b) { and(operand_byte()); } else { and(operand_word()); } }
        private void op_bit_m() { if (flags.m.b) { bit(operand_byte()); } else { bit(operand_word()); } }
        private void op_cmp_m() { if (flags.m.b) { cmp(operand_byte()); } else { cmp(operand_word()); } }
        private void op_cpx_m() { if (flags.x.b) { cpx(operand_byte()); } else { cpx(operand_word()); } }
        private void op_cpy_m() { if (flags.x.b) { cpy(operand_byte()); } else { cpy(operand_word()); } }
        private void op_eor_m() { if (flags.m.b) { eor(operand_byte()); } else { eor(operand_word()); } }
        private void op_ora_m() { if (flags.m.b) { ora(operand_byte()); } else { ora(operand_word()); } }
        private void op_sbc_m() { if (flags.m.b) { sbc(operand_byte()); } else { sbc(operand_word()); } }

        private void op_dex_i() { if (flags.x.b) { regs.xl = dec(regs.xl); } else { regs.x = dec(regs.x); } }
        private void op_dey_i() { if (flags.x.b) { regs.yl = dec(regs.yl); } else { regs.y = dec(regs.y); } }
        private void op_inx_i() { if (flags.x.b) { regs.xl = inc(regs.xl); } else { regs.x = inc(regs.x); } }
        private void op_iny_i() { if (flags.x.b) { regs.yl = inc(regs.yl); } else { regs.y = inc(regs.y); } }

        private void op_lda_m() { if (flags.m.b) { regs.al = mov(operand_byte()); } else { regs.a = mov(operand_word()); } }
        private void op_ldx_m() { if (flags.x.b) { regs.xl = mov(operand_byte()); } else { regs.x = mov(operand_word()); } }
        private void op_ldy_m() { if (flags.x.b) { regs.yl = mov(operand_byte()); } else { regs.y = mov(operand_word()); } }
        private void op_sta_m() { if (flags.m.b) { operand_byte(regs.al); } else { operand_word(regs.a); } }
        private void op_stx_m() { if (flags.x.b) { operand_byte(regs.xl); } else { operand_word(regs.x); } }
        private void op_sty_m() { if (flags.x.b) { operand_byte(regs.yl); } else { operand_word(regs.y); } }
        private void op_stz_m() { if (flags.m.b) { operand_byte(0x00); } else { operand_word(0x0000); } }

        private void op_asl_m() { if (flags.m.b) { var data = operand_byte(); op_io(); operand_byte(shl(data           )); } else { var data = operand_word(); op_io(); operand_word(shl(data           )); } }
        private void op_dec_m() { if (flags.m.b) { var data = operand_byte(); op_io(); operand_byte(dec(data           )); } else { var data = operand_word(); op_io(); operand_word(dec(data           )); } }
        private void op_inc_m() { if (flags.m.b) { var data = operand_byte(); op_io(); operand_byte(inc(data           )); } else { var data = operand_word(); op_io(); operand_word(inc(data           )); } }
        private void op_lsr_m() { if (flags.m.b) { var data = operand_byte(); op_io(); operand_byte(shr(data           )); } else { var data = operand_word(); op_io(); operand_word(shr(data           )); } }
        private void op_rol_m() { if (flags.m.b) { var data = operand_byte(); op_io(); operand_byte(shl(data, flags.c.i)); } else { var data = operand_word(); op_io(); operand_word(shl(data, flags.c.i)); } }
        private void op_ror_m() { if (flags.m.b) { var data = operand_byte(); op_io(); operand_byte(shr(data, flags.c.i)); } else { var data = operand_word(); op_io(); operand_word(shr(data, flags.c.i)); } }
        private void op_trb_m() { if (flags.m.b) { var data = operand_byte(); op_io(); operand_byte(trb(data           )); } else { var data = operand_word(); op_io(); operand_word(trb(data           )); } }
        private void op_tsb_m() { if (flags.m.b) { var data = operand_byte(); op_io(); operand_byte(tsb(data           )); } else { var data = operand_word(); op_io(); operand_word(tsb(data           )); } }

        private void op_bra_m() { Branch(true); }
        private void op_bpl_m() { Branch(flags.n.i == 0); }
        private void op_bmi_m() { Branch(flags.n.i == 1); }
        private void op_bvc_m() { Branch(flags.v.i == 0); }
        private void op_bvs_m() { Branch(flags.v.i == 1); }
        private void op_bne_m() { Branch(flags.z.i == 0); }
        private void op_beq_m() { Branch(flags.z.i == 1); }
        private void op_bcc_m() { Branch(flags.c.i == 0); }
        private void op_bcs_m() { Branch(flags.c.i == 1); }

        private void op_clc_i() { flags.c.i = 0; }
        private void op_sec_i() { flags.c.i = 1; }
        private void op_cld_i() { flags.d.i = 0; }
        private void op_sed_i() { flags.d.i = 1; }
        private void op_cli_i() { flags.i.i = 0; }
        private void op_sei_i() { flags.i.i = 1; }
        private void op_clv_i() { flags.v.i = 0; }

        private void op_brk_i()
        {
            op_io();

            if (flags.e.i == 0)
                Push(pc.b);

            Push(pc.h);
            Push(pc.l);
            Push(flags.Save());

            flags.d.i = 0;
            flags.i.i = 1;

            uint vector = flags.e.b ? 0xfffeu : 0xffe6u;

            pc.l = PeekByte(vector + 0u);
            pc.h = PeekByte(vector + 1u);
            pc.b = 0;
        }
        private void op_brl_m()
        {
            op_io();
            pc.w = ea.w;
        }
        private void op_cop_i()
        {
            op_io();

            if (flags.e.i == 0)
                Push(pc.b);

            Push(pc.h);
            Push(pc.l);
            Push(flags.Save());

            flags.d.i = 0;
            flags.i.i = 1;

            uint vector = flags.e.b ? 0xfff4u : 0xffe4u;

            pc.l = PeekByte(vector + 0u);
            pc.h = PeekByte(vector + 1u);
            pc.b = 0;
        }
        private void op_jsi_m()
        {
            ea.l = PeekByte(addr_pc());

            Push(pc.h);
            Push(pc.l);

            ea.h = PeekByte(addr_pc());

            op_io();
            ea.w += regs.x;
            ea.b = pc.b;

            pc.l = operand_byte(); ea.w++;
            pc.h = operand_byte(); ea.w++;
        }
        private void op_jsl_m()
        {
            ea.l = PeekByte(addr_pc());
            ea.h = PeekByte(addr_pc());

            Push(pc.b);

            op_io();
            ea.b = PeekByte(addr_pc());

            pc.w--;

            Push(pc.h);
            Push(pc.l);

            pc.w = ea.w;
            pc.b = ea.b;
        }
        private void op_jsr_m()
        {
            op_io();
            pc.w--;

            Push(pc.h);
            Push(pc.l);

            pc.w = ea.w;
        }
        private void op_jmi_m()
        {
            pc.l = PeekByte(ea.w++);
            pc.h = PeekByte(ea.w);
        }
        private void op_jml_m()
        {
            pc.l = PeekByte(ea.w++);
            pc.h = PeekByte(ea.w++);
            pc.b = PeekByte(ea.w);
        }
        private void op_jmp_m()
        {
            pc.w = ea.w;
        }
        private void op_jmp_l()
        {
            pc.w = ea.w;
            pc.b = ea.b;
        }
        private void op_mvn_i()
        {
            uint dst = (uint)((regs.db = PeekByte(addr_pc())) << 16) | regs.y;
            uint src = (uint)((          PeekByte(addr_pc())) << 16) | regs.x;

            PokeByte(dst, PeekByte(src));

            op_io(); regs.x++;
            op_io(); regs.y++;

            if (flags.x.b)
            {
                regs.xh = 0;
                regs.yh = 0;
            }

            if (--regs.a != 0xffff)
                pc.w -= 3;
        }
        private void op_mvp_i()
        {
            uint dst = (uint)((regs.db = PeekByte(addr_pc())) << 16) | regs.y;
            uint src = (uint)((          PeekByte(addr_pc())) << 16) | regs.x;

            PokeByte(dst, PeekByte(src));

            op_io(); regs.x--;
            op_io(); regs.y--;

            if (flags.x.b)
            {
                regs.xh = 0;
                regs.yh = 0;
            }

            if (--regs.a != 0xffff)
                pc.w -= 3;
        }
        private void op_nop_i() { }
        private void op_pea_m()
        {
            Push(ea.h);
            Push(ea.l);
        }
        private void op_pha_i() { if (flags.m.i == 0) { Push(regs.ah); } Push(regs.al); }
        private void op_phb_i()
        {
            Push(regs.db);
        }
        private void op_phd_i()
        {
            Push(regs.dh);
            Push(regs.dl);
        }
        private void op_phk_i()
        {
            Push(pc.b);
        }
        private void op_php_i()
        {
            Push(flags.Save());
        }
        private void op_phx_i() { if (flags.x.i == 0) { Push(regs.xh); } Push(regs.xl); }
        private void op_phy_i() { if (flags.x.i == 0) { Push(regs.yh); } Push(regs.yl); }
        private void op_pla_i()
        {
            op_io();

            if (flags.m.b) { regs.al = Pull(); /*             */ mov(regs.al); }
            else /*     */ { regs.al = Pull(); regs.ah = Pull(); mov(regs.a); }
        }
        private void op_plb_i()
        {
            op_io();

            regs.db = mov(Pull());
        }
        private void op_pld_i()
        {
            op_io();

            regs.dl = Pull();
            regs.dh = Pull();
            mov(regs.d);
        }
        private void op_plp_i()
        {
            op_io();

            flags.Load(Pull());

            UpdateMode();
        }
        private void op_plx_i()
        {
            op_io();

            if (flags.x.b) { regs.xl = Pull(); /*             */ mov(regs.xl); }
            else /*     */ { regs.xl = Pull(); regs.xh = Pull(); mov(regs.x); }
        }
        private void op_ply_i()
        {
            op_io();

            if (flags.x.b) { regs.yl = Pull(); /*             */ mov(regs.yl); }
            else /*     */ { regs.yl = Pull(); regs.yh = Pull(); mov(regs.y); }
        }
        private void op_rep_m()
        {
            flags.Load((byte)(flags.Save() & ~operand_byte()));
            UpdateMode();
        }
        private void op_rti_i()
        {
            op_io();

            flags.Load(Pull()); UpdateMode();
            pc.l = Pull();
            pc.h = Pull(); if (flags.e.i == 1) return;
            pc.b = Pull();
        }
        private void op_rtl_i()
        {
            op_io();

            pc.l = Pull();
            pc.h = Pull();
            pc.b = Pull(); pc.w++;
        }
        private void op_rts_i()
        {
            op_io();

            pc.l = Pull();
            pc.h = Pull();

            op_io();
            pc.w++;
        }
        private void op_sep_m()
        {
            flags.Load((byte)(flags.Save() | operand_byte()));
            UpdateMode();
        }
        private void op_stp_i() { throw new NotSupportedException("stp"); }
        private void op_tax_i() { if (flags.x.b) { regs.xl = mov(regs.al); } else { regs.x = mov(regs.a); } }
        private void op_tay_i() { if (flags.x.b) { regs.yl = mov(regs.al); } else { regs.y = mov(regs.a); } }
        private void op_tcd_i()
        {
            regs.d = regs.a;
        }
        private void op_tcs_i()
        {
            regs.sp = regs.a;

            if (flags.e.b)
                regs.sph = 0x01;
        }
        private void op_tdc_i()
        {
            regs.a = mov(regs.d);
        }
        private void op_tsc_i()
        {
            regs.a = mov(regs.sp);
        }
        private void op_tst_m()
        {
            if (flags.m.b) { flags.z.b = (operand_byte() & regs.al) == 0; }
            else /*     */ { flags.z.b = (operand_word() & regs.a ) == 0; }
        }
        private void op_tsx_i() { if (flags.x.b) { regs.xl = mov(regs.spl); } else { regs.x = mov(regs.sp); } }
        private void op_txa_i() { if (flags.m.b) { regs.al = mov(regs.xl); } else { regs.a = mov(regs.x); } }
        private void op_txs_i()
        {
            regs.sp = regs.x;

            if (flags.e.b)
                regs.sph = 0x01;
        }
        private void op_txy_i() { if (flags.x.b) { regs.yl = mov(regs.xl); } else { regs.y = mov(regs.x); } }
        private void op_tya_i() { if (flags.m.b) { regs.al = mov(regs.yl); } else { regs.a = mov(regs.y); } }
        private void op_tyx_i() { if (flags.x.b) { regs.xl = mov(regs.yl); } else { regs.x = mov(regs.y); } }
        private void op_wai_i() { throw new NotSupportedException("wai"); }
        private void op_wdm_i() { throw new NotSupportedException("wdm"); }
        private void op_xba_i()
        {
            op_io();

            var tmp = regs.ah;
            regs.ah = regs.al;
            regs.al = mov(tmp);
        }
        private void op_xce_i()
        {
            var tmp = flags.e;
            flags.e = flags.c;
            flags.c = tmp;

            UpdateMode();
        }
    }
}