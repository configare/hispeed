using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using Telerik.WinControls;
using Telerik.WinControls.Design;
using Telerik.WinControls.Themes.Design;
using System.Windows.Forms;
using System.Reflection;

namespace Telerik.WinControls.UI
{
    /// <summary>Used to group collections of controls.</summary>
    /// <remarks>
    /// 	<para>A <strong>RadPanel</strong> is a control that contains other controls. You
    ///     can use a <strong>RadPanel</strong> to group collections of controls such as a
    ///     group control of radio buttons. If the <strong>RadPanel</strong> control's
    ///     <strong>Enabled</strong> property is set to <strong>false</strong>, the controls
    ///     contained within the <strong>RadPanel</strong> will also be disabled.</para>
    /// 	<para>You can use the <strong>AutoScroll</strong> property to enable scroll bars in
    ///     the <strong>RadPanel</strong> control. When the <strong>AutoScroll</strong>
    ///     property is set to <strong>true</strong>, any controls located within the
    ///     <strong>RadPanel</strong> (but outside of its visible region), can be scrolled to
    ///     with the scroll bars provided.</para>
    /// 	<para>The <strong>RadPanel</strong> control is displayed by default with border and
    ///     a text (using <strong>TextPrimitive</strong>). There is a
    ///     <strong>FillPrimitive</strong> which is transparent by default. It allows gradients
    ///     to be used for background of the <strong>RadPanel</strong>.</para>
    /// </remarks>
    [TelerikToolboxCategory(ToolboxGroupStrings.ContainersGroup)]
    [ToolboxItem(true)]
    [RadThemeDesignerData(typeof(RadPanelThemeDesignerData))]
	[Description("Enables you to group collections of controls")]
	[DefaultProperty("BorderStyle"), DefaultEvent("Paint")]
    [Docking(DockingBehavior.Ask)]
	public class RadPanel : RadControl
	{
		private bool autoScrollToCurrentControl = true;

        /// <summary>Initializes new <strong>RadPanel</strong></summary>
        public RadPanel()
        {
            this.CausesValidation = true;

            // This is necessary because of the initial scrolling to show at maximum when part of the
            // nested controls is not visible.
            base.SetStyle(ControlStyles.Selectable, false);

			base.Scroll += new ScrollEventHandler(RadPanel_Scroll);
		}

		#region AutoScrollToCurrentControl property
		/// <summary>
		/// Gets or set a value indicating whether panel will scroll automatically to show 
		/// the currently focused control inside it.
		/// </summary>		
		[DefaultValue(true),
		Category(RadDesignCategory.BehaviorCategory),
		Description("Gets or set a value indicating whether panel will scroll automatically to show the currently focused control inside it.")]
		public bool AutoScrollToCurrentControl
		{
			get 
			{ 
				return this.autoScrollToCurrentControl; 
			}
			set 
			{ 
				this.autoScrollToCurrentControl = value; 
			}
		}

		private Point location;

        private void RadPanel_Scroll(object sender, ScrollEventArgs e)
		{
			location.X = this.DisplayRectangle.X;
			location.Y = this.DisplayRectangle.Y;  			
		}
		
		protected override Point ScrollToControl(Control activeControl)
		{
			if (this.autoScrollToCurrentControl)
			{
				return base.ScrollToControl(activeControl);
			}
			else
			{
				return location;
			}
		}
		#endregion

        private RadPanelElement panelElement;

        /// <summary>
        /// Gets the instance of RadPanelElement wrapped by this control. RadPanelElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadPanel.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadPanelElement PanelElement
        {
            get
            {
                return this.panelElement;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the text within Panel's bounds.
        /// </summary>
        [Description("Gets or sets the alignment of the text within Panel's bounds.")]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment TextAlignment
        {
            get
            {
                return this.panelElement.PanelText.TextAlignment;
            }
            set
            {
                this.panelElement.PanelText.TextAlignment = value;
            }
        }
		

        /// <summary>Gets the default size of the control.</summary>
        /// <value>The default System.Drawing.Size of the control.</value>
        /// <remarks>The default Size of the control.</remarks>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(200, 100);
            }
        }

        /// <summary>Gets or set a value indicating whether autosize is turned on.</summary>
        [DefaultValue(false)]
        [Browsable(false)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control causes validation to be
        /// performed on any controls that require validation when it receives focus.
        /// </summary>
        /// <value>
        /// true if the control causes validation to be performed on any controls requiring
        /// validation when it receives focus; otherwise, false.
        /// </value>
		[DefaultValue(true)] //must be true in order to be consistent with the System.Windows.Forms.Panel control
		public new bool CausesValidation
		{
			get
			{
				return base.CausesValidation;
			}
			set
			{
				base.CausesValidation = value;
			}
		}

		/// <summary>
		/// Gets or sets the text associated with this item. 
		/// </summary>
		[Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets the text associated with this item.")]
		[Bindable(true)]
		[SettingsBindable(true)]
		[Editor("Telerik.WinControls.UI.TextOrHtmlSelector, Telerik.WinControls.RadMarkupEditor, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e", typeof(UITypeEditor))]
		public override string Text
		{
			get
			{
				return this.panelElement.Text;
			}
			set
			{
				this.panelElement.Text = value;
			}
		}


        /// <summary>Creates the main panel element and adds it in the root element.</summary>
        protected override void CreateChildItems(RadElement parent)
        {
            if (this.panelElement == null)
            {
                this.panelElement = new RadPanelElement();
            }

            this.panelElement.AutoSizeMode = this.AutoSize ? RadAutoSizeMode.WrapAroundChildren : RadAutoSizeMode.FitToAvailableSize;

            this.RootElement.Children.Add(panelElement);
            base.CreateChildItems(parent);
        }
	}
}
