using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.FileProject;
using GeoDo.Project;
using GeoDo.RasterProject;
using GeoDo.ProjectDefine;

namespace GeoDo.ProjectUI
{
    public partial class frmProject : Form
    {
        private string[] _fileNames;
        private int _selctIndex=0;
        private StringBuilder _sbParaInfo;
        private ProjectParas _proParas;
        private string _sensorName;
        private string _outTypeName;

        public ProjectParas ProParas
        {
            get { return _proParas; }
            set { _proParas = value; }
        }

        public frmProject()
        {
            InitializeComponent();
            Init();
            InitSB();
        }

        private void InitSB()
        {
            SetParaSBValue();
        }

        private void Init()
        {
            this.Width = 500;
            this.Height = 310;
            PrjStdsMapTableParser.CnfgFile = @"E:\MAS二期\投影库\GeoDo.Project\Output\GeoDo.Project.Cnfg.xml";
            ProParas = new ProjectParas();
            _sbParaInfo = new StringBuilder();
            ProParas.DstSpatialRef = SpatialReference.GetDefault();
        }

        private void SetParaSBValue()
        {
            _sbParaInfo.Clear();
            _sbParaInfo.Append("输出位置：" + txtOutFile.Text + "\r\n");
            _sbParaInfo.Append("投影类型：" + _outTypeName + "\r\n");
            _sbParaInfo.Append("输出范围：" + "\r\n");
            _sbParaInfo.Append("转化为亮温：" + cbIsRadiation.Checked + "\r\n");
            _sbParaInfo.Append("可见光进行太阳高度角订正：" + cbIsSolarZenith.Checked + "\r\n");
            _sbParaInfo.Append("原始行列号订正：");
            ShowInfo();
        }

        public void ShowInfo()
        {
            if (_sbParaInfo != null)
            {
                txtParaInfo.Text = _sbParaInfo.ToString();
            }
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetDefaultPara(string filename)
        {
          
            ProParas.SrcRaster = GeoDataDriver.Open(_fileNames[0]) as IRasterDataProvider;
            IBandProvider bandprovider = ProParas.SrcRaster.BandProvider;
            Dictionary<string, string>  _fileAttrs = bandprovider.GetAttributes();
            _fileAttrs.TryGetValue("File Alias Name", out _sensorName);
            ucSelectBands.InitTreeView(ProParas.SrcRaster);
            ucSelectBands.AddAll();
            SetBandsInfo();
            switch (_sensorName)
            {
                case "VIRR_L1":
                    ProParas.VirrPrjSetting = new  FY3_VIRR_PrjSettings();
                    ProParas.VirrPrjSetting.OutPathAndFileName = _fileNames[0] + "_WHOLE.ldf";
                    //_WHOLE_LW.
                    //_DXX_LW.
                    //_EF_LW.
                    ProParas.VirrPrjSetting.OutResolutionX = 0.01F;
                    ProParas.VirrPrjSetting.OutResolutionY = 0.01F;
                    ProParas.VirrPrjSetting.OutFormat = "LDF";
                    ProParas.VirrPrjSetting.OutEnvelope = null;
                    ucSelectBands.InitTreeView(ProParas.SrcRaster);
                    ucSelectBands.AddAll();
                    //ProParas.VirrPrjSetting.BandMapTable = ucSelectBands.GetBandMapList();
                    break;
            }
        }

        private void AddSelctFileNames(string[] _fileNames)
        {
            foreach (string filename in _fileNames)
            {
                object obj = filename;
                lbSelctFiles.Items.Add(obj);
            }
        }

        private void tsbAdd_Click(object sender, EventArgs e)
        {
            OpenFileDialog("请选择添加要投影的文件", true, out _fileNames);
            if (_fileNames == null)
                return;
            AddSelctFileNames(_fileNames);
            txtOutFile.Text = _fileNames[0] + "_Whole.ldf";
            SetDefaultPara(_fileNames[0]);
        }

        private void OpenFileDialog(string title,bool multiselect, out string[] selectFilenames)
        {
            selectFilenames = null;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = title;
                openFileDialog.CheckFileExists = true;
                openFileDialog.Multiselect = multiselect;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Application.DoEvents();
                    selectFilenames = openFileDialog.FileNames;
                }
            }
        }

