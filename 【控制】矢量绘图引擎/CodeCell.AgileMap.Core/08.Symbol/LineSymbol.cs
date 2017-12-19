using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Linq;
using System.IO;
using CodeCell.Bricks.Serial;


namespace CodeCell.AgileMap.Core
{
    public interface ILineSymbol : ISymbol
    {
        float Width { get; set; }
        DashStyle DashStyle { get; set; }
        LineCap StartLineCap { get; set; }
        LineCap EndLineCap { get; set; }
        LineJoin LineJoin { get; set; }
    }

    [Serializable]
    public abstract class LineSymbol : Symbol, ILineSymbol, IFormattable,IPersistable
    {
        protected float _width = 1;
        protected DashStyle _dashStyle = DashStyle.Solid;
        protected float[] _dashPattern = null;
        protected LineCap _startLineCap = LineCap.Flat;
        protected LineCap _endLineCap = LineCap.Flat;
        protected LineJoin _lineJoin = LineJoin.Miter;
     
        public LineSymbol()
        { }

        public LineSymbol(Color color)
            : base(color)
        { }

        public LineSymbol(Color color, float width)
            : base(color)
        {
            _width = width;
            _pen.Width = width;
        }

        public LineSymbol(Color color, float width, DashStyle dashStyle)
            : base(color)
        {
            _width = width;
            _pen.Width = width;
            _pen.DashStyle = dashStyle;
        }

        protected override void ReBuildPen()
        {
            base.ReBuildPen();
            _pen.Width = _width;
        }

        #region ILineSymbol 成员

