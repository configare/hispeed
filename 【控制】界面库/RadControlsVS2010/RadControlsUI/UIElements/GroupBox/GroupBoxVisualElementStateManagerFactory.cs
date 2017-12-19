using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class GroupBoxVisualElementStateManagerFactory : ItemStateManagerFactory
    {
        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("Disabled");
        }
    }
}
