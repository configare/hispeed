using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using Telerik.WinControls.UI;
using System.IO;
using System.Windows.Forms;
using CodeCell.Bricks.UIs;
using System.Diagnostics;

namespace GeoDo.RSS.MIF.UI
{
    public abstract class Catalog : ICatalog
    {
        protected CatalogDef _definition;
        protected RadPageViewItemPage _uiPage;
        protected IWorkspace _wks;
        protected static ContextMenuStrip _menuStrip = new ContextMenuStrip();
        protected Dictionary<enumOperationsType, string> _operations = new Dictionary<enumOperationsType, string>();
        protected static CatalogItem _currentCatalogItem = null;

        public Catalog(IWorkspace wks, CatalogDef catalogDef, RadPageViewItemPage uiPage)
        {
            _wks = wks;
            _definition = catalogDef;
            _uiPage = uiPage;
            _menuStrip.RenderMode = ToolStripRenderMode.System;
            InitOperations();
        }

        protected void LoadCatalogItems()
        {
            if (_definition is SubProductCatalogDef)
                LoadSubProductItems();
        }

        private void LoadSubProductItems()
        {
            string wkddir = MifEnvironment.GetWorkspaceDir();
            string catalogDir = Path.Combine(wkddir, _wks.Definition.Identify);
            if (!Directory.Exists(catalogDir))
            {
                return;
            }
            string[] dateFolders = Directory.GetDirectories(catalogDir);
            dateFolders = FilterByDate(dateFolders);
            string dir;
            SubProductCatalogDef def = _definition as SubProductCatalogDef;

            foreach (string dateDir in dateFolders)
            {
                string date = (new DirectoryInfo(dateDir)).Name;
                dir = Path.Combine(dateDir, def.Folder);
                if (!Directory.Exists(dir))
                {
                    continue;
                }
                string[] fnames = GetFiles(dir, def.Identify, def.Pattern, true);
                if (fnames != null && fnames.Length > 0)
                    foreach (string f in fnames)
                        AddFileToUI(date, f);
            }
        }

        protected abstract void AddFileToUI(string dateDir, string fname);

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

        private string[] FilterByDate(string[] dateFolders)
        {
            List<string> rsFolders = new List<string>();
            DateTime filterDay = DateTime.Now.Date.AddDays((_wks.StrategyFilter.Days - 1) * (-1));
            foreach (string dateFolder in dateFolders)
            {
                var dir = Path.GetFileName(dateFolder);
                DateTime dt;
                if (!DateTime.TryParse(dir, out dt)) continue;
                if(dt.Date>=filterDay)
                    rsFolders.Add(dateFolder);
            }
            return rsFolders.ToArray();
        }

        public CatalogDef Definition
        {
            get { return _definition; }
        }

        public object UI
        {
            get { return _uiPage; }
        }

        public abstract ICatalogItem[] GetSelectedItems();

        public abstract string[] GetSelectedFiles();

        public string[] GetSelectedFiles(string identify)
        {
            string[] fs = GetSelectedFiles();
            if (fs == null || fs.Length == 0)
                return null;
            List<string> fnames = new List<string>();
            foreach (string f in fs)
                if (f.Contains("_" + identify + "_"))
                    fnames.Add(f);
            return fnames.Count > 0 ? fnames.ToArray() : null;
        }

        public abstract void AddItem(ICatalogItem item);

        public abstract bool IsExist(ICatalogItem item);

        public abstract void Update(ICatalogItem item);

        public abstract void Clear();

        public abstract bool RemoveItem(ICatalogItem item, bool removeOther);


        #region 右键菜单

        protected virtual void InitOperations()
        {
            //_operations.Add(enumOperationsType.Open, "打开");
            //_operations.Add(enumOperationsType.Theme, "生成专题图->……");
            //_operations.Add(enumOperationsType.Stat, "统计分析->……");
            _operations.Add(enumOperationsType.Remove, "移除");
            _operations.Add(enumOperationsType.SavePath, "拷贝路径");
            _operations.Add(enumOperationsType.OpenPath, "打开路径");
            _operations.Add(enumOperationsType.Delete, "删除原文件");
            _operations.Add(enumOperationsType.SaveAS, "另存文件为");
        }

