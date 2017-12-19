using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.Drawing;

namespace GeoDo.RSS.Layout.DataFrm
{
    [Export(typeof(IElement)), Category("图例")]
    public class LineDrawingElement : SizableElement, ISingleDrawingElement, IPersitable
    {
        private int _lineWid = 1;
        private Color _color = Color.Empty;
        private bool _isShowBorder = true;
        private Brush _brush = null;

        public LineDrawingElement()
            : base()
        {
            _name = "线矢量图例";
            _icon = Image.FromStream(this.GetType().Assembly.GetManifestResourceStream("GeoDo.RSS.Layout.DataFrm.Elements.Vector.png"));
            _color = Color.Red;
            _brush = new SolidBrush(_color);
            _size = new SizeF(60, 30);
        }

        [Persist(), DisplayName("线宽"), Category("布局")]
        public int LineWidth
        {
            get { return _lineWid; }
            set { _lineWid = value; }
        }

        [Persist(enumAttType.UnValueType), DisplayName("线条颜色"), Category("外观")]
        public System.Drawing.Color Color
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    _brush = new SolidBrush(_color);
                }
            }
        }

        [Persist(), DisplayName("是否显示边框"), Category("外观")]
        public bool IsShowBorder
        {
            get { return _isShowBorder; }
            set { _isShowBorder = value; }
        }

        public void Update()
        {
        }

        public override void Render(object sender, IDrawArgs drawArgs)
        {
            Graphics g = drawArgs.Graphics as Graphics;
            if (g == null)
                return;
            BeginRotate(drawArgs);
            try
            {
                float x = _location.X, y = _location.Y, w = _size.Width, h = _size.Height;
                drawArgs.Runtime.Layout2Screen(ref x, ref y);
                drawArgs.Runtime.Layout2Screen(ref w);
                drawArgs.Runtime.Layout2Screen(ref h);
                DrawLegend(g, x, y, w, h);
            }
            finally
            {
                EndRotate(drawArgs);
            }
        }

        private void DrawLegend(Graphics g, float x, float y, float w, float h)
        {
            if (x < 0 || y < 0 || w < 9 || h < 9)
                return;
            if (_isShowBorder)
                g.DrawRectangle(Pens.Black, x, y, w, h);
            g.FillRectangle(_brush, x + w / 6f, y + (h - _lineWid) / 2f, w * 2 / 3f, _lineWid);
        }

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            if (xml.Attribute("linewidth") != null)
                _lineWid = int.Parse(xml.Attribute("linewidth").Value);
            string value = string.Empty;
            if (xml.Attribute("isshowborder") != null)
            {
                value = xml.Attribute("isshowborder").Value;
                _isShowBorder = StringToBool(value);
            }
            if (xml.Attribute("color") != null)
            {
                _color = (Color)LayoutFromFile.Base64StringToObject(xml.Attribute("color").Value);
                _brush = new SolidBrush(_color);
            }
            base.InitByXml(xml);
        }

        private bool StringToBool(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;
            switch (value.ToLower())
            {
                case "true":
                    return true;
                case "false":
                    return false;
                default:
                    return false;
            }
        }

        public override void Dispose()
        {
            if (_brush != null)
            {
                _brush.Dispose();
                _brush = null;
            }
            base.Dispose();
        }
    }
}
