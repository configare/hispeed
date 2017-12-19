using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    public interface ICoordinateTransform
    {
        void GeoCoord2PrjCoord(ShapePoint[] points);
        PointF[] GeoCoord2PixelCoord(ShapePoint[] points);
        PointF[] PrjCoord2PixelCoord(ShapePoint[] points);
        void PrjCoord2GeoCoord(ShapePoint[] points);
        void GeoCoord2PixelCoord(PointF[] points);
        void PrjCoord2GeoCoord(PointF[] points);
        void PrjCoord2GeoCoord(ref PointF pointf);
        void PrjCoord2PixelCoord(PointF[] points);
        void PixelCoord2PrjCoord(Point[] points);
        void PixelCoord2PrjCoord(ref Point point);
        double GetPixelTolerance(int pixel);
    }
}
