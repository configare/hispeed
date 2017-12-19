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

namespace GeoDo.RSS.MIF.Prds.LST
{
    public partial class UITabLST : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 陆表高温按钮定义

        #region 判识相关按钮
        RadDropDownButtonElement dbtnComputeData = new RadDropDownButtonElement(); //高温计算
        RadDropDownButtonElement dbtnLMCZData = new RadDropDownButtonElement();    //热效应计算
        RadDropDownButtonElement dbtnIIData = new RadDropDownButtonElement();      //指数计算
        RadDropDownButtonElement dbtnDataCL = new RadDropDownButtonElement();      //数据合成
        #endregion

        #region 专题图相关按钮
        RadDropDownButtonElement btnMulImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnbinImage = new RadDropDownButtonElement();
        RadDropDownButtonElement gp2Image = new RadDropDownButtonElement();//二级应急响应
        #endregion

        #region 面积统计相关按钮
        RadButtonElement btnCurrentRegionArea = new RadButtonElement("当前区域");
        RadDropDownButtonElement dbtnDivision = new RadDropDownButtonElement();
        RadButtonElement btnLandType = new RadButtonElement();
        RadButtonElement btnCustomArea = new RadButtonElement("自定义");
        #endregion

        #region 周期合成相关按钮
        RadDropDownButtonElement btnCYCA = new RadDropDownButtonElement();
        #endregion

        RadButtonElement btnClose = new RadButtonElement("关闭陆表高温\n分析视图");
        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        #endregion

        public UITabLST()
        {
            InitializeComponent();
            CreateRibbonBar();
        }

        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "陆表高温专题";
            _tab.Text = "陆表高温专题";
            _tab.Name = "陆表高温专题";
            _bar.CommandTabs.Add(_tab);

            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            rbgCheck.Text = "数据生产";
            dbtnComputeData.Text = "高温计算";
            dbtnComputeData.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnComputeData.ImageAlignment = ContentAlignment.TopCenter;
            dbtnComputeData.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniCLM = new RadMenuItem("云检测");
            mniCLM.Click += new EventHandler(btnmniCLM_Click);
            dbtnComputeData.Items.Add(mniCLM);
            RadMenuItem mniAutoGenerate = new RadMenuItem("自动计算");
            mniAutoGenerate.Click += new EventHandler(mniAutoGenerate_Click);
            dbtnComputeData.Items.Add(mniAutoGenerate);
            RadMenuItem mniLst = new RadMenuItem("人机交互（修正）");
            mniLst.Click += new EventHandler(btnInteractive_Click);
            dbtnComputeData.Items.Add(mniLst);

            RadMenuItem mniLstAnlysis = new RadMenuItem("产品数据分析");
            mniLstAnlysis.Click += new EventHandler(mniLstAnlysis_Click);
            dbtnComputeData.Items.Add(mniLstAnlysis);
            rbgCheck.Items.Add(dbtnComputeData);

            dbtnDataCL.Text = "数据计算";
            dbtnDataCL.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnDataCL.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDataCL.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem btnAvgMake = new RadMenuItem("平均值计算");
            btnAvgMake.Click += new EventHandler(btnAvgMake_Click);
            dbtnDataCL.Items.Add(btnAvgMake);
            RadMenuItem btnMaxMake = new RadMenuItem("最大值计算");
            btnMaxMake.Click += new EventHandler(btnMaxMake_Click);
            dbtnDataCL.Items.Add(btnMaxMake);
            RadMenuItem btnMinMake = new RadMenuItem("最小值计算");
            btnMinMake.Click += new EventHandler(btnMinMake_Click);
            dbtnDataCL.Items.Add(btnMinMake);
            RadMenuItem btnCYAnomalies = new RadMenuItem("差异计算");
            btnCYAnomalies.Click += new EventHandler(btnCYAnomalies_Click);
            dbtnDataCL.Items.Add(btnCYAnomalies);
            RadMenuItem btnAnomalies = new RadMenuItem("距平计算");
            btnAnomalies.Click += new EventHandler(btnAnomalies_Click);
            dbtnDataCL.Items.Add(btnAnomalies);
            rbgCheck.Items.Add(dbtnDataCL);

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
            RadMenuItem mhiBinImgCurrent = new RadMenuItem("高温图（当前区域）");
            dbtnbinImage.Items.Add(mhiBinImgCurrent);
            mhiBinImgCurrent.Click += new EventHandler(mhiBinImgCurrent_Click);
            RadMenuItem mhiBinImgCustom = new RadMenuItem("高温图（自定义）");
            dbtnbinImage.Items.Add(mhiBinImgCustom);
            mhiBinImgCustom.Click += new EventHandler(mhiBinImgCustom_Click);

