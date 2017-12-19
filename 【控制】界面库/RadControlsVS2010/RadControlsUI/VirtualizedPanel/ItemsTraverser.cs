using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Telerik.WinControls.Data;
using System.Collections.ObjectModel;

namespace Telerik.WinControls.UI
{
    public class ItemsTraverser<T> : ITraverser<T> where T : class
    {
        protected IList<T> collection;
        protected T current;
        protected int position;
        private ItemsTraverser<T> enumerator;

        public ItemsTraverser(IList<T> collection)
        {
            this.collection = collection;
            this.position = -1;
        }

        public IList<T> Collection
        {
            get { return this.collection; }
            set
            {
                this.collection = value;
                this.position = -1;
                if (enumerator != null)
                {
                    this.enumerator.ItemsNavigating -= new ItemsNavigatingEventHandler<T>(Enumerator_ItemsNavigating);
                    this.enumerator = null;
                }
            }
        }

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get
            {
                return this.current;
            }
        }

        public virtual bool MoveNext()
        {
            while (MoveNextCore())
            {
                if (!OnItemsNavigating(current))
                {
                    return true;
                }
            }
            return false;
        }

        public void Reset()
        {
            this.position = -1;
            this.current = null;
        }

        #endregion

        #region IEnumerator<T> Members

        public T Current
        {
            get
            {
                return this.current;
            }
        }

        #endregion

        #region ITraverser Members

        public object Position
        {
            get
            {
                return this.position;
            }
            set
            {
                int newPosition = (int)value;
                if (newPosition >= 0 && newPosition < this.collection.Count)
                {
                    this.position = newPosition;                    
                    this.current = this.collection[this.position];
                }
                else
                {
                    this.position = -1;
                    this.current = null;
                }
            }
        }

        public bool MovePrevious()
        {
            while (MovePreviousCore())
            {
                if (!OnItemsNavigating(current))
                {
                    return true;
                }
            }
            return false;
        }

        public bool MoveToEnd()
        {
            if (position < this.collection.Count - 1)
            {
                position = this.collection.Count - 1;
                current = this.collection[position];
                return true;
            }
            return false;
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            if (enumerator == null)
            {
                this.enumerator = new ItemsTraverser<T>(this.collection);
                this.enumerator.ItemsNavigating += new ItemsNavigatingEventHandler<T>(Enumerator_ItemsNavigating);
            }

            this.enumerator.Position = this.position;
            return enumerator;
        }

        private void Enumerator_ItemsNavigating(object sender, ItemsNavigatingEventArgs<T> e)
        {
            e.SkipItem = this.OnItemsNavigating(e.Item);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
        }

        #endregion

        public event ItemsNavigatingEventHandler<T> ItemsNavigating;

        protected virtual bool OnItemsNavigating(T current)
        {
            if (this.ItemsNavigating != null)
            {
                ItemsNavigatingEventArgs<T> args = new ItemsNavigatingEventArgs<T>(current);
                this.ItemsNavigating(this, args);
                return args.SkipItem;
            }
            return false;
        }

        protected virtual bool MoveNextCore()
        {
            if (position < this.collection.Count - 1)
            {
                this.position++;
                this.current = this.collection[this.position];
                return true;
            }

            return false;
        }

        protected virtual bool MovePreviousCore()
        {
            if (position > 0)
            {
                this.position--;
                this.current = collection[this.position];
                return true;
            }

            this.position = -1;
            this.current = null;
            return false;
        }
    }
}
