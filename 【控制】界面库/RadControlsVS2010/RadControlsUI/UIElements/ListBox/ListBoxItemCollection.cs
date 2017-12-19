using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI
{
	public delegate void RadListBoxItemCollectionChangedEventHandler(object sender, RadListBoxItemCollectionChangedEventArgs e);

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	public class RadListBoxItemCollection : CollectionBase, IEnumerable<RadItem>
	{	
		public event RadListBoxItemCollectionChangedEventHandler CollectionChanged;
        private bool beginUpdate = false;

        public void BeginUpdate()
        {
            this.beginUpdate = true;
        }

        public void EndUpdate()
        {
            this.beginUpdate = false;
        }

		public RadListBoxItemCollection()
		{			
		}

        public RadListBoxItemCollection(RadItem[] value)
		{			
			this.AddRange(value);
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

        public int Add(RadItem value)
		{
			return List.Add(value);
		}

        public void AddRange(RadItem[] value)
		{
			for (int i = 0; i < value.Length; i = i + 1)
			{
				this.Add(value[i]);
			}
		}

        public bool Contains(RadItem value)
		{
			return List.Contains(value);
		}

        public void CopyTo(RadItem[] array, int index)
		{
			List.CopyTo(array, index);
		}

        public int IndexOf(RadItem value)
		{
			return List.IndexOf(value);
		}

        public void Insert(int index, RadItem value)
		{
			List.Insert(index, value);
		}

		public new RadListBoxItemEnumerator GetEnumerator()
		{
			return new RadListBoxItemEnumerator(this);
		}

        public void Remove(RadItem value)
		{
			List.Remove(value);
		}

		protected override void OnValidate(object value)
		{
			if (!(value is RadItem))
			{
                throw new InvalidOperationException("Collection contains only instances of Type RadItem");
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
            if (beginUpdate)
            {
                return;
            }

			foreach (RadElement alreadyAdded in this)
			{
				if (value == alreadyAdded)
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
            RadItem item = (RadItem)value;
            item.SetValue(RadListBoxItem.SelectedProperty, true);
			base.OnInsertComplete(index, value);
            RadListBoxItemCollectionChangedEventArgs args = new RadListBoxItemCollectionChangedEventArgs(null, item, ItemsChangeOperation.Inserted);
			this.OnCollectionChanged(args);
		}

		protected override void OnSet(int index, object oldValue, object newValue)
		{
			CheckElementAlreadyAdded(newValue);
			base.OnSet(index, oldValue, newValue);
		}

		protected override void OnSetComplete(int index, object oldValue, object newValue)
		{
			//((RadListBoxItem)oldValue).Selected = false;
			//((RadListBoxItem)newValue).Selected = true;
            ((RadItem)oldValue).SetValue(RadListBoxItem.SelectedProperty, false);
            ((RadItem)newValue).SetValue(RadListBoxItem.SelectedProperty, true);
			base.OnSetComplete(index, oldValue, newValue);
            RadListBoxItemCollectionChangedEventArgs args = new RadListBoxItemCollectionChangedEventArgs((RadItem)oldValue, (RadItem)newValue, ItemsChangeOperation.Set);
			this.OnCollectionChanged(args);
		}

		protected override void OnRemoveComplete(int index, object value)
		{
			//((RadListBoxItem)value).Selected = false;
            ((RadItem)value).SetValue(RadListBoxItem.SelectedProperty, false);
			base.OnRemoveComplete(index, value);
            RadListBoxItemCollectionChangedEventArgs args = new RadListBoxItemCollectionChangedEventArgs((RadItem)value, null, ItemsChangeOperation.Removed);
			this.OnCollectionChanged(args);
		}

		protected override void OnClear()
		{
            foreach (RadItem item in this.List)
			{
				//item.Selected = false;
                item.SetValue(RadListBoxItem.SelectedProperty, false);
			}
			base.OnClear();
			RadListBoxItemCollectionChangedEventArgs args = new RadListBoxItemCollectionChangedEventArgs(null, null, ItemsChangeOperation.Clearing);
			this.OnCollectionChanged(args);
		}

		protected override void OnClearComplete()
		{
			base.OnClearComplete();
			RadListBoxItemCollectionChangedEventArgs args = new RadListBoxItemCollectionChangedEventArgs(null, null, ItemsChangeOperation.Cleared);
			this.OnCollectionChanged(args);
		}

		protected virtual void OnCollectionChanged(RadListBoxItemCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null)
				CollectionChanged(this, e);
		}
		
		public class RadListBoxItemEnumerator : IEnumerator, IEnumerator<RadItem>
		{

			private IEnumerator baseEnumerator;

			private IEnumerable temp;

			public RadListBoxItemEnumerator(RadListBoxItemCollection mappings)
			{
				this.temp = ((IEnumerable)(mappings));
				this.baseEnumerator = temp.GetEnumerator();
			}

            public RadItem Current
			{
				get
				{
                    return ((RadItem)(baseEnumerator.Current));
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

        IEnumerator<RadItem> IEnumerable<RadItem>.GetEnumerator()
        {
			return new RadListBoxItemEnumerator(this);
        }

        #endregion
    }


	public class RadListBoxItemCollectionChangedEventArgs : EventArgs
	{
		private ItemsChangeOperation operation;
		private RadItem oldValue;
		private RadItem newValue;

        public RadItem OldValue
		{
			get { return this.oldValue; }
		}

        public RadItem NewValue
		{
			get { return this.newValue; }
		}

		public ItemsChangeOperation Operation
		{
			get { return this.operation; }
		}

        public RadListBoxItemCollectionChangedEventArgs(RadItem oldValue, RadItem newValue, ItemsChangeOperation operation)
		{
			this.oldValue = oldValue;
			this.newValue = newValue;
			this.operation = operation;			
		}
	}
}
