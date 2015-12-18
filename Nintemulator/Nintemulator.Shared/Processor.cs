namespace Nintemulator.Shared
{
    public partial class Console<TConsole, TCpu, TPpu, TApu>
    {
        public class Processor : Component
        {
            public SchedulerThread Thread;
            public Timing.System System;
            public int Cycles;
            public int Single;

            public Processor(TConsole console, Timing.System system)
                : base(console)
            {
                this.System = system;
                this.Thread = new SchedulerThread(Enter);
            }

            protected virtual void Enter() { }

            public virtual void Update() { }
            public virtual void Update(int cycles)
            {
                for (this.Cycles += cycles; this.Cycles >= 0; this.Cycles -= this.Single)
                {
                    Update();
                }
            }
        }
    }
}