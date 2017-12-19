using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a dependency between two properties.
    /// Used by a RadObject to bind a RadProperty to an external one and always use its value.
    /// The binding may be also two-way, in which special case the bound property updates its source.
    /// </summary>
    public class PropertyBinding
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the RadPropertyBinding class.
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="boundProperty"></param>
        /// <param name="sourceProperty"></param>
        /// <param name="options"></param>
        public PropertyBinding(RadObject sourceObject, RadProperty boundProperty, RadProperty sourceProperty, PropertyBindingOptions options)
        {
			this.sourceObject = new WeakReference(sourceObject);
            this.boundProperty = boundProperty;
			this.sourceProperty = sourceProperty;
			this.options = options;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            RadObject source = this.SourceObject;
            if (source != null)
            {
                return source.GetValue(this.sourceProperty);
            }

            return null;
        }

        /// <summary>
        /// Updates the binding source property.
        /// </summary>
        /// <param name="newValue"></param>
        public void UpdateSourceProperty(object newValue)
        {
            if ((this.options & PropertyBindingOptions.NoChangeNotify) == PropertyBindingOptions.NoChangeNotify)
            {
                return;
            }

            if ((this.options & PropertyBindingOptions.TwoWay) == PropertyBindingOptions.TwoWay)
            {
                RadObject source = this.SourceObject;
                if (source != null)
                {
                    source.OnTwoWayBoundPropertyChanged(this, newValue);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the binding source.
        /// </summary>
    	public RadObject SourceObject
    	{
            get
            {
                if (this.sourceObject.IsAlive)
                {
                    RadObject obj = (RadObject)this.sourceObject.Target;
                    if (obj.IsDisposed || obj.IsDisposing)
                    {
                        return null;
                    }

                    return obj;
                }

                return null;
            }
    	}

        public bool IsSourceObjectAlive
        {
            get
            {
                return this.sourceObject.IsAlive;
            }
        }

        public PropertyBindingOptions BindingOptions
        {
            get
            {
                return this.options;
            }
        }

        public RadProperty BoundProperty
        {
            get
            {
                return this.boundProperty;
            }
        }

        public RadProperty SourceProperty
        {
            get
            {
                return this.sourceProperty;
            }
        }

        #endregion

        #region Fields

        private RadProperty boundProperty;
        private RadProperty sourceProperty;
        private WeakReference sourceObject;
        private PropertyBindingOptions options;

        #endregion
    }
}