        public ContextMenuStrip ContentMenuStrip
        {
            get
            {
                _menuStrip.Items.Clear();
                if (_operations != null && _operations.Count > 0)
                {
                    foreach (enumOperationsType opr in _operations.Keys)
                    {
                        ToolStripMenuItem it = new ToolStripMenuItem(_operations[opr]);
                        it.Tag = opr;
                        it.Click += new EventHandler(it_Click);
                        _menuStrip.Items.Add(it);
                    }
                }
                return _menuStrip;
            }
        }

        public void it_Click(object sender, EventArgs e)
        {
            if (sender is enumOperationsType)
                Click((enumOperationsType)(sender), _currentCatalogItem);
            else
                Click((enumOperationsType)(sender as ToolStripMenuItem).Tag, _currentCatalogItem);
        }

        internal void SetCurrentDataItem(CatalogItem item)
        {
            _currentCatalogItem = item;
        }

        protected virtual void Click(enumOperationsType opr, CatalogItem catalogItem)
        {
            try
            {
                switch (opr)
                {
                    case enumOperationsType.Open:
                        OpenDataItem(GetSelectedItems());
                        break;
                    case enumOperationsType.Delete:
                        DeleteDataItem(GetSelectedItems());
                        break;
                    case enumOperationsType.SaveAS:
                        SaveAs(GetSelectedItems());
                        break;
                    case enumOperationsType.Theme:
                        break;
                    case enumOperationsType.Stat:
                        break;
                    case enumOperationsType.SavePath:
                        SavePath(GetSelectedItems());
                        break;
                    case enumOperationsType.OpenPath:
                        OpenPath(GetSelectedItems());
                        break;
                    case enumOperationsType.Remove:
                        RemoveFromWorkspace(GetSelectedItems());
                        break;
                    default:
                        break;
                }
            }
            finally
            {
            }
        }

        private void RemoveFromWorkspace(ICatalogItem[] items)
        {
            if (items == null || items.Length == 0)
                return;
            foreach (ICatalogItem catalogItem in items)
                RemoveFromWks(catalogItem);
        }

        protected virtual bool RemoveFromWks(ICatalogItem items)
        {
            return true;
        }

        private void OpenDataItem(ICatalogItem[] items)
        {
            if (items == null || items.Length == 0)
                return;
            foreach (ICatalogItem catalogItem in items)
            {
                OpenFile(catalogItem);
            }
        }

        private void OpenFile(ICatalogItem catalogItem)
        {
            string filename = catalogItem.FileName;
            if (!File.Exists(filename))
                return;
            try
            {
                //打开文件
            }
            catch (Exception ex)
            {
                MsgBox.ShowInfo(ex.Message);
            }
        }

        private void DeleteDataItem(ICatalogItem[] items)
        {
            if (items == null || items.Length == 0)
                return;
            DialogResult isYes = MessageBox.Show("确定要删除数据:\n" + GetCatalogItemNames(items) + "?\n\n按[是]删除,按[否]放弃删除!",
                                                "系统消息", MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);
            if (isYes == DialogResult.Yes)
            {
                foreach (ICatalogItem catalogItem in items)
                {
                    if (DeleteFile(catalogItem))
                        RemoveItem(catalogItem, true);
                }
            }
        }

        private string GetCatalogItemNames(ICatalogItem[] dataItems)
        {
            string itemNames = string.Empty;
            foreach (ICatalogItem catalogItem in dataItems)
            {
                itemNames += Path.GetFileName(catalogItem.FileName) + ",\n";
            }
            return string.IsNullOrEmpty(itemNames) ? "" : itemNames.Substring(0, itemNames.Length - 2);
        }

