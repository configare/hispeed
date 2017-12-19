using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>Represents parameters of the scroll bar such as small change and 
    /// large change in the scrolling position.</summary>
    public struct ScrollBarParameters
    {
        /// <summary>
        /// Represents the minimum value of the scrolling position.
        /// </summary>
        public int Minimum;
        
        /// <summary>
        /// Represents the maximum value of the scrolling position.
        /// </summary>
        public int Maximum;
        
        /// <summary>
        /// Represents a small change in the scrolling position; the value which will be
        /// added or substracted from the current position in case of small change.
        /// </summary>
        public int SmallChange;

        /// <summary>
        /// Represents a large change in the scrolling position; the value which will be
        /// added or substracted from the current position in case of large change.
        /// </summary>
        public int LargeChange;

        /// <summary>Initializes a new ScrollBarParameters structure.
        /// </summary>
        /// <param name="minimum">Initializes the <see cref="Minimum">minimum</see> value of the scrolling.</param>
        /// <param name="maximum">Initializes the <see cref="Maximum">maximum</see> value of the scrolling.</param>
        /// <param name="smallChange">Initializes the <see cref="SmallChange">small change</see> value.</param>
        /// <param name="largeChange">Initializes the <see cref="LargeChange">large change</see> value.</param>
        public ScrollBarParameters(int minimum, int maximum,
            int smallChange, int largeChange)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.SmallChange = smallChange;
            this.LargeChange = largeChange;
        }
    }
}
