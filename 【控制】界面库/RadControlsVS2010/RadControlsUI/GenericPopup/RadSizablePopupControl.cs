using System.Drawing.Design;
using Telerik.WinControls.Layouts;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System;
using System.Runtime.InteropServices;


namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents a pop-up form that exposes sizing-grip and
    /// thus can be resized by the user.
    /// </summary>
    [ToolboxItem(false)]
    public class RadSizablePopupControl : RadPopupControlBase
    {
        #region Fields

        private SizeGripElement sizingGrip;
        private DockLayoutPanel sizingGripDockLayout;

        #endregion

        #region Ctor

        static RadSizablePopupControl()
        {
            new Telerik.WinControls.Themes.ControlDefault.SizablePopup().DeserializeTheme();
        }

        /// <summary>
        /// Creates an instance of the RadSizablePopupControl class.
        /// </summary>
        /// <param name="owner">The owner of the popup-form</param>
        public RadSizablePopupControl(RadItem owner)
            : base(owner)
        {
            this.VerticalAlignmentCorrectionMode = AlignmentCorrectionMode.SnapToOuterEdges;
        }

        #endregion

        #region Properties

        public override string ThemeClassName
        {
            get
            {
                return typeof(RadSizablePopupControl).FullName;
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        /// <summary>
        /// Gets or sets a value of the <see cref=" Telerik.WinControls.UI.SizingMode"/> enumeration.
        /// This value determines how the pop-up form can be resized: vertically, horizontally or both.
        /// </summary>
        public SizingMode SizingMode
        {
            get
            {
                return this.sizingGrip.SizingMode;
            }
            set
            {
                this.sizingGrip.SizingMode = value;
            }
        }

        /// <summary>
        /// Gets the element that represents the sizing grip 
        /// of the popup.
        /// </summary>
        public SizeGripElement SizingGrip
        {
            get
            {
                return this.sizingGrip;
            }
        }

        /// <summary>
        /// Gets the DockLayoutPanel that holds the sizing grips.
        /// </summary>
        public DockLayoutPanel SizingGripDockLayout
        {
            get
            {
                return this.sizingGripDockLayout;
            }
        }

        #endregion

        #region Methods

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (element.GetType().Equals(typeof(SizeGripElement)))
            {
                return true; 
            }

            return base.ControlDefinesThemeForElement(element);
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);

            this.sizingGrip = new SizeGripElement();
            this.sizingGrip.SizingMode = SizingMode.None;
            this.sizingGrip.MinSize = new System.Drawing.Size(0, 12);
            this.sizingGripDockLayout = new DockLayoutPanel();
            this.sizingGripDockLayout.Class = "PopupPanel";
            this.sizingGripDockLayout.Children.Add(this.sizingGrip);
            this.sizingGripDockLayout.LastChildFill = true;
            DockLayoutPanel.SetDock(this.sizingGrip, Telerik.WinControls.Layouts.Dock.Bottom);
            parent.Children.Add(this.sizingGripDockLayout);
        }

        #endregion

    }
}
