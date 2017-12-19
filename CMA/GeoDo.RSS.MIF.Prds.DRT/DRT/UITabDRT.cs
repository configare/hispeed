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
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public partial class UITabDRT : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 干旱按钮定义

        #region 特征信息提取相关按钮
        RadDropDownButtonElement dbtnComputeData = new RadDropDownButtonElement();
        RadButtonElement btnDT = new RadButtonElement("亮温差计算");
        #endregion

        #region 专题图相关按钮
        RadDropDownButtonElement btnMulImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnTVDIImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnVTIImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnSWIImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnPDIImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnMPDIImage = new RadDropDownButtonElement();

        RadDropDownButtonElement dbtnCycTimeStatImage = new RadDropDownButtonElement();
        #endregion

        #region 面积统计相关按钮
        RadButtonElement btnCurrentRegionArea = new RadButtonElement("当前区域");
        RadButtonElement btnDivision = new RadButtonElement("");
        RadButtonElement btnLandType = new RadButtonElement();
        RadButtonElement btnCustomArea = new RadButtonElement("自定义");
        #endregion

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
            rbgQuantify.Text = "数据生产";
            dbtnComputeData.Text = "指数计算";
            dbtnComputeData.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnComputeData.ImageAlignment = ContentAlignment.TopCenter;
            dbtnComputeData.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniTVDI = new RadMenuItem("温度植被干旱指数");
            mniTVDI.Click += new EventHandler(btnTVDI_Click);
            dbtnComputeData.Items.Add(mniTVDI);
            RadMenuItem mhiVTI = new RadMenuItem("时序植被温度干旱指数");
            dbtnComputeData.Items.Add(mhiVTI);
            mhiVTI.Click += new EventHandler(btnVTI_Click);
            RadMenuItem mhiVCI = new RadMenuItem("植被状态指数");
            //dbtnVTI.Items.Add(mhiVCI);
            mhiVCI.Click += new EventHandler(btnVCI_Click);
            RadMenuItem mhiTCI = new RadMenuItem("温度状态指数");
            // dbtnVTI.Items.Add(mhiTCI);
            mhiTCI.Click += new EventHandler(btnTCI_Click);

            RadMenuItem mhiSWI = new RadMenuItem("热惯量干旱指数");
            dbtnComputeData.Items.Add(mhiSWI);
            mhiSWI.Click += new EventHandler(btnSWI_Click);
            RadMenuItem mhiPDI = new RadMenuItem("垂直干旱指数");
            dbtnComputeData.Items.Add(mhiPDI);
            mhiPDI.Click += new EventHandler(btnPDI_Click);
            RadMenuItem mhiMPDI = new RadMenuItem("改进型垂直干旱指数");
            dbtnComputeData.Items.Add(mhiMPDI);
            mhiMPDI.Click += new EventHandler(btnMPDI_Click);
            RadMenuItem mhiVSWI = new RadMenuItem("植被供水指数");
            dbtnComputeData.Items.Add(mhiVSWI);
            mhiVSWI.Click += new EventHandler(btnVSWI_Click);
            RadMenuItem mhi0CLM = new RadMenuItem("云检测");
            dbtnComputeData.Items.Add(mhi0CLM);
            mhi0CLM.Click += new EventHandler(mhi0CLM_Click);
            rbgQuantify.Items.Add(dbtnComputeData);
            btnDT.ImageAlignment = ContentAlignment.TopCenter;
            btnDT.TextAlignment = ContentAlignment.BottomCenter;
            rbgQuantify.Items.Add(btnDT);
            btnDT.Click += new EventHandler(btnDT_Click);

            #endregion

            #region 专题产品

            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "专题产品";
            btnMulImage.Text = "监测示意图";
            btnMulImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            btnMulImage.ImageAlignment = ContentAlignment.TopCenter;
            btnMulImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mnuMulImage = new RadMenuItem("监测示意图");
            mnuMulImage.Click += new EventHandler(mnuMulImage_Click);
            RadMenuItem mnuOMulImage = new RadMenuItem("监测示意图（原始分辩率）");
            mnuOMulImage.Click += new EventHandler(mnuOMulImage_Click);
            btnMulImage.Items.AddRange(mnuMulImage, mnuOMulImage);
            rbgProduct.Items.Add(btnMulImage);

            GreateProductButton(dbtnTVDIImage, "温度\n植被干旱指数图", "TVDI");
            rbgProduct.Items.Add(dbtnTVDIImage);
            GreateProductButton(dbtnVTIImage, "时序植被\n温度干旱指数图", "VTI");
            rbgProduct.Items.Add(dbtnVTIImage);
            GreateProductButton(dbtnSWIImage, "热惯量\n干旱指数图", "SWI");
            rbgProduct.Items.Add(dbtnSWIImage);
            GreateProductButton(dbtnPDIImage, "垂直\n干旱指数图", "PDI");
            rbgProduct.Items.Add(dbtnPDIImage);
            GreateProductButton(dbtnMPDIImage, "改进型\n垂直干旱指数图", "MPDI");
            rbgProduct.Items.Add(dbtnMPDIImage);
            #endregion

            #region 面积统计
            //从配置文件读取统计分析菜单名称
            string provinceName = AreaStatProvider.GetAreaStatItemMenuName("行政区划");
            string landTypeName = AreaStatProvider.GetAreaStatItemMenuName("土地利用类型");
            RadRibbonBarGroup rbgAreaStatic = new RadRibbonBarGroup();
            rbgAreaStatic.Text = "面积统计";
            btnDivision.Text = provinceName;
            btnDivision.TextAlignment = ContentAlignment.BottomCenter;
            btnDivision.ImageAlignment = ContentAlignment.TopCenter;
            btnDivision.Click += new EventHandler(btnDivision_Click);
            btnCurrentRegionArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCurrentRegionArea.TextAlignment = ContentAlignment.BottomCenter;
            rbgAreaStatic.Items.Add(btnCurrentRegionArea);
            btnCurrentRegionArea.Click += new EventHandler(btnCurrentRegionArea_Click);
            rbgAreaStatic.Items.Add(btnDivision);
            btnLandType.Text = landTypeName;
            btnLandType.ImageAlignment = ContentAlignment.TopCenter;
            btnLandType.TextAlignment = ContentAlignment.BottomCenter;
            rbgAreaStatic.Items.Add(btnLandType);
            btnLandType.Click += new EventHandler(btnLandType_Click);
            btnCustomArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCustomArea.TextAlignment = ContentAlignment.BottomCenter;
            rbgAreaStatic.Items.Add(btnCustomArea);
            btnCustomArea.Click += new EventHandler(btnCustomArea_Click);

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

            RadMenuItem mhiBinImgCurrentOrgin = new RadMenuItem("当前区域（原始分辨率）");
            mhiBinImgCurrentOrgin.Tag = btnTag;
            mhiBinImgCurrentOrgin.Click += new EventHandler(mhiBinImgCurrentOrgin_Click);
            dbtn.Items.Add(mhiBinImgCurrentOrgin);

            RadMenuItem mhiBinImgCustom = new RadMenuItem("自定义");
            dbtn.Items.Add(mhiBinImgCustom);
            mhiBinImgCustom.Click += new EventHandler(mhiBinImgCustom_Click);
            mhiBinImgCustom.Tag = btnTag;

            RadMenuItem mhiBinImgCustomOrgin = new RadMenuItem("自定义（原始分辨率）");
            dbtn.Items.Add(mhiBinImgCustomOrgin);
            mhiBinImgCustomOrgin.Click += new EventHandler(mhiBinImgCustomOrgin_Click);
            mhiBinImgCustomOrgin.Tag = btnTag;
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
            btnDivision.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            btnMulImage.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            btnCurrentRegionArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            btnLandType.Image = _session.UIFrameworkHelper.GetImage("system:landType.png");
            btnCustomArea.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            dbtnCycTimeStatImage.Image = _session.UIFrameworkHelper.GetImage("system:fogCycTime.png");
            btnDT.Image = _session.UIFrameworkHelper.GetImage("system:SWI32.png");
            dbtnComputeData.Image = _session.UIFrameworkHelper.GetImage("system:VTI32.png");
            dbtnSWIImage.Image = _session.UIFrameworkHelper.GetImage("system:SWIImage.png");
            dbtnVTIImage.Image = _session.UIFrameworkHelper.GetImage("system:VTIImage.png");
            dbtnTVDIImage.Image = _session.UIFrameworkHelper.GetImage("system:TVDIImage.png");
            dbtnPDIImage.Image = _session.UIFrameworkHelper.GetImage("system:PDIImage.png");
            dbtnMPDIImage.Image = _session.UIFrameworkHelper.GetImage("system:PDIImage.png");
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
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0DNT");
            GetCommandAndExecute(6602);
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

        /// <summary>
        /// PDI按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnPDI_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0PDI");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// MPDI按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMPDI_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("MPDI");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// SWI按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnVSWI_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("VSWI");
            GetCommandAndExecute(6602);
        }

        void mhi0CLM_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0CLM");
            GetCommandAndExecute(6602);
        }

        #endregion

        #region 专题图相关按钮事件

        /// <summary>
        /// 监测示意图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void mnuOMulImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "OMCS");
            ms.DoAutoExtract(true);
        }

        void mnuMulImage_Click(object sender, EventArgs e)
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
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "TVII", "DRTTVDI", "TVDI ", "温度植被干旱监测专题图模板", false, true) == null)
                        return;
                    break;
                case "VCI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "0VCI", "植被状态指数监测专题图模板", false, true) == null)
                        return;
                    break;
                case "TCI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "0TCI", "温度状态指数监测专题图模板", false, true) == null)
                        return;
                    break;
                case "VTI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "VTII", "", "0VTI", "时序植被温度干旱指数监测专题图模板", false, true) == null)
                        return;
                    break;
                case "SWI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "SWII", "DRT0SWI", "SWI", "热惯量监测专题图模板", false, true) == null)
                        return;
                    break;
                case "PDI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "PDII", "", "PDI", "垂直干旱指数监测专题图模板", false, true) == null)
                        return;
                    break;
                case "MPDI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "MPII", "", "MPDI", "垂直干旱指数监测专题图模板", false, true) == null)
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
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "CTVI", "", "TVDI ", "温度植被干旱监测专题图模板", true, true) == null)
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
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "CVTI", "", "VTI", "时序植被温度干旱指数监测专题图模板", true, true) == null)
                        return;
                    break;
                case "SWI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "CSWI", "", "SWI", "热惯量监测专题图模板", true, true) == null)
                        return;
                    break;
                case "PDI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "CPDI", "", "PDI", "垂直干旱指数监测专题图模板", true, true) == null)
                        return;
                    break;
                case "MPDI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "CMPI", "", "MPDI", "垂直干旱指数监测专题图模板", true, true) == null)
                        return;
                    break;
            }
        }

        /// <summary>
        /// 当前区域(原始分辨率)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiBinImgCurrentOrgin_Click(object sender, EventArgs e)
        {
            RadMenuItem rmi = sender as RadMenuItem;
            if (rmi == null)
                return;
            string type = rmi.Tag.ToString().ToUpper();
            switch (type)
            {
                case "TVDI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "OTVI", "DRTTVDI", "TVDI", "温度植被干旱监测专题图模板", false, true) == null)
                        return;
                    break;
                case "VCI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "O0VC", "", "0VCI", "植被状态指数监测专题图模板", false, true) == null)
                        return;
                    break;
                case "TCI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "O0TC", "", "0TCI", "温度状态指数监测专题图模板", false, true) == null)
                        return;
                    break;
                case "VTI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "OVTI", "", "0VTI", "时序植被温度干旱指数监测专题图模板", false, true) == null)
                        return;
                    break;
                case "SWI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "OSWI", "DRT0SWI", "SWI", "热惯量监测专题图模板", false, true) == null)
                        return;
                    break;
                case "PDI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "OPDI", "DRT0PDI", "PDI", "垂直干旱指数监测专题图模板", false, true) == null)
                        return;
                    break;
                case "MPDI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "OMPI", "", "MPDI", "垂直干旱指数监测专题图模板", false, true) == null)
                        return;
                    break;
            }
        }

        /// <summary>
        /// 自定义（原始分辨率）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiBinImgCustomOrgin_Click(object sender, EventArgs e)
        {
            RadMenuItem rmi = sender as RadMenuItem;
            if (rmi == null)
                return;
            string type = rmi.Tag.ToString().ToUpper();
            switch (type)
            {
                case "TVDI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "COTV", "DRTTVDI", "TVDI", "温度植被干旱监测专题图模板", true, true) == null)
                        return;
                    break;
                case "VCI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "COVC", "", "0VCI", "植被状态指数监测专题图模板", true, true) == null)
                        return;
                    break;
                case "TCI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "COTC", "", "0TCI", "温度状态指数监测专题图模板", true, true) == null)
                        return;
                    break;
                case "VTI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "COVT", "", "0VTI", "时序植被温度干旱指数监测专题图模板", true, true) == null)
                        return;
                    break;
                case "SWI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "COSW", "DRT0SWI", "SWI", "热惯量监测专题图模板", true, true) == null)
                        return;
                    break;
                case "PDI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "COPD", "", "PDI", "垂直干旱指数监测专题图模板", true, true) == null)
                        return;
                    break;
                case "MPDI":
                    if (MIFCommAnalysis.CreateThemeGraphy(_session, "COMI", "", "MPDI", "垂直干旱指数监测专题图模板", true, true) == null)
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
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "CCAR");
            ms.DoManualExtract(true);
        }

        /// <summary>
        /// 自定义面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCustomArea_Click(object sender, EventArgs e)
        {
            MIFDrtAnalysis.DrtAreaStat(_session, "CCCA", "干旱指数统计(自定义)", true);
        }

        /// <summary>
        /// 土地利用类型面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLandType_Click(object sender, EventArgs e)
        {
            //if (MIFCommAnalysis.AreaStat(_session, "CLUT", "土地利用类型", false, true) == null)
            //    return;
            //MIFVgtAnalysis.VgtAreaStat(_session, null, "干旱指数统计", false);
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "CLUT");
            ms.DoManualExtract(true);
        }

        /// <summary>
        /// 按省界面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiProvincBound_Click(object sender, EventArgs e)
        {
            //if (MIFCommAnalysis.AreaStat(_session, "0CBP", "省级行政区划", false, true) == null)
            //    return;
            //MIFVgtAnalysis.VgtAreaStat(_session, null, "干旱指数统计", false);
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0CBP");
            ms.DoManualExtract(true);
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


        void btnDivision_Click(object sender, EventArgs e)
        {
            //MIFDrtAnalysis.DrtAreaStat(_session, null, "植被指数统计", false);
            ////if (MIFCommAnalysis.AreaStat(_session, "0CBP", "省级行政区划", false, true) == null)
            ////    return;
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0CBP");
            ms.DoManualExtract(true);
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
