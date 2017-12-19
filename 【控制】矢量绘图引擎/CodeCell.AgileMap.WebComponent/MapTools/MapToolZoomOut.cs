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
    public class MapToolZoomOut:MapToolRect
    {
        public MapToolZoomOut()
            : base()
        {
            _name = "缩小";
            _image = "/CodeCell.AgileMap.WebComponent;component/Images/cmdZoomOut.png";
        }

        protected override void HandleRectBox(Point prjLeftUp, Point prjRightBottom)
        {
            PrjRectangleF rect = new PrjRectangleF();
            rect.MinX = prjLeftUp.X;
            rect.MaxX = prjRightBottom.X;
            rect.MinY = prjRightBottom.Y;
            rect.MaxY = prjLeftUp.Y;
            //
            PrjRectangleF oldrect = _mapcontrol.Viewport;
            double panzoomFactor = Math.Min(oldrect.Width / rect.Width, oldrect.Height / rect.Height);
            double zoomWidthAmount = oldrect.Width * panzoomFactor;
            double zoomHeightAmount = oldrect.Height * panzoomFactor;
            //
            rect.Inflate(zoomWidthAmount, zoomHeightAmount);
            //
            _mapcontrol.SetViewportByPrj(rect);
        }
    }
}
