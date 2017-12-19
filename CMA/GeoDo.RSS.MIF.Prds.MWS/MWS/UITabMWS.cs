#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：zhangyb     时间：2014-2-10 09:24:36
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

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
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：UITabMWS
    /// 属性描述：
    /// 创建者：zhangyb     时间：2014-2-10 09:24:36
    /// 修改者：Lixj            修改日期：2014-2-11
    /// 修改描述：增加各个功能按钮
    /// 备注：
    /// </summary>

    /// </summary>
    public partial class UITabMWS : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 微波积雪按钮定义
        RadButtonElement btnSqlSnowData = new RadButtonElement("数据检索");//"数据检索"按钮
        RadButtonElement btnHisDataProcess = new RadButtonElement("时间序列数据分析");    // 
        RadButtonElement btnHistoryData = new RadButtonElement("周期\n数据专题图");//查询出的历史周期数据出专题图、动画 
        RadButtonElement btnHisDataSata = new RadButtonElement("周期\n数据统计图");// 对查询出来的数据做统计柱状图
        RadButtonElement btnJuPingAnalysis = new RadButtonElement("距平分布图");//距平分析按钮
        RadButtonElement btnJuPingAnaStat = new RadButtonElement("距平统计图");//距平分析统计按钮
        RadButtonElement btnHisLayoutput = new RadButtonElement("专题图导出");//导出多个专题图
        RadButtonElement btnAVIPlayer = new RadButtonElement("AVI动画");//制作动画
        RadDropDownButtonElement dbtnManualExtract = new RadDropDownButtonElement();//交互判识
        RadButtonElement btnMWSParaComp = new RadButtonElement("反演雪深\n雪水当量");//"微波积雪参数计算"按钮
        RadButtonElement btnMergeCompDepth = new RadButtonElement("微波可见光\n融合雪深");//"微波可见光融合计算雪深"按钮
        RadButtonElement btnMWSParaAuto = new RadButtonElement("参数自动反演");//积雪参数全自动提取
        RadDropDownButtonElement dbtnAreaStatic = new RadDropDownButtonElement();//积雪参数面积统计
        RadDropDownButtonElement dbtnSweVolSatic = new RadDropDownButtonElement();//积雪等量水体积统计
        RadDropDownButtonElement dbtnDisLayout = new RadDropDownButtonElement();//专题产品
        RadButtonElement btnClose = new RadButtonElement("关闭积雪监测\n分析视图");
        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        //从配置文件读取统计分析菜单名称
        string provinceName = AreaStatProvider.GetAreaStatItemMenuName("行政区划");
        string landTypeName = AreaStatProvider.GetAreaStatItemMenuName("土地利用类型");
        string cityName = AreaStatProvider.GetAreaStatItemMenuName("三级行政区划");
       
        RadDropDownButtonElement dbtnDataCL = new RadDropDownButtonElement();      //数据合成
        #endregion

        public UITabMWS()
        {
            InitializeComponent();
            CreateRibbonBar();
        }

        #region CreateUI
        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "微波积雪监测专题";
            _tab.Text = "微波积雪监测专题";
            _tab.Name = "微波积雪监测专题";
            _bar.CommandTabs.Add(_tab);            
            CreateCheckGroup();//监测
            CreateDisasterGroup();//灾情事件
            CreateMWSParaStatic();//积雪参数统计
            CreateInqurGroup();//查询
            CreateOutputGroup();//输出
            btnToDb.TextAlignment = ContentAlignment.BottomCenter;
            btnToDb.ImageAlignment = ContentAlignment.TopCenter;
            btnToDb.Click += new EventHandler(btnToDb_Click);
            //RadRibbonBarGroup rbgToDb = new RadRibbonBarGroup();
            //rbgToDb.Text = "产品入库";
            //rbgToDb.Items.Add(btnToDb);
            //_tab.Items.Add(rbgToDb);
            Controls.Add(_bar);
        }

        /// <summary>
        /// 查询统计GROUP，zhangyb,20140126
        /// </summary>
        private void CreateInqurGroup()
        {
#if !PUB
            RadRibbonBarGroup MWSDataInqur = new RadRibbonBarGroup();
            MWSDataInqur.Text = "长时间序列数据分析";
            //时间序列数据分析
            btnHisDataProcess.ImageAlignment = ContentAlignment.TopCenter;
            btnHisDataProcess.TextAlignment = ContentAlignment.BottomCenter;
            btnHisDataProcess.Click += new EventHandler(btnHisDataProcess_Click);
            MWSDataInqur.Items.Add(btnHisDataProcess);
            ////数据检索
            //btnSqlSnowData.ImageAlignment = ContentAlignment.TopCenter;
            //btnSqlSnowData.TextAlignment = ContentAlignment.BottomCenter;
            //btnSqlSnowData.Click += new EventHandler(btnSqlSnowData_Click);
            //MWSDataInqur.Items.Add(btnSqlSnowData);
            ////历史统计数据专题图、柱状图输出
            //btnHistoryData.ImageAlignment = ContentAlignment.TopCenter;
            //btnHistoryData.TextAlignment = ContentAlignment.BottomCenter;
            //btnHistoryData.Click += new EventHandler(btnHistoryData_Click);
            //MWSDataInqur.Items.Add(btnHistoryData);
           
            ////历史周期数据出统计柱状图
            //btnHisDataSata.ImageAlignment = ContentAlignment.TopCenter;
            //btnHisDataSata.TextAlignment = ContentAlignment.BottomCenter;
            //btnHisDataSata.Click += new EventHandler(btnHisDataSata_Click);
            //MWSDataInqur.Items.Add(btnHisDataSata);
            ////距平分析
            //btnJuPingAnalysis.ImageAlignment = ContentAlignment.TopCenter;
            //btnJuPingAnalysis.TextAlignment = ContentAlignment.BottomCenter;
            //btnJuPingAnalysis.Click += new EventHandler(btnJuPingAnalysis_Click);
            //MWSDataInqur.Items.Add(btnJuPingAnalysis);
            ////距平分析统计
            //btnJuPingAnaStat.ImageAlignment = ContentAlignment.TopCenter;
            //btnJuPingAnaStat.TextAlignment = ContentAlignment.BottomCenter;
            //btnJuPingAnaStat.Click += new EventHandler(btnJuPingAnaStat_Click);
            //MWSDataInqur.Items.Add(btnJuPingAnaStat);
             _tab.Items.Add(MWSDataInqur);
#endif
        }
        private void CreateOutputGroup()
        {
            RadRibbonBarGroup MWSOutput = new RadRibbonBarGroup();
            MWSOutput.Text = "图像输出";
            //多专题图导出
            btnHisLayoutput.ImageAlignment = ContentAlignment.TopCenter;
            btnHisLayoutput.TextAlignment = ContentAlignment.BottomCenter;
            btnHisLayoutput.Click += new EventHandler(btnHisLayoutput_Click);
            MWSOutput.Items.Add(btnHisLayoutput);

            //制作动画
            btnAVIPlayer.ImageAlignment = ContentAlignment.TopCenter;
            btnAVIPlayer.TextAlignment = ContentAlignment.BottomCenter;
            btnAVIPlayer.Click += new EventHandler(btnAVIPlayer_Click);
            MWSOutput.Items.Add(btnAVIPlayer);

            _tab.Items.Add(MWSOutput);
        }

        /// <summary>
        /// 监测GROUP
        /// </summary>
        private void CreateCheckGroup()
        {
            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            rbgCheck.Text = "监测";
            dbtnManualExtract.Text = "交互判识";
            RadMenuItem btnInteractive = new RadMenuItem("积雪");
            btnInteractive.Click += new EventHandler(btnInteractive_Click);
            dbtnManualExtract.Items.Add(btnInteractive);
            dbtnManualExtract.TextAlignment = ContentAlignment.BottomCenter;
            dbtnManualExtract.ImageAlignment = ContentAlignment.TopCenter;
            rbgCheck.Items.Add(dbtnManualExtract);
            _tab.Items.Add(rbgCheck);
        }

        /// <summary>
        /// 日常业务group
        /// </summary>
        private void CreateDisasterGroup()
        {
            RadRibbonBarGroup rbgDisaster = new RadRibbonBarGroup();
            rbgDisaster.Text = "日常业务";
            btnMWSParaComp.ImageAlignment = ContentAlignment.TopCenter;
            btnMWSParaComp.TextAlignment = ContentAlignment.BottomCenter;
            btnMWSParaComp.Click += new EventHandler(btnMWSParaComp_Click);//积雪参数计算
            rbgDisaster.Items.Add(btnMWSParaComp);
#if!PUB
            btnMWSParaAuto.ImageAlignment = ContentAlignment.TopCenter;
            btnMWSParaAuto.TextAlignment = ContentAlignment.BottomCenter;
            btnMWSParaAuto.Click += new EventHandler(btnMWSParaAuto_Click);//积雪参数自动提取
            rbgDisaster.Items.Add(btnMWSParaAuto);
#endif   
            btnMergeCompDepth.ImageAlignment = ContentAlignment.TopCenter;
            btnMergeCompDepth.TextAlignment = ContentAlignment.BottomCenter;
            btnMergeCompDepth.Click += new EventHandler(mniMWMergeVIR_Click);//微波可见光数据融合计算雪深
            rbgDisaster.Items.Add(btnMergeCompDepth);

            #region 数据合成

            dbtnDataCL.Text = "数据计算";
            dbtnDataCL.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnDataCL.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDataCL.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem btnAvgMake = new RadMenuItem("平均值计算");
            btnAvgMake.Click += new EventHandler(btnAvgMake_Click);
            dbtnDataCL.Items.Add(btnAvgMake);
            RadMenuItem btnMaxMake = new RadMenuItem("最大值计算");
            btnMaxMake.Click += new EventHandler(btnMaxMake_Click);
            dbtnDataCL.Items.Add(btnMaxMake);
            RadMenuItem btnMinMake = new RadMenuItem("最小值计算");
            btnMinMake.Click += new EventHandler(btnMinMake_Click);
            dbtnDataCL.Items.Add(btnMinMake);
            rbgDisaster.Items.Add(dbtnDataCL);

            #endregion
            
            dbtnDisLayout.Text = "专题产品";
            dbtnDisLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnDisLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDisLayout.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniDisSnowDepth = new RadMenuItem("微波雪深监测图");
            mniDisSnowDepth.Click += new EventHandler(mniDisSnowDepth_Click);
            //RadMenuItem mniDisSnowDepthCustom = new RadMenuItem("雪深监测图（自定义）");
            //mniDisSnowDepthCustom.Click += new EventHandler(mniDisSnowDepthCustom_Click);
            RadMenuItem mniDisSnowDepthVI = new RadMenuItem("融合雪深监测图");
            mniDisSnowDepthVI.Click += new EventHandler(mniDisSnowDepthVI_Click);
            RadMenuItem mniDisSnowWE = new RadMenuItem("微波雪水当量监测图");
            mniDisSnowWE.Click += new EventHandler(mniDisSnowWE_Click);
            RadMenuItem mniDisSDSWECustom = new RadMenuItem("雪深雪水当量专题图（自定义）");
            mniDisSDSWECustom.Click += new EventHandler(mniDisSDSWECustom_Click);
            //RadMenuItem mniDisJuPingAna = new RadMenuItem("距平分析专题图");
            //mniDisJuPingAna.Click += new EventHandler(mniDisJuPingAna_Click);
            RadMenuItem mniDisPolarSnowWE = new RadMenuItem("MWRI雪深雪水当量日产品专题图");
            mniDisPolarSnowWE.Click += new EventHandler(mniDisPolarSnowWE_Click);
            dbtnDisLayout.Items.AddRange(new RadMenuItem[] { mniDisSnowDepth, mniDisSnowDepthVI, mniDisSnowWE, mniDisSDSWECustom,mniDisPolarSnowWE });
            rbgDisaster.Items.Add(dbtnDisLayout);

            _tab.Items.Add(rbgDisaster);

        }

        private void CreateMWSParaStatic()
        {
            RadRibbonBarGroup rbgFreqStatic = new RadRibbonBarGroup();
            RadRibbonBarGroup rbgSnowArgsSTA = new RadRibbonBarGroup();
            rbgSnowArgsSTA.Text = "积雪参数统计";

            dbtnAreaStatic.Text = "面积统计";
            dbtnAreaStatic.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnAreaStatic.ImageAlignment = ContentAlignment.TopCenter;
            dbtnAreaStatic.TextAlignment = ContentAlignment.BottomCenter;

            RadMenuItem mniNewDivision = new RadMenuItem("省界统计");
            mniNewDivision.Click += new EventHandler(mniNewDivision_Click);

            RadMenuItem mniNewCity = new RadMenuItem("市县统计");
            mniNewCity.Click += new EventHandler(mniNewCity_Click);

            RadMenuItem mniNewCurrentRegionArea = new RadMenuItem("当前区域统计");
            mniNewCurrentRegionArea.Click += new EventHandler(mniNewCurrentRegionArea_Click);

            RadMenuItem mniNewLandType = new RadMenuItem("土地类型");
            mniNewLandType.Click += new EventHandler(mniNewLandType_Click);

            dbtnAreaStatic.Items.AddRange(new RadMenuItem[] { mniNewCurrentRegionArea, mniNewDivision, mniNewCity,mniNewLandType });
            rbgSnowArgsSTA.Items.Add(dbtnAreaStatic);
            //体积统计 
            dbtnSweVolSatic.Text = "雪水当量体积统计";
            dbtnSweVolSatic.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnSweVolSatic.ImageAlignment = ContentAlignment.TopCenter;
            dbtnSweVolSatic.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniVolDivision = new RadMenuItem("省界统计");
            mniVolDivision.Click += new EventHandler(mniVolDivision_Click);
            RadMenuItem mniVolCity = new RadMenuItem("市县统计");
            mniVolCity.Click += new EventHandler(mniVolCity_Click);
            RadMenuItem mniVolCurrentRegionArea = new RadMenuItem("当前区域统计");
            mniVolCurrentRegionArea.Click += new EventHandler(mniVolCurrentRegionArea_Click);

            RadMenuItem mniVolLandType = new RadMenuItem("土地类型");
            mniVolLandType.Click += new EventHandler(mniVolLandType_Click);
            dbtnSweVolSatic.Items.AddRange(new RadMenuItem[] { mniVolCurrentRegionArea, mniVolDivision, mniVolCity,mniVolLandType });
            rbgSnowArgsSTA.Items.Add(dbtnSweVolSatic);
            _tab.Items.Add(rbgSnowArgsSTA);

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

        #endregion

        #region 事件
        /// <summary>
        /// 产品入库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnToDb_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(39002);
            if (cmd != null)
                cmd.Execute();
        }

        /// <summary>
        /// 交互积雪
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnInteractive_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExecute(6602);
        }
        /// <summary>
        /// 土地类型面积统计
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
        /// 自定义面积统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCustomArea_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.AreaStat(_session, "CCCA", "", true, true) == null)
                return;
        }

        /// <summary>
        /// 关闭
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
        
        /// <summary>
        /// 雪深监测图（当前区域）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mniDisSnowDepth_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            string[] selectedFiles = null;
            if (TryGetSelectedPrimaryFiles(ref selectedFiles, ms))
            {
                RasterIdentify id = new RasterIdentify(selectedFiles[0]);
                if (id.SubProductIdentify == "MFSD")
                    ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MSDI");
                else if (id.SubProductIdentify == "0SSD")
                    ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "SSDI");
            }
            else
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "SSDI");
            ms.DoAutoExtract(true);
        }

        /// <summary>
        /// 雪深监测图（自定义）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mniDisSnowDepthCustom_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            string[] selectedFiles = null;
            if (TryGetSelectedPrimaryFiles(ref selectedFiles, ms))
            {
                RasterIdentify id = new RasterIdentify(selectedFiles[0]);
                if (id.SubProductIdentify == "MWSD")
                    MIFCommAnalysis.CreateThemeGraphy(_session, "MSDC", "MWSMWSD", "MWSD", "积雪雪深监测图", true, true);
                else if (id.SubProductIdentify == "0SSD")
                    MIFCommAnalysis.CreateThemeGraphy(_session, "SSDC", "MWS0SSD", "0SSD", "积雪雪深监测图", true, true);
            }
            else
                MIFCommAnalysis.CreateThemeGraphy(_session, "MSDC", "MWSMWSD", "MWSD", "积雪雪深监测图", true, true);
        }
        /// <summary>
        /// 融合雪深监测图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mniDisSnowDepthVI_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MSVI");
            ms.DoAutoExtract(true);
        }
        /// <summary>
        /// 雪水当量监测图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mniDisSnowWE_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MSWI");
            ms.DoAutoExtract(true);
        }

        //void mniDisJuPingAna_Click(object sender, EventArgs e)
        //{
        //    IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
        //    ms.ChangeActiveSubProduct("0IMG");
        //    ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MJPI");
        //    ms.DoAutoExtract(true);
        //}
        /// <summary>
        /// 用于裁切后ldf 格式的雪深雪水当量专题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniDisSDSWECustom_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("SDWE");
            GetCommandAndExecute(6602);
        }
        /// <summary>
        /// 雪深雪水当量产品专题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mniDisPolarSnowWE_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0PSI");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 积雪参数计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMWSParaComp_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0SSD");
            GetCommandAndExecute(6602);
        }
        /// <summary>
        /// 积雪参数自动提取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMWSParaAuto_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("ATSD");
            GetCommandAndExecute(6602);
        }
        /// <summary>
        /// "微波积雪"--"数据检索"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSqlSnowData_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(36603);
            if (cmd != null)
                cmd.Execute();
        }
        void btnAVIPlayer_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(36605);
            if (cmd != null)
                cmd.Execute();
        }

        void btnHistoryData_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("HIST"); // 历史数据出专题图
            GetCommandAndExecute(6602);
        }
        //历史周期数据出统计柱状图
        void btnHisDataSata_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("HSTA");
            //ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "VCAR");
            ms.DoManualExtract(true);
        }
        //导出专题图按钮
        void btnHisLayoutput_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(36604);
            if (cmd != null)
                cmd.Execute();
        }
        //历史距平分析数据出专题图
        void btnJuPingAnalysis_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("JPAL"); // 距平分析
            GetCommandAndExecute(6602);
        }
        
        void btnJuPingAnaStat_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("JPST");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "JPEX");
            ms.DoManualExtract(true);
        }

        void btnHisDataProcess_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("TSAN");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "TSAN");
            ms.DoManualExtract(true);
        }
        /// <summary>
        /// "微波可见光数据融合"
        /// </summary>
        void mniMWMergeVIR_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("MWVI");
            GetCommandAndExecute(6602);
        }
        /// <summary>
        /// 当前区域面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mniNewCurrentRegionArea_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("ASTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "DCAR");
            ms.DoManualExtract(true);
        }

        /// <summary>
        /// 土地利用类型面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mniNewLandType_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("ASTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "DLUT");
            ms.DoManualExtract(true);
        }

        void mniNewDivision_Click(object sender, EventArgs e)
        {

            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("ASTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "DCBP");
            ms.DoManualExtract(true);
        }

        void mniNewCity_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("ASTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "DCBC");
            ms.DoManualExtract(true);
        }
        void mniVolDivision_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("VSTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "VCBP");
            ms.DoManualExtract(true);
        }

        void mniVolLandType_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("VSTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "VLUT");
            ms.DoManualExtract(true);
            //GetCommandAndExecute(6602);
        }

        void mniVolCurrentRegionArea_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("VSTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "VCAR");
            ms.DoManualExtract(true);
        }

        void mniVolCity_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("VSTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "VCBC");
            ms.DoManualExtract(true);
        }
        #endregion

        public object Content
        {
            get { return _tab; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            this._session = session;
            //CreateThemeGraphRegionButton();
            CreateCloseGroup();
            SetImage();
        }

        private void CreateThemeGraphRegionButton()
        {
            RadDropDownButtonElement _btnThemeGraphRegion = new RadDropDownButtonElement();
            ThemeGraphRegionFacctory.RegistToButton("MWS", _btnThemeGraphRegion, _tab, _session);
            _btnThemeGraphRegion.Image = _session.UIFrameworkHelper.GetImage("system:settings.png");
        }

        private void SetImage()
        {
            btnHisDataProcess.Image = _session.UIFrameworkHelper.GetImage("system:comList.png");
            //btnSqlSnowData.Image = _session.UIFrameworkHelper.GetImage("system:FileType_Normal.png");
            //btnHistoryData.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            //btnJuPingAnalysis.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
            //btnJuPingAnaStat.Image = _session.UIFrameworkHelper.GetImage("system:area_converDegree.png");
            //btnHisDataSata.Image = _session.UIFrameworkHelper.GetImage("system:comList.png");
            btnHisLayoutput.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            btnAVIPlayer.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            dbtnManualExtract.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
            btnMWSParaComp.Image = _session.UIFrameworkHelper.GetImage("system:fogLwp.png");
            btnMergeCompDepth.Image = _session.UIFrameworkHelper.GetImage("system:fogCycTime.png");
            dbtnDisLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnSweVolSatic.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            dbtnAreaStatic.Image = _session.UIFrameworkHelper.GetImage("system:landType.png");
            btnMWSParaAuto.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnDataCL.Image = _session.UIFrameworkHelper.GetImage("system:wydpz.png");
        }

        public void UpdateStatus()
        {
        }

        private void GetCommandAndExecute(int cmdID)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute("MWS");
        }
        
        private bool TryGetSelectedPrimaryFiles(ref string[] files, IMonitoringSession monitoringSession)
        {
            IWorkspace wks = monitoringSession.Workspace;
            if (wks == null)
                return false;
            ICatalog catalog = wks.ActiveCatalog;
            if (catalog == null)
                return false;
            string[] fnames = catalog.GetSelectedFiles();
            if (fnames == null || fnames.Length == 0)
                return false;
            files = fnames;
            return true;
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


    }
}
