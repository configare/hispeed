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
using System.Threading;

namespace CodeCell.AgileMap.WebComponent
{
    public class MapToolWheel:MapTool
    {
        private float panzoomFactor = 0.12f;
        private int _lastticks = 0;
        private const int cstMinInterval = 80;

        public MapToolWheel()
            : base()
        { 
        }

     
        public override void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Environment.TickCount - _lastticks < cstMinInterval)
            {
                _lastticks = Environment.TickCount;
                return;
            }
            if (e.Delta < 0)
                ZoomOut();
            else
                ZoomIn();
            _lastticks = Environment.TickCount;
        }

        public void ZoomIn()
        {
            PrjRectangleF _viewport = _mapcontrol.Viewport;
            double zoomWidthAmount = -_viewport.Width * panzoomFactor;
            double zoomHeightAmount = -_viewport.Height * panzoomFactor;
            //
            PrjRectangleF newViewport = _viewport;
            newViewport.Inflate(zoomWidthAmount, zoomHeightAmount);
            //
            _mapcontrol.SetViewportByPrj(newViewport);
        }

        public void ZoomOut()
        {
            PrjRectangleF _viewport = _mapcontrol.Viewport;
            double zoomWidthAmount = _viewport.Width * panzoomFactor;
            double zoomHeightAmount = _viewport.Height * panzoomFactor;
            PrjRectangleF newViewport = _viewport;
            newViewport.Inflate(zoomWidthAmount, zoomHeightAmount);
            //
            _mapcontrol.SetViewportByPrj(newViewport);
        }
    }
}
