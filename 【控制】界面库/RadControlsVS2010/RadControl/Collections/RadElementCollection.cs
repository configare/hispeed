using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace Telerik.WinControls
{
	/// <summary>
	///     <para>
	///       A collection that stores <see cref='Telerik.WinControls.RadElement'/> objects.
	///    </para>
	/// </summary>
	[Serializable()]
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	public class RadElementCollection : CollectionBase, IEnumerable<RadElement>
	{
		private RadElement owner;
		private bool suspended;

		/// <summary>
		///     <para>
		///       Initializes a new instance of the <see cref='Telerik.WinControls.RadElementCollection'/>.
		///    </para>
		/// </summary>
		public RadElementCollection(RadElement owner): base(1)
		{
			this.owner = owner;
		}

		/// <summary>
		///     <para>
		///       Initializes a new instance of the <see cref='Telerik.WinControls.RadElementCollection'/> based on another <see cref='Telerik.WinControls.RadElementCollection'/>.
		///    </para>
		/// </summary>
        /// <param name="owner"></param>
		/// <param name='value'>
		///       A <see cref='Telerik.WinControls.RadElementCollection'/> from which the contents are copied
		/// </param>
		public RadElementCollection(RadElement owner, RadElementCollection value)
			: base(1)
		{
			this.owner = owner;
			this.AddRange(value);
		}

		/// <summary>
		///     <para>
		///       Initializes a new instance of <see cref='Telerik.WinControls.RadElementCollection'/> containing any array of <see cref='Telerik.WinControls.RadElement'/> objects.
		///    </para>
		/// </summary>
        /// <param name="owner"></param>
		/// <param name='value'>
		///       A array of <see cref='Telerik.WinControls.RadElement'/> objects with which to intialize the collection
		/// </param>
		public RadElementCollection(RadElement owner, RadElement[] value)
			: base(1)
		{
			this.owner = owner;
			this.AddRange(value);
		}

		/// <summary>
		/// <para>Represents the entry at the specified index of the <see cref='Telerik.WinControls.RadElement'/>.</para>
		/// </summary>
		/// <param name='index'><para>The zero-based index of the entry to locate in the collection.</para></param>
		/// <value>
		///    <para> The entry at the specified index of the collection.</para>
		/// </value>
		/// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
		public RadElement this[int index]
		{
			get
			{
				return ((RadElement) (List[index]));
			}
			set
			{
				List[index] = value;
			}
		}

		public RadElement Owner
		{
			get { return owner; }
		}

		/// <summary>
		///    <para>Adds a <see cref='Telerik.WinControls.RadElement'/> with the specified value to the 
		///    <see cref='Telerik.WinControls.RadElementCollection'/> .</para>
		/// </summary>
		/// <param name='value'>The <see cref='Telerik.WinControls.RadElement'/> to add.</param>
		/// <returns>
		///    <para>The index at which the new element was inserted.</para>
		/// </returns>
		public int Add(RadElement value)
		{
			return List.Add(value);
		}

		/// <summary>
		/// <para>Copies the elements of an array to the end of the <see cref='Telerik.WinControls.RadElementCollection'/>.</para>
		/// </summary>
		/// <param name='value'>
		///    An array of type <see cref='Telerik.WinControls.RadElement'/> containing the objects to add to the collection.
		/// </param>
		/// <returns>
		///   <para>None.</para>
		/// </returns>
		public void AddRange(params RadElement[] value)
		{
			this.Capacity = this.Count + value.Length;
			for (int i = 0; i < value.Length; i++ )
			{
				this.Add(value[i]);
			}
		}

		/// <summary>
		///     <para>
		///       Adds the contents of another <see cref='Telerik.WinControls.RadElementCollection'/> to the end of the collection.
		///    </para>
		/// </summary>
		/// <param name='value'>
		///    A <see cref='Telerik.WinControls.RadElementCollection'/> containing the objects to add to the collection.
		/// </param>
		/// <returns>
		///   <para>None.</para>
		/// </returns>
		public void AddRange(RadElementCollection value)
		{
			this.Capacity = this.Count + value.Count;
			for (int i = 0; (i < value.Count); i = (i + 1))
			{
				this.Add(value[i]);
			}
		}

		/// <summary>
		/// <para>Gets a value indicating whether the 
		///    <see cref='Telerik.WinControls.RadElementCollection'/> contains the specified <see cref='Telerik.WinControls.RadElement'/>.</para>
		/// </summary>
		/// <param name='value'>The <see cref='Telerik.WinControls.RadElement'/> to locate.</param>
		/// <returns>
		/// <para><see langword='true'/> if the <see cref='Telerik.WinControls.RadElement'/> is contained in the collection; 
		///   otherwise, <see langword='false'/>.</para>
		/// </returns>
		public bool Contains(RadElement value)
		{
			return List.Contains(value);
		}

		/// <summary>
		/// <para>Copies the <see cref='Telerik.WinControls.RadElementCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
		///    specified index.</para>
		/// </summary>
		/// <param name='array'><para>The one-dimensional <see cref='System.Array'/> that is the destination of the values copied from <see cref='Telerik.WinControls.RadElementCollection'/> .</para></param>
		/// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
		/// <returns>
		///   <para>None.</para>
		/// </returns>
        /// <exception cref='System.ArgumentException'><para><paramref name='array'/> is multidimensional.</para> <para>-or-</para> <para>The number of elements in the <see cref='Telerik.WinControls.RadElementCollection'/> is greater than the available space between <paramref name='index'/> and the end of <paramref name='array'/>.</para></exception>
		/// <exception cref='System.ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is less than <paramref name='array'/>'s lowbound. </exception>
		public void CopyTo(RadElement[] array, int index)
		{
			List.CopyTo(array, index);
		}

		/// <summary>
		///    <para>Returns the index of a <see cref='Telerik.WinControls.RadElement'/> in 
		///       the <see cref='Telerik.WinControls.RadElementCollection'/> .</para>
		/// </summary>
		/// <param name='value'>The <see cref='Telerik.WinControls.RadElement'/> to locate.</param>
		/// <returns>
		/// <para>The index of the <see cref='Telerik.WinControls.RadElement'/> of <paramref name='value'/> in the 
		/// <see cref='Telerik.WinControls.RadElementCollection'/>, if found; otherwise, -1.</para>
		/// </returns>
		public int IndexOf(RadElement value)
		{
			return List.IndexOf(value);
		}

		/// <summary>
		/// <para>Inserts a <see cref='Telerik.WinControls.RadElement'/> into the <see cref='Telerik.WinControls.RadElementCollection'/> at the specified index.</para>
		/// </summary>
		/// <param name='index'>The zero-based index where <paramref name='value'/> should be inserted.</param>
		/// <param name=' value'>The <see cref='Telerik.WinControls.RadElement'/> to insert.</param>
		/// <returns><para>None.</para></returns>
		public void Insert(int index, RadElement value)
		{
			List.Insert(index, value);
		}

		/// <summary>
		///    <para>Returns an enumerator that can iterate through 
		///       the <see cref='Telerik.WinControls.RadElementCollection'/> .</para>
		/// </summary>
		/// <returns><para>None.</para></returns>
		public new RadElementEnumerator GetEnumerator()
		{
			return new RadElementEnumerator(this);
		}

		/// <summary>
		///    <para> Removes a specific <see cref='Telerik.WinControls.RadElement'/> from the 
		///    <see cref='Telerik.WinControls.RadElementCollection'/> .</para>
		/// </summary>
		/// <param name='value'>The <see cref='Telerik.WinControls.RadElement'/> to remove from the <see cref='Telerik.WinControls.RadElementCollection'/> .</param>
		/// <returns><para>None.</para></returns>
		/// <exception cref='System.ArgumentException'><paramref name='value'/> is not found in the Collection. </exception>
		public void Remove(RadElement value)
		{
			List.Remove(value);
		}

		protected override void OnValidate(object value)
		{
			if (!(value is RadElement))
			{
				throw new InvalidOperationException("Collection contains only instances of Type RadElement");
			}
		}

		/// <summary>
		/// <para>Sorts the elements in the entire <see cref='Telerik.WinControls.RadElementCollection'/> using the IComparable implementation of each element.</para>
		/// </summary>
		public void Sort()
		{
            if (!suspended)
                this.owner.ChangeCollection(null, ItemsChangeOperation.Sorting);
			this.InnerList.Sort();
            if (!suspended)
				this.owner.ChangeCollection(null, ItemsChangeOperation.Sorted);
		}

		/// <summary>
		/// <para>Sorts the elements in the entire <see cref='Telerik.WinControls.RadElementCollection'/> using the specified comparer.</para>
		/// </summary>
		/// <param name="comparer">The IComparer implementation to use when comparing elements.</param>
		public void Sort(IComparer comparer)
		{
            if (!suspended)
                this.owner.ChangeCollection(null, ItemsChangeOperation.Sorting);
			this.InnerList.Sort(comparer);
            if (!suspended)
				this.owner.ChangeCollection(null, ItemsChangeOperation.Sorted);
		}

		/// <summary>
		/// <para>Sorts the elements in a range of elements in <see cref='Telerik.WinControls.RadElementCollection'/> using the specified comparer.</para>
		/// </summary>
		/// <param name="index">The zero-based starting index of the range to sort. </param>
		/// <param name="count">The length of the range to sort. </param>
		/// <param name="comparer">The IComparer implementation to use when comparing elements.</param>
		public void Sort(int index, int count, IComparer comparer)
		{
            if (!suspended)
                this.owner.ChangeCollection(null, ItemsChangeOperation.Sorting);
			this.InnerList.Sort(index, count, comparer);
            if (!suspended)
				this.owner.ChangeCollection(null, ItemsChangeOperation.Sorted);
		}

		/// <summary>
		/// <para>Moves the element at position a given position to a new position</para>
		/// </summary>
		/// <param name="indexFrom">The zero-based index of the element to move</param>
		/// <param name="indexTo">The zero-based index of the position where the element is to be placed</param>
		public void Move(int indexFrom, int indexTo)
		{
			if (indexFrom >= 0 && indexFrom < this.List.Count && indexTo >= 0 && indexTo <= this.List.Count)
			{
                checkForAlreadyAddedItems = false;
				suspended = true;
				object element = this.List[indexFrom];
				this.List.RemoveAt(indexFrom);
				this.List.Insert(indexTo, element);
				suspended = false;
                checkForAlreadyAddedItems = true;
			}
		}

		bool checkValid = true;

		public void SwitchItems(int indexFrom, int indexTo)
		{
			if (indexFrom >= 0 && indexFrom < this.List.Count && indexTo >= 0 && indexTo <= this.List.Count)
			{
				checkValid = false;

				RadElement item = (RadElement)this.List[indexTo];
				this.List[indexTo] = this.List[indexFrom];
				this.List[indexFrom] = item;

				checkValid = true;
			}
		}


		private void CheckElementAlreadyAdded(RadElement element)
		{
            if (element.Parent == this.owner)
            {
                throw new InvalidOperationException("Element already added");
            }

            //Check whether the element is already parented and if yes remove it from its old parent
            //TODO: Check this change thoroughly, as it is a major one
            RadElement currParent = element.Parent;
            if (currParent != null && currParent != this.owner)
            {
                currParent.Children.Remove(element);
            }
		}

        internal bool checkForAlreadyAddedItems = true;

		protected override void OnInsert(int index, object value)
		{
            RadElement element = value as RadElement;
            if (checkForAlreadyAddedItems)
            {
                CheckElementAlreadyAdded(element);
            }

            if (!suspended)
            {
                this.owner.ChangeCollection(element, ItemsChangeOperation.Inserting);
            }

			base.OnInsert(index, value);
		}

        protected override void OnInsertComplete(int index, object value)
        {
            if (!suspended)
                this.owner.ChangeCollection(value as RadElement, ItemsChangeOperation.Inserted);

            base.OnInsertComplete(index, value);
        }

		protected override void OnSet(int index, object oldValue, object newValue)
		{
            RadElement element = newValue as RadElement;
            if (checkValid)
            {
                CheckElementAlreadyAdded(element);
            }

            if (!suspended)
            {
                //Georgi: Changing collection was for the old value, changed to new value
                this.owner.ChangeCollection(element, ItemsChangeOperation.Setting);
            }

			base.OnSet(index, oldValue, newValue);
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			base.OnSetComplete(index, oldValue, newValue);

			if (!suspended)
				this.owner.ChangeCollection(newValue as RadElement, ItemsChangeOperation.Set);
		}

		protected override void OnRemove(int index, object value)
        {
            if (!suspended)
                this.owner.ChangeCollection(value as RadElement, ItemsChangeOperation.Removing);

            base.OnRemove(index, value);
        }

		protected override void OnRemoveComplete(int index, object value)
		{
			if (!suspended)
				this.owner.ChangeCollection(value as RadElement, ItemsChangeOperation.Removed);
		}


        protected override void OnClear()
        {
            if (!suspended)
				this.owner.ChangeCollection(null, ItemsChangeOperation.Clearing);
            base.OnClear();            
        }

        protected override void OnClearComplete()
        {
            base.OnClearComplete();
            if (!suspended)
				this.owner.ChangeCollection(null, ItemsChangeOperation.Cleared);
        }

		public class RadElementEnumerator : IEnumerator, IEnumerator<RadElement>
		{

			private IEnumerator baseEnumerator;

			private IEnumerable temp;

			public RadElementEnumerator(RadElementCollection mappings)
			{
				this.temp = ((IEnumerable)(mappings));
				this.baseEnumerator = temp.GetEnumerator();
			}

			public RadElement Current
			{
				get
				{
					return ((RadElement) (baseEnumerator.Current));
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return baseEnumerator.Current;
				}
			}

			public bool MoveNext()
			{
				return baseEnumerator.MoveNext();
			}

			bool IEnumerator.MoveNext()
			{
				return baseEnumerator.MoveNext();
			}

			public void Reset()
			{
				baseEnumerator.Reset();
			}

			void IEnumerator.Reset()
			{
				baseEnumerator.Reset();
			}

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                //
            }

            #endregion
        }

        #region IEnumerable<RadElement> Members

        IEnumerator<RadElement> IEnumerable<RadElement>.GetEnumerator()
        {
            return new RadElementEnumerator(this);
        }

        #endregion
    }
}