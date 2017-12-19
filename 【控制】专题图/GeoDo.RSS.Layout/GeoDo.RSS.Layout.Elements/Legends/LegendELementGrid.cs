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
    /// 图例
    /// </summary>
    [Export(typeof(IElement)), Category("图例")]
    public class LegendELementGrid : LegendElementBase
    {
        private int _legendItemSpan = 4;
        private int _colCount = 2;
        private int _maxTextWidth = 0;
        private bool _isShowColor = true;
        private SizeF _titleSize = SizeF.Empty;

        #region constractors
        public LegendELementGrid()
            : base()
        {
        }

        public LegendELementGrid(LegendItem[] items)
            : base()
        {
        }

        public LegendELementGrid(PointF location)
            : base()
        {
        }

        public LegendELementGrid(PointF location, LegendItem[] items)
            : base()
        {
        }

        protected override void Init()
        {
            _name = "图例(网格)";
            _icon = ImageGetter.GetImageByName("Icon图例.png");
            _size.Width = 120;
            _size.Height = 60;
            _colCount = 2;
        }
        #endregion

        [Persist(), DisplayName("图例项列数"), Category("布局")]
        public int LineCount
        {
            get { return _colCount; }
            set
            {
                _colCount = value;
                _preSize = SizeF.Empty;
            }
        }

        [Persist(), DisplayName("是否显示图例块"), Category("布局")]
        public bool IsShowColor
        {
            get { return _isShowColor; }
            set
            {
                _isShowColor = value;
                _preSize = SizeF.Empty;
            }
        }

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
            using (Graphics g = Graphics.FromImage(bm))
            {
                g.Clear(_backColor);
                g.SmoothingMode = SmoothingMode.HighQuality;
                DrawLegendTitle(g);
                int i = 0;
                foreach (LegendItem it in _legendItems)
                {
                    DrawLegendItem(g, it, i, w);
                    i++;
                }
            }
            return bm;
        }
        
        private void DrawLegendTitle(Graphics g)
        {
            if (string.IsNullOrWhiteSpace(_text))
            {
                _titleSize = SizeF.Empty;
                return;
            }
            _titleSize = g.MeasureString(_text, _legendTextFont);
            g.DrawString(_text, _legendTextFont, Brushes.Black, BORDER_BLANK * 2, BORDER_BLANK);
        }

        private void DrawLegendItem(Graphics g, LegendItem legendItem, int i, int legendWidth)
        {
            int rowNo = i / _colCount;  //当前行
            int colNo = i % _colCount;  //当前列
            float y = 0f;               //图块x坐标
            float x = 0f;               //图块y坐标
            if (!string.IsNullOrEmpty(_text))
                y = rowNo * (_legendItemHeight + _legendItemSpan) + _titleSize.Height + _legendTextSpan + BORDER_BLANK;
            else
                y = rowNo * (_legendItemHeight + _legendItemSpan) + BORDER_BLANK;
            if (colNo == 0)
                x = BORDER_BLANK;
            else
                x = colNo * (_legendItemWidth + _legendItemSpan + _maxTextWidth) + BORDER_BLANK;
            if (!legendItem.Color.IsEmpty)
            {
                using (Brush brush = new SolidBrush(legendItem.Color))
                {
                    g.FillRectangle(brush, x, y, _legendItemWidth, _legendItemHeight);
                }
            }
            if (_isShowBorder)
            {
                g.DrawRectangle(new Pen(_borderColor), x, y, _legendItemWidth, _legendItemHeight);
            }
            SizeF textSize = g.MeasureString(legendItem.Text, _legendTextFont);
            g.DrawString(legendItem.Text, _legendTextFont, Brushes.Black, x + _legendItemWidth + _legendItemSpan, y + Math.Abs(_legendItemHeight - textSize.Height) / 2f);
        }

        protected override void ComputeItemSize(Graphics g, float w, float h)
        {
            int rolCount = (int)Math.Ceiling(_count * 1f / _colCount);  //行数
            ComputeLegendItemWid(g, (int)Math.Ceiling(w / _colCount));
            SizeF fontSize = g.MeasureString("字高", _legendTextFont);
            if (string.IsNullOrEmpty(_text))
                _legendItemHeight = (int)((h - BORDER_BLANK * 2 - _legendItemSpan * (rolCount - 1)) / rolCount);
            else
                _legendItemHeight = (int)((h - BORDER_BLANK * 2 - _legendItemSpan * (rolCount - 1) - _legendTextSpan - fontSize.Height) / rolCount);
            _legendItemHeight = Math.Max((int)fontSize.Height - 4, _legendItemHeight);
        }

        //计算图例中每一项的宽
        private void ComputeLegendItemWid(Graphics g, int w)
        {
            _maxTextWidth = int.MinValue;
            foreach (LegendItem it in _legendItems)
            {
                SizeF textSize = g.MeasureString(it.Text, _legendTextFont);
                if ((int)textSize.Width > _maxTextWidth)
                    _maxTextWidth = (int)textSize.Width;
            }
            if (_isShowColor)
                _legendItemWidth = w - 2 * BORDER_BLANK - _legendTextSpan - _maxTextWidth;
            else
                _legendItemWidth = 0;
        }
        #endregion

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            string att = null;
            if (xml.Attribute("legenditemspan") != null)
            {
                att = xml.Attribute("legenditemspan").Value;
                if (!string.IsNullOrEmpty(att))
                    int.TryParse(att, out _legendItemSpan);
            }
            if (xml.Attribute("linecount") != null)
            {
                att = xml.Attribute("linecount").Value;
                if (!string.IsNullOrEmpty(att))
                    int.TryParse(att, out _colCount);
            }
            if (xml.Attribute("isshowcolor") != null)
            {
                att = xml.Attribute("isshowcolor").Value;
                if (!string.IsNullOrEmpty(att))
                    bool.TryParse(att,out _isShowColor);
            }
            base.InitByXml(xml);
        }
    }
}
