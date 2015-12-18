using Nintemulator.Shared;
using System;

namespace Nintemulator.FC.CPU
{
    public partial class Cpu : Famicom.Processor
    {
        public Action[] codes;
        public Action[] modes;
        public Flags p;
        public Interrupts interrupts;
        public Register16 ea, pc, sp;
        public byte a, x, y;
        public byte ir;

        public Cpu( Famicom console, Timing.System system )
            : base( console, system )
        {
            Single = system.Cpu;

            Hook( 0x0000u, 0xffffu, PeekOpen, PokeOpen );

            modes = new Action[ 256 ]
            {
                // 0     1        2        3        4        5        6        7        8        9        A        B        C        D        E        F
                Am____a, AmInx_a, AmImp_a, AmInx_a, AmZpg_a, AmZpg_a, AmZpg_a, AmZpg_a, AmImp_a, AmImm_a, AmImp_a, AmImm_a, AmAbs_a, AmAbs_a, AmAbs_a, AmAbs_a, // 0
                AmImm_a, AmIny_r, AmImp_a, AmIny_w, AmZpx_a, AmZpx_a, AmZpx_a, AmZpx_a, AmImp_a, AmAby_r, AmImp_a, AmAby_w, AmAbx_r, AmAbx_r, AmAbx_w, AmAbx_w, // 1
                Am____a, AmInx_a, AmImp_a, AmInx_a, AmZpg_a, AmZpg_a, AmZpg_a, AmZpg_a, AmImp_a, AmImm_a, AmImp_a, AmImm_a, AmAbs_a, AmAbs_a, AmAbs_a, AmAbs_a, // 2
                AmImm_a, AmIny_r, AmImp_a, AmIny_w, AmZpx_a, AmZpx_a, AmZpx_a, AmZpx_a, AmImp_a, AmAby_r, AmImp_a, AmAby_w, AmAbx_r, AmAbx_r, AmAbx_w, AmAbx_w, // 3
                AmImp_a, AmInx_a, AmImp_a, AmInx_a, AmZpg_a, AmZpg_a, AmZpg_a, AmZpg_a, AmImp_a, AmImm_a, AmImp_a, AmImm_a, AmImm_a, AmAbs_a, AmAbs_a, AmAbs_a, // 4
                AmImm_a, AmIny_r, AmImp_a, AmIny_w, AmZpx_a, AmZpx_a, AmZpx_a, AmZpx_a, AmImp_a, AmAby_r, AmImp_a, AmAby_w, AmAbx_r, AmAbx_r, AmAbx_w, AmAbx_w, // 5
                AmImp_a, AmInx_a, AmImp_a, AmInx_a, AmZpg_a, AmZpg_a, AmZpg_a, AmZpg_a, AmImp_a, AmImm_a, AmImp_a, AmImm_a, AmAbs_a, AmAbs_a, AmAbs_a, AmAbs_a, // 6
                AmImm_a, AmIny_r, AmImp_a, AmIny_w, AmZpx_a, AmZpx_a, AmZpx_a, AmZpx_a, AmImp_a, AmAby_r, AmImp_a, AmAby_w, AmAbx_r, AmAbx_r, AmAbx_w, AmAbx_w, // 7
                AmImm_a, AmInx_a, AmImm_a, AmInx_a, AmZpg_a, AmZpg_a, AmZpg_a, AmZpg_a, AmImp_a, AmImm_a, AmImp_a, AmImm_a, AmAbs_a, AmAbs_a, AmAbs_a, AmAbs_a, // 8
                AmImm_a, AmIny_w, AmImp_a, AmIny_w, AmZpx_a, AmZpx_a, AmZpy_a, AmZpy_a, AmImp_a, AmAby_w, AmImp_a, AmAby_w, AmAbs_a, AmAbx_w, AmAbs_a, AmAby_w, // 9
                AmImm_a, AmInx_a, AmImm_a, AmInx_a, AmZpg_a, AmZpg_a, AmZpg_a, AmZpg_a, AmImp_a, AmImm_a, AmImp_a, AmImm_a, AmAbs_a, AmAbs_a, AmAbs_a, AmAbs_a, // A
                AmImm_a, AmIny_r, AmImp_a, AmIny_r, AmZpx_a, AmZpx_a, AmZpy_a, AmZpy_a, AmImp_a, AmAby_r, AmImp_a, AmAby_r, AmAbx_r, AmAbx_r, AmAby_r, AmAby_r, // B
                AmImm_a, AmInx_a, AmImm_a, AmInx_a, AmZpg_a, AmZpg_a, AmZpg_a, AmZpg_a, AmImp_a, AmImm_a, AmImp_a, AmImm_a, AmAbs_a, AmAbs_a, AmAbs_a, AmAbs_a, // C
                AmImm_a, AmIny_r, AmImp_a, AmIny_w, AmZpx_a, AmZpx_a, AmZpx_a, AmZpx_a, AmImp_a, AmAby_r, AmImp_a, AmAby_w, AmAbx_r, AmAbx_r, AmAbx_w, AmAbx_w, // D
                AmImm_a, AmInx_a, AmImm_a, AmInx_a, AmZpg_a, AmZpg_a, AmZpg_a, AmZpg_a, AmImp_a, AmImm_a, AmImp_a, AmImm_a, AmAbs_a, AmAbs_a, AmAbs_a, AmAbs_a, // E
                AmImm_a, AmIny_r, AmImp_a, AmIny_w, AmZpx_a, AmZpx_a, AmZpx_a, AmZpx_a, AmImp_a, AmAby_r, AmImp_a, AmAby_w, AmAbx_r, AmAbx_r, AmAbx_w, AmAbx_w, // F
            };
            codes = new Action[ 256 ]
            {
                // 0     1        2        3        4        5        6        7        8        9        A        B        C        D        E        F
                OpBrk_i, OpOra_m, OpJam_i, OpSlo_m, OpDop_i, OpOra_m, OpAsl_m, OpSlo_m, OpPhp_i, OpOra_m, OpAsl_a, OpAnc_m, OpTop_i, OpOra_m, OpAsl_m, OpSlo_m, // 0
                OpBpl_m, OpOra_m, OpJam_i, OpSlo_m, OpDop_i, OpOra_m, OpAsl_m, OpSlo_m, OpClc_i, OpOra_m, OpNop_i, OpSlo_m, OpTop_i, OpOra_m, OpAsl_m, OpSlo_m, // 1
                OpJsr_m, OpAnd_m, OpJam_i, OpRla_m, OpBit_m, OpAnd_m, OpRol_m, OpRla_m, OpPlp_i, OpAnd_m, OpRol_a, OpAnc_m, OpBit_m, OpAnd_m, OpRol_m, OpRla_m, // 2
                OpBmi_m, OpAnd_m, OpJam_i, OpRla_m, OpDop_i, OpAnd_m, OpRol_m, OpRla_m, OpSec_i, OpAnd_m, OpNop_i, OpRla_m, OpTop_i, OpAnd_m, OpRol_m, OpRla_m, // 3
                OpRti_i, OpEor_m, OpJam_i, OpSre_m, OpDop_i, OpEor_m, OpLsr_m, OpSre_m, OpPha_i, OpEor_m, OpLsr_a, OpAsr_m, OpJmp_m, OpEor_m, OpLsr_m, OpSre_m, // 4
                OpBvc_m, OpEor_m, OpJam_i, OpSre_m, OpDop_i, OpEor_m, OpLsr_m, OpSre_m, OpCli_i, OpEor_m, OpNop_i, OpSre_m, OpTop_i, OpEor_m, OpLsr_m, OpSre_m, // 5
                OpRts_i, OpAdc_m, OpJam_i, OpRra_m, OpDop_i, OpAdc_m, OpRor_m, OpRra_m, OpPla_i, OpAdc_m, OpRor_a, OpArr_m, OpJmi_m, OpAdc_m, OpRor_m, OpRra_m, // 6
                OpBvs_m, OpAdc_m, OpJam_i, OpRra_m, OpDop_i, OpAdc_m, OpRor_m, OpRra_m, OpSei_i, OpAdc_m, OpNop_i, OpRra_m, OpTop_i, OpAdc_m, OpRor_m, OpRra_m, // 7
                OpDop_i, OpSta_m, OpDop_i, OpAax_m, OpSty_m, OpSta_m, OpStx_m, OpAax_m, OpDey_i, OpDop_i, OpTxa_i, OpXaa_m, OpSty_m, OpSta_m, OpStx_m, OpAax_m, // 8
                OpBcc_m, OpSta_m, OpJam_i, OpAxa_m, OpSty_m, OpSta_m, OpStx_m, OpAax_m, OpTya_i, OpSta_m, OpTxs_i, OpXas_m, OpSya_m, OpSta_m, OpSxa_m, OpAxa_m, // 9
                OpLdy_m, OpLda_m, OpLdx_m, OpLax_m, OpLdy_m, OpLda_m, OpLdx_m, OpLax_m, OpTay_i, OpLda_m, OpTax_i, OpLax_m, OpLdy_m, OpLda_m, OpLdx_m, OpLax_m, // A
                OpBcs_m, OpLda_m, OpJam_i, OpLax_m, OpLdy_m, OpLda_m, OpLdx_m, OpLax_m, OpClv_i, OpLda_m, OpTsx_i, OpLar_m, OpLdy_m, OpLda_m, OpLdx_m, OpLax_m, // B
                OpCpy_m, OpCmp_m, OpDop_i, OpDcp_m, OpCpy_m, OpCmp_m, OpDec_m, OpDcp_m, OpIny_i, OpCmp_m, OpDex_i, OpAxs_m, OpCpy_m, OpCmp_m, OpDec_m, OpDcp_m, // C
                OpBne_m, OpCmp_m, OpJam_i, OpDcp_m, OpDop_i, OpCmp_m, OpDec_m, OpDcp_m, OpCld_i, OpCmp_m, OpNop_i, OpDcp_m, OpTop_i, OpCmp_m, OpDec_m, OpDcp_m, // D
                OpCpx_m, OpSbc_m, OpDop_i, OpIsc_m, OpCpx_m, OpSbc_m, OpInc_m, OpIsc_m, OpInx_i, OpSbc_m, OpNop_i, OpSbc_m, OpCpx_m, OpSbc_m, OpInc_m, OpIsc_m, // E
                OpBeq_m, OpSbc_m, OpJam_i, OpIsc_m, OpDop_i, OpSbc_m, OpInc_m, OpIsc_m, OpSed_i, OpSbc_m, OpNop_i, OpIsc_m, OpTop_i, OpSbc_m, OpInc_m, OpIsc_m, // F
            };
        }

