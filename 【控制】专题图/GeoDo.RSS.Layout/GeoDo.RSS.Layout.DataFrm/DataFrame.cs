using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using GeoDo.RSS.Core.View;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DrawEngine;
using System.ComponentModel;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;
using GeoDo.RSS.Layout.GDIPlus;
using CodeCell.AgileMap.Core;
using System.IO;
using GeoDo.RSS.Core.VectorDrawing;
using System.Xml;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using GeoDo.RSS.Core;
using GeoDo.Core;
using GeoDo.RSS.Core.Grid;

namespace GeoDo.RSS.Layout.DataFrm
{
    public class DataFrame : SizableElement, IDataFrame,
        IControlTool, IPersitable, ILayerObjectContainer,
        IAsyncDataArrivedNotify
    {
        protected ILayoutHost _host;
        protected IDataFrameDataProvider _provider;
        protected SizeF _preSize = System.Drawing.SizeF.Empty;
        protected int _borderWidth = 1;
        private bool _isShowBorderLine = true;
        protected Color _borderColor = Color.FromArgb(0, 0, 0);
        protected Pen _borderPen;
        protected bool _isInsideProvider = true;
        /*
         * 以下四个参数用于背景层
         * 注：下次重构时单独提成一个层保存
         */
        protected bool _isUseDefaultBackgroudLayer = true;
        protected Color _landColor = Color.FromArgb(255, 255, 255);
        protected Color _seaColor = Color.FromArgb(160, 190, 228);
        protected string[] _interestRegions;
        //
        protected float _offsetX = 0;
        protected float _offsetY = 0;
        protected SizeF _preApplyedSize;
        public static Action<string, object, ICanvas, string[], string> AddFileToCanvasViewerExecutor;
        private ILayerObjectContainer _layerObjectContainer;
        private string _spatialRef;
        private object _data;
        private XElement _gridXml;
        private XElement _documentableLayersHostXml;
        private bool _isSyncRefreshData = false;
        protected Color _backColor = Color.White;
        private int renderCount = 0;

        static DataFrame()
        {
            LayoutHost.LoadGeoGridLayerExecutor = (xml) =>
            {
                return GeoDo.RSS.Core.Grid.GeoGridLayer.FromXml(xml);
            };

            LayoutHost.LoadDataFrameExecutor = (gxfile, host, df, dfEle) =>
            {
                IDataFrameDataProvider prd = df.Provider as IDataFrameDataProvider;
                ICanvas canvas = prd.Canvas;
                string fname = Path.Combine(Path.GetDirectoryName(gxfile), "tempMcd.xml");
                XElement mapElement = dfEle.Element("Map");
                //旧模版没有Map节点
                if (mapElement != null)
                {
                    File.WriteAllText(fname, mapElement.ToString());
                    if (canvas == null)
                        return;
                    if (canvas.LayerContainer.VectorHost == null)
                    {
                        IVectorHostLayer vhost = new VectorHostLayer(null, fname);
                        vhost.IsEnableDummyRender = false;
                        vhost.SomeDataIsArrivedHandler += new EventHandler((sender, e) => { host.Render(true); });
                        canvas.LayerContainer.Layers.Add(vhost as GeoDo.RSS.Core.DrawEngine.ILayer);
                    }
                }
                double minX = GetDoubleAtt(dfEle, "minx");
                double maxX = GetDoubleAtt(dfEle, "maxx");
                double minY = GetDoubleAtt(dfEle, "miny");
                double maxY = GetDoubleAtt(dfEle, "maxy");
                if (Math.Abs(minX) > double.Epsilon &&
                    Math.Abs(minY) > double.Epsilon &&
                    Math.Abs(maxX) > double.Epsilon &&
                    Math.Abs(maxY) > double.Epsilon)
                {
                    canvas.CurrentEnvelope = new CoordEnvelope(minX, maxX, minY, maxY);
                }
                //
                df.IsLocked = true;
            };

            //模版保存时调用
            LayoutToFile.DataFrame2XmlNodeConverter = (dstFileName, df, doc) =>
            {
                if (df == null || doc == null)
                    return null;
                IDataFrameDataProvider prd = df.Provider as IDataFrameDataProvider;
                if (prd == null)
                    return null;
                ICanvas canvas = prd.Canvas;
                if (canvas == null)
                    return null;
                XmlElement dfNode = doc.CreateElement("DataFrame");
                dfNode.SetAttribute("name", df.Name);
                dfNode.SetAttribute("minx", canvas.CurrentEnvelope.MinX.ToString());
                dfNode.SetAttribute("miny", canvas.CurrentEnvelope.MinY.ToString());
                dfNode.SetAttribute("maxx", canvas.CurrentEnvelope.MaxX.ToString());
                dfNode.SetAttribute("maxy", canvas.CurrentEnvelope.MaxY.ToString());
                //
                IVectorHostLayer hostLayer = canvas.LayerContainer.VectorHost;
                if (hostLayer != null)
                {
                    IMap map = hostLayer.Map as IMap;
                    if (map == null)
                        return null;
                    string fname = Path.Combine(Path.GetDirectoryName(dstFileName), "TempMcd.xml");
                    StringBuilder sb = new StringBuilder();
                    try
                    {
                        map.SaveTo(fname, true);
                        string[] lines = File.ReadAllLines(fname);
                        for (int i = 1; i < lines.Length; i++)
                            sb.AppendLine(lines[i]);
                    }
                    finally
                    {
                        if (File.Exists(fname))
                            File.Delete(fname);
                    }
                    //
                    dfNode.InnerXml = sb.ToString();
                }
                //
                return dfNode;
            };

            GxdDocument.GxdVectorHostGettter = (df) =>
            {
                IDataFrameDataProvider provider = df.Provider as IDataFrameDataProvider;
                if (provider == null)
                    return null;
                ICanvas c = provider.Canvas;
                if (c == null)
                    return null;
                IVectorHostLayer hostLayer = c.LayerContainer.VectorHost;
                if (hostLayer == null)
                    return null;
                IMap map = hostLayer.Map as IMap;
                if (map == null)
                    return null;
                string fname = AppDomain.CurrentDomain.BaseDirectory + "TempMcd.xml";
                //这里相对路径不正确
                map.SaveTo(fname, false);
                string[] lines = File.ReadAllLines(fname);
                StringBuilder sb = new StringBuilder();
                for (int i = 1; i < lines.Length; i++)
                    sb.AppendLine(lines[i]);
                return new GxdVectorHost(sb.ToString());
            };
            //
            GxdDocument.GxdEnvelopeGetter = (df) =>
            {
                IDataFrameDataProvider provider = df.Provider as IDataFrameDataProvider;
                if (provider == null)
                    return null;
                ICanvas c = provider.Canvas;
                if (c == null)
                    return null;
                return new GxdEnvelope(c.CurrentEnvelope.MinX, c.CurrentEnvelope.MaxX, c.CurrentEnvelope.MinY, c.CurrentEnvelope.MaxY);
            };
            //
            GxdDocument.GxDataFrameRasterItemsSetter = (df, gxddf) =>
            {
                IDataFrameDataProvider provider = df.Provider as IDataFrameDataProvider;
                if (provider == null)
                    return;
                ICanvas c = provider.Canvas;
                if (c == null)
                    return;
                IRasterDrawing drawing = c.PrimaryDrawObject as IRasterDrawing;
                if (drawing == null)
                    return;
                IGxdRasterItem it = new GxdRasterItem(drawing.FileName, null);
                gxddf.GxdRasterItems.Add(it);
            };
            //
            GxdDocument.GxdAddDataFrameExecutor = (fileName, gxdf, host) =>
            {
                if (gxdf == null || host == null)
                    return;
                IElement[] dfs = host.LayoutRuntime.QueryElements((e) => { return e is IDataFrame; }, false);
                if (dfs == null || dfs.Length == 0)
                    return;
                foreach (IElement e in dfs)
                {
                    if (e.Name != null && gxdf.Name != null && e.Name == gxdf.Name)
                    {
                        IDataFrame crtDataFrame = e as IDataFrame;
                        if (crtDataFrame.Provider == null)
                            continue;
                        crtDataFrame.IsLocked = true;
                        ICanvas canvas = (crtDataFrame.Provider as IDataFrameDataProvider).Canvas;
                        //set spatial ref
                        SetSpatialRefForDataFrame(gxdf.SpatialRef, crtDataFrame);
                        //raster
                        if (gxdf.GxdRasterItems != null && gxdf.GxdRasterItems.Count > 0)
                        {
                            foreach (IGxdRasterItem rst in gxdf.GxdRasterItems)
                            {
                                if (rst == null || string.IsNullOrEmpty(rst.FileName))
                                    continue;
                                if (AddFileToCanvasViewerExecutor != null)
                                    AddFileToCanvasViewerExecutor(rst.FileName, rst.Arguments, canvas, rst.FileOpenArgs, rst.ColorTableName);
                            }
                        }
                        //vector
                        if (gxdf.GxdVectorHost != null && gxdf.GxdVectorHost.McdFileContent != null)
                        {
                            string fname = Path.Combine(Path.GetDirectoryName(fileName), "tempMcd.xml");
                            File.WriteAllText(fname, gxdf.GxdVectorHost.McdFileContent.ToString());
                            if (canvas.LayerContainer.VectorHost == null)
                            {
                                IVectorHostLayer vhost = new VectorHostLayer(null, fname);
                                vhost.IsEnableDummyRender = false;
                                vhost.SomeDataIsArrivedHandler += new EventHandler((sender, arge) => { host.Render(true); });
                                canvas.LayerContainer.Layers.Add(vhost as GeoDo.RSS.Core.DrawEngine.ILayer);
                            }
                        }
                        //coord envelope
                        if (gxdf.Envelope != null && !gxdf.Envelope.IsEmpty())
                        {
                            GeoDo.RSS.Core.DrawEngine.CoordEnvelope evp = new CoordEnvelope(gxdf.Envelope.MinX, gxdf.Envelope.MaxX, gxdf.Envelope.MinY, gxdf.Envelope.MaxY);
                            canvas.CurrentEnvelope = evp;
                        }
                        //vector host
                        if (canvas.LayerContainer.VectorHost == null)
                        {
                            IVectorHostLayer vhost = new VectorHostLayer(null);
                            vhost.IsEnableDummyRender = false;
                            vhost.SomeDataIsArrivedHandler += new EventHandler((sender, arge) => { host.Render(true); });
                            canvas.LayerContainer.Layers.Add(vhost as GeoDo.RSS.Core.DrawEngine.ILayer);
                        }
                        //documentable layers
                        XElement otherLayersHost = crtDataFrame.GetDocumentableLayersHostXml();
                        if (otherLayersHost != null)
                        {
                            Object2Xml obj2xml = new Object2Xml();
                            foreach (XElement otherLyr in otherLayersHost.Elements())
                            {
                                //IDocumentableLayer lyr = obj2xml.FromXml(otherLyr) as IDocumentableLayer;
                                //if (lyr != null)
                                //    lyr.Load();
                            }
                        }
                        break;
                    }
                }
            };
        }

        private static double GetDoubleAtt(System.Xml.Linq.XElement dfEle, string attName)
        {
            if (dfEle == null || string.IsNullOrEmpty(attName))
                return 0;
            if (dfEle.Attribute(attName) != null)
            {
                double d = 0;
                if (double.TryParse(dfEle.Attribute(attName).Value, out d))
                    return d;
            }
            return 0;
        }

        private static void SetSpatialRefForDataFrame(string spatialRef, IDataFrame dataFrame)
        {
            if (dataFrame == null)
                return;
            ICanvas canvas = (dataFrame.Provider as IDataFrameDataProvider).Canvas;
        }

        public DataFrame()
        {
            _name = Guid.NewGuid().ToString();
        }

        public DataFrame(ILayoutHost host)
        {
            _name = Guid.NewGuid().ToString();
            _isInsideProvider = true;
            _host = host;
            if (_host.LayoutRuntime == null || _host.LayoutRuntime.Layout == null)
                _size = new System.Drawing.SizeF(300, 300);
            else
            {
                ILayout layout = _host.LayoutRuntime.Layout;
                _size = new SizeF(layout.Size.Width / 2f, layout.Size.Height / 2f);
            }
            Update(_host);
            ReCreateBorderPen();
        }

        public DataFrame(ILayoutHost host, IDataFrameDataProvider provider)
        {
            _name = Guid.NewGuid().ToString();
            _host = host;
            _size = new System.Drawing.SizeF(300, 300);
            _isInsideProvider = false;
            _provider = provider;
            TrySetAsyncDataArrivedNotify();
            ReCreateBorderPen();
        }

        [Persist(), DisplayName("使用默认背景"), Category("外观")]
        public bool IsUseDefaultBackgroudLayer
        {
            get { return _isUseDefaultBackgroudLayer; }
            set { _isUseDefaultBackgroudLayer = value; }
        }

        [Persist(enumAttType.UnValueType), DisplayName("陆地颜色"), Category("外观")]
        public Color LandColor
        {
            get { return _landColor; }
            set { _landColor = value; }
        }

        [Persist(enumAttType.UnValueType), DisplayName("海洋颜色"), Category("外观")]
        public Color SeaColor
        {
            get { return _seaColor; }
            set { _seaColor = value; }
        }

        [Persist(enumAttType.UnValueType), DisplayName("关注水体"), Category("外观")]
        public string[] InterestRegions
        {
            get { return _interestRegions; }
            set { _interestRegions = value; }
        }

        [Persist(enumAttType.UnValueType), DisplayName("背景颜色"), Category("外观")]
        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        [Persist(enumAttType.ValueType), DisplayName("显示边框"), Category("外观")]
        public bool IsShowBorderLine
        {
            get { return _isShowBorderLine; }
            set { _isShowBorderLine = value; }
        }

        [Browsable(false)]
        public object Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public void Update(ILayoutHost host)
        {
            _host = host;
            ReCreateBorderPen();
            if (_isInsideProvider)
            {
                if (_provider != null)
                    _provider.Canvas.Dispose();
                _provider = new DataFrameDataProvider(this, _host.LayoutRuntime,
                    _isUseDefaultBackgroudLayer, _landColor, _seaColor, _interestRegions);
                _isInsideProvider = true;
                float w = _size.Width, h = _size.Height;
                _host.LayoutRuntime.Layout2Pixel(ref w, ref h);
                (_provider as UserControl).Size = new Size((int)w, (int)h);
                TrySetAsyncDataArrivedNotify();
            }
        }

        public void UpdateCanves()
        {
            if (!IsDoubleBorderLine)
                return;
            CoordEnvelope env = _provider.Canvas.CurrentEnvelope;
            double xOffect = (env.Width * (_size.Width + InSize.Width) / _size.Width - env.Width) / 2;
            double yOffect = (env.Height * (_size.Height + InSize.Height) / _size.Height - env.Height) / 2;
            _provider.Canvas.CurrentEnvelope = new CoordEnvelope(env.MinX - xOffect, env.MaxX + xOffect, env.MinY - yOffect, env.MaxY + yOffect);
            TrySetAsyncDataArrivedNotify();
        }

        private void TrySetAsyncDataArrivedNotify()
        {
            if (_provider == null)
                return;
            ICanvas c = _provider.Canvas;
            if (c == null)
                return;
            IVectorHostLayer host = c.LayerContainer.VectorHost;
            if (host == null)
                return;
            IMapRuntime runtime = host.MapRuntime as IMapRuntime;
            if (runtime == null)
                return;
            runtime.AsyncDataArrivedNotify = this;
        }

        [Browsable(false)]
        public string SpatialRef
        {
            get { return _spatialRef; }
            set { _spatialRef = value; }
        }

        [Persist(), DisplayName("边框线宽"), Category("布局")]
        public int BorderWidth
        {
            get { return _borderWidth; }
            set
            {
                if (_borderWidth != value)
                {
                    _borderWidth = value;
                    ReCreateBorderPen();
                }
            }
        }

        [Persist(enumAttType.UnValueType), DisplayName("边框颜色"), Category("外观")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                if (_borderColor != value)
                {
                    _borderColor = value;
                    ReCreateBorderPen();
                }
            }
        }

