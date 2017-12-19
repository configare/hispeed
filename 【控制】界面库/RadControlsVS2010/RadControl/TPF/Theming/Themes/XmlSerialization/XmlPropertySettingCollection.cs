using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.WinControls;

namespace Telerik.WinControls
{
    /// <summary>
    ///     <para>
    ///       A collection that stores <see cref='Telerik.WinControls.XmlPropertySetting'/> objects.
    ///    </para>
    /// </summary>
    //[Serializable()]
    public class XmlPropertySettingCollection : CollectionBase, ICollection<XmlPropertySetting>
    {

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='Telerik.WinControls.XmlPropertySettingCollection'/>.
        ///    </para>
        /// </summary>
        public XmlPropertySettingCollection()
        {
        }

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='Telerik.WinControls.XmlPropertySettingCollection'/> based on another <see cref='Telerik.WinControls.XmlPropertySettingCollection'/>.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A <see cref='Telerik.WinControls.XmlPropertySettingCollection'/> from which the contents are copied
        /// </param>
        public XmlPropertySettingCollection(XmlPropertySettingCollection value)
        {
            this.AddRange(value);
        }

        /// <summary>
        ///     <para>
        ///       Initializes a new instance of <see cref='Telerik.WinControls.XmlPropertySettingCollection'/> containing any array of <see cref='Telerik.WinControls.XmlPropertySetting'/> objects.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///       A array of <see cref='Telerik.WinControls.XmlPropertySetting'/> objects with which to intialize the collection
        /// </param>
        public XmlPropertySettingCollection(XmlPropertySetting[] value)
        {
            this.AddRange(value);
        }

        /// <summary>
        /// <para>Represents the entry at the specified index of the <see cref='Telerik.WinControls.XmlPropertySetting'/>.</para>
        /// </summary>
        /// <param name='index'><para>The zero-based index of the entry to locate in the collection.</para></param>
        /// <value>
        ///    <para> The entry at the specified index of the collection.</para>
        /// </value>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is outside the valid range of indexes for the collection.</exception>
        public XmlPropertySetting this[int index]
        {
            get
            {
                return ((XmlPropertySetting)(List[index]));
            }
            set
            {
                List[index] = value;
            }
        }

        /// <summary>
        ///    <para>Adds a <see cref='Telerik.WinControls.XmlPropertySetting'/> with the specified value to the 
        ///    <see cref='Telerik.WinControls.XmlPropertySettingCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.XmlPropertySetting'/> to add.</param>
        /// <returns>
        ///    <para>The index at which the new element was inserted.</para>
        /// </returns>
        public int Add(XmlPropertySetting value)
        {
            return List.Add(value);
        }

        void ICollection<XmlPropertySetting>.Add(XmlPropertySetting value)
        {
            this.Add(value);
        }

        bool ICollection<XmlPropertySetting>.Remove(XmlPropertySetting value)
        {
            int index = this.IndexOf(value);
            if (index >= 0)
            {
                this.RemoveAt(index);
                return true;
            }

            return false;
        }

        /// <summary>
        /// <para>Copies the elements of an array to the end of the <see cref='Telerik.WinControls.XmlPropertySettingCollection'/>.</para>
        /// </summary>
        /// <param name='value'>
        ///    An array of type <see cref='Telerik.WinControls.XmlPropertySetting'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        public void AddRange(XmlPropertySetting[] value)
        {
            for (int i = 0; (i < value.Length); i = (i + 1))
            {
                this.Add(value[i]);
            }
        }

        /// <summary>
        ///     <para>
        ///       Adds the contents of another <see cref='Telerik.WinControls.XmlPropertySettingCollection'/> to the end of the collection.
        ///    </para>
        /// </summary>
        /// <param name='value'>
        ///    A <see cref='Telerik.WinControls.XmlPropertySettingCollection'/> containing the objects to add to the collection.
        /// </param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        public void AddRange(XmlPropertySettingCollection value)
        {
            for (int i = 0; (i < value.Count); i = (i + 1))
            {
                this.Add(value[i]);
            }
        }

