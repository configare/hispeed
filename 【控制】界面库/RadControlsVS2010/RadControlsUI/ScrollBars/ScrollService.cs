using System;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class ScrollService
    {
        #region Fields

        private RadElement owner;
        private Point location;
        private Timer timer;
        private double delta;
        RadScrollBarElement scrollBar;
        private bool running; 

        #endregion

        #region Ctor

        public ScrollService(RadElement owner, RadScrollBarElement scrollBar)
        {
            this.owner = owner;
            this.scrollBar = scrollBar;
            this.timer = new Timer();
            this.timer.Interval = 5;
            this.timer.Tick += new EventHandler(timer_Tick);
        }

        #endregion

        #region Properties

        public int Interval
        {
            get
            {
                return this.timer.Interval;
            }
            set
            {
                this.timer.Interval = value;
            }
        }

        #endregion

        #region Methods

        public RadElement Owner
        {
            get
            {
                return owner;
            }
        }

        public bool IsScrolling
        {
            get
            {
                return this.running;
            }
        }

        public void Stop()
        {
            this.timer.Stop();
            this.running = false;
        }

        public void MouseDown(Point location)
        { 
            if (!this.owner.ControlBoundingRectangle.Contains(location))
            {
                return;
            }

            this.timer.Stop();
            this.location = location;
            this.delta = 0f;
            this.running = false;
        }

        public void MouseUp(Point point)
        { 
            if (Math.Abs(this.delta) > 0)
            {
                this.timer.Start();
                this.running = true;
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            int step = scrollBar.SmallChange;

            if (scrollBar.ScrollType == ScrollType.Horizontal)
            {
                step = (int)((step * delta) / (owner.Size.Width / 20f));
            }
            else
            {
                step = (int)((step * delta) / (owner.Size.Height / 20f));
            }

            if (!SetScrollValue(step) || step == 0)
            {
                timer.Stop();
                this.running = false;
            }
        }

        public void MouseMove(Point point)
        {
            if (Control.MouseButtons != MouseButtons.Left ||
                !this.owner.ContainsMouse) 
            {
                return;
            }
             
            if (scrollBar.ScrollType == ScrollType.Horizontal)
            {
                this.delta = -point.X + this.location.X;
            }
            else
            {
                this.delta = -point.Y + this.location.Y;
            }

            this.location = point;
            this.SetScrollValue((int)this.delta);
        }

        private bool SetScrollValue(int step)
        {
            bool result = true;
            int newValue = scrollBar.Value + step;

            if (newValue > scrollBar.Maximum - scrollBar.LargeChange + 1)
            {
                newValue = scrollBar.Maximum - scrollBar.LargeChange + 1;
                result = false;
            }
            if (newValue < scrollBar.Minimum)
            {
                newValue = scrollBar.Minimum;
                result = false;
            }

            scrollBar.Value = newValue; 
            return result;
        }

        #endregion
    }
}
