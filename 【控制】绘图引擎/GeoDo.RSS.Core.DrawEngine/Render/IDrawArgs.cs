using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface IDrawArgs:IDisposable
    {
        QuickTransform QuickTransformArgs { get; }
        object Graphics { get; }
        void Reset(object graphics);
    }
}
