using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.Drawing;

namespace GeoDo.RSS.Layout.Elements
{
    [Export(typeof(IElement)), Category("箭头")]
    public class ArrowLineElement : SizableElement, IPersitable, IDisposable
    {
        private float _angleHeader = 60; //箭头开口的度数
        protected Color _backColor = Color.Empty;
        private Bitmap _bmp = null;
        private SizeF _preSize = SizeF.Empty;
        private float _w = 0, _h = 0;

        #region contractors
        public ArrowLineElement()
            : base()
        {
            Init();
        }

        public ArrowLineElement(PointF location)
            : base()
        {
            _location = location;
            Init();
        }

        private void Init()
        {
            _name = "直线箭头";
            _icon = ImageGetter.GetImageByName("IconArrow.png");
            _size.Width = 15;
            _size.Height = 35;
        }
        #endregion

        #region attributes
        /// <summary>
        /// 箭头开口的角度
        /// </summary>
        [Persist(), DisplayName("箭头开口角度"), Category("布局")]
        public float AngleHeader
        {
            get { return _angleHeader; }
            set
            {
                if (value > 0 && value < 180 && value != _angleHeader)
                {
                    _angleHeader = value;
                    _preSize = SizeF.Empty;
                }
            }
        }

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
                    _preSize = SizeF.Empty;
                }
            }
        }
        #endregion

        public override void Render(object sender, IDrawArgs drawArgs)
        {
            try
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
                    if (newWidth < 1 || newHeight < 1)
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void CreatBitmap(IDrawArgs drawArgs)
        {
            _w = _size.Width; _h = _size.Height;
            drawArgs.Runtime.Layout2Screen(ref _w);
            drawArgs.Runtime.Layout2Screen(ref _h);
            _bmp = GetBitmap(_w, _h);
            _preSize = _size;
        }

        private Bitmap GetBitmap(float w, float h)
        {
            try
            {
                int iw = (int)w;
                int ih = (int)h;
                if (iw <= 0 || ih <= 0)
                    return null;
                Bitmap bmp = new Bitmap(iw, ih);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    PointF point1 = new PointF(w / 2f, 0);
                    PointF point2 = new PointF(0, w);
                    PointF point3 = new PointF(w, w);
                    PointF point4 = new PointF(w / 2f, h);
                    g.DrawLine(Pens.Black, point1, point2);
                    g.DrawLine(Pens.Black, point1, point4);
                    g.DrawLine(Pens.Black, point1, point3);
                }
                return bmp;
            }
            catch
            {
                return null;
            }
        }

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            string att = null;
            if (xml.Attribute("angleheader") != null)
            {
                att = xml.Attribute("angleheader").Value;
                if (!String.IsNullOrEmpty(att))
                    _angleHeader = float.Parse(att);
            }
            if (xml.Attribute("color") != null)
                _backColor = (Color)LayoutFromFile.Base64StringToObject(xml.Attribute("color").Value);
            base.InitByXml(xml);
        }

        public override void Dispose()
        {
            if (_bmp != null)
            {
                _bmp.Dispose();
                _bmp = null;
            }
            base.Dispose();
        }
    }
}
