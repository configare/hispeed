using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine
{
    /// <summary>
    /// 画布缩放时按照主显示对象(一般为影像)为参照进行
    /// 画布缩放的最大比例和最小比例实际上是参照主显示对象的
    /// </summary>
    public interface IPrimaryDrawObject
    {
        string SpatialRef { get; }
        Size Size { get; }
        CoordEnvelope OriginalEnvelope { get; }
        /// <summary>
        /// 数据原始分辨率
        /// 对于栅格坐标是：像素单位，是常数“1”
        /// 对于地理坐标是：投影后以米为单位的分辨率
        /// 多疑投影坐标是：以米为单位的分辨率
        /// </summary>
        float OriginalResolutionX { get; }
        /// <summary>
        /// 当前图像的缩放比
        /// </summary>
        float Scale { get;  }
        void Screen2Raster(float screenX, float screenY, out int row, out int col);
        void Screen2Raster(int screenX, int screenY, out int row, out int col);
        void Raster2Screen(int row, int col, out int screenX, out int screenY);
        void Screen2Raster(float screenX, float screenY, out float row, out float col);
        void Raster2Screen(float row, float col, out float screenX, out float screenY);
        void Geo2Prj(double geoX, double geoY, out double prjX, out double prjY);
        void Prj2Geo(double prjX, double prjY, out double geoX, out double geoY);
        void Raster2Geo(int row, int col, out double geoX, out double geoY);
        void Raster2Prj(int row, int col, out double prjX, out double prjY);
        void Raster2Prj(float row, float col, out double prjX, out double prjY);
        unsafe void Raster2Prj(PointF* rasterPoint);
        void Prj2Raster(double prjX, double prjY, out int row, out int col);
        //
        bool IsRasterCoord { get; }
    }
}
