using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents an object which property is bound to some other object's property.
    /// Stores the object itself and its bound property. Used internally by the DPS to notify
    /// bound objects for a change in a binding source property.
    /// </summary>
    public class PropertyBoundObject
    {
        #region Constructor

        public PropertyBoundObject(RadObject boundObject, RadProperty boundProperty)
        {
            this.boundObject = new WeakReference(boundObject, false);
            this.boundProperty = boundProperty;
        }

        #endregion

        #region Methods

        public void UpdateValue()
        {
            RadObject obj = this.Object;
            if (obj != null)
            {
                obj.UpdateValue(this.boundProperty);
            }
        }

        #endregion

        #region Properties

        public RadProperty Property
        {
            get
            {
                return this.boundProperty;
            }
        }

        public RadObject Object
        {
            get
            {
                if (this.boundObject.IsAlive)
                {
                    RadObject obj = (RadObject)this.boundObject.Target;
                    if (obj.IsDisposed || obj.IsDisposing)
                    {
                        return null;
                    }

                    return obj;
                }

                return null;
            }
        }

        public bool IsObjectAlive
        {
            get
            {
                return this.boundObject.IsAlive;
            }
        }

        #endregion

        #region Fields

        private WeakReference boundObject;
        private RadProperty boundProperty;

        #endregion
    }
}
