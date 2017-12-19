using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface IMemoryRaster<T>:IDisposable
    {
        bool IsGeoCoordinate { get; }
        string Identify { get; }
        T[] Data { get; }
        Size Size { get; }
        int SizeOfDataType { get; }
        CoordEnvelope Envelope { get; }
    }
}
