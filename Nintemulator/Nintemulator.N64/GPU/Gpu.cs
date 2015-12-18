using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nintemulator.N64.GPU
{
    // RCP
    public class Gpu : Nintendo64.Processor
    {
        public Gpu(Nintendo64 console, Timing.System system)
            : base(console, system) { }
    }
}