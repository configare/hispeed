
namespace Telerik.WinControls.Layouts
{
    public interface ILayoutHandler
    {
        ILayoutManager LayoutManager { get; }
        void InvokeLayoutCallback(LayoutCallback callback);
    }
}
