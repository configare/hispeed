using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.RasterDrawing;
using System.ComponentModel.Composition;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public partial class frmArgNDVIManager : Form
    {
        private RadTreeView _tree;
        private ErrorProvider _errorProvider = null;
        private List<NDVISettingItem> _settingList = null;

        public List<NDVISettingItem> SettingList
        {
            get { return _settingList; }
            set { _settingList = value; }
        }

        public frmArgNDVIManager()
        {
            InitializeComponent(); 
            InitTreeControl();
            //_errorProvider = new ErrorProvider(this as ContainerControl);
        }

        public void Init(List<NDVISettingItem> settingList, params object[] arguments)
        {
            _settingList = settingList;
            UpdateSession();
        }

        private void UpdateSession()
        {
            if (_settingList != null && _settingList.Count > 0)
            {
                LoadAvgNDVISetted();
            }
        }

        private void InitTreeControl()
        {
            _tree = new RadTreeView();
            _tree.Dock = DockStyle.Fill;
            _tree.ExpandAnimation = ExpandAnimation.Opacity;
            _tree.ExpandMode = ExpandMode.Multiple;
            _tree.LineStyle = TreeLineStyle.Dash;
            _tree.SelectedNodeChanged += new RadTreeView.RadTreeViewEventHandler(_tree_SelectedNodeChanged);
            panel1.Controls.Add(_tree);
        }

        void _tree_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            if (_tree.SelectedNode != null)
            {
                if (_tree.SelectedNode.Tag != null)
                {
                    NDVISettingItem item = _tree.SelectedNode.Tag as NDVISettingItem;
                    if (item != null)
                    {
                        txtName.Text = item.Name;
                        txtAvgMax.Text = item.MaxValue.ToString();
                        txtAvgMin.Text = item.MinValue.ToString();
                        txtMinLon.Text = item.Envelope.MinX.ToString();
                        txtMaxLon.Text = item.Envelope.MaxX.ToString();
                        txtMinLat.Text = item.Envelope.MinY.ToString();
                        txtMaxLat.Text = item.Envelope.MaxY.ToString();
                    }
                }
            }
        }

        private void LoadAvgNDVISetted()
        {
            _tree.Nodes.Clear();
            foreach (NDVISettingItem item in _settingList)
            {
                RadTreeNode node = new RadTreeNode(item.Name);
                node.Tag = item;
                _tree.Nodes.Add(node);
            }
            if (_tree.Nodes.Count != 0)
                _tree.SelectedNode = _tree.Nodes[0];
            _tree.ExpandAll();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadAvgNDVISetted();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            RadTreeNode node = _tree.SelectedNode;
            if (node == null)
                return;
            NDVISettingItem item = node.Tag as NDVISettingItem;
            _settingList.Remove(item);
            _tree.Nodes.Remove(node);
        }

        private void AddNDVISetting(NDVISettingItem item)
        {
            RadTreeNode node = new RadTreeNode(item.Name);
            node.Tag = item;
            _tree.Nodes.Add(node);
            _settingList.Add(item);
        }

        private void btnAddRegion_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            string name = txtName.Text;
            double minx = txtMinLon.Value;
            double maxx = txtMaxLon.Value;
            double miny = txtMinLat.Value;
            double maxy = txtMaxLat.Value;
            RSS.Core.DF.CoordEnvelope env = new RSS.Core.DF.CoordEnvelope(minx, maxx, miny, maxy);
            if (string.IsNullOrWhiteSpace(name))
            {
                errorProvider1.SetError(txtName, "名字不能为空");
                return;
            }
            if (env==null || env.Width <= 0 || env.Height <= 0
                || env.MinX < -180 || env.MaxX > 180
                || env.MinY < -90 || env.MaxY > 90)
            {
                errorProvider1.SetError(panel2, "范围不合法");
                return;
            }
            if (ContainItem(name))
            {
                errorProvider1.SetError(txtName, "已经含有该名字的范围定义");
                return;
            }
            float minValue;
            if (float.TryParse(txtAvgMin.Text, out minValue))
            {
                errorProvider1.SetError(txtAvgMin, "未指定最小端元值");
            }
            float maxValue;
            if (float.TryParse(txtAvgMax.Text, out maxValue))
            {
                errorProvider1.SetError(txtAvgMin, "未指定最大端元值");
            }
            NDVISettingItem item = new NDVISettingItem();
            item.Envelope = env;
            item.Name = name;
            item.MinValue = minValue;
            item.MaxValue = maxValue;
            AddNDVISetting(item);
        }

        private bool ContainItem(string name)
        {
            if (_settingList == null || _settingList.Count == 0)
                return false;
            foreach (NDVISettingItem item in _settingList)
            {
                if (item.Name == name)
                    return true;
            }
            return false;
        }

        private void txtMinLon_Leave(object sender, EventArgs e)
        {
            double minLon;
            if (double.TryParse(txtMinLon.Text, out minLon))
            {
                if (_tree.SelectedNode != null && _tree.SelectedNode.Tag != null)
                {
                    NDVISettingItem item = _tree.SelectedNode.Tag as NDVISettingItem;
                    double minLat = item.Envelope.MinY;
                    double maxLat = item.Envelope.MaxY;
                    double maxLon = item.Envelope.MaxX;
                    item.Envelope = new RSS.Core.DF.CoordEnvelope(minLon, maxLon, minLat, maxLat);
                }
            }
        }

        private void txtMaxLon_Leave(object sender, EventArgs e)
        {
            double maxLon;
            if (double.TryParse(txtMaxLon.Text, out maxLon))
            {
                if (_tree.SelectedNode != null && _tree.SelectedNode.Tag != null)
                {
                    NDVISettingItem item = _tree.SelectedNode.Tag as NDVISettingItem;
                    double minLat = item.Envelope.MinY;
                    double minLon = item.Envelope.MinX;
                    double maxLat = item.Envelope.MaxY;
                    item.Envelope = new RSS.Core.DF.CoordEnvelope(minLon, maxLon, minLat, maxLat);
                }
            }
        }

        private void txtMinLat_Leave(object sender, EventArgs e)
        {
            double minLat;
            if (double.TryParse(txtMinLat.Text, out minLat))
            {
                if (_tree.SelectedNode != null && _tree.SelectedNode.Tag != null)
                {
                    NDVISettingItem item = _tree.SelectedNode.Tag as NDVISettingItem;
                    double maxLat = item.Envelope.MaxY;
                    double minLon = item.Envelope.MinX;
                    double maxLon = item.Envelope.MaxX;
                    item.Envelope = new RSS.Core.DF.CoordEnvelope(minLon, maxLon, minLat, maxLat);
                }
            }
        }

        private void txtMaxLat_Leave(object sender, EventArgs e)
        {
            double maxLat;
            if (double.TryParse(txtMaxLat.Text, out maxLat))
            {
                if (_tree.SelectedNode != null && _tree.SelectedNode.Tag != null)
                {
                    NDVISettingItem item = _tree.SelectedNode.Tag as NDVISettingItem;
                    double minLat = item.Envelope.MinY;
                    double minLon=item.Envelope.MinX;
                    double maxLon=item.Envelope.MaxX;
                    item.Envelope = new RSS.Core.DF.CoordEnvelope(minLon, maxLon, minLat, maxLat);
                }
            }
        }

        private void txtAvgMin_Leave(object sender, EventArgs e)
        {
            float avgMin;
            if (float.TryParse(txtAvgMin.Text, out avgMin))
            {
                if (_tree.SelectedNode != null && _tree.SelectedNode.Tag != null)
                {
                    NDVISettingItem item = _tree.SelectedNode.Tag as NDVISettingItem;
                    item.MinValue = avgMin;
                }
            }
        }

        private void txtAvgMax_Leave(object sender, EventArgs e)
        {
            float avgMax;
            if (float.TryParse(txtAvgMax.Text, out avgMax))
            {
                if (_tree.SelectedNode != null && _tree.SelectedNode.Tag != null)
                {
                    NDVISettingItem item = _tree.SelectedNode.Tag as NDVISettingItem;
                    item.MaxValue = avgMax;
                }
            }
        }
    }
}
