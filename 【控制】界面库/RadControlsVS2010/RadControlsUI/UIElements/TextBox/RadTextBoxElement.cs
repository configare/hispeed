using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Primitives;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using System.Security.Permissions;

namespace Telerik.WinControls.UI
{
	/// <summary>
	///     Represents a text box element. The <see cref="RadTextBox">RadTextBox</see>
	///     class is a simple wrapper for the RadTextBoxElement class. All UI and logic
	///     functionality is implemented in the RadTextBoxElement class.
	///     <see cref="RadTextBox">RadTextBox</see> class acts to transfer events to and from
	///     its corresponding RadTextBoxElement instance. The RadTextBoxElement may be nested
	///     in other telerik controls.
	/// </summary>
	[ToolboxItem(false), ComVisible(false)]
	public class RadTextBoxElement : RadEditorElement
	{
		private RadTextBoxItem textBoxItem;
		private FillPrimitive fillPrimitive;
		private BorderPrimitive borderPrimitive;
		private bool showBorder = false;
		private Padding defaultPadding = new Padding(2, 2, 2, 3);

		private static readonly object AcceptsTabChangedEventKey;
		private static readonly object HideSelectionChangedEventKey;
		private static readonly object ModifiedChangedEventKey;
		private static readonly object MultilineChangedEventKey;
		private static readonly object ReadOnlyChangedEventKey;
		private static readonly object TextAlignChangedEventKey;
		private static readonly object TextChangingEventKey;

        static RadTextBoxElement()
        {
            new Themes.ControlDefault.TextBox().DeserializeTheme();

            AcceptsTabChangedEventKey = new object();
            HideSelectionChangedEventKey = new object();
            ModifiedChangedEventKey = new object();
            MultilineChangedEventKey = new object();
            ReadOnlyChangedEventKey = new object();
            TextAlignChangedEventKey = new object();
            TextChangingEventKey = new object();

            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadTextBoxElementStateManager(), typeof(RadTextBoxElement));
        }

		/// <summary>Initializes a new instance of the RadTextBoxElement class.</summary>
		public RadTextBoxElement()
            : this(new RadTextBoxItem())
		{
		}

