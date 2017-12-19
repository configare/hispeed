namespace Telerik.Data.Expressions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// 
    /// </summary>
    public abstract class DynamicObject : CustomTypeDescriptor, IEnumerable<KeyValuePair<string, object>>
    {
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return this.GetEnumeratorInternal();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumeratorInternal();
        }

        protected abstract IEnumerator<KeyValuePair<string, object>> GetEnumeratorInternal();
        protected abstract object GetValue(string name);
        protected abstract void SetValue(string name, object value);

        public override PropertyDescriptorCollection GetProperties()
        {
            return this.GetPropertiesInternal(new Attribute[] {});
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return this.GetPropertiesInternal(attributes);
        }

        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        PropertyDescriptorCollection GetPropertiesInternal(Attribute[] attributes)
        {
            List<PropertyDescriptor> list = new List<PropertyDescriptor>();
            foreach (KeyValuePair<string, object> pair in this)
            {
                Type type = null;
                if (null != pair.Value)
                {
                    type = pair.Value.GetType();
                }
                list.Add(new DynamicProperty(pair.Key, type, null));
            }

            return new PropertyDescriptorCollection(list.ToArray(), true);
        }

        /// <summary>
        /// 
        /// </summary>
        class DynamicProperty : PropertyDescriptor
        {
            Type type;

            public DynamicProperty(string name, Type type, Attribute[] attr)
                : base(name, attr)
            {
                this.type = type;
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override object GetValue(object component)
            {
                return ((DynamicObject)component).GetValue(base.Name);
            }

            public override void ResetValue(object component)
            {
                ((DynamicObject)component).SetValue(base.Name, null);
            }

            public override void SetValue(object component, object value)
            {
                ((DynamicObject)component).SetValue(base.Name, value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            public override Type ComponentType
            {
                get { return typeof(ExpressionContext); }
            }

            public override bool IsReadOnly
            {
                get { return true; }
            }

            public override Type PropertyType
            {
                get { return this.type; }
            }
        }
    }
}