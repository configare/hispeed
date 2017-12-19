using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    // 22.March.2010 - Svetlin
    // This class is created due to temporary solution.
    // DISADVANTAGE IN THEMES: When you have disabled state you cannot combine it with another states.
    // This causes to not have disabled expand image when the node in RadTreeView is expanded.
    public class TreeExpanderStateManager : ItemStateManager
    {
        public TreeExpanderStateManager(StateNodeBase rootNode)
            : base(rootNode)
        {

        }

        public override void ItemStateChanged(RadItem senderItem, RadPropertyChangedEventArgs changeArgs)
        {
            TreeNodeExpanderItem expanderItem = senderItem as TreeNodeExpanderItem;

            if (changeArgs != null && !expanderItem.Enabled)
            {
                if (expanderItem.Expanded)
                {
                    this.SetItemState(senderItem, "Disabled" + ItemStateManagerBase.stateDelimiter + "IsExpanded");
                    return;
                }
            }

            base.ItemStateChanged(senderItem, changeArgs);
        }
    }
}
