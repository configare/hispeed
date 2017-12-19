using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public interface IAVIHelper:IDisposable
    {
        Bitmap[] Bitmaps { get; }
        EventHandler OnTimerStopped { get; set; }
    }
}
