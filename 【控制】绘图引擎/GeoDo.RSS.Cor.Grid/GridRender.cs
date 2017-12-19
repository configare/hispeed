using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.Core.Grid
{
    public class GridRender : IDisposable
    {
        protected float _span = 1f;
        protected int _labelSpan = 0; //每个网格都标注
        protected Pen _pen = null;
        protected Font _font = null;
        protected Brush _fontBrush = null;
        protected bool _isLabeling = true;
        protected Color _maskColor = Color.White;
        protected bool _enabledMaskColor = false;
        protected bool _isSimpleLableString = false;
        protected bool _isExtandLabling = true;            //超过180是否还要标注
        protected int _offsetXLabelPoint = 0;
        protected int _offsetYLabelPoint = 0;
        protected int _offsetTopwidth = 0;
        protected int _offsetRightwidth = 0;
        protected int _offsetLeftWidth2 = 0;
        protected int _offsetBottomWidth2 = 0;
        protected int _offsetRightWidth2 = 0;
        protected int _offsetTopWidth2 = 0;
        protected bool _longitudeVertical = false;
        protected bool _latitudeVertical = true;
        protected float _preLeftPixel = float.NaN;
        public const int cstMinPixelWidth = 20;

        public GridRender()
        {
            _font = new Font("宋体", 9);
            _fontBrush = new SolidBrush(Color.Yellow);
        }

        public Color LabelColor
        {
            set 
            {
                if (_fontBrush == null)
                    _fontBrush = new SolidBrush(value);
                else
                    (_fontBrush as SolidBrush).Color = value;
            }
        }

        public Font LabelFont
        {
            set { _font = value; }
        }

        public string GetLonString(double x)
        {
            string strX = null;
            double absx = Math.Abs(x);
            if (absx > (int)absx)
                absx = Math.Round(absx, 1);
            if (x < 0)
            {
                strX = absx.ToString() + "°W";
            }
            else if (x > 0)
            {
                strX = absx.ToString() + "°E";
            }
            else
            {
                strX = absx.ToString() + "°";
            }
            return strX;
        }

        public string GetLatString(double y)
        {
            string strY = null;
            double absy = Math.Abs(y);
            if (absy > (int)absy)
                absy = Math.Round(absy, 1);
            if (y < 0)
            {
                strY = absy.ToString() + "°S";
            }
            else if (y > 0)
            {
                strY = absy.ToString() + "°N";
            }
            else
            {
                strY = absy.ToString() + "°";
            }
            return strY;
        }

        public void DrawString(Graphics g, double lon, double lat, float x, float y, bool is_lon)
        {
            string textX;
            string textY;
            textX = GetLonString(lon);
            textY = GetLatString(lat);
            if (is_lon)
                g.DrawString(textX, _font, _fontBrush, x + 1, y + 1);
            else
                g.DrawString(textY, _font, _fontBrush, x + 1, y - 1);
        }

        public void Dispose()
        {
            if (_pen != null)
            {
                _pen.Dispose();
                _pen = null;
            }
            if (_font != null)
            {
                _font.Dispose();
                _font = null;
            }
            if (_fontBrush != null)
            {
                _fontBrush.Dispose();
                _fontBrush = null;
            }
        }
    }
}
