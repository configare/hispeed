    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Drawing;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Drawing.Drawing2D;

namespace GeoDo.RSS.Layout.GDIPlus
{
    [Export(typeof(IElement)), Category("文本")]
    public class MultlineTextElement : SizableElement, IPersitable
    {
        protected string _text = "双击修改文本内容";
        protected Font _font = new Font("微软雅黑", 14);
        protected Color _fontColor = Color.Black;
        [NonSerialized]
        protected SolidBrush _fontBrush = null;
        [NonSerialized]
        protected SolidBrush _maskBrush = null;
        protected bool _displayMaskColor = false;
        protected Color _backColor = Color.Transparent;
        protected float _fontIncDlt;    //
        protected float _leanAngle = 0; //倾斜角度
        private float _shearX = 0; //Shear()的ShearX因子
        private bool _isRotate = false; // 当倾斜角度在90到270之间时，需要先将其旋转180度；

        public MultlineTextElement()
            : base()
        {
            Init();
        }

        public MultlineTextElement(string text)
            : base()
        {
            _text = text;
            Init();
        }

        public MultlineTextElement(string text, Font font)
            : base()
        {
            _text = text;
            _font = font;
            Init();
        }

        private void Init()
        {
            _name = "自定义多行文本";
            _icon = ImageGetter.GetImageByName("IconTextElement.png");
            _fontBrush = new SolidBrush(Color.Black);
            _maskBrush = new SolidBrush(Color.White);
            _size = new Size(80, 100);
            //UpdateSize();
        }

        //void UpdateSize()
        //{
        //    using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
        //    {
        //        UpdateSize(g, _font);
        //    }
        //}

        //void UpdateSize(Graphics g, Font font)
        //{
        //    _size = g.MeasureString(_text, font);
        //    _fontIncDlt = _size.Height / font.Size;
        //}

        #region Attributes
        [Persist(enumAttType.UnValueType), DisplayName("颜色"), Category("外观")]
        public Color Color
        {
            get
            {
                return _fontColor;
            }
            set
            {
                _fontColor = value;
                _fontBrush.Color = _fontColor;
            }
        }

