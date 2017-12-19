using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    internal static class DrawedGeometry2Polygon
    {
        public static CodeCell.AgileMap.Core.Shape ToPolygon(ICanvas canvas, GeometryOfDrawed geometry)
        {
            if (canvas == null || geometry == null || geometry.RasterPoints == null || geometry.RasterPoints.Length == 0)
                return null;
            int count = geometry.RasterPoints.Length;
            List<ShapePoint> pts = new List<ShapePoint>(count);
            double geoX = 0, geoY = 0;
            for (int i = 0; i < count; i++)
            {
                canvas.CoordTransform.Raster2Geo((int)geometry.RasterPoints[i].Y, (int)geometry.RasterPoints[i].X, out geoX, out geoY);
                pts.Add(new ShapePoint(geoX, geoY));
            }
            ShapeRing ring = new ShapeRing(pts.ToArray());
            ShapePolygon ply = new ShapePolygon(new ShapeRing[] { ring });
            return ply;
        }
    }
}
