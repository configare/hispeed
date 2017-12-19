using Telerik.WinControls.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a separator which is just a line separating one group of 
    /// controls from another. The RadSeparator is a simple wrapper of the 
    /// <see cref="SeparatorElement">RadSeparatorElement</see> class.
    /// </summary>
    [ToolboxItem(true)]
    public class RadSeparator : RadControl
    {
        #region Fields

        SeparatorElement separatorElement;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public bool ShowShadow
        {
            get
            {
                return this.SeparatorElement.ShowShadow;
            }
            set
            {
                this.SeparatorElement.ShowShadow = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(2)]
        public Point ShadowOffset
        {
            get
            {
                return this.SeparatorElement.ShadowOffset;
            }
            set
            {
                this.SeparatorElement.ShadowOffset = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(Orientation.Horizontal)]
        public Orientation Orientation
        {
            get
            {
                return this.SeparatorElement.Orientation;
            }
            set
            {
                this.SeparatorElement.Orientation = value;
            }
        }

        /// <summary>
        /// Gets the instance of RadSeparatorElement wrapped by this control. RadSeparatorElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadSeparator.
        /// </summary>
        public SeparatorElement SeparatorElement
        {
            get
            {
                return this.separatorElement;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(200, 4);
            }
        }

        #endregion

        #region Initialization

        public RadSeparator()
        {
            this.MinimumSize = new Size(0, 0);
            this.AutoSize = false;
        }

        protected override void CreateChildItems(RadElement parent)
        {
            separatorElement = new SeparatorElement();
            separatorElement.StretchHorizontally = true;
            separatorElement.StretchVertically = true;
            parent.Children.Add(separatorElement);
        }

        #endregion
    }
}
