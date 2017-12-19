using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls.UI;

namespace GeoDo.RSS.UI.AddIn.GeoVIS
{
    public partial class UITabGrpGeoVis : UserControl,IUIGroupProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        ISmartSession _session = null;
        RadRibbonBarGroup rbgGlobal = null;

        #region button define
        RadButtonElement btnBegin = null;//启动
        RadButtonElement btnGlobal = null; //全球区域
        RadButtonElement btnChinaRegion = null; //中国区域
        RadButtonElement btnLatLon = null;//经纬网
        RadDropDownButtonElement dropDem = null;  //高程
        RadDropDownButtonElement dropMeasure = null;//测量
        RadButtonElement btnAddData = null; //添加数据
        #endregion

        public UITabGrpGeoVis()
        {
            InitializeComponent();
            CreateTabGroup();
        }

        public object Content
        {
            get { return rbgGlobal; }
        }

        private void CreateTabGroup()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();

            rbgGlobal = new RadRibbonBarGroup();
            rbgGlobal.Text = "数字地球";
            btnBegin = new RadButtonElement("启动");
            btnBegin.TextAlignment = ContentAlignment.BottomCenter;
            btnBegin.ImageAlignment = ContentAlignment.TopCenter;
            btnBegin.Click += new EventHandler(_btnBegin_Click);
            rbgGlobal.Items.Add(btnBegin);

            btnGlobal = new RadButtonElement("全球视图");
            btnGlobal.TextAlignment = ContentAlignment.BottomCenter;
            btnGlobal.ImageAlignment = ContentAlignment.TopCenter;
            btnGlobal.Click += new EventHandler(_btnGlobal_Click);
            rbgGlobal.Items.Add(btnGlobal);

            btnChinaRegion = new RadButtonElement("中国区域");
            btnChinaRegion.TextAlignment = ContentAlignment.BottomCenter;
            btnChinaRegion.ImageAlignment = ContentAlignment.TopCenter;
            btnChinaRegion.Click += new EventHandler(_btnChinaRegion_Click);
            rbgGlobal.Items.Add(btnChinaRegion);

            btnLatLon = new RadButtonElement("经纬网");
            btnLatLon.ImageAlignment = ContentAlignment.TopCenter;
            btnLatLon.TextAlignment = ContentAlignment.BottomCenter;
            btnLatLon.Click += new EventHandler(_btnLatLon_Click);
            rbgGlobal.Items.Add(btnLatLon);

            dropDem = new RadDropDownButtonElement();
            dropDem.Text = "设置高程";
            dropDem.ToolTipText = "设置高程";
            dropDem.TextAlignment = ContentAlignment.BottomCenter;
            dropDem.ImageAlignment = ContentAlignment.TopCenter;
            dropDem.TextImageRelation = TextImageRelation.ImageAboveText;
            RadMenuHeaderItem itemHeaderSelect = new RadMenuHeaderItem("高程系数");
            RadMenuItem itemCoef1 = new RadMenuItem("0.5");
            itemCoef1.Click += new EventHandler(itemCoef1_Click);

            RadMenuItem itemCoef2 = new RadMenuItem("1");
            itemCoef2.Click += new EventHandler(itemCoef2_Click);

            RadMenuItem itemCoef3 = new RadMenuItem("3");
            itemCoef3.Click += new EventHandler(itemCoef3_Click);

            RadMenuItem itemCoef4 = new RadMenuItem("5");
            itemCoef4.Click += new EventHandler(itemCoef4_Click);

            RadMenuItem itemCoef5 = new RadMenuItem("10");
            itemCoef5.Click += new EventHandler(itemCoef5_Click);

            RadMenuItem itemCoef6 = new RadMenuItem("20");
            itemCoef6.Click += new EventHandler(itemCoef6_Click);

