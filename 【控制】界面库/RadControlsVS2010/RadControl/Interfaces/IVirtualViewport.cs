using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls
{
    public interface IVirtualViewport
    {
        bool Virtualized
        {
            get;
        }

        void SetVirtualItemsCollection(IVirtualizationCollection virtualItemsCollection);
        
        void OnItemDataInserted(int index, object itemData);
        void OnItemDataRemoved(int index, object itemData);
        void OnItemDataSet(int index, object oldItemData, object newItemData);
        void OnItemsDataClear();
        void OnItemsDataClearComplete();
        void OnItemsDataSort();
        void OnItemsDataSortComplete();

        void BeginUpdate();
        void EndUpdate();

        /*VisualElement GetVisibleElement(object data);

        bool CanAddVisualElement();
        void FillWithElements();
        //void CleanupNonVisibleElements();
        void ResetVisualList();

        IVisualElementProvider VisualElementProvider
        {
            get;
        }

        RadElementCollection Children
        {
            get;
        }*/
    }
}
