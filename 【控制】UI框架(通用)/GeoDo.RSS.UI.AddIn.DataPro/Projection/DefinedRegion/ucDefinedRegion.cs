using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using GeoDo.FileProject;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class ucDefinedRegion : UserControl
    {
        private RadTreeView _tree;
        private RadTreeNode m_SelectedNode = null;
        private BlockDefined _blockDefinedSelected = new BlockDefined(null);

        public event Action<PrjEnvelopeItem[]> CheckedChanged;

        public ucDefinedRegion()
        {
            InitializeComponent();
            InitTreeView();
            LoadDefinedRegion();
        }

        private void InitTreeView()
        {
            _tree = new RadTreeView();
            _tree.Dock = DockStyle.Fill;
            _tree.TriStateMode = true;
            _tree.AutoCheckChildNodes = true;
            _tree.NodeCheckedChanged += new RadTreeView.TreeViewEventHandler(_tree_NodeCheckedChanged);
            _tree.SelectedNodeChanged += new RadTreeView.RadTreeViewEventHandler(_tree_SelectedNodeChanged);
            this.Controls.Add(_tree);
        }

        void _tree_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            m_SelectedNode = e.Node;
        }
        
        public bool CheckBoxes
        {
            set { _tree.CheckBoxes = value; }
            get { return _tree.CheckBoxes; }
        }

        public PrjEnvelopeItem[] CheckedItem
        {
            get
            {
                return _blockDefinedSelected.GetPrjEnvelopeItems();
            }
        }

        /// <summary>
        /// BlockItemGroup或者PrjEnvelopeItem
        /// </summary>
        public object SelectedItem
        {
            get
            {
                return _tree.SelectedNode.Tag;
            }
        }

        private void LoadDefinedRegion()
        {
            _tree.Nodes.Clear();
            DefinedRegionParse defineRegion = new DefinedRegionParse();
            BlockDefined blockDefined = defineRegion.BlockDefined;
            if (blockDefined == null || blockDefined.BlockItemGroups == null || blockDefined.BlockItemGroups.Length == 0)
                return;
            foreach (BlockItemGroup group in blockDefined.BlockItemGroups)
            {
                PrjEnvelopeItem[] envs = group.BlockItems;
                RadTreeNode node = new RadTreeNode(string.Format("{0}[{1}]({2})", group.Name, group.Description, envs.Length));
                node.Tag = group;
                node.CheckType = CheckType.CheckBox;
                foreach (PrjEnvelopeItem env in envs)
                {
                    RadTreeNode item = new RadTreeNode(env.Name);
                    item.CheckType = CheckType.CheckBox;
                    item.Tag = env;
                    node.Nodes.Add(item);
                }
                _tree.Nodes.Add(node);
            }
        }

        void _tree_NodeCheckedChanged(object sender, RadTreeViewEventArgs e)
        {
            if (e.Node != m_SelectedNode)
                return;
            RadTreeNode rootNode = null;
            BlockItemGroup group = null;
            RadTreeNodeCollection nodes = null;
            if (e.Node.Level == 0)
            {
                rootNode = e.Node;
                group = (e.Node.Tag as BlockItemGroup);
                nodes = e.Node.Nodes;
            }
            else if (e.Node.Level == 1)
            {
                rootNode = e.Node.Parent;
                group = (e.Node.Parent.Tag as BlockItemGroup);
                nodes = e.Node.Parent.Nodes;
            }
            List<PrjEnvelopeItem> checkedNodes = new List<PrjEnvelopeItem>();
            int i = 0;
            foreach (RadTreeNode node in rootNode.Nodes)
            {
                if (node.Checked)
                {
                    checkedNodes.Add(node.Tag as PrjEnvelopeItem);
                    i++;
                }
            }
            BlockItemGroup bg = new BlockItemGroup(group.Name, group.Description, checkedNodes.ToArray());
            _blockDefinedSelected.Add(bg);
            rootNode.Text = string.Format("{0}[{1}]({3}/{2})", group.Name, group.Description, group.BlockItems.Length, i);
            if (CheckedChanged != null)
                CheckedChanged(_blockDefinedSelected.GetPrjEnvelopeItems());
        }
    }
}
