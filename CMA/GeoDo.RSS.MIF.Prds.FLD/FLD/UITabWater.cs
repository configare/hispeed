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
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public partial class UITabWater : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 水按钮定义
        RadButtonElement btnRough = new RadButtonElement("自动判识");
        RadDropDownButtonElement btnInteractive = new RadDropDownButtonElement();
        //快速生成
        RadDropDownButtonElement dbtGenerate = new RadDropDownButtonElement();
        RadMenuItem mniAllGenerate = new RadMenuItem("自动判识+专题产品");
        RadMenuItem mniProGenerate = new RadMenuItem("专题产品");

        //灾情事件快速生成
        RadDropDownButtonElement dbtnDisGenerate = new RadDropDownButtonElement();
        RadMenuItem mniDisAllGenerate = new RadMenuItem("自动判识+专题产品");
        RadMenuItem mniDisProGenerate = new RadMenuItem("专题产品");

        //日常业务
        RadDropDownButtonElement dbtnOrdLayout = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnOrdStatArea = new RadDropDownButtonElement();
        //日常业务快速生成
        RadDropDownButtonElement dbtnOrdGenerate = new RadDropDownButtonElement();
        RadMenuItem mniOrdAllGenerate = new RadMenuItem("判识+专题产品");
        RadMenuItem mniOrdProGenerate = new RadMenuItem("专题产品");

        //从配置文件读取统计分析菜单名称
        string provinceName = AreaStatProvider.GetAreaStatItemMenuName("行政区划");
        string landTypeName = AreaStatProvider.GetAreaStatItemMenuName("土地利用类型");
        string cityName = AreaStatProvider.GetAreaStatItemMenuName("三级行政区划");
        string landTypeRasterName = AreaStatProvider.GetAreaStatItemMenuName("土地利用类型(栅格)");

        //灾情事件
        RadButtonElement btnMixRaster = new RadButtonElement("混合像元");
        RadDropDownButtonElement dbtnComputeFlood = new RadDropDownButtonElement();
        RadMenuItem mniDisFlood = new RadMenuItem("泛滥缩小水体");
        RadMenuItem mniDisFloodCount = new RadMenuItem("水体泛滥天数");

        RadDropDownButtonElement growedFlood = new RadDropDownButtonElement();//洪涝，扩大水体
        RadDropDownButtonElement dbtnDisLayout = new RadDropDownButtonElement();
        RadMenuItem mniDisMulImage = new RadMenuItem("多通道合成图");
        RadMenuItem mniDisOMulImag = new RadMenuItem("多通道合成图（原始分辨率）");
        RadMenuItem mniDisBinImage = new RadMenuItem("二值图");
        RadMenuItem mniMixImage = new RadMenuItem("混合像元");
        RadMenuItem mniFloodImage = new RadMenuItem("泛滥缩小水体专题图");              //泛滥缩小水体
        RadMenuItem mniFloodCountImage = new RadMenuItem("水体泛滥天数专题图");
        RadDropDownButtonElement dbtnAreaStat = new RadDropDownButtonElement();   //判识水体面积统计
        RadMenuItem mniCurrentArea = new RadMenuItem("当前区域");

        RadDropDownButtonElement dbtFloodArea = new RadDropDownButtonElement();   //扩大缩小水体面积统计
        RadMenuItem mniCustomArea = new RadMenuItem("自定义");
        RadMenuItem mniDivAndType = new RadMenuItem("行政区+地类");

        //周期业务
        RadButtonElement btnFloodDayArea = new RadButtonElement("泛滥历时面积");
        RadDropDownButtonElement dbtnCycLayout = new RadDropDownButtonElement();  //专题产品
        RadMenuItem mniThreeLevel = new RadMenuItem("淹没历时（三级）");          //淹没历时
        RadMenuItem mniSixLevel = new RadMenuItem("淹没历时（六级）");
        RadMenuItem mniMutFloodImage = new RadMenuItem("多时次洪涝");
        RadDropDownButtonElement dbtnCycGenerate = new RadDropDownButtonElement();
        RadMenuItem mniCycProGenerate = new RadMenuItem("专题产品");
        //长序列生产
        RadDropDownButtonElement dbtnTFRLayout = new RadDropDownButtonElement();

        //其他业务
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

            #region 监测
            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            rbgCheck.Text = "监测";
            //粗判
            btnRough.Click += new EventHandler(btnRough_Click);
            btnRough.ImageAlignment = ContentAlignment.TopCenter;
            btnRough.TextAlignment = ContentAlignment.BottomCenter;
            rbgCheck.Items.Add(btnRough);
            //交互判识
            btnInteractive.Text = "交互判识";
            btnInteractive.ImageAlignment = ContentAlignment.TopCenter;
            btnInteractive.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem btnWater = new RadMenuItem("水体");
            btnWater.Click += new EventHandler(btnWater_Click);
            RadMenuItem btnCloudExtract = new RadMenuItem("云");
            btnCloudExtract.Click += new EventHandler(btnCloudExtract_Click);
            btnInteractive.Items.Add(btnWater);
            btnInteractive.Items.Add(btnCloudExtract);
            rbgCheck.Items.Add(btnInteractive);
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
            #endregion

            #region 日常业务
            RadRibbonBarGroup rbgOrdinary = new RadRibbonBarGroup();
            rbgOrdinary.Text = "日常业务";
            dbtnOrdLayout.Text = "专题产品";
            dbtnOrdLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnOrdLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnOrdLayout.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniOrdNetImag = new RadMenuItem("网络合成图");
            RadMenuItem mniOrdMulImg = new RadMenuItem("多通道合成图");
            RadMenuItem mniOMulImag = new RadMenuItem("多通道合成图（原始分辨率）");
            RadMenuItem mniOrdBinImg = new RadMenuItem("二值图");
            mniOrdMulImg.Click += new EventHandler(mniMulImage_Click);
            mniOrdBinImg.Click += new EventHandler(mniBinImage_Click);
            mniOrdNetImag.Click += new EventHandler(mniOrdNetImag_Click);
            mniOMulImag.Click += new EventHandler(mniOMulImag_Click);
            dbtnOrdLayout.Items.AddRange(new RadMenuItem[] { mniOrdNetImag, mniOrdMulImg, mniOMulImag, mniOrdBinImg });
            rbgOrdinary.Items.Add(dbtnOrdLayout);

            dbtnOrdStatArea.Text = "统计分析";
            dbtnOrdStatArea.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnOrdStatArea.ImageAlignment = ContentAlignment.TopCenter;
            dbtnOrdStatArea.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniOrdCurrent = new RadMenuItem("当前区域");
            RadMenuItem mniOrdProvince = new RadMenuItem(provinceName);
            RadMenuItem mniOrdCity = new RadMenuItem(cityName);
            RadMenuItem mniOrdLandType = new RadMenuItem(landTypeName);
            RadMenuItem mniOrdLandTypeRaster = new RadMenuItem(landTypeRasterName);
            mniOrdCurrent.Click += new EventHandler(mniCurrentArea_Click);
            mniOrdProvince.Click += new EventHandler(mniProvince_Click);
            mniOrdCity.Click += new EventHandler(mniCityArea_Click);
            mniOrdLandType.Click += new EventHandler(mniLandTypedblv_Click);
            mniOrdLandTypeRaster.Click += new EventHandler(mniOrdLandTypeRaster_Click);
            dbtnOrdStatArea.Items.AddRange(new RadMenuItem[] { mniOrdCurrent, mniOrdLandType, mniOrdProvince, mniOrdCity, mniOrdLandTypeRaster });
            rbgOrdinary.Items.Add(dbtnOrdStatArea);

            dbtnOrdGenerate.Text = "快速生成";
            dbtnOrdGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnOrdGenerate.ImageAlignment = ContentAlignment.TopCenter;
            dbtnOrdGenerate.TextAlignment = ContentAlignment.BottomCenter;
            mniOrdAllGenerate.Click += new EventHandler(mniAllGenerate_Click);
            dbtnOrdGenerate.Items.Add(mniOrdAllGenerate);
            mniOrdProGenerate.Click += new EventHandler(mniProGenerate_Click);
            dbtnOrdGenerate.Items.Add(mniOrdProGenerate);
            rbgOrdinary.Items.Add(dbtnOrdGenerate);
            #endregion 日常业务

            #region 灾情监测
            RadRibbonBarGroup rgbDisaster = new RadRibbonBarGroup();
            rgbDisaster.Text = "灾情事件";
            btnMixRaster.ImageAlignment = ContentAlignment.TopCenter;
            btnMixRaster.TextAlignment = ContentAlignment.BottomCenter;
            btnMixRaster.Click += new EventHandler(mniComputeMix_Click);
            rgbDisaster.Items.Add(btnMixRaster);
            dbtnComputeFlood.Text = "泛滥水体计算";
            dbtnComputeFlood.ImageAlignment = ContentAlignment.TopCenter;
            dbtnComputeFlood.TextAlignment = ContentAlignment.BottomCenter;
            mniDisFlood.Click += new EventHandler(mniDisFlood_Click);
            mniDisFloodCount.Click += new EventHandler(mniDisFloodCount_Click);
            dbtnComputeFlood.Items.AddRange(new RadMenuItem[] { mniDisFlood, mniDisFloodCount });
            rgbDisaster.Items.Add(dbtnComputeFlood);
            //专题图：多通道合成图，二值图（当前区域），混合像元专题图，泛滥缩小水体专题图
            dbtnDisLayout.Text = "专题产品";
            dbtnDisLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnDisLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDisLayout.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniDisNetImag = new RadMenuItem("网络合成图");
            mniDisNetImag.Click += new EventHandler(mniOrdNetImag_Click);
            mniDisMulImage.Click += new EventHandler(mniMulImage_Click);
            mniDisBinImage.Click += new EventHandler(mniBinImage_Click);
            mniMixImage.Click += new EventHandler(mniMixImage_Click);
            mniFloodImage.Click += new EventHandler(mniFloodImage_Click);
            mniDisOMulImag.Click += new EventHandler(mniOMulImag_Click);
            mniFloodCountImage.Click += new EventHandler(mniFloodCountImage_Click);
            dbtnDisLayout.Items.AddRange(new RadMenuItem[] { mniDisMulImage, mniDisOMulImag, mniDisNetImag, mniDisBinImage, mniMixImage, mniFloodImage, mniFloodCountImage });
            rgbDisaster.Items.Add(dbtnDisLayout);

            dbtnAreaStat.Text = "判识水体面积统计";
            dbtnAreaStat.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnAreaStat.ImageAlignment = ContentAlignment.TopCenter;
            dbtnAreaStat.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniProvince = new RadMenuItem(provinceName);
            RadMenuItem mniCityArea = new RadMenuItem(cityName);
            RadMenuItem mniLandTypedblv = new RadMenuItem(landTypeName);
            RadMenuItem mniLandTypeRaster = new RadMenuItem(landTypeRasterName);
            mniCurrentArea.Click += new EventHandler(mniCurrentArea_Click);
            mniProvince.Click += new EventHandler(mniProvince_Click);
            mniCityArea.Click += new EventHandler(mniCityArea_Click);
            mniLandTypedblv.Click += new EventHandler(mniLandTypedblv_Click);
            mniLandTypeRaster.Click += new EventHandler(mniOrdLandTypeRaster_Click);
            dbtnAreaStat.Items.AddRange(new RadMenuItem[] { mniCurrentArea, mniLandTypedblv, mniLandTypeRaster, mniProvince, mniCityArea });
            rgbDisaster.Items.Add(dbtnAreaStat);

            dbtFloodArea.Text = "变化水体面积统计";
            dbtFloodArea.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtFloodArea.ImageAlignment = ContentAlignment.TopCenter;
            dbtFloodArea.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniCurrentBound = new RadMenuItem("当前区域");
            mniCurrentBound.Click += new EventHandler(mniCurrentBound_Click);
            RadMenuItem mniProvincBound = new RadMenuItem(provinceName);
            mniProvincBound.Click += new EventHandler(mniProvincBound_Click);
            mniCustomArea.Click += new EventHandler(mniCustomArea_Click);
            RadMenuItem mniLandType = new RadMenuItem(landTypeName);
            mniLandType.Click += new EventHandler(mniLandType_Click);
            mniDivAndType.Click += new EventHandler(mniDivAndType_Click);
            RadMenuItem mniComaCityArea = new RadMenuItem(cityName);
            mniComaCityArea.Click += new EventHandler(mniComaCityArea_Click);
            dbtFloodArea.Items.AddRange(new RadMenuItem[] { mniCurrentBound, mniProvincBound, mniLandType, mniDivAndType, mniComaCityArea, mniCustomArea });
            rgbDisaster.Items.Add(dbtFloodArea);

            dbtnDisGenerate.Text = "快速生成";
            dbtnDisGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnDisGenerate.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDisGenerate.TextAlignment = ContentAlignment.BottomCenter;
            mniDisAllGenerate.Click += new EventHandler(mniDisAllGenerate_Click);
            dbtnDisGenerate.Items.Add(mniDisAllGenerate);
            mniDisProGenerate.Click += new EventHandler(mniDisProGenerate_Click);
            dbtnDisGenerate.Items.Add(mniDisProGenerate);
            rgbDisaster.Items.Add(dbtnDisGenerate);
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

            #region 周期业务
            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "周期业务";
            dbtnCycLayout.Text = "专题产品";
            dbtnCycLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnCycLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCycLayout.TextAlignment = ContentAlignment.BottomCenter;
            mniMutFloodImage.Click += new EventHandler(btnMutFloodImage_Click);
            mniThreeLevel.Click += new EventHandler(mhiThreeLevel_Click);
            mniSixLevel.Click += new EventHandler(mhiSixLevel_Click);
            dbtnCycLayout.Items.AddRange(new RadMenuItem[] { mniMutFloodImage, mniThreeLevel, mniSixLevel });
            rbgProduct.Items.Add(dbtnCycLayout);
            //泛滥历时面积
            btnFloodDayArea.TextAlignment = ContentAlignment.BottomCenter;
            btnFloodDayArea.ImageAlignment = ContentAlignment.TopCenter;
            btnFloodDayArea.Click += new EventHandler(btnFloodDayArea_Click);
            rbgProduct.Items.Add(btnFloodDayArea);

            dbtnCycGenerate.Text = "快速生成";
            dbtnCycGenerate.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnCycGenerate.ImageAlignment = ContentAlignment.TopCenter;
            dbtnCycGenerate.TextAlignment = ContentAlignment.BottomCenter;
            mniCycProGenerate.Click += new EventHandler(mniCycProGenerate_Click);

            dbtnTFRLayout.Text = "长序列";
            dbtnTFRLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnTFRLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnTFRLayout.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniCurrentTfre = new RadMenuItem("频次统计（当前区域）");
            mniCurrentTfre.Click += new EventHandler(mniCurrentTfre_Click);
            RadMenuItem mniCurrentTfreImage = new RadMenuItem("频次图（当前区域）");
            mniCurrentTfreImage.Click += new EventHandler(mniCurrentTfreImage_Click);
            RadMenuItem mniCurrentTfrq = new RadMenuItem("频率统计（当前区域）");
            mniCurrentTfrq.Click += new EventHandler(mniCurrentTfrq_Click);
            RadMenuItem mniCurrentTfrqImage = new RadMenuItem("频率图（当前区域）");
            mniCurrentTfrqImage.Click += new EventHandler(mniCurrentTfrqImage_Click);
            RadMenuItem mniEDGE = new RadMenuItem("水体边缘提取");
            mniEDGE.Click += new EventHandler(mniEDGE_Click);
            RadMenuItem mniTSTA = new RadMenuItem("长序列面积(当前)");
            mniTSTA.Click += new EventHandler(mniTSTA_Click);
            RadMenuItem mniTSTAPri = new RadMenuItem("长序列面积(省界)");
            mniTSTAPri.Click += new EventHandler(mniTSTAPri_Click);
            RadMenuItem mniTSTATD = new RadMenuItem("长序列面积(土地类型)");
            mniTSTATD.Click += new EventHandler(mniTSTATD_Click);
            RadMenuItem mniTSTADT = new RadMenuItem("长序列面积(洞庭湖区)");
            mniTSTADT.Click += new EventHandler(mniTSTADT_Click);

            dbtnTFRLayout.Items.AddRange(new RadMenuItem[] { mniCurrentTfre, mniCurrentTfreImage, mniCurrentTfrq, mniCurrentTfrqImage, mniEDGE, mniTSTA, mniTSTAPri, mniTSTATD, mniTSTADT });
            rbgProduct.Items.Add(dbtnTFRLayout);

            dbtnCycGenerate.Items.Add(mniCycProGenerate);
            //rbgProduct.Items.Add(dbtnCycGenerate);

            RadRibbonBarGroup rbgOther = new RadRibbonBarGroup();
            rbgOther.Text = "其他业务";
            btnMovie.ImageAlignment = ContentAlignment.TopCenter;                 //动画
            btnMovie.TextAlignment = ContentAlignment.BottomCenter;
            btnMovie.Click += new EventHandler(btnMovie_Click);
            rbgOther.Items.Add(btnMovie);
            #endregion

            btnToDb.TextAlignment = ContentAlignment.BottomCenter;
            btnToDb.ImageAlignment = ContentAlignment.TopCenter;
            btnToDb.Click += new EventHandler(btnToDb_Click);
            RadRibbonBarGroup rbgToDb = new RadRibbonBarGroup();
            rbgToDb.Text = "产品入库";
            rbgToDb.Items.Add(btnToDb);

            _tab.Items.Add(rbgCheck);
            _tab.Items.Add(rbgOrdinary);
            _tab.Items.Add(rgbDisaster);
            _tab.Items.Add(rbgProduct);
            _tab.Items.Add(rbgOther);
            _tab.Items.Add(rbgToDb);
            Controls.Add(_bar);
            #region 新增 洪涝
            growedFlood.Text = "洪涝灾害";
            growedFlood.ArrowPosition = DropDownButtonArrowPosition.Right;
            growedFlood.ImageAlignment = ContentAlignment.TopCenter;
            growedFlood.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem grownfloodjc = new RadMenuItem("洪涝监测专题图");//洪涝专题图
            RadMenuItem grownfloodjcsy = new RadMenuItem("洪涝监测示意图");//洪涝监测示意图
            RadMenuItem mniCityLandUsedArea = new RadMenuItem("洪涝水体面积统计");
            RadMenuItem mniFloodLastDays = new RadMenuItem("洪涝持续日数");
            RadMenuItem grownfloodlastDays = new RadMenuItem("洪涝监测日数图");
            RadMenuItem mniFloodLastDaysArea = new RadMenuItem("洪涝持续日数面积");
            grownfloodjc.Click += new EventHandler(grownfloodjc_Click);
            grownfloodjcsy.Click += new EventHandler(grownfloodjcsy_Click);
            mniCityLandUsedArea.Click += new EventHandler(mniCityLandUsedArea_Click);
            mniFloodLastDays.Click += new EventHandler(mniFloodLastDays_Click);
            grownfloodlastDays.Click += new EventHandler(grownfloodlastDays_Click);
            mniFloodLastDaysArea.Click += new EventHandler(mniFloodLastDaysArea_Click);
            growedFlood.Items.AddRange(new RadMenuItem[] { grownfloodjc, grownfloodjcsy, mniCityLandUsedArea, mniFloodLastDays, grownfloodlastDays, mniFloodLastDaysArea });
            rgbDisaster.Items.Add(growedFlood);
            #endregion 
        }
        //洪涝专题图
        public void grownfloodjc_Click(object sender, EventArgs e)
        {

            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0MSI");
            ms.DoAutoExtract(false);
        }
        public void grownfloodjcsy_Click(object sender, EventArgs e)
        {
            //SetIsFixImageRegion();
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "FLDB", "", "FLOD", "洪涝监测示意图模板", false, true) == null)
                return;
        }

        public void grownfloodlastDays_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "FLSI", "", "FLDS", "洪涝持续日数图模板", false, true) == null)
                return;
        }

        public void mniCityLandUsedArea_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "COCU", "FWAS", "FWAS", "省市县+地类", false, true) == null)
                return;
        }

        public void mniFloodLastDays_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("FLDS");
            GetCommandAndExcute(6602);
        }

        public void mniFloodLastDaysArea_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "CODU", "FWAS", "FWAS", "省市县+地类", false, true) == null)
                return;
        }

        void mniOMulImag_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "OCSI");
            ms.DoAutoExtract(true);
        }

        void mniOrdNetImag_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "NCSI");
            ms.DoAutoExtract(false);
        }

        #region 监测
        //粗判
        void btnRough_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExcute(6601);
        }

        //交互判时
        void btnWater_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExcute(6602);
        }

        //云判识
        void btnCloudExtract_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0CLM");
            GetCommandAndExcute(6602);
        }
        #endregion

        #region 专题图
        /// <summary>
        /// 水情产品多通道合成图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mniMulImage_Click(object sender, EventArgs e)
        {
            //MIFCommAnalysis.DisplayMonitorShow(_session, "template:水情产品多通道合成图,多通道合成图,FLD,MCSI");
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "MCSI");
            ms.DoAutoExtract(true);
        }

        //二值图
        void mniBinImage_Click(object sender, EventArgs e)
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

        void mniFloodImage_Click(object sender, EventArgs e)        //扩大缩小水体专题图
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "FLIM", "", "FLOD", "扩大缩小水体专题图模版", false, true) == null)
                return;
        }


        void mniFloodCountImage_Click(object sender, EventArgs e)  //水体泛滥天数专题图
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("0IMG");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "FLCI");
            ms.DoAutoExtract(true);
        }

        #endregion

        void mniDisFloodCount_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("FLDC");
            GetCommandAndExcute(6602);
        }

        //扩大缩小水体
        void mniDisFlood_Click(object sender, EventArgs e)       //生成扩大缩小水体栅格文件
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("FLOD");
            GetCommandAndExcute(6602);
        }

        //判时水体面积统计
        void mniCurrentArea_Click(object sender, EventArgs e)        //判识水体当前区域
        {
            //IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            //ms.ChangeActiveSubProduct("STAT");
            //ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "CCAR");
            //ms.DoAutoExtract(true);
            if (MIFCommAnalysis.AreaStat(_session, "CCAR", "", false, true) == null)
                return;
        }

        void mniCityArea_Click(object sender, EventArgs e)           //判时水体按市县统计
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "0CCC");
            ms.DoAutoExtract(true);
        }

        void mniOrdLandTypeRaster_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("STAT");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentify", "RLUT");
            ms.DoAutoExtract(true);
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
            AutoGenretateProvider.AutoGenrate(_session, "FLOD");
        }

        void mniAllGenerate_Click(object sender, EventArgs e)       //快速生成"判识+专题产品"
        {
            AutoGenretateProvider.AutoGenrate(_session, null);
        }

        void mniOrdProGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, null, null, "Ord");
        }

        void mniOrdAllGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "STAT", null, "Ord");
        }

        void mniDisProGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "STAT", null, "Dis");
        }

        void mniDisAllGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, null, null, "Dis");
        }

        void mniCycProGenerate_Click(object sender, EventArgs e)
        {
            AutoGenretateProvider.AutoGenrate(_session, "FREQ", null, "Cyc");
        }

        void mniCurrentTfre_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("TFRE");
            GetCommandAndExcute(6602);
        }

        void mniCurrentTfrq_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("TFRQ");
            GetCommandAndExcute(6602);
        }

        void mniCurrentTfreImage_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("TFRI");
            GetCommandAndExcute(6602);
        }

        void mniCurrentTfrqImage_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("TFQI");
            GetCommandAndExcute(6602);
        }

        void mniEDGE_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("EDGE");
            GetCommandAndExcute(6602);
        }

        void mniTSTA_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("TSTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentifyInstance", "TCCA");
            GetCommandAndExcute(6602);
        }

        void mniTSTATD_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("TSTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentifyInstance", "TCLU");
            GetCommandAndExcute(6602);
        }

        void mniTSTAPri_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("TSTA");
            GetCommandAndExcute(6602);
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentifyInstance", "TCBP");
        }

        void mniTSTADT_Click(object sender, EventArgs e)
        {
            IMonitoringSession ms = _session.MonitoringSession as IMonitoringSession;
            ms.ChangeActiveSubProduct("TSTA");
            ms.ActiveMonitoringSubProduct.ArgumentProvider.SetArg("OutFileIdentifyInstance", "TCDT");
            GetCommandAndExcute(6602);
        }

        //扩大水体面积统计

        void mniCurrentBound_Click(object sender, EventArgs e)       //扩大水体面积统计
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "FCAR", "FWAS", "FWAS", "", false, true) == null)
                return;
        }
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
        void mniComaCityArea_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.StatAreaNotBaseStatSubprd(_session, "COCC", "FWAS", "FWAS", "省市县行政区划", false, true) == null)
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
            CreateThemeGraphRegionButton();
            CreateqQuickReportRegionButton();
            CreatClose(); // 关闭
            SetImage();
        }

        private void CreateThemeGraphRegionButton()
        {
            RadDropDownButtonElement _btnThemeGraphRegion = new RadDropDownButtonElement();
            ThemeGraphRegionFacctory.RegistToButton("FLD", _btnThemeGraphRegion, _tab, _session);
            _btnThemeGraphRegion.Image = _session.UIFrameworkHelper.GetImage("system:settings.png");
        }

        private void CreateqQuickReportRegionButton()
        {
            QuickReportFactory.RegistToButton("FLD", _tab, _session);
        }

        private void CreatClose()
        {
            #region 关闭
            RadRibbonBarGroup rbgClose = new RadRibbonBarGroup();
            rbgClose.Text = "关闭";
            btnClose.ImageAlignment = ContentAlignment.TopCenter;
            btnClose.TextAlignment = ContentAlignment.BottomCenter;
            btnClose.Click += new EventHandler(btnClose_Click);
            rbgClose.Items.Add(btnClose);
            _tab.Items.Add(rbgClose);
            #endregion
        }

        #region 初始化

        public void UpdateStatus()
        {

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
            dbtnOrdLayout.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            dbtnOrdStatArea.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            dbtnOrdGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnComputeFlood.Image = _session.UIFrameworkHelper.GetImage("system:wydzdsy3.png");
            dbtFloodArea.Image = _session.UIFrameworkHelper.GetImage("system:landType.png");
            dbtnDisGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            dbtnDisLayout.Image = _session.UIFrameworkHelper.GetImage("system:bitImage.png");
            dbtnAreaStat.Image = _session.UIFrameworkHelper.GetImage("system:area_current.png");
            btnFloodDayArea.Image = _session.UIFrameworkHelper.GetImage("system:dem.png");
            dbtnCycLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnCycGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnMovie.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
            btnMixRaster.Image = _session.UIFrameworkHelper.GetImage("system:mixPixel.png");
            dbtnTFRLayout.Image = _session.UIFrameworkHelper.GetImage("system:iceTimeFreq.png");
            growedFlood.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
        }
        #endregion
    }
}
