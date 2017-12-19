using System;
using System.Collections;
using Telerik.WinControls;

namespace Telerik.WinControls
{
    /// <summary>
    ///     <para>
    ///       A collection that stores <see cref='Telerik.WinControls.XmlPropertySettingGroup'/> objects.
    ///    </para>
    /// </summary>
    //[Serializable()]
    public class XmlPropertySettingGroupCollection : CollectionBase
    {

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/>.
        ///    </para>
        /// </summary>
        public XmlPropertySettingGroupCollection()
        {
        }

        public XmlPropertySettingGroupCollection(int capacity): base(capacity)
        {
        }

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/> based on another <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/>.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/> from which the contents are copied
        /// </param>
        public XmlPropertySettingGroupCollection(XmlPropertySettingGroupCollection value)
        {
            this.AddRange(value);
        }

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/> containing any array of <see cref='Telerik.WinControls.XmlPropertySettingGroup'/> objects.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A array of <see cref='Telerik.WinControls.XmlPropertySettingGroup'/> objects with which to intialize the collection
        /// </param>
        public XmlPropertySettingGroupCollection(XmlPropertySettingGroup[] value)
        {
            this.AddRange(value);
        }

        /// <summary>
        /// <para>Represents the entry at the specified index of the <see cref='Telerik.WinControls.XmlPropertySettingGroup'/>.</para>
        /// </summary>
        /// <param name='index'><para>The zero-based index of the entry to locate in the collection.</para></param>
        /// <value>
        ///    <para> The entry at the specified index of the collection.</para>
        /// </value>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
        public XmlPropertySettingGroup this[int index]
        {
            get
            {
                return ((XmlPropertySettingGroup)(List[index]));
            }
            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        ///    <para>Adds a <see cref='Telerik.WinControls.XmlPropertySettingGroup'/> with the specified value to the 
        ///    <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.XmlPropertySettingGroup'/> to add.</param>
        /// <returns>
        ///    <para>The index at which the new element was inserted.</para>
        /// </returns>
        public int Add(XmlPropertySettingGroup value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// <para>Copies the elements of an array to the end of the <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/>.</para>
        /// </summary>
        /// <param name='value'>
        ///    An array of type <see cref='Telerik.WinControls.XmlPropertySettingGroup'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        public void AddRange(XmlPropertySettingGroup[] value)
        {
            for (int i = 0; (i < value.Length); i = (i + 1))
            {
                this.Add(value[i]);
            }
        }

        /// <summary>
        ///     <para>
        ///       Adds the contents of another <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/> to the end of the collection.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///    A <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        public void AddRange(XmlPropertySettingGroupCollection value)
        {
            for (int i = 0; (i < value.Count); i = (i + 1))
            {
                this.Add(value[i]);
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether the 
        ///    <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/> contains the specified <see cref='Telerik.WinControls.XmlPropertySettingGroup'/>.</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.XmlPropertySettingGroup'/> to locate.</param>
        /// <returns>
        /// <para><see langword='true'/> if the <see cref='Telerik.WinControls.XmlPropertySettingGroup'/> is contained in the collection; 
        ///   otherwise, <see langword='false'/>.</para>
        /// </returns>
        public bool Contains(XmlPropertySettingGroup value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// <para>Copies the <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </summary>
        /// <param name='array'><para>The one-dimensional <see cref='System.Array'/> that is the destination of the values copied from <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/> .</para></param>
        /// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <exception cref='System.ArgumentException'><para><paramref name='array'/> is multidimensional.</para> <para>-or-</para> <para>The number of elements in the <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/> is greater than the available space between <paramref name='index'/> and the end of <paramref name='array'/>.</para></exception>
        /// <exception cref='System.ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is less than <paramref name='array'/>'s lowbound. </exception>
        public void CopyTo(XmlPropertySettingGroup[] array, int index)
        {
            List.CopyTo(array, index);
        }

        /// <summary>
        ///    <para>Returns the index of a <see cref='Telerik.WinControls.XmlPropertySettingGroup'/> in 
        ///       the <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.XmlPropertySettingGroup'/> to locate.</param>
        /// <returns>
        /// <para>The index of the <see cref='Telerik.WinControls.XmlPropertySettingGroup'/> of <paramref name='value'/> in the 
        /// <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/>, if found; otherwise, -1.</para>
        /// </returns>
        public int IndexOf(XmlPropertySettingGroup value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// <para>Inserts a <see cref='Telerik.WinControls.XmlPropertySettingGroup'/> into the <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/> at the specified index.</para>
        /// </summary>
        /// <param name='index'>The zero-based index where <paramref name='value'/> should be inserted.</param>
        /// <param name=' value'>The <see cref='Telerik.WinControls.XmlPropertySettingGroup'/> to insert.</param>
        /// <returns><para>None.</para></returns>
        public void Insert(int index, XmlPropertySettingGroup value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        ///    <para>Returns an enumerator that can iterate through 
        ///       the <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/> .</para>
        /// </summary>
        /// <returns><para>None.</para></returns>
        public new XmlPropertySettingGroupEnumerator GetEnumerator()
        {
            return new XmlPropertySettingGroupEnumerator(this);
        }

        /// <summary>
        ///    <para> Removes a specific <see cref='Telerik.WinControls.XmlPropertySettingGroup'/> from the 
        ///    <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.XmlPropertySettingGroup'/> to remove from the <see cref='Telerik.WinControls.XmlPropertySettingGroupCollection'/> .</param>
        /// <returns><para>None.</para></returns>
        /// <exception cref='System.ArgumentException'><paramref name='value'/> is not found in the Collection. </exception>
        public void Remove(XmlPropertySettingGroup value)
        {
            //List.Remove(value);
            int index = List.IndexOf(value);
            if (index >= 0)
            {
                List.RemoveAt(index);
            }
        }

        public class XmlPropertySettingGroupEnumerator : object, IEnumerator
        {

            private IEnumerator baseEnumerator;

            private IEnumerable temp;

            public XmlPropertySettingGroupEnumerator(XmlPropertySettingGroupCollection mappings)
            {
                this.temp = ((IEnumerable)(mappings));
                this.baseEnumerator = temp.GetEnumerator();
            }

            public XmlPropertySettingGroup Current
            {
                get
                {
                    return ((XmlPropertySettingGroup)(baseEnumerator.Current));
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return baseEnumerator.Current;
                }
            }

            public bool MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            bool IEnumerator.MoveNext()
            {
                return baseEnumerator.MoveNext();
            }

            public void Reset()
            {
                baseEnumerator.Reset();
            }

            void IEnumerator.Reset()
            {
                baseEnumerator.Reset();
            }
        }
    }
}