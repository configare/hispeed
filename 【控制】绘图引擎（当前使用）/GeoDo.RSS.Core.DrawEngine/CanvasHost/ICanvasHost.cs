using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface ICanvasHost
    {
        ICanvas Canvas { get; }
        void DrawToBitmap(Bitmap buffer, Rectangle rect);
    }
}
