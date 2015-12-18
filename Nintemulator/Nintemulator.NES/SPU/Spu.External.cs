namespace Nintemulator.FC.SPU
{
    public partial class Spu
    {
        public class ChannelExt : Famicom.Component
        {
            public ChannelExt( Famicom console )
                : base( console ) { }

            public virtual void ClockHalf( ) { }
            public virtual void ClockQuad( ) { }
            public virtual short Render( ) { return 0; }
        }
    }
}