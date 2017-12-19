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
    public class CatalogTreeView
    {
        private RadTreeView _treeView;
        private Dictionary<string, RadTreeNode> _catalogNodes = new Dictionary<string, RadTreeNode>();
        private WorkspaceDef wDef;
        private CatalogDef _definition;
        private Font _font  = new Font("宋体",10);
        internal static string ToDBInfoKey = "ToDB";
        internal static string ToDBInfoValue = "true";
        private bool _showHasToDb = false;
        private DateTime _date = DateTime.Today;

        public CatalogTreeView(RadTreeView treeView, CatalogDef catalog, WorkspaceDef wDef, bool showHasToDb, DateTime date)
        {
            this._treeView = treeView;
            _treeView.AutoCheckChildNodes = true;
            this.wDef = wDef;
            this._definition = catalog;
            _showHasToDb = showHasToDb;
            _date = date;
            LoadTodayExtractResult(_date);
        }

        public ICatalogItem[] GetCheckedItem()
        {
            if (_treeView.CheckedNodes == null)
                return null;
            List<ICatalogItem> checks = new List<ICatalogItem>();
            for (int i = 0; i < _treeView.CheckedNodes.Count; i++)
            {
                string fname = _treeView.CheckedNodes[i].Tag != null ? _treeView.CheckedNodes[i].Tag.ToString() : null;
                if (string.IsNullOrEmpty(fname))
                    return null;
                ICatalogItem ca = new CatalogItem(fname, _definition as SubProductCatalogDef);
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

        /// <summary>
        /// 加载指定日期目录下的产品待入库文件
        /// yyyy-MM-dd\
        /// </summary>
        /// <param name="dt"></param>
        private void LoadTodayExtractResult(DateTime dt)
        {
            _catalogNodes.Clear();
            _treeView.Nodes.Clear();
            string dir = Path.Combine(MifEnvironment.GetWorkspaceDir(), wDef.Identify);
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

            string[] files = GetFiles(dateDir, group.Identify, group.Pattern, true);
            RadTreeNode crtNode = new RadTreeNode(group.Text);
            if (!_catalogNodes.ContainsKey(group.Identify))
            {
                AddMultiIdentigy(group.Identify, crtNode);
            }
            crtNode.Font = _font;

            crtNode.Image = GetImge("Open");
            parentNode.Nodes.Add(crtNode);
            if (files != null && files.Length > 0)
            {
                foreach (string f in files)
                {
                    ICatalogItem ca = new CatalogItem(f, _definition as SubProductCatalogDef);
                    if (!_showHasToDb && ca.Info.GetPropertyValue(ToDBInfoKey) == ToDBInfoValue)
                        continue;
                    RadTreeNode t = new RadTreeNode(GetCatalogCN(f));
                    t.Tag = f;
                    t.ToolTipText = Path.GetFileName(f);
                    t.Image = GetImge(Path.GetExtension(f).ToUpper());
                    t.CheckType = CheckType.CheckBox;
                    t.Checked = true;
                    crtNode.Nodes.Add(t);
                }
            }

            if (group.NodeDefs != null && group.NodeDefs.Count > 0)
            {
                foreach (CatalogNodeItemDef sub in group.NodeDefs)
                {
                    LoadFiles(sub, crtNode, dateDir);
                }
            }
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
            foreach (string id in ids)
            {
                string searchPattern = string.Format(pattern, id);
                string[] fs = Directory.GetFiles(dir, searchPattern, isIncludeSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                if (fs != null && fs.Length > 0)
                    files.AddRange(fs);
            }
            return files.Count > 0 ? files.ToArray() : null;
        }

    }
}
