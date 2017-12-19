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
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.Layout;
using System.Drawing.Imaging;

namespace GeoDo.RSS.MIF.Prds.FOG
{
    public partial class UITabFog : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 大雾按钮定义
        RadButtonElement btnNatrueColor = new RadButtonElement("真彩图");
        #region 判识相关按钮
        RadButtonElement btnRough = new RadButtonElement("自动判识");
        RadDropDownButtonElement btnInteractive = new RadDropDownButtonElement();
        RadDropDownButtonElement btnAutoGenerate = new RadDropDownButtonElement();
        #endregion

        #region 特征信息提取相关按钮
        RadButtonElement btnCSRCalc = new RadButtonElement("晴空反射率");
        RadDropDownButtonElement dbtnQuantifyCalc = new RadDropDownButtonElement();
        RadMenuItem OPTDRasterCalc;
        RadMenuItem LWPRasterCalc;
        RadMenuItem ERADRasterCalc;
        RadMenuItem quantifyRasterCalc;
        #endregion

        #region 专题图相关按钮
        private RadDropDownButtonElement _btnMonitorShow = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnbinImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnQuantifyImage = new RadDropDownButtonElement();
        #endregion

        #region 周期合成相关按钮
        RadButtonElement btnCurrentRegionCycTime = new RadButtonElement("当前区域");
        RadButtonElement btnCustomCycTime = new RadButtonElement("自定义");
        #endregion

        #region 频次统计相关按钮
        RadButtonElement btnCurrentRegionFreq = new RadButtonElement("当前区域");
        RadButtonElement btnCustomFreq = new RadButtonElement("自定义");
        #endregion

        #region 动画相关按钮
        RadButtonElement btnAnimationRegion = new RadButtonElement("当前区域");
        RadButtonElement btnAnimationCustom = new RadButtonElement("自定义");
        #endregion

        //#region 小仪器雾霾相关按钮
        //RadButtonElement btnImportTOU = new RadButtonElement("导入");
        //RadButtonElement btnTOUImage = new RadButtonElement("专题图");
        //#endregion

        #region 批量保存按钮
        RadButtonElement btnPicLayout = new RadButtonElement("天气网图像批量输出\n分析视图");//btnPicLayout
        #endregion 

        RadButtonElement btnClose = new RadButtonElement("关闭大雾监测\n分析视图");
        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        RadDropDownButtonElement dbtnOrdLayout = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnOrdStatArea = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnOrdGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnDisLayout = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnDisStat = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnDisGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnCycLayout = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnCycGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnTOU = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnAnimation = new RadDropDownButtonElement();
        #endregion

        public UITabFog()
        {
            InitializeComponent();
            CreateRibbonBar();
            AddPicLayoutButton();
        }

        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "大雾监测专题";
            _tab.Text = "大雾监测专题";
            _tab.Name = "大雾监测专题";
            _bar.CommandTabs.Add(_tab);

         

            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            rbgCheck.Text = "监测";
            rbgCheck.Items.Add(btnRough);
            btnRough.ImageAlignment = ContentAlignment.TopCenter;
            btnRough.TextAlignment = ContentAlignment.BottomCenter;
            btnRough.Click += new EventHandler(btnRough_Click);
            rbgCheck.Items.Add(btnInteractive);
            btnInteractive.Text = "交互判识";
            btnInteractive.ImageAlignment = ContentAlignment.TopCenter;
            btnInteractive.TextAlignment = ContentAlignment.BottomCenter;
            btnInteractive.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem fogProduct = new RadMenuItem("大雾");
            fogProduct.Click += new EventHandler(fogProduct_Click);
            btnInteractive.Items.Add(fogProduct);
            RadMenuItem cloudProduct = new RadMenuItem("云");
            cloudProduct.Click += new EventHandler(cloudProduct_Click);
            btnInteractive.Items.Add(cloudProduct);
            //
            btnAutoGenerate.Text = "快速生成";
            btnAutoGenerate.ImageAlignment = ContentAlignment.TopCenter;
            btnAutoGenerate.TextAlignment = ContentAlignment.BottomCenter;
            btnAutoGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem roughProduct = new RadMenuItem("自动判识+专题产品");
            roughProduct.Click += new EventHandler(roughProduct_Click);
            btnAutoGenerate.Items.Add(roughProduct);
            RadMenuItem autoProduct = new RadMenuItem("专题产品");
            autoProduct.Click += new EventHandler(autoProduct_Click);
            btnAutoGenerate.Items.Add(autoProduct);
            rbgCheck.Items.Add(btnAutoGenerate);
            //
            //#region 日常业务
            //RadRibbonBarGroup rbgDaily = new RadRibbonBarGroup();
            //rbgDaily.Text = "日常业务";
            //dbtnOrdLayout.Text = "专题产品";
            //dbtnOrdLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            //dbtnOrdLayout.ImageAlignment = ContentAlignment.TopCenter;
            //dbtnOrdLayout.TextAlignment = ContentAlignment.BottomCenter;
            //RadMenuItem btnMulImage = new RadMenuItem("监测图");
            //btnMulImage.Click += new EventHandler(btnMulImage_Click);
            //RadMenuItem btnMulImage2 = new RadMenuItem("监测示意图");
            //btnMulImage2.Click += new EventHandler(btnMulImage2_Click);
            //RadMenuItem btnMulImageO = new RadMenuItem("监测图（无边框）");
            //btnMulImageO.Click += new EventHandler(btnMulImageOriginal_Click);
            //RadMenuItem btnMulImageO2 = new RadMenuItem("监测示意图（无边框）");
            //btnMulImageO2.Click += new EventHandler(btnMulImageOriginal2_Click);
            //RadMenuItem mniBinImage = new RadMenuItem("二值图（当前区域）");
            //mniBinImage.Click += new EventHandler(mhiBinImgCurrent_Click);
            //RadMenuItem serverImage = new RadMenuItem("天气网用图");
            //serverImage.Click += new EventHandler(ServerImg_Click);
            //dbtnOrdLayout.Items.AddRange(new RadItem[] { btnMulImage2, mniBinImage,serverImage});
            //rbgDaily.Items.Add(dbtnOrdLayout);

