using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface ICanvasEvent
    {
        void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e);
    }
}
