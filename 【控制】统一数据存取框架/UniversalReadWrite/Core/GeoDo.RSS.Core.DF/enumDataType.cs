using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public enum enumDataType
    {
        /// <summary>
        /// 压缩格式
        /// </summary>
        Bits,
        Byte,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Int64,
        UInt64,
        Float,
        Double,
        /// <summary>
        /// 如果数据集各个波段的数据类型不一致则返回该值
        /// </summary>
        Atypism,
        Unknow
    }
}