        private void Branch( bool flag )
        {
            var data = PeekByte( ea.w, true );

            if ( flag )
            {
                PeekByte( pc.w );
                pc.l = alu.add( pc.l, data );

                switch ( ( data & 0x80u ) >> 7 )
                {
                default: if ( alu.c == 1 ) { PeekByte( pc.w, true ); pc.h += 0x01; } break; // unsigned, pcl+data carried
                case 1u: if ( alu.c == 0 ) { PeekByte( pc.w, true ); pc.h += 0xff; } break; //   signed, pcl-data borrowed
                }
            }
        }

        #region Codes

        private void OpAsl_a( ) { a = shl( a, 0 ); }
        private void OpLsr_a( ) { a = shr( a, 0 ); }
        private void OpRol_a( ) { a = shl( a, p.c.i ); }
        private void OpRor_a( ) { a = shr( a, p.c.i ); }

        private void OpAdc_m( ) { adc( PeekByte( ea.w, true ) ); }
        private void OpAnd_m( ) { and( PeekByte( ea.w, true ) ); }
        private void OpAsl_m( )
        {
            var data = PeekByte( ea.w );
            PokeByte( ea.w, data );
            PokeByte( ea.w, shl( data, 0 ), true );
        }
        private void OpBcc_m( ) { Branch( p.c.i == 0 ); }
        private void OpBcs_m( ) { Branch( p.c.i != 0 ); }
        private void OpBeq_m( ) { Branch( p.z.i != 0 ); }
        private void OpBit_m( )
        {
            var data = PeekByte( ea.w, true );

            p.n.i = ( data >> 7 ) & 1;
            p.v.i = ( data >> 6 ) & 1;
            p.z.b = ( data & a ) == 0;
        }
        private void OpBmi_m( ) { Branch( p.n.i != 0 ); }
        private void OpBne_m( ) { Branch( p.z.i == 0 ); }
        private void OpBpl_m( ) { Branch( p.n.i == 0 ); }
        private void OpBrk_i( )
        {
            const ushort DEC = 0xffff;
            const ushort INC = 0x0001;
            
            PeekByte( pc.w );
            pc.w += ( interrupts.available == 1u ) ? DEC : INC;

            if ( interrupts.rst != 0u )
            {
                PeekByte( sp.w ); sp.l--;
                PeekByte( sp.w ); sp.l--;
                PeekByte( sp.w ); sp.l--;
            }
            else
            {
                byte flag = ( byte )( p & ~( interrupts.available << 4 ) );

                PokeByte( sp.w, pc.h ); sp.l--;
                PokeByte( sp.w, pc.l ); sp.l--;
                PokeByte( sp.w, flag ); sp.l--;
            }

            var vector = ( interrupts.rst != 0u ? 0xfffcu : ( interrupts.nmi != 0u ? 0xfffau : 0xfffeu ) );
            interrupts.nmi = 0u;
            interrupts.rst = 0u;

            p.i.i = 1;
            pc.l   = PeekByte( vector + 0u );
            pc.h   = PeekByte( vector + 1u, true );
        }
        private void OpBvc_m( ) { Branch( p.v.i == 0 ); }
        private void OpBvs_m( ) { Branch( p.v.i != 0 ); }
        private void OpClc_i( ) { p.c.i = 0; }
        private void OpCld_i( ) { p.d.i = 0; }
        private void OpCli_i( ) { p.i.i = 0; }
        private void OpClv_i( ) { p.v.i = 0; }
        private void OpCmp_m( ) { cmp( a, PeekByte( ea.w, true ) ); }
        private void OpCpx_m( ) { cmp( x, PeekByte( ea.w, true ) ); }
        private void OpCpy_m( ) { cmp( y, PeekByte( ea.w, true ) ); }
        private void OpDec_m( )
        {
            var data = PeekByte( ea.w );
            PokeByte( ea.w, data ); mov( --data );
            PokeByte( ea.w, data, true );
        }
        private void OpDex_i( ) { mov( --x ); }
        private void OpDey_i( ) { mov( --y ); }
        private void OpEor_m( ) { eor( PeekByte( ea.w, true ) ); }
        private void OpInc_m( )
        {
            var data = PeekByte( ea.w );
            PokeByte( ea.w, data ); mov( ++data );
            PokeByte( ea.w, data, true );
        }
        private void OpInx_i( ) { mov( ++x ); }
        private void OpIny_i( ) { mov( ++y ); }
        private void OpJmi_m( )
        {
            pc.l = PeekByte( ea.w ); ea.l++; // Emulate the JMP ($nnnn) bug
            pc.h = PeekByte( ea.w, true );
        }
        private void OpJmp_m( )
        {
            pc.l = PeekByte( ea.w++ );
            pc.h = PeekByte( ea.w++, true );
        }
        private void OpJsr_m( )
        {
            ea.l = PeekByte( pc.w++ );

            PeekByte( sp.w );
            PokeByte( sp.w, pc.h ); sp.l--;
            PokeByte( sp.w, pc.l ); sp.l--;

            pc.h = PeekByte( pc.w++, true );
            pc.l = ea.l;
        }
        private void OpLda_m( ) { a = mov( PeekByte( ea.w, true ) ); }
        private void OpLdx_m( ) { x = mov( PeekByte( ea.w, true ) ); }
        private void OpLdy_m( ) { y = mov( PeekByte( ea.w, true ) ); }
        private void OpLsr_m( )
        {
            var data = PeekByte( ea.w );
            PokeByte( ea.w, data ); data = shr( data, 0 );
            PokeByte( ea.w, data, true );
        }
        private void OpOra_m( ) { ora( PeekByte( ea.w, true ) ); }
        private void OpNop_i( ) { }
        private void OpPha_i( )
        {
            PokeByte( sp.w, a, true );
            sp.l--;
        }
        private void OpPhp_i( )
        {
            PokeByte( sp.w, p, true );
            sp.l--;
        }
        private void OpPla_i( )
        {
            PeekByte( sp.w );
            sp.l++;

            a = mov( PeekByte( sp.w, true ) );
        }
        private void OpPlp_i( )
        {
            PeekByte( sp.w );
            sp.l++;

            p = PeekByte( sp.w, true );
        }
        private void OpRol_m( )
        {
            var data = PeekByte( ea.w );
            PokeByte( ea.w, data );
            PokeByte( ea.w, shl( data, p.c.i ), true );
        }
        private void OpRor_m( )
        {
            var data = PeekByte( ea.w );
            PokeByte( ea.w, data );
            PokeByte( ea.w, shr( data, p.c.i ), true );
        }
        private void OpRti_i( )
        {
                   PeekByte( sp.w ); sp.l++;
            p   = PeekByte( sp.w ); sp.l++;
            pc.l = PeekByte( sp.w ); sp.l++;
            pc.h = PeekByte( sp.w, true );
        }
        private void OpRts_i( )
        {
                   PeekByte( sp.w ); sp.l++;
            pc.l = PeekByte( sp.w ); sp.l++;
            pc.h = PeekByte( sp.w );
            PeekByte( pc.w++, true );
        }
        private void OpSbc_m( ) { sbc( PeekByte( ea.w, true ) ); }
        private void OpSec_i( ) { p.c.i = 1; }
        private void OpSed_i( ) { p.d.i = 1; }
        private void OpSei_i( ) { p.i.i = 1; }
        private void OpSta_m( ) { PokeByte( ea.w, a, true ); }
        private void OpStx_m( ) { PokeByte( ea.w, x, true ); }
        private void OpSty_m( ) { PokeByte( ea.w, y, true ); }
        private void OpTax_i( ) { mov( x    = a    ); }
        private void OpTay_i( ) { mov( y    = a    ); }
        private void OpTsx_i( ) { mov( x    = sp.l ); }
        private void OpTxa_i( ) { mov( a    = x    ); }
        private void OpTxs_i( ) {      sp.l = x;      }
        private void OpTya_i( ) { mov( a    = y    ); }

