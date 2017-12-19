using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a property key.   
    /// </summary>
    public sealed class RadPropertyKey
    {
    }
    /// <summary>
    /// Singleton.
    /// </summary>
    public sealed class DefaultValueOptions
    {
        // Methods
        static DefaultValueOptions()
        {
            DefaultValueOptions.CallCreateDefaultValue = new DefaultValueOptions();
        }

        private DefaultValueOptions()
        {
        }

        // Fields
        public static readonly object CallCreateDefaultValue;
    }

    /// <summary>
    /// Represents metadata for a RadProperty. RadPropertyMetadata describes the property. 
    /// For example, through DefaultValue property you can get or set the default value 
    /// for the property.
    /// </summary>
    public class RadPropertyMetadata
    {
        private int _flags;
        private object _defaultValue;
        private PropertyChangedCallback _propertyChangedCallback;

        private RadPropertyKey _readOnlyKey = null;
        private AttachedPropertyUsage _attachedPropertyUsage;

        /// <summary>Initializes a new instance of the RadPropertyMetadata class.</summary>
        public RadPropertyMetadata()
        {
        }

        /// <summary>Initializes a new instance of the RadPropertyMetadata class using the default value of the property.</summary>
        public RadPropertyMetadata(object defaultValue)
        {
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the RadPropertyMetadata class using a property 
        /// changed callback.
        /// </summary>
        public RadPropertyMetadata(PropertyChangedCallback propertyChangedCallback)
        {
            this.PropertyChangedCallback = propertyChangedCallback;
        }

        /// <summary>
        /// Initializes a new instance of the RadPropertyMetadata class using an object 
        /// and a property changed callback.
        /// </summary>
        public RadPropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback)
        {
            this.PropertyChangedCallback = propertyChangedCallback;
        }

        protected virtual RadPropertyMetadata CreateInstance()
        {
            return new RadPropertyMetadata();
        }

        /// <summary>Gets a value indicating whether the property is read-only.</summary>
        public bool ReadOnly
        {
            get
            {
                return false; //(this._readOnlyKey != null);
            }
        }

        internal bool DefaultValueWasSet()
        {
            return this.IsModified(1);
        }

        private CoerceValueCallback _coerceValueCallback;

        public CoerceValueCallback CoerceValueCallback
        {
            get
            {
                return this._coerceValueCallback;
            }
            set
            {
                if (this.Sealed)
                {
                    throw new InvalidOperationException(string.Format("TypeMetadataCannotChangeAfterUse", new object[0]));
                }
                this._coerceValueCallback = value;
                this.SetModified(8);
            }
        }

        internal bool IsDefaultValueModified
        {
            get
            {
                return this.IsModified(1);
            }
        }

        /// <summary>Gets or sets the default value of the property.</summary>
        public object DefaultValue
        {
            get
            {
                return this._defaultValue;
            }
            set
            {
                if (this.Sealed)
                {
                    throw new InvalidOperationException("TypeMetadataCannotChangeAfterUse");
                }
                if (value == RadProperty.UnsetValue)
                {
                    throw new ArgumentException("DefaultValueMayNotBeUnset");
                }
                /*Freezable freezable1 = value as Freezable;
                if (freezable1 != null)
                {
                    value = freezable1.GetAsFrozen();
                }*/
                this._defaultValue = value;
                this.SetModified(1);
            }
        }

        //Orriginally internal
        /// <summary>Gets or sets a value indicating whether the property is inherited.</summary>
        public bool IsInherited
        {
            get
            {
                return ((0x20 & this._flags) != 0);
            }
            set
            {
                if (value)
                {
                    this._flags |= 0x20;
                }
                else
                {
                    this._flags &= -33;
                }
            }
        }

        internal object GetDefaultValue(RadObject owner, RadProperty property)
        {
            if (this._defaultValue != DefaultValueOptions.CallCreateDefaultValue)
            {
                return this._defaultValue;
            }
            object obj1 = RadProperty.UnsetValue;//this.GetCachedDefaultValue(owner, property);
            if (obj1 == RadProperty.UnsetValue)
            {
                obj1 = this.CreateDefaultValue(owner, property);
                //property.ValidateDefaultValue(obj1);
                //this.SetCachedDefaultValue(owner, property, obj1);
            }

            return obj1;
        }

        private void SetModified(int id)
        {
            this._flags |= id;
        }

        private bool IsModified(int id)
        {
            return ((id & this._flags) != 0);
        }

        protected virtual object CreateDefaultValue(RadObject owner, RadProperty property)
        {
            throw new NotImplementedException("MissingCreateDefaultValue of property Metadata");
        }

        protected virtual void OnApply(RadProperty dp, Type targetType)
        {
        }

        internal void Seal(RadProperty dp, Type targetType)
        {
            this.OnApply(dp, targetType);
            this.Sealed = true;
        }

        internal bool Sealed
        {
            get
            {
                return this.ReadFlag(2);
            }
            set
            {
                this.WriteFlag(2, value);
            }
        }

        internal bool ReadFlag(int id)
        {
            return ((id & this._flags) != 0);
        }

        internal void WriteFlag(int id, bool value)
        {
            if (value)
            {
                this._flags |= id;
            }
            else
            {
                this._flags &= ~id;
            }
        }

        protected bool IsSealed
        {
            get
            {
                return this.Sealed;
            }
        }

        internal void SetAttachedPropertyUsage(AttachedPropertyUsage attachedPropertyUsage)
        {
            if (this.Sealed)
            {
                throw new InvalidOperationException(string.Format("TypeMetadataCannotChangeAfterUse", new object[0]));
            }
            this._attachedPropertyUsage = attachedPropertyUsage;
            this.SetModified(0x10);
        }
        /// <summary>
        /// Gets or sets the PropertyChangedCallback
        /// </summary>
        public PropertyChangedCallback PropertyChangedCallback
        {
            get
            {
                return this._propertyChangedCallback;
            }
            set
            {
                if (this.Sealed)
                {
                    throw new InvalidOperationException("Type metadata cannot change after it has been used");
                }
                this._propertyChangedCallback = value;
                this.SetModified(4);
            }
        }

        public AttachedPropertyUsage AttachedPropertyUsage
        {
            get
            {
                return this._attachedPropertyUsage;
            }
        }


        internal RadPropertyMetadata Copy(RadProperty dp)
        {
            RadPropertyMetadata metadata1 = this.CreateInstance();
            metadata1.InvokeMerge(this, dp);
            return metadata1;
        }

        internal void InvokeMerge(RadPropertyMetadata baseMetadata, RadProperty dp)
        {
            if (baseMetadata.ReadOnly)
            {
                this._readOnlyKey = baseMetadata._readOnlyKey;
            }
            this.Merge(baseMetadata, dp);
        }

        protected virtual void Merge(RadPropertyMetadata baseMetadata, RadProperty dp)
        {
            if (baseMetadata == null)
            {
                throw new ArgumentNullException("baseMetadata");
            }
            if (this.Sealed)
            {
                throw new InvalidOperationException(string.Format("TypeMetadataCannotChangeAfterUse", new object[0]));
            }
            if (!this.IsModified(1))
            {
                this._defaultValue = baseMetadata.DefaultValue;
            }
            if (!this.IsModified(0x10))
            {
                this._attachedPropertyUsage = baseMetadata.AttachedPropertyUsage;
            }
            if (baseMetadata.PropertyChangedCallback != null)
            {
                Delegate[] delegateArray1 = baseMetadata.PropertyChangedCallback.GetInvocationList();
                if (delegateArray1.Length > 0)
                {
                    PropertyChangedCallback callback1 = (PropertyChangedCallback)delegateArray1[0];
                    for (int num1 = 1; num1 < delegateArray1.Length; num1++)
                    {
                        callback1 = (PropertyChangedCallback)Delegate.Combine(callback1, (PropertyChangedCallback)delegateArray1[num1]);
                    }
                    callback1 = (PropertyChangedCallback)Delegate.Combine(callback1, this._propertyChangedCallback);
                    this._propertyChangedCallback = callback1;
                }
            }
            if (this._coerceValueCallback == null)
            {
                this._coerceValueCallback = baseMetadata.CoerceValueCallback;
            }
        }
    }

}