        protected virtual bool DeleteFile(ICatalogItem catalogItem)
        {
            string filename = catalogItem.FileName;
            if (!File.Exists(filename))
                return true;
            try
            {
                return DeleteFile(filename);
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("正由另一进程使用，因此该进程无法访问该文件") != -1)
                    MsgBox.ShowInfo("当前文件正在使用中，请结束使用后再执行删除!");
                return false;
            }
        }

        private bool DeleteFile(string mainFile)
        {
            try
            {
                string dir = Path.GetDirectoryName(mainFile);
                string[] files = Directory.GetFiles(dir, Path.GetFileNameWithoutExtension(mainFile) + ".*", SearchOption.TopDirectoryOnly);
                foreach (string filename in files)
                {
                    if (File.Exists(filename))
                        File.Delete(filename);
                }
                return true;
            }
            catch (Exception Ex)
            {
                MsgBox.ShowInfo(Ex.Message);
                return false;
            }
        }

        // by chennan 20131106 记录上一次的路径
        // by chennan 20131108 支持多文件拷贝
        private void SaveAs(ICatalogItem[] catalogItems)
        {
            if (catalogItems == null || catalogItems.Length == 0)
                return;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.RestoreDirectory = true;
            string fileExt = Path.GetExtension(catalogItems[0].FileName);
            string file = catalogItems[0].FileName;
            saveFileDialog.Filter = "保存文件类型(*" + fileExt + ")|*" + fileExt;
            saveFileDialog.FileName = Path.GetFileName(catalogItems[0].FileName);
            string error = string.Empty;
            StringBuilder sb = new StringBuilder("文件：\n");

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (ICatalogItem item in catalogItems)
                {
                    if (CopyFilesWithSameExt(item.FileName, saveFileDialog.FileName, ref error))
                        sb.Append(Path.GetFileName(item.FileName) + "\n");
                }
                MsgBox.ShowInfo(sb.ToString() + "保存成功!");
            }
        }

        public bool CopyFilesWithSameExt(string filename, string dstFullName, ref string error)
        {
            if (string.IsNullOrEmpty(dstFullName))
                return false;
            string[] files = Directory.GetFiles(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".*", SearchOption.TopDirectoryOnly);
            error = string.Empty;
            string dstPath = Path.GetDirectoryName(dstFullName);
            string dstFile = Path.GetFileNameWithoutExtension(dstFullName);
            if (string.IsNullOrEmpty(dstFile))
            {
                foreach (string fileStr in files)
                {
                    try
                    {
                        dstFile = Path.Combine(dstPath, Path.GetFileName(fileStr));
                        File.Copy(fileStr, dstFile, true);
                    }
                    catch (Exception ex)
                    {
                        error += "【" + fileStr + "】" + "拷贝失败,原因:" + ex.Message + "\n";
                    }
                }
            }
            else
            {
                foreach (string fileStr in files)
                {
                    try
                    {
                        dstFile = Path.Combine(dstPath, dstFile + Path.GetExtension(fileStr));
                        File.Copy(fileStr, dstFile, true);
                    }
                    catch (Exception ex)
                    {
                        error += "【" + fileStr + "】" + "拷贝失败,原因:" + ex.Message + "\n";
                    }
                }
            }
            return string.IsNullOrEmpty(error);
        }

        private void SavePath(ICatalogItem[] catalogItems)
        {
            if (catalogItems == null || catalogItems.Length == 0)
                return;
            string file = catalogItems[0].FileName;
            string path = Path.GetDirectoryName(catalogItems[0].FileName);
            string filePath = Path.Combine(path, file);
            Clipboard.SetText(filePath, TextDataFormat.Text);
        }

        private void OpenPath(ICatalogItem[] catalogItems)
        {
            if (catalogItems == null || catalogItems.Length == 0)
                return;
            string path = Path.GetDirectoryName(catalogItems[0].FileName);
            string file = catalogItems[0].FileName;
            ProcessStartInfo psi = new ProcessStartInfo("Explorer.exe");
            psi.Arguments = " /select," + file;
            Process.Start(psi);
        }

        #endregion
    }
}
