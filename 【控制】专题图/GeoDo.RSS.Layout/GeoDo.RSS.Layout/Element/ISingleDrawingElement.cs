using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Layout
{
   public interface ISingleDrawingElement :IElement,ISizableElement
    {
       Color Color { get; set; }
       bool IsShowBorder { get; set; }
    }
}
