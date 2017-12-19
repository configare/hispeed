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

namespace GeoDo.RSS.UI.AddIn.AddInMgr
{
    public partial class UITabPlugin : UserControl, IUIGroupProvider
    {
        RadRibbonBar _bar = null;
        RibbonTab _tab = null;
        RadRibbonBarGroup rbgUserDef = new RadRibbonBarGroup();
        ISmartSession _session = null;
        RadButtonElement _btnUserAdd = new RadButtonElement("添加插件");

        public UITabPlugin()
        {
            InitializeComponent();
            CreateRibbonBar();
        }

        private void CreateRibbonBar()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Title = "插件";
            _tab.Text = "插件";
            _tab.Name = "插件";

            rbgUserDef.Text = "自定义插件";
            _btnUserAdd.Margin = new Padding(2, 2, 2, 2);
            _btnUserAdd.TextAlignment = ContentAlignment.BottomCenter;
            _btnUserAdd.ImageAlignment = ContentAlignment.TopCenter;
            _btnUserAdd.Click += new EventHandler(_btnUserAdd_Click);
            rbgUserDef.Items.Add(_btnUserAdd);
        }

        public object Content
        {
            get { return rbgUserDef; }
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
            _btnUserAdd.Image = _session.UIFrameworkHelper.GetImage("system:plugin_add.png");
        }

        void _btnUserAdd_Click(object sender, EventArgs e)
        {
            frmAddPlugin pluginForm = new frmAddPlugin(_session, Content as RadRibbonBarGroup);
            pluginForm.TopMost = true;
            pluginForm.Show();
        }
    }
}
