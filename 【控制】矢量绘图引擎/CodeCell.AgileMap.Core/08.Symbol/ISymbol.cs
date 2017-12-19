using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Reflection;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Core
{
    public interface ISymbol : IDisposable,IPersistable
    {
        void Draw(Graphics g, GraphicsPath path);//polyline,Polygon
        void Draw(Graphics g, PointF point);
        Color Color { get; set; }
        SizeF SymbolSize { get; }
        float SymbolHalfWidth { get; }
        float SymbolHalfHeight { get; }
        float Angle { get; set; }
    }

    [Serializable]
    public abstract class Symbol : ISymbol, IDisposable,IPersistable
    {
        protected Color _color = Color.Red;
        [NonSerialized]
        protected Pen _pen = Pens.Black;
        [NonSerialized]
        protected Brush _brush = new SolidBrush(Color.Empty);
        [NonSerialized]
        protected SizeF _symbolSize = Size.Empty;
        [NonSerialized]
        protected float _symbolHalfWidth = 0;
         [NonSerialized]
        protected float _symbolHalfHeight = 0;
        //旋转角度,从x轴正向顺时针为0度
        protected float _angle = 0;

        public Symbol()
        {
            _brush = new SolidBrush(_color);
            _pen = Pens.Black;
        }

        public Symbol(Color color)
        {
            _color = color;
            _pen = new Pen(color);
            _brush = new SolidBrush(color);
        }

        [DisplayName("旋转角度"), Description("旋转角度,从x轴正向顺时针为0度,只对点图层起作用")]
        public float Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }

        [Browsable(false)]
        public SizeF SymbolSize
        {
            get { return _symbolSize; }
        }

        [Browsable(false)]
        public float SymbolHalfWidth
        {
            get { return _symbolHalfWidth; }
        }

        [Browsable(false)]
        public float SymbolHalfHeight
        {
            get { return _symbolHalfHeight; }
        }

        [DisplayName("颜色")]
        public virtual Color Color
        {
            get { return _color; }
            set
            {
                if (_color == value)
                    return;
                _color = value;
                ReBuildPen();
                ReBuildBrush();
            }
        }

        protected virtual void ReBuildPen()
        {
            //if (_pen != null)
            //    _pen.Color = _color;
            //else
            _pen = new Pen(_color);
        }

        protected virtual void ReBuildBrush()
        {
            //if (_brush != null)
            //    (_brush as SolidBrush).Color = _color;
            //else
            _brush = new SolidBrush(_color);
        }

        [Browsable(false)]
        public Pen Pen
        {
            get { return _pen; }
            set
            {
                if (_pen == value)
                    return;
                if (_pen != null)
                    _pen.Dispose();
                _pen = value;
            }
        }

        [Browsable(false)]
        public Brush Brush
        {
            get { return _brush; }
            set
            {
                if (_brush == value)
                    return;
                if (_brush != null)
                    _brush.Dispose();
                _brush = value;
            }
        }

        #region ISymbol 成员

        internal void Render(Graphics g, GraphicsPath path)//polyline,Polygon
        {
            Draw(g, path);
        }

        internal void Render(Graphics g, PointF point)
        {
            if (Math.Abs(_angle) < float.Epsilon)
            {
                Draw(g, point);
            }
            else
            {
                using (Matrix m = g.Transform.Clone())
                {
                    Matrix om = g.Transform;
                    try
                    {
                        m.RotateAt(_angle, point);
                        g.Transform = m;
                        Draw(g, point);
                    }
                    finally
                    {
                        g.Transform = om;
                    }
                }
            }
        }

        public virtual void Draw(Graphics g, GraphicsPath path)//polyline,Polygon
        {
            //
        }

        public virtual void Draw(Graphics g, PointF point)
        { }

        #endregion

        #region IDisposable 成员

        public virtual void Dispose()
        {
            try
            {
                if (_brush != null)
                {
                    _brush.Dispose();
                }
                if (_pen != null)
                {
                    _pen.Dispose();
                }
            }
            catch(Exception ex)
            {
                Log.WriterException("Symbol", "Dispose", ex);
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}", _color.Name);
        }

        #region IPersistable Members

        public abstract PersistObject ToPersistObject();

        #endregion
    }
}
