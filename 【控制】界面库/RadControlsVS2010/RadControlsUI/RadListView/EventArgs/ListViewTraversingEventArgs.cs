using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the method that will handle events in <see cref="ListViewTraverser"/>.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ListViewTraversingEventHandler(object sender, ListViewTraversingEventArgs e);

    /// <summary>
    /// Provides data for all events used in <see cref="ListViewTraverser"/>
    /// </summary>
    public class ListViewTraversingEventArgs : ListViewItemEventArgs
    {
        #region Fields
        
        private bool process;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ListViewTraversingEventArgs"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public ListViewTraversingEventArgs(ListViewDataItem content) : base(content)
        { 
            this.process = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ListViewDataItem"/> instance to be processed by <see cref="ListViewTraverser"/>.
        /// </summary>
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
         

        #endregion
    }
}
