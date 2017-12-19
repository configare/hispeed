using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    public static class PercentPixelFilterFactory
    {
        public static ICandidatePixelFilter GetFilter(enumDataType dataType, int bandNo, float percent, bool is_ASC_Order)
        {
            switch (dataType)
            {
                case enumDataType.Int16:
                    return new PercentPixelFilterInt16(bandNo, percent, is_ASC_Order);
                case enumDataType.UInt16:
                    return new PercentPixelFilterUInt16(bandNo, percent, is_ASC_Order);
                case enumDataType.Byte:
                    return new PercentPixelFilterByte(bandNo, percent, is_ASC_Order);
            }
            throw new NotSupportedException("百分比候选像元过滤器不支持数据类型\"" + dataType + "\"。");
        }
    }
}
