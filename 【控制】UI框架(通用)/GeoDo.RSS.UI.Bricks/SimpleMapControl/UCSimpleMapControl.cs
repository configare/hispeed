using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.VectorDrawing;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.RasterDrawing;
using System.IO;
using GeoDo.RSS.Core.UI;
using System.Threading;
using GeoDo.RSS.Core.Grid;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;

namespace GeoDo.RSS.UI.Bricks
{
    public partial class UCSimpleMapControl : UserControl, ISimpleMapControl
    {
        public event Action<MouseEventArgs, double, double> MouseDoubleClickMap;
        public event EventHandler AOIIsChanged;
        public event Action<object, CoordEnvelope> MapSelectedIsChanged;

        private CoordEnvelope _drawedAOI;
        private Dictionary<string, ISimpleVectorObjectHost> _objHosts = new Dictionary<string, ISimpleVectorObjectHost>();
        private IVectorHostLayer _vectorHostLayer;
        private GeoGridLayer _geoGridLayer;
        private IAOIContainerLayer _aoiContainer;
        private Font _normalFont = new Font("微软雅黑", 9);
        private Font _selectedFont = new Font("微软雅黑", 9, FontStyle.Bold);
        private bool _isAllowAOI = true;
        private string _curView = "中国视图";//世界视图
        private Color _gridColor = Color.Gray;

        public UCSimpleMapControl()
        {
            InitializeComponent();
            canvasHost1.Load += new EventHandler(canvasHost1_Load);
            canvasHost1.MouseDoubleClick += new MouseEventHandler(canvasHost1_MouseDoubleClick);
            Load += new EventHandler(UCSimpleMapControl_Load);
            SizeChanged += new EventHandler(UCSimpleMapControl_SizeChanged);
            Disposed += new EventHandler(UCSimpleMapControl_Disposed);
            btnSelect.Visible = false;
        }

        public bool IsAllowAOI
        {
            get { return _isAllowAOI; }
            set 
            {
                _isAllowAOI = value;
                btnAOI.Visible = _isAllowAOI;
            }
        }

        public bool IsAllowSelect
        {
            get { return btnSelect.Visible; }
            set
            {
                btnSelect.Visible = value;
            }
        }

        void UCSimpleMapControl_Disposed(object sender, EventArgs e)
        {
            if (_objHosts != null&&_objHosts.Count!=0)
            {
                foreach (string key in _objHosts.Keys)
                {
                    ISimpleVectorObjectHost host = _objHosts[key];
                    if (host != null)
                    {
                        host.Dispose();
                    }
                }
                _objHosts.Clear();
            }
            if (_aoiContainer != null)
            {
                (_aoiContainer as AOIContainerLayer).Dispose();
                _aoiContainer = null;
            }
            if (_vectorHostLayer != null)
            {
                (_vectorHostLayer as VectorHostLayer).Dispose();
                _vectorHostLayer = null;
            }
            if (_geoGridLayer != null)
            {
                _geoGridLayer.Dispose();
                _geoGridLayer = null;
            }
            if (canvasHost1 != null)
            {
                canvasHost1.Canvas.Dispose();
                canvasHost1.Dispose();
                //canvasHost1 = null;
            }
        }

        void UCSimpleMapControl_SizeChanged(object sender, EventArgs e)
        {
            if (canvasHost1.Canvas != null)
            {
                if (_curView == "世界视图")
                    ToWorldViewport();
                else
                    ToChinaViewport();
            }
        }

        void UCSimpleMapControl_Load(object sender, EventArgs e)
        {
            //AOI容器
            _aoiContainer = new AOIContainerLayer();
            _aoiContainer.IsAllowEdit = true;
            _aoiContainer.Color = Color.Red;
            _aoiContainer.LineWidth = 1;
            _aoiContainer.AOIGeometryIsUpdated += new AOIGeometryIsUpdatedHandler
                (
                    (sder, geometry) =>
                    {
                        UpdateDrawedAOI();
                        if (AOIIsChanged != null)
                            AOIIsChanged(this, null);
                    }
                );
            canvasHost1.Canvas.LayerContainer.Layers.Add(_aoiContainer as GeoDo.RSS.Core.DrawEngine.ILayer);
            canvasHost1.Canvas.CurrentViewControl = _aoiContainer as IControlLayer;
            canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        void canvasHost1_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;
            canvasHost1.MouseDoubleClick += new MouseEventHandler(canvasHost1_MouseDoubleClick);
            canvasHost1.Canvas.CanvasSetting.RenderSetting.BackColor = Color.White;
            CreateVectorHost();
            LoadDefaultMap();
        }

        void canvasHost1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            double prjlon;
            double prjlat;
            canvasHost1.Canvas.CoordTransform.Screen2Prj(e.X, e.Y, out prjlon, out prjlat);
            double lon;
            double lat;
            canvasHost1.Canvas.CoordTransform.Prj2Geo(prjlon, prjlat, out lon, out lat);
            if (MouseDoubleClickMap != null)
                MouseDoubleClickMap(e, lon, lat);
        }

