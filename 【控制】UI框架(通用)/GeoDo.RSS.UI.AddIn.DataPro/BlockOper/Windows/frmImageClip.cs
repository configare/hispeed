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
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.BlockOper;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class frmImageClip : Form
    {
        private CoordEnvelope _fileEnvelope = null;
        private Size _fileSize = Size.Empty;
        private double _resolutionX = 0;
        private double _resolutionY = 0;
        private CoordEnvelope _outsizeRegion = null;
        private bool canUpateGeoRangeBySize = true;
        private Feature _vectorFeature;
        private Project.ISpatialReference _activeSpatialRef;
        private enumCoordType _activeCoordType;
        private string _blockName = "DXX";
        private List<string> _InputFiles = new List<string>();//输入文件列表
        private List<string> _CustomSelectFiles = new List<string>();//输出文件列表

        public List<string> CustomSelectFiles
        {
            get { return _CustomSelectFiles; }
            set { _CustomSelectFiles = value; }
        }
        public List<string> InputFiles
        {
            get { return _InputFiles; }
            set { _InputFiles = value; }
        }

        public frmImageClip(ISmartSession session)
        {
            InitializeComponent();
            txtInputFile.DoubleClick += new EventHandler(txtInputFile_DoubleClick);
            txtOutFile.DoubleClick += new EventHandler(txtOutFile_DoubleClick);
            ucGeoRangeControl1.OnValueChanged += new UCGeoRangeControl.ValueChangedHandler(ucGeoRangeControl1_OnValueChanged);
            SetSession(session);
        }

        private void SetSession(ISmartSession smartSession)
        {
            if (smartSession == null)
                return;
            ICanvasViewer canViewer = smartSession.SmartWindowManager.ActiveCanvasViewer;
            if (canViewer == null)
                return;
            IRasterDrawing drawing = canViewer.Canvas.PrimaryDrawObject as IRasterDrawing;
            IRasterDataProvider activeRaster = drawing.DataProvider;
            if (activeRaster == null)
                return;
            _resolutionX = activeRaster.ResolutionX;
            _resolutionY = activeRaster.ResolutionY;
            _fileSize = new Size(activeRaster.Width, activeRaster.Height);
            _fileEnvelope = activeRaster.CoordEnvelope;
            _activeSpatialRef = activeRaster.SpatialRef;
            _activeCoordType = activeRaster.CoordType;
            IAOIProvider aoiProvider = canViewer.AOIProvider;
            if (aoiProvider == null)
                return;
            AOIItem[] aoiItems = aoiProvider.GetAOIItems();
            if (aoiItems != null && aoiItems.Length == 1)
            {
                txtRegionName.Text = string.IsNullOrEmpty(aoiItems[0].Name) ? "DXX" : aoiItems[0].Name;
                txtMask.Text = _blockName;
            }
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope corEnvelope = null;
            switch (_activeCoordType)
            {
                case enumCoordType.PrjCoord:
                    {
                        corEnvelope = aoiProvider.GetPrjRect();
                        break;
                    }
                case enumCoordType.GeoCoord:
                    {
                        corEnvelope = aoiProvider.GetGeoRect();
                        break;
                    }
            }
            if (corEnvelope == null)
                _outsizeRegion = new CoordEnvelope(activeRaster.CoordEnvelope.MinX, activeRaster.CoordEnvelope.MaxX, activeRaster.CoordEnvelope.MinY, activeRaster.CoordEnvelope.MaxY);
            else
                _outsizeRegion = new CoordEnvelope(corEnvelope.MinX, corEnvelope.MaxX, corEnvelope.MinY, corEnvelope.MaxY);
            FillControlsValuesByFileArgs();
            if (_outsizeRegion != null)
                GeoRegionChanged(_outsizeRegion);
            if (drawing != null)
            {
                IRasterDataProvider rdp = drawing.DataProvider;
                SetInputFilename(rdp.fileName);
            }
        }

        public void GetArgs(out BlockDef envelope, out Size size, out string dir, out string inputFilename)
        {
            envelope = new BlockDef(_blockName, ucGeoRangeControl1.MinX, ucGeoRangeControl1.MinY, ucGeoRangeControl1.MaxX, ucGeoRangeControl1.MaxY);
            size = new Size((int)txtWidth.Value, (int)txtHeight.Value);
            dir = txtOutFile.Text;
            inputFilename = txtInputFile.Text;
        }

        public void GetArgs(out BlockDefWithAOI envelope, out Size size, out string dir, out string inputFilename)
        {
            envelope = new BlockDefWithAOI(_blockName, ucGeoRangeControl1.MinX, ucGeoRangeControl1.MinY, ucGeoRangeControl1.MaxX, ucGeoRangeControl1.MaxY);
            size = new Size((int)txtWidth.Value, (int)txtHeight.Value);
            dir = txtOutFile.Text;
            inputFilename = txtInputFile.Text;
        }

        void ucGeoRangeControl1_OnValueChanged(object sender)
        {
            UpdateSizeByGeoRange();
        }

        void txtOutFile_DoubleClick(object sender, EventArgs e)
        {
            btnSaveFile_Click(null, null);
        }

        void txtInputFile_DoubleClick(object sender, EventArgs e)
        {
            btnInputFile_Click(null, null);
        }

        private void btnInputFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "LDF files(*.ldf)|*.ldf";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtInputFile.Text = dlg.FileName;
                    string extname = Path.GetExtension(dlg.FileName).ToUpper();
                    if (!string.IsNullOrEmpty(txtInputFile.Text))
                    {
                        using (IRasterDataProvider raster = GeoDataDriver.Open(dlg.FileName) as IRasterDataProvider)
                        {
                            _resolutionX = raster.ResolutionX;
                            _resolutionY = raster.ResolutionY;
                            _fileSize = new Size(raster.Width, raster.Height);
                            _fileEnvelope = raster.CoordEnvelope;
                            _activeSpatialRef = raster.SpatialRef;
                            _activeCoordType = raster.CoordType;
                        }
                        _outsizeRegion = new CoordEnvelope(_fileEnvelope.MinX, _fileEnvelope.MaxX, _fileEnvelope.MinY, _fileEnvelope.MaxY);
                        FillControlsValuesByFileArgs();
                        SetInputFilename(txtInputFile.Text);
                    }
                }
            }
        }

        private void FillControlsValuesByFileArgs()
        {
            if (_fileEnvelope != null)
            {
                ucGeoRangeControl1.MinX = _fileEnvelope.MinX;
                ucGeoRangeControl1.MaxX = _fileEnvelope.MaxX;
                ucGeoRangeControl1.MinY = _fileEnvelope.MinY;
                ucGeoRangeControl1.MaxY = _fileEnvelope.MaxY;
            }
            if (!_fileSize.IsEmpty)
            {
                txtWidth.Maximum = _fileSize.Width;
                txtHeight.Maximum = _fileSize.Height;
                txtWidth.Value = _fileSize.Width;
                txtHeight.Value = _fileSize.Height;
            }
        }

        private void btnSaveFile_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (string.IsNullOrWhiteSpace(txtInputFile.Text))
                    dlg.SelectedPath = txtInputFile.Text;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    SetOutDir(dlg.SelectedPath);
                }
            }
        }

        private void SetInputFilename(string filename)
        {
            txtInputFile.Text = filename;
            SetOutDir(Path.GetDirectoryName(filename));
        }

        private void SetOutDir(string dir)
        {
            txtOutFile.Text = dir;
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool IsOKArgs()
        {
            if (txtInputFile.Text.Trim() == string.Empty || !File.Exists(txtInputFile.Text))
            {
                MsgBox.ShowInfo("输入文件名输入错误或者文件不存在。");
                return false;
            }
            double tol = 0.000001d;
            if ((ucGeoRangeControl1.MinX > _fileEnvelope.MaxX) ||
               (ucGeoRangeControl1.MaxX < _fileEnvelope.MinX) ||
               (ucGeoRangeControl1.MinY > _fileEnvelope.MaxY) ||
               (ucGeoRangeControl1.MaxY < _fileEnvelope.MinY))
            {
                MsgBox.ShowInfo("输出文件的地理范围超过源文件的地理范围,请重新输入。");
                return false;
            }
            if ((_fileEnvelope.MinX - ucGeoRangeControl1.MinX) > tol ||
            (ucGeoRangeControl1.MaxX - _fileEnvelope.MaxX > tol) ||
            (_fileEnvelope.MinY - ucGeoRangeControl1.MinY > tol) ||
            (ucGeoRangeControl1.MaxY - _fileEnvelope.MaxY > tol))
            {
                DialogResult yes = MsgBox.ShowQuestionYesNo("输出文件的地理范围超过源文件的地理范围,要自动调整吗?");
                if (yes == DialogResult.Yes)
                {
                    ucGeoRangeControl1.MinX = Math.Max(ucGeoRangeControl1.MinX, _fileEnvelope.MinX);
                    ucGeoRangeControl1.MaxX = Math.Min(ucGeoRangeControl1.MaxX, _fileEnvelope.MaxX);
                    ucGeoRangeControl1.MinY = Math.Max(ucGeoRangeControl1.MinY, _fileEnvelope.MinY);
                    ucGeoRangeControl1.MaxY = Math.Min(ucGeoRangeControl1.MaxY, _fileEnvelope.MaxY);
                    UpdateSizeByGeoRange();
                }
                //return false;   //现在的裁切允许裁切范围超过图像范围
            }
            if (txtWidth.Value == 0 || txtHeight.Value == 0)
            {
                MsgBox.ShowInfo("输出文件高度和宽度输入错误。");
                return false;
            }
            if (txtOutFile.Text.Trim() == string.Empty)
            {
                MsgBox.ShowInfo("输出文件名输入错误。");
                return false;
            }
            else
            {
                try
                {
                    string tempDir = Path.GetFullPath(txtOutFile.Text);
                    if (!Directory.Exists(tempDir))
                    {
                        Directory.CreateDirectory(tempDir);
                    }
                    //string f = Path.GetFileName(txtOutFile.Text);
                    if (string.IsNullOrEmpty(tempDir))// || string.IsNullOrEmpty(f))
                    {
                        MsgBox.ShowInfo("输出路径输入错误。");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MsgBox.ShowInfo("输出文件名输入错误(" + ex.Message + ")。");
                    return false;
                }
            }
            if (File.Exists(txtOutFile.Text))
            {
                try
                {
                    string hdrfile = Path.Combine(Path.GetDirectoryName(txtOutFile.Text), Path.GetFileNameWithoutExtension(txtOutFile.Text)
                        + ".hdr");
                    if (File.Exists(hdrfile))
                    {
                        File.Delete(hdrfile);
                    }
                }
                catch { }
                if (txtOutFile.Text.ToUpper() == txtInputFile.Text.ToUpper())
                {
                    MsgBox.ShowInfo("输入文件与输出文件一样,请重新输入输出文件名。");
                    return false;
                }
                DialogResult isOK = MsgBox.ShowQuestionYesNo("文件\"" + txtOutFile.Text + "\"已经存在,要覆盖吗?");
                if (isOK == DialogResult.Yes)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void UpdateSizeByGeoRange()
        {
            if (_resolutionX - 0 < 0.0000001f || _resolutionY < 0.0000001f)
                return;
            canUpateGeoRangeBySize = false;
            try
            {
                Size aoiSize = new Size((int)GetInteger(((ucGeoRangeControl1.MaxX - ucGeoRangeControl1.MinX) / _resolutionX), _resolutionX),
                    (int)GetInteger(((ucGeoRangeControl1.MaxY - ucGeoRangeControl1.MinY) / _resolutionY), _resolutionY));

                txtWidth.Maximum = Math.Max(aoiSize.Width, txtWidth.Width);
                txtHeight.Maximum = Math.Max(aoiSize.Height, txtHeight.Height);
                txtWidth.Value = aoiSize.Width;
                txtHeight.Value = aoiSize.Height;
            }
            finally
            {
                canUpateGeoRangeBySize = true;
            }
        }

        protected int GetInteger(double fWidth, double res)
        {
            int v = (int)Math.Round(fWidth);
            if (fWidth - v > res)
                v++;
            return v;
        }

        private byte[] GetMaskArray()
        {
            return null;
        }

        public int[] GetFeatureAOIIndex()
        {
            if (_vectorFeature == null || _outsizeRegion == null)
                return null;
            VectorAOIGenerator vg = new VectorAOIGenerator();
            int[] aoi = null;
            aoi = vg.GetAOI(new ShapePolygon[] { _vectorFeature.Geometry as ShapePolygon },
                            new Envelope(_fileEnvelope.MinX, _fileEnvelope.MinY, _fileEnvelope.MaxX, _fileEnvelope.MaxY),
                            _fileSize);
            return aoi;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!IsOKArgs())
                return;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnAddMask_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInputFile.Text))
                return;
            using (frmStatSubRegionTemplates frmTemplates = new frmStatSubRegionTemplates())
            {
                if (frmTemplates.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Feature[] features = null;
                    string fieldName;
                    string shapeFilename;
                    int fieldIndex = -1;
                    features = frmTemplates.GetStatFeatures(out fieldName, out shapeFilename, out fieldIndex);
                    if (features == null)
                    {
                        MsgBox.ShowInfo("未选择任何自定义统计区域!");
                        return;
                    }
                    _vectorFeature = features[0];
                    string fieldValue = _vectorFeature.GetFieldValue(fieldIndex);
                    txtMask.Text = fieldName + ":" + fieldValue;
                    DialogResult isYes = MsgBox.ShowQuestionYesNo("是否将输出文件的地理范围更新为选中模版的地理范围?");
                    if (isYes == DialogResult.Yes)
                    {
                        Envelope evp = GetMaskEnvelope(_vectorFeature.Geometry as ShapePolygon);
                        if (evp != null)
                        {
                            ucGeoRangeControl1.MinX = evp.MinX;
                            ucGeoRangeControl1.MinY = evp.MinY;
                            ucGeoRangeControl1.MaxX = evp.MaxX;
                            ucGeoRangeControl1.MaxY = evp.MaxY;
                            UpdateSizeByGeoRange();
                            txtRegionName.Text = fieldValue;
                        }
                    }
                }
            }
        }

        private Envelope GetMaskEnvelope(ShapePolygon shapePolygon)
        {
            if (shapePolygon == null)
                return null;
            Envelope envelope = shapePolygon.Envelope;
            if (_activeCoordType == enumCoordType.GeoCoord)
                return envelope;
            else if (_activeCoordType == enumCoordType.PrjCoord)
            {
                GeoDo.Project.ISpatialReference srcProj = GeoDo.Project.SpatialReference.GetDefault();
                GeoDo.Project.ISpatialReference dstProj = _activeSpatialRef;
                GeoDo.Project.IProjectionTransform prj = GeoDo.Project.ProjectionTransformFactory.GetProjectionTransform(srcProj, dstProj);
                double[] xs = new double[] { shapePolygon.Rings[0].Points[0].X };
                double[] ys = new double[] { shapePolygon.Rings[0].Points[0].Y };
                prj.Transform(xs, ys);
                double minx = xs[0];
                double maxx = xs[0];
                double miny = ys[0];
                double maxy = ys[0];
                ShapePoint pt;
                foreach (ShapeRing ring in shapePolygon.Rings)
                {
                    for (int pti = 0; pti < ring.Points.Length; pti++)
                    {
                        pt = ring.Points[pti];
                        xs = new double[] { pt.X };
                        ys = new double[] { pt.Y };
                        prj.Transform(xs, ys);
                        minx = Math.Min(xs[0], minx);
                        maxx = Math.Max(xs[0], maxx);
                        miny = Math.Min(ys[0], miny);
                        maxy = Math.Max(ys[0], maxy);
                    }
                }
                Envelope corEnvelope = new Envelope(minx, miny, maxx, maxy);
                return corEnvelope;
            }
            else
                return null;
        }

        private void UpateGeoCoordBySize()
        {
            if (canUpateGeoRangeBySize)
            {
                ucGeoRangeControl1.MaxX = ucGeoRangeControl1.MinX + (int)txtWidth.Value * _resolutionX;
                ucGeoRangeControl1.MinY = ucGeoRangeControl1.MaxY - (int)txtHeight.Value * _resolutionY;
            }
        }

        private void txtWidth_ValueChanged(object sender, EventArgs e)
        {
            UpateGeoCoordBySize();
        }

        private void txtHeight_ValueChanged(object sender, EventArgs e)
        {
            UpateGeoCoordBySize();
        }

        private void txtMask_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                _vectorFeature = null;
                txtMask.Text = string.Empty;
            }
        }

        private void GeoRegionChanged(CoordEnvelope region)
        {
            ucGeoRangeControl1.MinX = region.MinX;
            ucGeoRangeControl1.MinY = region.MinY;
            ucGeoRangeControl1.MaxX = region.MaxX;
            ucGeoRangeControl1.MaxY = region.MaxY;
            UpdateSizeByGeoRange();
        }

        private void txtRegionName_TextChanged(object sender, EventArgs e)
        {
            _blockName = string.IsNullOrEmpty(txtRegionName.Text) ? "DXX" : txtRegionName.Text;
        }

        private void ckbangle_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbangle.Checked)
            {
                string inputraster = this.txtInputFile.Text;
                if (!string.IsNullOrEmpty(inputraster))
                {
                    string asunz = "SolarZenith";//太阳天顶角
                    string asuna = "SolarAzimuth";// 太阳方位角
                    string asatz = "SensorZenith";//卫星天顶角 
                    string asata = "SensorAzimuth";//卫星方位角
                    string file_name = inputraster.Insert(inputraster.LastIndexOf('.') + 1, "#.");
                    this.InputFiles.Add(File.Exists(file_name.Replace("#", asunz)) ? file_name.Replace("#", asunz) : string.Empty);
                    this.InputFiles.Add(File.Exists(file_name.Replace("#", asuna)) ? file_name.Replace("#", asuna) : string.Empty);
                    this.InputFiles.Add(File.Exists(file_name.Replace("#", asatz)) ? file_name.Replace("#", asatz) : string.Empty);
                    this.InputFiles.Add(File.Exists(file_name.Replace("#", asata)) ? file_name.Replace("#", asata) : string.Empty);
                }
            }
        }

        private void ckbothers_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ckbothers.Checked)
            {
                this.btnohters.Enabled = true;
            }
            else
            {
                this.btnohters.Enabled = false;
                this.txtOutFile.Enabled = false;

            }
        }

        private void btnohters_Click(object sender, EventArgs e)
        {
            this.CustomSelectFiles.Clear();
            using (OpenFileDialog op = new OpenFileDialog())
            {
                op.Multiselect = true;
                op.Filter = "LDF files(*.ldf)|*.ldf";
                if (op.ShowDialog() == DialogResult.OK)
                {
                    this.CustomSelectFiles.AddRange(op.FileNames);
                }
            }


        }
    }
}
