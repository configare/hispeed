using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.Bricks.Serial
{
    public static class ColorHelper
    {
        public static string ColorToString(Color color)
        {
            return color.A.ToString() + "," + color.R.ToString() + "," + color.G.ToString() + "," + color.B.ToString();
        }

        public static Color StringToColor(string str)
        {
            if (string.IsNullOrEmpty(str))
                return Color.Transparent;
            string[] ps = str.Split(',');
            if (ps.Length != 4)
                return Color.Transparent;
            try
            {
                return Color.FromArgb(int.Parse(ps[0]), int.Parse(ps[1]), int.Parse(ps[2]), int.Parse(ps[3]));
            }
            catch 
            {
                return Color.Transparent;
            }
        }
    }
}
