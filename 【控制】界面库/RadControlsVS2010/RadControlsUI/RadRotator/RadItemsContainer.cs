using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents TPF controls container
    /// </summary>
    [ToolboxItem(false), ComVisible(false)]
    public class RadItemsContainer : RadItem
    {
        private RadItemOwnerCollection items;

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
            this.ShouldHandleMouseInput = true;

            items = new RadItemOwnerCollection();
            items.ItemTypes = new Type[] {
                typeof(RadItemsContainer),
                typeof(RadRotatorElement),
                typeof(RadArrowButtonElement),
                typeof(RadComboBoxElement),
                typeof(RadButtonElement),
                typeof(RadWebBrowserElement),
                typeof(RadTextBoxElement),
                typeof(RadImageButtonElement),
                typeof(RadCheckBoxElement),
                typeof(RadMaskedEditBoxElement)
            };
            items.Owner = this;
            items.ItemsChanged += new ItemChangedDelegate(items_ItemsChanged);
        }

        protected override void DisposeManagedResources()
        {
            items.ItemsChanged -= new ItemChangedDelegate(items_ItemsChanged);

            base.DisposeManagedResources();
        }

        private void items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            if (target != null)
            {
                target.NotifyParentOnMouseInput = true;
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="RadItem"/>s contained in the <see cref="RadItemsContainer"/>
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection Items
        {
            get
            {
                return items;
            }
        }

    }
}
