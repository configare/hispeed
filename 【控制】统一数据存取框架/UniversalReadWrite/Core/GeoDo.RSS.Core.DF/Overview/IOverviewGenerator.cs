using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DF
{
    public interface IOverviewGenerator
    {
        /// <summary>
        /// 生成缩略图(使用配置好的拉伸器,from DataStretchers.xml)
        /// 注意:bitmap.Size一定要和DataProvider.Size的长宽比一致
        /// </summary>
        /// <param name="bandNos">RGB合成波段(Length = 1 or 3)</param>
        /// <param name="bitmap">生成的缩略图</param>
        void Generate(int[] bandNos,ref Bitmap bitmap);
        void Generate(int[] bandNos, object[] stretchers,ref Bitmap bitmap);
        /// <summary>
        /// 约定长宽比、最大尺寸计算实际尺寸
        /// </summary>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        Size ComputeSize(int maxSize);
    }
}
