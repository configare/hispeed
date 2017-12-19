using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 二值位图构造器
    /// </summary>
    public interface IBinaryBitmapBuilder:IDisposable
    {
        /// <summary>
        /// 创建空的二值位图
        /// </summary>
        /// <param name="size">位图大小</param>
        /// <param name="trueColor">真值颜色</param>
        /// <param name="falseColor">假值颜色</param>
        /// <returns>构造成功的位图</returns>
        Bitmap CreateBinaryBitmap(Size size,Color trueColor,Color falseColor);
        /// <summary>
        /// 根据二值图真值索引数组填充位图
        /// </summary>
        /// <param name="indexes">真值索引数组</param>
        /// <param name="size">二值图大小</param>
        /// <param name="bitmap">待填充的位图(若大小和二值图大小不同,则执行采样)</param>
        void Fill(int[] indexes, Size size, ref Bitmap bitmap);
        /// <summary>
        /// 清空图像
        /// </summary>
        void Reset(System.Drawing.Size size, ref System.Drawing.Bitmap bitmap);
    }
}
