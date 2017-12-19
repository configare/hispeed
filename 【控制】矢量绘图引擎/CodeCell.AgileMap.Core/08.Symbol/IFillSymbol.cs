using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Xml.Linq;
using System.Reflection;
using System.IO;
using CodeCell.Bricks.Serial;


namespace CodeCell.AgileMap.Core
{
    public interface IFillSymbol : ISymbol
    {
        ILineSymbol OutlineSymbol { get; set; }
        float Transparent { get; set; }
    }

    [Serializable]
    public abstract class FillSymbol : Symbol, IFillSymbol
    {
        protected ILineSymbol _outlineSymbol = new SimpleLineSymbol(Color.Yellow, 1);
        protected float _transparent = 0f;

        #region IFillSymbol 成员

        [DisplayName("外边线"), TypeConverter(typeof(ExpandableObjectConverter))]
        public ILineSymbol OutlineSymbol
        {
            get
            {
                return _outlineSymbol;
            }
            set
            {
                _outlineSymbol = value;
            }
        }

        [DisplayName("透明度"), Description("透明度:\n取值范围:0~100\n0为不透明,100为全部透明")]
        public float Transparent
        {
            get { return _transparent; }
            set
            {
                if (value < 0 || value > 100)
                    return;
                _transparent = value;
                (_brush as SolidBrush).Color = Color.FromArgb((int)(255 - _transparent / 100 * 255), _color.R, _color.G, _color.B);
            }
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
            if (_outlineSymbol != null)
                _outlineSymbol.Dispose();
        }
    }

    public interface ISimpleFillSymbol : IFillSymbol
    {

    }

    /// <summary>
    /// 简单填充符号
    /// 默认样式:
    ///    填充色 Color.Empty，
    ///    轮廓线 null
    /// 属性：
    ///    Color 填充色
    ///    OutlineSymbol 轮廓线
    /// </summary>
    [Serializable]
    public class SimpleFillSymbol : FillSymbol, ISimpleFillSymbol
    {
        public SimpleFillSymbol()
        {
            _color = Color.Transparent;
            _brush = new SolidBrush(_color);
        }

        public SimpleFillSymbol(Color fillColor)
        {
            _color = fillColor;
            _brush = new SolidBrush(fillColor);
        }

        public SimpleFillSymbol(ILineSymbol outlineSymbol)
        {
            _outlineSymbol = outlineSymbol;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color">填充色</param>
        /// <param name="outlineSymbol">轮廓线</param>
        public SimpleFillSymbol(Color fillColor, ILineSymbol outlineSymbol)
            : this(fillColor)
        {
            _outlineSymbol = outlineSymbol;
        }

        public override void Draw(Graphics g, GraphicsPath path)
        {
            if (path == null)
                return;
            if (_color != Color.Empty && _color != Color.Transparent)
            {
                if (_brush == null)
                    _brush = new SolidBrush(_color);
                g.FillPath(_brush, path);
            }
            if (_outlineSymbol != null)
                _outlineSymbol.Draw(g, path);
        }

        public override PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("Symbol");
            obj.AddAttribute("type", Path.GetFileName(this.GetType().Assembly.Location) + "," + this.GetType().ToString());
            obj.AddAttribute("fillcolor", ColorHelper.ColorToString(_color));
            if (_outlineSymbol != null)
            {
                PersistObject outobj = new PersistObject("OutlineSymbol");
                obj.AddSubNode(outobj);
                outobj.AddSubNode((_outlineSymbol as IPersistable).ToPersistObject());
            }
            return obj;
        }

