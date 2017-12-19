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
using GeoDo.RSS.MIF.Prds.FIR;
using GeoDo.RSS.DI;
using GeoDo.RSS.MIF.Prds.Comm;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.DF.MicapsData;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public partial class UITabFire : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 火情按钮定义
        RadButtonElement btnAutoExtract = new RadButtonElement("自动判识");
        RadDropDownButtonElement btnAutoGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement btnDayAutoGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement btnEvenAutoGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement btnCyctimeAutoGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement btnOtherAutoGenerate = new RadDropDownButtonElement();
        RadDropDownButtonElement btnManualExtract = new RadDropDownButtonElement();
        RadMenuItem btnManualExtract_FIR = new RadMenuItem("火");
        RadMenuItem btnManualExtract_FIRG = new RadMenuItem("过火区");
        RadMenuItem btnManualExtract_SMOK = new RadMenuItem("烟尘");
        RadMenuItem btnManualExtract_CLM = new RadMenuItem("云");
        RadMenuItem btnManualExtract_FLARE = new RadMenuItem("耀斑角");

        RadButtonElement btnImport = new RadButtonElement("导入");
        RadDropDownButtonElement dbtngFirepoint = new RadDropDownButtonElement();

        RadButtonElement btnMulImage = new RadButtonElement("多通道合成图");
        RadDropDownButtonElement dbtnbinImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnZbinImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnCycbinImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnOtherbinImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnStrenImage = new RadDropDownButtonElement();
        RadButtonElement btnFirepList = new RadButtonElement("信息列表");
        RadDropDownButtonElement btnCommFirepList = new RadDropDownButtonElement();
        RadButtonElement btnComChart = new RadButtonElement("对比图");
        RadButtonElement btnComList = new RadButtonElement("对比列表");
        RadButtonElement btnCurrentRegionArea = new RadButtonElement("当前区域");
        RadDropDownButtonElement dbtnDivision = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnZDivision = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnCycDivision = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnOtherDivision = new RadDropDownButtonElement();
        RadButtonElement btnLandType = new RadButtonElement("土地类型");
        RadButtonElement btnCustomArea = new RadButtonElement("自定义");
        RadButtonElement btnCurrentRegionFreq = new RadButtonElement("当前区域");
        RadButtonElement btnCustomFreq = new RadButtonElement("自定义");
        RadButtonElement btnClose = new RadButtonElement("关闭火情监测\n分析视图");
        RadButtonElement btnConfig = new RadButtonElement("配置");
        RadButtonElement btnToDb = new RadButtonElement("产品入库");

        RadDropDownButtonElement btnstraw = new RadDropDownButtonElement();
        RadButtonElement btnManualExtract_FIRF = new RadButtonElement("过火程度计算");
        RadDropDownButtonElement dbtnFLZbinDataCalc = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnFLZbinImage = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnFLZDivision = new RadDropDownButtonElement();

        RadDropDownButtonElement btnAddProduct = new RadDropDownButtonElement();

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
                btnManualExtract_CLM,
                btnManualExtract_FIRG,
                btnManualExtract_SMOK,
                btnManualExtract_FLARE,

            });
            rbgCheck.Items.Add(btnManualExtract);
            //
            btnManualExtract_FIR.Click += new EventHandler(btnManualExtract_FIR_Click);
            btnManualExtract_FIRG.Click += new EventHandler(btnManualExtract_FIRG_Click);
            btnManualExtract_SMOK.Click += new EventHandler(btnManualExtract_SMOK_Click);
            btnManualExtract_CLM.Click += new EventHandler(btnManualExtract_CLM_Click);

            btnManualExtract_FLARE.Click += new EventHandler(btnManualExtract_FLARE_Click);
            //
            btnAutoGenerate.Text = "快速生成";
            btnAutoGenerate.ImageAlignment = ContentAlignment.TopCenter;
            btnAutoGenerate.TextAlignment = ContentAlignment.BottomCenter;
            btnAutoGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem roughProGenerate = new RadMenuItem("自动判识+专题产品");
            roughProGenerate.Click += new EventHandler(RoughProGenerate_Click);
            btnAutoGenerate.Items.Add(roughProGenerate);
            RadMenuItem proAutoGenerate = new RadMenuItem("专题产品");
            proAutoGenerate.Click += new EventHandler(ProAutoGenerate_Click);
            btnAutoGenerate.Items.Add(proAutoGenerate);
            rbgCheck.Items.Add(btnAutoGenerate);

            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "日常业务";
            btnFirepList.ImageAlignment = ContentAlignment.TopCenter;
            btnFirepList.TextAlignment = ContentAlignment.BottomCenter;
            btnFirepList.Click += new EventHandler(btnFirepList_Click);
            rbgProduct.Items.Add(btnFirepList);
            dbtnbinImage.Text = "专题产品";
            dbtnbinImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnbinImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnbinImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem netRGBLayout = new RadMenuItem("网络合成图");
            dbtnbinImage.Items.Add(netRGBLayout);
            netRGBLayout.Click += new EventHandler(netRGBLayout_Click);
            RadMenuItem RGBLayout = new RadMenuItem("多通道合成图");
            //dbtnbinImage.Items.Add(RGBLayout);
            RGBLayout.Click += new EventHandler(btnRGBLayout_Click);
            RadMenuItem mhiBinImgCurrent = new RadMenuItem("二值图(当前区域)");
            //dbtnbinImage.Items.Add(mhiBinImgCurrent);
            mhiBinImgCurrent.Click += new EventHandler(mhiBinImgCurrent_Click);
            RadMenuItem mhiStrenImgCurrent = new RadMenuItem("强度图(当前区域)");
            mhiStrenImgCurrent.Click += new EventHandler(mhiStrenImgCurrent_Click);
            //dbtnbinImage.Items.Add(mhiStrenImgCurrent);
            rbgProduct.Items.Add(dbtnbinImage);
            dbtnDivision.Text = "统计分析";
            dbtnDivision.TextAlignment = ContentAlignment.BottomCenter;
            dbtnDivision.ImageAlignment = ContentAlignment.TopCenter;
            RadMenuItem mhiCurrentRegionArea = new RadMenuItem("当前区域");
            mhiCurrentRegionArea.Click += new EventHandler(btnCurrentRegionArea_Click);
            dbtnDivision.Items.Add(mhiCurrentRegionArea);
            RadMenuItem mhiLandType = new RadMenuItem("土地分类");
            mhiLandType.Click += new EventHandler(btnLandType_Click);
            //dbtnDivision.Items.Add(mhiLandType);
            RadMenuItem mhiProvincBound = new RadMenuItem("省界分类");
            mhiProvincBound.Click += new EventHandler(mhiProvincBound_Click);
            //dbtnDivision.Items.Add(mhiProvincBound);
            RadMenuItem mhiCity = new RadMenuItem("市县分级");
            mhiCity.Click += new EventHandler(mhiCity_Click);
            //dbtnDivision.Items.Add(mhiCity);
            string cityName = AreaStatProvider.GetAreaStatItemMenuName("三级行政区划");
            RadMenuItem mhiSxTj = new RadMenuItem(cityName + "（栅格）");
            mhiSxTj.Click += new EventHandler(mhiSxTj_Click);
            //dbtnDivision.Items.Add(mhiSxTj);
            rbgProduct.Items.Add(dbtnDivision);

            btnDayAutoGenerate.Text = "快速生成";
            btnDayAutoGenerate.ImageAlignment = ContentAlignment.TopCenter;
            btnDayAutoGenerate.TextAlignment = ContentAlignment.BottomCenter;
            btnDayAutoGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem roughDayProGenerate = new RadMenuItem("自动判识+专题产品");
            roughDayProGenerate.Click += new EventHandler(roughDayProGenerate_Click);
            btnDayAutoGenerate.Items.Add(roughDayProGenerate);
            RadMenuItem proDayAutoGenerate = new RadMenuItem("专题产品");
            proDayAutoGenerate.Click += new EventHandler(proDayAutoGenerate_Click);
            btnDayAutoGenerate.Items.Add(proDayAutoGenerate);
            rbgProduct.Items.Add(btnDayAutoGenerate);

            RadRibbonBarGroup rbgComparaAnaly = new RadRibbonBarGroup();
            rbgComparaAnaly.Text = "灾情事件";
            btnCommFirepList.Text = "信息列表";
            btnCommFirepList.ArrowPosition = DropDownButtonArrowPosition.Right;
            btnCommFirepList.ImageAlignment = ContentAlignment.TopCenter;
            btnCommFirepList.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiFireList = new RadMenuItem("火点信息列表");
            btnCommFirepList.Items.Add(mhiFireList);
            mhiFireList.Click += new EventHandler(btnFirepList_Click);
            RadMenuItem mhiComList = new RadMenuItem("对比火点列表");
            btnCommFirepList.Items.Add(mhiComList);
            mhiComList.Click += new EventHandler(btnFirepList_Click);
            rbgComparaAnaly.Items.Add(btnCommFirepList);
            dbtnZbinImage.Text = "专题产品";
            dbtnZbinImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnZbinImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnZbinImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiZComChart = new RadMenuItem("对比图(当前区域)");
            dbtnZbinImage.Items.Add(mhiZComChart);
            mhiZComChart.Click += new EventHandler(btnComChart_Click);
            RadMenuItem RGBZLayout = new RadMenuItem("多通道合成图");
            dbtnZbinImage.Items.Add(RGBZLayout);
            RGBZLayout.Click += new EventHandler(btnRGBLayout_Click);
            RadMenuItem netOMulImage = new RadMenuItem("网络合成图");
            dbtnZbinImage.Items.Add(netOMulImage);
            netOMulImage.Click += new EventHandler(netRGBLayout_Click);
            RadMenuItem mhiZBinImgCurrent = new RadMenuItem("二值图(当前区域)");
            dbtnZbinImage.Items.Add(mhiZBinImgCurrent);
            mhiZBinImgCurrent.Click += new EventHandler(mhiBinImgCurrent_Click);
            RadMenuItem mhiZBinImgCustom = new RadMenuItem("二值图(自定义)");
            dbtnZbinImage.Items.Add(mhiZBinImgCustom);
            mhiZBinImgCustom.Click += new EventHandler(mhiBinImgCustom_Click);
            RadMenuItem mhiZStrenImgCurrent = new RadMenuItem("强度图(当前区域)");
            mhiZStrenImgCurrent.Click += new EventHandler(mhiStrenImgCurrent_Click);
            dbtnZbinImage.Items.Add(mhiZStrenImgCurrent);
            RadMenuItem mhiZStrenImgCustom = new RadMenuItem("强度图(自定义)");
            mhiZStrenImgCustom.Click += new EventHandler(mhiStrenImgCustom_Click);
            dbtnZbinImage.Items.Add(mhiZStrenImgCustom);
            RadMenuItem mhiFIRGImgCurrent = new RadMenuItem("过火区图(当前区域)");
            dbtnZbinImage.Items.Add(mhiFIRGImgCurrent);
            mhiFIRGImgCurrent.Click += new EventHandler(mhiFIRGImgCurrent_Click);
            rbgComparaAnaly.Items.Add(dbtnZbinImage);
            dbtnZDivision.Text = "统计分析";
            dbtnZDivision.TextAlignment = ContentAlignment.BottomCenter;
            dbtnZDivision.ImageAlignment = ContentAlignment.TopCenter;
            RadMenuItem mhiZCurrentRegionArea = new RadMenuItem("当前区域");
            mhiZCurrentRegionArea.Click += new EventHandler(btnCurrentRegionArea_Click);
            dbtnZDivision.Items.Add(mhiZCurrentRegionArea);
            RadMenuItem mhiZLandType = new RadMenuItem("土地分类");
            mhiZLandType.Click += new EventHandler(btnLandType_Click);
            dbtnZDivision.Items.Add(mhiZLandType);
            RadMenuItem mhiZProvincBound = new RadMenuItem("省界分类");
            mhiZProvincBound.Click += new EventHandler(mhiProvincBound_Click);
            dbtnZDivision.Items.Add(mhiZProvincBound);
            RadMenuItem mhiZCity = new RadMenuItem("市县分级");
            mhiZCity.Click += new EventHandler(mhiCity_Click);
            dbtnZDivision.Items.Add(mhiZCity);
            RadMenuItem mhiDisSxTj = new RadMenuItem(cityName + "（栅格）");
            mhiDisSxTj.Click += new EventHandler(mhiSxTj_Click);
            dbtnZDivision.Items.Add(mhiDisSxTj);
            RadMenuItem mhiZCustomArea = new RadMenuItem("自定义");
            mhiZCustomArea.Click += new EventHandler(btnCustomArea_Click);
            dbtnZDivision.Items.Add(mhiZCustomArea);
            rbgComparaAnaly.Items.Add(dbtnZDivision);

            btnEvenAutoGenerate.Text = "快速生成";
            btnEvenAutoGenerate.ImageAlignment = ContentAlignment.TopCenter;
            btnEvenAutoGenerate.TextAlignment = ContentAlignment.BottomCenter;
            btnEvenAutoGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem roughEvenProGenerate = new RadMenuItem("自动判识+专题产品");
            roughEvenProGenerate.Click += new EventHandler(roughEvenProGenerate_Click);
            btnEvenAutoGenerate.Items.Add(roughEvenProGenerate);
            RadMenuItem proEvenAutoGenerate = new RadMenuItem("专题产品");
            proEvenAutoGenerate.Click += new EventHandler(proEvenAutoGenerate_Click);
            btnEvenAutoGenerate.Items.Add(proEvenAutoGenerate);
            rbgComparaAnaly.Items.Add(btnEvenAutoGenerate);

            #region 过火程度
            //秸秆焚烧
            btnstraw.Text = "过火程度交互判识";
            btnstraw.ArrowPosition = DropDownButtonArrowPosition.Right;
            btnstraw.ImageAlignment = ContentAlignment.TopCenter;
            btnstraw.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem btndatacollection = new RadMenuItem("高分数据集合成");
            btnstraw.Items.Add(btndatacollection);
            btndatacollection.Click += new EventHandler(btndatacollection_Click);

            RadRibbonBarGroup rbgFIRGLevel = new RadRibbonBarGroup();
            rbgFIRGLevel.Text = "过火程度";
            btnManualExtract_FIRF.ImageAlignment = ContentAlignment.TopCenter;
            btnManualExtract_FIRF.TextAlignment = ContentAlignment.BottomCenter;
            btnManualExtract_FIRF.Click += new EventHandler(btnManualExtract_FIRF_Click);
            rbgFIRGLevel.Items.Add(btnManualExtract_FIRF);
            //
            dbtnFLZbinDataCalc.Text = "数据合成";
            dbtnFLZbinDataCalc.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnFLZbinDataCalc.ImageAlignment = ContentAlignment.TopCenter;
            dbtnFLZbinDataCalc.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiCalcFLMX = new RadMenuItem("最大值合成");
            dbtnFLZbinDataCalc.Items.Add(mhiCalcFLMX);
            mhiCalcFLMX.Click += new EventHandler(mhiCalcFLMX_Click);
            RadMenuItem mhiCalcFLMI = new RadMenuItem("最小值合成");
            dbtnFLZbinDataCalc.Items.Add(mhiCalcFLMI);
            mhiCalcFLMI.Click += new EventHandler(mhiCalcFLMI_Click);
            RadMenuItem mhiCalcFLAV = new RadMenuItem("平均值合成");
            dbtnFLZbinDataCalc.Items.Add(mhiCalcFLAV);
            mhiCalcFLAV.Click += new EventHandler(mhiCalcFLAV_Click);
            RadMenuItem mhiCalcFLAN = new RadMenuItem("距平计算");
            dbtnFLZbinDataCalc.Items.Add(mhiCalcFLAN);
            mhiCalcFLAN.Click += new EventHandler(mhiCalcFLAN_Click);
            RadMenuItem mhiCalcFLCY = new RadMenuItem("差异计算");
            dbtnFLZbinDataCalc.Items.Add(mhiCalcFLCY);
            mhiCalcFLCY.Click += new EventHandler(mhiCalcFLCY_Click);
            rbgFIRGLevel.Items.Add(dbtnFLZbinDataCalc);
            //
            dbtnFLZbinImage.Text = "专题产品";
            dbtnFLZbinImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnFLZbinImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnFLZbinImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiFLFIRFImage = new RadMenuItem("过火区图");
            dbtnFLZbinImage.Items.Add(mhiFLFIRFImage);
            mhiFLFIRFImage.Click += new EventHandler(mhiFLFIRFImage_Click);
            RadMenuItem mhiFLFIRLevelImage = new RadMenuItem("过火程度图");
            mhiFLFIRLevelImage.Click += new EventHandler(mhiFLFIRLevelImage_Click);
            dbtnFLZbinImage.Items.Add(mhiFLFIRLevelImage);
            RadMenuItem RGBFLZLayout = new RadMenuItem("多通道合成图(原分)");
            dbtnFLZbinImage.Items.Add(RGBFLZLayout);
            RGBFLZLayout.Click += new EventHandler(RGBFLZLayout_Click);
            RadMenuItem mhiFLFIRFOImage = new RadMenuItem("过火区图(原分)");
            dbtnFLZbinImage.Items.Add(mhiFLFIRFOImage);
            mhiFLFIRFOImage.Click += new EventHandler(mhiFLFIRFOImage_Click);
            RadMenuItem mhiFLFIRLevelOImage = new RadMenuItem("过火程度图(原分)");
            mhiFLFIRLevelOImage.Click += new EventHandler(mhiFLFIRLevelOImage_Click);
            dbtnFLZbinImage.Items.Add(mhiFLFIRLevelOImage);
            RadMenuItem mhiPrvFLFIRLevelOImage = new RadMenuItem("过火程度图(省原分)");
            mhiPrvFLFIRLevelOImage.Click += new EventHandler(mhiPrvFLFIRLevelOImage_Click);
            dbtnFLZbinImage.Items.Add(mhiPrvFLFIRLevelOImage);
            rbgFIRGLevel.Items.Add(dbtnFLZbinImage);
            //
            dbtnFLZDivision.Text = "过火程度统计";
            dbtnFLZDivision.TextAlignment = ContentAlignment.BottomCenter;
            dbtnFLZDivision.ImageAlignment = ContentAlignment.TopCenter;
            RadMenuItem mhiFLZCurrentRegionArea = new RadMenuItem("当前区域");
            mhiFLZCurrentRegionArea.Click += new EventHandler(btnCurrentFLRegionArea_Click);
            dbtnFLZDivision.Items.Add(mhiFLZCurrentRegionArea);
            RadMenuItem mhiFLZLandType = new RadMenuItem("土地分类");
            mhiFLZLandType.Click += new EventHandler(btnFLLandType_Click);
            dbtnFLZDivision.Items.Add(mhiFLZLandType);
            RadMenuItem mhiFLZProvincBound = new RadMenuItem("省界分类");
            mhiFLZProvincBound.Click += new EventHandler(mhiFLProvincBound_Click);
            dbtnFLZDivision.Items.Add(mhiFLZProvincBound);
            RadMenuItem mhiFLZCity = new RadMenuItem("市县分级");
            mhiFLZCity.Click += new EventHandler(mhiFLCity_Click);
            dbtnFLZDivision.Items.Add(mhiFLZCity);
            rbgFIRGLevel.Items.Add(dbtnFLZDivision);

            #endregion
            //

            RadRibbonBarGroup rbgFreqStatic = new RadRibbonBarGroup();
            rbgFreqStatic.Text = "周期业务";
            btnCurrentRegionFreq.ImageAlignment = ContentAlignment.TopCenter;
            btnCurrentRegionFreq.TextAlignment = ContentAlignment.BottomCenter;
            dbtnCycbinImage.Text = "专题产品";
            dbtnCycbinImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnCycbinImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCycbinImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiCurrentRegionFreq = new RadMenuItem("频次图(当前区域)");
            dbtnCycbinImage.Items.Add(mhiCurrentRegionFreq);
            mhiCurrentRegionFreq.Click += new EventHandler(btnCurrentRegionFreq_Click);
            RadMenuItem mhiCustomFreq = new RadMenuItem("频次图(自定义)");
            dbtnCycbinImage.Items.Add(mhiCustomFreq);
            mhiCustomFreq.Click += new EventHandler(btnCustomFreq_Click);
            rbgFreqStatic.Items.Add(dbtnCycbinImage);
            btnCyctimeAutoGenerate.Text = "快速生成";
            btnCyctimeAutoGenerate.ImageAlignment = ContentAlignment.TopCenter;
            btnCyctimeAutoGenerate.TextAlignment = ContentAlignment.BottomCenter;
            btnCyctimeAutoGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem proCycAutoGenerate = new RadMenuItem("专题产品");
            proCycAutoGenerate.Click += new EventHandler(proCycAutoGenerate_Click);
            btnCyctimeAutoGenerate.Items.Add(proCycAutoGenerate);
            //rbgFreqStatic.Items.Add(btnCyctimeAutoGenerate);
            //
            RadRibbonBarGroup rbglevelProduct = new RadRibbonBarGroup();
            rbglevelProduct.Text = "其他业务";
            btnAddProduct.Text = "数据加载";
            btnAddProduct.ArrowPosition = DropDownButtonArrowPosition.Right;
            btnAddProduct.ImageAlignment = ContentAlignment.TopCenter;
            btnAddProduct.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiAutoAdd = new RadMenuItem("自动加载");
            btnAddProduct.Items.Add(mhiAutoAdd);
            mhiAutoAdd.Click += new EventHandler(mhiAutoAdd_Click);
            RadMenuItem mhiManualAdd = new RadMenuItem("手动加载");
            btnAddProduct.Items.Add(mhiManualAdd);
            mhiManualAdd.Click += new EventHandler(mhiManualAdd_Click);
            RadMenuItem mhiArgSetting = new RadMenuItem("参数配置");
            btnAddProduct.Items.Add(mhiArgSetting);
            mhiAutoAdd.Click += new EventHandler(mhiArgSetting_Click);
            rbglevelProduct.Items.Add(btnAddProduct);
            //btnImport.ImageAlignment = ContentAlignment.TopCenter;
            //btnImport.TextAlignment = ContentAlignment.BottomCenter;
            //rbglevelProduct.Items.Add(btnImport);
            dbtngFirepoint.Text = "全球火点";
            dbtngFirepoint.TextAlignment = ContentAlignment.BottomCenter;
            dbtngFirepoint.ImageAlignment = ContentAlignment.TopCenter;
            RadMenuItem mhiCorrect = new RadMenuItem("数据修正");
            dbtngFirepoint.Items.Add(mhiCorrect);
            mhiCorrect.Click += new EventHandler(mhiCorrect_Click);
            RadMenuItem mhiPointStren = new RadMenuItem("火点强度专题图");
            mhiPointStren.Click += new EventHandler(mhiPointStren_Click);
            dbtngFirepoint.Items.Add(mhiPointStren);
            RadMenuItem mhiPointFreq = new RadMenuItem("频次累计专题图");
            mhiPointFreq.Click += new EventHandler(mhiPointFreq_Click);
            dbtngFirepoint.Items.Add(mhiPointFreq);
            RadMenuItem mhiPointDay = new RadMenuItem("频次累计大洲统计");
            mhiPointDay.Click += new EventHandler(mhiPointDay_Click);
            dbtngFirepoint.Items.Add(mhiPointDay);
            rbglevelProduct.Items.Add(dbtngFirepoint);
            _tab.Items.Add(rbgCheck);
            _tab.Items.Add(rbgProduct);
            _tab.Items.Add(rbgComparaAnaly);
            _tab.Items.Add(rbgFIRGLevel);
            _tab.Items.Add(rbgFreqStatic);
            _tab.Items.Add(rbglevelProduct);

            btnToDb.TextAlignment = ContentAlignment.BottomCenter;
            btnToDb.ImageAlignment = ContentAlignment.TopCenter;
            btnToDb.Click += new EventHandler(btnToDb_Click);
            RadRibbonBarGroup rbgToDb = new RadRibbonBarGroup();
            rbgToDb.Text = "产品入库";
            rbgToDb.Items.Add(btnToDb);
            _tab.Items.Add(rbgToDb);

            Controls.Add(_bar);
        }

        /// <summary>
        /// 设置专题图是否固定区域
        /// </summary>
        private bool _isFixImageRegion = false;

        void netRGBLayout_Click(object sender, EventArgs e)
        {
            SetIsFixImageRegion();
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "NCSI");
            ms.DoAutoExtract(false);
        }

        private void SetIsFixImageRegion()
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("FixImageRegion", _isFixImageRegion ? "true" : "false");
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
            AddThemeGraphRegionBotton();
            CreateqQuickReportRegionButton();
            AddCloseButton();
            SetImage();
        }

        private void CreateqQuickReportRegionButton()
        {
            QuickReportFactory.RegistToButton("FIR", _tab, _session);
        }

        private void AddThemeGraphRegionBotton()
        {
            RadDropDownButtonElement _btnThemeGraphRegion = new RadDropDownButtonElement();
            ThemeGraphRegionFacctory.RegistToButton("FIR", _btnThemeGraphRegion, _tab, _session);
            _btnThemeGraphRegion.Image = _session.UIFrameworkHelper.GetImage("system:settings.png");
        }

        private void AddCloseButton()
        {
            RadRibbonBarGroup rbgClose = new RadRibbonBarGroup();
            rbgClose.Text = "关闭";
            //btnConfig.TextAlignment = ContentAlignment.BottomCenter;
            //btnConfig.ImageAlignment = ContentAlignment.TopCenter;
            //btnConfig.Click += new EventHandler(btnClose_Click);
            //rbgClose.Items.Add(btnConfig);
            btnClose.TextAlignment = ContentAlignment.BottomCenter;
            btnClose.ImageAlignment = ContentAlignment.TopCenter;
            btnClose.Click += new EventHandler(btnClose_Click);
            rbgClose.Items.Add(btnClose);
            _tab.Items.Add(rbgClose);
        }

        private void SetImage()
        {
            btnAutoExtract.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            btnConfig.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            dbtnDivision.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            dbtnZDivision.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            dbtnCycDivision.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            btnAutoGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnDayAutoGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnEvenAutoGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnCyctimeAutoGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnOtherAutoGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnAddProduct.Image = _session.UIFrameworkHelper.GetImage("system:import.png");
            //btnImport.Image = _session.UIFrameworkHelper.GetImage("system:import.png");
            btnManualExtract.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            btnMulImage.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnbinImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            dbtnZbinImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            dbtnCycbinImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            dbtngFirepoint.Image = _session.UIFrameworkHelper.GetImage("system:PGS.png");
            btnCurrentRegionArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            btnLandType.Image = _session.UIFrameworkHelper.GetImage("system:landType.png");
            btnCustomArea.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnCustomFreq.Image = _session.UIFrameworkHelper.GetImage("system:custom.png");
            btnFirepList.Image = _session.UIFrameworkHelper.GetImage("system:firePointList.png");
            btnCommFirepList.Image = _session.UIFrameworkHelper.GetImage("system:firePointList.png");
            btnComChart.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
            btnComList.Image = _session.UIFrameworkHelper.GetImage("system:comList.png");
            btnCurrentRegionFreq.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
            dbtnStrenImage.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");

            btnManualExtract_FIRF.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            dbtnFLZbinDataCalc.Image = _session.UIFrameworkHelper.GetImage("system:wydpz.png");
            dbtnFLZbinImage.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            dbtnFLZDivision.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
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

        //报告素材生成
        void btn2Report_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(400002);
            if (cmd != null)
                cmd.Execute();
        }

        //火情产品多通道合成图
        void btnRGBLayout_Click(object sender, EventArgs e)
        {
            //MIFCommAnalysis.DisplayMonitorShow(_session, "template:火情产品多通道合成图,多通道合成图,FIR,MCSI");

            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MCSI");
            ms.DoAutoExtract(false);
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

        void btnManualExtract_CLM_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0CLM");
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
            GetCommandAndExecute(6602);
        }

        private void GetCommandAndExecute(int cmdID)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute("FIR");
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
        /// 日常业务粗判+专题产品快速生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void roughDayProGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "DBLV", null, "Ord");
        }

        /// <summary>
        /// 日常业务专题产品快速生成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void proDayAutoGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "0IMG", null, "Ord");
        }

        void roughEvenProGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "DBLV", null, "Dis");
        }

        void proEvenAutoGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "0IMG", null, "Dis");
        }

        void proCycAutoGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "FREQ", null, "Cyc");
        }
        /// <summary>
        /// 火情当前区域二值图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiBinImgCurrent_Click(object sender, EventArgs e)
        {
            //if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "FIRDBLV", "DBLV", "火情监测专题图模板", false, true) == null)
            //    return;
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0SDI");
            ms.DoAutoExtract(false);
        }

        void mhiFIRGImgCurrent_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "FRGI");
            ms.DoAutoExtract(false);
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
            if (MIFCommAnalysis.AreaStat(_session, "0CCB", "分级行政区划", false, true) == null)
                return;
        }

        #region 过程程度计算按钮

        void btnManualExtract_FIRF_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("FIRF");
            ICommand cmd = _session.CommandEnvironment.Get(6602);
            if (cmd != null)
                cmd.Execute();
        }
        /// <summary>
        /// 高分数据集合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btndatacollection_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("FLMX");
            GetCommandAndExecute(6602);
        }
        /// <summary>
        /// 过火程度最大值合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiCalcFLMX_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("FLMX");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 过火程度最小值合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiCalcFLMI_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("FLMI");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 过火程度平均值合成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiCalcFLAV_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("FLAV");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 过火程度距平计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiCalcFLAN_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("FLAN");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 过火程度差异计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiCalcFLCY_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("FLCY");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 过火程度多通道合成图（原分辨率）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RGBFLZLayout_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "OMCS");
            ms.DoAutoExtract(true);
        }

        /// <summary>
        /// 过火区图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiFLFIRFOImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "OFFR", "", "FIRF", "过火程度过火区专题图模板(原分)", false, true) == null)
                return;
        }

        /// <summary>
        /// 过火程度图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiFLFIRLevelOImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "OFFL", "", "FIFL", "过火程度专题图模板(原分)", false, true) == null)
                return;
        }

        void mhiPrvFLFIRLevelOImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "OCFL", "", "FIFL", "过火程度专题图模板(省原分)", false, true) == null)
                return;
        }

        /// <summary>
        /// 过火区图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiFLFIRFImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "FFRI", "", "FIRF", "过火程度过火区专题图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 过火程度图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiFLFIRLevelImage_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "FFLI", "", "FIFL", "过火程度专题图模板", false, true) == null)
                return;
        }

        /// <summary>
        /// 过火程度按市县面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiFLCity_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("FSTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "FCCC");
            ms.DoManualExtract(true);
        }

        /// <summary>
        /// 省市县统计（基于栅格和二值图）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiSxTj_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0CCC");
            ms.DoAutoExtract(true);
        }

        //
        /// <summary>
        /// 过火程度当前区域面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCurrentFLRegionArea_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("FSTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "FCAR");
            ms.DoManualExtract(true);
        }

        /// <summary>
        /// 过火程度土地利用类型面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnFLLandType_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("FSTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "FCLU");
            ms.DoManualExtract(true);
        }

        /// <summary>
        /// 过火程度按省界面积统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiFLProvincBound_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("FSTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "FCBP");
            ms.DoManualExtract(true);
        }

        #endregion



        /// <summary>
        /// 自定义频次统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCustomFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "CFRI", "FIRFREQ", "DBLV", "火情频次统计专题图模板", true, true) == null)
                return;
        }

        /// <summary>
        /// 当前区域频次统计按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCurrentRegionFreq_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.TimeStat(_session, "FREQ", "FIRFREQ", "DBLV", "火情频次统计专题图模板", false, true) == null)
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
        /// 历史数据自动加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiAutoAdd_Click(object sender, EventArgs e)
        {
            GetCommandAndExecute(6633);
        }

        /// <summary>
        /// 历史数据手动加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiManualAdd_Click(object sender, EventArgs e)
        {
            //string fname = @"D:\11103011.000";
            //IVectorFeatureDataReader reader = MicapsDataReaderFactory.GetVectorFeatureDataReader(fname, "GroundObserveData");
            //if (reader == null)
            //    return;
            //else
            //    return;
        }

        /// <summary>
        /// 参数配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiArgSetting_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 全球火点修正
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiCorrect_Click(object sender, EventArgs e)
        {
            try
            {
                ISmartWindow smartWindow = _session.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(GbalFirRevisePage); });
                if (smartWindow == null)
                {
                    smartWindow = new GbalFirRevisePage(_session);
                    _session.SmartWindowManager.DisplayWindow(smartWindow);
                }
                else
                    _session.SmartWindowManager.DisplayWindow(smartWindow);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 强度专题图（GFRI）,输入为修正火点shp文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiPointStren_Click(object sender, EventArgs e)
        {
            try
            {
                dbtngFirepoint.Enabled = false;
                (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("GFRI");
                ICommand cmd = _session.CommandEnvironment.Get(6602);
                if (cmd != null)
                    cmd.Execute();
            }
            finally
            {
                dbtngFirepoint.Enabled = true;
            }
        }


        /// <summary>
        /// 频次累计专题图，
        /// 输入为多个修正火点shp文件，
        /// 输出为频次累计专题图和火点信息列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiPointFreq_Click(object sender, EventArgs e)
        {
            try
            {
                dbtngFirepoint.Enabled = false;
                (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("GFRF");
                ICommand cmd = _session.CommandEnvironment.Get(6602);
                if (cmd != null)
                    cmd.Execute();
            }
            finally
            {
                dbtngFirepoint.Enabled = true;
            }
        }

        /// <summary>
        /// 频次累计大洲统计,输入为多个修正火点shp文件或频次累计栅格图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiPointDay_Click(object sender, EventArgs e)
        {
            try
            {
                dbtngFirepoint.Enabled = false;
                IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
                ms.ChangeActiveSubProduct("GFCF");
                ms.DoAutoExtract(true);
            }
            finally
            {
                dbtngFirepoint.Enabled = true;
            }
        }


        void btnFirepList_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("FRIL");
            GetCommandAndExecute(6602);
        }
        void btnManualExtract_FLARE_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("FLAR");
            GetCommandAndExecute(6602);
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
