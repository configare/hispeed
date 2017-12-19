using System.Collections.Specialized;

namespace Telerik.WinControls.UI
{
    class ListViewDetailsCache
    {
        HybridDictionary dictionary = new HybridDictionary();

        public object this[ListViewDetailColumn column]
        {
            get
            {
                if (column.IsDataBound && !string.IsNullOrEmpty(column.FieldName))
                {
                    return dictionary[column.FieldName];
                }

                return dictionary[column];
            }
            set
            {
                if (column.IsDataBound && !string.IsNullOrEmpty(column.FieldName))
                {
                    dictionary[column.FieldName] = value;
                }

                dictionary[column] = value;
            }
        }

        public void Clear()
        {
            dictionary.Clear();
        }

        public void Remove(ListViewDetailColumn key)
        {
            if (this.dictionary.Contains(key))
            {
                this.dictionary.Remove(key);
            }
        }

        public void ReplaceKey(ListViewDetailColumn oldKey, ListViewDetailColumn newKey)
        {
            if (!this.dictionary.Contains(oldKey))
            {
                return;
            }

            object obj = this.dictionary[oldKey];
            this.dictionary.Remove(oldKey);
            this.dictionary.Add(newKey, obj);
        }

    }
}