        public static ISimpleFillSymbol FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            ILineSymbol outlineSymbol = null;
            Color fillColor = ColorHelper.StringToColor(ele.Attribute("fillcolor").Value);
            XElement outline = ele.Element("OutlineSymbol");
            if (outline != null)
            {
                outlineSymbol = PersistObject.ReflectObjFromXElement(outline.Element("Symbol")) as ILineSymbol;
            }
            else
            {
                outlineSymbol = new SimpleLineSymbol();
            }
            return new SimpleFillSymbol(fillColor, outlineSymbol);
        }
    }

    [Serializable]
    public class RailwayBrush : Symbol, ILineSymbol
    {
        private ISimpleLineSymbol _fillSymbolWhite = new SimpleLineSymbol(Color.White);
        private ISimpleLineSymbol _fillSymbolBlack = new SimpleLineSymbol(Color.Black);
        private ISimpleLineSymbol _outlineSymbol = new SimpleLineSymbol(Color.FromArgb(64, 64, 64));
        private float _width = 2.0f;

        public RailwayBrush()
        {
            _outlineSymbol = new SimpleLineSymbol(Color.FromArgb(64, 64, 64), _width);
            _fillSymbolBlack = new SimpleLineSymbol(Color.Black, _width - 0.8f);
            _fillSymbolWhite = new SimpleLineSymbol(Color.White, _width - 0.8f);
            _fillSymbolWhite.DashStyle = DashStyle.DashDot;
            _fillSymbolWhite.DashPattern = new float[] { 10, 10 };
        }

        public RailwayBrush(float width)
        {
            _width = width;
            _outlineSymbol = new SimpleLineSymbol(Color.FromArgb(64, 64, 64), _width);
            _fillSymbolBlack = new SimpleLineSymbol(Color.Black, _width - 2);
            _fillSymbolWhite = new SimpleLineSymbol(Color.White, _width - 2);
            _fillSymbolWhite.DashStyle = DashStyle.DashDot;
            _fillSymbolWhite.DashPattern = new float[] { 10, 10 };
        }

        public RailwayBrush(float width, ISimpleLineSymbol sym1, ISimpleLineSymbol sym2, ISimpleLineSymbol sym3)
        {
            _width = width;
            _outlineSymbol = sym3;
            _fillSymbolBlack = sym1;
            _fillSymbolWhite = sym2;
            _fillSymbolWhite.DashStyle = DashStyle.DashDot;
            _fillSymbolWhite.DashPattern = new float[] { 10, 10 };
        }

        [DisplayName("符号(2)"), TypeConverter(typeof(ExpandableObjectConverter))]
        public ISimpleLineSymbol Symbol1
        {
            get { return _fillSymbolBlack; }
            set { _fillSymbolBlack = value; }
        }

        [DisplayName("符号(1)"), TypeConverter(typeof(ExpandableObjectConverter))]
        public ISimpleLineSymbol Symbol2
        {
            get { return _fillSymbolWhite; }
            set { _fillSymbolWhite = value; }
        }

        [DisplayName("外边线"), TypeConverter(typeof(ExpandableObjectConverter))]
        public ISimpleLineSymbol OutLineSymbol
        {
            get { return _outlineSymbol; }
            set { _outlineSymbol = value; }
        }

        [Browsable(false)]
        public new Color Color
        {
            get { return _color; }
            set { ;}
        }

        [Browsable(false)]
        public new LineCap StartLineCap
        {
            get { return LineCap.Flat; }
            set { ;}
        }

        [Browsable(false)]
        public new LineCap EndLineCap
        {
            get { return LineCap.Flat; }
            set { ;}
        }

        [Browsable(false)]
        public new LineJoin LineJoin
        {
            get { return LineJoin.Miter; }
            set { ;}
        }

        public override void Draw(Graphics g, GraphicsPath path)
        {
            SmoothingMode sm = g.SmoothingMode;
            try
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                _outlineSymbol.Draw(g, path);
                _fillSymbolBlack.Draw(g, path);
                _fillSymbolWhite.Draw(g, path);
            }
            finally 
            {
                g.SmoothingMode = sm;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_outlineSymbol != null)
            {
                _outlineSymbol.Dispose();
                _outlineSymbol = null;
            }
            if (_fillSymbolBlack != null)
            {
                _fillSymbolBlack.Dispose();
                _fillSymbolBlack = null;
            }
            if (_fillSymbolWhite != null)
            {
                _fillSymbolWhite.Dispose();
                _fillSymbolWhite = null;
            }
        }

        #region ILineSymbol Members

        [DisplayName("宽度"), Browsable(false)]
        public float Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        [DisplayName("线风格"), Browsable(false)]
        public DashStyle DashStyle
        {
            get
            {
                if (_fillSymbolWhite == null)
                    return DashStyle.Solid;
                return _fillSymbolWhite.DashStyle;
            }
            set
            {
                if (_fillSymbolWhite != null)
                    _fillSymbolWhite.DashStyle = value;
            }
        }

        #endregion

        public override PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("Symbol");
            obj.AddAttribute("type",Path.GetFileName(this.GetType().Assembly.Location)+","+this.GetType().ToString());
            obj.AddAttribute("width", _width.ToString());
            //fill 1
            PersistObject fill1 = new PersistObject("FillSymbol1");
            obj.AddSubNode(fill1);
            fill1.AddSubNode((_fillSymbolBlack as IPersistable).ToPersistObject());
            //fill 2
            PersistObject fill2 = new PersistObject("FillSymbol2");
            obj.AddSubNode(fill2);
            fill2.AddSubNode((_fillSymbolWhite as IPersistable).ToPersistObject());
            //outline
            PersistObject outline = new PersistObject("OutlineSymbol");
            obj.AddSubNode(outline);
            outline.AddSubNode((_outlineSymbol as IPersistable).ToPersistObject());
            //
            return obj;
        }

        public static RailwayBrush FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            float width = float.Parse(ele.Attribute("width").Value);
            ISimpleLineSymbol sym1 = PersistObject.ReflectObjFromXElement(ele.Element("FillSymbol1").Element("Symbol")) as ISimpleLineSymbol;
            ISimpleLineSymbol sym2 = PersistObject.ReflectObjFromXElement(ele.Element("FillSymbol2").Element("Symbol")) as ISimpleLineSymbol;
            ISimpleLineSymbol sym3 = PersistObject.ReflectObjFromXElement(ele.Element("OutlineSymbol").Element("Symbol")) as ISimpleLineSymbol;
            return new RailwayBrush(width, sym1, sym2, sym3);
        }
    }
}
