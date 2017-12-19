using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml.Linq;
using System.ComponentModel;

namespace CodeCell.AgileMap.Core
{
    public class RoundRectContainerSym:ContainerSymbol
    {
        protected ILineSymbol _lineSymbol = null;
        protected IFillSymbol _fillSymbol = null;

        public RoundRectContainerSym()
        {
            _lineSymbol = new SimpleLineSymbol(Color.FromArgb(253,217,70));
            _fillSymbol = new SimpleFillSymbol(Color.FromArgb(254,247,221));
        }

        public RoundRectContainerSym(SizeF size,ILineSymbol outlineSymbol,IFillSymbol fillSymbol)
        {
            _sizef = size;
            _lineSymbol = outlineSymbol;
            _fillSymbol = fillSymbol;
        }

        [DisplayName("外边线"),TypeConverter(typeof(ExpandableObjectConverter))]
        public ILineSymbol OutlineSymbol
        {
            get { return _lineSymbol; }
            set 
            {
                if(value != null)
                    _lineSymbol = value; 
            }
        }

        [DisplayName("内填充"), TypeConverter(typeof(ExpandableObjectConverter))]
        public IFillSymbol FillSymbol
        {
            get { return _fillSymbol; }
            set 
            {
                if (value != null)
                    _fillSymbol = value;
            }
        }

        public override SizeF Draw(Graphics g, PointF location,SizeF size)
        {
            if (_isFixedSize)
                size = _sizef;
            using (GraphicsPath path = GetRoundRectangle(new RectangleF(location.X, location.Y, size.Width, size.Height),12))
            {
                if (_fillSymbol != null)
                    _fillSymbol.Draw(g, path);
                if (_lineSymbol != null)
                    _lineSymbol.Draw(g, path);
            }
            return size;
        }

        private GraphicsPath GetRoundRectangle(RectangleF rect, int arcLen)
        {
            GraphicsPath path = new GraphicsPath();
            RectangleF arcRect = new RectangleF();
            arcRect.Width = arcRect.Height = arcLen;
            //leftUp
            arcRect.Location = new PointF(rect.Left, rect.Top);
            path.AddArc(arcRect, 180, 90);
            //leftRight
            arcRect.Location = new PointF(rect.Right - arcLen, rect.Top);
            path.AddArc(arcRect, 270, 90);
            //rightDown 
            arcRect.Location = new PointF(rect.Right - arcLen, rect.Bottom - arcLen);
            path.AddArc(arcRect, 0, 90);
            //leftDown
            arcRect.Location = new PointF(rect.Left, rect.Bottom - arcLen);
            path.AddArc(arcRect, 90, 90);
            path.CloseAllFigures();
            return path;
        }

        public override PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("ContainerSymbol");
            obj.AddAttribute("type", this.GetType().ToString());
            obj.AddAttribute("size", _sizef.Width.ToString() + "," + _sizef.Height.ToString());
            obj.AddAttribute("fixedsize", _isFixedSize.ToString());
            if (_lineSymbol != null)
            {
                PersistObject objline = new PersistObject("OutlineSymbol");
                obj.AddSubNode(objline);
                objline.AddSubNode(_lineSymbol.ToPersistObject());
            }
            if (_fillSymbol != null)
            {
                PersistObject objfill = new PersistObject("FillSymbol");
                obj.AddSubNode(objfill);
                objfill.AddSubNode(_fillSymbol.ToPersistObject());
            }
            return obj;
        }

        public static IContainerSymbol FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            string[] sizeString = ele.Attribute("size").Value.Split(',');
            SizeF size = new SizeF(float.Parse(sizeString[0]), float.Parse(sizeString[1]));
            bool isFixed = bool.Parse(ele.Attribute("fixedsize").Value);
            ILineSymbol symline = null;
            IFillSymbol symfill = null;
            if (ele.Element("OutlineSymbol") != null)
                symline = PersistObject.ReflectObjFromXElement(ele.Element("OutlineSymbol").Element("Symbol")) as ILineSymbol;
            if (ele.Element("FillSymbol") != null)
                symfill = PersistObject.ReflectObjFromXElement(ele.Element("FillSymbol").Element("Symbol")) as IFillSymbol;
            RoundRectContainerSym sym = new RoundRectContainerSym(size, symline, symfill);
            sym.IsFixedSize = isFixed;
            return sym;
        }
    }
}
