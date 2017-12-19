using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public class CanvasProjectionArgs
    {
        public double Lon0 = double.NaN;
        public double Sp1 = double.NaN;
        public double Sp2 = double.NaN;

        public bool IsGeoCoord()
        {
            return double.IsNaN(Lon0) && double.IsNaN(Sp1) && double.IsNaN(Sp2);
        }
    }
}