            RadMenuHeaderItem itemHeaderCancel = new RadMenuHeaderItem("取消高程");
            itemHeaderCancel.Click += new EventHandler(itemHeaderCancel_Click);
            dropDem.ArrowPosition = DropDownButtonArrowPosition.Right;
            dropDem.Items.AddRange(new RadItem[] { itemHeaderSelect, itemCoef1, itemCoef2,
                                                        itemCoef3,itemCoef4,itemCoef5,itemCoef6,itemHeaderCancel});
            rbgGlobal.Items.Add(dropDem);

            dropMeasure = new RadDropDownButtonElement();
            dropMeasure.Text = "测量";
            dropMeasure.ToolTipText = "测量";
            dropMeasure.TextAlignment = ContentAlignment.BottomCenter;
            dropMeasure.ImageAlignment = ContentAlignment.TopCenter;
            dropMeasure.TextImageRelation = TextImageRelation.ImageAboveText;
            RadMenuItem itemDistance = new RadMenuItem("测量距离");
            itemDistance.Click += new EventHandler(itemDistance_Click);

            RadMenuItem itemArea = new RadMenuItem("测量面积");
            itemArea.Click += new EventHandler(itemArea_Click);
            dropMeasure.Items.AddRange(new RadItem[] { itemDistance, itemArea });
            rbgGlobal.Items.Add(dropMeasure);

            btnAddData = new RadButtonElement("添加数据");
            btnAddData.TextAlignment = ContentAlignment.BottomCenter;
            btnAddData.ImageAlignment = ContentAlignment.TopCenter;
            btnAddData.Click += new EventHandler(_btnAddData_Click);
            rbgGlobal.Items.Add(btnAddData);

            _tab.Items.Add(rbgGlobal);
            _bar.CommandTabs.Add(_tab);
            Controls.Add(_bar);
        }

        void itemDistance_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(8309);
            if (cmd != null)
            {
                cmd.Execute();
            }
        }

        void itemArea_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(8308);
            if (cmd != null)
            {
                cmd.Execute();
            }
        }

        void _btnBegin_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(8302);
            if (cmd != null)
            {
                cmd.Execute();
            }
        }

        void _btnChinaRegion_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(8315);
            if (cmd != null)
            {
                cmd.Execute();
            }
        }

        void _btnAddData_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(8310);
            if (cmd != null)
            {
                cmd.Execute();
            }
        }

        void _btnLatLon_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(8303);
            if (cmd != null)
            {
                cmd.Execute();
            }
        }

        void itemCoef6_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(8311);
            if (cmd != null)
            {
                cmd.Execute((20).ToString());
            }
        }

        void itemCoef5_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(8311);
            if (cmd != null)
            {
                cmd.Execute((10).ToString());
            }
        }

        void itemCoef4_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(8311);
            if (cmd != null)
            {
                cmd.Execute((5.0).ToString());
            }
        }

        void itemCoef3_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(8311);
            if (cmd != null)
            {
                cmd.Execute((3.0).ToString());
            }
        }

        void itemCoef2_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(8311);
            if (cmd != null)
            {
                cmd.Execute((1.0).ToString());
            }
        }

        void itemCoef1_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(8311);
            if (cmd != null)
            {
                cmd.Execute((0.5).ToString());
            }
        }

        void itemHeaderCancel_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(8307);
            if (cmd != null)
            {
                cmd.Execute();
            }
        }

        private void _btnGlobal_Click(object sender, EventArgs e)
        {
            ICommand cmd = _session.CommandEnvironment.Get(8316);
            if (cmd != null)
            {
                cmd.Execute();
            }
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
            btnGlobal.Image = _session.UIFrameworkHelper.GetImage("system:wydgjx.png");
            btnChinaRegion.Image = _session.UIFrameworkHelper.GetImage("system:wydxzqy.png");
            btnLatLon.Image = _session.UIFrameworkHelper.GetImage("system:wydff.png");
            btnAddData.Image = _session.UIFrameworkHelper.GetImage("system:adddata.png");
            dropDem.Image = _session.UIFrameworkHelper.GetImage("system:dem.png");
            btnBegin.Image = _session.UIFrameworkHelper.GetImage("system:play.png");
            dropMeasure.Image = _session.UIFrameworkHelper.GetImage("system:measure.png");
        }

    }
}
