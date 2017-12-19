using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel.Composition;
using System.ComponentModel;
using CodeCell.AgileMap.Core;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.Layout.DataFrm
{
    public abstract class DrawingElement : SizableElement, IDrawingElement, IPersitable
    {
        protected string _layerName;
        protected IDataFrame _dataFrame;
        protected const int DEFAULT_WIDTH = 60;
        protected const int DEFAULT_HEIGHT = 30;
        protected bool _isSimpleLegend = true;
        protected bool _isShowBorder = true;
        protected string _dataFrameId;

        public DrawingElement()
            : base()
        {
        }

        public void SetDataFrame(IDataFrame dataFrame)
        {
            _dataFrame = dataFrame;
        }

        [Persist(), Browsable(false)]
        public string DataFrameId
        {
            get { return _dataFrameId; }
            set { _dataFrameId = value; }
        }

        [Persist(), DisplayName("引用图层"), Category("数据")]
        public string RefLayerName
        {
            get { return _layerName; }
            set
            {
                _layerName = value;
                Update();
            }
        }

        [Persist(), DisplayName("使用简单图例"), Category("设计")]
        public bool IsSimpleLegend
        {
            get { return _isSimpleLegend; }
            set { _isSimpleLegend = value; }
        }

        [Persist(), DisplayName("是否显示边框"), Category("布局")]
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
            TrySetDataFrameById((sender as ILayoutRuntime).GetHost());
            if (_size.IsEmpty)
            {
                _size = new SizeF(DEFAULT_WIDTH, DEFAULT_HEIGHT);
                float w = _size.Width;
                float h = _size.Height;
                drawArgs.Runtime.Pixel2Layout(ref w, ref h);
                _size = new SizeF(w, h);
            }
            BeginRotate(drawArgs);
            try
            {
                float x = _location.X, y = _location.Y;
                drawArgs.Runtime.Layout2Screen(ref x, ref y);
                float w = _size.Width;
                float h = _size.Height;
                drawArgs.Runtime.Layout2Screen(ref w);
                drawArgs.Runtime.Layout2Screen(ref h);
                DrawLegend(g, x, y, w, h, drawArgs);
            }
            finally
            {
                EndRotate(drawArgs);
            }
        }

        private void TrySetDataFrameById(ILayoutHost host)
        {
            if (_dataFrame != null)
            {
                if (string.IsNullOrEmpty(_dataFrameId))
                    _dataFrameId = _dataFrame.Name;
                return;
            }
            else if (!string.IsNullOrEmpty(_dataFrameId))
            {
                IElement[] eles = host.LayoutRuntime.QueryElements((ele) =>
                {
                    IDataFrame df = ele as IDataFrame;
                    return df != null && df.Name == _dataFrameId;
                },
                true);
                if (eles != null && eles.Length > 0)
                {
                    _dataFrame = eles[0] as IDataFrame;
                    _dataFrameId = _dataFrame.Name;
                }
            }
        }

        private void DrawLegend(Graphics g, float x, float y, float width, float height, IDrawArgs drawArgs)
        {
            ISymbol sym = GetSymbol();
            if (_isShowBorder || sym == null)
                g.DrawRectangle(Pens.Black, x, y, width, height);
            if (sym == null)
                return;
            if (sym is IMarkerSymbol)
            {
                IMarkerSymbol msym = sym as IMarkerSymbol;
                x = x + width / 2f;
                y = y + height / 2f;
                sym.Draw(g, new PointF(x,y));
            }
            else if (sym is ILineSymbol || sym is IFillSymbol)
            {
                using (GraphicsPath path = GetGraphicsPath(sym, x, y, width, height))
                {
                    if (path != null)
                        sym.Draw(g, path);
                }
            }
        }

        protected abstract ISymbol GetSymbol();

        private GraphicsPath GetGraphicsPath(ISymbol sym, float x, float y, float width, float height)
        {
            if (_isSimpleLegend)
                return GetGraphicsPathForSimpleLegend(sym, x, y, width, height);
            GraphicsPath pth = GetGraphicsPathFromFeatureType(x, y, width, height);
            if (pth == null)
            {
                _isSimpleLegend = true;
                return GetGraphicsPathForSimpleLegend(sym, x, y, width, height);
            }
            return pth;
        }

        protected virtual GraphicsPath GetGraphicsPathFromFeatureType(float x, float y, float width, float height)
        {
            return null;
        }

        private GraphicsPath GetGraphicsPathForSimpleLegend(ISymbol sym, float x, float y, float width, float height)
        {
            GraphicsPath pth = new GraphicsPath();
            if (sym is ILineSymbol)
            {
                float offsetY = (height - (sym as ILineSymbol).Width) / 2;
                float offsetX = 3;
                pth.AddLine(x + offsetX, y + offsetY, x + width - 2 * offsetX, y + offsetY);
            }
            else if (sym is IFillSymbol)
            {
                float offsetY = 3;
                float offsetX = 3;
                pth.AddRectangle(new RectangleF(x + offsetX, y + offsetY, width - 2 * offsetX, height - 2 * offsetY));
            }
            return pth;
        }

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            if (xml.Attribute("reflayername") != null)
                _layerName = xml.Attribute("reflayername").Value;
            if (xml.Attribute("dataframeid") != null)
                _dataFrameId = xml.Attribute("dataframeid").Value;
            string value = string.Empty;
            if (xml.Attribute("issimplelegend") != null)
            {
                value = xml.Attribute("issimplelegend").Value;
                _isSimpleLegend = StringToBool(value);
            }
            if (xml.Attribute("isshowborder") != null)
            {
                value = xml.Attribute("isshowborder").Value;
                _isShowBorder = StringToBool(value);
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
    }
}
