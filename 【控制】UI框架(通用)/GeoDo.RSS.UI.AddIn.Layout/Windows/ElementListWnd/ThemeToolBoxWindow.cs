using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Layout.Elements;
using GeoDo.RSS.Layout;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public partial class ThemeToolBoxWindow : ToolWindow, ISmartToolWindow
    {
        private int _id = 5001;
        private ISmartSession _session = null;
        private string _text = null;
        private UCElementsListView _eleList = null;
        private EventHandler _onWindowClosed;

        public ThemeToolBoxWindow()
        {
            InitializeComponent();
            Text = "整饰工具箱";
            CreatToolbox();
        }

        public ThemeToolBoxWindow(string text)
            : base(text)
        {
            _text = text;
            InitializeComponent();
            Text = "整饰工具箱";
            CreatToolbox();
        }

        private void CreatToolbox()
        {
            _eleList = new UCElementsListView();
            _eleList.Dock = DockStyle.Fill;
            TryAddElementsCustom();
            this.Controls.Add(_eleList);
        }

        private void TryAddElementsCustom()
        {
            if (String.IsNullOrEmpty(_text))
                return;
            if (_text.ToLower() == "true")
                AddElementCustom();
        }

        private void AddElementCustom()
        {
            if (_session.SmartWindowManager.ActiveCanvasViewer == null)
                return;
            ILayoutViewer viewer = _session.SmartWindowManager.ActiveCanvasViewer as ILayoutViewer;
            if (viewer == null)
                return;
            ILayoutHost host = viewer.LayoutHost;
            if (host == null)
                return;
            ILayoutRuntime runtime = host.LayoutRuntime;
            if (runtime == null)
                return;
            IElement[] eles = runtime.Selection;
            if (eles == null || eles.Length == 0)
                return;
            if (_eleList == null)
                return;
            _eleList.CustomElements = eles;
        }

        public int Id
        {
            get { return _id; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;
        }

        public EventHandler OnWindowClosed
        {
            get { return _onWindowClosed; }
            set { _onWindowClosed = value; }
        }
    }
}
