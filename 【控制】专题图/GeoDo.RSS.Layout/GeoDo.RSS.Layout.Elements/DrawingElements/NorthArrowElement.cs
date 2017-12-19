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
    [Export(typeof(IElement)), Category("指北针")]
    public class NorthArrowElement : SizableElement, IPersitable, IDisposable
    {
        private int _cstSpan = 15;
        private int _cstHeightMax = 35;
        private int _cstHeightMin = 25;
        private Font _font = new Font("微软雅黑", 11, FontStyle.Regular);
        private string _NNameString = "N";
        protected Color _backColor = Color.Empty;
        private SizeF _preSize = SizeF.Empty;
        private Bitmap _northArrowBp = null;

        public NorthArrowElement()
            : base()
        {
            Init();
        }

        public NorthArrowElement(PointF location)
            : base()
        {
            _location = location;
            Init();
        }

        private void Init()
        {
            _name = "指北针";
            _icon = ImageGetter.GetImageByName("IconNorthArrow.png");
            _size.Width = 15;
            _size.Height = 35;
        }

        #region attributes
        [Persist(), DisplayName("指北针标注文本"), Category("外观")]
        public string NorthArrowLabelString
        {
            get { return _NNameString; }
            set
            {
                if (_NNameString != value)
                {
                    _NNameString = value;
                    _preSize = SizeF.Empty;
                }
            }
        }

        [Persist(enumAttType.UnValueType), DisplayName("颜色"), Category("外观")]
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
            //尺寸改变时才重新绘制
            if (_preSize != _size)
            {
                _northArrowBp = GetNorthArrowBitmap(drawArgs);
            }
            if (_northArrowBp == null)
                return;
            try
            {
                float x = _location.X, y = _location.Y, w = _size.Width, h = _size.Height;
                drawArgs.Runtime.Layout2Screen(ref x, ref y);
                drawArgs.Runtime.Layout2Screen(ref w);
                drawArgs.Runtime.Layout2Screen(ref h);
                float scale = runtime.Scale;
                int newWidth = (int)(_size.Width * scale + 0.1f);
                int newHeight = (int)(_size.Height * scale + 0.1f);
                if (newWidth < 1 || newHeight < 1 || newWidth > 10000 || newHeight > 10000)
                    return;
                x += 0.5f;
                y += 0.5f;
                using (Bitmap bm = new Bitmap(newWidth, newHeight))
                {
                    using (Graphics gsp = Graphics.FromImage(bm))
                    {
                        gsp.DrawImage(_northArrowBp, new Rectangle(0, 0, bm.Width, bm.Height));
                    }
                    g.DrawImage(bm, (int)x, (int)y);
                }
            }
            finally
            {
                EndRotate(drawArgs);
            }
        }

        private Bitmap GetNorthArrowBitmap(IDrawArgs drawArgs)
        {
            Graphics g = drawArgs.Graphics as Graphics;
            if (g == null)
                return null;
            SizeF size = g.MeasureString(_NNameString, _font);
            int crtWidth = Math.Max(_cstSpan, (int)size.Width);
            float x = crtWidth / 2;
            float y = size.Height;

            Bitmap bm = new Bitmap(Math.Max(_cstSpan, (int)size.Width), _cstHeightMax + (int)Math.Ceiling(size.Height));
            using (Graphics gp = Graphics.FromImage(bm))
            {
                GraphicsPath path = new GraphicsPath();
                GraphicsPath path1 = new GraphicsPath();
                path1.AddLine(x, y, x - _cstSpan / 2f, y + _cstHeightMax);
                path1.AddLine(x - _cstSpan / 2f, y + _cstHeightMax, x, y + _cstHeightMin);
                path1.AddLine(x, y + _cstHeightMin, x, y);
                GraphicsPath path2 = new GraphicsPath();
                path2.AddLine(x, y, x + _cstSpan / 2f, y + _cstHeightMax);
                path2.AddLine(x + _cstSpan / 2f, y + _cstHeightMax, x, y + _cstHeightMin);
                path2.AddLine(x, y + _cstHeightMin, x, y);
                path.AddPath(path1, true);
                path.AddPath(path2, true);
                gp.Clear(_backColor);
                gp.SmoothingMode = SmoothingMode.HighQuality;
                gp.DrawPath(Pens.Black, path);
                gp.FillPath(Brushes.Black, path1);
                gp.DrawString(_NNameString, _font, Brushes.Black, 1, 0);
            }
            _preSize = _size;
            return bm;
        }

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            string att = null;
            if (xml.Attribute("northarrowlabelstring") != null)
            {
                att = xml.Attribute("northarrowlabelstring").Value;
                if (!String.IsNullOrEmpty(att))
                    _NNameString = att;
            }
            if (xml.Attribute("color") != null)
                _backColor = (Color)LayoutFromFile.Base64StringToObject(xml.Attribute("color").Value);
            base.InitByXml(xml);
        }

        public override void Dispose()
        {
            if (_font != null)
            {
                _font.Dispose();
                _font = null;
            }
            if (_northArrowBp != null)
            {
                _northArrowBp.Dispose();
                _northArrowBp = null;
            }
            base.Dispose();
        }
    }
}
