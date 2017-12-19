using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Telerik.WinControls.UI
{
    public class SplitPanelCollection : IList<SplitPanel>, IList, ICollection, IEnumerable
    {
        private RadSplitContainer owner;

        public SplitPanelCollection(RadSplitContainer owner)
        {
            this.owner = owner;
        }

        public virtual SplitPanel this[int index]
        {
            get
            {
                return (SplitPanel)this.owner.Controls[index];
            }
			set
			{
				this.owner.Controls.RemoveAt(index);
				this.owner.Controls.Add(value);
				this.owner.Controls.SetChildIndex(value, index);
			}
        }

        public virtual SplitPanel this[string key]
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

        private bool IsValidIndex(int index)
        {
            return (index >= 0) && (index < this.Count);
        }

        public virtual int IndexOfKey(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, true))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public void Add(SplitPanel value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.owner.Controls.Add(value);
        }

        public void AddRange(IEnumerable<SplitPanel> splitPanels)
        {
            if (splitPanels == null)
            {
                throw new ArgumentNullException("SplitPanels");
            }

            foreach (SplitPanel pane in splitPanels)
            {
                this.Add(pane);
            }
        }

        public int IndexOf(SplitPanel splitPanel)
        {
            if (splitPanel == null)
            {
                throw new ArgumentNullException("value");
            }

            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] == splitPanel)
                {
                    return i;
                }
            }

            return -1;
        }

        public bool Contains(SplitPanel spliPanel)
        {
            if (spliPanel == null)
            {
                throw new ArgumentNullException("value");
            }

            return this.IndexOf(spliPanel) != -1;
        }

        public virtual bool ContainsKey(string key)
        {
            return this.IsValidIndex(this.IndexOfKey(key));
        }

        public void Insert(int index, string text)
        {
            SplitPanel splitPanel = new SplitPanel();
            splitPanel.Text = text;
            this.Insert(index, splitPanel);
        }

        public void Insert(int index, SplitPanel splitPanel)
        {
            this.owner.Controls.Add(splitPanel);
            this.owner.Controls.SetChildIndex(splitPanel, index);
        }

        public void Insert(int index, string key, string text)
        {
            SplitPanel splitPanel = new SplitPanel();
            splitPanel.Name = key;
            splitPanel.Text = text;
            this.Insert(index, splitPanel);
        }

        public bool Remove(SplitPanel value)
        {
            if (value == null)
            {
				//throw new ArgumentNullException("value");
				return false;
            }

            this.owner.Controls.Remove(value);
			return true;
        }

        public virtual void RemoveByKey(string key)
        {
            int index = this.IndexOfKey(key);
            if (this.IsValidIndex(index))
            {
                this.RemoveAt(index);
            }
        }

        #region IList Members

        int IList.Add(object value)
        {
            if (!(value is SplitPanel))
            {
                throw new ArgumentException("value");
            }

            this.Add((SplitPanel)value);
            return this.IndexOf((SplitPanel)value);
        }

        public void Clear()
        {
            this.owner.Controls.Clear();
        }

        bool IList.Contains(object value)
        {
            return (value is SplitPanel) && this.Contains((SplitPanel)value);
        }

        int IList.IndexOf(object value)
        {
            if (value is SplitPanel)
            {
                return this.IndexOf((SplitPanel)value);
            }

            return -1;
        }

        void IList.Insert(int index, object value)
        {
            if (!(value is SplitPanel))
            {
                throw new ArgumentException("value");
            }

            this.Insert(index, (SplitPanel)value);
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
            if (value is SplitPanel)
            {
                this.Remove((SplitPanel)value);
            }
        }

        public void RemoveAt(int index)
        {
            this.owner.Controls.RemoveAt(index);
        }
        
        object IList.this[int index]
        {
            get
            {
                return this.owner.Controls[index];
            }
            set
            {
                if (!(value is SplitPanel))
                {
                    throw new ArgumentException("value");
                }

                //this.owner.Controls[index] = (SplitPanel)value;
            }
        }

        #endregion

		void ICollection<SplitPanel>.CopyTo(SplitPanel[] dest, int index)
		{
			if (this.Count > 0)
			{
				this.owner.Controls.CopyTo(dest, index);
			}
		}

        #region ICollection Members

        void ICollection.CopyTo(Array dest, int index)
        {
            if (this.Count > 0)
            {
                this.owner.Controls.CopyTo(dest, index);
            }
        }

        public int Count
        {
            get { return this.owner.Controls.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return this; }
        }

        #endregion

		IEnumerator<SplitPanel> IEnumerable<SplitPanel>.GetEnumerator()
		{
			SplitPanel[] panelArray = new SplitPanel[this.Count];
			this.owner.Controls.CopyTo(panelArray, 0);
			
			List<SplitPanel> panels = new List<SplitPanel>(this.Count);
			panels.AddRange(panelArray);
			return panels.GetEnumerator();
		}

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			SplitPanel[] panels = new SplitPanel[this.Count];
			this.owner.Controls.CopyTo(panels, 0);
			return panels.GetEnumerator();
		}

		#endregion
    }
}
