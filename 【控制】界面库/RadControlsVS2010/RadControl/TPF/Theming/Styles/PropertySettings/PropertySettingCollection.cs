using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Collections;

namespace Telerik.WinControls
{
    internal class Locker
    {
        public static object SyncObj = new object();
    }
    /// <summary>
    /// Represents a property settings collection. Property settings are very
    /// similar to CSS style properties.
    /// </summary>
    [Serializable]
	[TypeConverter(typeof(ExpandableObjectConverter))]
    public class PropertySettingCollection : ICollection<IPropertySetting>
    {
        private List<IPropertySetting> innerList = new List<IPropertySetting>(0);
        private List<InheritedSetting> inheritedItems;
       
        #region list-like features

        public IList<IPropertySetting> OriginalPropertySettings 
        {
            get { return this.innerList; }
        }

        public void AddRange(IEnumerable<IPropertySetting> items)
        {
            this.innerList.AddRange(items);
        }

        #endregion

        #region property setttings inheritance supoprt

        internal void AddInheritedPropertySetting(IPropertySetting setting, string settingType)
        {
            lock (Locker.SyncObj)
            {
                if (this.inheritedItems == null)
                {
                    this.inheritedItems = new List<InheritedSetting>();
                }
                this.inheritedItems.Add(new InheritedSetting(settingType, setting));
            }
        }

        internal void ResetInheritedProperties()
        {
            lock (Locker.SyncObj)
            {
                if (this.inheritedItems == null)
                {
                    return;
                }

                //TODO: Think of some smarter way for resetting inherited properties
                this.inheritedItems = null;
                this.mappedSettings.Clear();
            }
        }

        #endregion

        #region ICollection<IPropertySetting> Members

        public bool ContainsInheritedSetting(RadProperty property)
        {
            return this.FindInheritedSetting(property) != null;
        }

        public IPropertySetting FindInheritedSetting(RadProperty property)
        {
            lock (Locker.SyncObj)
            {
                if (this.inheritedItems == null)
                {
                    return null;
                }

                foreach (InheritedSetting inheritedSetting in this.inheritedItems)
                {
                    if (inheritedSetting.property == property)
                    {
                        return inheritedSetting.setting;
                    }
                }
            }
            return null;
        }

        public void Add(IPropertySetting item)
        {
            this.innerList.Add(item);
        }

        public bool ContainsSetting(RadProperty property)
        {
            return this.FindSetting(property) != null;
        }

        public IEnumerable<IPropertySetting> EnumLocalSettings()
        {
            foreach (IPropertySetting setting in this.innerList)
            {
                yield return setting;
            }
        }

        public IPropertySetting FindSetting(RadProperty property)
        {
            foreach (IPropertySetting setting in this.innerList)
            {
                if (setting.Property == property)
                {
                    return setting;
                }
            }

            return null;
        }

