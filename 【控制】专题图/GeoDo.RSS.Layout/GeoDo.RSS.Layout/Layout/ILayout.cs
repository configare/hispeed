using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Layout
{
    public interface ILayout:IElement,IElementGroup
    {        
        enumLayoutUnit Unit { get; }
        SizeF Size { get; set; }
        IBorder GetBorder();
        IElement[] QueryElements(Func<IElement, bool> filter);
    }
}
