using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls.Collections
{
    internal class EventsCollection : IDictionary, ICollection, IEnumerable, ICloneable
    {
		#region Costructors
		public EventsCollection()
		{
			this.entries = null;
		}

		public EventsCollection(EventsCollection events)
		{
			if (events == null)
			{
				throw new ArgumentNullException("template");
			}
			DictionaryEntry[] entryArray1 = events.entries;
			if (entryArray1 == null)
			{
				this.entries = null;
			}
			else
			{
				int num1 = entryArray1.Length;
				this.entries = new DictionaryEntry[num1];
				Array.Copy(entryArray1, 0, this.entries, 0, num1);
			}
		} 
		#endregion

		// Fields
		private DictionaryEntry[] entries;

		#region Methods
		///<summary> Adds a delegate to the list. </summary>
		///<param name="eventKey">The object that owns the event.</param>
		///<param name="handler">The delegate to add to the list.</param>
		public void AddEventHandler(object eventKey, Delegate handler)
		{
			this[eventKey] = Delegate.Combine((Delegate)this[eventKey], handler);
		}

		///<summary> Removes a delegate from the list. </summary>
		///<param name="eventKey">The object that owns the event.</param>
		///<param name="handler">The delegate to remove from the list.</param>
		public void RemoveEventHandler(object eventKey, Delegate handler)
		{
			this[eventKey] = Delegate.Remove((Delegate)this[eventKey], handler);
		}

		///<summary> Raises the specified event. </summary>
		///<param name="eventKey">The object that owns the event.</param>
		///<param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
		public virtual void RaiseEvent(object eventKey, EventArgs e)
		{
			Delegate delegate1 = (Delegate)this[eventKey];
			if (delegate1 != null)
			{
				object[] objArray1 = new object[] { this, e };
				delegate1.DynamicInvoke(objArray1);
			}
		}

		public void Add(object key, object value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (this.Contains(key))
			{
				throw new ArgumentException("An element with the same key already exists in the LightHashtable.", "key");
			}
			if (this.entries == null)
			{
				this.entries = new DictionaryEntry[1];
			}
			else
			{
				DictionaryEntry[] entryArray1 = new DictionaryEntry[this.entries.Length + 1];
				this.entries.CopyTo(entryArray1, 1);
				this.entries = entryArray1;
			}
			this.entries[0] = new DictionaryEntry(key, value);
		}

		public void Clear()
		{
			this.entries = null;
		}

		public object Clone()
		{
			return new EventsCollection(this);
		}

		public bool Contains(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			if (this.entries != null)
			{
				for (int num1 = 0; num1 != this.entries.Length; num1++)
				{
					if (this.entries[num1].Key == key)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void CopyTo(Array array, int index)
		{
			if (this.entries != null)
			{
				this.entries.CopyTo(array, index);
			}
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return new HashtableEnumerator(this.entries);
		}

		internal int IndexOf(object key)
		{
			if (this.entries != null)
			{
				for (int num1 = 0; num1 != this.entries.Length; num1++)
				{
					if (this.entries[num1].Key == key)
					{
						return num1;
					}
				}
			}
			return -1;
		}

		public void Remove(object key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			int num1 = this.IndexOf(key);
			if (num1 != -1)
			{
				int num2 = this.entries.Length;
				if (num2 == 1)
				{
					this.entries = null;
				}
				else
				{
					num2--;
					DictionaryEntry[] entryArray1 = new DictionaryEntry[num2];
					if (num1 == 0)
					{
						Array.Copy(this.entries, 1, entryArray1, 0, num2);
					}
					else if (num1 == num2)
					{
						Array.Copy(this.entries, 0, entryArray1, 0, num2);
					}
					else
					{
						Array.Copy(this.entries, 0, entryArray1, 0, num1);
						Array.Copy(this.entries, (int)(num1 + 1), entryArray1, num1, (int)(num2 - num1));
					}
					this.entries = entryArray1;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new HashtableEnumerator(this.entries);
		} 
		#endregion

		#region Properties
        public int Count
        {
            get
            {
                if (this.entries != null)
                {
                    return this.entries.Length;
                }
                return 0;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object this[object key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                int num1 = this.IndexOf(key);
                if (num1 != -1)
                {
                    return this.entries[num1].Value;
                }
                return null;
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                int num1 = this.IndexOf(key);
                if (num1 == -1)
                {
                    this.Add(key, value);
                }
                else
                {
                    this.entries[num1].Value = value;
                }
            }
        }

        public ICollection Keys
        {
            get
            {
                if (this.entries == null)
                {
                    return new object[0];
                }
                int num1 = this.entries.Length;
                object[] objArray1 = new object[num1];
                for (int num2 = 0; num2 < num1; num2++)
                {
                    objArray1[num2] = this.entries[num2].Key;
                }
                return objArray1;
            }
        }

        public object SyncRoot
        {
            get
            {
                if (this.entries != null)
                {
                    return this.entries.SyncRoot;
                }
                return this;
            }
        }

        public ICollection Values
        {
            get
            {
                if (this.entries == null)
                {
                    return new object[0];
                }
                int num1 = this.entries.Length;
                object[] objArray1 = new object[num1];
                for (int num2 = 0; num2 < num1; num2++)
                {
                    objArray1[num2] = this.entries[num2].Value;
                }
                return objArray1;
            }
        }
		#endregion

		#region Nested Types
		private class HashtableEnumerator : IDictionaryEnumerator, IEnumerator
		{
			#region Methods
			public HashtableEnumerator(DictionaryEntry[] entries)
			{
				if (entries == null)
				{
					throw new ArgumentNullException("entries");
				}
				this.entries = (DictionaryEntry[])entries.Clone();
				this.currentIndex = -1;
			}

			public bool MoveNext()
			{
				int num1;
				this.currentIndex = num1 = this.currentIndex + 1;
				return (num1 < this.entries.Length);
			}

			public void Reset()
			{
				this.currentIndex = -1;
			}
			#endregion

			#region Properties
			public object Current
			{
				get
				{
					return this.Entry;
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					if ((this.currentIndex < 0) || (this.currentIndex >= this.entries.Length))
					{
						throw new InvalidOperationException("An attempt was made to position the enumerator before the first element of the collection or after the last element.");
					}
					return this.entries[this.currentIndex];
				}
			}

			public object Key
			{
				get
				{
					return this.Entry.Key;
				}
			}

			public object Value
			{
				get
				{
					return this.Entry.Value;
				}
			}
			#endregion

			// Fields
			private int currentIndex;
			private DictionaryEntry[] entries;
		} 
		#endregion
    }
}