            //从配置文件读取统计分析菜单名称
            string provinceName = AreaStatProvider.GetAreaStatItemMenuName("行政区划");
            string landTypeName = AreaStatProvider.GetAreaStatItemMenuName("土地利用类型");
            string cityName = AreaStatProvider.GetAreaStatItemMenuName("三级行政区划");
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
            dbtnOrdStatArea.Items.AddRange(new RadMenuItem[] { mniOrdCurrent, mniOrdLandType, mniOrdProvince, mniOrdCity});
            //rbgDaily.Items.Add(dbtnOrdStatArea);

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
            //rbgDaily.Items.Add(dbtnOrdGenerate);
            //#endregion

            #region 灾情事件
            RadRibbonBarGroup rbgDisaster = new RadRibbonBarGroup();
            rbgDisaster.Text = "灾情事件";
            btnCSRCalc.ImageAlignment = ContentAlignment.TopCenter;
            btnCSRCalc.TextAlignment = ContentAlignment.BottomCenter;
            rbgDisaster.Items.Add(btnCSRCalc);
            btnCSRCalc.Click += new EventHandler(btnCSRCalc_Click);
            dbtnQuantifyCalc.Text = "特征量计算";
            dbtnQuantifyCalc.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnQuantifyCalc.ImageAlignment = ContentAlignment.TopCenter;
            dbtnQuantifyCalc.TextAlignment = ContentAlignment.BottomCenter;
            dbtnQuantifyCalc.AutoToolTip = true;
            quantifyRasterCalc = new RadMenuItem("所有特征量");
            dbtnQuantifyCalc.Items.Add(quantifyRasterCalc);
            quantifyRasterCalc.Click += new EventHandler(quantifyRasterCalc_Click);
            OPTDRasterCalc = new RadMenuItem("光学厚度");
            dbtnQuantifyCalc.Items.Add(OPTDRasterCalc);
            OPTDRasterCalc.Click += new EventHandler(OPTDRasterCalc_Click);
            LWPRasterCalc = new RadMenuItem("液态水路径");
            dbtnQuantifyCalc.Items.Add(LWPRasterCalc);
            LWPRasterCalc.Click += new EventHandler(LWPRasterCalc_Click);
            ERADRasterCalc = new RadMenuItem("雾滴尺度");
            dbtnQuantifyCalc.Items.Add(ERADRasterCalc);
            ERADRasterCalc.Click += new EventHandler(ERADRasterCalc_Click);
            rbgDisaster.Items.Add(dbtnQuantifyCalc);
            dbtnDisLayout.Text = "专题产品";
            dbtnDisLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnDisLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDisLayout.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem btnDisMulImage = new RadMenuItem("监测图");
            btnDisMulImage.Click += new EventHandler(btnMulImage_Click);
            RadMenuItem btnDisMulImage2 = new RadMenuItem("监测示意图");
            btnDisMulImage2.Click += new EventHandler(btnMulImage2_Click);
            RadMenuItem btnDisMulImageO = new RadMenuItem("监测图（无边框）");
            btnDisMulImageO.Click += new EventHandler(btnMulImageOriginal_Click);
            RadMenuItem btnDisMulImageO2 = new RadMenuItem("监测示意图（无边框）");
            btnDisMulImageO2.Click += new EventHandler(btnMulImageOriginal2_Click);