            RadMenuItem mhiRegionImgCurrent = new RadMenuItem("强度图");
            dbtnbinImage.Items.Add(mhiRegionImgCurrent);
            mhiRegionImgCurrent.Click += new EventHandler(mhiRegionImgCurrent_Click);

            RadMenuItem mhiLMCZBinImgCustomOrginRes = new RadMenuItem("差异图");
            dbtnbinImage.Items.Add(mhiLMCZBinImgCustomOrginRes);
            mhiLMCZBinImgCustomOrginRes.Click += new EventHandler(mhiImgCustom_Click);

            RadMenuItem mhiDBLVTempImgCurrent = new RadMenuItem("高温图(印巴临时)");
            dbtnbinImage.Items.Add(mhiDBLVTempImgCurrent);
            mhiDBLVTempImgCurrent.Click += new EventHandler(mhiDBLVTempImgCurrent_Click);

            RadMenuItem mhiCZImgCurrent = new RadMenuItem("差异图(印巴临时)");
            dbtnbinImage.Items.Add(mhiCZImgCurrent);
            mhiCZImgCurrent.Click += new EventHandler(mhiCZImgCurrent_Click);

            RadMenuItem mhiBinImgCustomOrginRes = new RadMenuItem("距平图");
            dbtnbinImage.Items.Add(mhiBinImgCustomOrginRes);
            mhiBinImgCustomOrginRes.Click += new EventHandler(mhiBinImgCustomOrigin_Click);
            RadMenuItem mhiTimeImgCurrent = new RadMenuItem("高温天数图（当前区域）");
            dbtnbinImage.Items.Add(mhiTimeImgCurrent);
            mhiTimeImgCurrent.Click += new EventHandler(mhiTimeImgCurrent_Click);
            RadMenuItem mhiTimeImgByPro = new RadMenuItem("高温天数图（基于产品）");
            dbtnbinImage.Items.Add(mhiTimeImgByPro);
            mhiTimeImgByPro.Click += new EventHandler(mhiTimeImgByPro_Click);
            RadMenuItem mhiLowTimeImgCurrent = new RadMenuItem("低温天数图（当前区域）");
            dbtnbinImage.Items.Add(mhiLowTimeImgCurrent);
            mhiLowTimeImgCurrent.Click += new EventHandler(mhiLowTimeImgCurrent_Click);

            //RadMenuItem mhiImgLMCZ = new RadMenuItem("热效应图");
            //dbtnbinImage.Items.Add(mhiImgLMCZ);
            //mhiImgLMCZ.Click += new EventHandler(mhiImgLMCZ_Click);
            //rbgProduct.Items.Add(dbtnbinImage);

            //RadMenuItem mhiImgHFII = new RadMenuItem("热岛强度图");
            //dbtnbinImage.Items.Add(mhiImgHFII);
            //mhiImgHFII.Click += new EventHandler(mhiImgHFII_Click);
            rbgProduct.Items.Add(dbtnbinImage);

            RadRibbonBarGroup rbgAreaStatic = new RadRibbonBarGroup();
            rbgAreaStatic.Text = "面积统计";
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
            btnCurrentRegionArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCurrentRegionArea.TextAlignment = ContentAlignment.BottomCenter;
            rbgAreaStatic.Items.Add(btnCurrentRegionArea);
            btnCurrentRegionArea.Click += new EventHandler(btnCurrentRegionArea_Click);
            rbgAreaStatic.Items.Add(dbtnDivision);
            btnLandType.Text = landTypeName;
            btnLandType.ImageAlignment = ContentAlignment.TopCenter;
            btnLandType.TextAlignment = ContentAlignment.BottomCenter;
            rbgAreaStatic.Items.Add(btnLandType);
            btnLandType.Click += new EventHandler(btnLandType_Click);
            btnCustomArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCustomArea.TextAlignment = ContentAlignment.BottomCenter;
            //rbgAreaStatic.Items.Add(btnCustomArea);
            btnCustomArea.Click += new EventHandler(btnCustomArea_Click);

            RadRibbonBarGroup rbgCycPro = new RadRibbonBarGroup();
            rbgCycPro.Text = "周期产品";
            btnCYCA.Text = "周期计算";
            btnCYCA.ArrowPosition = DropDownButtonArrowPosition.Right;
            btnCYCA.ImageAlignment = ContentAlignment.TopCenter;
            btnCYCA.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiAllPro = new RadMenuItem("自定义");
            btnCYCA.Items.Add(mhiAllPro);
            mhiAllPro.Click += new EventHandler(mhiAllPro_Click);
            rbgCycPro.Items.Add(btnCYCA);

