using System.Threading;
using System.Windows.Forms;

namespace Nintemulator.Shared
{
    public partial class FormHost : Form
    {
        protected Console console;
        protected Thread thread;

        public Console Console { get { return console; } }

        public FormHost()
        {
            InitializeComponent();
        }

        protected void SetSize(int x, int y)
        {
            ClientSize = new System.Drawing.Size(x, y);
        }

        protected virtual void Play() { }
        protected virtual void Stop() { }
        protected virtual void Execute()
        {
            while (true)
            {
                Step();
            }
        }

        public virtual bool CanPlayGame() { return true; }
        public virtual void AbortThread()
        {
            if (thread != null && thread.IsAlive)
            {
                thread.Abort();
                thread.Join();
                thread = null;
            }
        }
        public virtual void StartThread()
        {
            AbortThread(); // stop thread if it's running currently

            thread = new Thread(Execute);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }
        public virtual void ShowDebugger() { }
        public virtual void Step() { }

        private void FormHost_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (console != null)
            {
                console.Dispose();
                console = null;
            }
        }
    }
}