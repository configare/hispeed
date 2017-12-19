using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    /// <summary>
    /// 反转视图下的坐标转换器
    /// </summary>
    public interface IReversedCoordinateTransform
    {
        void Screen2Prj(float screenX, float screenY, out double prjX, out double prjY);
        void Prj2Screen(double prjX, double prjY, out int screenX, out int screenY);
        void Screen2Raster(int screenX, int screenY, out int row, out int col);
        void Screen2Raster(float screenX, float screenY, out int row, out int col);
        void Raster2Screen(int row, int col, out int screenX, out int screenY);
        void Screen2Raster(float screenX, float screenY, out float row, out float col);
        void Raster2Screen(float row, float col, out float screenX, out float screenY);
    }
}
