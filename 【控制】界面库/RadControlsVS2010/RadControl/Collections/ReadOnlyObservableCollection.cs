using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Telerik.WinControls.Data
{
	/// <summary>
	/// Represents a read-only data collection that provides notifications when the original <see cref="ObservableCollection&lt;T&gt;"/> has changed. 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ReadOnlyObservableCollection<T> : ReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		/// <summary>
		/// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
		/// </summary>
		protected event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// Occurs when when a property of an object changes change. 
		/// Calling the event is developer's responsibility.
		/// </summary>
		protected event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.
		/// </summary>
		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
		{
			add
			{
				this.CollectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Combine(this.CollectionChanged, value);
			}
			remove
			{
				this.CollectionChanged = (NotifyCollectionChangedEventHandler)Delegate.Remove(this.CollectionChanged, value);
			}
		}

		/// <summary>
		/// Occurs when when a property of an object changes change. 
		/// Calling the event is developer's responsibility.
		/// </summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add
			{
				this.PropertyChanged = (PropertyChangedEventHandler)Delegate.Combine(this.PropertyChanged, value);
			}
			remove
			{
				this.PropertyChanged = (PropertyChangedEventHandler)Delegate.Remove(this.PropertyChanged, value);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyObservableCollection&lt;T&gt;"/> with an instance of a <see cref="ObservableCollection&lt;T&gt;"/> />
		/// </summary>
		/// <param name="list"></param>
		public ReadOnlyObservableCollection(ObservableCollection<T> list) : base(list)
		{
			((INotifyCollectionChanged) base.Items).CollectionChanged += new NotifyCollectionChangedEventHandler(this.HandleCollectionChanged);
			((INotifyPropertyChanged) base.Items).PropertyChanged += new PropertyChangedEventHandler(this.HandlePropertyChanged);
		}

		private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.OnCollectionChanged(e);
		}

		private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.OnPropertyChanged(e);
		}

		/// <summary>
		/// Fires the CollectionChanged event.
		/// </summary>
		/// <param name="args"></param>
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, args);
			}
		}

		/// <summary>
		/// Fires the PropertyChnaged event.
		/// </summary>
		/// <param name="args"></param>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, args);
			}
		}
	}
}
