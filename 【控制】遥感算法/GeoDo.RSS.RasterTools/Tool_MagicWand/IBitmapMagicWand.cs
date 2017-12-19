using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

namespace GeoDo.RSS.RasterTools
{
    /// <summary>
    /// 魔术棒工具
    /// </summary>
    public interface IBitmapMagicWand : IDisposable
    {
        /// <summary>
        /// 魔术棒颜色替换
        /// </summary>
        /// <param name="pData">位图</param>
        /// <param name="startPoint">种子点,通过种子点拾取源颜色</param>
        /// <param name="tolerance">容差,取值范围[0~255]</param>
        /// <param name="isContinued">是否连续,TRUE:只处理一个连续区域</param>
        /// <param name="targetColor">目标颜色</param>
        void FillColor(BitmapData pData, Point startPoint, byte tolerance, bool isContinued, byte[] targetColor);
        /// <summary>
        /// 提取魔术棒提取像素
        /// </summary>
        /// <param name="pData">位图</param>
        /// <param name="startPoint">种子点,通过种子点拾取源颜色</param>
        /// <param name="tolerance">容差,取值范围[0~255]</param>
        /// <param name="isContinued">是否连续,TRUE:只处理一个连续区域</param>
        /// <returns>选中行段(row,beginCol,endCol)集合</returns>
        ScanLineSegment[] ExtractSnappedPixels(BitmapData pData,Point startPoint,byte tolerance,bool isContinued);
        /// <summary>
        /// 魔术棒提取矢量轮廓
        /// </summary>
        /// <param name="pData">位图</param>
        /// <param name="startPoint">种子点,通过种子点拾取源颜色</param>
        /// <param name="tolerance">容差,取值范围[0~255]</param>
        /// <param name="isContinued">是否连续,TRUE:只处理一个连续区域</param>
        /// <returns>提取的矢量轮廓</returns>
        ScanLinePolygon[] ExtractSnappedPolygons(BitmapData pData, Point startPoint, byte tolerance, bool isContinued);
    }
}
