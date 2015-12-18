using System;
using System.Runtime.InteropServices;

namespace Nintemulator.Shared.Processors
{
    public abstract class RP6502
    {
        private Action[] table;
        private Flags flags;
        private Interrupt interrupt;
        private Registers registers;
        private byte data;

        public RP6502()
        {
            this.table = new Action[256]
            {
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null
            };
        }

        private void branch(bool value)
        {
            if (value)
            {
                registers.ea = registers.pc; registers.pcl = Alu.add(registers.pcl, data);

                switch (data >> 7)
                {
                case 0: peek(registers.ea); if (Alu.c == 1) { peek(registers.pc); registers.pch++; } break;
                case 1: peek(registers.ea); if (Alu.c == 0) { peek(registers.pc); registers.pch--; } break;
                }
            }
        }
        private void modify(byte value)
        {
            flags.n = (value >> 7);
            flags.z = (value == 0) ? 1 : 0;
        }

        #region Access Patterns

        private void pattern_i(Action mode)
        {
            mode(); registers.id = registers.ea;

            peek(registers.id); registers.eal = data; registers.idl++; interrupt.poll(flags.i);
            peek(registers.id); registers.eah = data;
        }
        private void pattern_o(bool write, byte offset)
        {
            registers.eal += offset;

            if (registers.eal < offset || write)
            {
                peek(registers.ea);

                if (registers.eal < offset) registers.eah++;
            }
        }
        private void pattern_r(Action code, Action mode)
        {
            mode();

            interrupt.poll(flags.i);

            peek(registers.ea);
            code();
        }
        private void pattern_m(Action code, Action mode)
        {
            mode();

            peek(registers.ea);
            poke(registers.ea);
            code(); interrupt.poll(flags.i);
            poke(registers.ea);
        }
        private void pattern_w(Action code, Action mode)
        {
            mode();

            interrupt.poll(flags.i);

            code();
            poke(registers.ea);
        }

        #endregion
        #region Addressing Modes

        private void am_abs()
        {
            peek(registers.pc++); registers.eal = data;
            peek(registers.pc++); registers.eah = data;
        }
        private void am_abx(bool write) { am_abs(); pattern_o(write, registers.x); }
        private void am_aby(bool write) { am_abs(); pattern_o(write, registers.y); }
        private void am_ind() { pattern_i(am_abs); }
        private void am_inx() { pattern_i(am_zpx); }
        private void am_iny(bool write)
        {
            pattern_i(am_zpg);
            pattern_o(write, registers.y);
        }
        private void am_zpg()
        {
            peek(registers.pc++); registers.ea = data;
        }
        private void am_zpx()
        {
            am_zpg();
            peek(registers.ea); registers.eal += registers.x; // "internal" operation (ea)
        }
        private void am_zpy()
        {
            am_zpg();
            peek(registers.ea); registers.eal += registers.y; // "internal" operation (ea)
        }

        #endregion
        #region Instructions

        private void op_asl_a() { modify(registers.a = Alu.shl(registers.a,       0)); flags.c = Alu.c; }
        private void op_lsr_a() { modify(registers.a = Alu.shr(registers.a,       0)); flags.c = Alu.c; }
        private void op_rol_a() { modify(registers.a = Alu.shl(registers.a, flags.c)); flags.c = Alu.c; }
        private void op_ror_a() { modify(registers.a = Alu.shr(registers.a, flags.c)); flags.c = Alu.c; }

