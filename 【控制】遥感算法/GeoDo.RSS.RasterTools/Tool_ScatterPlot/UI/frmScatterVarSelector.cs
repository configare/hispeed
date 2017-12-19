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
using GeoDo.MathAlg;

namespace GeoDo.RSS.RasterTools
{
    public partial class frmScatterVarSelector : Form
    {
        private IRasterDataProvider _dataProvider;
        private bool _isNewDataProvider = false;
        private int[] _aoi;

        public frmScatterVarSelector()
        {
            InitializeComponent();
        }

        public int XBandNo
        {
            get 
            {
                if (tvXBands.SelectedNode == null || tvXBands.SelectedNode.Equals(tvXBands.Nodes[0]))
                    return -1;
                return (int)tvXBands.SelectedNode.Tag;
            }
        }

        public int YBandNo
        {
            get
            {
                if (tvYBands.SelectedNode == null || tvYBands.SelectedNode.Equals(tvYBands.Nodes[0]))
                    return -1;
                return (int)tvYBands.SelectedNode.Tag;
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

        public LinearFitObject FitObj
        {
            get 
            {
                return ckNeedFit.Checked ? new LinearFitObject() : null;
            }
        }

        public int[] AOI
        {
            get 
            {
                return rdAOI.Checked ? _aoi : null; 
            }
        }

        public void Apply(IRasterDataProvider dataProvider,int[] aoi)
        {
            if (dataProvider == null)
                return;
            _aoi = aoi;
            if (aoi == null || aoi.Length == 0)
            {
                rdAOI.Enabled = false;
                rdFullImage.Checked = true;
            }
            else
            {
                rdAOI.Checked = true;
            }
            _dataProvider = dataProvider;
            _isNewDataProvider = false;
            txtFileName.Text = _dataProvider.fileName;
            FillBands(dataProvider, tvXBands);
            FillBands(dataProvider, tvYBands);
        }

        private void FillBands(IRasterDataProvider dataProvider, TreeView tv)
        {
            TreeNode root = new TreeNode(Path.GetFileName(dataProvider.fileName));
            for (int i = 1; i <= dataProvider.BandCount; i++)
            {
                TreeNode bNode = new TreeNode("波段 " + i.ToString());
                bNode.Tag = i;
                root.Nodes.Add(bNode);
            }
            tv.Nodes.Add(root);
            root.ExpandAll();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!BandNoIsOK(tvXBands) || !BandNoIsOK(tvYBands))
                return;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private bool BandNoIsOK(TreeView tv)
        {
            if (tv.SelectedNode == null || tv.SelectedNode.Equals(tv.Nodes[0]))
                return false;
            return true;
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
                        Apply(_dataProvider,null);
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

        private void tvXBands_MouseClick(object sender, MouseEventArgs e)
        {
            TreeNode tn = tvXBands.GetNodeAt(e.Location);
            if (tn == null || tn.Tag == null)
                return;
            groupBox1.Text = "X 轴波段 - [Band "+tn.Tag.ToString()+"]";
        }

        private void tvYBands_MouseClick(object sender, MouseEventArgs e)
        {
            TreeNode tn = tvYBands.GetNodeAt(e.Location);
            if (tn == null)
                return;
            groupBox2.Text = "Y 轴波段 - [Band " + tn.Tag.ToString() + "]";
        }
    }
}
