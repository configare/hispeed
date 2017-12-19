using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Linq;

namespace GeoDo.RSS.Layout.Elements
{
    public abstract class LegendElementBase : RasterLegendElement, ILegendElement
    {
        protected LegendItem[] _legendItems = null;
        protected int _legendTextSpan = 2;
        protected Font _legendTextFont = new Font("宋体", 9);
        protected string _text = "图例";
        protected Color _backColor = Color.Empty;
        protected string _colorTableName = null;
        protected int _count = 0;
        protected int _legendItemHeight = 0;
        protected int _legendItemWidth = 0;
        protected const int BORDER_BLANK = 1;
        protected bool _isShowBorder = false;
        protected Color _borderColor = Color.White;
        private Bitmap _legendBmp = null;
        protected SizeF _preSize = SizeF.Empty;
        private float _w = 0, _h = 0;

        #region constractors
        public LegendElementBase()
            : base()
        {
            Init();
            _legendItems = new LegendItem[3]
            {
                new LegendItem("图例1",Color.Red),
                new LegendItem("图例2",Color.Green),
                new LegendItem("图例3",Color.Blue)
            };
            _count = 3;
        }

        public LegendElementBase(LegendItem[] items)
            : base()
        {
            Init();
            if (_legendItems == null || _legendItems.Length == 0)
                return;
            _legendItems = items;
            _count = _legendItems.Length;
        }

        public LegendElementBase(PointF location)
            : base()
        {
            Init();
            _location = location;
        }

        public LegendElementBase(PointF location, LegendItem[] items)
            : base()
        {
            Init();
            _location = location;
            _legendItems = items;
        }
        #endregion

        protected abstract void Init();

        #region attributes
        [Persist(enumAttType.UnValueType), DisplayName("图例项集合"), Category("数据")]
        public LegendItem[] LegendItems
        {
            get { return _legendItems; }
            set
            {
                if (value != null && value.Length != 0 && _legendItems != value)
                {
                    _legendItems = value;
                    _count = value.Length;
                    _preSize = SizeF.Empty;
                }
            }
        }

        [Persist(), DisplayName("图例项与文本间距"), Category("布局")]
        public int LegendTextSpan
        {
            get { return _legendTextSpan; }
            set
            {
                if (_legendTextSpan != value)
                {
                    _legendTextSpan = value;
                    _preSize = SizeF.Empty;
                }
            }
        }

        [Persist(enumAttType.UnValueType), DisplayName("字体"), Category("外观")]
        public Font LegendTextFont
        {
            get { return _legendTextFont; }
            set
            {
                if (_legendTextFont != value)
                {
                    _legendTextFont = value;
                    _preSize = SizeF.Empty;
                }
            }
        }

