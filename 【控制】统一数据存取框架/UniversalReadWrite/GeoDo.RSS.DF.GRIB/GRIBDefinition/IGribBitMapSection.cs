using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// Grib位图段接口
    /// </summary>
    public interface IGribBitMapSection
    {
        int SectionLength { get; }
        bool[] Bitmap { get; }
    }
}
