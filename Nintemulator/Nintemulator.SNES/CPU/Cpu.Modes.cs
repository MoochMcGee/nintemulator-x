namespace Nintemulator.SFC.CPU
{
    public partial class Cpu
    {
        private void am______( ) { }
        private void am_abs_a( )
        {
            ea.l = PeekByte( addr_pc( ) );
            ea.h = PeekByte( addr_pc( ) );
            ea.b = regs.db;
        }
        private void am_abs_l( )
        {
            ea.l = PeekByte( addr_pc( ) );
            ea.h = PeekByte( addr_pc( ) );
            ea.b = PeekByte( addr_pc( ) );
        }
        private void am_abx_a( )
        {
            am_abs_a( );

            op_io( );
            ea.d += regs.x;
        }
        private void am_abx_i( )
        {
            ea.l = PeekByte( addr_pc( ) );
            ea.h = PeekByte( addr_pc( ) );
            ea.b = pc.b;

            op_io( );
            ea.w += regs.x;
        }
        private void am_abx_l( )
        {
            am_abs_l( );

            ea.d += regs.x;
        }
        private void am_aby_a( )
        {
            am_abs_a( );

            op_io( );
            ea.d += regs.y;
        }
        private void am_dpg_a( )
        {
            ea.l = PeekByte( addr_pc( ) );
            ea.h = regs.dh;
            ea.b = 0;

            if ( regs.dl != 0 )
            {
                op_io( );
                ea.w += regs.dl;
            }
        }
        private void am_dpx_a( )
        {
            am_dpg_a( );

            op_io( );
            ea.w += ( ushort )( flags.x.b ? regs.xl : regs.x );
        }
        private void am_dpy_a( )
        {
            am_dpg_a( );

            op_io( );
            ea.w += ( ushort )( flags.x.b ? regs.yl : regs.y );
        }
        private void am_imm_a( )
        {
            ea.w = pc.w++;
            ea.b = pc.b;
        }
        private void am_imm_m( )
        {
            ea.w = pc.w;
            ea.b = pc.b;
            pc.w += ( ushort )( 2 - flags.m.i );
        }
        private void am_imm_x( )
        {
            ea.w = pc.w;
            ea.b = pc.b;
            pc.w += ( ushort )( 2 - flags.x.i );
        }
        private void am_imp_a( )
        {
            op_io( );
        }
        private void am_ind_a( )
        {
            am_dpg_a( );

            var l = PeekByte( ea.d ); ea.w++;
            var h = PeekByte( ea.d );

            ea.l = l;
            ea.h = h;
            ea.b = regs.db;
        }
        private void am_ind_l( )
        {
            am_dpg_a( );

            var l = PeekByte( ea.d ); ea.w++;
            var h = PeekByte( ea.d ); ea.w++;
            var b = PeekByte( ea.d );

            ea.l = l;
            ea.h = h;
            ea.b = b;
        }
        private void am_inx_a( )
        {
            am_dpx_a( );

            var l = PeekByte( ea.d ); ea.w++;
            var h = PeekByte( ea.d );

            ea.l = l;
            ea.h = h;
            ea.b = regs.db;
        }
        private void am_iny_a( )
        {
            am_ind_a( );

            op_io( );
            ea.d += regs.y;
        }
        private void am_iny_l( )
        {
            am_ind_l( );

            ea.d += regs.y;
        }
        private void am_rel_l( )
        {
            ea.l = PeekByte( addr_pc( ) );
            ea.h = PeekByte( addr_pc( ) );
            ea.b = pc.b;

            ea.w += pc.w;
        }
        private void am_spr_a( )
        {
            ea.l = PeekByte( addr_pc( ) );
            ea.h = regs.sph;
            ea.b = 0;

            op_io( );
            ea.w += regs.spl;
        }
        private void am_spy_a( )
        {
            am_spr_a( );

            var l = PeekByte( ea.d ); ea.w++;
            var h = PeekByte( ea.d );

            ea.l = l;
            ea.h = h;
            ea.b = regs.db;

            op_io( );
            ea.d += regs.y;
        }
    }
}