using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls.UI
{
    public class TabPanelCollection : IList, ICollection, IEnumerable
    {
        private TabStripPanel owner;
        private int lastAccessedIndex;

        public TabPanelCollection(TabStripPanel owner)
        {
            this.lastAccessedIndex = -1;

            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }
            this.owner = owner;
        }

        public void Add(string text)
        {
            TabPanel tabPanel = new TabPanel();
            tabPanel.Text = text;

            this.Add(tabPanel);
        }

        public void Add(TabPanel value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.owner.Controls.Add(value);
        }

        public void Add(string key, string text)
        {
            TabPanel tabPanel = new TabPanel();
            tabPanel.Name = key;
            tabPanel.Text = text;

            this.Add(tabPanel);
        }

        public void Add(string key, string text, int imageIndex)
        {
            TabPanel tabPanel = new TabPanel();
            tabPanel.Name = key;
            tabPanel.Text = text;
            //tabPanel.ImageIndex = imageIndex;

            this.Add(tabPanel);
        }

        public void Add(string key, string text, string imageKey)
        {
            TabPanel tabPanel = new TabPanel();
            tabPanel.Name = key;
            tabPanel.Text = text;
            //tabPanel.ImageKey = imageKey;

            this.Add(tabPanel);
        }

        public void AddRange(TabPanel[] panels)
        {
            if (panels == null)
            {
                throw new ArgumentNullException("panels");
            }

            foreach (TabPanel panel in panels)
            {
                this.Add(panel);
            }
        }

        public bool Contains(TabPanel panel)
        {
            if (panel == null)
            {
                throw new ArgumentNullException("panel");
            }

            return this.IndexOf(panel) != -1;
        }

        public virtual bool ContainsKey(string key)
        {
            return this.IsValidIndex(this.IndexOfKey(key));
        }


        public int IndexOf(TabPanel panel)
        {
            if (panel == null)
            {
                throw new ArgumentNullException("panel");
            }

            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] == panel)
                {
                    return i;
                }
            }
            return -1;
        }

        public virtual int IndexOfKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (this.IsValidIndex(this.lastAccessedIndex) && WindowsFormsUtils.SafeCompareStrings(this[this.lastAccessedIndex].Name, key, true))
                {
                    return this.lastAccessedIndex;
                }

                for (int i = 0; i < this.Count; i++)
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, true))
                    {
                        this.lastAccessedIndex = i;
                        return i;
                    }
                }

                this.lastAccessedIndex = -1;
            }

            return -1;
        }

        public void Insert(int index, string text)
        {
            TabPanel tabPanel = new TabPanel();
            tabPanel.Text = text;
            this.Insert(index, tabPanel);
        }

        public void Insert(int index, TabPanel tabPanel)
        {
            this.owner.Controls.Add(tabPanel);
            this.owner.Controls.SetChildIndex(tabPanel, index);
            this.owner.SelectedIndex = index;
        }

        public void Insert(int index, string key, string text)
        {
            TabPanel tabPanel = new TabPanel();
            tabPanel.Name = key;
            tabPanel.Text = text;
            this.Insert(index, tabPanel);
        }

        public void Insert(int index, string key, string text, int imageIndex)
        {
            TabPanel tabPanel = new TabPanel();
            tabPanel.Name = key;
            tabPanel.Text = text;
            this.Insert(index, tabPanel);
            //tabPanel.ImageIndex = imageIndex;
        }

        public void Insert(int index, string key, string text, string imageKey)
        {
            TabPanel tabPanel = new TabPanel();
            tabPanel.Name = key;
            tabPanel.Text = text;
            this.Insert(index, tabPanel);
            //tabPanel.ImageKey = imageKey;
        }

        private bool IsValidIndex(int index)
        {
            return ((index >= 0) && (index < this.Count));
        }

        public void Remove(TabPanel value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.owner.Controls.Remove(value);
        }

        public void RemoveAt(int index)
        {
            this.owner.Controls.RemoveAt(index);
        }

        public virtual void RemoveByKey(string key)
        {
            int index = this.IndexOfKey(key);
            if (this.IsValidIndex(index))
            {
                this.RemoveAt(index);
            }
        }

        public virtual TabPanel this[int index]
        {
            get
            {
                return (TabPanel)this.owner.Controls[index];
            }
        }

        public virtual TabPanel this[string key]
        {
            get
            {
                if (!string.IsNullOrEmpty(key))
                {
                    int index = this.IndexOfKey(key);
                    if (this.IsValidIndex(index))
                    {
                        return this[index];
                    }
                }

                return null;
            }
        }

        #region IList Members

        int IList.Add(object value)
        {
            if (!(value is TabPanel))
            {
                throw new ArgumentException("value");
            }

            this.Add((TabPanel)value);
            return this.IndexOf((TabPanel)value);
        }

        public void Clear()
        {
            this.owner.Controls.Clear();
        }

        bool IList.Contains(object value)
        {
            return (value is TabPanel) && this.Contains((TabPanel)value);
        }

        int IList.IndexOf(object value)
        {
            if (value is TabPanel)
            {
                return this.IndexOf((TabPanel)value);
            }

            return -1;
        }

        void IList.Insert(int index, object value)
        {
            if (!(value is TabPanel))
            {
                throw new ArgumentException("tabPanel");
            }

            this.Insert(index, (TabPanel)value);

        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            if (value is TabPanel)
            {
                this.Remove((TabPanel)value);
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                if (!(value is TabPanel))
                {
                    throw new ArgumentException("value");
                }

                //this[index] = (TabPanel)value;
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            if (this.Count > 0)
            {
                //Array.Copy(this.owner.Controls, 0, dest, index, this.Count);
                this.owner.Controls.CopyTo(array, index);
            }

        }

        public int Count
        {
            get { return this.owner.Controls.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return this.owner.Controls.GetEnumerator();
        }

        #endregion
      
    }
}
