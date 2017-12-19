using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class RadPageViewOutlookItem : RadPageViewStackItem
    {
        #region Fields

        private RadPageViewOutlookAssociatedButton associatedOverflowButton;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the associated overflow button with the current page view item.
        /// This button is displayed below all items in the overflow items panel when the item
        /// is collapsed by using the outlook grip.
        /// When setting this property, the previously set item is disposed.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadPageViewOutlookAssociatedButton AssociatedOverflowButton
        {
            get
            {
                return this.associatedOverflowButton;
            }
            internal set
            {
                if (this.associatedOverflowButton != null && this.associatedOverflowButton != value)
                {
                    this.associatedOverflowButton.Dispose();
                }

                this.associatedOverflowButton = value;

                if (value != null)
                {
                    this.SynchronizeSelectedStateWithAssociatedButton();
                }
            }
        }

        #endregion

        #region Methods

        private void SynchronizeSelectedStateWithAssociatedButton()
        {
            if (associatedOverflowButton != null)
            {
                associatedOverflowButton.SetValue(OverflowItemsContainer.ItemSelectedProperty, this.IsSelected);
            }
        }

        #endregion

        #region Event handling

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == RadPageViewItem.IsSelectedProperty)
            {
                this.SynchronizeSelectedStateWithAssociatedButton();
            }
        }

        #endregion
    }
}
