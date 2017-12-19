using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI
{
	public delegate void RadListBoxItemIndexCollectionChangedEventHandler(object sender, RadListBoxItemIndexCollectionChangedEventArgs e);

	public class RadListBoxItemIndexCollection : CollectionBase, IEnumerable<int>
	{
		public event RadListBoxItemIndexCollectionChangedEventHandler CollectionChanged;

		public RadListBoxItemIndexCollection()
		{
		}

		public RadListBoxItemIndexCollection(int[] value)
		{
			this.AddRange(value);
		}

		public int this[int index]
		{
			get
			{
				return ((int)(List[index]));
			}
			set
			{
				List[index] = value;
			}
		}

		public int Add(int value)
		{
			return List.Add(value);
		}

		public void AddRange(int[] value)
		{
			for (int i = 0; (i < value.Length); i = (i + 1))
			{
				this.Add(value[i]);
			}
		}

		public bool Contains(int value)
		{
			return List.Contains(value);
		}

		public void CopyTo(int[] array, int index)
		{
			List.CopyTo(array, index);
		}

		public int IndexOf(int value)
		{
			return List.IndexOf(value);
		}

		public void Insert(int index, int value)
		{
			List.Insert(index, value);
		}

		public new RadListBoxItemIndexEnumerator GetEnumerator()
		{
			return new RadListBoxItemIndexEnumerator(this);
		}

		public void Remove(int value)
		{
			List.Remove(value);
		}

		protected override void OnValidate(object value)
		{
			if (!(value is int))
			{
				throw new InvalidOperationException("Collection contains only instances of type int");
			}
		}

		public void Sort()
		{
			this.InnerList.Sort();
		}

		public void Sort(IComparer comparer)
		{
			this.InnerList.Sort(comparer);
		}

		public void Sort(int index, int count, IComparer comparer)
		{
			this.InnerList.Sort(index, count, comparer);
		}

		private void CheckElementAlreadyAdded(object value)
		{
			foreach (int alreadyAdded in this)
			{
				if ((int)value == alreadyAdded)
					throw new InvalidOperationException("Element already added");
			}
		}

		protected override void OnInsert(int index, object value)
		{
			CheckElementAlreadyAdded(value);
			base.OnInsert(index, value);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			//((int)value).Selected = true;
			base.OnInsertComplete(index, value);
			RadListBoxItemIndexCollectionChangedEventArgs args = new RadListBoxItemIndexCollectionChangedEventArgs(-1, (int)value, ItemsChangeOperation.Inserted);
			this.OnCollectionChanged(args);
		}

		protected override void OnSet(int index, object oldValue, object newValue)
		{
			CheckElementAlreadyAdded(newValue);
			base.OnSet(index, oldValue, newValue);
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			//((int)oldValue).Selected = false;
			//((int)newValue).Selected = true;
			base.OnSetComplete(index, oldValue, newValue);
			RadListBoxItemIndexCollectionChangedEventArgs args = new RadListBoxItemIndexCollectionChangedEventArgs((int)oldValue, (int)newValue, ItemsChangeOperation.Set);
			this.OnCollectionChanged(args);
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			//((int)value).Selected = false;
			base.OnRemoveComplete(index, value);
			RadListBoxItemIndexCollectionChangedEventArgs args = new RadListBoxItemIndexCollectionChangedEventArgs((int)value, -1, ItemsChangeOperation.Removed);
			this.OnCollectionChanged(args);
		}

		protected override void OnClear()
		{
			base.OnClear();
			RadListBoxItemIndexCollectionChangedEventArgs args = new RadListBoxItemIndexCollectionChangedEventArgs(-1, -1, ItemsChangeOperation.Clearing);
			this.OnCollectionChanged(args);
		}

		protected override void OnClearComplete()
		{
			base.OnClearComplete();
			RadListBoxItemIndexCollectionChangedEventArgs args = new RadListBoxItemIndexCollectionChangedEventArgs(-1, -1, ItemsChangeOperation.Cleared);
			this.OnCollectionChanged(args);
		}

		protected virtual void OnCollectionChanged(RadListBoxItemIndexCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null)
				CollectionChanged(this, e);
		}

		public class RadListBoxItemIndexEnumerator : IEnumerator, IEnumerator<int>
		{
			private IEnumerator baseEnumerator;

			private IEnumerable temp;

			public RadListBoxItemIndexEnumerator(RadListBoxItemIndexCollection mappings)
			{
				this.temp = ((IEnumerable)(mappings));
				this.baseEnumerator = temp.GetEnumerator();
			}

			public int Current
			{
				get
				{
					return (int)baseEnumerator.Current;
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

		IEnumerator<int> IEnumerable<int>.GetEnumerator()
		{
			return new RadListBoxItemIndexEnumerator(this);
		}

		#endregion
	}


	public class RadListBoxItemIndexCollectionChangedEventArgs : EventArgs
	{
		private ItemsChangeOperation operation;
		private int oldValue;
		private int newValue;

		public int OldValue
		{
			get { return this.oldValue; }
		}

		public int NewValue
		{
			get { return this.newValue; }
		}

		public ItemsChangeOperation Operation
		{
			get { return this.operation; }
		}

		public RadListBoxItemIndexCollectionChangedEventArgs(int oldValue, int newValue, ItemsChangeOperation operation)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
			this.operation = operation;
		}
	}
}

