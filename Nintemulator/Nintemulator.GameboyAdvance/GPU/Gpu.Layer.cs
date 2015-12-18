using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nintemulator.GBA.GPU
{
    public partial class Gpu
    {
        public class Layer : GameboyAdvance.Component
        {
            public bool enabled;
            public bool[] enable = new bool[ 240 ];
            public int[] priority = new int[ 240 ];
            public int[] raster = new int[ 240 ];
            public int Index;

            public Layer( GameboyAdvance console )
                : base( console ) { }
        }
    }
}