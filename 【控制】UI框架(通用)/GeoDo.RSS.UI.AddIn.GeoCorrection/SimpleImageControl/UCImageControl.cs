using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;
using GeoDo.Project;
using GeoDo.RSS.Core.CA;
using GeoDo.RSS.CA;
using GeoDo.MEF;

namespace GeoDo.RSS.UI.AddIn.GeoCorrection
{
    public partial class UCImageControl : UserControl
    {
        public event EventHandler AOIIsChanged;
        public event EventHandler ImgIsChanged;

        private string _strRasterName = "";
        private IAOIContainerLayer _aoiContainer;
        private IAOIContainerLayer _aoiGCPContainer;
        private IVectorHostLayer _vectorHostLayer;
        private ISmartSession _smartSession;
        private GeoDo.RSS.Core.DrawEngine.CoordPoint _drawedAOI;
        private bool _isOnlyOneImg = true;
        private IRgbProcessorArgEditor[] _registeredEditors = null;

        private float _selPointWidth;
        private float _selPointHeight;

        public UCImageControl()
        {
            InitializeComponent();
            canvasHost1.Load += new EventHandler(canvasHost1_Load);
            Load += new EventHandler(UCImageControl_Load);
            SizeChanged += new EventHandler(UCImageControl_SizeChanged);
            Disposed += new EventHandler(UCImageControl_Disposed);
            _registeredEditors = GetRegisteredEditors();
        }

        public string RasterName
        {
            get { return _strRasterName; }
            set { _strRasterName = value; }
        }

        public void SetLebelName(string strLabel)
        {
            DataType.Text = strLabel;
        }

        public void SetSession(ISmartSession session)
        {
            _smartSession = session;
        }

        public bool IsOnlyOneImg
        {
            get { return _isOnlyOneImg; }
            set { _isOnlyOneImg = value; }
        }

        void UCImageControl_Disposed(object sender, EventArgs e)
        {
            if (_aoiContainer != null)
            {
                (_aoiContainer as AOIContainerLayer).Dispose();
                _aoiContainer = null;
            }
            if (_aoiGCPContainer != null)
            {
                (_aoiGCPContainer as AOIContainerLayer).Dispose();
                _aoiGCPContainer = null;
            }
            if (_vectorHostLayer != null)
            {
                (_vectorHostLayer as VectorHostLayer).Dispose();
                _vectorHostLayer = null;
            }
            if (canvasHost1 != null)
            {
                canvasHost1.Canvas.Dispose();
                canvasHost1.Dispose();
            }
        }


        private void canvasHost1_Load(object sender, EventArgs e)
        {
            canvasHost1.Canvas.CanvasSetting.RenderSetting.BackColor = Color.White;

            CreateVectorHost();
            if (File.Exists(_strRasterName))
            {
                 AddRasterLayer(_strRasterName);
            }
        }

        private void AddRasterLayer(string strRasName)
        {
            if (_isOnlyOneImg)
            {
                bool memoryIsEnough = CheckMemory(strRasName);
                if (!memoryIsEnough)
                {
                    MsgBox.ShowInfo("系统可用内存不足以对以下文件执行几何精校正操作。\n\n" + strRasName);
                    return;
                }
            }

            IRasterDrawing drawing = new RasterDrawing(strRasName, canvasHost1.Canvas, GetRgbStretcherProvider(strRasName));
            drawing.SelectedBandNos = GetDefaultBands(drawing);
            IRasterLayer rstLayer = new RasterLayer(drawing);
            canvasHost1.Canvas.LayerContainer.Layers.Add(rstLayer);
            canvasHost1.Canvas.PrimaryDrawObject = drawing;
            canvasHost1.Canvas.CurrentEnvelope = drawing.OriginalEnvelope;

            int times = drawing.GetOverviewLoadTimes();
            if (times == -1)
            {
                drawing.StartLoading(null);
                return;
            }
            string tipstring = "正在读取文件\"" + Path.GetFileName(_strRasterName) + "\"...";
            try
            {
                _smartSession.ProgressMonitorManager.DefaultProgressMonitor.Reset(tipstring, times);
                _smartSession.ProgressMonitorManager.DefaultProgressMonitor.Start(false);
                drawing.StartLoading((t, p) =>
                {
                    _smartSession.ProgressMonitorManager.DefaultProgressMonitor.Boost(p, tipstring);
                });
            }
            finally
            {
                _smartSession.ProgressMonitorManager.DefaultProgressMonitor.Finish();
            }
        }

