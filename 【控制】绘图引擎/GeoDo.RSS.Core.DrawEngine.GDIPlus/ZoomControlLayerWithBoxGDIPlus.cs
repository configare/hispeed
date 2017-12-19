using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public class ZoomControlLayerWithBoxGDIPlus : ZoomControlLayerWithBox
    {
        public ZoomControlLayerWithBoxGDIPlus()
            : base()
        {
        }

        public ZoomControlLayerWithBoxGDIPlus(enumZoomType zoomType)
            : base()
        {
            _zoomType = zoomType;
        }

        public override void Render(object sender, IDrawArgs drawArgs)
        {
            if (!_visible)
                return;
            if (_startLocation.IsEmpty && _endLocation.IsEmpty)
                return;
            Graphics g = drawArgs.Graphics as Graphics;
            g.DrawRectangle(Pens.SkyBlue,
                Math.Min(_startLocation.X, _endLocation.X),
                Math.Min(_startLocation.Y, _endLocation.Y),
                Math.Abs(_startLocation.X - _endLocation.X),
                Math.Abs(_startLocation.Y - _endLocation.Y));
        }
    }
}
