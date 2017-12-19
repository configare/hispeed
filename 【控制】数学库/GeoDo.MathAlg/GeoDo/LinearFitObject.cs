using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.MathAlg
{
    /// <summary>
    /// y = a + bx
    /// R^2:线性相关系数
    /// </summary>
    public class LinearFitObject:CurveFitObject
    {
        public double a;
        public double b;
        public double r2;
    }
}
