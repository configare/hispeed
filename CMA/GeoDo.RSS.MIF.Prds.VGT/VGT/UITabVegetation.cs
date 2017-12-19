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

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public partial class UITabVegetation : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 植被按钮定义
        RadDropDownButtonElement _btnRough = new RadDropDownButtonElement();
        RadButtonElement _btnSave = new RadButtonElement("批量生产");

        RadDropDownButtonElement _dbtnDataCL = new RadDropDownButtonElement();//数据合成

        RadDropDownButtonElement _dbtnComparaCom = new RadDropDownButtonElement();
        RadDropDownButtonElement _dbtnComparaImg = new RadDropDownButtonElement();
        RadDropDownButtonElement _dbtnComparaSta = new RadDropDownButtonElement();

        RadDropDownButtonElement _btnNdviImage = null; //多通道合成图
        RadDropDownButtonElement _btnProcuctImageGp= null;//("产品专题图");
        RadButtonElement _btnSmooth = null;//数据平滑

        RadButtonElement _btnNdviStatic = null; //行政区划
        RadButtonElement _btnAvgStatic = null; //平均值统计
        RadButtonElement _btnCustomStatic = null; //自定义

        RadButtonElement _btnClose = new RadButtonElement("关闭植被监测\n分析视图");

        RadDropDownButtonElement _dbtnDataProduct = null; //数据生产

        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        #endregion

        public UITabVegetation()
        {
            InitializeComponent();
            CreateRibbonBar();
        }

        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "植被监测专题";
            _tab.Text = "植被监测专题";
            _tab.Name = "植被监测专题";
            _bar.CommandTabs.Add(_tab);

            CreatComputeGroup(); //数据生产
            CreatThemeGraphGroup(); //专题图
            CreatStatGroup();//统计分析     
  
            btnToDb.TextAlignment = ContentAlignment.BottomCenter;
            btnToDb.ImageAlignment = ContentAlignment.TopCenter;
            btnToDb.Click += new EventHandler(btnToDb_Click);
            RadRibbonBarGroup rbgToDb = new RadRibbonBarGroup();
            rbgToDb.Text = "产品入库";
            rbgToDb.Items.Add(btnToDb);
            _tab.Items.Add(rbgToDb);
         

            RadRibbonBarGroup rbgClose = new RadRibbonBarGroup();
            rbgClose.Text = "关闭";
            _btnClose.ImageAlignment = ContentAlignment.TopCenter;
            _btnClose.TextAlignment = ContentAlignment.BottomCenter;
            _btnClose.Click += new EventHandler(btnClose_Click);
            rbgClose.Items.Add(_btnClose);

            _tab.Items.Add(rbgClose);
            Controls.Add(_bar);
        }

        //统计分析
        private void CreatStatGroup()
        {
            RadRibbonBarGroup rbgAreaStatic = new RadRibbonBarGroup();
            rbgAreaStatic.Text = "统计分析";
            _btnNdviStatic = new RadButtonElement("行政区划");
            _btnNdviStatic.ImageAlignment = ContentAlignment.TopCenter;
            _btnNdviStatic.TextAlignment = ContentAlignment.BottomCenter;
            _btnNdviStatic.Click += new EventHandler(btnNdviStatic_Click);
            rbgAreaStatic.Items.Add(_btnNdviStatic);

            _btnAvgStatic = new RadButtonElement("平均值统计");
            _btnAvgStatic.ImageAlignment = ContentAlignment.TopCenter;
            _btnAvgStatic.TextAlignment = ContentAlignment.BottomCenter;
            _btnAvgStatic.Click += new EventHandler(_btnAvgStatic_Click);
            rbgAreaStatic.Items.Add(_btnAvgStatic);

            _btnCustomStatic = new RadButtonElement("自定义");
            _btnCustomStatic.ImageAlignment = ContentAlignment.TopCenter;
            _btnCustomStatic.TextAlignment = ContentAlignment.BottomCenter;
            _btnCustomStatic.Click += new EventHandler(_btnCustomStatic_Click);
            rbgAreaStatic.Items.Add(_btnCustomStatic);

            _tab.Items.Add(rbgAreaStatic);
        }

        //生成专题图
        private void CreatThemeGraphGroup()
        {
            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "专题产品";
            _btnNdviImage = new RadDropDownButtonElement();
            _btnNdviImage.Text = "多通道合成图";
            _btnNdviImage.ImageAlignment = ContentAlignment.TopCenter;
            _btnNdviImage.TextAlignment = ContentAlignment.BottomCenter;
            _btnNdviImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem mnuMulLayout = new RadMenuItem("多通道合成图");
            mnuMulLayout.Click += new EventHandler(mnuMulLayout_Click);
            RadMenuItem mnuOMulLayout = new RadMenuItem("多通道合成图（原始分辩率）");
            mnuOMulLayout.Click += new EventHandler(mnuOMulLayout_Click);
            _btnNdviImage.Items.AddRange(mnuMulLayout,mnuOMulLayout);

            _btnProcuctImageGp = new RadDropDownButtonElement();
            _btnProcuctImageGp.Text = "产品专题图";
            _btnProcuctImageGp.ImageAlignment = ContentAlignment.TopCenter;
            _btnProcuctImageGp.TextAlignment = ContentAlignment.BottomCenter;

            RadMenuItem btnVciImg = new RadMenuItem("产品专题图");
            btnVciImg.ImageAlignment = ContentAlignment.TopCenter;
            btnVciImg.TextAlignment = ContentAlignment.BottomCenter;
            btnVciImg.Click += new EventHandler(btnNdviImg_Click);
            RadMenuItem btnVciOrginImg = new RadMenuItem("产品专题图（原始分辩率）");
            btnVciOrginImg.ImageAlignment = ContentAlignment.TopCenter;
            btnVciOrginImg.TextAlignment = ContentAlignment.BottomCenter;
            btnVciOrginImg.Click += new EventHandler(btnVciOrginImg_Click);

            _btnProcuctImageGp.Items.Add(btnVciImg);
            _btnProcuctImageGp.Items.Add(btnVciOrginImg);

            rbgProduct.Items.Add(_btnNdviImage);
            rbgProduct.Items.Add(_btnProcuctImageGp);
            _tab.Items.Add(rbgProduct);
        }

        //指数计算
        private void CreatComputeGroup()
        {
            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            rbgCheck.Text = "数据生产";
            _btnRough.Text = "植被指数计算";
            _btnRough.ArrowPosition = DropDownButtonArrowPosition.Right;
            _btnRough.ImageAlignment = ContentAlignment.TopCenter;
            _btnRough.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniAllGenerate = new RadMenuItem("全部指数");
            mniAllGenerate.Click += new EventHandler(btnSave_Click);
            _btnRough.Items.Add(mniAllGenerate);
            RadMenuItem btnNdviCom = new RadMenuItem("NDVI计算");
            btnNdviCom.Click += new EventHandler(btnNdviCom_Click);
            _btnRough.Items.Add(btnNdviCom);
            RadMenuItem btnRviCom = new RadMenuItem("RVI计算");
            btnRviCom.Click += new EventHandler(btnRviCom_Click);
            _btnRough.Items.Add(btnRviCom);
            RadMenuItem btnDviCom = new RadMenuItem("DVI计算");
            btnDviCom.Click += new EventHandler(btnDviCom_Click);
            _btnRough.Items.Add(btnDviCom);
            RadMenuItem btnEviCom = new RadMenuItem("EVI计算");
            btnEviCom.Click += new EventHandler(btnEviCom_Click);
            _btnRough.Items.Add(btnEviCom);
            RadMenuItem mniVCI = new RadMenuItem("VCI计算");
            mniVCI.Click += new EventHandler(btnVciCom_Click);
            _btnRough.Items.Add(mniVCI);
            RadMenuItem btnLAICom = new RadMenuItem("LAI计算");
            btnLAICom.Click += new EventHandler(btnLAICom_Click);
            _btnRough.Items.Add(btnLAICom);
            RadMenuItem mniCloudExtract = new RadMenuItem("云判识");
            mniCloudExtract.Click += new EventHandler(mniCloudExtract_Click);
            _btnRough.Items.Add(mniCloudExtract);
            rbgCheck.Items.Add(_btnRough);

            _dbtnDataCL.ArrowPosition = DropDownButtonArrowPosition.Right;
            _dbtnDataCL.ImageAlignment = ContentAlignment.TopCenter;
            _dbtnDataCL.TextAlignment = ContentAlignment.BottomCenter;
            _dbtnDataCL.Text = "数据合成";
            RadMenuItem btnAvgMake = new RadMenuItem("平均值合成");
            btnAvgMake.Click += new EventHandler(btnAvgMake_Click);
            _dbtnDataCL.Items.Add(btnAvgMake);
            RadMenuItem btnMaxMake = new RadMenuItem("最大值合成");
            btnMaxMake.Click += new EventHandler(btnMaxMake_Click);
            _dbtnDataCL.Items.Add(btnMaxMake);
            RadMenuItem btnMinMake = new RadMenuItem("最小值合成");
            btnMinMake.Click += new EventHandler(btnMinMake_Click);
            _dbtnDataCL.Items.Add(btnMinMake);
            rbgCheck.Items.Add(_dbtnDataCL);

            _dbtnComparaCom.ArrowPosition = DropDownButtonArrowPosition.Right;
            _dbtnComparaCom.ImageAlignment = ContentAlignment.TopCenter;
            _dbtnComparaCom.TextAlignment = ContentAlignment.BottomCenter;
            _dbtnComparaCom.Text = "对比计算";
            RadMenuItem btnAvgCom = new RadMenuItem("距平计算");
            btnAvgCom.Click += new EventHandler(btnAvgCom_Click);
            _dbtnComparaCom.Items.Add(btnAvgCom);
            RadMenuItem btnChaZhi = new RadMenuItem("差值计算");
            btnChaZhi.Click += new EventHandler(btnChaZhi_Click);
            _dbtnComparaCom.Items.Add(btnChaZhi);
            rbgCheck.Items.Add(_dbtnComparaCom);

            _btnSmooth = new RadButtonElement("数据平滑");
            _btnSmooth.ImageAlignment = ContentAlignment.TopCenter;
            _btnSmooth.TextAlignment = ContentAlignment.BottomCenter;
            _btnSmooth.Click += new EventHandler(btnData_Click);
            rbgCheck.Items.Add(_btnSmooth);
            _tab.Items.Add(rbgCheck);
        }

        private void CreatWeekStat()
        {
            _tab.Items.Add(_dbtnDataProduct);
        }

        #region 周期统计
        //年平均值合成
        void Days365Raster_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("CYCA");
            GetCommandAndExecute(6602);
            IMonitoringSubProduct msp = (_session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
            msp.ArgumentProvider.SetArg("CYCAIdentify", "CYCAAvg");
            IExtractResult result = msp.Make(null);
            DisplayResultClass.DisplayResult(_session, msp, result, false);
        }
        //季平均值合成
        void Days90Raster_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("CYCA");
            GetCommandAndExecute(6602);
            IMonitoringSubProduct msp = (_session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
            msp.ArgumentProvider.SetArg("CYCAIdentify", "CYCAAvg");
            IExtractResult result = msp.Make(null);
            DisplayResultClass.DisplayResult(_session, msp, result, false);
        }
        //月平均值合成
        void Days30Raster_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("CYCA");
            GetCommandAndExecute(6602);
            IMonitoringSubProduct msp = (_session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
            msp.ArgumentProvider.SetArg("CYCAIdentify", "CYCAAvg");
            IExtractResult result = msp.Make(null);
            DisplayResultClass.DisplayResult(_session, msp, result, false);
        }
        //旬平均值合成
        void Days10Raster_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("CYCA");
            GetCommandAndExecute(6602);
            IMonitoringSubProduct msp = (_session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
            msp.ArgumentProvider.SetArg("CYCAIdentify", "CYCAAvg");
            IExtractResult result = msp.Make(null);
            DisplayResultClass.DisplayResult(_session, msp, result, false);
        }
        //周最大值合成
        void Days7Raster_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("CYCA");
            GetCommandAndExecute(6602);
            IMonitoringSubProduct msp = (_session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
            msp.ArgumentProvider.SetArg("CYCAIdentify", "CYCAMax");
            IExtractResult result = msp.Make(null);
            DisplayResultClass.DisplayResult(_session, msp, result, false);
        }
        //日最大值合成
        void DayRaster_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("CYCA");
            GetCommandAndExecute(6602);
            IMonitoringSubProduct msp = (_session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
            msp.ArgumentProvider.SetArg("CYCAIdentify", "CYCAMax");
            IExtractResult result = msp.Make(null);
            DisplayResultClass.DisplayResult(_session, msp, result, false);
        }
        #endregion

        #region 统计
        /// <summary>
        /// VCI统计自定义
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnVciCustomStatic_Click(object sender, EventArgs e)
        {
            MIFVgtAnalysis.VgtAreaStat(_session, "CVCI", "植被状态指数统计(自定义)", true);
        }

        /// <summary>
        /// VCI统计行政区划
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnVciStatic_Click(object sender, EventArgs e)
        {
            MIFVgtAnalysis.VgtAreaStat(_session, "VCIC", "植被状态指数统计", false);
        }

        //植被指数统计
        void btnNdviStatic_Click(object sender, EventArgs e)
        {
            //MIFVgtAnalysis.VgtAreaStat(_session, null, "植被指数统计", false);
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("VTAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0CBP");
            ms.DoManualExtract(true);
        }

        /// <summary>
        /// 自定义统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnCustomStatic_Click(object sender, EventArgs e)
        {
            MIFVgtAnalysis.VgtAreaStat(_session, "0CCA", "植被指数统计(自定义)", true);
        }

        void _btnAvgStatic_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("AVGS");
            GetCommandAndExecute(6601);
        }

        #endregion

        #region 专题图
        //多通道合成图
        void mnuOMulLayout_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "OMCS");
            ms.DoAutoExtract(true);
        }

        void mnuMulLayout_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:植被多通道合成图,多通道合成图,VGT,MCSI");
        }

        //产品专题图,
        void btnNdviImg_Click(object sender, EventArgs e)
        {
            //if (MIFCommAnalysis.CreateThemeGraphy(_session, "ZTUI", "", "NDVI", "植被指数差值图", false, true) == null)
            //    return;
            if (MIFCommAnalysis.CreateThemeGraphy(_session, false, false, false) == null)
                return;
        }

        //产品专题图(演示分辨率)
        void btnVciOrginImg_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, false, false, true) == null)
                return;
        }

        #endregion

        #region 计算
        /// <summary>
        /// 差值计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnChaZhi_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("CHAZ");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 距平计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAvgCom_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("ANMI");
            GetCommandAndExecute(6602);
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

        /// <summary>
        /// 数据平滑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnData_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(6603);
            if (cmd != null)
                cmd.Execute();
        }

        /// <summary>
        /// 植被状态指数计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnVciCom_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0VCI");
            GetCommandAndExecute(6602);
        }
        #endregion

        #region 植被指数计算

        void btnLAICom_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0LAI");
            GetCommandAndExecute(6602);
        }

        void btnEviCom_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0EVI");
            GetCommandAndExecute(6602);
        }

        void btnDviCom_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0DVI");
            GetCommandAndExecute(6602);
        }

        void btnRviCom_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0RVI");
            GetCommandAndExecute(6602);
        }

        void btnNdviCom_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("NDVI");
            GetCommandAndExecute(6602);
        }

        void mniCloudExtract_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0CLM");
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

        //快速生成
        void btnSave_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, null);
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

        private void GetCommandAndExecute(int cmdID)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute("VGT");
        }

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

        private void SetImage()
        {
            _btnRough.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            _btnSave.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            _btnSmooth.Image = _session.UIFrameworkHelper.GetImage("system:fogCycTime.png"); 
            _dbtnDataCL.Image = _session.UIFrameworkHelper.GetImage("system:wydpz.png");
            _dbtnComparaCom.Image = _session.UIFrameworkHelper.GetImage("system:measure.png");
            _dbtnComparaImg.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
            _dbtnComparaSta.Image = _session.UIFrameworkHelper.GetImage("system:comList.png");
            _btnNdviImage.Image = _session.UIFrameworkHelper.GetImage("system:drought.png");
            _btnProcuctImageGp.Image = _session.UIFrameworkHelper.GetImage("system:surTemp.png");
            _btnNdviStatic.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            _btnCustomStatic.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            _btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
            _btnAvgStatic.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
        }
    }
}