        [Persist(enumAttType.ValueType), DisplayName("文本"), Category("外观")]
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                //UpdateSize();
            }
        }

        [Persist(enumAttType.UnValueType), DisplayName("掩码颜色"), Category("外观")]
        public Color MaskColor
        {
            get
            {
                if (_maskBrush == null)
                    _maskBrush = new SolidBrush(Color.White);
                return _maskBrush.Color;
            }
            set
            {
                _maskBrush.Color = value;
            }
        }

        [Persist(), DisplayName("是否显示掩码颜色"), Category("外观")]
        public bool DisplayMaskColor
        {
            get { return _displayMaskColor; }
            set { _displayMaskColor = value; }
        }

        [Persist(), DisplayName("倾斜角度"), Category("布局")]
        public float LeanAngle
        {
            get { return _leanAngle; }
            set
            {
                _leanAngle = value;
                ComputeShearXValue();
            }
        }

        private void ComputeShearXValue()
        {
            float arfa = _leanAngle;
            if (_leanAngle % 90 == 0)
                arfa += 0.1f;
            for (int i = 0; i < 10; i++)
            {
                arfa += 360 * i;
                if (arfa % 360 < 360 && arfa % 360 > 0)
                {
                    arfa %= 360;
                    break;
                }
            }
            _shearX = GetATanFromAngle(arfa);
            if (arfa > 90 && arfa < 270)
                _isRotate = true;
            else
                _isRotate = false;
        }

        private float GetATanFromAngle(float value)
        {
            double tan = Math.Tan(Math.PI * value / 180);
            return (float)tan;
        }

        [Persist(enumAttType.UnValueType), DisplayName("字体"), Category("外观")]
        public Font Font
        {
            get { return _font; }
            set
            {
                _font = value;
                //UpdateSize();
            }
        }

        #endregion

        #region SizableElement members
        public override void Render(object sender, IDrawArgs drawArgs)
        {
            Graphics g = drawArgs.Graphics as Graphics;
            if (g == null)
                return;
            BeginRotate(drawArgs);
            try
            {
                float x = _location.X, y = _location.Y;
                drawArgs.Runtime.Layout2Screen(ref x, ref y);
                using (Font font = GetFont((sender as ILayoutRuntime).Scale))
                {
                    Matrix preMatrix = g.Transform;
                    g.TranslateTransform(x, y);
                    if (_leanAngle != 0)
                    {
                        Matrix m = new Matrix();
                        m.Shear(-_shearX, 0);
                        if (_isRotate)
                        {
                            g.RotateTransform(180);
                        }
                        g.MultiplyTransform(m);
                        float ascent = font.FontFamily.GetCellAscent(FontStyle.Regular);
                        float space = font.FontFamily.GetLineSpacing(FontStyle.Regular);
                        float blank = font.GetHeight();
                        float height = blank * ascent / space;
                        SizeF newSize = g.MeasureString(_text, font);
                        if (_isRotate)
                            g.TranslateTransform(-newSize.Width - height * _shearX, -newSize.Height);
                        else
                            g.TranslateTransform(height * _shearX, 0);
                    }
                    if (_displayMaskColor)
                        DrawString(_text, g, _maskBrush, _fontBrush, font, 0, 0);
                    else
                        g.DrawString(_text, font, _fontBrush, 0, 0);
                    g.Transform = preMatrix;
                    //UpdateSize(g, font);
                }
            }
            finally
            {
                EndRotate(drawArgs);
            }
        }

        public void DrawString(string text, Graphics g, SolidBrush maskBrush, SolidBrush fontBrush, Font font, float x, float y)
        {
            g.DrawString(text, font, maskBrush, x + 1, y);
            g.DrawString(text, font, maskBrush, x + 1, y + 1);
            g.DrawString(text, font, maskBrush, x + 1, y - 1);
            g.DrawString(text, font, maskBrush, x - 1, y);
            g.DrawString(text, font, maskBrush, x - 1, y + 1);
            g.DrawString(text, font, maskBrush, x - 1, y - 1);
            g.DrawString(text, font, maskBrush, x, y + 1);
            g.DrawString(text, font, maskBrush, x, y - 1);
            g.DrawString(text, font, fontBrush, x, y);
        }

        private System.Drawing.Font GetFont(float scale)
        {
            Font fnt = new Font(_font.FontFamily, _font.Size * scale, _font.Style);
            return fnt;
        }

        //public override void ApplySize(float layoutXSize, float layoutYSize)
        //{
        //    float fsize = _font.Size + layoutYSize / _fontIncDlt;
        //    fsize = Math.Max(fsize, 5f);
        //    Font font = new Font(_font.FontFamily, fsize, _font.Style);
        //    _font.Dispose();
        //    _font = font;
        //    //UpdateSize();
        //}

        public override void Dispose()
        {
            if (_font != null)
            {
                _font.Dispose();
                _font = null;
            }
            if (_fontBrush != null)
            {
                _fontBrush.Dispose();
                _fontBrush = null;
            }
            if (_maskBrush != null)
            {
                _maskBrush.Dispose();
                _maskBrush = null;
            }
            base.Dispose();
        }
        #endregion

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            if (xml.Attribute("backcolor") != null)
                _backColor = (Color)LayoutFromFile.Base64StringToObject(xml.Attribute("backcolor").Value);
            if (xml.Attribute("color") != null)
            {
                _fontColor = (Color)LayoutFromFile.Base64StringToObject(xml.Attribute("color").Value);
                if (!_fontColor.IsEmpty)
                    _fontBrush.Color = _fontColor;
            }
            if (xml.Attribute("text") != null)
                _text = FilterTransstr(xml.Attribute("text").Value);
            if (xml.Attribute("font") != null)
                _font = (Font)LayoutFromFile.Base64StringToObject(xml.Attribute("font").Value);
            if (xml.Attribute("size") != null)
            {
                _size = (SizeF)LayoutFromFile.Base64StringToObject(xml.Attribute("size").Value);
            }
            if (xml.Attribute("maskcolor") != null)
                _maskBrush.Color = (Color)LayoutFromFile.Base64StringToObject(xml.Attribute("maskcolor").Value);
            if (xml.Attribute("displaymaskcolor") != null)
                _displayMaskColor = bool.Parse(xml.Attribute("displaymaskcolor").Value);
            if (xml.Attribute("leanangle") != null)
            {
                _leanAngle = float.Parse(xml.Attribute("leanangle").Value);
                ComputeShearXValue();
            }
            base.InitByXml(xml);
        }

        private string FilterTransstr(string value)
        {
            if (value.Contains("&amp;#xD;"))
                value = value.Replace("&amp;#xD;", "\r");
            if (value.Contains("&amp;#xA;"))
                value = value.Replace("&amp;#xA;", "\n");
            return value;
        }
    }
}