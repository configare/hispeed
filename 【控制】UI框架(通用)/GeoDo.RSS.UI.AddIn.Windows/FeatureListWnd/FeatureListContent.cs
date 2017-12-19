using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using System.Drawing.Imaging;
using GeoDo.RSS.MIF.Core;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    public partial class FeatureListContent : UserControl, IToolWindowContent
    {
        private ISmartSession _session;
        public delegate void SelectBandChangedHandler(int band);
        public SelectBandChangedHandler _selectBandChanged = null;
        public delegate void RemoveFeatureHandler(Feature feature);
        public event RemoveFeatureHandler RemoveFeature = null;
        private ICanvas _canvas;
        private IMonitoringSession _msession;
        private string _layerName;
        private Dictionary<string, Func<short, bool>> _filters = null;
        private string[] _fieldNames = null;
        private IPixelIndexMapper _extractedPixels = null;

        public FeatureListContent()
        {
            InitializeComponent();
            cbPencilType.SelectedIndex = 0;
        }

        public string LayerName
        {
            set
            {
                _layerName = value;
            }
        }

        public void InitList(string[] fieldNames)
        {
            if (fieldNames == null)
                return;
            UpdateHeadersOfListView(fieldNames);
        }

        /// <summary>
        /// 自定义面积统计项
        /// </summary>
        /// <param name="filters">统计的项目和条件</param>
        public void InitStatContent(Dictionary<string, Func<short, bool>> filters)
        {
            _filters = filters;
            _fieldNames = UpdateFieldNames(_filters);
            UpdateHeadersOfListView(_fieldNames);
        }

        private string[] UpdateFieldNames(Dictionary<string, Func<short, bool>> filters)
        {
            string[] statFieldNames = filters.Keys.ToArray();
            List<string> header = new List<string>();
            header.AddRange(new string[] { "名称" });
            header.AddRange(statFieldNames);
            return header.ToArray();
        }

        private void StatFilterAreas()
        {
            ICurrentRasterInteractiver inter = _session.SmartWindowManager.ActiveCanvasViewer as ICurrentRasterInteractiver;
            if (inter != null)
            {
                inter.StartAOIDrawing(GetPencilType(), (aoi, shape) =>
                {
                    string layerName = "System:StatArea";
                    ILabelLayer lyr = GetLabelLayer((_activeViewer as ICurrentRasterInteractiver), layerName, _fieldNames);
                    if (lyr != null)
                    {
                        Dictionary<string, double> areas = StatArea(aoi, _filters);
                        _layerName = layerName;
                        //ApplyColor(lyr);
                        //lyr.LabelDef.Fieldname = "面积(平方公里)";

                        ApplyColor(lyr);
                        lyr.LabelDef.Fieldname = _fieldNames[1];
                        lyr.LabelDef.EnableLabeling = true;

                        CodeCell.AgileMap.Core.Feature fet = null;
                        try
                        {
                            fet = ToFeature(shape, areas.Values.ToArray());
                        }
                        catch (Exception ex)
                        {
                            _session.PrintMessage(ex);
                        }
                        if (fet != null)
                        {
                            lyr.AddFeature(fet);
                            AddFeatureToListView(fet);
                        }
                    }
                });
            }
        }

        private Dictionary<string, double> StatArea(int[] aoi, Dictionary<string, Func<short, bool>> filters)
        {
            IRasterDrawing drawing = _activeViewer.Canvas.PrimaryDrawObject as IRasterDrawing;
            if (drawing == null)
                return null;
            IRasterDataProvider prd = drawing.DataProvider;
            if (prd == null)
                return null;
            RasterOperator<short> rs = new RasterOperator<short>();
            if (_extractedPixels == null)
            {
                Dictionary<string, double> areas = rs.Area(prd, aoi, filters);
                return areas;
            }
            else
            {
                double area = rs.Area(prd, _extractedPixels, aoi);
                Dictionary<string, double> areas = new Dictionary<string, double>();
                areas.Add("判识面积", area);
                return areas;
            }
        }

        private CodeCell.AgileMap.Core.Feature ToFeature(Shape shape, double[] statAreas)
        {
            string[] fieldValues = new string[_fieldNames.Length];
            fieldValues[0] = OID.ToString();
            for (int i = 0; i < statAreas.Length; i++)
            {
                fieldValues[i + 1] = statAreas[i].ToString("0.###");
            }
            CodeCell.AgileMap.Core.Feature fet = new CodeCell.AgileMap.Core.Feature(OID, shape, _fieldNames, fieldValues, null);
            OID++;
            return fet;
        }

        public void UpdateLayer(ref Feature fet)
        {
            if (fet != null)
            {
                ILabelLayer lyr = GetLabelLayer((_activeViewer as ICurrentRasterInteractiver), _layerName, fet.FieldNames);
                if (lyr != null)
                {
                    ApplyColor(lyr);
                    lyr.LabelDef.Fieldname = fet.FieldNames[0];
                    lyr.LabelDef.EnableLabeling = true;
                    lyr.AddFeature(fet);
                    Feature[] features = lyr.GetAllFeature();
                    fet = features[features.Length - 1];
                    AddFeatureToListView(fet);
                }
            }
        }

        public void InitBand(int bandCount, int defaultBand)
        {
            for (int i = 1; i <= bandCount; i++)
                cmbBand.Items.Add(i.ToString());
            if (bandCount >= defaultBand)
                cmbBand.SelectedIndex = defaultBand - 1;
        }

        public void InitControl(bool btnStatAreaVis, bool btnStatBinaryAreaVis, bool cbPencilTypeVis, bool cmbBandVis)
        {
            btnStatArea.Visible = btnStatAreaVis;
            btnStatBinaryArea.Visible = btnStatBinaryAreaVis;
            cmbBand.Visible = cmbBandVis;
            labBand.Visible = cmbBandVis;
            cbPencilType.Visible = cbPencilTypeVis;
            labPencilType.Visible = cbPencilTypeVis;
            InitItems();
        }

        public void Apply(ISmartSession session)
        {
            if (session == null)
                return;

            _session = session;
            _msession = _session.MonitoringSession as IMonitoringSession;
            if (_session.SmartWindowManager.OnActiveWindowChanged != null)
                _session.SmartWindowManager.OnActiveWindowChanged -= new OnActiveWindowChangedHandler(ActiveWindowChanged);
            _session.SmartWindowManager.OnActiveWindowChanged += new OnActiveWindowChangedHandler(ActiveWindowChanged);

            _activeViewer = session.SmartWindowManager.ActiveCanvasViewer;
            if (_activeViewer != null)
                _canvas = _activeViewer.Canvas;
        }

        private ICanvasViewer _activeViewer = null;//用于统计的窗口windows。

        /// <summary>
        /// 窗口切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="oldWindow"></param>
        /// <param name="newWindow"></param>
        void ActiveWindowChanged(object sender, ISmartWindow oldWindow, ISmartWindow newWindow)
        {
            _extractedPixels = null;
            if (newWindow == null || !(newWindow is ICanvasViewer))
                return;
            if (_activeViewer == newWindow)
                return;
            if (_filters != null)
                _filters.Clear();
            if (_activeViewer != null)
            {
                if (oldWindow != null && oldWindow is ICanvasViewer)
                {
                    ClearFeature(oldWindow as ICanvasViewer);
                }
                else
                {
                    ClearFeature(_activeViewer);
                }
                _activeViewer.Canvas.Refresh(enumRefreshType.VectorLayer);
            }
            _activeViewer = newWindow as ICanvasViewer;
            _activeViewer.OnWindowClosed =
                (wSender, e) =>
                {
                    Free();
                    _activeViewer = null;
                };
            StatContent statContent = null;
            GeoDo.RSS.Core.DrawEngine.ILayer[] layers = _activeViewer.Canvas.LayerContainer.Layers.ToArray();
            foreach (GeoDo.RSS.Core.DrawEngine.ILayer layer in layers)
            {
                if (layer is BinaryBitmapLayer)//二值图层
                {
                    IExtractingSession se = (_session.MonitoringSession as MonitoringSession).ExtractingSession;
                    _extractedPixels = se.GetBinaryValuesMapper(se.CurrentProduct.Identify, se.CurrentSubProduct.Identify);
                    if (_extractedPixels == null)
                        continue;
                    string layerName = layer.Name;
                    statContent = FeatureListStatConfig.MatchStatContent(layerName);
                }
                else if (layer is IRasterLayer)
                {
                    IRasterLayer rasterLayer = layer as IRasterLayer;
                    if (rasterLayer == null)
                        continue;

                    IRasterDrawing rasterDrawing = rasterLayer.Drawing as IRasterDrawing;
                    if (rasterDrawing == null)
                        continue;
                    string layerName = layer.Name;
                    statContent = FeatureListStatConfig.MatchStatContent(layerName);
                }
            }
            if (statContent != null)
            {
                Dictionary<string, Func<short, bool>> filters = new Dictionary<string, Func<short, bool>>();
                Dictionary<string, int> sataItems = statContent.StatItems;
                string[] statKeys = sataItems.Keys.ToArray();
                int[] statValues = sataItems.Values.ToArray();
                for (int i = 0; i < statKeys.Length; i++)
                {
                    string key = statKeys[i];
                    int value = statValues[i];
                    filters.Add(key, (val) => { return val == value; });
                }
                InitStatContent(filters);
            }
            if (_filters == null || _filters.Count == 0)
                SataSimpleArea();
            else
                StatFilterAreas();
        }

        private void ClearFeature(ICanvasViewer cv)
        {
            if (listView1.Items.Count != 0)
            {
                OID = 0;
                ILabelLayer lyr = GetLabelLayer((cv as ICurrentRasterInteractiver), _layerName, null);
                if (lyr == null)
                    return;
                for (int i = listView1.Items.Count - 1; i >= 0; i--)
                {
                    lyr.RemoveFeature(listView1.Items[i].Tag as Feature);
                    if (RemoveFeature != null)
                        RemoveFeature(listView1.Items[i].Tag as Feature);
                }
                listView1.Items.Clear();
            }
            (cv as ICurrentRasterInteractiver).LabelService.Reset();
            cv.Canvas.CurrentViewControl = new DefaultControlLayer();
            cv.Canvas.Refresh(enumRefreshType.VectorLayer);
        }

        public void Free()
        {
            if (_session.SmartWindowManager.OnActiveWindowChanged != null)
                _session.SmartWindowManager.OnActiveWindowChanged -= new OnActiveWindowChangedHandler(ActiveWindowChanged);
            if (_session.SmartWindowManager.ActiveCanvasViewer != null)
            {
                if (_activeViewer != null)
                    ClearFeature(_activeViewer);
            }
        }

        private void InitFeature()
        {
            if (listView1.Items.Count != 0)
            {
                OID = 0;
                ILabelLayer lyr = GetLabelLayer((_activeViewer as ICurrentRasterInteractiver), _layerName, null);
                if (lyr == null)
                    return;
                for (int i = listView1.Items.Count - 1; i >= 0; i--)
                {
                    lyr.RemoveFeature(listView1.Items[i].Tag as Feature);
                    if (RemoveFeature != null)
                        RemoveFeature(listView1.Items[i].Tag as Feature);
                }
                listView1.Items.Clear();
            }
            (_activeViewer as ICurrentRasterInteractiver).LabelService.Reset();
            _activeViewer.Canvas.CurrentViewControl = new DefaultControlLayer();
        }

        private void btnStatArea_Click(object sender, EventArgs e)
        {
            if (_filters == null || _filters.Count == 0)
                SataSimpleArea();
            else
                StatFilterAreas();
        }

        private void SataSimpleArea()
        {
            string[] lstViewfieldNames = new string[] { "名称", "像元个数", "面积(平方公里)" };
            string[] fieldNames = new string[] { "name", "pixelcount", "area" };
            UpdateHeadersOfListView(lstViewfieldNames);
            ICurrentRasterInteractiver inter = _activeViewer as ICurrentRasterInteractiver;
            if (inter != null)
            {
                //inter
                inter.StartAOIDrawing(GetPencilType(), (aoi, shape) =>
                {
                    string layerName = "System:StatArea";
                    ILabelLayer lyr = GetLabelLayer((_activeViewer as ICurrentRasterInteractiver), layerName, fieldNames);
                    if (lyr != null)
                    {
                        _layerName = layerName;
                        ApplyColor(lyr);
                        lyr.LabelDef.Fieldname = "area";
                        CodeCell.AgileMap.Core.Feature fet = null;
                        try
                        {
                            fet = ToFeature(aoi, shape, fieldNames);
                        }
                        catch (Exception ex)
                        {
                            _session.PrintMessage(ex);
                        }
                        if (fet != null)
                        {
                            lyr.AddFeature(fet);
                            AddFeatureToListView(fet);
                        }
                    }
                });
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
            fillSymbol.OutlineSymbol.Color = btnColor.BackColor;
        }

        private string GetPencilType()
        {
            switch (cbPencilType.Text)
            {
                case "自由多边形":
                    return "FreeCurve";
                case "线段多边形":
                    return "Polygon";
                case "矩形":
                    return "Rectangle";
                case "圆形":
                    return "Circle";
            }
            return "FreeCurve";
        }

        private void UpdateHeadersOfListView(string[] fieldNames)
        {
            listView1.Columns.Clear();
            int i = 0;
            foreach (string name in fieldNames)
            {
                listView1.Columns.Add(name);
                listView1.Columns[i++].Width = 120;
            }
        }

        public void AddFeatureToListView(CodeCell.AgileMap.Core.Feature fet)
        {
            ListViewItem it = fet.ToListViewItem();
            if (it != null)
            {
                it.Tag = fet;
                listView1.Items.Add(it);
            }
        }

        private static int OID = 0;
        private CodeCell.AgileMap.Core.Feature ToFeature(int[] aoi,
            CodeCell.AgileMap.Core.Shape shape, string[] fieldNames)
        {
            string[] fieldValues = new string[fieldNames.Length];
            fieldValues[0] = OID.ToString();
            fieldValues[1] = aoi.Length.ToString();
            fieldValues[2] = StatArea(aoi).ToString("0.##");
            CodeCell.AgileMap.Core.Feature fet = new CodeCell.AgileMap.Core.Feature(OID, shape, fieldNames, fieldValues, null);
            OID++;
            return fet;
        }

        private double StatArea(int[] aoi)
        {
            IRasterDrawing drawing = _activeViewer.Canvas.PrimaryDrawObject as IRasterDrawing;
            if (drawing == null)
                return 0;
            IRasterDataProvider prd = drawing.DataProvider;
            if (prd == null)
                return 0;

            double retArea = 0;
            if (prd.CoordType == enumCoordType.GeoCoord)
            {
                Size size = new Size(prd.Width, prd.Height);
                int row = 0;
                double resX = prd.ResolutionX;
                double maxLat = prd.CoordEnvelope.MaxY;
                for (int i = 0; i < aoi.Length; i++)
                {
                    row = aoi[i] / size.Width;
                    retArea += RasterOperator<int>.ComputePixelArea(row, maxLat, resX);

                }
            }
            else if (prd.CoordType == enumCoordType.PrjCoord)
            {
                double pixelArea = prd.ResolutionX * prd.ResolutionY / 1000000;//平方公里
                for (int i = 0; i < aoi.Length; i++)
                {
                    retArea += pixelArea;
                }
            }
            else//raster pixelcount
            {
                retArea = aoi.Length;
            }
            return retArea;
        }

        private ILabelLayer GetLabelLayer(ICurrentRasterInteractiver viewer, string name, string[] fieldNames)
        {
            ILabelService srv = viewer.LabelService;
            if (srv == null)
                return null;
            return srv.GetLabelLayer(name, fieldNames);
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            using (ColorDialog dlg = new ColorDialog())
            {
                dlg.Color = btnColor.BackColor;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    btnColor.BackColor = dlg.Color;
                    UpdateLayerColor();
                }
            }
        }

        private void UpdateLayerColor()
        {
            IVectorHostLayer hostLayer = _activeViewer.Canvas.LayerContainer.VectorHost as IVectorHostLayer;
            if (hostLayer == null)
                return;
            IMap map = hostLayer.Map as IMap;
            CodeCell.AgileMap.Core.ILayer lyr = map.LayerContainer.GetLayerByName(_layerName);
            if (lyr != null)
            {
                CodeCell.AgileMap.Core.IFeatureLayer fetLayer = lyr as CodeCell.AgileMap.Core.IFeatureLayer;
                ISymbol symbol = fetLayer.Renderer.CurrentSymbol;
                if (symbol != null)
                {
                    IFillSymbol fillSymbol = symbol as IFillSymbol;
                    if (fillSymbol == null)
                        return;
                    fillSymbol.OutlineSymbol.Color = btnColor.BackColor;
                }
            }
        }

        private void btnRemoveFeatures_Click(object sender, EventArgs e)
        {
            if (_layerName == null)
                return;
            if (listView1.SelectedIndices.Count == 0)
                return;
            ILabelLayer lyr = GetLabelLayer((_activeViewer as ICurrentRasterInteractiver), _layerName, null);
            if (lyr == null)
                return;
            for (int i = listView1.SelectedItems.Count - 1; i >= 0; i--)
            {
                lyr.RemoveFeature(listView1.SelectedItems[i].Tag as Feature);
                if (RemoveFeature != null)
                    RemoveFeature(listView1.SelectedItems[i].Tag as Feature);
                listView1.Items.Remove(listView1.SelectedItems[i]);
            }
            _activeViewer.Canvas.Refresh(enumRefreshType.VectorLayer);
        }

        private void InitItems()
        {
            if (listView1.Items.Count != 0)
            {
                OID = 0;
                ILabelLayer lyr = GetLabelLayer((_activeViewer as ICurrentRasterInteractiver), _layerName, null);
                if (lyr == null)
                    return;
                for (int i = listView1.Items.Count - 1; i >= 0; i--)
                {
                    lyr.RemoveFeature(listView1.Items[i].Tag as Feature);
                    if (RemoveFeature != null)
                        RemoveFeature(listView1.Items[i].Tag as Feature);
                }
                listView1.Items.Clear();
            }
            _activeViewer.Canvas.Refresh(enumRefreshType.VectorLayer);
        }

        private CodeCell.AgileMap.Core.ILayer TryGetLayer(string layerName)
        {
            IVectorHostLayer hostLayer = _activeViewer.Canvas.LayerContainer.VectorHost as IVectorHostLayer;
            if (hostLayer == null)
                return null;
            IMap map = hostLayer.Map as IMap;
            CodeCell.AgileMap.Core.ILayer lyr = map.LayerContainer.GetLayerByName(layerName);
            return lyr;
        }

        private void cmbBand_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_selectBandChanged != null)
                _selectBandChanged(int.Parse(cmbBand.Text) - 1);
        }

        private void btnSaveToFile_Click(object sender, EventArgs e)
        {
            SaveToShp();
        }

        private void cbPencilType_Click(object sender, EventArgs e)
        {

        }

        private void SaveToShp()
        {
            try
            {
                string layerName = "System:StatArea";
                ILabelLayer lyr = GetLabelLayer((_activeViewer as ICurrentRasterInteractiver), layerName, _fieldNames);
                if (lyr == null || lyr.GetAllFeature().Length == 0)
                {
                    MsgBox.ShowInfo("没有绘制AOI！");
                    return;
                }
                Feature[] features = lyr.GetAllFeature();
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Filter = SupportedFileFilters.VectorFilterString;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string fileName = dialog.FileName;
                        TryExportPolygon(features, fileName);
                        StringBuilder str = new StringBuilder();
                        str.AppendLine("矢量导出成功：");
                        str.AppendLine(fileName);
                        MsgBox.ShowInfo(str.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBox.ShowInfo(ex.Message);
            }
            finally
            {
            }
        }

        private void TryExportPolygon(Feature[] features, string shpFileName)
        {
            EsriShapeFilesWriterII w = new EsriShapeFilesWriterII(shpFileName, enumShapeType.Polygon);
            w.BeginWrite();
            Feature feature = features[0];
            string[] fieldNames = feature.FieldNames;
            Feature[] retFeature = new Feature[features.Length];
            for (int i = 0; i < features.Length; i++)
            {
                ShapePolygon ply = (features[i].Geometry as ShapePolygon).Clone() as ShapePolygon;
                ShapeRing[] rings = ply.Rings;
                ShapeRing[] newRings = new ShapeRing[rings.Length];
                for (int j = 0; j < rings.Length; j++)
                {
                    ShapePoint[] points = rings[j].Points;
                    ShapePoint[] newPoints = new ShapePoint[points.Length];
                    for (int p = 0; p < points.Length; p++)
                    {
                        double prjx = points[p].X;
                        double prjy = points[p].Y;
                        double geox, geoy;
                        _canvas.CoordTransform.Prj2Geo(prjx, prjy, out geox, out geoy);
                        newPoints[p] = new ShapePoint(geox, geoy);
                    }
                    newRings[j] = new ShapeRing(newPoints);
                }
                ShapePolygon newPly = new ShapePolygon(newRings);
                Feature fet = new Feature(features[i].OID, newPly, fieldNames, features[i].FieldValues, null);
                retFeature[i] = fet;
            }
            w.Write(retFeature);
            w.EndWriter();
        }
    }
}
