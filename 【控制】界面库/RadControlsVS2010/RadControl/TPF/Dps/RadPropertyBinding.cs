using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    [Flags]
    public enum PropertyBindingOptions
    {
        /// <summary>
        /// One-way binding
        /// </summary>
        OneWay = 1,
        /// <summary>
        /// Two-way binding. Both source and target objects can modify the current value.
        /// </summary>
        TwoWay = OneWay << 1,
        /// <summary>
        /// No notifications are raised for the bound object.
        /// </summary>
        NoChangeNotify = TwoWay << 1,
        /// <summary>
        /// Binding value is preserved as local upon unbind.
        /// </summary>
        PreserveAsLocalValue = NoChangeNotify << 1,
    }

    /// <summary>
    /// Supports methods for bound properties of two <see cref="RadObject"/> instances.
    /// </summary>
    public interface IPropertyBinding
    {
        bool CanGetValueForProperty(RadProperty property);
        object GetValue();
    }

	public interface ITwoWayPropertyBinding: IPropertyBinding
	{
		void UpdateBindingSourceProperty(RadProperty boundProperty, object newValue);
	}

    /// <summary>
    /// Supports methods for general binding of properties of two
    /// <see cref="RadObject"/> instances.
    /// </summary>
    public class RadPropertyBinding : ITwoWayPropertyBinding
    {
        private RadProperty fromProperty;
        private RadObject bindingSourceObject;
        private RadProperty toProperty;
        private PropertyBindingOptions options;
        /// <summary>
        /// Initializes a new instance of the RadPropertyBinding class.
        /// </summary>
        /// <param name="bindingSourceObject"></param>
        /// <param name="fromProperty"></param>
        /// <param name="bindingSourceProperty"></param>
        /// <param name="options"></param>
		public RadPropertyBinding(RadObject bindingSourceObject, RadProperty fromProperty, RadProperty bindingSourceProperty, PropertyBindingOptions options)
        {
			this.bindingSourceObject = bindingSourceObject;
            this.fromProperty = fromProperty;
			this.toProperty = bindingSourceProperty;
			this.options = options;
        }
        /// <summary>
        /// Gets the binding source.
        /// </summary>
    	public RadObject BindingSourceObject
    	{
    		get { return bindingSourceObject; }
    	}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
    	public bool CanGetValueForProperty(RadProperty property)
        {
            return property == this.fromProperty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            return this.BindingSourceObject.GetValue(toProperty);
        }

        /// <summary>
        /// Reset the bound properties
        /// </summary>
        public void ResetValue()
        {
            this.BindingSourceObject.ResetValue(fromProperty);
            this.BindingSourceObject.ResetValue(toProperty);
        }

        /// <summary>
        /// Updates the binding source property.
        /// </summary>
        /// <param name="boundProperty"></param>
        /// <param name="newValue"></param>
    	public void UpdateBindingSourceProperty(RadProperty boundProperty, object newValue)
    	{
			if ( this.options == PropertyBindingOptions.TwoWay )
			{
				this.BindingSourceObject.SetValue(toProperty, newValue);
			}
    	}
    }
}
