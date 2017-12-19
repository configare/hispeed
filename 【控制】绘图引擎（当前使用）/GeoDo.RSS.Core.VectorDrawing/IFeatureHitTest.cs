using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using System.Drawing;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public interface IFeatureHitTest
    {
        Feature HitTest(PointF screenPoint);
        Feature[] HitTest(RectangleF screenRect);
    }
}
