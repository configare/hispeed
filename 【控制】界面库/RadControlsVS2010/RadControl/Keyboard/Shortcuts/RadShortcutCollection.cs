using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls
{
    public class RadShortcutCollection : IList<RadShortcut>
    {
        #region Fields

        private List<RadShortcut> list;
        private IShortcutProvider owner;
        public const char ShortcutDelimiter = ',';

        #endregion

        #region Constructor

        public RadShortcutCollection(IShortcutProvider owner)
        {
            this.owner = owner;
            this.list = new List<RadShortcut>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the IShortcutProvider instance that owns this collection.
        /// </summary>
        public IShortcutProvider Owner
        {
            get
            {
                return this.owner;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets a human readable string representation of the collection.
        /// </summary>
        /// <returns></returns>
        public string GetDisplayText()
        {
            int count = this.list.Count;
            if (count == 0)
            {
                return "None";
            }

            StringBuilder builder = new StringBuilder(20);
            for (int i = 0; i < count; i++)
            {
                builder.Append(this.list[i].GetDisplayText());
                builder.Append(ShortcutDelimiter);
            }

            //remove last delimiter
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        #endregion

        #region Private Implementation

        private void OnCollectionChanged()
        {
            this.owner.OnShortcutsChanged();
        }

        #endregion

        #region IList<RadShortcut> Members

        public int IndexOf(RadShortcut item)
        {
            return this.list.IndexOf(item);
        }

        public void Insert(int index, RadShortcut item)
        {
            this.list.Insert(index, item);
            this.OnCollectionChanged();
        }

        public void RemoveAt(int index)
        {
            this.list.RemoveAt(index);
            this.OnCollectionChanged();
        }

        public RadShortcut this[int index]
        {
            get
            {
                return this.list[index];
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Value");
                }
                this.list[index] = value;
                this.OnCollectionChanged();
            }
        }

        #endregion

        #region ICollection<RadShortcut> Members

        public void Add(RadShortcut item)
        {
            this.list.Add(item);
            this.OnCollectionChanged();
        }

        public void Clear()
        {
            this.list.Clear();
            this.OnCollectionChanged();
        }

        public bool Contains(RadShortcut item)
        {
            return this.list.Contains(item);
        }

        public void CopyTo(RadShortcut[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(RadShortcut item)
        {
            bool result = this.list.Remove(item);
            if (result)
            {
                this.OnCollectionChanged();
            }
            return result;
        }

        #endregion

        #region IEnumerable<RadShortcut> Members

        public IEnumerator<RadShortcut> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion
    }
}
