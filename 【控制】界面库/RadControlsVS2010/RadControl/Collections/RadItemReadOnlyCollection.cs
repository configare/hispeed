using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;

namespace Telerik.WinControls.Elements
{
	[ListBindable(false)]
	//[Editor(typeof(RadItemReadOnlyCollectionEditor), typeof(UITypeEditor))]
	public class RadItemReadOnlyCollection : ReadOnlyCollectionBase, IEnumerable<RadItem>
	{
        public event ItemChangedDelegate ItemsChanged;

		#region Constructors
		/// <summary>
		///     <para>
		///       Initializes a new instance of <see cref='Telerik.WinControls.RadItemCollection'/> based on another <see cref='Telerik.WinControls.RadItemCollection'/>.
		///    </para>
		/// </summary>
		/// <param name='value'>
		///       A <see cref='Telerik.WinControls.RadItemCollection'/> from which the contents are copied
		/// </param>
		public RadItemReadOnlyCollection(RadItemOwnerCollection value)
		{
			this.AddRange(value);
		}

		/// <summary>
		///     <para>
		///       Initializes a new instance of <see cref='Telerik.WinControls.RadItemCollection'/> containing any array of <see cref='Telerik.WinControls.RadElement'/> objects.
		///    </para>
		/// </summary>
		/// <param name='value'>
		///       A array of <see cref='Telerik.WinControls.RadElement'/> objects with which to intialize the collection
		/// </param>
		public RadItemReadOnlyCollection(RadItem[] value)
		{
			this.AddRange(value);
		} 
		#endregion

		protected virtual void OnItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
		{
			ItemChangedDelegate handler = this.ItemsChanged;
			if (handler != null)
			{
				handler(changed, target, operation);
			}
		}

		/// <summary>
		/// <para>Represents the entry at the specified index of the <see cref='Telerik.WinControls.RadElement'/>.</para>
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
				return (RadItem)this.InnerList[index];
			}
		}

		/// <summary>
		/// <para>Gets a value indicating whether the 
		///    <see cref='Telerik.WinControls.RadItemCollection'/> contains the specified <see cref='Telerik.WinControls.RadElement'/>.</para>
		/// </summary>
		/// <param name='value'>The <see cref='Telerik.WinControls.RadElement'/> to locate.</param>
		/// <returns>
		/// <para><see langword='true'/> if the <see cref='Telerik.WinControls.RadElement'/> is contained in the collection; 
		///   otherwise, <see langword='false'/>.</para>
		/// </returns>
		public bool Contains(RadItem value)
		{
			return InnerList.Contains(value);
		}

		/// <summary>
		/// <para>Copies the <see cref='Telerik.WinControls.RadItemCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
		///    specified index.</para>
		/// </summary>
		/// <param name='array'><para>The one-dimensional <see cref='System.Array'/> that is the destination of the values copied from <see cref='Telerik.WinControls.RadItemCollection'/> .</para></param>
		/// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
		/// <returns>
		///   <para>None.</para>
		/// </returns>
        /// <exception cref='System.ArgumentException'><para><paramref name='array'/> is multidimensional.</para> <para>-or-</para> <para>The number of elements in the <see cref='Telerik.WinControls.RadItemCollection'/> is greater than the available space between <paramref name='index'/> and the end of <paramref name='array'/>.</para></exception>
		/// <exception cref='System.ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is less than <paramref name='array'/>'s lowbound. </exception>
		public void CopyTo(RadItem[] array, int index)
		{
			InnerList.CopyTo(array, index);
		}

		/// <summary>
		///    <para>Returns the index of a <see cref='Telerik.WinControls.RadElement'/> in 
		///       the <see cref='Telerik.WinControls.RadItemCollection'/> .</para>
		/// </summary>
		/// <param name='value'>The <see cref='Telerik.WinControls.RadElement'/> to locate.</param>
		/// <returns>
		/// <para>The index of the <see cref='Telerik.WinControls.RadElement'/> of <paramref name='value'/> in the 
		/// <see cref='Telerik.WinControls.RadItemCollection'/>, if found; otherwise, -1.</para>
		/// </returns>
		public int IndexOf(RadItem value)
		{
			return InnerList.IndexOf(value);
		}

		public new RadItemReadOnlyEnumerator GetEnumerator()
		{
			return new RadItemReadOnlyEnumerator(this);
		}

		internal int Add(RadItem value)
		{
			return InnerList.Add(value);
		}

		internal void AddRange(RadItem[] value)
		{
			for (int i = 0; (i < value.Length); i = (i + 1))
			{
				this.Add(value[i]);
			}
		}

		internal void AddRange(RadItemOwnerCollection value)
		{
			for (int i = 0; (i < value.Count); i = (i + 1))
			{
				this.Add(value[i]);
			}
		}

		/// <summary>
		/// Represents an element enumerator. 
		/// </summary>
		public class RadItemReadOnlyEnumerator : IEnumerator, IEnumerator<RadItem>
		{

			private IEnumerator baseEnumerator;

			private IEnumerable temp;

			/// <summary>
			/// Initializes a new instance of the RadElementEnumerator class.
			/// </summary>
			/// <param name="mappings"></param>
			public RadItemReadOnlyEnumerator(RadItemReadOnlyCollection mappings)
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

        #region IEnumerable<RadItem> Members

        IEnumerator<RadItem> IEnumerable<RadItem>.GetEnumerator()
        {
			return new RadItemReadOnlyEnumerator(this);
        }

        #endregion

        /// <summary>Retrieves an array of the items in the collection.</summary>
		public RadItem[] ToArray()
		{
			RadItem[] res = new RadItem[this.Count];
			this.CopyTo(res, 0);
			return res;
		}

		/// <summary>
		/// <para>Sorts the elements in the entire <see cref='Telerik.WinControls.RadElementCollection'/> using the IComparable implementation of each element.</para>
		/// </summary>
		public void Sort()
		{
			this.InnerList.Sort();
		}

		/// <summary>
		/// <para>Sorts the elements in the entire <see cref='Telerik.WinControls.RadElementCollection'/> using the specified comparer.</para>
		/// </summary>
		/// <param name="comparer">The IComparer implementation to use when comparing elements.</param>
		public void Sort(IComparer comparer)
		{
			this.InnerList.Sort(comparer);
		}

		/// <summary>
		/// <para>Sorts the elements in a range of elements in <see cref='Telerik.WinControls.RadElementCollection'/> using the specified comparer.</para>
		/// </summary>
		/// <param name="index">The zero-based starting index of the range to sort. </param>
		/// <param name="count">The length of the range to sort. </param>
		/// <param name="comparer">The IComparer implementation to use when comparing elements.</param>
		public void Sort(int index, int count, IComparer comparer)
		{
			this.InnerList.Sort(index, count, comparer);
		}	
	}
}
