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
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public partial class UITabSeaIce : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 海冰按钮定义

        #region 判识相关
        RadButtonElement btnRough = new RadButtonElement("自动判识");
        RadDropDownButtonElement btnAutoGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement btnInteractive = new RadDropDownButtonElement();
        #endregion

        #region 专题图相关
        RadDropDownButtonElement dbtnInfoExtract = new RadDropDownButtonElement();
        RadDropDownButtonElement btnConverDegreeImage = new RadDropDownButtonElement();
        #endregion

        #region 动画相关
        RadButtonElement btnAnimationRegion = new RadButtonElement("当前区域");
        RadButtonElement btnAnimationCustom = new RadButtonElement("自定义");
        #endregion

        #region 对比分析
        RadButtonElement btnCompareRaster = new RadButtonElement("计算");
        RadButtonElement btnCompareImg = new RadButtonElement("对比分析专题图");
        RadDropDownButtonElement dbtnCompareStat = new RadDropDownButtonElement();
        #endregion

        RadDropDownButtonElement dbtnOrdLayout = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnOrdStatArea = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnOrdGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnCycLayout = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnTFRLayout = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnCycGenerate = new RadDropDownButtonElement();

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
            btnRough.ImageAlignment = ContentAlignment.TopCenter;
            btnRough.TextAlignment = ContentAlignment.BottomCenter;
            btnRough.Click += new EventHandler(btnRough_Click);
            rbgCheck.Items.Add(btnRough);
            btnInteractive.Text = "交互判识";
            btnInteractive.ImageAlignment = ContentAlignment.TopCenter;
            btnInteractive.TextAlignment = ContentAlignment.BottomCenter;
            btnInteractive.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem mniIceCheck = new RadMenuItem("海冰");
            mniIceCheck.Click += new EventHandler(btnInteractive_Click);
            RadMenuItem mniCloud = new RadMenuItem("云");
            mniCloud.Click += new EventHandler(btnIceAutoCLMExtract_Click);
            btnInteractive.Items.AddRange(new RadMenuItem[] { mniIceCheck, mniCloud });
            rbgCheck.Items.Add(btnInteractive);
            //
            btnAutoGenerate.Text = "快速生成";
            btnAutoGenerate.ImageAlignment = ContentAlignment.TopCenter;
            btnAutoGenerate.TextAlignment = ContentAlignment.BottomCenter;
            btnAutoGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem roughProGenerate = new RadMenuItem("自动判识+专题产品");
            roughProGenerate.Click += new EventHandler(RoughProGenerate_Click);
            btnAutoGenerate.Items.Add(roughProGenerate);
            RadMenuItem proAutoGenerate = new RadMenuItem("专题产品");
            proAutoGenerate.Click += new EventHandler(ProAutoGenerate_Click);
            btnAutoGenerate.Items.Add(proAutoGenerate);
            rbgCheck.Items.Add(btnAutoGenerate);

            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "日常业务";
            dbtnInfoExtract.Text = "特征信息提取";
            dbtnInfoExtract.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnInfoExtract.ImageAlignment = ContentAlignment.TopCenter;
            dbtnInfoExtract.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniIceEdgeExtract = new RadMenuItem("冰缘线提取");
            mniIceEdgeExtract.Click += new EventHandler(btnIceEdgeExtract_Click);
            RadMenuItem mniIceAutoEdgeExtract = new RadMenuItem("自动冰缘线提取");
            mniIceAutoEdgeExtract.Click += new EventHandler(btnIceAutoEdgeExtract_Click);
            RadMenuItem mniTempExtract = new RadMenuItem("冰面温度提取");
            mniTempExtract.Click += new EventHandler(btnTempExtract_Click);
            RadMenuItem mniIceThinkness = new RadMenuItem("海冰厚度");
            mniIceThinkness.Click += new EventHandler(mniIceThinkness_Click);
            dbtnInfoExtract.Items.AddRange(new RadMenuItem[] { mniIceEdgeExtract, mniIceAutoEdgeExtract, mniTempExtract, mniIceThinkness });
            rbgProduct.Items.Add(dbtnInfoExtract);
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

            dbtnOrdLayout.Text = "专题产品";
            dbtnOrdLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnOrdLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnOrdLayout.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniMulImage = new RadMenuItem("多通道合成图");
            mniMulImage.Click += new EventHandler(btnMulImage_Click);
            dbtnOrdLayout.Items.Add(mniMulImage);
            RadMenuItem mniOMulImage = new RadMenuItem("多通道合成图（原始分辨率）");
            mniOMulImage.Click += new EventHandler(mniOMulImage_Click);
            dbtnOrdLayout.Items.Add(mniOMulImage);
            RadMenuItem mhiBinImgCurrent = new RadMenuItem("二值图（当前区域）");
            dbtnOrdLayout.Items.Add(mhiBinImgCurrent);
            mhiBinImgCurrent.Click += new EventHandler(mhiBinImgCurrent_Click);
            RadMenuItem IceEdgeImage = new RadMenuItem("冰缘线图");
            dbtnOrdLayout.Items.Add(IceEdgeImage);
            IceEdgeImage.Click += new EventHandler(IceEdgeImage_Click);
            RadMenuItem IceTempImage = new RadMenuItem("冰面温度图");
            dbtnOrdLayout.Items.Add(IceTempImage);
            IceTempImage.Click += new EventHandler(IceTempImage_Click);
            RadMenuItem IceThinknessImage = new RadMenuItem("海冰厚度图");
            dbtnOrdLayout.Items.Add(IceThinknessImage);
            IceThinknessImage.Click += new EventHandler(IceThinknessImage_Click);
            RadMenuItem IceAreaImage = new RadMenuItem("海冰面积图");
            dbtnOrdLayout.Items.Add(IceAreaImage);
            IceAreaImage.Click += new EventHandler(IceAreaImage_Click);
            RadMenuItem IceCoverageImage = new RadMenuItem("极区海冰覆盖度图");
            dbtnOrdLayout.Items.Add(IceCoverageImage);
            IceCoverageImage.Click += new EventHandler(IceCoverageImage_Click);
            rbgProduct.Items.Add(dbtnOrdLayout);

            dbtnOrdStatArea.Text = "统计分析";
            dbtnOrdStatArea.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnOrdStatArea.ImageAlignment = ContentAlignment.TopCenter;
            dbtnOrdStatArea.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniOrdCurrent = new RadMenuItem("当前区域");
            mniOrdCurrent.Click += new EventHandler(btnCurrentRegionArea_Click);
            RadMenuItem mniOrdCustom = new RadMenuItem("自定义");
            mniOrdCustom.Click += new EventHandler(btnCustomArea_Click);
            dbtnOrdStatArea.Items.AddRange(new RadMenuItem[] { mniOrdCurrent, mniOrdCustom });
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
            dbtnCompareStat.Items.Add(mniCompStatCurrent);
            rbgCompare.Items.AddRange(btnCompareRaster, btnCompareImg, dbtnCompareStat);

            RadRibbonBarGroup rbgCycBusiness = new RadRibbonBarGroup();
            rbgCycBusiness.Text = "周期业务";
            dbtnCycLayout.Text = "专题产品";
            dbtnCycLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnCycLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCycLayout.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniCurrentFreq = new RadMenuItem("累计天数（当前区域）");
            mniCurrentFreq.Click += new EventHandler(btnCurrentRegionFreq_Click);
            RadMenuItem mniCustomFreq = new RadMenuItem("累计天数（自定义）");
            mniCustomFreq.Click += new EventHandler(btnCustomFreq_Click);
            RadMenuItem mniCurrentCycTime = new RadMenuItem("周期合成（当前区域）");
            mniCurrentCycTime.Click += new EventHandler(btnCurrentRegionCycTime_Click);
            RadMenuItem mniCustomCycTime = new RadMenuItem("周期合成（自定义）");
            mniCustomCycTime.Click += new EventHandler(btnCustomCycTime_Click);
            dbtnCycLayout.Items.AddRange(new RadMenuItem[] { mniCurrentFreq, mniCustomFreq, mniCurrentCycTime, mniCustomCycTime });
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
            dbtnTFRLayout.Items.AddRange(new RadMenuItem[] { mniCurrentTfre, mniCurrentTfreImage, mniCurrentTfrq, mniCurrentTfrqImage });
            rbgCycBusiness.Items.Add(dbtnCycLayout);
            rbgCycBusiness.Items.Add(dbtnTFRLayout);
            dbtnCycGenerate.Text = "快速生成";
            dbtnCycGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnCycGenerate.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCycGenerate.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniCycProGenerate = new RadMenuItem("专题产品");
            mniCycProGenerate.Click += new EventHandler(mniCycProGenerate_Click);
            dbtnCycGenerate.Items.Add(mniCycProGenerate);
            //rbgCycBusiness.Items.Add(dbtnCycGenerate);

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
            _tab.Items.Add(rbgProduct);
            _tab.Items.Add(rbgCompare);
            _tab.Items.Add(rbgCycBusiness);
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
            dbtnCycGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnOrdGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnOrdStatArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            dbtnOrdLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            btnAnimationRegion.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            dbtnInfoExtract.Image = _session.UIFrameworkHelper.GetImage("system:iceTemp.png");
            btnConverDegreeImage.Image = _session.UIFrameworkHelper.GetImage("system:iceConver.png");
            dbtnCycLayout.Image = _session.UIFrameworkHelper.GetImage("system:iceCycTime.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnAnimationCustom.Image = _session.UIFrameworkHelper.GetImage("system:customVedio.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
            btnCompareRaster.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            dbtnCompareStat.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            btnCompareImg.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnTFRLayout.Image = _session.UIFrameworkHelper.GetImage("system:iceTimeFreq.png");
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
            AutoGenretateProvider.AutoGenrate(_session, "0IMG", null, "Cyc");
        }

        void mniCycProGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "FREQ", null, "Cyc");
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
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("EDGE");
            //GetCommandAndExecute(78000);
            GetCommandAndExecute(6601);
        }

        /// <summary>
        /// 自动冰缘线提取按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnIceAutoEdgeExtract_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "AEDG", "", "AEDG", "AEDGAlgorithm", "AEDG", "海冰冰缘线模板", false, false) == null)
                return;
        }

        /// <summary>
        /// 冰面温度提取按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnTempExtract_Click(object sender, EventArgs e)
        {
            try
            {
                dbtnInfoExtract.Enabled = false;
                IMonitoringSession ms = (_session.MonitoringSession as IMonitoringSession);
                ms.ChangeActiveSubProduct("ISOT");
                ms.DoManualExtract(true);
            }
            finally
            {
                dbtnInfoExtract.Enabled = true;
            }
        }

        /// <summary>
        /// 云判识按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnIceAutoCLMExtract_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0CLM");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 海冰厚度按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mniIceThinkness_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0SIT");
            GetCommandAndExecute(6602);
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
            //MIFCommAnalysis.DisplayMonitorShow(_session, "template:海冰产品多通道合成图,多通道合成图,ICE,MCSI");
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MCSI");
            ms.DoAutoExtract(true);
        }

        void mniOMulImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "OMCS");
            ms.DoAutoExtract(true);
        }

        /// <summary>
        /// 自定义覆盖度图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnConverDImageCustom_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(78001);
            if (cmd == null)
                return;
            cmd.Execute("DEGX");
        }

        /// <summary>
        /// 0.25度覆盖度图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnConverDImage0_25_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(78001);
            if (cmd == null)
                return;
            cmd.Execute("DEG2");
        }

        /// <summary>
        /// 0.1度覆盖度图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnConverDImage0_1_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(78001);
            if (cmd == null)
                return;
            cmd.Execute("DEG1");
        }


        /// <summary>
        /// 冰面温度专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void IceTempImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("ISOI");
            ms.DoAutoExtract(true);
            //MIFCommAnalysis.CreateThemeGraphy(_session, "ISOI", "ICEISOT", "ISOT", "海冰等温线模板", false, false);
        }

        /// <summary>
        /// 冰缘线专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void IceEdgeImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("EDGI");
            ms.DoAutoExtract(true);
        }

        /// <summary>
        /// 海冰厚度专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void IceThinknessImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("SITI");
            ms.DoAutoExtract(true);
        }

        void IceAreaImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("SIAI");
            ms.DoAutoExtract(true);
        }

        void IceCoverageImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("PICI");
            GetCommandAndExecute(6602);
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
            if (MIFCommAnalysis.CycleTimeStat(_session, "CCYI", "ICECYCT", "DBLV", "海冰周期统计专题图模板", true, true) == null)
                return;
        }

        /// <summary>
        /// 当前区域多时段合成按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCurrentRegionCycTime_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CycleTimeStat(_session, "CCYI", "ICECYCT", "DBLV", "海冰周期统计专题图模板", false, true) == null)
                return;
        }

        #endregion

        #region 对比分析相关按钮

        public void btnCompareRaster_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("COMP");
            GetCommandAndExecute(6602);
        }

        public void btnCompareImg_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "COMI");
            ms.DoAutoExtract(true);
        }

        public void mniCompStatCurrent_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "0CCA", "COMA", "COMA", "", false, true) == null)
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
            cmd.Execute("ICE");
        }
    }
}
