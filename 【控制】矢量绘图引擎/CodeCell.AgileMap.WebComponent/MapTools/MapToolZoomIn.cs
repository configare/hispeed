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
    public class MapToolZoomIn:MapToolRect
    {
        public MapToolZoomIn()
            : base()
        {
            _name = "放大";
            _image = "/CodeCell.AgileMap.WebComponent;component/Images/cmdZoomIn.png";
        }

        protected override void HandleRectBox(Point prjLeftUp, Point prjRightBottom)
        {
            PrjRectangleF viewport = new PrjRectangleF();
            viewport.MinX = prjLeftUp.X;
            viewport.MaxX = prjRightBottom.X;
            viewport.MinY = prjRightBottom.Y;
            viewport.MaxY = prjLeftUp.Y;
            _mapcontrol.SetViewportByPrj(viewport);
        }
    }
}
