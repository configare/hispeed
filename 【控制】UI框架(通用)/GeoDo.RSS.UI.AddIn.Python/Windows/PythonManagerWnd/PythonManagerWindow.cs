using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Python
{
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")] //指定DLL的输出接口以便MEF模块进行查询
    public partial class PythonManagerWindow : ToolWindow, ISmartToolWindow, IPythonManagerWindow, ISmartViewer
    {
        private int _id = 20001;
        private ISmartSession _session = null;
        private string workspace;
        TreeView tree;
        ImageList imglist;
        string title;
        EventHandler _onWindowClosed;

        public PythonManagerWindow()
        {
            InitializeComponent();
            workspace = null;
            imglist = new ImageList();
        }

        private void init_manager_wnd()
        {
            imglist.Images.Add(_session.UIFrameworkHelper.GetImage("system:gear.png"));
            imglist.Images.Add(_session.UIFrameworkHelper.GetImage("system:document-library.png"));
            Text = "脚本管理器";
            tree = new TreeView();
            tree.Text = "test";
            tree.Dock = DockStyle.Fill;
            tree.ImageList = imglist;
            Controls.Add(tree);
            TreeNode root = new TreeNode();
            root.Text = "工作区";
            root.ImageIndex = 1;
            root.SelectedImageIndex = 1;
            GetFiles(workspace, root);
            tree.Nodes.Add(root);
            tree.MouseDoubleClick += new MouseEventHandler(tv_dblk);
            tree.ExpandAll();
        }

        public void RefreshWorkspace()
        {
            TreeNode root = new TreeNode();
            root.Text = "工作区";
            root.ImageIndex = 1;
            root.SelectedImageIndex = 1;
            GetFiles(workspace, root);
            tree.Nodes.Clear();
            tree.Nodes.Add(root);
            tree.Refresh();
            tree.ExpandAll();
        }

        public void tv_dblk(object sender, MouseEventArgs e)
        {
            string s;
            s = tree.SelectedNode.Text;
            if (s.Length <= 3) return;
            if (s.Substring(s.Length - 3, 3).ToUpper() == ".PY")
            {
                _session.CommandEnvironment.Get(20003).Execute("FILL|FILL|" + get_node_path(tree.SelectedNode));
            }
        }
        //上溯合成文件路径
        public string get_node_path(TreeNode tr)
        {
            return tr.Tag as string;
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;
        }

        public int Id
        {
            get { return _id; }
        }

        public EventHandler OnWindowClosed
        {
            get { return _onWindowClosed; }
            set { _onWindowClosed = value; }
        }

        public void SetupWorkspace(string wp)
        {
            workspace = wp;
            init_manager_wnd();
        }

        private void GetFiles(string filePath, TreeNode node)
        {
            if (filePath == null) return;
            DirectoryInfo folder = new DirectoryInfo(filePath);
            node.Text = folder.Name;
            node.Tag = folder.FullName;
            FileInfo[] chldFiles = folder.GetFiles("*.*");
            foreach (FileInfo chlFile in chldFiles)
            {
                TreeNode chldNode = new TreeNode();
                chldNode.Text = chlFile.Name;
                chldNode.Tag = chlFile.FullName;
                if ((chldNode.Text.Length <= 3) || (chldNode.Text.Substring(chldNode.Text.Length - 3, 3).ToUpper() == ".PY"))
                {
                    node.Nodes.Add(chldNode);
                }
            }

            DirectoryInfo[] chldFolders = folder.GetDirectories();
            foreach (DirectoryInfo chldFolder in chldFolders)
            {
                TreeNode chldNode = new TreeNode();
                chldNode.Text = folder.Name;
                chldNode.Tag = folder.FullName;
                node.Nodes.Add(chldNode);
                GetFiles(chldFolder.FullName, chldNode);
            }
        }

        public string Title
        {
            get { throw new NotImplementedException(); }
        }

        public object ActiveObject
        {
            get { throw new NotImplementedException(); }
        }

        public TreeView GetTree()
        {
            return tree;
        }

        public void CloseWnd()
        {
        }

        public void DisposeViewer()
        { }
    }
}
