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
using GeoDo.RSS.Core.DF;
using Telerik.WinControls;
using Telerik.WinControls.Primitives;
using GeoDo.RSS.UI.AddIn.CMA.UIProvider.FIR;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    public partial class UITabFire : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 火情按钮定义
        RadButtonElement btnAutoExtract = new RadButtonElement("粗判");
        RadDropDownButtonElement btnAutoGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement btnManualExtract = new RadDropDownButtonElement();
        RadMenuItem btnManualExtract_FIR = new RadMenuItem("火");
        RadMenuItem btnManualExtract_FIRG = new RadMenuItem("过火区");
        RadMenuItem btnManualExtract_SMOK = new RadMenuItem("烟尘");

        RadButtonElement btnImport = new RadButtonElement("导入");
        RadButtonElement btntransf = new RadButtonElement("转换");
        RadDropDownButtonElement dbtngFirepoint = new RadDropDownButtonElement();

        RadButtonElement btnMulImage = new RadButtonElement("多通道合成图");
        RadDropDownButtonElement dbtnbinImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnStrenImage = new RadDropDownButtonElement();
        RadButtonElement btnFirepList = new RadButtonElement("火点信息列表");
        RadButtonElement btnFireaList = new RadButtonElement("火区信息列表");
        RadButtonElement btnComChart = new RadButtonElement("对比图");
        RadButtonElement btnComList = new RadButtonElement("对比列表");
        RadButtonElement btnCurrentRegionArea = new RadButtonElement("当前区域");
        RadDropDownButtonElement dbtnDivision = new RadDropDownButtonElement();
        RadButtonElement btnLandType = new RadButtonElement("土地类型");
        RadButtonElement btnCustomArea = new RadButtonElement("自定义");
        RadButtonElement btnCurrentRegionFreq = new RadButtonElement("当前区域");
        RadButtonElement btnCustomFreq = new RadButtonElement("自定义");
        RadButtonElement btnClose = new RadButtonElement("关闭火情监测\n分析视图");

        RadButtonElement btnToDb = new RadButtonElement("产品入库");

        #endregion

        public UITabFire()
        {
            InitializeComponent();
            CreateRibbonBar();
        }

        #region 工具栏定义

        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "火情监测专题";
            _tab.Text = "火情监测专题";
            _tab.Name = "火情监测专题";
            _bar.CommandTabs.Add(_tab);

            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            rbgCheck.Text = "监测";
            btnAutoExtract.ImageAlignment = ContentAlignment.TopCenter;
            btnAutoExtract.TextAlignment = ContentAlignment.BottomCenter;
            btnAutoExtract.Click += new EventHandler(btnAutoExtract_Click);
            rbgCheck.Items.Add(btnAutoExtract);
            //
            btnManualExtract.Text = "交互判识";
            btnManualExtract.ImageAlignment = ContentAlignment.TopCenter;
            btnManualExtract.TextAlignment = ContentAlignment.BottomCenter;
            btnManualExtract.Items.AddRange(new RadItem[] 
            {
                btnManualExtract_FIR,
                btnManualExtract_FIRG,
                btnManualExtract_SMOK
            });
            rbgCheck.Items.Add(btnManualExtract);
            //
            btnManualExtract_FIR.Click += new EventHandler(btnManualExtract_FIR_Click);
            btnManualExtract_FIRG.Click += new EventHandler(btnManualExtract_FIRG_Click);
            btnManualExtract_SMOK.Click += new EventHandler(btnManualExtract_SMOK_Click);
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
            RadRibbonBarGroup rbglevelProduct = new RadRibbonBarGroup();
            rbglevelProduct.Text = "二三级产品";
            btnImport.ImageAlignment = ContentAlignment.TopCenter;
            btnImport.TextAlignment = ContentAlignment.BottomCenter;
            rbglevelProduct.Items.Add(btnImport);
            rbglevelProduct.Items.Add(btntransf);
            dbtngFirepoint.Text = "全球火点";
            dbtngFirepoint.TextAlignment = ContentAlignment.BottomCenter;
            dbtngFirepoint.ImageAlignment = ContentAlignment.TopCenter;
            RadMenuItem mhiCorrect = new RadMenuItem("数据修正");
            dbtngFirepoint.Items.Add(mhiCorrect);
            mhiCorrect.Click += new EventHandler(mhiCorrect_Click);
            RadMenuItem mhiPointDay = new RadMenuItem("火点面积");
            dbtngFirepoint.Items.Add(mhiPointDay);
            RadMenuItem mhiPointFreq = new RadMenuItem("频次火点");
            dbtngFirepoint.Items.Add(mhiPointFreq);
            rbglevelProduct.Items.Add(dbtngFirepoint);
            btntransf.ImageAlignment = ContentAlignment.TopCenter;
            btntransf.TextAlignment = ContentAlignment.BottomCenter;

            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "专题产品";
            btnMulImage.ImageAlignment = ContentAlignment.TopCenter;
            btnMulImage.TextAlignment = ContentAlignment.BottomCenter;
            btnMulImage.Click += new EventHandler(btnRGBLayout_Click);
            rbgProduct.Items.Add(btnMulImage);
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
            dbtnStrenImage.Text = "强度图";
            dbtnStrenImage.TextAlignment = ContentAlignment.BottomCenter;
            dbtnStrenImage.ImageAlignment = ContentAlignment.TopCenter;
            RadMenuItem mhiStrenImgCurrent = new RadMenuItem("当前区域");
            mhiStrenImgCurrent.Click += new EventHandler(mhiStrenImgCurrent_Click);
            dbtnStrenImage.Items.Add(mhiStrenImgCurrent);
            RadMenuItem mhiStrenImgCustom = new RadMenuItem("自定义");
            mhiStrenImgCustom.Click += new EventHandler(mhiStrenImgCustom_Click);
            dbtnStrenImage.Items.Add(mhiStrenImgCustom);
            rbgProduct.Items.Add(dbtnStrenImage);
            btnFirepList.ImageAlignment = ContentAlignment.TopCenter;
            btnFirepList.TextAlignment = ContentAlignment.BottomCenter;
            rbgProduct.Items.Add(btnFirepList);
            btnFireaList.ImageAlignment = ContentAlignment.TopCenter;
            btnFireaList.TextAlignment = ContentAlignment.BottomCenter;
            rbgProduct.Items.Add(btnFireaList);

            RadRibbonBarGroup rbgComparaAnaly = new RadRibbonBarGroup();
            rbgComparaAnaly.Text = "对比分析";
            btnComChart.ImageAlignment = ContentAlignment.TopCenter;
            btnComChart.TextAlignment = ContentAlignment.BottomCenter;
            btnComChart.Click += new EventHandler(btnComChart_Click);
            rbgComparaAnaly.Items.Add(btnComChart);
            btnComList.ImageAlignment = ContentAlignment.TopCenter;
            btnComList.TextAlignment = ContentAlignment.BottomCenter;
            rbgComparaAnaly.Items.Add(btnComList);

            RadRibbonBarGroup rbgAreaStatic = new RadRibbonBarGroup();
            rbgAreaStatic.Text = "面积统计";
            dbtnDivision.Text = "行政区划";
            dbtnDivision.TextAlignment = ContentAlignment.BottomCenter;
            dbtnDivision.ImageAlignment = ContentAlignment.TopCenter;
            RadMenuItem mhiCity = new RadMenuItem("按市县");
            mhiCity.Click += new EventHandler(mhiCity_Click);
            dbtnDivision.Items.Add(mhiCity);
            RadMenuItem mhiProvincBound = new RadMenuItem("按省界");
            mhiProvincBound.Click += new EventHandler(mhiProvincBound_Click);
            dbtnDivision.Items.Add(mhiProvincBound);
            btnCurrentRegionArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCurrentRegionArea.TextAlignment = ContentAlignment.BottomCenter;
            btnCurrentRegionArea.Click += new EventHandler(btnCurrentRegionArea_Click);
            rbgAreaStatic.Items.Add(btnCurrentRegionArea);
            rbgAreaStatic.Items.Add(dbtnDivision);
            btnLandType.ImageAlignment = ContentAlignment.TopCenter;
            btnLandType.TextAlignment = ContentAlignment.BottomCenter;
            btnLandType.Click += new EventHandler(btnLandType_Click);
            rbgAreaStatic.Items.Add(btnLandType);
            btnCustomArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCustomArea.TextAlignment = ContentAlignment.BottomCenter;
            btnCustomArea.Click += new EventHandler(btnCustomArea_Click);
            rbgAreaStatic.Items.Add(btnCustomArea);

            RadRibbonBarGroup rbgFreqStatic = new RadRibbonBarGroup();
            rbgFreqStatic.Text = "频次统计";
            btnCurrentRegionFreq.ImageAlignment = ContentAlignment.TopCenter;
            btnCurrentRegionFreq.TextAlignment = ContentAlignment.BottomCenter;
            btnCurrentRegionFreq.Click += new EventHandler(btnCurrentRegionFreq_Click);
            rbgFreqStatic.Items.Add(btnCurrentRegionFreq);
            btnCustomFreq.TextAlignment = ContentAlignment.BottomCenter;
            btnCustomFreq.ImageAlignment = ContentAlignment.TopCenter;
            btnCustomFreq.Click += new EventHandler(btnCustomFreq_Click);
            rbgFreqStatic.Items.Add(btnCustomFreq);

            RadRibbonBarGroup rbgClose = new RadRibbonBarGroup();
            rbgClose.Text = "关闭";
            btnClose.TextAlignment = ContentAlignment.BottomCenter;
            btnClose.ImageAlignment = ContentAlignment.TopCenter;
            btnClose.Click += new EventHandler(btnClose_Click);
            rbgClose.Items.Add(btnClose);

            _tab.Items.Add(rbgCheck);
            _tab.Items.Add(rbglevelProduct);
            _tab.Items.Add(rbgProduct);
            _tab.Items.Add(rbgComparaAnaly);
            _tab.Items.Add(rbgAreaStatic);
            _tab.Items.Add(rbgFreqStatic);

            
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

        #endregion

        #region 初始化Session + 图标设置 + IUITabProvider接口实现

        public object Content
        {
            get { return _tab; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            this._session = session;
            SetImage();
        }

        private void SetImage()
        {
            btnAutoExtract.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            dbtnDivision.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            btnAutoGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnImport.Image = _session.UIFrameworkHelper.GetImage("system:import.png");
            btnManualExtract.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            btnMulImage.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnbinImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            dbtngFirepoint.Image = _session.UIFrameworkHelper.GetImage("system:PGS.png");
            btnCurrentRegionArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            btnLandType.Image = _session.UIFrameworkHelper.GetImage("system:landType.png");
            btnCustomArea.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnCustomFreq.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnFirepList.Image = _session.UIFrameworkHelper.GetImage("system:firePointList.png");
            btnComChart.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
            btnComList.Image = _session.UIFrameworkHelper.GetImage("system:comList.png");
            btnCurrentRegionFreq.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
            dbtnStrenImage.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
            btnFireaList.Image = _session.UIFrameworkHelper.GetImage("system:fireRigionList.png");
            btntransf.Image = _session.UIFrameworkHelper.GetImage("system:transform.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
        }

        public void UpdateStatus()
        {

        }

        #endregion

        #region 按钮事件
        //产品入库
        void btnToDb_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(39002);
            if (cmd != null)
                cmd.Execute();
        }

        void btnRGBLayout_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:火情产品多通道合成图,多通道合成图,FIR,MCSI");
        }

        void btnManualExtract_SMOK_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("SMOK");
            ICommand cmd = _session.CommandEnvironment.Get(6602);
            if (cmd != null)
                cmd.Execute();
        }

        void btnManualExtract_FIRG_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("FIRG");
            ICommand cmd = _session.CommandEnvironment.Get(6602);
            if (cmd != null)
                cmd.Execute();
        }

        void btnManualExtract_FIR_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            ICommand cmd = _session.CommandEnvironment.Get(6602);
            if (cmd != null)
                cmd.Execute();
        }

        void btnAutoExtract_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            ICommand cmd = _session.CommandEnvironment.Get(6601);
            if (cmd != null)
                cmd.Execute();
        }

        void btnClose_Click(object sender, EventArgs e)
        {
            if (_session == null)
                return;
            _session.UIFrameworkHelper.Remove(_tab.Text);
            _session.UIFrameworkHelper.ActiveDefaultTab();
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveProduct(null);
        }

        #region by chennan 20120813

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
        /// 火情当前区域二值图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiBinImgCurrent_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "FIRDBLV", "DBLV", "火情监测专题图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 火情自定义二值图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiBinImgCustom_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "CSDI", "FIRDBLV", "DBLV", "火情监测专题图模板", true, true) == null)
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

        /// <summary>
        /// 自定义频次统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCustomFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "CFRI", "", "DBLV", "火情频次统计专题图模板", true, true) == null)
                return;
        }

        /// <summary>
        /// 当前区域频次统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCurrentRegionFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "FREQ", "", "DBLV", "火情频次统计专题图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 当前区域火点强度图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiStrenImgCurrent_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "FPGI", "FIR0FPG", "0FPG", "火情强度图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 当前区域火点强度图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiStrenImgCustom_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "CFPG", "FIR0FPG", "0FPG", "火情强度图模板", true, true) == null)
                return;
        }
        #endregion

        #region  by chennan 20121024

        /// <summary>
        /// 全球火点修正
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiCorrect_Click(object sender, EventArgs e)
        {
            try
            {
                ISmartWindow smartWindow = _session.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(masGbalFireCorrectPage); });
                if (smartWindow == null)
                {
                    smartWindow = new masGbalFireCorrectPage(_session);
                    _session.SmartWindowManager.DisplayWindow(smartWindow);
                    SetArgsPanelWidth(smartWindow);
                }
                else
                    _session.SmartWindowManager.DisplayWindow(smartWindow);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SetArgsPanelWidth(ISmartWindow smartWindow)
        {
            masGbalFireCorrectPage window = smartWindow as masGbalFireCorrectPage;
            window.SetArgsPanelWidth((int)(window.Width / 4.5f));
        }

        private void btnComChart_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "COMP", "FIRCOMP", "COMP", "COMP", "DBLV", "火情对比分析专题图", false, true) == null)
                return;
        }

        #endregion

        #endregion
    }
}