        private void op_adc() { modify(registers.a = Alu.add(registers.a, data, flags.c)); flags.c = Alu.c; flags.v = Alu.v; }
        private void op_and() { modify(registers.a &= data); }
        private void op_asl() { modify(data = Alu.shl(data,       0)); flags.c = Alu.c; }
        private void op_bcc() { branch(flags.c == 0); }
        private void op_bcs() { branch(flags.c == 1); }
        private void op_beq() { branch(flags.z == 1); }
        private void op_bit()
        {
            flags.z = (data & registers.a) == 0 ? 1 : 0;
            flags.n = (data >> 7) & 1;
            flags.v = (data >> 6) & 1;
        }
        private void op_bmi() { branch(flags.n == 1); }
        private void op_bne() { branch(flags.z == 0); }
        private void op_bpl() { branch(flags.n == 0); }
        private void op_brk()
        {
            const ushort inc = 0x0001;
            const ushort dec = 0xffff;

            peek(registers.pc);
            registers.pc += interrupt.available ? dec : inc;

            if (interrupt.res_state != 0)
            {
                peek(registers.sp); registers.spl--;
                peek(registers.sp); registers.spl--;
                peek(registers.sp); registers.spl--;
            }
            else
            {
                var p = (byte)(flags.save() & (interrupt.available ? 0xef : 0xff));

                data = registers.pch; poke(registers.sp); registers.spl--;
                data = registers.pcl; poke(registers.sp); registers.spl--;
                data = p;             poke(registers.sp); registers.spl--;
            }

            registers.ea = (ushort)(interrupt.res_state != 0 ? 0xfffc : (interrupt.nmi_state != 0 ? 0xfffa : 0xfffe));

            peek(registers.ea); registers.pcl = data; registers.ea++;

            flags.i = 1;

            peek(registers.ea); registers.pch = data;
        }
        private void op_bvc() { branch(flags.v == 0); }
        private void op_bvs() { branch(flags.v == 1); }
        private void op_clc() { flags.c = 0; }
        private void op_cld() { flags.d = 0; }
        private void op_cli() { flags.i = 0; }
        private void op_clv() { flags.v = 0; }
        private void op_cmp() { modify(Alu.sub(registers.a, data, 1)); flags.c = Alu.c; }
        private void op_cpx() { modify(Alu.sub(registers.x, data, 1)); flags.c = Alu.c; }
        private void op_cpy() { modify(Alu.sub(registers.y, data, 1)); flags.c = Alu.c; }
        private void op_dec() { modify(--data); }
        private void op_dex() { modify(--registers.x); }
        private void op_dey() { modify(--registers.y); }
        private void op_eor() { modify(registers.a ^= data); }
        private void op_inc() { modify(++data); }
        private void op_inx() { modify(++registers.x); }
        private void op_iny() { modify(++registers.y); }
        private void op_jmp()
        {
            registers.pc = registers.ea;
        }
        private void op_jsr()
        {
            /*                     */ peek(registers.pc++); registers.eal = data;
            /*                     */ peek(registers.sp  ); // why is this cycle here?
            data = registers.pch; poke(registers.sp  ); registers.spl--;
            data = registers.pcl; poke(registers.sp  ); registers.spl--;
            /*                     */ peek(registers.pc++); registers.eah = data;

            registers.pc = registers.ea;
        }
        private void op_lda() { modify(registers.a  = data); }
        private void op_ldx() { modify(registers.x  = data); }
        private void op_ldy() { modify(registers.y  = data); }
        private void op_lsr() { modify(data = Alu.shr(data,       0)); flags.c = Alu.c; }
        private void op_ora() { modify(registers.a |= data); }
        private void op_nop() { }
        private void op_pha()
        {
            data = registers.a; poke(registers.sp); registers.spl--;
        }
        private void op_php()
        {
            data = flags.save(); poke(registers.sp); registers.spl--;
        }
        private void op_pla()
        {
            peek(registers.sp); registers.spl++;
            peek(registers.sp); modify(registers.a = data);
        }
        private void op_plp()
        {
            peek(registers.sp); registers.spl++;
            peek(registers.sp); flags.load(data);
        }
        private void op_rol() { modify(data = Alu.shl(data, flags.c)); flags.c = Alu.c; }
        private void op_ror() { modify(data = Alu.shr(data, flags.c)); flags.c = Alu.c; }
        private void op_rti()
        {
            peek(registers.sp  ); registers.spl++;
            peek(registers.sp  ); registers.spl++; flags.load(data);
            peek(registers.sp  ); registers.spl++; registers.pcl = data;
            peek(registers.sp  ); /*            */ registers.pch = data;
        }
        private void op_rts()
        {
            peek(registers.sp  ); registers.spl++;
            peek(registers.sp  ); registers.spl++; registers.pcl = data;
            peek(registers.sp  ); /*            */ registers.pch = data;
            peek(registers.pc++);
        }
        private void op_sbc() { modify(registers.a = Alu.sub(registers.a, data, flags.c)); flags.c = Alu.c; flags.v = Alu.v; }
        private void op_sec() { flags.c = 1; }
        private void op_sed() { flags.d = 1; }
        private void op_sei() { flags.i = 1; }
        private void op_sta() { data = registers.a; }
        private void op_stx() { data = registers.x; }
        private void op_sty() { data = registers.y; }
        private void op_tax() { modify(registers.x = registers.a); }
        private void op_tay() { modify(registers.y = registers.a); }
        private void op_tsx() { modify(registers.x = registers.spl); }
        private void op_txa() { modify(registers.a = registers.y); }
        private void op_txs() { registers.spl = registers.x; }
        private void op_tya() { modify(registers.a = registers.y); }

