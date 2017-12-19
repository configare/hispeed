using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.Design
{
	public class RadObjectCustomPropertyDescriptor: PropertyDescriptor
	{
		private PropertyDescriptor wrapped;
        
        bool readOnly = false;

        public RadObjectCustomPropertyDescriptor(PropertyDescriptor wrapped, Attribute[] attributes, bool readOnly)
            : base(wrapped.Name, attributes)
        {
            this.wrapped = wrapped;
            this.readOnly = readOnly;
        }

		public RadObjectCustomPropertyDescriptor(PropertyDescriptor wrapped, Attribute[] attributes)
            : base(wrapped.Name, attributes)
        {
            this.wrapped = wrapped;
            this.readOnly = wrapped.IsReadOnly;
        }

        public override bool CanResetValue(object component)
        {
            return Wrapped.CanResetValue(component);
        }

        public override Type ComponentType
        {
            get { return Wrapped.ComponentType; }
        }

        protected override void OnValueChanged(object component, EventArgs e)
        {
            base.OnValueChanged(component, e);
        }

        public override object GetValue(object component)
        {
            return Wrapped.GetValue(component);
        }

        public override bool IsReadOnly
        {
            get
            {
                return readOnly;
            }
        }

        public override Type PropertyType
        {
            get
            {
                return Wrapped.PropertyType;
            }
        }

		public PropertyDescriptor Wrapped
		{
			get { return wrapped; }			
		}

		public override void ResetValue(object component)
        {
            Wrapped.ResetValue(component);
        }

        public override void SetValue(object component, object value)
        {
            try
            {
				//The following is considered working well only when
				//RadElement.SerializeChildren defaults to true;
                
                //TODO: remove when lifecycle of elements is completed
                if (component is RadElement)
                {
                    if (this.SerializationVisibility != DesignerSerializationVisibility.Hidden)
                    {
                        RadElement targetElement = component as RadElement;
                        targetElement.SerializeProperties = true;
                    }
                }

				Wrapped.SetValue(component, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting property \"" + this.Name + "\" in class \"" + this.Wrapped.ComponentType.FullName + "\" to value \"" + value.ToString() + "\".\n\n" +
                    "The error was:\n" + ex.ToString());
            }
        }

		protected bool? GetShouldSerializeFromRadobject(object component)
		{
			if (component is RadObject)
			{
                return (component as RadObject).ShouldSerializeProperty(this);
			}

			return null;
		}

        public override void AddValueChanged(object component, EventHandler handler)
        {
            base.AddValueChanged(component, handler);
            this.wrapped.AddValueChanged(component, handler);
        }

        public override void RemoveValueChanged(object component, EventHandler handler)
        {
            base.RemoveValueChanged(component, handler);
            this.wrapped.RemoveValueChanged(component, handler);
        }

		public override bool ShouldSerializeValue(object component)
		{
			bool? res = GetShouldSerializeFromRadobject(component);
			if (res != null && res.HasValue)
			{
				return res.Value;
			}

			return Wrapped.ShouldSerializeValue(component);
		}
	}

    /// <exclude/>
	public class RadObjectCustomRadPropertyDescriptor : RadObjectCustomPropertyDescriptor
    {
        public RadObjectCustomRadPropertyDescriptor(PropertyDescriptor wrapped, Attribute[] attributes, bool readOnly)
            : base(wrapped, attributes, readOnly)
        {            
        }

        public RadObjectCustomRadPropertyDescriptor(PropertyDescriptor wrapped, Attribute[] attributes)
            : base(wrapped, attributes)
        {
        }

        private RadProperty radProperty;

        private RadProperty GetRadProperty(RadObject component)
        {
            if (radProperty == null)
            {
                radProperty = component.GetRegisteredRadProperty(this.Name);
            }

            return radProperty;
        }

		public override void SetValue(object component, object value)
		{
			base.SetValue(component, value);

            RadObject element = (RadObject)component;
            if (element == null)
            {
                return;
            }

            RadProperty property = this.GetRadProperty(element);
            if (property != null && element.IsDesignMode)
            {
                element.SetPropertyValueSetAtDesignTime(property);
            }
		}

		public override bool ShouldSerializeValue(object component)
		{
			bool? res = GetShouldSerializeFromRadobject(component);
			if (res != null && res.HasValue)
			{
				return res.Value;
			}

			RadObject radObject = (RadObject)component;
			RadProperty property = this.GetRadProperty(radObject);
            RadPropertyValue propVal = radObject.GetPropertyValue(property);

            //no entry registered for this RadProperty
            if (propVal == null)
            {
                return false;
            }
            
            //TODO: Research why the DefaultValueOverride is part of this condition?
            if (propVal.ValueSource == ValueSource.Local || propVal.ValueSource == ValueSource.LocalFromBinding)
            {
                if (property == RadElement.BoundsProperty)
                {
                    return (bool)radObject.GetValue(RadElement.AutoSizeProperty) == false;
                }

                if (propVal.IsSetAtDesignTime)
                {
                    return true;
                }

                return Wrapped.ShouldSerializeValue(component);
            }

            //do not serialize value
            return false;
		}
    }	
}