        /// <summary>
        /// 比例尺(分母)
        /// </summary>
        [Browsable(false)]
        public float LayoutScale
        {
            get
            {
                return ComputeLayoutScale();
            }
            set
            {
                SetLayoutScale(value);
            }
        }

        private void SetLayoutScale(float value)
        {
            ICanvas cavas = _provider.Canvas;
            if (cavas == null)
                return;
            float scale = 1f;
            if (cavas.PrimaryDrawObject != null)
            {
                float meters = (cavas.Container.Width / _host.DPI) * LayoutRuntime.INCH_TO_CENTIMETER / 100f;
                scale = (float)(meters / (cavas.PrimaryDrawObject.OriginalEnvelope.Width / value));
                scale *= cavas.PrimaryObjectScale;
            }
            else
            {
                float meters = (cavas.Container.Width / _host.DPI) * LayoutRuntime.INCH_TO_CENTIMETER / 100f;
                scale = (float)(meters / (cavas.CurrentEnvelope.Width / value));
            }
            cavas.Scale = scale;
            cavas.Refresh(enumRefreshType.All);
        }

        private float ComputeLayoutScale()
        {
            ICanvas cavas = _provider.Canvas;
            if (cavas == null)
                return 0f;
            float metersPerPixel = (float)(cavas.CurrentEnvelope.Width / cavas.Container.Width);
            float metersPerInch = metersPerPixel * _host.DPI;
            return metersPerInch / (LayoutRuntime.INCH_TO_CENTIMETER / 100f);//1m = 100cm
        }

