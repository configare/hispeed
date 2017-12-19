using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.Bricks.Serial
{
    public static class FontHelper
    {
        public static string FontToString(Font font)
        {
            return font.Name + "," + font.Size.ToString() + "," + font.Style.ToString();
        }

        public static Font StringToFont(string str)
        {
            if (string.IsNullOrEmpty(str))
                return new Font("宋体", 9);
            string[] ps = str.Split(',');
            if(ps.Length !=3)
                return new Font("宋体", 9);
            try
            {
                FontStyle fs = FontStyle.Regular;
                foreach (FontStyle f in Enum.GetValues(typeof(FontStyle)))
                {
                    if (f.ToString() == ps[2])
                    {
                        fs = f;
                    }
                }
                return new Font(ps[0], float.Parse(ps[1]), fs);
            }
            catch
            {
                return new Font("宋体", 9);
            }
        }
    }
}
