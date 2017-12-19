using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Telerik.WinControls.Design;

namespace Telerik.WinControls
{
    /// <summary>
    /// Base for all TPF classes. Implements WPF-like property system with different value sources.
    /// Provides public interface for getting, setting value or re-setting property value.
    /// </summary>
    public class RadObject : DisposableObject, INotifyPropertyChanged, ICustomTypeDescriptor
    {
        #region Constructor

        public RadObject()
        {
            this.propertyValues = new RadPropertyValueCollection(this);
            this.radType = RadObjectType.FromSystemType(this.GetType());
        }
        static RadObject()
        {
            RadType = RadObjectType.FromSystemType(typeof(RadObject));
            PropertyChangedEventKey = new object();
            RadPropertyChangedEventKey = new object();
            RadPropertyChangingEventKey = new object();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property of an object changes.
        /// Note: if a property which is not a RadProperty changes,
        /// the developer is responsible for firing this event by using the
        /// <see cref="INotifyPropertyChanged"/> API.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                this.Events.AddHandler(PropertyChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PropertyChangedEventKey, value);
            }
        }

        /// <summary>
        /// Occurs when a property of a RadObject changes.		
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event RadPropertyChangedEventHandler RadPropertyChanged
        {
            add
            {
                this.Events.AddHandler(RadPropertyChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadPropertyChangedEventKey, value);
            }
        }


        /// <summary>
        /// Occurs prior to property of a RadObject changes.        
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event RadPropertyChangingEventHandler RadPropertyChanging
        {
            add
            {
                this.Events.AddHandler(RadPropertyChangingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadPropertyChangingEventKey, value);
            }
        }

        #endregion

        #region Interfaces

        #region ICustomTypeDescriptor

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return null;
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return null;
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        /// <summary>
        /// Replaces the default property descriptors of properties of the object in order to perform Rad-Object specific 
        /// tasks like checking ShouldSerialize and RadProperty-DefaultValue...
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        internal virtual PropertyDescriptorCollection ReplaceDefaultDescriptors(PropertyDescriptorCollection props)
        {
            List<PropertyDescriptor> newProps = new List<PropertyDescriptor>(props.Count);

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor propDesc = props[i];

                ////Skip name property, as the designer adds it an extended property Name to each component
                ////RadItemDesigner takes care for the design time property to update and serialize the value.
                //if (propDesc.Name == "Name" && propDesc.ComponentType == typeof(RadElement))
                //{
                //    continue;
                //}

                ////WinForms designer by default extendts each component with properties like: Name, Modifier,
                ////ApplicationSettings, etc. If not skipped here, these proeprties will appear 2 times in the designer's
                ////property grid
                //if (propDesc.GetType().FullName == "System.ComponentModel.ExtendedPropertyDescriptor")
                //{
                //    newProps.Add(propDesc);
                //    continue;
                //}

                //RadObjectCustomRadPropertyDescriptor for RadProperties and 
                //RadObjectCustomPropertyDescriptor for ordinary properties

                Attribute[] attr = new Attribute[propDesc.Attributes.Count];
                propDesc.Attributes.CopyTo(attr, 0);

                RadProperty property = RadTypeResolver.Instance.GetRegisteredRadProperty(this.GetType(), propDesc.Name);
                PropertyDescriptor newDesc;
                if (property != null)
                {
                    newDesc = new RadObjectCustomRadPropertyDescriptor(propDesc, attr);
                }
                else
                {
                    newDesc = new RadObjectCustomPropertyDescriptor(propDesc, attr);
                }

                //check whether we have a property filter applied
                if (this.propertyFilter != null && !this.propertyFilter.Match(propDesc))
                {
                    continue;
                }

                newProps.Add(newDesc);
            }

            PropertyDescriptorCollection res = new PropertyDescriptorCollection((PropertyDescriptor[])(newProps.ToArray()));

