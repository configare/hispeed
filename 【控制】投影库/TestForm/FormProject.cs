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

namespace TestForm
{
    public partial class frmProject : Form
    {
        private FormSetParas frmSetPara;
        private Point _oldLocation;
        private string[] _fileNames;
        private int _selctIndex=0;
        private StringBuilder _sbInfo;
        private string _srcFilename = null;
        private string _dstFilename = null;
        private ProjectParas _proParas;

        public ProjectParas ProParas
        {
            get { return _proParas; }
            set { _proParas = value; }
        }

        public StringBuilder SbInfo
        {
            get { return _sbInfo; }
            set { _sbInfo = value; }
        }

        public string SrcFilename
        {
            get { return _srcFilename; }
            set { _srcFilename = value; }
        }

        public string DstFilename
        {
            get { return _dstFilename; }
            set { _dstFilename = value; }
        }

        public frmProject()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            this.Height = 350;
            PrjStdsMapTableParser.CnfgFile = @"E:\MAS二期\投影库\GeoDo.Project\Output\GeoDo.Project.Cnfg.xml";
            ProParas = new ProjectParas();
            frmSetPara = new FormSetParas();
            _sbInfo = new StringBuilder();
            ProParas.PrjSetting = new FY3_MERSI_PrjSettings();
        }

        private void btnSetPara_Click(object sender, EventArgs e)
        {
            if (frmSetPara.IsDisposed)
            {
                frmSetPara = new FormSetParas();
            }
            frmSetPara.Show();
            frmSetPara.ProParas = this.ProParas;
            frmSetPara.SetDesktopLocation(this.DesktopLocation.X + this.Width, this.DesktopLocation.Y);
            frmSetPara.MainFrmPoint = this.DesktopLocation;
            frmSetPara.MainFrmSize = this.Size;
            _oldLocation = this.Location;
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmProject_Move(object sender, EventArgs e)
        {
            int a = frmSetPara.DesktopLocation.Y - _oldLocation.Y;
            if (frmSetPara != null)
            {
                frmSetPara.SetDesktopLocation(this.DesktopLocation.X + this.Width,this.DesktopLocation.Y+a);
                frmSetPara.MainFrmPoint = this.DesktopLocation;
                frmSetPara.MainFrmSize = this.Size;
            }
            _oldLocation = this.Location;
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            OpenFileDialog("请选择添加要投影的文件", true, out _fileNames);
            if (_fileNames == null)
                return;
            AddSelctFileNames(_fileNames);
            txtOutFile.Text = _fileNames[0] + ".ldf";
            SetDefaultPara(_fileNames[0]);
        }

        private void SetDefaultPara(string filename)
        {
            ProParas.SrcRaster = GeoDataDriver.Open(_fileNames[0]) as IRasterDataProvider;
            //IBandProvider provider = ProParas.SrcRaster.BandProvider;
            //IRasterBand[] bands = provider.GetBands("EV_250_Aggr.1KM_RefSB");
            ProParas.PrjSetting.OutPathAndFileName = _fileNames[0] + ".ldf";
            //_WHOLE_LW.
            //_DXX_LW.
            //_EF_LW.
            ProParas.PrjSetting.OutResolutionX = 0.01F;
            ProParas.PrjSetting.OutResolutionY = 0.01F;
            ProParas.PrjSetting.OutFormat = "LDF";
            //PrjSetting.OutEnvelope = PrjEnvelope.CreateByCenter(116.890377, 27.7621965, 33.49, 21.53);
            ProParas.PrjSetting.OutEnvelope = null;
            List<BandMap> bandmapList = new List<BandMap>();
            bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr.1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 0 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr.1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 1 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr.1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 2 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr.1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 3 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_250_Aggr.1KM_Emissive", File = ProParas.SrcRaster, BandIndex = 0 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 0 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 1 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 2 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 3 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 4 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 5 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 6 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 7 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 8 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 9 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 10 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 11 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 12 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 13 });
            bandmapList.Add(new BandMap() { DatasetName = "EV_1KM_RefSB", File = ProParas.SrcRaster, BandIndex = 14 });
            ProParas.PrjSetting.BandMapTable = bandmapList;
        }

        private void AddSelctFileNames(string[] _fileNames)
        {
            foreach (string filename in _fileNames)
            {
                object obj = filename;
                lbSelctFiles.Items.Add(obj);
            }
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

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lbSelctFiles.Items.Count != 0)
            {
                lbSelctFiles.Items.Remove(lbSelctFiles.Items[_selctIndex]);
                _selctIndex = 0;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lbSelctFiles.Items.Clear();
            _selctIndex = 0;
        }

        private void btnOutFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".ldf";
            saveFileDialog.ShowDialog();
            txtOutFile.Text = saveFileDialog.FileName;
            ProParas.PrjSetting.OutPathAndFileName = saveFileDialog.FileName;
            ProParas.PrjSetting.OutFormat = "LDF";
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

        private void btnDo_Click(object sender, EventArgs e)
        {
            IFileProjector proj = FileProjector.GetFileProjectByName("FY3_MERSI");
            proj.Project(ProParas.SrcRaster, ProParas.PrjSetting, ProParas.DstSpatialRef, ProParas.ProgressCallback);
            MessageBox.Show("投影成功！");
                                                                               
        }

        private void txtOutFile_TextChanged(object sender, EventArgs e)
        {
            SbInfo.AppendLine(txtOutFile.Text);
            ShowInfo();
        }

        public void ShowInfo()
        {
            if (SbInfo != null)
            {
                txtParaInfo.Text = SbInfo.ToString();
            }
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            gBoxParas.Visible = !gBoxParas.Visible;
            if (gBoxParas.Visible)
            {
                this.Height = 500;
                btnDetail.Image = TestForm.Properties.Resources.arrow_up;
            }
            else
            {
                this.Height = 350;
                btnDetail.Image = TestForm.Properties.Resources.arrow_down;
            }
        }
    }
}
