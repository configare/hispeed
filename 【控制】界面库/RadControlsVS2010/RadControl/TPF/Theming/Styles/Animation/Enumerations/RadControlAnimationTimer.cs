using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Telerik.WinControls
{
    public class RadControlAnimationTimer
    {
        [ThreadStatic]
        private static System.Windows.Forms.Timer internalTimer = null;
        [ThreadStatic]
        private static int timersReferenceCount = 0;
        private static Stopwatch tickCounter;

        private int interval;
        private bool enabled;
        private long lastTicks;

        public int Interval
        {
            get { return interval; }
            set { interval = value; }
        }

        static RadControlAnimationTimer()
        {
            tickCounter = Stopwatch.StartNew();
        }

        public event EventHandler Tick;

        public void Start()
        {
            if (this.enabled)
            {
                Debug.Assert(false, "Timer is already started");
                return;
            }

            timersReferenceCount += 1;
            this.enabled = true;
            this.EnsureTimer();
            internalTimer.Tick += new EventHandler(internalTimer_Tick);
        }

        private void EnsureTimer()
        {
            if (internalTimer == null)
            {
                internalTimer = new Timer();
                internalTimer.Interval = 5;
                internalTimer.Start();
            }
            else if (!internalTimer.Enabled)
            {
                internalTimer.Start();
            }
        }

        void internalTimer_Tick(object sender, EventArgs e)
        {
            if (!this.enabled)
            {
                return;
            }

            long elapsed = tickCounter.ElapsedMilliseconds;
            if (elapsed - this.lastTicks > this.interval)
            {
                this.lastTicks = elapsed;
                this.OnTick(EventArgs.Empty);
            }
        }

        protected void OnTick(EventArgs e)
        {
            EventHandler eh = this.Tick;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        public void Stop()
        {
            if (!this.enabled)
            {
                Debug.Assert(false, "Timer is already stopped");
                return;
            }

            timersReferenceCount -= 1;
            this.enabled = false;
            internalTimer.Tick -= new EventHandler(internalTimer_Tick);
            if (timersReferenceCount == 0)
            {
                internalTimer.Stop();
            }
        }

        public bool IsRunning
        {
            get { return this.enabled; }
        }
    }
}
