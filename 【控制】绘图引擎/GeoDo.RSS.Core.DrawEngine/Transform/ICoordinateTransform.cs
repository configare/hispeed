using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface ICoordinateTransform
    {
        object SpatialRefOfViewer { get; set; }
        enumDataCoordType DataCoordType { get; set; }
        QuickTransform QuickTransform { get; }
        void Raster2Prj(int row, int col, out double prjX, out double prjY);
        void Raster2Prj(float row, float col, out double prjX, out double prjY);
        unsafe void Raster2Prj(PointF* rasterPoint);
        void Raster2Geo(int row, int col, out double geoX, out double geoY);
        void Prj2Raster(double prjX, double prjY, out int row, out int col);
        void Screen2Prj(float screenX, float screenY, out double prjX, out double prjY);
        void Prj2Screen(double prjX, double prjY, out int screenX, out int screenY);
        void Prj2Screen(PointF[] points);
        void Screen2Raster(int screenX, int screenY, out int row, out int col);
        void Screen2Raster(float screenX, float screenY, out int row, out int col);
        void Raster2Screen(int row, int col, out int screenX, out int screenY);
        void Screen2Raster(float screenX, float screenY, out float row, out float col);
        void Raster2Screen(float row, float col, out float screenX, out float screenY);
        void Geo2Prj(double geoX, double geoY, out double prjX, out double prjY);
        void Prj2Geo(double prjX, double prjY, out double geoX, out double geoY);
        void Geo2Prj(CoordEnvelope envelope);
    }
}
