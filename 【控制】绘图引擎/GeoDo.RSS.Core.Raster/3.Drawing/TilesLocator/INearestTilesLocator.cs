using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    /// <summary>
    /// 根据当前画图的视窗计算最合适的显示级和在可视范围内的瓦片
    /// </summary>
    public interface INearestTilesLocator : IDisposable
    {
        /// <summary>
        /// [GET]根据画布(Canvas)中的瓦片大小和缩放级数定义创建的瓦片计算器
        /// </summary>
        ITileComputer TileComputer { get; }
        /// <summary>
        /// 根据当前画图的视窗计算最合适的显示级和在可视范围内的瓦片
        /// </summary>
        /// <param name="canvas">画布</param>
        /// <param name="level">返回最适合的级数</param>
        /// <param name="tiles">返回视窗内可视的瓦片</param>
        void Compute(ICanvas canvas, out LevelDef level, out TileIdentify[] tiles);
        void ComputeExtand(ICanvas canvas,LevelDef level, out TileIdentify[] tiles);
        void ComputeByLevel(ICanvas canvas, LevelDef level, out TileIdentify[] tiles);
        /// <summary>
        /// 计算画布视窗范围对应在栅格图像中的栅格坐标范围(行、列号)
        /// (raster coord by left-upper)
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="beginRow"></param>
        /// <param name="beginCol"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        void GetRasterRowColOfViewWnd(ICanvas canvas, out int beginRow, out int beginCol, out int width, out int height);
        void GetRasterRowColOfViewWnd(ICanvas canvas, out float beginRow, out float beginCol, out float width, out float height);
        /// <summary>
        /// 计算画布视窗范围中心点对应在栅格图像中的栅格坐标(行、列号)
        /// (raster coord by left-upper)
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="centerRow"></param>
        /// <param name="centerRow"></param>
        void GetCenterRowColOfViewWnd(ICanvas canvas, out int centerRow, out int centerCol);
        void UpdateTileComputer(ICanvas canvas);
    }
}
