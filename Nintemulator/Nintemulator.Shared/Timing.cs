namespace Nintemulator
{
    public struct Timing
    {
        public int Cycles;
        public int Period;
        public int Single;

        public Timing( int period, int single )
        {
            this.Cycles = 0;
            this.Period = period;
            this.Single = single;
        }

        public bool Clock( )
        {
            Cycles += Single;

            if ( Cycles >= Period )
            {
                Cycles -= Period;
                return true;
            }

            return false;
        }
        public bool ClockDown( )
        {
            Cycles -= Single;

            if ( Cycles < 0 )
            {
                Cycles += Period;
                return true;
            }

            return false;
        }

        public struct System
        {
            public int Period;
            public int Serial;
            public int Apu;
            public int Cpu;
            public int Ppu;

            public System( int serial, int period, int cpu, int ppu, int apu )
            {
                this.Period = period;
                this.Serial = serial;
                this.Cpu = cpu;
                this.Ppu = ppu;
                this.Apu = apu;
            }
        }
    }
}