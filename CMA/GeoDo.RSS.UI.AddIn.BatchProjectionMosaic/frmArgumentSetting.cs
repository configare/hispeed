using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.Tools.Projection;
using GeoDo.FileProject;
using System.Diagnostics;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.BatchProjectionMosaic
{
    public partial class frmArgumentSetting : Form
    {
        public string _path = AppDomain.CurrentDomain.BaseDirectory + "BatchProjectionMoasic.xml";
        private List<PrjEnvelopeItem> _envList = null;

        public frmArgumentSetting()
        {
            InitializeComponent();
            Load += new EventHandler(frmArgumentSetting_Load);
        }

        void frmArgumentSetting_Load(object sender, EventArgs e)
        {
            _envList = new List<PrjEnvelopeItem>();
            InputArg arg = InputArg.ParseXml(_path);
            if (arg != null)
            {
                InitSetting(arg);
            }
            cmbProjectID.SelectedIndex = 0;
        }

        private void ckbMoasic_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbMoasic.Checked == true)
            {
                ckbOnlyMoasicFile.Visible = true;
                ckbOnlyMoasicFile.Enabled = true;
            }
            else
            {
                ckbOnlyMoasicFile.Visible = false;
                ckbOnlyMoasicFile.Visible = false;
            }
        }

        private void rdoCResolution_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoCResolution.Checked == true)
            {
                txtResolution.Enabled = true;
                txtResolution.Visible = true;
            }
            else
            {
                txtResolution.Enabled = false;
                txtResolution.Visible = false;
            }
        }

        private void btnInputDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtInputDir.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void InitSetting(InputArg arg)
        {
            //band setting
            string bandsString = "";
            if (arg.Bands == null || arg.Bands.Length == 0)
                rdoAllBands.Checked = true;
            else
            {
                int i = 0;
                for (; i < arg.Bands.Length - 1; i++)
                    bandsString += (arg.Bands[i].ToString() + ",");
                bandsString += arg.Bands[i].ToString();
                bandsString.Remove(bandsString.Length - 1);
                rdoCustomBands.Checked = true;
                txtBands.Text = bandsString;
            }
            //Input dir
            if (!string.IsNullOrEmpty(arg.InputFilename))
            {
                //解析inputFilename
                string inputFilename = arg.InputFilename;
                if (!inputFilename.Contains(";"))
                {
                    string[] inputFName = arg.InputFilename.Split(' ');
                    if (inputFName != null && inputFName.Length > 0)
                    {
                        rdoInputDir.Checked = true;
                        txtInputDir.Text = inputFName[0];
                        txtFilter.Text = inputFName[inputFName.Length - 1];
                    }
                }
                else
                {
                    rdbInputFiles.Checked = true;
                    txtInputFiles.Text = inputFilename;
                }
            }
            //Output dir
            txtOutputDir.Text = arg.OutputDir;
            //Project ID
            switch (arg.ProjectionIdentify)
            {
                case "GLL": cmbProjectID.SelectedIndex = 0;
                    break;
                default: cmbProjectID.SelectedIndex = 0;
                    break;
            }
            //Resultion
            if (arg.ResolutionX != 0 || arg.ResolutionY != 0)
            {
                rdoCResolution.Checked = true;
                txtResolution.Text = arg.ResolutionX.ToString() + "," + arg.ResolutionY.ToString();
            }
            //Region
            if (arg.Envelopes != null && arg.Envelopes.Length > 0)
            {
                foreach (PrjEnvelopeItem item in arg.Envelopes)
                {
                    lstRegions.Items.Add(item.Name);
                    _envList.Add(new PrjEnvelopeItem(item.Name, (item.PrjEnvelope.Clone() as RasterProject.PrjEnvelope)));
                }
                lstRegions.SelectedIndex = 0;
                txtRegionName.Text = arg.ValidEnvelopes[0].Name;
                ucOutputRegion.MaxX = arg.ValidEnvelopes[0].PrjEnvelope.MaxX;
                ucOutputRegion.MaxY = arg.ValidEnvelopes[0].PrjEnvelope.MaxY;
                ucOutputRegion.MinX = arg.ValidEnvelopes[0].PrjEnvelope.MinX;
                ucOutputRegion.MinY = arg.ValidEnvelopes[0].PrjEnvelope.MinY;
            }
            if (arg.MosaicInputArg != null)
            {
                ckbMoasic.Checked = true;
                ckbOnlyMoasicFile.Checked = arg.IsOnlySaveMosaicFile;
                txtMoasicOutDir.Text = arg.MosaicInputArg.OutputDir;
            }
            else
            {
                ckbMoasic.Checked = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnExcute_Click(object sender, EventArgs e)
        {
            //Save arg xml
            ////check argments
            if (CheckArgument() == false)
            {
                MessageBox.Show("参数设置不完整！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            InputArg arg = new InputArg();
            if (rdbInputFiles.Checked)
                arg.InputFilename = txtInputFiles.Text;
            else
                arg.InputFilename = txtInputDir.Text + " " + txtFilter.Text;
            if (rdoAllBands.Checked == true)
                arg.Bands = null;
            else
            {
                int bandNo;
                string[] bandNos = txtBands.Text.Trim().Split(',');
                arg.Bands = new int[bandNos.Length];
                for (int i = 0; i < bandNos.Length; i++)
                    if (int.TryParse(bandNos[i], out bandNo))
                        arg.Bands[i] = bandNo;
            }
            arg.OutputDir = txtOutputDir.Text;
            if (rdoCResolution.Checked == true)
            {
                float resX;
                if (float.TryParse(txtResolution.Text, out resX))
                    arg.ResolutionX = arg.ResolutionY = resX;
            }
            //region
            arg.ValidEnvelopes = new PrjEnvelopeItem[_envList.Count()];
            arg.Envelopes = new PrjEnvelopeItem[_envList.Count()];
            if (ckbMoasic.Checked == true)
            {
                arg.MosaicInputArg = new MosaicInputArg();
                RasterProject.PrjEnvelope envelope = GetEnvelopeFromUI();
                arg.MosaicInputArg.Envelope = new PrjEnvelopeItem(txtRegionName.Text, envelope);
                arg.MosaicInputArg.OutputDir = txtMoasicOutDir.Text;
            }
            for (int i = 0; i < _envList.Count; i++)
            {
                arg.ValidEnvelopes[i] =
                arg.Envelopes[i] = _envList[i];
            }
            arg.IsOnlySaveMosaicFile = ckbOnlyMoasicFile.Checked;
            arg.ProjectionIdentify = "GLL";
            arg.ToXml(_path);
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool CheckArgument()
        {
            if (string.IsNullOrEmpty(txtInputDir.Text) && string.IsNullOrEmpty(txtInputFiles.Text))
                return false;
            if (string.IsNullOrEmpty(txtOutputDir.Text))
                return false;
            if (lstRegions.Items.Count<1)
                return false;
            if (ckbMoasic.Checked == true&&string.IsNullOrWhiteSpace(txtMoasicOutDir.Text))
            {
                return false;
            }
            return true;
        }

        private void cboSatellite_SelectedValueChanged(object sender, EventArgs e)
        {
            string satellite = string.Empty;
            string sensor = string.Empty;
            if (cboSatellite.SelectedIndex != -1)
            {
                switch (cboSatellite.SelectedIndex)
                {
                    case 0:
                        satellite = "FY3A";
                        break;
                    case 1:
                        satellite = "FY3B";
                        break;
                    case 2:
                        satellite = "na16";
                        break;
                    case 3:
                        satellite = "na18";
                        break;
                }
                switch (cboSensor.SelectedIndex)
                {
                    case 0:
                        sensor = "MERSI";
                        break;
                    case 1:
                        sensor = "VIRR";
                        break;
                    case 2:
                        sensor = "VIRRX";
                        break;
                    case 3:
                        sensor = "AVHRR";
                        break;
                }
                if (satellite == "na16" || satellite == "na18")
                    txtFilter.Text = "*" + satellite + "*.1BD";
                else
                    txtFilter.Text = "*" + satellite + "_" + sensor + "*.HDF";
            }
        }

        private void btnOutputDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtOutputDir.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void cboSensor_SelectedValueChanged(object sender, EventArgs e)
        {
            string satellite = string.Empty;
            string sensor = string.Empty;
            if (cboSensor.SelectedIndex != -1)
            {
                switch (cboSatellite.SelectedIndex)
                {
                    case 0:
                        satellite = "FY3A";
                        break;
                    case 1:
                        satellite = "FY3B";
                        break;
                    case 2:
                        satellite = "na16";
                        break;
                    case 3:
                        satellite = "na18";
                        break;
                }
                switch (cboSensor.SelectedIndex)
                {
                    case 0:
                        sensor = "MERSI";
                        break;
                    case 1:
                        sensor = "VIRR";
                        break;
                    case 2:
                        sensor = "VIRRX";
                        break;
                    case 3:
                        sensor = "AVHRR";
                        break;
                }
                if (satellite == "na16" || satellite == "na18")
                    txtFilter.Text = "*" + satellite + "*.1BD";
                else
                    txtFilter.Text = "*" + satellite + "_" + sensor + "*.HDF";
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoAllBands.Checked == true)
            {
                txtBands.Enabled = false;
                txtBands.Visible = false;
                rdoCustomBands.Checked = false;
            }
            else
            {
                txtBands.Enabled = true;
                txtBands.Visible = true;
                rdoCustomBands.Checked = true;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoCustomBands.Checked == true)
            {
                txtBands.Enabled = true;
                txtBands.Visible = true;
                rdoAllBands.Checked = false;
            }
            else
            {
                txtBands.Enabled = false;
                txtBands.Visible = false;
                rdoAllBands.Checked = true;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstRegions.SelectedItem != null)
            {
                foreach (PrjEnvelopeItem item in _envList)
                {
                    if (item.Name == lstRegions.SelectedItem.ToString())
                    {
                        _envList.Remove(item);
                        break;
                    }
                }
                lstRegions.Items.Remove(lstRegions.SelectedItem);
            }
        }

        private void btnMoasicOutDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtMoasicOutDir.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnInputFiles_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Multiselect = true;
                dialog.Filter = SupportedFileFilters.HdfFilter + "|" + SupportedFileFilters.NoaaFilter;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (dialog.FileNames.Length > 0)
                    {
                        string fileNames = "";
                        foreach (string name in dialog.FileNames)
                            fileNames += name + ";";
                        txtInputFiles.Text = fileNames;
                    }
                }
            }
        }

        private void btnAddEvp_Click(object sender, EventArgs e)
        {
            string evpName = txtRegionName.Text.Trim();
            if (string.IsNullOrWhiteSpace(evpName))
            {
                MsgBox.ShowInfo("请输入范围标识");
                return;
            }
            foreach (PrjEnvelopeItem item in _envList)
            {
                if (item.Name == evpName)
                {
                    MsgBox.ShowInfo("已存在名为" + evpName + "的区域范围名称，请重新输入！");
                    return;
                }
            }
            RasterProject.PrjEnvelope envelope = GetEnvelopeFromUI();
            if (envelope.Width == 0 || envelope.Height == 0
                ||!CheckRegion(envelope.MinX, envelope.MaxX, -180, 180) 
                ||!CheckRegion(envelope.MinY, envelope.MaxY, -90, 90))
            {
                MsgBox.ShowInfo("请输入正确的地理坐标范围值！");
                return;
            }
            PrjEnvelopeItem env = new PrjEnvelopeItem(txtRegionName.Text, envelope);
            lstRegions.Items.Add(env.Name);
            _envList.Add(env);
        }

        private RasterProject.PrjEnvelope GetEnvelopeFromUI()
        {
            if (radCenter.Checked)
                return geoRegionEditCenter1.GeoEnvelope;
            else
                return new RasterProject.PrjEnvelope(ucOutputRegion.MinX, ucOutputRegion.MaxX,
                        ucOutputRegion.MinY, ucOutputRegion.MaxY);
        }

        private bool CheckRegion(double min, double max, double minLimit, double maxLimit)
        {
            if (min < max)
            {
                if (max <= maxLimit && min >= minLimit)
                {
                    return true;
                }
            }
            return false;
        }

        private void lstRegions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstRegions.SelectedItem == null)
                return;
            string name = lstRegions.SelectedItem.ToString();
            PrjEnvelopeItem p = GetEnvelopeItemByName(name);
            if (p == null)
                return;
            txtRegionName.Text = p.Name;
            SetEnvelopeToUI(p.PrjEnvelope);
        }

        private void SetEnvelopeToUI(GeoDo.RasterProject.PrjEnvelope prjEnvelope)
        {
            if (radCenter.Checked)
            {
                geoRegionEditCenter1.SetGeoEnvelope(prjEnvelope);
            }
            else
            {
                ucOutputRegion.MaxX = prjEnvelope.MaxX;
                ucOutputRegion.MinX = prjEnvelope.MinX;
                ucOutputRegion.MaxY = prjEnvelope.MaxY;
                ucOutputRegion.MinY = prjEnvelope.MinY;
            }
        }


        private PrjEnvelopeItem GetEnvelopeItemByName(string name)
        {
            if (_envList == null || _envList.Count < 1)
                return null;
            foreach (PrjEnvelopeItem item in _envList)
            {
                if (item.Name == name)
                    return item;
            }
            return null;
        }

        private void rdbInputFiles_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbInputFiles.Checked)
            {
                txtInputFiles.Enabled = btnInputFiles.Enabled = true;
                btnInputDir.Enabled = txtInputDir.Enabled = false;
                cboSatellite.Enabled = cboSensor.Enabled = false;
                txtFilter.Enabled = false;
            }
            else
            {
                txtInputFiles.Enabled = btnInputFiles.Enabled = false;
                btnInputDir.Enabled = txtInputDir.Enabled = true;
                cboSatellite.Enabled = cboSensor.Enabled = true;
                txtFilter.Enabled = true;
            }
        }

        private void ckbSameOutDir_CheckedChanged(object sender, EventArgs e)
        {
            if (ckbSameOutDir.Checked)
            {
                txtMoasicOutDir.Enabled = btnMoasicOutDir.Enabled = false;
                txtMoasicOutDir.Text = txtOutputDir.Text;
            }
            else
            {
                txtMoasicOutDir.Enabled = btnMoasicOutDir.Enabled = true;
            }
        }

        private void radCenter_CheckedChanged(object sender, EventArgs e)
        {
            ChangeVisible();
        }

        private void radRect_CheckedChanged(object sender, EventArgs e)
        {
            ChangeVisible();
        }

        private void ChangeVisible()
        {
            if (radCenter.Checked)
            {
                geoRegionEditCenter1.Visible = true;
                ucOutputRegion.Visible = false;
                geoRegionEditCenter1.SetGeoEnvelope(new RasterProject.PrjEnvelope(ucOutputRegion.MinX,ucOutputRegion.MaxX,ucOutputRegion.MinY,ucOutputRegion.MaxY));
            }
            else
            {
                geoRegionEditCenter1.Visible = false;
                ucOutputRegion.Visible = true;
                ucOutputRegion.MinX = geoRegionEditCenter1.GeoEnvelope.MinX;
                ucOutputRegion.MaxX = geoRegionEditCenter1.GeoEnvelope.MaxX;
                ucOutputRegion.MinY = geoRegionEditCenter1.GeoEnvelope.MinY;
                ucOutputRegion.MaxY = geoRegionEditCenter1.GeoEnvelope.MaxY;
            }
        }
    }
}
