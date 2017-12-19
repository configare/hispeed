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

namespace GeoDo.RSS.UI.AddIn.CMA
{
    public partial class UITabLST : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 陆表高温按钮定义

        #region 判识相关按钮
        RadButtonElement btnRough = new RadButtonElement("计算");
        RadButtonElement btnSave = new RadButtonElement("快速生成");
        RadButtonElement btnInteractive = new RadButtonElement("交互判识");
        #endregion

        #region 专题图相关按钮
        RadButtonElement btnMulImage = new RadButtonElement("监测示意图");
        RadDropDownButtonElement dbtnbinImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnCycTimeStatImage = new RadDropDownButtonElement();
        #endregion

        #region 面积统计相关按钮
        RadButtonElement btnCurrentRegionArea = new RadButtonElement("当前区域");
        RadDropDownButtonElement dbtnDivision = new RadDropDownButtonElement();
        RadButtonElement btnLandType = new RadButtonElement("土地类型");
        RadButtonElement btnCustomArea = new RadButtonElement("自定义");
        #endregion

        #region 周期统计相关按钮
        RadDropDownButtonElement btnCycCurrentRegion = new RadDropDownButtonElement();
        //RadDropDownButtonElement btnCycCustom = new RadDropDownButtonElement();
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
            rbgCheck.Text = "分析";
            rbgCheck.Items.Add(btnRough);
            btnRough.Click += new EventHandler(btnRough_Click);
            btnSave.Click += new EventHandler(btnSave_Click);
            btnSave.ImageAlignment = ContentAlignment.TopCenter;
            btnSave.TextAlignment = ContentAlignment.BottomCenter;
            rbgCheck.Items.Add(btnInteractive);
            btnInteractive.Click += new EventHandler(btnInteractive_Click);
            //rbgCheck.Items.Add(btnSave);
            foreach (RadButtonElement item in rbgCheck.Items)
            {
                item.ImageAlignment = ContentAlignment.TopCenter;
                item.TextAlignment = ContentAlignment.BottomCenter;
            }

            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "专题产品";
            btnMulImage.ImageAlignment = ContentAlignment.TopCenter;
            btnMulImage.TextAlignment = ContentAlignment.BottomCenter;
            rbgProduct.Items.Add(btnMulImage);
            btnMulImage.Click += new EventHandler(btnMulImage_Click);
            dbtnbinImage.Text = "分析图";
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

            dbtnCycTimeStatImage.Text = "周期统计专题图";
            dbtnCycTimeStatImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnCycTimeStatImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCycTimeStatImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem DayRasterImage = new RadMenuItem("日最大合成图");
            dbtnCycTimeStatImage.Items.Add(DayRasterImage);
            DayRasterImage.Click += new EventHandler(DayRasterImage_Click);
            RadMenuItem Days7RasterImage = new RadMenuItem("周最大合成图");
            dbtnCycTimeStatImage.Items.Add(Days7RasterImage);
            Days7RasterImage.Click += new EventHandler(Days7RasterImage_Click);
            RadMenuItem Days10RasterImage = new RadMenuItem("旬合成图");
            dbtnCycTimeStatImage.Items.Add(Days10RasterImage);
            Days10RasterImage.Click += new EventHandler(Days10RasterImage_Click);
            RadMenuItem Days30RasterImage = new RadMenuItem("月合成图");
            dbtnCycTimeStatImage.Items.Add(Days30RasterImage);
            Days30RasterImage.Click += new EventHandler(Days30RasterImage_Click);
            RadMenuItem Days90RasterImage = new RadMenuItem("季合成图");
            dbtnCycTimeStatImage.Items.Add(Days90RasterImage);
            Days90RasterImage.Click += new EventHandler(Days90RasterImage_Click);
            RadMenuItem Days365RasterImage = new RadMenuItem("年合成图");
            dbtnCycTimeStatImage.Items.Add(Days365RasterImage);
            Days365RasterImage.Click += new EventHandler(Days365RasterImage_Click);
            rbgProduct.Items.Add(dbtnCycTimeStatImage);

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

