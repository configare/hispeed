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
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.FileProject;

namespace GeoDo.RSS.MIF.Prds.DST
{
    public partial class UITabSandDust : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 沙尘按钮定义
        RadButtonElement btnRough = new RadButtonElement("自动判识");
        RadDropDownButtonElement btnSave = new RadDropDownButtonElement();
        RadDropDownButtonElement btnManualExtract = new RadDropDownButtonElement();

        //动画显示
        RadButtonElement _btnCurrent = null; //当前区域
        RadButtonElement _btnCustom = null; // 自定义

        //统计分析
        //RadDropDownButtonElement dbtnQuantifyCalc = null; //特征量计算
        RadButtonElement _btnDisVisibility = null;

        RadButtonElement _btnClose = null;
        RadButtonElement _btnToDb = new RadButtonElement("产品入库");

       // RadDropDownButtonElement dbtnOrdLayout = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnOrdStatArea = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnOrdGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnDisLayout = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnDisStatArea = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnDisGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnCycLayout = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnCycStat = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnCycGenerate = new RadDropDownButtonElement();
        //批量保存按钮
        RadButtonElement btnPicLayout = new RadButtonElement("天气网图像批量输出\n分析视图");//btnPicLayout

        RadButtonElement btncmpcal = new RadButtonElement("计算");
        RadButtonElement btnsubject = new RadButtonElement("对比分析专题图");

        //从配置文件读取统计分析菜单名称
        string provinceName = AreaStatProvider.GetAreaStatItemMenuName("行政区划");
        string landTypeName = AreaStatProvider.GetAreaStatItemMenuName("土地利用类型");
        string cityName = AreaStatProvider.GetAreaStatItemMenuName("三级行政区划");


        #endregion
       

        RadButtonElement btnNatrueColor = new RadButtonElement("真彩图");

        public UITabSandDust()
        {
            InitializeComponent();
            CreateRibbonBar();
        }

        #region Creat UI
        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "沙尘监测专题";
            _tab.Text = "沙尘监测专题";
            _tab.Name = "沙尘监测专题";
            _bar.CommandTabs.Add(_tab);

            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            rbgCheck.Text = "监测";
            rbgCheck.Items.Add(btnRough);
            foreach (RadButtonElement item in rbgCheck.Items)
            {
                item.ImageAlignment = ContentAlignment.TopCenter;
                item.TextAlignment = ContentAlignment.BottomCenter;
            }
            btnManualExtract.Text = "交互判识";
            btnManualExtract.ImageAlignment = ContentAlignment.TopCenter;
            btnManualExtract.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem riInteractive = new RadMenuItem("沙尘");
            riInteractive.Click += new EventHandler(btnInteractive_Click);
            btnManualExtract.Items.Add(riInteractive);
            RadMenuItem riCLM = new RadMenuItem("云");
            riCLM.Click += new EventHandler(riCLM_Click);
            btnManualExtract.Items.Add(riCLM);
            rbgCheck.Items.Add(btnManualExtract);
            btnSave.Text = "快速生成";
            RadMenuItem item1 = new RadMenuItem();
            item1.Text = "自动判识+专题产品";
            item1.Click += new EventHandler(btnSave_Click);
            btnSave.Items.Add(item1);
            RadMenuItem item2 = new RadMenuItem();
            item2.Click += new EventHandler(item2_Click);
            item2.Text = "专题产品";
            btnSave.Items.Add(item2);
            btnSave.ImageAlignment = ContentAlignment.TopCenter;
            btnSave.TextAlignment = ContentAlignment.BottomCenter;
            rbgCheck.Items.Add(btnSave);

          

            btnRough.Click += new EventHandler(btnRough_Click);
            _tab.Items.Add(rbgCheck);
           

            //CreateDailyWorkGroup();
            CreateDisasterGroup();
            //既有细节 又有方法 需要重构
            CreateCompareGroup();
            //真彩图相关按钮
            RadRibbonBarGroup rbgDST = new RadRibbonBarGroup();
            rbgDST.Text = "真彩图";
            btnNatrueColor.ImageAlignment = ContentAlignment.TopCenter;
            btnNatrueColor.TextAlignment = ContentAlignment.BottomCenter;
            btnNatrueColor.Click += new EventHandler(btnNatrueColor_Click);
            rbgDST.Items.Add(btnNatrueColor);
            _tab.Items.Add(rbgDST);
            CreateFreqStatic();
            CreatCartoon(); //动画显示
            
