namespace GeoDo.RSS.MIF.Core
{
    public interface IExtractPanel
    {
        IMonitoringSubProduct MonitoringSubProduct { get; }
        IWorkspace Workspace { get; }
        void Apply(IWorkspace wks, IMonitoringSubProduct subProduct);
        string[] AOIs { get; }
        void SetArg(string argName, object argValue);
        void ResetArgsValue();
        object MonitoringSubProductTag { get; set; }
        void CanResetUserControl();
    }
}