        public static float Resolution2Scale(float resMeters, int dpi)
        {
            return dpi * resMeters * 100f / LayoutRuntime.INCH_TO_CENTIMETER;

        }

        private void ReCreateBorderPen()
        {
            if (_borderPen != null)
            {
                _borderPen.Dispose();
                _borderPen = null;
            }
            _borderPen = new Pen(_borderColor, _borderWidth);
        }

        [Browsable(false)]
        public object Provider
        {
            get { return _provider; }
        }

        public XElement GetDocumentableLayersHostXml()
        {
            XElement ele = new XElement("DocumentableLayersHost");
            if (_provider == null)
                return ele;
            ICanvas c = _provider.Canvas;
            if (c == null)
                return ele;
            Object2Xml obj2xml = new Object2Xml();
            foreach (GeoDo.RSS.Core.DrawEngine.ILayer lyr in c.LayerContainer.Layers)
            {
                //if (lyr is IDocumentableLayer)
                //{
                //    ele.Add(obj2xml.ToXml(lyr));
                //}
            }
            return ele;
        }

        public XElement GetGridXml()
        {
            if (_provider == null)
                return null;
            ICanvas c = _provider.Canvas;
            if (c == null)
                return null;
            IGridLayer gridLayer = c.LayerContainer.Get((lyr) => { return lyr is IGridLayer; }) as IGridLayer;
            if (gridLayer == null)
                return null;
            (gridLayer as IGridLayerAtrEdit).DrawDoubleBorder(IsDoubleBorderLine, InSize);
            return gridLayer.ToXml();
        }

