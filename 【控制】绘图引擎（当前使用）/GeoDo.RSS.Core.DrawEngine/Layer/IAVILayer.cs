using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface IAVILayer:IRenderLayer
    {
        int BitmapCount { get; }
        int Interval { get; set; }
        EventHandler OnTicked { get; set; }
        bool IsRunning { get; set; }
    }
}
