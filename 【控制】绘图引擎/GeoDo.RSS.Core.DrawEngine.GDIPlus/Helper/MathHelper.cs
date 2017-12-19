using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public static class MathHelper
    {
        public static bool MatrixIsSame(Matrix a, Matrix b)
        {
            for (int i = 0; i < a.Elements.Length; i++)
            {
                if (Math.Abs(a.Elements[i] - b.Elements[i]) > float.Epsilon)
                    return false;
            }
            return true;
        }
    }
}
