using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Multimedia
{
    public class RadPlayerEventArgs : EventArgs
    {
        private EventCode evCode;
        public EventCode EventCode
        {
            get { return this.evCode; }
        }

        public RadPlayerEventArgs(EventCode evCode)
        {
            this.evCode = evCode;
        }
    }

    public delegate void RadPlayerEventHandler(object sender, RadPlayerEventArgs args);
}
