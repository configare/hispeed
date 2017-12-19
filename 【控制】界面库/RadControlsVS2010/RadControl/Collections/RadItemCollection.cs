using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using Telerik.WinControls.Design;
using System.Diagnostics;

namespace Telerik.WinControls
{
    /// <summary>
    /// 
    /// </summary>
    public enum ItemsChangeOperation
    {
        /// <summary>
        /// Indicates that an insert operation will commence.
        /// </summary>
        Inserting = 0,
        /// <summary>
        /// Indicates that an insert operation is performed.
        /// </summary>
        Inserted,
        /// <summary>
        /// Indicates that a remove operation will commence.
        /// </summary>
        Removing,
        /// <summary>
        /// Indicates that a remove operation is performed.
        /// </summary>
        Removed,
        /// <summary>
        /// Indicates that an item is going to be set
        /// </summary>
        Setting,
        /// <summary>
        /// Indicates that an item is set
        /// </summary>
        Set,
        /// <summary>
        /// Indicates that the items will be cleared
        /// </summary>
        Clearing,
        /// <summary>
        /// Indicates that the items are cleared
        /// </summary>
        Cleared,
        /// <summary>
        /// Indicates that the items will be sorted
        /// </summary>
        Sorting,
        /// <summary>
        /// Indicates that the items are sorted
        /// </summary>
        Sorted
    }

    /// <summary>
    /// Represents the method that will handle the %ItemChanged:ItemChanged% event.
    /// </summary>
    /// <param name="changed"></param>
    /// <param name="target"></param>
    /// <param name="operation"></param>
    public delegate void ItemChangedDelegate(RadItemCollection changed, RadItem target, ItemsChangeOperation operation);