        [Persist(), DisplayName("图例名称"), Category("设计")]
        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    _preSize = SizeF.Empty;
                }
            }
        }

        [Persist(enumAttType.UnValueType), DisplayName("背景色"), Category("外观")]
        public Color Color
        {
            get { return _backColor; }
            set
            {
                if (_backColor != value)
                {
                    _backColor = value;
                    _preSize = SizeF.Empty;
                }
            }
        }

        [Persist(), DisplayName("颜色表名字"), Category("数据")]
        public string ColorTableName
        {
            get { return _colorTableName; }
            set
            {
                if (_colorTableName != value)
                {
                    _colorTableName = value;
                    bool getColorTableOk = TryGetLegendItemsByColorTable();
                    if (getColorTableOk)
                        _preSize = SizeF.Empty;
                }
            }
        }

        [Persist(), DisplayName("是否显示边框"), Category("外观")]
        public bool IsShowBorder
        {
            get { return _isShowBorder; }
            set
            {
                if (_isShowBorder != value)
                {
                    _isShowBorder = value;
                    _preSize = SizeF.Empty;
                }
            }
        }

        [Persist(enumAttType.UnValueType), DisplayName("边框颜色"), Category("外观")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                if (_borderColor != value)
                {
                    _borderColor = value;
                    _preSize = SizeF.Empty;
                }
            }
        }
        #endregion

        public override void Render(object sender, IDrawArgs drawArgs)
        {
            Graphics g = drawArgs.Graphics as Graphics;
            if (g == null)
                return;
            ILayoutRuntime runtime = sender as ILayoutRuntime;
            if (runtime == null)
                return;
            BeginRotate(drawArgs);
            if (_preSize != _size)
                CreatBitmap(drawArgs, g);
            if (_legendBmp == null)
                return;
            try
            {
                float x = _location.X, y = _location.Y;
                drawArgs.Runtime.Layout2Screen(ref x, ref y);
                float scale = runtime.Scale;
                float newWidth = _size.Width * scale + 0.1f;
                float newHeight = _size.Height * scale + 0.1f;
                if (newWidth < 1 || newHeight < 1 || newWidth > 10000 || newHeight > 10000)
                    return;
                using (Bitmap bm = new Bitmap((int)newWidth, (int)newHeight))
                {
                    using (Graphics g1 = Graphics.FromImage(bm))
                    {
                        g1.DrawImage(_legendBmp, 0, 0, bm.Width, bm.Height);
                    }
                    g.DrawImage(bm, (int)x, (int)y);
                }
            }
            finally
            {
                EndRotate(drawArgs);
            }
        }

        private void CreatBitmap(IDrawArgs drawArgs, Graphics g)
        {
            _w = _size.Width; _h = _size.Height;
            drawArgs.Runtime.Layout2Pixel(ref _w, ref _h);
            ComputeItemSize(g, _w, _h);
            if (_legendBmp != null)
                _legendBmp = null;
            _legendBmp = GetLegendBmp((int)_w, (int)_h);
            _preSize = _size;
        }

        protected abstract Bitmap GetLegendBmp(int w, int h);

        //计算图例中每一项的尺寸
        protected abstract void ComputeItemSize(Graphics g, float w, float h);

        private bool TryGetLegendItemsByColorTable()
        {
            if (string.IsNullOrEmpty(_colorTableName))
                return false;
            LegendItem[] _items = null;
            if (RasterLegendElement.RasterLegendItemsGetter != null)
                _items = RasterLegendElement.RasterLegendItemsGetter(_colorTableName);
            if (_items == null || _items.Length == 0)
                return false;
            _legendItems = _items;
            _count = _items.Length;
            return true;
        }

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            string att = null;
            if (xml.Attribute("legenditems") != null)
            {
                _legendItems = (LegendItem[])LayoutFromFile.Base64StringToObject(xml.Attribute("legenditems").Value);
                if (_legendItems != null || _legendItems.Length != 0)
                    _count = _legendItems.Length;
            }
            if (xml.Attribute("legendtextspan") != null)
            {
                att = xml.Attribute("legendtextspan").Value;
                if (!string.IsNullOrEmpty(att))
                    _legendTextSpan = int.Parse(att);
            }
            if (xml.Attribute("legendtextfont") != null)
                _legendTextFont = (Font)LayoutFromFile.Base64StringToObject(xml.Attribute("legendtextfont").Value);
            if (xml.Attribute("text") != null)
                _text = xml.Attribute("text").Value;
            if (xml.Attribute("color") != null)
                _backColor = (Color)LayoutFromFile.Base64StringToObject(xml.Attribute("color").Value);
            if (xml.Attribute("colortablename") != null)
                _colorTableName = xml.Attribute("colortablename").Value;
            if (xml.Attribute("isshowborder") != null)
                _isShowBorder = bool.Parse(xml.Attribute("isshowborder").Value);
            if (xml.Attribute("bordercolor") != null)
                _borderColor = (Color)LayoutFromFile.Base64StringToObject(xml.Attribute("bordercolor").Value);
            base.InitByXml(xml);
        }

        public void Update(LegendItem[] items)
        {
        }

        public override void Dispose()
        {
            if (_legendItems != null && _count > 0)
            {
                foreach (LegendItem it in _legendItems)
                    it.Dispose();
                _legendItems = null;
            }
            if (_legendTextFont != null)
            {
                _legendTextFont.Dispose();
                _legendTextFont = null;
            }
            if (_legendBmp != null)
            {
                _legendBmp.Dispose();
                _legendBmp = null;
            }
            base.Dispose();
        }

        /// <summary>
        /// 暂未使用
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static XElement ToXml(LegendItem[] items)
        {
            if (items == null || items.Length == 0)
                return null;
            List<XElement> content = new List<XElement>();
            foreach (LegendItem item in items)
            {
                XElement xml = LegendItem.ToXml(item);
                if (xml != null)
                    content.Add(xml);
            }
            if (content.Count == 0)
                return null;
            else
                return new XElement("LegendItems", content);
        }

        /// <summary>
        /// 暂未使用
        /// </summary>
        /// <param name="xml">Parent xml</param>
        /// <returns></returns>
        public static LegendItem[] ToLegendItems(XElement parentXml)
        {
            if (parentXml == null)
                return null;
            XElement root = parentXml.Element("LegendItems");
            if (root == null)
                return null;
            IEnumerable<XElement> xmls = root.Elements("LegendItem");
            if (xmls == null || xmls.Count() == 0)
                return null;
            List<LegendItem> items = new List<LegendItem>();
            foreach (XElement xml in xmls)
            {
                LegendItem item = LegendItem.ToLegendItem(xml);
                if (item != null)
                    items.Add(item);
            }
            if (items.Count == 0)
                return null;
            return items.ToArray();
        }
    }
}
