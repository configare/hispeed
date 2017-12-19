using System;
using System.ComponentModel;
using System.Reflection;
using Telerik.WinControls.Data;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Design;
using System.ComponentModel.Design;
using Telerik.WinControls.UI.PropertyGridData;

namespace Telerik.WinControls.UI
{
    public class PropertyGridItem : PropertyGridItemBase, IDataItem, ITypeDescriptorContext
    {
        #region Fields
        
        private PropertyGridItem parentItem;
        private PropertyGridItemCollection items;
        private object originalValue;
        private string errorMessage;
        private object dataBoundItem;
        private IItemAccessor itemAccessor;

        protected object cachedValue;

        #endregion

        #region Constructors

        public PropertyGridItem(PropertyGridTableElement propertyGridElement)
            : this(propertyGridElement, null)
        {
        }
        
        public PropertyGridItem(PropertyGridTableElement propertyGridElement, PropertyGridItem parentItem)
            : base(propertyGridElement)
        {
            this.parentItem = parentItem;
        }
        
        #endregion

        #region Properties

        public override string Name
        {
            get
            {
                return itemAccessor.Name;
            }
        }

        public override string Label
        {
            get
            {
                if (base.Label != null)
                {
                    return base.Label;
                }
                return this.Name;
            }
            set
            {
                base.Label = value;
            }
        }

        public override string Description
        {
            get
            {
                if (base.Description != null)
                {
                    return base.Description;
                }
                return itemAccessor.Description;
            }
            set
            {
                base.Description = value;
            }
        }
        
        /// <summary>
        /// Gets the categoty of the property from its <see cref="CategoryAttribute"/> or returns "Other" if no category is specified.
        /// </summary>
        public virtual string Category
        {
            get
            {
                return itemAccessor.Category;
            }
        }   

        /// <summary>
        /// Gets or sets the item value.
        /// </summary>
        /// <value>The text.</value>  
        public virtual object Value
        {
            get
            {
                return itemAccessor.Value;
            }
            set
            {
                object convertedValue = null;
                object oldValue = Value;

                if (!ConvertValue(value, out convertedValue) || convertedValue == oldValue)
                {
                    return;
                }

                if (this.PropertyGridTableElement != null)
                {
                    PropertyGridItemValueChangingEventArgs args = new PropertyGridItemValueChangingEventArgs(this, convertedValue, Value);
                    this.PropertyGridTableElement.OnPropertyValueChanging(args);
                    if (args.Cancel)
                    {
                        return;
                    }
                }

                itemAccessor.Value = convertedValue;

                if (this.Accessor.GetType() == typeof(DescriptorItemAccessor))
                {
                    if (!this.IsModified)
                    {
                        this.originalValue = oldValue;
                        this.state[IsModifiedState] = true;
                    }
                    else if (this.originalValue != null && this.originalValue.Equals(convertedValue))
                    {
                        this.state[IsModifiedState] = false;
                    }
                }

                this.PropertyGridTableElement.Update(PropertyGridTableElement.UpdateActions.ValueChanged);

                if (this.PropertyGridTableElement != null)
                {
                    this.PropertyGridTableElement.OnPropertyValueChanged(new PropertyGridItemValueChangedEventArgs(this));
                }
            }
        }

        /// <summary>
        /// Gets the value of the property as a string using its <see cref="TypeConverter"/>.
        /// </summary>
        public virtual string FormattedValue
        {
            get
            {
                object currentValue = this.Value;
                string stringValue = String.Empty;

                try
                {
                    if (currentValue is string)
                    {
                        return currentValue.ToString();
                    }

                    TypeConverter converter = TypeConverter;
                    if (converter != null && converter.CanConvertTo(typeof(string)))
                    {
                        stringValue = converter.ConvertToString(currentValue);
                    }
                }
                catch { }

                return stringValue;
            }
            set
            {
                TypeConverter converter = TypeConverter;
                if (converter != null && converter.CanConvertFrom(typeof(string)))
                {
                    object valueToSet = converter.ConvertFromString(value);
                    this.Value = valueToSet;
                }
            }
        }

