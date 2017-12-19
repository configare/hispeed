using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public interface IDrawArgs
    {
        object Graphics { get; }
        void Reset(object graphics);
        ILayoutRuntime Runtime { get; }
    }
}
