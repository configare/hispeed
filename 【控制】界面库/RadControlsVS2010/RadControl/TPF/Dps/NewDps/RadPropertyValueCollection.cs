using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Allows RadObject inheritors to replace RadProperty instances with another one.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal delegate RadProperty MapPropertyDelegate(RadProperty request);
    /// <summary>
    /// Represents a storage for RadPropertyValue entries, which are accessed by their GlobalIndex property.
    /// </summary>
    internal class RadPropertyValueCollection
    {
        #region Constructor

        internal RadPropertyValueCollection(RadObject owner)
        {
            this.owner = owner;
            this.entries = new Dictionary<int, RadPropertyValue>();
        }

        #endregion

        #region Public Methods

        public RadPropertyValue GetEntry(RadProperty prop, bool createNew)
        {
            if (prop == null)
            {
                throw new ArgumentNullException("RadProperty");
            }

            //check for property mapping
            if (this.propertyMapper != null)
            {
                RadProperty mappedProperty = this.propertyMapper(prop);
                if (mappedProperty != null)
                {
                    prop = mappedProperty;
                }
            }

            RadPropertyValue entry;
            if (!this.entries.TryGetValue(prop.GlobalIndex, out entry) && createNew)
            {
                entry = new RadPropertyValue(this.owner, prop);
                this.entries[prop.GlobalIndex] = entry;
            }

            return entry;
        }

        internal void Clear()
        {
            this.entries = null;
        }

        #endregion

        #region Private Implementation

        internal MapPropertyDelegate PropertyMapper
        {
            get
            {
                return this.propertyMapper;
            }
            set
            {
                this.propertyMapper = value;
            }
        }

        internal void Reset()
        {
            foreach (RadPropertyValue entry in this.entries.Values)
            {
                entry.Dispose();
            }

            this.entries.Clear();
            this.propertyMapper = null;
        }

        internal void ResetStyleProperties()
        {
            if (this.entries.Count == 0)
            {
                return;
            }

            //copy values in separate list as the Reset process may change the collection
            List<RadPropertyValue> values = new List<RadPropertyValue>(this.entries.Values);
            int length = values.Count;

            for (int i = 0; i < length; i++)
            {
                RadPropertyValue propVal = values[i];
                if (propVal.ValueSource == ValueSource.Style)
                {
                    this.owner.ResetValueCore(propVal, ValueResetFlags.Style);
                }
            }
        }

        internal void ResetInheritableProperties()
        {
            if (this.entries.Count == 0)
            {
                return;
            }

            //copy values in separate list as the Reset process may change the collection
            List<RadPropertyValue> values = new List<RadPropertyValue>(this.entries.Values);
            int length = values.Count;

            for (int i = 0; i < length; i++)
            {
                RadPropertyValue propVal = values[i];
                if (propVal.Metadata.IsInherited && propVal.InvalidateInheritedValue())
                {
                    this.owner.UpdateValueCore(propVal);
                }
            }
        }

        internal void SetLocalValuesAsDefault()
        {
            foreach (RadPropertyValue entry in this.entries.Values)
            {
                if (entry.ValueSource != ValueSource.Local)
                {
                    continue;
                }

                entry.BeginUpdate(true, true);
                object val = entry.LocalValue;
                entry.SetLocalValue(RadProperty.UnsetValue);
                entry.SetDefaultValueOverride(val);
                entry.EndUpdate(true, true);
            }
        }

        #endregion

        #region Fields

        //TODO: We may consider implementing our own custom hashtable without grow factor
        private Dictionary<int, RadPropertyValue> entries;
        private RadObject owner;
        private MapPropertyDelegate propertyMapper;

        #endregion
    }
}
