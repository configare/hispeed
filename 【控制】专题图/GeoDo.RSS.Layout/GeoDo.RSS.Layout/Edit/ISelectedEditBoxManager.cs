using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public interface ISelectedEditBoxManager : IRenderable
    {
        void Update(ILayoutRuntime layoutRuntime);
        void Attach(ISizableElement element);
        ISelectedEditBox Get(ISizableElement element);
    }
}
