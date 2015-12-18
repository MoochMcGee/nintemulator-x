using Nintemulator.Shared;

namespace Nintemulator.FC.PAD
{
    public abstract class Pad : Famicom.Input
    {
        public static bool AutofireState;

        public Pad( Famicom console, int index, int buttons )
            : base( console, index, buttons ) { }

        public abstract byte GetData( );
        public abstract void SetData( );
    }
}