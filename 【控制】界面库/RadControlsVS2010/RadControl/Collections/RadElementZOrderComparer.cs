using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a Z-order comparer. The Z-Order determines the overlapping of the 
    /// RadElements.
    /// </summary>
    public class RadElementZOrderComparer : Comparer<RadElement>
    {
        private bool reverse;
        /// <summary>
        /// Initializes a new instance of the RadElementZOrderComparer class.
        /// </summary>
        /// <param name="reverse"></param>
        public RadElementZOrderComparer(bool reverse)
        {
            this.reverse = reverse;
        }
        /// <summary>
        /// Compares the Z-order of the two RadElement arguments. Retrieves 0 if the
        /// two elements are equal, positive number if the first element has a greater 
        /// Z-Order than the second argument, and a negative number otherwise. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override int Compare(RadElement x, RadElement y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            int reverseFactor = this.reverse ? -1 : 1;

            if (x.ZIndex > y.ZIndex)
                return 1 * reverseFactor;
            else
                if (x.ZIndex < y.ZIndex)
                {
                    return -1 * reverseFactor;
                }
                else
                    return 0;
        }
    }
}