    /// <summary>
    ///     <para>
    ///       A collection that stores <see cref='Telerik.WinControls.RadItem'/> objects.
    ///    </para>
    /// </summary>
    [Editor( DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor))]
    [Serializable()]
    //[ListBindable(false)]
    [DebuggerDisplay("Count = {Count}")]
    public class RadItemCollection : CollectionBase, IEnumerable<RadItem>
    {
        #region Events

        /// <summary>
        /// Fires when item is changed.
        /// </summary>
        public event ItemChangedDelegate ItemsChanged;
        private int suspendNotifyCount;

        protected virtual void OnItemsChanged(RadItem target, ItemsChangeOperation operation)
        {
            if (this.suspendNotifyCount > 0)
            {
                return;
            }

            if (this.ItemsChanged != null)
                ItemsChanged(this, target, operation);
        }

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the RadItemCollectionBase class.</summary>
        public RadItemCollection()
        {
        }

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of RadItemCollection based on another RadItemCollection.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A RadItemCollection from which the contents are copied.
        /// </param>
        public RadItemCollection(RadItemCollection value)
        {
            this.AddRange(value);
        }

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of RadItemCollection containing any array of <see cref='Telerik.WinControls.RadItem'/> objects.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A array of <see cref='Telerik.WinControls.RadItem'/> objects with which to intialize the collection
        /// </param>
        public RadItemCollection(RadItem[] value)
        {
            this.AddRange(value);
        }

        #endregion

        protected override void OnValidate(object value)
        {
            if (!(value is RadItem))
            {
                throw new InvalidOperationException("Collection contains only instances of Type RadItem");
            }
        }

        public void SuspendNotifications()
        {
            this.suspendNotifyCount++;
        }

        public void ResumeNotifications()
        {
            if (this.suspendNotifyCount > 0)
            {
                this.suspendNotifyCount--;
            }
        }

        #region Design time support

        private Type[] itemTypes = new Type[] { typeof(RadItem) };
        /// <summary>
        /// Gets or sets an array of the items' types in the collection.
        /// </summary>
        public virtual Type[] ItemTypes
        {
            get
            {
                return itemTypes;
            }
            set
            {
                itemTypes = value;
            }
        }

		private Type[] excludedTypes;
		/// <summary>
		/// Gets or sets an array of the excluded items' types for this collection.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Type[] ExcludedTypes
		{
			get
			{
				return this.excludedTypes;
			}
			set
			{
				this.excludedTypes = value;
			}
		}

		private Type[] sealedTypes;
		/// <summary>
		/// Gets or sets an array of the sealed items' types for this collection. 
		/// That are types that are allowed but not their descendants.
		/// </summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual Type[] SealedTypes
		{
			get
			{
				return this.sealedTypes;
			}
			set
			{
				this.sealedTypes = value;
			}
		}

        private Type defaultType = null;

        public virtual Type DefaultType
        {
            get
            {
                return defaultType;
            }
            set
            {
                defaultType = value;
            }
        }

        #endregion

        #region IEnumerable<RadItem> Members

        IEnumerator<RadItem> IEnumerable<RadItem>.GetEnumerator()
        {
            return new RadItemEnumerator(this);
        }

        #endregion

        /// <summary>
        ///    <para>Returns an enumerator that can iterate through 
        ///       the RadItemCollection .</para>
        /// </summary>
        /// <returns><para>None.</para></returns>
        public new RadItemEnumerator GetEnumerator()
        {
            return new RadItemEnumerator(this);
        }

        #region OnXXX and ItemsChanged Event

        //TODO: Uncomment this when notifications are refactored
        //protected override void OnInsert(int index, object value)
        //{
        //    OnItemsChanged((RadItem)value, ItemsChangeOperation.Inserting);
        //}

        protected override void OnInsertComplete(int index, object value)
        {
            OnItemsChanged((RadItem)value, ItemsChangeOperation.Inserted);
        }

        protected override void OnRemove(int index, object value)
        {
            OnItemsChanged((RadItem)value, ItemsChangeOperation.Removing);
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            OnItemsChanged((RadItem)value, ItemsChangeOperation.Removed);
        }

        protected override void OnSet(int index, object oldValue, object newValue)
        {
            OnItemsChanged((RadItem)newValue, ItemsChangeOperation.Setting);
        }

        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            OnItemsChanged((RadItem)newValue, ItemsChangeOperation.Set);
        }

        protected override void OnClear()
        {
            OnItemsChanged(null, ItemsChangeOperation.Clearing);
        }

        protected override void OnClearComplete()
        {
            OnItemsChanged(null, ItemsChangeOperation.Cleared);
        }

        protected virtual void OnSort()
        {
            OnItemsChanged(null, ItemsChangeOperation.Sorting);
        }

        protected virtual void OnSortComplete()
        {
            OnItemsChanged(null, ItemsChangeOperation.Sorted);
        }

        #endregion

        #region Operations

        /// <summary>
        ///    <para>Adds a <see cref='Telerik.WinControls.RadItem'/> with the specified value to the 
        ///    Telerik.WinControls.RadItemCollection .</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.RadItem'/> to add.</param>
        /// <returns>
        ///    <para>The index at which the new element was inserted.</para>
        /// </returns>
        public int Add(RadItem value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// <para>Copies the elements of an array to the end of the RadItemCollection.</para>
        /// </summary>
        /// <param name='value'>
        ///    An array of type <see cref='Telerik.WinControls.RadItem'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        public void AddRange(params RadItem[] value)
        {
            for (int i = 0; (i < value.Length); i = (i + 1))
            {
                this.Add(value[i]);
            }
        }

        /// <summary>
        ///     <para>
        ///       Adds the contents of another RadItemCollection to the end of the collection.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///    A RadItemCollection containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        public void AddRange(RadItemCollection value)
        {
            for (int i = 0; (i < value.Count); i = (i + 1))
            {
                this.Add(value[i]);
            }
        }

        /// <summary>
        /// <para>Inserts a <see cref='Telerik.WinControls.RadItem'/> into the RadItemCollection at the specified index.</para>
        /// </summary>
        /// <param name='index'>The zero-based index where <paramref name='value'/> should be inserted.</param>
        /// <param name=' value'>The <see cref='Telerik.WinControls.RadItem'/> to insert.</param>
        /// <returns><para>None.</para></returns>
        public void Insert(int index, RadItem value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        ///    <para> Removes a specific <see cref='Telerik.WinControls.RadItem'/> from the 
        ///    RadItemCollection .</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.RadItem'/> to remove from the RadItemCollection .</param>
        /// <returns><para>None.</para></returns>
        /// <exception cref='System.ArgumentException'><paramref name='value'/> is not found in the Collection. </exception>
        public void Remove(RadItem value)
        {
            int itemIndex = List.IndexOf(value);
            if (itemIndex >= 0)
            {
                List.RemoveAt(itemIndex);
            }
        }

        /// <summary>
        /// <para>Sorts the elements in the entire <see cref='Telerik.WinControls.RadElementCollection'/> using the IComparable implementation of each element.</para>
        /// </summary>
        public void Sort()
        {
            this.OnSort();
            this.InnerList.Sort();
            this.OnSortComplete();
        }

        /// <summary>
        /// <para>Sorts the elements in the entire <see cref='Telerik.WinControls.RadElementCollection'/> using the specified comparer.</para>
        /// </summary>
        /// <param name="comparer">The IComparer implementation to use when comparing elements.</param>
        public void Sort(IComparer comparer)
        {
            this.OnSort();
            this.InnerList.Sort(comparer);
            this.OnSortComplete();
        }

        /// <summary>
        /// <para>Sorts the elements in a range of elements in <see cref='Telerik.WinControls.RadElementCollection'/> using the specified comparer.</para>
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to sort. </param>
        /// <param name="count">The length of the range to sort. </param>
        /// <param name="comparer">The IComparer implementation to use when comparing elements.</param>
        public void Sort(int index, int count, IComparer comparer)
        {
            this.OnSort();
            this.InnerList.Sort(index, count, comparer);
            this.OnSortComplete();
        }

        /// <summary>
        /// <para>Gets a value indicating whether the 
        ///    RadItemCollection contains the specified <see cref='Telerik.WinControls.RadItem'/>.</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.RadItem'/> to locate.</param>
        /// <returns>
        /// <para><see langword='true'/> if the <see cref='Telerik.WinControls.RadItem'/> is contained in the collection; 
        ///   otherwise, <see langword='false'/>.</para>
        /// </returns>
        public bool Contains(RadItem value)
        {
            return List.Contains(value);
        }

        /// <summary>
        ///    <para>Returns the index of a <see cref='Telerik.WinControls.RadItem'/> in 
        ///       the RadItemCollection .</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.RadItem'/> to locate.</param>
        /// <returns>
        /// <para>The index of the <see cref='Telerik.WinControls.RadItem'/> of <paramref name='value'/> in the 
        /// RadItemCollection, if found; otherwise, -1.</para>
        /// </returns>
        public int IndexOf(RadItem value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// <para>Copies the RadItemCollection values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </summary>
        /// <param name='array'><para>The one-dimensional <see cref='System.Array'/> that is the destination of the values copied from RadItemCollection .</para></param>
        /// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <exception cref='System.ArgumentException'><para><paramref name='array'/> is multidimensional.</para> <para>-or-</para> <para>The number of elements in the RadItemCollection is greater than the available space between <paramref name='index'/> and the end of <paramref name='array'/>.</para></exception>
        /// <exception cref='System.ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is less than <paramref name='array'/>'s lowbound. </exception>
        public void CopyTo(RadItem[] array, int index)
        {
            List.CopyTo(array, index);
        }

        /// <summary>Retrieves an array of the items in the collection.</summary>
        public RadItem[] ToArray()
        {
            RadItem[] res = new RadItem[this.Count];
            this.CopyTo(res, 0);
            return res;
        }

        /// <summary>
        /// <para>Represents the entry at the specified index of the <see cref='Telerik.WinControls.RadItem'/>.</para>
        /// </summary>
        /// <param name='index'><para>The zero-based index of the entry to locate in the collection.</para></param>
        /// <value>
        ///    <para> The entry at the specified index of the collection.</para>
        /// </value>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
        public RadItem this[int index]
        {
            get
            {
                return (RadItem)List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        [Browsable(false)]
        public RadItem First
        {
            get
            {
                return this.Count == 0 ? null : this[0];
            }
        }

        [Browsable(false)]
        public RadItem Last
        {
            get
            {
                return this.Count == 0 ? null : this[this.Count - 1];
            }
        }

        /// <summary>
        /// Gets the first found item, with Name property equal to itemName specified, case-sensitive.
        /// </summary>
        /// <param name="itemName">item Name</param>
        /// <returns>RadItem if found, null (Nothing in VB.NET) otherwise</returns>
        public RadItem this[string itemName]
        {
            get
            {
                for (int i = 0; i < this.List.Count; i++)
                {
                    RadItem currRitem = (RadItem)this.List[i];
                    if (currRitem.Name == itemName)
                    {
                        return currRitem;
                    }
                }

                return null;
            }
        }

        #endregion

        /// <summary>
        /// Represents an element enumerator. 
        /// </summary>
        public class RadItemEnumerator : IEnumerator, IEnumerator<RadItem>
        {

            private IEnumerator baseEnumerator;

            private IEnumerable temp;

            /// <summary>
            /// Initializes a new instance of the RadElementEnumerator class.
            /// </summary>
            /// <param name="mappings"></param>
            public RadItemEnumerator(RadItemCollection mappings)
            {
                this.temp = ((IEnumerable)(mappings));
                this.baseEnumerator = temp.GetEnumerator();
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            public RadItem Current
            {
                get
                {
                    return ((RadItem)(baseEnumerator.Current));
                }
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return baseEnumerator.Current;
                }
            }

            /// <summary>
            /// Moves to the next element in the collection.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            /// <summary>
            /// Moves to the the next element of the collection.
            /// </summary>
            /// <returns></returns>
            bool IEnumerator.MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            /// <summary>
            /// Resets the enumerator position.
            /// </summary>
            public void Reset()
            {
                baseEnumerator.Reset();
            }

            /// <summary>
            /// Resets the enumerator position.
            /// </summary>
            void IEnumerator.Reset()
            {
                baseEnumerator.Reset();
            }

            #region IDisposable Members

            /// <summary>
            /// Disposes the enumeration.
            /// </summary>
            void IDisposable.Dispose()
            {
                //
            }

            #endregion
        }
    }
}
