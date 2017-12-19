using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface IMemoryBitmapLayer:IFlyLayer
    {
        void Update();
        void UpdateColorMap(object colorMap);
        object Raster { get; }
    }
}
