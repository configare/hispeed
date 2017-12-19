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

namespace GeoDo.RSS.UI.AddIn.CMA
{
    public partial class UITabSandDust : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 沙尘按钮定义
        RadButtonElement btnRough = new RadButtonElement("粗判");
        RadDropDownButtonElement btnSave = new RadDropDownButtonElement();
        RadButtonElement btnInteractive = new RadButtonElement("交互判识");

        //专题产品
        RadButtonElement _btnMonitorShow = null;  //"沙尘监测示意图"
        RadDropDownButtonElement _dbtnbinImage = null; //二值图
        RadDropDownButtonElement _dbtnWeekImerg = null; //周期合成图
        RadButtonElement _btnVisibityImage = null; //"能见度图"

        //动画显示
        RadButtonElement _btnCurrent = null; //当前区域
        RadButtonElement _btnCustom = null; // 自定义

        //统计分析
        RadButtonElement _btnVisibility = null; //能见度计算
        RadButtonElement _btnCurrentRegionArea = null;
        RadDropDownButtonElement _dbtnDivision = null;//行政区划
        RadButtonElement _btnLandType = null;
        RadButtonElement _btnCustomArea = null;
        RadButtonElement _btnCurrentRegionFreq = null;
        RadButtonElement _btnCustomFreq = null;

        RadButtonElement _btnClose = null;
        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        #endregion

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
            rbgCheck.Items.Add(btnInteractive);
            foreach (RadButtonElement item in rbgCheck.Items)
            {
                item.ImageAlignment = ContentAlignment.TopCenter;
                item.TextAlignment = ContentAlignment.BottomCenter;
            }

            btnSave.Text = "快速生成";
            RadMenuItem item1 = new RadMenuItem();
            item1.Text = "粗判+专题产品";
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
            btnInteractive.Click += new EventHandler(btnInteractive_Click);
            //btnSave.Click += new EventHandler(btnSave_Click);
            _tab.Items.Add(rbgCheck);

            CreatThemeProductGroup(); //专题产品
            CreatComputGroup();  //统计计算：能见度计算
            CreatAreaStatic(); //面积统计
            CreatFreqStatic(); //频次统计
            CreatWeekImerg(); //周期合成
            CreatCartoon(); //动画显示
            
            btnToDb.TextAlignment = ContentAlignment.BottomCenter;
            btnToDb.ImageAlignment = ContentAlignment.TopCenter;
            btnToDb.Click += new EventHandler(btnToDb_Click);
            RadRibbonBarGroup rbgToDb = new RadRibbonBarGroup();
            rbgToDb.Text = "产品入库";
            rbgToDb.Items.Add(btnToDb);
            _tab.Items.Add(rbgToDb);

