using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Encapsulates the layout information for a layout pass of a RadSplitContainer.
    /// </summary>
    public class SplitContainerLayoutInfo
    {
        #region Constructor

        public SplitContainerLayoutInfo()
        {
            this.layoutTargets = new List<SplitPanel>();
            this.autoSizeTargets = new List<SplitPanel>();
            this.absoluteSizeTargets = new List<SplitPanel>();
        }

        #endregion

        #region Methods

        public void Reset()
        {
            this.fillPanel = null;
            this.layoutTargets.Clear();
            this.autoSizeTargets.Clear();
            this.absoluteSizeTargets.Clear();

            this.totalMeasuredLength = 0;
            this.totalMinLength = 0;
            this.autoSizeCountFactor = 0;
            this.availableLength = 0;
            this.contentRect = Rectangle.Empty;
            this.autoSizeLength = 0;
            this.absoluteSizeLength = 0;
            this.totalSplitterLength = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list with all the panels that are target of a layout operation.
        /// </summary>
        public List<SplitPanel> LayoutTargets
        {
            get
            {
                return this.layoutTargets;
            }
        }

        /// <summary>
        /// Gets a list with all the panels that are target of an auto-size logic.
        /// </summary>
        public List<SplitPanel> AutoSizeTargets
        {
            get
            {
                return this.layoutTargets;
            }
        }

        /// <summary>
        /// Gets or sets the auto-size factor which depends on the auto-sizable targets per container.
        /// </summary>
        public float AutoSizeCountFactor
        {
            get
            {
                return this.autoSizeCountFactor;
            }
            set
            {
                this.autoSizeCountFactor = value;
            }
        }

        /// <summary>
        /// Gets or sets the length (width or height, depending on the orientation) that is avilable for layout.
        /// </summary>
        public int AvailableLength
        {
            get
            {
                return this.availableLength;
            }
            set
            {
                this.availableLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the length vailable for all panels with AutoSize mode.
        /// </summary>
        public int AutoSizeLength
        {
            get
            {
                return this.autoSizeLength;
            }
            set
            {
                this.autoSizeLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the length of all panels which are with Absolute size mode.
        /// </summary>
        public int AbsoluteSizeLength
        {
            get
            {
                return this.absoluteSizeLength;
            }
            set
            {
                this.absoluteSizeLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the total length, reserved for splitters.
        /// </summary>
        public int SplitterLength
        {
            get
            {
                return this.splitterLength;
            }
            set
            {
                this.splitterLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the total length, reserved for splitters.
        /// </summary>
        public int TotalSplitterLength
        {
            get
            {
                return this.totalSplitterLength;
            }
            set
            {
                this.totalSplitterLength = value;
            }
        }

        /// <summary>
        /// Gets or sets the content rectangle that represents the layoutable area of the container.
        /// </summary>
        public Rectangle ContentRect
        {
            get
            {
                return this.contentRect;
            }
            set
            {
                this.contentRect = value;
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the associated container.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return this.orientation;
            }
            set
            {
                this.orientation = value;
            }
        }

        #endregion

        #region Fields

        //fill panel behave like Dock = DockStyle.Fill, e.g. it will occupy all the available area
        internal SplitPanel fillPanel;
        internal List<SplitPanel> layoutTargets;
        internal List<SplitPanel> autoSizeTargets;
        internal List<SplitPanel> absoluteSizeTargets;
        internal Orientation orientation;
        internal float autoSizeCountFactor;
        internal int availableLength;
        internal int autoSizeLength;
        internal int totalSplitterLength;
        internal int splitterLength;
        internal int absoluteSizeLength;
        internal int totalMeasuredLength;
        internal int totalMinLength;
        internal Rectangle contentRect;

        #endregion
    }
}
