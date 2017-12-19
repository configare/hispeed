using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface IColorMapTableGetter
    {
        object Stretcher { get; }
        ColorMapTable<int> ColorTable { get; }
    }
}
