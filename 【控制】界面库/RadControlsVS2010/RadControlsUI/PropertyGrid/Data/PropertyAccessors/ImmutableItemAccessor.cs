using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace Telerik.WinControls.UI.PropertyGridData
{
    public class ImmutableItemAccessor: DescriptorItemAccessor
    {
        public ImmutableItemAccessor(PropertyGridItem owner, PropertyDescriptor descriptor)
            : base(owner, descriptor)
        {
        }

        public override object Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                object valueOwner = this.owner.GetValueOwner();
                PropertyGridItem instanceParentGridEntry = this.InstanceParentGridEntry;
                TypeConverter typeConverter = instanceParentGridEntry.TypeConverter;
                PropertyDescriptorCollection properties = typeConverter.GetProperties(instanceParentGridEntry, valueOwner);
                IDictionary propertyValues = new Hashtable(properties.Count);
                object obj3 = null;
                for (int i = 0; i < properties.Count; i++)
                {
                    if ((base.PropertyDescriptor.Name != null) && base.PropertyDescriptor.Name.Equals(properties[i].Name))
                    {
                        propertyValues[properties[i].Name] = value;
                    }
                    else
                    {
                        propertyValues[properties[i].Name] = properties[i].GetValue(valueOwner);
                    }
                }
                try
                {
                    obj3 = typeConverter.CreateInstance(instanceParentGridEntry, propertyValues);
                }
                catch (Exception)
                {
                    throw;
                }
                if (obj3 != null)
                {
                    instanceParentGridEntry.Value = obj3;
                }
            }
        }

        private PropertyGridItem InstanceParentGridEntry
        {
            get
            {
                PropertyGridItem parentGridEntry = this.owner.Parent as PropertyGridItem;
                //if (parentGridEntry is PropertyGridGroupItem)
                //{
                //    parentGridEntry = parentGridEntry.Parent;
                //}
                return parentGridEntry;
            }
        }
    }
}
