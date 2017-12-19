using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Imaging;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using GeoDo.HDF4;
using GeoDo.HDF;
using GeoDo.RasterProject;
using HDF5DotNet;
using GeoDo.Project;

namespace GeoDo.Smart.Tools.MODLSTPro
{
    public partial class frmMODLSTPro : Form
    {
        private string _extersion = "HDF";
        private Regex _regex = null;
        private string[] _projectNames = new string[] { "Projection" };
        private string[] _projectArgsNames = new string[] { "ProjParams" };
        private string[] _leftupPointNames = new string[] { "UpperLeftPointMtrs" };
        private string[] _rightDownPointNames = new string[] { "LowerRightMtrs" };
        private string[] _bandNames = new string[] { "band_names" };
        private string[] _validRegionNames = new string[] { "valid_range" };
        private string[] _sacleNames = new string[] { "scale_factor" };
        private string[] _offsetNames = new string[] { "add_offset" };
        private MosaicInfos _currMosaicInfos = null;
        private Dictionary<string, H5T.H5Type> _currMosaicDatasets = null;
        //private PrjEnvelope _chinaEnv = new PrjEnvelope(5559752.5988, 12231455.6750, 1111950.5492, 5559752.6000);
        private PrjEnvelope _chinaEnv = new PrjEnvelope(72, 136, 15, 55);

        public frmMODLSTPro()
        {
            InitializeComponent();
            Load += new EventHandler(frmMosaicData_Load);
        }

        void frmMosaicData_Load(object sender, EventArgs e)
        {
            InitDataPicker();
        }

        private void InitDataPicker()
        {
            lstFileInfoList.Items.Clear();
            SetEnableDp(false);
            dpBegin.Value = DateTime.Now.AddMonths(-1);
            dpEnd.Value = DateTime.Now;
        }

        private void SetEnableDp(bool isEnable)
        {
            dpBegin.Enabled = isEnable;
            dpEnd.Enabled = isEnable;
        }

        private void ckTimeRegion_CheckedChanged(object sender, EventArgs e)
        {
            SetEnableDp(ckTimeRegion.Checked);
        }

        private void btChooseDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog pathDialog = new FolderBrowserDialog();
            if (pathDialog.ShowDialog() == DialogResult.OK)
            {
                lstFileInfoList.Items.Clear();
                txtFileDir.Text = pathDialog.SelectedPath;
                LoadFiles(txtFileDir.Text, _extersion, ckTimeRegion.Checked, dpBegin.Value, dpEnd.Value);
                txtOutDir.Text = txtFileDir.Text + "\\Output";
            }
            lbFileCount.Text = lstFileInfoList.Items.Count.ToString();
        }

        private void LoadFiles(string fileDir, string extension, bool checkTime, DateTime beginTime, DateTime endTime)
        {
            if (string.IsNullOrEmpty(fileDir) || string.IsNullOrEmpty(extension))
                return;
            string[] extensions = extension.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> files = new List<string>();
            foreach (string item in extensions)
                files.AddRange(Directory.GetFiles(fileDir, "*." + item, SearchOption.AllDirectories));
            UpdateFileItemsAddToList(files.Count == 0 ? null : files.ToArray(), checkTime, beginTime, endTime);
        }

        private void UpdateFileItemsAddToList(string[] files, bool checkTime, DateTime beginTime, DateTime endTime)
        {
            lstFileInfoList.Items.Clear();
            RasterIdentify rid = null;
            FileListItem fli = null;
            if (checkTime)
                foreach (string file in files)
                {
                    DataIdentify di = DataIdentifyMatcher.Match(Path.GetFileName(file));
                    rid = new RasterIdentify();
                    rid.OriFileName = new string[] { file };
                    rid.Satellite = di.Satellite;
                    rid.Sensor = di.Sensor;
                    rid.OrbitDateTime = di.OrbitDateTime;
                    FillRid(ref rid);
                    if (rid.OrbitDateTime == DateTime.MinValue || rid.OrbitDateTime > Convert.ToDateTime(endTime.ToString("MM-dd-yyyy") + " 23:59:59")
                        || rid.OrbitDateTime < Convert.ToDateTime(beginTime.ToString("MM-dd-yyyy") + " 00:00:00"))
                        continue;
                    fli = new FileListItem(file, rid);
                    AddToFileListItem(fli);
                }
            else
                foreach (string file in files)
                {
                    DataIdentify di = DataIdentifyMatcher.Match(Path.GetFileName(file));
                    rid = new RasterIdentify();
                    rid.OriFileName = new string[] { file };
                    rid.Satellite = di.Satellite;
                    rid.Sensor = di.Sensor;
                    rid.OrbitDateTime = di.OrbitDateTime;
                    FillRid(ref rid);
                    fli = new FileListItem(file, rid);
                    AddToFileListItem(fli);
                }
            if (lstFileInfoList.Items.Count != 0)
                lstFileInfoList.Items[0].Selected = true;
        }

