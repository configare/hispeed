using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    public interface IBitmap2RasterConverter:IDisposable
    {
        /// <summary>
        /// 返回多值图(完全矩阵)
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="colors"></param>
        /// <returns></returns>
        byte[] ToRaster(Bitmap bitmap, Color[] colors);
        /// <summary>
        /// 返回二值图(稀疏矩阵)
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="interestedColor"></param>
        /// <returns></returns>
        int[] ToRaster(Bitmap bitmap,Color interestedColor);
    }
}
