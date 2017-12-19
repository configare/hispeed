using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Telerik.WinControls.Data
{
    public interface ICollectionView<T>
        where T : IDataItem
    {
        bool CanFilter { get; }
        bool CanGroup { get; }
        bool CanSort { get; }
        Predicate<T> Filter { get; set; }
        SortDescriptorCollection SortDescriptors { get; }
        GroupDescriptorCollection GroupDescriptors { get; }
        GroupCollection<T> Groups { get; }

        IEnumerable<T> SourceCollection { get; }
        void Refresh();
        event NotifyCollectionChangedEventHandler CollectionChanged;

        T CurrentItem { get; }
        int CurrentPosition { get; }
        event EventHandler CurrentChanged;
        event CancelEventHandler CurrentChanging;
        bool MoveCurrentTo(T item);
        bool MoveCurrentToFirst();
        bool MoveCurrentToLast();
        bool MoveCurrentToNext();
        bool MoveCurrentToPosition(int position);
        bool MoveCurrentToPrevious();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISnapshotCollectionView<T> : ICollectionView<T>, IReadOnlyCollection<T> where T : IDataItem
    {

    }
}
