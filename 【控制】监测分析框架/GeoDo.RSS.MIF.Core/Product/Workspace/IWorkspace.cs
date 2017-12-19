using System;

namespace GeoDo.RSS.MIF.Core
{
    public interface IWorkspace
    {
        WorkspaceDef Definition { get; }
        StrategyFilter StrategyFilter { get; }
        ICatalog ActiveCatalog { get; }
        ICatalog GetCatalog(string classIdentify);
        void ChangeTo(ICatalog catalog);
        object GetEnvironmentVar(string identify);
        ICatalog GetCatalogByIdentify(string identify);
        string GetNewestFile(string subProductIdentify);
        void Apply(string productIdentify);
        IWorkspace Workspace { get; }
        void SetDoubleClickHandler(Action<object> itemDoubleClickedHandler);
    }
}