            RadRibbonBarGroup rbgCycTimeStat = new RadRibbonBarGroup();
            rbgCycTimeStat.Text = "周期统计";
            btnCycCurrentRegion.Text = "数据生产";
            btnCycCurrentRegion.ImageAlignment = ContentAlignment.TopCenter;
            btnCycCurrentRegion.TextAlignment = ContentAlignment.BottomCenter;
            btnCycCurrentRegion.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem DayRaster = new RadMenuItem("日最大合成");
            btnCycCurrentRegion.Items.Add(DayRaster);
            DayRaster.Click += new EventHandler(DayRaster_Click);
            RadMenuItem Days7Raster = new RadMenuItem("周最大合成");
            btnCycCurrentRegion.Items.Add(Days7Raster);
            Days7Raster.Click += new EventHandler(Days7Raster_Click);
            RadMenuItem Days10Raster = new RadMenuItem("旬合成");
            btnCycCurrentRegion.Items.Add(Days10Raster);
            Days10Raster.Click += new EventHandler(Days10Raster_Click);
            RadMenuItem Days30Raster = new RadMenuItem("月合成");
            btnCycCurrentRegion.Items.Add(Days30Raster);
            Days30Raster.Click += new EventHandler(Days30Raster_Click);
            RadMenuItem Days90Raster = new RadMenuItem("季合成");
            btnCycCurrentRegion.Items.Add(Days90Raster);
            Days90Raster.Click += new EventHandler(Days90Raster_Click);
            RadMenuItem Days365Raster = new RadMenuItem("年合成");
            btnCycCurrentRegion.Items.Add(Days365Raster);
            Days365Raster.Click += new EventHandler(Days365Raster_Click);
            rbgCycTimeStat.Items.Add(btnCycCurrentRegion);

            //btnCycCustom.Text = "自定义";
            //btnCycCustom.ImageAlignment = ContentAlignment.TopCenter;
            //btnCycCustom.TextAlignment = ContentAlignment.BottomCenter;
            //btnCycCustom.ArrowPosition = DropDownButtonArrowPosition.Right;
            //RadMenuItem Days10RasterCustom = new RadMenuItem("旬合成");
            //btnCycCustom.Items.Add(Days10RasterCustom);
            ////Days10RasterCustom.Click += new EventHandler(Days10RasterCustom_Click);
            //RadMenuItem Days30RasterCustom = new RadMenuItem("月合成图");
            //btnCycCustom.Items.Add(Days30RasterCustom);
            //// Days30RasterCustom.Click += new EventHandler(Days30RasterCustom_Click);
            //RadMenuItem Days90RasterCustom = new RadMenuItem("季合成图");
            //btnCycCustom.Items.Add(Days90RasterCustom);
            //// Days90RasterCustom.Click += new EventHandler(Days90RasterCustom_Click);
            //RadMenuItem Days365RasterCustom = new RadMenuItem("年合成图");
            //btnCycCustom.Items.Add(Days365RasterCustom);
            ////Days365RasterCustom.Click += new EventHandler(Days365RasterCustom_Click);
            //rbgCycTimeStat.Items.Add(btnCycCustom);

            RadRibbonBarGroup rbgClose = new RadRibbonBarGroup();
            rbgClose.Text = "关闭";
            btnClose.ImageAlignment = ContentAlignment.TopCenter;
            btnClose.TextAlignment = ContentAlignment.BottomCenter;
            btnClose.Click += new EventHandler(btnClose_Click);
            rbgClose.Items.Add(btnClose);

            _tab.Items.Add(rbgCheck);
            _tab.Items.Add(rbgCycTimeStat);
            _tab.Items.Add(rbgProduct);
            _tab.Items.Add(rbgAreaStatic);

            
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
            btnSave.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnInteractive.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            btnMulImage.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnbinImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            btnCurrentRegionArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            btnLandType.Image = _session.UIFrameworkHelper.GetImage("system:landType.png");
            btnCustomArea.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");

            btnCycCurrentRegion.Image = _session.UIFrameworkHelper.GetImage("system:fogCycTime.png");
            //btnCycCustom.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            dbtnCycTimeStatImage.Image = _session.UIFrameworkHelper.GetImage("system:fogCycTime.png");

            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
        }
        #endregion

        #region 按钮相关事件

        #region 计算相关按钮事件
        /// <summary>
        /// 快速生成按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSave_Click(object sender, EventArgs e)
        {
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

        /// <summary>
        /// 计算按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnRough_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExecute(6601);
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
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "CSDI", "", "DBLV", "陆表高温专题图模板", true, true) == null)
                return;
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

        /// <summary>
        /// 监测示意图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMulImage_Click(object sender, EventArgs e)
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

        #region 周期统计相关按钮事件
        /// <summary>
        /// 日合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DayRaster_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("CYCA");
            GetCommandAndExecute(6602);
            IMonitoringSubProduct msp = (_session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
            IExtractResult result = msp.Make(null);
            DisplayResultClass.DisplayResult(_session, msp, result, false);
        }

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
            cmd.Execute("LST");
        }

        #endregion
    }
}
