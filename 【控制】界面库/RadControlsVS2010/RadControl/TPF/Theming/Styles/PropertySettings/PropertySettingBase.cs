using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using Telerik.WinControls.Styles.PropertySettings;
using System.Drawing;
using System.Threading;

namespace Telerik.WinControls
{
    public abstract class PropertySettingBase : IPropertySetting
    {
        private string _propertyFromName = string.Empty;
        private RadProperty property;

        protected internal class ValueProviderHelper
        {
            private bool valueChanged;
            private IValueProvider valueProvider;
            private object value;
            //thread-safety support
            private Dictionary<int, object> valuesPerThread;
            private bool isValueThreadSafe;
            private static object syncRoot = new object();

            /// <summary>
            /// Empties the ValuesPerThread cache.
            /// </summary>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public void CleanValuesPerThread()
            {
                if (this.valuesPerThread == null)
                {
                    return;
                }

                foreach (object val in this.valuesPerThread.Values)
                {
                    IDisposable disposable = val as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }

                this.valuesPerThread = null;
            }

            public static bool IsValueThreadSafe(object value)
            {
                return !(value is Image || value is Font);
            }

            public object UnderlayingValue
            {
                get { return value; }
            }

            public object Value
            {
                get
                {
                    if (valueChanged)
                    {
                        this.valueProvider = value as IValueProvider;
                        this.valueChanged = false;
                    }

                    if (this.valueProvider != null)
                    {
                        return this.valueProvider.GetValue();
                    }

                    if (this.isValueThreadSafe)
                    {
                        return value;
                    }

                    return this.GetValuePerThread();
                }
                set
                {
                    if (value != this.value)
                    {
                        this.value = value;
                        this.valueChanged = true;
                        this.isValueThreadSafe = IsValueThreadSafe(this.value);
                    }
                }
            }

            private object GetValuePerThread()
            {
                if (this.value == null)
                {
                    return null;
                }

                if (this.valuesPerThread == null)
                {
                    lock (syncRoot)
                    {
                        //concurrency double-check pattern
                        if (this.valuesPerThread == null)
                        {
                            this.valuesPerThread = new Dictionary<int, object>();
                        }
                    }
                }

                int threadId = Thread.CurrentThread.ManagedThreadId;
                object val;
                this.valuesPerThread.TryGetValue(threadId, out val);

                if (val != null)
                {
                    return val;
                }

                //create clone of the object to be used by the other thread
                lock (syncRoot)
                {
                    ICloneable cloneable = this.value as ICloneable;
                    if (cloneable != null)
                    {
                        val = cloneable.Clone();
                        this.valuesPerThread.Add(threadId, val);
                    }
                }

                return val;
            }

            public IValueProvider ValueProvider
            {
                get { return valueProvider; }
            }
        }

        public PropertySettingBase()
        {
        }

        public PropertySettingBase(RadProperty property)
        {
            this.property = property;
        }

        [XmlIgnore]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadProperty Property
        {
            get
            {
                AssurePropertyFromName();
                return this.property;
            }
            set
            {
                this.property = value;
            }
        }

        private void AssurePropertyFromName()
        {
            if (property == null && _propertyFromName != string.Empty)
            {
                string[] propertyParts = _propertyFromName.Split('.');
                string propertyName;
                string className;
                if (propertyParts.Length > 1)
                {
                    propertyName = propertyParts[propertyParts.Length - 1];
                    className = string.Join(".", propertyParts, 0, propertyParts.Length - 1);
                }
                else
                {
                    throw new Exception("Invalid property parts");
                }

                this.property = RadProperty.Find(className, propertyName);
            }
        }

        public string PropertyToSet
        {
            get
            {
                return _propertyFromName;
            }
            set
            {
                _propertyFromName = value;
            }
        }

        protected void RegisterStyleValueBase(RadElement element)
        {
            element.AddStylePropertySetting(this);            
        }
        
        protected void UnregisterStyleValueBase(RadElement element)
        {
            element.RemoveStylePropertySetting(this);            
        }

        protected void UnapplyStyleValueBase(RadElement element)
        {
            element.RemoveStylePropertySetting(this);
        }

        #region IPropertySetting Members

        public abstract object GetCurrentValue(RadObject forObject);
        
        public abstract void ApplyValue(RadElement element);

        public abstract void UnapplyValue(RadElement element);

        public abstract void UnregisterValue(RadElement selectedElement);

		void IPropertySetting.PropertySettingRemoving(RadObject targetRadObject)
		{
			this.PropertySettingRemoving(targetRadObject);
		}

        XmlPropertySetting IPropertySetting.Serialize()
        {
            return this.Serialize();
        }

        #endregion

        protected abstract XmlPropertySetting Serialize();

		protected virtual void PropertySettingRemoving(RadObject targetRadObject)
		{
		}



        #region IPropertySetting Members

        public abstract object Clone();

        #endregion
    }
}
