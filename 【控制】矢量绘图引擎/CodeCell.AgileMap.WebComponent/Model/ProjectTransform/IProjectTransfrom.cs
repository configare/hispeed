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
    public interface IProjectTransfrom
    {
        Point Prj2Geo(Point prjPt);
        Point[] Prj2Geo(Point[] prjPts);
        Point Geo2Prj(Point geoPt);
        Point[] Geo2Prj(Point[] geoPts);
        void Prj2Geo(GeoPoint geoPt);
        void Prj2Geo(GeoPoint[] geoPts);
        void Geo2Prj(GeoPoint prjPt);
        void Geo2Prj(GeoPoint[] prjPts);
    }
}
