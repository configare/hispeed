using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RadDropDownListEditableAreaElementStateManagerFactory : ItemStateManagerFactory
    {
        protected override void AddDefaultVisibleStates(ItemStateManager sm)
        {
            sm.AddDefaultVisibleState("Disabled");
        }
    }
}
