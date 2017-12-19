using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.Layout.Elements
{
    /// <summary>
    /// 水平图例
    /// </summary>
    [Export(typeof(IElement)), Category("图例")]
    public class LegendELementH : LegendElementBase
    {
        #region constractors
        public LegendELementH()
            : base()
        {
        }

        public LegendELementH(LegendItem[] items)
            : base()
        {
        }

        public LegendELementH(PointF location)
            : base()
        {
        }

        public LegendELementH(PointF location, LegendItem[] items)
            : base()
        {
        }

        protected override void Init()
        {
            _name = "图例(横向)";
            _icon = ImageGetter.GetImageByName("Icon图例.png");
            _size.Width = 135;
            _size.Height = 55;
        }
        #endregion

        #region render
        protected override Bitmap GetLegendBmp(int w, int h)
        {
            if (_legendItems == null || _legendItems.Length == 0)
                return null;
            if (w <= 0 || h <= 0)
                return null;
            Bitmap bm = new Bitmap(w, h);
            using (Graphics gsp = Graphics.FromImage(bm))
            {
                gsp.Clear(_backColor);
                gsp.SmoothingMode = SmoothingMode.HighQuality;
                int i = 0;
                if (!string.IsNullOrEmpty(_text))
                {
                    using (LegendItem textItem = new LegendItem(_text))
                    {
                        DrawLegendItem(gsp, textItem, i, _legendItemHeight);
                    }
                }
                foreach (LegendItem it in _legendItems)
                {
                    DrawLegendItem(gsp, it, i, _legendItemHeight);
                    i++;
                }
            }
            return bm;
        }

        protected override void ComputeItemSize(System.Drawing.Graphics g, float w, float h)
        {
            _legendItemWidth = (int)((w - BORDER_BLANK * 4) / _legendItems.Length);
            SizeF fontSize = g.MeasureString("字高", _legendTextFont);
            if (string.IsNullOrEmpty(_text))
                _legendItemHeight = (int)(h - BORDER_BLANK * 2 - _legendTextSpan - (int)fontSize.Height);
            else
                _legendItemHeight = (int)(h - BORDER_BLANK * 2 - _legendTextSpan * 2 - (int)fontSize.Height * 2);
        }

        private void DrawLegendItem(Graphics g, LegendItem legendItem, int i, int legendWidth)
        {
            int x = i * _legendItemWidth + BORDER_BLANK * 2;
            SizeF textSize = g.MeasureString(_text, _legendTextFont);
            if (legendItem.Color.IsEmpty)
            {
                g.DrawString(_text, _legendTextFont, Brushes.Black, x + BORDER_BLANK, BORDER_BLANK);
            }
            else
            {
                float y = textSize.Height == 0 ?BORDER_BLANK: BORDER_BLANK + textSize.Height + _legendTextSpan ;
                using (Brush brush = new SolidBrush(legendItem.Color))
                {
                    if (brush == null)
                        return;
                    g.FillRectangle(brush, x, y, _legendItemWidth, _legendItemHeight);
                    if (_isShowBorder)
                    {
                        g.DrawRectangle(new Pen(_borderColor), x, y, _legendItemWidth, _legendItemHeight);
                    }
                }
                float textY = textSize.Height == 0 ? _legendItemHeight + BORDER_BLANK + _legendTextSpan : _legendItemHeight + BORDER_BLANK + _legendTextSpan * 2 + textSize.Height;
                g.DrawString(legendItem.Text, _legendTextFont, Brushes.Black, x - BORDER_BLANK, textY);
            }
        }
        #endregion render
    }
}