        // Unofficial codes
        private void OpAax_m( )
        {
            PokeByte( ea.w, ( byte )( a & x ), true );
        }
        private void OpAnc_m( )
        {
            and( PeekByte( ea.w, true ) );
            p.c.i = ( a >> 7 );
        }
        private void OpArr_m( )
        {
            and( PeekByte( ea.w, true ) );
            a = shr( a, p.c.i );

            p.c.i = ( a >> 6 ) & 1;
            p.v.i = ( a >> 5 ^ p.c.i ) & 1;
        }
        private void OpAsr_m( )
        {
            and( PeekByte( ea.w, true ) );
            a = shr( a, 0 );
        }
        private void OpAxa_m( )
        {
            PokeByte( ea.w, ( byte )( a & x & 7 ), true );
        }
        private void OpAxs_m( )
        {
            x = cmp( ( byte )( a & x ), PeekByte( ea.w, true ) );
        }
        private void OpDcp_m( )
        {
            var data = PeekByte( ea.w );
            PokeByte( ea.w, data ); mov( --data );
            PokeByte( ea.w, data, true );

            cmp( a, data );
        }
        private void OpDop_i( )
        {
            PeekByte( ea.w, true );
        }
        private void OpIsc_m( )
        {
            var data = PeekByte( ea.w );
            PokeByte( ea.w, data ); mov( ++data );
            PokeByte( ea.w, data, true );

            sbc( data );
        }
        private void OpJam_i( )
        {
            global::System.Windows.Forms.MessageBox.Show( "Keep on jammin'!" );
            pc.w--;
        }
        private void OpLar_m( ) { a = x = mov( sp.l &= PeekByte( ea.w, true ) ); }
        private void OpLax_m( ) { a = x = mov(         PeekByte( ea.w, true ) ); }
        private void OpRla_m( )
        {
            var data = PeekByte( ea.w );
            PokeByte( ea.w, data ); and( data = shl( data, p.c.i ) );
            PokeByte( ea.w, data, true );
        }
        private void OpRra_m( )
        {
            var data = PeekByte( ea.w );
            PokeByte( ea.w, data ); adc( data = shr( data, p.c.i ) );
            PokeByte( ea.w, data, true );
        }
        private void OpSlo_m( )
        {
            var data = PeekByte( ea.w );
            PokeByte( ea.w, data ); ora( data = shl( data, 0 ) );
            PokeByte( ea.w, data, true );
        }
        private void OpSre_m( )
        {
            var data = PeekByte( ea.w );
            PokeByte( ea.w, data ); eor( data = shr( data, 0 ) );
            PokeByte( ea.w, data, true );
        }
        private void OpSxa_m( )
        {
            var data = ( byte )( x & ( ea.h + 1 ) );

            ea.l += y;
            PeekByte( ea.w );

            if ( ea.l < y )
                ea.h = data;

            PokeByte( ea.w, data, true );
        }
        private void OpSya_m( )
        {
            var data = ( byte )( y & ( ea.h + 1 ) );

            ea.l += x;
            PeekByte( ea.w );

            if ( ea.l < x )
                ea.h = data;

            PokeByte( ea.w, data, true );
        }
        private void OpTop_i( )
        {
            PeekByte( ea.w, true );
        }
        private void OpXaa_m( )
        {
            a = mov( ( byte )( x & PeekByte( ea.w, true ) ) );
        }
        private void OpXas_m( )
        {
            sp.l = ( byte )( a & x );

            PokeByte( ea.w, ( byte )( sp.l & ( ea.h + 1 ) ), true );
        }

