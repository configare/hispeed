using System;
using System.Collections;
using System.Runtime.Serialization;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Summary description for CalendarDayCollection.
    /// </summary>
    public class CalendarDayCollection : IEnumerable<RadCalendarDay>, IEnumerable, IList, ICollection, ICloneable
	{ 
        // Fields
        private RadCalendar calendar = null;
        private List<RadCalendarDay> children = new List<RadCalendarDay>();

        #region Constructors
        public CalendarDayCollection()
        {
        }

        public CalendarDayCollection(RadCalendar owner, CalendarDayCollection days)
            : this(owner, days, 10)
        {
        }

        internal CalendarDayCollection(CalendarDayCollection days)
            : this(null, days, 10)
        {
        }

        internal CalendarDayCollection(RadCalendar calnedar)
            : this(calnedar, null, 10)
        {
        }

        internal CalendarDayCollection(RadCalendar owner, CalendarDayCollection days, int capacity)
        {
            this.children.Capacity = capacity;
            this.calendar = owner;
            if (days != null)
            {
                this.AddRange(days);
            }
        } 

        #endregion

        internal protected virtual void UpdateOwnerShip(RadCalendarDay day)
        {
            if (this != day.Owner && day != null)
            {
                day.Owner = this;
            }
        }

        internal protected virtual void UpdateVisuals()
        {
            if (this.calendar != null)
            {
            //    this.calendar.InvalidateLayout();
				this.calendar.CalendarElement.RefreshVisuals(true);
		
            }
        }

        /// <summary>
        /// Finds the RadCalendarDay with specified key, optionally searching child days.
        /// </summary>
        /// <param name="key">The date bound to a particular RadCalendarDay object to search for.</param>
        /// <returns>An array of RadCalendarDay objects whose Date property matches the specified key.</returns>
        public RadCalendarDay[] Find(DateTime key)
        {
            ArrayList list = this.FindInternal(key, this);
            RadCalendarDay[] array = new RadCalendarDay[list.Count];
            list.CopyTo(array, 0);
            return array;
        }

        private ArrayList FindInternal(DateTime key, CalendarDayCollection viewsCollectionToLookIn)
        {
            ArrayList foundDays = new ArrayList();
            if ((viewsCollectionToLookIn == null) || (foundDays == null))
            {
                return null;
            }
            for (int i = 0; i < viewsCollectionToLookIn.Count; i++)
            {
                if ((viewsCollectionToLookIn[i] != null) &&
                    DateTime.Equals(key, viewsCollectionToLookIn[i].Date))
                {
                    foundDays.Add(viewsCollectionToLookIn[i]);
                }
            }
            return foundDays;
        }

        /// <summary>
        /// Returns the index of the specified RadCalendarDay object in the collection.
        /// </summary>
        /// <param name="day">The RadCalendarDay object to locate in the collection.</param>
        /// <returns>The zero-based index of the item found in the CalendarDayCollection; otherwise, -1.</returns>
        public virtual int IndexOf(RadCalendarDay day)
        {
            return this.children.IndexOf(day);
        }

        /// <summary>
        /// Adds an collection of previously created RadCalendarDay objects to the collection.
        /// </summary>
        /// <param name="days">An array of RadCalendarDay objects representing the views to add to the collection.</param>
        public virtual CalendarDayCollection AddRange(CalendarDayCollection days)
        {
            int updateIndex = this.children.Count;
            this.children.AddRange(days);
            for (int i = updateIndex; i < this.children.Count; i++)
            {
                this.UpdateOwnerShip(this.children[i]);
            }
            this.UpdateVisuals();
            return days;
        }

        /// <summary>
        /// Adds a previously created RadCalendarDay object to the end of the CalendarDayCollection.
        /// </summary>
        /// <param name="day">The RadCalendarDay object to add to the collection.</param>
        /// <returns>The zero-based index value of the RadCalendarDay object added to the CalendarDayCollection.</returns>
        public virtual RadCalendarDay Add(RadCalendarDay day)
        {
            if (this.children.Contains(day))
            {
                return null;
            }
            this.UpdateOwnerShip(day);
            this.children.Add(day);
            this.UpdateVisuals();
            return day;
        }

        /// <summary>
        /// Returns an enumerator that can be used to iterate through the RadCalendarDay collection.
        /// </summary>
        /// <returns>An IEnumerator that represents the RadCalendarDay collection.</returns>
        public virtual IEnumerator<RadCalendarDay> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        /// <summary>
        /// Inserts an existing RadCalendarDay object into the CalendarDayCollection at the specified location.
        /// </summary>
        /// <param name="index">The indexed location within the collection to insert the RadCalendarDay object. </param>
        /// <param name="day">The RadCalendarDay object to insert into the collection.</param>
        public virtual void Insert(int index, RadCalendarDay day)
        {
            if (this.children.Contains(day))
            {
                return;
            }
            this.UpdateOwnerShip(day);
            this.children.Insert(index, day);
            this.UpdateVisuals();
        }

        /// <summary>
        /// Removes the specified RadCalendarDay object from the CalendarDayCollection.
        /// </summary>
        /// <param name="day">The RadCalendarDay object to remove.</param>
        public virtual void Remove(RadCalendarDay day)
        {
            if (this.children.Contains(day))
            {
                day.Owner = null;
                this.children.Remove(day);
                this.UpdateVisuals();
            }
        }

        /// <summary>
        /// Returns an enumerator that can be used to iterate through the RadCalendarDay collection.
        /// </summary>
        /// <returns>An IEnumerator that represents the RadCalendarDay collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Removes all RadCalendarDay objects in the collection of CalendarDays.
        /// </summary>
        public void Clear()
        {
            RadCalendarDay[] temp = new RadCalendarDay[this.children.Count];
            this.children.CopyTo(temp);
            this.children.Clear();
            for (int i = 0; i < this.children.Count; i++)
            {
                this.children[i].Owner = null;
            }
            this.UpdateVisuals();
        }

        /// <summary>
        /// Copies the elements of CalendarDayCollection to a new
        /// <see cref="Array"/> of <see cref="RadCalendarDay"/> elements.
        /// </summary>
        /// <returns>A one-dimensional <see cref="Array"/> of <see cref="RadCalendarDay"/>
        /// elements containing copies of the elements of the <see cref="CalendarDayCollection"/>.</returns>
        /// <remarks>Please refer to <see cref="ArrayList.ToArray()"/> for details.</remarks>
        public virtual RadCalendarDay[] ToArray()
        {
            return this.children.ToArray();
        }

        #region ICollection Members

        /// <summary>
        /// Copies the elements of the CalendarDayCollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from CalendarDayCollection. 
        /// The Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        void ICollection.CopyTo(Array array, int index)
        {
            this.children.CopyTo(array as RadCalendarDay[], index);
        }

        /// <summary>
        /// Gets a value indicating whether access to the CalendarDayCollection is synchronized (thread safe). 
        /// </summary>
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the CalendarDayCollection. 
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
        /// Adds a RadCalendarDay object to the collection of CalendarDays.
        /// </summary>
        /// <param name="value">The RadCalendarDay object to add to the collection.</param>
        int IList.Add(object value)
        {
            return this.children.IndexOf(this.Add(value as RadCalendarDay));
        }

        /// <summary>
        /// Removes all RadCalendarDay objects in the collection of CalendarDays.
        /// </summary>
        void IList.Clear()
        {
            this.Clear();
        }

        /// <summary>
        /// Checks whether a specific RadCalendarDay object is in the collection of CalendarDays.
        /// </summary>
        /// <param name="value">The RadCalendarDay object to search.</param>
        /// <returns>True if the RadCalendarDay is found, false otherwise.</returns>
        bool IList.Contains(object value)
        {
            return this.children.Contains(value as RadCalendarDay);
        }

        /// <summary>
        /// Returns a zero based index of a RadCalendarDay object depending on the passed index.
        /// </summary>
        /// <param name="value">The zero-based index, RadCalendarDay object or the date represented by  the searched RadCalendarDay object.</param>
        /// <returns>A zero based index of the RadCalendarDay object in the collection, or -1 if the RadCalendarDay object is not found.</returns>
        int IList.IndexOf(object value)
        {
            if (value is RadCalendarDay)
            {
                return this.children.IndexOf(value as RadCalendarDay);
            }
            if (value is string)
            {
                DateTime temp = DateTime.Parse((string)value);
                for (int i = 0; i < this.children.Count; i++)
                {
                    if (this.children[i].Date == temp)
                    {
                        return i;
                    }
                }
                return -1;
            }
            if (value is DateTime)
            {
                for (int i = 0; i < this.children.Count; i++)
                {
                    if (this.children[i].Date == ((DateTime)value))
                    {
                        return i;
                    }
                }
                return -1;
            }
            throw new ArgumentException("You may use only a RadCalendarDay object, date string or DateTime structure as index in this " + this.GetType().ToString() + " type collection.");
        }

        /// <summary>
        /// Adds a RadCalendarDay object in the collection at the specified index.
        /// </summary>
        /// <param name="index">The index after which the RadCalendarDay object is inserted.</param>
        /// <param name="value">The RadCalendarDay object to insert.</param>
        void IList.Insert(int index, object value)
        {
            this.Insert(index, value as RadCalendarDay);
        }

        /// <summary>
        /// Gets a value indicating whether the CalendarDayCollection has a fixed size. 
        /// </summary>
        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the CalendarDayCollection is read-only. 
        /// </summary>
        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a RadCalendarDay object from the collection.
        /// </summary>
        /// <param name="value">The RadCalendarDay object to remove.</param>
        void IList.Remove(object value)
        {
            this.Remove(value as RadCalendarDay);
        }

        /// <summary>
        /// Deletes the RadCalendarDay object from the collection at the specified index.
        /// </summary>
        /// <param name="index">The index in collection at which the RadCalendarDay object will be deleted.</param>
        void IList.RemoveAt(int index)
        {
            this.Remove(this.children[index]);
        }

        /// <summary>
        /// Gets or sets the RadCalendarDay at the specified indexed location in the collection.
        /// </summary>
        /// <param name="index">The indexed location of the RadCalendarDay in the collection.</param>
        /// <returns>The RadCalendarDay at the specified indexed location in the collection.</returns>
        object IList.this[int index]
        {
            get
            {
                return this.children[index];
            }
            set
            {
                this.children[index] = value as RadCalendarDay;
            }
        }

        #endregion

        /// <summary>
        /// Gets the total number of RadCalendarDay objects in the collection.
        /// </summary>
        public virtual int Count
        {
            get
            {
                return this.children.Count;
            }
        }

        /// <summary>
        /// Gets or sets the RadCalendarDay at the specified indexed location in the collection.
        /// </summary>
        /// <param name="index">The indexed location of the RadCalendarDay in the collection.</param>
        /// <returns>The RadCalendarDay at the specified indexed location in the collection.</returns>
        public virtual RadCalendarDay this[int index]
        {
            get
            {
                return this.children[index];
            }
            set
            {
                if (this.children[index] != value)
                {
                    this.UpdateOwnerShip(value);
                    this.children[index] = value;
                    this.UpdateVisuals();
                }
            }
        }

        /// <summary>
        /// Gets or sets a RadCalendarDay object depending on the passed key.
        /// Only integer and string indexes are valid.
        /// </summary>
        public virtual RadCalendarDay this[object obj]  // int index 
        {
            get
            {
                if ((obj == null) || (obj is string && ((string)obj) == String.Empty))
                {
                    return null;
                }
                int index = ((IList)this).IndexOf(obj);
                if (index < 0)
                {
                    return null;
                }
                return this.children[index];
            }
            set
            {
                int index = ((IList)this).IndexOf(obj);
                if (this.children[index] != value)
                {
                    this.UpdateOwnerShip(value);
                    this.children[index] = value;
                    this.UpdateVisuals();
                }
            }
        }

        public RadCalendar Calendar
        {
            get
            {
                return this.calendar;
            }
            set
            {
                this.calendar = value;
            }
        }

        #region ICloneable Members

        /// <summary>
        /// Creates a new CalendarDayCollection object that is a copy of the current instance. 
        /// </summary>
        /// <returns>A new CalendarDayCollection object that is a copy of this instance.</returns>
        public virtual CalendarDayCollection Clone()
        {
            CalendarDayCollection clonedCollection = new CalendarDayCollection(this);
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
    //internal virtual RadCalendarDay AddNew()
    //{
    //    RadCalendarDay temp = new RadCalendarDay();
    //    temp.Owner = this;
    //     this.children.Add(temp);
    //     return temp;
    //}
}
