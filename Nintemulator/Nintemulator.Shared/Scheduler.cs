using System.Threading;

namespace Nintemulator.Shared
{
    public class Scheduler
    {
        private static SchedulerThread active;

        public static SchedulerThread Active( )
        {
            if ( active == null )
                active = new SchedulerThread( Thread.CurrentThread );

            return active;
        }
        public static SchedulerThread Create( ThreadStart start )
        {
            if ( active == null )
                active = new SchedulerThread( Thread.CurrentThread );

            return new SchedulerThread( start );
        }

        public static void Delete( SchedulerThread fiber )
        {
            fiber.thread.Abort( );
        }
        public static void Switch( SchedulerThread fiber )
        {
            SchedulerThread previous = active;
            active = fiber;

            previous.handle.Reset( );

            if ( active.thread.ThreadState == ThreadState.Unstarted )
            {
                active.thread.Start( );
            }
            else
            {
                active.handle.Set( );
            }

            previous.handle.Wait( );
        }
    }
    public class SchedulerThread
    {
        public ManualResetEventSlim handle;
        public Thread thread;

        public SchedulerThread( Thread thread )
        {
            this.thread = thread;
            this.handle = new ManualResetEventSlim( false );
        }
        public SchedulerThread( ThreadStart start )
        {
            this.thread = new Thread( start );
            this.handle = new ManualResetEventSlim( false );
        }
    }
}