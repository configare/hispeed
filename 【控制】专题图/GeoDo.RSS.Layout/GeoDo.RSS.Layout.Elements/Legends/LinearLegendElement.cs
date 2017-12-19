using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel.Composition;
using System.ComponentModel;

namespace GeoDo.RSS.Layout.Elements
{
    [Export(typeof(IElement)), Category("图例")]
    public class LinearLegendElement : LegendElementBase
    {
        protected override void Init()
        {
            _name = "渐变图例";
            _icon = ImageGetter.GetImageByName("Icon图例.png");
            _size.Width = 135;
            _size.Height = 55;
        }

        protected override System.Drawing.Bitmap GetLegendBmp(int w, int h)
        {
            if (_legendItems == null || _legendItems.Length == 0)
                return null;
            if (w <= 0 || h <= 0)
                return null;
            Bitmap bm = new Bitmap(w, h);
            using (Graphics gsp = Graphics.FromImage(bm))
            {
                gsp.Clear(_backColor);
                SizeF textSize = gsp.MeasureString(_text, _legendTextFont);
                if (!string.IsNullOrEmpty(_text))
                    gsp.DrawString(_text, _legendTextFont, Brushes.Black, (w - BORDER_BLANK * 4) / 2f, BORDER_BLANK);
                DrawLinearLegend(gsp, textSize, w, h);
            }
            return bm;
        }

        private void DrawLinearLegend(Graphics gsp, SizeF textSize, int w, int h)
        {
            PointF begin = PointF.Empty;
            if (string.IsNullOrEmpty(_text))
                begin = new PointF(0, BORDER_BLANK);
            else
                begin =new PointF(0, BORDER_BLANK + textSize.Height + _legendTextSpan);
            Color[] colors = new Color[_count];
            float[] poses = new float[_count];
            for (int i = 0; i < _count; i++)
            {
                colors[i] = _legendItems[i].Color;
                poses[i] = i * 1f / _count;
            }
            poses[_count - 1] = 1f;
            using (LinearGradientBrush brush = new LinearGradientBrush(begin, new PointF(begin.X + w, begin.Y), colors[0], Color.White))
            {
                if (_count > 1)
                {
                    ColorBlend blend = new ColorBlend(_count);
                    blend.Colors = colors.ToArray();
                    blend.Positions = poses;
                    brush.InterpolationColors = blend;
                }
                gsp.FillRectangle(brush, begin.X, begin.Y, w, _legendItemHeight);
            }
            float length = w * 1f / _count;
            float textY = textSize.Height == 0 ? _legendItemHeight + BORDER_BLANK + _legendTextSpan : _legendItemHeight + BORDER_BLANK + _legendTextSpan * 2 + textSize.Height;
            for (int i = 0; i < _count; i++)
                gsp.DrawString(_legendItems[i].Text, _legendTextFont, Brushes.Black, new PointF(length * i, textY));
        }

        protected override void ComputeItemSize(System.Drawing.Graphics g, float w, float h)
        {
            SizeF fontSize = g.MeasureString("字高", _legendTextFont);
            if (string.IsNullOrEmpty(_text))
                _legendItemHeight = (int)(h - BORDER_BLANK * 2 - _legendTextSpan - (int)fontSize.Height);
            else
                _legendItemHeight = (int)(h - BORDER_BLANK * 2 - _legendTextSpan * 2 - (int)fontSize.Height * 2);
        }
    }
}
