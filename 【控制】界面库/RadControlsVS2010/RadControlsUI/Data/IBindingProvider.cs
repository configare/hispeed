using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Data
{
    /// <summary>
    /// Base interface for providers.
    /// </summary>
    /// <typeparam name="T">The type used to specialize the provider implementation.</typeparam>
    public interface IBindingProvider<T> where T : IDataBoundItem
    {
        /// <summary>
        /// Gets IEnumerable&lt;T&gt; for items that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="filterFunction">The Predicate&lt;T&gt; delegate that defines the conditions of the item to search for.</param>
        /// <returns>IEnumerable&lt;T&gt; for items that match the conditions defined by the specified predicate, if found;</returns>
        IEnumerable<T> GetItems(Predicate<T> filterFunction);

        /// <summary>
        /// Inserts an item of type T.
        /// </summary>
        /// <param name="itemToInsert">The item of type T to insert.</param>
        void Insert(T itemToInsert);

        /// <summary>
        /// Updates he first occurrence of a specific item in the data store.
        /// </summary>
        /// <param name="itemToUpdate">The item of type T to update.</param>
        /// <param name="propertyName">Name of the property which value changed.
        /// Null or an empty string if all properties should be updated.</param>
        void Update(T itemToUpdate, string propertyName);

        /// <summary>
        /// Removes the first occurrence of a specific item from the data store.
        /// </summary>
        /// <param name="itemToDelete">The item of type T to delete.</param>
        void Delete(T itemToDelete);

        /// <summary>
        /// The ItemsChanged event is raised by the provider to inform all listeners that the items in the data store have changed. 
        /// </summary>
        event EventHandler<ListChangedEventArgs<T>> ItemsChanged;

        /// <summary>
        /// The PositionChanged event is raised by the provider to inform all listeners that the current position in data items list has changed.
        /// </summary>
        event PositionChangedEventHandler PositionChanged;

        /// <summary>
        /// Gets or sets the current position in the list of data items.
        /// </summary>
        int Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a data store mapping to the provider.
        /// </summary>
        IPropertyMappingInfo PropertyMappings
        {
            get;
            set;
        }
    }

    public delegate void PositionChangedEventHandler(object sender, PositionChangedEventArgs e);
    public delegate void PositionChangingEventHandler(object sender, PositionChangingCancelEventArgs e);
    public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);
}