            RadMenuItem mniDisBinImage = new RadMenuItem("二值图（当前区域）");
            mniDisBinImage.Click += new EventHandler(mhiBinImgCurrent_Click);
            RadMenuItem mniCustomBinImage = new RadMenuItem("二值图（自定义）");
            mniCustomBinImage.Click += new EventHandler(mhiBinImgCustom_Click);
            RadMenuItem mniOPTDImage = new RadMenuItem("光学厚度");
            mniOPTDImage.Click += new EventHandler(OPTDRasterImage_Click);
            RadMenuItem mniLWPImage = new RadMenuItem("液态水路径");
            mniLWPImage.Click += new EventHandler(LWPRasterImage_Click);
            RadMenuItem mniERADImage = new RadMenuItem("雾滴尺度");
            mniERADImage.Click += new EventHandler(ERADRasterImage_Click);

            RadMenuItem serverImage_dest = new RadMenuItem("天气网用图");
            serverImage_dest.Click += new EventHandler(ServerImg_Click);
            dbtnDisLayout.Items.AddRange(new RadMenuItem[] { btnDisMulImage, btnDisMulImage2, btnDisMulImageO, btnDisMulImageO2, mniDisBinImage, mniCustomBinImage, mniOPTDImage, mniLWPImage, mniERADImage, serverImage_dest });
            rbgDisaster.Items.Add(dbtnDisLayout);
            dbtnDisStat.Text = "统计分析";
            dbtnDisStat.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnDisStat.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDisStat.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniDisCurrent = new RadMenuItem("当前区域");
            mniDisCurrent.Click += new EventHandler(btnCurrentRegionArea_Click);
            RadMenuItem mniDisProvince = new RadMenuItem(provinceName);
            mniDisProvince.Click += new EventHandler(mhiProvincBound_Click);
            RadMenuItem mniDisCity = new RadMenuItem(cityName);
            mniDisCity.Click += new EventHandler(mhiCity_Click);
            RadMenuItem mniDisLandType = new RadMenuItem(landTypeName);
            mniDisLandType.Click += new EventHandler(btnLandType_Click);
            RadMenuItem mniDisCustom = new RadMenuItem("自定义");
            mniDisCustom.Click += new EventHandler(btnCustomArea_Click);
            dbtnDisStat.Items.AddRange(new RadMenuItem[] { mniDisCurrent , mniDisLandType,mniDisProvince, mniDisCity, mniDisCustom });
            rbgDisaster.Items.Add(dbtnDisStat);
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
            #endregion

            #region 周期业务
            RadRibbonBarGroup rbgCycBusiness = new RadRibbonBarGroup();
            rbgCycBusiness.Text = "周期业务";
            dbtnCycLayout.Text = "专题产品";
            dbtnCycLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnCycLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCycLayout.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniCurrentFreq = new RadMenuItem("频次图（当前区域）");
            mniCurrentFreq.Click += new EventHandler(btnCurrentRegionFreq_Click);
            RadMenuItem mniCustomFreq = new RadMenuItem("频次图（自定义）");
            mniCustomFreq.Click += new EventHandler(btnCustomFreq_Click);
            RadMenuItem mniCurrentCycTime = new RadMenuItem("周期合成（当前区域）");
            mniCurrentCycTime.Click += new EventHandler(btnCurrentRegionCycTime_Click);
            RadMenuItem mniCustomCycTime = new RadMenuItem("周期合成（自定义）");
            mniCustomCycTime.Click += new EventHandler(btnCustomCycTime_Click);
            dbtnCycLayout.Items.AddRange(new RadMenuItem[] { mniCurrentFreq, mniCustomFreq, mniCurrentCycTime, mniCustomCycTime });
            rbgCycBusiness.Items.Add(dbtnCycLayout);
            dbtnCycGenerate.Text = "快速生成";
            dbtnCycGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnCycGenerate.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCycGenerate.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniCycProGenerate = new RadMenuItem("专题产品");
            mniCycProGenerate.Click += new EventHandler(mniCycProGenerate_Click);
            dbtnCycGenerate.Items.Add(mniCycProGenerate);
            //rbgCycBusiness.Items.Add(dbtnCycGenerate);
            #endregion 周期业务

