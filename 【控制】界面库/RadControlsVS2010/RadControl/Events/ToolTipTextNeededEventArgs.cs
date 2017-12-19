using System;
using System.Drawing;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents the method that will handle the ToolTipTextNeeded event of a RadCOntrol. 
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A ToolTipTextNeededEventArgs that contains the event data.</param>
    public delegate void ToolTipTextNeededEventHandler(object sender, ToolTipTextNeededEventArgs e);

    /// <summary>
    /// Provides data for the ToolTipTextNeeded event. 
    /// </summary>
    public class ToolTipTextNeededEventArgs : EventArgs
    {
        #region Fields

        private string toolTipText = null;
        private Size offset = Size.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolTipTextNeededEventArgs"/> class.
        /// </summary>
        /// <param name="toolTipText">The tool tip text.</param>
        public ToolTipTextNeededEventArgs(string toolTipText) :
            this(toolTipText, Size.Empty)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolTipTextNeededEventArgs"/> class.
        /// </summary>
        public ToolTipTextNeededEventArgs()
            : this(String.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolTipTextNeededEventArgs"/> class.
        /// </summary>
        /// <param name="toolTipText">The tool tip text.</param>
        /// <param name="offset">The offset.</param>
        public ToolTipTextNeededEventArgs(string toolTipText, Size offset)
        {
            this.offset = offset;
            this.toolTipText = toolTipText;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ToolTip text. 
        /// </summary>
        public string ToolTipText
        {
            get
            {
                return this.toolTipText;
            }
            set
            {
                this.toolTipText = value;
            }
        }

        /// <summary>
        /// Gets or sets the offset from the Cursor.HotSpot
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