        private void AddVectorLayer(string strVecName)
        {
            if (_vectorHostLayer == null)
                return;
            if (canvasHost1.Canvas.IsRasterCoord)
                return;
            IRasterDrawing drawing = canvasHost1.Canvas.PrimaryDrawObject as IRasterDrawing;
            if (drawing != null)
                drawing.TryCreateOrbitPrjection();

            string extName = Path.GetExtension(strVecName).ToUpper();
            if (extName == ".SHP")
            {
                _vectorHostLayer.AddData(strVecName, null);

                CodeCell.AgileMap.Core.IMap map = _vectorHostLayer.Map as CodeCell.AgileMap.Core.IMap;
                CodeCell.AgileMap.Core.FeatureLayer fetL = map.LayerContainer.Layers[0] as CodeCell.AgileMap.Core.FeatureLayer;
                CodeCell.AgileMap.Core.FeatureClass fetc = fetL.Class as CodeCell.AgileMap.Core.FeatureClass;
                CodeCell.AgileMap.Core.Envelope evp = fetc.FullEnvelope.Clone() as CodeCell.AgileMap.Core.Envelope;
                GeoDo.RSS.Core.DrawEngine.CoordEnvelope cvEvp = new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(evp.MinX, evp.MaxX, evp.MinY, evp.MaxY);
                canvasHost1.Canvas.CurrentEnvelope = cvEvp;
            }

            canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        private void UCImageControl_Load(object sender, EventArgs e)
        {
            _aoiGCPContainer = new AOIContainerLayer();
            _aoiGCPContainer.IsAllowEdit = false;
            _aoiGCPContainer.Color = Color.LightGreen;
            _aoiGCPContainer.LineWidth = 2;
            canvasHost1.Canvas.LayerContainer.Layers.Add(_aoiGCPContainer as GeoDo.RSS.Core.DrawEngine.ILayer);

            _aoiContainer = new AOIContainerLayer();
            _aoiContainer.IsAllowEdit = true;
            _aoiContainer.IsOnlyOneAOI = true;
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

        private void UpdateDrawedAOI()
        {
            object geometry = _aoiContainer.FirstAOI;
            if (geometry is GeometryOfDrawed)
                UpdateDrawedAOI(geometry as GeometryOfDrawed);
        }

        private void UpdateDrawedAOI(GeometryOfDrawed geometry)
        {
            _drawedAOI = GetCoordPoint(geometry);
        }

        private GeoDo.RSS.Core.DrawEngine.CoordPoint GetCoordPoint(GeometryOfDrawed geometry)
        {
            if (geometry.RasterPoints.Length == 0)
                return null;
            double geoX, geoY;
            if (canvasHost1.Canvas.PrimaryDrawObject != null)
            {
                if (canvasHost1.Canvas.PrimaryDrawObject.IsRasterCoord)
                {
                    geoX = geometry.RasterPoints[0].X;
                    geoY = geometry.RasterPoints[0].Y;
                }
                else
                {
                    if (canvasHost1.Canvas.CoordTransform.DataCoordType == enumDataCoordType.Geo)
                    {
                        canvasHost1.Canvas.CoordTransform.Raster2Geo(Convert.ToInt32(geometry.RasterPoints[0].Y), Convert.ToInt32(geometry.RasterPoints[0].X), out geoX, out geoY);
                    }
                    else
                    {
                        canvasHost1.Canvas.CoordTransform.Raster2Prj(geometry.RasterPoints[0].Y, geometry.RasterPoints[0].X, out geoX, out geoY);
                    }
                }
            }
            else
            {
                if (canvasHost1.Canvas.CoordTransform.DataCoordType == enumDataCoordType.Geo)
                {
                    canvasHost1.Canvas.CoordTransform.Prj2Geo(geometry.RasterPoints[0].X, geometry.RasterPoints[0].Y, out geoX, out geoY);
                }
                else
                {
                    geoX = geometry.RasterPoints[0].X;
                    geoY = geometry.RasterPoints[0].Y;
                }
            }

            return new GeoDo.RSS.Core.DrawEngine.CoordPoint(geoX, geoY);
        }

        private GeoDo.RSS.Core.DrawEngine.CoordEnvelope GetCoordEnvelope(GeometryOfDrawed geometry)
        {
            if (geometry.RasterPoints.Length == 0)
                return null;
            double geoX1, geoY1, geoX2, geoY2;
            if (canvasHost1.Canvas.PrimaryDrawObject != null)
            {
                if (canvasHost1.Canvas.PrimaryDrawObject.IsRasterCoord)
                {
                    geoX1 = geometry.RasterPoints[0].X;
                    geoY1 = geometry.RasterPoints[1].Y;
                    geoX2 = geometry.RasterPoints[2].X;
                    geoY2 = geometry.RasterPoints[3].Y;
                }
                else
                {
                    if (canvasHost1.Canvas.CoordTransform.DataCoordType == enumDataCoordType.Geo)
                    {
                        canvasHost1.Canvas.CoordTransform.Raster2Geo(Convert.ToInt32(geometry.RasterPoints[1].Y), Convert.ToInt32(geometry.RasterPoints[0].X), out geoX1, out geoY1);
                        canvasHost1.Canvas.CoordTransform.Raster2Geo(Convert.ToInt32(geometry.RasterPoints[3].Y), Convert.ToInt32(geometry.RasterPoints[2].X), out geoX2, out geoY2);
                    }
                    else
                    {
                        canvasHost1.Canvas.CoordTransform.Raster2Prj(geometry.RasterPoints[1].Y, geometry.RasterPoints[0].X, out geoX1, out geoY1);
                        canvasHost1.Canvas.CoordTransform.Raster2Prj(geometry.RasterPoints[3].Y, geometry.RasterPoints[2].X, out geoX2, out geoY2);
                    }
                }
            }
            else
            {
                if (canvasHost1.Canvas.CoordTransform.DataCoordType == enumDataCoordType.Geo)
                {
                    canvasHost1.Canvas.CoordTransform.Prj2Geo(geometry.RasterPoints[0].X, geometry.RasterPoints[1].Y, out geoX1, out geoY1);
                    canvasHost1.Canvas.CoordTransform.Prj2Geo(geometry.RasterPoints[2].X, geometry.RasterPoints[3].Y, out geoX2, out geoY2);
                }
                else
                {
                    geoX1 = geometry.RasterPoints[0].X;
                    geoY1 = geometry.RasterPoints[1].Y;
                    geoX2 = geometry.RasterPoints[2].X;
                    geoY2 = geometry.RasterPoints[3].Y;
                }
            }

            return new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(Math.Min(geoX1, geoX2), Math.Max(geoX1, geoX2), Math.Min(geoY2, geoY1), Math.Max(geoY2, geoY1));
        }

        public GeoDo.RSS.Core.DrawEngine.CoordPoint DrawedAOI
        {
            get { return _drawedAOI; }
            set
            {
                _drawedAOI = value;
                //SetAOIGeometry(_drawedAOI);
            }
        }

        private void SetAOIGeometry(GeoDo.RSS.Core.DrawEngine.CoordEnvelope evp)
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

        private RgbStretcherProvider GetRgbStretcherProvider(string fname)
        {
            IRasterDataProvider prd = null;
            try
            {
                prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
                if (prd != null && prd.BandCount == 1)
                    return new RgbStretcherProvider();
            }
            finally
            {
                if (prd != null)
                    prd.Dispose();
            }
            return null;
        }

        private static int[] GetDefaultBands(IRasterDrawing drawing)
        {
            IRasterDataProvider prd = drawing.DataProvider;
            if (prd == null)
                return null;
            int[] defaultBands = prd.GetDefaultBands();
            if (defaultBands == null)
                defaultBands = new int[] { 1, 2, 3 };
            return defaultBands;
        }

        private void ImgZoomIn_Click(object sender, EventArgs e)
        {
            if (_aoiContainer.AOIs.Count() == 0)
            {
                canvasHost1.Canvas.CurrentViewControl = new ZoomControlLayerWithBoxGDIPlus(GeoDo.RSS.Core.DrawEngine.ZoomControlLayerWithBox.enumZoomType.ZoomIn);
            }
            else
            {
                GeometryOfDrawed geometry = _aoiContainer.FirstAOI as GeometryOfDrawed;
                canvasHost1.Canvas.CurrentViewControl = new ZoomControlLayerWithBoxGDIPlus(GeoDo.RSS.Core.DrawEngine.ZoomControlLayerWithBox.enumZoomType.ZoomIn);
                _aoiContainer.IsAllowEdit = false;
                _aoiContainer.AddAOI(geometry);
            }
        }

        private void ImgZoomOut_Click(object sender, EventArgs e)
        {
            if (_aoiContainer.AOIs.Count() == 0)
            {
                canvasHost1.Canvas.CurrentViewControl = new ZoomControlLayerWithBoxGDIPlus(GeoDo.RSS.Core.DrawEngine.ZoomControlLayerWithBox.enumZoomType.ZoomOut);
            }
            else
            {
                GeometryOfDrawed geometry = _aoiContainer.FirstAOI as GeometryOfDrawed;
                canvasHost1.Canvas.CurrentViewControl = new ZoomControlLayerWithBoxGDIPlus(GeoDo.RSS.Core.DrawEngine.ZoomControlLayerWithBox.enumZoomType.ZoomOut);
                _aoiContainer.IsAllowEdit = false;
                _aoiContainer.AddAOI(geometry);
            }
        }

        private void ImgPan_Click(object sender, EventArgs e)
        {
            if (_aoiContainer.AOIs.Count() == 0)
            {
                canvasHost1.Canvas.CurrentViewControl = new DefaultControlLayer();
            }
            else
            {
                GeometryOfDrawed geometry = _aoiContainer.FirstAOI as GeometryOfDrawed;
                canvasHost1.Canvas.CurrentViewControl = new DefaultControlLayer();
                _aoiContainer.IsAllowEdit = false;
                _aoiContainer.AddAOI(geometry);
            }
        }

        private void ImgFullMap_Click(object sender, EventArgs e)
        {
            if (canvasHost1.Canvas.PrimaryDrawObject is IRasterDrawing)
            {
                canvasHost1.Canvas.CurrentEnvelope = (canvasHost1.Canvas.PrimaryDrawObject as IRasterDrawing).OriginalEnvelope;
            }
            else
            {
                CodeCell.AgileMap.Core.IMap map = _vectorHostLayer.Map as CodeCell.AgileMap.Core.IMap;
                if ((map.LayerContainer.Layers != null) && (map.LayerContainer.Layers.Length > 0))
                {
                    CodeCell.AgileMap.Core.FeatureLayer fetL = map.LayerContainer.Layers[0] as CodeCell.AgileMap.Core.FeatureLayer;
                    CodeCell.AgileMap.Core.FeatureClass fetc = fetL.Class as CodeCell.AgileMap.Core.FeatureClass;
                    CodeCell.AgileMap.Core.Envelope evp = fetc.FullEnvelope.Clone() as CodeCell.AgileMap.Core.Envelope;
                    GeoDo.RSS.Core.DrawEngine.CoordEnvelope cvEvp = new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(evp.MinX, evp.MaxX, evp.MinY, evp.MaxY);
                    canvasHost1.Canvas.CurrentEnvelope = cvEvp;
                }
            }
            //canvasHost1.Canvas.SetToFullEnvelope();
            canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        private void CreateVectorHost()
        {
            ICanvas c = canvasHost1.Canvas;
            _vectorHostLayer = new VectorHostLayer(null);
            _vectorHostLayer.Set(c);
            c.LayerContainer.Layers.Add(_vectorHostLayer as GeoDo.RSS.Core.DrawEngine.ILayer);
        }

        private void AddSelPoint()
        {
            CrossLayer drawTool = new CrossLayer();
            drawTool.IsMoved = true;
            if (canvasHost1.Canvas.PrimaryDrawObject != null)
            {
                float centerRow, centerCol;
                canvasHost1.Canvas.CoordTransform.Screen2Raster(Width / 2, Height / 2, out centerRow, out centerCol);
                drawTool.SetVertext(centerCol, centerRow);
                _aoiContainer.AddAOI(drawTool.AddGeometry(true));
            }
            else
            {
                double centerX, centerY;
                canvasHost1.Canvas.CoordTransform.Screen2Prj(Width / 2, Height / 2, out centerX, out centerY);
                drawTool.SetVertext((float)centerX, (float)centerY);
                _aoiContainer.AddAOI(drawTool.AddGeometry(false));
            }
            //PointF pt = new PointF(this.Width / 2, this.Height / 2);
            //drawTool.SetVertext(pt);

            //_aoiContainer.AddAOI(drawTool.AddGeometry(canvasHost1.Canvas));
            UpdateDrawedAOI();
            if (AOIIsChanged != null)
                AOIIsChanged(this, null);
        }

        private void SelPoint_Click(object sender, EventArgs e)
        {
            bool hasDataShowed = false;
            foreach (ILayer layer in canvasHost1.Canvas.LayerContainer.Layers)
            {
                if (layer is RasterLayer)
                {
                    hasDataShowed = true;
                    break;
                }

                if (layer is IVectorHostLayer)
                {
                    CodeCell.AgileMap.Core.IMap map = (layer as IVectorHostLayer).Map as CodeCell.AgileMap.Core.IMap;
                    if (map != null)
                    {
                        if ((map.LayerContainer.Layers != null) && (map.LayerContainer.Layers.Length > 0))
                        {
                            hasDataShowed = true;
                            break;
                        }
                    }
                }
            }
            if (!hasDataShowed)
            {
                return;
            }
            //_aoiContainer.Reset();
            _aoiContainer.IsAllowEdit = true;
            if (_aoiContainer.AOIs.Count() == 0)
            {
                canvasHost1.Canvas.CurrentViewControl = _aoiContainer as IControlLayer;
                AddSelPoint();
            }
            else
            {
                GeometryOfDrawed geometry = _aoiContainer.FirstAOI as GeometryOfDrawed;
                canvasHost1.Canvas.CurrentViewControl = _aoiContainer as IControlLayer;
                _aoiContainer.AddAOI(geometry);
            }
           // canvasHost1.Canvas.CurrentViewControl = _aoiContainer as IControlLayer;
            //drawTool.PencilIsFinished = (x) =>
            //{
            //    _aoiContainer.AddAOI(x);
            //    UpdateDrawedAOI();
            //    if (AOIIsChanged != null)
            //        AOIIsChanged(this, null);
            //    canvasHost1.Canvas.CurrentViewControl = _aoiContainer as IControlLayer;
            //};
            canvasHost1.Canvas.Refresh(enumRefreshType.All);
            //canvasHost1.Canvas.CurrentViewControl = drawTool;
        }

        private void CalGeometryPara()
        {
            if (canvasHost1.Canvas.PrimaryDrawObject != null)//有活动影像是返回栅格坐标
            {
                float row1, row2, col1, col2;
                canvasHost1.Canvas.CoordTransform.Screen2Raster(0, 0, out row1, out col1);
                canvasHost1.Canvas.CoordTransform.Screen2Raster(50, 50, out row2, out col2);
                _selPointWidth = Math.Abs(col1 - col2);
                _selPointHeight = Math.Abs(row1 - row2);
            }
            else//无活动影像时返回投影坐标
            {
                double prjX1, prjY1, prjX2, prjY2;
                canvasHost1.Canvas.CoordTransform.Screen2Prj(0, 0, out prjX1, out prjY1);
                canvasHost1.Canvas.CoordTransform.Screen2Prj(50, 50, out prjX2, out prjY2);
                _selPointWidth = Math.Abs((float)(prjX1 - prjX2));
                _selPointHeight = Math.Abs((float)(prjY1 - prjY2));
            }
        }

        public void AddGCP_Draw()
        {
            GeometryOfDrawed geometry = _aoiContainer.FirstAOI as GeometryOfDrawed;
            if (geometry != null)
            {
                if (geometry.RasterPoints.Length <= 0)
                    return;

                CrossLayer drawTool = new CrossLayer();
                drawTool.IsMoved = false;

                drawTool.SetVertext(geometry.RasterPoints[0].X, geometry.RasterPoints[0].Y);
                if (canvasHost1.Canvas.PrimaryDrawObject != null)
                {
                    _aoiGCPContainer.AddAOI(drawTool.AddGeometry(true));               
                }
                else
                {
                    _aoiGCPContainer.AddAOI(drawTool.AddGeometry(false));  
                }
                //PointF pt = new PointF(screenX, screenY);
                //drawTool.SetVertext(pt);
                

                canvasHost1.Canvas.Refresh(enumRefreshType.All);
            }
        }

        public void DelGCP_Draw(int index)
        {
            (_aoiGCPContainer as AOIContainerLayer).RemoveAOI(index);
            canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        public void GetRasCoord(ref double col, ref double row)
        {
            GeometryOfDrawed geometry = _aoiContainer.FirstAOI as GeometryOfDrawed;
            if (geometry != null)
            {
                row = geometry.RasterPoints[0].Y;
                col = geometry.RasterPoints[0].X;
            }
        }
        public int GetCoordType()
        {
            if (canvasHost1.Canvas.PrimaryDrawObject != null)
            {
                if (canvasHost1.Canvas.PrimaryDrawObject.IsRasterCoord)
                    return 0;
                else if (canvasHost1.Canvas.CoordTransform.DataCoordType == enumDataCoordType.Geo)
                    return 1;
                else
                    return 2;
            }
            else
            {
                CodeCell.AgileMap.Core.IMap map = _vectorHostLayer.Map as CodeCell.AgileMap.Core.IMap;
                if ((map.LayerContainer.Layers != null) && (map.LayerContainer.Layers.Length > 0))
                {
                    CodeCell.AgileMap.Core.FeatureLayer fetL = map.LayerContainer.Layers[0] as CodeCell.AgileMap.Core.FeatureLayer;
                    CodeCell.AgileMap.Core.FeatureClass fetc = fetL.Class as CodeCell.AgileMap.Core.FeatureClass;
                    CodeCell.AgileMap.Core.enumCoordinateType coordType = fetc.OriginalCoordinateType;
                    if (coordType == 0)
                        return 1;
                    else
                        return 2;
                }
            }
            return 1;
        }
        public ISpatialReference GetSpatialRef()
        {
            ISpatialReference spatialRef = null;
            if (canvasHost1.Canvas.PrimaryDrawObject != null)
                spatialRef = SpatialReference.FromWkt(canvasHost1.Canvas.PrimaryDrawObject.SpatialRef, enumWKTSource.GDAL);
            else
            {
                CodeCell.AgileMap.Core.IMap map = _vectorHostLayer.Map as CodeCell.AgileMap.Core.IMap;
                if ((map.LayerContainer.Layers != null) && (map.LayerContainer.Layers.Length > 0))
                {
                    CodeCell.AgileMap.Core.FeatureLayer fetL = map.LayerContainer.Layers[0] as CodeCell.AgileMap.Core.FeatureLayer;
                    CodeCell.AgileMap.Core.FeatureClass fetc = fetL.Class as CodeCell.AgileMap.Core.FeatureClass;
                    if (fetc.SpatialReference != null)
                    {
                        spatialRef = SpatialReference.FromWkt(fetc.SpatialReference.ToWKTString(), enumWKTSource.GDAL);
                    }
                }
            }
            return spatialRef;
        }

        public void SetCurrentGCPPos(int index)
        {
            if (_aoiContainer.AOIs.Count() == 1)
            {
                //GeometryOfDrawed geometry = _aoiContainer.FirstAOI as GeometryOfDrawed;
                //canvasHost1.Canvas.CurrentViewControl = _aoiContainer as IControlLayer;
                //_aoiContainer.AddAOI(geometry);
                //_aoiContainer.IsAllowEdit = true;

                if ((_aoiContainer.AOIs.ElementAt(0) is GeometryOfDrawed) && (_aoiGCPContainer.AOIs.ElementAt(index) is GeometryOfDrawed))
                {
                    float distanceX = (_aoiGCPContainer.AOIs.ElementAt(index) as GeometryOfDrawed).RasterPoints[0].X - (_aoiContainer.AOIs.ElementAt(0) as GeometryOfDrawed).RasterPoints[0].X;
                    float distanceY = (_aoiGCPContainer.AOIs.ElementAt(index) as GeometryOfDrawed).RasterPoints[0].Y - (_aoiContainer.AOIs.ElementAt(0) as GeometryOfDrawed).RasterPoints[0].Y;
                    int length = (_aoiGCPContainer.AOIs.ElementAt(index) as GeometryOfDrawed).RasterPoints.Length;
                    for (int i = 0; i < length; i++ )
                    {
                        (_aoiContainer.AOIs.ElementAt(0) as GeometryOfDrawed).RasterPoints[i].X += distanceX;
                        (_aoiContainer.AOIs.ElementAt(0) as GeometryOfDrawed).RasterPoints[i].Y += distanceY;
                    }
                }
                UpdateDrawedAOI();
                if (AOIIsChanged != null)
                    AOIIsChanged(this, null);

                double drawedX, drawedY;
                if (canvasHost1.Canvas.CoordTransform.DataCoordType == enumDataCoordType.Geo)
                {
                    canvasHost1.Canvas.CoordTransform.Geo2Prj(_drawedAOI.X, _drawedAOI.Y, out drawedX, out drawedY);
                }
                else
                {
                    drawedX = _drawedAOI.X;
                    drawedY = _drawedAOI.Y;
                }

                if ((drawedX > canvasHost1.Canvas.CurrentEnvelope.MinX) && (drawedX < canvasHost1.Canvas.CurrentEnvelope.MaxX) && (drawedY > canvasHost1.Canvas.CurrentEnvelope.MinY) && (drawedY < canvasHost1.Canvas.CurrentEnvelope.MaxY))
                {
                }
                else
                {
                    double dWidth = canvasHost1.Canvas.CurrentEnvelope.Width;
                    double dHeight = canvasHost1.Canvas.CurrentEnvelope.Height;
                    GeoDo.RSS.Core.DrawEngine.CoordEnvelope temp = new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(drawedX - dWidth / 2, drawedX + dWidth / 2, drawedY - dHeight / 2, drawedY + dHeight / 2);

                    canvasHost1.Canvas.CurrentEnvelope = temp;
                }

                canvasHost1.Canvas.Refresh(enumRefreshType.All);
            }
        }

        private void DataOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (_isOnlyOneImg)
            {
                dlg.Filter = "普通影像文件(*.tif, *.tiff)|*.tif;*.tiff|LDF文件(*.ldf)|*.ldf";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(dlg.FileName))
                    {
                        foreach (ILayer layer in canvasHost1.Canvas.LayerContainer.Layers)
                        {
                            if (layer is RasterLayer)
                            {
                                canvasHost1.Canvas.LayerContainer.Layers.Remove(layer);
                                break;
                            }
                        }
                        _strRasterName = dlg.FileName;
                        AddRasterLayer(_strRasterName);
                        _aoiContainer.Reset();
                        if (ImgIsChanged != null)
                            ImgIsChanged(this, null);
                        canvasHost1.Canvas.Refresh(enumRefreshType.RasterLayer);
                    }
                }
            }
            else
            {
                dlg.Filter = "普通影像文件(*.tif, *.tiff)|*.tif;*.tiff|LDF文件(*.ldf)|*.ldf|矢量文件(*.shp)|*.shp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetExtension(dlg.FileName).ToUpper() == ".SHP")
                    {
                        if (File.Exists(dlg.FileName))
                        {
                            AddVectorLayer(dlg.FileName);
                            _aoiContainer.Reset();
                            canvasHost1.Canvas.Refresh(enumRefreshType.VectorLayer);
                        }
                    }
                    else
                    {
                        if (File.Exists(dlg.FileName))
                        {
                            _strRasterName = dlg.FileName;
                            AddRasterLayer(_strRasterName);
                            _aoiContainer.Reset();
                            canvasHost1.Canvas.Refresh(enumRefreshType.RasterLayer);
                        }
                    }
                }
            }
        }