		/// <summary>
		/// Initializes a new instance of RadTextBoxElemenet
		/// </summary>
		/// <param name="textBoxItem"></param>
		public RadTextBoxElement(RadTextBoxItem textBoxItem)
		{
            // !!! PATCH !!!
            // Event when AutoSizeMode is WrapArroundChildren this element could stretch.
            // Some layouts have problems with stretchable children that are WrapArroundChildren
            // so by default the AutoSizeMode is FitToAvailableSize.
            // For such layouts in some moment of time Min and Max size for RadTextBoxElement are set
            // and after that the AutoSizeMode is set to Auto / WrapArroundChildren.

            this.InitializeTextBoxItem(textBoxItem);
            this.BindProperty(RadTextBoxItem.IsNullTextProperty, this.textBoxItem, RadTextBoxItem.IsNullTextProperty, PropertyBindingOptions.OneWay);
		}

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.NotifyParentOnMouseInput = true;
            this.Padding = defaultPadding;
        }

        private void InitializeTextBoxItem(RadTextBoxItem textBoxItem)
        {
            this.textBoxItem = textBoxItem;
            this.textBoxItem.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            this.textBoxItem.BindProperty(RadItem.TextProperty, this, RadItem.TextProperty, PropertyBindingOptions.TwoWay);
            this.StretchHorizontally = textBoxItem.StretchHorizontally;
            this.StretchVertically = textBoxItem.StretchVertically;

            this.textBoxItem.AcceptsTabChanged += new EventHandler(textBoxItem_AcceptsTabChanged);
            this.textBoxItem.HideSelectionChanged += new EventHandler(textBoxItem_HideSelectionChanged);
            this.textBoxItem.ModifiedChanged += new EventHandler(textBoxItem_ModifiedChanged);
            this.textBoxItem.MultilineChanged += new EventHandler(textBoxItem_MultilineChanged);
            this.textBoxItem.ReadOnlyChanged += new EventHandler(textBoxItem_ReadOnlyChanged);
            this.textBoxItem.TextAlignChanged += new EventHandler(textBoxItem_TextAlignChanged);
            this.textBoxItem.TextChanged += new EventHandler(textBoxItem_TextChanged);
            this.textBoxItem.TextChanging += new TextChangingEventHandler(textBoxItem_TextChanging);
            this.textBoxItem.KeyDown += new KeyEventHandler(textBoxItem_KeyDown);
            this.textBoxItem.KeyUp += new KeyEventHandler(textBoxItem_KeyUp);
            this.textBoxItem.KeyPress += new KeyPressEventHandler(textBoxItem_KeyPress);

            this.Children.Insert(0, this.textBoxItem);
        }

		protected override void CreateChildElements()
		{
			borderPrimitive = new BorderPrimitive();
			borderPrimitive.Class = "TextBoxBorder";
			fillPrimitive = new FillPrimitive();
			fillPrimitive.Class = "TextBoxFill";

			this.Children.Add(fillPrimitive);
			this.Children.Add(borderPrimitive);
            
		}

		#region Properties

        public HorizontalAlignment TextAlign
        {
            get
            {
                return this.textBoxItem.TextAlign;
            }
            set
            {
                this.textBoxItem.TextAlign = value;
            }
        }

        public FillPrimitive Fill
        {
            get
            {
                return this.fillPrimitive;
            }
        }

        public BorderPrimitive Border
        {
            get
            {
                return this.borderPrimitive;
            }
        }

		/// <summary>
		/// Gets an instance of the corresponding RadTextBoxItem 
		/// </summary>
		public RadTextBoxItem TextBoxItem
		{
			get { return this.textBoxItem; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to show the bottom part of characters, clipped 
		/// due to font name or size particularities
		/// </summary>
        [RadDescription("UseGenericBorderPaint", typeof(RadTextBoxItem))]
        [RadDefaultValue("UseGenericBorderPaint", typeof(RadTextBoxItem))]
		public bool UseGenericBorderPaint
		{
			get
			{
				return this.textBoxItem.UseGenericBorderPaint;
			}
			set
			{
				if (value)
					defaultPadding = new Padding(0);
				else
					defaultPadding = new Padding(2, 2, 2, 3);

				this.Padding = defaultPadding;
				this.textBoxItem.UseGenericBorderPaint = value;
			}
		}

		/// <summary>Gets or sets a value indicating whether the border is shown.</summary>
        [DefaultValue(false)]
		public bool ShowBorder
		{
			get
			{
				return showBorder;
			}
			set
			{
				showBorder = value;
				if (borderPrimitive != null)
				{
					this.borderPrimitive.Visibility = showBorder ?
						ElementVisibility.Visible : ElementVisibility.Collapsed;
				}
			}
		}

		/// <summary>
		/// Gets or sets the StretchHorizontally value
		/// </summary>
		[RadDefaultValue("StretchHorizontally", typeof(RadTextBoxItem))]
		public override bool StretchHorizontally
		{
			get { return base.StretchHorizontally; }
			set { base.StretchHorizontally = value; }
		}

		/// <summary>
		/// Gets or sets the StretchVertically value
		/// </summary>
		[RadDefaultValue("StretchVertically", typeof(RadTextBoxItem))]
		public override bool StretchVertically
		{
			get { return base.StretchVertically; }
			set { base.StretchVertically = value; }
		}

		/// <summary>
		/// Gets or sets the TextRenderingHint value
		/// </summary>
		[Browsable(false)]
		public override System.Drawing.Text.TextRenderingHint TextRenderingHint
		{
			get
			{
				return base.TextRenderingHint;
			}
			set
			{
				base.TextRenderingHint = value;
			}
		}

        /// <summary>
        /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
        /// the character used to mask characters of a password in a single-line TextBox
        /// control.</span>
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("PasswordChar", typeof(RadTextBoxItem))]
        public char PasswordChar
        {
            get
            {
                return this.TextBoxItem.PasswordChar;
            }
            set
            {
                this.TextBoxItem.PasswordChar = value;
            }
        }

		#endregion

		/// <summary>
		/// Gets a value whether the default layout is overriden
		/// </summary>
		public override bool OverridesDefaultLayout
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Performs the custom layout logic
		/// </summary>
		/// <param name="affectedElement"></param>
		public override void PerformLayoutCore(RadElement affectedElement)
		{
			if (this.AutoSize && this.AutoSizeMode == RadAutoSizeMode.WrapAroundChildren &&
				(this.textBoxItem.StretchHorizontally || this.textBoxItem.StretchVertically))
			{
				Size proposedSize = this.Size;

				Size prefSize = this.borderPrimitive.GetPreferredSize(proposedSize);

				Size fillSize = Size.Subtract(proposedSize, this.LayoutEngine.GetBorderSize());
				prefSize = this.fillPrimitive.GetPreferredSize(fillSize);

				Size textBoxSize = Size.Subtract(fillSize, this.Padding.Size);
				prefSize = this.textBoxItem.GetPreferredSize(textBoxSize);
				if (!this.textBoxItem.StretchHorizontally)
					textBoxSize.Width = prefSize.Width;
				if (!this.textBoxItem.StretchVertically)
					textBoxSize.Height = prefSize.Height;

				this.textBoxItem.Size = textBoxSize;
				this.fillPrimitive.Size = fillSize;
				this.borderPrimitive.Size = proposedSize;
			}
			else
			{
				base.PerformLayoutCore(affectedElement);
			}
		}

		/// <summary>
		/// Gets the preferred size of the text box element
		/// </summary>
		/// <param name="proposedSize"></param>
		/// <returns></returns>
		public override Size GetPreferredSizeCore(Size proposedSize)
		{
			Size res = base.GetPreferredSizeCore(proposedSize);

			if (this.AutoSize)
			{
				if (this.AutoSizeMode == RadAutoSizeMode.FitToAvailableSize)
				{
					res = proposedSize;
				}
				else
				{
					if (this.textBoxItem.StretchHorizontally)
						res.Width = proposedSize.Width - this.LayoutEngine.GetBorderSize().Width - this.Padding.Horizontal;
					if (this.textBoxItem.StretchVertically)
						res.Height = proposedSize.Height - this.LayoutEngine.GetBorderSize().Height - this.Padding.Vertical;
				}
			}
            return res;
		}

		/// <summary>
		/// Handles the associated tunnel events
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
        protected override void OnTunnelEvent(RadElement sender, RoutedEventArgs args)
		{
			base.OnTunnelEvent(sender, args);

			if (args.RoutedEvent == RootRadElement.AutoSizeChangedEvent)
			{
				if (this.ElementTree == null || typeof(RadTextBox).IsAssignableFrom(this.ElementTree.Control.GetType()))
				{
					AutoSizeEventArgs eventArgs = (AutoSizeEventArgs)args.OriginalEventArgs;
					if (eventArgs.AutoSize)
						this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
					else
						this.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
				}
			}
		}

		protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (this.textBoxItem != null && this.textBoxItem.HostedControl != null &&
				!this.textBoxItem.HostedControl.Focused)
			{
				this.textBoxItem.HostedControl.Focus();
			}
		}

		/// <summary>
		/// Handles the bubble events 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
		{
			if (args.RoutedEvent == RadElement.MouseDownEvent && sender == this.textBoxItem)
			{
				if (RadModalFilter.Instance.ActiveDropDown != null && !RadModalFilter.Instance.Suspended)
				{
					RadModalFilter.Instance.Suspended = true;
					Point point = this.textBoxItem.HostedControl.PointToClient(Control.MousePosition);
					if (this.textBoxItem.HostedControl.ClientRectangle.Contains(point))
					{
						NativeMethods.SendMessage(new HandleRef(null, this.textBoxItem.HostedControl.Handle),
							NativeMethods.WM_LBUTTONDOWN, 1, NativeMethods.Util.MAKELPARAM(point.X, point.Y));
					}
				}
			}
            else if (args.RoutedEvent == RadTextBoxItem.MultilineEvent && sender == this.textBoxItem)
            {
                args.Canceled = true;

                if (this.UseNewLayoutSystem)
                {
                    this.StretchVertically = this.textBoxItem.Multiline;
                    RootRadElement parent = this.Parent as RootRadElement;
                    if (parent != null)
                    {
                        parent.StretchVertically = this.textBoxItem.Multiline;
                    }
                }
            }

			base.OnBubbleEvent(sender, args);
		}

		#region TextBoxItem events		

		private void textBoxItem_AcceptsTabChanged(object sender, EventArgs e)
		{
			this.OnAcceptsTabChanged(e);
		}

		private void textBoxItem_HideSelectionChanged(object sender, EventArgs e)
		{
			this.OnHideSelectionChanged(e);
		}

		private void textBoxItem_ModifiedChanged(object sender, EventArgs e)
		{
			this.OnModifiedChanged(e);
		}

		private void textBoxItem_MultilineChanged(object sender, EventArgs e)
		{
			this.OnMultilineChanged(e);
		}

		private void textBoxItem_ReadOnlyChanged(object sender, EventArgs e)
		{
			this.OnReadOnlyChanged(e);
		}

		private void textBoxItem_TextAlignChanged(object sender, EventArgs e)
		{
			this.OnTextAlignChanged(e);
		}

		private void textBoxItem_TextChanging(object sender, TextChangingEventArgs e)
		{
			base.OnTextChanging(e);
		}

		private void textBoxItem_TextChanged(object sender, EventArgs e)
		{
			base.OnTextChanged(e);
		}

		private void textBoxItem_KeyDown(object sender, KeyEventArgs e)
		{
			base.OnKeyDown(e);
		}

		private void textBoxItem_KeyUp(object sender, KeyEventArgs e)
		{
			base.OnKeyUp(e);
		}

		private void textBoxItem_KeyPress(object sender, KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
		}
		#endregion

		#region Events
		/// <summary>
		/// Occurs when the value of the AcceptsTab property has changed.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the AcceptsTab property has changed.")]
		public event EventHandler AcceptsTabChanged
		{
			add
			{
				this.Events.AddHandler(AcceptsTabChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(AcceptsTabChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the AcceptsTabChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnAcceptsTabChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[AcceptsTabChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when the value of the HideSelection property changes.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the HideSelection property has changed.")]
		public event EventHandler HideSelectionChanged
		{
			add
			{
				this.Events.AddHandler(HideSelectionChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(HideSelectionChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the HideSelectionChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnHideSelectionChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[HideSelectionChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when the value of the Modified property has changed.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the Modified property has changed.")]
		public event EventHandler ModifiedChanged
		{
			add
			{
				this.Events.AddHandler(ModifiedChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(ModifiedChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the ModifiedChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnModifiedChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[ModifiedChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when the value of the Multiline property has changed.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the Multiline property has changed.")]
		public event EventHandler MultilineChanged
		{
			add
			{
				this.Events.AddHandler(MultilineChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(MultilineChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the MultilineChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnMultilineChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[MultilineChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when the ReadOnly property changes.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the ReadOnly property has changed.")]
		public event EventHandler ReadOnlyChanged
		{
			add
			{
				this.Events.AddHandler(ReadOnlyChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(ReadOnlyChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the ReadOnlyChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnReadOnlyChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[ReadOnlyChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when the value of the TextAlign property has changed.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the TextAlign property has changed.")]
		public event EventHandler TextAlignChanged
		{
			add
			{
				this.Events.AddHandler(TextAlignChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(TextAlignChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the TextAlignChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnTextAlignChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[TextAlignChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}	

		#endregion      
	}
}