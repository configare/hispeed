using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;


namespace Telerik.WinControls.UI
{
    public class SplitterCollection : ICollection, IEnumerable<SplitterElement>
    {
        private RadSplitContainer owner;

        internal SplitterCollection(RadSplitContainer owner)
        {
            this.owner = owner;
        }

        public SplitterElement this[int index]
        {
            get
            {
                return this.owner.splitContainerElement.Children[index] as SplitterElement;
            }
        }

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            ArrayList list = new ArrayList(this.Count);
            foreach (SplitterElement splitter in this)
            {
                list.Add(splitter);
            }

            list.CopyTo(array, index);
        }

        public int Count
        {
            get 
            {
                return this.owner.splitContainerElement.Count;
            }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return this.owner; }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        #endregion

        #region IEnumerable<SplitterElement> Members

        IEnumerator<SplitterElement> IEnumerable<SplitterElement>.GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        #endregion
    }
}
