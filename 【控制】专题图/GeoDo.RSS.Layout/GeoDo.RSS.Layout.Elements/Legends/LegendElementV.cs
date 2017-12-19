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
    /// 垂直图例
    /// </summary>
    [Export(typeof(IElement)), Category("图例")]
    public class LegendElementV : LegendElementBase
    {
        private int _legendItemSpan = 4;

        #region constractors
        public LegendElementV()
            : base()
        {
        }

        public LegendElementV(LegendItem[] items)
            : base()
        {
        }

        public LegendElementV(PointF location)
            : base()
        {
        }

        public LegendElementV(PointF location, LegendItem[] items)
            : base()
        {
        }

        protected override void Init()
        {
            _name = "图例(纵向)";
            _icon = ImageGetter.GetImageByName("Icon图例.png");
            _size.Width = 95;
            _size.Height = 80;
        }
        #endregion

        #region attributes
        [Persist(), DisplayName("图例项间距"), Category("布局")]
        public int LegendItemSpan
        {
            get { return _legendItemSpan; }
            set
            {
                if (_legendItemSpan != value)
                {
                    _legendItemSpan = value;
                    _preSize = SizeF.Empty;
                }
            }
        }
        #endregion

        #region render
        protected override Bitmap GetLegendBmp(int w, int h)
        {
            if (_legendItems == null || _count == 0)
                return null;
            if (w <= 0 || h <= 0)
                return null;
            Bitmap bm = new Bitmap(w, h);
            using (Graphics gps = Graphics.FromImage(bm))
            {
                gps.Clear(_backColor);
                gps.SmoothingMode = SmoothingMode.HighQuality;
                int i = 0;
                if (!string.IsNullOrEmpty(_text))
                {
                    using (LegendItem textItem = new LegendItem(_text))
                    {
                        DrawLegendItem(gps, textItem, i, w);
                        i++;
                    }
                }
                foreach (LegendItem it in _legendItems)
                {
                    DrawLegendItem(gps, it, i, w);
                    i++;
                }
            }
            return bm;
        }

        private void DrawLegendItem(Graphics g, LegendItem legendItem, int i, int legendWidth)
        {
            SizeF textSize = g.MeasureString(legendItem.Text, _legendTextFont);
            float y;
            if (!string.IsNullOrEmpty(_text))
            {
                if (i == 0)
                    y = BORDER_BLANK;
                else
                    y = textSize.Height + _legendTextSpan + BORDER_BLANK + (i - 1) * (_legendItemHeight + _legendItemSpan);
            }
            else
            {
                y = BORDER_BLANK + i * (_legendItemHeight + _legendItemSpan);
            }
            if (legendItem.Color.IsEmpty)
                g.DrawString(legendItem.Text, _legendTextFont, Brushes.Black, BORDER_BLANK*2, y);
            else
            {
                using (Brush brush = new SolidBrush(legendItem.Color))
                {
                    g.FillRectangle(brush, BORDER_BLANK, y, _legendItemWidth, _legendItemHeight);
                    if (_isShowBorder)
                    {
                        g.DrawRectangle(new Pen(_borderColor), BORDER_BLANK, y, _legendItemWidth, _legendItemHeight);
                    }
                }
                g.DrawString(legendItem.Text, _legendTextFont, Brushes.Black, _legendItemWidth + BORDER_BLANK + _legendTextSpan, y + Math.Abs(_legendItemHeight - textSize.Height) / 2f);
            }
        }

        protected override void ComputeItemSize(Graphics g, float w, float h)
        {
            ComputeLegendItemWid(g, (int)w);
            SizeF fontSize = g.MeasureString("字高", _legendTextFont);
            if (string.IsNullOrEmpty(_text))
                _legendItemHeight = (int)((h - BORDER_BLANK * 2 - _legendItemSpan * (_legendItems.Length - 1)) / _count);
            else
                _legendItemHeight = (int)((h - BORDER_BLANK * 2 - _legendItemSpan * (_legendItems.Length - 1) - _legendTextSpan - fontSize.Height) / _count);
            _legendItemHeight = Math.Max((int)fontSize.Height - 4, _legendItemHeight);
        }

        //计算图例中每一项的宽
        private void ComputeLegendItemWid(Graphics g, int w)
        {
            int maxWid = int.MinValue;
            foreach (LegendItem it in _legendItems)
            {
                SizeF textSize = g.MeasureString(it.Text, _legendTextFont);
                if ((int)textSize.Width > maxWid)
                    maxWid = (int)textSize.Width;
            }
            _legendItemWidth = w - 2 * BORDER_BLANK - _legendTextSpan - maxWid;
        }
        #endregion

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            string att = null;
            if (xml.Attribute("legendItemspan") != null)
            {
                att = xml.Attribute("legendItemspan").Value;
                if (!string.IsNullOrEmpty(att))
                    _legendItemSpan = int.Parse(att);
            }
            base.InitByXml(xml);
        }
    }
}
