using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public class MathCompare
    {
        public static bool FloatCompare(float bef, float last)
        {
            if (Math.Abs(bef / last) > 1 - Math.Pow(10, -6))
                return true;
            return false;
        }
    }
}
