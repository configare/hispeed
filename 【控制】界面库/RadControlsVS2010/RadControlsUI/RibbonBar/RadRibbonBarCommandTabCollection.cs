using System;
using System.Collections;
using Telerik.WinControls;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     <para>
    ///       A collection that stores <see cref='Telerik.WinControls.UI.RadRibbonBarCommandTab'/> objects.
    ///    </para>
    /// </summary>
    /// <seealso cref='Telerik.WinControls.UI.RadRibbonBarCommandTabCollection'/>
    [Serializable()]
    public class RadRibbonBarCommandTabCollection : RadItemOwnerCollection
    {
        #region Fields

        private RadPageViewElement pageViewElement;
        private bool shouldSetParentCollection = true;
        private bool suspendPageViewItemRemoving;

        #endregion

        #region Initialization

        /// <summary>
        /// 	<para>
        ///         Initializes a new instance of the
        ///         <see cref="Telerik.WinControls.UI.RadRibbonBarCommandTabCollection"/>.
        ///     </para>
        /// </summary>
        public RadRibbonBarCommandTabCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Telerik.WinControls.UI.RadRibbonBarCommandTabCollection"/>.
        /// </summary>
        /// <param name="owner">Collection owner.</param>
        public RadRibbonBarCommandTabCollection(RadElement owner)
        {
            RadPageViewStripElement pageViewStripElement = owner as RadPageViewStripElement;
            if (owner != null)
            {
                this.pageViewElement = pageViewStripElement;
                this.Owner = ((RadPageViewStripElement)this.pageViewElement).ItemContainer;
            }
            else
            {
                this.Owner = owner;
            }
        }

        #endregion

        #region Events

        /// <summary>Fires when the collection is changed.</summary>
        public event CollectionChangedHandler CollectionChanged;

        #endregion

        #region Properties

        public bool ShouldSetParentCollection
        {
            get
            {
                return this.shouldSetParentCollection;
            }
            set
            {
                this.shouldSetParentCollection = value;
            }
        }

        #endregion

        #region Event Handlers

        protected override void OnRemoveComplete(int index, object value)
        {
            if (this.suspendPageViewItemRemoving)
            {
                return;
            }

            if (this.shouldSetParentCollection)
            {
                ((RibbonTab)value).ParentCollection = null;
            }

            base.OnRemoveComplete(index, value);

            RadPageViewItem item = (RadPageViewItem)value;
            if (this.pageViewElement != null && this.pageViewElement.Items.Contains(item))
            {
                this.pageViewElement.RemoveItem(item);
            }
        }

        protected override void OnInsertComplete(int index, object value)
        {
            if (this.suspendPageViewItemRemoving)
            {
                return;
            }

            if (this.shouldSetParentCollection)
            {
                ((RibbonTab)value).ParentCollection = this;
            }

            base.OnInsertComplete(index, value);

            RadPageViewItem item = (RadPageViewItem)value;
            if (this.pageViewElement != null && !this.pageViewElement.Items.Contains(item))
            {
                this.pageViewElement.InsertItem(index, item);
                this.EnsureSingleSelectedItem(item);
            }

            if (this.Owner.IsDesignMode)
            {
                this.SuspendNotifications();
                this.suspendPageViewItemRemoving = true;

                this.pageViewElement.UpdateLayout();
                this.Clear();

                foreach (RadItem pageViewItem in this.pageViewElement.Items)
                {
                    this.Add(pageViewItem);
                }

                this.suspendPageViewItemRemoving = false;
                this.ResumeNotifications();
            }

            if (this.CollectionChanged != null)
                this.CollectionChanged(this, new CollectionChangedEventArgs(value, index, ItemsChangeOperation.Inserted));
        }

        private void EnsureSingleSelectedItem(RadPageViewItem newItem)
        {
            if (!newItem.IsSelected)
            {
                return;
            }

            foreach (RadPageViewItem item in this.pageViewElement.Items)
            {
                if (item == newItem)
                {
                    continue;
                }

                if (item.IsSelected)
                {
                    newItem.IsSelected = false;
                    this.pageViewElement.selectedItem = item;
                    break;
                }
            }
        }

        protected override void OnClear()
        {
            if (this.suspendPageViewItemRemoving)
            {
                return;
            }

            foreach (RibbonTab tab in this.InnerList)
            {
                tab.ParentCollection = null;

                if (this.pageViewElement != null && this.pageViewElement.Items.Contains(tab))
                {
                    this.pageViewElement.RemoveItem(tab);
                }
            }

            base.OnClear();
        }

        #endregion
    }
}