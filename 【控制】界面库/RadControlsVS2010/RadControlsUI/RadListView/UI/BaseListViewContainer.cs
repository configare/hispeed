using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class BaseListViewContainer : VirtualizedStackContainer<ListViewDataItem>
    {
        protected BaseListViewElement owner;

        public BaseListViewContainer(BaseListViewElement owner)
        {
            this.owner = owner;
        }

        protected override bool IsItemVisible(ListViewDataItem data)
        {
            return data.Visible;
        }

        protected override bool BeginMeasure(System.Drawing.SizeF availableSize)
        {
            this.SuspendThemeRefresh();

            foreach (ListViewDataItem data in DataProvider)
            {
                if (data.IsMeasureValid)
                {
                    continue;
                }

                IVirtualizedElement<ListViewDataItem> element = UpdateElement(data is ListViewDataItemGroup ? 1 : 0, data);
                RadElement radElement = element as RadElement;
                if (radElement != null)
                {
                    radElement.InvalidateMeasure();
                    radElement.Measure(availableSize);
                }
            }

            this.owner.Owner.IsItemsMeasureValid = true;
            this.ResumeThemeRefresh();
            owner.Scroller.UpdateScrollRange();

            return base.BeginMeasure(availableSize);
        }

        protected override SizeF MeasureElementCore(RadElement element, SizeF availableSize)
        {
            element.Measure(availableSize);

            return element.DesiredSize;
        }

        protected override int FindCompatibleElement(int position, ListViewDataItem data)
        {
            if (data is ListViewDataItemGroup)
            {
                for (int i = position + 1; i < this.Children.Count; i++)
                {
                    BaseListViewGroupVisualItem visualGroupItem = this.Children[i] as BaseListViewGroupVisualItem;
                    if (visualGroupItem != null)
                    {
                        return i;
                    }
                }

                return -1;
            }

            if (data is ListViewDataItem)
            {
                for (int i = position + 1; i < this.Children.Count; i++)
                {
                    BaseListViewVisualItem visualItem = (BaseListViewVisualItem)this.Children[i];
                    if (visualItem.Data.GetType() == data.GetType())
                    {
                        return i;
                    }
                }
                return -1;
            }

            return -1;
        }

        protected override IVirtualizedElement<ListViewDataItem> UpdateElement(int position, ListViewDataItem data)
        {
            IVirtualizedElement<ListViewDataItem> element = null;
            object elementContext = this.GetElementContext();

            if (position < this.Children.Count)
            {
                element = (IVirtualizedElement<ListViewDataItem>)this.Children[position];
                if (ElementProvider.ShouldUpdate(element, data, elementContext))
                {
                    if (position < this.Children.Count - 1)
                    {
                        IVirtualizedElement<ListViewDataItem> nextElement = (IVirtualizedElement<ListViewDataItem>)this.Children[position + 1];

                        if (nextElement.Data.Equals(data))
                        {
                            this.Children.Move(position + 1, position);
                            nextElement.Synchronize();
                            return nextElement;
                        }
                    }

                    if (ElementProvider.IsCompatible(element, data, elementContext))
                    {
                        element.Detach();
                        element.Attach(data, elementContext);
                    }
                    else
                    {
                        int index = FindCompatibleElement(position, data);
                        if (index > position)
                        {
                            this.Children.Move(index, position);
                            element = (IVirtualizedElement<ListViewDataItem>)this.Children[position];
                            element.Detach();
                            element.Attach(data, elementContext);
                        }
                        else
                        {
                            position = Math.Min(position, this.Children.Count);
                            element = ElementProvider.GetElement(data, elementContext);
                            InsertElement(position, element, data);
                        }
                    }
                }
            }
            else
            {
                position = Math.Min(position, this.Children.Count);
                element = ElementProvider.GetElement(data, elementContext);
                InsertElement(position, element, data);
            }

            return element;
        } 
    }
}
