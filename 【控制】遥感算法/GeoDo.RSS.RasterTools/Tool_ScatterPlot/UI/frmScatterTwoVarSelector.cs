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
    public partial class frmScatterTwoVarSelector : Form
    {
        private IRasterDataProvider _XdataProvider=null;
        private bool _isNewXDataProvider = false;
        private IRasterDataProvider _YdataProvider = null;
        private bool _isNewYDataProvider = false;
        private int[] _aoi;
        private string _aoiName = null;

        public frmScatterTwoVarSelector()
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

        public IRasterDataProvider XDataProvider
        {
            get { return _XdataProvider; }
        }
        public bool IsNewXDataProvider
        {
            get { return _isNewXDataProvider; }
        }

        public IRasterDataProvider YDataProvider
        {
            get { return _YdataProvider; }
        }

        public bool IsNewYDataProvider
        {
            get { return _isNewYDataProvider; }
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

        public string  AOIName
        {
            get{ return rdAOI.Checked ?_aoiName:null;}
            set { _aoiName = value; }
        }

        public void Apply(IRasterDataProvider dataProvider,int[] aoi,bool isX=true)
        {
            if (dataProvider == null)
                return;
            //_aoi = aoi;
            //if (_aoiName == null )//|| aoi == null || aoi.Length == 0
            //{
            //    rdAOI.Enabled = false;
            //    rdFullImage.Checked = true;
            //}
            //else
            //{
            //    rdAOI.Checked = true;
            //    rdAOI.Name += ":" + _aoiName;
            //}
            if (isX)
            {
                _XdataProvider = dataProvider;
                _isNewXDataProvider = false;
                txtXFileName.Text = dataProvider.fileName;
                FillBands(dataProvider, tvXBands);
            } 
            else
            {
                _YdataProvider = dataProvider;
                _isNewYDataProvider = false;
                txtYFileName.Text = dataProvider.fileName;
                FillBands(dataProvider, tvYBands);
            }
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
            try
            {
                if (!BandNoIsOK(tvXBands) || !BandNoIsOK(tvYBands))
                    throw new ArgumentException("请正确选择待处理的波段！");
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool BandNoIsOK(TreeView tv)
        {
            if (tv.SelectedNode == null || tv.SelectedNode.Equals(tv.Nodes[0]))
            {
                throw new ArgumentNullException("请选择待运算的波段！");
            }
            return true;
        }

        private IRasterDataProvider TryCreateDataProvider(string fName)
        {
            try
            {
                IRasterDataProvider prd = GeoDataDriver.Open(fName) as IRasterDataProvider;
                if (prd == null)
                    throw new ArgumentNullException(fName+"打开失败！");
                else if (prd.CoordEnvelope == null || prd.CoordEnvelope.Width <= 0 || prd.CoordEnvelope.Height<=0)
                    throw new ArgumentException(fName + "空间范围信息不可用！");
                return prd;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        private void btnSelectYFile_Click(object sender, EventArgs e)
        {
            if (_isNewYDataProvider && _YdataProvider != null)
            {
                txtYFileName.Text = string.Empty;
                _YdataProvider.Dispose();
                _YdataProvider = null;
            }
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "All Smart Supported Files(*.*)|*.*";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _YdataProvider = TryCreateDataProvider(dlg.FileName);
                    if (_YdataProvider != null)
                    {
                        Apply(_YdataProvider, null,false);
                        _isNewYDataProvider = true;
                    }
                }
            }
        }

        private void btnSelectXFile_Click(object sender, EventArgs e)
        {
            if (_isNewXDataProvider && _XdataProvider != null)
            {
                txtXFileName.Text = string.Empty;
                _XdataProvider.Dispose();
                _XdataProvider = null;
            }
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "All Smart Supported Files(*.*)|*.*";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _XdataProvider = TryCreateDataProvider(dlg.FileName);
                    if (_XdataProvider != null)
                    {
                        Apply(_XdataProvider, null,true);
                        _isNewXDataProvider = true;
                    }
                }
            }
        }

        private void frmScatterTwoVarSelector_Load(object sender, EventArgs e)
        {
            if (_aoiName == null)//|| aoi == null || aoi.Length == 0
            {
                rdAOI.Enabled = false;
                rdFullImage.Checked = true;
            }
            else
            {
                rdAOI.Checked = true;
                rdAOI.Text += ":" + _aoiName;
            }
        }
    }
}
