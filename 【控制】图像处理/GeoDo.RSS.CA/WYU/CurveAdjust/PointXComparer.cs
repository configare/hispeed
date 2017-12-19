using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.CA
{
  public  class PointXComparer:IComparer<Point>
    {
      public int Compare(Point p1, Point p2)
        {
            if (p1.X > p2.X)
                return 1;
            else if (p1.X < p2.X)
                return -1;
            else return 0;
        }
    }
}
