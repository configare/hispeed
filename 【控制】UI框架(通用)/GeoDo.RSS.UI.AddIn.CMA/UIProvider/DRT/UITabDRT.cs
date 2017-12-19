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
using GeoDo.RSS.Core.DF;
using System.IO;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    public partial class UITabDRT : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 干旱按钮定义

        #region 特征信息提取相关按钮
        RadButtonElement btnTVDI = new RadButtonElement("温度\n植被干旱指数");
        RadDropDownButtonElement dbtnVTI = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnSWI = new RadDropDownButtonElement();
        RadButtonElement btnSave = new RadButtonElement("快速生成");
        #endregion

        #region 专题图相关按钮
        RadButtonElement btnMulImage = new RadButtonElement(" 监测\n示意图");
        RadDropDownButtonElement dbtnTVDIImage = new RadDropDownButtonElement();
        //RadDropDownButtonElement dbtnVCIImage = new RadDropDownButtonElement();
        //RadDropDownButtonElement dbtnTCIImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnVTIImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnSWIImage = new RadDropDownButtonElement();

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
        RadDropDownButtonElement btnCycCustom = new RadDropDownButtonElement();
        #endregion

        //#region 动画相关按钮
        //RadButtonElement btnAnimationRegion = new RadButtonElement("当前区域");
        //RadButtonElement btnAnimationCustom = new RadButtonElement("自定义");
        //#endregion

        RadButtonElement btnClose = new RadButtonElement("关闭干旱监测\n分析视图");

        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        #endregion

        public UITabDRT()
        {
            InitializeComponent();
            CreateRibbonBar();
        }

        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "干旱监测专题";
            _tab.Text = "干旱监测专题";
            _tab.Name = "干旱监测专题";
            _bar.CommandTabs.Add(_tab);

            #region 特征信息提取

            RadRibbonBarGroup rbgQuantify = new RadRibbonBarGroup();
            rbgQuantify.Text = "特征信息提取";
            btnTVDI.ImageAlignment = ContentAlignment.TopCenter;
            btnTVDI.TextAlignment = ContentAlignment.BottomCenter;
            //rbgQuantify.Items.Add(btnTVDI);
            btnTVDI.Click += new EventHandler(btnTVDI_Click);
            //rbgQuantify.Items.Add(btnTVDI);

            dbtnVTI.Text = "时序植被\n温度干旱指数";
            dbtnVTI.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnVTI.ImageAlignment = ContentAlignment.TopCenter;
            dbtnVTI.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiVTI = new RadMenuItem("时序植被温度干旱指数");
            dbtnVTI.Items.Add(mhiVTI);
            mhiVTI.Click += new EventHandler(btnVTI_Click);
            RadMenuItem mhiVCI = new RadMenuItem("植被状态指数");
            //dbtnVTI.Items.Add(mhiVCI);
            mhiVCI.Click += new EventHandler(btnVCI_Click);
            RadMenuItem mhiTCI = new RadMenuItem("温度状态指数");
            // dbtnVTI.Items.Add(mhiTCI);
            mhiTCI.Click += new EventHandler(btnTCI_Click);
            rbgQuantify.Items.Add(dbtnVTI);

            dbtnSWI.Text = " 热惯量\n干旱指数";
            dbtnSWI.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnSWI.ImageAlignment = ContentAlignment.TopCenter;
            dbtnSWI.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiSWI = new RadMenuItem("热惯量计算");
            dbtnSWI.Items.Add(mhiSWI);
            mhiSWI.Click += new EventHandler(btnSWI_Click);
            RadMenuItem mhiDT = new RadMenuItem("亮温差计算");
            dbtnSWI.Items.Add(mhiDT);
            mhiDT.Click += new EventHandler(btnDT_Click);
            rbgQuantify.Items.Add(dbtnSWI);

            btnSave.Click += new EventHandler(btnSave_Click);
            btnSave.ImageAlignment = ContentAlignment.TopCenter;
            btnSave.TextAlignment = ContentAlignment.BottomCenter;
            //rbgQuantify.Items.Add(btnSave);

            #endregion

            #region 专题产品

            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "专题产品";
            btnMulImage.ImageAlignment = ContentAlignment.TopCenter;
            btnMulImage.TextAlignment = ContentAlignment.BottomCenter;
            rbgProduct.Items.Add(btnMulImage);
            btnMulImage.Click += new EventHandler(btnMulImage_Click);

            GreateProductButton(dbtnTVDIImage, "温度\n植被干旱指数图", "TVDI");
            rbgProduct.Items.Add(dbtnTVDIImage);
            //GreateProductButton(dbtnVCIImage, "植被状态指数图", "VCI");
            //rbgProduct.Items.Add(dbtnVCIImage);
            //GreateProductButton(dbtnTCIImage, "温度状态指数图", "TCI");
            //rbgProduct.Items.Add(dbtnTCIImage);
            GreateProductButton(dbtnVTIImage, "时序植被\n温度干旱指数图", "VTI");
            rbgProduct.Items.Add(dbtnVTIImage);
            GreateProductButton(dbtnSWIImage, "热惯量\n干旱指数图", "SWI");
            rbgProduct.Items.Add(dbtnSWIImage);

            dbtnCycTimeStatImage.Text = "周期\n统计专题图";
            dbtnCycTimeStatImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnCycTimeStatImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCycTimeStatImage.TextAlignment = ContentAlignment.BottomCenter;
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

            #endregion

            #region 面积统计

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

            #endregion

            #region 周期统计

            RadRibbonBarGroup rbgCycTimeStat = new RadRibbonBarGroup();
            rbgCycTimeStat.Text = "周期统计";
            btnCycCurrentRegion.Text = "当前区域";
            btnCycCurrentRegion.ImageAlignment = ContentAlignment.TopCenter;
            btnCycCurrentRegion.TextAlignment = ContentAlignment.BottomCenter;
            btnCycCurrentRegion.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem Days10Raster = new RadMenuItem("旬合成");
            btnCycCurrentRegion.Items.Add(Days10Raster);
            //Days10Raster.Click += new EventHandler(Days10Raster_Click);
            RadMenuItem Days30Raster = new RadMenuItem("月合成图");
            btnCycCurrentRegion.Items.Add(Days30Raster);
            //Days30Raster.Click += new EventHandler(Days30Raster_Click);
            RadMenuItem Days90Raster = new RadMenuItem("季合成图");
            btnCycCurrentRegion.Items.Add(Days90Raster);
            //Days90Raster.Click += new EventHandler(Days90Raster_Click);
            RadMenuItem Days365Raster = new RadMenuItem("年合成图");
            btnCycCurrentRegion.Items.Add(Days365Raster);
            //Days365Raster.Click += new EventHandler(Days365Raster_Click);
            rbgCycTimeStat.Items.Add(btnCycCurrentRegion);

            btnCycCustom.Text = "自定义";
            btnCycCustom.ImageAlignment = ContentAlignment.TopCenter;
            btnCycCustom.TextAlignment = ContentAlignment.BottomCenter;
            btnCycCustom.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem Days10RasterCustom = new RadMenuItem("旬合成");
            btnCycCustom.Items.Add(Days10RasterCustom);
            //Days10RasterCustom.Click += new EventHandler(Days10RasterCustom_Click);
            RadMenuItem Days30RasterCustom = new RadMenuItem("月合成图");
            btnCycCustom.Items.Add(Days30RasterCustom);
            // Days30RasterCustom.Click += new EventHandler(Days30RasterCustom_Click);
            RadMenuItem Days90RasterCustom = new RadMenuItem("季合成图");
            btnCycCustom.Items.Add(Days90RasterCustom);
            // Days90RasterCustom.Click += new EventHandler(Days90RasterCustom_Click);
            RadMenuItem Days365RasterCustom = new RadMenuItem("年合成图");
            btnCycCustom.Items.Add(Days365RasterCustom);
            //Days365RasterCustom.Click += new EventHandler(Days365RasterCustom_Click);
            rbgCycTimeStat.Items.Add(btnCycCustom);

            #endregion

            RadRibbonBarGroup rbgClose = new RadRibbonBarGroup();
            rbgClose.Text = "关闭";
            btnClose.ImageAlignment = ContentAlignment.TopCenter;
            btnClose.TextAlignment = ContentAlignment.BottomCenter;
            btnClose.Click += new EventHandler(btnClose_Click);
            rbgClose.Items.Add(btnClose);

            _tab.Items.Add(rbgQuantify);
            _tab.Items.Add(rbgProduct);
            _tab.Items.Add(rbgAreaStatic);
            _tab.Items.Add(rbgCycTimeStat);

            
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

        private void GreateProductButton(RadDropDownButtonElement dbtn, string btnText, string btnTag)
        {
            dbtn.Text = btnText;
            dbtn.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtn.ImageAlignment = ContentAlignment.TopCenter;
            dbtn.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiBinImgCurrent = new RadMenuItem("当前区域");
            mhiBinImgCurrent.Tag = btnTag;
            dbtn.Items.Add(mhiBinImgCurrent);
            mhiBinImgCurrent.Click += new EventHandler(mhiBinImgCurrent_Click);
            RadMenuItem mhiBinImgCustom = new RadMenuItem("自定义");
            dbtn.Items.Add(mhiBinImgCustom);
            mhiBinImgCustom.Click += new EventHandler(mhiBinImgCustom_Click);
            mhiBinImgCustom.Tag = btnTag;
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
            dbtnDivision.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            btnSave.Image = _session.UIFrameworkHelper.GetImage("system:quickgenerate.png");
            btnMulImage.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            btnCurrentRegionArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            btnLandType.Image = _session.UIFrameworkHelper.GetImage("system:landType.png");
            btnCustomArea.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");

            btnCycCurrentRegion.Image = _session.UIFrameworkHelper.GetImage("system:fogCycTime.png");
            btnCycCustom.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            dbtnCycTimeStatImage.Image = _session.UIFrameworkHelper.GetImage("system:fogCycTime.png");

            dbtnSWI.Image = _session.UIFrameworkHelper.GetImage("system:SWI32.png");
            dbtnVTI.Image = _session.UIFrameworkHelper.GetImage("system:VTI32.png");
            btnTVDI.Image = _session.UIFrameworkHelper.GetImage("system:TVDI32.png");

            dbtnSWIImage.Image = _session.UIFrameworkHelper.GetImage("system:SWIImage.png");
            dbtnVTIImage.Image = _session.UIFrameworkHelper.GetImage("system:VTIImage.png");
            dbtnTVDIImage.Image = _session.UIFrameworkHelper.GetImage("system:TVDIImage.png");

            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
        }
        #endregion

        #region 按钮相关事件

        #region 特征信息提取相关按钮事件
        /// <summary>
        /// 快速生成按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSave_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// TVDI按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnTVDI_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("TVDI");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// VCI按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnVCI_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// TCI按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnTCI_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// VTI按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnVTI_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0VTI");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// DT按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnDT_Click(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// SWI按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSWI_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0SWI");
            GetCommandAndExecute(6602);
        }

        #endregion

        #region 专题图相关按钮事件

        /// <summary>
        /// 监测示意图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMulImage_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:干旱监测示意图,监测示意图,DRT,MCSI");
        }

        /// <summary>
        /// 专题图当前区域按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiBinImgCurrent_Click(object sender, EventArgs e)
        {
            RadMenuItem rmi = sender as RadMenuItem;
            if (rmi == null)
                return;
            string type = rmi.Tag.ToString().ToUpper();
            switch (type)
            {
                case "TVDI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "TVDI ", "温度植被干旱监测专题图模板", false, true) == null)
                        return;
                    break;
                case "VCI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "VCI", "植被状态指数监测专题图模板", false, true) == null)
                        return;
                    break;
                case "TCI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "TCI", "温度状态指数监测专题图模板", false, true) == null)
                        return;
                    break;
                case "VTI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "VTI", "时序植被温度干旱指数监测专题图模板", false, true) == null)
                        return;
                    break;
                case "SWI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "SWI", "热惯量监测专题图模板", false, true) == null)
                        return;
                    break;
            }
        }

        /// <summary>
        /// 专题图自定义区域按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiBinImgCustom_Click(object sender, EventArgs e)
        {
            RadMenuItem rmi = sender as RadMenuItem;
            if (rmi == null)
                return;
            string type = rmi.Tag.ToString().ToUpper();
            switch (type)
            {
                case "TVDI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "CSDI", "", "TVDI ", "温度植被干旱监测专题图模板", true, true) == null)
                        return;
                    break;
                case "VCI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "CSDI", "", "VCI", "植被状态指数监测专题图模板", true, true) == null)
                        return;
                    break;
                case "TCI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "CSDI", "", "TCI", "温度状态指数监测专题图模板", true, true) == null)
                        return;
                    break;
                case "VTI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "CSDI", "", "VTI", "时序植被温度干旱指数监测专题图模板", true, true) == null)
                        return;
                    break;
                case "SWI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "CSDI", "", "SWI", "热惯量监测专题图模板", true, true) == null)
                        return;
                    break;
            }
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
            cmd.Execute("DRT");
        }
        #endregion
    }
}
