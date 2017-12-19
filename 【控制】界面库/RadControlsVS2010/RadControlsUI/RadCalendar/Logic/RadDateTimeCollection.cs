using System;
using System.Collections;
using System.Collections.Generic;

namespace Telerik.WinControls.UI
{
	public class DateTimeCollection : IEnumerable<DateTime>, IEnumerable, IList, ICollection, ICloneable
	{
        #region Fields
        private RadCalendar calendar = null;
        private List<DateTime> children = new List<DateTime>();
        private bool updating; 
        #endregion

        #region Constructors
        public DateTimeCollection(RadCalendar calendar)
        {
            this.calendar = calendar;
        }

        public DateTimeCollection()
        {
        } 
        #endregion

        #region Properties
        /// <summary>
        /// Gets the total number of DateTime objects in the collection.
        /// </summary>
        public virtual int Count
        {
            get
            {
                return this.children.Count;
            }
        }

        /// <summary>
        /// Gets or sets the DateTime at the specified indexed location in the collection.
        /// </summary>
        /// <param name="index">The indexed location of the DateTime in the collection.</param>
        /// <returns>The DateTime at the specified indexed location in the collection.</returns>
        public virtual DateTime this[int index]
        {
            get
            {
                return this.children[index];
            }
            set
            {
                if (this.calendar != null && !this.updating)
                {
                    SelectionEventArgs args = this.calendar.CallOnSelectionChanging(this);
                    if (args.Cancel)
                    {
                        return;
                    }
                }
                if (!this.calendar.AllowMultipleSelect)
                {
                    this.children.Clear();
                    this.children[0] = value;
                }
                else
                {
                    this.children[index] = value;
                }
                if (this.calendar != null && !this.updating)
                {
                    this.calendar.CallOnSelectionChanged();
                }
            }
        }
        #endregion

        #region Methods
        public void BeginUpdate()
        {
            this.updating = true;
        }

        public void EndUpdate()
        {
            this.updating = false;
        }

        /// <summary>
        /// Returns the index of the specified DateTime object in the collection.
        /// </summary>
        /// <param name="date">The DateTime object to locate in the collection.</param>
        /// <returns>The zero-based index of the item found in the DateTimeCollection; otherwise, -1.</returns>
        public virtual int IndexOf(DateTime date)
        {
            return this.children.IndexOf(date);
        }

        /// <summary>
        /// Adds a previously created DateTime object to the end of the DateTimeCollection.
        /// </summary>
        /// <param name="date">The DateTime object to add to the collection.</param>
        /// <returns>The zero-based index value of the DateTime object added to the DateTimeCollection.</returns>
        public virtual DateTime Add(DateTime date)
        {
            if (!this.CanAdd(date))
            {
                return DateTime.MinValue;
            }

            if (this.calendar != null && !this.updating)
            {
                SelectionEventArgs args = this.calendar.CallOnSelectionChanging(this);
                if (args.Cancel)
                {
                    return DateTime.MinValue;
                }
            }

            if (!this.calendar.AllowMultipleSelect)
                this.children.Clear();

            this.children.Add(date);
            if (this.calendar != null && !this.updating)
            {
                this.calendar.CallOnSelectionChanged();
            }

            return date;
        }

        internal List<DateTime> Dates
        {
            get
            {
                return this.children;
            }
            set
            {
                this.children = value;
            }
        }

        /// <summary>
        /// Returns an enumerator that can be used to iterate through the DateTime collection.
        /// </summary>
        /// <returns>An IEnumerator that represents the DateTime collection.</returns>
        public virtual IEnumerator<DateTime> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        /// <summary>
        /// Inserts an existing DateTime object into the DateTimeCollection at the specified location.
        /// </summary>
        /// <param name="index">The indexed location within the collection to insert the DateTime object. </param>
        /// <param name="date">The DateTime object to insert into the collection.</param>
        public virtual void Insert(int index, DateTime date)
        {
            if (!this.CanAdd(date))
            {
                return;
            }

            if (this.calendar != null && !this.updating)
            {
                SelectionEventArgs args = this.calendar.CallOnSelectionChanging(this);
                if (args.Cancel)
                {
                    return;
                }
            }

            if (this.calendar != null && !this.calendar.AllowMultipleSelect)
                this.children.Clear();

            this.children.Insert(index, date);
            if (this.calendar != null && !this.updating)
            {
                this.calendar.CallOnSelectionChanged();
            }
        }

