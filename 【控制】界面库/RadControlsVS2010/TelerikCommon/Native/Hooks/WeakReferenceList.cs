using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;

namespace Telerik.WinControls
{
    /// <summary>
    /// Wraps instances of type T in WeakReference and stores them in a List.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeakReferenceList<T> : IEnumerable<T> where T : class
    {
        #region Constructor

        public WeakReferenceList()
        {
            list = new List<WeakReference>();
            autoCleanNonAlive = false;
            trackResurrection = false;
        }

        public WeakReferenceList(bool cleanNonAlive)
            : this()
        {
            this.autoCleanNonAlive = cleanNonAlive;
        }

        public WeakReferenceList(bool cleanNonAlive, bool trackResurrection)
            : this(cleanNonAlive)
        {
            this.trackResurrection = trackResurrection;
        }

        #endregion

        #region Collection Methods

        public void Add(T value)
        {
            this.InsertCore(this.Count, value);
        }

        public void Insert(int index, T value)
        {
            WeakReference reference = new WeakReference(value, this.trackResurrection);
            this.InsertCore(index, value);
        }

        protected virtual void InsertCore(int index, T value)
        {
            WeakReference reference = new WeakReference(value, this.trackResurrection);
            this.list.Insert(index, reference);
        }

        public int IndexOf(T value)
        {
            int count = this.list.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                WeakReference reference = this.list[i];
                if (reference.IsAlive)
                {
                    if (reference.Target == value)
                    {
                        return i;
                    }
                }
                else if (this.autoCleanNonAlive)
                {
                    this.list.RemoveAt(i);
                }
            }

            return -1;
        }

        public void Remove(T value)
        {
            int index = this.IndexOf(value);
            if (index >= 0)
            {
                this.list.RemoveAt(index);
            }
        }

        public void Clear()
        {
            this.list.Clear();
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this.list.Count)
            {
                throw new IndexOutOfRangeException();
            }

            this.list.RemoveAt(index);
        }

        public void CleanNonAlive()
        {
            int count = this.list.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                WeakReference reference = this.list[i];
                if (!reference.IsAlive)
                {
                    this.list.RemoveAt(i);
                }
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= this.list.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                return (T)this.list[index].Target;
            }
            set
            {
                if (index < 0 || index >= this.list.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                this.list[index] = new WeakReference(value, this.trackResurrection);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.list.Count; i++)
            {
                WeakReference reference = this.list[i];
                if (!reference.IsAlive)
                {
                    if (this.autoCleanNonAlive)
                    {
                        this.list.RemoveAt(i--);
                    }
                    continue;
                }

                yield return (T)reference.Target;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this.list.Count; i++)
            {
                WeakReference reference = this.list[i];
                if (!reference.IsAlive)
                {
                    if (this.autoCleanNonAlive)
                    {
                        this.list.RemoveAt(i--);
                    }
                    continue;
                }

                yield return (T)reference.Target;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether the list will automatically remove any containednon-alive weak reference.
        /// </summary>
        public bool AutoCleanNonAlive
        {
            get
            {
                return this.autoCleanNonAlive;
            }
            set
            {
                this.autoCleanNonAlive = value;
            }
        }

        /// <summary>
        /// Determines the WeakReference.TrackRessurection property for all T instances added.
        /// </summary>
        public bool TrackRessurection
        {
            get
            {
                return this.trackResurrection;
            }
            set
            {
                this.trackResurrection = value;
            }
        }

        /// <summary>
        /// Gets the count of the list.
        /// </summary>
        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        /// <summary>
        /// Gets the internal List used to store all instances.
        /// </summary>
        protected List<WeakReference> List
        {
            get
            {
                return this.list;
            }
        }

        #endregion

        #region Fields

        private List<WeakReference> list;
        private bool autoCleanNonAlive;
        private bool trackResurrection;

        #endregion
    }
}
