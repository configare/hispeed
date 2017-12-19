using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.UHE
{
    public partial class UITabUHE : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 城市热效应按钮定义

        #region 判识相关按钮
        RadDropDownButtonElement dbtnLMCZData = new RadDropDownButtonElement();    //热效应计算
        RadDropDownButtonElement dbtnIIData = new RadDropDownButtonElement();      //指数计算
        #endregion

        #region 专题图相关按钮
        RadDropDownButtonElement btnMulImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnbinImage = new RadDropDownButtonElement();
        RadDropDownButtonElement gp2Image = new RadDropDownButtonElement();//二级应急响应
        #endregion

        #region 面积统计相关按钮
        RadButtonElement btnCurrentRegionArea = new RadButtonElement("当前区域");
        RadDropDownButtonElement dbtnDivision = new RadDropDownButtonElement();
        RadButtonElement btnLandType = new RadButtonElement("");
        RadButtonElement btnCustomArea = new RadButtonElement("自定义");
        #endregion

        RadButtonElement btnClose = new RadButtonElement("关闭城市热岛\n分析视图");
        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        #endregion

        public UITabUHE()
        {
            InitializeComponent();
            CreateRibbonBar();
        }

        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "城市热岛专题";
            _tab.Text = "城市热岛专题";
            _tab.Name = "城市热岛专题";
            _bar.CommandTabs.Add(_tab);

            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            dbtnLMCZData.Text = "城市热岛";
            dbtnLMCZData.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnLMCZData.ImageAlignment = ContentAlignment.TopCenter;
            dbtnLMCZData.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniLMCZCLM = new RadMenuItem("云检测");
            mniLMCZCLM.Click += new EventHandler(btnmniCLM_Click);
            dbtnLMCZData.Items.Add(mniLMCZCLM);
            RadMenuItem mniLMCZAutoGenerate = new RadMenuItem("自动计算");
            mniLMCZAutoGenerate.Click += new EventHandler(mniLMCZAutoGenerate_Click);
            dbtnLMCZData.Items.Add(mniLMCZAutoGenerate);
            RadMenuItem mniLMCZLst = new RadMenuItem("人机交互");
            mniLMCZLst.Click += new EventHandler(btnLMCZInteractive_Click);
            dbtnLMCZData.Items.Add(mniLMCZLst);
            rbgCheck.Items.Add(dbtnLMCZData);

            dbtnIIData.Text = "指数计算";
            dbtnIIData.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnIIData.ImageAlignment = ContentAlignment.TopCenter;
            dbtnIIData.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniIIInteractive = new RadMenuItem("强度指数");
            mniIIInteractive.Click += new EventHandler(btnIIInteractive_Click);
            dbtnIIData.Items.Add(mniIIInteractive);
            RadMenuItem mniInteractiveLst = new RadMenuItem("比例指数");
            mniInteractiveLst.Click += new EventHandler(mniInteractiveLst_Click);
            dbtnIIData.Items.Add(mniInteractiveLst);
            rbgCheck.Items.Add(dbtnIIData);

            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "专题产品";
            btnMulImage.Text = "多通道合成图";
            btnMulImage.ImageAlignment = ContentAlignment.TopCenter;
            btnMulImage.TextAlignment = ContentAlignment.BottomCenter;
            btnMulImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem mnuMulImage = new RadMenuItem("多通道合成图");
            mnuMulImage.Click += new EventHandler(mnuMulImage_Click);
            RadMenuItem mnuOMulImage = new RadMenuItem("多通道合成图（原始分辩率）");
            mnuOMulImage.Click += new EventHandler(mnuOMulImage_Click);
            btnMulImage.Items.AddRange(mnuMulImage, mnuOMulImage);
            rbgProduct.Items.Add(btnMulImage);

            dbtnbinImage.Text = "专题图";
            dbtnbinImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnbinImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnbinImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiImgLMCZ = new RadMenuItem("城市热岛监测图");
            dbtnbinImage.Items.Add(mhiImgLMCZ);
            mhiImgLMCZ.Click += new EventHandler(mhiImgLMCZ_Click);
            rbgProduct.Items.Add(dbtnbinImage);

            RadMenuItem mhiImgHFII = new RadMenuItem("热岛强度指数图");
            dbtnbinImage.Items.Add(mhiImgHFII);
            mhiImgHFII.Click += new EventHandler(mhiImgHFII_Click);
            rbgProduct.Items.Add(dbtnbinImage);

            RadMenuItem mhiBinImgCurrent = new RadMenuItem("高温图（当前区域）");
            dbtnbinImage.Items.Add(mhiBinImgCurrent);
            mhiBinImgCurrent.Click += new EventHandler(mhiBinImgCurrent_Click);
            RadMenuItem mhiBinImgCustom = new RadMenuItem("高温图（自定义）");
            dbtnbinImage.Items.Add(mhiBinImgCustom);
            mhiBinImgCustom.Click += new EventHandler(mhiBinImgCustom_Click);

            RadRibbonBarGroup rbgAreaStatic = new RadRibbonBarGroup();
            rbgAreaStatic.Text = "面积统计";
            btnCurrentRegionArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCurrentRegionArea.TextAlignment = ContentAlignment.BottomCenter;
            btnCurrentRegionArea.Click += new EventHandler(btnCurrentRegionArea_Click);
            rbgAreaStatic.Items.Add(btnCurrentRegionArea);
            dbtnDivision.Text = "行政区划";
            string provinceName = AreaStatProvider.GetAreaStatItemMenuName("行政区划");
            string landTypeName = AreaStatProvider.GetAreaStatItemMenuName("土地利用类型");
            string cityName = AreaStatProvider.GetAreaStatItemMenuName("三级行政区划");
            dbtnDivision.TextAlignment = ContentAlignment.BottomCenter;
            dbtnDivision.ImageAlignment = ContentAlignment.TopCenter;
            RadMenuItem mhiProvincBound = new RadMenuItem(provinceName);
            dbtnDivision.Items.Add(mhiProvincBound);
            mhiProvincBound.Click += new EventHandler(mhiProvincBound_Click);
            RadMenuItem mhiCity = new RadMenuItem(cityName);
            dbtnDivision.Items.Add(mhiCity);
            mhiCity.Click += new EventHandler(mhiCity_Click);
            rbgAreaStatic.Items.Add(dbtnDivision);
            btnLandType.Text = landTypeName;
            btnLandType.ImageAlignment = ContentAlignment.TopCenter;
            btnLandType.TextAlignment = ContentAlignment.BottomCenter;
            rbgAreaStatic.Items.Add(btnLandType);
            btnLandType.Click += new EventHandler(btnLandType_Click);
            //btnCustomArea.ImageAlignment = ContentAlignment.TopCenter;
            //btnCustomArea.TextAlignment = ContentAlignment.BottomCenter;
            //rbgAreaStatic.Items.Add(btnCustomArea);
            //btnCustomArea.Click += new EventHandler(btnCustomArea_Click);

            _tab.Items.Add(rbgCheck);
            _tab.Items.Add(rbgProduct);
            _tab.Items.Add(rbgAreaStatic);

            btnToDb.TextAlignment = ContentAlignment.BottomCenter;
            btnToDb.ImageAlignment = ContentAlignment.TopCenter;
            btnToDb.Click += new EventHandler(btnToDb_Click);
            RadRibbonBarGroup rbgToDb = new RadRibbonBarGroup();
            rbgToDb.Text = "产品入库";
            rbgToDb.Items.Add(btnToDb);
            _tab.Items.Add(rbgToDb);

            Controls.Add(_bar);
        }

        #region 初始化
        public object Content
        {
            get { return _tab; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            this._session = session;
            CreateThemeGraphRegionButton();
            CreateClose();
            SetImage();
        }

        private void CreateClose()
        {
            RadRibbonBarGroup rbgClose = new RadRibbonBarGroup();
            rbgClose.Text = "关闭";
            btnClose.ImageAlignment = ContentAlignment.TopCenter;
            btnClose.TextAlignment = ContentAlignment.BottomCenter;
            btnClose.Click += new EventHandler(btnClose_Click);
            rbgClose.Items.Add(btnClose);
            _tab.Items.Add(rbgClose);

        }

        public void UpdateStatus()
        { }

        private void SetImage()
        {
            dbtnDivision.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            btnMulImage.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnbinImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            gp2Image.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            btnCurrentRegionArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            btnLandType.Image = _session.UIFrameworkHelper.GetImage("system:landType.png");
            btnCustomArea.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
            dbtnLMCZData.Image = _session.UIFrameworkHelper.GetImage("system:rexiaoying.png");
            dbtnIIData.Image = _session.UIFrameworkHelper.GetImage("system:HFII.png");
        }
        #endregion

        #region 按钮相关事件

        #region 计算相关按钮事件

        /// <summary>
        /// 交互按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLMCZInteractive_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("LMCZ");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 交互按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnIIInteractive_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("HFII");
            GetCommandAndExecute(6602);
        }

        void mniInteractiveLst_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("UHPI");
            GetCommandAndExecute(6602);
        }

        void btnmniCLM_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0CLM");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 计算按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mniLMCZAutoGenerate_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("LMCZ");
            GetCommandAndExecute(6601);
        }

        #endregion

        #region 专题图相关按钮事件

        //陆表温度专题图 自定义原分辨率
        void mhiBinImgCustomOrginRes_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "DSDI", "", "DBLV", "陆表高温专题图模板", true, true) == null)
                return;
        }

        /// <summary>
        /// 自定义二值图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiBinImgCustom_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "CSDI", "", "DBLV", "陆表高温专题图模板", true, true) == null)
                return;
        }

        /// <summary>
        /// 热岛效应按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiImgLMCZ_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0LMC");
            ms.DoAutoExtract(true);
        }

        /// <summary>
        /// 热岛强度按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiImgHFII_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0HFI");
            ms.DoAutoExtract(true);
        }

        /// <summary>
        /// 当前区域二值图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiBinImgCurrent_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "DBLV", "陆表高温专题图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 监测示意图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mnuOMulImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "OMCS");
            ms.DoAutoExtract(true);
        }

        void mnuMulImage_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:城市热岛监测示意图,监测示意图,UHE,MCSI");
        }

        #endregion

        #region 面积统计相关按钮事件

        /// <summary>
        /// 当前区域面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCurrentRegionArea_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "CCAR");
            ms.DoManualExtract(true);
        }

        /// <summary>
        /// 自定义面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCustomArea_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.AreaStat(_session, "CCCA", "", true, true) == null)
                return;
        }

        /// <summary>
        /// 土地利用类型面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLandType_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "CLUT");
            ms.DoManualExtract(true);
        }

        /// <summary>
        /// 按省界面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiProvincBound_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0CBP");
            ms.DoManualExtract(true);
        }

        /// <summary>
        /// 按市县面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiCity_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0CCC");
            ms.DoManualExtract(true);
        }

        #endregion

        #region 关闭相关按钮事件
        /// <summary>
        /// 关闭按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnClose_Click(object sender, EventArgs e)
        {
            if (_session == null)
                return;
            _session.UIFrameworkHelper.Remove(_tab.Text);
            _session.UIFrameworkHelper.ActiveDefaultTab();
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveProduct(null);
            GetCommandAndExecute(6602);
        }
        #endregion

        //产品入库
        void btnToDb_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(39002);
            if (cmd != null)
                cmd.Execute();
        }

        private void GetCommandAndExecute(int cmdID)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute("UHE");
        }

        #endregion

        private void CreateThemeGraphRegionButton()
        {
            RadDropDownButtonElement _btnThemeGraphRegion = new RadDropDownButtonElement();
            ThemeGraphRegionFacctory.RegistToButton("UHE", _btnThemeGraphRegion, _tab, _session);
            _btnThemeGraphRegion.Image = _session.UIFrameworkHelper.GetImage("system:settings.png");
        }


    }
}
