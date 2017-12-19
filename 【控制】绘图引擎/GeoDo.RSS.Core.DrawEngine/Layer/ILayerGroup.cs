using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface ILayerGroup:ILayer,ILayerOrderAdjustable
    {
        bool IsEmpty();
        List<ILayer> Layers { get; }
        ILayer GetByName(string name);
        ILayer Get(Func<ILayer, bool> where);
    }
}
