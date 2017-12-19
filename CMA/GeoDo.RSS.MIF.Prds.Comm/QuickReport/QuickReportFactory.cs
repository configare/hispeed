using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.WinControls.UI;
using GeoDo.RSS.Core.UI;
using System.Drawing;
using Telerik.WinControls;
using GeoDo.RSS.UI.AddIn.ReportService;
using System.IO;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public class QuickReportFactory
    {
        public static void RegistToButton(string productIdentify, RibbonTab tab, ISmartSession session)
        {
            QuickReportFactory rad = new QuickReportFactory(tab, productIdentify, session);
            rad.AddQuickReportButton();
        }

        private RibbonTab _tab;
        private string _productIdentify;
        private ISmartSession _session;
        private RadDropDownButtonElement _btnReportResource;
        private RadDropDownButtonElement _btnQuickReport;
        private RadDropDownButtonElement _btnReportModel;
        private ReportTemplateInfo[] _templateInfos = null;
        private ReportTemplateInfo[] _templateInfoForMode = null;
        private string _reportTimeFile;
        private RadMenuItem _btnStart = null;

        private QuickReportFactory(RibbonTab tab, string productIdentify, ISmartSession session)
        {
            _tab = tab;
            _productIdentify = productIdentify;
            _session = session;
            _reportTimeFile = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\ProductArgs\" + _productIdentify + @"\QuickReportTime.txt";
            if (File.Exists(_reportTimeFile))
                File.Delete(_reportTimeFile);
        }

        //素材准备（开始、结束、素材提交）、快速报告、报告模板
        private void AddQuickReportButton()
        {
            AddBtnReportResource();
            GetDefalutReport();
            AddBtnQuickReport();
            AddBtnReportModel();

            RadRibbonBarGroup gpQuickReport = new RadRibbonBarGroup();
            gpQuickReport.Text = "专题报告输出";
            gpQuickReport.Items.AddRange(new RadItem[] { _btnReportResource, _btnQuickReport, _btnReportModel });
            _tab.Items.Add(gpQuickReport);
        }

        private void GetDefalutReport()
        {
            if (string.IsNullOrEmpty(_productIdentify))
                return;
            string outFilename = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\ProductArgs\" + _productIdentify + @"\QuickTempalteInfo.txt";
            _templateInfos = GetQuickReportModel.GetReportTemplateInfo(_productIdentify, outFilename);
            if (_templateInfos == null)
                _templateInfoForMode = null;
            else
            {
                _templateInfoForMode = new ReportTemplateInfo[_templateInfos.Length];
                for (int i = 0; i < _templateInfos.Length; i++)
                    _templateInfoForMode[i] = new ReportTemplateInfo(_templateInfos[i].ReportSubProType, _templateInfos[i].ReportTemplateName, -1);
            }
        }

        #region 素材准备按钮

        private void AddBtnReportResource()
        {
            _btnReportResource = new RadDropDownButtonElement();
            _btnReportResource.Image = _session.UIFrameworkHelper.GetImage("system:reportResource.png");
            _btnReportResource.Text = "素材准备";
            _btnReportResource.TextAlignment = ContentAlignment.BottomCenter;
            _btnReportResource.ImageAlignment = ContentAlignment.TopCenter;
            _btnStart = new RadMenuItem();
            _btnStart.Text = "开始...";
            _btnStart.Click += new EventHandler(btnStart_Click);
            _btnReportResource.Items.Add(_btnStart);
            RadMenuItem btnCommit = new RadMenuItem();
            btnCommit.Text = "素材提交";
            btnCommit.Click += new EventHandler(btnCommit_Click);
            _btnReportResource.Items.Add(btnCommit);
        }

        void btnCommit_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(400002);
            if (cmd != null)
                cmd.Execute();
        }

        void btnStart_Click(object sender, EventArgs e)
        {
            if (_btnStart.Text == "开始...")
            {
                if (File.Exists(_reportTimeFile))
                    File.Delete(_reportTimeFile);
                File.WriteAllLines(_reportTimeFile, new string[] { DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }, Encoding.Default);
                _btnStart.Text = "结束...";
            }
            else
            {
                if (!File.Exists(_reportTimeFile))
                    return;
                File.AppendAllLines(_reportTimeFile, new string[] { DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }, Encoding.Default);
                _btnStart.Text = "开始...";
            }
        }

        private void AfterQuickReport()
        {
            if (File.Exists(_reportTimeFile))
                File.Delete(_reportTimeFile);
        }

        #endregion

        #region 快速报告按钮

        private void AddBtnQuickReport()
        {
            _btnQuickReport = new RadDropDownButtonElement();
            _btnQuickReport.Image = _session.UIFrameworkHelper.GetImage("system:ReportDoc.png");
            _btnQuickReport.Text = "快速报告";
            _btnQuickReport.TextAlignment = ContentAlignment.BottomCenter;
            _btnQuickReport.ImageAlignment = ContentAlignment.TopCenter;
            FillBtnMenu(_btnQuickReport, _templateInfos);
            AfterQuickReport();
        }

        #endregion

        #region 报告模板按钮

        private void AddBtnReportModel()
        {
            _btnReportModel = new RadDropDownButtonElement();
            _btnReportModel.Image = _session.UIFrameworkHelper.GetImage("system:ReportModel.png");
            _btnReportModel.Text = "报告模板";
            _btnReportModel.TextAlignment = ContentAlignment.BottomCenter;
            _btnReportModel.ImageAlignment = ContentAlignment.TopCenter;
            FillBtnMenu(_btnReportModel, _templateInfoForMode);
        }

        #endregion

        private void FillBtnMenu(RadDropDownButtonElement btn, ReportTemplateInfo[] templateInfos)
        {
            if (templateInfos == null)
                return;
            string hardItem = templateInfos[0].ReportSubProType;
            RadMenuHeaderItem headerItem = new RadMenuHeaderItem(hardItem);
            btn.Items.Add(headerItem);
            RadMenuItem menuItem = null;
            foreach (ReportTemplateInfo item in templateInfos)
            {
                if (hardItem == item.ReportSubProType)
                {
                    menuItem = new RadMenuItem(item.ReportTemplateName);
                    menuItem.Tag = item;
                    menuItem.Click += new EventHandler(menuItem_Click);
                    btn.Items.Add(menuItem);
                }
                else
                {
                    headerItem = new RadMenuHeaderItem(item.ReportSubProType);
                    btn.Items.Add(headerItem);
                }
            }
        }

        void menuItem_Click(object sender, EventArgs e)
        {
            if (_session.ProgressMonitorManager.DefaultProgressMonitor != null)
                _session.ProgressMonitorManager.DefaultProgressMonitor.Start(false);
            if (_session.ProgressMonitorManager.DefaultProgressMonitor != null)
                _session.ProgressMonitorManager.DefaultProgressMonitor.Boost(20, "正在导出当前专题图...");
            //保存当前所有专题图
            ICommand cmd = _session.CommandEnvironment.Get(36604);
            if (cmd != null)
                cmd.Execute(_productIdentify, new string[] { "BMP", "false" });
            //
            if (_session.ProgressMonitorManager.DefaultProgressMonitor != null)
                _session.ProgressMonitorManager.DefaultProgressMonitor.Boost(40, "正在整理文档资料，请稍后...");
            ReportTemplateInfo rti = (sender as RadMenuItem).Tag as ReportTemplateInfo;
            QuickReportXML.WriteTempQuickReport(rti);
            QuickReportXML.WriteQuickReportArgs(rti, _reportTimeFile);
            if (!File.Exists(_reportTimeFile))
            {
                if (_session.ProgressMonitorManager.DefaultProgressMonitor != null)
                    _session.ProgressMonitorManager.DefaultProgressMonitor.Finish();
                btnCommit_Click(null, null);
            }
            else
            {
                SearchData();
            }
            if (File.Exists(_reportTimeFile))
            {
                File.Delete(_reportTimeFile);
                _btnStart.Text = "开始...";
            }
        }

        private void SearchData()
        {
            string dir = Path.Combine(MifEnvironment.GetWorkspaceDir(), _productIdentify);
            string data = DateTime.Now.ToString("yyyy-MM-dd");
            string prdRootDir = Path.Combine(dir, data);
            string[] files = Directory.GetFiles(prdRootDir, "*.*", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return;
            DateTime startTime, endTime;
            GetReportTime(out startTime, out  endTime);
            List<string> fileList = new List<string>();
            string reportDir = CreateSubFoldername();
            RasterIdentify rid = null;
            foreach (string file in files)
            {
                rid = new RasterIdentify(file);
                if (rid.GenerateDateTime >= startTime && rid.GenerateDateTime <= endTime)
                {
                    fileList.Add(file);
                    CopyFile(reportDir, file);
                }
            }
            SaveToDb(reportDir);
        }

        private void GetReportTime(out DateTime startTime, out DateTime endTime)
        {
            startTime = DateTime.MinValue;
            endTime = DateTime.MaxValue;
            string[] reportTime = File.ReadAllLines(_reportTimeFile, Encoding.Default);
            if (reportTime == null || reportTime.Length == 0)
                return;
            startTime = DateTime.Parse(reportTime[0]);
            if (reportTime.Length > 1)
                endTime = DateTime.Parse(reportTime[1]);
        }

        private string CreateSubFoldername()
        {
            string baseDir = MifEnvironment.GetReportDir() + "\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            string folderTemp = baseDir + "新素材信息-";
            string newFoldername = folderTemp + "1";
            int index = 1;
            while (Directory.Exists(newFoldername))
            {
                index++;
                newFoldername = folderTemp + index;
            }
            Directory.CreateDirectory(newFoldername);
            return newFoldername;
        }

        private void CopyFile(string dir, string file)
        {
            if (string.IsNullOrEmpty(dir))
                return;
            string dstFilename = System.IO.Path.Combine(dir, System.IO.Path.GetFileName(file));
            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(dstFilename)))
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dstFilename));
            if (System.IO.File.Exists(file))
                System.IO.File.Copy(file, dstFilename, true);
        }

        private void SaveToDb(string dir)
        {
            try
            {
                if (_session.ProgressMonitorManager.DefaultProgressMonitor != null)
                    _session.ProgressMonitorManager.DefaultProgressMonitor.Start(true);
                if (_session.ProgressMonitorManager.DefaultProgressMonitor != null)
                    _session.ProgressMonitorManager.DefaultProgressMonitor.Boost(50, "正在进行资料入库，请稍后...");
                CopyQuickReportArgsFile(dir);
                string toDBConsole = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\Report\GeoDo.Mms.Tools.InfosCollectionDemo.exe";
                if (File.Exists(toDBConsole))
                {
                    System.Diagnostics.Process exep = new System.Diagnostics.Process();
                    exep.StartInfo = new System.Diagnostics.ProcessStartInfo(toDBConsole);
                    exep.StartInfo.Arguments = dir + " " + QuickReportXML.tempQuickReportXML;//设定参数
                    exep.StartInfo.UseShellExecute = false;//不使用系统外壳程序启动
                    exep.StartInfo.RedirectStandardInput = false;//不重定向输入
                    exep.StartInfo.RedirectStandardOutput = true;//重定向输出，而不是默认的显示在dos控制台上
                    exep.StartInfo.CreateNoWindow = true;//不创建窗口
                    exep.Start();
                    exep.WaitForExit();
                    exep.Close();
                }
            }
            finally
            {
                if (_session.ProgressMonitorManager.DefaultProgressMonitor != null)
                    _session.ProgressMonitorManager.DefaultProgressMonitor.Finish();
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
    }

}
