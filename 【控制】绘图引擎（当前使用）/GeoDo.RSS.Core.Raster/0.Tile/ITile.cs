using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface ITile<T>
    {
        TileIdentify Id { get; }
        int BandNo { get; }
        T[] Data { get; }
    }
}