        [Browsable(false)]
        public XElement GeoGridXml
        {
            get { return _gridXml; }
        }

        public override void Render(object sender, IDrawArgs drawArgs)
        {
            if (_provider == null)
                return;
            if (_runtime == null)
                _runtime = drawArgs.Runtime;
            //
            _isSyncRefreshData = DrawArgs.IsStrongRefreshData;
            //
            Graphics g = drawArgs.Graphics as Graphics;
            if (_provider != null && _provider.Canvas.CanvasSetting != null)//
                _provider.Canvas.CanvasSetting.RenderSetting.BackColor = _backColor;
            BeginRotate(drawArgs);
            float x = _location.X, y = _location.Y, w = _size.Width, h = _size.Height;
            drawArgs.Runtime.Layout2Screen(ref x, ref y);
            drawArgs.Runtime.Layout2Screen(ref w);
            drawArgs.Runtime.Layout2Screen(ref h);
            //x += 0.5f;
            //y += 0.5f;
            if (_isInsideProvider)
            {
                CheckSizeIsChanged();
                g.DrawImage(_provider.GetBuffer(_isSyncRefreshData),
                    (int)x,
                    (int)y,
                    (int)w,
                    (int)h);
            }
            else
            {
                Bitmap bitmap = _provider.GetBuffer(_isSyncRefreshData);
                float scaleX = w / bitmap.Width;
                float scaleY = h / bitmap.Height;
                float scale = Math.Min(scaleX, scaleY);
                _offsetX = (bitmap.Width * scale - w) / 2;
                _offsetY = (bitmap.Height * scale - h) / 2;
                g.DrawImage(bitmap, x + _offsetX, y + _offsetY, bitmap.Width * scale, bitmap.Height * scale);
            }
            //
            if (_isShowBorderLine)
                g.DrawRectangle(_borderPen, x, y, w, h);
            // 
            DrawGrid();
            //
            //UpdateCanves();
            //
            EndRotate(drawArgs);
        }