        private void FillRid(ref RasterIdentify rid)
        {
            if (rid.OriFileName[0].Contains("MOD11"))
            {
                rid.ProductIdentify = "LST";
                rid.SubProductIdentify = "DBLV";
                if (ckGCTPChina.Checked)
                    rid.SetRegionIdentify("CHINA");
                else
                    rid.SetRegionIdentify("DXX");
            }
        }

        private void AddToFileListItem(FileListItem fli)
        {
            ListViewItem lvi = new ListViewItem(Path.GetFileName(fli.FileName));
            lvi.SubItems.Add(fli.Rid.OrbitDateTime.ToString("yyyy-MM-dd"));
            lvi.Tag = fli;
            lstFileInfoList.Items.Add(lvi);
        }

        private void txtFileDir_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(txtFileDir.Text))
                LoadFiles(txtFileDir.Text, _extersion, ckTimeRegion.Checked, dpBegin.Value, dpEnd.Value);
        }

        private void lstFileInfoList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstFileInfoList.SelectedItems.Count == 0)
                return;
            FileListItem fli = lstFileInfoList.SelectedItems[0].Tag as FileListItem;
            if (Path.GetExtension(fli.FileName).ToUpper() == ".HDF")
            {
                SetAttrInfosEnable(true);
                GetFileAttrInfos(fli);
            }
            else
            {
                SetAttrInfosEnable(false);
            }
        }

        private void GetFileAttrInfos(FileListItem fli)
        {
            if (HDF4Helper.IsHdf4(fli.FileName))
            {
                Hdf4Operator oper = new Hdf4Operator(fli.FileName);
                try
                {
                    Dictionary<string, string> fileAtrr = null;
                    if (oper != null)
                    {
                        fileAtrr = oper.GetAttributes();
                        if (fileAtrr != null)
                            InitFileAttr(fileAtrr);
                        else
                            ClearFileAtrr();
                        InitDataset(oper.GetDatasetNames, oper);
                    }
                }
                catch (Exception ex)
                {
                    AddLog(fli.FileName + "文件读取失败:" + ex.Message);
                }
                finally
                {
                    if (oper != null)
                        oper.Dispose();
                }
            }
        }

        private void InitDataset(string[] dataset, Hdf4Operator oper)
        {
            lstDataSets.Items.Clear();
            if (dataset == null || dataset.Length == 0)
                return;
            ListViewItem lvi = null;
            Dictionary<string, string> attValues = null;
            foreach (string item in dataset)
            {
                lvi = new ListViewItem(item);
                attValues = oper.GetAttributes(item);
                lvi.SubItems.Add(attValues != null && attValues.ContainsKey("long_name") ? attValues["long_name"] : "");
                lvi.Tag = new DataSetMosaicInfo(item);
                lvi.Checked = true;
                lstDataSets.Items.Add(lvi);
            }
        }

        private void InitFileAttr(Dictionary<string, string> fileAtrr)
        {
            gbFileAttr.Enabled = true;
            string[] fileAttrs = GetAttrStr(fileAtrr);
            if (fileAttrs == null)
                return;
            gbFileAttr.Tag = fileAtrr;
            ClearCombox();
            cbProject.Items.AddRange(fileAttrs);
            SetAttr(fileAtrr, _projectNames, cbProject, lbProject);
            cbProjectAgrs.Items.AddRange(fileAttrs);
            SetAttr(fileAtrr, _projectArgsNames, cbProjectAgrs, lbProjectAgrs);
            cbLeftUpPoint.Items.AddRange(fileAttrs);
            SetAttr(fileAtrr, _leftupPointNames, cbLeftUpPoint, lbLeftUpPoint);
            cbRightDownPoint.Items.AddRange(fileAttrs);
            SetAttr(fileAtrr, _rightDownPointNames, cbRightDownPoint, lbRightDownPoint);
        }

        private void ClearFileAtrr()
        {
            ClearCombox();
            cbProject.Text = "";
            lbProject.Text = "";
            cbProjectAgrs.Text = "";
            lbProjectAgrs.Text = "";
            cbLeftUpPoint.Text = "";
            lbLeftUpPoint.Text = "";
            cbRightDownPoint.Text = "";
            lbRightDownPoint.Text = "";
        }

        private void ClearCombox()
        {
            cbProject.Items.Clear();
            cbProject.Tag = lbProject;
            cbProjectAgrs.Items.Clear();
            cbProjectAgrs.Tag = lbProjectAgrs;
            cbLeftUpPoint.Items.Clear();
            cbLeftUpPoint.Tag = lbLeftUpPoint;
            cbRightDownPoint.Items.Clear();
            cbRightDownPoint.Tag = lbRightDownPoint;
        }

        private string[] GetAttrStr(Dictionary<string, string> atrrs)
        {
            List<string> result = new List<string>();
            foreach (string item in atrrs.Keys)
                result.Add(item);
            return result.ToArray();
        }

        private void SetAttr(Dictionary<string, string> attrDic, string[] attrNames, ComboBox cb, Label lb)
        {
            if (attrNames == null || attrNames.Length == 0)
                return;
            foreach (string attr in attrNames)
            {
                if (attrDic.ContainsKey(attr))
                {
                    cb.Text = attr;
                    lb.Text = attrDic[attr];
                    return;
                }
            }
            foreach (string attr in attrDic.Keys)
            {
                foreach (string attrName in attrNames)
                {
                    if (attrDic[attr].IndexOf(attrName) != -1)
                    {
                        _regex = new Regex(@"\S*" + attrName + @"=(?<attrValue>\S+)\S*");
                        if (_regex.IsMatch(attrDic[attr]))
                        {
                            cb.Text = attr;
                            lb.Text = _regex.Match(attrDic[attr]).Groups["attrValue"].ToString();
                            return;
                        }
                    }
                }
            }

        }

        private void SetAttr(Dictionary<string, string> attrDic, string[] attrNames, ref string arg)
        {
            if (attrNames == null || attrNames.Length == 0)
                return;
            arg = null;
            foreach (string attr in attrNames)
            {
                if (attrDic.ContainsKey(attr))
                {
                    arg = attrDic[attr];
                    return;
                }
            }
            foreach (string attr in attrDic.Keys)
            {
                foreach (string attrName in attrNames)
                {
                    if (attrDic[attr].IndexOf(attrName) != -1)
                    {
                        _regex = new Regex(@"\S*" + attrName + @"=(?<attrValue>\S+)\S*");
                        if (_regex.IsMatch(attrDic[attr]))
                        {
                            arg = _regex.Match(attrDic[attr]).Groups["attrValue"].ToString();
                            return;
                        }
                    }
                }
            }

        }

        private void SetAttrInfosEnable(bool isEnable)
        {
            gbFileAttr.Enabled = isEnable;
            gbDataSets.Enabled = isEnable;
            gbDataSetAttr.Enabled = isEnable;
        }

        private void btAddFiles_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.RestoreDirectory = true;
            open.Multiselect = true;
            string[] split = _extersion.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string extersionStr = null; ;
            foreach (string item in split)
                extersionStr = "*." + item + ";";
            open.Filter = "文件(" + extersionStr + ")|" + extersionStr;
            if (open.ShowDialog() == DialogResult.OK)
            {
                UpdateFileItemsAddToList(open.FileNames, ckTimeRegion.Checked, dpBegin.Value, dpEnd.Value);
            }
            lbFileCount.Text = lstFileInfoList.Items.Count.ToString();
        }

        private void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.Tag == null || (cb.Tag as Label) == null)
                return;
            GroupBox gb = cb.Parent as GroupBox;
            if (gb == null || gb.Tag == null)
                return;
            Dictionary<string, string> attrs = gb.Tag as Dictionary<string, string>;
            if (attrs == null || attrs.Count == 0 || !attrs.ContainsKey(cb.Text))
                return;
            (cb.Tag as Label).Text = attrs[cb.Text];
        }

        private void lstDataSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearDataSetCombox();
            if (lstDataSets.SelectedItems.Count == 0 || lstDataSets.SelectedItems[0].Tag == null ||
                lstFileInfoList.SelectedItems.Count == 0 || (lstFileInfoList.SelectedItems[0]).Tag == null)
                return;
            FileListItem fli = (lstFileInfoList.SelectedItems[0]).Tag as FileListItem;
            DataSetMosaicInfo dsi = lstDataSets.SelectedItems[0].Tag as DataSetMosaicInfo;
            if (fli == null)
                return;
            if (HDF4Helper.IsHdf4(fli.FileName))
            {
                Hdf4Operator oper = new Hdf4Operator(fli.FileName);
                try
                {
                    Dictionary<string, string> datasetAtrr = new Dictionary<string, string>();
                    if (oper != null)
                    {
                        datasetAtrr = oper.GetAttributes(dsi.DataSetName);
                        if (datasetAtrr != null)
                            InitDataSetAttr(datasetAtrr, ref dsi);
                        Size size = Size.Empty;
                        int bandCount = 0;
                        Type datatype;
                        int datatypeSize = 0;
                        HDF4Helper.DataTypeDefinitions hdf4DataType = HDF4Helper.DataTypeDefinitions.DFNT_NUINT16;
                        if (GetDataSetSize(dsi.DataSetName, oper, out size, out bandCount, out hdf4DataType, out datatype, out datatypeSize))
                        {
                            InitHeigthWidth(size.Width, size.Height, bandCount, ref dsi);
                            if (dsi != null)
                                dsi.HDF4DataType = hdf4DataType;
                        }
                    }
                }
                finally
                {
                    if (oper != null)
                        oper.Dispose();
                }
            }
        }

        private bool GetDataSetSize(string datasetName, Hdf4Operator oper, out Size size, out int bandCount, out HDF4Helper.DataTypeDefinitions hdf4DataType, out Type datatype, out int datatypeSize)
        {
            StringBuilder sds_name = new StringBuilder();
            int rank = 0;
            int[] dimsizes = null;
            datatype = typeof(UInt16);
            datatypeSize = 0;
            hdf4DataType = HDF4Helper.DataTypeDefinitions.DFNT_NUINT16;
            oper.GetDataSizeInfos(datasetName, out rank, out dimsizes, out hdf4DataType, out datatype, out datatypeSize);
            size = new Size(0, 0);
            bandCount = 1;
            switch (rank)
            {
                case 1:
                    size = new Size(dimsizes[0], -1);
                    bandCount = 1;
                    break;
                case 2:
                    size = new Size(dimsizes[1], dimsizes[0]);
                    bandCount = 1;
                    break;
                case 3:
                    if (dimsizes[0] == dimsizes.Min())
                        size = new Size(dimsizes[2], dimsizes[1]);
                    else if (dimsizes[1] == dimsizes.Min())
                        size = new Size(dimsizes[2], dimsizes[0]);
                    else if (dimsizes[2] == dimsizes.Min())
                        size = new Size(dimsizes[1], dimsizes[0]);
                    bandCount = dimsizes.Min();
                    break;
                default:
                    return false;
            }
            return true;
        }

        private void InitHeigthWidth(int width, int height, int bandcount, ref DataSetMosaicInfo dsi)
        {
            txtHeight.Text = height == -1 ? "" : height.ToString();
            txtWidth.Text = width == -1 ? "" : width.ToString();
            dsi.Height = height;
            dsi.Width = width;
            dsi.BandCount = bandcount;
        }

        private void InitDataSetAttr(Dictionary<string, string> datasetAtrr, ref DataSetMosaicInfo dsi)
        {
            gbDataSetAttr.Enabled = true;
            string[] dataSetAttrs = GetAttrStr(datasetAtrr);
            if (dataSetAttrs == null)
                return;
            gbFileAttr.Tag = datasetAtrr;
            ClearDataSetCombox();
            cbBandName.Items.AddRange(dataSetAttrs);
            SetAttr(datasetAtrr, _bandNames, cbBandName, lbBandName);
            if (!string.IsNullOrEmpty(lbBandName.Text))
                dsi.BandNames = lbBandName.Text;
            cbValidRegion.Items.AddRange(dataSetAttrs);
            SetAttr(datasetAtrr, _validRegionNames, cbValidRegion, lbValidRegion);
            if (!string.IsNullOrEmpty(lbValidRegion.Text))
            {
                string[] splitMinMax = lbValidRegion.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                dsi.MinValue = float.Parse(splitMinMax[0]);
                dsi.MaxValue = float.Parse(splitMinMax[1]);
            }
            cbScaleValue.Items.AddRange(dataSetAttrs);
            SetAttr(datasetAtrr, _sacleNames, cbScaleValue, lbScaleValue);
            if (!string.IsNullOrEmpty(lbScaleValue.Text))
                dsi.Scale = float.Parse(lbScaleValue.Text);
            cbOffsetValue.Items.AddRange(dataSetAttrs);
            SetAttr(datasetAtrr, _offsetNames, cbOffsetValue, lbOffsetValue);
            if (!string.IsNullOrEmpty(lbOffsetValue.Text))
                dsi.Offset = float.Parse(lbOffsetValue.Text);
        }

        private void ClearDataSetCombox()
        {
            cbBandName.Items.Clear();
            cbBandName.Tag = lbBandName;
            cbValidRegion.Items.Clear();
            cbValidRegion.Tag = lbValidRegion;
            cbScaleValue.Items.Clear();
            cbScaleValue.Tag = lbScaleValue;
            cbOffsetValue.Items.Clear();
            cbOffsetValue.Tag = lbOffsetValue;
            txtHeight.Text = "";
            txtWidth.Text = "";
            lbBandName.Text = "";
            lbValidRegion.Text = "";
            lbScaleValue.Text = "";
            lbOffsetValue.Text = "";
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnChooseOutDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
                txtOutDir.Text = dialog.SelectedPath;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (!CheckEnv())
                return;
            try
            {
                FileListItem[] fli = GetFiles();
                PrjEnvelope dstEnvelope = GetDstEnvelope();
                float outResolution = GetDstResultion();
                if (fli == null)
                    return;
                int length = fli.Length;
                Dictionary<string, PrjEnvelope> MosaicFiles = null;
                List<string> gllFiles = null;
                if (ckGCTPChina.Checked)
                {
                    gllFiles = new List<string>();
                    MosaicFiles = MosaicRegion(fli, length, gllFiles);
                }

                MessageBox.Show("完成【" + (MosaicFiles == null ? 0 : MosaicFiles.Count) + "】个文件\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (_logList != null && _logList.Count != 0)
                {
                    string logFile = AppDomain.CurrentDomain.BaseDirectory + @"\Log\" + DateTime.Now.ToString("yyyyMMdd HHmmss") + @".log";
                    if (!Directory.Exists(Path.GetDirectoryName(logFile)))
                        Directory.CreateDirectory(logFile);
                    File.WriteAllLines(logFile, _logList.ToArray(), Encoding.Default);
                }
            }
        }

        private float GetDstResultion()
        {
            return 0.01f;
        }

        private PrjEnvelope GetDstEnvelope()
        {
            return _chinaEnv;
        }

        private string[] ProjectGLL(Dictionary<string, PrjEnvelope> srcFiles, PrjEnvelope dstEnvelope, float outResolution)
        {
            List<string> gllFiles = new List<string>();
            string dstFile;
            if (ckGLL.Checked)
            {
                foreach (string srcFile in srcFiles.Keys)
                {
                    dstFile = Path.Combine(Path.GetDirectoryName(srcFile), Path.GetFileNameWithoutExtension(srcFile) + "_GLL.HDF");
                    if (UniversalDataProcess.PrjTransf(srcFile, srcFiles[srcFile], _currMosaicDatasets, dstFile, dstEnvelope, outResolution))
                        gllFiles.Add(dstFile);
                }
            }
            return gllFiles.Count == 0 ? null : gllFiles.ToArray();
        }

        private Dictionary<string, PrjEnvelope> MosaicRegion(FileListItem[] fli, int length, List<string> gllFiles)
        {
            Dictionary<string, PrjEnvelope> mosaicFiles = new Dictionary<string, PrjEnvelope>();
            Dictionary<string, List<string>> fileArray = new Dictionary<string, List<string>>();
            List<string> files = null;
            string dstFilename;
            for (int i = 0; i < length; i++)
            {
                dstFilename = Path.Combine(txtOutDir.Text + fli[i].Rid.OrbitDateTime.ToString("-yyyy-MM").Replace("-", "\\"), fli[i].Rid.ToWksFileName(".HDF"));
                if (!fileArray.ContainsKey(dstFilename))
                {
                    files = new List<string>();
                    files.Add(fli[i].FileName);
                    fileArray.Add(dstFilename, files);
                }
                else
                    fileArray[dstFilename].Add(fli[i].FileName);
            }
            string path = string.Empty;
            foreach (string key in fileArray.Keys)
            {
                try
                {
                    path = Path.GetDirectoryName(key);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    SHdf4To5 shdf4To5 = new SHdf4To5(fileArray[key].ToArray(), key);
                    Hdf4FileAttrs hdf4FileAttrs = UtilHdf4To5.PreConvertHdf4To5(shdf4To5);
                    UtilHdf4To5.DoConvertHdf4To5(shdf4To5, hdf4FileAttrs, GetDstEnvelope(), GetDstResultion(), ckGLL.Checked ? SpatialReference.GetDefault() : null);
                    if (File.Exists(key))
                        mosaicFiles.Add(key, new PrjEnvelope(hdf4FileAttrs.Hdf4FileAttr.Envelope.XMin, hdf4FileAttrs.Hdf4FileAttr.Envelope.XMax,
                                                              hdf4FileAttrs.Hdf4FileAttr.Envelope.YMin, hdf4FileAttrs.Hdf4FileAttr.Envelope.YMax));
                }
                catch (Exception ex)
                {
                    AddLog(ex.Message);
                }
            }
            return mosaicFiles.Count == 0 ? null : mosaicFiles;
        }

        private Dictionary<string, PrjEnvelope> GetSrcFiles(Dictionary<string, PrjEnvelope> MosaicFiles, FileListItem[] fli)
        {
            Dictionary<string, PrjEnvelope> srcFiles = new Dictionary<string, PrjEnvelope>();
            if (ckGCTPChina.Checked)
                srcFiles = MosaicFiles;
            else
                foreach (FileListItem filename in fli)
                {
                    SHdf4To5 shdf4To5 = new SHdf4To5(new string[] { filename.FileName }, filename.FileName);
                    Hdf4FileAttrs hdf4FileAttrs = UtilHdf4To5.PreConvertHdf4To5(shdf4To5);
                    SEnvelope envelopef4 = hdf4FileAttrs.Hdf4FileAttr.Envelope;
                    srcFiles.Add(filename.FileName, new PrjEnvelope(envelopef4.XMin, envelopef4.XMax, envelopef4.YMin, envelopef4.YMax));
                }
            return srcFiles.Count == 0 ? null : srcFiles;
        }

        List<string> _logList = new List<string>();
        public void AddLog(string msg)
        {
            _logList.Add(_logList.Count.ToString().PadLeft(6, '0') + "\t" + DateTime.Now.ToString("MMdd HH:mm:ss") + ":\t" + msg);
        }

        private bool CheckResultion(string filename, ref float lonResultion, ref float latResultion, HdrFile.Envelope srcEnvelope, Size srcSize)
        {
            float maxCY = 0.0001f;
            float currLonResultion = (float)((srcEnvelope.MaxX - srcEnvelope.MinX) / srcSize.Width);
            float currLatResultion = (float)((srcEnvelope.MaxY - srcEnvelope.MinY) / srcSize.Height);
            if (lonResultion == 0)
                lonResultion = currLonResultion;
            if (latResultion == 0)
                latResultion = currLatResultion;
            if (lonResultion != 0 && latResultion != 0)
            {
                if (lonResultion - currLonResultion > maxCY ||
                    latResultion - currLatResultion > maxCY)
                {
                    AddLog(Path.GetFileName(filename) + "分辨率不一致!");
                    return false;
                }
            }
            return true;
        }

        private bool CheckRegion(string filename, Hdf4Operator oper, out HdrFile.Envelope envelope)
        {
            envelope = null;
            string lefrupArg = "";
            SetAttr(oper.GetAttributes(), _leftupPointNames, ref lefrupArg);
            if (string.IsNullOrEmpty(lefrupArg))
            {
                AddLog(Path.GetFileName(filename) + "左上角坐标未找到!");
                return false;
            }
            string ringhtDownArg = "";
            SetAttr(oper.GetAttributes(), _rightDownPointNames, ref ringhtDownArg);
            if (string.IsNullOrEmpty(ringhtDownArg))
            {
                AddLog(Path.GetFileName(filename) + "右下角坐标未找到!");
                return false;
            }
            envelope = GetEnvelopeByAttr(lefrupArg, ringhtDownArg);
            return true;
        }

        private bool CheckDataSetExists(string filename, Hdf4Operator oper, out Size size, out Type datatype, out int datatypeSize)
        {
            size = new Size(0, 0);
            Size proSize = Size.Empty;
            datatype = typeof(UInt16);
            datatypeSize = 0;
            foreach (DataSetMosaicInfo item in _currMosaicInfos.DataSetMosaicInfos)
            {
                if (!oper.GetDatasetNames.Contains(item.DataSetName))
                {
                    AddLog(Path.GetFileName(filename) + "的[" + item.DataSetName + "]数据集不存在!");
                    return false;
                }
                else
                {
                    int bandCount = 0;
                    HDF4Helper.DataTypeDefinitions hdf4DataType = HDF4Helper.DataTypeDefinitions.DFNT_NUINT16;
                    if (GetDataSetSize(item.DataSetName, oper, out size, out bandCount, out hdf4DataType, out datatype, out datatypeSize))
                    {
                        if (proSize.Height == 0 && proSize.Width == 0)
                            proSize = size;
                        if (proSize.Width != size.Width || proSize.Height != size.Height)
                        {
                            AddLog(Path.GetFileName(filename) + "的[" + item.DataSetName + "]数据集行列数不一致!");
                            return false;
                        }
                        proSize = size;
                    }
                }
            }
            return true;
        }

        private bool CheckProjectInfos(string filename, Hdf4Operator oper)
        {
            string arg = "";
            SetAttr(oper.GetAttributes(), _projectNames, ref arg);
            if (!string.IsNullOrEmpty(arg) || !string.IsNullOrEmpty(_currMosaicInfos.Project))
            {
                if ((string.IsNullOrEmpty(arg) || string.IsNullOrEmpty(_currMosaicInfos.Project))
                    || arg.Trim().ToUpper() != _currMosaicInfos.Project.Trim().ToUpper())
                {
                    AddLog(Path.GetFileName(filename) + "投影方式不一致!");
                    return false;
                }
                else if (!string.IsNullOrEmpty(arg) && arg.Trim().ToUpper().IndexOf("GLL") == -1)
                {
                    SetAttr(oper.GetAttributes(), _projectArgsNames, ref arg);
                    if ((string.IsNullOrEmpty(arg) || string.IsNullOrEmpty(_currMosaicInfos.ProjectArgs))
                         || arg.Trim().ToUpper() != _currMosaicInfos.ProjectArgs.Trim().ToUpper())
                    {
                        AddLog(Path.GetFileName(filename) + "投影参数不一致!");
                        return false;
                    }
                }
            }
            return true;
        }

        private FileListItem[] GetFiles()
        {
            if (lstFileInfoList.Items.Count == 0)
                return null;
            List<FileListItem> files = new List<FileListItem>();
            foreach (ListViewItem item in lstFileInfoList.Items)
                files.Add(item.Tag as FileListItem);
            if (ckTimeMosaic.Checked)
                files.Sort(CompareClass.CompareSortByFilenameInfos);
            return files.Count == 0 ? null : files.ToArray();
        }

        private bool CheckEnv()
        {
            _currMosaicInfos = new MosaicInfos();
            _currMosaicInfos.Project = lbProject.Text;
            _currMosaicInfos.ProjectArgs = lbProjectAgrs.Text;
            _currMosaicInfos.ProjectAttrName = cbProject.Text;
            _currMosaicInfos.ProjectArgsAttrName = cbProjectAgrs.Text;
            if (!string.IsNullOrEmpty(lbProject.Text) && lbProject.Text.IndexOf("GLL") == -1 && string.IsNullOrEmpty(lbProjectAgrs.Text))
            {
                MessageBox.Show("未设定投影参数!");
                return false;
            }
            if (string.IsNullOrEmpty(lbLeftUpPoint.Text) || string.IsNullOrEmpty(lbRightDownPoint.Text))
            {
                MessageBox.Show("四角坐标未设定完整!");
                return false;
            }
            else
            {
                _currMosaicInfos.LeftUpAttrName = cbLeftUpPoint.Text;
                _currMosaicInfos.RightDownAttrName = cbRightDownPoint.Text;
            }
            if (lstDataSets.CheckedItems.Count == 0)
            {
                MessageBox.Show("未选择任何拼接数据集!");
                return false;
            }
            if (string.IsNullOrEmpty(txtFileDir.Text) && !Directory.Exists(txtFileDir.Text)
                && string.IsNullOrEmpty(txtOutDir.Text) && !Directory.Exists(txtOutDir.Text))
            {
                MessageBox.Show("请设置输出路径或路径不存在!");
                return false;
            }
            List<DataSetMosaicInfo> mosaicInfoList = new List<DataSetMosaicInfo>();
            _currMosaicDatasets = new Dictionary<string, H5T.H5Type>();
            foreach (ListViewItem item in lstDataSets.CheckedItems)
            {
                if (item.Tag == null)
                {
                    MessageBox.Show("数据集[" + item + "]未定义行列等信息!");
                    return false;
                }
                DataSetMosaicInfo dmi = item.Tag as DataSetMosaicInfo;
                if (dmi != null)
                {
                    mosaicInfoList.Add(dmi);
                    _currMosaicDatasets.Add(dmi.DataSetName, Utility.GetH5Type(dmi.HDF4DataType));
                }
            }
            _currMosaicInfos.DataSetMosaicInfos = mosaicInfoList.Count == 0 ? null : mosaicInfoList.ToArray();
            return true;
        }

        private HdrFile.Envelope GetEnvelopeByAttr(string leftUpStr, string RightDownStr)
        {
            HdrFile.Envelope env = new HdrFile.Envelope();
            string[] split = leftUpStr.Replace("(", "").Replace(")", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            env.MaxY = double.Parse(split[1]);
            env.MinX = double.Parse(split[0]);
            split = RightDownStr.Replace("(", "").Replace(")", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            env.MinY = double.Parse(split[1]);
            env.MaxX = double.Parse(split[0]);
            return env;
        }

        private void btDelFiles_Click(object sender, EventArgs e)
        {
            if (lstFileInfoList.SelectedItems == null || lstFileInfoList.SelectedIndices.Count == 0)
                return;
            if (MessageBox.Show("确定删除选定的" + lstFileInfoList.SelectedIndices.Count + "个文件吗?", "删除确认", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int i = 0; i < lstFileInfoList.SelectedIndices.Count; i++)
                    lstFileInfoList.Items.Remove(lstFileInfoList.SelectedItems[0]);
                lbFileCount.Text = lstFileInfoList.Items.Count.ToString();
            }
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定清空当前文件列表吗?", "确认清空", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                lstFileInfoList.Items.Clear();
                lbFileCount.Text = lstFileInfoList.Items.Count.ToString();
            }
        }

        private void lstDataSets_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (lstDataSets.CheckedItems.Count == 0)
                return;
            if (e.Item.Checked)
                e.Item.Selected = true;
        }
    }

    public class CompareClass
    {
        public static int CompareSortByFilenameInfos(FileListItem pro, FileListItem next)
        {
            if (pro.Rid.OrbitDateTime < next.Rid.OrbitDateTime)
                return -1;
            if (pro.Rid.OrbitDateTime > next.Rid.OrbitDateTime)
                return 1;
            return 0;
        }
    }
}