            return res;
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this, true);
            return this.ReplaceDefaultDescriptors(props);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this, attributes, true);
            return this.ReplaceDefaultDescriptors(props);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// 
        /// </summary>
        protected override void DisposeManagedResources()
        {
            this.ClearPropertyStore();
            base.DisposeManagedResources();
        }

        /// <summary>
        /// Removes all references to external property modifiers such as 
        /// property bindings, style settings and animations.
        /// </summary>
        protected virtual void ClearPropertyStore()
        {
            this.propertyValues.Reset();
        }

        #endregion

        #endregion

        #region Property System

        #region General

        /// <summary>
        /// Allows PropertyChanging and PropertyChanged notifications to be temporary suspended.
        /// </summary>
        public void SuspendPropertyNotifications()
        {
            this.suspendPropertyNotifications++;
        }

        /// <summary>
        /// Resumes property notifications after a previous <see cref="RadObject.SuspendPropertyNotifications">SuspendPropertyNotifications</see> call.
        /// </summary>
        public void ResumePropertyNotifications()
        {
            if (this.suspendPropertyNotifications > 0)
            {
                this.suspendPropertyNotifications--;
            }
        }

        /// <summary>
        /// Gets the RadPropertyValue structure that holds information
        /// about the specified property's effective value for this instance.
        /// May be null if no effective value is recorded.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public RadPropertyValue GetPropertyValue(RadProperty property)
        {
            return this.propertyValues.GetEntry(property, false);
        }

        /// <summary>
        /// Applies the provided value as an override
        /// of the Default value provided by the specified property's metadata.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ValueUpdateResult SetDefaultValueOverride(RadProperty property, object value)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(property, true);
            return this.SetValueCore(propVal, null, value, ValueSource.DefaultValueOverride);
        }

        /// <summary>
        /// Marks the current PropertyValue entry for the specified property as "Set at design-time".
        /// This is used by our custom code-dom serializer to determine which properties needs to be persisted.
        /// </summary>
        /// <param name="property"></param>
        internal void SetPropertyValueSetAtDesignTime(RadProperty property)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(property, false);
            if (propVal != null)
            {
                propVal.IsSetAtDesignTime = true;
            }
        }

        /// <summary>
        /// Applies the specified value as Local for the desired property
        /// and raises the flag IsLocalValueSetAtDesignTime for that property.
        /// All design-time direct property modifications (e.g. item.Text = "Item1")
        /// should be done through this method for the property to be properly serialized.
        /// If a property is modified through a property grid, the custom property descriptor will automatically apply this logic.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        internal void SetValueAtDesignTime(RadProperty property, object value)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(property, true);
            propVal.IsSetAtDesignTime = true;
            this.SetValueCore(propVal, null, value, ValueSource.Local);
        }

        /// <summary>
        /// Retrieves the current value for the specified property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public object GetValue(RadProperty property)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(property, true);
            return propVal.GetCurrentValue(true);
        }

        /// <summary>
        /// Applies the provided value as Local for the specified property.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns>The result of the operation.</returns>
        public ValueUpdateResult SetValue(RadProperty property, object value)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(property, true);
            return this.SetValueCore(propVal, null, value, ValueSource.Local);
        }

        /// <summary>
        /// Resets the current value of the specified property.
        /// This method will remove any effective value modifier
        /// (such as style or animation setting) for the specified property.
        /// </summary>
        /// <param name="property">The RadProperty that should be reset.</param>
        /// <returns>The result of the operation.</returns>
        public ValueUpdateResult ResetValue(RadProperty property)
        {
            return this.ResetValue(property, ValueResetFlags.All);
        }

        /// <summary>
        /// Resets the current value of the specified property using the provided flags.
        /// </summary>
        /// <param name="property">The RadProperty that should be reset.</param>
        /// <param name="flags">Additional flags that specify which effective modifiers should be reset.</param>
        /// <returns>The result of the operation.</returns>
        public ValueUpdateResult ResetValue(RadProperty property, ValueResetFlags flags)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(property, false);
            if (propVal != null)
            {
                return this.ResetValueCore(propVal, flags);
            }

            return ValueUpdateResult.NotUpdated;
        }

        /// <summary>
        /// Forces re-evaluation of the current value for the specified property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns>The result of the operation.</returns>
        public ValueUpdateResult UpdateValue(RadProperty property)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(property, false);
            if (propVal == null)
            {
                return ValueUpdateResult.NotUpdated;
            }

            return UpdateValueCore(propVal);
        }

        /// <summary>
        /// Gets the source of the current value for the specified property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public ValueSource GetValueSource(RadProperty property)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(property, true);
            if (propVal.ValueSource == ValueSource.Unknown)
            {
                propVal.ComposeCurrentValue();
            }

            return propVal.ValueSource;
        }

        /// <summary>
        /// Gets the registered property with the specified name.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public RadProperty GetRegisteredRadProperty(string propertyName)
        {
            return RadTypeResolver.Instance.GetRegisteredRadProperty(GetType(), propertyName);
        }

        /// <summary>
        /// Performs the core value update logic.
        /// </summary>
        /// <param name="propVal"></param>
        /// <returns>The result of the operation.</returns>
        protected internal virtual ValueUpdateResult UpdateValueCore(RadPropertyValue propVal)
        {
            object oldValue = propVal.GetCurrentValue(false);
            ValueSource oldSource = propVal.ValueSource;
            //force re-composition of property's current value
            propVal.ComposeCurrentValue();

            return this.RaisePropertyNotifications(propVal, oldValue, propVal.GetCurrentValue(false), oldSource);
        }

        /// <summary>
        /// Performs the core logic of updating property value.
        /// </summary>
        /// <param name="propVal">The property value structure, holding property information.</param>
        /// <param name="propModifier">Additional modifier, like IPropertySetting</param>
        /// <param name="newValue">The actual new value to be set, valid for Local and DefaultValue sources.</param>
        /// <param name="source">Specifies the source of the provided new value.</param>
        /// <returns>The result of the operation.</returns>
        protected virtual ValueUpdateResult SetValueCore(RadPropertyValue propVal, object propModifier, object newValue, ValueSource source)
        {
            //do not set anything while disposing
            if (this.GetBitState(DisposingStateKey) || this.GetBitState(DisposedStateKey))
            {
                return ValueUpdateResult.Canceled;
            }

            object oldValue = propVal.GetCurrentValue(false);
            ValueSource oldSource = propVal.ValueSource;

            RadPropertyValue oldPropState = null;
            bool updatingValue = propVal.IsUpdatingValue;
            if (!propVal.IsCompositionLocked && this.IsPropertyCancelable(propVal.Metadata))
            {
                //create a copy of the current property value
                //so that we may restore it later if property change is not accepted
                oldPropState = new RadPropertyValue(propVal);
            }
            //perform value update
            propVal.BeginUpdate(true, true);

            switch (source)
            {
                case ValueSource.Animation:
                    if (!updatingValue)
                    {
                        this.RemoveAnimation(propVal);
                    }
                    propVal.SetAnimation((AnimatedPropertySetting)propModifier);
                    break;
                case ValueSource.DefaultValue:
                case ValueSource.DefaultValueOverride:
                    propVal.SetDefaultValueOverride(newValue);
                    break;
                case ValueSource.Local:
                    propVal.SetLocalValue(newValue);
                    break;
                case ValueSource.LocalFromBinding:
                    propVal.SetLocalValueFromBinding(newValue);
                    break;
                case ValueSource.PropertyBinding:
                    if (!updatingValue)
                    {
                        RemoveBinding(propVal);
                    }
                    propVal.SetBinding((PropertyBinding)propModifier);
                    break;
                case ValueSource.Style:
                    if (!updatingValue)
                    {
                        this.RemoveAnimation(propVal);
                    }
                    propVal.SetStyle((IPropertySetting)propModifier);
                    break;
                case ValueSource.Inherited:
                    propVal.InvalidateInheritedValue();
                    break;
                default:
                    Debug.Assert(false, "Invalid value source");
                    break;
            }

            propVal.EndUpdate(true, true);
            //are we still in a process of updating?
            if (propVal.IsCompositionLocked)
            {
                return ValueUpdateResult.Updating;
            }

            ValueUpdateResult result = this.RaisePropertyNotifications(propVal, oldValue, propVal.GetCurrentValue(true), oldSource);
            if (result == ValueUpdateResult.Canceled && oldPropState != null)
            {
                //restore previous state as operation was canceled
                propVal.Copy(oldPropState);
            }

            return result;
        }

        /// <summary>
        /// Resets the specified property value, using the provided reset flags.
        /// </summary>
        /// <param name="propVal"></param>
        /// <param name="flags"></param>
        /// <returns>The result of the operation.</returns>
        protected internal virtual ValueUpdateResult ResetValueCore(RadPropertyValue propVal, ValueResetFlags flags)
        {
            //no flags are specified
            if (flags == ValueResetFlags.None)
            {
                return ValueUpdateResult.NotUpdated;
            }

            object oldValue = propVal.GetCurrentValue(false);
            ValueSource oldSource = propVal.ValueSource;

            RadPropertyValue oldState = null;
            //check whether we are in nested update block
            if (!propVal.IsCompositionLocked)
            {
                //create a copy of the current property value
                oldState = new RadPropertyValue(propVal);
            }
            propVal.BeginUpdate(true, false);

            //update property value as specified by the provided flags
            if ((flags & ValueResetFlags.Animation) == ValueResetFlags.Animation)
            {
                this.SetValueCore(propVal, null, null, ValueSource.Animation);
            }
            if ((flags & ValueResetFlags.Local) == ValueResetFlags.Local)
            {
                this.SetValueCore(propVal, null, RadProperty.UnsetValue, ValueSource.Local);
            }
            if ((flags & ValueResetFlags.DefaultValueOverride) == ValueResetFlags.DefaultValueOverride)
            {
                this.SetValueCore(propVal, null, RadProperty.UnsetValue, ValueSource.DefaultValue);
            }
            if ((flags & ValueResetFlags.Binding) == ValueResetFlags.Binding)
            {
                this.SetValueCore(propVal, null, null, ValueSource.PropertyBinding);
            }
            if ((flags & ValueResetFlags.TwoWayBindingLocal) == ValueResetFlags.TwoWayBindingLocal)
            {
                this.SetValueCore(propVal, null, RadProperty.UnsetValue, ValueSource.LocalFromBinding);
            }
            if ((flags & ValueResetFlags.Style) == ValueResetFlags.Style)
            {
                this.SetValueCore(propVal, null, null, ValueSource.Style);
            }
            if ((flags & ValueResetFlags.Inherited) == ValueResetFlags.Inherited)
            {
                this.SetValueCore(propVal, null, RadProperty.UnsetValue, ValueSource.Inherited);
            }

            propVal.EndUpdate(true, false);
            if (propVal.IsCompositionLocked)
            {
                return ValueUpdateResult.Updating;
            }

            ValueUpdateResult result = this.RaisePropertyNotifications(propVal, oldValue, propVal.GetCurrentValue(true), oldSource);
            if (result == ValueUpdateResult.Canceled)
            {
                //restore previous state as operation was canceled
                propVal.Copy(oldState);
            }

            return result;
        }

        /// <summary>
        /// Allows inheritors to provide custom default value.
        /// </summary>
        /// <param name="propVal"></param>
        /// <param name="baseDefaultValue"></param>
        /// <returns></returns>
        protected internal virtual object GetDefaultValue(RadPropertyValue propVal, object baseDefaultValue)
        {
            return RadProperty.UnsetValue;
        }

        /// <summary>
        /// Allows inheritors to force a coersion of the current calculated value for the given property.
        /// </summary>
        /// <param name="propVal">The property value.</param>
        /// <param name="baseValue">The current caluclated value of the property.</param>
        /// <returns>Null if no coersion is needed.</returns>
        protected internal virtual object CoerceValue(RadPropertyValue propVal, object baseValue)
        {
            return RadProperty.UnsetValue;
        }

        /// <summary>
        /// Determines whether the property defined by the provided property descriptor should be serialized.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        protected internal virtual bool? ShouldSerializeProperty(PropertyDescriptor property)
        {
            return null;
        }

        /// <summary>
        /// Checks needed conditions to perform property update.
        /// </summary>
        /// <param name="propVal"></param>
        /// <param name="value"></param>
        internal void EnsurePropertySet(RadPropertyValue propVal, object value)
        {
            //1. It should not be marked as "Read-only"
            if (propVal.Metadata.ReadOnly)
            {
                throw new ArgumentException("Attemt to modify the value of a read-only property");
            }

            //2. Type of the specified value should match property type
            if (value != null && value != RadProperty.UnsetValue)
            {
                if (!RadProperty.IsValidType(value, propVal.Property.PropertyType))
                {
                    throw new ArgumentException("New value does not match declared property type.");
                }
            }

            //3. Is value valid - use metadata's defined callback
            if (propVal.Property.ValidateValueCallback != null)
            {
                if (!propVal.Property.ValidateValueCallback(value, this))
                {
                    throw new ArgumentException("Specified value " + value.ToString() + " is not valid for property " + propVal.Property.Name);
                }
            }
        }

        /// <summary>
        /// Performs the following logic:
        /// 1. Compares oldValue and newValue and returns ValueUpdateResult.NotChanged if they are equal.
        /// 2. Raises the PropertyChanging notification. If the event is canceled returns ValueUpdateResult.Canceled.
        /// 3. Raises PropertyChanged notification and returns ValueUpdateResult.Updated.
        /// </summary>
        /// <param name="propVal"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="oldSource"></param>
        /// <returns>The result of the operation.</returns>
        internal ValueUpdateResult RaisePropertyNotifications(RadPropertyValue propVal, object oldValue, object newValue, ValueSource oldSource)
        {
            if (!this.CanRaisePropertyChangeNotifications(propVal))
            {
                return ValueUpdateResult.NotUpdated;
            }

            //in some cases we may run into composite property update and property notifications will not be needed.
            if (propVal.IsCompositionLocked)
            {
                return ValueUpdateResult.Updating;
            }

            //compare the new value of the property with its previous one
            if (object.Equals(oldValue, newValue))
            {
                //current property value has not changed, do not raise notifications
                return ValueUpdateResult.UpdatedNotChanged;
            }

            RadPropertyChangingEventArgs changingArgs = new RadPropertyChangingEventArgs(propVal.Property, oldValue, newValue, propVal.Metadata);
            this.OnPropertyChanging(changingArgs);

            ValueUpdateResult result;
            if (changingArgs.Cancel)
            {
                result = ValueUpdateResult.Canceled;
            }
            else
            {
#if DEBUG
                //Validate needed conditions to update the property
                //TODO: This is currently performed during debug cycle, may be it should be done in release also
                EnsurePropertySet(propVal, newValue);
#endif
                //change is accepted, fire changed notification
                RadPropertyChangedEventArgs changedArgs = new RadPropertyChangedEventArgs(propVal.Property, propVal.Metadata, oldValue, newValue, false, false, oldSource, propVal.ValueSource);
                this.OnPropertyChanged(changedArgs);

                //notify all objects bound to the property we have just updated
                propVal.NotifyBoundObjects();

                //use the metadata callback if specified
                if (propVal.Metadata.PropertyChangedCallback != null)
                {
                    propVal.Metadata.PropertyChangedCallback(this, changedArgs);
                }

                //value was successfully updated
                result = ValueUpdateResult.UpdatedChanged;
            }

            return result;
        }

        internal RadPropertyValueCollection PropertyValues
        {
            get
            {
                return this.propertyValues;
            }
        }

        /// <summary>
        /// Determines whether the object can raise PropertyChanging and PropertyChanged notifications.
        /// Current implementation checks whether the object is disposing or is already disposed of.
        /// </summary>
        /// <param name="propVal"></param>
        /// <returns></returns>
        protected virtual bool CanRaisePropertyChangeNotifications(RadPropertyValue propVal)
        {
            if (this.suspendPropertyNotifications > 0)
            {
                return false;
            }

            if (this.GetBitState(DisposingStateKey) || this.GetBitState(DisposedStateKey))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Animation

        internal void RemoveAnimation(RadPropertyValue propVal)
        {
            AnimatedPropertySetting animation = this.GetCurrentAnimation(propVal);
            if (animation != null)
            {
                animation.RemovePreviousAnimation((RadElement)this, animation);
            }
        }

        /// <summary>
        /// Gets the animation (if any) attached to the current property.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        internal AnimatedPropertySetting GetCurrentAnimation(RadProperty property)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(property, false);
            if (propVal == null)
            {
                return null;
            }

            return this.GetCurrentAnimation(propVal);
        }

        internal AnimatedPropertySetting GetCurrentAnimation(RadPropertyValue propVal)
        {
            //since an AnimatedPropertySetting may be plugged either as a Style
            //or as a regular Animation, we need to check both Style and Animation fields
            if (propVal.AnimationSetting != null)
            {
                return propVal.AnimationSetting;
            }

            return propVal.StyleSetting as AnimatedPropertySetting;
        }

        /// <summary>
        /// Gets notified for a change in an animated property.
        /// </summary>
        /// <param name="setting">The property which is currently animated.</param>
        internal ValueUpdateResult OnAnimatedPropertyValueChanged(AnimatedPropertySetting setting)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(setting.Property, false);
            if (propVal == null)
            {
                return ValueUpdateResult.NotUpdated;
            }
            return this.UpdateValueCore(propVal);
        }

        internal ValueUpdateResult OnAnimationStarted(AnimatedPropertySetting setting)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(setting.Property, true);
            if (propVal == null)
            {
                return ValueUpdateResult.NotUpdated;
            }
            //register the setting with the specified property
            //the setting may have two meanings:
            //1. If it comes from a StyleSheet it will be treated as a Style setting
            //2. Otherwise it will be treated as Animation and will have highest precendence
            //   when composing associated property's value.
            ValueSource source = setting.IsStyleSetting ? ValueSource.Style : ValueSource.Animation;
            return this.SetValueCore(propVal, setting, null, source);
        }

        internal ValueUpdateResult OnAnimationFinished(AnimatedPropertySetting setting)
        {
            //check whether we have registered animation setting for this element
            RadPropertyValue propVal = this.propertyValues.GetEntry(setting.Property, false);
            if (propVal == null)
            {
                return ValueUpdateResult.NotUpdated;
            }
            //check whether this is StyleSetting or regular Animation
            object propModifier;
            ValueSource source;
            if (setting.IsStyleSetting)
            {
                propModifier = setting;
                source = ValueSource.Style;
            }
            else
            {
                propModifier = null;
                source = ValueSource.Animation;
            }

            return this.SetValueCore(propVal, propModifier, null, source);
        }

        #endregion

        #region Binding

        /// <summary>
        /// Binds the specified property to a property of the provided binding source object.
        /// </summary>
        /// <param name="propertyToBind">Our property that is about to be bound.</param>
        /// <param name="sourceObject">The object to which source property belongs.</param>
        /// <param name="sourceProperty">The property to which we will bind.</param>
        /// <param name="options">Additional options, specifying the binding operation.</param>
        public ValueUpdateResult BindProperty(RadProperty propertyToBind, RadObject sourceObject,
                                              RadProperty sourceProperty, PropertyBindingOptions options)
        {
            if (sourceObject == null)
            {
                throw new ArgumentNullException("Binding source object");
            }
            if (sourceObject.IsDisposing || sourceObject.IsDisposed)
            {
                return ValueUpdateResult.NotUpdated;
                //throw new ObjectDisposedException("Binding source object");
            }

            RadPropertyValue propVal = this.propertyValues.GetEntry(propertyToBind, true);
            //remove previous binding (if any)
            if (propVal.PropertyBinding != null)
            {
                //lock subsequent value updates
                propVal.BeginUpdate(true, false);
                this.ResetValueCore(propVal, ValueResetFlags.Binding);
                propVal.EndUpdate(true, false);
            }

            //create a new binding
            PropertyBinding binding = new PropertyBinding(sourceObject, propertyToBind, sourceProperty, options);
            //apply binding
            ValueUpdateResult result = this.SetValueCore(propVal, binding, null, ValueSource.PropertyBinding);

            if ((options & PropertyBindingOptions.NoChangeNotify) == 0)
            {
                //register ourselves as bound for source's property
                sourceObject.OnPropertyBoundExternally(binding, this);
            }

            return result;
        }

        /// <summary>
        /// Removes the binding for the specified property.
        /// </summary>
        /// <param name="boundProperty"></param>
        /// <returns>The result of the operation.</returns>
        public ValueUpdateResult UnbindProperty(RadProperty boundProperty)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(boundProperty, false);
            if (propVal == null || propVal.PropertyBinding == null)
            {
                return ValueUpdateResult.NotUpdated;
            }

            //reset binding
            return this.ResetValueCore(propVal, ValueResetFlags.Binding);
        }

        /// <summary>
        /// Gets notified that the specified object has bound to a property of ours.
        /// </summary>
        /// <param name="boundObject">The instance that has bound the specified property.</param>
        /// <param name="binding"></param>
        internal void OnPropertyBoundExternally(PropertyBinding binding, RadObject boundObject)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(binding.SourceProperty, true);
            //register the bound object and its property
            propVal.AddBoundObject(new PropertyBoundObject(boundObject, binding.BoundProperty));
        }

        /// <summary>
        /// Gets notified that the specified object has unbound itself from a property of ours.
        /// </summary>
        /// <param name="boundObject"></param>
        /// <param name="binding"></param>
        internal void OnPropertyUnboundExternally(PropertyBinding binding, RadObject boundObject)
        {
            if (this.IsDisposing)
            {
                return;
            }

            RadPropertyValue propVal = this.propertyValues.GetEntry(binding.SourceProperty, false);
            if (propVal == null)
            {
                return;
            }

            propVal.BeginUpdate(true, false);

            //remove previously registered relations
            propVal.RemoveBoundObject(boundObject);

            if ((binding.BindingOptions & PropertyBindingOptions.TwoWay) == PropertyBindingOptions.TwoWay)
            {
                //reset the local value applied from the two-way binding
                this.ResetValueCore(propVal, ValueResetFlags.TwoWayBindingLocal);
            }

            if ((binding.BindingOptions & PropertyBindingOptions.PreserveAsLocalValue) == PropertyBindingOptions.PreserveAsLocalValue)
            {
                propVal.SetLocalValue(binding.GetValue());
            }

            propVal.EndUpdate(true, false);
            this.UpdateValueCore(propVal);
        }

        /// <summary>
        /// Notifies a binding source that a change occured in a two-way bound property.
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="newValue"></param>
        internal void OnTwoWayBoundPropertyChanged(PropertyBinding binding, object newValue)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(binding.SourceProperty, false);
            if (propVal != null)
            {
                this.SetValueCore(propVal, null, newValue, ValueSource.LocalFromBinding);
            }
        }

        /// <summary>
        /// Gets notified for a change in an already bound external property.
        /// </summary>
        /// <param name="boundProperty"></param>
        internal void OnBoundSourcePropertyChanged(RadProperty boundProperty)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(boundProperty, false);
            if (propVal == null || propVal.IsCompositionLocked)
            {
                return;
            }
            //force current value update for the bound property
            this.UpdateValueCore(propVal);
        }

        /// <summary>
        /// Detaches binding reference from the binding source.
        /// </summary>
        /// <param name="propVal"></param>
        internal void RemoveBinding(RadPropertyValue propVal)
        {
            PropertyBinding binding = propVal.PropertyBinding;
            if (binding != null)
            {
                //we need to reset current binding (unregister from source object's bound objects list)
                RadObject bindingSource = binding.SourceObject;
                if (bindingSource != null &&
                    ((binding.BindingOptions & PropertyBindingOptions.NoChangeNotify) == 0))
                {
                    bindingSource.OnPropertyUnboundExternally(binding, this);
                }
            }
        }

        #endregion

        #region Styling

        /// <summary>
        /// Registers a style setting for this instance.
        /// </summary>
        /// <param name="setting"></param>
        internal ValueUpdateResult AddStylePropertySetting(IPropertySetting setting)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(setting.Property, true);
            return this.SetValueCore(propVal, setting, null, ValueSource.Style);
        }

        internal virtual void RemoveStylePropertySetting(IPropertySetting setting)
        {
            this.RemoveStylePropertySetting(setting.Property);
        }

        /// <summary>
        /// Called when element style condition changes
        /// </summary>
        /// <param name="property"></param>
        internal void RemoveStylePropertySetting(RadProperty property)
        {
            RadPropertyValue propVal = this.propertyValues.GetEntry(property, false);
            if (propVal != null)
            {
                this.ResetValueCore(propVal, ValueResetFlags.Style);
            }
        }

        #endregion

        #region Inheritance

        /// <summary>
        /// Searches up in the chain of InheritanceParents for a value for the specified property.
        /// </summary>
        /// <param name="property">The property to examine.</param>
        /// <returns></returns>
        protected internal virtual object GetInheritedValue(RadProperty property)
        {
            if (this.GetBitState(DisposingStateKey) || this.GetBitState(DisposedStateKey))
            {
                return RadProperty.UnsetValue;
            }

            int propertyIndex = property.GlobalIndex;
            Type propDeclaringType = property.OwnerType;

            object value = RadProperty.UnsetValue;
            RadObject parent = this.InheritanceParent;

            while (parent != null)
            {
                if (propDeclaringType.IsInstanceOfType(parent))
                {
                    RadPropertyValue propVal = parent.propertyValues.GetEntry(property, false);
                    if (propVal != null)
                    {
                        value = propVal.GetCurrentValue(true);
                        break;
                    }
                }

                parent = parent.InheritanceParent;
            }

            return value;
        }

        /// <summary>
        /// Gets the RadObject which is treated as the parent from which inheritable properties are composed.
        /// </summary>
        internal virtual RadObject InheritanceParent
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Notifications

        /// <summary>
        /// Raises the <see cref="RadPropertyChanging"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnPropertyChanging(RadPropertyChangingEventArgs args)
        {
            RadPropertyChangingEventHandler handler = this.Events[RadPropertyChangingEventKey] as RadPropertyChangingEventHandler;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            RadPropertyChangedEventHandler handler = this.Events[RadPropertyChangedEventKey] as RadPropertyChangedEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }

            //raise the standard .NET PropertyChanged event
            this.OnNotifyPropertyChanged(new PropertyChangedEventArgs(e.Property.Name));
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected virtual void OnNotifyPropertyChanged(string propertyName)
        {
            this.OnNotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the standard .NET PropertyChanged event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = this.Events[PropertyChangedEventKey] as PropertyChangedEventHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #endregion

        #region Protected Properties

        /// <summary>
        /// Determines whether the element is in design mode.
        /// </summary>
        protected internal virtual bool IsDesignMode
        {
            get
            {
                return this.GetBitState(IsDesignModeStateKey);
            }
            set
            {
                this.SetBitState(IsDesignModeStateKey, value);
            }
        }

        /// <summary>
        /// Determines whether the specified property may be canceled.
        /// </summary>
        /// <param name="metadata">The metadata associated with the property change.</param>
        protected internal virtual bool IsPropertyCancelable(RadPropertyMetadata metadata)
        {
            return false;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a <see cref="Filter">Filter</see> instance, used to filter the ICustomPropertyDescriptor.GetProperties collection.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Filter PropertyFilter
        {
            get
            {
                return this.propertyFilter;
            }
            set
            {
                this.propertyFilter = value;
            }
        }

        /// <summary>
        /// Gets the RadObjectType which is associated with this system type.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadObjectType RadObjectType
        {
            get
            {
                return this.radType;
            }
        }

        // Partial implementation of the IBindableComponent. We define the BindingContext here
        // as often in WinForms the binding context is defined on a per Form basis and is consistent
        // over the multiple controls hosted by that Form instance.
        public static RadProperty BindingContextProperty =
            RadProperty.Register("BindingContext",
            typeof(BindingContext), typeof(RadObject),
            new RadElementPropertyMetadata(null, ElementPropertyOptions.CanInheritValue));

        /// <summary>
        /// Gets or sets the BindingContext for the object.
        /// </summary>
        [RadPropertyDefaultValue("BindingContext", typeof(RadObject))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual BindingContext BindingContext
        {
            get
            {
                return (BindingContext)this.GetValue(BindingContextProperty);
            }
            set
            {
                this.SetValue(BindingContextProperty, value);
            }
        }

        #endregion

        #region Fields

        private byte suspendPropertyNotifications;

        private RadPropertyValueCollection propertyValues;
        private RadObjectType radType;
        private Filter propertyFilter;

        #endregion

        #region Static

        private static readonly object PropertyChangedEventKey;
        private static readonly object RadPropertyChangedEventKey;
        private static readonly object RadPropertyChangingEventKey;

        public static readonly RadObjectType RadType;

        //bit state keys
        internal const ulong IsDesignModeStateKey = DisposableObjectLastStateKey << 1;
        internal const ulong RadObjectLastStateKey = IsDesignModeStateKey;

        #endregion
    }
}
