using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// RadRotator BeginRotate Event Arguments
    /// </summary>
    public class BeginRotateEventArgs : CancelEventArgs
    {
        private int from;
        private int to;

        public BeginRotateEventArgs(int from, int to)
        {
            this.from = from;
            this.to = to;
        }

        public int From
        {
            get { return this.from; }
        }

        public int To
        {
            get { return this.to; }
            set { this.to = value; }
        }
    }

    /// <summary>
    /// Delegate for the BeginRotate event
    /// </summary>
    /// <param name="sender">The RadRotator that rotates</param>
    /// <param name="e"></param>
    public delegate void BeginRotateEventHandler(object sender, BeginRotateEventArgs e);
}