        public bool RemoveSetting(RadProperty property)
        {
            int count = this.innerList.Count;
            for (int i = 0; i < count; i++)
            {
                IPropertySetting setting = this.innerList[i];
                if (setting.Property == property)
                {
                    this.innerList.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            this.innerList.Clear();
        }

        public bool Contains(IPropertySetting item)
        {
            return this.innerList.Contains(item);
        }

        public void CopyTo(IPropertySetting[] array, int arrayIndex)
        {
            this.innerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get 
            {
                return this.innerList.Count + (this.inheritedItems != null ? inheritedItems.Count : 0);
            }
        }

        bool ICollection<IPropertySetting>.IsReadOnly
        {
            get { return ((ICollection<IPropertySetting>)this.innerList).IsReadOnly; }
        }

        public bool Remove(IPropertySetting item)
        {
            return this.innerList.Remove(item);
        }

        #endregion

        #region IEnumerable<IPropertySetting> Members

        IEnumerator<IPropertySetting> IEnumerable<IPropertySetting>.GetEnumerator()
        {
            foreach (IPropertySetting setting in this.EnumeratePropertySettingsForElement(null))
            {
                yield return setting;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (IPropertySetting setting in this.EnumeratePropertySettingsForElement(null))
            {
                yield return setting;
            }
        }

        #endregion

        private struct InheritedSetting
        {
            public string settingType;
            public IPropertySetting setting;
            public RadProperty property;
            private int hash;

            public InheritedSetting(string type, IPropertySetting setting)
            {
                this.settingType = type;
                this.setting = setting;
                this.property = setting.Property;
                this.hash = this.property.NameHash;
                if(!string.IsNullOrEmpty(type))
                {
                    this.hash ^= type.GetHashCode();
                }
            }

            public InheritedSetting(string type, RadProperty property)
            {
                this.settingType = type;
                this.setting = null;
                this.property = property;
                this.hash = this.property.NameHash;
                if(!string.IsNullOrEmpty(type))
                {
                    this.hash ^= type.GetHashCode();
                }
            }

            public override int GetHashCode()
            {
                return hash;
            }

            public override bool Equals(object obj)
            {
                InheritedSetting settingToCompare = (InheritedSetting)obj;
                return this.settingType == settingToCompare.settingType && this.property == settingToCompare.property;
            }
        }

        private Dictionary<InheritedSetting, IPropertySetting> mappedSettings = new Dictionary<InheritedSetting, IPropertySetting>();
        
        internal IEnumerable<IPropertySetting> EnumeratePropertySettingsForElement(RadElement element)
        {
            return this.EnumeratePropertySettingsForElement(element, true, true);
        }

        internal IEnumerable<IPropertySetting> EnumeratePropertySettingsForElement(RadElement element, bool inherited, bool local)
        {
            lock (Locker.SyncObj)
            {
                if (this.inheritedItems != null && inherited)
                {
                    foreach (InheritedSetting inheritedSetting in this.inheritedItems)
                    {
                        IPropertySetting newSetting = this.MapProperty(inheritedSetting, element);
                        if (newSetting != null)
                        {
                            yield return newSetting;
                        }
                        else
                        {
                            yield return inheritedSetting.setting;
                        }
                    }
                }

                if (local)
                {
                    foreach (IPropertySetting setting in this.innerList)
                    {   
                        yield return setting;
                    }
                }
            }
        }

        private IPropertySetting MapProperty(InheritedSetting inheritedSetting, RadElement element)
        {
            if (element == null)
            {
                return null;
            }
            RadProperty mappedProperty = element.MapStyleProperty(inheritedSetting.property, inheritedSetting.settingType);
            if (mappedProperty == null)
            {
                return null;
            }

            InheritedSetting newInheritedSetting = new InheritedSetting(inheritedSetting.settingType, mappedProperty);
            IPropertySetting newSetting;
            if (!mappedSettings.TryGetValue(newInheritedSetting, out newSetting))
            {
                lock (Locker.SyncObj)
                {
                    //double check concurrency pattern
                    if (!mappedSettings.TryGetValue(newInheritedSetting, out newSetting))
                    {
                        newSetting = inheritedSetting.setting.Clone() as IPropertySetting;
                        newSetting.Property = mappedProperty;
                        mappedSettings[newInheritedSetting] = newSetting;
                    }
                }
            }

            return newSetting;
        }

        public struct PropertySettingsCollectionEnumerator : IEnumerator<IPropertySetting>
        {
            IEnumerator<IPropertySetting> ownerEnumerator;
            IEnumerator<IPropertySetting> inheritedPropertiesEnumerator;
            IEnumerator<IPropertySetting> activeEnumerator;

            public PropertySettingsCollectionEnumerator(PropertySettingCollection owner)
            {
                this.ownerEnumerator = ((IEnumerable<IPropertySetting>)owner.innerList).GetEnumerator();
                this.activeEnumerator = this.ownerEnumerator;

                if (owner.inheritedItems != null)
                {
                    this.inheritedPropertiesEnumerator = ((IEnumerable<IPropertySetting>)owner.inheritedItems).GetEnumerator();
                }
                else
                {
                    this.inheritedPropertiesEnumerator = null;
                }
            }

            #region IEnumerator<PropertySetting> Members

            public IPropertySetting Current
            {
                get 
                { 
                    return activeEnumerator.Current; 
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                this.ownerEnumerator.Dispose();
                if (inheritedPropertiesEnumerator != null)
                {
                    this.inheritedPropertiesEnumerator.Dispose();
                }
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                bool res = activeEnumerator.MoveNext();
                if (!res && 
                    activeEnumerator == ownerEnumerator && 
                    this.inheritedPropertiesEnumerator != null)
                {
                    this.activeEnumerator = inheritedPropertiesEnumerator;
                    res = this.inheritedPropertiesEnumerator.MoveNext();                    
                }

                return res;
            }

            public void Reset()
            {
                this.ownerEnumerator.Reset();
                if (this.inheritedPropertiesEnumerator != null)
                {
                    this.inheritedPropertiesEnumerator.Reset();
                }
            }

            #endregion
        }        
    }
}