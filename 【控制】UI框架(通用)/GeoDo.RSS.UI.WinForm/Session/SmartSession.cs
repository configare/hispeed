using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.PythonEngine;
using GeoDo.RSS.UI.AddIn.Theme;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.WinForm
{
    internal class SmartSession:ISmartSession,ICloseActionLocker,ISmartSessionEvents
    {
        private ICommandEnvironment _commandEnvironment;
        private ISmartWindowManager _smartWindowManager;
        private IConfiger _configer;
        private IUIFrameworkHelper _uiFrameworkHelper;
        private IProgressMonitorManager _progressMonitorManager;
        private IPythonEngine _pythonEngine;
        private IMonitoringSession _monitoringSession;
        private IRecentFilesManager _recentFilesManager;
        private ITemporalFileManager _temporalFileManager;
        private string _taskDescriptionOfLocked;
        private SmartSessionLoadedHandler _smartSessionLoaded;
        private FileOpenedHandler _fileOpened;

        public SmartSession(ISmartWindowManager smartWindowManager)
        {
            _smartWindowManager = smartWindowManager;
            _commandEnvironment = new CommandEnvironment(this);
            _configer = new Configer();
            _progressMonitorManager = new ProgressMonitorManager(smartWindowManager.MainForm as Form);
            _recentFilesManager = new RecentFilesManager(20);
            _temporalFileManager = new TemporalFileManager();
            AttachEvents();
            CreatePythonEngine();
            CreateMonitoringSession();
        }

        public ITemporalFileManager TemporalFileManager
        {
            get { return _temporalFileManager; }
        }

        private void AttachEvents()
        {
            (_smartWindowManager.MainForm as Form).FormClosing += new FormClosingEventHandler(SmartSession_FormClosing);
        }

        void SmartSession_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_taskDescriptionOfLocked != null)
            {
                MsgBox.ShowInfo("正在执行" + _taskDescriptionOfLocked + ",不能关闭系统。");
                e.Cancel = true;
            }
        }

        private void CreateMonitoringSession()
        {
            _monitoringSession = new MonitoringSession(this);
        }

        private void CreatePythonEngine()
        {
            _pythonEngine = new GeoDo.PythonEngine.PythonEngine();
            Dictionary<string, object> vars = new Dictionary<string, object>();
            vars.Add("session", this);
            _pythonEngine.SetupEngine(vars);
        }

        internal void SetUIFrameworkHelper(IUIFrameworkHelper uiFrameworkHelper)
        {
            _uiFrameworkHelper = uiFrameworkHelper;
        }

        public IRecentFilesManager RecentFilesManager
        {
            get { return _recentFilesManager; }
        }

        public object MonitoringSession
        {
            get { return _monitoringSession; }
        }

        public object PythonEngine
        {
            get { return _pythonEngine; }
        }

        public ICommandEnvironment CommandEnvironment
        {
            get { return _commandEnvironment; }
        }

        public ISmartWindowManager SmartWindowManager
        {
            get { return _smartWindowManager; }
        }

        public IOperationContextView OperationContextView
        {
            get { throw new NotImplementedException(); }
        }

        public void ChangeOperationContextView(IOperationContextView operationContextView)
        {
            throw new NotImplementedException();
        }

        public IProgressMonitorManager ProgressMonitorManager
        {
            get { return _progressMonitorManager; }
        }

        public IUIFrameworkHelper UIFrameworkHelper
        {
            get { return _uiFrameworkHelper; }
        }

        public IConfiger Configer
        {
            get { return _configer; }
        }

        public ICloseActionLocker CloseActionLocker
        {
            get { return this; }
        }

        void ICloseActionLocker.Lock(string taskDescription)
        {
            _taskDescriptionOfLocked = taskDescription ?? string.Empty;
        }

        void ICloseActionLocker.Unlock()
        {
            _taskDescriptionOfLocked = null;
        }

        public void PrintMessage(Exception ex)
        {
            IContextMessage msg = _smartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(9006) as IContextMessage;
            if (msg == null)
            {
                ICommand cmd = _commandEnvironment.Get(9006) as ICommand;
                if (cmd != null)
                {
                    cmd.Execute();
                    msg = _smartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(9006) as IContextMessage;
                }
            }
            if (msg != null)
                msg.PrintMessage(ex.Message);
        }

        public void printMessage(string message)
        {
            IContextMessage msg = _smartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(9006) as IContextMessage;
            if (msg == null)
            {
                ICommand cmd = _commandEnvironment.Get(9006) as ICommand;
                if (cmd != null)
                {
                    cmd.Execute();
                    msg = _smartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(9006) as IContextMessage;
                }
            }
            if (msg != null)
                msg.PrintMessage(message);
        }

        SmartSessionLoadedHandler ISmartSessionEvents.OnSmartSessionLoaded
        {
            get { return _smartSessionLoaded; }
            set { _smartSessionLoaded = value; }
        }

        FileOpenedHandler ISmartSessionEvents.OnFileOpended
        {
            get { return _fileOpened; }
            set { _fileOpened = value; }
        }
    }
}
