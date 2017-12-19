using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.CA
{
    public interface IControlPointInterpolator:IInterpolator
    {
        Point[] ControlPoints { get; }
        void UpdateControlPoints(Point[] controlPoints);
    }
}
