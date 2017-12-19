using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    [Editor(DesignerConsts.RadPageViewPageCollectionEditorString, typeof(UITypeEditor))]
    public class RadPageViewPageCollection : 
        RadPageViewObject, 
        ICollection<RadPageViewPage>, 
        IList<RadPageViewPage>,
        IList
    {
        #region Fields

        private RadPageViewControlCollection controls;

        #endregion

        #region Constructor

        public RadPageViewPageCollection(RadPageView owner)
        {
            this.Owner = owner;
            this.controls = owner.Controls as RadPageViewControlCollection;
        }

        #endregion

        #region Collection Methods

        public void Add(RadPageViewPage item)
        {
            this.controls.Add(item);
        }

        public void Clear()
        {
            this.controls.Clear();
        }

        public bool Contains(RadPageViewPage item)
        {
            return this.controls.Contains(item);
        }

        public void CopyTo(RadPageViewPage[] array, int arrayIndex)
        {
            this.controls.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return this.controls.Count;
            }
        }

        bool ICollection<RadPageViewPage>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(RadPageViewPage item)
        {
            this.controls.Remove(item);
            return true;
        }

        public int IndexOf(RadPageViewPage item)
        {
            return this.controls.IndexOf(item);
        }

        public void Insert(int index, RadPageViewPage item)
        {
            this.controls.Add(item);
            this.ChangeIndex(item, index);
        }

        public void Swap(RadPageViewPage page1, RadPageViewPage page2)
        {
            if(page1.Owner != this.Owner || page2.Owner != this.Owner)
            {
                throw new ArgumentException("Pages do not belong to this PageView.");
            }

            int index1 = this.controls.IndexOf(page1);
            int index2 = this.controls.IndexOf(page2);

            if (index1 > index2)
            {
                this.ChangeIndex(page1, index2);
                this.ChangeIndex(page2, index1);
            }
            else
            {
                this.ChangeIndex(page2, index1);
                this.ChangeIndex(page1, index2);
            }
        }

        public void ChangeIndex(RadPageViewPage page, int newIndex)
        {
            this.Owner.EnablePageIndexChange();
            this.controls.SetChildIndex(page, newIndex);
        }

        public void RemoveAt(int index)
        {
            this.controls.RemoveAt(index);
        }

        public RadPageViewPage this[int index]
        {
            get
            {
                return this.controls[index] as RadPageViewPage;
            }
            set
            {
                throw new InvalidOperationException("Indexer is read-only.");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.controls.GetEnumerator();
        }

        public IEnumerator<RadPageViewPage> GetEnumerator()
        {
            foreach (RadPageViewPage page in this.controls)
            {
                yield return page;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        void ICollection.CopyTo(Array array, int index)
        {
            this.controls.CopyTo(array, index);
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
            get
            {
                return null;
            }
        }

        int IList.Add(object value)
        {
            RadPageViewPage page = value as RadPageViewPage;
            this.Add(page);
            return this.controls.IndexOf(page);
        }

        bool IList.Contains(object value)
        {
            return this.controls.Contains(value as RadPageViewPage);
        }

        int IList.IndexOf(object value)
        {
            return this.controls.IndexOf(value as RadPageViewPage);
        }

        void IList.Insert(int index, object value)
        {
            this.Insert(index, value as RadPageViewPage);
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void IList.Remove(object value)
        {
            this.Remove(value as RadPageViewPage);
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new InvalidOperationException("Indexer is read-only.");
            }
        }

        /// <summary>
        /// Gets the RadPageViewPage instance that matches the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RadPageViewPage this[string name]
        {
            get
            { 
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].Name == name)
                    {
                        return this[i];
                    }
                }

                // if no page found, return null
                return null;
            }
        }

        #endregion
    }
}
