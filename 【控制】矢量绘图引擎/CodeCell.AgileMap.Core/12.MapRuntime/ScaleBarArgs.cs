using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    public class ScaleBarArgs:IDisposable
    {
        public bool Enabled = true;
        public int BarWidth = 160;
        public int BarHeight = 8;
        public Brush FontBrush = new SolidBrush(Color.Black);
        public Brush MaskBrush = new SolidBrush(Color.White);
        public Font Font = new Font("微软雅黑", 9);

        public ScaleBarArgs()
        { 
        }

        #region IDisposable Members

        public void Dispose()
        {
            if(FontBrush != null)
                FontBrush.Dispose();
            if(MaskBrush != null)
                MaskBrush.Dispose();
            if (Font != null)
                Font.Dispose();
        }

        #endregion
    }
}
