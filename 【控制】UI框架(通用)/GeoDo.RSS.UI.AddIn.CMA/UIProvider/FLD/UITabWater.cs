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
    public partial class UITabWater : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 水按钮定义
        RadButtonElement btnRough = new RadButtonElement("粗判");
        //快速生成
        //RadDropDownButtonElement dbtGenerate = new RadDropDownButtonElement();
        RadSplitButtonElement dbtGenerate = new RadSplitButtonElement();
        RadMenuItem mniAllGenerate = new RadMenuItem("粗判+专题产品");
        RadMenuItem mniProGenerate = new RadMenuItem("专题产品");
        RadButtonElement btnInteractive = new RadButtonElement("交互判识");
        //RadButtonElement btnCloud = new RadButtonElement("云覆盖度计算");
        RadButtonElement btnMulImage = new RadButtonElement("多通道合成图");
        RadButtonElement btnBinImage = new RadButtonElement("二值图");
        //混合像元
        RadDropDownButtonElement dbtnMix = new RadDropDownButtonElement();
        RadMenuItem mniComputeMix = new RadMenuItem("计算");
        RadMenuItem mniMixImage = new RadMenuItem("专题图");
        //RadButtonElement btnChangeImage = new RadButtonElement("水体变化分析");
        //泛滥缩小水体
        RadDropDownButtonElement dbtFlood = new RadDropDownButtonElement();
        RadMenuItem mniComputeFlood = new RadMenuItem("计算");
        RadMenuItem mniFloodImage = new RadMenuItem("生成专题图");
        //判识水体面积统计
        RadDropDownButtonElement dbtAreaStat = new RadDropDownButtonElement();
        RadMenuItem mniCurrentArea = new RadMenuItem("当前区域");
        RadMenuItem mniProvince = new RadMenuItem("按省界");
        RadMenuItem mniCityArea = new RadMenuItem("按市县");
        RadMenuItem mniLandTypedblv = new RadMenuItem("土地类型");    
        //扩大缩小水体面积统计
        RadDropDownButtonElement dbtFloodArea = new RadDropDownButtonElement();
        RadMenuItem mniProvincBound = new RadMenuItem("按省界");
        RadMenuItem mniCustomArea = new RadMenuItem("自定义");
        RadMenuItem mniLandType = new RadMenuItem("土地类型");
        RadMenuItem mniDivAndType = new RadMenuItem("行政区+地类");
        RadButtonElement btnFloodDayArea = new RadButtonElement("泛滥历时面积");
        //淹没历时
        RadDropDownButtonElement dbtnFloodDayImage = new RadDropDownButtonElement();
        RadMenuItem mhiThreeLevel = new RadMenuItem("三级");
        RadMenuItem mhiSixLevel = new RadMenuItem("六级");
        RadButtonElement btnMutFloodImage = new RadButtonElement("多时次洪涝");
        RadButtonElement btnMovie = new RadButtonElement("动画显示");               //动画
        RadButtonElement btnClose = new RadButtonElement("关闭水情监测\n分析视图");
        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        #endregion

        public UITabWater()
        {
            InitializeComponent();
            CreateRibbonBar();
        }

        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "水情监测专题";
            _tab.Text = "水情监测专题";
            _tab.Name = "水情监测专题";
            _bar.CommandTabs.Add(_tab);

            #region 日常监测
            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            rbgCheck.Text = "日常水体监测";
            //粗判
            btnRough.Click += new EventHandler(btnRough_Click);
            btnRough.ImageAlignment = ContentAlignment.TopCenter;
            btnRough.TextAlignment = ContentAlignment.BottomCenter;
            rbgCheck.Items.Add(btnRough);
            //交互判识
            btnInteractive.Click += new EventHandler(btnInteractive_Click);
            btnInteractive.ImageAlignment = ContentAlignment.TopCenter;
            btnInteractive.TextAlignment = ContentAlignment.BottomCenter;
            rbgCheck.Items.Add(btnInteractive);
            //多通道合成图
            btnMulImage.ImageAlignment = ContentAlignment.TopCenter;
            btnMulImage.TextAlignment = ContentAlignment.BottomCenter;
            btnMulImage.Click += new EventHandler(btnMulImage_Click);
            rbgCheck.Items.Add(btnMulImage);
            //二值图
            btnBinImage.ImageAlignment = ContentAlignment.TopCenter;
            btnBinImage.TextAlignment = ContentAlignment.BottomCenter;
            btnBinImage.Click += new EventHandler(btnBinImage_Click);
            rbgCheck.Items.Add(btnBinImage);
            //混合像元
            mniComputeMix.Click += new EventHandler(mniComputeMix_Click);
            mniMixImage.Click += new EventHandler(mniMixImage_Click);
            dbtnMix.Items.Add(mniComputeMix);
            dbtnMix.Items.Add(mniMixImage);
            dbtnMix.Text = "混合像元";
            dbtnMix.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnMix.ImageAlignment = ContentAlignment.TopCenter;
            dbtnMix.TextAlignment = ContentAlignment.BottomCenter;
            rbgCheck.Items.Add(dbtnMix);

            mniCurrentArea.Click+=new EventHandler(mniCurrentArea_Click);
            mniCityArea.Click += new EventHandler(mniCityArea_Click);
            mniProvince.Click += new EventHandler(mniProvince_Click);
            mniLandTypedblv.Click += new EventHandler(mniLandTypedblv_Click);
            dbtAreaStat.Text = "判识水体面积统计";
            dbtAreaStat.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtAreaStat.ImageAlignment = ContentAlignment.TopCenter;
            dbtAreaStat.TextAlignment = ContentAlignment.BottomCenter;
            dbtAreaStat.Items.Add(mniCurrentArea);
            dbtAreaStat.Items.Add(mniProvince);
            dbtAreaStat.Items.Add(mniCityArea);
            dbtAreaStat.Items.Add(mniLandTypedblv);
            rbgCheck.Items.Add(dbtAreaStat);
            //快速生成
            dbtGenerate.Text = "快速生成";
            dbtGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtGenerate.ImageAlignment = ContentAlignment.TopCenter;
            dbtGenerate.TextAlignment = ContentAlignment.BottomCenter;
            mniAllGenerate.Click += new EventHandler(mniAllGenerate_Click);
            dbtGenerate.Items.Add(mniAllGenerate);
            mniProGenerate.Click += new EventHandler(mniProGenerate_Click);
            dbtGenerate.Items.Add(mniProGenerate);   
            rbgCheck.Items.Add(dbtGenerate);
            #endregion 日常监测

            #region 水体变化监测
            RadRibbonBarGroup rgbChange = new RadRibbonBarGroup();
            rgbChange.Text = "水体变化监测";
            dbtFlood.Text = "扩大缩小水体";
            dbtFlood.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtFlood.ImageAlignment = ContentAlignment.TopCenter;
            dbtFlood.TextAlignment = ContentAlignment.BottomCenter;
            mniComputeFlood.Click += new EventHandler(mniComputeFlood_Click);
            mniFloodImage.Click += new EventHandler(mniFloodImage_Click);
            dbtFlood.Items.Add(mniComputeFlood);
            dbtFlood.Items.Add(mniFloodImage);
            rgbChange.Items.Add(dbtFlood);
            dbtFloodArea.Text = "变化水体面积统计";
            dbtFloodArea.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtFloodArea.ImageAlignment = ContentAlignment.TopCenter;
            dbtFloodArea.TextAlignment = ContentAlignment.BottomCenter;
            mniProvincBound.Click += new EventHandler(mniProvincBound_Click);
            mniCustomArea.Click += new EventHandler(mniCustomArea_Click);
            mniLandType.Click += new EventHandler(mniLandType_Click);
            mniDivAndType.Click += new EventHandler(mniDivAndType_Click);
            dbtFloodArea.Items.AddRange(new RadMenuItem[] { mniProvincBound, mniCustomArea, mniLandType, mniDivAndType });
            rgbChange.Items.Add(dbtFloodArea);
            #endregion

            #region 隐藏代码
            //btnCloud.ImageAlignment = ContentAlignment.TopCenter;
            //btnCloud.TextAlignment = ContentAlignment.BottomCenter;
            //btnCloud.Click += new EventHandler(btnCloud_Click);
            //jiaoHuFenXi.Items.Add(btnCloud);
            //dbtnCloud.Text = "云监测";
            //dbtnCloud.ArrowPosition = DropDownButtonArrowPosition.Right;
            //dbtnCloud.ImageAlignment = ContentAlignment.TopCenter;
            //dbtnCloud.TextAlignment = ContentAlignment.BottomCenter;
            //RadMenuItem btnCloud = new RadMenuItem("云判识");
            //btnCloud.Click += new EventHandler(btnCloud_Click);
            //dbtnCloud.Items.Add(btnCloud);
            //RadMenuItem btnCloudCover = new RadMenuItem("云覆盖度");
            //btnCloudCover.Click += new EventHandler(btnCloudCover_Click);
            //dbtnCloud.Items.Add(btnCloudCover);
            //jiaoHuFenXi.Items.Add(dbtnCloud);
            //btnChangeImage.ImageAlignment = ContentAlignment.TopCenter;
            //btnChangeImage.TextAlignment = ContentAlignment.BottomCenter;
            //btnChangeImage.Click += new EventHandler(btnChangeImage_Click);//...功能未实现，暂时隐蔽其事件。
            //rbgComparaAnaly.Items.Add(btnChangeImage);
            #endregion

            #region 长时间序列分析
            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "长时间序列分析";
            //泛滥历时面积
            btnFloodDayArea.TextAlignment = ContentAlignment.BottomCenter;
            btnFloodDayArea.ImageAlignment = ContentAlignment.TopCenter;
            btnFloodDayArea.Click+=new EventHandler(btnFloodDayArea_Click);
            rbgProduct.Items.Add(btnFloodDayArea);
            //多时次洪涝
            btnMutFloodImage.ImageAlignment = ContentAlignment.TopCenter;
            btnMutFloodImage.TextAlignment = ContentAlignment.BottomCenter;
            btnMutFloodImage.Click += new EventHandler(btnMutFloodImage_Click);
            rbgProduct.Items.Add(btnMutFloodImage);
            //淹没历时
            dbtnFloodDayImage.Text = "淹没历时";
            dbtnFloodDayImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnFloodDayImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnFloodDayImage.TextAlignment = ContentAlignment.BottomCenter; 
            RadMenuItem mhiThreeLevel = new RadMenuItem("三级");
            mhiThreeLevel.Click += new EventHandler(mhiThreeLevel_Click);
            dbtnFloodDayImage.Items.Add(mhiThreeLevel);
            RadMenuItem mhiSixLevel = new RadMenuItem("六级");
            mhiSixLevel.Click += new EventHandler(mhiSixLevel_Click);
            dbtnFloodDayImage.Items.Add(mhiSixLevel);
            rbgProduct.Items.Add(dbtnFloodDayImage);
            //动画
            btnMovie.ImageAlignment = ContentAlignment.TopCenter;
            btnMovie.TextAlignment = ContentAlignment.BottomCenter;
            btnMovie.Click += new EventHandler(btnMovie_Click);
            rbgProduct.Items.Add(btnMovie);
            #endregion

            #region 关闭
            RadRibbonBarGroup rbgClose = new RadRibbonBarGroup();
            rbgClose.Text = "关闭";
            btnClose.ImageAlignment = ContentAlignment.TopCenter;
            btnClose.TextAlignment = ContentAlignment.BottomCenter;
            btnClose.Click += new EventHandler(btnClose_Click);
            rbgClose.Items.Add(btnClose);
            #endregion

            _tab.Items.Add(rbgCheck);
            _tab.Items.Add(rgbChange);
            _tab.Items.Add(rbgProduct);

            
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

        #region 日常水情监测
        //粗判
        void btnRough_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExcute(6601);
        }

        //交互判时
        void btnInteractive_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExcute(6602);
        }

        /// <summary>
        /// 水情产品多通道合成图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMulImage_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:水情产品多通道合成图,多通道合成图,FLD,MCSI");
        }

        //二值图
        void btnBinImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "DBLV", "水情二值图模版", false, true) == null)
                return;
        }

        //混合像元
        void mniComputeMix_Click(object sender, EventArgs e)    //混合像元栅格产品
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0MIX");
            GetCommandAndExcute(6602);
        }

        void mniMixImage_Click(object sender, EventArgs e)     //混合像元专题图
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "MIXI", "", "0MIX", "水情混合像元专题图模版", false, true) == null)
                return;
        }

        //判时水体面积统计

        void mniCurrentArea_Click(object sender, EventArgs e)        //判识水体当前区域
        {
            if (MIFCommAnalysis.AreaStat(_session, "CCAR", "", false, true) == null)
                return;
        }

        void mniCityArea_Click(object sender, EventArgs e)           //判时水体按市县统计
        {
            if (MIFCommAnalysis.AreaStat(_session, "0CCC", "分级行政区划", false, true) == null)
                return;
        }

        void mniProvince_Click(object sender, EventArgs e)           //判时水体按省界统计
        {
            if (MIFCommAnalysis.AreaStat(_session, "0CBP", "省级行政区划", false, true) == null)
                return;
        }

        void mniLandTypedblv_Click(object sender, EventArgs e)      //判时水体按土地类型统计面积
        {
            if (MIFCommAnalysis.AreaStat(_session, "CLUT", "土地利用类型", false, true) == null)
                return;
        }

        //快速生成
        void mniProGenerate_Click(object sender, EventArgs e)       //快速生成专题产品
        {
            AutoGenretateProvider.AutoGenrate(_session, "STAT");
        }

        void mniAllGenerate_Click(object sender, EventArgs e)       //快速生成粗判+专题产品
        {
            AutoGenretateProvider.AutoGenrate(_session, null);
        }
        #endregion

        #region  水体变化监测
        //扩大缩小水体
        void mniComputeFlood_Click(object sender, EventArgs e)      //生成扩大缩小水体栅格文件
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("FLOD");
            GetCommandAndExcute(6602);
        }

        void mniFloodImage_Click(object sender, EventArgs e)        //扩大缩小水体专题图
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "FLIM", "", "FLOD", "扩大缩小水体专题图模版", false, true) == null)
                return;
        }
        //扩大水体面积统计
        void mniProvincBound_Click(object sender, EventArgs e)       //扩大水体按省界面积统计
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "FCBP", "FWAS", "FWAS", "省级行政区划", false, true) == null)
                return;
        }
        void mniDivAndType_Click(object sender, EventArgs e)         //扩大水体按行政区划+地类
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "ADLU", "FWAS", "FWAS", "省级行政区+土地利用类型", false, true) == null)
                return;
        }
        void mniLandType_Click(object sender, EventArgs e)           //扩大水体按土地类型
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "FLUT", "FWAS", "FWAS", "土地利用类型", false, true) == null)
                return;
        }

        void mniCustomArea_Click(object sender, EventArgs e)         //扩大水体当前区域面积统计
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "CCCA", "FWAS", "FWAS", "", true, true) == null)
                return;
        }
        #endregion

        //云覆盖度计算
        void btnCloud_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("YFGD");
            GetCommandAndExcute(6602);
        }
        /// <summary>
        /// 变化水体分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnChangeImage_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0MIX");
            GetCommandAndExcute(6602);
            frmChangeWater frmChangeWater = new frmChangeWater();
            frmChangeWater.Show();
        }
   
        /// <summary>
        /// 泛滥历时面积
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnFloodDayArea_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "FLLS", "FLLS", "FLLS", "省级行政区+土地利用类型", false, true) == null)
                return;
        }

        #region 长时间序列分析
        /// <summary>
        /// 淹没历时  六级
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiSixLevel_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "LS6J", "", "DBLV", "六级水情淹没历时专题图模版", false, true) == null)
                return;
        }

        /// <summary>
        /// 淹没历时 三级
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiThreeLevel_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "LS3J", "", "DBLV", "三级水情淹没历时专题图模版", false, true) == null)
                return;
        }

        /// <summary>
        /// 多时次洪涝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMutFloodImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CycleTimeStat(_session, "MUTI", "", "DBLV", "多时次洪涝特点专题分析图", false, true) == null)
                return;
        }

        /// <summary>
        /// 动画当前区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMovie_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.CreatAVI(_session, "template:水情动画展示专题图,动画展示专题图,FLD,MEDI");
        }
  
        #endregion

        void btnClose_Click(object sender, EventArgs e)
        {
            if (_session == null)
                return;
            _session.UIFrameworkHelper.Remove(_tab.Text);
            _session.UIFrameworkHelper.ActiveDefaultTab();
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveProduct(null);
            GetCommandAndExcute(6602);
        }

        //产品入库
        void btnToDb_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(39002);
            if (cmd != null)
                cmd.Execute();
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

        #region 初始化
        public void UpdateStatus()
        {
        }

        private void GetCommand(int id)
        {
            ICommand cmd = _session.CommandEnvironment.Get(id);
            if (cmd != null)
                cmd.Execute("water");
        }

        private void GetCommandAndExcute(int cmdID)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute("FLD");
        }

        private void SetImage()
        {
            btnRough.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            dbtGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnInteractive.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            //btnCloud.Image = _session.UIFrameworkHelper.GetImage("system:wydcloud.png");
            btnMulImage.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            btnBinImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            dbtnMix.Image = _session.UIFrameworkHelper.GetImage("system:mixPixel.png");
            dbtFloodArea.Image = _session.UIFrameworkHelper.GetImage("system:landType.png");
            //btnChangeImage.Image = _session.UIFrameworkHelper.GetImage("system:wydhlhp2.png");
            dbtFlood.Image = _session.UIFrameworkHelper.GetImage("system:wydzdsy3.png");
            dbtAreaStat.Image = _session.UIFrameworkHelper.GetImage("system:landType.png");
            btnFloodDayArea.Image = _session.UIFrameworkHelper.GetImage("system:dem.png");
            dbtnFloodDayImage.Image = _session.UIFrameworkHelper.GetImage("system:wydhupo.png");
            btnMutFloodImage.Image = _session.UIFrameworkHelper.GetImage("system:AdjustByPan.png");
            btnMovie.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
        }
        #endregion
    }
}
