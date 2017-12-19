using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Layout
{
    public interface ISizableElement : IElement, ICanvasEvent,IInteractiveEditable,IRotateableElement
    {
        SizeF Size { get; set; }
        PointF Location { get; set; }
    }
}
