using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace Telerik.WinControls.UI.PropertyGridData
{
    public abstract class ItemAccessor: IItemAccessor
    {
        protected UITypeEditor editor;
        protected TypeConverter converter;
        protected PropertyGridItem owner;

        public ItemAccessor(PropertyGridItem owner)
        {
            this.owner = owner;
        }

        #region IItemAccessor Members

        /// <summary>
        ///  Gets the property name.
        /// </summary>
        public virtual string Name
        {
            get 
            {
                return ""; 
            }
        }

        /// <summary>
        /// Gets or sets the property value.
        /// </summary>
        public virtual object Value
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the description associated with this property.
        /// </summary>
        public virtual string Description
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the categoty of the property from its <see cref="CategoryAttribute"/> or returns "Other" if no category is specified.
        /// </summary>
        public virtual string Category
        {
            get
            {
                return CategoryAttribute.Default.Category;
            }
        }

        /// <summary>
        /// Gets the property type.
        /// </summary>
        public virtual Type PropertyType
        {
            get
            {
                object propertyValue = this.Value;
                if (propertyValue != null)
                {
                    return propertyValue.GetType();
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the property descriptor for this property.
        /// </summary>
        public virtual PropertyDescriptor PropertyDescriptor
        {
            get 
            { 
                return null; 
            }
        }

        /// <summary>
        /// Gets the UITypeEditor associated with this property
        /// </summary>
        public virtual UITypeEditor UITypeEditor
        {
            get
            {
                if ((this.editor == null) && (this.PropertyType != null))
                {
                    this.editor = (UITypeEditor)TypeDescriptor.GetEditor(this.PropertyType, typeof(UITypeEditor));
                }
                return this.editor;
            }
        }

        /// <summary>
        /// Gets the TypeConverter associated with this property
        /// </summary>
        public virtual TypeConverter TypeConverter
        {
            get
            {
                if (this.converter == null)
                {
                    object propertyValue = this.Value;
                    if (propertyValue == null)
                    {
                        this.converter = TypeDescriptor.GetConverter(this.PropertyType);
                    }
                    else
                    {
                        this.converter = TypeDescriptor.GetConverter(propertyValue);
                    }
                }
                return this.converter;
            }
        }

        #endregion

        /// <summary>
        /// Gets the <see cref="PropertyGridItem"/> associated with this accessor.
        /// </summary>
        public PropertyGridItem Owner
        {
            get 
            {
                return owner;
            }
        }
    }
}
