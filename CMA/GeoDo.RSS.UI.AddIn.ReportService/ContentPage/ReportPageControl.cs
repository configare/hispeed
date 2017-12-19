using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.UI;
using GeoDo.RSS.UI.AddIn.Theme;
using Telerik.WinControls.UI;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.ReportService
{
    public partial class ReportPageControl : UserControl
    {
        private string _reportPath = "";
        private ISmartSession _smartSession;
        private WorkspaceDef _wDef = null;
        private RadPageView _radPageView = new RadPageView();
        private CatalogTreeView _catalogTreeView;

        public ReportPageControl()
        {
            InitializeComponent();
            _radPageView.Font = this.Font;
            _radPageView.Dock = DockStyle.Fill;
            _radPageView.ViewElement.ShowItemCloseButton = false;
            this.panel1.Controls.Add(_radPageView);
            _reportPath = MifEnvironment.GetReportDir();
            CreateSubFoldername();
        }

        private void CreateSubFoldername()
        {
            cbSubFolder.Items.Clear();
            string baseDir = _reportPath + "\\" + dateTimePicker1.Value.ToString("yyyyMMdd") + "\\";
            if (!Directory.Exists(baseDir))
                cbSubFolder.Text = "新素材信息-1";
            else
            {
                string[] subDirs = Directory.GetDirectories(baseDir, ".", SearchOption.TopDirectoryOnly);

                if (subDirs == null || subDirs.Length == 0)
                    cbSubFolder.Items.Insert(0, "新素材信息-1");
                else
                {
                    foreach (string item in subDirs)
                        cbSubFolder.Items.Add(item.Replace(baseDir, ""));
                    string folderTemp = baseDir + "新素材信息-";
                    string newFoldername = folderTemp + "1";
                    int index = 1;
                    while (Directory.Exists(newFoldername))
                    {
                        index++;
                        newFoldername = folderTemp + index;
                    }
                    cbSubFolder.Items.Insert(0, newFoldername.Replace(baseDir, ""));
                }
                cbSubFolder.SelectedIndex = 0;
            }
        }

        public void UpdateData(Core.UI.ISmartSession smartSession)
        {
            this._smartSession = smartSession;
            MonitoringSession monitSession = _smartSession.MonitoringSession as MonitoringSession;
            ProductDef productDef = monitSession.ActiveMonitoringProduct.Definition;
            string _identify = productDef.Identify;
            _wDef = WorkspaceDefinitionFactory.GetWorkspaceDef(_identify);
            LoadData();
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
            {
                _catalogTreeView = null;
                return;
            }
            _radPageView.Pages.Clear();

            RadPageViewItemPage tabPage = new RadPageViewItemPage();
            tabPage.Text = "待生成报告素材";
            tabPage.Dock = DockStyle.Fill;
            _radPageView.Pages.Add(tabPage);

            RadTreeView ui = GetUserControl();
            tabPage.Controls.Add(ui);

            _catalogTreeView = new CatalogTreeView(ui, _wDef, chkHasToDb.Checked, dateTimePicker1.Value);

            lbMessage.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadData();
            CreateSubFoldername();
        }

        private RadTreeView GetUserControl()
        {
            RadTreeView tv = new RadTreeView();
            tv.ShowLines = true;
            tv.HideSelection = false;
            tv.Font = this.Font;
            tv.Dock = DockStyle.Fill;
            return tv;
        }

        private void btnToDb_Click(object sender, EventArgs e)
        {
            try
            {
                btnToDb.Enabled = false;
                if (_catalogTreeView == null)
                {
                    MsgBox.ShowInfo("没有数据");
                    return;
                }
                Dictionary<string, List<ICatalogItem>> catalogItems = _catalogTreeView.GetCheckedItem();
                if (catalogItems == null || catalogItems.Count == 0)
                {
                    MsgBox.ShowInfo("没有数据");
                    return;
                }
                OnProgress(0);
                string dir = string.Empty;
                foreach (string key in catalogItems.Keys)
                {
                    List<ICatalogItem> items = catalogItems[key];
                    for (int i = 0; i < items.Count; i++)
                    {
                        OnProgress((int)((i + 1.0) / items.Count));
                        dir = GenralReportInfos(items[i]);
                    }
                }

                MonitorProductInfo info = new MonitorProductInfo();
                //
                SaveToDb(null, info, dir);
                OnSendMessage("完成报告素材提交");
                MsgBox.ShowInfo("完成报告素材提交");
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

        /// <param name="catalog"></param>
        /// <returns></returns>
        private string GenralReportInfos(ICatalogItem catalog)
        {
            MonitorProductInfo info = new MonitorProductInfo();
            //fir ,fog
            info.productType = catalog.Info.GetPropertyValue("ProductIdentify");
            //tjcp:统计产品  xxcp;信息列表  sgcp:栅格产品 ztcp:专题产品   slcp:矢量产品 dhcp:动画产品
            info.productDataType = catalog.Info.GetPropertyValue("CatalogDef");         //VectorProduct、
            info.productIdentify = catalog.Info.GetPropertyValue("SubProductIdentify"); //DBLV,0CSR,"PLST";
            info.productIdentifyName = catalog.Info.GetPropertyValue("CatalogItemCN");  //"火点二值图";
            info.satellite = catalog.Info.GetPropertyValue("Satellite");
            info.sensor = catalog.Info.GetPropertyValue("Sensor");
            info.subProductType = "";                                                   //通常为空
            info.orbitDateTime = DateTime.Parse(catalog.Info.GetPropertyValue("OrbitDateTime"));
            info.productArea = catalog.Info.GetPropertyValue("Region");
            string dir = "\\" + dateTimePicker1.Value.ToString("yyyyMMdd") + "\\" + cbSubFolder.Text;
            if (catalog.Info.Properties.ContainsKey(CatalogTreeView.ToReportInfoKey))
                catalog.Info.Properties[CatalogTreeView.ToReportInfoKey] = CatalogTreeView.ToReportInfoValue;
            else
                catalog.Info.Properties.Add(CatalogTreeView.ToReportInfoKey, CatalogTreeView.ToReportInfoValue);
            string infoFilename = System.IO.Path.ChangeExtension(catalog.FileName, ".info");
            catalog.Info.SaveTo(infoFilename);
            CopyFile(dir, catalog.FileName);
            string productFilename = System.IO.Path.GetFileName(catalog.FileName);
            info.productFileName = productFilename;
            info.productFilePath = dir;
            return dir;
        }

        private void OnSendMessage(string msg)
        {
            lbMessage.Text = msg;
        }

        private void CopyFile(string dir, string file)
        {
            if (string.IsNullOrEmpty(_reportPath))
                return;
            string dstFilename = System.IO.Path.Combine(_reportPath + dir, System.IO.Path.GetFileName(file));
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(dstFilename)))
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dstFilename));
            if (System.IO.File.Exists(file))
                System.IO.File.Copy(file, dstFilename, true);
        }

        private void SaveToDb(string key, MonitorProductInfo info, string dir)
        {
            CopyQuickReportArgsFile(_reportPath + dir);
            string toDBConsole = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\Report\GeoDo.Mms.Tools.InfosCollectionDemo.exe";
            if (File.Exists(toDBConsole))
            {
                System.Diagnostics.Process exep = new System.Diagnostics.Process();
                exep.StartInfo = new System.Diagnostics.ProcessStartInfo(toDBConsole);
                exep.StartInfo.Arguments = _reportPath + dir + " " + QuickReportXML.tempQuickReportXML;//设定参数
                exep.StartInfo.UseShellExecute = false;//不使用系统外壳程序启动
                exep.StartInfo.RedirectStandardInput = false;//不重定向输入
                exep.StartInfo.RedirectStandardOutput = true;//重定向输出，而不是默认的显示在dos控制台上
                exep.StartInfo.CreateNoWindow = true;//不创建窗口
                exep.Start();
                exep.WaitForExit();
                exep.Close();
            }
        }

        private void CopyQuickReportArgsFile(string dstDir)
        {
            if (File.Exists(QuickReportXML.ReportArgsFile))
            {
                File.Copy(QuickReportXML.ReportArgsFile, dstDir + "\\QuickReportArg.txt", true);
                QuickReportXML.UpdateReportArgsFile();
            }
        }

        private void btnCheckedAll_Click(object sender, EventArgs e)
        {
            _catalogTreeView.CheckedALL();
        }

        private void btnCheckedNone_Click(object sender, EventArgs e)
        {
            _catalogTreeView.CheckedNone();
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
