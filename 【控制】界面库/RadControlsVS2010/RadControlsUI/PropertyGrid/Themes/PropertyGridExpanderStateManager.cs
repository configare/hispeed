using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    // 22.March.2010 - Svetlin
    // This class is created due to temporary solution.
    // DISADVANTAGE IN THEMES: When you have disabled state you cannot combine it with another states.
    // This causes to not have disabled expand image when the item in RadPropertyGrid is expanded.
    public class PropertyGridExpanderStateManager : ItemStateManager
    {
        public PropertyGridExpanderStateManager(StateNodeBase rootNode)
            : base(rootNode)
        {

        }

        public override void ItemStateChanged(RadItem senderItem, RadPropertyChangedEventArgs changeArgs)
        {
            PropertyGridExpanderElement expanderElement = senderItem as PropertyGridExpanderElement;

            if (changeArgs != null && !expanderElement.Enabled)
            {
                if (expanderElement.ExpanderItem.Expanded)
                {
                    this.SetItemState(senderItem, "Disabled" + ItemStateManagerBase.stateDelimiter + "IsExpanded");
                    return;
                }
            }

            base.ItemStateChanged(senderItem, changeArgs);
        }
    }
}
