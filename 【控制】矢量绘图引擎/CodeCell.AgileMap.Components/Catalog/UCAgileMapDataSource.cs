using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.AgileMap.Components
{
    public enum enumCatalogItemType
    { 
        Location,
        FeatureClass,
        Unknow
    }
    public delegate void OnSelectedCatalogItemChangedHandler(object sender,ICatalogItem catalogItem);
    public delegate void OnCatalogItemDoubleClickedHandler(object sender,ICatalogItem catalogItem);

    public partial class UCAgileMapDataSource : UserControl, ICatalogItemRefresh
    {
        private enumCatalogItemType _selectableType = enumCatalogItemType.FeatureClass;
        public event OnSelectedCatalogItemChangedHandler OnSelectedCatalogItemChanged = null;
        public event OnCatalogItemDoubleClickedHandler OnCatalogItemDoubleClicked = null;
        protected TreeNode _preNode = null;
        protected ICatalogItem _selectedCatalogItem = null;

        public UCAgileMapDataSource()
        {
            InitializeComponent();
            InitCatalogs();
            AttachEvents();
        }

        internal enumCatalogItemType SelectableType
        {
            get { return _selectableType; }
            set { _selectableType = value; }
        }

        internal ICatalogItem SelectedCatalogItem
        {
            get { return _selectedCatalogItem; }
        }

        private void AttachEvents()
        {
            treeView1.MouseUp += new MouseEventHandler(treeView1_MouseUp);
            treeView1.MouseDown += new MouseEventHandler(treeView1_MouseDown);
            treeView1.MouseDoubleClick += new MouseEventHandler(treeView1_MouseDoubleClick);
        }

        void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode tn = treeView1.SelectedNode;
                if (tn == null)
                    return;
                ICatalogItem c = tn.Tag as ICatalogItem;
                ContextOprItem[] oprs = c.ContextOprItems;
                if (oprs == null || oprs.Length == 0)
                    return;
                contextMenuStrip1.Items.Clear();
                foreach (ContextOprItem op in oprs)
                {
                    if (op == null)
                        contextMenuStrip1.Items.Add(new ToolStripSeparator());
                    else
                    {
                        ToolStripMenuItem it = new ToolStripMenuItem(op.Name, op.Image);
                        it.Tag = new object[] { op, c };
                        it.Click += new EventHandler(it_Click);
                        contextMenuStrip1.Items.Add(it);
                    }
                }
                contextMenuStrip1.Show(treeView1, e.Location);
            }
        }

        void it_Click(object sender, EventArgs e)
        {
            object[] objs = (sender as ToolStripMenuItem).Tag as object[];
            ICatalogItem c = objs[1] as ICatalogItem;
            ContextOprItem opr = objs[0] as ContextOprItem;
            c.Click(opr.EnumKey);
        }

        void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TreeNode tn = treeView1.GetNodeAt(e.Location);
            if (tn == null || tn.Tag == null)
                return;
            enumCatalogItemType cType = GetCatalogItemType(tn.Tag as ICatalogItem);
            if (cType == _selectableType)
            {
                _selectedCatalogItem = tn.Tag as ICatalogItem;
                if (OnCatalogItemDoubleClicked != null)
                {
                    OnCatalogItemDoubleClicked(this, tn.Tag as ICatalogItem);
                }
            }
        }

        private enumCatalogItemType GetCatalogItemType(ICatalogItem catalogItem)
        {
            if (catalogItem is CatalogLocal ||
                catalogItem is CatalogDatabaseConn || 
                catalogItem is CatalogFeatureDataset || 
                catalogItem is CatalogNetInstance)
                return enumCatalogItemType.Location;
            else if (catalogItem is CatalogFeatureClass ||
                catalogItem is CatalogFile || catalogItem is CatalogNetFeatureClass)
                return enumCatalogItemType.FeatureClass;
            return enumCatalogItemType.Unknow;
        }

        void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            _selectedCatalogItem = null;
            TreeNode tn = treeView1.GetNodeAt(e.Location);
            treeView1.SelectedNode = tn;
            if (tn != null && tn.Tag != null)
            {
                (tn.Tag as CatalogItem).LoadChildren();
                if (_preNode == null || !_preNode.Equals(tn))
                {
                    enumCatalogItemType cType = GetCatalogItemType(tn.Tag as ICatalogItem);
                    if (cType == _selectableType)
                    {
                        _selectedCatalogItem = (tn.Tag as ICatalogItem);
                        if (OnSelectedCatalogItemChanged != null)
                            OnSelectedCatalogItemChanged(this, tn.Tag as ICatalogItem);
                    }
                }
                _preNode = tn;
            }
        }

        private void InitCatalogs()
        {
            if (Application.StartupPath.Contains("IDE"))
                return;
            CatalogRoot root = new CatalogRoot();
            InitCatalogs(root, null);
        }

        private void InitCatalogs(ICatalogItem c, TreeNode pNode)
        {
            TreeNode cNode = new TreeNode(c.Name);
            Image img = c.Image;
            if (img != null)
            {
                bool existed = false;
                int idx = 0;
                foreach (Image ig in imageList1.Images)
                {
                    if (ig.Equals(img))
                    {
                        existed = true;
                        break;
                    }
                    idx++;
                }
                if (!existed)
                {
                    imageList1.Images.Add(img);
                    idx = imageList1.Images.Count - 1;
                }
                cNode.ImageIndex = idx;
                cNode.SelectedImageIndex = idx;
            }
            cNode.Tag = c;
            (c as CatalogItem).SetCatalogItemRefresh(this);
            if (pNode == null)
                treeView1.Nodes.Add(cNode);
            else
                pNode.Nodes.Add(cNode);
            if (c.ChildCount > 0)
                foreach (ICatalogItem h in c.Children)
                    InitCatalogs(h, cNode);
        }

        private void FindTreeNode(ICatalogItem c, TreeNode pNode, ref TreeNode tn)
        {
            if (pNode.Tag != null && pNode.Tag.Equals(c))
            {
                tn = pNode;
                return;
            }
            if (pNode.Nodes.Count > 0)
                foreach (TreeNode cNode in pNode.Nodes)
                    FindTreeNode(c, cNode, ref tn);
        }

        public void Refresh(ICatalogItem c)
        {
            TreeNode tn = null;
            FindTreeNode(c, treeView1.Nodes[0], ref tn);
            if (tn != null)
            {
                tn.Nodes.Clear();
                if (c.ChildCount > 0)
                {
                    foreach (ICatalogItem subc in c.Children)
                    {
                        InitCatalogs(subc, tn);
                    }
                }
            }
        }
    }
}
