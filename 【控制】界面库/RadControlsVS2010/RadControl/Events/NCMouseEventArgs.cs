using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    public class NCMouseEventArgs : MouseEventArgs
    {
        public NCMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta) :
            base(button, clicks, x, y, delta)
        {
        }

        public NCMouseEventArgs(MouseEventArgs args) :
            base(args.Button, args.Clicks, args.X, args.Y, args.Delta)
        {
        }
    }
}
