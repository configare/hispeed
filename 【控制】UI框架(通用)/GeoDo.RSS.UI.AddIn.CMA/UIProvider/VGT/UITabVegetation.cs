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
    public partial class UITabVegetation : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 植被按钮定义
        RadDropDownButtonElement _btnRough = new RadDropDownButtonElement();
        RadButtonElement _btnSave = new RadButtonElement("批量生产");

        RadButtonElement _btnVciCom = null;//= new RadButtonElement("植被状态指数计算");
        //RadButtonElement btnPixel = new RadButtonElement("像元百分比计算");
        RadDropDownButtonElement _dbtnDataCL = new RadDropDownButtonElement();//数据合成

        RadDropDownButtonElement _dbtnComparaCom = new RadDropDownButtonElement();
        RadDropDownButtonElement _dbtnComparaImg = new RadDropDownButtonElement();
        RadDropDownButtonElement _dbtnComparaSta = new RadDropDownButtonElement();

        //RadDropDownButtonElement _dbtnNdviImage = new RadDropDownButtonElement();
        RadButtonElement _btnNdviImage = null; //多通道合成图
        RadButtonElement _btnVciImg = new RadButtonElement("产品专题图");
        RadButtonElement _btnSmooth = null;//数据平滑

        //RadDropDownButtonElement _dbtnVciStatic = new RadDropDownButtonElement();
        //RadButtonElement btnCndviStatic = new RadButtonElement("CNDVI统计分析");
        RadButtonElement _btnNdviStatic = null;//行政区划
        RadButtonElement _btnEnginStatic = null;//气象分区
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

            CreatWholeGenerate(); //批量生产     
            CreatComputeGroup(); //指数计算
            CreatDataProcess(); //数据处理
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

        //批处理
        private void CreatWholeGenerate()
        {
            RadRibbonBarGroup allGenerate = new RadRibbonBarGroup();
            allGenerate.Text = "批量生产";
            _btnSave.ImageAlignment = ContentAlignment.TopCenter;
            _btnSave.TextAlignment = ContentAlignment.BottomCenter;
            _btnSave.Click += new EventHandler(btnSave_Click);
            allGenerate.Items.Add(_btnSave);

            _tab.Items.Add(allGenerate);
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
            _btnNdviImage = new RadButtonElement("多通道合成图");
            _btnNdviImage.ImageAlignment = ContentAlignment.TopCenter;
            _btnNdviImage.TextAlignment = ContentAlignment.BottomCenter;
            _btnNdviImage.Click += new EventHandler(_btnNdviImage_Click);

            _btnVciImg.ImageAlignment = ContentAlignment.TopCenter;
            _btnVciImg.TextAlignment = ContentAlignment.BottomCenter;
            _btnVciImg.Click += new EventHandler(btnNdviImg_Click);

            rbgProduct.Items.Add(_btnNdviImage);
            rbgProduct.Items.Add(_btnVciImg);
            _tab.Items.Add(rbgProduct);
        }


        //数据处理
        private void CreatDataProcess()
        {
            RadRibbonBarGroup productCompute = new RadRibbonBarGroup();
            productCompute.Text = "数据处理";

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
            productCompute.Items.Add(_dbtnDataCL);

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
            productCompute.Items.Add(_dbtnComparaCom);

            _btnSmooth = new RadButtonElement("数据平滑");
            _btnSmooth.ImageAlignment = ContentAlignment.TopCenter;
            _btnSmooth.TextAlignment = ContentAlignment.BottomCenter;
            _btnSmooth.Click += new EventHandler(btnData_Click);
            productCompute.Items.Add(_btnSmooth);

            _tab.Items.Add(productCompute);
        }

        //指数计算
        private void CreatComputeGroup()
        {
            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            rbgCheck.Text = "指数计算";
            _btnRough.Text = "植被指数计算";
            _btnRough.ArrowPosition = DropDownButtonArrowPosition.Right;
            _btnRough.ImageAlignment = ContentAlignment.TopCenter;
            _btnRough.TextAlignment = ContentAlignment.BottomCenter;
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

            _btnVciCom = new RadButtonElement("植被状态指数计算");
            _btnVciCom.ImageAlignment = ContentAlignment.TopCenter;
            _btnVciCom.TextAlignment = ContentAlignment.BottomCenter;
            _btnVciCom.Click += new EventHandler(btnVciCom_Click);

            rbgCheck.Items.Add(_btnRough);
            rbgCheck.Items.Add(_btnVciCom);

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
            MIFVgtAnalysis.VgtAreaStat(_session, null, "植被指数统计", false);
        }

        /// <summary>
        /// 差值统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCZStatic_Click(object sender, EventArgs e)
        {
            MIFVgtAnalysis.VgtAreaStat(_session, "DVIC", "差值统计", false);
        }

        /// <summary>
        /// 自定义统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _btnCustomStatic_Click(object sender, EventArgs e)
        {
            MIFVgtAnalysis.VgtAreaStat(_session, "CVCI", "植被指数统计(自定义)", true);
        }
        #endregion

        #region 专题图
        //多通道合成图
        void _btnNdviImage_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:植被多通道合成图,多通道合成图,VGT,MCSI");
        }

        #region don't need anymore
        void btnVciImg_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "VCII", "", "0VCI", "植被状态指数图", false, true) == null)
                return;
        }

        void btnEviImg_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "EVII", "", "0EVI", "植被指数EVI", false, true) == null)
                return;
        }

        void btnDviImg_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "DVII", "", "0DVI", "植被指数DVI", false, true) == null)
                return;
        }

        void btnRviImg_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "RVII", "", "0RVI", "植被指数RVI", false, true) == null)
                return;
        }
        #endregion

        //产品专题图
        void btnNdviImg_Click(object sender, EventArgs e)
        {
            //if (MIFCommAnalysis.CreateThemeGraphy(_session, "ZTUI", "", "NDVI", "植被指数差值图", false, true) == null)
            //    return;
            if (MIFCommAnalysis.CreateThemeGraphy(_session, false, false) == null)
                return;
        }

        ///// <summary>
        ///// 差值图
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void btnCZImg_Click(object sender, EventArgs e)
        //{
        //    if (MIFCommAnalysis.CreateThemeGraphy(_session, "ZTUI", "", "NDVI", "植被指数差值图", false, true) == null)
        //        return;
        //}

        ///// <summary>
        ///// 距平图
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void btnJPImg_Click(object sender, EventArgs e)
        //{
        //    if (MIFCommAnalysis.CreateThemeGraphy(_session, "PTUI", "", "NDVI", "植被指数距平图", false, true) == null)
        //        return;
        //}
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

            _btnVciCom.Image = _session.UIFrameworkHelper.GetImage("system:measure.png");
            _btnSmooth.Image = _session.UIFrameworkHelper.GetImage("system:fogCycTime.png");
            //btnPixel.Image = _session.UIFrameworkHelper.GetImage("system:percent32.png");
            _dbtnDataCL.Image = _session.UIFrameworkHelper.GetImage("system:wydpz.png");

            _dbtnComparaCom.Image = _session.UIFrameworkHelper.GetImage("system:measure.png");
            _dbtnComparaImg.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
            _dbtnComparaSta.Image = _session.UIFrameworkHelper.GetImage("system:comList.png");

            _btnNdviImage.Image = _session.UIFrameworkHelper.GetImage("system:drought.png");
            _btnVciImg.Image = _session.UIFrameworkHelper.GetImage("system:surTemp.png");

            //btnCndviStatic.Image = _session.UIFrameworkHelper.GetImage("system:wydVgtCndvi.png");
            _btnNdviStatic.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            //_dbtnVciStatic.Image = _session.UIFrameworkHelper.GetImage("system:wydVgtVci.png");
            _btnCustomStatic.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            //_btnEnginStatic.Image = _session.UIFrameworkHelper.GetImage("system:wydVgtNdvi.png");

            //_dbtnDataProduct.Image = _session.UIFrameworkHelper.GetImage("system:fogCycTime.png");

            _btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");

            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
        }
    }
}
