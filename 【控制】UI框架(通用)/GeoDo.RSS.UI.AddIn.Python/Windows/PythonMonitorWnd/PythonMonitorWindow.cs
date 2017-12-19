using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using GeoDo.PythonEngine;


namespace GeoDo.RSS.UI.AddIn.Python
{
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")] //指定DLL的输出接口以便MEF模块进行查询
    public partial class PythonMonitorWindow : ToolWindow, ISmartToolWindow,IPythonMonitorWindow,ISmartViewer
    {
        private int _id = 20002;
        private ISmartSession _session = null;
        private ListBox lb;
        private IPythonMonitorWindow _me;
        private EventHandler _onWindowClosed;

        public PythonMonitorWindow()
        {
            InitializeComponent();
            _me = this as IPythonMonitorWindow;
        }
        private void init_manager_wnd()
        {
            Text = "输出";
            lb = new ListBox();
            lb.Dock = DockStyle.Fill;
            Controls.Add(lb);
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;
            //启动左侧的脚本管理器窗口
            init_manager_wnd();
        }

        public EventHandler OnWindowClosed
        {
            get { return _onWindowClosed; }
            set { _onWindowClosed = value; }
        }

        public int Id
        {
            get { return _id; }
        }

        public void NotityFetchLog()
        {
            IPythonEngine pyEngine = _session.PythonEngine as IPythonEngine;
            if (pyEngine != null)
            {
                GeoDo.PythonEngine.SimpleLogger.Entry[] items = pyEngine.GetAllLog();
                if (items == null || items.Length == 0)
                    return;
                foreach (GeoDo.PythonEngine.SimpleLogger.Entry it in items)
                    lb.Items.Insert(0,it.msg);
            }
        }

        public string Title
        {
            get { throw new NotImplementedException(); }
        }

        public object ActiveObject
        {
            get { throw new NotImplementedException(); }
        }

        public void CloseWnd()
        { 
        }

        public void DisposeViewer()
        { }
    }
}