        #endregion

        protected abstract void peek(ushort address);
        protected abstract void poke(ushort address);

        protected void step()
        {
            peek(registers.pc++); registers.i = data;

            if (interrupt.available)
                registers.i = 0;

            table[registers.i]();
        }

        public struct Alu
        {
            public static int v;
            public static int c;

            public static byte add(byte augend, byte addend, int carry = 0)
            {
                byte sum = (byte)((augend + addend) + carry);

                v = (~(augend ^ addend) & (augend ^ sum)) >> 7;
                c = ((augend & addend) | ((augend ^ addend) & ~sum)) >> 7;

                return sum;
            }
            public static byte sub(byte minuend, byte subtrahend, int carry = 1)
            {
                subtrahend ^= 0xff;
                return add(minuend, subtrahend, carry);
            }
            public static byte shl(byte operand, int carry = 0)
            {
                c = (operand >> 7);
                return (byte)((operand << 1) | (carry >> 0));
            }
            public static byte shr(byte operand, int carry = 0)
            {
                c = (operand & 1);
                return (byte)((operand >> 1) | (carry << 7));
            }
        }
        public struct Flags
        {
            public int n, v, d, i, z, c;

            public byte save()
            {
                return (byte)(
                    (n << 7) |
                    (v << 6) |
                    (d << 3) |
                    (i << 2) |
                    (z << 1) |
                    (c << 0) | 0x30); // unused bits 4 and 5 are "active low" logic, (0 = on, 1 = off)
            }
            public void load(byte value)
            {
                n = (value >> 7) & 1;
                v = (value >> 6) & 1;
                d = (value >> 3) & 1;
                i = (value >> 2) & 1;
                z = (value >> 1) & 1;
                c = (value >> 0) & 1;
            }
        }
        public struct Interrupt
        {
            public bool available;
            public int irq_state;
            public int nmi_state, nmi_pin, nmi_latch;
            public int res_state;

            public void poll(int i)
            {
                if (nmi_latch < nmi_pin)
                    nmi_state = 1;

                available = (res_state | nmi_state | (irq_state & ~i)) != 0;
            }
        }
        [StructLayout(LayoutKind.Explicit)]
        public struct Registers
        {
            [FieldOffset( 0)] public byte eal;
            [FieldOffset( 1)] public byte eah;

            [FieldOffset( 2)] public byte idl;
            [FieldOffset( 3)] public byte idh;

            [FieldOffset( 4)] public byte pcl;
            [FieldOffset( 5)] public byte pch;

            [FieldOffset( 6)] public byte spl;
            [FieldOffset( 7)] public byte sph;

            [FieldOffset( 8)] public byte a; // accumulator
            [FieldOffset( 9)] public byte x; // x-index register
            [FieldOffset(10)] public byte y; // y-index register
            [FieldOffset(11)] public byte i; // instruction register

            [FieldOffset(0)] public ushort ea; // effective address register
            [FieldOffset(2)] public ushort id; // indirect address register
            [FieldOffset(4)] public ushort pc; // program cursor
            [FieldOffset(6)] public ushort sp; // stack pointer
        }
    }
}