        private void LoadDefaultMap()
        {
            string mcdfile = AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\SimpleMap.mcd";
            if (File.Exists(mcdfile))
            {
                _vectorHostLayer.Apply(mcdfile);
            }
            else
                MsgBox.ShowInfo("地图配置文件\"SimpleMap.mcd\"丢失,地图初始化不正确！");
        }

        private void TryApplyDefaultBackgroudLayer()
        {
            string fname = AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\矢量模版\海陆模版.shp";
            if (!File.Exists(fname))
            {
                Console.WriteLine("文件\"" + fname + "\"未找到,无法应用默认背景。");
                return;
            }
            IBackgroundLayer lyr = new BackgroundLayer(fname);
            this.canvasHost1.Canvas.LayerContainer.Layers.Add(lyr);
        }

        private void CreateVectorHost()
        {
            ICanvas c = canvasHost1.Canvas;
            _vectorHostLayer = new VectorHostLayer(null);
            _vectorHostLayer.Set(c);
            c.LayerContainer.Layers.Add(_vectorHostLayer as GeoDo.RSS.Core.DrawEngine.ILayer);
            TryApplyDefaultBackgroudLayer();
            AddGeoGrid(c);
        }

        private void AddGeoGrid(ICanvas c)
        {
            GeoGridLayer lyr = new GeoGridLayer();
            lyr.GridSpan = 5;
            lyr.GridColor = _gridColor;//Color.Gray;
            lyr.LatLabelStep = 2;
            lyr.LonLabelStep = 2;
            c.LayerContainer.Layers.Add(lyr);
            _geoGridLayer = lyr;
        }

        private bool _isAllowPanMap = false;

        public bool IsAllowPanMap
        {
            get 
            {
                 return canvasHost1.Canvas.CurrentViewControl != null;//_isAllowPanMap
            }
            set
            {
                _isAllowPanMap = value;
                UpdateAllowPanMap();
            }
        }

        private void UpdateAllowPanMap()
        {
            if (_isAllowPanMap)
                canvasHost1.Canvas.CurrentViewControl = new DefaultControlLayer();
            else
                canvasHost1.Canvas.CurrentViewControl = null;
        }

        public void ApplyMap(string mcdfile)
        {
            _vectorHostLayer.Apply(mcdfile);
        }

        public CoordEnvelope CurrentEnvelope
        {
            get { return canvasHost1.Canvas.CurrentEnvelope.Clone(); }
            set
            {
                if (value != null)
                    canvasHost1.Canvas.CurrentEnvelope = value.Clone();
            }
        }

        public void ToViewport(CoordEnvelope geoCoordEnvelope)
        {
            canvasHost1.Canvas.CoordTransform.Geo2Prj(geoCoordEnvelope);
            CurrentEnvelope = geoCoordEnvelope;
        }

        public void ToChinaViewport()
        {
            _curView = "中国视图";
            btnToChina.Text = " √ " + "中国视图";
            btnToWorld.Text = "世界视图";
            btnToWorld.Font = _normalFont;
            btnToChina.Font = _selectedFont;
            CoordEnvelope evp = new CoordEnvelope(70, 150, 5, 70);
            _geoGridLayer.GridSpan = 5;
            ToViewport(evp);
        }

        public void ToWorldViewport()
        {
            _curView = "世界视图";
            btnToChina.Text = "中国视图";
            btnToWorld.Text = " √ " + "世界视图";
            btnToWorld.Font = _selectedFont;
            btnToChina.Font = _normalFont;
            //投影坐标时该范围可能有误
            CoordEnvelope evp = new CoordEnvelope(-180, 180, -90, 90);
            _geoGridLayer.GridSpan = 20;
            ToViewport(evp);
        }

