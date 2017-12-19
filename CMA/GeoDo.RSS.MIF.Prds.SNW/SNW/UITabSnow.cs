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
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    public partial class UITabSnow : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 积雪按钮定义
        RadButtonElement btnRough = new RadButtonElement("自动判识");
        RadDropDownButtonElement dbtnSave = new RadDropDownButtonElement();
        RadDropDownButtonElement btnManualExtract = new RadDropDownButtonElement();
        RadButtonElement btnAnimationRegion = new RadButtonElement("当前区域");
        RadButtonElement btnAnimationCustom = new RadButtonElement("自定义");
        RadButtonElement btnClose = new RadButtonElement("关闭积雪监测\n分析视图");
        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        RadButtonElement btnSnowArgs = new RadButtonElement("积雪深度计算");
        RadDropDownButtonElement dbtnOrdLayout = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnOrdStatArea = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnOrdGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnCompareStat = new RadDropDownButtonElement();
        RadButtonElement btnCompareRaster = new RadButtonElement("计算");
        RadButtonElement btnCompareImg = new RadButtonElement("对比分析专题图");
        RadDropDownButtonElement dbtnComGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnDisLayout = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnDisStatArea = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnDisGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnCycLayout = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnCycStat = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnTFRLayout = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnCycGenerate = new RadDropDownButtonElement();
        //从配置文件读取统计分析菜单名称
        string provinceName = AreaStatProvider.GetAreaStatItemMenuName("行政区划");
        string landTypeName = AreaStatProvider.GetAreaStatItemMenuName("土地利用类型");
        string cityName = AreaStatProvider.GetAreaStatItemMenuName("三级行政区划");
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
            CreateDisasterGroup();
            CreateCompareGroup();
            CreateFreqStatic();
            CreateAnimationGroup();

            btnToDb.TextAlignment = ContentAlignment.BottomCenter;
            btnToDb.ImageAlignment = ContentAlignment.TopCenter;
            btnToDb.Click += new EventHandler(btnToDb_Click);
            RadRibbonBarGroup rbgToDb = new RadRibbonBarGroup();
            rbgToDb.Text = "产品入库";
            rbgToDb.Items.Add(btnToDb);
            _tab.Items.Add(rbgToDb);

            Controls.Add(_bar);
        }

        private void CreateDailyWorkGroup()
        {
            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "日常业务";
            dbtnOrdLayout.Text = "专题产品";
            dbtnOrdLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnOrdLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnOrdLayout.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniOrdNetImg = new RadMenuItem("网络专题图");
            mniOrdNetImg.Click += new EventHandler(mniOrdNetImg_Click);
            RadMenuItem mniOrdNetMCSImg = new RadMenuItem("网络合成图");
            mniOrdNetMCSImg.Click += new EventHandler(mniDisNetMCSImg_Click);
            RadMenuItem mniOrdNetBinImg = new RadMenuItem("网络灰度图");
            mniOrdNetBinImg.Click += new EventHandler(mniOrdNetBinImg_Click);
            dbtnOrdLayout.Items.AddRange(new RadMenuItem[] { mniOrdNetImg, mniOrdNetMCSImg, mniOrdNetBinImg });
            rbgProduct.Items.Add(dbtnOrdLayout);

            dbtnOrdStatArea.Text = "统计分析";
            dbtnOrdStatArea.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnOrdStatArea.ImageAlignment = ContentAlignment.TopCenter;
            dbtnOrdStatArea.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniOrdCurrent = new RadMenuItem("当前区域");
            mniOrdCurrent.Click += new EventHandler(btnCurrentRegionArea_Click);
            RadMenuItem mniOrdProvince = new RadMenuItem(provinceName);
            mniOrdProvince.Click += new EventHandler(mhiProvincBound_Click);
            RadMenuItem mniOrdCity = new RadMenuItem(cityName);
            mniOrdCity.Click += new EventHandler(mhiCity_Click);
            RadMenuItem mniOrdLandType = new RadMenuItem(landTypeName);
            mniOrdLandType.Click += new EventHandler(btnLandType_Click);
            dbtnOrdStatArea.Items.AddRange(new RadMenuItem[] { mniOrdCurrent, mniOrdLandType, mniOrdProvince, mniOrdCity });
            rbgProduct.Items.Add(dbtnOrdStatArea);

            dbtnOrdGenerate.Text = "快速生成";
            dbtnOrdGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnOrdGenerate.ImageAlignment = ContentAlignment.TopCenter;
            dbtnOrdGenerate.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniOrdAllGenerate = new RadMenuItem("自动判识+专题产品");
            mniOrdAllGenerate.Click += new EventHandler(mniOrdAllGenerate_Click);
            dbtnOrdGenerate.Items.Add(mniOrdAllGenerate);
            RadMenuItem mniOrdProGenerate = new RadMenuItem("专题产品");
            mniOrdProGenerate.Click += new EventHandler(mniOrdProGenerate_Click);
            dbtnOrdGenerate.Items.Add(mniOrdProGenerate);
            rbgProduct.Items.Add(dbtnOrdGenerate);
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
            btnManualExtract.Text = "交互判识";
            RadMenuItem btnInteractive = new RadMenuItem("积雪");
            btnInteractive.Click += new EventHandler(btnInteractive_Click);
            btnManualExtract.Items.Add(btnInteractive);
            RadMenuItem btnCloud = new RadMenuItem("云");
            btnCloud.Click += new EventHandler(btnCloud_Click);
            btnManualExtract.Items.Add(btnCloud);
            btnManualExtract.TextAlignment = ContentAlignment.BottomCenter;
            btnManualExtract.ImageAlignment = ContentAlignment.TopCenter;
            rbgCheck.Items.Add(btnManualExtract);
            dbtnSave.Text = "快速生成";
            dbtnSave.ImageAlignment = ContentAlignment.TopCenter;
            dbtnSave.TextAlignment = ContentAlignment.BottomCenter;
            dbtnSave.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem roughProGenerate = new RadMenuItem("自动判识+专题产品");
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

        private void CreateDisasterGroup()
        {
            RadRibbonBarGroup rbgDisaster = new RadRibbonBarGroup();
            rbgDisaster.Text = "灾情事件";
            //微波积雪深度
            btnSnowArgs.ImageAlignment = ContentAlignment.TopCenter;
            btnSnowArgs.TextAlignment = ContentAlignment.BottomCenter;
            btnSnowArgs.Click += new EventHandler(btnSnowArgs_Click);
            rbgDisaster.Items.Add(btnSnowArgs);
            dbtnDisLayout.Text = "专题产品";
            dbtnDisLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnDisLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDisLayout.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniDisMulImg = new RadMenuItem("多通道合成图");
            mniDisMulImg.Click += new EventHandler(btnMulImage_Click);
            RadMenuItem mniDisOMulImg = new RadMenuItem("多通道合成图（原始分辨率）");
            mniDisOMulImg.Click += new EventHandler(mniDisOMulImg_Click);
            RadMenuItem mniDisBinImg = new RadMenuItem("二值图（当前区域）");
            mniDisBinImg.Click += new EventHandler(mhiBinImgCurrent_Click);
            RadMenuItem mniDisCustomBinImg = new RadMenuItem("二值图（自定义）");
            mniDisCustomBinImg.Click += new EventHandler(mhiBinImgCustom_Click);
            RadMenuItem mniDisNetBinImg = new RadMenuItem("网络灰度图（无边框）");
            mniDisNetBinImg.Click += new EventHandler(mniOrdNetBinImg_Click);
            RadMenuItem mniDisNetNImg = new RadMenuItem("网络灰度图（有边框）");
            mniDisNetNImg.Click += new EventHandler(btnNetBin_Click);
            RadMenuItem mniDisNetImg = new RadMenuItem("网络专题图");
            mniDisNetImg.Click += new EventHandler(mniOrdNetImg_Click);
            RadMenuItem mniDisNetMCSImg = new RadMenuItem("网络合成图");
            mniDisNetMCSImg.Click += new EventHandler(mniDisNetMCSImg_Click);
            RadMenuItem mniDisMaxImg = new RadMenuItem("日最大合成图");
            mniDisMaxImg.Click += new EventHandler(btnDayMax_Click);
            RadMenuItem mniDisSnowDepth = new RadMenuItem("雪深监测图（当前区域）");
            mniDisSnowDepth.Click += new EventHandler(mniDisSnowDepth_Click);
            RadMenuItem mniDisSnowDepthCustom = new RadMenuItem("雪深监测图（自定义）");
            mniDisSnowDepthCustom.Click += new EventHandler(mniDisSnowDepthCustom_Click);
            //RadMenuItem mniDisPolarSnowWE = new RadMenuItem("极地雪深雪水当量专题图");
            //mniDisPolarSnowWE.Click += new EventHandler(mniDisPolarSnowWE_Click);
            dbtnDisLayout.Items.AddRange(new RadMenuItem[] { mniDisMulImg,mniDisOMulImg,mniDisBinImg, mniDisCustomBinImg,
                mniDisNetBinImg, mniDisNetNImg, mniDisNetImg,mniDisNetMCSImg, mniDisMaxImg,mniDisSnowDepth,mniDisSnowDepthCustom});
            rbgDisaster.Items.Add(dbtnDisLayout);

            dbtnDisStatArea.Text = "统计分析";
            dbtnDisStatArea.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnDisStatArea.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDisStatArea.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniDisCurrent = new RadMenuItem("当前区域");
            mniDisCurrent.Click += new EventHandler(btnCurrentRegionArea_Click);
            RadMenuItem mniDisProvince = new RadMenuItem(provinceName);
            mniDisProvince.Click += new EventHandler(mhiProvincBound_Click);
            RadMenuItem mniDisCity = new RadMenuItem(cityName);
            mniDisCity.Click += new EventHandler(mhiCity_Click);
            RadMenuItem mniDisLandType = new RadMenuItem(landTypeName);
            mniDisLandType.Click += new EventHandler(btnLandType_Click);
            RadMenuItem mniDivAndType = new RadMenuItem("行政区+地类");
            mniDivAndType.Click += new EventHandler(mniDivAndType_Click);
            RadMenuItem mniDisCustom = new RadMenuItem("自定义");
            mniDisCustom.Click += new EventHandler(btnCustomArea_Click);
            dbtnDisStatArea.Items.AddRange(new RadMenuItem[] { mniDisCurrent, mniDisLandType, mniDisProvince, mniDisCity, mniDivAndType, mniDisCustom });
            rbgDisaster.Items.Add(dbtnDisStatArea);

            dbtnDisGenerate.Text = "快速生成";
            dbtnDisGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnDisGenerate.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDisGenerate.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniDisAllGenerate = new RadMenuItem("自动判识+专题产品");
            mniDisAllGenerate.Click += new EventHandler(mniDisAllGenerate_Click);
            dbtnDisGenerate.Items.Add(mniDisAllGenerate);
            RadMenuItem mniDisProGenerate = new RadMenuItem("专题产品");
            mniDisProGenerate.Click += new EventHandler(mniDisProGenerate_Click);
            dbtnDisGenerate.Items.Add(mniDisProGenerate);
            rbgDisaster.Items.Add(dbtnDisGenerate);
            _tab.Items.Add(rbgDisaster);
        }


        private void CreateCompareGroup()
        {
            RadRibbonBarGroup rbgCompare = new RadRibbonBarGroup();
            rbgCompare.Text = "对比分析";
            btnCompareRaster.TextAlignment = ContentAlignment.BottomCenter;
            btnCompareRaster.ImageAlignment = ContentAlignment.TopCenter;
            btnCompareRaster.Click += new EventHandler(btnCompareRaster_Click);
            btnCompareImg.TextAlignment = ContentAlignment.BottomCenter;
            btnCompareImg.ImageAlignment = ContentAlignment.TopCenter;
            btnCompareImg.Click += new EventHandler(btnCompareImg_Click);
            dbtnCompareStat.Text = "统计分析";
            dbtnCompareStat.TextAlignment = ContentAlignment.BottomCenter;
            dbtnCompareStat.ImageAlignment = ContentAlignment.TopCenter;
            RadMenuItem mniCompStatCurrent = new RadMenuItem("当前区域");
            mniCompStatCurrent.Click += new EventHandler(mniCompStatCurrent_Click);
            RadMenuItem mniCompStatLandType = new RadMenuItem(landTypeName);
            mniCompStatLandType.Click += new EventHandler(mniCompStatLandType_Click);
            RadMenuItem mniCompStatProvince = new RadMenuItem(provinceName);
            mniCompStatProvince.Click += new EventHandler(mmniCompStatProvince_Click);
            RadMenuItem mniCompStatCity = new RadMenuItem(cityName);
            mniCompStatCity.Click += new EventHandler(mniCompStatCity_Click);
            RadMenuItem mniCompStatCustom = new RadMenuItem("自定义");
            mniCompStatCustom.Click += new EventHandler(mniCompStatCustom_Click);
            dbtnCompareStat.Items.AddRange(mniCompStatCurrent, mniCompStatLandType, mniCompStatProvince, mniCompStatCity, mniCompStatCustom);
            dbtnComGenerate.Text = "快速生成";
            dbtnComGenerate.TextAlignment = ContentAlignment.BottomCenter;
            dbtnComGenerate.ImageAlignment = ContentAlignment.TopCenter;
            RadMenuItem mniComAllGenerate = new RadMenuItem("计算+专题产品");
            mniComAllGenerate.Click += new EventHandler(mniComAllGenerate_Click);
            RadMenuItem mniComProGenerate = new RadMenuItem("专题产品");
            mniComProGenerate.Click += new EventHandler(mniComProGenerate_Click);
            dbtnComGenerate.Items.AddRange(mniComAllGenerate, mniComProGenerate);
            rbgCompare.Items.AddRange(btnCompareRaster, btnCompareImg, dbtnCompareStat, dbtnComGenerate);
            _tab.Items.Add(rbgCompare);
        }

        private void CreateFreqStatic()
        {
            RadRibbonBarGroup rbgFreqStatic = new RadRibbonBarGroup();
            rbgFreqStatic.Text = "周期业务";
            dbtnCycLayout.Text = "专题产品";
            dbtnCycLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnCycLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCycLayout.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniCurrentFrds = new RadMenuItem("天数图（当前区域）");
            mniCurrentFrds.Click += new EventHandler(btnCurrentRegionFrds_Click);
            RadMenuItem mniCurrentFreq = new RadMenuItem("频次图（当前区域）");
            mniCurrentFreq.Click += new EventHandler(btnCurrentRegionFreq_Click);
            RadMenuItem mniCustomFreq = new RadMenuItem("频次图（自定义）");
            mniCustomFreq.Click += new EventHandler(btnCustomFreq_Click);
            dbtnCycLayout.Items.AddRange(new RadMenuItem[] { mniCurrentFreq, mniCustomFreq, mniCurrentFrds });
            rbgFreqStatic.Items.Add(dbtnCycLayout);

            dbtnCycStat.Text = "统计分析";
            dbtnCycStat.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnCycStat.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCycStat.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniCycCurrent = new RadMenuItem("程度指数（当前区域）");
            mniCycCurrent.Click += new EventHandler(btnCurrentRegionDegree_Click);
            RadMenuItem mniCycProvince = new RadMenuItem("程度指数（按省界）");
            mniCycProvince.Click += new EventHandler(mhiProvincBoundDegree_Click);
            RadMenuItem mniCycCity = new RadMenuItem("程度指数（按市县）");
            mniCycCity.Click += new EventHandler(mhiCityDegree_Click);
            RadMenuItem mniCycLandType = new RadMenuItem("程度指数（土地类型）");
            mniCycLandType.Click += new EventHandler(btnLandTypeDegree_Click);
            dbtnCycStat.Items.AddRange(new RadMenuItem[] { mniCycCurrent, mniCycProvince, 
                //mniCycCity,
                mniCycLandType});
            rbgFreqStatic.Items.Add(dbtnCycStat);

            dbtnTFRLayout.Text = "长序列";
            dbtnTFRLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnTFRLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnTFRLayout.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniCurrentTfre = new RadMenuItem("频次统计（当前区域）");
            mniCurrentTfre.Click += new EventHandler(mniCurrentTfre_Click);
            RadMenuItem mniCurrentTfreImage = new RadMenuItem("频次图（当前区域）");
            mniCurrentTfreImage.Click += new EventHandler(mniCurrentTfreImage_Click);
            RadMenuItem mniCurrentTfrq = new RadMenuItem("频率统计（当前区域）");
            mniCurrentTfrq.Click += new EventHandler(mniCurrentTfrq_Click);
            RadMenuItem mniCurrentTfrqImage = new RadMenuItem("频率图（当前区域）");
            mniCurrentTfrqImage.Click += new EventHandler(mniCurrentTfrqImage_Click);
            RadMenuItem mniEDGE = new RadMenuItem("积雪边缘提取");
            mniEDGE.Click += new EventHandler(mniEDGE_Click);
            RadMenuItem mniTSTA = new RadMenuItem("长序列面积(当前)");
            mniTSTA.Click += new EventHandler(mniTSTA_Click);
            RadMenuItem mniTSTAPri = new RadMenuItem("长序列面积(省界)");
            mniTSTAPri.Click += new EventHandler(mniTSTAPri_Click);
            RadMenuItem mniTSTATD = new RadMenuItem("长序列面积(土地类型)");
            mniTSTATD.Click += new EventHandler(mniTSTATD_Click);
            dbtnTFRLayout.Items.AddRange(new RadMenuItem[] { mniCurrentTfre, mniCurrentTfreImage, mniCurrentTfrq, mniCurrentTfrqImage, mniEDGE, mniTSTA, mniTSTAPri, mniTSTATD });
            rbgFreqStatic.Items.Add(dbtnTFRLayout);

            dbtnCycGenerate.Text = "快速生成";
            dbtnCycGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnCycGenerate.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCycGenerate.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniCycProGenerate = new RadMenuItem("专题产品");
            mniCycProGenerate.Click += new EventHandler(mniCycProGenerate_Click);
            dbtnCycGenerate.Items.Add(mniCycProGenerate);
            //rbgFreqStatic.Items.Add(dbtnCycGenerate);
            _tab.Items.Add(rbgFreqStatic);
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
        //报告素材生成
        void btn2Report_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(400002);
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
        //交互积雪
        void btnInteractive_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExecute(6602);
        }
        //交互云
        void btnCloud_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0CLM");
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

        void mniOrdAllGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, null, null, "Ord");
        }

        void mniOrdProGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "0IMG", null, "Ord");
        }

        void mniDisAllGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, null, null, "Dis");
        }

        void mniDisProGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "0IMG", null, "Dis");
        }

        void mniCycProGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "FREQ", null, "Cyc");
        }

        void mniCompStatCustom_Click(object sender, EventArgs e)   //自定义
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "COCA", "COMA", "COMA", "", true, true) == null)
                return;
        }

        void mniCompStatCurrent_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "0CCA", "COMA", "COMA", "", false, true) == null)
                return;
        }

        void mniDivAndType_Click(object sender, EventArgs e)         //扩行政区划+地类
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "ADLU", "STAT", "STATAlgorithm", "省级行政区+土地利用类型", false, true) == null)
                return;
        }

        void mniCompStatLandType_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "LUTC", "COMA", "COMA", "土地利用类型", false, true) == null)
                return;
        }

        void mmniCompStatProvince_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "CCBP", "COMA", "COMA", "省级行政区划", false, true) == null)
                return;
        }

        void mniCompStatCity_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "COCC", "COMA", "COMA", "省市县行政区划", false, true) == null)
                return;
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
            /*
             * 1、参数在CMAThemes.xml的SuProduct中已经设置
             * 2、这里不需要再设置参数
             * 3、如果有监测分析面板则激活面板，否则直接执行Make函数
             */
            //IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            //ms.ChangeActiveSubProduct("NIMG");
            //ms.DoAutoExtract(false);
        }

        //多通道合成图
        void btnMulImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MCSI");
            ms.DoAutoExtract(true);
        }


        void mniDisOMulImg_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "OMCS");
            ms.DoAutoExtract(true);
        }

        void mniDisNetMCSImg_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "NMCS");
            ms.DoAutoExtract(true);
        }

        //分省界面积统计
        void mhiProvincBound_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0CBP");
            ms.DoAutoExtract(true);
        }

        //分市界面积统计
        void mhiCity_Click(object sender, EventArgs e)
        {
            //按照Instance执行统计操作
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0CCC");
            ms.DoAutoExtract(true);
        }

        //土地类型面积统计
        void btnLandType_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "CLUT");
            ms.DoAutoExtract(true);
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
            if (MIFCommAnalysis.TimeStat(_session, "CFRI", "SNWFREQ", "DBLV", "积雪频次监测专题图", true, true) == null)
                return;
        }

        //当前区域频次统计
        void btnCurrentRegionFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "FREQ", "SNWFREQ", "DBLV", "积雪频次监测专题图", false, true) == null)
                return;
        }

        //当前区域天数统计
        void btnCurrentRegionFrds_Click(object sender, EventArgs e)
        {
            //if (MIFCommAnalysis.TimeStat(_session, "FRDS", "SNWFRDS", "DBLV", "积雪天数专题图模板", false, true) == null)
            //    return;
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("FRDS");
            GetCommandAndExecute(6602);
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
            //IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            //ms.ChangeActiveSubProduct("0SDC");
            //ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "PSDC");
            //ms.DoAutoExtract(true);
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
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("COMP");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "COMP");
            ms.DoAutoExtract(true);
        }
        void btnCompareRaster_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("COMP");
            GetCommandAndExecute(6602);
        }

        void mniComProGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "0IMG", null, "Com");
        }

        void mniComAllGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "COMP", CustomVarSetterHandler, "Com");
        }

        void btnCompareImg_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "COMI");
            ms.DoAutoExtract(true);
        }

       

        void btnDayMax_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "MAXI", "SNWMAXI", "MAXI", "MAXI", "DBLV", "积雪日最大合成专题图", false, true) == null)
                return;
        }

        void mniDisSnowDepth_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "SSDI");
            ms.DoAutoExtract(true);
        }

        void mniDisSnowDepthCustom_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.CreateThemeGraphy(_session, "MSDC", "SNWMWSD", "MWSD", "积雪雪深监测图", true, true);
        }

        void mniDisPolarSnowWE_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0PSI");
            GetCommandAndExecute(6602);
        }

        void mniOrdNetImg_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "NCSI", "", "DBLV", "积雪日常业务二值图", false, true, true) == null)
                return;
        }

        void mniOrdNetBinImg_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "NSNI", "", "NIMG", "NIMGAlgorithm", "DBLV", "积雪日常业务网络二值图", false, true, true) == null)
                return;
        }

        void btnSnowArgs_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0SSD");
            GetCommandAndExecute(6602);
        }

        #region 长序列分析

        void mniCurrentTfre_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("TFRE");
            GetCommandAndExecute(6602);
        }

        void mniCurrentTfrq_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("TFRQ");
            GetCommandAndExecute(6602);
        }

        void mniCurrentTfreImage_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("TFRI");
            GetCommandAndExecute(6602);
        }

        void mniCurrentTfrqImage_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("TFQI");
            GetCommandAndExecute(6602);
        }

        void mniEDGE_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("EDGE");
            GetCommandAndExecute(6602);
        }

        void mniTSTA_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("TSTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentifyInstance", "TCCA");
            GetCommandAndExecute(6602);
        }

        void mniTSTATD_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("TSTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentifyInstance", "TCLU");
            GetCommandAndExecute(6602);
        }

        void mniTSTAPri_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("TSTA");
            GetCommandAndExecute(6602);
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentifyInstance", "TCBP");
        }

        #endregion

        #endregion

        public object Content
        {
            get { return _tab; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            this._session = session;
            CreateThemeGraphRegionButton();
            CreateqQuickReportRegionButton();
            CreateCloseGroup();
            SetImage();
        }

        private void CreateqQuickReportRegionButton()
        {
            QuickReportFactory.RegistToButton("SNW", _tab, _session);
        }

        private void CreateThemeGraphRegionButton()
        {
            RadDropDownButtonElement _btnThemeGraphRegion = new RadDropDownButtonElement();
            ThemeGraphRegionFacctory.RegistToButton("SNW", _btnThemeGraphRegion, _tab, _session);
            _btnThemeGraphRegion.Image = _session.UIFrameworkHelper.GetImage("system:settings.png");
        }

        private void SetImage()
        {
            btnRough.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            dbtnSave.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnManualExtract.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            btnAnimationRegion.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            btnAnimationCustom.Image = _session.UIFrameworkHelper.GetImage("system:customVedio.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
            dbtnDisLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnOrdLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnOrdStatArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            dbtnOrdGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnDisLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnDisStatArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            dbtnDisGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnCycLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnCycStat.Image = _session.UIFrameworkHelper.GetImage("system:area_converDegree.png");
            dbtnCycGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            //dbtnCompareProduct.Image = _session.UIFrameworkHelper.GetImage("system:comList.png");
            dbtnComGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnCompareStat.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            btnCompareImg.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            btnCompareRaster.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            btnSnowArgs.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
        }

        public void UpdateStatus()
        {
        }

        private void GetCommandAndExecute(int cmdID)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute("SNW");
        }

        private void CustomVarSetterHandler(string[] dblvFileNames, string beginSubprod, IContextEnvironment contextEnv)
        {
            if (dblvFileNames == null || dblvFileNames.Length < 1)
                return;
            string varValue = dblvFileNames[0];
            for (int i = 1; i < dblvFileNames.Length; i++)
            {
                varValue += "*" + dblvFileNames[i];
            }
            contextEnv.PutContextVar("DBLV", varValue);
        }

        private bool TryGetSelectedPrimaryFiles(ref string[] files, IMonitoringSession monitoringSession)
        {
            IWorkspace wks = monitoringSession.Workspace;
            if (wks == null)
                return false;
            ICatalog catalog = wks.ActiveCatalog;
            if (catalog == null)
                return false;
            string[] fnames = catalog.GetSelectedFiles();
            if (fnames == null || fnames.Length == 0)
                return false;
            files = fnames;
            return true;
        }
    }
}
