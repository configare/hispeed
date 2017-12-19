using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.UI;
using GeoDo.RSS.UI.AddIn.HdService.HdDataServer;
using GeoDo.RSS.UI.AddIn.Theme;
using Telerik.WinControls.UI;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public partial class toDbPageControl : UserControl
    {
        private string _hdProductPath = "";
        private ISmartSession _smartSession;
        private WorkspaceDef _wDef = null;
        private Dictionary<string, CatalogTreeView> _catalogs = new Dictionary<string, CatalogTreeView>();
        private RadPageView _radPageView = new RadPageView();

        public toDbPageControl()
        {
            InitializeComponent();
            _radPageView.Font = this.Font;
            _radPageView.Dock = DockStyle.Fill;
            _radPageView.ViewElement.ShowItemCloseButton = false;
            this.panel1.Controls.Add(_radPageView);
            _hdProductPath = ConfigurationManager.AppSettings.Get("HdProductPath");
        }

        public void UpdateData(Core.UI.ISmartSession smartSession)
        {
            this._smartSession = smartSession;
            MonitoringSession monitSession = _smartSession.MonitoringSession as MonitoringSession;
            ProductDef productDef = monitSession.ActiveMonitoringProduct.Definition;
            string _identify = productDef.Identify;
            _wDef = WorkspaceDefinitionFactory.GetWorkspaceDef(_identify);
            LoadData();
            InitHdData();
        }

        private void CreateView()
        {
            RadListView lv = new RadListView();
            lv.Columns.Add("日期");
            lv.Columns.Add("产品名称");
        }

        private void LoadData()
        {
            if (_wDef == null)
                return;
            _catalogs.Clear();
            _radPageView.Pages.Clear();
            List<CatalogDef> catalogDefs = _wDef.CatalogDefs;//当前产品的定义目录
            foreach (CatalogDef catalog in catalogDefs)
            {
                if (catalog is ExtractingCatalogDef)
                {
                    RadPageViewItemPage tabPage = GetRadPage(catalog);
                    tabPage.Text = "待入库产品";
                    tabPage.Dock = DockStyle.Fill;
                    tabPage.Tag = catalog;
                    _radPageView.Pages.Add(tabPage);
                }
            }
            lbMessage.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private RadPageViewItemPage GetRadPage(CatalogDef catalog)
        {
            RadPageViewItemPage page = new RadPageViewItemPage();
            page.Text = catalog.Text;
            Control ui = GetUserControl(catalog);
            if (ui != null)
            {
                ui.Dock = DockStyle.Fill;
                page.Controls.Add(ui);
            }
            if (catalog is ExtractingCatalogDef)
            {
                CatalogTreeView ct = new CatalogTreeView(ui as RadTreeView, catalog, _wDef, chkHasToDb.Checked, dateTimePicker1.Value);
                _catalogs.Add(catalog.ClassString, ct);
            }
            //else if (catalog is SubProductCatalogDef)
            //{
            //    UCOperationBar oprBar = new UCOperationBar();
            //    oprBar.btnOpenFiles.Tag = catalog.ClassString;
            //    oprBar.Dock = DockStyle.Top;
            //    oprBar.btnOpenFiles.Click += new EventHandler(oprBar_Click);
            //    oprBar.tsCBGroup.Tag = catalog.ClassString;
            //    page.Controls.Add(oprBar);
            //    _catalogs.Add(catalog.ClassString, new CatalogListView(this, catalog, page, ui as RadListView));
            //    InitOprBarGroupItem(oprBar.tsCBGroup);
            //    oprBar.tsCBGroup.SelectedIndexChanged += new EventHandler(tsCBGroup_SelectedIndexChanged);
            //}
            return page;
        }

        private Control GetUserControl(CatalogDef catalog)
        {
            RadTreeView tv = new RadTreeView();
            tv.Tag = catalog;
            tv.ShowLines = true;
            tv.HideSelection = false;
            tv.Font = this.Font;
            return tv;
        }

        private void btnToDb_Click(object sender, EventArgs e)
        {
            try
            {
                btnToDb.Enabled = false;
                ICatalogItem[] items = null;
                foreach (CatalogTreeView cv in _catalogs.Values)
                {
                    items = cv.GetCheckedItem();
                }
                //
                if (items == null)
                    return;
                OnProgress(0);
                for (int i = 0; i < items.Length; i++)
                {
                    OnProgress((int)((i + 1.0) / items.Length));
                    string dir = SaveToDb(items[i]);
                }
                OnSendMessage("入库完成");
                MsgBox.ShowInfo("入库完成");
            }
            catch (Exception ex)
            {
                OnSendMessage(ex.Message);
                MsgBox.ShowInfo(ex.Message);
            }
            finally
            {
                LoadData();
                btnToDb.Enabled = true;
                OnProgress(0);
            }
        }

        private void OnProgress(int persent)
        {
            toolStripProgressBar1.Value = persent;
        }

        #region 华迪入库服务
        private HdDataServer.DataSearchServerPortTypeClient _client = null;

        private void InitHdData()
        {
            _client = new HdDataServer.DataSearchServerPortTypeClient();
            _client.Open();
            _client.getDataDirCompleted += new EventHandler<HdDataServer.getDataDirCompletedEventArgs>(_client_getDataDirCompleted);
            _client.saveProductInfoCompleted += new EventHandler<saveProductInfoCompletedEventArgs>(_client_saveProductInfoCompleted);
        }

        void _client_saveProductInfoCompleted(object sender, saveProductInfoCompletedEventArgs e)
        {
            
        }

        void _client_getDataDirCompleted(object sender, HdDataServer.getDataDirCompletedEventArgs e)
        {
        }
        #endregion

        private string SaveToDb(ICatalogItem catalog)
        {
            MonitorProductInfo info = new MonitorProductInfo();
            //fir ,fog
            info.productType = catalog.Info.GetPropertyValue("ProductIdentify");
            //tjcp:统计产品  xxcp;信息列表  sgcp:栅格产品 ztcp:专题产品   slcp:矢量产品 dhcp:动画产品
            info.productDataType = catalog.Info.GetPropertyValue("CatalogDef"); //VectorProduct
            info.productIdentify = catalog.Info.GetPropertyValue("SubProductIdentify"); //DBLV,0CSR,"PLST";
            info.productIdentifyName = catalog.Info.GetPropertyValue("CatalogItemCN");  //"火点二值图";
            info.satellite = catalog.Info.GetPropertyValue("Satellite");
            info.sensor = catalog.Info.GetPropertyValue("Sensor");
            info.subProductType = "";                       //通常为空
            info.orbitDateTime = DateTime.Parse(catalog.Info.GetPropertyValue("OrbitDateTime"));
            info.info = new Info();
            info.info.orbitDateTime = info.orbitDateTime;
            info.info.satellite = info.satellite;
            info.info.sensor = info.sensor;
            string dir = _client.getDataDir(info);
            if (catalog.Info.Properties.ContainsKey(CatalogTreeView.ToDBInfoKey))
                catalog.Info.Properties[CatalogTreeView.ToDBInfoKey] = CatalogTreeView.ToDBInfoValue;
            else
                catalog.Info.Properties.Add(CatalogTreeView.ToDBInfoKey, CatalogTreeView.ToDBInfoValue);
            string infoFilename = Path.ChangeExtension(catalog.FileName, ".info");
            catalog.Info.SaveTo(infoFilename);

            string[] extFiles = GetExtFiles(catalog.FileName);
            info.extendFiles = ExtFilesToString(extFiles);

            CopyFile(dir, catalog.FileName);
            if (extFiles != null && extFiles.Length != 0)
            {
                for (int i = 0; i < extFiles.Length; i++)
                {
                    CopyFile(dir,extFiles[i]);
                }
            }

            string productFilename = Path.GetFileName(catalog.FileName);
            info.productFileName = productFilename;
            info.productFilePath = dir;

            SaveToDb(info.productIdentify, info);
            return dir;
        }

        private string ExtFilesToString(string[] extFiles)
        {
            if (extFiles == null || extFiles.Length == 0)
                return "";
            string extString = "";
            for (int i = 0; i < extFiles.Length; i++)
            {
                extString = extString + Path.GetFileName(extFiles[i]) + ",";
            }
            extString = extString.TrimEnd(',');
            return extString;
        }

        private string[] GetExtFiles(string filename)
        {
            List<string> extfiles = new List<string>();
            string[] exts = new string[] { ".info", ".hdr", ".png" };
            for (int i = 0; i < exts.Length; i++)
            {
                string extfile = GetExtFile(filename, exts[i]);
                if (!string.IsNullOrWhiteSpace(extfile))
                    extfiles.Add(extfile);
            }
            return extfiles.ToArray();
        }

        private string GetExtFile(string filename,string exten)
        {
            string extfile = Path.ChangeExtension(filename, exten);
            if (File.Exists(extfile))
                return extfile;
            return null;
        }

        private void OnSendMessage(string msg)
        {
            lbMessage.Text = msg;
        }

        private void CopyFile(string dir, string file)
        {
            string dstFilename = Path.Combine(_hdProductPath + dir, Path.GetFileName(file));
            if (!Directory.Exists(Path.GetDirectoryName(dstFilename)))
                Directory.CreateDirectory(Path.GetDirectoryName(dstFilename));
            if (File.Exists(file))
                File.Copy(file, dstFilename, true);
        }

        /// <summary>
        /// public boolean saveFir(Map<String,MonitorProductInfo> firMap) {
        /// PLST.
        /// </summary>
        private void SaveToDb(string key, MonitorProductInfo info)
        {
            _client.saveProductInfo(info);
        }

        private void btnCheckedAll_Click(object sender, EventArgs e)
        {
            foreach (CatalogTreeView cv in _catalogs.Values)
            {
                cv.CheckedALL();
            }
        }

        private void btnCheckedNone_Click(object sender, EventArgs e)
        {
            foreach (CatalogTreeView cv in _catalogs.Values)
            {
                cv.CheckedNone();
            }
        }

        private void chkHasToDb_CheckedChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}
