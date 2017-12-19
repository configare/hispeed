using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls.UI;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.ASL
{
    public partial class UITabASL : UserControl,IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 气溶胶按钮定义

        #region 判识相关按钮
        RadButtonElement btnRough = new RadButtonElement("自动判识");
        RadDropDownButtonElement btnManualExtract = new RadDropDownButtonElement();//交互判识
        RadDropDownButtonElement btnSave = new RadDropDownButtonElement();//快速生成

        //RadDropDownButtonElement dbtnAtomsCorrect = new RadDropDownButtonElement(); //大气校正
        //RadDropDownButtonElement dbtnCloudMask = new RadDropDownButtonElement();    //云检测
        //RadDropDownButtonElement dbtnAerosolRevision = new RadDropDownButtonElement();      //气溶胶反演
        #endregion

        #region 专题图相关按钮
        RadDropDownButtonElement btnAODMultiImage = new RadDropDownButtonElement();//气溶胶光学厚度AOD多通道合成图
        RadDropDownButtonElement dbtnbinImage = new RadDropDownButtonElement();//专题图
        #endregion

        #region 面积统计相关按钮
        RadButtonElement btnCurrentRegionArea = new RadButtonElement("当前区域");
        RadDropDownButtonElement dbtnDivision = new RadDropDownButtonElement();//行政区划
        RadButtonElement btnLandType = new RadButtonElement();//土地类型
        RadButtonElement btnCustomArea = new RadButtonElement("自定义");
        #endregion

        RadButtonElement btnClose = new RadButtonElement("关闭气溶胶\n分析视图");
        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        #endregion


        public UITabASL()
        {
            InitializeComponent();
            CreateRibbonBar();
        }

        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "气溶胶专题";
            _tab.Text = "气溶胶专题";
            _tab.Name = "气溶胶专题";
            _bar.CommandTabs.Add(_tab);

            RadRibbonBarGroup rbgMonitor = new RadRibbonBarGroup();
            rbgMonitor.Text = "监测";
            CreatMonitorGroup(rbgMonitor);
            _tab.Items.Add(rbgMonitor);
            
            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "专题产品";
            CreatbinImageGroup(rbgProduct);
            _tab.Items.Add(rbgProduct);

            RadRibbonBarGroup rbgAreaStatic = new RadRibbonBarGroup();
            rbgAreaStatic.Text = "面积统计";
            CreatAreaStaticGroup(rbgAreaStatic);
            _tab.Items.Add(rbgAreaStatic);

            //产品入库
            btnToDb.TextAlignment = ContentAlignment.BottomCenter;
            btnToDb.ImageAlignment = ContentAlignment.TopCenter;
            //btnToDb.Click += new EventHandler(btnToDb_Click);
            RadRibbonBarGroup rbgToDb = new RadRibbonBarGroup();
            rbgToDb.Text = "产品入库";
            rbgToDb.Items.Add(btnToDb);
            _tab.Items.Add(rbgToDb);

            Controls.Add(_bar);

        }

        private void CreatMonitorGroup(RadRibbonBarGroup rbgMonitor)
        {
            btnRough.ImageAlignment = ContentAlignment.TopCenter;
            btnRough.TextAlignment = ContentAlignment.BottomCenter;
            rbgMonitor.Items.Add(btnRough);//自动判识
            btnRough.Click += new EventHandler(btnRough_Click);

            btnManualExtract.Text = "交互判识";
            btnManualExtract.ImageAlignment = ContentAlignment.TopCenter;
            btnManualExtract.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem riAtomsCorrect = new RadMenuItem("大气校正");
            btnManualExtract.Items.Add(riAtomsCorrect);
            RadMenuItem riCloudMask = new RadMenuItem("云检测");
            btnManualExtract.Items.Add(riCloudMask);
            RadMenuItem riAerosolRevision = new RadMenuItem("气溶胶反演");
            btnManualExtract.Items.Add(riAerosolRevision);
            rbgMonitor.Items.Add(btnManualExtract);

            btnSave.Text = "快速生成";
            RadMenuItem item1 = new RadMenuItem();
            item1.Text = "自动判识+专题产品";
            //item1.Click += new EventHandler(btnSave_Click);
            btnSave.Items.Add(item1);
            RadMenuItem item2 = new RadMenuItem();
            //item2.Click += new EventHandler(item2_Click);
            item2.Text = "专题产品";
            btnSave.Items.Add(item2);
            btnSave.ImageAlignment = ContentAlignment.TopCenter;
            btnSave.TextAlignment = ContentAlignment.BottomCenter;
            rbgMonitor.Items.Add(btnSave);
            //foreach (RadButtonElement item in rbgMonitor.Items)
            //{
            //    item.ImageAlignment = ContentAlignment.TopCenter;
            //    item.TextAlignment = ContentAlignment.BottomCenter;
            //}
        }

        private void CreatbinImageGroup(RadRibbonBarGroup rbgProduct)
        {
            btnAODMultiImage.Text = "AOD多通道合成图";
            btnAODMultiImage.ImageAlignment = ContentAlignment.TopCenter;
            btnAODMultiImage.TextAlignment = ContentAlignment.BottomCenter;
            btnAODMultiImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem mnuMulImage = new RadMenuItem("AOD多通道合成图");
            mnuMulImage.Click += new EventHandler(mnuMulImage_Click);
            RadMenuItem mnuOMulImage = new RadMenuItem("AOD多通道合成图（原始分辩率）");
            mnuOMulImage.Click += new EventHandler(mnuOMulImage_Click);
            btnAODMultiImage.Items.AddRange(mnuMulImage, mnuOMulImage);
            rbgProduct.Items.Add(btnAODMultiImage);

            dbtnbinImage.Text = "专题图";
            dbtnbinImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnbinImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnbinImage.TextAlignment = ContentAlignment.BottomCenter;
            //RadMenuItem tauBinImg047Current = new RadMenuItem("AOD0.47μm（当前区域）");
            //dbtnbinImage.Items.Add(tauBinImg047Current);
            //tauBinImg047Current.Click += new EventHandler(tauBinImg047Current_Click);
            RadMenuItem tauBinImg055Current = new RadMenuItem("AOD0.55μm（当前区域）");
            dbtnbinImage.Items.Add(tauBinImg055Current);
            tauBinImg055Current.Click += new EventHandler(tauBinImg055Current_Click);
            //RadMenuItem tauBinImg066Current = new RadMenuItem("AOD0.66μm（当前区域）");
            //dbtnbinImage.Items.Add(tauBinImg066Current);
            //tauBinImg066Current.Click += new EventHandler(tauBinImg066Current_Click);
            //RadMenuItem SDS_Dust_WeightingCurrent = new RadMenuItem("气溶胶类型");
            //dbtnbinImage.Items.Add(SDS_Dust_WeightingCurrent);
            //SDS_Dust_WeightingCurrent.Click += new EventHandler(SDS_Dust_WeightingCurrent_Click);
            //RadMenuItem Angstrom_Coefficient = new RadMenuItem("Angstrom指数");
            //dbtnbinImage.Items.Add(Angstrom_Coefficient);
            //Angstrom_Coefficient.Click += new EventHandler(Angstrom_Coefficient_Click);
            //RadMenuItem Mass_Concentration = new RadMenuItem("柱状质量浓度");
            //dbtnbinImage.Items.Add(Mass_Concentration);
            //Mass_Concentration.Click += new EventHandler(Mass_Concentration_Click);
            rbgProduct.Items.Add(dbtnbinImage);
        }

        private void CreatAreaStaticGroup(RadRibbonBarGroup rbgAreaStatic)
        {
            //当前区域
            btnCurrentRegionArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCurrentRegionArea.TextAlignment = ContentAlignment.BottomCenter;
            btnCurrentRegionArea.Click += new EventHandler(btnCurrentRegionArea_Click);
            rbgAreaStatic.Items.Add(btnCurrentRegionArea);

            dbtnDivision.Text = "行政区划";
            string provinceName = AreaStatProvider.GetAreaStatItemMenuName("行政区划");
            string cityName = AreaStatProvider.GetAreaStatItemMenuName("三级行政区划");
            dbtnDivision.TextAlignment = ContentAlignment.BottomCenter;
            dbtnDivision.ImageAlignment = ContentAlignment.TopCenter;
            RadMenuItem mhiProvincBound = new RadMenuItem(provinceName);//省界分类
            dbtnDivision.Items.Add(mhiProvincBound);
            mhiProvincBound.Click += new EventHandler(mhiProvincBound_Click);
            RadMenuItem mhiCity = new RadMenuItem(cityName);//市界分级
            dbtnDivision.Items.Add(mhiCity);
            mhiCity.Click += new EventHandler(mhiCity_Click);
            rbgAreaStatic.Items.Add(dbtnDivision);

            //土地分类
            string landTypeName = AreaStatProvider.GetAreaStatItemMenuName("土地利用类型");
            btnLandType.Text = landTypeName;
            btnLandType.ImageAlignment = ContentAlignment.TopCenter;
            btnLandType.TextAlignment = ContentAlignment.BottomCenter;
            btnLandType.Click += new EventHandler(btnLandType_Click);
            rbgAreaStatic.Items.Add(btnLandType);
            
            //自定义
            btnCustomArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCustomArea.TextAlignment = ContentAlignment.BottomCenter;
            rbgAreaStatic.Items.Add(btnCustomArea);
            btnCustomArea.Click += new EventHandler(btnCustomArea_Click);
        }

        #region 初始化
        public object Content
        {
            get { return _tab; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            this._session = session;
            CreateThemeGraphRegionButton();
            CreateClose();
            SetImage();
        }

        private void CreateThemeGraphRegionButton()
        {
            RadDropDownButtonElement _btnThemeGraphRegion = new RadDropDownButtonElement();
            ThemeGraphRegionFacctory.RegistToButton("ASL", _btnThemeGraphRegion, _tab, _session);
            _btnThemeGraphRegion.Image = _session.UIFrameworkHelper.GetImage("system:settings.png");
        }

        private void CreateClose()
        {
            RadRibbonBarGroup rbgClose = new RadRibbonBarGroup();
            rbgClose.Text = "关闭";
            btnClose.ImageAlignment = ContentAlignment.TopCenter;
            btnClose.TextAlignment = ContentAlignment.BottomCenter;
            btnClose.Click += new EventHandler(btnClose_Click);
            rbgClose.Items.Add(btnClose);
            _tab.Items.Add(rbgClose);
        }

        public void UpdateStatus()
        { }

        private void SetImage()
        {
            btnRough.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            btnManualExtract.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            btnSave.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnAODMultiImage.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnbinImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            btnCurrentRegionArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            dbtnDivision.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            btnLandType.Image = _session.UIFrameworkHelper.GetImage("system:landType.png");
            btnCustomArea.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");                  
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
        }
        #endregion

        #region 按钮事件

        #region 监测相关按钮事件
        /// <summary>
        /// 自动判识
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnRough_Click(object sender, EventArgs e)
        {
            frmDoAerosolReversion frm = new frmDoAerosolReversion();
            frm.Show();
        }

        /// <summary>
        /// 快速生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSave_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "055T");
        }
        #endregion

        #region 专题产品相关按钮事件
        /// <summary>
        /// AOD多通道合成图（原始分辩率）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuOMulImage_Click(object sender, EventArgs e)
        {
            //IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            //ms.ChangeActiveSubProduct("0IMG");
            //ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "OMCS");
            //ms.DoAutoExtract(true);
        }

        /// <summary>
        /// AOD多通道合成图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuMulImage_Click(object sender, EventArgs e)
        {
            //MIFCommAnalysis.DisplayMonitorShow(_session, "template:陆表高温监测示意图,监测示意图,LST,MCSI");
        }

        private void tauBinImg047Current_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 0.55微米波段的AOD
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tauBinImg055Current_Click(object sender, EventArgs e)
        {
            try
            {
                (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0IMG");
                ICommand cmd = _session.CommandEnvironment.Get(6601);
                if (cmd != null)
                    cmd.Execute();
            }
            finally
            {
            }

        }

        private void tauBinImg066Current_Click(object sender, EventArgs e)
        {

        }

        private void SDS_Dust_WeightingCurrent_Click(object sender, EventArgs e)
        {

        }

        private void Angstrom_Coefficient_Click(object sender, EventArgs e)
        {

        }

        private void Mass_Concentration_Click(object sender, EventArgs e)
        {

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
            ms.DoManualExtract(true);
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
            ms.DoManualExtract(true);
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
            ms.DoManualExtract(true);
        }
        #endregion

        #region 关闭相关按钮事件
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

        private void GetCommandAndExecute(int cmdID)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute("ASL");
        }    

        #endregion

    }
}
