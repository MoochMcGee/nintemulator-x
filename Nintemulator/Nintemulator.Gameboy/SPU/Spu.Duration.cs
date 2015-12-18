namespace Nintemulator.GB.SPU
{
    public partial class Spu
    {
        public class Duration
        {
            public bool Enabled;
            public int Counter;
            public int Refresh;

            public bool Clock()
            {
                return (Enabled && Counter != 0 && --Counter == 0);
            }
        }
    }
}