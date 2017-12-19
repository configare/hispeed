using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    internal abstract class MapZoomToolBase:MapPanToolBase
    {
        private bool _draging = false;
        private Point _startPoint = Point.Empty;
        private Point _endPoint = Point.Empty;

        public MapZoomToolBase()
            : base()
        { 
        }

        protected override void MouseDown(IMapControl mapcontrol, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _startPoint = e.Location;
                _draging = true;
                (mapcontrol as IMapControlDummySupprot).SetToDummyRenderMode();
            }
        }

        protected override void MouseMove(IMapControl mapcontrol, MouseEventArgs e)
        {
            if (!_draging)
                return;
            _endPoint = e.Location;
            mapcontrol.Render();
        }

        protected override void MouseUp(IMapControl mapcontrol, MouseEventArgs e)
        {
            if (!_draging)
                return;
            try
            {
                int minx = Math.Min(_startPoint.X, _endPoint.X);
                int maxx = Math.Max(_startPoint.X, _endPoint.X);
                int miny = Math.Min(_startPoint.Y, _endPoint.Y);
                int maxy = Math.Max(_startPoint.Y, _endPoint.Y);
                Point[] pts = new Point[] { new Point(minx, miny), new Point(maxx, maxy) };
                mapcontrol.CoordinateTransfrom.PixelCoord2PrjCoord(pts);
                RectangleF rect = RectangleF.FromLTRB(pts[0].X, -pts[0].Y, pts[1].X, -pts[1].Y);
                HandleZoom(mapcontrol, rect);
                (mapcontrol as IMapControlDummySupprot).ResetToNormalRenderMode();
                mapcontrol.ReRender();
            }
            finally 
            {
                _startPoint = Point.Empty;
                _endPoint = Point.Empty;
                _draging = false;
            }
        }

        protected abstract void HandleZoom(IMapControl mapcontrol, RectangleF rect);

        public override void Render(RenderArgs arg)
        {
            if (_endPoint.IsEmpty || _startPoint.IsEmpty)
                return;
            int minx = Math.Min(_startPoint.X, _endPoint.X);
            int maxx = Math.Max(_startPoint.X, _endPoint.X);
            int miny = Math.Min(_startPoint.Y, _endPoint.Y);
            int maxy = Math.Max(_startPoint.Y, _endPoint.Y);

            using (Pen p = new Pen(Color.FromArgb(128,128,128),2))
            {
                using (SolidBrush b = new SolidBrush(Color.FromArgb(64, 220, 218, 228)))
                {
                    arg.Graphics.FillRectangle(b, Rectangle.FromLTRB(minx, miny, maxx, maxy));
                    arg.Graphics.DrawRectangle(p, Rectangle.FromLTRB(minx, miny, maxx, maxy));
                }
            }
        }
    }
}
