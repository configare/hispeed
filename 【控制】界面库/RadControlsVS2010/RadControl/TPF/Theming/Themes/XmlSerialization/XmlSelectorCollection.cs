using System;
using System.Collections;
using Telerik.WinControls;

namespace Telerik.WinControls
{
    /// <summary>
    ///     <para>
    ///       A collection that stores <see cref='Telerik.WinControls.XmlElementSelector'/> objects.
    ///    </para>
    /// </summary>
    //[Serializable()]
    public class XmlSelectorCollection : CollectionBase
    {

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='Telerik.WinControls.XmlSelectorCollection'/>.
        ///    </para>
        /// </summary>
        public XmlSelectorCollection(): base(1)
        {
        }

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='Telerik.WinControls.XmlSelectorCollection'/>.
        ///    </para>
        /// </summary>
        public XmlSelectorCollection(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='Telerik.WinControls.XmlSelectorCollection'/> based on another <see cref='Telerik.WinControls.XmlSelectorCollection'/>.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A <see cref='Telerik.WinControls.XmlSelectorCollection'/> from which the contents are copied
        /// </param>
        public XmlSelectorCollection(XmlSelectorCollection value)
        {
            this.AddRange(value);
        }

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='Telerik.WinControls.XmlSelectorCollection'/> containing any array of <see cref='Telerik.WinControls.XmlElementSelector'/> objects.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A array of <see cref='Telerik.WinControls.XmlElementSelector'/> objects with which to intialize the collection
        /// </param>
        public XmlSelectorCollection(XmlElementSelector[] value)
        {
            this.AddRange(value);
        }

        /// <summary>
        /// <para>Represents the entry at the specified index of the <see cref='Telerik.WinControls.XmlElementSelector'/>.</para>
        /// </summary>
        /// <param name='index'><para>The zero-based index of the entry to locate in the collection.</para></param>
        /// <value>
        ///    <para> The entry at the specified index of the collection.</para>
        /// </value>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
        public XmlElementSelector this[int index]
        {
            get
            {
                return ((XmlElementSelector)(List[index]));
            }
            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        ///    <para>Adds a <see cref='Telerik.WinControls.XmlElementSelector'/> with the specified value to the 
        ///    <see cref='Telerik.WinControls.XmlSelectorCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.XmlElementSelector'/> to add.</param>
        /// <returns>
        ///    <para>The index at which the new element was inserted.</para>
        /// </returns>
        public int Add(XmlElementSelector value)
        {
            return List.Add(value);
        }

        /// <summary>
        /// <para>Copies the elements of an array to the end of the <see cref='Telerik.WinControls.XmlSelectorCollection'/>.</para>
        /// </summary>
        /// <param name='value'>
        ///    An array of type <see cref='Telerik.WinControls.XmlElementSelector'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        public void AddRange(XmlElementSelector[] value)
        {
            for (int i = 0; (i < value.Length); i = (i + 1))
            {
                this.Add(value[i]);
            }
        }

        /// <summary>
        ///     <para>
        ///       Adds the contents of another <see cref='Telerik.WinControls.XmlSelectorCollection'/> to the end of the collection.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///    A <see cref='Telerik.WinControls.XmlSelectorCollection'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        public void AddRange(XmlSelectorCollection value)
        {
            for (int i = 0; (i < value.Count); i = (i + 1))
            {
                this.Add(value[i]);
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether the 
        ///    <see cref='Telerik.WinControls.XmlSelectorCollection'/> contains the specified <see cref='Telerik.WinControls.XmlElementSelector'/>.</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.XmlElementSelector'/> to locate.</param>
        /// <returns>
        /// <para><see langword='true'/> if the <see cref='Telerik.WinControls.XmlElementSelector'/> is contained in the collection; 
        ///   otherwise, <see langword='false'/>.</para>
        /// </returns>
        public bool Contains(XmlElementSelector value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// <para>Copies the <see cref='Telerik.WinControls.XmlSelectorCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </summary>
        /// <param name='array'><para>The one-dimensional <see cref='System.Array'/> that is the destination of the values copied from <see cref='Telerik.WinControls.XmlSelectorCollection'/> .</para></param>
        /// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <exception cref='System.ArgumentException'><para><paramref name='array'/> is multidimensional.</para> <para>-or-</para> <para>The number of elements in the <see cref='Telerik.WinControls.XmlSelectorCollection'/> is greater than the available space between <paramref name='index'/> and the end of <paramref name='array'/>.</para></exception>
        /// <exception cref='System.ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is less than <paramref name='array'/>'s lowbound. </exception>
        public void CopyTo(XmlElementSelector[] array, int index)
        {
            List.CopyTo(array, index);
        }

        /// <summary>
        ///    <para>Returns the index of a <see cref='Telerik.WinControls.XmlElementSelector'/> in 
        ///       the <see cref='Telerik.WinControls.XmlSelectorCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.XmlElementSelector'/> to locate.</param>
        /// <returns>
        /// <para>The index of the <see cref='Telerik.WinControls.XmlElementSelector'/> of <paramref name='value'/> in the 
        /// <see cref='Telerik.WinControls.XmlSelectorCollection'/>, if found; otherwise, -1.</para>
        /// </returns>
        public int IndexOf(XmlElementSelector value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// <para>Inserts a <see cref='Telerik.WinControls.XmlElementSelector'/> into the <see cref='Telerik.WinControls.XmlSelectorCollection'/> at the specified index.</para>
        /// </summary>
        /// <param name='index'>The zero-based index where <paramref name='value'/> should be inserted.</param>
        /// <param name='value'>The <see cref='Telerik.WinControls.XmlElementSelector'/> to insert.</param>
        public void Insert(int index, XmlElementSelector value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        ///    <para>Returns an enumerator that can iterate through 
        ///       the <see cref='Telerik.WinControls.XmlSelectorCollection'/> .</para>
        /// </summary>
        /// <returns><para>None.</para></returns>
        public new XmlSelectorEnumerator GetEnumerator()
        {
            return new XmlSelectorEnumerator(this);
        }

        /// <summary>
        ///    <para> Removes a specific <see cref='Telerik.WinControls.XmlElementSelector'/> from the 
        ///    <see cref='Telerik.WinControls.XmlSelectorCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.XmlElementSelector'/> to remove from the <see cref='Telerik.WinControls.XmlSelectorCollection'/> .</param>
        /// <returns><para>None.</para></returns>
        /// <exception cref='System.ArgumentException'><paramref name='value'/> is not found in the Collection. </exception>
        public void Remove(XmlElementSelector value)
        {
            List.Remove(value);
        }

        public class XmlSelectorEnumerator : object, IEnumerator
        {

            private IEnumerator baseEnumerator;

            private IEnumerable temp;

            public XmlSelectorEnumerator(XmlSelectorCollection mappings)
            {
                this.temp = ((IEnumerable)(mappings));
                this.baseEnumerator = temp.GetEnumerator();
            }

            public XmlElementSelector Current
            {
                get
                {
                    return ((XmlElementSelector)(baseEnumerator.Current));
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
