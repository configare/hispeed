using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Layout
{
    public interface ISelectedEditBox : IRenderable
    {
        int IndexOfAnchor(float screenX, float screenY);
        void Apply(ILayoutRuntime runtime, int anchorIndex, PointF bPoint, PointF ePoint);
    }
}
