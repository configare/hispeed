using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;

namespace Telerik.WinControls
{
    /// <summary>
    /// Stores all the information needed for composing a RadProperty's value for a given object.
    /// </summary>
    public class RadPropertyValue
    {
        #region Constructor

        internal RadPropertyValue(RadObject owner, RadProperty property)
        {
			this.owner = owner;
            this.property = property;
            //check which metadata to cache
            //there are two options:
            //1. Property is declared by owner's class hierarchy.
            //   In this case we retrieve the metadata for the owner itself.
            //2. Property is NOT declared by owner's class hierarchy.
            //   In this case if we request metadata for our owner, we will get the default one,
            //   so we need to reflect this by getting the metadata for the declaring type.
            bool metadataFound;
            this.metadata = property.GetMetadata(this.owner.RadObjectType, out metadataFound);
            if (!metadataFound)
            {
                RadObjectType declaringType = RadObjectType.FromSystemType(property.OwnerType);
                this.metadata = property.GetMetadata(declaringType);
            }
            this.valueSource = ValueSource.Unknown;
        }

        /// <summary>
        /// Internal constructor used to store existing property state.
        /// </summary>
        /// <param name="source"></param>
        internal RadPropertyValue(RadPropertyValue source)
        {
            Copy(source);
            this.lockComposeCount++;
            this.lockValueUpdateCount++;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Resets all references - such as binding references and property modifiers.
        /// </summary>
        internal void Dispose()
        {
            this.lockComposeCount++;
            this.lockValueUpdateCount++;
            //remove binding references
            this.owner.RemoveBinding(this);

            if (this.boundObjects != null)
            {
                int count = this.boundObjects.Count;
                for(int i = count - 1; i >= 0; i--)
                {
                    PropertyBoundObject reference = this.boundObjects[i];
                    RadObject boundObject = reference.Object;
                    if (boundObject != null &&
                        !(boundObject.IsDisposed || boundObject.IsDisposing))
                    {
                        boundObject.UnbindProperty(reference.Property);
                    }
                }
                this.boundObjects.Clear();
            }

            //reset references
            this.property = null;
            this.metadata = null;
            this.owner = null;
            this.propertyBinding = null;
            this.animationSetting = null;
            this.styleSetting = null;
        }

        /// <summary>
        /// Restores the state of this property using the provided source.
        /// </summary>
        /// <param name="source"></param>
        internal void Copy(RadPropertyValue source)
        {
            //copy all needed fields
            this.valueSource = source.valueSource;
            this.localValue = source.localValue;
            this.localValueFromBinding = source.localValueFromBinding;
            this.defaultValueOverride = source.defaultValueOverride;
            this.currentValue = source.currentValue;
            this.iscurrentValueCoerced = source.iscurrentValueCoerced;
            this.isSetAtDesignTime = source.isSetAtDesignTime;
            this.animationSetting = source.animationSetting;
            this.styleSetting = source.styleSetting;
            this.propertyBinding = source.propertyBinding;
            this.animatedValue = source.animatedValue;
        }

        /// <summary>
        /// Registers an object which is bound to this property value.
        /// </summary>
        /// <param name="relation"></param>
        internal void AddBoundObject(PropertyBoundObject relation)
        {
            if (boundObjects == null)
            {
                boundObjects = new List<PropertyBoundObject>();
            }

            int index = this.FindBoundObjectIndex(relation.Object);
            if(index != -1)
            {
                return;
            }

            //register relation
            boundObjects.Add(relation);
        }

        /// <summary>
        /// Gets the current value and optionally forces re-evaluation.
        /// </summary>
        /// <param name="composeIfNeeded"></param>
        /// <returns></returns>
        internal object GetCurrentValue(bool composeIfNeeded)
        {
            if (composeIfNeeded && this.valueSource == ValueSource.Unknown)
            {
                this.ComposeCurrentValue();
            }
            else if (this.currentValue == RadProperty.UnsetValue)
            {
                this.ComposeCurrentValue();
            }

            return this.currentValue;
        }

        /// <summary>
        /// Removes previously registered bound object.
        /// </summary>
        /// <param name="boundObject"></param>
        internal void RemoveBoundObject(RadObject boundObject)
        {
            int index = this.FindBoundObjectIndex(boundObject);
            if(index != -1)
            {
                this.boundObjects.RemoveAt(index);
            }
        }

        /// <summary>
        /// Notifies all bound objects for a change in this property.
        /// </summary>
        internal void NotifyBoundObjects()
        {
            //no bound objects registered
            if (this.boundObjects == null)
            {
                return;
            }

            //update all bound objects that our value has changed
            int count = this.boundObjects.Count;
            for(int i = count - 1; i >= 0; i--)
            {
                PropertyBoundObject relation = this.boundObjects[i];
                RadObject obj = relation.Object;
                if (obj != null)
                {
                    obj.OnBoundSourcePropertyChanged(relation.Property);
                }
                else
                {
                    this.boundObjects.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Forces value composition, using default precedence order.
        /// </summary>
        public void ComposeCurrentValue()
        {
            if (this.lockComposeCount > 0)
            {
                return;
            }

            ValueSource source = ValueSource.Unknown;
            object value = RadProperty.UnsetValue;
            //property composition flows like:
            //1. Animation
            //2. Property Binding
            //  2.1. Binding Local Value
            //  2.2. Property Binding member
            //3. Local Value
            //4. Style
            //5. Inherited value
            //6. Default value
            object animatedVal = this.animationSetting == null ? this.animatedValue : this.animationSetting.GetCurrentValue(this.owner);
            if(animatedVal != RadProperty.UnsetValue)
            {
                value = animatedVal;
                source = ValueSource.Animation;
            }
            else if (this.localValueFromBinding != RadProperty.UnsetValue)
            {
                //in a two-way binding we may have a special value treated as LocalFromBinding
                value = this.localValueFromBinding;
                source = ValueSource.LocalFromBinding;
            }
            else if (this.propertyBinding != null)
            {
                value = this.propertyBinding.GetValue();
                source = ValueSource.PropertyBinding;
            }
            else if (this.localValue != RadProperty.UnsetValue)
            {
                value = this.localValue;
                source = ValueSource.Local;
            }
            else if (this.styleSetting != null)
            {
                value = this.styleSetting.GetCurrentValue(this.owner);
                source = ValueSource.Style;
            }
            else if (this.defaultValueOverride != RadProperty.UnsetValue)
            {
                value = this.defaultValueOverride;
                source = ValueSource.DefaultValueOverride;
            }

            if (!IsValueValid(value))
            {
                this.SetInheritedOrDefaultValue(ref value, ref source);
            }

            this.SetCurrentValue(value, source);
        }

        internal void SetInheritedOrDefaultValue(ref object value, ref ValueSource source)
        {
            if (this.metadata.IsInherited)
            {
                if (!isInheritedValueValid)
                {
                    this.inheritedValue = this.owner.GetInheritedValue(this.property);
                    this.isInheritedValueValid = true;
                }
                value = this.inheritedValue;
                source = ValueSource.Inherited;
            }
            if (!IsValueValid(value))
            {
                value = this.GetDefaultValue();
                source = ValueSource.DefaultValue;
            }
        }

        internal bool IsValueValid(object value)
        {
            if (value == RadProperty.UnsetValue ||
                (value == null && this.property.PropertyType.IsValueType))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Resets the state of the inherited value.
        /// </summary>
        /// <returns>True if the property needs re-evaluation, false otherwise.</returns>
        internal bool InvalidateInheritedValue()
        {
            this.isInheritedValueValid = false;
            this.inheritedValue = RadProperty.UnsetValue;

            bool reEvaluate = false;
            if (this.valueSource == ValueSource.DefaultValue || this.valueSource == ValueSource.Inherited)
            {
                this.valueSource = ValueSource.Unknown;
                reEvaluate = true;
            }

            return reEvaluate;
        }

        /// <summary>
        /// Applies the specified value as local and forces current value re-evaluation.
        /// </summary>
        /// <param name="value"></param>
        internal void SetLocalValue(object value)
        {
            //if we have a two-way binding we need to force an update in the bound property also
            if (this.propertyBinding != null && 
                (this.propertyBinding.BindingOptions & PropertyBindingOptions.TwoWay) == PropertyBindingOptions.TwoWay)
            {
                this.propertyBinding.UpdateSourceProperty(value);
            }

            this.localValueFromBinding = RadProperty.UnsetValue;
            this.localValue = value;
            this.valueSource = ValueSource.Unknown;
        }

        /// <summary>
        /// Applies the specified value as local and forces current value re-evaluation.
        /// </summary>
        /// <param name="value"></param>
        internal void SetLocalValueFromBinding(object value)
        {
            //if we have a two-way binding we need to force an update in the bound property also
            if (this.propertyBinding != null &&
                (this.propertyBinding.BindingOptions & PropertyBindingOptions.TwoWay) == PropertyBindingOptions.TwoWay)
            {
                this.propertyBinding.UpdateSourceProperty(value);
            }

            this.localValueFromBinding = value;
            this.valueSource = ValueSource.Unknown;
        }

        /// <summary>
        /// Applies the specified animation and forces current value re-evaluation.
        /// </summary>
        /// <param name="animation"></param>
        internal void SetAnimation(AnimatedPropertySetting animation)
        {
            //check whether we need to keep the animated value
            if (animation == null)
            {
                if (this.animationSetting != null && !this.animationSetting.RemoveAfterApply)
                {
                    this.animatedValue = this.animationSetting.GetCurrentValue(this.owner);
                }
                else
                {
                    this.animatedValue = RadProperty.UnsetValue;
                }
            }
            this.animationSetting = animation;
            //clear current value source
            this.valueSource = ValueSource.Unknown;
        }

        /// <summary>
        /// Applies the specified style setting and forces current value re-evaluation.
        /// </summary>
        /// <param name="setting"></param>
        internal void SetStyle(IPropertySetting setting)
        {
            this.styleSetting = setting;
            this.valueSource = ValueSource.Unknown;
        }

        /// <summary>
        /// Applies the specified binding and forces current value re-evaluation.
        /// </summary>
        /// <param name="binding"></param>
        internal void SetBinding(PropertyBinding binding)
        {
            //check whether we need to remember the bindign value as local
            if (binding == null && this.propertyBinding != null)
            {
                if ((this.propertyBinding.BindingOptions & PropertyBindingOptions.PreserveAsLocalValue) == PropertyBindingOptions.PreserveAsLocalValue)
                {
                    this.localValue = this.propertyBinding.GetValue();
                }
            }

            this.propertyBinding = binding;
            this.valueSource = ValueSource.Unknown;
        }

        /// <summary>
        /// Determines whether the specified object is already bound.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        internal bool IsObjectBound(RadObject target)
        {
            return this.FindBoundObjectIndex(target) != -1;
        }

        /// <summary>
        /// Begins an update operation.
        /// </summary>
        /// <param name="lockCompose">Value composition will be locked.</param>
        /// <param name="lockValueUpdate">Specifies that we are currently applying new value.</param>
        internal void BeginUpdate(bool lockCompose, bool lockValueUpdate)
        {
            if (lockCompose)
            {
                this.lockComposeCount++;
            }
            if (lockValueUpdate)
            {
                this.lockValueUpdateCount++;
            }
        }

        internal void EndUpdate(bool unlockCompose, bool unlockValue)
        {
            if (unlockCompose && this.lockComposeCount > 0)
            {
                this.lockComposeCount--;
            }
            if (unlockValue && this.lockValueUpdateCount > 0)
            {
                this.lockValueUpdateCount--;
            }
        }

        /// <summary>
        /// Registers the provided value as a default for the property.
        /// </summary>
        /// <param name="value"></param>
        internal void SetDefaultValueOverride(object value)
        {
            this.defaultValueOverride = value;
            this.valueSource = ValueSource.Unknown;
        }

        #endregion

        #region Private Implementation

        private int FindBoundObjectIndex(RadObject boundObject)
        {
            if(this.boundObjects == null)
            {
                return -1;
            }

            int count = this.boundObjects.Count;
            for(int i = count - 1; i >= 0; i--)
            {
                PropertyBoundObject relation = this.boundObjects[i];
                RadObject obj = relation.Object;
                if (obj == null)
                {
                    this.boundObjects.RemoveAt(i);
                }
                else if(obj == boundObject)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Assigns the specified value and source as current.
        /// Internally checks for possible coersion.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="source"></param>
        private void SetCurrentValue(object value, ValueSource source)
        {
            this.iscurrentValueCoerced = false;
            //at last we check whether a coerce callback is registered
            object coercedValue = this.owner.CoerceValue(this, value);
            if (coercedValue != RadProperty.UnsetValue)
            {
                value = coercedValue;
                this.iscurrentValueCoerced = true;
            }

            this.currentValue = value;
            this.valueSource = source;
        }

        /// <summary>
        /// Retrieves the default value for the property.
        /// Custom value may be defined, using the DefaultValueCallback
        /// </summary>
        /// <returns></returns>
        private object GetDefaultValue()
        {
            object defaultValue = this.metadata.DefaultValue;
            //allow default value override
            object overridenValue = this.owner.GetDefaultValue(this, defaultValue);
            if (overridenValue != RadProperty.UnsetValue)
            {
                defaultValue = overridenValue;
            }

            return defaultValue;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether we have objects already bound to this property.
        /// </summary>
        public bool HasBoundObjects
        {
            get
            {
                return this.boundObjects != null && this.boundObjects.Count > 0;
            }
        }

        /// <summary>
        /// Determines whether current value composition is currently locked.
        /// </summary>
        public bool IsCompositionLocked
        {
            get
            {
                return this.lockComposeCount > 0;
            }
        }

        /// <summary>
        /// Determines whether we are in a process of updating a modifier.
        /// </summary>
        public bool IsUpdatingValue
        {
            get
            {
                return this.lockValueUpdateCount > 0;
            }
        }

        /// <summary>
        /// Gets the index of the associated RadProperty.
        /// </summary>
        public RadProperty Property
        {
            get
            {
                return this.property;
            }
        }

        /// <summary>
        /// Gets the current value for the property.
        /// </summary>
        public object CurrentValue
        {
            get
            {
                return this.currentValue;
            }
        }

        /// <summary>
        /// Gets the local value for this property.
        /// </summary>
        public object LocalValue
        {
            get
            {
                return this.localValue;
            }
        }

        /// <summary>
        /// Gets the value which is set through a two-way property binding.
        /// This value has higher priority that the local one.
        /// </summary>
        public object BindingLocalValue
        {
            get
            {
                return this.localValueFromBinding;
            }
        }

        /// <summary>
        /// Gets the property binding relation for this property.
        /// </summary>
        public PropertyBinding PropertyBinding
        {
            get
            {
                return this.propertyBinding;
            }
        }

        /// <summary>
        /// Gets the animation setting (if any) for this property.
        /// </summary>
        public AnimatedPropertySetting AnimationSetting
        {
            get
            {
                return this.animationSetting;
            }
        }

        /// <summary>
        /// Gets the current style setting for the property.
        /// </summary>
        public IPropertySetting StyleSetting
        {
            get
            {
                return this.styleSetting;
            }
        }

        /// <summary>
        /// Gets the current animated value.
        /// </summary>
        public object AnimatedValue
        {
            get
            {
                return this.animatedValue;
            }
        }

        /// <summary>
        /// Gets the source of the current value.
        /// </summary>
        public ValueSource ValueSource
        {
            get
            {
                return this.valueSource;
            }
        }

        /// <summary>
        /// Gets the Metadata associated with this property for the current owner.
        /// </summary>
        public RadPropertyMetadata Metadata
        {
            get
            {
                return this.metadata;
            }
        }

        /// <summary>
        /// The current value is forced to some custom value by a Coerce callback.
        /// </summary>
        public bool IsCurrentValueCoerced
        {
            get
            {
                return this.iscurrentValueCoerced;
            }
        }

        /// <summary>
        /// Gets the custom default value associated with this property.
        /// </summary>
        public object DefaultValueOverride
        {
            get
            {
                return this.defaultValueOverride;
            }
        }

        /// <summary>
        /// Determines whether the current local value (if any) is set at design-time.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSetAtDesignTime
        {
            get
            {
                return this.isSetAtDesignTime;
            }
            internal set
            {
                this.isSetAtDesignTime = value;
            }
        }

        #endregion

        #region Fields

        //incremental update counter
        private byte lockComposeCount;
        private byte lockValueUpdateCount;

        //references
        private RadProperty property;
        private RadObject owner;
        private RadPropertyMetadata metadata;

        //value modifiers
        private object localValue = RadProperty.UnsetValue;
        private object localValueFromBinding = RadProperty.UnsetValue;//special case for two-way binding
        private object inheritedValue = RadProperty.UnsetValue;
        private object defaultValueOverride = RadProperty.UnsetValue;
        private object animatedValue = RadProperty.UnsetValue;
        private AnimatedPropertySetting animationSetting;
        private IPropertySetting styleSetting;
        private PropertyBinding propertyBinding;

        //current settings
        private object currentValue = RadProperty.UnsetValue;
        private ValueSource valueSource;
        private bool iscurrentValueCoerced;
        private bool isInheritedValueValid;
        private bool isSetAtDesignTime;

        //list with all objects that are bound to this property value
        private List<PropertyBoundObject> boundObjects;

        #endregion
    }
}
