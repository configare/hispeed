using Telerik.WinControls.Data;

namespace Telerik.WinControls.UI
{
    public class PropertyGridGroup : DataItemGroup<PropertyGridItem>
    {
        #region Fields

        private PropertyGridGroupItem groupData;
        
        #endregion

        #region Constructors

        public PropertyGridGroup(object key, Group<PropertyGridItem> parent, PropertyGridTableElement propertyGridElement)
            : base(key, parent)
        {
            this.groupData = new PropertyGridGroupItem(propertyGridElement, this);
            this.groupData.SuspendPropertyNotifications();
            this.groupData.Expanded = propertyGridElement.AutoExpandGroups;            
            this.groupData.ResumePropertyNotifications();
        }

        #endregion 

        #region Method

        /// <summary>
        /// Expands this instance.
        /// </summary>
        public void Expand()
        {
            this.groupData.Expanded = true;
        }

        /// <summary>
        /// Collapses this instance.
        /// </summary>
        public void Collapse()
        {
            this.groupData.Expanded = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the group item.
        /// </summary>
        /// <value>The group item.</value>
        public PropertyGridGroupItem GroupItem
        {
            get
            {
                return this.groupData;
            }
        }

        /// <summary>
        /// Gets the expanded state of the group.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.groupData.Expanded;
            }
        }

        #endregion
    }
}
