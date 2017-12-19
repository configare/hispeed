using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a mouse timer.
    /// </summary>
    public class MouseHoverTimer : IDisposable
    {

        // Methods
        public MouseHoverTimer()
        {
            this.mouseHoverTimer = new Timer();
            int num1 = SystemInformation.MouseHoverTime;
            if (num1 == 0)
            {
                num1 = 400;
            }
            this.mouseHoverTimer.Interval = num1;
            this.mouseHoverTimer.Tick += new EventHandler(this.OnTick);
        }

        public void Cancel()
        {
			if (this.mouseHoverTimer != null)
			{
				this.mouseHoverTimer.Enabled = false;
				this.currentElement = null;
			}
        }

        public void Cancel(RadElement element)
        {
			if (element == this.currentElement)
            {
                this.Cancel();
            }
        }

        public void Dispose()
        {
            if (this.mouseHoverTimer != null)
            {
                this.Cancel();
                this.mouseHoverTimer.Tick -= new EventHandler(this.OnTick);
                this.mouseHoverTimer.Stop();
				this.mouseHoverTimer.Dispose();
                this.mouseHoverTimer = null;
            }
        }

        private void OnTick(object sender, EventArgs e)
        {
            this.mouseHoverTimer.Enabled = false;

			if (this.currentElement != null && this.currentElement.ElementState == ElementState.Loaded)
            {
				this.currentElement.CallDoMouseHover(EventArgs.Empty);
            }

            this.currentElement = null;
        }

        public void Start(RadElement element)
        {
			if (element != this.currentElement)
            {
				this.Cancel(this.currentElement);
            }
			this.currentElement = element;
			if (this.currentElement != null)
            {
                this.mouseHoverTimer.Enabled = true;
            }
        }


        // Fields
        private RadElement currentElement;
        private Timer mouseHoverTimer;
        private const int SPI_GETMOUSEHOVERTIME_WIN9X = 400;
    }
}

