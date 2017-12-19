using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using Telerik.WinControls.UI;
using System.IO;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.ReportService
{
    /// <summary>
    /// 入库的目录树
    /// </summary>
    public class CatalogTreeView
    {
        private RadTreeView _treeView;
        private Dictionary<string, RadTreeNode> _catalogNodes = new Dictionary<string, RadTreeNode>();
        private WorkspaceDef _wDef;
        private Font _font = new Font("宋体", 10);
        internal static string ToReportInfoKey = "ToReport";
        internal static string ToReportInfoValue = "true";
        private bool _showHasToReport = false;
        private DateTime _date = DateTime.Today;

        public CatalogTreeView(RadTreeView treeView, WorkspaceDef wDef, bool showHasToReport, DateTime date)
        {
            _treeView = treeView;
            _treeView.AutoCheckChildNodes = true;
            _wDef = wDef;
            _showHasToReport = showHasToReport;
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

            //区域名称，时间，文件
            Dictionary<string, Dictionary<string, List<string>>> fileClassic = null;

            if (Directory.Exists(prdRootDir))
            {
                //整理产品数据，用于TreeView
                fileClassic = ClassicFiles(prdRootDir, rootNode, fileClassic);
                if (fileClassic != null)
                    FullTreeView(rootNode, fileClassic);
            }
            rootNode.ExpandAll();
        }

        private void FullTreeView(RadTreeNode rootNode, Dictionary<string, Dictionary<string, List<string>>> fileClassic)
        {
            RadTreeNode regionNode = null;
            foreach (var region in fileClassic.OrderBy(s => s.Key))
            {
                regionNode = new RadTreeNode(region.Key);
                regionNode.Tag = fileClassic[region.Key];
                regionNode.CheckType = CheckType.CheckBox;
                regionNode.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(regionNode_PropertyChanged);
                rootNode.Nodes.Add(regionNode);
                FullRegionNodes(regionNode, fileClassic[region.Key]);
            }
        }

        void regionNode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RadTreeNode node = sender as RadTreeNode;
            if (node != null)
            {
                if (node.Nodes.Count != 0)
                {
                    foreach (var item in node.Nodes)
                        item.Checked = node.Checked;
                }
            }
        }

        private void FullRegionNodes(RadTreeNode regionNode, Dictionary<string, List<string>> fileDic)
        {
            RadTreeNode dateNode = null;

            foreach (var dateStr in fileDic.OrderBy(s => s.Key))
            {
                dateNode = new RadTreeNode(dateStr.Key);
                dateNode.Tag = fileDic;
                dateNode.CheckType = CheckType.CheckBox;
                regionNode.Nodes.Add(dateNode);
            }
        }

        private Dictionary<string, Dictionary<string, List<string>>> ClassicFiles(string prdRootDir, RadTreeNode rootNode, Dictionary<string, Dictionary<string, List<string>>> fileClassic)
        {
            string[] files = Directory.GetFiles(prdRootDir, "*.*", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                rootNode.Nodes.Add(new RadTreeNode("当期日期下没有产品生成"));
            else
            {
                int length = files.Length;
                RasterIdentify rid = null;
                string regionIdentify;
                string dateStr;
                string satelliteFlog;
                string dateFormatStr = "yyyy年MM月dd日 HH：mm：ss";
                Dictionary<string, List<string>> timeClassic = null;
                fileClassic = new Dictionary<string, Dictionary<string, List<string>>>();
                ICatalogItem ca = null;
                for (int i = 0; i < length; i++)
                {
                    if (files[i].ToLower().Contains("tempmcd"))
                        continue;
                    rid = new RasterIdentify(files[i]);
                    ca = GetFileInfo(files[i]);
                    if (ca != null && ca.Info != null && (!_showHasToReport && ca.Info.GetPropertyValue(ToReportInfoKey) == ToReportInfoValue))//是否标记为已提交过的素材
                        continue;
                    satelliteFlog = string.IsNullOrEmpty(rid.Satellite) ? "气象卫星--" : rid.Satellite + "--";
                    regionIdentify = string.IsNullOrEmpty(rid.RegionIdentify) ? "未知区域" : rid.RegionIdentify;
                    if (!fileClassic.ContainsKey(regionIdentify))
                    {
                        timeClassic = new Dictionary<string, List<string>>();
                        if (rid.MinOrbitDate == DateTime.MinValue)
                            dateStr = satelliteFlog + rid.OrbitDateTime.AddHours(8).ToString(dateFormatStr) + "（北京时）";
                        else
                            dateStr = satelliteFlog + rid.ObritTimeRegion;
                        if (!timeClassic.ContainsKey(dateStr))
                            timeClassic.Add(dateStr, new List<string>() { files[i] });
                        else
                            timeClassic[dateStr].Add(files[i]);
                        fileClassic.Add(regionIdentify, timeClassic);
                    }
                    else
                    {
                        if (rid.MinOrbitDate == DateTime.MinValue)
                            dateStr = satelliteFlog + rid.OrbitDateTime.AddHours(8).ToString(dateFormatStr) + "（北京时）";
                        else
                            dateStr = satelliteFlog + rid.ObritTimeRegion;
                        regionIdentify = string.IsNullOrEmpty(rid.RegionIdentify) ? "未知区域" : rid.RegionIdentify;
                        if (!fileClassic[regionIdentify].ContainsKey(dateStr))
                            fileClassic[regionIdentify].Add(dateStr, new List<string>() { files[i] });
                        else
                            fileClassic[regionIdentify][dateStr].Add(files[i]);
                    }
                }
            }
            return fileClassic == null || fileClassic.Count == 0 ? null : fileClassic;
        }

        #endregion

        #region 返回选中的文件
        public Dictionary<string, List<ICatalogItem>> GetCheckedItem()
        {
            if (_treeView.CheckedNodes == null)
                return null;
            Dictionary<string, List<ICatalogItem>> checks = new Dictionary<string, List<ICatalogItem>>();
            for (int i = 0; i < _treeView.CheckedNodes.Count; i++)
            {
                RadTreeNode fileNode = _treeView.CheckedNodes[i];
                Dictionary<string, List<string>> fileDic = fileNode.Tag != null ? fileNode.Tag as Dictionary<string, List<string>> : null;
                if (fileDic == null || fileDic.Count == 0)
                    continue;
                ICatalogItem ca = null;
                foreach (string date in fileDic.Keys)
                {
                    if (fileDic[date] == null || fileDic[date].Count == 0)
                        continue;
                    foreach (string fname in fileDic[date])
                    {
                        ca = CreateFileInfo(fname);
                        if (ca == null)
                            continue;
                        if (!checks.ContainsKey(date))
                            checks.Add(date, new List<ICatalogItem>() { ca });
                        else
                            checks[date].Add(ca);
                    }
                }

            }
            return checks.Count == 0 ? null : checks;
        }


        private ICatalogItem GetFileInfo(string fname)
        {
            CatalogItemInfo cii = null;
            string infoFile = Path.ChangeExtension(fname, ".info");
            if (File.Exists(infoFile))
            {
                cii = new CatalogItemInfo(infoFile);
                return new CatalogItem(fname, cii);
            }
            return null;
        }

        private ICatalogItem CreateFileInfo(string fname)
        {
            ICatalogItem ca = GetFileInfo(fname);
            if (ca != null)
                return ca;
            SubProductCatalogDef subProductCatalogDef = GetSubProCatalogDef(fname, _wDef.CatalogDefs);
            if (subProductCatalogDef == null)
                return null;
            ca = new CatalogItem(fname, subProductCatalogDef);
            return ca;
        }

        private SubProductCatalogDef GetSubProCatalogDef(string fname, List<CatalogDef> catalogDefList)
        {
            RasterIdentify rid = new RasterIdentify(fname);
            foreach (var item in catalogDefList)
            {
                if (item is SubProductCatalogDef)
                {
                    if (string.IsNullOrEmpty(rid.SubProductIdentify))
                        continue;
                    if (item.Identify.ToUpper().Contains(rid.SubProductIdentify.ToUpper()))
                        return item as SubProductCatalogDef;
                }
            }
            return null;
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
