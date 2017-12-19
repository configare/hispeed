using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.Grid
{
    /// <summary>
    /// 定义网格区间
    /// </summary>
    internal class LineRegion
    {
        private double _x1;
        private double _x2;

        public LineRegion(double min, double max)
        {
            _x1 = min;
            _x2 = max;
        }

        public double Min
        {
            get { return _x1; }
        }

        public double Max
        {
            get { return _x2; }
        }
    }
}