            #region 其他业务
            RadRibbonBarGroup rbgOther = new RadRibbonBarGroup();
            rbgOther.Text = "其他业务";
            //dbtnTOU.Text = "小仪器雾霾";
            //dbtnTOU.ArrowPosition = DropDownButtonArrowPosition.Right;
            //dbtnTOU.ImageAlignment = ContentAlignment.TopCenter;
            //dbtnTOU.TextAlignment = ContentAlignment.BottomCenter;
            //RadMenuItem mniImputTou = new RadMenuItem("导入");
            //mniImputTou.Click += new EventHandler(btnImportTOU_Click);
            //RadMenuItem mniTouImage = new RadMenuItem("专题图");
            //mniTouImage.Click += new EventHandler(btnTOUImage_Click);
            //dbtnTOU.Items.AddRange(new RadMenuItem[] { mniImputTou, mniTouImage });
            //rbgOther.Items.Add(dbtnTOU);
            dbtnAnimation.Text = "动画";
            dbtnAnimation.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnAnimation.ImageAlignment = ContentAlignment.TopCenter;
            dbtnAnimation.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniCurrentAnimation = new RadMenuItem("动画（当前区域）");
            mniCurrentAnimation.Click += new EventHandler(btnAnimationRegion_Click);
            RadMenuItem mniCustomAnimation = new RadMenuItem("动画（自定义）");
            mniCustomAnimation.Click += new EventHandler(btnAnimationCustom_Click);
            dbtnAnimation.Items.AddRange(new RadMenuItem[] { mniCurrentAnimation, mniCustomAnimation });
            rbgOther.Items.Add(dbtnAnimation);
            #endregion

            _tab.Items.Add(rbgCheck);
            //_tab.Items.Add(rbgDaily);
            _tab.Items.Add(rbgDisaster);
            //真彩图分组
            RadRibbonBarGroup rbgFOG = new RadRibbonBarGroup();
            rbgFOG.Text = "真彩图";
            btnNatrueColor.ImageAlignment = ContentAlignment.TopCenter;
            btnNatrueColor.TextAlignment = ContentAlignment.BottomCenter;
            btnNatrueColor.Click += new EventHandler(btnNatrueColor_Click);
            rbgFOG.Items.Add(btnNatrueColor);
            _tab.Items.Add(rbgFOG);
            _tab.Items.Add(rbgCycBusiness);
            _tab.Items.Add(rbgOther);

            btnToDb.TextAlignment = ContentAlignment.BottomCenter;
            btnToDb.ImageAlignment = ContentAlignment.TopCenter;
            btnToDb.Click += new EventHandler(btnToDb_Click);
            RadRibbonBarGroup rbgToDb = new RadRibbonBarGroup();
            rbgToDb.Text = "产品入库";
            rbgToDb.Items.Add(btnToDb);
            _tab.Items.Add(rbgToDb);

