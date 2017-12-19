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
using System.Diagnostics;

namespace GeoDo.ProjectDefine
{
    public partial class SpatialReferenceSelection : Form
    {
        private List<TreeNode> _nodeList = null;   //存放关键词匹配的所有节点（搜索）
        private TreeNode[] _selectNodes = null;
        private int _preIndex = -1;  //存放上一次选择的投影的下标
        private ISpatialReference _spatialReference = null;
        private IGeographicCoordSystem _geoCoordSystem = null;
        private IProjectionCoordSystem _prjCoordSystem = null;
        private bool _isSpatialInfoShow = true;  //默认显示信息显示框
        private ISpatialReference _preSpatialReference = null;

        public SpatialReferenceSelection()
        {
            InitializeComponent();
            Load += new EventHandler(SpatialReferenceSelection_Load);
        }

        public ISpatialReference SpatialReference
        {
            get { return _spatialReference; }
            set { _spatialReference = value; }
        }

        public IGeographicCoordSystem GeoCoordSystem
        {
            get { return _geoCoordSystem; }
            set { _geoCoordSystem = value; }
        }

        public IProjectionCoordSystem PrjCoordSystem
        {
            get { return _prjCoordSystem; }
            set { _prjCoordSystem = value; }
        }

        /// <summary>
        /// 自定义投影坐标
        /// </summary>
        public static ISpatialReference[] CustomSpatialReferences
        {
            get 
            {
                return GetCustomSpatialReferences();
            }
        }

