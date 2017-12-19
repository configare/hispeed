using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.Layout
{
    public interface ICanvasEvent
    {
        void Event(object sender, enumCanvasEventType eventType, CanvasEventArgs e);
    }
}
