using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.AgileMap.Core
{
    public partial class frmFeatureRendererEditor : Form, IFeatureRendererProvider
    {
        private IFeatureLayer _vectorFeatureLayer = null;
        private IFeaureRenderEditorControl _currentEditorControl = null;

        public frmFeatureRendererEditor()
        {
            InitializeComponent();
            InitRendererCatagories();
        }

        public frmFeatureRendererEditor(IFeatureLayer vectorFeatureLayer)
        {
            _vectorFeatureLayer = vectorFeatureLayer;
            InitializeComponent();
            InitRendererCatagories();
        }

        private void InitRendererCatagories()
        {
            List<FeatureRendererCategory> roots = new List<FeatureRendererCategory>();
            FeatureRendererCategory c1 = new FeatureRendererCategory("按要素着色");
            FeatureRendererCategory c2 = new FeatureRendererCategory("按类别着色");
            //FeatureRendererCategory c3 = new FeatureRendererCategory("按梯度着色");
            //FeatureRendererCategory c4 = new FeatureRendererCategory("按图形着色");
            roots.Add(c1);
            roots.Add(c2);
            //roots.Add(c3);
            //roots.Add(c4);
            c1.Add(new FeatureRendererCategorySimple("简单着色器"));
            c1.Add(new FeatureRendererCategoryComposite("组合着色器"));
            c1.Add(new FeatureRendererCategorySimpleTwoStep("两阶段着色器"));
            c2.Add(new FeatureRendererCategoryUniqueValue("唯一值着色器"));

            foreach(FeatureRendererCategory c in roots)
            {
                TreeNode tn = new TreeNode(c.Name);
                tn.Tag = c ;
                treeView1.Nodes.Add(tn);
                AddFeatureCategoryToTreeView(tn,c);
            }
            treeView1.MouseClick += new MouseEventHandler(treeView1_MouseClick);
        }

        private void AddFeatureCategoryToTreeView(TreeNode parent, FeatureRendererCategory c)
        {
            if (c.Children != null)
            {
                foreach (FeatureRendererCategory sub in c.Children)
                {
                    TreeNode tn = new TreeNode(sub.Name);
                    tn.Tag = sub;
                    parent.Nodes.Add(tn);
                    AddFeatureCategoryToTreeView(tn, sub);
                }
            }
        }

        #region IFeatureRendererProvider Members

        public IFeatureRenderer Renderer
        {
            get
            {
                return _currentEditorControl != null ? _currentEditorControl.Renderer : null;
            }
        }

        #endregion

        void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            TreeNode tn = treeView1.GetNodeAt(e.Location);
            if (tn == null)
                return;
            FeatureRendererCategory c = tn.Tag as FeatureRendererCategory;
            if (c == null && c.Children == null)
                return;
            foreach (Control ctr in panel1.Controls)
                ctr.Visible = false;
            Control ctr1 = null;
            if (c != null && c.Children == null)
            {
                ctr1 = GetControlByFeatureCategory(c);
                if (ctr1 != null)
                    panel1.Controls.Add(ctr1);
            }
            //else if (c != null && c.Children != null)
            //{
            //    ctr1 = GetControlByFeatureCategory(c.Children[0]);
            //    if (ctr1 != null)
            //        panel1.Controls.Add(ctr1);
            //}
        }

        private Control GetControlByFeatureCategory(FeatureRendererCategory c)
        {
            foreach (Control ctr in panel1.Controls)
            {
                if (ctr.Tag != null && ctr.Tag.Equals(c))
                {
                    ctr.Visible = true;
                    _currentEditorControl = ctr as IFeaureRenderEditorControl;
                    return ctr;
                }
            }
            Control cctr = c.GetFeaureRenderEditorControl() as Control;
            if (cctr == null)
                return null;
            cctr.Tag = c;
            (cctr as UCFeatureRendererControlBase).Layer = _vectorFeatureLayer;
            cctr.Dock = DockStyle.Fill;
            _currentEditorControl = cctr as IFeaureRenderEditorControl;
            return cctr;
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
    }
}
