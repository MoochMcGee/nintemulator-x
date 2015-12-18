namespace Nintemulator.GB
{
    public class Pad : Gameboy.Input
    {
        public static bool AutofireState;

        private bool p14;
        private bool p15;
        private byte p14latch;
        private byte p15latch;

        public Pad( Gameboy console )
            : base( console, 0, 10 )
        {
            Map( 0, "A" );
            Map( 1, "X" );
            Map( 2, "Back" );
            Map( 3, "Menu" );
            Map( 4, "DPad-R" );
            Map( 5, "DPad-L" );
            Map( 6, "DPad-U" );
            Map( 7, "DPad-D" );
            Map( 8, "B" );
            Map( 9, "Y" );
        }

        private byte Peek( uint address )
        {
            if ( p15 ) { return p15latch; }
            if ( p14 ) { return p14latch; }

            return 0xff;
        }
        private void Poke( uint address, byte data )
        {
            p15 = ( data & 0x20 ) == 0;
            p14 = ( data & 0x10 ) == 0;
        }

        public override void Update( )
        {
            base.Update( );

            p15latch = 0xff ^ 0x20;

            if ( Pressed( 0 ) ) p15latch ^= 0x1;
            if ( Pressed( 1 ) ) p15latch ^= 0x2;
            if ( Pressed( 2 ) ) p15latch ^= 0x4;
            if ( Pressed( 3 ) ) p15latch ^= 0x8;

            p14latch = 0xff ^ 0x10;

            if ( Pressed( 4 ) ) p14latch ^= 0x1;
            if ( Pressed( 5 ) ) p14latch ^= 0x2;
            if ( Pressed( 6 ) ) p14latch ^= 0x4;
            if ( Pressed( 7 ) ) p14latch ^= 0x8;

            if ( AutofireState )
            {
                if ( Pressed( 8 ) ) p15latch ^= 0x1;
                if ( Pressed( 9 ) ) p15latch ^= 0x2;
            }
        }

        protected override void OnInitialize( )
        {
            base.OnInitialize( );

            console.Hook( 0xff00, Peek, Poke );
        }
    }
}