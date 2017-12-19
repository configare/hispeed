using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Primitives;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using System.Windows.Forms.VisualStyles;
using Telerik.WinControls.Themes.Design;
using System.ComponentModel.Design;
//using System.Windows.Forms.Design.Behavior;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// 	<para>
	///         Represents a drop down button. Essentially the RadDropDownButton class is a
	///         simple wrapper for
	///         <see cref="RadDropDownButtonElement">RadDropDownButtonElement</see>.
	///     </para>
	/// 	<para>You can set items that appear when the drop down button is pressed. Also you
	///     can configure the visual appearance in numerous ways through themes.</para>
	/// 	<para>
	///         The <see cref="RadDropDownButtonElement">RadDropDownButtonElement</see> class
	///         implements all UI and logic functionality. The RadDropDownButton acts to
	///         transfer the events to and from its
	///         <see cref="RadDropDownButtonElement">RadDropDownButtonElement</see>instance.
	///     </para>
	/// </summary>
    [Designer(DesignerConsts.RadDropDownButtonDesignerString)]
	[Description("Provides a menu-like interface within a button")]
	[DefaultBindingProperty("Text"), DefaultEvent("Click"), DefaultProperty("Text")]
	[ToolboxItem(true)]
	public class RadDropDownButton : RadControl, ITooltipOwner
	{
		private static readonly object DropDownOpeningEventKey;
		private static readonly object DropDownOpenedEventKey;
		private static readonly object DropDownClosedEventKey;

		#region Properties

        private RadDropDownButtonElement dropDownButtonElement = null;
        /// <summary>
        /// Gets the instance of RadDropDownButtonElement wrapped by this control. RadDropDownButtonElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadDropDownButton.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual RadDropDownButtonElement DropDownButtonElement
		{
			get
            {
                return this.dropDownButtonElement;
            }
		}

        /// <summary>Gets or set a value indicating whether automatic sizing is turned on.</summary>
		[DefaultValue(true)]
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

        protected override Size DefaultSize
        {
            get
            {
                return new Size(140, 24);
            }
        }

		/// <commentsfrom cref="RadButtonItem.DisplayStyle" filter=""/>
        [DefaultValue(DisplayStyle.ImageAndText)]
        [Browsable(true),
		RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory)]
		public virtual DisplayStyle DisplayStyle
		{
			get
			{
				return this.dropDownButtonElement.DisplayStyle;
			}
			set
			{
				this.dropDownButtonElement.DisplayStyle = value;
			}
		}

        /// <commentsfrom cref="RadDropDownButtonElement.DropDownDirection" filter=""/>
		[DefaultValue(RadDirection.Down)]
        [Browsable(true),
		RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory)]
		public RadDirection DropDownDirection
		{
			get
			{
				return this.dropDownButtonElement.DropDownDirection;
			}
			set
			{
				this.dropDownButtonElement.DropDownDirection = value;
			}
		}

        /// <commentsfrom cref="RadArrowButtonElement.Direction" filter=""/>
        [DefaultValue(ArrowDirection.Down)]
        [Browsable(true),
        RefreshProperties(RefreshProperties.Repaint),
        Category(RadDesignCategory.AppearanceCategory)]
        public ArrowDirection ArrowDirection
        {
            get
            {
                return this.dropDownButtonElement.ArrowButton.Direction;
            }
            set
            {
                this.dropDownButtonElement.ArrowButton.Direction = value;
            }
        }

		/// <commentsfrom cref="RadDropDownButtonElement.Items" filter=""/>
		[RadEditItemsAction]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor)),
		Category(RadDesignCategory.DataCategory)]
		public virtual RadItemOwnerCollection Items
		{
			get
			{
				return dropDownButtonElement.Items;
			}
		}

		/// <commentsfrom cref="RadDropDownButtonElement.Image" filter=""/>
		[Category(RadDesignCategory.AppearanceCategory),
	    RadDescription("Image", typeof(RadDropDownButtonElement)),
		RefreshProperties(RefreshProperties.All), Localizable(true)]
		public Image Image
		{
			get
			{
				return this.dropDownButtonElement.Image;
			}
			set
			{
				this.dropDownButtonElement.Image = value;

                if (this.dropDownButtonElement.Image != null)
                    this.ImageList = null;
			}
		}

        public bool ShouldSerializeImage()
        {
            return this.Image != null && this.ImageList == null;
        }

        public void ResetImage()
        {
            this.Image = null;
        }

		/// <commentsfrom cref="RadButtonItem.ImageAlignment" filter=""/>
        [DefaultValue(System.Drawing.ContentAlignment.MiddleLeft)]
        [RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the alignment of image content on the drawing surface.")]
		public System.Drawing.ContentAlignment ImageAlignment
		{
			get
			{
				return this.dropDownButtonElement.ImageAlignment;
			}
			set
			{
				this.dropDownButtonElement.ImageAlignment = value;
			}
		}

		/// <commentsfrom cref="RadDropDownButtonElement.ImageIndex" filter=""/>
		[RadDefaultValue("ImageIndex", typeof(RadDropDownButtonElement)),
		Category(RadDesignCategory.AppearanceCategory),
	    RadDescription("ImageIndex", typeof(RadDropDownButtonElement)),
		RefreshProperties(RefreshProperties.All),
		RelatedImageList("ImageList"),
		Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
		TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString)]
		public int ImageIndex
		{
			get
			{
				return this.dropDownButtonElement.ImageIndex;
			}
			set
			{
				this.dropDownButtonElement.ImageIndex = value;
			}
		}

		/// <commentsfrom cref="RadDropDownButtonElement.ImageKey" filter=""/>
		[RadDefaultValue("ImageKey", typeof(RadDropDownButtonElement)),
        Category(RadDesignCategory.AppearanceCategory),
	    RadDescription("ImageKey", typeof(RadDropDownButtonElement)),
        RefreshProperties(RefreshProperties.All), Localizable(true),
	    RelatedImageList("ImageList"),
	    Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.RadImageKeyConverterString)]
		public virtual string ImageKey
		{
			get
			{
				return this.dropDownButtonElement.ImageKey;
			}
			set
			{
				this.dropDownButtonElement.ImageKey = value;
			}
		}

		/// <commentsfrom cref="RadButtonItem.IsPressed" filter=""/>
		[Browsable(false)]
		public bool IsPressed
		{
			get
			{
				return this.dropDownButtonElement.IsPressed;
			}
		}

		/// <commentsfrom cref="RadDropDownButtonElement.ShowArrow" filter=""/>
		[Browsable(true),		
		Category(RadDesignCategory.AppearanceCategory),
		RadDefaultValue("ShowArrow", typeof(RadDropDownButtonElement)),
		RadDescription("ShowArrow", typeof(RadDropDownButtonElement))]
		public bool ShowArrow
		{
			get
			{
				return this.dropDownButtonElement.ShowArrow;
			}
			set
			{
				this.dropDownButtonElement.ShowArrow = value;
			}
		}

        /// <summary>Gets or sets the text value that is displayed on the button.</summary>
        /// <commentsfrom cref="RadItem.Text" filter=""/>
		[Category(RadDesignCategory.BehaviorCategory),
		Description("Gets or sets the text associated with this item."),
		Bindable(true),
		SettingsBindable(true),
        Localizable(true),
		//Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))
        Editor("Telerik.WinControls.UI.TextOrHtmlSelector, Telerik.WinControls.RadMarkupEditor, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e", typeof(UITypeEditor))]
		public override string Text
		{
			get
			{
				return this.dropDownButtonElement.Text;
			}
			set
			{
				base.Text = value;
				this.dropDownButtonElement.Text = value;
			}
		}

		/// <commentsfrom cref="RadButtonItem.TextAlignment" filter=""/>
        [DefaultValue(System.Drawing.ContentAlignment.MiddleCenter)]
		[RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the alignment of text content on the drawing surface.")]
		public System.Drawing.ContentAlignment TextAlignment
		{
			get
			{
				return this.dropDownButtonElement.TextAlignment;
			}
			set
			{
				this.dropDownButtonElement.TextAlignment = value;
			}
		}

		/// <commentsfrom cref="RadButtonItem.TextImageRelation" filter=""/>
        [DefaultValue(TextImageRelation.Overlay)]
		[RefreshProperties(RefreshProperties.Repaint),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the position of text and image relative to each other.")]
		public TextImageRelation TextImageRelation
		{
			get
			{
				return this.dropDownButtonElement.TextImageRelation;
			}
			set
			{
				this.dropDownButtonElement.TextImageRelation = value;
			}
		}

		#endregion

		static RadDropDownButton()
		{
			DropDownOpeningEventKey = new object();
			DropDownOpenedEventKey = new object();
			DropDownClosedEventKey = new object();
		}

		/// <summary>Initializes a new instance of the RadDropDownButton class.</summary>
		public RadDropDownButton()
		{
			this.AutoSize = true;
		}

		private void dropDownButtonElement_DropDownClosed(object sender, EventArgs e)
		{
			this.OnDropDownClosed(e);
		}

		/// <summary>
        /// Override this method to create custom main element. By default the main element is an instance of
        /// RadDropDownButtonElement.
        /// </summary>
        /// <returns>Instance of the one-and-only child of the root element of RadDropDownButton.</returns>
        protected virtual RadDropDownButtonElement CreateButtonElement()
        {
            RadDropDownButtonElement res = new RadDropDownButtonElement();
            return res;
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            if (this.dropDownButtonElement.DropDownMenu != null)
            {
                if (!this.dropDownButtonElement.DropDownMenu.Bounds.Contains(MousePosition))
                {
                    this.dropDownButtonElement.DropDownMenu.ClosePopup(RadPopupCloseReason.AppFocusChange);
                }
            }
        }

		private void dropDownButtonElement_DropDownOpened(object sender, EventArgs e)
		{
			this.OnDropDownOpened(e);
		}

		private void dropDownButtonElement_DropDownOpening(object sender, CancelEventArgs e)
		{
			this.OnDropDownOpening(e);
		}

		#region Methods
        /// <commentsfrom cref="RadDropDownButtonElement.ShowDropDown()" filter=""/>
		public virtual void ShowDropDown()
		{
			this.dropDownButtonElement.ShowDropDown();
		}

		/// <commentsfrom cref="RadDropDownButtonElement.HideDropDown()" filter=""/>
		public virtual void HideDropDown()
		{
			this.dropDownButtonElement.HideDropDown();
		}
		#endregion

		#region Events
        /// <commentsfrom cref="RadDropDownButtonElement.DropDownOpening" filter=""/>
		[Browsable(true),
		Category(RadDesignCategory.BehaviorCategory)]
		public event EventHandler DropDownOpening
		{
			add
			{
				this.Events.AddHandler(RadDropDownButton.DropDownOpeningEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadDropDownButton.DropDownOpeningEventKey, value);
			}
		}

        /// <commentsfrom cref="RadDropDownButtonElement.OnDropDownOpening" filter=""/>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDropDownOpening(CancelEventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadDropDownButton.DropDownOpeningEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <commentsfrom cref="RadDropDownButtonElement.DropDownOpened" filter=""/>
		[Browsable(true),
		Category(RadDesignCategory.BehaviorCategory)]
		public event EventHandler DropDownOpened
		{
			add
			{
				this.Events.AddHandler(RadDropDownButton.DropDownOpenedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadDropDownButton.DropDownOpenedEventKey, value);
			}
		}

		/// <commentsfrom cref="RadDropDownButtonElement.OnDropDownOpened" filter=""/>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDropDownOpened(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadDropDownButton.DropDownOpenedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <commentsfrom cref="RadDropDownButtonElement.DropDownClosed" filter=""/>
		[Browsable(true),
		Category(RadDesignCategory.BehaviorCategory)]
		public event EventHandler DropDownClosed
		{
			add
			{
				this.Events.AddHandler(RadDropDownButton.DropDownClosedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadDropDownButton.DropDownClosedEventKey, value);
			}
		}

		/// <commentsfrom cref="RadDropDownButtonElement.OnDropDownClosed" filter=""/>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDropDownClosed(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadDropDownButton.DropDownClosedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		#endregion

		protected override void InitializeRootElement(RootRadElement rootElement)
        {
            base.InitializeRootElement(rootElement);
            rootElement.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
        }

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            Type elementType = element.GetType();

            if (element is RadDropDownButtonElement)
                return true;

            if (elementType == typeof(RadButtonElement))
                return true;

            return false;
        }

        protected override void CreateChildItems(RadElement parent)
        {
            if (this.dropDownButtonElement == null)
                this.dropDownButtonElement = this.CreateButtonElement();
            this.dropDownButtonElement.Owner = this;
            this.dropDownButtonElement.DropDownOpening += new CancelEventHandler(this.dropDownButtonElement_DropDownOpening);
            this.dropDownButtonElement.DropDownOpened += new EventHandler(this.dropDownButtonElement_DropDownOpened);
            this.dropDownButtonElement.DropDownClosed += new EventHandler(this.dropDownButtonElement_DropDownClosed);

            this.RootElement.Children.Add(this.dropDownButtonElement);
            this.dropDownButtonElement.BindProperty(RadDropDownButtonElement.AutoSizeModeProperty,
                this.RootElement, RootRadElement.AutoSizeModeProperty, PropertyBindingOptions.OneWay);

            RadArrowButtonElement arrowButton = this.dropDownButtonElement.ArrowButton;
            arrowButton.Arrow.AutoSize = true;

            base.CreateChildItems(parent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.dropDownButtonElement != null)
                {
                    this.dropDownButtonElement.DropDownOpening -= new CancelEventHandler(this.dropDownButtonElement_DropDownOpening);
                    this.dropDownButtonElement.DropDownOpened -= new EventHandler(this.dropDownButtonElement_DropDownOpened);
                    this.dropDownButtonElement.DropDownClosed -= new EventHandler(this.dropDownButtonElement_DropDownClosed);
                }
            }
            base.Dispose(disposing);
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadDropDownButtonAccessibleObject(this);
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public object Owner
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }
    }
}