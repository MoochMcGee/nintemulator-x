namespace Nintemulator.GBA
{
    public class Timer : GameboyAdvance.Component
    {
        private const int Resolution = 10;
        private const int Overflow = ( 1 << 16 ) << Resolution;

        private static readonly int[] ResolutionLut = new int[]
        {
            10, // Log2( 1024 ) - Log2(    1 ),
             4, // Log2( 1024 ) - Log2(   64 ),
             2, // Log2( 1024 ) - Log2(  256 ),
             0  // Log2( 1024 ) - Log2( 1024 )
        };

        private ushort interruptType;
        private int control;
        private int counter;
        private int refresh;
        private int cycles;
        private int number;

        public Timer NextTimer;

        public bool Countup { get { return ( control & 0x0084 ) == 0x0084; } }
        public bool Enabled { get { return ( control & 0x0084 ) == 0x0080; } }

        public Timer( GameboyAdvance console, ushort interruptType, int number )
            : base( console )
        {
            this.interruptType = interruptType;
            this.number = number;
        }

        #region Registers

        private byte PeekCounter_0( uint address ) { return ( byte )( counter ); }
        private byte PeekCounter_1( uint address ) { return ( byte )( counter >> 8 ); }
        private byte PeekControl_0( uint address ) { return ( byte )( control ); }
        private byte PeekControl_1( uint address ) { return ( byte )( control >> 8 ); }
        private void PokeCounter_0( uint address, byte value ) { refresh = ( refresh & ~0x00FF ) | ( value << 0 ); }
        private void PokeCounter_1( uint address, byte value ) { refresh = ( refresh & ~0xFF00 ) | ( value << 8 ); }
        private void PokeControl_0( uint address, byte value ) { control = ( control & ~0x00FF ) | ( value << 0 ); }
        private void PokeControl_1( uint address, byte value ) { control = ( control & ~0xFF00 ) | ( value << 8 ); }

        #endregion

        private void Clock( int amount = 1 << Resolution )
        {
            this.cycles += amount;

            if ( this.cycles >= Overflow )
            {
                this.cycles -= Overflow;
                this.cycles += this.refresh << Resolution;

                if ( spu.dsa.timer == number ) spu.dsa.Clock( );
                if ( spu.dsb.timer == number ) spu.dsb.Clock( );

                if ( ( control & 0x40 ) != 0 )
                    cpu.Interrupt( interruptType );

                if ( NextTimer != null && NextTimer.Countup )
                    NextTimer.Update( );
            }

            this.counter = this.cycles >> Resolution;
        }

        public void Initialize( uint address )
        {
            Initialize( );

            console.Hook( address + 0U, PeekCounter_0, PokeCounter_0 );
            console.Hook( address + 1U, PeekCounter_1, PokeCounter_1 );
            console.Hook( address + 2U, PeekControl_0, PokeControl_0 );
            console.Hook( address + 3U, PeekControl_1, PokeControl_1 );
        }
        public void Update( )
        {
            Clock( );
        }
        public void Update( int amount )
        {
            Clock( amount << ResolutionLut[ control & 0x0003 ] );
        }
    }
}