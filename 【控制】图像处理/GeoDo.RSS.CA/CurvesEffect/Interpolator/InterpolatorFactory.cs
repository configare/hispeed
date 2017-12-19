using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.CA
{
    public class InterpolatorFactory
    {
        public static IInterpolator GetInterpolator(string interpolatorType)
        {
            IInterpolator interpolator;
            if (interpolatorType == "曲线")
            {
                interpolator = new SplineInterpolator();
            }
            else
            {
                interpolator = new LineInterpolator();
            }
            return interpolator;
        }
    }
}
