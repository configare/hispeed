using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls.UI
{
    public class CalendarViewCollection : IEnumerable<CalendarView>, IEnumerable, IList, ICollection
    {
        private List<CalendarView> children = new List<CalendarView>();
        private CalendarView owner;
        private RadCalendar calendar;

        #region Constructors
        internal CalendarViewCollection(CalendarView owner)
            : this(owner.Calendar, owner, 10)
        {
        }

        internal CalendarViewCollection(RadCalendar calnedar)
            : this(calnedar, null, 10)
        {
        }

        internal CalendarViewCollection(RadCalendar calnedar, CalendarView owner, int capacity)
        {
            this.children.Capacity = capacity;
            this.owner = owner;
            this.calendar = calnedar;
        } 
        #endregion

        /// <summary>
        /// Updates correctly the visual appearance of RadCalendar. Updates the parential 
        /// dependencies (parent and Calendar properties) also.
        /// </summary>
        /// <param name="view">the CalendarView that will be updated</param>
        internal protected virtual void UpdateOwnerShip(CalendarView view)
        {
            if (this.calendar != view.Calendar ||
                this.owner != view.Parent)
            {
                view.Remove();
            }
            view.Parent = this.owner;
            view.Calendar = this.calendar;
        }

        /// <summary>
        /// Finds the calendar views with specified key, optionally searching child views.
        /// </summary>
        /// <param name="key">The name of the calendar view to search for.</param>
        /// <param name="searchAllChildren">true to search child views; otherwise, false.</param>
        /// <returns>An array of CalendarView objects whose Name property matches the specified key.</returns>
        public CalendarView[] Find(string key, bool searchAllChildren)
		{
			ArrayList list = this.FindInternal(key, searchAllChildren, this);
            CalendarView[] array = new CalendarView[list.Count];
			list.CopyTo(array, 0);
			return array;
		}

        private ArrayList FindInternal(string key, bool searchAllChildren, CalendarViewCollection viewsCollectionToLookIn)
		{
            ArrayList foundViews = new ArrayList();
			if ((viewsCollectionToLookIn == null) || (foundViews == null))
			{
				return null;
			}
			for (int i = 0; i < viewsCollectionToLookIn.Count; i++)
			{
				if ((viewsCollectionToLookIn[i] != null) && 
                    WindowsFormsUtils.SafeCompareStrings(viewsCollectionToLookIn[i].Name, key, true))
				{
					foundViews.Add(viewsCollectionToLookIn[i]);
                    if (searchAllChildren && viewsCollectionToLookIn[i].Children.Count > 0)
                    {
                        foundViews.AddRange(this.FindInternal(key, searchAllChildren, viewsCollectionToLookIn[i].Children));
                    }
				}
			}
			return foundViews;
		}

        /// <summary>
        /// Returns the index of the specified calendar view in the collection.
        /// </summary>
        /// <param name="view">The CalendarView to locate in the collection.</param>
        /// <returns>The zero-based index of the item found in the calendar view collection; otherwise, -1.</returns>
        public virtual int IndexOf(CalendarView view)
        {
            return this.children.IndexOf(view);
        }

        /// <summary>
        /// Adds an collection of previously created CalendarView objects to the collection.
        /// </summary>
        /// <param name="viewsCollection">An array of CalendarView objects representing the views to add to the collection.</param>
        public virtual CalendarViewCollection AddRange(CalendarViewCollection viewsCollection)
        {
            int updateIndex = this.children.Count;
            this.children.AddRange(viewsCollection);
            for (int i = updateIndex; i < this.children.Count; i++)
            {
                this.UpdateOwnerShip(this.children[i]);
            }
            //this.UpdateVisuals();
            return viewsCollection;
        }

        /// <summary>
        /// Adds a previously created CalendarView object to the end of the CalendarViewCollection.
        /// </summary>
        /// <param name="view">The CalendarView object to add to the collection.</param>
        /// <returns>The zero-based index value of the CalendarView object added to the CalendarViewCollection.</returns>
        public virtual CalendarView Add(CalendarView view)
		{
            if (this.children.Contains(view))
	        {
        		 return null;
	        }
            this.UpdateOwnerShip(view);
			this.children.Add(view);
            //this.UpdateVisuals();
			return view;
		}

        /// <summary>
        /// Returns an enumerator that can be used to iterate through the CalendarView collection.
        /// </summary>
        /// <returns>An IEnumerator that represents the CalendarView collection.</returns>
        public virtual IEnumerator<CalendarView> GetEnumerator()
		{
			return children.GetEnumerator();
		}

        /// <summary>
        /// Inserts an existing CalendarView object into the CalendarViewCollection at the specified location.
        /// </summary>
        /// <param name="index">The indexed location within the collection to insert the CalendarView object. </param>
        /// <param name="view">The CalendarView object to insert into the collection.</param>
        public virtual void Insert(int index, CalendarView view)
		{
            if (this.children.Contains(view))
            {
                return;
            }
            this.UpdateOwnerShip(view);
            this.children.Insert(index, view); 
            //this.UpdateVisuals();
		}

        /// <summary>
        /// Removes the specified CalendarView object from the CalendarViewCollection.
        /// </summary>
        /// <param name="view">The CalendarView object to remove.</param>
        public virtual void Remove(CalendarView view)
		{
			if (this.children.Contains(view))
			{
                view.Parent = null;
                view.Calendar = null;
				this.children.Remove(view);
                //this.UpdateVisuals();
			}
		}

        /// <summary>
        /// Returns an enumerator that can be used to iterate through the CalendarView collection.
        /// </summary>
        /// <returns>An IEnumerator that represents the CalendarView collection.</returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

        /// <summary>
        /// Removes all CalendarView objects from the collection.
        /// </summary>
        public void Clear()
        {
            CalendarView[] temp = new CalendarView[this.children.Count];
            this.children.CopyTo(temp);
            this.children.Clear();
			for (int i = 0; i < temp.Length; i++)
            {
				temp[i].Parent = null;
				temp[i].Calendar = null;
            }
            //this.UpdateVisuals();
        }

        #region Properties

        public RadCalendar Calendar
        {
            get
            {
                if (this.owner != null)
                {
                    return this.owner.Calendar;
                }
                return this.calendar;
            }
        }

        public CalendarView Owner
        {
            get
            {
                return this.owner;
            }
        }

        /// <summary>
        /// Gets the total number of CalendarView objects in the collection.
        /// </summary>
        public virtual int Count
        {
            get
            {
                return this.children.Count;
            }
        }

        /// <summary>
        /// Gets or sets the CalendarView at the specified indexed location in the collection.
        /// </summary>
        /// <param name="index">The indexed location of the CalendarView in the collection.</param>
        /// <returns>The CalendarView at the specified indexed location in the collection.</returns>
        public virtual CalendarView this[int index]
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
                    //this.UpdateVisuals();
                }
            }
        }

        /// <summary>
        /// Gets or sets by name the CalendarView instance in the collection.
        /// </summary>
        /// <param name="str">The name of the CalendarView in the collection.</param>
        /// <returns>The CalendarView with a specified name in the collection.</returns>
        public virtual CalendarView this[string str]
        {
            get
            {
                return null;// this.children.Find(delegate(CalendarView node) { return node.Text == s; });
            }
        } 

        #endregion

		#region ICollection Members

        /// <summary>
        /// Copies the elements of the CalendarViewCollection to an Array, starting at a particular Array index.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from CalendarViewCollection. 
        /// The Array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
		void ICollection.CopyTo(Array array, int index)
		{
            this.children.CopyTo(array as CalendarView[], index);
		}

        /// <summary>
        /// Gets a value indicating whether access to the CalendarViewCollection is synchronized (thread safe). 
        /// </summary>
		bool ICollection.IsSynchronized
		{
			get 
			{
				return false;
			}
		}

        /// <summary>
        /// Gets an object that can be used to synchronize access to the CalendarViewCollection. 
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
        /// Adds a previously created CalendarView object to the end of the CalendarViewCollection.
        /// </summary>
        /// <param name="value">The CalendarView object to add to the collection.</param>
        /// <returns>The zero-based index value of the CalendarView object added to the CalendarViewCollection.</returns>
		int IList.Add(object value)
		{
            return this.children.IndexOf(this.Add(value as CalendarView));
		}

        /// <summary>
        /// Removes all CalendarView objects from the collection.
        /// </summary>
		void IList.Clear()
		{
			this.Clear();
		}

        /// <summary>
        /// Determines whether the specified CalendarView object is a member of the collection.
        /// </summary>
        /// <param name="value">The CalendarView to locate in the collection.</param>
        /// <returns>true if the CalendarView is a member of the collection; otherwise, false.</returns>
		bool IList.Contains(object value)
		{
            return this.children.Contains(value as CalendarView);
		}

        /// <summary>
        /// Returns the index of the specified calendar view in the collection.
        /// </summary>
        /// <param name="value">The CalendarView to locate in the collection.</param>
        /// <returns>The zero-based index of the item found in the calendar view collection; otherwise, -1.</returns>
		int IList.IndexOf(object value)
		{
            return this.children.IndexOf(value as CalendarView);
		}

        /// <summary>
        /// Inserts an existing CalendarView object into the CalendarViewCollection at the specified location.
        /// </summary>
        /// <param name="index">The indexed location within the collection to insert the CalendarView object. </param>
        /// <param name="value">The CalendarView object to insert into the collection.</param>
		void IList.Insert(int index, object value)
		{
            this.Insert(index, value as CalendarView);
		}

        /// <summary>
        /// Gets a value indicating whether the CalendarViewCollection has a fixed size. 
        /// </summary>
		bool IList.IsFixedSize
		{
			get
			{
				return false;
			}
		}

        /// <summary>
        /// Gets a value indicating whether the CalendarViewCollection is read-only. 
        /// </summary>
		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

        /// <summary>
        /// Removes the specified CalendarView object from the CalendarViewCollection.
        /// </summary>
        /// <param name="value">The CalendarView object to remove.</param>
		void IList.Remove(object value)
		{
            this.Remove(value as CalendarView);
		}

        /// <summary>
        /// Removes the element at the specified index of the CalendarViewCollection. 
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
		void IList.RemoveAt(int index)
		{
			this.Remove(this.children[index]);
		}

        /// <summary>
        /// Gets or sets the CalendarView at the specified indexed location in the collection.
        /// </summary>
        /// <param name="index">The indexed location of the CalendarView in the collection.</param>
        /// <returns>The CalendarView at the specified indexed location in the collection.</returns>
		object IList.this[int index]
		{
			get
			{
				return this.children[index];
			}
			set
			{
                this.children[index] = value as CalendarView;
			}
		}

		#endregion
    }
}

//internal protected virtual void UpdateVisuals()
//{
//    if (this.owner != null)
//    {
//        //this.owner.DirtyLayout = true;
//    }
//    else if (this.calendar != null)
//    {
//        //this.calendar.InvalidateLayout();
//    }
//}