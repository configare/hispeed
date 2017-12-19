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

namespace GeoDo.RSS.UI.AddIn.CMA
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
        RadButtonElement _btnRough = new RadButtonElement("粗判");
        RadDropDownButtonElement btnSave = new RadDropDownButtonElement();
        RadButtonElement btnInteractive = new RadButtonElement("交互判识");
        /*特征信息提取*/
        RadButtonElement btnCoverDegree = new RadButtonElement("像元覆盖度计算");
        /*专题产品*/
        RadButtonElement btnMulImage = new RadButtonElement("多通道合成图");
        RadDropDownButtonElement dbtnbinImage = new RadDropDownButtonElement();     //二值图
        RadDropDownButtonElement dbtnStrenImage = new RadDropDownButtonElement();
        RadButtonElement btnCDImage = new RadButtonElement("覆盖度图");
        /*覆盖度统计*/
        RadButtonElement btnRateCurrentRegion = new RadButtonElement("整个湖区");
        RadButtonElement btnRateCustom = new RadButtonElement("分湖区");
        /*面积统计*/
        RadButtonElement btnCurrentRegionArea = new RadButtonElement("当前区域");
        RadButtonElement btnCustomArea = new RadButtonElement("自定义");
        RadButtonElement dbtnCoverDArea = new RadButtonElement("基于强度/覆盖度");
        /*频次统计*/
        RadButtonElement btnCurrentRegionFreq = new RadButtonElement("当前区域");
        RadButtonElement btnCustomFreq = new RadButtonElement("自定义");
        RadButtonElement btnCoverDFreq = new RadButtonElement("基于覆盖度");
        RadButtonElement btnCoverAFreq = new RadButtonElement("基于覆盖面积");
        /*动画*/
        RadButtonElement btnAnimationRegion = new RadButtonElement("当前区域");
        RadButtonElement btnAnimationCustom = new RadButtonElement("自定义");
        /*关闭*/
        RadButtonElement _btnClose = new RadButtonElement("关闭蓝藻监测\n分析视图");
        /*设置*/
        RadDropDownListElement _dltLakeSetting = new RadDropDownListElement();
        RadDropDownListElement _dltLakeRange = new RadDropDownListElement();
        RadLabelElement lblDataSource = new RadLabelElement();
        RadLabelElement lblLakeRange = new RadLabelElement();
        RadButtonElement btnClose = new RadButtonElement("关闭蓝藻监测\n分析视图");
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
            CreateQuantifyGroup();
            CreateProductGroup();
            CreateCoverDegreeGroup();
            CreateAreaStaticGroup();
            CreateFreqStatic();
            CreateAnimationGroup();

            btnToDb.TextAlignment = ContentAlignment.BottomCenter;
            btnToDb.ImageAlignment = ContentAlignment.TopCenter;
            btnToDb.Click += new EventHandler(btnToDb_Click);
            RadRibbonBarGroup rbgToDb = new RadRibbonBarGroup();
            rbgToDb.Text = "产品入库";
            rbgToDb.Items.Add(btnToDb);
            _tab.Items.Add(rbgToDb);

            CreateCloseGroup();
            CreateSettingGroup();

            Controls.Add(_bar);
        }

        private void CreateCheckGroup()
        {
            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            rbgCheck.Text = "监测";
            _btnRough.Click += new EventHandler(btnRough_Click);
            rbgCheck.Items.Add(_btnRough);
            btnInteractive.Click += new EventHandler(btnInteractive_Click);
            rbgCheck.Items.Add(btnInteractive);
            foreach (RadButtonElement item in rbgCheck.Items)
            {
                item.ImageAlignment = ContentAlignment.TopCenter;
                item.TextAlignment = ContentAlignment.BottomCenter;
            }
            btnSave.Text = "快速生成";
            btnSave.ImageAlignment = ContentAlignment.TopCenter;
            btnSave.TextAlignment = ContentAlignment.BottomCenter;
            btnSave.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem roughProGenerate = new RadMenuItem("粗判+专题产品");
            roughProGenerate.Click += new EventHandler(roughProGenerate_Click);
            btnSave.Items.Add(roughProGenerate);
            RadMenuItem proGenerate = new RadMenuItem("专题产品");
            proGenerate.Click += new EventHandler(proGenerate_Click);
            btnSave.Items.Add(proGenerate);
            rbgCheck.Items.Add(btnSave);
            _tab.Items.Add(rbgCheck);
        }

        private void CreateQuantifyGroup()
        {
            RadRibbonBarGroup rbgQuantify = new RadRibbonBarGroup();
            rbgQuantify.Text = "特征信息提取";
            btnCoverDegree.Click += new EventHandler(btnCoverDegree_Click);
            btnCoverDegree.ImageAlignment = ContentAlignment.TopCenter;
            btnCoverDegree.TextAlignment = ContentAlignment.BottomCenter;
            rbgQuantify.Items.Add(btnCoverDegree);
            _tab.Items.Add(rbgQuantify);
        }

        private void CreateProductGroup()
        {
            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "专题产品";
            btnMulImage.ImageAlignment = ContentAlignment.TopCenter;
            btnMulImage.TextAlignment = ContentAlignment.BottomCenter;
            btnMulImage.Click += new EventHandler(btnMulImage_Click);
            rbgProduct.Items.Add(btnMulImage);
            dbtnbinImage.Text = "二值图";
            dbtnbinImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnbinImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnbinImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiBinImgCurrent = new RadMenuItem("当前区域");
            mhiBinImgCurrent.Click += new EventHandler(mhiBinImgCurrent_Click);
            dbtnbinImage.Items.Add(mhiBinImgCurrent);
            RadMenuItem mhiBinImgCustom = new RadMenuItem("自定义");
            mhiBinImgCustom.Click += new EventHandler(mhiBinImgCustom_Click);
            dbtnbinImage.Items.Add(mhiBinImgCustom);
            rbgProduct.Items.Add(dbtnbinImage);
            dbtnStrenImage.Text = "强度图";
            dbtnStrenImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnStrenImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnStrenImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiTenGradeImage = new RadMenuItem("强度图（10级）");
            mhiTenGradeImage.Click += new EventHandler(mhiTenGradeImage_Click);
            dbtnStrenImage.Items.Add(mhiTenGradeImage);
            RadMenuItem mhiTreGradeImage = new RadMenuItem("强度图（3级）");
            mhiTreGradeImage.Click += new EventHandler(mhiTreGradeImage_Click);
            dbtnStrenImage.Items.Add(mhiTreGradeImage);
            rbgProduct.Items.Add(dbtnStrenImage);
            btnCDImage.TextAlignment = ContentAlignment.BottomCenter;
            btnCDImage.ImageAlignment = ContentAlignment.TopCenter;
            btnCDImage.Click += new EventHandler(btnCDImage_Click);
            rbgProduct.Items.Add(btnCDImage);
            _tab.Items.Add(rbgProduct);
        }

        private void CreateCoverDegreeGroup()
        {
            RadRibbonBarGroup rbgCoverRate = new RadRibbonBarGroup();
            rbgCoverRate.Text = "覆盖度统计";
            btnRateCurrentRegion.Click += new EventHandler(btnRateCurrentRegion_Click);
            rbgCoverRate.Items.Add(btnRateCurrentRegion);
            btnRateCustom.Click += new EventHandler(btnRateCustom_Click);
            //rbgCoverRate.Items.Add(btnRateCustom);
            foreach (RadButtonElement item in rbgCoverRate.Items)
            {
                item.ImageAlignment = ContentAlignment.TopCenter;
                item.TextAlignment = ContentAlignment.BottomCenter;
            }
            _tab.Items.Add(rbgCoverRate);
        }

        private void CreateAreaStaticGroup()
        {
            RadRibbonBarGroup rbgAreaStatic = new RadRibbonBarGroup();
            rbgAreaStatic.Text = "面积统计";
            btnCurrentRegionArea.TextAlignment = ContentAlignment.BottomCenter;
            btnCurrentRegionArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCurrentRegionArea.Click += new EventHandler(btnCurrentRegionArea_Click);
            rbgAreaStatic.Items.Add(btnCurrentRegionArea);
            //dbtnCoverDArea.Text = "基于强度/覆盖度";
            dbtnCoverDArea.TextAlignment = ContentAlignment.BottomCenter;
            dbtnCoverDArea.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCoverDArea.Click += new EventHandler(dbtnCoverDArea_Click);
            //dbtnCoverDArea.ArrowPosition = DropDownButtonArrowPosition.Right;
            //RadMenuItem mhiTreGradeArea = new RadMenuItem("强度（三级）");
            //mhiTreGradeArea.TextAlignment = ContentAlignment.BottomCenter;
            //mhiTreGradeArea.Click += new EventHandler(mhiTreGradeArea_Click);
            //RadMenuItem mhiTenGradeArea = new RadMenuItem("强度（十级）");
            //mhiTreGradeArea.TextAlignment = ContentAlignment.BottomCenter;
            //mhiTenGradeArea.Click += new EventHandler(mhiTenGradeArea_Click);
            //RadMenuItem mhiCustomArea = new RadMenuItem("自定义");
            //mhiCustomArea.Click += new EventHandler(mhiCustomArea_Click);
            //dbtnCoverDArea.Items.AddRange(mhiTreGradeArea, mhiTenGradeArea, mhiCustomArea);
            rbgAreaStatic.Items.Add(dbtnCoverDArea);
            btnCustomArea.TextAlignment = ContentAlignment.BottomCenter;
            btnCustomArea.ImageAlignment = ContentAlignment.TopCenter;
            btnCustomArea.Click += new EventHandler(btnCustomArea_Click);
            rbgAreaStatic.Items.Add(btnCustomArea);
            _tab.Items.Add(rbgAreaStatic);
        }

        private void CreateFreqStatic()
        {
            RadRibbonBarGroup rbgFreqStatic = new RadRibbonBarGroup();
            rbgFreqStatic.Text = "频次统计";
            btnCurrentRegionFreq.Click += new EventHandler(btnCurrentRegionFreq_Click);
            rbgFreqStatic.Items.Add(btnCurrentRegionFreq);
            btnCoverDFreq.Click += new EventHandler(btnCoverDFreq_Click);
            rbgFreqStatic.Items.Add(btnCoverDFreq);
            btnCoverAFreq.Click += new EventHandler(btnCoverAFreq_Click);
            rbgFreqStatic.Items.Add(btnCoverAFreq);
            btnCustomFreq.Click += new EventHandler(btnCustomFreq_Click);
            rbgFreqStatic.Items.Add(btnCustomFreq);
            foreach (RadButtonElement item in rbgFreqStatic.Items)
            {
                item.ImageAlignment = ContentAlignment.TopCenter;
                item.TextAlignment = ContentAlignment.BottomCenter;
            }
            _tab.Items.Add(rbgFreqStatic);
        }

        private void CreateAnimationGroup()
        {
            RadRibbonBarGroup rbgAnimation = new RadRibbonBarGroup();
            rbgAnimation.Text = "动画";
            btnAnimationRegion.ImageAlignment = ContentAlignment.TopCenter;
            btnAnimationRegion.TextAlignment = ContentAlignment.BottomCenter;
            btnAnimationRegion.Click += new EventHandler(btnAnimationRegion_Click);
            rbgAnimation.Items.Add(btnAnimationRegion);
            btnAnimationCustom.TextAlignment = ContentAlignment.BottomCenter;
            btnAnimationCustom.ImageAlignment = ContentAlignment.TopCenter;
            btnAnimationCustom.Click += new EventHandler(btnAnimationCustom_Click);
            rbgAnimation.Items.Add(btnAnimationCustom);
            _tab.Items.Add(rbgAnimation);
        }

        private void CreateCloseGroup()
        {
            RadRibbonBarGroup rbgClose = new RadRibbonBarGroup();
            rbgClose.Text = "关闭";
            _btnClose.TextAlignment = ContentAlignment.BottomCenter;
            _btnClose.ImageAlignment = ContentAlignment.TopCenter;
            _btnClose.Click += new EventHandler(btnClose_Click);
            rbgClose.Items.Add(_btnClose);
            _tab.Items.Add(rbgClose);
        }

        private void CreateSettingGroup()
        {
            RadRibbonBarGroup rbgSetting = new RadRibbonBarGroup();
            rbgSetting.Text = "设置";
            lblDataSource.Text = "活动数据源:";
            List<string> lakeList = GetlakeList();
            //if (lakeList != null && lakeList.Count() != 0)
            //{
            //    foreach (string item in lakeList)
            //        _dltLakeSetting.Items.Add(item);
            //}

            _dltLakeSetting.DropDownStyle = RadDropDownStyle.DropDownList;
            _dltLakeSetting.SelectedIndex = 0;
            _dltLakeSetting.DropDownMaxSize = new System.Drawing.Size(85, 200);
            _dltLakeSetting.DropDownMinSize = new System.Drawing.Size(80, 200);
            RadRibbonBarButtonGroup grouplakeSetting = new RadRibbonBarButtonGroup();
            RadItem[] items = new RadItem[] { lblDataSource, _dltLakeSetting };
            grouplakeSetting.Items.AddRange(_dltLakeSetting);
            grouplakeSetting.Orientation = Orientation.Horizontal;
            grouplakeSetting.Margin = new Padding(0, 6, 0, 0);
            grouplakeSetting.ShowBorder = false;

            RadRibbonBarButtonGroup group1 = new RadRibbonBarButtonGroup();
            group1.Items.AddRange(new RadItem[] {/* groupRomance,*/ grouplakeSetting });
            group1.Orientation = Orientation.Vertical;
            group1.ShowBorder = false;
            //lblLakeRange.Text = "设置区域：";
            //List<string>
            rbgSetting.Items.Add(group1);
            //_tab.Items.Add(rbgSetting);
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
            AutoGenretateProvider.AutoGenrate(_session, "BPCD",CustomVarSetterHandler);
        }

        //覆盖度计算
        void btnCoverDegree_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("BPCD");
            GetCommandAndExecute(6602);
        }

        //多通道合成图
        void btnMulImage_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:蓝藻多通道合成图,多通道合成图,BAG,MCSI");
        }
        //覆盖度专题图
        void btnCDImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "PCDI", "PCDI", "BPCD", "蓝藻覆盖度专题图", false, true) == null)
                return;
        }
        //三级强度图
        void mhiTreGradeImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0QD3", "0QD3", "BPCD", "蓝藻强度专题图（三级）", false, true) == null)
                return;
        }
        //十级强度图
        void mhiTenGradeImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "QD10", "QD10", "BPCD", "蓝藻强度专题图（十级）", false, true) == null)
                return;
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
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "BAGDBLV", "DBLV", "蓝藻监测专题图", false, true) == null)
                return;
        }

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
        void btnCurrentRegionArea_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.AreaStat(_session, "CCAR", "", false, true) == null)
                return;
        }

        //自定义面积统计
        void btnCustomArea_Click(object sender, EventArgs e)
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

        //覆盖度面积统计
        void mhiCustomArea_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("BCDA");
            GetCommandAndExecute(6602);
        }

        //十级强度面积统计
        void mhiTenGradeArea_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "BCDA", "BCDA", "BCDA", null, false, false) == null)
                return;
        }
        //三级强度面积统计
        void mhiTreGradeArea_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "BCDA", "BCDA", "BCDA", null, false, false) == null)
                return;
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
            CreateThemeGraphyBase(_session, "BCDF", "", "BPCD", "蓝藻频次监测专题图", false,"BAGFREQ");
        }
        //当前区域频次统计
        void btnCurrentRegionFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "FREQ", "", "DBLV", "蓝藻频次监测专题图", false, true) == null)
                return;
        }
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

        private void CreateThemeGraphyBase(ISmartSession session, string algorithmName, string outFileIdentify, string dataIdentify, string templateName, bool isCustom, string colorTableName)
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

        private List<string> GetlakeList()
        {
            if (_configDoc != null)
            {
                List<string> lakeList = new List<string>();
                var lakeNodes = _configDoc.Element("DataSources").Elements("DataSource");
                if (lakeNodes != null && lakeNodes.Count() != 0)
                {
                    foreach (XElement element in lakeNodes)
                    {
                        string lake = element.Attribute("name").Value;
                        if (!string.IsNullOrEmpty(lake))
                            lakeList.Add(lake);
                    }
                }
                return lakeList;
            }
            return null;
        }

        #region IUITabProvider
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
            _btnRough.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            btnSave.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnInteractive.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            btnCoverDegree.Image = _session.UIFrameworkHelper.GetImage("system:coverDegree_current.png");
            btnMulImage.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnbinImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            dbtnStrenImage.Image = _session.UIFrameworkHelper.GetImage("system:intensityImage.png");
            btnRateCurrentRegion.Image = _session.UIFrameworkHelper.GetImage("system:coverDegree_current.png");
            btnRateCustom.Image = _session.UIFrameworkHelper.GetImage("system:CoverDegreeCustom.png");
            btnCurrentRegionFreq.Image = _session.UIFrameworkHelper.GetImage("system:TimesGraph.png");
            dbtnCoverDArea.Image = _session.UIFrameworkHelper.GetImage("system:area_converDegree.png");
            btnCustomFreq.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnCoverDFreq.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
            btnCoverAFreq.Image = _session.UIFrameworkHelper.GetImage("system:TimesByArea.png");
            btnCurrentRegionArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            btnCustomArea.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnCDImage.Image = _session.UIFrameworkHelper.GetImage("system:coverDegree.png");
            btnAnimationRegion.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            btnAnimationCustom.Image = _session.UIFrameworkHelper.GetImage("system:customVedio.png");
            _btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
        }

        public void UpdateStatus()
        {
        }
        #endregion

        private void CustomVarSetterHandler(string dblvFileName,string beginSubprod, IContextEnvironment contextEnv)
        {
            string ndviFileName = Path.GetFileName(dblvFileName);
            if (string.IsNullOrEmpty(ndviFileName))
                return;
            string dblvName = Path.GetDirectoryName(dblvFileName)+"\\"+ ndviFileName.Replace("NDVI", "DBLV");
            if(File.Exists(dblvName))
            contextEnv.PutContextVar("DBLV", dblvName);
        }

    }
}
