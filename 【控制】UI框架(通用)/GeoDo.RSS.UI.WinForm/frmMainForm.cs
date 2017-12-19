using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using GeoDo.RSS.Core.UI;
using System.Threading;
using GeoDo.RSS.MIF.Core;
using System.Threading.Tasks;
using CodeCell.Bricks.Runtime;
using GeoDo.RSS.UI.AddIn.Theme;
using System.IO;

namespace GeoDo.RSS.UI.WinForm
{
    public partial class frmMainForm : Form
    {
        private RadRibbonBar _radRibbonBar;
        private UIFrameworkDefinition _uiDefinition;
        private ISmartSession _session;
        private UCSmartWindowManager _ucSmartWindowManager;
        private IStartProgress _startProgress = null;

        private string error;

        public frmMainForm(IStartProgress startProgress)
        {
            _startProgress = startProgress;
            ClearTemporalDir();
            try
            {
                _startProgress.PrintStartInfo("正在构造界面框架......");
                InitializeComponent();
                AttachEvents();
                LoadThemes();
                CreateUI();
                RegisterFileProcessors();
                GlobalInit();
                BuildGeoBaseIdx();
                Application.DoEvents();
                _startProgress.Stop();
                _startProgress = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(error + " : " + ex.Message);
            }
        }

        public ISmartSession Session
        {
            get { return _session; }
        }

        private void LoadThemes()
        {
            error = "注册监测分析专题";
            _startProgress.PrintStartInfo("正在注册监测分析专题......");
            ThemeGlobalManager.Register(ThemeParser.Parse());
        }

        private void GlobalInit()
        {
            error = "创建内存监视对象";
            GlobaCacherlInitializer.Init(_session);
            _startProgress.PrintStartInfo("正在创建内存监视对象......");
            MemoryIsEnoughChecker._session = _session;
            Parallel.Invoke(() => { PerformanceMonitoring.GetAvailableRAM(); });
        }

        private void BuildGeoBaseIdx()
        {
            error = "建立数字地球数据索引";
            _startProgress.PrintStartInfo("正在建立数字地球数据索引......");
            GeoVISIdxBuilder.BuildGeoVISIdx();
        }

        private void ClearTemporalDir()
        {
            error = "清理临时文件";
            _startProgress.PrintStartInfo("正在清理临时文件......");
            string dir = AppDomain.CurrentDomain.BaseDirectory + "TemporalFileDir";
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            dir = AppDomain.CurrentDomain.BaseDirectory + ".prjChche";
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
            dir = AppDomain.CurrentDomain.BaseDirectory + ".TEMP";
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }

        private void RegisterFileProcessors()
        {
            error = "注册数据驱动";
            _startProgress.PrintStartInfo("正在注册数据驱动......");
            OpenFileFactory.RegisterAll(_session);
        }

        private void AttachEvents()
        {
            SizeChanged += new EventHandler(frmMainForm_SizeChanged);
            Load += new EventHandler(frmMainForm_Load);
            FormClosed += new FormClosedEventHandler(frmMainForm_FormClosed);
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
        }

        void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            _session.PrintMessage(e.Exception);
        }

        void frmMainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseUnClosedWindows();
            _session.RecentFilesManager.SaveToDisk();
        }

        private void CloseUnClosedWindows()
        {
            var wnds = _session.SmartWindowManager.GetSmartWindows((wnd) => { return true; });
            if (wnds != null && wnds.Length > 0)
                foreach (ISmartWindow wnd in wnds)
                    wnd.Free();
        }

        void frmMainForm_Load(object sender, EventArgs e)
        {
            AdjustSizeAndLocation();
            _radRibbonBar.Expanded = false;
            Application.DoEvents();
            _radRibbonBar.Visible = true;
            Application.DoEvents();
            _radRibbonBar.Expanded = true;
            //
            ISmartSessionEvents evts = _session as ISmartSessionEvents;
            if (evts.OnSmartSessionLoaded != null)
                evts.OnSmartSessionLoaded(_session);
        }

        void frmMainForm_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
                AdjustSizeAndLocation();
            AdjustLocationOfTile();
        }

        private void AdjustSizeAndLocation()
        {
            WindowState = FormWindowState.Normal;
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);
            Height = Screen.PrimaryScreen.WorkingArea.Height;
            Width = Screen.PrimaryScreen.WorkingArea.Width;
        }

        private void CreateUI()
        {
            error = "CreateUI";
            _startProgress.PrintStartInfo("正在加载界面元数据......");
            _uiDefinition = new UIFrameworkDefinition();
            _startProgress.PrintStartInfo("正在创建工具栏......");
            CreateRibbonBar();
            _startProgress.PrintStartInfo("正在创建视窗区......");
            CreateRadDock();
            _startProgress.PrintStartInfo("正在创建会话......");
            CreateSession();
            LayoutDataFrameInitlializer.Init();
            _startProgress.PrintStartInfo("加载界面组件......");
            LoadUIFromXml();
            SetTileInfo();
        }

        private void CreateSession()
        {
            _session = new SmartSession(_ucSmartWindowManager as ISmartWindowManager);
            _ucSmartWindowManager.SetSession(_session);
            SmartApp.SmartSession = _session;
        }

        private void SetTileInfo()
        {
            txtTile.BringToFront();
            if (_uiDefinition.AppInfo != null && _uiDefinition.AppInfo.TileInfo != null && _uiDefinition.AppInfo.TileInfo.Tile != null)
                txtTile.Text = _uiDefinition.AppInfo.TileInfo.Tile;
            else
                txtTile.Text = string.Empty;
            txtTile.ForeColor = Color.FromArgb(66, 131, 198);
            txtTile.BackColor = Color.FromArgb(224, 228, 233);
        }

        private void AdjustLocationOfTile()
        {
            txtTile.Top = 2;
            SizeF fontSize = SizeF.Empty;
            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                fontSize = g.MeasureString(txtTile.Text, txtTile.Font);
            }
            txtTile.Left = (int)(Width / 2 - fontSize.Width / 2);
            txtTile.Top = (int)(_radRibbonBar.QuickAccessToolBarHeight / 2 - fontSize.Height / 2);
        }

        private void LoadUIFromXml()
        {
            UIFrameworkBuilder builder = new UIFrameworkBuilder(_session, _uiDefinition);
            (_session as SmartSession).SetUIFrameworkHelper(builder);
            builder.Build(_radRibbonBar);
        }

        private void CreateRadDock()
        {
            _ucSmartWindowManager = new UCSmartWindowManager();
            _ucSmartWindowManager.Dock = DockStyle.Fill;
            this.Controls.Add(_ucSmartWindowManager);
            this.Controls.SetChildIndex(_ucSmartWindowManager, 0);
        }

        private void CreateRibbonBar()
        {
            _radRibbonBar = new RadRibbonBar();
            _radRibbonBar.Visible = false;
            _radRibbonBar.Dock = DockStyle.Top;
            _radRibbonBar.StartButtonImage = null;
            _radRibbonBar.ShowHelpButton = false;
            _radRibbonBar.ApplicationMenuStyle = Telerik.WinControls.UI.ApplicationMenuStyle.BackstageView;
            this.Controls.Add(_radRibbonBar);
            this.Controls.SetChildIndex(_radRibbonBar, 0);
        }
    }
}