        private void ImgHistEven_Click(object sender, EventArgs e)
        {
            IRasterDrawing drawing = canvasHost1.Canvas.PrimaryDrawObject as IRasterDrawing;
            if (drawing == null)
                return;

            IRgbProcessor pro = new HisEqualizing();
            drawing.RgbProcessorStack.Process(pro);

            foreach (IRgbProcessorArgEditor editor in _registeredEditors)
            {
                if (editor.IsSupport(pro.GetType()))
                {
                    IRgbProcessorArgEditor argeditor = Activator.CreateInstance(editor.GetType()) as IRgbProcessorArgEditor;
                    if (pro.Arguments == null)
                        pro.CreateDefaultArguments();
                    if (drawing.SelectedBandNos.Length == 1 || drawing.BandCount == 1)
                    {
                        pro.BytesPerPixel = 1;
                        if (argeditor is frmReversalColorEditor)
                        {
                            return;
                        }
                    }
                    else
                        pro.BytesPerPixel = 3;
                    argeditor.OnPreviewing += new OnArgEditorPreviewing((senser, arg) =>
                    {
                        canvasHost1.Canvas.Refresh(Core.DrawEngine.enumRefreshType.RasterLayer);
                    }
                    );
                    argeditor.OnApplyClicked += new OnArgEditorApplyClick((senser, arg) =>
                    {
                        canvasHost1.Canvas.Refresh(Core.DrawEngine.enumRefreshType.RasterLayer);
                    }
                    );
                    argeditor.OnCancelClicked += new OnArgEditorCancelClick((senser, arg) =>
                    {
                        drawing.RgbProcessorStack.RemoveLast();
                        canvasHost1.Canvas.Refresh(Core.DrawEngine.enumRefreshType.RasterLayer);
                    });
                    if (argeditor is Form)
                    {
                        (argeditor as Form).Owner = _smartSession.SmartWindowManager.MainForm as Form;
                        (argeditor as Form).StartPosition = FormStartPosition.Manual;
                        (argeditor as Form).Location = _smartSession.SmartWindowManager.ViewLeftUpperCorner;
                        (argeditor as Form).Text = pro.Name + "参数设置...";
                    }
                    argeditor.Init(null, pro);
                    argeditor.Show(pro.Arguments);
                    return;
                }
            }
            canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        private IRgbProcessorArgEditor[] GetRegisteredEditors()
        {
            string[] dlls = MefConfigParser.GetAssemblysByCatalog("图像处理");
            using (IComponentLoader<IRgbProcessorArgEditor> loader = new ComponentLoader<IRgbProcessorArgEditor>())
            {
                return loader.LoadComponents(dlls);
            }
        }

        private void ImgRecover_Click(object sender, EventArgs e)
        {
            IRasterDrawing drawing = canvasHost1.Canvas.PrimaryDrawObject as IRasterDrawing;
            if (drawing == null)
                return;
            drawing.RgbProcessorStack.Clear();
            canvasHost1.Canvas.Refresh(Core.DrawEngine.enumRefreshType.RasterLayer);
        }

        private void UCImageControl_SizeChanged(object sender, EventArgs e)
        {
            if (canvasHost1.Canvas != null)
            {
                ImgFullMap_Click(null, null);
            }
        }

        private bool CheckMemory(string strRasName)
        {
            float availableSize = PerformanceMonitoring.GetAvailableRAM();
            if (availableSize < 0)
                return true;
            int memoryOfTile, tileCount;
            float requiredSize = RasterDrawing.EstimateRequiredMemory(strRasName, out memoryOfTile, out tileCount);

            IRasterDataProvider prd = GeoDataDriver.Open(strRasName) as IRasterDataProvider;
            int nHeight = prd.Height;
            int nWidth = prd.Width;
            enumDataType dataType = prd.DataType;
            int nByteCount = DataTypeHelper.SizeOf(dataType);
            requiredSize = requiredSize * 2 + nByteCount * nHeight * nWidth / 1024 / 1024;

            if (availableSize < requiredSize + 200)//预留200MB
                return false;
            else                //内存碎片的可能性非常大，需要先尝试内存申请能否成功
            {
                for (int i = 0; i < tileCount; i++)
                {
                    try
                    {
                        byte[] tileBuffer = new byte[memoryOfTile];
                    }
                    catch
                    {
                        return false;
                    }
                }

                try
                {
                    byte[] dataBuffer = new byte[nByteCount * nHeight * nWidth];
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }
    }
}
