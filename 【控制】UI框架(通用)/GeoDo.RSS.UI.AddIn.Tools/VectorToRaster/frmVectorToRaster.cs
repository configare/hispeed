using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public partial class frmVectorToRaster : Form
    {
        string[] outTypes = new string[] { "Float", "Int32", "Int16", "UInt16", "Byte" };
        Envelope _envelope = null;
        Feature[] _vectorFeatures = null;
        private Dictionary<string, ListViewItem> _StrToListViewItem = new Dictionary<string, ListViewItem>();
        double _resolution;

        public frmVectorToRaster()
        {
            InitializeComponent();
            InitControls();
        }

        private void InitControls()
        {
            for (int i = 0; i < outTypes.Length;i++ )
                cmbOutputType.Items.Add(outTypes[i]);
            cmbOutputType.SelectedIndex = 0;
            btnOpenOutputFile.Image = imageList1.Images[0];
            btnOpenShpFile.Image = imageList1.Images[0];
        }

        private bool TryLoadVectorFeatures(string fileName)
        {
            using(IVectorFeatureDataReader vdr = VectorDataReaderFactory.GetUniversalDataReader(fileName) as IVectorFeatureDataReader)
            {
                if (vdr == null)
                {
                    MsgBox.ShowInfo("矢量文件\"" + fileName + "\"不是标准的shapefiles文件或者文件已损坏。");
                    return false;
                }
                if (vdr.ShapeType != enumShapeType.Polygon)
                {
                    MsgBox.ShowInfo("矢量文件\"" + fileName + "\"的几何类型为非多边形类型,不支持栅格化。");
                    return false;
                }
                _envelope = vdr.Envelope;
                txtMinLon.Value = Math.Round(_envelope.MinX, 2);
                txtMaxLat.Value = Math.Round(_envelope.MaxY, 2);
                SetDefaultSize();
                _vectorFeatures = vdr.Features;
                InitVectorFeatures();
                return true;
            }
        }

        private void InitVectorFeatures()
        {
            lvFeatures.Items.Clear();
            if (_vectorFeatures == null || _vectorFeatures.Length == 0)
                return;
            Feature fet = _vectorFeatures[0];
            if (fet.FieldNames != null && fet.FieldNames.Length > 0)
            {
                foreach (string field in fet.FieldNames)
                {
                    ColumnHeader header = new ColumnHeader(field);
                    header.Text = field;
                    lvFeatures.Columns.Add(header);
                }
            }
            lvFeatures.BeginUpdate();
            foreach (Feature f in _vectorFeatures)
            {
                string text = (f.FieldValues != null && f.FieldValues.Length > 0) ? f.FieldValues[0] : f.OID.ToString();
                ListViewItem it = new ListViewItem(text);
                if (f.FieldValues != null && f.FieldValues.Length > 0)
                {
                    for (int i = 1; i < f.FieldValues.Length; i++)
                        it.SubItems.Add(f.GetFieldValue(i));
                }
                it.Tag = f;
                it.Checked = true;
                lvFeatures.Items.Add(it);
            }
            lvFeatures.EndUpdate();
            int idx = 0;
            int ifld = 0;
            ListViewItem li = null;
            int index = 0;
            cmbFields.Items.Add("OID");
            lsFields.Items.Add("OID");
            li = lsFields.Items[lsFields.Items.Count - 1];
            li.Tag = index++;
            foreach (string fld in _vectorFeatures[0].FieldNames)
            {
                cmbFields.Items.Add(fld);
                lsFields.Items.Add(fld);
                li = lsFields.Items[lsFields.Items.Count - 1];
                li.Checked = true;
                li.Tag = index;
                _StrToListViewItem.Add(fld, li);
                if (fld == "土地利用类型" || fld.ToUpper().Contains("NAME") ||
                    fld.Contains("名称") || fld.ToUpper().Contains("CHINESE"))
                {
                    idx = ifld;
                }
                ifld++;
                index++;
            }
            cmbFields.SelectedIndex = idx;
        }

        private void SetDefaultSize()
        {
            if (_envelope == null)
                return;
            double res = txtResolution.Value;
            if (res == 0)
                return;
            double w = _envelope.Width / res;
            double h = _envelope.Height / res;
            try
            {
                while ((w * h) > (10000 * 10000))
                {
                    w = _envelope.Width / res;
                    h = _envelope.Height / res;
                    res += 0.01f;
                }
                txtResolution.Value = Math.Round(res, 4);
                txtWidth.Value = (int)Math.Ceiling(w);
                txtHeight.Value = (int)Math.Ceiling(h);
            }
            catch
            {
                MsgBox.ShowInfo("当前设置的栅格化参数将导致栅格化后文件过大,建议降低分辨率以缩小栅格化后文件大小。");
                txtWidth.Maximum = (int)Math.Ceiling(w);
                txtHeight.Maximum = (int)Math.Ceiling(h);
                txtWidth.Value = (int)Math.Ceiling(w);
                txtHeight.Value = (int)Math.Ceiling(h);
            }
        }

        private void InitComponents()
        {
            lsFields.Clear();
            lvFeatures.Clear();
            _StrToListViewItem.Clear();
            cmbFields.Items.Clear();
        }

        private bool IsAllPaint()
        {
            double resolution=double.Parse(txtResolution.Text);
            int owidth = (int)Math.Ceiling((_envelope.MaxX - _envelope.MinX) / resolution);
            int ohight = (int)Math.Ceiling((_envelope.MaxY - _envelope.MinY) / resolution);
            int width = int.Parse(txtWidth.Text);
            int hight = int.Parse(txtHeight.Text);
            if (width != owidth || hight != ohight)
                return false;
            else if(txtMaxLat.Value!=Math.Round(_envelope.MaxY,2)||txtMinLon.Value!=Math.Round(_envelope.MinX,2))
                return false;
            return true;
        }

        private Feature[] CollectCheckedFeatures()
        {
            Feature[] fets = new Feature[lvFeatures.CheckedIndices.Count];
            for (int i = 0; i < fets.Length; i++)
            {
                fets[i] = lvFeatures.CheckedItems[i].Tag as Feature;
            }
            return fets;
        }

        private bool ArgIsOK()
        {
            if (txtShpFile.Text.Trim() == string.Empty)
            {
                MsgBox.ShowInfo("请选择要栅格化的矢量文件。");
                return false;
            }
            if (lvFeatures.CheckedIndices.Count == 0 )
            {
                MsgBox.ShowInfo("请选择要栅格化的矢量要素。");
                return false;
            }
            if (txtMinLon.Value < -180 || txtMinLon.Value > 180)
            {
                MsgBox.ShowInfo("栅格化参数[左上角经度]设置错误。");
                return false;
            }
            if (txtMaxLat.Value < -90 || txtMaxLat.Value > 90)
            {
                MsgBox.ShowInfo("栅格化参数[左上角纬度]设置错误。");
                return false;
            }
            int w = (int)txtWidth.Value;
            int h = (int)txtHeight.Value;
            if (w <= 0)
            {
                MsgBox.ShowInfo("栅格化参数[输出宽度]设置错误。");
                return false;
            }
            if (h <= 0)
            {
                MsgBox.ShowInfo("栅格化参数[输出高度]设置错误。");
                return false;
            }
            double resolution = txtResolution.Value;
            if (resolution <= 0)
            {
                MsgBox.ShowInfo("栅格化参数[输出分辨率]设置错误。");
                return false;
            }
            _resolution = resolution;
            if (txtOutFile.Text.Trim() == string.Empty)
            {
                MsgBox.ShowInfo("输出文件名设置错误。");
                return false;
            }
            string dir = Path.GetDirectoryName(txtOutFile.Text);
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
            }
            catch (Exception ex)
            {
                MsgBox.ShowInfo("输出文件名设置错误," + ex.Message);
            }
            if (File.Exists(txtOutFile.Text))
            {
                DialogResult isOk = MsgBox.ShowQuestionYesNo("文件\"" + txtOutFile.Text + "\"已经存在,要覆盖吗?");
                if (isOk == DialogResult.Yes)
                    return true;
                return false;
            }
            return true;
        }

        private void SelectAllFeatures(bool isSelect)
        {
            if (lvFeatures.Items.Count == 0)
                return;
            lvFeatures.BeginUpdate();
            foreach (ListViewItem it in lvFeatures.Items)
            {
                it.Checked = isSelect;
            }
            lvFeatures.EndUpdate();
        }

        private CoordEnvelope GetEnvelope(Envelope env)
        {
            if (env == null)
                return null;
            else
                return new CoordEnvelope(env.MinX, env.MaxX, env.MinY, env.MaxY);
        }

        private void WriteMapTable(Feature[] features, string outfilename,int[] selectFieldIndex)
        {
            if (features == null || features.Length == 0)
                return;
            if (selectFieldIndex == null || selectFieldIndex.Length < 1)
                return;
            Dictionary<string, List<string>> mapTable = new Dictionary<string, List<string>>();
            string keyField = cmbFields.Text;
            if (keyField == "OID")
            {
                foreach (Feature item in features)
                {
                    List<string> fieldValues = new List<string>();
                    for (int i = 0; i < selectFieldIndex.Length; i++)
                    {
                        int index = selectFieldIndex[i] - 1;
                        fieldValues.Add(item.FieldValues[index]);
                    }
                    mapTable.Add(item.OID.ToString(), fieldValues);
                }
            }
            else
            {
                foreach (Feature item in features)
                {
                    List<string> fieldValues = new List<string>();
                    for (int i = 0; i < selectFieldIndex.Length; i++)
                    {
                        if (selectFieldIndex[i] == 0)
                            fieldValues.Add(item.OID.ToString());
                        else
                        {
                            int index = selectFieldIndex[i] - 1;
                            fieldValues.Add(item.FieldValues[index]);
                        }
                    }
                    mapTable.Add(item.GetFieldValue(keyField), fieldValues);
                }
            }
            if (mapTable.Count < 1)
                return;
            string outFile = Path.Combine(Path.GetDirectoryName(outfilename), Path.GetFileNameWithoutExtension(outfilename) + ".txt");
            List<string> contexts = new List<string>();
            string context = null;
            foreach (string key in mapTable.Keys)
            {
                context = key;
                foreach (string item in mapTable[key])
                    context += "\t" + item;
                contexts.Add(context);
            }
            File.WriteAllLines(outFile, contexts.ToArray(), Encoding.Default);
        }

        private int[] GetMapTableIndex()
        {
            if (lsFields.CheckedItems.Count == 0)
                return null;
            List<int> mapTableItems = new List<int>();
            foreach (ListViewItem item in lsFields.CheckedItems)
            {
                mapTableItems.Add(int.Parse(item.Tag.ToString()));
            }
            return mapTableItems.Count == 0 ? null : mapTableItems.ToArray();
        }

        #region 进度条

        private void StartProgress()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => { progressBar1.Value = 0; progressBar1.Visible = true; }));
            }
            else
            {
                progressBar1.Value = 0;
                progressBar1.Visible = true;
            }
        }

        private void FinishProgerss()
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    progressBar1.Visible = false;
                }));
            else
            {
                progressBar1.Visible = false;
            }
        }

        private void ChangeProgress(int progerss,string text)
        {
            progerss = progerss > 100 ? 100 : progerss;
            if (this.InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    progressBar1.Value = progerss;
                }));
            else
            {
                progressBar1.Value = progerss;
            }
        }

        #endregion

        private void btnOpenShpFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "ESRI Shape files(*.shp)|*.shp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    InitComponents();
                    bool isOK = TryLoadVectorFeatures(dlg.FileName);
                    if (isOK)
                    {
                        txtShpFile.Text = dlg.FileName;
                        txtOutFile.Text = Path.Combine(Path.GetDirectoryName(dlg.FileName),
                                                    "Raster_" + Path.GetFileNameWithoutExtension(dlg.FileName) +
                                                    ".dat");
                    }
                }
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (btnSelect.Text == "全不选")
            {
                btnSelect.Text = "全选";
                SelectAllFeatures(false);
            }
            else
            {
                btnSelect.Text = "全不选";
                SelectAllFeatures(true);
            }
        }

        private void btnOpenOutputFile_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = SupportedFileFilters.LdfFilter + "|" + SupportedFileFilters.SrfFilterString;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtOutFile.Text = dlg.FileName;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            btnOk.Enabled = btnCancel.Enabled = false;
            btnOk.Text = "执行中...";
            try
            {
                if (!ArgIsOK())
                    return;
                enumDataType dataType = enumDataType.Int16;
                switch (cmbOutputType.SelectedIndex)
                {
                    case 0:
                        {
                            dataType = enumDataType.Float;
                            break;
                        }
                    case 1:
                        {
                            dataType = enumDataType.Int32;
                            break;
                        }
                    case 2:
                        {
                            dataType = enumDataType.Int16;
                            break;
                        }
                    case 3:
                        {
                            dataType = enumDataType.UInt16;
                            break;
                        }
                    case 4:
                        {
                            dataType = enumDataType.Byte;
                            break;
                        }
                }
                IVectorToRaster vector2Raster = new VectorToRaster(new Action<int, string>(ChangeProgress));
                StartProgress();
                Feature[] features = CollectCheckedFeatures();
                //手动更改栅格文件大小，需要对栅格文件进行裁切或填补！
                if (IsAllPaint())
                    vector2Raster.ProcessVectorToRaster(features, cmbFields.Text, dataType, _resolution, GetEnvelope(_envelope), txtOutFile.Text);
                else
                {
                    //栅格临时文件
                    string tempRasterName = Path.GetFileNameWithoutExtension(txtOutFile.Text) + "_temp";
                    tempRasterName = Path.Combine(Path.GetDirectoryName(txtOutFile.Text), tempRasterName+Path.GetExtension(txtOutFile.Text));
                    vector2Raster.ProcessVectorToRaster(features, cmbFields.Text, dataType, _resolution, GetEnvelope(_envelope), tempRasterName);
                    using (IRasterDataProvider dataPrd = GeoDataDriver.Open(tempRasterName) as IRasterDataProvider)
                    {
                        if (dataPrd == null)
                            return;
                        RasterMaper[] rasterInputMaps = new RasterMaper[] { new RasterMaper(dataPrd, new int[] { 1 }) };
                        using(IRasterDataProvider outRaster = CreateOutRaster(txtOutFile.Text, rasterInputMaps))
                        {
                            RasterMaper[] rasterOutputMaps = new RasterMaper[] { new RasterMaper(outRaster, new int[] { 1 }) };
                            switch (dataPrd.DataType)
                            {
                                case enumDataType.Float:
                                    {
                                        RasterProcessModel<float, float> rfr = new RasterProcessModel<float, float>(ChangeProgress);
                                        rfr.SetRaster(rasterInputMaps, rasterOutputMaps);
                                        rfr.RegisterCalcModel(new RasterCalcHandler<float, float>((rvInVistor, rvOutVistor, aoi) =>
                                        {
                                            if (rvInVistor[0].RasterBandsData[0] == null)
                                                return;
                                            int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                                            for (int index = 0; index < dataLength; index++)
                                            {
                                                float value = rvInVistor[0].RasterBandsData[0][index];
                                                if (value != 0)
                                                    rvOutVistor[0].RasterBandsData[0][index] = value;
                                            }
                                        }));
                                        //执行
                                        rfr.Excute();
                                        break;
                                    }
                                case enumDataType.Int16:
                                    {
                                        RasterProcessModel<Int16, Int16> rfr = new RasterProcessModel<Int16, Int16>(ChangeProgress);
                                        rfr.SetRaster(rasterInputMaps, rasterOutputMaps);
                                        rfr.RegisterCalcModel(new RasterCalcHandler<Int16, Int16>((rvInVistor, rvOutVistor, aoi) =>
                                        {
                                            if (rvInVistor[0].RasterBandsData[0] == null)
                                                return;
                                            int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                                            for (int index = 0; index < dataLength; index++)
                                            {
                                                Int16 value = rvInVistor[0].RasterBandsData[0][index];
                                                if (value != 0)
                                                    rvOutVistor[0].RasterBandsData[0][index] = value;
                                            }
                                        }));
                                        //执行
                                        rfr.Excute();
                                        break;
                                    }
                                case enumDataType.Int32:
                                    {
                                        RasterProcessModel<Int32, Int32> rfr = new RasterProcessModel<Int32, Int32>(ChangeProgress);
                                        rfr.SetRaster(rasterInputMaps, rasterOutputMaps);
                                        rfr.RegisterCalcModel(new RasterCalcHandler<Int32, Int32>((rvInVistor, rvOutVistor, aoi) =>
                                        {
                                            if (rvInVistor[0].RasterBandsData[0] == null)
                                                return;
                                            int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                                            for (int index = 0; index < dataLength; index++)
                                            {
                                                Int32 value = rvInVistor[0].RasterBandsData[0][index];
                                                if (value != 0)
                                                    rvOutVistor[0].RasterBandsData[0][index] = value;
                                            }
                                        }));
                                        //执行
                                        rfr.Excute();
                                        break;
                                    }
                                case enumDataType.UInt16:
                                    {
                                        RasterProcessModel<UInt16, UInt16> rfr = new RasterProcessModel<UInt16, UInt16>(ChangeProgress);
                                        rfr.SetRaster(rasterInputMaps, rasterOutputMaps);
                                        rfr.RegisterCalcModel(new RasterCalcHandler<UInt16, UInt16>((rvInVistor, rvOutVistor, aoi) =>
                                        {
                                            if (rvInVistor[0].RasterBandsData[0] == null)
                                                return;
                                            int dataLength = rvOutVistor[0].SizeY * rvOutVistor[0].SizeX;
                                            for (int index = 0; index < dataLength; index++)
                                            {
                                                UInt16 value = rvInVistor[0].RasterBandsData[0][index];
                                                if (value != 0)
                                                    rvOutVistor[0].RasterBandsData[0][index] = value;
                                            }
                                        }));
                                        //执行
                                        rfr.Excute();
                                        break;
                                    }
                            }
                        }
                    }
                    if (File.Exists(tempRasterName))
                        File.Delete(tempRasterName);
                    string hdrFileName = Path.Combine(Path.GetDirectoryName(tempRasterName), 
                        Path.GetFileNameWithoutExtension(tempRasterName) + ".hdr");
                    if (File.Exists(hdrFileName))
                        File.Delete(hdrFileName);
                }
                WriteMapTable(features, txtOutFile.Text, GetMapTableIndex());
                FinishProgerss();
                MsgBox.ShowInfo("栅格化已完成!");
            }
            catch (Exception ex)
            {
                MsgBox.ShowError("对文件\"" + txtShpFile.Text + "\"进行栅格化失败:" + ex.Message);
                return;
            }
            finally
            {
                btnCancel.Enabled = btnOk.Enabled = true;
                btnOk.Text = "执行";
            }
        }

        private IRasterDataProvider CreateOutRaster(string fileName, RasterMaper[] rasterMaper)
        {
            string extension = Path.GetExtension(fileName).ToUpper();
            IRasterDataDriver raster = null;
            if(extension==".LDF")
               raster = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            else if (extension==".DAT")
               raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            int width = (int)(txtWidth.Value);
            int height = (int)(txtHeight.Value);
            double minY = txtMaxLat.Value - _resolution * height;
            double maxX = txtMinLon.Value + _resolution * width;
            CoordEnvelope outEnv = new CoordEnvelope(txtMinLon.Value, maxX, minY, txtMaxLat.Value);
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(fileName, width, height, 1, rasterMaper[0].Raster.DataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        private void cmbFields_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = cmbFields.SelectedIndex;
            if (index < 0)
                return;
            else
            {
                foreach (ListViewItem item in lsFields.Items)
                    item.Checked = true;
                lsFields.Items[index].Checked= false;
            }
        }

    }
}
