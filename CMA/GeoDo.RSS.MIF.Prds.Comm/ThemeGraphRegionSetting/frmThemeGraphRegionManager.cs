using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using GeoDo.FileProject;
using GeoDo.RasterProject;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.RasterDrawing;
using System.ComponentModel.Composition;
using GeoDo.RSS.UI.AddIn.Windows;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    //[Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public partial class frmThemeGraphRegionManager : Form  /*ToolWindowBase, ISmartToolWindow*/
    {
        private RadTreeView _tree;
        private ErrorProvider _errorProvider = null;
        private ThemeGraphRegion _region = null;
        List<PrjEnvelopeItem> _items = new List<PrjEnvelopeItem>();
        private ISmartSession _session;

        public frmThemeGraphRegionManager()
        {
            //_id = 19020;
            InitializeComponent(); 
            InitTreeControl();
            _errorProvider = new ErrorProvider(this as ContainerControl);
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;
            UpdateSession();
        }

        private void UpdateSession()
        {
            string productIdentify = (_session.MonitoringSession as IMonitoringSession).ActiveMonitoringProduct.Identify;
            ThemeGraphRegion newRegion = ThemeGraphRegionSetting.GetThemeGraphRegion(productIdentify);
            _region = newRegion;

            if (_region.PrjEnvelopeItems != null && _region.PrjEnvelopeItems.Length != 0)
                _items = new List<PrjEnvelopeItem>(_region.PrjEnvelopeItems);
            LoadUserDefinedRegion();
        }

        private void InitTreeControl()
        {
            _tree = new RadTreeView();
            _tree.Dock = DockStyle.Fill;
            _tree.ExpandAnimation = ExpandAnimation.Opacity;
            _tree.ExpandMode = ExpandMode.Multiple;
            _tree.LineStyle = TreeLineStyle.Dash;
            panel1.Controls.Add(_tree);
        }

        private void LoadUserDefinedRegion()
        {
            _tree.Nodes.Clear();
            _region = ThemeGraphRegionSetting.GetThemeGraphRegion(_region.ProductIdentify);
            PrjEnvelopeItem[] items = _region.PrjEnvelopeItems;
            if (_region.PrjEnvelopeItems != null && _region.PrjEnvelopeItems.Length != 0)
                _items = new List<PrjEnvelopeItem>(_region.PrjEnvelopeItems);
            foreach (PrjEnvelopeItem item in _items)
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
            SaveRegion();
            //this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void SaveRegion()
        {
            _region.PrjEnvelopeItems = _items.ToArray();
            ThemeGraphRegionSetting.SaveThemeGraphRegion(_region);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadUserDefinedRegion();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            RadTreeNode node = _tree.SelectedNode;
            if (node == null)
                return;
            PrjEnvelopeItem item = node.Tag as PrjEnvelopeItem;
            _items.Remove(item);
            _tree.Nodes.Remove(node);
        }

        private void AddDefinedRegion(PrjEnvelopeItem item)
        {
            RadTreeNode node = new RadTreeNode(item.Name);
            node.Tag = item;
            _tree.Nodes.Add(node);
            _items.Add(item);
        }

        private void btnAddRegion_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            string name = textBox1.Text;
            double minx = doubleTextBox1.Value;
            double maxx = doubleTextBox2.Value;
            double miny = doubleTextBox3.Value;
            double maxy = doubleTextBox4.Value;
            PrjEnvelope env = new RasterProject.PrjEnvelope(minx, maxx, miny, maxy);
            if (string.IsNullOrWhiteSpace(name))
            {
                errorProvider1.SetError(textBox1, "名字不能为空");
                return;
            }
            if (env.IsEmpty || env.Width <= 0 || env.Height <= 0
                || env.MinX < -180 || env.MaxX > 180
                || env.MinY < -90 || env.MaxY > 90)
            {
                errorProvider1.SetError(panel2, "范围不合法");
                return;
            }
            if (ContainItem(name))
            {
                errorProvider1.SetError(textBox1, "已经含有该名字的范围定义");
                return;
            }
            PrjEnvelopeItem item = new PrjEnvelopeItem(name, env);
            AddDefinedRegion(item);
        }

        private bool ContainItem(string name)
        {
            if (_items == null || _items.Count == 0)
                return false;
            foreach (PrjEnvelopeItem item in _items)
            {
                if (item.Name == name)
                    return true;
            }
            return false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveRegion();
        }

        private void btnGetAoiRegion_Click(object sender, EventArgs e)
        {
            GetAoiRegion();
        }

        private void GetAoiRegion()
        {
            if (_session.SmartWindowManager.ActiveCanvasViewer == null)
                return;
            AOIItem[] aoiItems = _session.SmartWindowManager.ActiveCanvasViewer.AOIProvider.GetAOIItems();
            if (aoiItems == null || aoiItems.Length == 0)
                return;
            CoordEnvelope env = aoiItems[0].GeoEnvelope;
            string aoiName = TryGetRegionName();

            textBox1.Text = string.IsNullOrWhiteSpace(aoiName) ? "AOI" : aoiName;
            doubleTextBox1.Value = env.MinX;
            doubleTextBox2.Value = env.MaxX;
            doubleTextBox3.Value = env.MinY;
            doubleTextBox4.Value = env.MaxY;
        }

        private string TryGetRegionName()
        {
            string aoiName = "";
            IRasterDrawing drawing = _session.SmartWindowManager.ActiveCanvasViewer.ActiveObject as IRasterDrawing;
            if (!string.IsNullOrWhiteSpace(drawing.FileName))
            {
                RasterIdentify ri = new RasterIdentify(drawing.FileName);
                aoiName = ri.RegionIdentify;
            }
            return aoiName;
        }
    }
}