            _btnToDb.TextAlignment = ContentAlignment.BottomCenter;
            _btnToDb.ImageAlignment = ContentAlignment.TopCenter;
            _btnToDb.Click += new EventHandler(btnToDb_Click);
            RadRibbonBarGroup rbgToDb = new RadRibbonBarGroup();
            rbgToDb.Text = "产品入库";
            rbgToDb.Items.Add(_btnToDb);
            _tab.Items.Add(rbgToDb);
            Controls.Add(_bar);

            AddPicLayoutButton();
        }

        private void CreatCartoon()
        {
            RadRibbonBarGroup group = new RadRibbonBarGroup();
            group.Text = "动画显示";
            _btnCurrent = new RadButtonElement("当前区域");
            _btnCurrent.ImageAlignment = ContentAlignment.TopCenter;
            _btnCurrent.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnCurrent.TextAlignment = ContentAlignment.BottomCenter;
            _btnCurrent.Click += new EventHandler(_btnCurrent_Click);

            _btnCustom = new RadButtonElement("自定义");
            _btnCustom.ImageAlignment = ContentAlignment.TopCenter;
            _btnCustom.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnCustom.TextAlignment = ContentAlignment.BottomCenter;
            _btnCustom.Click += new EventHandler(_btnCustom_Click);
            group.Items.AddRange(new RadItem[] { _btnCurrent, _btnCustom });
            _tab.Items.Add(group);
        }

        private void CreatClose()
        {
            RadRibbonBarGroup close = new RadRibbonBarGroup();
            close.Text = "关闭";
            _btnClose = new RadButtonElement("关闭沙尘监测\n分析视图");
            _btnClose.TextAlignment = ContentAlignment.MiddleCenter;
            _btnClose.ImageAlignment = ContentAlignment.TopCenter;
            _btnClose.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnClose.Click += new EventHandler(_btnClose_Click);
            close.Items.Add(_btnClose);
            _tab.Items.Add(close);
        }

        private void CreateDisasterGroup()
        {
            RadRibbonBarGroup rbgDisaster = new RadRibbonBarGroup();
            rbgDisaster.Text = "灾情事件";
            _btnDisVisibility = new RadButtonElement("能见度计算");
            _btnDisVisibility.ImageAlignment = ContentAlignment.TopCenter;
            _btnDisVisibility.TextAlignment = ContentAlignment.BottomCenter;
            _btnDisVisibility.Click+=new EventHandler(_btnVisibility_Click);
            rbgDisaster.Items.Add(_btnDisVisibility);
            dbtnDisLayout.Text = "专题产品";
            dbtnDisLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnDisLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDisLayout.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniDisMulImg = new RadMenuItem("监测图");
            mniDisMulImg.Click += new EventHandler(mniOrdMulImage_Click);
            RadMenuItem mniDisMulImg2 = new RadMenuItem("监测示意图");
            mniDisMulImg2.Click += new EventHandler(_btnMonitorShow_Click);
            RadMenuItem mniDisNulImg = new RadMenuItem("监测图（无边框）");
            mniDisNulImg.Click += new EventHandler(menuItemMulImage_Click);
            RadMenuItem mniDisONulImg = new RadMenuItem("监测示意图（无边框）");
            mniDisONulImg.Click += new EventHandler(menuItemMulImage2_Click);
            RadMenuItem mniDisBinImg = new RadMenuItem("二值图（当前区域）");
            mniDisBinImg.Click += new EventHandler(curItem_Click);
            RadMenuItem mniDisCustomBinImg = new RadMenuItem("二值图（自定义）");
            mniDisCustomBinImg.Click += new EventHandler(cusItem_Click);
            RadMenuItem mniDisVisiImg = new RadMenuItem("能见度图");
            mniDisVisiImg.Click += new EventHandler(_btnVisibityImage_Click);
            RadMenuItem mniserverimg = new RadMenuItem("天气网用图");
            mniserverimg.Click += new EventHandler(_btnServerImage_Click);
            RadMenuItem mniDisStationMulImg = new RadMenuItem("监测图(加站点)");
            mniDisStationMulImg.Click += new EventHandler(mniOrdStationMulImage_Click);
            RadMenuItem mniDisStationMulImg2 = new RadMenuItem("监测示意图(加站点)");
            mniDisStationMulImg2.Click += new EventHandler(_btnMonitorStationShow_Click);
            dbtnDisLayout.Items.AddRange(new RadMenuItem[] { mniDisMulImg, mniDisMulImg2, mniDisNulImg,
                mniDisONulImg, mniDisBinImg, mniDisCustomBinImg, mniDisVisiImg, mniserverimg,mniDisStationMulImg,mniDisStationMulImg2 });
            rbgDisaster.Items.Add(dbtnDisLayout);

            dbtnDisStatArea.Text = "统计分析";
            dbtnDisStatArea.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnDisStatArea.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDisStatArea.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniDisCurrent = new RadMenuItem("当前区域");
            mniDisCurrent.Click += new EventHandler(_btnCurrentRegionArea_Click);
            RadMenuItem mniDisProvince = new RadMenuItem(provinceName);
            mniDisProvince.Click += new EventHandler(mhiCity_Click);
            RadMenuItem mniDisCity = new RadMenuItem(cityName);
            mniDisCity.Click += new EventHandler(_dbtnDivision_Click);
            RadMenuItem mniDisLandType = new RadMenuItem(landTypeName);
            mniDisLandType.Click += new EventHandler(_btnLandType_Click);
            RadMenuItem mniDisCustom = new RadMenuItem("自定义");
            mniDisCustom.Click += new EventHandler(_btnCustomArea_Click);
            dbtnDisStatArea.Items.AddRange(new RadMenuItem[] { mniDisCurrent, mniDisLandType, mniDisProvince, mniDisCity, mniDisCustom });
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
        /// <summary>
        /// 创建对比分析功能组
        /// </summary>
        private void CreateCompareGroup()
        {
            RadRibbonBarGroup cmpgroup = new RadRibbonBarGroup();
             
           
            cmpgroup.Text = "对比分析";
            btncmpcal = new RadButtonElement("计算");
            btncmpcal.ImageAlignment = ContentAlignment.TopCenter;
            btncmpcal.TextAlignment = ContentAlignment.BottomCenter;
           
            btncmpcal.Click += new EventHandler(_btncmpcal_Click);
            cmpgroup.Items.Add(btncmpcal);

             btnsubject = new RadButtonElement("对比分析专题图");
            btnsubject.ImageAlignment = ContentAlignment.TopCenter;
            btnsubject.TextAlignment = ContentAlignment.BottomCenter;
            btnsubject.Click += new EventHandler(_btncmpsubject_Click);
           
            cmpgroup.Items.Add(btnsubject);
            _tab.Items.Add(cmpgroup);
        }
        //对比分析计算
        void _btncmpcal_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("COMD");
            GetCommandAndExecute(6602);
        }
        //对比分析专题图
        void _btncmpsubject_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "CODI");
            ms.DoAutoExtract(true);
        }

        private void CreateFreqStatic()
        {
            RadRibbonBarGroup rbgFreqStatic = new RadRibbonBarGroup();
            rbgFreqStatic.Text = "周期业务";
            dbtnCycLayout.Text = "专题产品";
            dbtnCycLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnCycLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCycLayout.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniCurrentFreq = new RadMenuItem("频次图（当前区域）");
            mniCurrentFreq.Click += new EventHandler(_btnCurrentRegionFreq_Click);
            RadMenuItem mniCustomFreq = new RadMenuItem("频次图（自定义）");
            mniCustomFreq.Click += new EventHandler(_btnCustomFreq_Click);
            RadMenuItem mniCurrentDays = new RadMenuItem("日数图（当前区域）");
            mniCurrentDays.Click += new EventHandler(_btnCurrentRegionDays_Click);
            RadMenuItem mniCustomDays = new RadMenuItem("日数图（自定义）");
            mniCustomDays.Click += new EventHandler(_btnCustomnDays_Click);
            RadMenuItem mniCurrentCycTime = new RadMenuItem("周期合成（当前区域）");
            mniCurrentCycTime.Click += new EventHandler(cur_Click);
            RadMenuItem mniCustomCycTime = new RadMenuItem("周期合成（自定义）");
            mniCustomCycTime.Click += new EventHandler(cus_Click);
            dbtnCycLayout.Items.AddRange(new RadMenuItem[] { mniCurrentFreq, mniCustomFreq,mniCurrentDays,mniCustomDays, mniCurrentCycTime, mniCustomCycTime });
            rbgFreqStatic.Items.Add(dbtnCycLayout);

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

        private void SetImage()
        {
            btnNatrueColor.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            btnRough.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            btnSave.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnManualExtract.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            _btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            _btnCurrent.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            _btnCustom.Image = _session.UIFrameworkHelper.GetImage("system:customVedio.png");
            //dbtnQuantifyCalc.Image = _session.UIFrameworkHelper.GetImage("system:sand_visibility.png");
            _btnDisVisibility.Image = _session.UIFrameworkHelper.GetImage("system:sand_visibility.png");
            _btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
            //dbtnOrdLayout.Image = _session.UIFrameworkHelper.GetImage("system: mulImage.png");
            dbtnOrdStatArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            dbtnOrdGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnDisLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnDisStatArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            dbtnDisGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnCycLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnCycGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            //dbtnOrdLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnCycLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            btnPicLayout.Image = _session.UIFrameworkHelper.GetImage("system:Layout_ToFullEnvelope.png");

            btncmpcal.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");//自动判识
            btnsubject.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
        }
        #endregion

        #region ICommand members
        public object Content
        {
            get { return _tab; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            this._session = session;
            CreateThemeGraphRegionButton();
            CreateqQuickReportRegionButton();
            CreatClose(); // 关闭
            SetImage();
        }

        private void CreateqQuickReportRegionButton()
        {
            QuickReportFactory.RegistToButton("DST", _tab, _session);
        }

        private void CreateThemeGraphRegionButton()
        {
            RadDropDownButtonElement _btnThemeGraphRegion = new RadDropDownButtonElement();
            ThemeGraphRegionFacctory.RegistToButton("DST", _btnThemeGraphRegion, _tab, _session);
            _btnThemeGraphRegion.Image = _session.UIFrameworkHelper.GetImage("system:settings.png");
        }

        public void UpdateStatus()
        {
        }

        #endregion

        #region button clicks

        #region 判识
        /// <summary>
        /// 粗判
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnRough_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExecute(6601);
        }

        /// <summary>
        /// 交互判识
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnInteractive_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 交互判识云
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void riCLM_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0CLM");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 快速生成:粗判+专题产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSave_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "DBLV");
        }

        /// <summary>
        /// 快速生成：专题产品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void item2_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "VISY");
        }

        /// <summary>
        /// 日常业务快速生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mniOrdAllGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "DBLV", null, "Ord");
        }

        void mniOrdProGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "VISY", null, "Ord");
        }

        /// <summary>
        /// 灾情事件快速生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mniDisAllGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "DBLV", null, "Dis");
        }

        void mniDisProGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "VISY", null, "Dis");
        }

        void mniCycProGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "FREQ", null, "Cyc");
        }

        #endregion

        #region 面积统计
        /// <summary>
        /// 面积统计—当前区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnCurrentRegionArea_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.AreaStat(_session, "CCAR", "", false, true) == null)
                return;
        }

        /// <summary>
        /// 面积统计—自定义
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnCustomArea_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.AreaStat(_session, "CCCA", "", true, true) == null)
                return;
        }

        /// <summary>
        /// 面积统计—土地利用类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnLandType_Click(object sender, EventArgs e)
        {
            //if (MIFCommAnalysis.AreaStat(_session, "CLUT", "土地利用类型", false, true) == null)
            //    return;
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "CLUT");
            ms.DoAutoExtract(true);
        }

        /// <summary>
        /// 面积统计—按省界
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiCity_Click(object sender, EventArgs e)
        {
            //if (MIFCommAnalysis.AreaStat(_session, "0CBP", "省级行政区划", false, true) == null)
            //    return;
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0CBP");
            ms.DoAutoExtract(true);
        }

        /// <summary>
        /// 面积统计—按市县
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _dbtnDivision_Click(object sender, EventArgs e)
        {
            //if (MIFCommAnalysis.AreaStat(_session, "0CCC", "分级行政区划", false, true) == null)
            //    return;
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0CCC");
            ms.DoAutoExtract(true);
        }
        #endregion

        #region 专题图产品

        //监测图
        void mniOrdMulImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MCSI");
            ms.DoAutoExtract(false);
        }

        //监测图(加站点)
        void mniOrdStationMulImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MSSI");
            ms.DoAutoExtract(false);
        }

        /// <summary>
        /// 监测图无边框（原始分辨率）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuItemMulImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "OMCS");
            ms.DoAutoExtract(false);
        }

        /// <summary>
        /// 沙尘监测示意图
        /// 有边框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnMonitorShow_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0MSI");
            ms.DoAutoExtract(false);
        }

        /// <summary>
        /// 沙尘监测示意图（加站点）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnMonitorStationShow_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0SSI");
            ms.DoAutoExtract(false);
        }

        /// <summary>
        /// 监测示意图无边框（原始分辨率）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuItemMulImage2_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MOSI");
            ms.DoAutoExtract(false);
        }
         



        #region 二值图
        /// <summary>
        /// 自定义二值图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cusItem_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "CSDI", "", "DBLV", "沙尘二值图模版", true, true) == null)
                return;
        }

        /// <summary>
        /// 当前区域二值图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void curItem_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "DBLV", "沙尘二值图模版", false, true) == null)
                return;
        }

        /// <summary>
        /// 能见度专题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnVisibityImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "VISI", "", "VISY", "沙尘能见度模版", false, true) == null)
                return;
        }
        /// <summary>
        /// 服务器用图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnServerImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "TNCI");
            ms.DoAutoExtract(false);

            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "ONCI");   //原始
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("HEAThemeGraphTemplateName", "");
            ms.DoAutoExtract(false);
        }

        #endregion

        #endregion

        #region 频次统计
        /// <summary>
        /// 当前区域频次统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnCurrentRegionFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "FREQ", "DSTFREQ", "DBLV", "沙尘频次统计图模版", false, true) == null)
                return;
        }

        void _btnCurrentRegionDays_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "0MDS", "DST0MDS", "DBLV", "沙尘日数统计图模版", false, true) == null)
                return;
        }
        

        /// <summary>
        /// 自定义区域频次统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnCustomFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "CFRI", "DSTFREQ", "DBLV", "沙尘频次统计图模版", true, true) == null)
                return;
        }

        void _btnCustomnDays_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "0MDS", "DST0MDS", "DBLV", "沙尘日数统计图模版", true, true) == null)
                return;
        }
        #endregion

        #region 周期合成
        /// <summary>
        /// 当前区域周期合成图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cur_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CycleTimeStat(_session, "CYCI", "DSTCYCI", "DBLV", "沙尘周期合成图模版", false, true) == null)
                return;
        }

        /// <summary>
        /// 自定义周期合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cus_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CycleTimeStat(_session, "CCYI", "DSTCYCI", "DBLV", "沙尘周期合成图模版", true, true) == null)
                return;
        }
        #endregion

        #region 特征量计算
        /// <summary>
        /// 能见度计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnVisibility_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("VISY");
            GetCommandAndExecute(6602);
        }

        void minOPTRE_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("QIPC");
            GetCommandAndExecute(6602);
        }

        #endregion

        #region 动画显示
        /// <summary>
        /// 当前区域动画显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnCurrent_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.CreatAVI(_session, "template:沙尘动画展示专题图,动画展示专题图,DST,MEDI");
        }

        /// <summary>
        /// 自定义动画显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnCustom_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.CreatAVI(_session, "template:沙尘动画展示专题图,动画展示专题图,DST,CMED");
        }
        #endregion

        /// <summary>
        /// 关闭沙尘判识工具栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnClose_Click(object sender, EventArgs e)
        {
            if (_session == null)
                return;
            _session.UIFrameworkHelper.Remove(_tab.Text);
            _session.UIFrameworkHelper.ActiveDefaultTab();
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveProduct(null);
        }
        //产品入库
        void btnToDb_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(39002);
            if (cmd != null)
                cmd.Execute();
        }

        private void GetCommand(int id)
        {
            ICommand cmd = _session.CommandEnvironment.Get(id);
            if (cmd != null)
                cmd.Execute("dust");
        }

        private void GetCommandAndExecute(int cmdID)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute("DST");
        }

        #endregion

        private void AddPicLayoutButton()
        {
            RadRibbonBarGroup rbgPicLayout = new RadRibbonBarGroup();
            rbgPicLayout.Text = "图像批量输出";
            btnPicLayout.ImageAlignment = ContentAlignment.TopCenter;
            btnPicLayout.TextAlignment = ContentAlignment.BottomCenter;
            btnPicLayout.Click += new EventHandler(btnHisLayoutput_Click);
            rbgPicLayout.Items.Add(btnPicLayout);
            _tab.Items.Add(rbgPicLayout);
        }
        //导出专题图按钮
        void btnHisLayoutput_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(36604);
            if (cmd != null)
                cmd.Execute("DST","png");

        }

        /// <summary>
        /// 真彩图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnNatrueColor_Click(object sender, EventArgs e)
        {
            IMonitoringSession monitoring = _session.MonitoringSession as IMonitoringSession;
            monitoring.CanResetUserControl();
            monitoring.ChangeActiveSubProduct("NCIM");
            GetCommandAndExecute(6602);
        }

    }
}
