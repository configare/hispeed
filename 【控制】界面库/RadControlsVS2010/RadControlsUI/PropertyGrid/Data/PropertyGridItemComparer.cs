using System;
using System.Collections.Generic;
using System.ComponentModel;
using Telerik.WinControls.Data;


namespace Telerik.WinControls.UI
{
    public class PropertyGridItemComparer : IComparer<PropertyGridItem>
    {
        private PropertyGridTableElement propertyGridElement;
        private List<PropertyGridItemDescriptor> context = new List<PropertyGridItemDescriptor>();

        class PropertyGridItemDescriptor
        {
            public PropertyDescriptor Descriptor;
            public int Index;
            public bool Descending;
        }

        public PropertyGridItemComparer(PropertyGridTableElement propertyGridElement)
        {
            this.propertyGridElement = propertyGridElement;

            this.Update();
        }

        public void Update()
        {
            this.context.Clear();

            if (!this.propertyGridElement.ListSource.IsDataBound)
            {
                return;
            }

            for (int i = 0; i < this.propertyGridElement.SortDescriptors.Count; i++)
            {
                SortDescriptor sd = this.propertyGridElement.SortDescriptors[i];
                PropertyDescriptor pd = this.propertyGridElement.ListSource.BoundProperties.Find(sd.PropertyName, true);
                if (pd != null)
                {
                    PropertyGridItemDescriptor descriptor = new PropertyGridItemDescriptor();
                    descriptor.Descriptor = pd;
                    descriptor.Index = this.propertyGridElement.ListSource.BoundProperties.IndexOf(pd);
                    descriptor.Descending = (sd.Direction == ListSortDirection.Descending);
                    this.context.Add(descriptor);
                }
            }
        }

        #region IComparer<PropertyGridDataItem> Members

        public int Compare(PropertyGridItem x, PropertyGridItem y)
        {
            object xValue = x.Name;
            object yValue = y.Name;

            int result = 0;
            IComparable xCompVal = xValue as IComparable;
            if (xCompVal != null && yValue != null && yValue.GetType() == xValue.GetType())
            {
                result = ((IComparable)xValue).CompareTo(yValue);
            }
            else
            {
                result = Telerik.Data.Expressions.DataStorageHelper.CompareNulls(xValue, yValue);
            }

            if (result != 0 && propertyGridElement.SortOrder == System.Windows.Forms.SortOrder.Descending)
            {
                return -result;
            }

            return result;
        }

        #endregion
    }
}
