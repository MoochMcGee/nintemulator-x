using Nintemulator.GB.CPU;
using Nintemulator.Shared;

namespace Nintemulator.GB
{
    public class Tma : Gameboy.Component
    {
        private static readonly byte[] Lut =
        {
            0x01, // (1.048576MHz / 256) =   4.096KHz
            0x40, // (1.048576MHz /   4) = 262.144KHz
            0x10, // (1.048576MHz /  16) =  65.536KHz
            0x04  // (1.048576MHz /  64) =  16.384KHz
        };

        private Register16 div;
        private Register16 tma;
        private byte cnt;
        private byte mod;

        public Tma( Gameboy console )
            : base( console ) { }

        protected override void OnInitialize( )
        {
            base.OnInitialize( );

            console.Hook( 0xFF04U, ( a ) => div.h, ( a, d ) => div.h = 0 );
            console.Hook( 0xFF05U, ( a ) => tma.h, ( a, d ) => tma.h = d );
            console.Hook( 0xFF06U, ( a ) => mod,   ( a, d ) => mod   = d );
            console.Hook( 0xFF07U, ( a ) => cnt,   ( a, d ) => cnt   = d );
        }

        public void Update( )
        {
            div.w += Lut[ 3 ];

            if ( ( cnt & 0x4 ) != 0 )
            {
                tma.w += Lut[ cnt & 3 ];

                if ( tma.w < Lut[ cnt & 3 ] )
                {
                    tma.h = mod;
                    cpu.RequestInterrupt( Cpu.Interrupt.Elapse );
                }
            }
        }
    }
}