        #endregion
        #region Modes

        private void Am____a( ) { }
        private void AmAbs_a( )
        {
            ea.l = PeekByte( pc.w++ );
            ea.h = PeekByte( pc.w++ );
        }
        private void AmAbx_r( )
        {
            ea.l = PeekByte( pc.w++ );
            ea.h = PeekByte( pc.w++ );
            ea.l += x;

            if ( ea.l < x )
            {
                PeekByte( ea.w );
                ea.h++;
            }
        }
        private void AmAbx_w( )
        {
            ea.l = PeekByte( pc.w++ );
            ea.h = PeekByte( pc.w++ );
            ea.l += x;

            PeekByte( ea.w );

            if ( ea.l < x )
                ea.h++;
        }
        private void AmAby_r( )
        {
            ea.l = PeekByte( pc.w++ );
            ea.h = PeekByte( pc.w++ );
            ea.l += y;

            if ( ea.l < y )
            {
                PeekByte( ea.w );
                ea.h++;
            }
        }
        private void AmAby_w( )
        {
            ea.l = PeekByte( pc.w++ );
            ea.h = PeekByte( pc.w++ );
            ea.l += y;

            PeekByte( ea.w );

            if ( ea.l < y )
                ea.h++;
        }
        private void AmImm_a( )
        {
            ea.w = pc.w++;
        }
        private void AmImp_a( )
        {
            PeekByte( pc.w, true );
        }
        private void AmInx_a( )
        {
            var pointer = PeekByte( pc.w++ );

            PeekByte( pc.w );
            pointer += x;

            ea.l = PeekByte( pointer++ );
            ea.h = PeekByte( pointer++ );
        }
        private void AmIny_r( )
        {
            var pointer = PeekByte( pc.w++ );

            ea.l = PeekByte( pointer++ );
            ea.h = PeekByte( pointer++ );
            ea.l += y;

            if ( ea.l < y )
            {
                PeekByte( ea.w );
                ea.h++;
            }
        }
        private void AmIny_w( )
        {
            var pointer = PeekByte( pc.w++ );

            ea.l = PeekByte( pointer++ );
            ea.h = PeekByte( pointer++ );
            ea.l += y;

            PeekByte( ea.w );

            if ( ea.l < y )
                ea.h++;
        }
        private void AmZpg_a( )
        {
            ea.l = PeekByte( pc.w++ );
            ea.h = 0;
        }
        private void AmZpx_a( )
        {
            ea.l = PeekByte( pc.w++ );
            ea.h = 0;

            PeekByte( ea.w );
            ea.l += x;
        }
        private void AmZpy_a( )
        {
            ea.l = PeekByte( pc.w++ );
            ea.h = 0;

            PeekByte( ea.w );
            ea.l += y;
        }

