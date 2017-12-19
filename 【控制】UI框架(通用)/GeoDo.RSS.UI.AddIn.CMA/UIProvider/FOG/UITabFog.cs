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
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    public partial class UITabFog : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 大雾按钮定义

        #region 判识相关按钮
        RadButtonElement btnRough = new RadButtonElement("粗判");
        RadButtonElement btnInteractive = new RadButtonElement("交互判识");
        RadDropDownButtonElement btnAutoGenerate = new RadDropDownButtonElement();
        #endregion

        #region 特征信息提取相关按钮
        RadButtonElement btnCSRCalc = new RadButtonElement("晴空反射率");
        RadSplitButtonElement dbtnQuantifyCalc = new RadSplitButtonElement();
        RadMenuItem OPTDRasterCalc;
        RadMenuItem LWPRasterCalc;
        RadMenuItem ERADRasterCalc;
        #endregion

        #region 专题图相关按钮
        RadButtonElement btnMulImage = new RadButtonElement("监测示意图");
        RadDropDownButtonElement dbtnbinImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnQuantifyImage = new RadDropDownButtonElement();
        #endregion

        #region 面积统计相关按钮
        RadButtonElement btnCurrentRegionArea = new RadButtonElement("当前区域");
        RadDropDownButtonElement dbtnDivision = new RadDropDownButtonElement();
        RadButtonElement btnLandType = new RadButtonElement("土地类型");
        RadButtonElement btnCustomArea = new RadButtonElement("自定义");
        #endregion

        #region 周期合成相关按钮
        RadButtonElement btnCurrentRegionCycTime = new RadButtonElement("当前区域");
        RadButtonElement btnCustomCycTime = new RadButtonElement("自定义");
        #endregion

        #region 频次统计相关按钮
        RadButtonElement btnCurrentRegionFreq = new RadButtonElement("当前区域");
        RadButtonElement btnCustomFreq = new RadButtonElement("自定义");
        #endregion

        #region 动画相关按钮
        RadButtonElement btnAnimationRegion = new RadButtonElement("当前区域");
        RadButtonElement btnAnimationCustom = new RadButtonElement("自定义");
        #endregion

        RadButtonElement btnClose = new RadButtonElement("关闭大雾监测\n分析视图");

        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        #endregion

        public UITabFog()
        {
            InitializeComponent();
            CreateRibbonBar();
        }

        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "大雾监测专题";
            _tab.Text = "大雾监测专题";
            _tab.Name = "大雾监测专题";
            _bar.CommandTabs.Add(_tab);

            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            rbgCheck.Text = "监测";
            rbgCheck.Items.Add(btnRough);
            btnRough.Click += new EventHandler(btnRough_Click);
            rbgCheck.Items.Add(btnInteractive);
            btnInteractive.Click += new EventHandler(btnInteractive_Click);
            foreach (RadButtonElement item in rbgCheck.Items)
            {
                item.ImageAlignment = ContentAlignment.TopCenter;
                item.TextAlignment = ContentAlignment.BottomCenter;
            }
            //
            btnAutoGenerate.Text = "快速生成";
            btnAutoGenerate.ImageAlignment = ContentAlignment.TopCenter;
            btnAutoGenerate.TextAlignment = ContentAlignment.BottomCenter;
            btnAutoGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem roughProduct = new RadMenuItem("粗判 + 专题产品");
            roughProduct.Click += new EventHandler(roughProduct_Click);
            btnAutoGenerate.Items.Add(roughProduct);
            RadMenuItem autoProduct = new RadMenuItem("专题产品");
            autoProduct.Click += new EventHandler(autoProduct_Click);
            btnAutoGenerate.Items.Add(autoProduct);
            rbgCheck.Items.Add(btnAutoGenerate);
            //

            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "专题产品";
            btnMulImage.ImageAlignment = ContentAlignment.TopCenter;
            btnMulImage.TextAlignment = ContentAlignment.BottomCenter;
            rbgProduct.Items.Add(btnMulImage);
            btnMulImage.Click += new EventHandler(btnMulImage_Click);
            dbtnbinImage.Text = "二值图";
            dbtnbinImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnbinImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnbinImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiBinImgCurrent = new RadMenuItem("当前区域");
            dbtnbinImage.Items.Add(mhiBinImgCurrent);
            mhiBinImgCurrent.Click += new EventHandler(mhiBinImgCurrent_Click);
            RadMenuItem mhiBinImgCustom = new RadMenuItem("自定义");
            dbtnbinImage.Items.Add(mhiBinImgCustom);
            mhiBinImgCustom.Click += new EventHandler(mhiBinImgCustom_Click);
            rbgProduct.Items.Add(dbtnbinImage);

            dbtnQuantifyImage.Text = "特征专题图";
            dbtnQuantifyImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnQuantifyImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnQuantifyImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem OPTDRasterImage = new RadMenuItem("光学厚度");
            dbtnQuantifyImage.Items.Add(OPTDRasterImage);
            OPTDRasterImage.Click += new EventHandler(OPTDRasterImage_Click);
            RadMenuItem LWPRasterImage = new RadMenuItem("液态水路径");
            dbtnQuantifyImage.Items.Add(LWPRasterImage);
            LWPRasterImage.Click += new EventHandler(LWPRasterImage_Click);
            RadMenuItem ERADRasterImage = new RadMenuItem("雾滴尺度");
            dbtnQuantifyImage.Items.Add(ERADRasterImage);
            ERADRasterImage.Click += new EventHandler(ERADRasterImage_Click);
            rbgProduct.Items.Add(dbtnQuantifyImage);

            RadRibbonBarGroup rbgQuantify = new RadRibbonBarGroup();
            rbgQuantify.Text = "特征信息提取";
            btnCSRCalc.ImageAlignment = ContentAlignment.TopCenter;
            btnCSRCalc.TextAlignment = ContentAlignment.BottomCenter;
            rbgQuantify.Items.Add(btnCSRCalc);
            btnCSRCalc.Click += new EventHandler(btnCSRCalc_Click);
            dbtnQuantifyCalc.Text = "特征量计算";
            dbtnQuantifyCalc.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnQuantifyCalc.ImageAlignment = ContentAlignment.TopCenter;
            dbtnQuantifyCalc.TextAlignment = ContentAlignment.BottomCenter;
            dbtnQuantifyCalc.AutoToolTip = true;
            RadMenuItem quantifyRasterCalc = new RadMenuItem("特征量");
            dbtnQuantifyCalc.Items.Add(quantifyRasterCalc);
            quantifyRasterCalc.Click += new EventHandler(quantifyRasterCalc_Click);
            quantifyRasterCalc.Visibility = ElementVisibility.Collapsed;
            OPTDRasterCalc = new RadMenuItem("光学厚度");
            dbtnQuantifyCalc.Items.Add(OPTDRasterCalc);
            OPTDRasterCalc.Click += new EventHandler(OPTDRasterCalc_Click);
            LWPRasterCalc = new RadMenuItem("液态水路径");
            dbtnQuantifyCalc.Items.Add(LWPRasterCalc);
            LWPRasterCalc.Click += new EventHandler(LWPRasterCalc_Click);
            ERADRasterCalc = new RadMenuItem("雾滴尺度");
            dbtnQuantifyCalc.Items.Add(ERADRasterCalc);
            dbtnQuantifyCalc.DefaultItem = dbtnQuantifyCalc.Items[0];
            ERADRasterCalc.Click += new EventHandler(ERADRasterCalc_Click);
            rbgQuantify.Items.Add(dbtnQuantifyCalc);

            RadRibbonBarGroup rbgAreaStatic = new RadRibbonBarGroup();
            rbgAreaStatic.Text = "面积统计";
            dbtnDivision.Text = "行政区划";
            dbtnDivision.TextAlignment = ContentAlignment.BottomCenter;
            dbtnDivision.ImageAlignment = ContentAlignment.TopCenter;
            RadMenuItem mhiCity = new RadMenuItem("按市县");
            dbtnDivision.Items.Add(mhiCity);
            mhiCity.Click += new EventHandler(mhiCity_Click);
            RadMenuItem mhiProvincBound = new RadMenuItem("按省界");
            dbtnDivision.Items.Add(mhiProvincBound);
            mhiProvincBound.Click += new EventHandler(mhiProvincBound_Click);
            btnCurrentRegionArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCurrentRegionArea.TextAlignment = ContentAlignment.BottomCenter;
            rbgAreaStatic.Items.Add(btnCurrentRegionArea);
            btnCurrentRegionArea.Click += new EventHandler(btnCurrentRegionArea_Click);
            rbgAreaStatic.Items.Add(dbtnDivision);
            btnLandType.ImageAlignment = ContentAlignment.TopCenter;
            btnLandType.TextAlignment = ContentAlignment.BottomCenter;
            rbgAreaStatic.Items.Add(btnLandType);
            btnLandType.Click += new EventHandler(btnLandType_Click);
            btnCustomArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCustomArea.TextAlignment = ContentAlignment.BottomCenter;
            rbgAreaStatic.Items.Add(btnCustomArea);
            btnCustomArea.Click += new EventHandler(btnCustomArea_Click);

            RadRibbonBarGroup rbgCycTimeStatic = new RadRibbonBarGroup();
            rbgCycTimeStatic.Text = "周期合成";
            btnCurrentRegionCycTime.ImageAlignment = ContentAlignment.TopCenter;
            btnCurrentRegionCycTime.TextAlignment = ContentAlignment.BottomCenter;
            rbgCycTimeStatic.Items.Add(btnCurrentRegionCycTime);
            btnCurrentRegionCycTime.Click += new EventHandler(btnCurrentRegionCycTime_Click);
            btnCustomCycTime.TextAlignment = ContentAlignment.BottomCenter;
            btnCustomCycTime.ImageAlignment = ContentAlignment.TopCenter;
            rbgCycTimeStatic.Items.Add(btnCustomCycTime);
            btnCustomCycTime.Click += new EventHandler(btnCustomCycTime_Click);

            RadRibbonBarGroup rbgFreqStatic = new RadRibbonBarGroup();
            rbgFreqStatic.Text = "频次统计";
            btnCurrentRegionFreq.ImageAlignment = ContentAlignment.TopCenter;
            btnCurrentRegionFreq.TextAlignment = ContentAlignment.BottomCenter;
            rbgFreqStatic.Items.Add(btnCurrentRegionFreq);
            btnCurrentRegionFreq.Click += new EventHandler(btnCurrentRegionFreq_Click);
            btnCustomFreq.TextAlignment = ContentAlignment.BottomCenter;
            btnCustomFreq.ImageAlignment = ContentAlignment.TopCenter;
            rbgFreqStatic.Items.Add(btnCustomFreq);
            btnCustomFreq.Click += new EventHandler(btnCustomFreq_Click);

            RadRibbonBarGroup rbgAnimation = new RadRibbonBarGroup();
            rbgAnimation.Text = "动画";
            btnAnimationRegion.ImageAlignment = ContentAlignment.TopCenter;
            btnAnimationRegion.TextAlignment = ContentAlignment.BottomCenter;
            rbgAnimation.Items.Add(btnAnimationRegion);
            btnAnimationRegion.Click += new EventHandler(btnAnimationRegion_Click);
            btnAnimationCustom.TextAlignment = ContentAlignment.BottomCenter;
            btnAnimationCustom.ImageAlignment = ContentAlignment.TopCenter;
            rbgAnimation.Items.Add(btnAnimationCustom);
            btnAnimationCustom.Click += new EventHandler(btnAnimationCustom_Click);

            RadRibbonBarGroup rbgClose = new RadRibbonBarGroup();
            rbgClose.Text = "关闭";
            btnClose.ImageAlignment = ContentAlignment.TopCenter;
            btnClose.TextAlignment = ContentAlignment.BottomCenter;
            btnClose.Click += new EventHandler(btnClose_Click);
            rbgClose.Items.Add(btnClose);

            _tab.Items.Add(rbgCheck);
            _tab.Items.Add(rbgQuantify);
            _tab.Items.Add(rbgProduct);
            _tab.Items.Add(rbgAreaStatic);
            _tab.Items.Add(rbgCycTimeStatic);
            _tab.Items.Add(rbgFreqStatic);
            _tab.Items.Add(rbgAnimation);

            btnToDb.TextAlignment = ContentAlignment.BottomCenter;
            btnToDb.ImageAlignment = ContentAlignment.TopCenter;
            btnToDb.Click += new EventHandler(btnToDb_Click);
            RadRibbonBarGroup rbgToDb = new RadRibbonBarGroup();
            rbgToDb.Text = "产品入库";
            rbgToDb.Items.Add(btnToDb);
            _tab.Items.Add(rbgToDb);

            _tab.Items.Add(rbgClose);
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
            SetImage();
        }

        public void UpdateStatus()
        { }

        private void SetImage()
        {
            btnRough.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            dbtnDivision.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            btnAutoGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnInteractive.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            btnMulImage.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnbinImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            btnCurrentRegionArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            btnLandType.Image = _session.UIFrameworkHelper.GetImage("system:landType.png");
            btnCustomArea.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnCustomFreq.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnCurrentRegionFreq.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
            //btnAnimationCustom.Image = _session.UIFrameworkHelper.GetImage(".png");
            btnAnimationRegion.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            OPTDRasterCalc.Image = _session.UIFrameworkHelper.GetImage("system:fogOPTD.png");
            LWPRasterCalc.Image = _session.UIFrameworkHelper.GetImage("system:fogLwp.png");
            ERADRasterCalc.Image = _session.UIFrameworkHelper.GetImage("system:fogErad.png");
            dbtnQuantifyImage.Image = _session.UIFrameworkHelper.GetImage("system:fogQuantify.png");
            dbtnQuantifyCalc.Image = _session.UIFrameworkHelper.GetImage("system:fogQuantifyCalc.png");
            btnCSRCalc.Image = _session.UIFrameworkHelper.GetImage("system:fogCSR.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnAnimationCustom.Image = _session.UIFrameworkHelper.GetImage("system:customVedio.png");
            btnCurrentRegionCycTime.Image = _session.UIFrameworkHelper.GetImage("system:fogCycTime.png");
            btnCustomCycTime.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
        }
        #endregion

        #region 按钮相关事件

        #region 判识相关按钮事件

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
        /// 粗判按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnRough_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExecute(6601);
        }
        
        /// <summary>
        /// 带粗判的快速生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void roughProduct_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "DBLV");
        }

        /// <summary>
        /// 快速生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void autoProduct_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "0CSR");
        }

        #endregion

        #region 特征信息提取相关按钮事件

        /// <summary>
        /// 晴空反射率提取按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCSRCalc_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0CSR");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 特征量信息提取按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void quantifyRasterCalc_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("QUTY");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 光学厚度计算按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OPTDRasterCalc_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("OPTD");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 雾滴尺度计算按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ERADRasterCalc_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("ERAD");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 液态水路径计算按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LWPRasterCalc_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0LWP");
            GetCommandAndExecute(6602);
        }

        #endregion

        #region 专题图相关按钮事件

        /// <summary>
        /// 自定义二值图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiBinImgCustom_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "CSDI", "", "DBLV", "大雾监测专题图模板", true, true) == null)
                return;
        }

        /// <summary>
        /// 当前区域二值图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiBinImgCurrent_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "DBLV", "大雾监测专题图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 监测示意图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMulImage_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:大雾监测示意图模版,监测示意图,FOG,MCSI");
        }


        /// <summary>
        /// 雾滴尺度专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ERADRasterImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "ERAI", "", "ERAD", "大雾雾滴尺度专题图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 液态水路径专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LWPRasterImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "LWPI", "", "0LWP", "大雾液态水路径专题图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 光学厚度专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OPTDRasterImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "OPTI", "", "OPTD", "大雾光学厚度专题图模板", false, true) == null)
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
            if (MIFCommAnalysis.AreaStat(_session, "CCAR", "", false, true) == null)
                return;
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
            if (MIFCommAnalysis.AreaStat(_session, "CLUT", "土地利用类型", false, true) == null)
                return;
        }

        /// <summary>
        /// 按省界面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiProvincBound_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.AreaStat(_session, "0CBP", "省级行政区划", false, true) == null)
                return;
        }

        /// <summary>
        /// 按市县面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiCity_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.AreaStat(_session, "0CCC", "分级行政区划", false, true) == null)
                return;
        }

        #endregion

        #region 周期合成相关按钮事件

        /// <summary>
        /// 自定义周期合成按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCustomCycTime_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CycleTimeStat(_session, "CCYI", "", "DBLV", "大雾周期统计专题图模板", true, true) == null)
                return;
        }

        /// <summary>
        /// 当前区域周期合成按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCurrentRegionCycTime_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CycleTimeStat(_session, "CYCI", "", "DBLV", "大雾周期统计专题图模板", false, true) == null)
                return;
        }

        #endregion

        #region 频次统计相关按钮事件

        /// <summary>
        /// 自定义频次统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCustomFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "CFRI", "", "DBLV", "大雾频次统计专题图模板", true, true) == null)
                return;
        }

        /// <summary>
        /// 当前区域频次统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCurrentRegionFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "FREQ", "", "DBLV", "大雾频次统计专题图模板", false, true) == null)
                return;
        }

        #endregion

        #region 动画相关按钮事件

        /// <summary>
        /// 自定义动画按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAnimationCustom_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.CreatAVI(_session, "template:大雾动画展示专题图,动画展示专题图,FOG,CMED");
        }

        /// <summary>
        /// 当前区域动画按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAnimationRegion_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.CreatAVI(_session, "template:大雾动画展示专题图,动画展示专题图,FOG,MEDI");
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
            cmd.Execute("FOG");
        }

        private void GetCommandAndExecute(int cmdID, string template)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute(template);
        }

        #endregion

    }
}
