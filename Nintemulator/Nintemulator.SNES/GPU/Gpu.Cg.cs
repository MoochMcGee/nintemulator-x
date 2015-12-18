using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nintemulator.SFC.GPU
{
    public partial class Gpu
    {
        internal Cg clr;

        public sealed class Cg : Gpu.Layer
        {
            public Cg( SuperFamicom console )
                : base( console, 0 ) { }

            public sealed override uint GetColorM( uint index, uint priority ) { throw new NotImplementedException( ); }
            public sealed override uint GetColorS( uint index, uint priority ) { throw new NotImplementedException( ); }
        }
    }
}