            CreatClose(); // 关闭
            Controls.Add(_bar);
        }

        private void CreatThemeProductGroup()
        {
            RadRibbonBarGroup themeGroup = new RadRibbonBarGroup();
            themeGroup.Text = "专题产品";

            _btnMonitorShow = new RadButtonElement("沙尘监测\n示意图");
            _btnMonitorShow.ImageAlignment = ContentAlignment.TopCenter;
            _btnMonitorShow.TextAlignment = ContentAlignment.BottomCenter;
            _btnMonitorShow.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnMonitorShow.Click += new EventHandler(_btnMonitorShow_Click);

            _dbtnbinImage = new RadDropDownButtonElement();
            _dbtnbinImage.Text = "二值图";
            _dbtnbinImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            _dbtnbinImage.ImageAlignment = ContentAlignment.TopCenter;
            _dbtnbinImage.TextAlignment = ContentAlignment.BottomCenter;
            _dbtnbinImage.TextImageRelation = TextImageRelation.ImageAboveText;
            RadMenuItem curItem = new RadMenuItem("当前区域");
            curItem.Click += new EventHandler(curItem_Click);
            RadMenuItem cusItem = new RadMenuItem("自定义");
            cusItem.Click += new EventHandler(cusItem_Click);
            _dbtnbinImage.Items.AddRange(new RadItem[] { curItem, cusItem });

            _btnVisibityImage = new RadButtonElement("能见度图");
            _btnVisibityImage.TextAlignment = ContentAlignment.BottomCenter;
            _btnVisibityImage.ImageAlignment = ContentAlignment.TopCenter;
            _btnVisibityImage.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnVisibityImage.Click += new EventHandler(_btnVisibityImage_Click);

            themeGroup.Items.AddRange(new RadItem[] {  _btnMonitorShow, _dbtnbinImage, 
                                                        _btnVisibityImage});
            _tab.Items.Add(themeGroup);
        }

        private void CreatComputGroup()
        {
            RadRibbonBarGroup computGroup = new RadRibbonBarGroup();
            computGroup.Text = "特征信息提取";
            _btnVisibility = new RadButtonElement("能见度计算");
            _btnVisibility.ImageAlignment = ContentAlignment.TopCenter;
            _btnVisibility.TextAlignment = ContentAlignment.BottomCenter;
            _btnVisibility.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnVisibility.Click += new EventHandler(_btnVisibility_Click);
            computGroup.Items.Add(_btnVisibility);
            _tab.Items.Add(computGroup);
        }

        private void CreatAreaStatic()
        {
            RadRibbonBarGroup rbgAreaStatic = new RadRibbonBarGroup();
            rbgAreaStatic.Text = "面积统计";
            _btnCurrentRegionArea = new RadButtonElement("当前区域");
            _btnCurrentRegionArea.ImageAlignment = ContentAlignment.TopCenter;
            _btnCurrentRegionArea.TextAlignment = ContentAlignment.BottomCenter;
            _btnCurrentRegionArea.Click += new EventHandler(_btnCurrentRegionArea_Click);

            _dbtnDivision = new RadDropDownButtonElement();
            _dbtnDivision.Text = "行政区划";
            _dbtnDivision.TextAlignment = ContentAlignment.BottomCenter;
            _dbtnDivision.ImageAlignment = ContentAlignment.TopCenter;
            RadMenuItem mhiCity = new RadMenuItem("按省界");
            mhiCity.Click += new EventHandler(mhiCity_Click);
            _dbtnDivision.Items.Add(mhiCity);
            RadMenuItem mhiProvincBound = new RadMenuItem("按市县");
            mhiProvincBound.Click += new EventHandler(_dbtnDivision_Click);
            _dbtnDivision.Items.Add(mhiProvincBound);

            _btnLandType = new RadButtonElement("土地类型");
            _btnLandType.ImageAlignment = ContentAlignment.TopCenter;
            _btnLandType.TextAlignment = ContentAlignment.BottomCenter;
            _btnLandType.Click += new EventHandler(_btnLandType_Click);

            _btnCustomArea = new RadButtonElement("自定义");
            _btnCustomArea.ImageAlignment = ContentAlignment.TopCenter;
            _btnCustomArea.TextAlignment = ContentAlignment.BottomCenter;
            _btnCustomArea.Click += new EventHandler(_btnCustomArea_Click);

            rbgAreaStatic.Items.AddRange(new RadItem[] { _btnCurrentRegionArea, _dbtnDivision,
                                                                                      _btnLandType,_btnCustomArea});
            _tab.Items.Add(rbgAreaStatic);
        }

        private void CreatFreqStatic()
        {
            RadRibbonBarGroup rbgFreqStatic = new RadRibbonBarGroup();
            rbgFreqStatic.Text = "频次统计";
            _btnCurrentRegionFreq = new RadButtonElement("当前区域");
            _btnCurrentRegionFreq.ImageAlignment = ContentAlignment.TopCenter;
            _btnCurrentRegionFreq.TextAlignment = ContentAlignment.BottomCenter;
            _btnCurrentRegionFreq.Click += new EventHandler(_btnCurrentRegionFreq_Click);

            _btnCustomFreq = new RadButtonElement("自定义");
            _btnCustomFreq.TextAlignment = ContentAlignment.BottomCenter;
            _btnCustomFreq.ImageAlignment = ContentAlignment.TopCenter;
            _btnCustomFreq.Click += new EventHandler(_btnCustomFreq_Click);

            rbgFreqStatic.Items.AddRange(new RadItem[] { _btnCurrentRegionFreq, _btnCustomFreq });
            _tab.Items.Add(rbgFreqStatic);
        }

        private void CreatWeekImerg()
        {
            RadRibbonBarGroup weekImerg = new RadRibbonBarGroup();
            weekImerg.Text = "周期合成";

            _dbtnWeekImerg = new RadDropDownButtonElement();
            _dbtnWeekImerg.Text = "周期合成图";
            _dbtnWeekImerg.ArrowPosition = DropDownButtonArrowPosition.Right;
            _dbtnWeekImerg.ImageAlignment = ContentAlignment.TopCenter;
            _dbtnWeekImerg.TextAlignment = ContentAlignment.BottomCenter;
            _dbtnWeekImerg.TextImageRelation = TextImageRelation.ImageAboveText;
            RadMenuItem cur = new RadMenuItem("当前区域");
            cur.Click += new EventHandler(cur_Click);
            RadMenuItem cus = new RadMenuItem("自定义");
            _dbtnWeekImerg.Items.AddRange(new RadItem[] { cur, cus });
            cus.Click += new EventHandler(cus_Click);

            weekImerg.Items.Add(_dbtnWeekImerg);
            _tab.Items.Add(weekImerg);
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

        private void SetImage()
        {
            btnRough.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            _dbtnDivision.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            btnSave.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnInteractive.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            _dbtnbinImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            _btnLandType.Image = _session.UIFrameworkHelper.GetImage("system:landType.png");
            _btnCustomArea.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            _btnCustomFreq.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            _btnCurrentRegionFreq.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
            _btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            _btnCurrent.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            _btnCustom.Image = _session.UIFrameworkHelper.GetImage("system:customVedio.png");
            _btnVisibility.Image = _session.UIFrameworkHelper.GetImage("system:sand_visibility.png");
            _btnMonitorShow.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            _dbtnWeekImerg.Image = _session.UIFrameworkHelper.GetImage("system:fogCycTime.png");
            _btnCurrentRegionArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            _btnVisibityImage.Image = _session.UIFrameworkHelper.GetImage("system:fogQuantify.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
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
            SetImage();
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
            if (MIFCommAnalysis.AreaStat(_session, "CLUT", "土地利用类型", false, true) == null)
                return;
        }

        /// <summary>
        /// 面积统计—按省界
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiCity_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.AreaStat(_session, "0CBP", "省级行政区划", false, true) == null)
                return;
        }

        /// <summary>
        /// 面积统计—按市县
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _dbtnDivision_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.AreaStat(_session, "0CCC", "分级行政区划", false, true) == null)
                return;
        }
        #endregion

        #region 专题图产品
        /// <summary>
        /// 生成沙尘监测示意图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnMonitorShow_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:沙尘监测示意图模版,监测示意图,DST,MCSI");
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
            if (MIFCommAnalysis.TimeStat(_session, "FREQ", "", "DBLV", "沙尘频次统计图模版", false, true) == null)
                return;
        }

        /// <summary>
        /// 自定义区域频次统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnCustomFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "CFRI", "", "DBLV", "沙尘频次统计图模版", true, true) == null)
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
            if (MIFCommAnalysis.CycleTimeStat(_session, "CYCI", "", "DBLV", "沙尘周期合成图模版", false, true) == null)
                return;
        }

        /// <summary>
        /// 自定义周期合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cus_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CycleTimeStat(_session, "CCYI", "", "DBLV", "沙尘周期合成图模版", true, true) == null)
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
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct(null);
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

    }
}
