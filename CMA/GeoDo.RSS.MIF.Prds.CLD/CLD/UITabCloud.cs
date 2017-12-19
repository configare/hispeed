#region Version Info
/*========================================================================
* 功能概述：云监测专题界面定义
* 
* 创建者：zhangyb     时间：2014-2-25 09:24:36
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
using System.Collections;
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
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using System.Threading;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    /// <summary>
    /// 类名：UITabCloud
    /// 属性描述：云监测专题界面定义
    /// 创建者：zhangyb     时间：2014-2-25 09:24:36
    /// 修改者：           修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public partial class UITabCloud : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 卫星遥感云参数按钮定义
        RadButtonElement btnClose = new RadButtonElement("关闭云监测\n分析视图");
        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        #region 数据处理相关按钮
        RadButtonElement btnCldDataContinutyCheck = new RadButtonElement("数据完整性检验");
         RadDropDownButtonElement dbtnCldParasExtr = new RadDropDownButtonElement();//云参数数据提取
        RadDropDownButtonElement dbtnCldPeriodMerge = new RadDropDownButtonElement();

        #endregion

        #region 数据集成和管理相关按钮
        RadButtonElement btnCldDataBaseArgs = new RadButtonElement("数据库配置");
        RadButtonElement btnData2MySQL = new RadButtonElement("归档入库");
        RadButtonElement btnCldDataInquire = new RadButtonElement("数据检索");
        RadButtonElement btnCldDataManage= new RadButtonElement("数据清理");
        #endregion

        #region 统计分析相关按钮
        RadButtonElement btnCldStatRegionSet = new RadButtonElement("区域设置");
        RadDropDownButtonElement dbtnCldCommonStatics = new RadDropDownButtonElement();//统计分析
        RadDropDownButtonElement dbtnCldFieldStatics = new RadDropDownButtonElement();//场分析
        RadDropDownButtonElement dbtnBandsScatters = new RadDropDownButtonElement();//场分析
        RadButtonElement btnCldStatContour = new RadButtonElement("等值线");
        RadDropDownButtonElement dbtnCldLayout = new RadDropDownButtonElement();//分析结果显示输出
        RadDropDownButtonElement dbtnSingleProfile = new RadDropDownButtonElement();//单点廓线

        #endregion
        #endregion

        public UITabCloud()
        {
            InitializeComponent();
            CreateRibbonBar();
        }

        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "云监测专题";
            _tab.Text = "云监测专题";
            _tab.Name = "云监测专题";
            _bar.CommandTabs.Add(_tab);
            CreateDataProGroup();//数据处理
            CreateDataManagerGroup();//数据集成和管理
            CreateCLDStatic();//统计分析
            AddToDbButton();
            Controls.Add(_bar);
        }

        private void CreateDataProGroup()
        {
            RadRibbonBarGroup rbgDataPro = new RadRibbonBarGroup();
            rbgDataPro.Text = "数据处理";
            btnCldDataContinutyCheck.TextAlignment = ContentAlignment.BottomCenter;
            btnCldDataContinutyCheck.ImageAlignment = ContentAlignment.TopCenter;
            btnCldDataContinutyCheck.Click += new EventHandler(btnCldDataContinutyCheck_Click);
            rbgDataPro.Items.Add(btnCldDataContinutyCheck);
            #region 参数数据提取
            //RadMenuItem btnISCCPD2 = new RadMenuItem("ISCCP D2云参数数据");
            //btnISCCPD2.Click += new EventHandler(btnISCCPD2_Click);
            //dbtnCldParasExtr.Items.Add(btnISCCPD2);
            RadMenuItem btnMODISL2 = new RadMenuItem("Terra MOD06云参数产品数据");
            btnMODISL2.Click += new EventHandler(btnMOD06_Click);
            dbtnCldParasExtr.Items.Add(btnMODISL2);
            RadMenuItem btnAIRSL2 = new RadMenuItem("AIRS L2云参数产品数据");
            btnAIRSL2.Click += new EventHandler(btnAIRSL2_Click);
            dbtnCldParasExtr.Items.Add(btnAIRSL2);
            RadMenuItem btnMYD06 = new RadMenuItem("Aqua MOD06云参数产品数据");
            btnMYD06.Click += new EventHandler(btnMYD06_Click);
            dbtnCldParasExtr.Items.Add(btnMYD06);

            dbtnCldParasExtr.Text = "数据处理";
            dbtnCldParasExtr.TextAlignment = ContentAlignment.BottomCenter;
            dbtnCldParasExtr.ImageAlignment = ContentAlignment.TopCenter;
            #endregion
            rbgDataPro.Items.Add(dbtnCldParasExtr);

            //周期合成
            RadMenuItem btnMODISAM = new RadMenuItem("MOD06云参数产品周期合成");//terra
            btnMODISAM.Click += new EventHandler(btnMOD06CldPeriodMerge_Click);
            dbtnCldPeriodMerge.Items.Add(btnMODISAM);
            RadMenuItem btnMODISPM = new RadMenuItem("MYD06云参数产品周期合成");//aqua
            btnMODISPM.Click += new EventHandler(btnMYD06CldPeriodMerge_Click);
            dbtnCldPeriodMerge.Items.Add(btnMODISPM);
            RadMenuItem btnAIRSPM = new RadMenuItem("AIRS L2云参数产品周期合成");
            btnAIRSPM.Click += new EventHandler(btnAIRSCldPeriodMerge_Click);
            dbtnCldPeriodMerge.Items.Add(btnAIRSPM);
            dbtnCldPeriodMerge.Text = "周期合成";
            dbtnCldPeriodMerge.TextAlignment = ContentAlignment.BottomCenter;
            dbtnCldPeriodMerge.ImageAlignment = ContentAlignment.TopCenter;
            rbgDataPro.Items.Add(dbtnCldPeriodMerge);
            _tab.Items.Add(rbgDataPro);
        }

        private void CreateDataManagerGroup()
        {
            RadRibbonBarGroup rbgDataMana = new RadRibbonBarGroup();
            rbgDataMana.Text = "数据管理";
            btnCldDataBaseArgs.TextAlignment = ContentAlignment.BottomCenter;
            btnCldDataBaseArgs.ImageAlignment = ContentAlignment.TopCenter;
            btnCldDataBaseArgs.Click += new EventHandler(btnCldDataBaseArgs_Click);
            rbgDataMana.Items.Add(btnCldDataBaseArgs);//数据库配置

            btnData2MySQL.TextAlignment = ContentAlignment.BottomCenter;
            btnData2MySQL.ImageAlignment = ContentAlignment.TopCenter;
            btnData2MySQL.Click += new EventHandler(btnData2MySQL_Click);
            rbgDataMana.Items.Add(btnData2MySQL);//数据入库
            btnCldDataInquire.TextAlignment = ContentAlignment.BottomCenter;
            btnCldDataInquire.ImageAlignment = ContentAlignment.TopCenter;
            btnCldDataInquire.Click += new EventHandler(btnCldDataInquire_Click);
            rbgDataMana.Items.Add(btnCldDataInquire);//数据查询

            btnCldDataManage.TextAlignment = ContentAlignment.BottomCenter;
            btnCldDataManage.ImageAlignment = ContentAlignment.TopCenter;
            btnCldDataManage.Click += new EventHandler(btnCldDataManage_Click);
            rbgDataMana.Items.Add(btnCldDataManage);//数据清理
            _tab.Items.Add(rbgDataMana);
        }

        private void CreateCLDStatic()
        {
            RadRibbonBarGroup rbgStatic = new RadRibbonBarGroup();
            rbgStatic.Text = "统计分析";
            btnCldStatRegionSet.TextAlignment = ContentAlignment.BottomCenter;
            btnCldStatRegionSet.ImageAlignment = ContentAlignment.TopCenter;
            btnCldStatRegionSet.Click += new EventHandler(btnCldStatRegionSet_Click);
            rbgStatic.Items.Add(btnCldStatRegionSet);//数据入库

            RadMenuItem btnHistogramStatics = new RadMenuItem("直方图统计");
            btnHistogramStatics.Click += new EventHandler(btnHistogramStatics_Click);
            dbtnCldCommonStatics.Items.Add(btnHistogramStatics);
            RadMenuItem btnSeriesMeanStatics = new RadMenuItem("长时间序列均值统计");
            btnSeriesMeanStatics.Click += new EventHandler(btnSeriesMeanStatics_Click);
            dbtnCldCommonStatics.Items.Add(btnSeriesMeanStatics);

            RadMenuItem btnCorrelatStatics = new RadMenuItem("相关系数计算");
            btnCorrelatStatics.Click += new EventHandler(btnCorrelatStatics_Click);
            dbtnCldCommonStatics.Items.Add(btnCorrelatStatics);

            dbtnCldCommonStatics.Text = "常规统计分析";
            dbtnCldCommonStatics.TextAlignment = ContentAlignment.BottomCenter;
            dbtnCldCommonStatics.ImageAlignment = ContentAlignment.TopCenter;
            rbgStatic.Items.Add(dbtnCldCommonStatics);

            RadMenuItem btnSVDStatics = new RadMenuItem("SVD分解");
            btnSVDStatics.Click += new EventHandler(btnCldStatSingleData_Click);
            dbtnCldFieldStatics.Items.Add(btnSVDStatics);
            RadMenuItem btnSVDModeLayout = new RadMenuItem("SVD分解模态专题图");
            btnSVDModeLayout.Click += new EventHandler(btnSVDModeLayout_Click);
            dbtnCldFieldStatics.Items.Add(btnSVDModeLayout);

            RadMenuItem btnEOFStatics = new RadMenuItem("EOF分解");
            btnEOFStatics.Click += new EventHandler(btnEOFStatics_Click);
            dbtnCldFieldStatics.Items.Add(btnEOFStatics);

            dbtnCldFieldStatics.Text = "场分析";
            dbtnCldFieldStatics.TextAlignment = ContentAlignment.BottomCenter;
            dbtnCldFieldStatics.ImageAlignment = ContentAlignment.TopCenter;
            rbgStatic.Items.Add(dbtnCldFieldStatics);

            //波段散点图
            
            RadMenuItem btnBandsScatterSingle = new RadMenuItem("单文件");
            btnBandsScatterSingle.Click += new EventHandler(btnBandsScatter_Click);
            dbtnBandsScatters.Items.Add(btnBandsScatterSingle);
            RadMenuItem btnBandsScatterTwo = new RadMenuItem("文件间");
            btnBandsScatterTwo.Click += new EventHandler(btnBandsScatterTwo_Click);
            dbtnBandsScatters.Items.Add(btnBandsScatterTwo);
            dbtnBandsScatters.Text = "波段散点图";
            dbtnBandsScatters.TextAlignment = ContentAlignment.BottomCenter;
            dbtnBandsScatters.ImageAlignment = ContentAlignment.TopCenter;
            rbgStatic.Items.Add(dbtnBandsScatters);

            //等值线
            btnCldStatContour.TextAlignment = ContentAlignment.BottomCenter;
            btnCldStatContour.ImageAlignment = ContentAlignment.TopCenter;
            btnCldStatContour.Click += new EventHandler(btnCldStatContour_Click);
            rbgStatic.Items.Add(btnCldStatContour);
            //单点廓线
            RadMenuItem btnCloudsatSingleProfile = new RadMenuItem("Cloudsat单点廓线图");
            btnCloudsatSingleProfile.Click += new EventHandler(btnCloudsatSingleProfile_Click);
            dbtnSingleProfile.Items.Add(btnCloudsatSingleProfile);
            RadMenuItem btnAIRSSingleProfile = new RadMenuItem("AIRS单点廓线图");
            btnAIRSSingleProfile.Click += new EventHandler(btnAIRSSingleProfile_Click);
            dbtnSingleProfile.Items.Add(btnAIRSSingleProfile);
            dbtnSingleProfile.Text = "单点廓线图";
            dbtnSingleProfile.TextAlignment = ContentAlignment.BottomCenter;
            dbtnSingleProfile.ImageAlignment = ContentAlignment.TopCenter;
            //rbgStatic.Items.Add(dbtnSingleProfile);
            #region ISCCP专题图
            RadMenuItem btnLayoutTCA = new RadMenuItem("月平均总云量专题图");
            btnLayoutTCA.Click += new EventHandler(btnLayoutTCA_Click);
            dbtnCldLayout.Items.Add(btnLayoutTCA);
            RadMenuItem btnLayoutTC = new RadMenuItem("月平均云顶温度专题图");
            btnLayoutTC.Click += new EventHandler(btnLayoutTC_Click);
            dbtnCldLayout.Items.Add(btnLayoutTC);
            RadMenuItem btnLayoutPC = new RadMenuItem("月平均云顶压力专题图");
            btnLayoutPC.Click += new EventHandler(btnLayoutPC_Click);
            dbtnCldLayout.Items.Add(btnLayoutPC);
            RadMenuItem btnLayoutTAU = new RadMenuItem("月平均云光学厚度专题图");
            btnLayoutTAU.Click += new EventHandler(btnLayoutTAU_Click);
            dbtnCldLayout.Items.Add(btnLayoutTAU);
            RadMenuItem btnLayoutWP = new RadMenuItem("月平均云水路径专题图");
            btnLayoutWP.Click += new EventHandler(btnLayoutWP_Click);
            dbtnCldLayout.Items.Add(btnLayoutWP);
            RadMenuItem btnLayoutTS = new RadMenuItem("月平均地表温度专题图");
            btnLayoutTS.Click += new EventHandler(btnLayoutTS_Click);
            dbtnCldLayout.Items.Add(btnLayoutTS);
            RadMenuItem btnLayoutRS = new RadMenuItem("月平均可见光波段地表反射率专题图");
            btnLayoutRS.Click += new EventHandler(btnLayoutRS_Click);
            dbtnCldLayout.Items.Add(btnLayoutRS);
            RadMenuItem btnLayoutMSIA = new RadMenuItem("月平均冰雪覆盖度专题图");
            btnLayoutMSIA.Click += new EventHandler(btnLayoutMSIA_Click);
            dbtnCldLayout.Items.Add(btnLayoutMSIA);
            RadMenuItem btnLayoutMOCA = new RadMenuItem("月平均臭氧柱状浓度专题图");
            btnLayoutMOCA.Click += new EventHandler(btnLayoutMOCA_Click);
            dbtnCldLayout.Items.Add(btnLayoutMOCA);
            RadMenuItem btnLayoutMPWHigh = new RadMenuItem("月平均大气可降水(680-310mb)专题图");
            btnLayoutMPWHigh.Click += new EventHandler(btnLayoutMPWHigh_Click);
            dbtnCldLayout.Items.Add(btnLayoutMPWHigh);
            RadMenuItem btnLayoutMPWLow = new RadMenuItem("月平均大气可降水(1000-680mb)专题图");
            btnLayoutMPWLow.Click += new EventHandler(btnLayoutMPWLow_Click);
            dbtnCldLayout.Items.Add(btnLayoutMPWLow);
            RadMenuItem btnLayoutTCWV = new RadMenuItem("月平均总柱状水汽专题图");
            btnLayoutTCWV.Click += new EventHandler(btnLayoutTCWV_Click);
            dbtnCldLayout.Items.Add(btnLayoutTCWV);
            dbtnCldLayout.Text = "ISCCP D2专题图";
            dbtnCldLayout.TextAlignment = ContentAlignment.BottomCenter;
            dbtnCldLayout.ImageAlignment = ContentAlignment.TopCenter;
            //rbgStatic.Items.Add(dbtnCldLayout);
            #endregion
            _tab.Items.Add(rbgStatic);
        }

        private void AddToDbButton()
        {
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


        #region 按钮相关事件
        #region 数据处理
        void btnISCCPD2_Click(object sender, EventArgs e)
        {
            GPCPreProcess frm = new GPCPreProcess();
            frm.Show();
        }

        void btnMOD06_Click(object sender, EventArgs e)
        {
            try
            {
                frmMod06DataPro frm = new frmMod06DataPro();
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void btnMYD06_Click(object sender, EventArgs e)
        {
            try
            {
                frmMod06DataPro frm = new frmMod06DataPro("MYD06");
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void btnCloudSAT_Click(object sender, EventArgs e)
        {
            //frmMod06DataPro frm = new frmMod06DataPro();
            //frm.Show();
        }

        void btnAIRSL2_Click(object sender, EventArgs e)
        {
            try
            {
                frmMod06DataPro frm = new frmMod06DataPro("AIRS");
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Show();
                        }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void btnCldDataContinutyCheck_Click(object sender, EventArgs e)
        {
            DataContinuityCheck check = new DataContinuityCheck();
            check.StartPosition = FormStartPosition.CenterScreen;
            check.Show();
        }

        void btnMOD06CldPeriodMerge_Click(object sender, EventArgs e)
        {
            try
            {
                PeriodComp pm = new PeriodComp("MOD06");
                if (pm.ParseArgsXml())
                {
                    pm.StartPosition = FormStartPosition.CenterScreen;
                    pm.Show();
                }
                else
                    MessageBox.Show("文件配置出错，请确认配置文件存在！");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void btnMYD06CldPeriodMerge_Click(object sender, EventArgs e)
        {
            try
            {
                PeriodComp pm = new PeriodComp("MYD06");
                if (pm.ParseArgsXml())
                {
                    pm.StartPosition = FormStartPosition.CenterScreen;
                    pm.Show();
                }
                else
                    MessageBox.Show("文件配置出错，请确认配置文件存在！");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        void btnAIRSCldPeriodMerge_Click(object sender, EventArgs e)
        {
            try
            {
                PeriodComp pm = new PeriodComp("AIRS");
                if (pm.ParseArgsXml())
                {
                    pm.StartPosition = FormStartPosition.CenterScreen;
                    pm.Show();
                }
                else
                    MessageBox.Show("文件配置出错，请确认配置文件存在！");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region 数据查询与入库
        /// <summary>
        /// 数据查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCldDataInquire_Click(object sender, EventArgs e)
        {
            try
            {
                CLDPrdsDataRetrieval frm = new CLDPrdsDataRetrieval();
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm._openEventhandler += new CLDPrdsDataRetrieval.DoubleClickRowHdrEventHandler(OpenRetrievalFile);
                frm.Show();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void OpenRetrievalFile(object source, DoubleClickRowHdrEventArgs args)
        {
            try
            {
                ICommand cmd = _session.CommandEnvironment.Get(2000);
                if (cmd != null)
                    cmd.Execute(args.FullName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        /// <summary>
        /// 数据入库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnData2MySQL_Click(object sender, EventArgs e)
        {
            FileToDatabase frm = new FileToDatabase();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        /// <summary>
        /// 数据库配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCldDataBaseArgs_Click(object sender, EventArgs e)
        {
            DataBaseArgsSet frm = new DataBaseArgsSet();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        /// <summary>
        /// 归档数据清理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCldDataManage_Click(object sender, EventArgs e)
        {
            frmUniformDataManage frm = new frmUniformDataManage();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        #endregion

        #region 统计分析与专题图
        void btnCldStatRegionSet_Click(object sender, EventArgs e)
        {
            StatRegionSet frm = new StatRegionSet();
            frm.StartPosition = FormStartPosition.CenterScreen;
            frm.Show();
        }

        /// <summary>
        /// 直方图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnHistogramStatics_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(35201);
            if (cmd != null)
                cmd.Execute();
        }
                
                    /// <summary>
        /// 长时间序列均值统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSeriesMeanStatics_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(35218);
            if (cmd != null)
                cmd.Execute();
        }

        /// <summary>
        /// 相关性分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCorrelatStatics_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(35202);
            if (cmd != null)
                cmd.Execute();
        }

        /// <summary>
        ///波段散点图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnBandsScatter_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(7102);
            if (cmd != null)
                cmd.Execute();
        }

        void btnCldStatContour_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(7105);
            if (cmd != null)
                cmd.Execute();
        }

        /// <summary>
        /// 文件间波段散点图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnBandsScatterTwo_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(71020);
            if (cmd != null)
                cmd.Execute();
        }

        void btnCloudsatSingleProfile_Click(object sender, EventArgs e)
        {
            //string fname = null;
            //using (OpenFileDialog diag =new  OpenFileDialog())
            //{
            //    diag.InitialDirectory = @"E:\原始数据\CLOUDSAT\2007\4";
            //    diag.Multiselect = false;
            //    if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    {
            //        fname = (diag.FileName);
            //    }
            //}
            ICommand cmd = _session.CommandEnvironment.Get(29141);
            if (cmd != null)
                cmd.Execute();
        }

        void btnAIRSSingleProfile_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(29001);
            if (cmd != null)
                cmd.Execute();
        }


        /// <summary>
        /// SVD分解
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCldStatSingleData_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(35203);
            if (cmd != null)
                cmd.Execute();
        }

        void btnSVDModeLayout_Click(object sender, EventArgs e)
        {
            try
            {
                IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
                ms.ChangeActiveSubProduct("0IMG");
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MSVD");
                //GetCommandAndExecute(6602);//不显示面板
                ms.DoAutoExtract(false);
            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// EOF分解
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnEOFStatics_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(35205);
            if (cmd != null)
                cmd.Execute();
        }


        /// <summary>
        /// 总云量专题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLayoutTCA_Click(object sender, EventArgs e)
        {
            try
            {
                IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
                ms.ChangeActiveSubProduct("0IMG");
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "TCAM");
                //GetCommandAndExecute(6601);//不显示面板
                ms.DoAutoExtract(false);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 云顶温度专题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLayoutTC_Click(object sender, EventArgs e)
        {
            try
            {
                IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
                ms.ChangeActiveSubProduct("0IMG");
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0TCM");
                GetCommandAndExecute(6601);//不显示面板
            }
            catch
            {
            }
        }

        /// <summary>
        /// 云顶压力专题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLayoutPC_Click(object sender, EventArgs e)
        {
            try
            {
                IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
                ms.ChangeActiveSubProduct("0IMG");
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0PCM");
                GetCommandAndExecute(6601);//不显示面板
            }
            catch
            {
            }
        }

        /// <summary>
        /// 云光学厚度专题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLayoutTAU_Click(object sender, EventArgs e)
        {
            try
            {
                IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
                ms.ChangeActiveSubProduct("0IMG");
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "TAUM");
                GetCommandAndExecute(6601);//不显示面板
            }
            catch
            {
            }
        }

        /// <summary>
        /// 云水路径专题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLayoutWP_Click(object sender, EventArgs e)
        {
            try
            {
                IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
                ms.ChangeActiveSubProduct("0IMG");
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0WPM");
                GetCommandAndExecute(6601);//不显示面板
            }
            catch
            {
            }
        }

        /// <summary>
        /// 地表温度专题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLayoutTS_Click(object sender, EventArgs e)
        {
            try
            {
                IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
                ms.ChangeActiveSubProduct("0IMG");
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0TSM");
                GetCommandAndExecute(6601);//不显示面板
            }
            catch
            {
            }
        }

        /// <summary>
        /// 月平均可见光波段地表反射率专题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLayoutRS_Click(object sender, EventArgs e)
        {
            try
            {
                IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
                ms.ChangeActiveSubProduct("0IMG");
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0RSM");
                GetCommandAndExecute(6601);//不显示面板
            }
            catch
            {
            }
        }

        /// <summary>
        /// 月平均冰雪覆盖度专题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLayoutMSIA_Click(object sender, EventArgs e)
        {
            try
            {
                IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
                ms.ChangeActiveSubProduct("0IMG");
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MSIA");
                GetCommandAndExecute(6601);//不显示面板
            }
            catch
            {
            }
        }

        /// <summary>
        /// 月平均臭氧柱状浓度专题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLayoutMOCA_Click(object sender, EventArgs e)
        {
            try
            {
                IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
                ms.ChangeActiveSubProduct("0IMG");
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MOCA");
                GetCommandAndExecute(6601);//不显示面板
            }
            catch
            {
            }
        }

        /// <summary>
        /// 月平均大气可降水(680-310mb)专题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLayoutMPWHigh_Click(object sender, EventArgs e)
        {
            try
            {
                IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
                ms.ChangeActiveSubProduct("0IMG");
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MPWH");
                GetCommandAndExecute(6601);//不显示面板
            }
            catch
            {
            }
        }

        /// <summary>
        /// 月平均大气可降水(1000-680mb)专题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLayoutMPWLow_Click(object sender, EventArgs e)
        {
            try
            {
                IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
                ms.ChangeActiveSubProduct("0IMG");
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MPWL");
                GetCommandAndExecute(6601);//不显示面板
            }
            catch
            {
            }
        }

        /// <summary>
        /// 月平均总柱状水汽专题图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLayoutTCWV_Click(object sender, EventArgs e)
        {
            try
            {
                IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
                ms.ChangeActiveSubProduct("0IMG");
                ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "TCWV");
                GetCommandAndExecute(6601);//不显示面板
            }
            catch
            {
            }
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
        #endregion

        private void GetCommandAndExecute(int cmdID)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute("CLD");
        }

        private void GetCommandAndExecute(int cmdID, string template)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute(template);
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
            AddCloseButton();
            SetImage();
        }

        private void AddThemeGraphRegionBotton()
        {
            RadDropDownButtonElement _btnThemeGraphRegion = new RadDropDownButtonElement();
            ThemeGraphRegionFacctory.RegistToButton("CLD", _btnThemeGraphRegion, _tab, _session);
            _btnThemeGraphRegion.Image = _session.UIFrameworkHelper.GetImage("system:settings.png");
        }

        public void UpdateStatus()
        { }

        private void SetImage()
        {
            //连续性检测
            btnCldDataContinutyCheck.Image = _session.UIFrameworkHelper.GetImage("system:activity_window.png");
            //周期合成
            dbtnCldPeriodMerge.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            //预处理
            dbtnCldParasExtr.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            //查询
            btnCldDataInquire.Image = _session.UIFrameworkHelper.GetImage("system:FileType_Normal.png");
            //数据清理
            btnCldDataManage.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_清除判识结果.png");
            btnData2MySQL.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
            //数据库配置
            btnCldDataBaseArgs.Image = _session.UIFrameworkHelper.GetImage("system:do.png");
            //btnCldStatSingleData.Image = _session.UIFrameworkHelper.GetImage("system:fogErad.png");
            //统计区域设置
            btnCldStatRegionSet.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_导入AOI.png");
            //等值线
            btnCldStatContour.Image = _session.UIFrameworkHelper.GetImage("system:Tool_Contour.png");
            //波段散点图
            dbtnBandsScatters.Image = _session.UIFrameworkHelper.GetImage("system:Tool_Scatter.png");
            dbtnCldFieldStatics.Image = _session.UIFrameworkHelper.GetImage("system:fogCycTime.png");
            //统计分析
            dbtnCldCommonStatics.Image = _session.UIFrameworkHelper.GetImage("system:comList.png");
            dbtnCldLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
        }

        #endregion


    }
}
