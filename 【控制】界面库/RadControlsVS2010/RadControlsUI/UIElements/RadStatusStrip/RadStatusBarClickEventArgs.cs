using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// represent the RadStatusBarClickEventArgs object
    /// </summary>
    public class RadStatusBarClickEventArgs : MouseEventArgs
    {
        //members
        private RadElement clickedElement;

        /// <summary>
        /// present the clicked element
        /// </summary>
        public RadElement ClickedElement
        {
            get { return clickedElement; }
            set { clickedElement = value; }
        }

        /// <summary>
        /// create a instance of 
        /// </summary>
        /// <param name="clickedElement"></param>
        /// <param name="e"></param>
        public RadStatusBarClickEventArgs(RadElement clickedElement, MouseEventArgs e)
            :base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        {
            this.ClickedElement = clickedElement;
        }


    }
}