        /// <summary>
        /// <para>Gets a value indicating whether the 
        ///    <see cref='Telerik.WinControls.XmlPropertySettingCollection'/> contains the specified <see cref='Telerik.WinControls.XmlPropertySetting'/>.</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.XmlPropertySetting'/> to locate.</param>
        /// <returns>
        /// <para><see langword='true'/> if the <see cref='Telerik.WinControls.XmlPropertySetting'/> is contained in the collection; 
        ///   otherwise, <see langword='false'/>.</para>
        /// </returns>
        public bool Contains(XmlPropertySetting value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// <para>Copies the <see cref='Telerik.WinControls.XmlPropertySettingCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </summary>
        /// <param name='array'><para>The one-dimensional <see cref='System.Array'/> that is the destination of the values copied from <see cref='Telerik.WinControls.XmlPropertySettingCollection'/> .</para></param>
        /// <param name='index'>The index in <paramref name='array'/> where copying begins.</param>
        /// <returns>
        ///   <para>None.</para>
        /// </returns>
        /// <exception cref='System.ArgumentException'><para><paramref name='array'/> is multidimensional.</para> <para>-or-</para> <para>The number of elements in the <see cref='Telerik.WinControls.XmlPropertySettingCollection'/> is greater than the available space between <paramref name='index'/> and the end of <paramref name='array'/>.</para></exception>
        /// <exception cref='System.ArgumentNullException'><paramref name='array'/> is <see langword='null'/>. </exception>
        /// <exception cref='System.ArgumentOutOfRangeException'><paramref name='index'/> is less than <paramref name='array'/>'s lowbound. </exception>
        public void CopyTo(XmlPropertySetting[] array, int index)
        {
            List.CopyTo(array, index);
        }

        /// <summary>
        ///    <para>Returns the index of a <see cref='Telerik.WinControls.XmlPropertySetting'/> in 
        ///       the <see cref='Telerik.WinControls.XmlPropertySettingCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.XmlPropertySetting'/> to locate.</param>
        /// <returns>
        /// <para>The index of the <see cref='Telerik.WinControls.XmlPropertySetting'/> of <paramref name='value'/> in the 
        /// <see cref='Telerik.WinControls.XmlPropertySettingCollection'/>, if found; otherwise, -1.</para>
        /// </returns>
        public int IndexOf(XmlPropertySetting value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// <para>Inserts a <see cref='Telerik.WinControls.XmlPropertySetting'/> into the <see cref='Telerik.WinControls.XmlPropertySettingCollection'/> at the specified index.</para>
        /// </summary>
        /// <param name='index'>The zero-based index where <paramref name='value'/> should be inserted.</param>
        /// <param name=' value'>The <see cref='Telerik.WinControls.XmlPropertySetting'/> to insert.</param>
        /// <returns><para>None.</para></returns>
        public void Insert(int index, XmlPropertySetting value)
        {
            List.Insert(index, value);
        }        

        /// <summary>
        ///    <para>Returns an enumerator that can iterate through 
        ///       the <see cref='Telerik.WinControls.XmlPropertySettingCollection'/> .</para>
        /// </summary>
        /// <returns><para>None.</para></returns>
        public new XmlPropertySettingEnumerator GetEnumerator()
        {
            return new XmlPropertySettingEnumerator(this);
        }

        IEnumerator<XmlPropertySetting> IEnumerable<XmlPropertySetting>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        ///    <para> Removes a specific <see cref='Telerik.WinControls.XmlPropertySetting'/> from the 
        ///    <see cref='Telerik.WinControls.XmlPropertySettingCollection'/> .</para>
        /// </summary>
        /// <param name='value'>The <see cref='Telerik.WinControls.XmlPropertySetting'/> to remove from the <see cref='Telerik.WinControls.XmlPropertySettingCollection'/> .</param>
        /// <returns><para>None.</para></returns>
        /// <exception cref='System.ArgumentException'><paramref name='value'/> is not found in the Collection. </exception>
        public void Remove(XmlPropertySetting value)
        {
            int index = List.IndexOf(value);
            if (index >= 0)
            {
                List.RemoveAt(index);
            }
        }

        bool ICollection<XmlPropertySetting>.IsReadOnly
        {
            get
            { 
                return base.InnerList.IsReadOnly;
            }
        }

        public class XmlPropertySettingEnumerator : IEnumerator, IEnumerator<XmlPropertySetting>
        {

            private IEnumerator baseEnumerator;

            private IEnumerable temp;

            public XmlPropertySettingEnumerator(XmlPropertySettingCollection mappings)
            {
                this.temp = ((IEnumerable)(mappings));
                this.baseEnumerator = temp.GetEnumerator();
            }

            public XmlPropertySetting Current
            {
                get
                {
                    return ((XmlPropertySetting)(baseEnumerator.Current));
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

            void IDisposable.Dispose()
            { 
            }
        }
    }
}
