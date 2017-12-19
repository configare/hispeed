using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public interface IElementGroup:IElement
    {
        bool IsEmpty();
        List<IElement> Elements { get; }
        IElement GetByName(string name);
        IElement FindParent(IElement element);
    }
}
