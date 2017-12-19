using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using Telerik.WinControls.UI;
using System.IO;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    /// <summary>
    /// 入库的目录树
    /// </summary>
    public class CatalogTreeView
    {
        private RadTreeView _treeView;
        private Dictionary<string, RadTreeNode> _catalogNodes = new Dictionary<string, RadTreeNode>();
        private WorkspaceDef _wDef;
        private Font _font  = new Font("宋体",10);
        internal static string ToDBInfoKey = "ToDB";
        internal static string ToDBInfoValue = "true";
        private bool _showHasToDb = false;
        private DateTime _date = DateTime.Today;

        public CatalogTreeView(RadTreeView treeView,WorkspaceDef wDef, bool showHasToDb, DateTime date)
        {
            _treeView = treeView;
            _treeView.AutoCheckChildNodes = true;
            _wDef = wDef;
            _showHasToDb = showHasToDb;
            _date = date;
            LoadTheDayExtractResult(_date);
        }

        #region 加载文件
        /// <summary>
        /// 加载指定日期目录下的产品待入库文件
        /// \Workspace\{PrdIdentify}\yyyy-MM-dd\...
        /// </summary>
        /// <param name="dt"></param>
        private void LoadTheDayExtractResult(DateTime dt)
        {
            _catalogNodes.Clear();
            _treeView.Nodes.Clear();

            string dir = Path.Combine(MifEnvironment.GetWorkspaceDir(), _wDef.Identify);
            string data = dt.ToString("yyyy-MM-dd");
            string prdRootDir = Path.Combine(dir, data);
            RadTreeNode rootNode = new RadTreeNode(data);
            rootNode.Font = _font;
            _treeView.Nodes.Add(rootNode);

            if (Directory.Exists(prdRootDir))
            {
                foreach (CatalogDef catalogDef in _wDef.CatalogDefs)
                {
                    if (catalogDef is SubProductCatalogDef)//子产品目录
                    {
                        SubProductCatalogDef subProductdef = catalogDef as SubProductCatalogDef;
                        string subProductDir = Path.Combine(prdRootDir, subProductdef.Folder);
                        RadTreeNode subProductNode = new RadTreeNode(subProductdef.Text);
                        subProductNode.Tag = subProductdef;
                        subProductNode.ToolTipText = subProductDir;
                        subProductNode.Font = _font;
                        subProductNode.Image = GetImge("Open");
                        rootNode.Nodes.Add(subProductNode);
                        string[] files = GetFiles(subProductDir, subProductdef.Identify, subProductdef.Pattern, false);
                        if (files != null && files.Length > 0)
                        {
                            foreach (string file in files)
                            {
                                ICatalogItem ca = new CatalogItem(file, subProductdef);
                                if (!_showHasToDb && ca.Info.GetPropertyValue(ToDBInfoKey) == ToDBInfoValue)//是否标记为已入库
                                    continue;
                                RadTreeNode fileNode = new RadTreeNode(GetCatalogCN(file));
                                fileNode.Tag = file;
                                fileNode.ToolTipText = Path.GetFileName(file);
                                fileNode.Image = GetImge(Path.GetExtension(file).ToUpper());
                                fileNode.CheckType = CheckType.CheckBox;
                                fileNode.Checked = true;
                                subProductNode.Nodes.Add(fileNode);
                            }
                        }
                    }
                }
            }
            else
            {
                rootNode.Nodes.Add(new RadTreeNode("当期日期下没有产品生成"));
            }
            rootNode.ExpandAll();
        }

        private string GetCatalogCN(string filename)
        {
            CatalogItem ci = new CatalogItem(filename, null, null);
            string result = ci.Info.GetPropertyValue("CatalogItemCN");
            string extInfo = string.Empty;
            if (result.IndexOf("(") == -1)
            {
                RasterIdentify ri = new RasterIdentify(filename);
                extInfo = (string.IsNullOrEmpty(ri.RegionIdentify) ? "" : ri.RegionIdentify + "_") + ri.OrbitDateTime.ToString("yyyyMMdd HH:mm") +
                    (string.IsNullOrEmpty(ri.ExtInfos) ? "" : "_" + ri.ExtInfos);
            }
            if (string.IsNullOrEmpty(extInfo))
                return result;
            else if (extInfo.EndsWith("_"))
                extInfo = extInfo.Substring(0, extInfo.Length - 1);
            return result + "(" + extInfo + ")";
        }

        private Image GetImge(string fileExt)
        {
            switch (fileExt)
            {
                case ".DAT":
                    return Properties.Resources.DBLVFile;
                case ".XLSX":
                case ".XLS":
                    return Properties.Resources.ExcelFile;
                case ".GXD":
                    return Properties.Resources.GXDFile;
                case "Category":
                    return Properties.Resources.Category;
                case "Open":
                    return Properties.Resources.cmdOpen;
                default:
                    return Properties.Resources.DBLVFile;
            }
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

        protected string[] GetFiles(string dir, string identify, string pattern, bool isIncludeSubDir)
        {
            List<string> files = new List<string>();
            string[] ids = identify.Split(',');
            if (!Directory.Exists(dir))
                return null;
            foreach (string id in ids)
            {
                string searchPattern = string.Format(pattern, id);
                string[] fs = Directory.GetFiles(dir, searchPattern, isIncludeSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                if (fs != null && fs.Length > 0)
                    files.AddRange(fs);
            }
            return files.Count > 0 ? files.ToArray() : null;
        }
        #endregion 

        #region 返回选中的文件
        public ICatalogItem[] GetCheckedItem()
        {
            if (_treeView.CheckedNodes == null)
                return null;
            List<ICatalogItem> checks = new List<ICatalogItem>();
            for (int i = 0; i < _treeView.CheckedNodes.Count; i++)
            {
                RadTreeNode fileNode = _treeView.CheckedNodes[i];
                string fname = fileNode.Tag != null ? fileNode.Tag.ToString() : null;
                if (string.IsNullOrEmpty(fname))
                    return null;
                SubProductCatalogDef subProductCatalogDef = fileNode.Parent.Tag as SubProductCatalogDef;
                ICatalogItem ca = new CatalogItem(fname, subProductCatalogDef);
                checks.Add(ca);
            }
            return checks.ToArray();
        }

        public string[] GetCheckedFiles()
        {
            if (_treeView.CheckedNodes == null)
                return null;
            List<string> checks = new List<string>();
            for (int i = 0; i < _treeView.CheckedNodes.Count; i++)
            {
                RadTreeNode node = _treeView.CheckedNodes[i];
                string fname = _treeView.SelectedNode.Tag != null ? _treeView.SelectedNode.Tag.ToString() : null;
                if (string.IsNullOrEmpty(fname))
                    return null;
                checks.Add(fname);
            }
            return checks.ToArray();
        }

        public void CheckedALL()
        {
            foreach (RadTreeNode node in _treeView.Nodes)
            {
                CheckedALL(node);
            }
        }

        private void CheckedALL(RadTreeNode pnode)
        {
            foreach (RadTreeNode node in pnode.Nodes)
            {
                if (node.CheckType == CheckType.CheckBox)
                {
                    if (node.CheckState == Telerik.WinControls.Enumerations.ToggleState.Off)
                        node.CheckState = Telerik.WinControls.Enumerations.ToggleState.On;
                }
                if (node.Nodes.Count == 0)
                    continue;
                else
                    CheckedALL(node);
            }
        }

        internal void CheckedNone()
        {
            foreach (RadTreeNode node in _treeView.Nodes)
            {
                CheckedNone(node);
            }
        }

        private void CheckedNone(RadTreeNode pnode)
        {
            foreach (RadTreeNode node in pnode.Nodes)
            {
                if (node.CheckType == CheckType.CheckBox)
                {
                    if (node.CheckState == Telerik.WinControls.Enumerations.ToggleState.On)
                        node.CheckState = Telerik.WinControls.Enumerations.ToggleState.Off;
                }
                if (node.Nodes.Count == 0)
                    continue;
                else
                    CheckedNone(node);
            }
        }
        #endregion

    }
}