        private static ISpatialReference[] GetCustomSpatialReferences()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "坐标系统\\自定义");
            if (!Directory.Exists(path))
                return null;
            string[] files = Directory.GetFiles(path);
            if (files == null || files.Length == 0)
                return null;
            List<ISpatialReference> spatialReferenceList = new List<ISpatialReference>();
            ISpatialReference spatialReference;
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                spatialReference = SpatialReferenceFactory.GetSpatialReferenceByPrjFile(file);
                if (spatialReference == null)
                    continue;
                spatialReferenceList.Add(spatialReference);
            }
            //将等经纬度的调整为第一个
            List<ISpatialReference> glls = spatialReferenceList.FindAll(
                match =>
                    match.Name.Contains("等经纬度") ||
                    (match.ProjectionCoordSystem == null && match.GeographicsCoordSystem != null)
                    );
            if (glls != null && glls.Count != 0)
            {
                foreach (ISpatialReference gl in glls)
                {
                    spatialReferenceList.Remove(gl);
                }
                spatialReferenceList.InsertRange(0, glls);
            }
            return spatialReferenceList.ToArray();
        }

        #region  Init functions
        void SpatialReferenceSelection_Load(object sender, EventArgs args)
        {
            InitSpatialRefNames();
            this.Text = "选择坐标系统";
            cmdDisplayBlank.ItemHeight = 36;
        }

        private void InitSpatialRefNames()
        {
            TreeNode root = new TreeNode();
            root.Text = @"坐标系统";
            root.Name = root.Text;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "坐标系统");
            if (!Directory.Exists(path))
                return;
            root.Name = path;
            tvSpatialRefNames.Nodes.Add(root);
            BindChild(root);
            tvSpatialRefNames.Nodes[0].Expand();
            tvSpatialRefNames.Nodes[0].Nodes[0].Expand(); //默认展开第一级目录
        }

        /// <summary>
        /// 解析坐标系统文件夹，并添加到树中
        /// </summary>
        /// <param name="fNode"></param>
        private void BindChild(TreeNode fNode)
        {
            string path = fNode.Name;
            if (path == null || !Directory.Exists(path))
                return;
            string[] paths = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);
            SpatialReferenceFile spatialRefFile;
            for (int i = 0; i < paths.Length; i++)
            {
                string subPath = paths[i];
                TreeNode node = new TreeNode();
                node.Name = subPath; 
                spatialRefFile = new SpatialReferenceFile();
                spatialRefFile.IsPrjFile = false;
                node.Tag = spatialRefFile;
                node.ImageIndex = 0;
                node.SelectedImageIndex = 2;
                node.Text = Path.GetFileNameWithoutExtension(subPath);
                fNode.Nodes.Add(node);
                BindChild(node);
            }
            for (int i = 0; i < files.Length; i++)
            {
                string subFile = files[i];
                string fileExt = Path.GetExtension(subFile);
                if (fileExt != ".prj")
                    continue;
                TreeNode node = new TreeNode();
                node.Name = subFile;
                node.Text = Path.GetFileNameWithoutExtension(subFile);
                spatialRefFile = new SpatialReferenceFile();
                spatialRefFile.SpatialReference = SpatialReferenceFactory.GetSpatialReferenceByPrjFile(subFile);
                spatialRefFile.IsPrjFile = true;
                node.Tag = spatialRefFile;
                node.ImageIndex = 1;
                node.SelectedImageIndex = 1;
                fNode.Nodes.Add(node);
            }
        }
        #endregion

        #region  search spatialReference by keywords
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (cmdDisplayBlank.Text == null)
                return;
            cmdDisplayBlank.Items.Clear();
            _nodeList = new List<TreeNode>();
            TreeNodeCollection nodes = tvSpatialRefNames.Nodes[0].Nodes;
            FindNodeInHierarchy(nodes, cmdDisplayBlank.Text);
            if (_nodeList.Count == 0)
            {
                MessageBox.Show("没有与关键字匹配的投影", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            AddToCmdDisplayBlank();
            cmdDisplayBlank.DroppedDown = true;
            cmdDisplayBlank.Focus(); //此时需要让cmdDisplayBlank控件获得焦点
        }

        /// <summary>
        /// 关键字搜索的递归函数
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="strSearchValue">关键字</param>
        private void FindNodeInHierarchy(TreeNodeCollection nodes, string strSearchValue)
        {
            for (int iCount = 0; iCount < nodes.Count; iCount++)
            {
                if (nodes[iCount].Text.ToUpper().Contains(strSearchValue.ToUpper()))
                {
                    _nodeList.Add(nodes[iCount]);
                }
                FindNodeInHierarchy(nodes[iCount].Nodes, strSearchValue);
            }
        }

        /// <summary>
        /// 将搜索的结果添加到空白栏中
        /// </summary>
        private void AddToCmdDisplayBlank()
        {
            if (_nodeList.Count == 0)
                return;
            string path;
            string replacePath;
            foreach (TreeNode node in _nodeList)
            {
                path = node.FullPath.Replace("坐标系统\\", "..");
                replacePath = path.Substring(0, node.FullPath.IndexOf("\\") + 1);
                path = path.Replace(replacePath, "..");
                cmdDisplayBlank.Items.Add(path);
            }
        }

        private void cmdDisplayBlank_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = cmdDisplayBlank.SelectedIndex;
            if (index == -1)
                return;
            _selectNodes = _nodeList.ToArray();
            tvSpatialRefNames.SelectedNode = _selectNodes[index];
            if (_preIndex > -1)
                _selectNodes[_preIndex].BackColor = Color.White;
            tvSpatialRefNames.SelectedNode.BackColor = BackColor;
            _preIndex = index;
        }
        #endregion

        private void tvSpatialRefNames_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = tvSpatialRefNames.SelectedNode;
            ShowSpatialRefInfos(node);
        }

        private void ShowSpatialRefInfos(TreeNode node)
        {
            if (node.Level == 0)
            {
                node.Expand();
                return;
            }
            if ((node.Tag as SpatialReferenceFile).IsPrjFile == false)
            {
                node.Expand();
                return;
            }
            btnModify.Enabled = true;
            txtSpatialRefInfo.Clear();
            string txtCoordinateInfo;
            _spatialReference = (node.Tag as SpatialReferenceFile).SpatialReference;
            txtCoordinateInfo = _spatialReference.ToString();
            txtSpatialRefInfo.Text = txtCoordinateInfo;
        }

        private void btnCreatGeo_Click(object sender, EventArgs e)
        {
            GeoCoordinateDefine geoCoordinateDefine = new GeoCoordinateDefine(_spatialReference, enumControlType.Creat);
            geoCoordinateDefine.Text = "新建地理坐标系";
            TryApplyGeo(geoCoordinateDefine);
            if (geoCoordinateDefine.DialogResult == System.Windows.Forms.DialogResult.OK)
                NeedSaveToFile();
        }

        #region 新建文件，添加到“自定义文件夹中”，并在目录树中添加节点
        private void NeedSaveToFile()
        {
            if (_spatialReference == null)
                return;
            SaveToPrjFile();
            AddToTreeView();
        }

        /// <summary>
        /// 保存为*.prj文件,同时在目录树上添加节点
        /// </summary>
        private void SaveToPrjFile()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"坐标系统\自定义\");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string fileName = _spatialReference.Name + ".prj";
            string filePath = Path.Combine(path, fileName);
            File.WriteAllText(filePath, _spatialReference.ToWKTString(), Encoding.Default);
        }

        /// <summary>
        /// 将新建的空间参考添加到左侧树节点中
        /// </summary>
        private void AddToTreeView()
        {
            SpatialReferenceFile newSpatialRefFile = new SpatialReferenceFile();
            newSpatialRefFile.SpatialReference = _spatialReference;
            newSpatialRefFile.IsPrjFile = true;
            //建立新的节点
            TreeNode newNode = new TreeNode();
            newNode.Tag = newSpatialRefFile;
            newNode.Text = newSpatialRefFile.SpatialReference.Name;
            newNode.ImageIndex = 1;
            newNode.SelectedImageIndex = 1;

            TreeNodeCollection nodes = tvSpatialRefNames.Nodes[0].Nodes;
            AddToSpecificNode(nodes, newNode);
            tvSpatialRefNames.SelectedNode = newNode;
        }

        //自动添加到“自定义”节点下
        private void AddToSpecificNode(TreeNodeCollection nodes, TreeNode newNode)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text == "自定义")
                {
                    node.Nodes.Add(newNode);
                    return;
                }
                else
                {
                    TreeNodeCollection treeNodes = node.Nodes;
                    AddToSpecificNode(treeNodes, newNode);
                }
            }
        }
        #endregion

        private void btnCreatPrj_Click(object sender, EventArgs e)
        {
            PrjCoordinateDefine prjCoordinateDefine = new PrjCoordinateDefine(_spatialReference, enumControlType.Creat);
            prjCoordinateDefine.Text = "新建投影坐标系";
            TryApplyPrj(prjCoordinateDefine);
            if (prjCoordinateDefine.DialogResult == System.Windows.Forms.DialogResult.OK)
                NeedSaveToFile();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            if (_spatialReference == null)
                return;
            _preSpatialReference = _spatialReference;
            if (_spatialReference.ProjectionCoordSystem == null)
            {
                GeoCoordinateDefine geoCoordinateDefine = new GeoCoordinateDefine(_spatialReference, enumControlType.Modify);
                geoCoordinateDefine.Text = "地理坐标系属性";
                TryApplyGeo(geoCoordinateDefine);
                if (geoCoordinateDefine.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    //IsSame()函数中没有对名字进行比较
                    if (!_spatialReference.IsSame(_preSpatialReference) ||
                       (_spatialReference.GeographicsCoordSystem.Name != _preSpatialReference.GeographicsCoordSystem.Name))
                        NeedSaveToFile();
                }
            }
            else
            {
                PrjCoordinateDefine prjCoordinateDefine = new PrjCoordinateDefine(_spatialReference, enumControlType.Modify);
                prjCoordinateDefine.Text = "投影坐标系属性";
                TryApplyPrj(prjCoordinateDefine);
                if (prjCoordinateDefine.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    if (!_spatialReference.IsSame(_preSpatialReference)
                        || (_spatialReference.GeographicsCoordSystem.Name != _preSpatialReference.GeographicsCoordSystem.Name)
                        || (_spatialReference.Name != _preSpatialReference.Name))
                        NeedSaveToFile();
                }
            }
        }

        private void TryApplyGeo(GeoCoordinateDefine geoCoordinateDefine)
        {
            geoCoordinateDefine.ShowDialog();
            if (geoCoordinateDefine.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (geoCoordinateDefine.GeographicCoordSystem != null)
                {
                    _geoCoordSystem = geoCoordinateDefine.GeographicCoordSystem;
                    _spatialReference = new SpatialReference(_geoCoordSystem);
                    txtSpatialRefInfo.Text = _spatialReference.ToString();
                }
            }
        }

        private void TryApplyPrj(PrjCoordinateDefine prjCoordinateDefine)
        {
            prjCoordinateDefine.ShowDialog();
            if (prjCoordinateDefine.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (prjCoordinateDefine.SpatialReference != null)
                {
                    _spatialReference = prjCoordinateDefine.SpatialReference;
                    txtSpatialRefInfo.Text = _spatialReference.ToString();
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.No;
            Close();
        }

        //可用于收起右侧坐标信息显示框
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            _isSpatialInfoShow = !_isSpatialInfoShow;
            if (_isSpatialInfoShow == true)
                FoldedInfoTextBox();  //展开信息提示框
            else
                UnFoldInfoTextBox();  //收起信息提示框
        }

        /// <summary>
        /// 展开信息提示框
        /// </summary>
        private void FoldedInfoTextBox()
        {
            txtSpatialRefInfo.Visible = true;
            lblSpatialRefDisplay.Visible = true;
            this.Width += txtSpatialRefInfo.Width + tvSpatialRefNames.Left;
            tvSpatialRefNames.Width = this.Width - tvSpatialRefNames.Left * 3 - txtSpatialRefInfo.Width - pctFold.Width;
            pctFold.Left = tvSpatialRefNames.Width + tvSpatialRefNames.Left;
            txtSpatialRefInfo.Left = pctFold.Left + pctFold.Width;
            lblSpatialRefDisplay.Left = txtSpatialRefInfo.Left;
            pctFold.Image = GeoDo.ProjectDefine.Properties.Resources.arrow_down_9x10;
        }

        /// <summary>
        /// 收起信息提示框
        /// </summary>
        private void UnFoldInfoTextBox()
        {
            txtSpatialRefInfo.Visible = false;
            lblSpatialRefDisplay.Visible = false;
            this.Width -= txtSpatialRefInfo.Width;

            tvSpatialRefNames.Width = this.Width - tvSpatialRefNames.Left * 2 - pctFold.Width;
            cmdDisplayBlank.Width = tvSpatialRefNames.Width - btnSearch.Width - tvSpatialRefNames.Left;
            pctFold.Left = tvSpatialRefNames.Width + tvSpatialRefNames.Left;
            pctFold.Image = GeoDo.ProjectDefine.Properties.Resources.arrow_right_9x10;
            btnSearch.Left = cmdDisplayBlank.Width + cmdDisplayBlank.Left * 2;
        }

        private void cmdDisplayBlank_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch_Click(null, null);
                this.Cursor = Cursors.Arrow;
                cmdDisplayBlank.DroppedDown = true;
            }
        }

        private void tvSpatialRefNames_DoubleClick(object sender, EventArgs e)
        {
            btnModify_Click(null, null);
        }
    }
}