        public void Render()
        {
            canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        public void Reset()
        {
            RemoveAllImageLayers();
            _drawedAOI = null;
        }

        public void AddImageLayer(string name, Bitmap bitmap, CoordEnvelope coordEnvelope, bool isGeoCoord)
        {
            IRasterLayer lyr = new SimpleBitmapLayer(name, bitmap, coordEnvelope, isGeoCoord);
            canvasHost1.Canvas.LayerContainer.Layers.Add(lyr);
        }

        public void RemoveImageLayer(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;
            GeoDo.RSS.Core.DrawEngine.ILayer lyr = canvasHost1.Canvas.LayerContainer.GetByName(name);
            if (lyr != null)
                canvasHost1.Canvas.LayerContainer.Layers.Remove(lyr);
        }

        public void RemoveAllImageLayers()
        {
            canvasHost1.Canvas.LayerContainer.Layers.RemoveAll((lyr) => { return lyr is SimpleBitmapLayer; });
        }

        public ISimpleVectorObjectHost CreateObjectHost(string name)
        {
            if (!_objHosts.ContainsKey(name))
                _objHosts.Add(name, new SimpleVectorObjectHost(name, canvasHost1.Canvas));
            return _objHosts[name];
        }

        public CoordEnvelope DrawedAOI
        {
            get { return _drawedAOI; }
            set
            {
                _drawedAOI = value;
                SetAOIGeometry(_drawedAOI);
            }
        }

        private void SetAOIGeometry(CoordEnvelope evp)
        {
            if (evp == null)
                _aoiContainer.Reset();
            else
            {
                double prjX1, prjY1, prjX2, prjY2;
                GeometryOfDrawed geo = new GeometryOfDrawed();
                geo.ShapeType = "Rectangle";
                geo.IsPrjCoord = true;
                canvasHost1.Canvas.CoordTransform.Geo2Prj(evp.MinX, evp.MaxY, out prjX1, out prjY1);
                canvasHost1.Canvas.CoordTransform.Geo2Prj(evp.MaxX, evp.MinY, out prjX2, out prjY2);
                geo.RasterPoints = new PointF[] 
                {
                    new PointF((float)prjX1,(float)prjY1),
                    new PointF((float)prjX2,(float)prjY1),
                    new PointF((float)prjX2,(float)prjY2),
                    new PointF((float)prjX1,(float)prjY2)
                };
                geo.Types = new byte[] { 1, 1, 1, 129 };
                _aoiContainer.AddAOI(geo);
            }
            canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        private void btnToChina_Click(object sender, EventArgs e)
        {
            ToChinaViewport();
            canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        private void btnToWorld_Click(object sender, EventArgs e)
        {
            ToWorldViewport();
            canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        private void btnAOI_Click(object sender, EventArgs e)
        {
            _aoiContainer.Reset();
            PencilToolLayer drawTool = new PencilToolLayer();
            drawTool.PencilType = enumPencilType.Rectangle;
            drawTool.PencilIsFinished = (x) =>
            {
                _aoiContainer.AddAOI(x);
                UpdateDrawedAOI();
                if (AOIIsChanged != null)
                    AOIIsChanged(this, null);
                canvasHost1.Canvas.CurrentViewControl = _aoiContainer as IControlLayer;
            };
            canvasHost1.Canvas.CurrentViewControl = drawTool;
        }

        private void UpdateDrawedAOI()
        {
            object geometry = _aoiContainer.FirstAOI;
            if (geometry is GeometryOfDrawed)
                UpdateDrawedAOI(geometry as GeometryOfDrawed);
        }

        private void UpdateDrawedAOI(GeometryOfDrawed geometry)
        {
            _drawedAOI = GetCoordEnvelope(geometry); 
        }

        private CoordEnvelope GetCoordEnvelope(GeometryOfDrawed geometry)
        {
            if (geometry.RasterPoints.Length==0)
                return null;
            double geoX1, geoY1, geoX2, geoY2;
            if (geometry.IsPrjCoord)
            {
                canvasHost1.Canvas.CoordTransform.Prj2Geo(geometry.RasterPoints[0].X, geometry.RasterPoints[0].Y, out geoX1, out geoY1);
                canvasHost1.Canvas.CoordTransform.Prj2Geo(geometry.RasterPoints[2].X, geometry.RasterPoints[2].Y, out geoX2, out geoY2);
            }
            else
            {
                canvasHost1.Canvas.CoordTransform.Raster2Geo((int)geometry.RasterPoints[0].X, (int)geometry.RasterPoints[0].Y, out geoX1, out geoY1);
                canvasHost1.Canvas.CoordTransform.Raster2Geo((int)geometry.RasterPoints[2].X, (int)geometry.RasterPoints[2].Y, out geoX2, out geoY2);
            }
            return new CoordEnvelope(Math.Min(geoX1, geoX2), Math.Max(geoX1, geoX2), Math.Min(geoY2, geoY1), Math.Max(geoY2, geoY1));
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            _aoiContainer.Reset();
            PencilToolLayer drawTool = new PencilToolLayer();
            drawTool.PencilType = enumPencilType.Rectangle;
            drawTool.PenColor = Color.Red;
            drawTool.PencilIsFinished = (result) =>
            {
                //后的操作
                //canvasHost1.Canvas.CurrentViewControl = new DefaultControlLayer();
                //UpdateAllowPanMap();
                if (MapSelectedIsChanged != null)
                {
                    CoordEnvelope geoEnv = GetCoordEnvelope(result);
                    MapSelectedIsChanged(this, geoEnv);
                }
                canvasHost1.Canvas.Refresh(enumRefreshType.All);
            };
            canvasHost1.Canvas.CurrentViewControl = drawTool;
        }
    }
}
