using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Data
{
    public interface IReadOnlyCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        int Count { get; }
        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <value></value>
        T this[int index] { get; }
        /// <summary>
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains(T value);
        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="index">The index.</param>
        void CopyTo(T[] array, int index);
        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        int IndexOf(T value);
    }
}