        [DisplayName("线宽")]
        public float Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (_width == value)
                    return;
                _width = value;
                _pen.Width = _width;
            }
        }

        [DisplayName("线样式")]
        public DashStyle DashStyle
        {
            get
            {
                return _dashStyle;
            }
            set
            {
                if (_dashStyle == value)
                    return;
                _dashStyle = value;
                _pen.DashStyle = _dashStyle;
            }
        }

        [DisplayName("断线模式")]
        public float[] DashPattern
        {
            get
            {
                return _dashPattern;
            }
            set
            {
                _dashPattern = value;
                lock (_pen)
                {
                    _pen.DashPattern = _dashPattern;
                }
            }
        }

        [DisplayName("起始端线帽")]
        public LineCap StartLineCap
        {
            get { return _startLineCap; }
            set { _startLineCap = value; }
        }

        [DisplayName("结束端线帽")]
        public LineCap EndLineCap
        {
            get { return _endLineCap; }
            set { _endLineCap = value; }
        }

        [DisplayName("连接点风格")]
        public LineJoin LineJoin
        {
            get { return _lineJoin; }
            set { _lineJoin = value; }
        }

        #endregion
        /// <summary>
        /// 将此类型转化为可读的字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(null, null);
        }

        #region IFormattable 成员

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null) return String.Format("({0},{1},{2})", _dashStyle.ToString(), _width.ToString(), _color.ToString());
            return String.Format("{0}", format);
        }

        #endregion
    }

    public interface ISimpleLineSymbol : ILineSymbol
    { 
        [Browsable(false)]
        float[] DashPattern{get;set;}
    }

    /// <summary>
    /// 简单线符号
    /// 默认样式：Yellow，1像素,实线
    /// </summary>
    [Serializable]
    public class SimpleLineSymbol : LineSymbol, ISimpleLineSymbol
    {
        public SimpleLineSymbol()
        {
            _color = Color.Blue;
            _pen = new Pen(_color);
        }

        public SimpleLineSymbol(Color color)
            : base(color)
        { }

        public SimpleLineSymbol(Color color, float width)
            : base(color, width)
        { }

        public override void Draw(Graphics g, GraphicsPath path)
        {
            if (path == null)
                return;
            if (_pen == null)
            {
                _pen = new Pen(_color);
            }
            _pen.StartCap = _startLineCap;
            _pen.EndCap = _endLineCap;
            _pen.LineJoin = _lineJoin;
            g.DrawPath(_pen, path);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("Symbol");
            obj.AddAttribute("type", Path.GetFileName(this.GetType().Assembly.Location) + "," + this.GetType().ToString());
            obj.AddAttribute("color", ColorHelper.ColorToString(_color));
            obj.AddAttribute("width", _width.ToString());
            obj.AddAttribute("startcap", _startLineCap.ToString());
            obj.AddAttribute("endcap", _endLineCap.ToString());
            obj.AddAttribute("linejoin", _lineJoin.ToString());
            return obj;
        }

        public static ISimpleLineSymbol FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            Color color = ColorHelper.StringToColor(ele.Attribute("color").Value);
            float width = float.Parse(ele.Attribute("width").Value);
            ISimpleLineSymbol lneSym = new SimpleLineSymbol(color, width);
            string cap = null ;
            if (ele.Attribute("startcap") != null)
            {
                cap = ele.Attribute("startcap").Value ;
                foreach(LineCap c in Enum.GetValues(typeof(LineCap)))
                    if (c.ToString() == cap)
                    {
                        lneSym.StartLineCap = c;
                        break;
                    }
            }
            if (ele.Attribute("endcap") != null)
            {
                cap = ele.Attribute("endcap").Value;
                foreach (LineCap c in Enum.GetValues(typeof(LineCap)))
                    if (c.ToString() == cap)
                    {
                        lneSym.EndLineCap = c;
                        break;
                    }
            }
            if (ele.Attribute("linejoin") != null)
            {
                cap = ele.Attribute("linejoin").Value;
                foreach (LineJoin c in Enum.GetValues(typeof(LineJoin)))
                    if (c.ToString() == cap)
                    {
                        lneSym.LineJoin = c;
                        break;
                    }
            }
            return lneSym;
        }
    }

    public interface IFillLineSymbol:ILineSymbol,ITwoStepDrawSymbol
    {
        float OutlineWidth { get; set; }
        Color OutlineColor { get; set; }
        Color FillColor { get; set; }
        SmoothingMode SmoothingMode { get; set; }
    }

    public class FillLineSymbol : LineSymbol,IFillLineSymbol,ITwoStepDrawSymbol
    {
        private float _outlineWidth = 1;
        private Color _outlineColor = Color.Black;
        private Color _fillColor = Color.White;
        private SmoothingMode _smoothingMode = SmoothingMode.Default;

        public FillLineSymbol()
        {
            _width = 6;
        }

        [Browsable(false)]
        public new  DashStyle DashStyle
        {
            get{return DashStyle.Solid;}
            set{;}
        }

        [Browsable(false)]
        public new Color Color
        {
            get { return Color.Empty; }
            set { ;}
        }

        [Browsable(false)]
        public new float[] DashPattern
        {
            get { return null; }
            set { ;}
        }

        [Browsable(false)]
        public new float Angle
        {
            get { return _angle; }
            set { ; }
        }

        #region IFillLineSymbol Members

        [DisplayName("线宽")]
        public new  float Width
        {
            get { return _width; }
            set { _width = value; }
        }

        [DisplayName("外边线宽度")]
        public float OutlineWidth
        {
            get { return _outlineWidth; }
            set 
            {
                if (value > 0)
                    _outlineWidth = value;
            }
        }

        [DisplayName("外边线颜色")]
        public Color OutlineColor
        {
            get { return _outlineColor; }
            set { _outlineColor = value; }
        }

        [DisplayName("填充颜色")]
        public Color FillColor
        {
            get { return _fillColor; }
            set { _fillColor = value; }
        }

        [DisplayName("平滑选项")]
        public SmoothingMode SmoothingMode
        {
            get { return _smoothingMode; }
            set { _smoothingMode = value; }
        }

        public override void Draw(Graphics g, GraphicsPath path)
        {
            DrawOutline(g, path);
            Fill(g, path);
        }

        public void DrawOutline(Graphics g, GraphicsPath path)
        {
            SmoothingMode oldM = g.SmoothingMode;
            try
            {
                g.SmoothingMode = _smoothingMode;
                using (Pen p = new Pen(_outlineColor, _width))
                {
                    //_pen.StartCap = _startLineCap;
                    //_pen.EndCap = _endLineCap;
                    //_pen.LineJoin = _lineJoin;
                    p.Alignment = PenAlignment.Center;
                    g.DrawPath(p, path);
                }
            }
            finally 
            {
                g.SmoothingMode = oldM;
            }
        }

        public void Fill(Graphics g, GraphicsPath path)
        {
            float w = _width - 2 * _outlineWidth;
            if (w <= 0)
                w = 1;
            SmoothingMode oldM = _smoothingMode;
            try
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                using (Pen p = new Pen(_fillColor, w))
                {
                    //_pen.StartCap = _startLineCap;
                    //_pen.EndCap = _endLineCap;
                    //_pen.LineJoin = _lineJoin;
                    p.Alignment = PenAlignment.Center;
                    g.DrawPath(p, path);
                }
            }
            finally
            {
                g.SmoothingMode = oldM;
           }
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
            if (_pen != null)
            {
                _pen.Dispose();
                _pen = null;
            }
        }

        public override PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("Symbol");
            obj.AddAttribute("type", Path.GetFileName(this.GetType().Assembly.Location) + "," + this.GetType().ToString());
            obj.AddAttribute("outlinecolor", ColorHelper.ColorToString(_outlineColor));
            obj.AddAttribute("fillcolor", ColorHelper.ColorToString(_fillColor));
            obj.AddAttribute("width", _width.ToString());
            obj.AddAttribute("outlinewidth", _outlineWidth.ToString());
            obj.AddAttribute("smoothingmode", _smoothingMode.ToString());
            return obj;
        }

        public static IFillLineSymbol FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            Color outlinecolor = ColorHelper.StringToColor(ele.Attribute("outlinecolor").Value);
            Color fillcolor = ColorHelper.StringToColor(ele.Attribute("fillcolor").Value);
            int outlinewidth = int.Parse(ele.Attribute("outlinewidth").Value);
            float width = float.Parse(ele.Attribute("width").Value);
            IFillLineSymbol sym = new FillLineSymbol();
            sym.OutlineColor = outlinecolor;
            sym.FillColor = fillcolor;
            sym.OutlineWidth = outlinewidth;
            sym.Width = width;
            //
            if (ele.Attribute("smoothingmode") != null)
            {
                string s = ele.Attribute("smoothingmode").Value;
                foreach (SmoothingMode sm in Enum.GetValues(typeof(SmoothingMode)))
                {
                    if (sm.ToString() == s)
                    {
                        sym.SmoothingMode = sm;
                        break;
                    }
                }
            }
            //
            return sym;
        }
    }

}
