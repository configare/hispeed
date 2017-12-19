using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    public interface IRasterReader:IDisposable
    {
        bool IsReady { get; }
        void BeginRead();
        /// <summary>
        /// 读取指定区域的影像数据到位图
        /// </summary>
        /// <param name="envelope">矩形区域(使用物理数据的坐标单位)</param>
        /// <param name="width">宽度(像素)</param>
        /// <param name="height">高度(像素)</param>
        /// <returns>位图</returns>
        Bitmap Read(Envelope envelope, int width, int height);
        bool Read(Envelope envelope, int width, int height, ref Bitmap bitmap);
        void EndRead();
        Envelope GetFullEnvelope();
        enumCoordinateType GetCoordinateType();
        ISpatialReference GetSpatialReference();
    }
}
