using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using Telerik.WinControls.UI;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace GeoDo.RSS.MIF.UI
{
    public class CatalogTreeView : Catalog
    {
        private RadTreeView _treeView;
        private Font _font;
        private Dictionary<string, RadTreeNode> _catalogNodes = new Dictionary<string, RadTreeNode>();

        public CatalogTreeView(IWorkspace wks, CatalogDef catalogDef,
            RadPageViewItemPage uiPage, RadTreeView treeView, Font font)
            : base(wks, catalogDef, uiPage)
        {
            _font = font;
            _treeView = treeView;
            //LoadTodayExtractResult(MifEnvironment.OrbitDateTime);
            LoadTodayExtractResult(DateTime.Now);
            _treeView.MouseUp += new System.Windows.Forms.MouseEventHandler(_treeView_MouseUp);
            _treeView.NodeExpandedChanged += new RadTreeView.TreeViewEventHandler(_treeView_NodeExpandedChanged);
        }

        void _treeView_NodeExpandedChanged(object sender, RadTreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count != 0)
            {
                if (e.Node.Expanded)
                    e.Node.Image = GetImge("Open");
                else
                    e.Node.Image = GetImge("Category");
            }
        }

        void _treeView_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                RadTreeNode it = _treeView.SelectedNode;
                if (it == null || it.Tag == null || string.IsNullOrEmpty(it.Tag.ToString()))
                    return;
                _currentCatalogItem = it.Tag as CatalogItem;
                ContentMenuStrip.Show(_treeView, e.Location);
            }
        }

        private void LoadTodayExtractResult(DateTime dt)
        {
            _catalogNodes.Clear();
            _treeView.Nodes.Clear();
            string dir = Path.Combine(MifEnvironment.GetWorkspaceDir(), _wks.Definition.Identify);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string dateDir = Path.Combine(dir, dt.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(dateDir))
                Directory.CreateDirectory(dateDir);
            RadTreeNode rootNode = new RadTreeNode((new DirectoryInfo(dateDir)).Name);
            rootNode.Font = _font;
            _treeView.Nodes.Add(rootNode);
            ExtractingCatalogDef c = _definition as ExtractingCatalogDef;
            foreach (CatalogNodeItemDef it in (c.NodeDefs[0] as CatalogNodeGroupDef).NodeDefs)
            {
                LoadFiles(it, rootNode, dateDir);
            }
            rootNode.ExpandAll();
        }

        private void LoadFiles(CatalogNodeItemDef it, RadTreeNode parentNode, string dateDir)
        {
            if (it is CatalogNodeDef)
            {
                RadTreeNode xtn = new RadTreeNode(it.Text);
                xtn.Font = _font;
                parentNode.Nodes.Add(xtn);
                return;
            }
            CatalogNodeGroupDef group = it as CatalogNodeGroupDef;
            //by chennan 20120821 只显示当前判识相关的结果信息
            //string[] files = GetFiles(dateDir, group.Identify, group.Pattern, true);
            RadTreeNode crtNode = new RadTreeNode(group.Text);
            if (!_catalogNodes.ContainsKey(group.Identify))
            {
                AddMultiIdentigy(group.Identify, crtNode);
            }
            crtNode.Font = _font;
            //by chennan 20120821 只显示当前判识相关的结果信息
            crtNode.Image = GetImge("Open");
            parentNode.Nodes.Add(crtNode);
            //if (files != null && files.Length > 0)
            //{
            //    foreach (string f in files)
            //    {
            //        //by chennan 20120814 今日监测分析面板中文显示
            //        //RadTreeNode node = new RadTreeNode(Path.GetFileName(item.FileName));
            //        RadTreeNode t = new RadTreeNode(GetCatalogCN(f));
            //        t.Tag = f;
            //        t.ToolTipText = Path.GetFileName(f);
            //        t.Image = GetImge(Path.GetExtension(f).ToUpper());
            //        crtNode.Nodes.Add(t);
            //    }
            //}
            //
            if (group.NodeDefs != null && group.NodeDefs.Count > 0)
            {
                foreach (CatalogNodeItemDef sub in group.NodeDefs)
                {
                    LoadFiles(sub, crtNode, dateDir);
                }
            }
        }

        private Image GetImge(string fileExt)
        {
            if (_wks is UCWorkspace)
            {
                UCWorkspace uw = _wks as UCWorkspace;
                switch (fileExt)
                {
                    case ".DAT":
                        return uw.workspaceImages.Images["DBLVFile.png"];
                    case ".XLSX":
                    case ".XLS":
                        return uw.workspaceImages.Images["ExcelFile.png"];
                    case ".GXD":
                        return uw.workspaceImages.Images["GXDFile.png"];
                    case "Category":
                        return uw.workspaceImages.Images["Category.png"];
                    case "Open":
                        return uw.workspaceImages.Images["cmdOpen.png"];
                    default:
                        return uw.workspaceImages.Images["DBLVFile.png"];
                }
            }
            return null;
        }

        private string GetCatalogCN(string filename)
        {
            CatalogItem ci = new CatalogItem(filename, null, null);
            string result = ci.Info.GetPropertyValue("CatalogItemCN");
            string extInfo = string.Empty;
            if (result.IndexOf("(") == -1)
            {
                RasterIdentify ri = new RasterIdentify(filename);
                if (!string.IsNullOrEmpty(ri.CYCFlag))
                    result = ci.Info.GetPropertyValue("CycFlagCN") + "_" + result;
                extInfo = (string.IsNullOrEmpty(ri.RegionIdentify) ? "" : ri.RegionIdentify + "_") + ri.OrbitDateTime.ToString("yyyyMMdd HH:mm") +
                    (string.IsNullOrEmpty(ri.ExtInfos) ? "" : "_" + ri.ExtInfos);
            }
            if (string.IsNullOrEmpty(extInfo))
                return result;
            else if (extInfo.EndsWith("_"))
                extInfo = extInfo.Substring(0, extInfo.Length - 1);
            return result + "(" + extInfo + ")";

        }

        private void AddMultiIdentigy(string identify, RadTreeNode crtNode)
        {
            string[] identifys = identify.Split(new char[] { ',' });
            if (identify == null || identify.Length == 0)
                _catalogNodes.Add(identify, crtNode);
            else
                foreach (string item in identifys)
                {
                    if (!_catalogNodes.ContainsKey(item))
                        _catalogNodes.Add(item, crtNode);
                }
        }

        public override string[] GetSelectedFiles()
        {
            if (_treeView.SelectedNode == null)
                return null;
            string fname = _treeView.SelectedNode.Tag != null ? _treeView.SelectedNode.Tag.ToString() : null;
            if (string.IsNullOrEmpty(fname))
                return null;
            return new string[] { fname };
        }

        public override ICatalogItem[] GetSelectedItems()
        {
            if (_treeView.SelectedNode == null)
                return null;
            string fname = _treeView.SelectedNode.Tag != null ? _treeView.SelectedNode.Tag.ToString() : null;
            if (string.IsNullOrEmpty(fname))
                return null;
            return new ICatalogItem[] { new CatalogItem(fname, _definition as SubProductCatalogDef) };
        }

        public override void AddItem(ICatalogItem item)
        {
            if (item == null || string.IsNullOrEmpty(item.FileName))
                return;
            //by chennan 20120814 今日监测分析面板中文显示
            //RadTreeNode[] nodes = _treeView.FindNodes(Path.GetFileName(item.FileName));
            RadTreeNode[] nodes = FindNodes(Path.GetFileName(item.FileName));
            string identify = null;
            if (item.Info.Properties.ContainsKey("SubProductIdentify"))
            {
                object v = item.Info.Properties["SubProductIdentify"];
                identify = v != null ? v.ToString() : null;
            }
            if (nodes != null && nodes.Length != 0)
            {
                AddOtherCatalog(identify, item);
                return;
            }
            AddNodeByCatalogItem(item, identify);
        }

        private void AddNodeByCatalogItem(ICatalogItem item, string identify)
        {
            if (string.IsNullOrEmpty(identify))
                return;
            if (_catalogNodes.ContainsKey(identify))
            {
                //by chennan 20120814 今日监测分析面板中文显示
                //RadTreeNode node = new RadTreeNode(Path.GetFileName(item.FileName));
                RadTreeNode node = new RadTreeNode(GetCatalogCN(item.FileName));
                node.Tag = item.FileName;
                node.ToolTipText = Path.GetFileName(item.FileName);
                node.Image = GetImge(Path.GetExtension(item.FileName).ToUpper());
                _catalogNodes[identify].Nodes.Add(node);
                AddOtherCatalog(identify, item);
            }
        }

        private void AddOtherCatalog(string identify, ICatalogItem item)
        {
            if (string.IsNullOrEmpty(identify))
                return;
            ICatalog catalog = _wks.GetCatalogByIdentify(identify);
            if (catalog != null)
                catalog.AddItem(item);
        }


        private void RemoveOtherCatalog(ICatalogItem item, bool removeOther)
        {
            if (item == null || string.IsNullOrEmpty(item.FileName))
                return;
            if (item.Info.Properties.ContainsKey("SubProductIdentify"))
            {
                object v = item.Info.Properties["SubProductIdentify"];
                string identify = v != null ? v.ToString() : null;
                if (string.IsNullOrEmpty(identify))
                    return;
                ICatalog catalog = _wks.GetCatalogByIdentify(identify);
                if (catalog != null)
                    catalog.RemoveItem(item, removeOther);
            }
        }

        public override bool IsExist(ICatalogItem item)
        {
            //if (item == null || string.IsNullOrEmpty(item.FileName))
            //    return false;
            //if (item.Info.Properties.ContainsKey("SubProductIdentify"))
            //{
            //    object v = item.Info.Properties["SubProductIdentify"];
            //    string identify = v != null ? v.ToString() : null;
            //    if (_catalogNodes.ContainsKey(identify))
            //    {
            //        RadTreeNode tn = _catalogNodes[identify];
            //        foreach (RadTreeNode t in tn.Nodes)
            //            if (t.Tag != null && t.Tag.ToString() == item.FileName)
            //                return true;
            //    }
            //}
            //return false;
            return IsExist(item, false);
        }

        public bool IsExist(ICatalogItem item, bool isRemove)
        {
            if (item == null || string.IsNullOrEmpty(item.FileName))
                return false;
            if (item.Info.Properties.ContainsKey("SubProductIdentify"))
            {
                object v = item.Info.Properties["SubProductIdentify"];
                string identify = v != null ? v.ToString() : null;
                if (_catalogNodes.ContainsKey(identify))
                {
                    RadTreeNode tn = _catalogNodes[identify];
                    foreach (RadTreeNode t in tn.Nodes)
                        if (t.Tag != null && t.Tag.ToString() == item.FileName)
                        {
                            if (isRemove)
                                tn.Nodes.Remove(t);
                            return true;
                        }
                }
            }
            return false;
        }

        protected override bool RemoveFromWks(ICatalogItem item)
        {
            return IsExist(item, true);
        }

        public override void Update(ICatalogItem item)
        {

        }

        public override void Clear()
        {
            foreach (RadTreeNode tn in _catalogNodes.Values)
                tn.Nodes.Clear();
        }

        protected override void AddFileToUI(string dateDir, string fname)
        {
            var catalogItem = new CatalogItem(fname, _definition as SubProductCatalogDef);
            AddItem(catalogItem);
        }

        public override bool RemoveItem(ICatalogItem item, bool removeOther)
        {
            if (item == null || _treeView.Nodes.Count == 0)
                return true;
            RadTreeNode[] nodes = FindNodes(Path.GetFileName(item.FileName));
            //by chennan 20120814 今日监测分析面板中文显示
            //RadTreeNode[] nodes = _treeView.FindNodes(Path.GetFileName(item.FileName));
            if (nodes == null || nodes.Length == 0)
                return false;
            List<RadTreeNode> tempNodes = new List<RadTreeNode>();
            tempNodes.AddRange(nodes);
            foreach (RadTreeNode node in tempNodes)
            {
                if (node.Tag == null || string.IsNullOrEmpty(node.Tag.ToString()))
                    continue;
                if (node.Tag.ToString().ToUpper() == item.FileName.ToUpper())
                    node.Remove();
            }
            if (removeOther)
                RemoveOtherCatalog(item, false);
            return true;
        }

        private RadTreeNode[] FindNodes(string filename)
        {
            List<RadTreeNode> nodeList = new List<RadTreeNode>();
            int length = _treeView.Nodes.Count;
            for (int i = 0; i < length; i++)
            {
                if (_treeView.Nodes[0].Tag != null)
                    if (_treeView.Nodes[0].Tag.ToString().Contains(filename))
                        nodeList.Add(_treeView.Nodes[i]);
                FindSubNodes(_treeView.Nodes[i], filename, ref nodeList);
            }
            return nodeList.Count == 0 ? null : nodeList.ToArray();
        }

        private void FindSubNodes(RadTreeNode pNode, string filename, ref List<RadTreeNode> nodeList)
        {
            int length = pNode.Nodes.Count;
            for (int i = 0; i < length; i++)
            {
                if (pNode.Nodes[i].Tag != null)
                    if (pNode.Nodes[i].Tag.ToString().Contains(filename))
                        nodeList.Add(pNode.Nodes[i]);
                FindSubNodes(pNode.Nodes[i], filename, ref nodeList);
            }
        }
    }
}