        public void DrawGrid()
        {
            if (_provider == null)
                return;
            ICanvas c = _provider.Canvas;
            if (c == null)
                return;
            IGridLayer gridLayer = c.LayerContainer.Get((lyr) => { return lyr is IGridLayer; }) as IGridLayer;
            if (gridLayer == null)
                return;
            (gridLayer as IGridLayerAtrEdit).DrawDoubleBorder(IsDoubleBorderLine, InSize);
        }

        private void CheckSizeIsChanged()
        {
            if (!_preSize.IsEmpty && (Math.Abs(_size.Width - _preSize.Width) > float.Epsilon || Math.Abs(_size.Height - _preSize.Height) > float.Epsilon))
            {
                float w = _size.Width, h = _size.Height;
                _host.LayoutRuntime.Layout2Screen(ref w);
                if (w < float.Epsilon + 1)
                    w = 1;
                _host.LayoutRuntime.Layout2Screen(ref h);
                if (h < float.Epsilon + 11)
                    h = 1;
                (_provider as UserControl).Size = new Size((int)w, (int)h);
            }
            _preSize = _size;
        }

        [Browsable(false)]
        public Cursor Cursor
        {
            get { return Cursors.Default; }
        }

        public override void Event(object sender, enumCanvasEventType eventType, CanvasEventArgs e)
        {
            if (_runtime == null)
                return;
            float x0 = _location.X, y0 = _location.Y;
            _runtime.Layout2Screen(ref x0, ref y0);
            float x = e.ScreenX - x0;
            float y = e.ScreenY - y0;
            if (!_isInsideProvider)
            {
                x += _offsetX;
                y += _offsetY;
            }
            MouseButtons mouseButtons = (e.E is MouseEventArgs) ? (e.E as MouseEventArgs).Button : Control.MouseButtons;

            IControlMessageAccepter msgAccepter = (_provider as IDataFrameDataProvider).Canvas as IControlMessageAccepter;
            switch (eventType)
            {
                case enumCanvasEventType.MouseWheel:
                    msgAccepter.AcceptMouseWheel(this, new MouseEventArgs(mouseButtons, 0, (int)x, (int)y, e.WheelDelta));
                    (_provider as IDataFrameDataProvider).Canvas.Refresh(enumRefreshType.All);
                    _host.Render();
                    break;
                case enumCanvasEventType.MouseDown:
                    msgAccepter.AcceptMouseDown(this, new MouseEventArgs(mouseButtons, 0, (int)x, (int)y, e.WheelDelta));
                    break;
                case enumCanvasEventType.MouseMove:
                    msgAccepter.AcceptMouseMove(this, new MouseEventArgs(mouseButtons, 0, (int)x, (int)y, e.WheelDelta));
                    (_provider as IDataFrameDataProvider).Canvas.Refresh(enumRefreshType.All);
                    _host.Render();
                    break;
                case enumCanvasEventType.MouseUp:
                    msgAccepter.AcceptMouseUp(this, new MouseEventArgs(mouseButtons, 0, (int)x, (int)y, e.WheelDelta));
                    (_provider as IDataFrameDataProvider).Canvas.Refresh(enumRefreshType.All);
                    _host.Render();
                    break;
                case enumCanvasEventType.KeyDown:
                    msgAccepter.AcceptKeyDown(this, new KeyEventArgs(Keys.G));
                    (_provider as IDataFrameDataProvider).Canvas.Refresh(enumRefreshType.All);
                    _host.Render();
                    break;
            }
        }

