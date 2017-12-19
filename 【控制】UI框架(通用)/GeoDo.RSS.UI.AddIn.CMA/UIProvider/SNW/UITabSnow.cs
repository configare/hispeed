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
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    public partial class UITabSnow : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 积雪按钮定义
        RadButtonElement btnRough = new RadButtonElement("粗判");
        RadDropDownButtonElement dbtnSave = new RadDropDownButtonElement();
        RadButtonElement btnInteractive = new RadButtonElement("交互判识");
        RadButtonElement btnMulImage = new RadButtonElement("多通道合成图");
        RadButtonElement btnNetImage = new RadButtonElement("网络合成图");
        RadButtonElement btnNetBin = new RadButtonElement("网络二值图");
        RadDropDownButtonElement dbtnbinImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnDivision = new RadDropDownButtonElement();
        RadButtonElement btnCurrentRegionArea = new RadButtonElement("当前区域");
        RadButtonElement btnLandType = new RadButtonElement("土地类型");
        RadButtonElement btnCustomArea = new RadButtonElement("自定义");
        RadButtonElement btnCurrentRegionFreq = new RadButtonElement("当前区域");
        RadButtonElement btnCustomFreq = new RadButtonElement("自定义");
        RadButtonElement btnCurrentRegionDegree = new RadButtonElement("当前区域");
        RadDropDownButtonElement dbtnDivisionDegree = new RadDropDownButtonElement();
        RadButtonElement btnLandTypeDegree = new RadButtonElement("土地类型");
        RadButtonElement btnCustomDegree = new RadButtonElement("自定义");
        RadButtonElement btnAnimationRegion = new RadButtonElement("当前区域");
        RadButtonElement btnAnimationCustom = new RadButtonElement("自定义");
        RadButtonElement btnClose = new RadButtonElement("关闭积雪监测\n分析视图");
        RadButtonElement btnCompare = new RadButtonElement("对比分析图");
        RadButtonElement btnDayMax = new RadButtonElement("日最大合成图");
        RadButtonElement btnToDb = new RadButtonElement("产品入库");

        /// <summary>
        /// 日常业务-专题图输出
        /// </summary>
        RadButtonElement btnDailyWorkNetImage = new RadButtonElement("网络\n合成图");
        RadButtonElement btnDailyWorkNetBin = new RadButtonElement("网络\n二值图");
        RadButtonElement btnSetRange = new RadButtonElement("范围设定");

        #endregion

        public UITabSnow()
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
            _tab.Title = "积雪监测专题";
            _tab.Text = "积雪监测专题";
            _tab.Name = "积雪监测专题";
            _bar.CommandTabs.Add(_tab);
            CreateCheckGroup();
            CreateDailyWorkGroup();
            CreateProductGroup();
            CreateCompareAnalysisGroup();
            CreateAreaStaticGroup();
            CreateFreqStatic();
            CreateDegreeGroup();
            CreateAnimationGroup();

            
            btnToDb.TextAlignment = ContentAlignment.BottomCenter;
            btnToDb.ImageAlignment = ContentAlignment.TopCenter;
            btnToDb.Click += new EventHandler(btnToDb_Click);
            RadRibbonBarGroup rbgToDb = new RadRibbonBarGroup();
            rbgToDb.Text = "产品入库";
            rbgToDb.Items.Add(btnToDb);
            _tab.Items.Add(rbgToDb);

            CreateCloseGroup();
            Controls.Add(_bar);
        }

        private void CreateDailyWorkGroup()
        {
            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "日常工作";

            btnSetRange.ImageAlignment = ContentAlignment.TopCenter;
            btnSetRange.TextAlignment = ContentAlignment.BottomCenter;
            btnSetRange.Click += new EventHandler(btnSetRange_Click);
            
            btnDailyWorkNetImage.ImageAlignment = ContentAlignment.TopCenter;
            btnDailyWorkNetImage.TextAlignment = ContentAlignment.BottomCenter;
            btnDailyWorkNetImage.Click += new EventHandler(btnDailyWorkNetImage_Click);

            btnDailyWorkNetBin.ImageAlignment = ContentAlignment.TopCenter;
            btnDailyWorkNetBin.TextAlignment = ContentAlignment.BottomCenter;
            btnDailyWorkNetBin.Click += new EventHandler(btnDailyWorkNetBin_Click);

            rbgProduct.Items.Add(btnSetRange);
            rbgProduct.Items.Add(btnDailyWorkNetImage);
            rbgProduct.Items.Add(btnDailyWorkNetBin);
            _tab.Items.Add(rbgProduct);
        }

        private void CreateCheckGroup()
        {
            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            rbgCheck.Text = "监测";
            btnRough.Click += new EventHandler(btnRough_Click);
            rbgCheck.Items.Add(btnRough);
            btnRough.TextAlignment = ContentAlignment.BottomCenter;
            btnRough.ImageAlignment = ContentAlignment.TopCenter;
            btnInteractive.Click += new EventHandler(btnInteractive_Click);
            rbgCheck.Items.Add(btnInteractive);
            btnInteractive.TextAlignment = ContentAlignment.BottomCenter;
            btnInteractive.ImageAlignment = ContentAlignment.TopCenter;
            dbtnSave.Text = "快速生成";
            dbtnSave.ImageAlignment = ContentAlignment.TopCenter;
            dbtnSave.TextAlignment = ContentAlignment.BottomCenter;
            dbtnSave.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem roughProGenerate = new RadMenuItem("粗判+专题产品");
            roughProGenerate.Click += new EventHandler(roughProGenerate_Click);
            dbtnSave.Items.Add(roughProGenerate);
            RadMenuItem proGenerate = new RadMenuItem("专题产品");
            proGenerate.Click += new EventHandler(proGenerate_Click);
            dbtnSave.Items.Add(proGenerate);
            rbgCheck.Items.Add(dbtnSave);
            dbtnSave.ImageAlignment = ContentAlignment.TopCenter;
            dbtnSave.TextAlignment = ContentAlignment.BottomCenter;
            _tab.Items.Add(rbgCheck);
        }

        private void CreateProductGroup()
        {
            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "专题产品";
            btnSetRange.TextAlignment = ContentAlignment.BottomCenter;
            btnSetRange.ImageAlignment = ContentAlignment.TopCenter;
            btnSetRange.Click += new EventHandler(btnSetRange_Click);
            //rbgProduct.Items.Add(btnSetRange);
            btnMulImage.ImageAlignment = ContentAlignment.TopCenter;
            btnMulImage.TextAlignment = ContentAlignment.BottomCenter;
            btnMulImage.Click += new EventHandler(btnMulImage_Click);
            rbgProduct.Items.Add(btnMulImage);
            //btnNetImage.ImageAlignment = ContentAlignment.TopCenter;
            //btnNetImage.TextAlignment = ContentAlignment.BottomCenter;
            //rbgProduct.Items.Add(btnNetImage);
            btnNetBin.ImageAlignment = ContentAlignment.TopCenter;
            btnNetBin.TextAlignment = ContentAlignment.BottomCenter;
            btnNetBin.Click += new EventHandler(btnNetBin_Click);
            rbgProduct.Items.Add(btnNetBin);
            dbtnbinImage.Text = "二值图";
            dbtnbinImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnbinImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnbinImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiBinImgCurrent = new RadMenuItem("当前区域");
            mhiBinImgCurrent.Click += new EventHandler(mhiBinImgCurrent_Click);
            dbtnbinImage.Items.Add(mhiBinImgCurrent);
            RadMenuItem mhiBinImgCustom = new RadMenuItem("自定义");
            mhiBinImgCustom.Click += new EventHandler(mhiBinImgCustom_Click);
            dbtnbinImage.Items.Add(mhiBinImgCustom);
            rbgProduct.Items.Add(dbtnbinImage);
            btnDayMax.ImageAlignment = ContentAlignment.TopCenter;
            btnDayMax.TextAlignment = ContentAlignment.BottomCenter;
            btnDayMax.Click += new EventHandler(btnDayMax_Click);
            rbgProduct.Items.Add(btnDayMax);
            _tab.Items.Add(rbgProduct);
        }

        private void CreateAreaStaticGroup()
        {
            RadRibbonBarGroup rbgAreaStatic = new RadRibbonBarGroup();
            rbgAreaStatic.Text = "面积统计";
            dbtnDivision.Text = "行政区划";
            dbtnDivision.TextAlignment = ContentAlignment.BottomCenter;
            dbtnDivision.ImageAlignment = ContentAlignment.TopCenter;
            RadMenuItem mhiCity = new RadMenuItem("按市县");
            mhiCity.Click += new EventHandler(mhiCity_Click);
            dbtnDivision.Items.Add(mhiCity);
            RadMenuItem mhiProvincBound = new RadMenuItem("按省界");
            mhiProvincBound.Click += new EventHandler(mhiProvincBound_Click);
            dbtnDivision.Items.Add(mhiProvincBound);
            btnCurrentRegionArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCurrentRegionArea.TextAlignment = ContentAlignment.BottomCenter;
            btnCurrentRegionArea.Click += new EventHandler(btnCurrentRegionArea_Click);
            rbgAreaStatic.Items.Add(btnCurrentRegionArea);
            rbgAreaStatic.Items.Add(dbtnDivision);
            btnLandType.ImageAlignment = ContentAlignment.TopCenter;
            btnLandType.TextAlignment = ContentAlignment.BottomCenter;
            btnLandType.Click += new EventHandler(btnLandType_Click);
            rbgAreaStatic.Items.Add(btnLandType);
            btnCustomArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCustomArea.TextAlignment = ContentAlignment.BottomCenter;
            btnCustomArea.Click += new EventHandler(btnCustomArea_Click);
            rbgAreaStatic.Items.Add(btnCustomArea);
            _tab.Items.Add(rbgAreaStatic);
        }

        private void CreateFreqStatic()
        {
            RadRibbonBarGroup rbgFreqStatic = new RadRibbonBarGroup();
            rbgFreqStatic.Text = "频次统计";
            btnCurrentRegionFreq.ImageAlignment = ContentAlignment.TopCenter;
            btnCurrentRegionFreq.TextAlignment = ContentAlignment.BottomCenter;
            btnCurrentRegionFreq.Click += new EventHandler(btnCurrentRegionFreq_Click);
            rbgFreqStatic.Items.Add(btnCurrentRegionFreq);
            btnCustomFreq.TextAlignment = ContentAlignment.BottomCenter;
            btnCustomFreq.ImageAlignment = ContentAlignment.TopCenter;
            btnCustomFreq.Click += new EventHandler(btnCustomFreq_Click);
            rbgFreqStatic.Items.Add(btnCustomFreq);
            _tab.Items.Add(rbgFreqStatic);
        }

        private void CreateDegreeGroup()
        {
            RadRibbonBarGroup rbgDegree = new RadRibbonBarGroup();
            rbgDegree.Text = "程度指数";
            dbtnDivisionDegree.Text = "行政区划";
            dbtnDivisionDegree.TextAlignment = ContentAlignment.BottomCenter;
            dbtnDivisionDegree.ImageAlignment = ContentAlignment.TopCenter;
            RadMenuItem mhiCityDegree = new RadMenuItem("按市县");
            mhiCityDegree.Click += new EventHandler(mhiCityDegree_Click);
            dbtnDivisionDegree.Items.Add(mhiCityDegree);
            RadMenuItem mhiProvincBoundDegree = new RadMenuItem("按省界");
            mhiProvincBoundDegree.Click += new EventHandler(mhiProvincBoundDegree_Click);
            dbtnDivisionDegree.Items.Add(mhiProvincBoundDegree);
            btnCurrentRegionDegree.ImageAlignment = ContentAlignment.TopCenter;
            btnCurrentRegionDegree.TextAlignment = ContentAlignment.BottomCenter;
            btnCurrentRegionDegree.Click += new EventHandler(btnCurrentRegionDegree_Click);
            rbgDegree.Items.Add(btnCurrentRegionDegree);
            rbgDegree.Items.Add(dbtnDivisionDegree);
            btnLandTypeDegree.ImageAlignment = ContentAlignment.TopCenter;
            btnLandTypeDegree.TextAlignment = ContentAlignment.BottomCenter;
            btnLandTypeDegree.Click += new EventHandler(btnLandTypeDegree_Click);
            rbgDegree.Items.Add(btnLandTypeDegree);
            //btnCustomDegree.ImageAlignment = ContentAlignment.TopCenter;
            //btnCustomDegree.TextAlignment = ContentAlignment.BottomCenter;
            //btnCustomDegree.Click += new EventHandler(btnCustomDegree_Click);
            //rbgDegree.Items.Add(btnCustomDegree);
            _tab.Items.Add(rbgDegree);
        }

        private void CreateCompareAnalysisGroup()
        {
            RadRibbonBarGroup rbgAnalysis = new RadRibbonBarGroup();
            rbgAnalysis.Text = "对比分析";
            btnCompare.TextAlignment = ContentAlignment.BottomCenter;
            btnCompare.ImageAlignment = ContentAlignment.TopCenter;
            btnCompare.Click += new EventHandler(btnCompare_Click);
            rbgAnalysis.Items.Add(btnCompare);
            _tab.Items.Add(rbgAnalysis);
        }

        private void CreateAnimationGroup()
        {
            RadRibbonBarGroup rbgAnimation = new RadRibbonBarGroup();
            rbgAnimation.Text = "动画";
            btnAnimationRegion.ImageAlignment = ContentAlignment.TopCenter;
            btnAnimationRegion.TextAlignment = ContentAlignment.BottomCenter;
            btnAnimationRegion.Click += new EventHandler(btnAnimationRegion_Click);
            rbgAnimation.Items.Add(btnAnimationRegion);
            btnAnimationCustom.TextAlignment = ContentAlignment.BottomCenter;
            btnAnimationCustom.ImageAlignment = ContentAlignment.TopCenter;
            btnAnimationCustom.Click += new EventHandler(btnAnimationCustom_Click);
            rbgAnimation.Items.Add(btnAnimationCustom);
            _tab.Items.Add(rbgAnimation);
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
        #endregion

        #region 事件
        //产品入库
        void btnToDb_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(39002);
            if (cmd != null)
                cmd.Execute();
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
        //交互
        void btnInteractive_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExecute(6602);
        }
        /// <summary>
        /// 粗判 + 专题产品快速生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void roughProGenerate_Click(object sender, EventArgs e)
        {
            //open contxtmessage windo
            AutoGenretateProvider.AutoGenrate(_session, null);
        }

        /// <summary>
        /// 专题产品快速生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void proGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "0IMG");
        }
        //二值图自定义
        void mhiBinImgCustom_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "CSDI", "", "DBLV", "积雪二值图", true, true) == null)
                return;
        }
        //二值图当前区域
        void mhiBinImgCurrent_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "DBLV", "积雪二值图", false, true) == null)
                return;
        }
        //网络二值图
        void btnNetBin_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "NSDI", "", "NIMG", "NIMGAlgorithm", "DBLV", "积雪网络二值图", false, true) == null)
                return;
        }
        //多通道合成图
        void btnMulImage_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:积雪多通道合成图,多通道合成图,SNW,MCSI");
        }
        //分省界面积统计
        void mhiProvincBound_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.AreaStat(_session, "0CBP", "省级行政区划", false, true) == null)
                return;
        }
        //分市界面积统计
        void mhiCity_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.AreaStat(_session, "0CCC", "分级行政区划", false, true) == null)
                return;
        }
        //土地类型面积统计
        void btnLandType_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.AreaStat(_session, "CLUT", "土地利用类型", false, true) == null)
                return;
        }
        //自定义面积统计
        void btnCustomArea_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.AreaStat(_session, "CCCA", "", true, true) == null)
                return;
        }
        //当前区域面积统计
        void btnCurrentRegionArea_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.AreaStat(_session, "CCAR", "", false, true) == null)
                return;
        }
        //自定义频次统计
        void btnCustomFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "CFRI", "", "DBLV", "积雪频次监测专题图", true, true) == null)
                return;
        }
        //当前区域频次统计
        void btnCurrentRegionFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "FREQ", "", "DBLV", "积雪频次监测专题图", false, true) == null)
                return;
        }
        //自定义程度指数统计
        void btnCustomDegree_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "CSDC", "0SDC", "0SDC", "", true, true) == null)
                return;
        }
        //土地类型程度指数统计
        void btnLandTypeDegree_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "LSDC", "0SDC", "0SDC", "土地利用类型", false, true) == null)
                return;
        }
        //省界程度指数统计
        void mhiProvincBoundDegree_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "PSDC", "0SDC", "0SDC", "省级行政区划", false, true) == null)
                return;
        }
        //省市界程度指数统计
        void mhiCityDegree_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "TSDC", "0SDC", "0SDC", "分级行政区划", false, true) == null)
                return;
        }
        //当前区域程度指数统计
        void btnCurrentRegionDegree_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "0SDC", "0SDC", "0SDC", "", false, true) == null)
                return;
        }
        /// <summary>
        /// 自定义动画按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAnimationCustom_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.CreatAVI(_session, "template:积雪动画展示专题图,动画展示专题图,SNW,CMED");
        }

        /// <summary>
        /// 当前区域动画按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAnimationRegion_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.CreatAVI(_session, "template:积雪动画展示专题图,动画展示专题图,SNW,MEDI");
        }
        //关闭
        void btnClose_Click(object sender, EventArgs e)
        {
            if (_session == null)
                return;
            _session.UIFrameworkHelper.Remove(_tab.Text);
            _session.UIFrameworkHelper.ActiveDefaultTab();
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveProduct(null);
            GetCommandAndExecute(6602);
        }

        void btnCompare_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "COMP", "SNWCOMP", "COMP", "COMP", "DBLV", "积雪对比分析专题图", false, true) == null)
                return;
        }


        void btnDayMax_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "MAXI", "SNWMAXI", "MAXI", "MAXI", "DBLV", "积雪日最大合成专题图", false, true) == null)
                return;
        }

        private void GetCommandAndExecute(int cmdID)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute("SNW");
        }

        private void GetCommandAndExecute(int cmdID, string template)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute(template);
        }

        #endregion

        #region 日常业务
        void btnDailyWorkMulImage_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:积雪日常业务多通道合成图,多通道合成图,SNW,MCSI");
        }

        void btnDailyWorkNetImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "NSDI", "", "NIMG", "NIMGAlgorithm", "DBLV", "积雪日常业务网络二值图", false, true, true) == null)
                return;
        }

        void btnDailyWorkNetBin_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "DBLV", "积雪日常业务二值图", false, true, true) == null)
                return;
        }

        /// <summary>
        /// 设定区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSetRange_Click(object sender, EventArgs e)
        {

        }
        #endregion

        public object Content
        {
            get { return _tab; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            this._session = session;
            SetImage();
        }

        private void SetImage()
        {
            btnRough.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            dbtnSave.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnInteractive.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            btnMulImage.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            btnNetImage.Image = _session.UIFrameworkHelper.GetImage("");
            btnNetBin.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            dbtnbinImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            btnCurrentRegionArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            dbtnDivision.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            btnLandType.Image = _session.UIFrameworkHelper.GetImage("system:landType.png");
            btnCustomArea.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnCurrentRegionFreq.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
            btnCustomFreq.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnAnimationRegion.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            btnCurrentRegionDegree.Image = _session.UIFrameworkHelper.GetImage("system:area_converDegree.png");
            dbtnDivisionDegree.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            btnLandTypeDegree.Image = _session.UIFrameworkHelper.GetImage("system:landType.png");
            btnCustomDegree.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnAnimationCustom.Image = _session.UIFrameworkHelper.GetImage("system:customVedio.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnCompare.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
            btnDayMax.Image = _session.UIFrameworkHelper.GetImage("system:snow.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");

            btnSetRange.Image = _session.UIFrameworkHelper.GetImage("system:map_edit.png");
            btnDailyWorkNetImage.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            btnDailyWorkNetBin.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");

        }

        public void UpdateStatus()
        {
        }
    }
}
