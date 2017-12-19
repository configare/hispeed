using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.RasterTools
{
    public partial class frmDataProviderSelector : Form
    {
        private IRasterDataProvider _dataProvider;
        private bool _isNewDataProvider = false;

        public frmDataProviderSelector()
        {
            InitializeComponent();
        }

        public frmDataProviderSelector(bool hasAOi)
        {
            InitializeComponent();
            if (hasAOi)
            {
                chkApplyAoi.Checked = true;
            }
            else
            {
                chkApplyAoi.Visible = false;
            }
        }

        public int[] BandNos
        {
            get 
            {
                List<int> bandNos = new List<int>();
                foreach (ListViewItem it in lvBands.Items)
                {
                    if (it.Checked)
                    {
                        int bNo = (int)it.Tag;
                        bandNos.Add(bNo);
                    }
                }
                return bandNos.Count > 0 ? bandNos.ToArray() : null;
            }
        }

        public bool ApplyAoi
        {
            get
            {
                return chkApplyAoi.Checked;
            }
        }

        public IRasterDataProvider DataProvider
        {
            get { return _dataProvider; }
        }

        public bool IsNewDataProvider
        {
            get { return _isNewDataProvider; }
        }

        public void SetDataProvider(IRasterDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            _isNewDataProvider = false;
            txtFileName.Text = _dataProvider.fileName;
            FillBandNos();
        }

        public void SetSelectedBands(int[] bandNos)
        {
            foreach (ListViewItem it in lvBands.Items)
            {
                int bandNo = int.Parse(it.Tag.ToString());
                foreach (int b in bandNos)
                {
                    if (b == bandNo)
                    {
                        it.Checked = true;
                        break;
                    }
                }
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
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
                        FillBandNos();
                    }
                }
            }
        }

        private void FillBandNos()
        {
            lvBands.Items.Clear();
            if (_dataProvider == null)
                return;
            for (int i = 0; i < _dataProvider.BandCount; i++)
            {
                ListViewItem it = new ListViewItem("Band " + (i + 1).ToString());
                it.Tag = i + 1;
                lvBands.Items.Add(it);
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lvBands.CheckedIndices.Count == 0)
                return;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem it in lvBands.Items)
                it.Checked = true;
        }

        private void btnUnselectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem it in lvBands.Items)
                it.Checked = false;
        }

        private void btnRSelect_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem it in lvBands.Items)
                it.Checked = !it.Checked;
        }
    }
}
