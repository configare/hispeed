using System;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the method that will handle events in <see cref="PropertyGridTraverser"/>.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PropertyGridTraversingEventHandler(object sender, PropertyGridTraversingEventArgs e);

    /// <summary>
    /// Provides data for all events used in <see cref="PropertyGridTraverser"/>
    /// </summary>
    public class PropertyGridTraversingEventArgs : EventArgs
    {
        #region Fields

        private PropertyGridItemBase item;
        private bool process;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGridTraversingEventArgs"/> class.
        /// </summary>
        /// <param name="item">The content.</param>
        public PropertyGridTraversingEventArgs(PropertyGridItemBase item)
        {
            this.item = item;
            this.process = item != null ? item.Visible : true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PropertyGridItemBase"/> instance to be processed by <see cref="PropertyGridTraverser"/>.
        /// </summary>
        /// <value><c>true</c> if [process PropertyGridItemBase]; otherwise, <c>false</c>.</value>
        public bool Process
        {
            get
            {
                return this.process;
            }
            set
            {
                this.process = value;
            }
        }
        
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        public PropertyGridItemBase Item
        {
            get
            {
                return this.item;
            }
        }

        #endregion
    }
}
