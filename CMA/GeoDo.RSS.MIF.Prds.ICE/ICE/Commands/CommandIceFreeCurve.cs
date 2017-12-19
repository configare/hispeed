using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;
using GeoDo.RSS.Core.RasterDrawing;
using System.Drawing;
using GeoDo.RSS.UI.AddIn.Windows;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    [Export(typeof(ICommand))]
    public class CommandIceFreeCurve : Command
    {
        private IceFreeCurveInfoLayer _infoLayer = null;
        private ICanvas _canvas = null;
        private FeatureListContent _content = null;
        private int _selectBand = 4;
        private Dictionary<Feature, IceFreeCurveInfoLayer.InfoItem[]> _iceFeatures = new Dictionary<Feature, IceFreeCurveInfoLayer.InfoItem[]>();

        public CommandIceFreeCurve()
            : base()
        {
            _id = 78000;
            _name = _text = _toolTip = "冰缘线绘制";
        }

        public override void Execute()
        {
            Execute(null);
        }

        public override void Execute(string argument)
        {
            if (_smartSession.SmartWindowManager.ActiveCanvasViewer == null)
                return;
            ICommand cmd = _smartSession.CommandEnvironment.Get(78008);//78008
            if (cmd != null)
                cmd.Execute("Bottom:false");

            InitIceWnd(argument);

            //TryAddIceLayers();

            //InitFeatureListWindow();
        }

        private void InitIceWnd(string defaultBand)
        {
            ISmartWindow window = _smartSession.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(IceFeatureListWnd); });
            if (window == null)
                return;
            IceFeatureListWnd iceWnd = window as IceFeatureListWnd;
            iceWnd.InitIce(defaultBand);
        }

        //private void TryAddIceLayers()
        //{
        //    _canvas = GetCurrentCanvas();
        //    if (_infoLayer == null)
        //    {
        //        _infoLayer = new IceFreeCurveInfoLayer();
        //        if (!_canvas.LayerContainer.Layers.Contains(_infoLayer))
        //            _canvas.LayerContainer.Layers.Add(_infoLayer);
        //    }
        //    else
        //    {
        //        _infoLayer.InfoItems.Clear();
        //        if (!_canvas.LayerContainer.Layers.Contains(_infoLayer))
        //        {
        //            _canvas.LayerContainer.Layers.Add(_infoLayer);
        //        }
        //    }
        //    _iceFeatures.Clear();
        //    int iceControlPointNo = 0;
        //    int iceLineFeatureOid = 0;

        //    IPencilToolLayer toolLayer = new PencilToolLayer();
        //    toolLayer.PencilType = enumPencilType.ControlFreeCurve;
        //    _canvas.CurrentViewControl = toolLayer;
        //    toolLayer.PencilIsFinished = new Action<GeometryOfDrawed>
        //        (
        //           (geo) =>
        //           {
        //               //增加冰缘线
        //               Feature feature = GetIceLineFeature(ref iceLineFeatureOid, geo);
        //               _content.UpdateLayer(ref feature);
        //               //_infoLayer.AddFeature(GetPointFeature(ref oid, geo));
        //               //增加冰缘线拐点
        //               List<IceFreeCurveInfoLayer.InfoItem> infoList = new List<IceFreeCurveInfoLayer.InfoItem>();
        //               foreach (PointF rasterPoint in geo.ControlRasterPoints)
        //               {
        //                   double geoX, geoY;
        //                   double temp;
        //                   _canvas.CoordTransform.Raster2Geo((int)rasterPoint.Y, (int)rasterPoint.X, out geoX, out geoY);
        //                   temp = GetPixelTemperature(_canvas, rasterPoint.X, rasterPoint.Y);
        //                   //
        //                   IceFreeCurveInfoLayer.InfoItem info = new IceFreeCurveInfoLayer.InfoItem();
        //                   info.No = ++iceControlPointNo;
        //                   info.Longitude = geoX;
        //                   info.Latitude = geoY;
        //                   info.Temperature = temp;
        //                   //
        //                   infoList.Add(info);
        //               }
        //               IceFreeCurveInfoLayer.InfoItem[] infos = infoList.ToArray();
        //               _infoLayer.InfoItems.AddRange(infos);
        //               _iceFeatures.Add(feature, infos);
        //           }
        //        );
        //    //toolLayer.ControlPointIsAdded = new Action<PointF>
        //    //    (
        //    //(rasterPoint) =>
        //    //{
        //    //    double geoX, geoY;
        //    //    double temp;
        //    //    _canvas.CoordTransform.Raster2Geo((int)rasterPoint.Y, (int)rasterPoint.X, out geoX, out geoY);
        //    //    temp = GetPixelTemperature(_canvas, rasterPoint.X, rasterPoint.Y);
        //    //    //
        //    //    IceFreeCurveInfoLayer.InfoItem info = new IceFreeCurveInfoLayer.InfoItem();
        //    //    info.No = ++No;
        //    //    info.Longitude = geoX;
        //    //    info.Latitude = geoY;
        //    //    info.Temperature = temp;
        //    //    //
        //    //    _infoLayer.InfoItems.Add(info);
        //    //    //
        //    //}
        //    //);
        //}

        //private void InitFeatureListWindow()
        //{
        //    ISmartWindow window = _smartSession.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(FeatureListWindow); });
        //    if (window != null)
        //    {
        //        FeatureListWindow featureWnd = window as FeatureListWindow;
        //        featureWnd.Text = "冰缘线窗口";
        //        featureWnd.OnWindowClosed = new EventHandler((obj, args) =>
        //        {
        //            _canvas.CurrentViewControl = new DefaultControlLayer();
        //            //删除左上角显示的点信息
        //            _infoLayer.InfoItems.Clear();
        //            //移除信息层
        //            if (_canvas.LayerContainer != null && _canvas.LayerContainer.Layers.Contains(_infoLayer))
        //                _canvas.LayerContainer.Layers.Remove(_infoLayer);
        //        });

        //        _content = featureWnd.ToolWindowContent as FeatureListContent;
        //        _content.LayerName = "冰缘线";
        //        _content.InitControl(false, false, false, true);
        //        _content.InitList(new string[] { "name" });
        //        //_content.LayerName = "System:StatArea";
        //        IRasterDrawing drawing = _canvas.PrimaryDrawObject as IRasterDrawing;
        //        if (drawing == null)
        //            _content.InitBand(0, 1);
        //        else
        //            _content.InitBand(drawing.BandCount, _selectBand);
        //        _content._selectBandChanged += new FeatureListContent.SelectBandChangedHandler((band) =>
        //            {
        //                _selectBand = band;
        //            }
        //        );
        //        _content.RemoveFeature += new FeatureListContent.RemoveFeatureHandler((feature) =>
        //            {
        //                //同步删除冰缘线拐点
        //                if (_iceFeatures.Count != 0)
        //                {
        //                    IceFreeCurveInfoLayer.InfoItem[] infoItem = _iceFeatures[feature];
        //                    if (infoItem != null && infoItem.Length != 0)
        //                    {
        //                        for (int i = 0; i < infoItem.Length; i++)
        //                        {
        //                            _infoLayer.InfoItems.Remove(infoItem[i]);
        //                        }
        //                    }
        //                }
        //                _canvas.Refresh(enumRefreshType.All);
        //            }
        //            );
        //    }
        //}

        //private Feature GetPointFeature(ref int oid, GeometryOfDrawed geo)
        //{
        //    if (geo.ControlRasterPoints == null || geo.ControlRasterPoints.Length == 0)
        //        return null;
        //    Feature fet = null;
        //    List<ShapePoint> points = new List<ShapePoint>();
        //    foreach (PointF rasterPoint in geo.RasterPoints)
        //    {
        //        double geoX, geoY;
        //        _canvas.CoordTransform.Raster2Geo((int)rasterPoint.Y, (int)rasterPoint.X, out geoX, out geoY);
        //        points.Add(new ShapePoint(geoX, geoY));
        //    }
        //    ShapeLineString shapeLine = new ShapeLineString(points.ToArray());
        //    oid++;
        //    string name = "冰缘线" + oid.ToString();
        //    fet = new Feature(oid, new ShapePolyline(new ShapeLineString[] { shapeLine }), new string[] { "name" }, new string[] { name }, new LabelLocation[] { new LabelLocation() });
        //    return fet;
        //}

        //private ShapePoint[] GetGeometry(ref int oid, GeometryOfDrawed geo)
        //{
        //    if (geo.ControlRasterPoints == null || geo.ControlRasterPoints.Length == 0)
        //        return null;
        //    List<ShapePoint> points = new List<ShapePoint>();
        //    foreach (PointF rasterPoint in geo.RasterPoints)
        //    {
        //        double geoX, geoY;
        //        _canvas.CoordTransform.Raster2Geo((int)rasterPoint.Y, (int)rasterPoint.X, out geoX, out geoY);
        //        points.Add(new ShapePoint(geoX, geoY));
        //    }
        //    return points.ToArray();
        //}

        //private Feature GetIceLineFeature(ref int oid, GeometryOfDrawed geo)
        //{
        //    if (geo.ControlRasterPoints == null || geo.ControlRasterPoints.Length == 0)
        //        return null;
        //    Feature fet = null;
        //    List<ShapePoint> points = new List<ShapePoint>();
        //    foreach (PointF rasterPoint in geo.RasterPoints)
        //    {
        //        double geoX, geoY;
        //        _canvas.CoordTransform.Raster2Geo((int)rasterPoint.Y, (int)rasterPoint.X, out geoX, out geoY);
        //        points.Add(new ShapePoint(geoX, geoY));
        //    }
        //    ShapeLineString shapeLine = new ShapeLineString(points.ToArray());

        //    string fieldValue = "冰缘线" + (oid + 1).ToString();
        //    fet = new Feature(oid, new ShapePolyline(new ShapeLineString[] { shapeLine }), new string[] { "name" }, new string[] { fieldValue }, new LabelLocation[] { new LabelLocation() });
        //    oid++;
        //    return fet;
        //}

        //private ILabelLayer GetLabelLayer(string name, string[] fieldNames)
        //{
        //    ICanvasViewer viewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
        //    if (viewer == null)
        //        return null;
        //    ILabelService srv = (viewer as ICurrentRasterInteractiver).LabelService;
        //    if (srv == null)
        //        return null;
        //    return srv.GetLabelLayer(name, fieldNames);
        //}

        //private Feature GetIceControlpointsFeature()
        //{
        //    Feature fet = null;

        //    return fet;
        //}

        //private unsafe double GetPixelTemperature(ICanvas canvas, float x, float y)
        //{
        //    IRasterDrawing drawing = canvas.PrimaryDrawObject as IRasterDrawing;
        //    if (drawing == null || x < 0 || x > drawing.Size.Width || y < 0 || y > drawing.Size.Height)
        //        return 0;
        //    double[] buffer = new double[drawing.BandCount];
        //    fixed (double* ptr0 = buffer)
        //    {
        //        double* ptr = ptr0;
        //        drawing.ReadPixelValues((int)x, (int)y, ptr);
        //    }
        //    return buffer[_selectBand] / 10 - 273;
        //}

        //private ICanvas GetCurrentCanvas()
        //{
        //    ICanvasViewer v = _smartSession.SmartWindowManager.ActiveCanvasViewer;
        //    return v.Canvas;
        //}

        //public EventHandler WindowClosed { get; set; }
    }
}
