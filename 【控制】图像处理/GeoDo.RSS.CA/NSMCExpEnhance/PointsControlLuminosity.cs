using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.CA
{
    public class PointsControlLuminosity : PointsControl
    {
        public PointsControlLuminosity()
            : base(1, 512)
        {
            mask = new bool[1] { true};
            visualColors = new Color[] { Color.Black};
            channelNames = new string[] { "Luminosity"};

            ResetControlPoints();
        }
    }
}