        /// <summary>
        /// Gets the original property value.
        /// </summary>
        public virtual object OriginalValue
        {
            get
            {
                return originalValue;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the property value is modified.
        /// </summary>
        public virtual bool IsModified
        {
            get
            {
                if (this.Accessor is DescriptorItemAccessor)
                {
                    return state[IsModifiedState];
                }
                PropertyGridItem parentItem = this.Parent as PropertyGridItem;
                if (parentItem != null)
                {
                    return parentItem.IsModified;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is a complex property.
        /// </summary>
        public override bool Expandable
        {
            get
            {
                return this.GridItems.Count > 0;
            }
        }

        /// <summary>
        /// Gets the sub items of the current if it is composed of several subitems.
        /// </summary>
        public override PropertyGridItemCollection GridItems
        {
            get
            {
                if (this.items == null)
                {
                    items = GetChildItems(this, this.Value, this.PropertyType);
                }
                return this.items;
            }
        }

        /// <summary>
        /// Gets or sets the parent of this item.
        /// </summary>
        public override PropertyGridItemBase Parent
        {
            get
            {
                return this.parentItem;
            }
        }

        /// <summary>
        /// Gets or sets an error message to be displayed when property value validation fails.
        /// </summary>
        public virtual string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                if (this.errorMessage != value)
                {
                    this.errorMessage = value;
                    Update(UI.PropertyGridTableElement.UpdateActions.StateChanged);
                    OnNotifyPropertyChanged("ErrorMessage");
                }
            }
        }

        /// <summary>
        /// Gets the UITypeEditor associated with this property
        /// </summary>
        public virtual UITypeEditor UITypeEditor
        {
            get
            {
                return itemAccessor.UITypeEditor;
            }
        }

        /// <summary>
        /// Gets the TypeConverter associated with this property
        /// </summary>
        public virtual TypeConverter TypeConverter
        {
            get
            {
                return itemAccessor.TypeConverter;
            }
        }

        /// <summary>
        /// Gets the property type
        /// </summary>
        public virtual Type PropertyType
        {
            get
            {
                return itemAccessor.PropertyType;
            }
        }

        /// <summary>
        /// Gets the property descriptor for this property.
        /// </summary>
        public virtual PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return itemAccessor.PropertyDescriptor;
            }
        }

        /// <summary>
        /// Gets the item accessor for this property item.
        /// </summary>
        public IItemAccessor Accessor
        {
            get 
            {
                return this.itemAccessor;
            }
            internal set
            {
                this.itemAccessor = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets the property value to its default value.
        /// </summary>
        public virtual void ResetValue()
        {
            if (this.IsModified)
            {
                this.Value = originalValue;
                this.state[IsModifiedState] = false;
                this.originalValue = null;
            }
        }

        public virtual void BeginEdit()
        {
            this.PropertyGridTableElement.SelectedGridItem = this;
            this.PropertyGridTableElement.BeginEdit();
        }

        internal virtual object GetValueOwner()
        {
            if (this.parentItem == null)
            {
                return this.PropertyGridTableElement.SelectedObject;
            }
            return this.parentItem.Value;
        }
       
        #endregion

        #region Implementation

        private bool ConvertValue(object value, out object convertedValue)
        {
            convertedValue = value;

            if (value == null)
            {
                return true;
            }

            Type sourceType = value.GetType();
            Type targetType = this.PropertyType;
            if (targetType.IsAssignableFrom(sourceType))
            {
                return true;
            }

            TypeConverter valueConverter = TypeDescriptor.GetConverter(sourceType);
            if (valueConverter != null && valueConverter.CanConvertTo(targetType))
            {
                convertedValue = valueConverter.ConvertTo(null, System.Threading.Thread.CurrentThread.CurrentUICulture, value, targetType);
                return true;
            }

            valueConverter = TypeConverter;
            if (valueConverter != null && valueConverter.CanConvertFrom(sourceType))
            {
                convertedValue = valueConverter.ConvertFrom(null, System.Threading.Thread.CurrentThread.CurrentUICulture, value);
                return true;
            }

            IConvertible convertibleValue = value as IConvertible;
            if (convertibleValue == null)
            {
                convertedValue = null;
                return false;
            }

            try
            {
                convertedValue = Convert.ChangeType(convertibleValue, targetType);
            }
            catch (InvalidCastException)
            {
                convertedValue = null;
                return false;
            }

            return true;
        }

        protected virtual PropertyGridItemCollection GetChildItems(PropertyGridItem parentItem, object obj, Type objType)
        {
            PropertyGridItemCollection items = new PropertyGridItemCollection(new List<PropertyGridItem>());
            if (obj == null)
            {
                return items;
            }
            
            try
            {
                if (!this.TypeConverter.GetPropertiesSupported(this))
                {
                    return items;
                }
                PropertyDescriptorCollection descriptors = this.TypeConverter.GetProperties(this, dataBoundItem, new Attribute[] { new BrowsableAttribute(true) });
                if (descriptors == null)
                {
                    return items;
                }               
                if (((descriptors == null) || (descriptors.Count == 0)) && (((objType != null) && objType.IsArray) && (obj != null)))
                {
                    Array array = (Array)obj;
                    for (int i = 0; i < array.Length; i++)
                    {
                        PropertyGridItem item = new PropertyGridItem(this.PropertyGridTableElement, parentItem);
                        item.Accessor = new ArrayItemAccessor(item, i);
                        items.AddProperty(item);
                    }
                    return items;
                }
                bool createInstanceSupported = this.TypeConverter.GetCreateInstanceSupported(this);
                foreach (PropertyDescriptor childDescriptor in descriptors)
                {
                    PropertyGridItem item;
                    try
                    {
                        object component = obj;
                        if (obj is ICustomTypeDescriptor)
                        {
                            component = ((ICustomTypeDescriptor)obj).GetPropertyOwner(childDescriptor);
                        }
                        childDescriptor.GetValue(component);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    if (createInstanceSupported)
                    {
                        item = new PropertyGridItem(this.PropertyGridTableElement, parentItem);
                        item.Accessor = new ImmutableItemAccessor(item, childDescriptor);
                    }
                    else
                    {
                        item = new PropertyGridItem(this.PropertyGridTableElement, parentItem);
                        item.Accessor = new DescriptorItemAccessor(item, childDescriptor);
                    }
                    items.AddProperty(item);
                }
            }
            catch (Exception)
            {
            }
            return items;
        }

        #endregion

        #region IDataItem Members

        public object DataBoundItem
        {
            get
            {
                return this.dataBoundItem;
            }
            set
            {
                if (value != this.dataBoundItem)
                {
                    this.dataBoundItem = value;
                    itemAccessor = new DescriptorItemAccessor(this, this.dataBoundItem as PropertyDescriptor);
                }
            }
        }

        public int FieldCount
        {
            get
            {
                return 3;
            }
        }

        public object this[string name]
        {
            get
            {
                if (name == "Name")
                {
                    return this.Name;
                }
                else if (name == "Value")
                {
                    return this.Value;
                }
                else if (name == "Category")
                {
                    return this.Category;
                }
                else if (name == "FormattedValue")
                {
                    return this.FormattedValue;
                }
                else if (name == "Label")
                {
                    return this.Label;
                }
                else if (name == "Description")
                {
                    return this.Description;
                }
                else if (name == "OriginalValue")
                {
                    return this.OriginalValue;
                }

                return null;
            }
            set
            {
            }
        }

        public object this[int index]
        {
            get
            {
                if (index == 0)
                {
                    return this.Name;
                }
                else if (index == 1)
                {
                    return this.Value;
                }
                else if (index == 2)
                {
                    return this.Category;
                }
                else if (index == 3)
                {
                    return this.FormattedValue;
                }
                else if (index == 4)
                {
                    return this.Label;
                }
                else if (index == 5)
                {
                    return this.Description;
                }
                else if (index == 6)
                {
                    return this.OriginalValue;
                }

                return null;
            }
            set
            {
            }
        }

        public int IndexOf(string name)
        {
            if (name == "Name")
            {
                return 0;
            }
            else if (name == "Value")
            {
                return 1;
            }
            else if (name == "Category")
            {
                return 2;
            }
            else if (name == "FormattedValue")
            {
                return 3;
            }
            else if (name == "Label")
            {
                return 4;
            }
            else if (name == "Description")
            {
                return 5;
            }
            else if (name == "OriginalValue")
            {
                return 6;
            }

            return 0;
        }

        #endregion

        #region ITypeDescriptorContext Members

        public IContainer Container
        {
            get 
            {
                IComponent component = this.PropertyGridTableElement.ElementTree.Control;
                if (component != null)
                {
                    ISite site = component.Site;
                    if (site != null)
                    {
                        return site.Container;
                    }
                }
                return null;
            }
        }

        public object Instance
        {
            get 
            {
                object valueOwner = this.GetValueOwner();
                if (this.parentItem != null && valueOwner == null)
                {
                    return this.parentItem.Instance;
                }
                return valueOwner;
            }
        }

        public void OnComponentChanged()
        {
            //if (this.ComponentChangeService != null)
            //{
            //    this.ComponentChangeService.OnComponentChanged(this.GetValueOwner(), this.PropertyDescriptor, null, null);
            //}
        }

        public bool OnComponentChanging()
        {
            //if (this.ComponentChangeService != null)
            //{
            //    try
            //    {
            //        this.ComponentChangeService.OnComponentChanging(this.GetValueOwner(), this.PropertyDescriptor);
            //    }
            //    catch (CheckoutException exception)
            //    {
            //        if (exception != CheckoutException.Canceled)
            //        {
            //            throw exception;
            //        }
            //        return false;
            //    }
            //}
            return true;
        }

        #endregion

        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(PropertyGridItem))
            {
                return this;
            }
            if (this.parentItem != null)
            {
                return this.parentItem.GetService(serviceType);
            }
            return null;
        }

        #endregion
    }
}