            _tab.Items.Add(rbgCheck);
            _tab.Items.Add(rbgProduct);
            _tab.Items.Add(rbgAreaStatic);
            _tab.Items.Add(rbgCycPro);

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
            dbtnComputeData.Image = _session.UIFrameworkHelper.GetImage("system:LstCalc.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
            dbtnDataCL.Image = _session.UIFrameworkHelper.GetImage("system:wydpz.png");
            dbtnLMCZData.Image = _session.UIFrameworkHelper.GetImage("system:rexiaoying.png");
            dbtnIIData.Image = _session.UIFrameworkHelper.GetImage("system:HFII.png");
            btnCYCA.Image = _session.UIFrameworkHelper.GetImage("system:fogCycTime.png");
        }
        #endregion

        #region 按钮相关事件

        #region 计算相关按钮事件

        /// <summary>
        /// 交互按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnInteractive_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExecute(6602);
        }

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

        void mniLstAnlysis_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("THAN");
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
        void mniAutoGenerate_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExecute(6601);
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

        /// <summary>
        /// 最大值合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMaxMake_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0MAX");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 最小值合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMinMake_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0MIN");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 平均值合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAvgMake_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0AVG");
            GetCommandAndExecute(6602);
        }

        void btnAnomalies_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("ANMI");
            GetCommandAndExecute(6602);
        }

        void btnCYAnomalies_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("CHAZ");
            GetCommandAndExecute(6602);
        }
        #endregion

        #region 周期产品

        /// <summary>
        /// 自定义周期产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiAllPro_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("CYCA");
            GetCommandAndExecute(6602);
        }

        #endregion

        #region 专题图相关按钮事件

        void mhiBinImgCustomOrigin_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "ONMI", "LSTANMI", "ANMI", "陆表高温监测示意图_二级应急", true, true) == null)
                return;
        }

        //陆表温度专题图 自定义原分辨率
        void mhiBinImgCustomOrginRes_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "DSDI", "", "DBLV", "陆表高温专题图模板", true, true) == null)
                return;
        }

        //对比分析图
        void mhiImgCustom_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "CNMI");
            ms.DoAutoExtract(true);
        }

        //对比分析图(印巴临时)
        void mhiCZImgCurrent_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "TCNI");
            ms.DoAutoExtract(true);
        }

        //对比分析图(印巴临时)
        void mhiDBLVTempImgCurrent_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "TDBI");
            ms.DoAutoExtract(true);
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
            //if (MIFCommAnalysis.CreateThemeGraphy(_session, "0LMC", "", "LMCZ", "陆表高温热效应图模板", false, true) == null)
            //    return;
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

        void mhiTimeImgByPro_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "LTFI", "", "LTFR", "高温天数专题图模板", false, true) == null)
                return;
        }

        void mhiTimeImgCurrent_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("LTFR");
            GetCommandAndExecute(6602);
        }

        void mhiLowTimeImgCurrent_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("LTLR");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 当前区域分布图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiRegionImgCurrent_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SRI", "", "DBLV", "陆表高温强度专题图模板", false, true) == null)
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
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:陆表高温监测示意图,监测示意图,LST,MCSI");
        }
        /// <summary>
        /// 日合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DayRasterImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "ADSI", "", "DBLV", "日合成专题图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 周合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Days7RasterImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "SDSI", "", "DBLV", "周合成专题图模板", false, true) == null)
                return;
        }


        /// <summary>
        /// 旬合成专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Days10RasterImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "TDSI", "", "0TDS", "旬合成专题图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 月合成专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Days30RasterImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "TTDI", "", "TTDS", "月合成专题图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 季合成专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Days90RasterImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "NTDI", "", "NTDS", "季合成专题图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 年合成专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Days365RasterImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0YAI", "", "0YAS", "年合成专题图模板", false, true) == null)
                return;
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

        #region 周期统计相关按钮事件

        /// <summary>
        /// 旬合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Days10Raster_Click(object sender, EventArgs e)
        { }

        /// <summary>
        /// 周合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Days7Raster_Click(object sender, EventArgs e)
        { }

        /// <summary>
        /// 月合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Days30Raster_Click(object sender, EventArgs e)
        { }

        /// <summary>
        /// 季合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Days90Raster_Click(object sender, EventArgs e)
        { }

        /// <summary>
        /// 年合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Days365Raster_Click(object sender, EventArgs e)
        { }
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
            cmd.Execute("LST");
        }

        #endregion

        private void CreateThemeGraphRegionButton()
        {
            RadDropDownButtonElement _btnThemeGraphRegion = new RadDropDownButtonElement();
            ThemeGraphRegionFacctory.RegistToButton("LST", _btnThemeGraphRegion, _tab, _session);
            _btnThemeGraphRegion.Image = _session.UIFrameworkHelper.GetImage("system:settings.png");
        }


    }
}
