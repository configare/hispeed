using System;

namespace Telerik.WinControls.UI
{
    class ListViewVirtualizedElementProvider : VirtualizedPanelElementProvider<ListViewDataItem, BaseListViewVisualItem>
    {
        #region Fields

        private BaseListViewElement owner;

        #endregion

        #region Ctor

        public ListViewVirtualizedElementProvider(BaseListViewElement owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Overrides

        public override IVirtualizedElement<ListViewDataItem> CreateElement(ListViewDataItem data, object context)
        {
            Type type = this.owner.GetType();
            ListViewType listViewType = GetListViewType(type);
            if (data is ListViewDataItemGroup)
            {
                if (type == typeof(SimpleListViewElement))
                {
                    return OnElementCreating(new SimpleListViewGroupVisualItem(), listViewType);
                }
                if (type == typeof(IconListViewElement))
                {
                    return OnElementCreating(new IconListViewGroupVisualItem(), listViewType);
                }
                if (type == typeof(DetailListViewElement))
                {
                    return OnElementCreating(new DetailListViewGroupVisualItem(), listViewType);
                }

                return OnElementCreating(new SimpleListViewGroupVisualItem(), listViewType);
            }

            else
            {
                if (type == typeof(SimpleListViewElement))
                {
                    return OnElementCreating(new SimpleListViewVisualItem(), listViewType);
                }
                if (type == typeof(IconListViewElement))
                {
                    return OnElementCreating(new IconListViewVisualItem(), listViewType);
                }
                if (type == typeof(DetailListViewElement))
                {
                    return OnElementCreating(new DetailListViewVisualItem(), listViewType);
                }

                return OnElementCreating(new SimpleListViewVisualItem(), listViewType);
            }

        }

        public override System.Drawing.SizeF GetElementSize(ListViewDataItem item)
        {
            return item.ActualSize;
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Fires the VisualItemCreating event of <see cref="RadListView"/>.
        /// </summary>
        /// <param name="item">The visual item.</param>
        /// <param name="viewType">The view type of <see cref="RadListView"/></param>
        /// <returns>The new visual item.</returns>
        protected virtual BaseListViewVisualItem OnElementCreating(BaseListViewVisualItem item, ListViewType viewType)
        {
            ListViewVisualItemCreatingEventArgs args = new ListViewVisualItemCreatingEventArgs(item,viewType);
            this.owner.Owner.OnVisualItemCreating(args);

            return args.VisualItem;
        }

        #endregion

        #region Helpers

        private ListViewType GetListViewType(Type type)
        {
            if (type == typeof(SimpleListViewElement))
            {
                return ListViewType.ListView;
            }
            if (type == typeof(IconListViewElement))
            {
                return ListViewType.IconsView;
            }
            if (type == typeof(DetailListViewElement))
            {
                return ListViewType.DetailsView;
            }

            return ListViewType.ListView;
        }

        #endregion
    }
}
