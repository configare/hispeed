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
    public class LegendElementVFixItemHeight : LegendElementBase
    {
        private int _legendItemSpan = 4;
        private bool _isTopToBottom = false;
        private float _topY = 0f;

        #region constractors
        public LegendElementVFixItemHeight()
            : base()
        {
        }

        public LegendElementVFixItemHeight(LegendItem[] items)
            : base()
        {
        }

        public LegendElementVFixItemHeight(PointF location)
            : base()
        {
        }

        public LegendElementVFixItemHeight(PointF location, LegendItem[] items)
            : base()
        {
        }

        protected override void Init()
        {
            _name = "图例(纵向,指定图例项高度)";
            _icon = ImageGetter.GetImageByName("Icon图例.png");
            _size.Width = 95;
            _size.Height = 80;
            _legendItemHeight = 18;
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

        [Persist(), DisplayName("图例项高度"), Category("布局")]
        public int LegendItemHeight
        {
            get { return _legendItemHeight; }
            set
            {
                if (_legendItemHeight != value)
                {
                    _legendItemHeight = value;
                    _preSize = SizeF.Empty;
                }
            }
        }

        [Persist(), DisplayName("图例项是否顶部对齐"), Category("布局")]
        public bool IsTopToBottom
        {
            get { return _isTopToBottom; }
            set
            {
                if (_isTopToBottom != value)
                {
                    _isTopToBottom = value;
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
                if (_isTopToBottom)
                    _topY = 0;
                else
                {
                    SizeF textSize = gps.MeasureString(_text, _legendTextFont);
                    _topY = h - (_legendItems.Length * (_legendItemHeight + _legendItemSpan) + textSize.Height);
                }
                int i = 0;
                if (!string.IsNullOrEmpty(_text))   //图例标题
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
            float textY;
            if (!string.IsNullOrEmpty(_text))
            {
                if (i == 0)
                    y = BORDER_BLANK + _topY;
                else
                    y = textSize.Height + _legendTextSpan + BORDER_BLANK + (i - 1) * (_legendItemHeight + _legendItemSpan) + _topY;
            }
            else
            {
                y = (BORDER_BLANK + i * (_legendItemHeight + _legendItemSpan)) + _topY;
            }
            if (legendItem.Color.IsEmpty)
                g.DrawString(legendItem.Text, _legendTextFont, Brushes.Black, BORDER_BLANK * 2, y);
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
                textY = (_legendItemHeight < textSize.Height ? y : y + Math.Abs(_legendItemHeight - textSize.Height) / 2f);
                g.DrawString(legendItem.Text, _legendTextFont, Brushes.Black, _legendItemWidth + BORDER_BLANK + _legendTextSpan, textY);
            }
        }

        protected override void ComputeItemSize(Graphics g, float w, float h)
        {
            ComputeLegendItemWidth(g, (int)w);
            SizeF fontSize = g.MeasureString("字高", _legendTextFont);
        }

        //计算图例项的宽
        private void ComputeLegendItemWidth(Graphics g, int w)
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
            if (xml.Attribute("legenditemheight") != null)
            {
                att = xml.Attribute("legenditemheight").Value;
                if (!string.IsNullOrEmpty(att))
                    _legendItemHeight = int.Parse(att);
            }
            base.InitByXml(xml);
        }
    }
}
