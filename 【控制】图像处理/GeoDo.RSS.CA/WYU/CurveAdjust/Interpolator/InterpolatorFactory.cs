using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.CA
{
    public class InterpolatorFactory
    {
        public InterpolatorFactory()
        { 
        }

        public static IInterpolator GetInterpolator(enumInterpolatorType type)
        {
            switch (type)
            { 
                case enumInterpolatorType.Line:
                    return new LineInterpolator();
                case enumInterpolatorType.Curve:
                    return new SplineInterpolator();
                case enumInterpolatorType.Log:
                    return new LogInterpolator();
                default:
                    return null;
            }
        }
    }
}
