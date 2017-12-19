using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;

namespace Telerik.WinControls
{

    public interface IExpression
    {
        object GetValue(RadObject forObject, RadProperty property);

        bool SetValue(RadObject radObject, RadProperty dp, object value);

        void OnDetach(RadObject radObject, RadProperty dp);

        void OnAttach(RadObject radObject, RadProperty dp);
    }

    /*public class Expression : IExpression
    {
        public object GetValue(RadObject forObject, RadProperty property)
        {
            return RadProperty.UnsetValue;
        }
    }*/

    /// <summary>
    /// Defines the usage of a given attached property.
    /// </summary>
	public enum AttachedPropertyUsage
	{
        /// <summary>
        /// 
        /// </summary>
		Self,
        /// <summary>
        /// 
        /// </summary>
		Children,
        /// <summary>
        /// 
        /// </summary>
		Descendants,
        /// <summary>
        /// 
        /// </summary>
		Anywhere
	}

	internal abstract class DeferredReference
	{
		// Methods
		protected DeferredReference() { }
		internal abstract object GetValue();
	}
    /// <summary>
    /// Represents the method that will be an alternative expression storage callback.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="dp"></param>
    /// <param name="metadata"></param>
    /// <returns></returns>
    public delegate IExpression AlternativeExpressionStorageCallback(RadObject d, RadProperty dp, RadPropertyMetadata metadata);
    /// <summary>
    /// Represents the method that will be a coerce value callback.
    /// </summary>
    /// <param name="d"></param>
    /// <param name="baseValue"></param>
    /// <returns></returns>
    public delegate object CoerceValueCallback(RadObject d, object baseValue);

	/// <summary>
    /// Represents the method that will be a property changed callback.
	/// </summary>
	/// <param name="d">
    /// 
    /// </param>
	/// <param name="e">
    /// Initializes the property change arguments.
    /// </param>
	public delegate void PropertyChangedCallback(RadObject d, RadPropertyChangedEventArgs e);

    /// <summary>
    /// Defines the source of current property value. See also
    /// %RadObject.GetValueSource:<br/>
    /// Telerik.WinControls.RadObject.GetValueSource%.
    /// </summary>
    public enum ValueSource : short
    {
        /// <summary>
        /// Indicates that the reason is unknown.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Indicates that the default value is set.
        /// </summary>
        DefaultValue,
        /// <summary>
        /// Indicates that the property changed is inherited.
        /// </summary>
        Inherited,
        /// <summary>
        /// An overriden default value, has higher priority than Default and Inherited source.
        /// </summary>
        DefaultValueOverride,
        /// <summary>
        /// Indicates that the reason for the property change is an applied theme.
        /// </summary>
        Style,
        /// <summary>
        /// Value is set locally through a CLR property setter.
        /// </summary>
        Local,
        /// <summary>
        /// Indicates that the reason for the property change is data binding.
        /// </summary>
        PropertyBinding,
        /// <summary>
        /// A value is applied through two-way binding.
        /// </summary>
        LocalFromBinding,
        /// <summary>
        /// Indicates that the reason for the property change is an animation effect.
        /// </summary>
        Animation,
    }

    /// <summary>
    /// Defines a mask enumeration which is used when updating rad properties' values.
    /// </summary>
    [Flags]
    public enum ValueResetFlags
    {
        None = 0,
        Inherited = 1,
        Binding = Inherited << 1,
        TwoWayBindingLocal = Binding << 1,
        Style = TwoWayBindingLocal << 1,
        Animation = Style << 1,
        Local = Animation << 1,
        DefaultValueOverride = Local << 1,
        All = Inherited | Binding | TwoWayBindingLocal | Style | Animation | Local | DefaultValueOverride
    }

    /// <summary>
    /// Defines the possible results for a property value update operation.
    /// </summary>
    public enum ValueUpdateResult
    {
        /// <summary>
        /// A composite value update is still running.
        /// </summary>
        Updating,
        /// <summary>
        /// There was no need of updating the property.
        /// </summary>
        NotUpdated,
        /// <summary>
        /// The property has been successfully updated and its value has changed.
        /// </summary>
        UpdatedChanged,
        /// <summary>
        /// The property has been successfully updated but its value has not changed.
        /// </summary>
        UpdatedNotChanged,
        /// <summary>
        /// Update operation was canceled.
        /// </summary>
        Canceled,
    }
}