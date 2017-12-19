using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents a serializable version of a <see cref="FloatingWindow">FloatingWindow</see> instance.
    /// </summary>
    public class SerializableFloatingWindow
    {
        #region Fields

        private RadSplitContainer dockContainer;
		private Point location;
		private Size clientSize;
		private Color backColor;
        private int zIndex;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SerializableFloatingWindow()
        {
            dockContainer = new RadSplitContainer();
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="floatingWindow"></param>
        public SerializableFloatingWindow(FloatingWindow floatingWindow)
        {
            //copy properties for seriazliation
            this.dockContainer = floatingWindow.DockContainer;
            this.ClientSize = floatingWindow.ClientSize;
            this.Location = floatingWindow.Location;
            this.BackColor = floatingWindow.BackColor;
            this.zIndex = floatingWindow.ZIndex;
            //TODO
            //this.AllowTheming = floatingWindow.AllowTheming;
            //this.AllowTransparency = floatingWindow.AllowTransparency;
            //this.BackgroundImage = floatingWindow.BackgroundImage;
            //this.BackgroundImageLayout = floatingWindow.BackgroundImageLayout;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the SplitContiner instance that is the root of the hierarchy of DockWindows inside this FloatingWindow
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadSplitContainer DockContainer
        {
            get { return this.dockContainer; }
        }

        /// <summary>
        /// Gets or sets the associated floating window's BackColor.
        /// </summary>
        public Color BackColor
        {
            get
            {
                return this.backColor;
            }
            set
            {
                this.backColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the desktop location of the associated floating window.
        /// </summary>
        public Point Location
        {
            get
            {
                return this.location;
            }
            set
            {
                this.location = value;
            }
        }

        /// <summary>
        /// Gets or sets the client size of the associated floating window.
        /// </summary>
		public Size ClientSize
		{
			set
			{
				clientSize = value;
			}
			get
			{
				return clientSize;
			}
        }

        /// <summary>
        /// Gets or sets the z-idex of the associated floating window.
        /// </summary>
        public int ZIndex
        {
            set
            {
                zIndex = value;
            }
            get
            {
                return zIndex;
            }
        }

        #endregion
    }
}
