using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CodeCell.AgileMap.WebComponent
{
    public static class GeometryFactory
    {
        public static Shape FromWkt(string wkt)
        {
            if (wkt == null)
                return null;
            if (wkt.Contains("POINT"))
                return PointFromWKT(wkt);
            else if (wkt.Contains("MULTILINESTRING"))
                return PolylineFromWKT(wkt);
            else if (wkt.Contains("POLYGON"))
                return PolygonFromWKT(wkt);
            else
                throw new NotSupportedException("不支持的几何类型或者错误的OGC WKT表达式," + wkt + "。");
        }

        private static Shape PointFromWKT(string wkt)
        {
            if (wkt == null)
                return null;
            wkt = wkt.Replace("POINT(", string.Empty).Replace(")", string.Empty);
            string[] vs = wkt.Split(' ');
            //return new Ellipse(double.Parse(vs[0]), double.Parse(vs[1]));
            return null;
        }

        private static Shape PolylineFromWKT(string wkt)
        {
            //string owkt = wkt;
            //if (wkt == null)
            //    return null;
            //wkt = wkt.Replace("MULTILINESTRING(", string.Empty);
            //wkt = wkt.Substring(0, wkt.Length - 1);//remove )
            //string[] parts1 = wkt.Split(new string[] { "),(" }, StringSplitOptions.RemoveEmptyEntries);
            //if (parts1 == null || parts1.Length == 0)
            //    return null;
            //if (parts1.Length == 1)
            //{
            //    parts1[0] = parts1[0].Substring(1, parts1[0].Length - 2);
            //}
            //else
            //{
            //    int idx0 = 0;
            //    int idxn = parts1.Length - 1;
            //    parts1[idx0] = parts1[idx0].Substring(1, parts1[idx0].Length - 1);
            //    if (parts1.Length > 1)
            //        parts1[idxn] = parts1[idxn].Substring(0, parts1[idxn].Length - 1);
            //}
            //Polyline ply = new Polyline();
            //List<ShapeLineString> part = new List<ShapeLineString>(parts1.Length);
            //foreach (string partstring in parts1)
            //{
            //    string[] parts2 = partstring.Split(',');
            //    List<ShapePoint> pts = new List<ShapePoint>(parts2.Length);
            //    foreach (string partpt in parts2)
            //    {
            //        string[] xy = partpt.Split(' ');//bank
            //        pts.Add(new ShapePoint(double.Parse(xy[0]), double.Parse(xy[1])));
            //    }
            //    part.Add(new ShapeLineString(pts.ToArray()));
            //}
            //return new ShapePolyline(part.ToArray());
            return null;
        }

        private static Shape PolygonFromWKT(string wkt)
        {
            throw new NotImplementedException();
        }
    }
}
