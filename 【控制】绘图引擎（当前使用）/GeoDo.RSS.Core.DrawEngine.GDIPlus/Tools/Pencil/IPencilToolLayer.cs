using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public interface IPencilToolLayer:IFlyLayer
    {
        enumPencilType PencilType { get; set; }
        Action<GeometryOfDrawed> PencilIsFinished { get; set; }
        Action<PointF> ControlPointIsAdded { get; set; }
        void Reset();
    }
}
