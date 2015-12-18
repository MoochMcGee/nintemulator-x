using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nintemulator.Shared.Processors
{
    public abstract class RP65816
    {
        public abstract void peek();
        public abstract void poke();
    }
}