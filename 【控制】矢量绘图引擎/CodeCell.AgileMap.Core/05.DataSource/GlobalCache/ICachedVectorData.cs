using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    /// <summary>
    /// 缓存的矢量数据实例
    /// 例如：中国行政区、河流等
    /// </summary>
    public interface ICachedVectorData:IDisposable
    {
        string Identify { get; }
        enumCoordinateType CoordType { get; }
        Feature[] GetFeatures(Envelope envelope);
    }
}
