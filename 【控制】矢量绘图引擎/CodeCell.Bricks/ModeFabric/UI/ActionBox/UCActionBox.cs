using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace CodeCell.Bricks.ModelFabric
{
    public partial class UCActionBox : UserControl,IComparer
    {
        private ActionInfo[] _actionInfos = null;
        private ActionInfo _dragActionInfo = null;

        public UCActionBox()
        {
            InitializeComponent();
            tvActions.MouseDown += new MouseEventHandler(tvActions_MouseDown);
            tvActions.MouseMove += new MouseEventHandler(tvActions_MouseMove);
            tvActions.MouseUp += new MouseEventHandler(tvActions_MouseUp);
            lvResult.MouseDown += new MouseEventHandler(lvResult_MouseDown);
            lvResult.MouseMove += new MouseEventHandler(lvResult_MouseMove);
            lvResult.MouseUp += new MouseEventHandler(lvResult_MouseUp);
            lstIndex.MouseDown += new MouseEventHandler(lstIndex_MouseDown);
            lstIndex.MouseMove += new MouseEventHandler(lstIndex_MouseMove);
            lstIndex.MouseUp += new MouseEventHandler(lstIndex_MouseUp);
        }

        void lstIndex_MouseUp(object sender, MouseEventArgs e)
        {
            _dragActionInfo = null;
        }

        void lstIndex_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_dragActionInfo != null)
                    lstIndex.DoDragDrop(new DataObject("ActionInfo", _dragActionInfo),
                        DragDropEffects.Copy);
            }
        }

        void lstIndex_MouseDown(object sender, MouseEventArgs e)
        {
            if (lstIndex.SelectedItem == null)
                return;
            if (e.Button == MouseButtons.Left)
            {
                _dragActionInfo = lstIndex.SelectedItem as ActionInfo;
            }
        }

        void lvResult_MouseUp(object sender, MouseEventArgs e)
        {
            _dragActionInfo = null;
        }

        void lvResult_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_dragActionInfo != null)
                    lvResult.DoDragDrop(new DataObject("ActionInfo", _dragActionInfo),DragDropEffects.Copy);
            }
        }

        void lvResult_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ListViewItem it = lvResult.GetItemAt(e.X, e.Y);
                if (it == null)
                    return;
                _dragActionInfo = it.Tag as ActionInfo;
            }
        }

        void tvActions_MouseUp(object sender, MouseEventArgs e)
        {
            _dragActionInfo = null;
        }

        void tvActions_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_dragActionInfo != null)
                    tvActions.DoDragDrop(new DataObject("ActionInfo", _dragActionInfo),
                        DragDropEffects.Copy);
            }
        }

        void tvActions_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TreeNode tn = tvActions.GetNodeAt(e.X, e.Y);
                if (tn == null)
                    return;
                _dragActionInfo =tn.Tag as ActionInfo;
            }
        }

        public void SetActionInfos(ActionInfo[] actionInfos)
        {
            _actionInfos = actionInfos;
            LoadActionInfos();
        }

        private void LoadActionInfos()
        {
            if (_actionInfos == null || _actionInfos.Length == 0)
                return;
            LoadActionInfosIntoCatalog();
            LoadActionInfosIntoIndex();
        }

        private void LoadActionInfosIntoIndex()
        {
            Array.Sort(_actionInfos,this);
            foreach (ActionInfo info in _actionInfos)
            {
                lstIndex.Items.Add(info);
            }
        }

        private void LoadActionInfosIntoCatalog()
        {
            TreeNode rootNode = new TreeNode("所有工具");
            rootNode.SelectedImageIndex = rootNode.ImageIndex = 0;
            tvActions.Nodes.Add(rootNode);
            //
            Dictionary<string, TreeNode> catalogs = new Dictionary<string, TreeNode>();
            foreach (ActionInfo info in _actionInfos)
            {
                TreeNode cNode = null;
                if (catalogs.ContainsKey(info.ActionAttribute.Category))
                {
                    cNode = catalogs[info.ActionAttribute.Category];
                }
                else
                {
                    cNode = new TreeNode(info.ActionAttribute.Category);
                    cNode.SelectedImageIndex = rootNode.ImageIndex = 0;
                    rootNode.Nodes.Add(cNode);
                    catalogs.Add(info.ActionAttribute.Category, cNode);
                }
                TreeNode aNode = new TreeNode(info.ActionAttribute.Name);
                aNode.ImageIndex = aNode.SelectedImageIndex = 1;
                aNode.Tag = info;
                cNode.Nodes.Add(aNode);
            }
        }

        #region IComparer 成员

        public int Compare(object x, object y)
        {
            ActionInfo a = x as ActionInfo;
            ActionInfo b = y as ActionInfo;
            return string.Compare(a.ActionAttribute.Name, b.ActionAttribute.Name);
        }

        #endregion

        private void btnSearch_Click(object sender, EventArgs e)
        {
            lvResult.Items.Clear();
            if (string.IsNullOrEmpty(txtKeyWord.Text) || _actionInfos == null || _actionInfos.Length ==0)
                return;
            string key = txtKeyWord.Text;
            if (key.Trim() == ",")
                return;
            if(key.Contains(","))
                key = key.Replace(","," ");
            string[] keys = key.Split(' ');
            foreach (ActionInfo act in _actionInfos)
            {
                if (IsContainsKey(act.ActionAttribute.Name, keys) || IsContainsKey(act.ActionAttribute.Description, keys))
                {
                    ListViewItem it = new ListViewItem(act.ActionAttribute.Name);
                    it.SubItems.Add(act.ActionAttribute.Category);
                    it.SubItems.Add(act.ActionAttribute.Description);
                    it.ImageIndex = 1;
                    it.Tag = act;
                    lvResult.Items.Add(it);
                }
            }
        }

        private bool IsContainsKey(string v, string[] keys)
        {
            v = v.ToUpper();
            foreach (string key in keys)
            {
                if (!v.Contains(key.ToUpper()))
                    return false;
            }
            return true;
        }

        private void txtKeyWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch_Click(null, null);
            }
        }

        private void lvResult_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem it = lvResult.GetItemAt(e.Location.X, e.Location.Y);
            if (it == null)
                return;
            DirectExecute(it.Tag as ActionInfo);
        }

        private void tvActions_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TreeNode tn = tvActions.GetNodeAt(e.Location);
            if (tn == null || tn.Tag == null)
                return;
            DirectExecute(tn.Tag as ActionInfo);
        }

        private void lstIndex_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstIndex.SelectedIndices.Count == 0)
                return;
            DirectExecute(lstIndex.SelectedItem as ActionInfo);
        }

        private void DirectExecute(ActionInfo actionInfo)
        {
            using (frmPropertyEditorDialog frm = new frmPropertyEditorDialog())
            {
                IPropertyEditorDialog dlg = frm as IPropertyEditorDialog;
                IAction action = actionInfo.ToAction();
                bool isOk = dlg.ShowDialog(action, null);
                if (isOk)
                {
                    using (frmActionExecutor exefrm = new frmActionExecutor())
                    {
                        IActionExecutor exe = exefrm as IActionExecutor;
                        exe.Queue(action);
                        exefrm.ShowDialog();
                    }
                }
            }
        }
    }
}
