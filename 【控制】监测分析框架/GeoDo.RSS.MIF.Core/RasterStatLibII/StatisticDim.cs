using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 统计维度
    /// 例如：行政区划、土地利用类型等
    /// </summary>
    public class StatisticDim
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 像元类型
        /// 例如：林地、耕地、沙漠等
        /// </summary>
        public string[] PixelTypes;
        /// <summary>
        /// 像元类型索引(与像元类型一致)
        /// 例如：林地=0
        ///       耕地=1
        ///       沙漠=2 
        /// </summary>
        public byte[] PixelTypeIndexes;
    }
}
