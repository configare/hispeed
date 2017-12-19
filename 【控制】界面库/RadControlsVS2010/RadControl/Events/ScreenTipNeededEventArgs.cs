using System;
using System.Drawing;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents the method that will handle the ScreenTipNeeded event of a RadControl. 
    /// </summary>
    public delegate void ScreenTipNeededEventHandler(object sender, ScreenTipNeededEventArgs e);

    /// <summary>
    /// Provides data for the ScreenTipNeeded event. 
    /// </summary>
    public class ScreenTipNeededEventArgs : EventArgs
    {
        #region Fields

        private RadItem item;
        private int delay = 900;
        private Size offset = Size.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenTipNeededEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ScreenTipNeededEventArgs(RadItem item)
            : this(item, Size.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenTipNeededEventArgs"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="offset">The offset.</param>
        public ScreenTipNeededEventArgs(RadItem item, Size offset)
        {
            this.item = item;
            this.offset = offset;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the item for which the ScreenTipNeeded event occurs.
        /// </summary>
        /// <value>The item.</value>
        public RadItem Item
        {
            get
            {
                return item;
            }
        }

        /// <summary>
        /// Gets or sets the delay.
        /// </summary>
        /// <value>The delay.</value>
        public int Delay
        {
            get
            {
                return this.delay;
            }
            set
            {
                this.delay = value;
            }
        }

        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public Size Offset
        {
            get
            {
                return this.offset;
            }
            set
            {
                this.offset = value;
            }
        }

        #endregion
    }
}
