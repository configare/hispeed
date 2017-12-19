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
using System.ComponentModel.Composition;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using Telerik.WinControls.UI.Data;
using System.IO;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.GDIPlus;
using GeoDo.RSS.Layout.Elements;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public partial class ThemeTemplateSelectWindow : ToolWindow, ISmartToolWindow
    {
        private int _id = 5002;
        private ISmartSession _smartSession = null;
        private UCTemplateSelectWindow _tempSel = null;
        private EventHandler _onWindowClosed;
        private string _selectedTheme;

        public ThemeTemplateSelectWindow()
        {
            InitializeComponent();
            Text = "选择专题图模版";
            CreatUC();
        }

        private void CreatUC()
        {
            _tempSel = new UCTemplateSelectWindow();
            _tempSel.Dock = DockStyle.Fill;
            this.Controls.Add(_tempSel);
            _tempSel.TemplateClicked += new TemplateClickedHandler(TemplateClick);
        }

        void TemplateClick(object sender, ILayoutTemplate template)
        {
            _selectedTheme = template.FullPath;
            if (_smartSession.SmartWindowManager.ActiveViewer is ILayoutViewer)
            {
                (_smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer).LayoutHost.ApplyTemplate(template);
                (_smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer).LayoutHost.Render();
            }
        }

        public string SelectedTheme
        {
            get { return _selectedTheme; }
        }

        public EventHandler OnWindowClosed
        {
            get { return _onWindowClosed; }
            set { _onWindowClosed = value; }
        }

        #region ToolWindow members
        public int Id
        {
            get { return _id; }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _smartSession = session;         
        }
        #endregion
    }
}
