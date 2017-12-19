using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.CA
{
  public class PixelControlPointXComparer:IComparer<PixelControlPoint>
    {
        public int Compare(PixelControlPoint p1, PixelControlPoint p2)
        {
            if (p1.Location.X > p2.Location.X)
                return 1;
            else if (p1.Location.X < p2.Location.X)
                return -1;
            else return 0;
        }
    }
}
