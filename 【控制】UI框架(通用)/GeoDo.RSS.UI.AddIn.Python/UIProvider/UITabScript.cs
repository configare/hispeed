using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls.UI.Docking;
using FastColoredTextBoxNS;
using Telerik.WinControls.UI;
using GeoDo.PythonEngine;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Python
{
    public partial class UITabScript : UserControl, IUITabProvider
    {
        RibbonTab _tab = null;
        ISmartSession _session = null;
        RadRibbonBar _bar = null;
        Dictionary<string, RadButtonElement> button_items;

        public UITabScript()
        {
            InitializeComponent();
            button_items = new Dictionary<string, RadButtonElement>();
            CreateRibbonTab();
        }

        private void CreateRibbonTab()
        {
            _bar = new RadRibbonBar();
            _bar.Dock = DockStyle.Top;
            _tab = new RibbonTab();
            _tab.Text = "脚本开发";
            _bar.CommandTabs.Add(_tab);
            //
            RadRibbonBarGroup g = new RadRibbonBarGroup();
            g.Text = "脚本管理";
            _tab.Items.Add(g);
            RadButtonElement btn;
            btn = new RadButtonElement("打开工作区");
            btn.Click += new EventHandler(btn_Click);
            btn.Tag = 20001;
            button_items.Add("打开工作区", btn);
            g.Items.Add(btn);
            btn = new RadButtonElement("新建脚本");
            btn.Click += new EventHandler(btn_Click);
            button_items.Add("新建脚本", btn);
            g.Items.Add(btn);
            btn = new RadButtonElement("删除文件");
            btn.Click += new EventHandler(btn_Click);
            button_items.Add("删除脚本", btn);
            g.Items.Add(btn);
            btn = new RadButtonElement("关闭工作区");
            btn.Click += new EventHandler(btn_Click);
            button_items.Add("关闭工作区", btn);
            g.Items.Add(btn);
            btn = new RadButtonElement("保存");
            btn.Click += new EventHandler(btn_Click);
            button_items.Add("保存", btn);
            g.Items.Add(btn);
            g = new RadRibbonBarGroup();
            g.Text = "运行脚本";
            _tab.Items.Add(g);
            btn = new RadButtonElement("运行");
            btn.Click += new EventHandler(btn_Click);
            button_items.Add("运行", btn);
            g.Items.Add(btn);
            //btn = new RadButtonElement("暂停");
            //btn.Click += new EventHandler(btn_Click);
            //button_items.Add("暂停", btn);
            //g.Items.Add(btn);
            //btn = new RadButtonElement("停止");
            //btn.Click += new EventHandler(btn_Click);
            //button_items.Add("停止", btn);
            //g.Items.Add(btn);
        }

        public object Content
        {
            get { return _tab; }
        }

        void btn_Click(object sender, EventArgs e)
        {
            RadButtonElement btn;
            btn = sender as RadButtonElement;
            if (btn.Text == "打开工作区")
            {
                _session.CommandEnvironment.Get(20001).Execute();
                _session.CommandEnvironment.Get(20002).Execute();
            }
            if (btn.Text == "关闭工作区")
            {
                _session.CommandEnvironment.Get(20007).Execute();
            }
            else if (btn.Text == "运行")
            {
                _session.CommandEnvironment.Get(20008).Execute();
            }
            else if (btn.Text == "保存")
            {
                _session.CommandEnvironment.Get(20004).Execute("ALL");
            }
            else if (btn.Text == "新建脚本")
            {
                _session.CommandEnvironment.Get(20005).Execute();
            }
            else if (btn.Text == "删除文件")
            {
                _session.CommandEnvironment.Get(20006).Execute();

            }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;
            string tmp;
            tmp = "打开工作区";
            button_items[tmp].Image = _session.UIFrameworkHelper.GetImage("system:document-library.png");
            button_items[tmp].ImageAlignment = ContentAlignment.TopCenter;
            button_items[tmp].TextAlignment = ContentAlignment.BottomCenter;
            tmp = "新建脚本";
            button_items[tmp].Image = _session.UIFrameworkHelper.GetImage("system:issue.png");
            button_items[tmp].ImageAlignment = ContentAlignment.TopCenter;
            button_items[tmp].TextAlignment = ContentAlignment.BottomCenter;
            tmp = "删除脚本";
            button_items[tmp].Image = _session.UIFrameworkHelper.GetImage("system:delete32.png");
            button_items[tmp].ImageAlignment = ContentAlignment.TopCenter;
            button_items[tmp].TextAlignment = ContentAlignment.BottomCenter;
            tmp = "关闭工作区";
            button_items[tmp].Image = _session.UIFrameworkHelper.GetImage("system:findfile.png");
            button_items[tmp].ImageAlignment = ContentAlignment.TopCenter;
            button_items[tmp].TextAlignment = ContentAlignment.BottomCenter;
            tmp = "保存";
            button_items[tmp].Image = _session.UIFrameworkHelper.GetImage("system:save.png");
            button_items[tmp].ImageAlignment = ContentAlignment.TopCenter;
            button_items[tmp].TextAlignment = ContentAlignment.BottomCenter;
            tmp = "运行";
            button_items[tmp].Image = _session.UIFrameworkHelper.GetImage("system:play.png");
            button_items[tmp].ImageAlignment = ContentAlignment.TopCenter;
            button_items[tmp].TextAlignment = ContentAlignment.BottomCenter;
            //tmp = "暂停";
            //button_items[tmp].Image = _session.UIFrameworkDefinition.GetImage("system:pause.png");
            //button_items[tmp].ImageAlignment = ContentAlignment.TopCenter;
            //button_items[tmp].TextAlignment = ContentAlignment.BottomCenter;
            //tmp = "停止";
            //button_items[tmp].Image = _session.UIFrameworkDefinition.GetImage("system:stop.png");
            //button_items[tmp].ImageAlignment = ContentAlignment.TopCenter;
            //button_items[tmp].TextAlignment = ContentAlignment.BottomCenter;
        }

        public void UpdateStatus()
        {
        }
    }
}
