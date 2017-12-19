using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    internal class OprChangeExtent:OprBudGIS
    {
        private RectangleF _oldExtent = RectangleF.Empty;
        private RectangleF _newExtent = RectangleF.Empty;

        public OprChangeExtent(IMapControl mapControl,RectangleF newExtent)
            : base(mapControl)
        {
            _oldExtent = mapControl.ExtentPrj;
            _newExtent = newExtent;
        }

        public OprChangeExtent(IMapControl mapControl,RectangleF oldExtent, RectangleF newExtent)
            : base(mapControl)
        {
            _oldExtent = oldExtent;
            _newExtent = newExtent;
        }

        public override void Do()
        {
            _mapControl.ExtentPrj = _newExtent;
            _mapControl.ReRender();
        }

        public override void Undo()
        {
            _mapControl.ExtentPrj = _oldExtent;
            _mapControl.ReRender();
        }
    }
}
