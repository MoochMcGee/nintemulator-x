namespace Nintemulator.FC.SPU
{
    public partial class Spu
    {
        public const int DELAY = 13125;
        public const int PHASE = 352;

        public class Channel : Famicom.Component
        {
            protected Timing timing;
            protected Timing.System system;
            protected int frequency;

            public virtual bool Enabled { get; set; }

            public Channel( Famicom console, Timing.System system )
                : base( console ) { }

            public void Initialize( uint address )
            {
                base.Initialize( );

                cpu.Hook( address + 0U, PokeReg1 );
                cpu.Hook( address + 1U, PokeReg2 );
                cpu.Hook( address + 2U, PokeReg3 );
                cpu.Hook( address + 3U, PokeReg4 );
            }

            public virtual void PokeReg1( uint address, byte data ) { }
            public virtual void PokeReg2( uint address, byte data ) { }
            public virtual void PokeReg3( uint address, byte data ) { }
            public virtual void PokeReg4( uint address, byte data ) { }
            public virtual byte Render( ) { return 0; }
        }
    }
}