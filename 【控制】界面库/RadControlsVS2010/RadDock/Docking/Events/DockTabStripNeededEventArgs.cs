using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A method template used to handle a <see cref="RadDock.DockTabStripNeeded">DockTabStripNeeded</see> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void DockTabStripNeededEventHandler(object sender, DockTabStripNeededEventArgs e);

    /// <summary>
    /// Represents the arguments associated with a <see cref="RadDock.DockTabStripNeeded">DockTabStripNeeded</see> event.
    /// </summary>
    public class DockTabStripNeededEventArgs : EventArgs
    {
        #region Fields

        private DockType dockType;
        private DockTabStrip strip;
        private TabStripAlignment tabAlignment;
        private TabStripTextOrientation tabTextOrientation;
        private bool? tabStripVisible;
        private bool? showCloseButton;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DockTabStripNeededEventArgs">DockTabStripNeededEventArgs</see> class.
        /// </summary>
        /// <param name="type"></param>
        public DockTabStripNeededEventArgs(DockType type)
        {
            this.dockType = type;
            this.tabStripVisible = null;
            this.tabTextOrientation = TabStripTextOrientation.Default;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <see cref="DockType">DockType</see> of the needed strip.\
        /// If the value is <see cref="Telerik.WinControls.UI.Docking.DockType.ToolWindow">ToolWindow</see> then a <see cref="ToolTabStrip">ToolTabStrip</see> instance is needed.
        /// If the value is <see cref="Telerik.WinControls.UI.Docking.DockType.Document">Document</see> then a <see cref="DocumentTabStrip">DocumentTabStrip</see> instance is needed.
        /// </summary>
        public DockType DockType
        {
            get
            {
                return this.dockType;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DockTabStrip">DockTabStrip</see> instance to be used.
        /// </summary>
        public DockTabStrip Strip
        {
            get
            {
                return this.strip;
            }
            set
            {
                //validate the provided value
                if (value != null)
                {
                    switch(this.dockType)
                    {
                        case DockType.Document:
                            if (!(value is DocumentTabStrip))
                            {
                                throw new ArgumentException("Needed a DocumentTabStrip instance.");
                            }
                            break;
                        case DockType.ToolWindow:
                            if (!(value is ToolTabStrip))
                            {
                                throw new ArgumentException("Needed a ToolTabStrip instance.");
                            }
                            break;
                    }
                }
                this.strip = value;
            }
        }

        /// <summary>
        /// Determines whether the TabStripElement should be visible.
        /// </summary>
        public bool? TabStripVisible
        {
            get
            {
                return this.tabStripVisible;
            }
            set
            {
                this.tabStripVisible = value;
            }
        }

        /// <summary>
        /// Determines whether the ShowItemCloseButton will be true for the associated DockTabStrip instance.
        /// </summary>
        public bool? ShowCloseButton
        {
            get
            {
                return this.showCloseButton;
            }
            set
            {
                this.showCloseButton = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the TabStripElement within the TabStrip instance.
        /// </summary>
        public TabStripAlignment TabStripAlignment
        {
            get
            {
                return this.tabAlignment;
            }
            set
            {
                this.tabAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the TabStripElement within the TabStrip instance.
        /// </summary>
        public TabStripTextOrientation TabStripTextOrientation
        {
            get
            {
                return this.tabTextOrientation;
            }
            set
            {
                this.tabTextOrientation = value;
            }
        }

        #endregion
    }
}
