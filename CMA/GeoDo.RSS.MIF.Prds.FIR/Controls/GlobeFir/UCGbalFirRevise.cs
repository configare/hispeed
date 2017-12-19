/*========================================================================
* 功能概述：全球火点数据修正界面,实现矢量火点的读取,显示,可视化编辑,保存等功能
* 
* 创建者：张延冰    时间：2013.08.12
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;
using GeoDo.RSS.Core.Grid;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.RSS.UI.Bricks;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public partial class UCGbalFirRevise : System.Windows.Forms.UserControl
    {
        private const string _openFileFilter = "HDF File(*.hdf)|*.hdf";
        private const string _saveFileFilter = "SHP File(*.shp)|*.shp";
        private GeoDo.RSS.Core.View.CanvasHost canvasHost1;
        private IVectorHostLayer _vectorMapHostLayer;
        private IVectorHostLayer _shpFileHostLayer;
        private GeoGridLayer _geoGridLayer;
        private Color _gridColor = Color.Gray;
        private bool _isDataLoaded = false;//是否加载了数据
        private string _hdfFileName = "";
        private string _filepath = "";
        private string _outFilename = "";
        private Font _normalFont = new Font("微软雅黑", 9);
        private Font _selectedFont = new Font("微软雅黑", 9, FontStyle.Bold);
        private ISmartSession _smartSession;
        private int _allPtCount = 0;
        private int _labeledPt = 0;
        private bool _gridVisible = true;
        private Feature[] _features;
        private string _curShpLayerName = null;
        //标记的动作是删除还是取消删除
        private bool _maskIsDelete = true;
        //标记层，用于标记选中的矢量点
        private SelectMaskLayer _maskLayer = null;
        private PencilToolLayer _toolLayer = null;

        public UCGbalFirRevise(ISmartSession session)
        {
            InitializeComponent();
            this.Text = "全球火点修正";
            _smartSession = session;
            AddCanvasHost();
            Load += new EventHandler(UCGbalFirRevise_Load);
            SizeChanged += new EventHandler(UCGbalFirRevise_SizeChanged);
            Disposed += new EventHandler(UCGbalFirRevise_Disposed);
            this.lstbFilelist.MouseDoubleClick += new MouseEventHandler(lstbFilelist_MouseDoubleClick);
            panel1.Visible = false;
            InitializeInterface();
        }

        #region 界面初始化
        private void InitializeInterface()
        {
            btnDeleteFile.Enabled = false;
            btnSave.Enabled = false;
            btnAOIDraw.Enabled = false;
            btnOpenOutDir.Enabled = false;
            txtOutFile.Text = "";
            sbInfo.Text = "消息栏";
            txtPointCount.Visible = false;
            txtDelPtCount.Visible = false;
            txtReCount.Visible = false;
            btnDelSelectPt.Enabled = false;
            btnUndo.Enabled = false;
        }

        private void EnableEdit()
        {
            btnDeleteFile.Enabled = true;
            btnAOIDraw.Enabled = true;
            txtPointCount.Visible = true;
            txtDelPtCount.Visible = true;
            txtReCount.Visible = true;
            btnDelSelectPt.Enabled = true;
            btnUndo.Enabled = true;

        }
        
        void UCGbalFirRevise_SizeChanged(object sender, EventArgs e)
        {
            if (canvasHost1.Canvas != null)
            {
                ToWorldViewport();
            }
        }

        private void AddCanvasHost()
        {
            this.canvasHost1 = new GeoDo.RSS.Core.View.CanvasHost();
            this.canvasHost1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasHost1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.canvasHost1.Location = new System.Drawing.Point(0, 25);
            this.canvasHost1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.canvasHost1.Name = "canvasHost1";
            int width = cvPanel.Width;
            int height = cvPanel.Height - toolStrip3.Height;
            this.canvasHost1.Size = new System.Drawing.Size(width, height);
            this.canvasHost1.AutoSize = true;
            this.canvasHost1.Load += new System.EventHandler(this.canvasHost1_Load);
            cvPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            cvPanel.Visible = true;
            cvPanel.Controls.Add(this.canvasHost1);
        }

        private void UCGbalFirRevise_Load(object sender, EventArgs e)
        {
            _maskLayer = new SelectMaskLayer();
            this.canvasHost1.Canvas.LayerContainer.Layers.Add(_maskLayer);
            this.canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        void canvasHost1_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
                return;
            CreateVectorHost();
        }

        private void CreateVectorHost()
        {
            TryApplyDefaultBackgroudLayer();
            ICanvas canvas = this.canvasHost1.Canvas;
            _vectorMapHostLayer = new VectorHostLayer(null);
            _vectorMapHostLayer.Set(canvas);
            canvas.LayerContainer.Layers.Add(_vectorMapHostLayer as GeoDo.RSS.Core.DrawEngine.ILayer);
            _shpFileHostLayer = new VectorHostLayer(null);
            _shpFileHostLayer.Set(canvas);
            canvas.LayerContainer.Layers.Add(_shpFileHostLayer as GeoDo.RSS.Core.DrawEngine.ILayer);
            LoadWorldMap();
            AddGeoGrid(canvas);
        }
        #endregion

        #region 加载地图
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

        private void AddGeoGrid(ICanvas c)
        {
            GeoGridLayer lyr = new GeoGridLayer();
            lyr.GridSpan = 10;//经纬网间隔5
            lyr.GridColor = _gridColor;//网格颜色
            lyr.LatLabelStep = 5;//纬度步长2
            lyr.LonLabelStep = 5;//经度步长2
            _geoGridLayer = lyr;
            c.LayerContainer.Layers.Add(_geoGridLayer);
        }

        private void LoadWorldMap()
        {
            string mcdfile = AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\矢量地图\世界.mcd";//"数据引用\基础矢量\SimpleMap.mcd";
            if (File.Exists(mcdfile))
            {
                ApplyMap(mcdfile);
            }
            else
                MsgBox.ShowInfo("地图配置文件\"世界.mcd\"丢失,地图初始化不正确！");
        }

        public void ApplyMap(string mcdfile)
        {
            _vectorMapHostLayer.Apply(mcdfile);
        }

        #endregion

        #region 添加文件
        private void ItemAddFile_Click(object sender, EventArgs e)
        {
            CheckFileSave();
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Title = "添加要修正的全球火点文件";
                    openFileDialog.CheckFileExists = true;
                    openFileDialog.Multiselect = false;
                    openFileDialog.Filter = _openFileFilter;
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        panel1.Visible = false;
                        TryRemoveShpLayer();
                        _hdfFileName = openFileDialog.FileName;
                        ReadGbalFirePtData(_hdfFileName);
                        _outFilename = GenRGFRFiename(_hdfFileName);
                        if (_isDataLoaded)
                        {
                            txtOutFile.Text = Path.GetDirectoryName(_outFilename);
                            btnSave.Enabled = true;
                            sbInfo.Text = Path.GetFileName(_hdfFileName);
                            EnableEdit();
                        }
                    }
                }
                UpdateMaskText();
            }
            catch (SystemException ex)
            {
                MsgBox.ShowInfo("出错：" + ex.Message);
            }
        }

        private void ItemAddPath_Click(object sender, EventArgs e)
        {
            CheckFileSave();
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                panel1.Visible = true;
                sbInfo.Text = dialog.SelectedPath;
                _filepath = dialog.SelectedPath;
                InitFileList(_filepath);
            }
        }

        private void btnOpenOutDir_Click(object sender, EventArgs e)
        {
            string dir = Path.GetDirectoryName(_outFilename);
            if (!string.IsNullOrEmpty(dir))
            {
                System.Diagnostics.Process.Start("Explorer.exe", "/select," + _outFilename);
            }
            else
            {
                MessageBox.Show("无法打开文件位置，请确定输出文件路径正确！");
            }
        }

        private void InitFileList(string path)
        {
            lstbFilelist.Items.Clear();
            string[] files = Directory.GetFiles(path, "*.HDF", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return;
            foreach (string file in files)
                lstbFilelist.Items.Add(Path.GetFileName(file));
        }

        void lstbFilelist_MouseDoubleClick(object sender, EventArgs e)
        {
            try
            {
                CheckFileSave();
                string fname = this.lstbFilelist.SelectedItem.ToString();
                _hdfFileName = _filepath + '\\' + fname;
                TryRemoveShpLayer();
                ReadGbalFirePtData(_hdfFileName);
                _outFilename = GenRGFRFiename(_hdfFileName);
                if (_isDataLoaded)
                {
                    txtOutFile.Text = Path.GetDirectoryName(_outFilename);
                    btnSave.Enabled = true;
                    sbInfo.Text = fname;
                    UpdateMaskText();
                    EnableEdit();
                }
            }
            catch (System.Exception ex)
            {
                MsgBox.ShowInfo("出错：" + ex.Message);
            }
        }
        #endregion

        #region 文件处理

        private void CheckFileSave()
        {
            if (_labeledPt != 0)
            {
                DialogResult dlgResult = MessageBox.Show("文件已改变，是否将更改保存到文件中？", "提示", MessageBoxButtons.YesNo);
                if (dlgResult == DialogResult.Yes)
                {
                    Feature[] lestfeatures = GetLeftFeatures();
                    if (lestfeatures != null && lestfeatures.Length > 0)
                    {
                        SaveToShpFile(_outFilename, lestfeatures);
                        this.canvasHost1.Canvas.CurrentViewControl = new DefaultControlLayer();
                        this.canvasHost1.Canvas.Refresh(enumRefreshType.All);
                    }
                }
                else
                {
                    TryRemoveShpLayer();
                    InitializeInterface();
                }
            }
            return;
        }

        private void ReadGbalFirePtData(string filename)
        {
            using (HdfGlobalFirePointReader dr = VectorDataReaderFactory.GetUniversalDataReader(filename) as HdfGlobalFirePointReader)
            {
                if (dr == null)
                {
                    throw new FileLoadException("读取火点信息失败！文件中可能不存在火点！", Path.GetFileName(filename));
                }
                Feature[] fets = dr.Features;
                if (fets != null || fets.Length > 0)
                {
                    _features = fets;
                    _allPtCount = _features.Length;
                    AddShpLayer(_hdfFileName);
                    _isDataLoaded = true;
                }
            }
        }

        public void AddShpLayer(string strVecName)
        {
            _shpFileHostLayer.AddData(strVecName, null);
            CodeCell.AgileMap.Core.IMap map = _shpFileHostLayer.Map as CodeCell.AgileMap.Core.IMap;
            CodeCell.AgileMap.Core.FeatureLayer fetL = map.LayerContainer.Layers[0] as CodeCell.AgileMap.Core.FeatureLayer;
            _curShpLayerName = fetL.Name;
            SimpleMarkerSymbol sym = new SimpleMarkerSymbol(masSimpleMarkerStyle.Circle);
            sym.Size = new System.Drawing.Size(4, 4);
            fetL.Renderer = new SimpleFeatureRenderer(sym);
            CodeCell.AgileMap.Core.FeatureClass fetc = fetL.Class as CodeCell.AgileMap.Core.FeatureClass;
            CodeCell.AgileMap.Core.Envelope evp = fetc.FullEnvelope.Clone() as CodeCell.AgileMap.Core.Envelope;
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope cvEvp = new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(evp.MinX, evp.MaxX, evp.MinY, evp.MaxY);
            this.canvasHost1.Canvas.CurrentEnvelope = cvEvp;
            this.canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        private string GenRGFRFiename(string GFRFilename)
        {
            RasterIdentify id = new RasterIdentify(GFRFilename);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "RGFR";
            id.IsOutput2WorkspaceDir = false;
            id.Format = ".shp";
            return id.ToWksFullFileName(".shp");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Feature[] lestfeatures = GetLeftFeatures();
            if (lestfeatures != null && lestfeatures.Length > 0)
            {
                SaveToShpFile(_outFilename, lestfeatures);
                this.canvasHost1.Canvas.CurrentViewControl = new DefaultControlLayer();
                this.canvasHost1.Canvas.Refresh(enumRefreshType.All);
            }
        }

        private void hDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog dlg = new SaveFileDialog())
                {
                    dlg.Title = "修正全球火点HDF另存";
                    dlg.OverwritePrompt = true;
                    dlg.Filter = _openFileFilter;
                    dlg.FileName = _hdfFileName;
                    dlg.InitialDirectory = Path.GetDirectoryName(_hdfFileName);
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        string outhdffilename = dlg.FileName;
                        Feature[] lestfeatures = GetLeftFeatures();
                        if (lestfeatures != null && lestfeatures.Length > 0)
                        {
                            HDF5Filter.SaveAsNewHDF5ByFeatures(_hdfFileName, outhdffilename, lestfeatures, new Action<int, string>(Progress));
                            this.canvasHost1.Canvas.CurrentViewControl = new DefaultControlLayer();
                            this.canvasHost1.Canvas.Refresh(enumRefreshType.All);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("处理出错：" + ex.Message);
            }
            finally
            {
                FinishProgress();
            }
        }

        private void dATToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog dlg = new SaveFileDialog())
                {
                    dlg.Title = "火点另存为判识结果";
                    dlg.OverwritePrompt = true;
                    dlg.Filter = "判识结果文件(*.DAT)|*.dat";
                    RasterIdentify datid=new RasterIdentify(Path.GetFileName(_hdfFileName));
                    datid.ProductIdentify = "FIR";
                    datid.SubProductIdentify = "DBLV";
                    dlg.FileName = datid.ToWksFileName(".dat");
                   //dlg.FileName = Path.GetFileNameWithoutExtension(_hdfFileName) + ".dat";
                    dlg.InitialDirectory = Path.GetDirectoryName(_hdfFileName);
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        SetDBLVParas paraset = new SetDBLVParas(dlg.FileName);
                        paraset.StartPosition = FormStartPosition.CenterScreen;
                        if (paraset.ShowDialog() == DialogResult.OK)
                        {
                            string outfilename = dlg.FileName;
                            if (File.Exists(outfilename))
                                File.Delete(outfilename);
                            Feature[] lestfeatures = GetLeftFeatures();
                            double resolution = paraset._resl;
                            if (lestfeatures != null && lestfeatures.Length > 0)
                            {
                                Save2DBLV.SaveToDBLVDat(_hdfFileName, outfilename, lestfeatures, resolution, new Action<int, string>(Progress));
                                Save2DBLV.OutputFireList(lestfeatures, GenOutPLSTname(_hdfFileName, outfilename), new Action<int, string>(Progress));
                                this.canvasHost1.Canvas.CurrentViewControl = new DefaultControlLayer();
                                this.canvasHost1.Canvas.Refresh(enumRefreshType.All);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("处理出错：" + ex.Message);
            }
            finally
            {
                FinishProgress();
            }
        }

        private string GenOutPLSTname(string hdffname,string datfname)
        {
            RasterIdentify id = new RasterIdentify(Path.GetFileName(hdffname));
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "PLST";
            string txtname = Path.Combine(Path.GetDirectoryName(datfname), id.ToWksFileName(".txt"));
            return txtname;
        }

        protected Feature[] GetLeftFeatures()
        {
            ShapePoint pt;
            string[] fieldValues;
            string[] fieldNames = new string[] { "年份", "月日", "时分", "纬度", "经度", "亚像元火点面积", "像元火点温度", "火点强度", "可信度" };
            List<Feature> leftFeatures = new List<Feature>();
            int objId = 0;
            try
            {
                Start(_allPtCount - _maskLayer.MaskPoints.Count);
                if (_maskLayer.MaskPoints.Count != 0)
                {
                    for (int i = 0; i < _allPtCount; i++)
                    {
                        if (_maskLayer.MaskPoints.ContainsKey(i))
                            continue;
                        pt = _features[i].Geometry as ShapePoint;
                        fieldValues = _features[i].FieldValues;
                        Feature f = new Feature(objId, pt, fieldNames, fieldValues, null);
                        leftFeatures.Add(f);
                        objId++;
                    }
                }
                else
                {
                    for (int i = 0; i < _allPtCount; i++)
                    {
                        pt = _features[i].Geometry as ShapePoint;
                        fieldValues = _features[i].FieldValues;
                        Feature f = new Feature(objId, pt, fieldNames, fieldValues, null);
                        leftFeatures.Add(f);
                        objId++;
                    }
                }
                return leftFeatures.ToArray();
            }
            catch (SystemException e)
            {
                MessageBox.Show("获取Features失败：" + e.Message);
                return null;
            }
        }

        protected void SaveToShpFile(string RGFRfilename, Feature[] leftFeatures)
        {
            try
            {
                EsriShapeFilesWriterII HdfRGFRWriter = new EsriShapeFilesWriterII(RGFRfilename, enumShapeType.Point);
                HdfRGFRWriter.BeginWrite();
                HdfRGFRWriter.Write(leftFeatures, new Action<int, string>(Progress));
                HdfRGFRWriter.EndWriter();
                btnOpenOutDir.Enabled = true;
                _labeledPt = 0;
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "HdfGlobalFirePointReader", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            finally
            {
                FinishProgress();
            }
        }

        private void btnDeleteFile_Click(object sender, EventArgs e)
        {
            CheckFileSave();
            TryRemoveShpLayer();
            InitializeInterface();
            this.canvasHost1.Canvas.CurrentViewControl = new DefaultControlLayer();
            this.canvasHost1.Canvas.Refresh(enumRefreshType.All);
            _isDataLoaded = false;
            _labeledPt = 0;
        }
        #endregion

        #region 视图响应
        private void btnGrid_Click(object sender, EventArgs e)
        {
            if (_gridVisible)
            {
                this.canvasHost1.Canvas.LayerContainer.Layers.Remove(_geoGridLayer);
                _gridVisible = false;
            }
            else
            {
                this.canvasHost1.Canvas.LayerContainer.Layers.Add(_geoGridLayer);
                _gridVisible = true;
            }
            this.canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        public void ToWorldViewport()
        {
            btnToWorld.Text = " √ " + "全球视图";
            btnToWorld.Font = _selectedFont;
            //投影坐标时该范围可能有误
            CoordEnvelope evp = new CoordEnvelope(-180, 180, -90, 90);
            _geoGridLayer.GridSpan = 20;
            ToViewport(evp);
            this.canvasHost1.Canvas.Refresh(enumRefreshType.All);
        }

        public void ToViewport(CoordEnvelope geoCoordEnvelope)
        {
            this.canvasHost1.Canvas.CoordTransform.Geo2Prj(geoCoordEnvelope);
            CurrentEnvelope = geoCoordEnvelope;
        }

        public CoordEnvelope CurrentEnvelope
        {
            set
            {
                if (value != null)
                    this.canvasHost1.Canvas.CurrentEnvelope = value.Clone();
            }

        }

        private void btnToWorld_Click(object sender, EventArgs e)
        {
            ToWorldViewport();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            this.canvasHost1.Canvas.CurrentViewControl = new ZoomControlLayerWithBoxGDIPlus(GeoDo.RSS.Core.DrawEngine.ZoomControlLayerWithBox.enumZoomType.ZoomIn);
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            this.canvasHost1.Canvas.CurrentViewControl = new ZoomControlLayerWithBoxGDIPlus(GeoDo.RSS.Core.DrawEngine.ZoomControlLayerWithBox.enumZoomType.ZoomOut);
        }

        private void btnImgPan_Click(object sender, EventArgs e)
        {
            this.canvasHost1.Canvas.CurrentViewControl = new DefaultControlLayer();
        }

        #endregion

        #region 标记删除点

        public void TryRemoveShpLayer()
        {
            if (!string.IsNullOrWhiteSpace(_curShpLayerName))
            {
                IMap map = _shpFileHostLayer.Map as IMap;
                FeatureLayer fetLayer = map.LayerContainer.GetLayerByName(_curShpLayerName) as FeatureLayer;
                if (fetLayer != null)
                {
                    map.LayerContainer.Remove(fetLayer);
                }
                _shpFileHostLayer.ClearAll();
            }
            _maskLayer.Clear();
        }

        private void btnAOIDraw_Click(object sender, EventArgs e)
        {
            ActiveToolLayer();
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            _maskIsDelete = false;
            if (_maskIsDelete)
            {
                btnDelSelectPt.Image = imageList1.Images[0];
                btnUndo.Image = imageList1.Images[1];
            }
            else
            {
                btnDelSelectPt.Image = imageList1.Images[1];
                btnUndo.Image = imageList1.Images[0];
            }
            ActiveToolLayer();
        }

        private void btnDelSelectPt_Click(object sender, EventArgs e)
        {
            _maskIsDelete = true;
            if (_maskIsDelete)
            {
                btnDelSelectPt.Image = imageList1.Images[0];
                btnUndo.Image = imageList1.Images[1];
            }
            else
            {
                btnDelSelectPt.Image = imageList1.Images[1];
                btnUndo.Image = imageList1.Images[0];
            }
            ActiveToolLayer();
        }

        private void ActiveToolLayer()
        {
            if (_toolLayer == null)
                _toolLayer = CreateToolLayer();
            if (this.canvasHost1.Canvas.CurrentViewControl != _toolLayer)
                this.canvasHost1.Canvas.CurrentViewControl = _toolLayer;
        }

        #endregion

        #region 实现标记
        private PencilToolLayer CreateToolLayer()
        {
            PencilToolLayer toolLayer = new PencilToolLayer();
            toolLayer.PencilType = enumPencilType.Rectangle;
            toolLayer.PencilIsFinished = (geometry) =>
            {
                CoordEnvelope env = GeometryToCoordEnvelope(geometry);
                if (env == null)
                    return;
                Dictionary<int, CoordPoint> points = GetSelectedPoints(env);
                if (_maskIsDelete)
                    _maskLayer.Add(points);
                else
                    _maskLayer.Remove(points);
                _labeledPt = _maskLayer.MaskPoints.Count;
                canvasHost1.Canvas.Refresh(enumRefreshType.All);
                UpdateMaskText();
            };
            return toolLayer;
        }

        private Dictionary<int, CoordPoint> GetSelectedPoints(CoordEnvelope env)
        {
            Dictionary<int, CoordPoint> maskPoints = new Dictionary<int, CoordPoint>();
            ShapePoint pt = new ShapePoint();
            for (int i = 0; i < _allPtCount; i++)
            {
                pt = _features[i].Geometry as ShapePoint;
                if (pt.Y >= env.MinY && pt.Y <= env.MaxY && pt.X >= env.MinX && pt.X <= env.MaxX)
                {
                    CoordPoint ptf = new CoordPoint(pt.X, pt.Y);
                    maskPoints.Add(i, ptf);
                }
            }
            return maskPoints;
        }

        private CoordEnvelope GeometryToCoordEnvelope(GeometryOfDrawed geometry)
        {
            if (geometry.RasterPoints.Length == 0)
                return null;
            double geoX1, geoY1, geoX2, geoY2;
            if (geometry.IsPrjCoord)
            {
                canvasHost1.Canvas.CoordTransform.Prj2Geo(geometry.RasterPoints[0].X, geometry.RasterPoints[0].Y, out geoX1, out geoY1);//左上角
                canvasHost1.Canvas.CoordTransform.Prj2Geo(geometry.RasterPoints[2].X, geometry.RasterPoints[2].Y, out geoX2, out geoY2);//右下角
            }
            else
            {
                canvasHost1.Canvas.CoordTransform.Raster2Geo((int)geometry.RasterPoints[0].X, (int)geometry.RasterPoints[0].Y, out geoX1, out geoY1);
                canvasHost1.Canvas.CoordTransform.Raster2Geo((int)geometry.RasterPoints[2].X, (int)geometry.RasterPoints[2].Y, out geoX2, out geoY2);
            }
            return new CoordEnvelope(Math.Min(geoX1, geoX2), Math.Max(geoX1, geoX2), Math.Min(geoY2, geoY1), Math.Max(geoY2, geoY1));
        }

        private void UpdateMaskText()
        {
            txtPointCount.Text = "火点数：" + _allPtCount.ToString();
            txtDelPtCount.Text = "删除火点数：" + _maskLayer.MaskPoints.Count.ToString();
            txtReCount.Text = "剩余火点数：" + (_allPtCount - _maskLayer.MaskPoints.Count).ToString();
        }
        #endregion

        void UCGbalFirRevise_Disposed(object sender, EventArgs e)
        {
            if (_vectorMapHostLayer != null)
            {
                (_vectorMapHostLayer as VectorHostLayer).Dispose();
                _vectorMapHostLayer = null;
            }
            if (_geoGridLayer != null)
            {
                _geoGridLayer.Dispose();
                _geoGridLayer = null;
            }
            if (this.canvasHost1 != null)
            {
                this.canvasHost1.Canvas.Dispose();
                this.canvasHost1.Dispose();
            }
        }

        private void Start(int progressCount)
        {
            if (_smartSession.ProgressMonitorManager.DefaultProgressMonitor != null)
            {
                _smartSession.ProgressMonitorManager.DefaultProgressMonitor.Start(false);
                _smartSession.ProgressMonitorManager.DefaultProgressMonitor.Reset(null, progressCount);
            }
        }

        private void Progress(int progress, string text)
        {
            if (_smartSession.ProgressMonitorManager.DefaultProgressMonitor != null)
            {
                _smartSession.ProgressMonitorManager.DefaultProgressMonitor.Boost(progress, text);
            }
        }

        private void FinishProgress()
        {
            if (_smartSession.ProgressMonitorManager.DefaultProgressMonitor != null)
            {
                _smartSession.ProgressMonitorManager.DefaultProgressMonitor.Finish();
            }
        }

        public void Free()
        {
            if (_maskLayer != null)
            {
                _maskLayer.Dispose();
                _maskLayer = null;
            }
            if (_toolLayer != null)
            {
                _toolLayer.Dispose();
                _toolLayer = null;
            }
            if (_geoGridLayer != null)
            {
                _geoGridLayer.Dispose();
                _geoGridLayer = null;
            }
        }




    }
}
