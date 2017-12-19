using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.RasterTools
{
    public partial class frmContourSettings : Form
    {
        public class ContourItem
        {
            public double ContourValue;
            public Color Color;
        }

        public delegate void OnStatMinMaxValueHander(object sender, int bandNo, out double minValue, out double maxValue);

        private IRasterDataProvider _dataProvider;
        private bool _isNewDataProvider = false;
        private int[] _aoi;
        private Rectangle _aoiRect;
        private const int MIN_SIZE_LIMIT = 10;

        public frmContourSettings()
        {
            InitializeComponent();
            ckNeedOutputShp_CheckedChanged(null, null);
        }

        public IRasterDataProvider DataProvider
        {
            get { return _dataProvider; }
        }

        public int Sample
        {
            get { return int.Parse(txtSamples.Text); }
        }

        public int BandNo
        {
            get
            {
                return txtBandNos.SelectedIndex + 1;
            }
        }

        public bool IsNewDataProvider
        {
            get { return _isNewDataProvider; }
        }

        public int[] AOI
        {
            get
            {
                return rdAOI.Checked ? _aoi : null;
            }
        }

        public bool IsNeedDisplay
        {
            get { return ckIsNeedDisplay.Checked; }
        }

        public bool IsNeedFillColor
        {
            get { return ckNeedFillColor.Checked; }
        }

        public bool IsNeedLabel
        {
            get { return ckNeedLabel.Checked; }
        }

        public string ShpFileName
        {
            get { return ckNeedOutputShp.Checked ? txtSaveAs.Text : null; }
        }

        public ContourItem[] ContourValues
        {
            get
            {
                if (lvValues.Items.Count == 0)
                    return null;
                ContourItem[] vs = new ContourItem[lvValues.Items.Count];
                for (int i = 0; i < vs.Length; i++)
                {
                    ContourItem it = new ContourItem();
                    object[] args = lvValues.Items[i].Tag as object[];
                    it.ContourValue = (double)args[0];
                    it.Color = (Color)args[1];
                    vs[i] = it;
                }
                return vs;
            }
        }

        public void Apply(IRasterDataProvider dataProvider, int bandNo, int[] aoi)
        {
            if (dataProvider == null)
                return;
            _aoi = aoi;
            _aoiRect = AOIHelper.ComputeAOIRect(aoi, new Size(dataProvider.Width, dataProvider.Height));
            _dataProvider = dataProvider;
            _isNewDataProvider = false;
            txtFileName.Text = _dataProvider.fileName;
            FillBandNos(dataProvider, bandNo);
            TrySetShpFile();
            //
            if (aoi == null || aoi.Length == 0)
            {
                rdAOI.Enabled = false;
                rdFullImage.Checked = true;
            }
            else
            {
                rdAOI.Checked = true;
                txtSamples.SelectedIndex = 0;
            }
        }

        private void FillBandNos(IRasterDataProvider dataProvider, int bandNo)
        {
            int bandCount = dataProvider.BandCount;
            for (int i = 1; i <= bandCount; i++)
                txtBandNos.Items.Add(i.ToString());
            txtBandNos.SelectedIndex = bandNo - 1;
            TrySetDefaultSample();
        }

        private void TrySetDefaultSample()
        {
            int size = 0;
            if(_aoi == null)
                size = Math.Max(_dataProvider.Width, _dataProvider.Height);
            else
                size = Math.Max(_aoiRect.Width, _aoiRect.Height);
            if (size <= MIN_SIZE_LIMIT)
                return;
            txtSamples.Items.Clear();
            int samples = size / MIN_SIZE_LIMIT;
            for (int i = 1; i <= samples; i++)
                txtSamples.Items.Add(i);
            txtSamples.SelectedIndex = txtSamples.Items.Count - 1;
        }

        private void TrySetShpFile()
        {
            if (string.IsNullOrEmpty(txtFileName.Text))
                return;
            string fname = Path.Combine(Path.GetDirectoryName(txtFileName.Text),
                Path.GetFileNameWithoutExtension(txtFileName.Text) + "_Contour.shp");
            txtSaveAs.Text = fname;
        }

        private void rdRanges_CheckedChanged(object sender, EventArgs e)
        {
            if (rdRanges.Checked)
            {
                lbMaxValue.Visible = lbSpan.Visible = true;
                txtMaxValue.Visible = txtSpan.Visible = true;
                lbMinValue.Text = "最小值";
            }
            else
            {
                lbMaxValue.Visible = lbSpan.Visible = false;
                txtMaxValue.Visible = txtSpan.Visible = false;
                lbMinValue.Text = "输入值";
            }
        }

        private void rdSingleValue_CheckedChanged(object sender, EventArgs e)
        {
            if (rdSingleValue.Checked)
            {
                lbMaxValue.Visible = lbSpan.Visible = false;
                txtMaxValue.Visible = txtSpan.Visible = false;
                lbMinValue.Text = "输入值";
                txtMinValue.Focus();
                txtMinValue.SelectAll();
            }
            else
            {
                lbMaxValue.Visible = lbSpan.Visible = true;
                txtMaxValue.Visible = txtSpan.Visible = true;
                lbMinValue.Text = "最小值";
            }
        }

        private void btnAddValue_Click(object sender, EventArgs e)
        {
            if (rdRanges.Checked)
            {
                AddValueByRange();
            }
            else
            {
                AddSingleValue();
            }
        }

        private void AddSingleValue()
        {
            double v = 0;
            if (double.TryParse(txtMinValue.Text, out v))
            {
                lvValues.Items.Add(ToListView(v));
            }
            else
            {
                txtMinValue.Focus();
                txtMinValue.SelectAll();
            }
        }

        private void AddValueByRange()
        {
            double v1, v2, v3;
            if (!double.TryParse(txtMinValue.Text, out v1))
            {
                txtMinValue.Focus();
                txtMinValue.SelectAll();
                return;
            }
            if (!double.TryParse(txtMaxValue.Text, out v2))
            {
                txtMaxValue.Focus();
                txtMaxValue.SelectAll();
                return;
            }
            if (!double.TryParse(txtSpan.Text, out v3))
            {
                txtSpan.Focus();
                txtSpan.SelectAll();
                return;
            }
            while (v1 < v2)
            {
                lvValues.Items.Add(ToListView(v1));
                v1 += v3;
            }
            if (v1 != v2)
                lvValues.Items.Add(ToListView(v2));
        }

        private ListViewItem ToListView(double v1)
        {
            ListViewItem it = new ListViewItem(v1.ToString());
            Color c = GetNextColor();
            it.SubItems.Add(c.ToString());
            it.Tag = new object[] { v1, c };
            it.BackColor = c;
            return it;
        }

        Random _random = new Random();
        private Color GetNextColor()
        {
            byte[] rgb = new byte[3];
            _random.NextBytes(rgb);
            return Color.FromArgb(rgb[0], rgb[1], rgb[2]);
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "ESRI Shape Files(*.shp)|*.shp";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtSaveAs.Text = dlg.FileName;
                }
            }
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            if (_isNewDataProvider && _dataProvider != null)
            {
                txtFileName.Text = string.Empty;
                _dataProvider.Dispose();
                _dataProvider = null;
            }
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "All Smart Supported Files(*.*)|*.*";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _dataProvider = TryCreateDataProvider(dlg.FileName);
                    if (_dataProvider != null)
                    {
                        _isNewDataProvider = true;
                        txtFileName.Text = dlg.FileName;
                        Apply(_dataProvider, 1, null);
                        _isNewDataProvider = true;
                    }
                }
            }
        }

        private IRasterDataProvider TryCreateDataProvider(string fName)
        {
            try
            {
                IRasterDataProvider prd = GeoDataDriver.Open(fName) as IRasterDataProvider;
                return prd;
            }
            catch (Exception ex)
            {
                MessageBox.Show("系统消息", ex.Message, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                return null;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_dataProvider == null)
            {
                MessageBox.Show("要生成等直线的栅格文件不能为空！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (lvValues.Items.Count == 0)
            {
                MessageBox.Show("等值线级别不能为空！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!ckIsNeedDisplay.Checked && !ckNeedOutputShp.Checked)
            {
                MessageBox.Show("是否显示和是否输出矢量必须勾选一项！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void lvValues_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem it = lvValues.GetItemAt(e.X, e.Y);
            if (it == null)
                return;
            using (ColorDialog dlg = new ColorDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    object[] args = it.Tag as object[];
                    args[1] = dlg.Color;
                    it.SubItems[1].Text = args[1].ToString();
                    it.BackColor = (Color)args[1];
                }
            }
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            lvValues.Items.Clear();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvValues.SelectedIndices.Count == 0)
                return;
            for (int i = lvValues.SelectedIndices.Count - 1; i >= 0; i--)
                lvValues.Items.RemoveAt(lvValues.SelectedIndices[i]);
        }

        private void ckNeedOutputShp_CheckedChanged(object sender, EventArgs e)
        {
            if (ckNeedOutputShp.Checked)
                this.Height = 437;
            else
                this.Height = 374;
        }

        private void btnStat_Click(object sender, EventArgs e)
        {
            if (_dataProvider == null)
                return;
            try
            {
                IRasterBand band = _dataProvider.GetRasterBand(BandNo);
                double minValue, maxValue;
                string str = "正在统计({0}%)...";
                band.ComputeMinMax(out minValue, out maxValue, false,
                    (pro, tip) =>
                    {
                        btnStat.Text = string.Format(str, pro);
                        btnStat.Refresh();
                    });
                txtMinValue.Text = minValue.ToString("0.####");
                txtMaxValue.Text = maxValue.ToString("0.####");
            }
            finally
            {
                btnStat.Text = "统计";
            }
        }

        private void lvValues_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtMinValue_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void txtMinValue_KeyDown(object sender, KeyEventArgs e)
        {
            if (rdSingleValue.Checked)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAddValue_Click(null, null);
                    txtMinValue.Focus();
                    txtMinValue.Text = string.Empty;
                }
            }
        }

        private void txtSamples_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_dataProvider == null)
                return;
            int sample = this.Sample;
            int w = 0;
            int h = 0;
            if (_aoi != null && _aoi.Length != 0)
            {
                h = _aoiRect.Height / sample;
                w = _aoiRect.Width / sample;
            }
            else
            {
                h = _dataProvider.Height / sample;
                w = _dataProvider.Width / sample;
            }

            if (w <= MIN_SIZE_LIMIT || h <= MIN_SIZE_LIMIT)
            {
                return;
            }
            lbRasterSize.Text = string.Format("({0} x {1})", w, h);
        }

        private void rdAOI_CheckedChanged(object sender, EventArgs e)
        {
            if (rdAOI.Checked)
            {
                if (txtSamples.Items.Count == 0)
                    txtSamples.Items.Add(1);
                txtSamples.SelectedIndex = 0;//1
            }
        }

        private void rdFullImage_CheckedChanged(object sender, EventArgs e)
        {
            if (rdFullImage.Checked)
            {
                txtSamples.Enabled = true;
            }
        }
    }
}
