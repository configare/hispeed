using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.CA
{
    public class PointsControlRGB : PointsControl
    {
        public PointsControlRGB()
            : base(3, 512)
        {
            mask = new bool[3] { true, false, false };
            visualColors = new Color[] { Color.Red, Color.Green, Color.Blue };
            channelNames = new string[] { "Red", "Green", "Blue" };

            ResetControlPoints();
        }
    }
}
