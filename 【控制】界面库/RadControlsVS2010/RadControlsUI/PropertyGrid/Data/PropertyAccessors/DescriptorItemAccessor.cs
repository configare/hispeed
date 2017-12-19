using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace Telerik.WinControls.UI.PropertyGridData
{
    public class DescriptorItemAccessor: ItemAccessor
    {
        PropertyDescriptor descriptor;

        public DescriptorItemAccessor(PropertyGridItem owner, PropertyDescriptor descriptor): base(owner)
        {
            this.descriptor = descriptor;
        }

        public override PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return this.descriptor;
            }
        }

        public override string Name
        {
            get
            {
                return descriptor.Name;
            }
        }

        public override string Description
        {
            get
            {
                return descriptor.Description;
            }
        }

        public override string Category
        {
            get
            {
                string category = this.descriptor.Category;
                if (category != null && category.Length != 0)
                {
                    return category;
                }
                return base.Category;
            }
        }

        public override object Value
        {
            get
            {
                try
                {
                    return this.GetPropertyValueCore(this.owner.GetValueOwner());
                }
                catch (Exception exception)
                {
                    return exception;
                }
            }
            set
            {
                try
                {
                    object valueOwner = this.owner.GetValueOwner();
                    object propertyValueCore = this.GetPropertyValueCore(valueOwner);
                    if ((value != null) && value.Equals(propertyValueCore))
                    {
                        return;
                    }
                    this.SetPropertyValueCore(valueOwner, value);
                }
                catch { }
            }
        }

        public override Type PropertyType
        {
            get
            {
                return descriptor.PropertyType;
            }
        }

        public override UITypeEditor UITypeEditor
        {
            get
            {
                base.editor = (UITypeEditor)this.descriptor.GetEditor(typeof(UITypeEditor));
                return base.UITypeEditor;
            }
        }

        public override TypeConverter TypeConverter
        {
            get
            {
                try
                {
                    if (base.converter == null)
                    {
                        base.converter = this.descriptor.Converter;
                    }
                }
                catch { }

                return base.TypeConverter;
            }
        }

        protected virtual object GetPropertyValueCore(object target)
        {
            object value;
            if (this.descriptor == null)
            {
                return null;
            }
            if (target is ICustomTypeDescriptor)
            {
                target = ((ICustomTypeDescriptor)target).GetPropertyOwner(this.descriptor);
            }
            try
            {
                value = this.descriptor.GetValue(target);
            }
            catch
            {
                throw;
            }
            return value;
        }

        protected virtual void SetPropertyValueCore(object obj, object value)
        {
            if (this.descriptor != null)
            {
                try
                {
                    object component = obj;
                    if (component is ICustomTypeDescriptor)
                    {
                        component = ((ICustomTypeDescriptor)component).GetPropertyOwner(this.descriptor);
                    }
                    bool isValueTypeOrArray = false;
                    PropertyGridItem parent = this.owner.Parent as PropertyGridItem;
                    if (parent != null)
                    {
                        Type propertyType = parent.PropertyType;
                        isValueTypeOrArray = propertyType.IsValueType || propertyType.IsArray;
                    }
                    if (component != null)
                    {
                        this.descriptor.SetValue(component, value);
                        if (isValueTypeOrArray)
                        {
                            parent.Value = obj;
                        }
                    }
                }
                finally
                {
                }
            }
        }
    }
}
