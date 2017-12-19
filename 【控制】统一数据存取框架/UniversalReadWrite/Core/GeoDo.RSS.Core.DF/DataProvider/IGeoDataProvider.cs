using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;

namespace GeoDo.RSS.Core.DF
{
    public interface IGeoDataProvider:IDisposable
    {
        /// <summary>
        /// [GET]数据源
        /// </summary>
        string fileName { get; }
        /// <summary>
        /// [GET]驱动
        /// </summary>
        IGeoDataDriver Driver { get; }
        /// <summary>
        /// [GET]坐标类型
        /// </summary>
        enumCoordType CoordType { get; }
        /// <summary>
        /// [GET]空间参考(如果坐标类型为Raster，则返回null）
        /// </summary>
        ISpatialReference SpatialRef { get; }
    }
}
