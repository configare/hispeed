using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Telerik.WinControls.UI.Data
{
    /// <summary>
    /// Contains information about a list change event.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListChangedEventArgs<T> : EventArgs
    {
        private ListChangedType listChangeType = ListChangedType.Reset;
        private IList<T> newItems = null;
        private IList<T> oldItems = null;
        private int newIndex = -1;
        private string propertyName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListChangedEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of the list change.</param>
        public ListChangedEventArgs(ListChangedType listChangedType)
        {
            this.listChangeType = listChangedType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListChangedEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of the list change.</param>
        /// <param name="newItem">The new item.</param>
        /// <param name="newIndex">The new index.</param>
        public ListChangedEventArgs(ListChangedType listChangedType, T newItem, int newIndex)
        {
            this.listChangeType = listChangedType;
            this.newItems = new ReadOnlyCollection<T>(new T[] { newItem });
            this.newIndex = newIndex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListChangedEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of the list change.</param>
        /// <param name="changedItem">The changed item.</param>
        /// <param name="propertyName">Name of the property.</param>
        public ListChangedEventArgs(ListChangedType listChangedType, T changedItem, string propertyName)
        {
            this.listChangeType = listChangedType;
            this.oldItems = new ReadOnlyCollection<T>(new T[] { changedItem });
            this.propertyName = propertyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListChangedEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of the list change.</param>
        /// <param name="newItem">The new item.</param>
        /// <param name="oldItem">The old item.</param>
        public ListChangedEventArgs(ListChangedType listChangedType, T newItem, T oldItem)
            : this(listChangedType, newItem, -1)
        {
            this.oldItems = new ReadOnlyCollection<T>(new T[] { oldItem });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListChangedEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of the list change.</param>
        /// <param name="newItems">The new items.</param>
        public ListChangedEventArgs(ListChangedType listChangedType, IList<T> newItems)
        {
            this.listChangeType = listChangedType;
            this.newItems = new ReadOnlyCollection<T>(newItems);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListChangedEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of the list change.</param>
        /// <param name="changedItems">The changed items.</param>
        /// <param name="propertyName">Name of the property.</param>
        public ListChangedEventArgs(ListChangedType listChangedType, IList<T> changedItems, string propertyName)
        {
            this.listChangeType = listChangedType;
            this.oldItems = new ReadOnlyCollection<T>(changedItems);
            this.propertyName = propertyName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListChangedEventArgs&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="listChangedType">Type of the list change.</param>
        /// <param name="newItems">The new items.</param>
        /// <param name="oldItems">The old items.</param>
        public ListChangedEventArgs(ListChangedType listChangedType, IList<T> newItems, IList<T> oldItems)
            : this(listChangedType, newItems)
        {
            this.oldItems = new ReadOnlyCollection<T>(oldItems);
        }


        /// <summary>
        /// Gets the type of the list change.
        /// </summary>
        /// <value>The type of the list change.</value>
        public ListChangedType ListChangedType
        {
            get
            {
                return this.listChangeType;
            }
        }

        public int NewIndex
        {
            get
            {
                return this.newIndex;
            }
        }

        /// <summary>
        /// Gets the new items.
        /// </summary>
        /// <value>The new items.</value>
        public IList<T> NewItems
        {
            get
            {
                return this.newItems;
            }
        }

        /// <summary>
        /// Gets the old items.
        /// </summary>
        /// <value>The old items.</value>
        public IList<T> OldItems
        {
            get
            {
                return this.oldItems;
            }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
        }
    }
}
