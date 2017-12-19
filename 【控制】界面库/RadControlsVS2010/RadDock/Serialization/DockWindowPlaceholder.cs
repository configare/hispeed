using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A serializable placeholder, used primarily during RadDock's serialization process.
    /// </summary>
    public class DockWindowPlaceholder: DockWindow
    {
        #region Fields

        private DockWindow target;
        private string dockWindowName = string.Empty;
        private string dockWindowText = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DockWindowPlaceholder()
        {
        }

        /// <summary>
        /// Initializes the placeholder for the specified DockWindow instance.
        /// </summary>
        /// <param name="target"></param>
        public DockWindowPlaceholder(DockWindow target)
        {
            this.target = target;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the associated DockWindow instance.
        /// </summary>
        public string DockWindowName
        {
            get
            {
                if (this.target != null)
                {
                    return this.target.Name;
                }

                return this.dockWindowName;
            }
            set
            {
                if (this.target == null)
                {
                    this.dockWindowName = value;
                }
                else
                {
                    throw new InvalidOperationException("Target window name can not be assigned");
                }
            }
        }

        public bool ShouldSerializeDockWindowName()
        {
            return !string.IsNullOrEmpty(this.DockWindowName);
        }

        /// <summary>
        /// Gets the name of the associated DockWindow instance.
        /// </summary>
        public string DockWindowText
        {
            get
            {
                if (this.target != null)
                {
                    return this.target.Text;
                }

                return this.dockWindowText;
            }
            set
            {
                if (this.target == null)
                {
                    this.dockWindowText = value;
                }
                else
                {
                    throw new InvalidOperationException("Target window name can not be assigned");
                }
            }
        }

        public bool ShouldSerializeDockWindowText()
        {
            return !string.IsNullOrEmpty(this.DockWindowText);
        }

        #endregion
    }
}
