using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public class Border : SizableElement, IBorder, IPersitable
    {
        private Color _backColor = Color.White;
        private Brush _brush = null;
        private bool _isShowBorderLine = true;

        public Border()
            : base()
        {
            Init();
        }

        public Border(SizeF size)
            : base()
        {
            Init(size);
        }

        private void Init()
        {
            _name = "图廓";
            _icon = ImageGetter.GetImageByName("BorderElement.png");
            _size = new SizeF(800, 600);
            _brush = new SolidBrush(_backColor);
        }

        private void Init(SizeF size)
        {
            _name = "图廓";
            _icon = ImageGetter.GetImageByName("BorderElement.png");
            _size = size;
            _brush = new SolidBrush(_backColor);
        }

        [Persist(enumAttType.ValueType), DisplayName("显示边框"), Category("外观")]
        public bool IsShowBorderLine
        {
            get { return _isShowBorderLine; }
            set { _isShowBorderLine = value; }
        }

        [Persist(enumAttType.UnValueType), DisplayName("背景颜色"), Category("外观")]
        public Color BackColor
        {
            get { return _backColor; }
            set
            {
                _backColor = value;
                _brush = new SolidBrush(_backColor);
            }
        }

        public override bool IsHited(float layoutX, float layoutY)
        {
            return false;
        }

        public override bool IsHited(System.Drawing.RectangleF layoutRect)
        {
            return false;
        }

        public override void Render(object sender, IDrawArgs drawArgs)
        {
            Graphics g = drawArgs.Graphics as Graphics;
            if (g == null)
                return;
            ILayoutRuntime runtime = sender as ILayoutRuntime;
            float screenX = _location.X, screenY = _location.Y;
            runtime.Layout2Screen(ref screenX, ref screenY);
            //Pen p = new Pen(Brushes.Black);
            //p.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
            //注释掉的语句不起作用,因此为了保证右边和下边的边框能够被导出,特意减了1
            float w = _size.Width, h = _size.Height;
            runtime.Layout2Pixel(ref w, ref h);
            if (DrawArgs.IsExporting)
            {
                if (_backColor != Color.Empty && _brush != null)
                    g.FillRectangle(_brush, screenX, screenY, w * runtime.Scale - 1, h * runtime.Scale - 1);
                if (_isShowBorderLine)
                    g.DrawRectangle(Pens.Black, screenX, screenY, w * runtime.Scale - 1, h * runtime.Scale - 1);
            }
            else
            {
                if (_backColor != Color.Empty && _brush != null)
                    g.FillRectangle(_brush, screenX, screenY, w * runtime.Scale - 1, h * runtime.Scale - 1);
                if (_isShowBorderLine)
                    g.DrawRectangle(Pens.Black, screenX, screenY, w * runtime.Scale, h * runtime.Scale);
            }
        }

        public override void Event(object sender, enumCanvasEventType eventType, CanvasEventArgs e)
        {
        }

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            if (xml.Attribute("backcolor") != null)
            {
                _backColor = (Color)LayoutFromFile.Base64StringToObject(xml.Attribute("backcolor").Value);
                _brush = new SolidBrush(_backColor);
            }
            if (xml.Attribute("size") != null)
                _size = (SizeF)LayoutFromFile.Base64StringToObject(xml.Attribute("size").Value);
            if (xml.Attribute("isshowborderline") != null)
            {
                var att = xml.Attribute("isshowborderline").Value;
                if (att != null)
                    _isShowBorderLine = bool.Parse(att);
            }
            base.InitByXml(xml);
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
