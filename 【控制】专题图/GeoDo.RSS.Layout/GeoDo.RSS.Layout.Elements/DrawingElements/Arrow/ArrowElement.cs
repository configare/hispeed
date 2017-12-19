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
    [Export(typeof(IElement)), Category("箭头")]
    public class ArrowElement : SizableElement, IPersitable, IDisposable
    {
        private Color _backColor = Color.Red;
        private Brush _brush = null;
        private Pen _pen = null;
        private SizeF _preSize = SizeF.Empty;
        private Bitmap _bmp = null;
        private float _w = 0, _h = 0;        

        #region contractors
        public ArrowElement()
            : base()
        {
            Init();
        }

        public ArrowElement(PointF location)
            : base()
        {
            _location = location;
            Init();
        }

        private void Init()
        {
            _name = "箭头(默认红色)";
            _icon = ImageGetter.GetImageByName("IconArrowRec.png");
            _size.Width = 15;
            _size.Height = 35;
            _pen = new Pen(Color.Black, 0.1f);
            _brush = new SolidBrush(_backColor);
        }
        #endregion

        #region attributes
        /// <summary>
        /// 颜色
        /// </summary>
        [Persist(enumAttType.UnValueType), DisplayName("颜色"), Category("外观")]
        public Color Color
        {
            get { return _backColor; }
            set
            {
                if (_backColor != value)
                {
                    _backColor = value;
                    _brush = new SolidBrush(_backColor);
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
            {
                CreatBitmap(drawArgs);
            }
            if (_bmp == null)
                return;
            try
            {
                float x = _location.X, y = _location.Y;
                drawArgs.Runtime.Layout2Screen(ref x, ref y);
                float scale = runtime.Scale;
                int newWidth = (int)(_size.Width * scale + 0.1f);
                int newHeight = (int)(_size.Height * scale + 0.1f);
                if (newWidth < 1 || newHeight < 1 || newWidth > 10000 || newHeight > 10000)
                    return;
                using (Bitmap bm = new Bitmap(newWidth, newHeight))
                {
                    using (Graphics gsp = Graphics.FromImage(bm))
                    {
                        gsp.DrawImage(_bmp, new Rectangle(0, 0, bm.Width, bm.Height));
                    }
                    g.DrawImage(bm, x, y);
                }
            }
            finally
            {
                EndRotate(drawArgs);
            }
        }

        private void CreatBitmap(IDrawArgs drawArgs)
        {
            _w = _size.Width; _h = _size.Height;
            drawArgs.Runtime.Layout2Screen(ref _w);
            drawArgs.Runtime.Layout2Screen(ref _h);
            if (_bmp != null)
                _bmp.Dispose();
            _bmp = GetArrowBitmap(_w - 2 * _pen.Width, _h - 2 * _pen.Width);//需减去线的宽度
            _preSize = _size;
        }

        private Bitmap GetArrowBitmap(float w, float h)
        {
            if (w <= 9 || h <= 9)
                return null;
            Matrix matrix = new Matrix();
            matrix.Scale(w / 20.2f, h / 40.2f);
            Bitmap bmp = new Bitmap((int)Math.Ceiling(w) + 1, (int)Math.Ceiling(h) + 1);//需要在这里各加上线宽
            using (Graphics g = Graphics.FromImage(bmp))
            {
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddLine(10, 0, 0, 20);
                    path.AddLine(0, 20, 7, 17);
                    path.AddLine(7, 17, 7, 40);
                    path.AddLine(7, 40, 13, 40);
                    path.AddLine(13, 40, 13, 17);
                    path.AddLine(13, 17, 20, 20);
                    path.AddLine(20, 20, 10, 0);
                    g.Transform = matrix;
                    g.FillPath(_brush, path);
                    g.DrawPath(_pen, path);
                }
            }
            return bmp;
        }

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            if (xml.Attribute("color") != null)
                _backColor = (Color)LayoutFromFile.Base64StringToObject(xml.Attribute("color").Value);
            base.InitByXml(xml);
        }

        public override void Dispose()
        {
            if (_pen != null)
            {
                _pen.Dispose();
                _pen = null;
            }
            if (_brush != null)
            {
                _brush.Dispose();
                _brush = null;
            }
            if (_bmp != null)
            {
                _bmp.Dispose();
                _bmp = null;
            }
            base.Dispose();
        }
    }
}
