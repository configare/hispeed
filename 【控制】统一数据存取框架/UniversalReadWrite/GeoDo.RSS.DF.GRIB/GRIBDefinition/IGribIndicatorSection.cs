using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// Grib指示段接口
    /// </summary>
    public interface IGribIndicatorSection
    {
        /// <summary>
        /// 版本号
        /// </summary>
        int GribEdition { get; }

        /// <summary>
        /// Grib记录长度（总长度）
        /// </summary>
        long GribLength { get; }
    }
}
