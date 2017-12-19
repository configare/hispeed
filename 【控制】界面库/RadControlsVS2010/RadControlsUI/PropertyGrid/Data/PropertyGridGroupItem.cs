using System.Collections.Generic;

namespace Telerik.WinControls.UI
{
    public class PropertyGridGroupItem : PropertyGridItemBase
    {
        #region Fields

        private PropertyGridGroup dataGroup;
        private PropertyGridItemCollection items;

        #endregion

        #region Constructors

        public PropertyGridGroupItem(PropertyGridTableElement propertyGridElement, PropertyGridGroup dataGroup)
            : base(propertyGridElement)
        {
            this.dataGroup = dataGroup;
        }

        #endregion

        #region Properties

        public override string Label
        {
            get
            {
                if (base.Label != null)
                {
                    return base.Label;
                }
                return Group.Header;
            }
            set
            {
                base.Label = value;
            }
        }

        /// <summary>
        /// Gets the items collection of the group.
        /// </summary>
        public override PropertyGridItemCollection GridItems
        {
            get
            {
                if (items == null)
                {
                    items = new PropertyGridItemCollection(this.Group.Items);
                }
                return items;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this item is expandable.
        /// </summary>
        public override bool Expandable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the group created by the Group Factory
        /// </summary>
        public PropertyGridGroup Group
        {
            get
            {
                return this.dataGroup;
            }
        }

        public override string Name
        {
            get 
            {
                return Group.Header;
            }
        }

        #endregion
    }
}
