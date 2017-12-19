using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Defines settings for the Docking Guides used by the DragDropService.
    /// </summary>
    [ToolboxItem(false)]
    public class DockingGuidesTemplate : Component, IDockingGuidesTemplate
    {
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DockingGuidesTemplate()
        {
            this.leftImage = new DockingGuideImage();
            this.topImage = new DockingGuideImage();
            this.rightImage = new DockingGuideImage();
            this.bottomImage = new DockingGuideImage();
            this.fillImage = new DockingGuideImage();
            this.centerBackgroundImage = new DockingGuideImage();
            this.dockingHintBackColor = this.DefaultDockingHintBackColor;
            this.dockingHintBorderColor = this.DefaultDockingHintBorderColor;
            this.dockingHintBorderWidth = this.DefaultDockingHintBorderWidth;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the color used to fill the area of the docking hint popup.
        /// </summary>
        [Description("Gets or sets the color used to fill the area of the docking hint popup.")]
        public int DockingHintBorderWidth
        {
            get
            {
                return this.dockingHintBorderWidth;
            }
            set
            {
                this.dockingHintBorderWidth = value;
            }
        }

        /// <summary>
        /// Gets the default DockingHintBackColor value.
        /// </summary>
        protected virtual int DefaultDockingHintBorderWidth
        {
            get
            {
                return 2;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeDockingHintBorderWidth()
        {
            return this.dockingHintBorderWidth != this.DefaultDockingHintBorderWidth;
        }

        /// <summary>
        /// Gets or sets the color used to fill the area of the docking hint popup.
        /// </summary>
        [Description("Gets or sets the color used to fill the area of the docking hint popup.")]
        public Color DockingHintBackColor
        {
            get
            {
                return this.dockingHintBackColor;
            }
            set
            {
                this.dockingHintBackColor = value;
            }
        }

        /// <summary>
        /// Gets the default DockingHintBackColor value.
        /// </summary>
        protected virtual Color DefaultDockingHintBackColor
        {
            get
            {
                return Color.Empty;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeDockingHintBackColor()
        {
            return this.dockingHintBackColor != this.DefaultDockingHintBackColor;
        }

        /// <summary>
        /// Gets or sets the color used to outline the area of the docking hint popup.
        /// </summary>
        [Description("Gets or sets the color used to outline the area of the docking hint popup.")]
        public Color DockingHintBorderColor
        {
            get
            {
                return this.dockingHintBorderColor;
            }
            set
            {
                this.dockingHintBorderColor = value;
            }
        }

        /// <summary>
        /// Gets the default DockingHintBorderColor value.
        /// </summary>
        protected virtual Color DefaultDockingHintBorderColor
        {
            get
            {
                return Color.Empty;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeDockingHintBorderColor()
        {
            return this.dockingHintBorderColor != this.DefaultDockingHintBorderColor;
        }

        /// <summary>
        /// Gets the DockingGuideImage instance that represents the left guide image.
        /// </summary>
        [Description("Gets the DockingGuideImage instance that represents the left guide image.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IDockingGuideImage LeftImage
        {
            get
            {
                return this.leftImage;
            }
        }

        /// <summary>
        /// Gets the DockingGuideImage instance that represents the top guide image.
        /// </summary>
        [Description("Gets the DockingGuideImage instance that represents the top guide image.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IDockingGuideImage TopImage
        {
            get
            {
                return this.topImage;
            }
        }

        /// <summary>
        /// Gets the DockingGuideImage instance that represents the right guide image.
        /// </summary>
        [Description("Gets the DockingGuideImage instance that represents the right guide image.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IDockingGuideImage RightImage
        {
            get
            {
                return this.rightImage;
            }
        }

        /// <summary>
        /// Gets the DockingGuideImage instance that represents the bottom guide image.
        /// </summary>
        [Description("Gets the DockingGuideImage instance that represents the bottom guide image.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IDockingGuideImage BottomImage
        {
            get
            {
                return this.bottomImage;
            }
        }

        /// <summary>
        /// Gets the DockingGuideImage instance that represents the fill guide image.
        /// </summary>
        [Description("Gets the DockingGuideImage instance that represents the fill guide image.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IDockingGuideImage FillImage
        {
            get
            {
                return this.fillImage;
            }
        }

        /// <summary>
        /// Gets the DockingGuideImage instance that represents center guide's background image.
        /// </summary>
        [Description("Gets the DockingGuideImage instance that represents center guide's background image.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IDockingGuideImage CenterBackgroundImage
        {
            get
            {
                return this.centerBackgroundImage;
            }
        }

        #endregion

        #region Fields

        private DockingGuideImage leftImage;
        private DockingGuideImage topImage;
        private DockingGuideImage rightImage;
        private DockingGuideImage bottomImage;
        private DockingGuideImage fillImage;
        private DockingGuideImage centerBackgroundImage;
        private Color dockingHintBackColor;
        private Color dockingHintBorderColor;
        private int dockingHintBorderWidth;

        #endregion
    }
}