        #endregion

        protected override void OnInitialize( )
        {
            InitializeMemory( );
        }
        protected override void OnResetHard( )
        {
            ea.w = 0x0000;
            pc.w = 0x0000;
            sp.w = 0x0100;

            a = 0x00;
            x = 0x00;
            y = 0x00;

            p = 0;

            wram.Initialize<byte>( 0xff );

            interrupts.rst = 1u;
            interrupts.Poll( p.i.u );
        }
        protected override void OnResetSoft( )
        {
            ea.w = 0x0000;

            interrupts.rst = 1u;
            interrupts.Poll( p.i.u );
        }

        public override void Update( )
        {
            ir = PeekByte( pc.w++ );
#if DEBUG
            global::System.Diagnostics.Debug.Assert( interrupts.available == 0u || interrupts.available == 1u );
#endif
            if ( interrupts.available == 1u )
                ir = 0;

            modes[ ir ]( );
            codes[ ir ]( );

            if ( dma )
            {
                dma = false;

                for ( int i = 0; i < 256; i++, dma_addr++ )
                    PokeByte( 0x2004u, PeekByte( dma_addr ) );
            }
        }

        public void Irq( uint value )
        {
            interrupts.irq = value; // level sensitive
            interrupts.irq_latch = value;
        }
        public void Nmi( uint value )
        {
            if ( interrupts.nmi_latch < value ) // edge sensitive (0 -> 1)
                interrupts.nmi = 1u;

            interrupts.nmi_latch = value;
        }

        public struct Flags
        {
            public Flag n, v, d, i, z, c;

            public static implicit operator byte( Flags value )
            {
                return ( byte )(
                    ( value.n.i << 7 ) |
                    ( value.v.i << 6 ) |
                    ( value.d.i << 3 ) |
                    ( value.i.i << 2 ) |
                    ( value.z.i << 1 ) |
                    ( value.c.i << 0 ) | 0x30 );
            }
            public static implicit operator Flags( byte value )
            {
                Flags result = new Flags( );
                result.n.i = ( value >> 7 ) & 1;
                result.v.i = ( value >> 6 ) & 1;
                result.d.i = ( value >> 3 ) & 1;
                result.i.i = ( value >> 2 ) & 1;
                result.z.i = ( value >> 1 ) & 1;
                result.c.i = ( value >> 0 ) & 1;
                return result;
            }
        }
        public struct Interrupts
        {
            public uint irq, irq_latch;
            public uint nmi, nmi_latch;
            public uint rst, rst_latch;
            public uint available;

            public void Poll( uint i )
            {
                available = rst | nmi | ( irq & ~i );
            }
        }
    }
}