        public override void Dispose()
        {
            if (_borderPen != null)
            {
                _borderPen.Dispose();
                _borderPen = null;
            }
            if (_provider != null)
            {
                if (_isInsideProvider)
                    (_provider as IDisposable).Dispose();
                _provider = null;
            }
            if (_lockImage != null)
            {
                _lockImage.Dispose();
                _lockImage = null;
            }
            _runtime = null;
            _host = null;
            base.Dispose();
        }

        public override void InitByXml(System.Xml.Linq.XElement xml)
        {
            if (xml.Attribute("bordercolor") != null)
            {
                _borderColor = (Color)LayoutFromFile.Base64StringToObject(xml.Attribute("bordercolor").Value);
                ReCreateBorderPen();
            }
            if (xml.Attribute("landcolor") != null)
            {
                _landColor = (Color)LayoutFromFile.Base64StringToObject(xml.Attribute("landcolor").Value);
            }
            if (xml.Attribute("seacolor") != null)
            {
                _seaColor = (Color)LayoutFromFile.Base64StringToObject(xml.Attribute("seacolor").Value);
            }
            if (xml.Attribute("interestregions") != null)
            {
                _interestRegions = (string[])LayoutFromFile.Base64StringToObject(xml.Attribute("interestregions").Value);
            }
            if (xml.Attribute("borderwidth") != null)
                _borderWidth = int.Parse(xml.Attribute("borderwidth").Value);
            if (xml.Attribute("isusedefaultbackgroudlayer") != null)
            {
                _isUseDefaultBackgroudLayer = bool.Parse(xml.Attribute("isusedefaultbackgroudlayer").Value);
            }
            if (xml.Attribute("backcolor") != null)
            {
                _backColor = (Color)LayoutFromFile.Base64StringToObject(xml.Attribute("backcolor").Value);
            }
            if (xml.Attribute("isshowborderline") != null)
            {
                var att = xml.Attribute("isshowborderline").Value;
                if (att != null)
                    _isShowBorderLine = bool.Parse(att);
            }
            if (xml.HasElements)
                this._gridXml = xml.Element("GeoGridLayer");
            base.InitByXml(xml);
        }

