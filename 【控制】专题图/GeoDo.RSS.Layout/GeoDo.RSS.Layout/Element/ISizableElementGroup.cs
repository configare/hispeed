using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public interface ISizableElementGroup:IElementGroup,ISizableElement
    {
        void ApplyLocationByItemSelected(float layoutOffsetX, float layoutOffsetY);
    }
}
