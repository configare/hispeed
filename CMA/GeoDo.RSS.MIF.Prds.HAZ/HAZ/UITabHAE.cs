using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Layout;
using GeoDo.RSS.UI.AddIn.Layout;
using Telerik.WinControls.UI;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.HAZ
{
    public partial class UITabHAE : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 雾霾按钮定义

        #region 真彩图相关按钮
        RadButtonElement btnNatrueColor = new RadButtonElement("真彩图");
        RadButtonElement btnRasterFile = new RadButtonElement("影像图");

        RadButtonElement btnNatrueOAColor = new RadButtonElement("雾霾流程");
        RadButtonElement btnNatrueOAFlow = new RadButtonElement("流程面板");
        #endregion

        #region 小仪器雾霾相关按钮
        RadButtonElement btnImportTOU = new RadButtonElement("导入");
        RadButtonElement btnTOUImage = new RadButtonElement("专题图");
        #endregion

        #region 动画相关按钮
        RadButtonElement btnAnimationRegion = new RadButtonElement("当前区域");
        RadButtonElement btnAnimationCustom = new RadButtonElement("自定义");
        #endregion

        #region 定量产品
        RadDropDownButtonElement dbtnLevel = new RadDropDownButtonElement();
        #endregion

        RadButtonElement btnClose = new RadButtonElement("关闭雾霾监测\n分析视图");//btnPicLayout
        RadButtonElement btnPicLayout = new RadButtonElement("图像批量输出\n分析视图");//btnPicLayout
        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        RadButtonElement dbtnTOU = new RadButtonElement();
        RadButtonElement dbtnFreq = new RadButtonElement();
        RadDropDownButtonElement dbtnAnimation = new RadDropDownButtonElement();
        RadDropDownButtonElement btnInteractive = new RadDropDownButtonElement();
        RadDropDownButtonElement dbtnDisLayout = new RadDropDownButtonElement();
        #endregion

        public UITabHAE()
        {
            InitializeComponent();
            CreateRibbonBar();
        }

        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "雾霾监测专题";
            _tab.Text = "雾霾监测专题";
            _tab.Name = "雾霾监测专题";
            _bar.CommandTabs.Add(_tab);

            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            _tab.Items.Add(rbgCheck);
            rbgCheck.Text = "监测";
            rbgCheck.Items.Add(btnInteractive);
            btnInteractive.Text = "交互判识";
            btnInteractive.ImageAlignment = ContentAlignment.TopCenter;
            btnInteractive.TextAlignment = ContentAlignment.BottomCenter;
            btnInteractive.ArrowPosition = DropDownButtonArrowPosition.Right;
            RadMenuItem fogProduct = new RadMenuItem("霾");
            fogProduct.Click += new EventHandler(fogProduct_Click);
            btnInteractive.Items.Add(fogProduct);
            RadMenuItem cloudProduct = new RadMenuItem("云");
            cloudProduct.Click += new EventHandler(cloudProduct_Click);
            btnInteractive.Items.Add(cloudProduct);
            RadMenuItem LevelProduct = new RadMenuItem("分级");
            LevelProduct.Click += new EventHandler(LevelProduct_Click);
            btnInteractive.Items.Add(LevelProduct);

            #region 灾情事件
            RadRibbonBarGroup rbgDisaster = new RadRibbonBarGroup();
            rbgDisaster.Text = "灾情事件";
            _tab.Items.Add(rbgDisaster);

            dbtnDisLayout.Text = "专题产品";
            dbtnDisLayout.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnDisLayout.ImageAlignment = ContentAlignment.TopCenter;
            dbtnDisLayout.TextAlignment = ContentAlignment.BottomCenter;

            RadMenuItem btnDisMulImage2 = new RadMenuItem("监测示意图");
            btnDisMulImage2.Click += new EventHandler(btnMulImage2_Click);
            RadMenuItem mniDisBinImage = new RadMenuItem("二值图（当前区域）");
            mniDisBinImage.Click += new EventHandler(mhiBinImgCurrent_Click);

            dbtnDisLayout.Items.AddRange(new RadMenuItem[] { btnDisMulImage2, mniDisBinImage });
            rbgDisaster.Items.Add(dbtnDisLayout);

            #endregion 灾情事件

            #region 雾霾业务
            RadRibbonBarGroup rbgHAZE = new RadRibbonBarGroup();
            rbgHAZE.Text = "雾霾";
            btnNatrueColor.ImageAlignment = ContentAlignment.TopCenter;
            btnNatrueColor.TextAlignment = ContentAlignment.BottomCenter;
            btnNatrueColor.Click += new EventHandler(btnNatrueColor_Click);
            rbgHAZE.Items.Add(btnNatrueColor);
            btnRasterFile.ImageAlignment = ContentAlignment.TopCenter;
            btnRasterFile.TextAlignment = ContentAlignment.BottomCenter;
            btnRasterFile.Click += new EventHandler(btnRasterFile_Click);
            rbgHAZE.Items.Add(btnRasterFile);


            dbtnTOU.Text = "小仪器雾霾";
            dbtnTOU.ImageAlignment = ContentAlignment.TopCenter;
            dbtnTOU.TextAlignment = ContentAlignment.BottomCenter;
            dbtnTOU.Click += new EventHandler(btnThinMonitoring_Click);
            rbgHAZE.Items.Add(dbtnTOU);

            btnNatrueOAColor.Text = "雾霾流程";
            btnNatrueOAColor.ImageAlignment = ContentAlignment.TopCenter;
            btnNatrueOAColor.TextAlignment = ContentAlignment.BottomCenter;
            btnNatrueOAColor.Click += new EventHandler(btnNatrueOAColor_Click);
            //rbgHAZE.Items.Add(btnNatrueOAColor);

            btnNatrueOAFlow.Text = "流程面板";
            btnNatrueOAFlow.ImageAlignment = ContentAlignment.TopCenter;
            btnNatrueOAFlow.TextAlignment = ContentAlignment.BottomCenter;
            btnNatrueOAFlow.Click += new EventHandler(btnNatrueOAFlow_Click);
            //rbgHAZE.Items.Add(btnNatrueOAFlow);

            #endregion

            #region

            RadRibbonBarGroup rbgLevel = new RadRibbonBarGroup();
            rbgLevel.Text = "定量产品";
            dbtnLevel.Text = "强度等级";
            dbtnLevel.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnLevel.ImageAlignment = ContentAlignment.TopCenter;
            dbtnLevel.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mniLevelByAOD = new RadMenuItem("参考AOD");
            mniLevelByAOD.Click += new EventHandler(mniLevelByAOD_Click);
            dbtnLevel.Items.AddRange(new RadMenuItem[] { mniLevelByAOD });
            rbgLevel.Items.Add(dbtnLevel);

            #endregion

            RadRibbonBarGroup rbgOther = new RadRibbonBarGroup();
            rbgOther.Text = "其他业务";

            dbtnFreq.Text = "频次统计";
            dbtnFreq.ImageAlignment = ContentAlignment.TopCenter;
            dbtnFreq.TextAlignment = ContentAlignment.BottomCenter;
            dbtnFreq.Click += new EventHandler(btnFreq_Click);
            rbgOther.Items.Add(dbtnFreq);

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

            _tab.Items.Add(rbgHAZE);
            _tab.Items.Add(rbgLevel);
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
            AddPicLayoutButton();
            AddCloseButton();
            SetImage();
        }

        private void AddThemeGraphRegionBotton()
        {
            RadDropDownButtonElement _btnThemeGraphRegion = new RadDropDownButtonElement();
            ThemeGraphRegionFacctory.RegistToButton("HAZ", _btnThemeGraphRegion, _tab, _session);
            _btnThemeGraphRegion.Image = _session.UIFrameworkHelper.GetImage("system:settings.png");
        }

        public void UpdateStatus()
        { }

        private void SetImage()
        {
            btnNatrueColor.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            btnNatrueOAColor.Image = _session.UIFrameworkHelper.GetImage("system:fogCycTime.png");
            btnInteractive.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnPicLayout.Image = _session.UIFrameworkHelper.GetImage("system:Layout_ToFullEnvelope.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
            btnImportTOU.Image = _session.UIFrameworkHelper.GetImage("system:import.png");
            btnTOUImage.Image = _session.UIFrameworkHelper.GetImage("system:TOUImage.png");
            dbtnTOU.Image = _session.UIFrameworkHelper.GetImage("system:TOUImage.png");
            dbtnAnimation.Image = _session.UIFrameworkHelper.GetImage("system:customVedio.png");
            btnAnimationRegion.Image = _session.UIFrameworkHelper.GetImage("system:exportVedio.png");
            dbtnAnimation.Image = _session.UIFrameworkHelper.GetImage("system:customVedio.png");
            dbtnDisLayout.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            btnRasterFile.Image = _session.UIFrameworkHelper.GetImage("system:fogCSR.png");
            dbtnFreq.Image = _session.UIFrameworkHelper.GetImage("system:strenFreq.png");
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

        void LevelProduct_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("0LEV");
            GetCommandAndExecute(6602);
        }

        #endregion 判识相关按钮事件

        #region 真彩图相关按钮事件

        /// <summary>
        /// 真彩图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnNatrueColor_Click(object sender, EventArgs e)
        {
            IMonitoringSession monitoring = _session.MonitoringSession as IMonitoringSession;
            monitoring.CanResetUserControl();
            monitoring.ChangeActiveSubProduct("NCIM");
            GetCommandAndExecute(6602);
        }

        void btnRasterFile_Click(object sender, EventArgs e)
        {
            IMonitoringSession monitoring = _session.MonitoringSession as IMonitoringSession;
            monitoring.CanResetUserControl();
            monitoring.ChangeActiveSubProduct("RFIM");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 雾霾流程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnNatrueOAColor_Click(object sender, EventArgs e)
        {
            IMonitoringSession monitoring = _session.MonitoringSession as IMonitoringSession;
            monitoring.CanResetUserControl();
            monitoring.ChangeActiveSubProduct("OAIM");
            GetCommandAndExecute(6602);
        }

        /// <summary>
        /// 流程面板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnNatrueOAFlow_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(90191);
            if (cmd != null)
                cmd.Execute();
        }

        #endregion


        #region 专题图相关按钮事件

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
        /// 当前区域二值图按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mhiBinImgCurrent_Click(object sender, EventArgs e)
        {
            //SetIsFixImageRegion();
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "DBLV", "霾监监测专题图模板", false, true) == null)
                return;
        }
        #endregion 专题图相关按钮事件


        #region 关闭相关按钮事件

        //导出专题图按钮
        void btnHisLayoutput_Click(object sender, EventArgs e)
        {
            SavePngs();

        }
        private string GetPicDir()
        {
            string txtFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MonitoringProductArgs", "HAZ", "真彩图数据输出目录.txt");
            string dir = null;
            using (var sr = new StreamReader(txtFile, Encoding.GetEncoding("GB2312")))
            {
                string dirPath = sr.ReadLine();
                if (Directory.Exists(dirPath))
                {
                    dir = dirPath;
                }
            }
            return dir;
        }

        private void SavePngs()
        {
            string dir = GetPicDir();
            if (string.IsNullOrWhiteSpace(dir))
            {
                MessageBox.Show("没有设置工作路径，不能保存图片！");
                return;
            }
            string day = DateTime.Now.ToString("yyyyMMddHHmmss");
            string outdir = Path.Combine(dir, day);

            ISmartWindow[] wnds = _session.SmartWindowManager.GetSmartWindows((wnd) => { return wnd is ILayoutViewer; });
            if (wnds != null)
            {
                foreach (ISmartWindow wnd in wnds)
                {
                    ILayoutViewer viewer = wnd as ILayoutViewer;
                    if (viewer == null)
                        continue;
                    if (viewer.LayoutHost == null)
                        continue;
                    //刷新命令
                    IDataFrame frm = viewer.LayoutHost.ActiveDataFrame;
                    if (frm != null)
                        viewer.LayoutHost.Render(true);

                    string defaultFname = string.Empty;
                    if ((viewer as LayoutViewer).Tag != null)
                        defaultFname = (viewer as LayoutViewer).Tag as string;
                    defaultFname = Path.Combine(Path.GetDirectoryName(defaultFname), Path.GetFileNameWithoutExtension(defaultFname) + ".png");
                    ImageFormat imgFormat = ImageFormat.Png;// Jpeg;

                    using (Bitmap bmp = viewer.LayoutHost.ExportToBitmap(PixelFormat.Format32bppArgb))
                    {
                        bmp.Save(defaultFname, imgFormat);
                    }

                    string pngtarname = "";
                    string pngname = Path.GetFileName(defaultFname);
                    if (defaultFname.Contains("NCIM"))
                    {
                        string pngrsname = Path.GetFileNameWithoutExtension(pngname);
                        string[] ss = pngrsname.Split('_');
                        for (int i = 0; i < ss.Length; i++)
                        {
                            string s = ss[i];
                            string ns = s;
                            if (i == 4)
                                ns = "WordM";
                            if (string.IsNullOrWhiteSpace(pngtarname))
                                pngtarname = ns;
                            else
                                pngtarname = string.Format("{0}_{1}", pngtarname, ns);
                        }
                        pngtarname = string.Format("{0}.png", pngtarname);
                    }
                    else
                    {
                        pngtarname = pngname;
                    }

                    string outfile = Path.Combine(outdir, pngtarname);
                    string outtardir = Path.GetDirectoryName(outfile);
                    if (!Directory.Exists(outtardir))
                        Directory.CreateDirectory(outtardir);
                    File.Copy(defaultFname, outfile, true);
                }
            }

            //ICanvasViewer view = _session.SmartWindowManager.ActiveCanvasViewer;
            //ICanvas canvas = view.Canvas;
            //IRasterDrawing rasterDrawing = canvas.PrimaryDrawObject as IRasterDrawing;
            //string tifsourcefile = rasterDrawing.FileName;
            //string tifsourcefilename = Path.GetFileName(tifsourcefile);
            //string tiftargetfile = Path.Combine(outdir, tifsourcefilename);
            //string tifouttardir = Path.GetDirectoryName(tiftargetfile);
            //if (!Directory.Exists(tifouttardir))
            //    Directory.CreateDirectory(tifouttardir);
            //File.Copy(tifsourcefile, tiftargetfile, true);

            MessageBox.Show("专题图导出完毕！");
        }



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


        void btnImportTOU_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("HAZE");
            GetCommandAndExecute(6602);
        }

        void btnTOUImage_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:雾霾监测示意图,多通道合成图,HAZ,HAEI");
        }
        /// <summary>
        /// 小仪器雾霾点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnThinMonitoring_Click(object sender, EventArgs e)
        {
            IMonitoringSession monitoring = _session.MonitoringSession as IMonitoringSession;
            monitoring.CanResetUserControl();
            monitoring.ChangeActiveSubProduct("HAZE");
            GetCommandAndExecute(6602);
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
            MIFCommAnalysis.CreatAVI(_session, "template:雾霾动画展示专题图,动画展示专题图,HAZ,CMED");
        }

        /// <summary>
        /// 当前区域动画按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAnimationRegion_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.CreatAVI(_session, "template:雾霾动画展示专题图,动画展示专题图,HAZ,MEDI");
        }

        void btnFreq_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("TFRE");
            GetCommandAndExecute(6602);
        }

        #endregion


        #region 定量产品

        void mniLevelByAOD_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("LAOD");
            GetCommandAndExecute(6602);
        }
        
        #endregion

        private void GetCommandAndExecute(int cmdID)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute("HAZ");
        }

        private void GetCommandAndExecute(int cmdID, string template)
        {
            ICommand cmd = _session.CommandEnvironment.Get(cmdID);
            if (cmd == null)
                return;
            cmd.Execute(template);
        }

        #endregion

    }
}
