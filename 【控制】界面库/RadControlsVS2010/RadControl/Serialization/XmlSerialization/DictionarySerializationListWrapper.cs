using System;
using System.Collections;
using System.Text;

namespace Telerik.WinControls.XmlSerialization
{
    //internal class GenericDictionarySerializationListWrapper: IList
    //{
    //    public GenericDictionarySerializationListWrapper(IDictionary<object, object> test)
    //    {
    //    }
    //}

    public class DictionarySerializationListWrapper: IList
    {
        private IDictionary wrapped;

        public DictionarySerializationListWrapper(IDictionary toWrap)
        {
            wrapped = toWrap;
        }

        #region IList Members

        public int Add(object value)
        {
            if (value != null && typeof(DictionaryEntry).IsAssignableFrom(value.GetType()))
            {
                DictionaryEntry entry = (DictionaryEntry)value;

                this.wrapped[entry.Key] = entry.Value;
            }

            return this.wrapped.Count;
        }

        public void Clear()
        {
            this.wrapped.Clear();
        }

        public bool Contains(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int IndexOf(object value)
        {
            int i = 0;
            foreach (object presentValue in this.wrapped)
            {
                if (presentValue == value)
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        public void Insert(int index, object value)
        {
            this.Add(value);
        }

        public bool IsFixedSize
        {
            get
            {
                return this.wrapped.IsFixedSize;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.wrapped.IsReadOnly;
            }
        }

        public void Remove(object value)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RemoveAt(int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public object this[int index]
        {
            get
            {
                int i = 0;
                foreach(object value in this.wrapped)
                {
                    if (i == index)
                        return value;

                    i++;
                }

                throw new ArgumentOutOfRangeException("index");
                
            }
            set
            {
                DictionaryEntry entry = (DictionaryEntry) value;
            
                this.wrapped[entry.Key] = entry.Value;                
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public int Count
        {
            get 
            {
                return this.wrapped.Count;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return this.wrapped.IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this.wrapped.SyncRoot;
            }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.wrapped.GetEnumerator();
        }

        #endregion
    }
}
