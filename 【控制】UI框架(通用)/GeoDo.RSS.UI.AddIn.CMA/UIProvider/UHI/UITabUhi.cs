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
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    public partial class UITabUhi : UserControl, IUITabProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;

        #region 城市热岛按钮定义
        RadButtonElement btnAnalysis = new RadButtonElement("分析");
        RadButtonElement btnInteractive = new RadButtonElement("交互判识");
        RadDropDownButtonElement btnAutoGenerate = new RadDropDownButtonElement();
        RadButtonElement btnMulImage = new RadButtonElement("多通道合成图");
        RadDropDownButtonElement dbtnLayoutImage = new RadDropDownButtonElement();
        RadButtonElement btnClose = new RadButtonElement("关闭热岛\n监测分析视图");
        RadButtonElement btnToDb = new RadButtonElement("产品入库");
        #endregion

        public UITabUhi()
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
            _tab.Title = "城市热岛监测专题";
            _tab.Text = "城市热岛监测专题";
            _tab.Name = "城市热岛监测专题";
            _bar.CommandTabs.Add(_tab);
            CreateCheckGroup();
            CreateProductGroup();

            
            btnToDb.TextAlignment = ContentAlignment.BottomCenter;
            btnToDb.ImageAlignment = ContentAlignment.TopCenter;
            btnToDb.Click += new EventHandler(btnToDb_Click);
            RadRibbonBarGroup rbgToDb = new RadRibbonBarGroup();
            rbgToDb.Text = "产品入库";
            rbgToDb.Items.Add(btnToDb);
            _tab.Items.Add(rbgToDb);

            CreateCloseGroup();
        }

        private void CreateCheckGroup()
        {
            RadRibbonBarGroup rbgCheck = new RadRibbonBarGroup();
            rbgCheck.Text = "监测";
            btnAnalysis.Click += new EventHandler(btnAnalysis_Click);
            rbgCheck.Items.Add(btnAnalysis);
            //btnInteractive.Click += new EventHandler(btnInteractive_Click);
            //rbgCheck.Items.Add(btnInteractive);
            foreach (RadButtonElement item in rbgCheck.Items)
            {
                item.ImageAlignment = ContentAlignment.TopCenter;
                item.TextAlignment = ContentAlignment.BottomCenter;
            }
            _tab.Items.Add(rbgCheck);
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
        }

        private void CreateProductGroup()
        {
            RadRibbonBarGroup rbgProduct = new RadRibbonBarGroup();
            rbgProduct.Text = "专题产品";
            _tab.Items.Add(rbgProduct);
            btnMulImage.ImageAlignment = ContentAlignment.TopCenter;
            btnMulImage.TextAlignment = ContentAlignment.BottomCenter;
            btnMulImage.Click += new EventHandler(btnMulImage_Click);
            rbgProduct.Items.Add(btnMulImage);
            dbtnLayoutImage.Text = "热岛专题图";
            dbtnLayoutImage.ArrowPosition = DropDownButtonArrowPosition.Right;
            dbtnLayoutImage.ImageAlignment = ContentAlignment.TopCenter;
            dbtnLayoutImage.TextAlignment = ContentAlignment.BottomCenter;
            RadMenuItem mhiLayoutImgCurrent = new RadMenuItem("当前区域");
            mhiLayoutImgCurrent.Click += new EventHandler(mhiLayoutImgCurrent_Click);
            dbtnLayoutImage.Items.Add(mhiLayoutImgCurrent);
            RadMenuItem mhiLayoutImgCustom = new RadMenuItem("自定义");
            mhiLayoutImgCustom.Click += new EventHandler(mhiLayoutImgCustom_Click);
            dbtnLayoutImage.Items.Add(mhiLayoutImgCustom);
            rbgProduct.Items.Add(dbtnLayoutImage);
            _tab.Items.Add(rbgProduct);
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

        public object Content
        {
            get { return _tab; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            this._session = session;
            SetImage();
        }

        public void UpdateStatus()
        {
        }

        private void SetImage()
        {
            btnAnalysis.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Auto.png");
            btnAutoGenerate.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Batch.png");
            btnInteractive.Image = _session.UIFrameworkHelper.GetImage("system:Extracting_Manual.png");
            btnMulImage.Image = _session.UIFrameworkHelper.GetImage("system:mulImage.png");
            dbtnLayoutImage.Image = _session.UIFrameworkHelper.GetImage("system:uHIsland.png");
            btnClose.Image = _session.UIFrameworkHelper.GetImage("system:exit32.png");
            btnToDb.Image = _session.UIFrameworkHelper.GetImage("system:database_save.png");
        }

        #region 事件
        //产品入库
        void btnToDb_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(39002);
            if (cmd != null)
                cmd.Execute();
        }
        void btnInteractive_Click(object sender, EventArgs e)
        {
        }

        void btnSave_Click(object sender, EventArgs e)
        {
        }

        void btnAnalysis_Click(object sender, EventArgs e)
        {
            (_session.MonitoringSession as IMonitoringSession).ChangeActiveSubProduct("DBLV");
            GetCommandAndExecute(6601);
        }

        void btnMulImage_Click(object sender, EventArgs e)
        {
            MIFCommAnalysis.DisplayMonitorShow(_session, "template:城市热岛多通道合成图,多通道合成图,UHI,MCSI");
        }

        void mhiLayoutImgCurrent_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "DBLV", "城市热岛专题图", false, true) == null)
                return;
        }

        void mhiLayoutImgCustom_Click(object sender, EventArgs e)
        {
            if (MIFCommAnalysis.CreateThemeGraphy(_session, "0SDI", "", "DBLV", "城市热岛专题图", true, true) == null)
                return;
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
            cmd.Execute("UHI");
        }

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
        #endregion
    }
}
