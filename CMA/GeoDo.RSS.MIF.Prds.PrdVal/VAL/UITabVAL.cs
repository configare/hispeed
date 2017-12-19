using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls.UI;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    public partial class UITabVAL : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;
        #region 按钮定义
        //RadButtonElement btnValSite = new RadButtonElement("观测站数据");
        //RadButtonElement btnSatPrds = new RadButtonElement("同类卫星产品");
        RadButtonElement btnProDAT = new RadButtonElement("数据预处理");
        RadButtonElement btnValASL = new RadButtonElement("汽溶胶");
        RadButtonElement btnValLST = new RadButtonElement("陆表温度");
        RadButtonElement btnValSST = new RadButtonElement("海表温度");
        RadButtonElement btnValCLD = new RadButtonElement("云参数产品");
        RadButtonElement btnClose = new RadButtonElement("关闭产品评估专题");
        #endregion
        public UITabVAL()
        {
            InitializeComponent();
            CreateRibbonBar();
          
        }
        #region CreateUI
        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "产品评估监测专题";
            _tab.Text = "产品评估监测专题";
            _tab.Name = "产品评估监测专题";
            _bar.CommandTabs.Add(_tab);
            CreateMonGroup();        //产品评估组
            CreateProcessGroup();    //数据预处理组
            Controls.Add(_bar);
        }
        #endregion

        private void CreateProcessGroup()
        {
            RadRibbonBarGroup rbgPro = new RadRibbonBarGroup();
            rbgPro.Text = "产品预处理";
            btnProDAT.ImageAlignment = ContentAlignment.TopCenter;
            btnProDAT.TextAlignment = ContentAlignment.BottomCenter;
            btnProDAT.Click += new EventHandler(btnProDAT_Click);
            rbgPro.Items.Add(btnProDAT);
            _tab.Items.Add(rbgPro);
        }
        private void CreateMonGroup()
        {
            RadRibbonBarGroup rbgMon = new RadRibbonBarGroup();
            rbgMon.Text = "评估";
            //btnValSite.ImageAlignment = ContentAlignment.TopCenter;
            //btnValSite.TextAlignment = ContentAlignment.BottomCenter;
            //btnValSite.Click += new EventHandler(btnValSite_Click);
            //rbgMon.Items.Add(btnValSite);

            //btnSatPrds.ImageAlignment = ContentAlignment.TopCenter;
            //btnSatPrds.TextAlignment = ContentAlignment.BottomCenter;
            //btnSatPrds.Click += new EventHandler(btnSatPrds_Click);
            //rbgMon.Items.Add(btnSatPrds);

            btnValASL.ImageAlignment = ContentAlignment.TopCenter;
            btnValASL.TextAlignment = ContentAlignment.BottomCenter;
            btnValASL.Click += new EventHandler(btnValASL_Click);
            rbgMon.Items.Add(btnValASL);

            btnValLST.ImageAlignment = ContentAlignment.TopCenter;
            btnValLST.TextAlignment = ContentAlignment.BottomCenter;
            btnValLST.Click += new EventHandler(btnValLST_Click);
            rbgMon.Items.Add(btnValLST);

            btnValSST.ImageAlignment = ContentAlignment.TopCenter;
            btnValSST.TextAlignment = ContentAlignment.BottomCenter;
            btnValSST.Click += new EventHandler(btnValSST_Click);
            rbgMon.Items.Add(btnValSST);

            btnValCLD.ImageAlignment = ContentAlignment.TopCenter;
            btnValCLD.TextAlignment = ContentAlignment.BottomCenter;
            btnValCLD.Click += new EventHandler(btnValCLD_Click);
            rbgMon.Items.Add(btnValCLD);

            _tab.Items.Add(rbgMon);
        }
        //void btnValSite_Click(object sender, EventArgs e)
        //{
        //    IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
        //    ms.ChangeActiveSubProduct("SITE"); //站点数据
        //    GetCommandAndExecute(6602);
        //}
        //void btnSatPrds_Click(object sender, EventArgs e)
        //{
        //    IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
        //    ms.ChangeActiveSubProduct("SATE"); //卫星产品
        //    GetCommandAndExecute(6602);
        //}
        void btnProDAT_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("PROD");
            GetCommandAndExecute(6602);
        }
        void btnValASL_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("VASL"); 
            GetCommandAndExecute(6602);
        }
        void btnValSST_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("VSST");
            GetCommandAndExecute(6602);
        }
        void btnValLST_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("VLST");
            GetCommandAndExecute(6602);
        }
        void btnValCLD_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("VCLD");
            GetCommandAndExecute(6602);
        }
        public object Content
        {
            get { return _tab; }
        }
        public void Init(ISmartSession session, params object[] arguments)
        {
            this._session = session;
            CreateThemeGraphRegionButton();
            CreateCloseGroup();
            SetImage();
        }
        private void CreateThemeGraphRegionButton()
        {
            RadDropDownButtonElement _btnThemeGraphRegion = new RadDropDownButtonElement();
            ThemeGraphRegionFacctory.RegistToButton("VAL", _btnThemeGraphRegion, _tab, _session);
            _btnThemeGraphRegion.Image = _session.UIFrameworkHelper.GetImage("system:settings.png");
        }
        private void CreateCloseGroup()
        {
            RadRibbonBarGroup rbgClose = new RadRibbonBarGroup();
            rbgClose.Text = "关闭";
            btnClose.TextAlignment = ContentAlignment.BottomCenter;
            btnClose.ImageAlignment = ContentAlignment.TopCenter;
            btnClose.Click += new EventHandler(btnClose_Click);
            rbgClose.Items.Add(btnClose);
            _tab.Items.Add(rbgClose);
        }
        void btnClose_Click(object sender, EventArgs e)
        {
            if (_session == null)
                return;
            _session.UIFrameworkHelper.Remove(_tab.Text);
            _session.UIFrameworkHelper.ActiveDefaultTab();
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveProduct(null);
            GetCommandAndExecute(6602);
        }
        private void SetImage()
        {
            //btnSatPrds.Image = _session.UIFrameworkHelper.GetImage("system:map_edit.png");
            //btnValSite.Image = _session.UIFrameworkHelper.GetImage("system:Tool_Interpolation.png");
            btnProDAT.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            btnValCLD.Image = _session.UIFrameworkHelper.GetImage("system:cloud.png");
            btnValLST.Image = _session.UIFrameworkHelper.GetImage("system:surTemp.png");
            btnValSST.Image = _session.UIFrameworkHelper.GetImage("system:dem.png");
            btnValASL.Image = _session.UIFrameworkHelper.GetImage("system:Aerosol.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
        }
        public void UpdateStatus()
        {
        }

        private void GetCommandAndExecute(int cmdID)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute("VAL");
        }
    }
}