        /// <summary>
        /// CanAdd method verify whether the date can be add to the collection.
        /// </summary>
        /// <param name="date">The DateTime object to insert into the collection.</param>
        public virtual bool CanAdd(DateTime date)
        {
            if (date == null || this.children.Contains(date))
            {
                return false;
            }

            if (this.calendar == null)
                return true;

            if ( date < this.calendar.RangeMinDate || date > this.calendar.RangeMaxDate)
            {
                return false;
            }

            if (!this.calendar.AllowMultipleSelect && this.children.Count > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes the specified DateTime object from the DateTimeCollection.
        /// </summary>
        /// <param name="date">The DateTime object to remove.</param>
        public virtual void Remove(DateTime date)
        {
            if (this.children.Contains(date))
            {
                if (this.calendar != null && !this.updating)
                {
                    SelectionEventArgs args = this.calendar.CallOnSelectionChanging(this);
                    if (args.Cancel)
                    {
                        return;
                    }
                }

                this.children.Remove(date);
                if (this.calendar != null && !this.updating)
                {
                    this.calendar.CallOnSelectionChanged();
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that can be used to iterate through the DateTime collection.
        /// </summary>
        /// <returns>An IEnumerator that represents the DateTime collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Removes all DateTime objects from the collection.
        /// </summary>
        public void Clear()
        {
            if (this.calendar != null && !this.updating)
            {
                SelectionEventArgs args = this.calendar.CallOnSelectionChanging(this);
                if (args.Cancel)
                {
                    return;
                }
            }

            this.children.Clear();
            if (this.calendar != null && this.calendar.AllowMultipleSelect && !this.updating)
            {
                this.calendar.CallOnSelectionChanged();
            }

            this.UpdateOwnerVisuals();
        }

        protected void UpdateOwnerVisuals()
        {
            if (this.calendar != null)
            {
                this.calendar.CalendarElement.RefreshVisuals(true);
                this.calendar.CalendarElement.Invalidate();
            }
        }

        /// <summary>
        /// Removes a range of DateTime elements from the DateTimeCollection. 
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        public virtual void RemoveRange(int index, int count)
        {
            if (count == 0)
            {
                return;
            }
            for (int i = index; i < count; i++)
            {
                if (this.calendar != null && !this.updating)
                {
                    SelectionEventArgs args = this.calendar.CallOnSelectionChanging(this);
                    if (args.Cancel)
                    {
                        return;
                    }
                }

                children.RemoveAt(i);
                if (this.calendar != null && !this.updating)
                {
                    this.calendar.CallOnSelectionChanged();
                }
            }
        }

        public virtual void RemoveRange(DateTime[] dates)
        {
            if (this.calendar == null)
            {
                return;
            }

            if (!this.updating)
            {
                SelectionEventArgs args = this.calendar.CallOnSelectionChanging(this);
                if (args.Cancel)
                {
                    return;
                }
            }

            foreach (DateTime date in dates)
            {
                if (this.children.Contains(date))
                {
                    this.children.Remove(date);
                }
            }

            if (!this.updating)
            {
                this.calendar.CallOnSelectionChanged();
            }
        }


        /// <summary>
        /// Adds an array of previously created DateTime objects to the collection.
        /// </summary>
        /// <param name="inputItems">An array of DateTime objects representing the dates to add to the collection.</param>
        public virtual void AddRange(DateTime[] inputItems)
        {
            if (inputItems == null)
            {
                throw new ArgumentNullException();
            }

            if (inputItems.Length == 0)
            {
                return;
            }

            if (!this.calendar.AllowMultipleSelect)
            {
                this.Add(inputItems[0]);
                return;
            }

            if (!this.updating)
            {
                SelectionEventArgs args = this.calendar.CallOnSelectionChanging(this);
                if (args.Cancel)
                {
                    return;
                }
            }

            for (int i = 0; i < inputItems.Length; i++)
            {
                if (this.CanAdd(inputItems[i]))
                {
                    this.children.Add(inputItems[i]);
                }
            }

            if (!this.updating)
            {
                this.calendar.CallOnSelectionChanged();
            }
        }

        /// <summary>
        /// Determines whether the specified DateTime object is a member of the collection.
        /// </summary>
        /// <param name="value">The DateTime to locate in the collection.</param>
        /// <returns>true if the DateTime is a member of the collection; otherwise, false.</returns>
        public virtual bool Contains(DateTime value)
        {
            return this.children.Contains(value);
        }

        /// <summary>
        /// Copies the elements of the DateTime collection to a new DateTime array. 
        /// </summary>
        /// <returns> A DateTime array</returns>
        public virtual DateTime[] ToArray()
        {
            return this.children.ToArray();
        } 
        #endregion

		#region ICollection Members

        /// <summary>
        /// Copies the elements of the DateTimeCollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from DateTimeCollection. 
        /// The Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			this.children.CopyTo(array as DateTime[], index);
		}

        /// <summary>
        /// Gets a value indicating whether access to the DateTimeCollection is synchronized (thread safe). 
        /// </summary>
		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

        /// <summary>
        /// Gets an object that can be used to synchronize access to the DateTimeCollection. 
        /// </summary>
		object ICollection.SyncRoot
		{
			get
			{
				return null;
			}
		}

		#endregion

		#region IList Members
        
        /// <summary>
        /// Adds a previously created DateTime object to the end of the DateTimeCollection.
        /// </summary>
        /// <param name="value">The DateTime object to add to the collection.</param>
        /// <returns>The zero-based index value of the DateTime object added to the DateTimeCollection.</returns>
		int IList.Add(object value)
		{
			return this.children.IndexOf(this.Add((DateTime)value));
		}

        /// <summary>
        /// Removes all DateTime objects from the collection.
        /// </summary>
		void IList.Clear()
		{
			this.Clear();
		}

        /// <summary>
        /// Determines whether the specified DateTime object is a member of the collection.
        /// </summary>
        /// <param name="value">The DateTime to locate in the collection.</param>
        /// <returns>true if the DateTime is a member of the collection; otherwise, false.</returns>
		bool IList.Contains(object value)
		{
			return this.children.Contains((DateTime)value);
		}

        /// <summary>
        /// Returns the index of the specified DateTime object in the collection.
        /// </summary>
        /// <param name="value">The  DateTime object to locate in the collection.</param>
        /// <returns>The zero-based index of the item found in the DateTimeCollection</returns>
		int IList.IndexOf(object value)
		{
			return this.children.IndexOf((DateTime)value);
		}

        /// <summary>
        /// Inserts an existing DateTime object into the DateTimeCollection at the specified location.
        /// </summary>
        /// <param name="index">The indexed location within the collection to insert the DateTime object. </param>
        /// <param name="value">The DateTime object to insert into the collection.</param>
		void IList.Insert(int index, object value)
		{
			this.Insert(index, (DateTime)value);
		}

        /// <summary>
        /// Gets a value indicating whether the DateTimeCollection has a fixed size. 
        /// </summary>
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

        /// <summary>
        /// Gets a value indicating whether the DateTimeCollection is read-only. 
        /// </summary>
		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

        /// <summary>
        /// Removes the specified DateTime object from the DateTimeCollection.
        /// </summary>
        /// <param name="value">The DateTime object to remove.</param>
		void IList.Remove(object value)
		{
			this.Remove((DateTime)value);
		}

        /// <summary>
        /// Removes the element at the specified index of the DateTimeCollection. 
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
		void IList.RemoveAt(int index)
		{
			this.Remove(this.children[index]);
		}

        /// <summary>
        /// Gets or sets the DateTime at the specified indexed location in the collection.
        /// </summary>
        /// <param name="index">The indexed location of the DateTime in the collection.</param>
        /// <returns>The DateTime at the specified indexed location in the collection.</returns>
		object IList.this[int index]
		{
			get
			{
				return this.children[index];
			}
			set
			{
                this[index] = (DateTime)value;
			}
		}

		#endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a new DateTimeCollection object that is a copy of the current instance. 
        /// </summary>
        /// <returns>A new DateTimeCollection object that is a copy of this instance.</returns>
        public virtual DateTimeCollection Clone()
        {
            DateTimeCollection clonedCollection = new DateTimeCollection(this.calendar);
            for (int i = 0; i < this.Dates.Count; i++)
            {
                clonedCollection.Dates.Add(this.Dates[i]);
            }

            return clonedCollection;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance. 
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}

