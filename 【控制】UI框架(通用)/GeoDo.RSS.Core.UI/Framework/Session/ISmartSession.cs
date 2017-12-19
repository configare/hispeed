using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Windows.Forms;
using System.Drawing;

namespace GeoDo.RSS.Core.UI
{
    /// <summary>
    /// 会话，应用程序的顶层指针
    /// </summary>
    public interface ISmartSession
    {
        ICommandEnvironment CommandEnvironment { get; }
        ISmartWindowManager SmartWindowManager { get; }
        IOperationContextView OperationContextView { get; }
        void ChangeOperationContextView(IOperationContextView operationContextView);
        IProgressMonitorManager ProgressMonitorManager { get; }
        IUIFrameworkHelper UIFrameworkHelper { get; }
        IConfiger Configer { get; }
        object PythonEngine { get; }
        object MonitoringSession { get; }
        IRecentFilesManager RecentFilesManager { get; }
        ICloseActionLocker CloseActionLocker { get; }
        void PrintMessage(Exception ex);
        void printMessage(string msg);
        ITemporalFileManager TemporalFileManager { get; }
    }
}