        private void lbSelctFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selctIndex = lbSelctFiles.SelectedIndex;
        }

        private void btnOutFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".ldf";
            saveFileDialog.ShowDialog();
            if (saveFileDialog.FileName != "")
            {
                txtOutFile.Text = saveFileDialog.FileName;
                ProParas.VirrPrjSetting.OutPathAndFileName = saveFileDialog.FileName;
                ProParas.VirrPrjSetting.OutFormat = "LDF";
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (lbSelctFiles.Items!=null&&lbSelctFiles.Items.Count>2)
            {
                if (lbSelctFiles.SelectedIndex != 0)
                {
                    object obj;
                    obj = lbSelctFiles.Items[_selctIndex];
                    lbSelctFiles.Items[_selctIndex] = lbSelctFiles.Items[_selctIndex-1];
                    lbSelctFiles.Items[_selctIndex - 1] = obj;
                    lbSelctFiles.SelectedIndex = _selctIndex - 1;
                }
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (lbSelctFiles.Items != null && lbSelctFiles.Items.Count > 2)
            {
                if (lbSelctFiles.SelectedIndex != lbSelctFiles.Items.Count-1)
                {
                    object obj;
                    obj = lbSelctFiles.Items[_selctIndex];
                    lbSelctFiles.Items[_selctIndex] = lbSelctFiles.Items[_selctIndex +1];
                    lbSelctFiles.Items[_selctIndex + 1] = obj;
                    lbSelctFiles.SelectedIndex = _selctIndex + 1;
                }
            }
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDo_Click(object sender, EventArgs e)
        {
            foreach (string fileName in _fileNames)
            {
                ProParas.SrcRaster = GeoDataDriver.Open(fileName) as IRasterDataProvider;
                switch (_sensorName)
                {
                    case "VIRR_L1":
                        IFileProjector projVirr = FileProjector.GetFileProjectByName("FY3_VIRR");
                        //if (frmOutRange != null)
                        //{
                        //    ProParas.VirrPrjSetting.OutResolutionX = frmOutRange.OutResolution.Y;
                        //    ProParas.VirrPrjSetting.OutResolutionX = frmOutRange.OutResolution.Y;
                        //    ProParas.VirrPrjSetting.OutEnvelope = frmOutRange.PrjEnvelop;
                        //}
                        //ProParas.VirrPrjSetting.BandMapTable = ucSelectBands.GetBandMapList();
                        projVirr.Project(ProParas.SrcRaster, ProParas.VirrPrjSetting, ProParas.DstSpatialRef, ProParas.ProgressCallback);
                        break;
                }
            }
            MessageBox.Show("投影成功！");                                                                    
        }

        private void txtOutFile_TextChanged(object sender, EventArgs e)
        {
            SetParaSBValue();
        }

        private void tsbRemove_Click(object sender, EventArgs e)
        {
            if (lbSelctFiles.Items.Count != 0)
            {
                lbSelctFiles.Items.Remove(lbSelctFiles.Items[_selctIndex]);
                _selctIndex = 0;
                if (lbSelctFiles.Items.Count == 0)
                {
                    ucSelectBands.Clear();
                    txtOutFile.Text = "";
                }
            }
        }

        private void tsbClear_Click(object sender, EventArgs e)
        {
            lbSelctFiles.Items.Clear();
            ucSelectBands.Clear();
            _selctIndex = 0;
            txtOutFile.Text = "";
        }

        private void tsbChBands_Click(object sender, EventArgs e)
        {
            panelBands.Visible = !panelBands.Visible;
            this.Width = panelBands.Visible ? 680 : 500;
        }

        private void tsbOutRange_Click(object sender, EventArgs e)
        {
            //frmOutRange = new FormOutRange();
            //frmOutRange.Show();
        }

        private void cbIsRadiation_CheckedChanged(object sender, EventArgs e)
        {
            //ProParas.MersiPrjSetting.IsRadiation = cbIsRadiation.Checked;
            //ProParas.VirrPrjSetting.IsRadiation = cbIsRadiation.Checked;
            SetParaSBValue();
        }

        private void cbIsSolarZenith_CheckedChanged(object sender, EventArgs e)
        {
            //ProParas.MersiPrjSetting.IsSolarZenith = cbIsSolarZenith.Checked;
            //ProParas.VirrPrjSetting.IsSolarZenith = cbIsSolarZenith.Checked;
            SetParaSBValue();
        }

        private void tsmiMoreType_Click(object sender, EventArgs e)
        {
            using (SpatialReferenceSelection spatialReferenceSelUI = new SpatialReferenceSelection())
            {
                if (spatialReferenceSelUI.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
                {
                    MessageBox.Show(spatialReferenceSelUI.SpatialReference != null ?
                        spatialReferenceSelUI.SpatialReference.ToString() : string.Empty);
                    ProParas.DstSpatialRef = spatialReferenceSelUI.SpatialReference;
                    _outTypeName = ProParas.DstSpatialRef.Name;
                }
            }
            SetParaSBValue();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {

            gBoxParas.Visible = !gBoxParas.Visible;
            this.Height = gBoxParas.Visible ? 450 : 310;
        }

        private void SetBandsInfo()
        {
            //StringBuilder sb = new StringBuilder();
            //int i = 1;
            //sb.Append("选择波段：\r\n");
            //foreach (BandMap banMap in ucSelectBands.GetBandMapList())
            //{
            //    sb.Append("\tband_" + i + "\r\n");
            //    i++;
            //}
            //txtBandInfo.Text = sb.ToString();
        }


    }
}
