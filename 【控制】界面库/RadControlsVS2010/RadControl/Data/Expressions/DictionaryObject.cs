namespace Telerik.Data.Expressions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class DictionaryObject : DynamicObject, IDictionary<string, object>
    {
        Dictionary<string, object> properties;
        
        public object this[string name]
        {
            get { return this.GetValue(name); }
            set { this.SetValue(name, value); }
        }

        public DictionaryObject() : this(new Dictionary<string, object>())
        {
        }

        public DictionaryObject(Dictionary<string, object> properties)
        {
            this.properties = properties;
        }

        protected override IEnumerator<KeyValuePair<string, object>> GetEnumeratorInternal()
        {
            return this.properties.GetEnumerator();
        }

        protected override object GetValue(string name)
        {
            if (this.properties.ContainsKey(name))
            {
                return this.properties[name];
            }
            throw new ArgumentOutOfRangeException(string.Format("Property {0} does not exist.", name));
        }

        protected override void SetValue(string name, object value)
        {
            this.properties[name] = value;
        }

        #region IDictionary<string,object> Members

        public bool ContainsKey(string key)
        {
            return this.properties.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            this.properties.Add(key, value);
        }

        public bool Remove(string key)
        {
            return this.properties.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return this.properties.TryGetValue(key, out value);
        }

        public ICollection<string> Keys
        {
            get { return this.properties.Keys; }
        }

        public ICollection<object> Values
        {
            get { return this.properties.Values; }
        }

        #endregion

        #region ICollection<KeyValuePair<string,object>> Members
        
        public void Add(KeyValuePair<string, object> item)
        {
            ((ICollection<KeyValuePair<string, object>>)this.properties).Add(item);
        }

        public void Clear()
        {
            this.properties.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)this.properties).Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, object>>)this.properties).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)this.properties).Remove(item);
        }

        public int Count
        {
            get { return this.properties.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<string, object>>)this.properties).IsReadOnly; }
        }

        #endregion
    }
}