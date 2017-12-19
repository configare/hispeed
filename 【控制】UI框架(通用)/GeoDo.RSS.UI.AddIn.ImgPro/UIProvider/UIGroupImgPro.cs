using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.ImgPro
{
    public partial class UIGroupImgPro : UserControl, IUIGroupProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        RadRibbonBarGroup _group = null;
        ISmartSession _session = null;

        #region buttons definition
        RadButtonElement _btnResum = null; //恢复图像
        RadButtonElement _redo = null;  //重做
        RadButtonElement _undo = null;  //撤销
        RadButtonElement _btnCurve = null; //曲线调整
        RadButtonElement _btnLogEnhance = null;//对数增强
        RadButtonElement _btnExponentEnhance = null;//指数增强
        RadButtonElement _btnReversal = null; //反相
        RadButtonElement _btnElimination = null; //去色
        RadButtonElement _btnSharpen = null; //锐化
        RadButtonElement _leverColor = null; //色阶
        RadButtonElement _edge = null;//边缘提取
        RadButtonElement _brightContrast = null; //亮度对比度
        RadButtonElement _hueSaturation = null; //色相/饱和度
        RadDropDownButtonElement _dropDownHis = null;  //直方图
        RadDropDownButtonElement _dropDownFliter = null;  //滤波
        RadButtonElement _selectColor = null; //可选颜色
        RadButtonElement _replaceColor = null;  //颜色替换
        RadButtonElement _hisEquilibria = null; //直方图均衡
        #endregion

        public UIGroupImgPro()
        {
            InitializeComponent();
            CreatGroups();
        }

        private void CreatGroups()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "图像处理";
            _tab.Text = "图像处理wy";
            _tab.Name = "图像处理";
            _bar.CommandTabs.Add(_tab);
            _group = new RadRibbonBarGroup();
            _group.Text = "图像处理";

            //
            _btnResum = new RadButtonElement("恢复图像");
            _btnResum.ImageAlignment = ContentAlignment.TopCenter;
            _btnResum.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnResum.Click += new EventHandler(_btnResum_Click);

            //
            RadRibbonBarButtonGroup group3 = new RadRibbonBarButtonGroup();
            group3.Orientation = Orientation.Vertical;
            _undo = new RadButtonElement("撤销");
            _undo.ImageAlignment = ContentAlignment.MiddleLeft;
            _undo.TextImageRelation = TextImageRelation.ImageBeforeText;
            _undo.Margin = new System.Windows.Forms.Padding(0, 7, 0, 12);
            _undo.Click += new EventHandler(_undo_Click);

            _redo = new RadButtonElement("重做");
            _redo.ImageAlignment = ContentAlignment.MiddleLeft;
            _redo.TextImageRelation = TextImageRelation.ImageBeforeText;
            _redo.Margin = new System.Windows.Forms.Padding(0, 0, 0, 12);
            _redo.Click += new EventHandler(_redo_Click);

            group3.Items.AddRange(new RadItem[] { _undo, _redo });
            //            
            _btnCurve = new RadButtonElement("曲线调整");
            _btnCurve.ImageAlignment = ContentAlignment.TopCenter;
            _btnCurve.TextImageRelation = TextImageRelation.ImageAboveText;
            _btnCurve.TextAlignment = ContentAlignment.MiddleCenter;
            _btnCurve.Margin = new System.Windows.Forms.Padding(0, 7, 0, 12);
            _btnCurve.Click += new EventHandler(_btnCurve_Click);
            //
            RadRibbonBarButtonGroup group21 = new RadRibbonBarButtonGroup();
            group21.Orientation = Orientation.Vertical;
            _btnLogEnhance = new RadButtonElement("对数增强");
            _btnLogEnhance.ImageAlignment = ContentAlignment.TopCenter;
            _btnLogEnhance.TextImageRelation = TextImageRelation.ImageBeforeText;
            _btnLogEnhance.TextAlignment = ContentAlignment.MiddleCenter;
            _btnLogEnhance.Margin = new System.Windows.Forms.Padding(0, 7, 0, 12);
            _btnLogEnhance.Click += new EventHandler(_btnLogEnhance_Click);            
            //
            _btnExponentEnhance = new RadButtonElement("指数增强");
            _btnExponentEnhance.ImageAlignment = ContentAlignment.TopCenter;
            _btnExponentEnhance.TextImageRelation = TextImageRelation.ImageBeforeText;
            _btnExponentEnhance.TextAlignment = ContentAlignment.MiddleCenter;
            _btnExponentEnhance.Margin  = new System.Windows.Forms.Padding(0, 0, 0, 12);
            _btnExponentEnhance.Click += new EventHandler(_btnExponentEnhance_Click);
            group21.Items.AddRange(new RadItem[] { _btnLogEnhance, _btnExponentEnhance });
            //
            _selectColor = new RadButtonElement("可选颜色");
            _selectColor.ImageAlignment = ContentAlignment.TopCenter;
            _selectColor.TextImageRelation = TextImageRelation.ImageAboveText;
            _selectColor.Click += new EventHandler(_selectColor_Click);

            _replaceColor = new RadButtonElement("颜色替换");
            _replaceColor.ImageAlignment = ContentAlignment.TopCenter;
            _replaceColor.TextImageRelation = TextImageRelation.ImageAboveText;
            _replaceColor.Click += new EventHandler(_replaceColor_Click);

            //
            _dropDownHis = new RadDropDownButtonElement();
            _dropDownHis.ToolTipText = "直方图";
            _dropDownHis.ImageAlignment = ContentAlignment.TopCenter;
            _dropDownHis.TextAlignment = ContentAlignment.BottomCenter;
            _dropDownHis.TextImageRelation = TextImageRelation.ImageAboveText;
            RadMenuHeaderItem itemHeader1 = new RadMenuHeaderItem("直方图拉伸");
            RadMenuItem item1His = new RadMenuItem("1%");
            item1His.Click += new EventHandler(item1His_Click);

            RadMenuItem item2His = new RadMenuItem("2%");
            item2His.Click += new EventHandler(item2His_Click);

            RadMenuItem item3His = new RadMenuItem("5%");
            item3His.Click += new EventHandler(item3His_Click);

            RadMenuItem item4His = new RadMenuItem("自定义");
            item4His.Click += new EventHandler(item4His_Click);

            RadMenuHeaderItem itemHeader2 = new RadMenuHeaderItem("直方图显示");
            RadMenuItem item5HisDisplay = new RadMenuItem("直方图显示");
            item5HisDisplay.Click += new EventHandler(item5HisDisplay_Click);

            _dropDownHis.ArrowPosition = DropDownButtonArrowPosition.Bottom;
            _dropDownHis.Items.AddRange(new RadItem[] { itemHeader1, item1His, item2His,
                                                        item3His,item4His,itemHeader2,item5HisDisplay});
            //

            //
            _hueSaturation = new RadButtonElement("色相/饱和度");
            _hueSaturation.TextImageRelation = TextImageRelation.ImageBeforeText;
            _hueSaturation.Margin = new System.Windows.Forms.Padding(0, 8, 0, 10);
            _hueSaturation.Click += new EventHandler(_hueSaturation_Click);

            _brightContrast = new RadButtonElement("亮度/对比度");
            _brightContrast.TextImageRelation = TextImageRelation.ImageBeforeText;
            _brightContrast.Margin = new System.Windows.Forms.Padding(0, 0, 0, 13);
            _brightContrast.Click += new EventHandler(_brightContrast_Click);
            //

            _leverColor = new RadButtonElement("色阶");
            _leverColor.TextImageRelation = TextImageRelation.ImageBeforeText;
            _leverColor.TextAlignment = ContentAlignment.MiddleRight;
            _leverColor.Click += new EventHandler(_leverColor_Click);
            _leverColor.Margin = new System.Windows.Forms.Padding(0, 4, 2, 0);

            _btnReversal = new RadButtonElement("反相");
            _btnReversal.TextImageRelation = TextImageRelation.ImageBeforeText;
            _btnReversal.TextAlignment = ContentAlignment.MiddleRight;
            _btnReversal.Click += new EventHandler(_btnReversal_Click);

            _btnElimination = new RadButtonElement("去色");
            _btnElimination.TextImageRelation = TextImageRelation.ImageBeforeText;
            _btnElimination.TextAlignment = ContentAlignment.MiddleRight;
            _btnElimination.Click += new EventHandler(_btnElimination_Click);
            _btnElimination.Margin = new System.Windows.Forms.Padding(0, 2, 0, 7);
            //

            _btnSharpen = new RadButtonElement("锐化");
            _btnSharpen.TextAlignment = ContentAlignment.MiddleLeft;
            _btnSharpen.TextImageRelation = TextImageRelation.ImageBeforeText;
            _btnSharpen.Click += new EventHandler(_btnSharpen_Click);
            _btnSharpen.Margin = new System.Windows.Forms.Padding(0, 4, 2, 0);

            _edge = new RadButtonElement("边缘提取");
            _edge.TextAlignment = ContentAlignment.MiddleLeft;
            _edge.TextImageRelation = TextImageRelation.ImageBeforeText;
            _edge.Click += new EventHandler(_edge_Click);

            _hisEquilibria = new RadButtonElement("直方图均衡");
            _hisEquilibria.TextImageRelation = TextImageRelation.ImageBeforeText;
            _hisEquilibria.Click += new EventHandler(_hisEquilibria_Click);
            _hisEquilibria.Margin = new System.Windows.Forms.Padding(0, 2, 0, 7);

            //色相/饱和度，亮度/对比度
            RadRibbonBarButtonGroup subGroup3 = new RadRibbonBarButtonGroup();
            subGroup3.Orientation = Orientation.Vertical;
            subGroup3.Items.AddRange(new RadButtonItem[] { _hueSaturation, _brightContrast });

            //色阶、反相、去色
            RadRibbonBarButtonGroup subGroup2 = new RadRibbonBarButtonGroup();
            subGroup2 = new RadRibbonBarButtonGroup();
            subGroup2.Orientation = Orientation.Vertical;
            subGroup2.Items.AddRange(new RadButtonItem[] { _leverColor, _btnReversal, _btnElimination });
            subGroup2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);

            //锐化、边缘提取、直方图均衡
            RadRibbonBarButtonGroup subGroup1 = new RadRibbonBarButtonGroup();
            subGroup1.Orientation = Orientation.Vertical;
            subGroup1.Items.AddRange(new RadItem[] { _btnSharpen, _edge, _hisEquilibria });
            //

            _dropDownFliter = new RadDropDownButtonElement();
            _dropDownFliter.Text = "滤波";
            _dropDownFliter.ImageAlignment = ContentAlignment.TopCenter;
            _dropDownFliter.TextAlignment = ContentAlignment.BottomCenter;
            _dropDownFliter.TextImageRelation = TextImageRelation.ImageAboveText;
            _dropDownFliter.ArrowPosition = DropDownButtonArrowPosition.Bottom;
            _dropDownFliter.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);

            RadMenuItem itemAve = new RadMenuItem("均值滤波");
            itemAve.Click += new EventHandler(itemAve_Click);

            RadMenuItem itemMid = new RadMenuItem("中值滤波");
            itemMid.Click += new EventHandler(itemMid_Click);

            RadMenuItem itemGus = new RadMenuItem("高斯滤波");
            itemGus.Click += new EventHandler(itemGus_Click);

            RadMenuItem itemCus = new RadMenuItem("自定义滤波");
            itemCus.Click += new EventHandler(itemCus_Click);

            _dropDownFliter.Items.AddRange(new RadItem[] { itemAve, itemMid, itemGus, itemCus });
            RadRibbonBarButtonGroup groupFilter = new RadRibbonBarButtonGroup();
            groupFilter.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            groupFilter.Items.Add(_dropDownFliter);
            //

            _group.Items.AddRange(new RadItem[] { _btnResum, group3, _btnCurve,group21,
                                                  _selectColor,_replaceColor,
                                             //     _dropDownHis,
                                                  subGroup3, subGroup2, subGroup1 ,
                                                  groupFilter});
            //
            _tab.Items.Add(_group);
            Controls.Add(_bar);
        }

        #region Commands
        void itemCus_Click(object sender, EventArgs e)
        {
            GetCommand(3015);
        }

        void itemGus_Click(object sender, EventArgs e)
        {
            GetCommand(3003);
        }

        void itemMid_Click(object sender, EventArgs e)
        {
            GetCommand(3011);
        }

        void itemAve_Click(object sender, EventArgs e)
        {
            GetCommand(3009);
        }

        void item5HisDisplay_Click(object sender, EventArgs e)
        {
            //
        }

        void item4His_Click(object sender, EventArgs e)
        {
            GetCommand(3005);
        }

        void item3His_Click(object sender, EventArgs e)
        {
            GetCommandWithArg(3005, "5%");
        }

        void item2His_Click(object sender, EventArgs e)
        {
            GetCommandWithArg(3005, "2%");
        }

        void item1His_Click(object sender, EventArgs e)
        {
            GetCommandWithArg(3005, "1%");
        }

        void _hisEquilibria_Click(object sender, EventArgs e)
        {
            GetCommand(3004);
        }

        void _edge_Click(object sender, EventArgs e)
        {
            GetCommand(3001);
        }

        void _btnSharpen_Click(object sender, EventArgs e)
        {
            GetCommand(3006);
        }

        void _btnElimination_Click(object sender, EventArgs e)
        {
            GetCommand(3002);
        }

        void _btnReversal_Click(object sender, EventArgs e)
        {
            GetCommand(3013);
        }

        void _leverColor_Click(object sender, EventArgs e)
        {
            GetCommand(3008);
        }

        void _brightContrast_Click(object sender, EventArgs e)
        {
            GetCommand(3007);
        }

        void _hueSaturation_Click(object sender, EventArgs e)
        {
            GetCommand(3000);
        }

        void _replaceColor_Click(object sender, EventArgs e)
        {
            GetCommand(3012);
        }

        void _selectColor_Click(object sender, EventArgs e)
        {
            GetCommand(3014);
        }

        void _redo_Click(object sender, EventArgs e)
        {
            GetCommand(3017);
        }

        void _undo_Click(object sender, EventArgs e)
        {
            GetCommand(3018);
        }

        void _btnResum_Click(object sender, EventArgs e)
        {
            GetCommand(3016);
        }

        void _btnCurve_Click(object sender, EventArgs e)
        {
            GetCommand(3010);
        }

        void _btnLogEnhance_Click(object sender, EventArgs e)
        {
            GetCommand(3020);
        }

        void _btnExponentEnhance_Click(object sender, EventArgs e)
        {
            GetCommand(3021);
        }

        private void GetCommand(int id)
        {
            ICommand cmd = _session.CommandEnvironment.Get(id);
            if (cmd != null)
                cmd.Execute();
        }

        private void GetCommandWithArg(int id, string arg)
        {
            ICommand cmd = _session.CommandEnvironment.Get(id);
            if (cmd != null)
                cmd.Execute(arg);
        }
        #endregion

        public object Content
        {
            get { return _group; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;
            _btnResum.Image = _session.UIFrameworkHelper.GetImage("system:reset2.png");
            _redo.Image = _session.UIFrameworkHelper.GetImage("system:redo.png");
            _undo.Image = _session.UIFrameworkHelper.GetImage("system:undo.png");
            _btnCurve.Image = _session.UIFrameworkHelper.GetImage("system:image-curve.png");
            _selectColor.Image = _session.UIFrameworkHelper.GetImage("system:image-selectColor.png");
            _replaceColor.Image = _session.UIFrameworkHelper.GetImage("system:image-replaceColor.png");
            _btnReversal.Image = _session.UIFrameworkHelper.GetImage("system:image-reverse.png");
            _btnElimination.Image = _session.UIFrameworkHelper.GetImage("system:image-elimination.png");
            _leverColor.Image = _session.UIFrameworkHelper.GetImage("system:image-colorLever.png");
            _btnSharpen.Image = _session.UIFrameworkHelper.GetImage("system:image-sharpen.png");
            _hisEquilibria.Image = _session.UIFrameworkHelper.GetImage("system:image-hisEquilibria.png");
            _edge.Image = _session.UIFrameworkHelper.GetImage("system:image-edge.png");
            _brightContrast.Image = _session.UIFrameworkHelper.GetImage("system:image-brightContrast.png");
            _hueSaturation.Image = _session.UIFrameworkHelper.GetImage("system:image-hue.png");
            _dropDownHis.Image = _session.UIFrameworkHelper.GetImage("system:image-histogram.png");
            _dropDownFliter.Image = _session.UIFrameworkHelper.GetImage("system:image-filter.png");
            _btnExponentEnhance.Image = _session.UIFrameworkHelper.GetImage("system:image-exEnhance.png");
            _btnLogEnhance.Image = _session.UIFrameworkHelper.GetImage("system:image-logEnhance.png");
        }

        public void UpdateStatus()
        {
        }
    }
}
