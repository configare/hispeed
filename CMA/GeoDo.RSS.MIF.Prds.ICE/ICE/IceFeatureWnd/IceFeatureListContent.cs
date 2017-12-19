using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.UI.AddIn.Windows;
using GeoDo.RSS.Core.UI;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;
using GeoDo.RSS.Core.DF;
using System.IO;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public delegate void ExportToShapeFileHandler(string file);

    /// <summary>
    /// 海冰冰缘线 列表窗口：
    /// 1、记录所绘制后生成的冰缘线、冰缘线拐点信息
    /// 2、有删除功能
    /// 3、可以导出冰缘线、冰缘线拐点信息为shp。依据产品名。
    /// </summary>
    public partial class IceFeatureListContent : UserControl, IToolWindowContent
    {
        private ISmartSession _session = null;
        private Dictionary<Feature, IceFreeCurveInfoLayer.InfoItem[]> _iceFeatures = new Dictionary<Feature, IceFreeCurveInfoLayer.InfoItem[]>();
        private List<string> linLayerName = new List<string>();
        private string _layerName = "冰缘线";
        private IceFreeCurveInfoLayer _infoLayer = null;
        private ICanvas _canvas = null;
        private Action<int, string> _progressTracker = null;

        public delegate void SelectBandChangedHandler(int band);
        public SelectBandChangedHandler _selectBandChanged = null;
        public delegate void RemoveFeatureHandler(Feature feature);
        public event RemoveFeatureHandler RemoveFeature = null;

        private string _subProductIdentify = "EDGE";
        private IMonitoringSession _mSession = null;

        private int _selectedBand = 4;
        private string _fieldName = "name";
        private int _iceControlPointNo = 0;
        private int _iceLineFeatureOid = 0;

        public IceFeatureListContent()
        {
            InitializeComponent();
        }

        public void Init(string defaultband)
        {
            int bandCount = 1;
            IRasterDrawing drawing = _canvas.PrimaryDrawObject as IRasterDrawing;
            if (drawing != null)
                bandCount = drawing.BandCount;
            if (!string.IsNullOrEmpty(defaultband) && !int.TryParse(defaultband, out _selectedBand))
                _selectedBand = 4;
            if (_selectedBand < 0)
                _selectedBand = 4;
            if (_selectedBand > bandCount)
                _selectedBand = bandCount - 1;

            SetBandCombox(drawing.BandCount, _selectedBand);
            InitListView();
            InitInfoLayer();
        }

        private void InitInfoLayer()
        {
            if (_infoLayer == null)
            {
                _infoLayer = new IceFreeCurveInfoLayer();
                if (!_canvas.LayerContainer.Layers.Contains(_infoLayer))
                    _canvas.LayerContainer.Layers.Add(_infoLayer);
            }
            else
            {
                _infoLayer.InfoItems.Clear();
                if (!_canvas.LayerContainer.Layers.Contains(_infoLayer))
                {
                    _canvas.LayerContainer.Layers.Add(_infoLayer);
                }
            }
            _iceFeatures.Clear();

            IPencilToolLayer toolLayer = new PencilToolLayer();
            toolLayer.PencilType = enumPencilType.ControlFreeCurve;
            _canvas.CurrentViewControl = toolLayer;
            toolLayer.PencilIsFinished = new Action<GeometryOfDrawed>
                (
                   (geo) =>
                   {
                       //增加冰缘线
                       Feature feature = GetIceLineFeature(ref _iceLineFeatureOid, geo);

                       //增加冰缘线拐点
                       List<IceFreeCurveInfoLayer.InfoItem> infoList = new List<IceFreeCurveInfoLayer.InfoItem>();
                       foreach (PointF rasterPoint in geo.ControlRasterPoints)
                       {
                           double geoX, geoY;
                           double temp;
                           _canvas.CoordTransform.Raster2Geo((int)rasterPoint.Y, (int)rasterPoint.X, out geoX, out geoY);
                           temp = GetPixelTemperature(_canvas, rasterPoint.X, rasterPoint.Y);
                           //
                           IceFreeCurveInfoLayer.InfoItem info = new IceFreeCurveInfoLayer.InfoItem();
                           info.No = ++_iceControlPointNo;
                           info.Longitude = geoX;
                           info.Latitude = geoY;
                           info.Temperature = temp;
                           infoList.Add(info);
                       }
                       //if the last point is same to the one before it ,remove one
                       int count=infoList.Count;
                       if(count>=2)
                       {
                           IceFreeCurveInfoLayer.InfoItem info = infoList.Last();
                           int firstNo = infoList[0].No;
                           for (int i = count - 2; i >= 0; i--)
                           {
                               if ((infoList[i].Latitude == info.Latitude) && (infoList[i].Longitude == info.Longitude))
                                   infoList.RemoveAt(infoList.Count - 2);
                               else
                                   break;
                           }
                           for (int i = 0; i < infoList.Count; i++)
                           {
                               infoList[i].No = firstNo + i;
                           }
                           _iceControlPointNo = infoList.Last().No;
                       }
                       IceFreeCurveInfoLayer.InfoItem[] infos = infoList.ToArray();
                       _infoLayer.InfoItems.AddRange(infos);
                       _iceFeatures.Add(feature, infos);

                       UpdateLayer(feature);
                   }
                );
        }

        public void UpdateLayer(Feature fet)
        {
            if (fet != null)
            {
                ILabelLayer lyr = GetLabelLayer(_layerName, new string[] { _fieldName });
                if (lyr != null)
                {
                    ApplyColor(lyr);
                    lyr.LabelDef.Fieldname = _fieldName;
                    lyr.LabelDef.EnableLabeling = true;
                    lyr.AddFeature(fet);
                    AddFeatureToListView(fet);
                }
            }
        }

        private Feature GetIceLineFeature(ref int oid, GeometryOfDrawed geo)
        {
            if (geo.ControlRasterPoints == null || geo.ControlRasterPoints.Length == 0)
                return null;
            Feature fet = null;
            List<ShapePoint> points = new List<ShapePoint>();
            foreach (PointF rasterPoint in geo.RasterPoints)
            {
                double geoX, geoY;
                _canvas.CoordTransform.Raster2Geo((int)rasterPoint.Y, (int)rasterPoint.X, out geoX, out geoY);
                points.Add(new ShapePoint(geoX, geoY));
            }
            ShapeLineString shapeLine = new ShapeLineString(points.ToArray());
            string fieldValue = "冰缘线" + (oid + 1).ToString();
            fet = new Feature(oid, new ShapePolyline(new ShapeLineString[] { shapeLine }), new string[] { _fieldName }, new string[] { fieldValue }, new LabelLocation[] { new LabelLocation() });
            oid++;
            return fet;
        }

        private unsafe double GetPixelTemperature(ICanvas canvas, float x, float y)
        {
            IRasterDrawing drawing = canvas.PrimaryDrawObject as IRasterDrawing;
            if (drawing == null || x < 0 || x > drawing.Size.Width || y < 0 || y > drawing.Size.Height)
                return 0;
            double[] buffer = new double[drawing.BandCount];
            fixed (double* ptr0 = buffer)
            {
                double* ptr = ptr0;
                drawing.ReadPixelValues((int)x, (int)y, ptr);
            }
            return buffer[_selectedBand] / 10 - 273;
        }

        private void SetBandCombox(int bandCount, int defaultBand)
        {
            cmbBand.Items.Clear();
            for (int i = 1; i <= bandCount; i++)
                cmbBand.Items.Add(i.ToString());
            if (bandCount >= defaultBand)
                cmbBand.SelectedIndex = defaultBand - 1;
        }

        public void Apply(ISmartSession session)
        {
            _session = session;
            ICanvasViewer v = _session.SmartWindowManager.ActiveCanvasViewer;
            if (v != null)
                _canvas = v.Canvas;
            _mSession = _session.MonitoringSession as IMonitoringSession;
        }

        private void InitListView()
        {
            listView1.Columns.Clear();
            listView1.Columns.Add(_fieldName);
            listView1.Columns[0].Width = 120;
            ClearListViewItems();
        }

        public void Free()
        {
            ClearLineLayers();
            ClearListViewItems();
            //删除左上角显示的点信息
            _infoLayer.InfoItems.Clear();
            //移除信息层
            if (_canvas.LayerContainer != null && _canvas.LayerContainer.Layers.Contains(_infoLayer))
                _canvas.LayerContainer.Layers.Remove(_infoLayer);
            _canvas.CurrentViewControl = new DefaultControlLayer();
            _progressTracker = null;
            _selectBandChanged = null;
            RemoveFeature = null;
        }

        private void ClearLineLayers()
        {
            if (linLayerName == null || linLayerName.Count == 0)
                return;
            ICanvasViewer viewer = _session.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return;
            CodeCell.AgileMap.Core.ILayer removeLayer = null;
            foreach (string name in linLayerName)
            {
                foreach (GeoDo.RSS.Core.DrawEngine.ILayer layer in viewer.Canvas.LayerContainer.Layers)
                {
                    if (layer is IVectorHostLayer)
                    {
                        IVectorHostLayer hostLayer = layer as IVectorHostLayer;
                        IMap map = hostLayer.Map as IMap;
                        if (map.LayerContainer != null)
                            removeLayer = map.LayerContainer.GetLayerByName(name);
                        if (removeLayer != null)
                        {
                            map.LayerContainer.Remove(removeLayer);
                            break;
                        }
                    }
                }
            }
            Update();
        }

        private void ClearListViewItems()
        {
            if (listView1.Items.Count != 0)
            {
                for (int i = listView1.Items.Count - 1; i >= 0; i--)
                {
                    if (RemoveFeature != null)
                        RemoveFeature(listView1.Items[i].Tag as Feature);
                }
                listView1.Items.Clear();
            }
            ILabelLayer lyr = FindLabelLayer(_layerName);
            if (lyr != null)
                lyr.Dispose();
            ICanvasViewer viewer = _session.SmartWindowManager.ActiveCanvasViewer;
            if (viewer != null)
            {
                ILabelService srv = (viewer as ICurrentRasterInteractiver).LabelService;
                srv.Reset();
            }
            _iceLineFeatureOid = 0;
        }

        private void AddFeatureToListView(Feature feature)
        {
            ListViewItem it = feature.ToListViewItem();
            if (it != null)
            {
                it.Tag = feature;
                listView1.Items.Add(it);
            }
        }

        private void ApplyColor(ILabelLayer lyr)
        {
            ISymbol symbol = lyr.Symbol;
            if (symbol == null)
                return;
            IFillSymbol fillSymbol = symbol as IFillSymbol;
            if (fillSymbol == null)
                return;
            fillSymbol.OutlineSymbol.Color = Color.FromArgb(255, 128, 128);//btnColor.BackColor;
        }

        private ILabelLayer GetLabelLayer(string name, string[] fieldNames)
        {
            ICanvasViewer viewer = _session.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return null;
            ILabelService srv = (viewer as ICurrentRasterInteractiver).LabelService;
            if (srv == null)
                return null;
            return srv.GetLabelLayer(name, fieldNames);
        }

        private ILabelLayer FindLabelLayer(string name)
        {
            ICanvasViewer viewer = _session.SmartWindowManager.ActiveCanvasViewer;
            if (viewer == null)
                return null;
            ILabelService labelService = (viewer as ICurrentRasterInteractiver).LabelService;
            if (labelService == null)
                return null;
            return labelService.FindLabelLayer(name);
        }

        private void cmbBand_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedBand = int.Parse(cmbBand.Text) - 1;
            if (_selectBandChanged != null)
                _selectBandChanged(_selectedBand);
        }

        private void btnRemoveFeatures_Click(object sender, EventArgs e)
        {
            if (_layerName == null)
                return;
            if (listView1.SelectedIndices.Count == 0)
                return;
            ILabelLayer lyr = FindLabelLayer(_layerName);
            if (lyr != null)
            {
                for (int i = listView1.SelectedItems.Count - 1; i >= 0; i--)
                {
                    lyr.RemoveFeature(listView1.SelectedItems[i].Tag as Feature);
                    if (RemoveFeature != null)
                        RemoveFeature(listView1.SelectedItems[i].Tag as Feature);
                    RemoveFeatureToDic(listView1.SelectedItems[i].Tag as Feature);
                    listView1.Items.Remove(listView1.SelectedItems[i]);
                }
            }
        }

        private void RemoveFeatureToDic(Feature feature)
        {
            //同步删除冰缘线拐点
            if (feature != null && _iceFeatures != null &&
                _iceFeatures.Count != 0 && _iceFeatures.ContainsKey(feature))
            {
                IceFreeCurveInfoLayer.InfoItem[] infoItem = _iceFeatures[feature];
                if (infoItem != null && infoItem.Length != 0)
                {
                    for (int i = 0; i < infoItem.Length; i++)
                    {
                        _infoLayer.InfoItems.Remove(infoItem[i]);
                    }
                }
                _iceFeatures.Remove(feature);
            }
            _canvas.Refresh(enumRefreshType.All);
        }

        private void btnSaveToFile_Click(object sender, EventArgs e)
        {
            linLayerName.Clear();
            SaveToShp();
        }

        private void SaveToShp()
        {
            try
            {
                if (_iceFeatures == null || _iceFeatures.Count == 0)
                {
                    MsgBox.ShowInfo("没有绘制的海冰冰缘线");
                    return;
                }
                string iceLineShpFileName = GetIceLineShpFileName();

                TryExportIceLine(_iceFeatures.Keys.ToArray(), iceLineShpFileName);

                //TryExport2ShapeFile(_iceFeatures.Keys.ToArray(), iceLineShpFileName, enumShapeType.Polyline);

                IExtractResult result = new FileExtractResult("IEDG", iceLineShpFileName, true);

                IMonitoringSubProduct msp = (_session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
                //by chennan 修正冰缘线结果显示为多个图层问题
                //DisplayResultClass.DisplayResult(_session, msp, result, false);

                string iceControlPointShpFileName = GeoDo.RSS.MIF.Prds.ICE.IceEdgeFileNameHelper.GetIceEdgeControlInfoFilename(iceLineShpFileName);
                //Path.ChangeExtension(iceLineShpFileName, ".controlpoint.shp");                
                TryExportIceControlPoint(_iceFeatures.Values.ToArray(), iceControlPointShpFileName);
                StringBuilder str = new StringBuilder();
                str.AppendLine("冰缘线导出成功：");
                str.AppendLine(iceLineShpFileName);
                //by chennan 记录导出时的冰缘线图层名,用于关闭窗体时删除相应图层
                linLayerName.Add(Path.GetFileNameWithoutExtension(iceLineShpFileName));
                str.AppendLine(iceControlPointShpFileName);
                //if (ExportToShapeFile != null)
                //    ExportToShapeFile(iceLineShpFileName);
                ActiveWorkSpace(iceLineShpFileName);
                MsgBox.ShowInfo(str.ToString());
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }
        }

        private void ActiveWorkSpace(string filename)
        {
            if (_mSession != null && _mSession.ActiveMonitoringSubProduct != null)
            {
                FileExtractResult result = new FileExtractResult(_subProductIdentify, filename, true);
                DisplayResultClass.DisplayResult(_session, _mSession.ActiveMonitoringSubProduct, result, false);
            }
        }

        private void TryExportIceLine(Feature[] features, string iceLineShpFileName)
        {
            double geoX, geoY = 0;
            GeoDo.RSS.Core.DrawEngine.ICoordinateTransform tran = _canvas.CoordTransform;
            List<Feature> fets = new List<Feature>();
            int oid = 0;
            foreach (Feature fet in features)
            {
                ShapePolyline line = fet.Geometry as ShapePolyline;
                List<ShapeLineString> newLineStrings = new List<ShapeLineString>();
                ShapeLineString newpart = null;
                ShapePoint newPt = null;
                ShapeLineString part = null; ;
                List<ShapePoint> newpts = new List<ShapePoint>();
                for (int m = 0; m < line.Parts.Length; m++)
                {
                    part = line.Parts[m];
                    for (int j = 0; j < part.Points.Length; j++)
                    {
                        ShapePoint sp = part.Points[j];
                        if (line.IsProjected)
                        {
                            tran.Prj2Geo(sp.X, sp.Y, out geoX, out geoY);
                            newPt = new ShapePoint(geoX, geoY);
                        }
                        else
                        {
                            newPt = new ShapePoint(sp.X, sp.Y);
                        }
                        newpts.Add(newPt);
                    }
                    newpart = new ShapeLineString(newpts.ToArray());
                    newLineStrings.Add(newpart);
                }
                ShapePolyline sply = new ShapePolyline(newLineStrings.ToArray());
                string[] fieldvalue = new string[] { fet.FieldValues[0] };
                Feature outFet = new Feature(oid, sply, new string[] { _fieldName }, fieldvalue, null);
                oid++;
                fets.Add(outFet);
            }
            TryExport2ShapeFile(fets.ToArray(), iceLineShpFileName, enumShapeType.Polyline);
        }

        private void TryExportIceControlPoint(IceFreeCurveInfoLayer.InfoItem[][] infoItem, string iceControlPointShpFileName)
        {
            List<Feature> fets = new List<Feature>();
            int i = 0;
            foreach (IceFreeCurveInfoLayer.InfoItem[] items in infoItem)
            {
                foreach (IceFreeCurveInfoLayer.InfoItem item in items)
                {
                    Feature fet = GetFeature(item, i);
                    i++;
                    fets.Add(fet);
                }
            }
            TryExport2ShapeFile(fets.ToArray(), iceControlPointShpFileName, enumShapeType.Point);
        }

        private Feature GetFeature(IceFreeCurveInfoLayer.InfoItem item, int oid)
        {
            ShapePoint pt = new ShapePoint(item.Longitude, item.Latitude);
            //ShapePoint[] pts =new ShapePoint[]{pt};
            //ShapeMultiPoint mpt = new ShapeMultiPoint(pts);
            Feature fet = new Feature(oid, pt, new string[] { "No", "Temp" }, new string[] { item.No.ToString(), item.Temperature.ToString("f2") }, null);
            return fet;
        }

        private string GetIceLineShpFileName()
        {
            IRasterDrawing drawing = _canvas.PrimaryDrawObject as IRasterDrawing;
            IRasterDataProvider dp = drawing.DataProviderCopy;
            RasterIdentify rid = new RasterIdentify(dp.fileName);
            rid.ProductIdentify = "ICE";
            rid.SubProductIdentify = _subProductIdentify;
            rid.Format = ".shp";
            return rid.ToWksFullFileName(".shp");
        }

        private void TryExport2ShapeFile(Feature[] iceLines, string shpFileName, enumShapeType shapeType)
        {
            int cntCount = iceLines.Length;
            string tip = "正在将等值线导出为矢量文件({0}/{1})...";
            int progress = 0;
            float interval = 100f / cntCount;
            IEsriShapeFilesWriter writer = null;
            try
            {
                writer = new EsriShapeFilesWriterII(shpFileName, shapeType);
                writer.BeginWrite();
                Feature[] buffer = new Feature[1];
                for (int i = 0; i < cntCount; i++)
                {
                    if (iceLines[i] == null)
                        continue;
                    progress = (int)(Math.Floor(interval * i));
                    //Feature fet = GetIceLineFeature(iceLines[i], resX, resY, minX, maxY, i);
                    Feature fet = iceLines[i];
                    if (fet != null)
                    {
                        buffer[0] = fet;
                        writer.Write(buffer);
                    }
                    if (_progressTracker != null)
                        _progressTracker(progress, string.Format(tip, i, cntCount));
                }
            }
            finally
            {
                if (writer != null)
                    writer.EndWriter();
            }
        }

        private void TryExportIceLine2Shp(Feature[] iceLines, string shpFileName)
        {
            int cntCount = iceLines.Length;
            string tip = "正在将等值线导出为矢量文件({0}/{1})...";
            int progress = 0;
            float interval = 100f / cntCount;
            IEsriShapeFilesWriter writer = null;
            try
            {
                writer = new EsriShapeFilesWriterII(shpFileName, enumShapeType.Polyline);
                writer.BeginWrite();
                Feature[] buffer = new Feature[1];
                for (int i = 0; i < cntCount; i++)
                {
                    if (iceLines[i] == null)
                        continue;
                    progress = (int)(Math.Floor(interval * i));
                    //Feature fet = GetIceLineFeature(iceLines[i], resX, resY, minX, maxY, i);
                    Feature fet = iceLines[i];
                    if (fet != null)
                    {
                        buffer[0] = fet;
                        writer.Write(buffer);
                    }
                    if (_progressTracker != null)
                        _progressTracker(progress, string.Format(tip, i, cntCount));
                }
            }
            finally
            {
                if (writer != null)
                    writer.EndWriter();
            }
        }
    }

    public static class FeatureExt
    {
        public static ListViewItem ToListViewItem(this CodeCell.AgileMap.Core.Feature fet)
        {
            string[] values = fet.FieldValues;
            if (values == null || values.Length == 0)
                return null;
            ListViewItem it = new ListViewItem(values[0]);
            for (int i = 1; i < values.Length; i++)
            {
                it.SubItems.Add(values[i]);
            }
            return it;
        }
    }
}
