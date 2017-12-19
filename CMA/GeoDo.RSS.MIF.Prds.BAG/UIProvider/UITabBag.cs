using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Prds.BAG;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public partial class UITabBag : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;
        XDocument _configDoc = null;
        Dictionary<string, string> _aoiTemplateList = null;
        bool _isGetConfig = false;

        #region 蓝藻控件定义
        /*监测*/
        RadButtonElement btnRough = new RadButtonElement("自动判识");
        RadDropDownButtonElement dbtnGenarate = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnInteractive = new RadDropDownButtonElement();
        /*日常业务*/
        RadButtonElement btnCoverDegree = new RadButtonElement("像元覆盖度计算");
        RadDropDownButtonElement dbtnOrdinaryImage = new RadDropDownButtonElement();     //专题产品
        RadButtonElement btnStatArea = new RadButtonElement("面积统计");          //面积统计
        RadDropDownButtonElement dbtnOrdinaryGenarate = new RadDropDownButtonElement();
        /*灾情事件*/
        RadDropDownButtonElement dbtnDisasterImage = new RadDropDownButtonElement();     //专题产品
        RadDropDownButtonElement dbtnDisasterArea = new RadDropDownButtonElement();      //面积统计
        RadButtonElement btnRateCurrentRegion = new RadButtonElement("覆盖度统计");      //覆盖度统计
        RadDropDownButtonElement dbtnDisasterGenarate = new RadDropDownButtonElement();
        /*频次统计*/
        RadDropDownButtonElement dbtnCycImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnCycStat = new RadDropDownButtonElement();
        RadButtonElement btnCoverAFreq = new RadButtonElement("基于覆盖面积");
        /*其他*/
        RadButtonElement btnAnimationRegion = new RadButtonElement("当前区域");        //动画
        RadButtonElement btnAnimationCustom = new RadButtonElement("自定义");
        /*浒苔监测*/
        RadDropDownButtonElement dbtnEnteromorphaMImg = new RadDropDownButtonElement();

        /*关闭*/
        RadButtonElement btnClose = new RadButtonElement("关闭蓝藻监测\n分析视图");
        /*端元值设置*/
        RadDropDownButtonElement dbtnSetting = new RadDropDownButtonElement();
        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        #endregion

        public UITabBag()
        {
            InitializeComponent();
            CreateRibbonBar();
        }

        #region CreateUI

        private void CreateRibbonBar()
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "BAGConfig.xml"))
            {
                _configDoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "BAGConfig.xml");
            }
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "蓝藻监测专题";
            _tab.Text = "蓝藻监测专题";
            _tab.Name = "蓝藻监测专题";
            _bar.CommandTabs.Add(_tab);
            CreateCheckGroup();
            CreateOrdinaryBusinessGroup();
            CreateDisasterGroup();
            CreateFreqStatic();
            CreateAnimationGroup();
            CreateEnteromorphaGroup();
            btnToDb.TextAlignment = ContentAlignment.BottomCenter;
            btnToDb.ImageAlignment = ContentAlignment.TopCenter;
            btnToDb.Click += new EventHandler(btnToDb_Click);
            RadRibbonBarGroup rbgToDb = new RadRibbonBarGroup();
            rbgToDb.Text = "产品入库";
            rbgToDb.Items.Add(btnToDb);
            _tab.Items.Add(rbgToDb);
            Controls.Add(_bar);
        }

        private void CreateEnteromorphaGroup()
        {
            RadRibbonBarGroup rbgEnteromorpha = new RadRibbonBarGroup();
            rbgEnteromorpha.Text = "浒苔监测";
            dbtnEnteromorphaMImg.Text = "专题产品";
            dbtnEnteromorphaMImg.ImageAlignment = ContentAlignment.TopCenter;
            dbtnEnteromorphaMImg.TextAlignment = ContentAlignment.BottomCenter;
            dbtnEnteromorphaMImg.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem mniMulImg = new RadMenuItem("多通道合成图");
            mniMulImg.Click += new EventHandler(mniMulImg_Click);
            dbtnEnteromorphaMImg.Items.Add(mniMulImg);
            RadMenuItem mniBinImg = new RadMenuItem("监测示意图");
            mniBinImg.Click += new EventHandler(mniBinImg_Click);
            dbtnEnteromorphaMImg.Items.Add(mniBinImg);
            rbgEnteromorpha.Items.Add(dbtnEnteromorphaMImg);
            _tab.Items.Add(rbgEnteromorpha);
        }

        private void CreateCheckGroup()
        {
            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            rbgCheck.Text = "监测";
            btnRough.ImageAlignment = ContentAlignment.TopCenter;
            btnRough.TextAlignment = ContentAlignment.BottomCenter;
            btnRough.Click += new EventHandler(btnRough_Click);
            rbgCheck.Items.Add(btnRough);
            dbtnInteractive.Text = "交互判识";
            dbtnInteractive.ImageAlignment = ContentAlignment.TopCenter;
            dbtnInteractive.TextAlignment = ContentAlignment.BottomCenter;
            dbtnInteractive.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem mniInterBag = new RadMenuItem("蓝藻");
            mniInterBag.Click += new EventHandler(btnInteractive_Click);
            dbtnInteractive.Items.Add(mniInterBag);
            RadMenuItem mniInterClm = new RadMenuItem("云");
            mniInterClm.Click += new EventHandler(mniInterClm_Click);
            dbtnInteractive.Items.Add(mniInterClm);

            RadMenuItem mniThAnlysisBag = new RadMenuItem("阈值分析");
            mniThAnlysisBag.Click += new EventHandler(mniThAnlysisBag_Click);
            dbtnInteractive.Items.Add(mniThAnlysisBag);

            rbgCheck.Items.Add(dbtnInteractive);
            dbtnGenarate.Text = "快速生成";
            dbtnGenarate.ImageAlignment = ContentAlignment.TopCenter;
            dbtnGenarate.TextAlignment = ContentAlignment.BottomCenter;
            dbtnGenarate.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem roughProGenerate = new RadMenuItem("自动判识+专题产品");
            roughProGenerate.Click += new EventHandler(roughProGenerate_Click);
            dbtnGenarate.Items.Add(roughProGenerate);
            RadMenuItem proGenerate = new RadMenuItem("专题产品");
            proGenerate.Click += new EventHandler(proGenerate_Click);
            dbtnGenarate.Items.Add(proGenerate);
            rbgCheck.Items.Add(dbtnGenarate);
            _tab.Items.Add(rbgCheck);
        }

        private void CreateOrdinaryBusinessGroup()
        {
            RadRibbonBarGroup rbgOrdinary = new RadRibbonBarGroup();
            rbgOrdinary.Text = "日常业务";

            dbtnOrdinaryImage.Text = "专题产品";
            dbtnOrdinaryImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnOrdinaryImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnOrdinaryImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiMulImage = new RadMenuItem("多通道合成图");
            mhiMulImage.Click += new EventHandler(mhiMulImage_Click);
            dbtnOrdinaryImage.Items.Add(mhiMulImage);
            RadMenuItem mhiBinImg = new RadMenuItem("二值图");
            mhiBinImg.Click += new EventHandler(mhiBinImgCurrent_Click);
            //dbtnOrdinaryImage.Items.Add(mhiBinImg);
            RadMenuItem mhiTreGradeImage = new RadMenuItem("强度图（三级）");
            mhiTreGradeImage.Click += new EventHandler(mhiTreGradeImage_Click);
            dbtnOrdinaryImage.Items.Add(mhiTreGradeImage);
            rbgOrdinary.Items.Add(dbtnOrdinaryImage);

            btnStatArea.Text = "面积统计";
            btnStatArea.ImageAlignment = ContentAlignment.TopCenter;
            btnStatArea.TextAlignment = ContentAlignment.BottomCenter;
            btnStatArea.Click += new EventHandler(mniTreDegreeArea_Click);
            rbgOrdinary.Items.Add(btnStatArea);

            dbtnOrdinaryGenarate.Text = "快速生成";
            dbtnOrdinaryGenarate.ImageAlignment = ContentAlignment.TopCenter;
            dbtnOrdinaryGenarate.TextAlignment = ContentAlignment.BottomCenter;
            dbtnOrdinaryGenarate.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem mniOrdinaryGenerate = new RadMenuItem("自动判识+专题产品");
            mniOrdinaryGenerate.Click += new EventHandler(mniOrdinaryGenerate_Click);
            dbtnOrdinaryGenarate.Items.Add(mniOrdinaryGenerate);
            RadMenuItem mniOrdLayoutGenerate = new RadMenuItem("专题产品");
            mniOrdLayoutGenerate.Click += new EventHandler(mniOrdLayoutGenerate_Click);
            dbtnOrdinaryGenarate.Items.Add(mniOrdLayoutGenerate);
            rbgOrdinary.Items.Add(dbtnOrdinaryGenarate);
            _tab.Items.Add(rbgOrdinary);
        }

        private void CreateDisasterGroup()
        {
            RadRibbonBarGroup rbgDisaster = new RadRibbonBarGroup();
            rbgDisaster.Text = "灾情事件";
            dbtnDisasterImage.Text = "专题产品";
            dbtnDisasterImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnDisasterImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDisasterImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiDMulImage = new RadMenuItem("多通道合成图");
            mhiDMulImage.Click += new EventHandler(mhiMulImage_Click);
            dbtnDisasterImage.Items.Add(mhiDMulImage);
            RadMenuItem mniOMulImage = new RadMenuItem("多通道合成图（原始分辨率）");
            mniOMulImage.Click += new EventHandler(mniOMulImage_Click);
            //dbtnDisasterGenarate.Items.Add(mniOMulImage);
            RadMenuItem mhiDBinImg = new RadMenuItem("二值图（当前区域）");
            mhiDBinImg.Click += new EventHandler(mhiBinImgCurrent_Click);
            dbtnDisasterImage.Items.Add(mhiDBinImg);
            RadMenuItem mhiDBinImgCustom = new RadMenuItem("二值图（自定义）");
            mhiDBinImgCustom.Click += new EventHandler(mhiBinImgCustom_Click);
            dbtnDisasterImage.Items.Add(mhiDBinImgCustom);
            RadMenuItem mhiTreGradeImage = new RadMenuItem("强度图（三级）");
            mhiTreGradeImage.Click += new EventHandler(mhiTreGradeImage_Click);
            dbtnDisasterImage.Items.Add(mhiTreGradeImage);
            RadMenuItem mhiTenGradeImage = new RadMenuItem("强度图（十级）");
            mhiTenGradeImage.Click += new EventHandler(mhiTenGradeImage_Click);
            dbtnDisasterImage.Items.Add(mhiTenGradeImage);
            RadMenuItem mhiCoverDegreeImage = new RadMenuItem("覆盖度图");
            mhiCoverDegreeImage.Click += new EventHandler(mhiCoverDegreeImage_Click);
            dbtnDisasterImage.Items.Add(mhiCoverDegreeImage);
            rbgDisaster.Items.Add(dbtnDisasterImage);

            dbtnDisasterArea.Text = "面积统计";
            dbtnDisasterArea.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnDisasterArea.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDisasterArea.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniDCurrentArea = new RadMenuItem("当前区域");
            mniDCurrentArea.Click += new EventHandler(mniCurrentArea_Click);
            dbtnDisasterArea.Items.Add(mniDCurrentArea);
            RadMenuItem mniTreDegreeArea = new RadMenuItem("基于强度（三级）");
            mniTreDegreeArea.Click += new EventHandler(mniTreDegreeArea_Click);
            dbtnDisasterArea.Items.Add(mniTreDegreeArea);
            RadMenuItem mniTenDegreeArea = new RadMenuItem("基于强度（十级）");
            mniTenDegreeArea.Click += new EventHandler(mniTenDegreeArea_Click);
            dbtnDisasterArea.Items.Add(mniTenDegreeArea);
            RadMenuItem mniDCoverDArea = new RadMenuItem("基于强度（自定义）");
            mniDCoverDArea.Click += new EventHandler(mniCoverDArea_Click);
            dbtnDisasterArea.Items.Add(mniDCoverDArea);
            RadMenuItem mniCustomArea = new RadMenuItem("自定义");
            mniCustomArea.Click += new EventHandler(mniCustomArea_Click);
            dbtnDisasterArea.Items.Add(mniCustomArea);
            rbgDisaster.Items.Add(dbtnDisasterArea);
            btnRateCurrentRegion.TextAlignment = ContentAlignment.BottomCenter;
            btnRateCurrentRegion.ImageAlignment = ContentAlignment.TopCenter;
            btnRateCurrentRegion.Click += new EventHandler(btnRateCurrentRegion_Click);
            rbgDisaster.Items.Add(btnRateCurrentRegion);

            dbtnDisasterGenarate.Text = "快速生成";
            dbtnDisasterGenarate.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDisasterGenarate.TextAlignment = ContentAlignment.BottomCenter;
            dbtnDisasterGenarate.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem mniDisasterGenarate = new RadMenuItem("自动判识+专题产品");
            mniDisasterGenarate.Click += new EventHandler(mniDisasterGenarate_Click);
            dbtnDisasterGenarate.Items.Add(mniDisasterGenarate);
            RadMenuItem mniDstLayoutGenarate = new RadMenuItem("专题产品");
            mniDstLayoutGenarate.Click += new EventHandler(mniDstLayoutGenarate_Click);
            dbtnDisasterGenarate.Items.Add(mniDstLayoutGenarate);
            rbgDisaster.Items.Add(dbtnDisasterGenarate);
            _tab.Items.Add(rbgDisaster);
        }

        private void CreateFreqStatic()
        {
            RadRibbonBarGroup rbgFreqStatic = new RadRibbonBarGroup();
            rbgFreqStatic.Text = "周期业务";
            dbtnCycImage.Text = "专题产品";
            dbtnCycImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnCycImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCycImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniCurrentFreq = new RadMenuItem("频次统计（当前区域）");
            mniCurrentFreq.Click += new EventHandler(btnCurrentRegionFreq_Click);
            dbtnCycImage.Items.Add(mniCurrentFreq);
            RadMenuItem mniCoverDFreq = new RadMenuItem("频次统计（基于覆盖度）");
            mniCoverDFreq.Click += new EventHandler(btnCoverDFreq_Click);
            dbtnCycImage.Items.Add(mniCoverDFreq);
            RadMenuItem mniCurrentOldFreq = new RadMenuItem("频次统计（区域老模板）");
            mniCurrentOldFreq.Click += new EventHandler(btnmniCurrentOldFreq_Click);
            dbtnCycImage.Items.Add(mniCurrentOldFreq);
            RadMenuItem mniCoverDOldFreq = new RadMenuItem("频次统计（覆盖度老模板）");
            mniCoverDOldFreq.Click += new EventHandler(btnCoverDOldFreq_Click);
            dbtnCycImage.Items.Add(mniCoverDOldFreq);
            RadMenuItem mniTimeFreq = new RadMenuItem("频次均值统计（长序列）");
            mniTimeFreq.Click += new EventHandler(mniTimeFreq_Click);
            dbtnCycImage.Items.Add(mniTimeFreq);
            RadMenuItem mniTimeFreqImage = new RadMenuItem("频次均值图");
            mniTimeFreqImage.Click += new EventHandler(mniTimeFreqImage_Click);
            dbtnCycImage.Items.Add(mniTimeFreqImage);
            rbgFreqStatic.Items.Add(dbtnCycImage);
            dbtnCycStat.Text = "统计分析";
            dbtnCycStat.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnCycStat.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCycStat.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniCoverAFreq = new RadMenuItem("频次统计（基于覆盖面积）");
            mniCoverAFreq.Click += new EventHandler(btnCoverAFreq_Click);
            dbtnCycStat.Items.Add(mniCoverAFreq);
            rbgFreqStatic.Items.Add(dbtnCycStat);
            _tab.Items.Add(rbgFreqStatic);
        }

        private void CreateAnimationGroup()
        {
            RadRibbonBarGroup rbgAnimation = new RadRibbonBarGroup();
            rbgAnimation.Text = "其他业务";
            btnAnimationRegion.ImageAlignment = ContentAlignment.TopCenter;
            btnAnimationRegion.TextAlignment = ContentAlignment.BottomCenter;
            btnAnimationRegion.Click += new EventHandler(btnAnimationRegion_Click);
            rbgAnimation.Items.Add(btnAnimationRegion);
            btnAnimationCustom.TextAlignment = ContentAlignment.BottomCenter;
            btnAnimationCustom.ImageAlignment = ContentAlignment.TopCenter;
            btnAnimationCustom.Click += new EventHandler(btnAnimationCustom_Click);
            rbgAnimation.Items.Add(btnAnimationCustom);
            btnCoverDegree.Click += new EventHandler(btnCoverDegree_Click);
            btnCoverDegree.ImageAlignment = ContentAlignment.TopCenter;
            btnCoverDegree.TextAlignment = ContentAlignment.BottomCenter;
            rbgAnimation.Items.Add(btnCoverDegree);
            _tab.Items.Add(rbgAnimation);
        }

        private void CreateCloseGroup()
        {
            RadRibbonBarGroup rbgClose = new RadRibbonBarGroup();
            rbgClose.Text = "关闭";
            btnClose.TextAlignment = ContentAlignment.BottomCenter;
            btnClose.ImageAlignment = ContentAlignment.TopCenter;
            btnClose.Click += new EventHandler(btnClose_Click);
            rbgClose.Items.Add(btnClose);
            _tab.Items.Add(rbgClose);
        }

        private void CreateSettingGroup()
        {
            AvgNDVISetter.RegistButton(dbtnSetting, _tab, _session);
        }

        #endregion

        #region 事件
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
        /// 人机交互
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnInteractive_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExecute(6602);
        }

        void mniInterClm_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0CLM");
            ICommand cmd = _session.CommandEnvironment.Get(6602);
            if (cmd != null)
                cmd.Execute();
        }

        void mniThAnlysisBag_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("THAN");
            ICommand cmd = _session.CommandEnvironment.Get(6602);
            if (cmd != null)
                cmd.Execute();
        }

        /// <summary>
        /// 粗判 + 专题产品快速生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void roughProGenerate_Click(object sender, EventArgs e)
        {
            //open contxtmessage window
            AutoGenretateProvider.AutoGenrate(_session, null);
        }

        /// <summary>
        /// 专题产品快速生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void proGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "BACD", CustomVarSetterHandler, null);
        }

        //日常业务快速生成
        void mniOrdinaryGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, null, null, "Ord");
        }

        //日常业务专题图快速生成
        void mniOrdLayoutGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "0IMG", null, "Ord");
        }

        //灾情事件粗判+专题图快速生成
        void mniDisasterGenarate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, null, null, "Dis");
        }

        //灾情事件专题图快速生成
        void mniDstLayoutGenarate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "BACD", null, "Dis");
        }

        //覆盖度计算
        void btnCoverDegree_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("BPCD");
            GetCommandAndExecute(6602);
        }

        //多通道合成图
        void mhiMulImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MCSI");
            ms.DoAutoExtract(false);
        }

        void mniOMulImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "OMCS");
            ms.DoAutoExtract(false);
        }

        //覆盖度专题图
        void mhiCoverDegreeImage_Click(object sender, EventArgs e)
        {
            //if (MIFCommAnalysis.CreateThemeGraphy(_session, "PCDI", "PCDI", "BPCD", "蓝藻覆盖度专题图", false, true) == null)
            //    return;
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "PCDI");
            ms.DoAutoExtract(false);
        }

        //三级强度图
        void mhiTreGradeImage_Click(object sender, EventArgs e)
        {
            //if (MIFCommAnalysis.CreateThemeGraphy(_session, "0QD3", "0QD3", "BPCD", "蓝藻强度专题图（三级）", false, true) == null)
            //    return;
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0QD3");
            ms.DoAutoExtract(false);
        }

        //十级强度图
        void mhiTenGradeImage_Click(object sender, EventArgs e)
        {
            //if (MIFCommAnalysis.CreateThemeGraphy(_session, "QD10", "QD10", "BPCD", "蓝藻强度专题图（十级）", false, true) == null)
            //    return;
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "QD10");
            ms.DoAutoExtract(false);
        }

        /// <summary>
        /// 自定义二值图按钮事件
        /// </summary>
        void mhiBinImgCustom_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "CSDI", "BAGDBLV", "DBLV", "蓝藻监测专题图", true, true) == null)
                return;
        }

        void mhiBinImgCurrent_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0SDI");
            ms.DoAutoExtract(false);
        }

        void mniTimeFreqImage_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "TFRI");
            ms.DoAutoExtract(false);
        }

        #region 统计分析

        //当前区域覆盖度统计
        void btnRateCurrentRegion_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("BACD");
            GetCommandAndExecute(6602);
        }

        //自定义覆盖度统计
        void btnRateCustom_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("BACD");
            GetCommandAndExecute(6602);
        }

        void dbtnCoverDArea_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "BCDA", "BCDA", "BCDA", null, false, false) == null)
                return;
        }

        //当前区域面积统计
        void mniCurrentArea_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "CCAR");
            ms.DoAutoExtract(true);
        }

        //自定义面积统计
        void mniCustomArea_Click(object sender, EventArgs e)
        {
            if (!_isGetConfig)
                _aoiTemplateList = GetAOITemplateList();
            if (_aoiTemplateList != null && _aoiTemplateList.Count != 0)
            {
                if (MIFCommAnalysis.AreaStat(_session, "CCCA", "", true, true, _aoiTemplateList) == null)
                    return;
            }
            else
            {
                if (MIFCommAnalysis.AreaStat(_session, "CCCA", "", true, true) == null)
                    return;
            }
        }

        //自定义强度面积统计
        void mniCoverDArea_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("BCDA");
            GetCommandAndExecute(6602);
        }

        //三级强度面积统计
        void mniTreDegreeArea_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("BCDA");
            if (SetFileArg(_session, ms.ActiveMonitoringSubProduct.ArgumentProvider, "SelectedPrimaryFiles"))
            {
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "CDAH");
                ms.DoAutoExtract(true);
            }
        }

        //十级强度面积统计
        void mniTenDegreeArea_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("BCDA");
            if (SetFileArg(_session, ms.ActiveMonitoringSubProduct.ArgumentProvider, "SelectedPrimaryFiles"))
            {
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "CDAE");
                ms.DoAutoExtract(true);
            }
        }

        //自定义频次统计
        void btnCustomFreq_Click(object sender, EventArgs e)
        {
            if (!_isGetConfig)
                _aoiTemplateList = GetAOITemplateList();
            if (_aoiTemplateList != null && _aoiTemplateList.Count != 0)
            {
                if (MIFCommAnalysis.TimeStat(_session, "CFRI", "", "DBLV", "蓝藻频次监测专题图", true, true, _aoiTemplateList) == null)
                    return;
            }
            else
            {
                if (MIFCommAnalysis.TimeStat(_session, "CFRI", "", "DBLV", "蓝藻频次监测专题图", true, true) == null)
                    return;
            }
        }

        //基于覆盖面积频次
        void btnCoverAFreq_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("BCAF");
            GetCommandAndExecute(6602);
        }

        //基于覆盖度频次统计
        void btnCoverDFreq_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("BCDF");
            GetCommandAndExecute(6602);
            CreateThemeGraphyBase(_session, "BCDF", "BCDF", "蓝藻频次监测专题图", false, "BAGFREQ");
        }

        //基于覆盖度频次统计(老模板)
        void btnCoverDOldFreq_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("BCDO");
            GetCommandAndExecute(6602);
            CreateThemeGraphyBase(_session, "BCDF", "BCDO", "蓝藻频次监测专题图_老模板", false, "BAGFREQ");
        }

        //长序列频次统计
        void mniTimeFreq_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("TFRV");
            GetCommandAndExecute(6602);
        }

        //当前区域频次统计
        void btnCurrentRegionFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "FREQ", "", "DBLV", "蓝藻频次监测专题图", false, true) == null)
                return;
        }
            
        //当前区域频次统计(老模板)
        void btnmniCurrentOldFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "FREO", "FREO", "BAGFREQ", "DBLV", "蓝藻频次监测专题图_老模板", false, true) == null)
                return;
        }

        #endregion

        #region 浒苔监测

        void mniMulImg_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "EMCS");
            ms.DoAutoExtract(false);
        }

        void mniBinImg_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "ESDI");
            ms.DoAutoExtract(false);
        }

        #endregion

        //自定义动画
        void btnAnimationCustom_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.CreatAVI(_session, "template:蓝藻动画专题图,动画展示专题图,BAG,CMED");
        }
        //当前区域动画
        void btnAnimationRegion_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.CreatAVI(_session, "template:蓝藻动画专题图,动画展示专题图,BAG,MEDI");
        }

        //产品入库
        void btnToDb_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(39002);
            if (cmd != null)
                cmd.Execute();
        }

        //关闭产品
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
            cmd.Execute("BAG");
        }

        private void GetCommandAndExecute(int cmdID, string template)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute(template);
        }

        private void CreateThemeGraphyBase(ISmartSession session, string algorithmName, string outFileIdentify, string templateName, bool isCustom, string colorTableName)
        {
            IMonitoringSubProduct msp = (session.MonitoringSession as IMonitoringSession).ActiveMonitoringSubProduct;
            if (msp == null)
                return;
            IThemeGraphGenerator tgg = (session.MonitoringSession as IMonitoringSession).ThemeGraphGenerator;
            msp.ArgumentProvider.SetArg("ThemeGraphyGenerator", tgg);
            msp.ArgumentProvider.SetArg("OutFileIdentify", outFileIdentify);
            msp.ArgumentProvider.SetArg("ThemeGraphTemplateName", templateName);
            msp.ArgumentProvider.SetArg("colortablename", "colortablename=" + colorTableName);
        }

        private Dictionary<string, string> GetAOITemplateList()
        {
            Dictionary<string, string> aoiTemplateList = new Dictionary<string, string>();
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "BAGConfig.xml"))
            {
                return null;
            }
            XDocument doc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "BAGConfig.xml");
            if (doc == null)
            {
                _isGetConfig = true;
                return null;
            }
            XElement root = doc.Element("DataSources");
            if (root == null)
            {
                _isGetConfig = true;
                return null;
            }
            foreach (XElement item in root.Elements("DataSource"))
            {
                string nameValue = item.Attribute("name").Value;
                string regionShapeFile = item.Attribute("regionShapeFile").Value;
                aoiTemplateList.Add(nameValue, regionShapeFile);
            }
            _isGetConfig = true;
            return aoiTemplateList;
        }

        #region IUITabProvider

        public object Content
        {
            get { return _tab; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            this._session = session;
            CreateSettingGroup();
            CreateqQuickReportRegionButton();
            CreateCloseGroup();
            SetImage();
        }

        private void CreateqQuickReportRegionButton()
        {
            QuickReportFactory.RegistToButton("BAG", _tab, _session);
        }

        private void SetImage()
        {
            btnRough.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            dbtnGenarate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnInteractive.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            btnCoverDegree.Image = _session.UIFrameworkHelper.GetImage("system:coverDegree_current.png");
            btnRateCurrentRegion.Image = _session.UIFrameworkHelper.GetImage("system:coverDegree_current.png");
            dbtnCycImage.Image = _session.UIFrameworkHelper.GetImage("system:TimesGraph.png");
            dbtnCycStat.Image = _session.UIFrameworkHelper.GetImage("system:TimesByArea.png");
            btnAnimationRegion.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            btnAnimationCustom.Image = _session.UIFrameworkHelper.GetImage("system:customVedio.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
            btnStatArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            dbtnOrdinaryImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            dbtnDisasterArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            dbtnDisasterImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            dbtnDisasterGenarate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnOrdinaryGenarate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnEnteromorphaMImg.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            dbtnSetting.Image = _session.UIFrameworkHelper.GetImage("system:settings.png");
        }

        public void UpdateStatus()
        {
        }

        #endregion

        private void CustomVarSetterHandler(string[] dblvFileNames, string beginSubprod, IContextEnvironment contextEnv)
        {
            string ndviFileName = Path.GetFileName(dblvFileNames[0]);
            if (string.IsNullOrEmpty(ndviFileName))
                return;
            string dblvName = Path.GetDirectoryName(dblvFileNames[0]) + "\\" + ndviFileName.Replace("NDVI", "DBLV");
            if (File.Exists(dblvName))
                contextEnv.PutContextVar("DBLV", dblvName);
        }

        public bool SetSelectedPrimaryFiles(ISmartSession session, IArgumentProvider iArgumentProvider, string outFileIdentify)
        {
            IWorkspace wks = (session.MonitoringSession as IMonitoringSession).Workspace;
            if (wks == null)
                return false;
            ICatalog catalog = wks.ActiveCatalog;
            if (catalog == null)
                return false;
            string[] fnames = catalog.GetSelectedFiles();
            if (fnames == null || fnames.Length == 0)
                return false;
            iArgumentProvider.SetArg("SelectedPrimaryFiles", fnames);
            iArgumentProvider.SetArg("OutFileIdentify", outFileIdentify);
            return true;
        }

        public bool SetFileArg(ISmartSession session, IArgumentProvider iArgumentProvider, string argName)
        {
            IWorkspace wks = (session.MonitoringSession as IMonitoringSession).Workspace;
            if (wks == null)
                return false;
            ICatalog catalog = wks.ActiveCatalog;
            if (catalog == null)
                return false;
            string[] fnames = catalog.GetSelectedFiles();
            if (fnames == null || fnames.Length == 0)
                return false;
            string selectFileName = Path.GetFileName(fnames[0]);
            if (selectFileName.Contains("BPCD"))
            {
                iArgumentProvider.SetArg(argName, fnames[0]);
                return true;
            }
            string bpcdName = Path.GetDirectoryName(fnames[0]) + "\\" + selectFileName.Replace("DBLV", "BPCD");
            if (File.Exists(bpcdName))
                iArgumentProvider.SetArg(argName, bpcdName);
            return true;
        }

    }
}
