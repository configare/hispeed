using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Text;
using System.Collections;

namespace Telerik.WinControls
{
    /// <summary>
	///     <para>
    ///       A collection that stores <see cref='Telerik.WinControls.RadItem'/> objects.
	///    </para>
	/// </summary>
    public class RadItemOwnerCollection : RadItemCollection
	{

        private RadElement owner;

		/// <summary>
		///     <para>
		///       Initializes a new instance of <see cref='RadItemCollection'/>.
		///    </para>
		/// </summary>
		public RadItemOwnerCollection(RadElement owner)
		{
			this.owner = owner;
		}

        /// <summary>Initializes a new instance of the RadItemCollection class.</summary>
        public RadItemOwnerCollection()
        {
        }

		/// <summary>
		///     <para>
		///       Initializes a new instance of <see cref='Telerik.WinControls.RadItemCollection'/> based on another <see cref='Telerik.WinControls.RadItemCollection'/>.
		///    </para>
		/// </summary>
		/// <param name='value'>
		///       A <see cref='Telerik.WinControls.RadItemCollection'/> from which the contents are copied
		/// </param>
		public RadItemOwnerCollection(RadItemOwnerCollection value) : base(value)
		{	
		}

		/// <summary>
		///     <para>
        ///       Initializes a new instance of <see cref='Telerik.WinControls.RadItemCollection'/> containing any array of <see cref='Telerik.WinControls.RadItem'/> objects.
		///    </para>
		/// </summary>
		/// <param name='value'>
        ///       A array of <see cref='Telerik.WinControls.RadItem'/> objects with which to intialize the collection
		/// </param>
		public RadItemOwnerCollection(RadItem[] value) : base(value)
		{
		}

		/// <summary>
        /// Gets or sets the owner of the collection.
        /// </summary>
        public RadElement Owner
        {
            get
            { 
                return owner;
            }
            set
            {
				if (this.owner == value)
				{
					return;
				}

				if (this.owner != null)
                {
					this.RemoveAllFromOwner();
                }

                this.owner = value;

                if (this.owner != null)
                {
                    this.SynchronizeAllWithOwner();
                }				
            }
        }
  
        private void SynchronizeAllWithOwner()
        {
            if (!this.owner.UseNewLayoutSystem)
			    this.owner.SuspendLayout();

            foreach (RadItem element in this)
            {
                if (element.Parent != this.owner)
                {
                    this.owner.Children.Add(element);
                }
            }

            if (!this.owner.UseNewLayoutSystem)
			    this.owner.ResumeLayout(true);
        }

        private void RemoveAllFromOwner()
        {
            if (!this.owner.UseNewLayoutSystem)
			    this.owner.SuspendLayout();

            int count = this.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                RadElement child = this[i];
                int index = this.owner.Children.IndexOf(child);
                if (index >= 0)
                {
                    this.owner.Children.RemoveAt(index);
                }
            }

            if (!this.owner.UseNewLayoutSystem)
			    this.owner.ResumeLayout(true);
        }

        protected override void OnItemsChanged(RadItem target, ItemsChangeOperation operation)
        {
            switch (operation)
            {
                case ItemsChangeOperation.Cleared:
                    foreach (RadItem item in this)
                    {
                        item.SetOwnerCollection(null);
                    }
                    break;
                case ItemsChangeOperation.Inserted:
                case ItemsChangeOperation.Set:
                    target.SetOwnerCollection(this);
                    break;
                case ItemsChangeOperation.Removed:
                    target.SetOwnerCollection(null);
                    break;
            }

            //no need to raise notifications in Dispose life cycle
            if (this.owner != null && (this.owner.ElementState == ElementState.Disposing || this.owner.ElementState == ElementState.Disposed))
            {
                return;
            }

            base.OnItemsChanged(target, operation);
        }

        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            if (this.owner != null && this.owner.IsInValidState(false))
            {
                RadItem item = newValue as RadItem;
                int childIndex = this.owner.Children.IndexOf(item);
                if (childIndex >= 0)
                {
                    this.owner.Children[childIndex] = item;
                }
                else
                {
                    this.owner.Children.Add(item);
                }
            }

            base.OnSetComplete(index, oldValue, newValue);
        }

        protected override void OnClear()
        {
            base.OnClear();
            //Clear all the corresponding items from the children collection
            if (this.owner != null && this.owner.IsInValidState(false))
            {
                this.RemoveAllFromOwner();
            }            
        }

		protected override void OnInsertComplete(int index, object value)
		{
            if (this.owner != null)
            {
                RadItem item = value as RadItem;
                if (item.Parent != this.owner)
                {
                    int newIndex = index;
                    if (newIndex > this.owner.Children.Count)
                        newIndex = this.owner.Children.Count;

                    this.owner.Children.Insert(newIndex, item);
                }
            }

            base.OnInsertComplete(index, value);
		}

		protected override void OnRemoveComplete(int index, object value)
		{
            if (this.owner != null && this.owner.IsInValidState(false))
            {
                RadItem item = value as RadItem;
                if (item.Parent == this.owner)
                {
                    this.owner.Children.Remove(item);
                }
            }

            base.OnRemoveComplete(index, value);
		}

        protected override void OnSortComplete()
        {
            if (this.owner != null)
                this.owner.ChangeCollection(null, ItemsChangeOperation.Sorted);
            base.OnSortComplete();
        }
	}	
}