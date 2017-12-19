using System;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public interface IMonitoringSession : IDisposable, IEnvironmentVarProvider
    {
        ICanvasViewer CurrentCanvasViewer { get; }
        IMonitoringProduct ActiveMonitoringProduct { get; }
        IMonitoringSubProduct ActiveMonitoringSubProduct { get; }
        IMonitoringProduct ChangeActiveProduct(string identify);
        IMonitoringSubProduct ChangeActiveSubProduct(string identify);
        IMonitoringProduct ChangeActiveProduct(string identify, bool isOpenWorkspace);
        void DoAutoExtract();
        void DoAutoExtract(bool isOpenWorkspace);
        void DoManualExtract();
        void DoManualExtract(bool isOpenWorkspace);
        IWorkspace GetWorkspace();
        IWorkspace Workspace { get; }
        IExtractPanelWindow ExtractPanelWidow { get; }
        IExtractingSession ExtractingSession { get; }
        IThemeGraphGenerator ThemeGraphGenerator { get; }
        IFileNameGenerator FileNameGenerator { get; }
        void Close();
        IMonitoringProduct FindMonitoringProduct(string identify);
        void CanResetUserControl();
    }
}
