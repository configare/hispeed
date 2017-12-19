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

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class frmDefinedRegionManager : Form
    {
        private RadTreeView _tree;
        private BlockDefined _blockRef = null;
        private ErrorProvider _errorProvider = null;
        private List<BlockItemGroup> _groups = null;

        public frmDefinedRegionManager()
        {
            InitializeComponent();
            InitTreeControl();
            LoadUserDefinedRegion();
            _errorProvider = new ErrorProvider(this);
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
            RadTreeNode node = e.Node;
            if (node == null)
                return;
            if (node.Level == 1)
            {
                PrjEnvelopeItem item = node.Tag as PrjEnvelopeItem;
                if (item != null)
                {
                    textBox1.Text = item.Name;
                    txtIdentify.Text = item.Identify;
                    SetEnvelopeToUI(item.PrjEnvelope);
                }
            }
        }

        private void LoadUserDefinedRegion()
        {
            _groups = new List<BlockItemGroup>();
            _tree.Nodes.Clear();
            DefinedRegionParse p = new DefinedRegionParse();
            _blockRef = p.BlockDefined;
            BlockItemGroup[] blockGroup = _blockRef.UserDefineRegion;
            if (blockGroup == null || blockGroup.Length == 0)
                return;
            for (int i = 0; i < blockGroup.Length; i++)
            {
                BlockItemGroup group = blockGroup[i];
                _groups.Add(group);
                AddGroupToTree(group);
            }
            if (_tree.Nodes.Count > 0)
            {
                _tree.SelectedNode = _tree.Nodes[_tree.Nodes.Count - 1];
                _tree.SelectedNode.ExpandAll();
            }
            //_tree.ExpandAll();
        }

        private void AddGroupToTree(BlockItemGroup group)
        {
            RadTreeNode node = new RadTreeNode(group.ToString());
            node.Tag = group;
            PrjEnvelopeItem[] items = group.BlockItems;
            if (items.Length > 0)
            {
                foreach (PrjEnvelopeItem item in items)
                {
                    AddRegionToNode(node, item);
                }
            }
            _tree.Nodes.Add(node);
            _tree.SelectedNode = node;
        }

        private static void AddRegionToNode(RadTreeNode node, PrjEnvelopeItem env)
        {
            RadTreeNode item = new RadTreeNode(env.ToString());
            item.Tag = env;
            node.Nodes.Add(item);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveUserDefineRegion();
            this.Close();
        }

        private void SaveUserDefineRegion()
        {
            _blockRef.SetUserDefineRegion(_groups.ToArray());
            DefinedRegionParse.Save(_blockRef);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadUserDefinedRegion();
        }

        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            using (frmDefinedRegionGroupName frm = new frmDefinedRegionGroupName())
            {
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string groupname = frm.GroupName;
                    string groupIdentify = frm.GroupIdentify;
                    AddGroup(groupname, groupIdentify);
                }
            }
        }

        private void AddGroup(string groupname, string groupIdentify)
        {
            BlockItemGroup group = new BlockItemGroup(groupname, "", groupIdentify);
            _groups.Add(group);
            AddGroupToTree(group);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            RadTreeNode node = _tree.SelectedNode;
            if (node == null)
                return;
            if (node.Level == 0)
            {
                _groups.Remove(node.Tag as BlockItemGroup);
                _tree.Nodes.Remove(node);
            }
            else if (node.Level == 1)
            {
                PrjEnvelopeItem item = node.Tag as PrjEnvelopeItem;
                BlockItemGroup group = node.Parent.Tag as BlockItemGroup;
                group.Remove(item);
                node.Parent.Nodes.Remove(node);
            }
        }

        private void AddDefinedRegion(PrjEnvelopeItem item)
        {
            RadTreeNode node = _tree.SelectedNode;
            if (node == null)
            {
                return;
            }
            if (node.Level == 0)
            {
                BlockItemGroup group = node.Tag as BlockItemGroup;
                group.Add(item);
                AddRegionToNode(node, item);
            }
            else if (node.Level == 1)
            {
                BlockItemGroup group = node.Parent.Tag as BlockItemGroup;
                group.Add(item);
                AddRegionToNode(node.Parent, item);
            }
        }

        private BlockItemGroup GetSelectedGroup()
        {
            RadTreeNode node = _tree.SelectedNode;
            if (node == null)
            {
                return null;
            }
            if (node.Level == 0)
            {
                BlockItemGroup group = node.Tag as BlockItemGroup;
                return group;
            }
            else if (node.Level == 1)
            {
                BlockItemGroup group = node.Parent.Tag as BlockItemGroup;
                return group;
            }
            return null;
        }

        private void btnAddRegion_Click(object sender, EventArgs e)
        {
            errorProvider1.Clear();
            string name = textBox1.Text;
            string identify = txtIdentify.Text;
            PrjEnvelope env = GetEnvelopeFromUI();

            if (string.IsNullOrWhiteSpace(name))
            {
                errorProvider1.SetError(textBox1, "名字不能为空");
                return;
            }
            if (env.IsEmpty || env.Width <= 0 || env.Height <= 0)
            {
                errorProvider1.SetError(radCenter, "范围不合法,不能为空或零");
                return;
            }
            if (env.MinX < -180d)
            {
                errorProvider1.SetError(radCenter, "范围不合法,最小经度不能小于-180");
                return;
            }
            if (env.MaxX > 180d)
            {
                errorProvider1.SetError(radCenter, "范围不合法,最大经度不能大于180");
                return;
            }
            if (env.MinY < -90d)
            {
                errorProvider1.SetError(radCenter, "范围不合法,最小纬度不能小于-90");
                return;
            }
            if (env.MaxY > 90d)
            {
                errorProvider1.SetError(radCenter, "范围不合法,最大纬度不能大于90");
                return;
            }
            BlockItemGroup group = GetSelectedGroup();
            if (group == null)
            {
                errorProvider1.SetError(panel1, "请选择或者添加一个分组");
                return;
            }
            if (group.GetPrjEnvelopeItem(identify) != null)
            {
                errorProvider1.SetError(textBox1, "标识不能重复");
                return;
            }
            PrjEnvelopeItem item = new PrjEnvelopeItem(name, env, identify);
            AddDefinedRegion(item);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveUserDefineRegion();
        }

        #region 范围定义编辑

        private void radRect_CheckedChanged(object sender, EventArgs e)
        {
            ChangeVisible();
        }

        private void radCenter_CheckedChanged(object sender, EventArgs e)
        {
            ChangeVisible();
        }

        private void ChangeVisible()
        {
            if (radCenter.Checked)
            {
                geoRegionEditCenter1.Visible = true;
                geoRange1.Visible = false;
                geoRegionEditCenter1.SetGeoEnvelope(new RasterProject.PrjEnvelope(geoRange1.MinX, geoRange1.MaxX, geoRange1.MinY, geoRange1.MaxY));
            }
            else
            {
                geoRegionEditCenter1.Visible = false;
                geoRange1.Visible = true;
                geoRange1.MinX = geoRegionEditCenter1.GeoEnvelope.MinX;
                geoRange1.MaxX = geoRegionEditCenter1.GeoEnvelope.MaxX;
                geoRange1.MinY = geoRegionEditCenter1.GeoEnvelope.MinY;
                geoRange1.MaxY = geoRegionEditCenter1.GeoEnvelope.MaxY;
            }
        }

        private void SetEnvelopeToUI(GeoDo.RasterProject.PrjEnvelope prjEnvelope)
        {
            if (radCenter.Checked)
            {
                geoRegionEditCenter1.SetGeoEnvelope(prjEnvelope);
            }
            else
            {
                geoRange1.MaxX = prjEnvelope.MaxX;
                geoRange1.MinX = prjEnvelope.MinX;
                geoRange1.MaxY = prjEnvelope.MaxY;
                geoRange1.MinY = prjEnvelope.MinY;
            }
        }

        private RasterProject.PrjEnvelope GetEnvelopeFromUI()
        {
            if (radCenter.Checked)
                return geoRegionEditCenter1.GeoEnvelope;
            else
                return new RasterProject.PrjEnvelope(geoRange1.MinX, geoRange1.MaxX,
                        geoRange1.MinY, geoRange1.MaxY);
        }
        #endregion

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            txtIdentify.Text = Chinese2PY.GetPYString(textBox1.Text);
        }
    }
}