            Controls.Add(_bar);
        }

        private void AddCloseButton()
        {
            RadRibbonBarGroup rbgClose = new RadRibbonBarGroup();
            rbgClose.Text = "关闭";
            btnClose.ImageAlignment = ContentAlignment.TopCenter;
            btnClose.TextAlignment = ContentAlignment.BottomCenter;
            btnClose.Click += new EventHandler(btnClose_Click);
            rbgClose.Items.Add(btnClose);
            _tab.Items.Add(rbgClose);
        }

        #region 初始化
        public object Content
        {
            get { return _tab; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;
            AddThemeGraphRegionBotton();
            CreateqQuickReportRegionButton();
            AddCloseButton();
            SetImage();
        }

        private void CreateqQuickReportRegionButton()
        {
            QuickReportFactory.RegistToButton("FOG", _tab, _session);
        }

        private void AddThemeGraphRegionBotton()
        {
            RadDropDownButtonElement _btnThemeGraphRegion = new RadDropDownButtonElement();
            ThemeGraphRegionFacctory.RegistToButton("FOG", _btnThemeGraphRegion, _tab, _session);
            _btnThemeGraphRegion.Image = _session.UIFrameworkHelper.GetImage("system:settings.png");
        }
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
                cmd.Execute("FOG","PNG");
          
        }


       

        public void UpdateStatus()
        { }

        private void SetImage()
        {
            btnNatrueColor.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            btnRough.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            btnAutoGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnInteractive.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            _btnMonitorShow.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnbinImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            btnCustomFreq.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnCurrentRegionFreq.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");   
            btnAnimationRegion.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            OPTDRasterCalc.Image = _session.UIFrameworkHelper.GetImage("system:fogOPTD.png");
            quantifyRasterCalc.Image = _session.UIFrameworkHelper.GetImage("system:fogQuantifyCalc.png");
            LWPRasterCalc.Image = _session.UIFrameworkHelper.GetImage("system:fogLwp.png");
            ERADRasterCalc.Image = _session.UIFrameworkHelper.GetImage("system:fogErad.png");
            dbtnQuantifyImage.Image = _session.UIFrameworkHelper.GetImage("system:fogQuantify.png");
            dbtnQuantifyCalc.Image = _session.UIFrameworkHelper.GetImage("system:fogQuantifyCalc.png");
            btnCSRCalc.Image = _session.UIFrameworkHelper.GetImage("system:fogCSR.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnAnimationCustom.Image = _session.UIFrameworkHelper.GetImage("system:customVedio.png");
            btnCurrentRegionCycTime.Image = _session.UIFrameworkHelper.GetImage("system:fogCycTime.png");
            btnCustomCycTime.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
            //btnImportTOU.Image = _session.UIFrameworkHelper.GetImage("system:import.png");
            //btnTOUImage.Image = _session.UIFrameworkHelper.GetImage("system:TOUImage.png");
            dbtnOrdLayout.Image = _session.UIFrameworkHelper.GetImage("system: mulImage.png");
            dbtnOrdStatArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            dbtnOrdGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnDisLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnDisStat.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            dbtnDisGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnCycLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnCycGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnTOU.Image = _session.UIFrameworkHelper.GetImage("system:TOUImage.png");
            dbtnAnimation.Image = _session.UIFrameworkHelper.GetImage("system:customVedio.png");
            dbtnOrdLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnCycLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            btnPicLayout.Image = _session.UIFrameworkHelper.GetImage("system:Layout_ToFullEnvelope.png");
        }
        #endregion

        #region 按钮相关事件

        #region 判识相关按钮事件

        /// <summary>
        /// 交互按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void fogProduct_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExecute(6602);
        }

        void cloudProduct_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0CLM");
            GetCommandAndExecute(6602);
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
        /// 带粗判的快速生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void roughProduct_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "DBLV");
        }

        /// <summary>
        /// 快速生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void autoProduct_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "0CSR");
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
            AutoGenretateProvider.AutoGenrate(_session, "0IMG", null, "Ord");
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
            AutoGenretateProvider.AutoGenrate(_session, "0CSR", null, "Dis");
        }

        void mniCycProGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "FREQ", null, "Cyc");
        }

        #endregion

        #region 特征信息提取相关按钮事件

        /// <summary>
        /// 晴空反射率提取按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCSRCalc_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0CSR");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 特征量信息提取按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void quantifyRasterCalc_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "0CSR", null, "Quantify");
            //(_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("QUTY");
            //GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 光学厚度计算按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OPTDRasterCalc_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("OPTD");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 雾滴尺度计算按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ERADRasterCalc_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("ERAD");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 液态水路径计算按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LWPRasterCalc_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0LWP");
            GetCommandAndExecute(6602);
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
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "CSDI", "", "DBLV", "大雾监测专题图模板", true, true) == null)
                return;
        }

        /// <summary>
        /// 当前区域二值图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiBinImgCurrent_Click(object sender, EventArgs e)
        {
            //SetIsFixImageRegion();
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "DBLV", "大雾监测专题图模板", false, true) == null)
                return;
        }
        /// <summary>
        /// 当前 服务器用图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ServerImg_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "TNCI");//根据模板自适应
            ms.DoAutoExtract(false);

            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "ONCI");   //原始
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("HEAThemeGraphTemplateName", "");
            ms.DoAutoExtract(false);

        }

        /// <summary>
        /// 监测图 有边框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMulImage_Click(object sender, EventArgs e)
        {
            //SetIsFixImageRegion();
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MCSI");
            ms.DoAutoExtract(false);
        }

        /// <summary>
        /// 监测图 无边框（原始分辨率）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMulImageOriginal_Click(object sender, EventArgs e)
        {
            //SetIsFixImageRegion();
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "OMCS");
            ms.DoAutoExtract(false);
        }

        /// <summary>
        /// 监测示意图 有边框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMulImage2_Click(object sender, EventArgs e)
        {
            //SetIsFixImageRegion();
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0MSI");
            ms.DoAutoExtract(false);
        }

        /// <summary>
        /// 监测示意图 无边框（原始分辨率）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMulImageOriginal2_Click(object sender, EventArgs e)
        {
            //SetIsFixImageRegion();
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MOSI");
            ms.DoAutoExtract(false);
        }

        /// <summary>
        /// 雾滴尺度专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ERADRasterImage_Click(object sender, EventArgs e)
        {
            SetIsFixImageRegion();
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "ERAI", "", "ERAD", "大雾雾滴尺度专题图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 液态水路径专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LWPRasterImage_Click(object sender, EventArgs e)
        {
            SetIsFixImageRegion();
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "LWPI", "", "0LWP", "大雾液态水路径专题图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 光学厚度专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OPTDRasterImage_Click(object sender, EventArgs e)
        {
            SetIsFixImageRegion();
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "OPTI", "", "OPTD", "大雾光学厚度专题图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 大雾强度专题图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FOGGRasterImage_Click(object sender, EventArgs e)
        {
            SetIsFixImageRegion();
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "FOGI", "FOGGIMG", "OPTD", "大雾强度专题图模板", false, true) == null)
                return;
        }
        
        private void SetIsFixImageRegion()
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("FixImageRegion", _isFixImageRegion ? "true" : "false");
        }

        /// <summary>
        /// 设置专题图是否固定区域
        /// </summary>
        private bool _isFixImageRegion = false;

        void btnFixImageRegion_Click(object sender, EventArgs e)
        {
            (sender as RadMenuItem).IsChecked = !(sender as RadMenuItem).IsChecked;
            _isFixImageRegion = (sender as RadMenuItem).IsChecked;
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
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "CLUT");
            ms.DoAutoExtract(true);
        }

        /// <summary>
        /// 按省界面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiProvincBound_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0CBP");
            ms.DoAutoExtract(true);
        }

        /// <summary>
        /// 按市县面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiCity_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0CCC");
            ms.DoAutoExtract(true);
        }

        #endregion

        #region 周期合成相关按钮事件

        /// <summary>
        /// 自定义周期合成按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCustomCycTime_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CycleTimeStat(_session, "CCYI", "FOGCYCI", "DBLV", "大雾周期统计专题图模板", true, true) == null)
                return;
        }

        /// <summary>
        /// 当前区域周期合成按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCurrentRegionCycTime_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CycleTimeStat(_session, "CYCI", "FOGCYCI", "DBLV", "大雾周期统计专题图模板", false, true) == null)
                return;
        }

        #endregion

        #region 频次统计相关按钮事件

        /// <summary>
        /// 自定义频次统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCustomFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "CFRI", "FOGFREQ", "DBLV", "大雾频次统计专题图模板", true, true) == null)
                return;
        }

        /// <summary>
        /// 当前区域频次统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCurrentRegionFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "FREQ", "FOGFREQ", "DBLV", "大雾频次统计专题图模板", false, true) == null)
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
            MIFCommAnalysis.CreatAVI(_session, "template:大雾动画展示专题图,动画展示专题图,FOG,CMED");
        }

        /// <summary>
        /// 当前区域动画按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAnimationRegion_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.CreatAVI(_session, "template:大雾动画展示专题图,动画展示专题图,FOG,MEDI");
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

        #region 产品入库

        void btnToDb_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(39002);
            if (cmd != null)
                cmd.Execute();
        }

        #endregion

        #region 小仪器雾霾

        /// <summary>
        /// 导入小仪器雾霾数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnImportTOU_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0TOU");
            GetCommandAndExecute(6602);
        }

        void btnTOUImage_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:雾霾监测示意图,多通道合成图,FOG,TOUI");
        }

        #endregion

        private void GetCommandAndExecute(int cmdID)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute("FOG");
        }

        private void GetCommandAndExecute(int cmdID, string template)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute(template);
        }
        /// <summary>
        /// 真彩图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnNatrueColor_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("NCIM");
            GetCommandAndExecute(6602);
        }

        #endregion

    }
}
