using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.Project;

namespace GeoDo.ProjectDefine
{
    public partial class GeoSystemTree : Form
    {
        private IGeographicCoordSystem _currentGeoCoordSystem = null;

        public GeoSystemTree()
        {
            InitializeComponent();
            InitSpatialRefTree();
            this.Text = "选择地理坐标系统";
        }

        public IGeographicCoordSystem CurrentGeoCoordSystem
        {
            get
            {
                return _currentGeoCoordSystem;
            }
        }

        private void InitSpatialRefTree()
        {
            TreeNode root = new TreeNode();
            root.Text = @"地理坐标系统";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "坐标系统\\预定义\\地理坐标系统");
            root.Name = path;
            SpatialReferenceFile spatialRefFile = new SpatialReferenceFile();
            spatialRefFile.IsPrjFile = false;
            root.Tag = spatialRefFile;
            tvSpatialRefTree.Nodes.Add(root);
            BindChild(root);
            tvSpatialRefTree.Nodes[0].Expand();
        }

        private void BindChild(TreeNode fNode)
        {
            string path = fNode.Name;
            if (path == null||!Directory.Exists(path))
                return;
            DirectoryInfo fDir = new DirectoryInfo(path);
            FileSystemInfo[] finfos = fDir.GetFileSystemInfos();
            SpatialReferenceFile spatialRefFile;
            foreach (FileSystemInfo f in finfos)
            {
                spatialRefFile = new SpatialReferenceFile();
                string type = f.GetType().ToString();
                TreeNode node = new TreeNode();
                node.Name = f.FullName;  //将文件的完整路径保存在节点的名字中
                if ("System.IO.DirectoryInfo" == type)   //是文件夹才递归调用自己
                {
                    spatialRefFile.IsPrjFile = false;
                    node.Tag = spatialRefFile;
                    node.ImageIndex = 0;
                    node.SelectedImageIndex = 2;
                    node.Text = f.Name;  //将文件的名字保存在节点的文本显示中
                    fNode.Nodes.Add(node);
                    BindChild(node);
                }
                else
                {
                    if (!f.Name.Contains(".prj"))
                        continue;
                    if (f.Name.Substring(f.Name.LastIndexOf('.')) != ".prj")
                        continue;
                    node.Text = f.Name.Remove(f.Name.LastIndexOf('.'));
                    spatialRefFile.SpatialReference = SpatialReferenceFactory.GetSpatialReferenceByPrjFile(f.FullName);   //是否能成功解析prj文件
                    spatialRefFile.IsPrjFile = true;
                    node.Tag = spatialRefFile;
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                    fNode.Nodes.Add(node);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!(tvSpatialRefTree.SelectedNode.Tag as SpatialReferenceFile).IsPrjFile)
                return;
            if ((tvSpatialRefTree.SelectedNode.Tag as SpatialReferenceFile).SpatialReference.GeographicsCoordSystem == null)
                return;
            _currentGeoCoordSystem = (tvSpatialRefTree.SelectedNode.Tag as SpatialReferenceFile).SpatialReference.GeographicsCoordSystem;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tvSpatialRefTree_DoubleClick(object sender, EventArgs e)
        {
            btnOK_Click(null,null);
        }
    }
}
