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
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.UI.AddIn.DataPro;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    public partial class UITabSeaIce : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 海冰按钮定义

        #region 判识相关
        RadButtonElement btnRough = new RadButtonElement("粗判");
        RadDropDownButtonElement btnAutoGenerate = new RadDropDownButtonElement();
        RadButtonElement btnInteractive = new RadButtonElement("交互判识");
        #endregion

        #region 专题图相关
        RadButtonElement btnMulImage = new RadButtonElement("多通道合成图");
        RadDropDownButtonElement dbtnbinImage = new RadDropDownButtonElement();
        RadDropDownButtonElement btnConverDegreeImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnQuantifyImage = new RadDropDownButtonElement();

        RadButtonElement btnIceEdgeImage = new RadButtonElement("冰缘线图");
        RadButtonElement btnTempImage = new RadButtonElement("冰面温度图");
        #endregion

        #region 面积统计相关
        RadButtonElement btnCurrentRegionArea = new RadButtonElement("当前区域");
        RadButtonElement btnCustomArea = new RadButtonElement("自定义");
        #endregion

        #region 频次统计相关
        RadButtonElement btnCurrentRegionFreq = new RadButtonElement("当前区域");
        RadButtonElement btnCustomFreq = new RadButtonElement("自定义");
        #endregion

        #region 动画相关
        RadButtonElement btnAnimationRegion = new RadButtonElement("当前区域");
        RadButtonElement btnAnimationCustom = new RadButtonElement("自定义");
        #endregion

        #region 特征信息提取相关
        RadButtonElement btnIceEdgeExtract = new RadButtonElement("冰缘线提取");
        RadButtonElement btnTempExtract = new RadButtonElement("冰面温度提取");
        #endregion

        #region 多时段合成相关
        RadButtonElement btnCurrentRegionCycTime = new RadButtonElement("当前区域");
        RadButtonElement btnCustomCycTime = new RadButtonElement("自定义");
        #endregion


        RadButtonElement btnClose = new RadButtonElement("关闭海冰监测\n分析视图");

        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        #endregion

        public UITabSeaIce()
        {
            InitializeComponent();
            CreateRibbonBar();
        }

        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "海冰监测专题";
            _tab.Text = "海冰监测专题";
            _tab.Name = "海冰监测专题";
            _bar.CommandTabs.Add(_tab);

            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            rbgCheck.Text = "监测";
            rbgCheck.Items.Add(btnRough);
            rbgCheck.Items.Add(btnInteractive);
            btnInteractive.Click += new EventHandler(btnInteractive_Click);
            foreach (RadButtonElement item in rbgCheck.Items)
            {
                item.ImageAlignment = ContentAlignment.TopCenter;
                item.TextAlignment = ContentAlignment.BottomCenter;
            }
            btnRough.Click += new EventHandler(btnRough_Click);
            //
            btnAutoGenerate.Text = "快速生成";
            btnAutoGenerate.ImageAlignment = ContentAlignment.TopCenter;
            btnAutoGenerate.TextAlignment = ContentAlignment.BottomCenter;
            btnAutoGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem roughProGenerate = new RadMenuItem("粗判+专题产品");
            roughProGenerate.Click += new EventHandler(RoughProGenerate_Click);
            btnAutoGenerate.Items.Add(roughProGenerate);
            RadMenuItem proAutoGenerate = new RadMenuItem("专题产品");
            proAutoGenerate.Click += new EventHandler(ProAutoGenerate_Click);
            btnAutoGenerate.Items.Add(proAutoGenerate);
            rbgCheck.Items.Add(btnAutoGenerate);
            //

            RadRibbonBarGroup rbgQuantify = new RadRibbonBarGroup();
            rbgQuantify.Text = "特征信息提取";
            btnIceEdgeExtract.ImageAlignment = ContentAlignment.TopCenter;
            btnIceEdgeExtract.TextAlignment = ContentAlignment.BottomCenter;
            rbgQuantify.Items.Add(btnIceEdgeExtract);
            btnIceEdgeExtract.Click += new EventHandler(btnIceEdgeExtract_Click);
            btnTempExtract.ImageAlignment = ContentAlignment.TopCenter;
            btnTempExtract.TextAlignment = ContentAlignment.BottomCenter;
            rbgQuantify.Items.Add(btnTempExtract);
            btnTempExtract.Click += new EventHandler(btnTempExtract_Click);

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

            btnConverDegreeImage.Text = "覆盖度图";
            btnConverDegreeImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            btnConverDegreeImage.ImageAlignment = ContentAlignment.TopCenter;
            btnConverDegreeImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem btnConverDImage0_1 = new RadMenuItem("0.1度");
            btnConverDegreeImage.Items.Add(btnConverDImage0_1);
            btnConverDImage0_1.Click += new EventHandler(btnConverDImage0_1_Click);
            RadMenuItem btnConverDImage0_25 = new RadMenuItem("0.25度");
            btnConverDegreeImage.Items.Add(btnConverDImage0_25);
            btnConverDImage0_25.Click += new EventHandler(btnConverDImage0_25_Click);
            RadMenuItem btnConverDImageCustom = new RadMenuItem("自定义");
            btnConverDegreeImage.Items.Add(btnConverDImageCustom);
            btnConverDImageCustom.Click += new EventHandler(btnConverDImageCustom_Click);
            rbgProduct.Items.Add(btnConverDegreeImage);

            dbtnQuantifyImage.Text = "特征专题图";
            dbtnQuantifyImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnQuantifyImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnQuantifyImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem IceEdgeImage = new RadMenuItem("冰缘线图");
            dbtnQuantifyImage.Items.Add(IceEdgeImage);
            IceEdgeImage.Click += new EventHandler(IceEdgeImage_Click);
            RadMenuItem IceTempImage = new RadMenuItem("冰面温度图");
            dbtnQuantifyImage.Items.Add(IceTempImage);
            IceTempImage.Click += new EventHandler(IceTempImage_Click);
            rbgProduct.Items.Add(dbtnQuantifyImage);

            RadRibbonBarGroup rbgAreaStatic = new RadRibbonBarGroup();
            rbgAreaStatic.Text = "面积统计";
            btnCurrentRegionArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCurrentRegionArea.TextAlignment = ContentAlignment.BottomCenter;
            rbgAreaStatic.Items.Add(btnCurrentRegionArea);
            btnCurrentRegionArea.Click += new EventHandler(btnCurrentRegionArea_Click);
            btnCustomArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCustomArea.TextAlignment = ContentAlignment.BottomCenter;
            rbgAreaStatic.Items.Add(btnCustomArea);
            btnCustomArea.Click += new EventHandler(btnCustomArea_Click);

            RadRibbonBarGroup rbgCycTimeStatic = new RadRibbonBarGroup();
            rbgCycTimeStatic.Text = "多时段合成";
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
            btnAutoGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnInteractive.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            btnMulImage.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnbinImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            btnCurrentRegionArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            btnCustomArea.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnCustomFreq.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnCurrentRegionFreq.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
            btnAnimationRegion.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            btnIceEdgeExtract.Image = _session.UIFrameworkHelper.GetImage("system:iceBrinK.png");
            btnTempExtract.Image = _session.UIFrameworkHelper.GetImage("system:iceTemp.png");
            btnConverDegreeImage.Image = _session.UIFrameworkHelper.GetImage("system:iceConver.png");
            dbtnQuantifyImage.Image = _session.UIFrameworkHelper.GetImage("system:iceQuantify.png");
            btnCurrentRegionCycTime.Image = _session.UIFrameworkHelper.GetImage("system:iceCycTime.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnAnimationCustom.Image = _session.UIFrameworkHelper.GetImage("system:customVedio.png");
            btnCustomCycTime.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");

            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
        }


        public object Content
        {
            get { return _tab; }
        }

        #endregion

        private void GetCommand(int id)
        {
            ICommand cmd = _session.CommandEnvironment.Get(id);
            if (cmd != null)
                cmd.Execute("ice");
        }

        #region 判识相关按钮事件

        /// <summary>
        /// 粗判+专题产品快速生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RoughProGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "DBLV");
        }

        /// <summary>
        /// 专题产品快速生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ProAutoGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "0IMG");
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
        /// 交互按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnInteractive_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExecute(6602);
        }

        #endregion

        #region 特征信息提取相关按钮事件

        /// <summary>
        /// 冰缘线提取按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnIceEdgeExtract_Click(object sender, EventArgs e)
        {
        }


        /// <summary>
        /// 冰面温度提取按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnTempExtract_Click(object sender, EventArgs e)
        {
            frmEqualValueCal evc = new frmEqualValueCal(_session);
            evc.Show();
        }

        #endregion

        #region 专题图产品相关按钮事件

        /// <summary>
        /// 自定义二值图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiBinImgCustom_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "CSDI", "", "DBLV", "海冰二值图模板", true, true) == null)
                return;
        }

        /// <summary>
        /// 当前区域二值图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiBinImgCurrent_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "DBLV", "海冰二值图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 多通道合成图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMulImage_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:海冰产品多通道合成图,多通道合成图,ICE,MCSI");
        }

        /// <summary>
        /// 自定义覆盖度图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnConverDImageCustom_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 0.25度覆盖度图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnConverDImage0_25_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 0.1度覆盖度图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnConverDImage0_1_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 冰面温度专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void IceTempImage_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 冰缘线专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void IceEdgeImage_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region 多时段合成相关按钮事件

        /// <summary>
        /// 自定义多时段合成按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCustomCycTime_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CycleTimeStat(_session, "CCYI", "", "DBLV", "海冰周期统计专题图模板", true, true) == null)
                return;
        }

        /// <summary>
        /// 当前区域多时段合成按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCurrentRegionCycTime_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CycleTimeStat(_session, "CCYI", "", "DBLV", "海冰周期统计专题图模板", false, true) == null)
                return;
        }

        #endregion

        #region 面积统计相关按钮事件
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
        /// 当前区域面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCurrentRegionArea_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.AreaStat(_session, "CCAR", "", false, true) == null)
                return;
        }
        #endregion

        #region 频次统计按钮事件
        /// <summary>
        /// 自定义频次统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCustomFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "FREQ", "", "DBLV", "海冰频次统计专题图模板", true, true) == null)
                return;
        }

        /// <summary>
        ///  当前区域频次统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCurrentRegionFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "FREQ", "", "DBLV", "海冰频次统计专题图模板", false, true) == null)
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
            MIFCommAnalysis.CreatAVI(_session, "template:海冰动画专题图,动画展示专题图,ICE,CMED");
        }

        /// <summary>
        /// 当前区域动画按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAnimationRegion_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.CreatAVI(_session, "template:海冰动画专题图,动画展示专题图,ICE,MEDI");
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
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct(null);
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
            cmd.Execute("ICE");
        }
    }
}