        [Browsable(false)]
        List<ILayerObjectBase> ILayerObjectContainer.LayerObjects
        {
            get
            {
                if (_layerObjectContainer == null)
                    _layerObjectContainer = new LayerObjectContainer(this);
                return _layerObjectContainer.LayerObjects;
            }
        }

        public void NotifyInitFinished()
        {
            TrySetAsyncDataArrivedNotify();
        }

        //矢量数据异步加载后刷新(从矢量绘图引擎中MapRuntime中调用)
        public void SomeDataIsArrived()
        {
            _host.Render();
        }

        public void SyncAttrbutes()
        {
            if (_provider == null)
                return;
            IBackgroundLayer bkLayer = GetBackgroundLayer();
            if (bkLayer == null)
                return;
            _isUseDefaultBackgroudLayer = false;
            BackgroundLayer lyr = bkLayer as BackgroundLayer;
            if (lyr != null)
            {
                _seaColor = lyr.SeaColor;
                _landColor = lyr.LandColor;
                _interestRegions = lyr.InterestRegions;
            }
            _isUseDefaultBackgroudLayer = true;
        }

        private IBackgroundLayer GetBackgroundLayer()
        {
            if (_provider == null)
                return null;
            return _provider.Canvas.LayerContainer.GetByName("海陆背景") as IBackgroundLayer;
        }
    }
}
