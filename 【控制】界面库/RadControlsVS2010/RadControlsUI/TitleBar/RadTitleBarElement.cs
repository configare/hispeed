using Telerik.WinControls.Primitives;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Telerik.WinControls.Layouts;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System;
using System.Text;
using Telerik.WinControls;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


namespace Telerik.WinControls.UI
{

    /// <summary>
    /// Represents the method that will handle some of the following events: Close,
    /// MaximizeRestore, Minimize, and MinimizeInTheTray.
    /// </summary>
    public delegate void TitleBarSystemEventHandler(object sender, EventArgs args);


    /// <summary>
    /// 	<para>Represents a title bar element. The RadTitleBar class is a simple
    ///     wrapper for the RadTitleBarElement class. The former acts to transfer events to and
    ///     from its corresponding RadTitleBarElement instance. All logic and UI functionality
    ///     is implemented in the RadTitleBarElement class.</para>
    /// 	<para>You can use RadTitleBarElement events to substitute the title bar in a
    ///     borderless application.</para>
    /// </summary>
	[ToolboxItem(false), ComVisible(false)]
    public class RadTitleBarElement : RadItem
    {
        private BorderPrimitive border;
        private FillPrimitive fill;
        protected TextPrimitive caption;
        protected ImagePrimitive titleBarIcon;
		protected DockLayoutPanel layout;
        private RadImageButtonElement closeButton;
		private RadImageButtonElement minimizeButton;
		private RadImageButtonElement maximizeButton;
        private StackLayoutPanel systemButtons;
		private Image leftImage;
		private Image rightImage;
		private Image middleImage;
		private Point downPoint;
        private bool allowResize = true;
        private bool canManageOwnerForm = true;
        private Icon imageIcon;

        static RadTitleBarElement()
        {
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadTitleBar().DeserializeTheme();
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadTitleBarElementStateManager(), typeof(RadTitleBarElement));
        }

		#region Properties

        [Obsolete("Please, use the Text property instead.")]
        [Browsable(false)]
		public string Caption
		{
			get
			{
				return caption.Text;
			}
			set
			{
				caption.Text = value;
			}
		}

        [Browsable(false)]
        public StackLayoutPanel SystemButtons
        {
            get
            {
                return this.systemButtons;
            }
        }

		public RadButtonElement MinimizeButton
		{
			get
			{
				return minimizeButton;
			}
		}

		public RadButtonElement MaximizeButton
		{
			get
			{
				return maximizeButton;
			}
		}

		public RadButtonElement CloseButton
		{
			get
			{
				return closeButton;
			}
		}

		public virtual Image LeftImage
		{
			get
			{
				return leftImage;
			}
			set
			{
				if (this.leftImage != value)
				{
					this.leftImage = value;
                    this.OnNotifyPropertyChanged("LeftImage");
				}
			}
		}

		public virtual Image RightImage
		{
			get
			{
				return rightImage;
			}
			set
			{
				if (this.rightImage != value)
				{
					this.rightImage = value;
                    this.OnNotifyPropertyChanged("RightImage");
				}
			}
		}

		public virtual Image MiddleImage
		{
			get
			{
				return middleImage;
			}
			set
			{
				if (this.middleImage != value)
				{
					this.middleImage = value;
                    this.OnNotifyPropertyChanged("MiddleImage");
				}
			}
		}

        [Browsable(false)]
        public FillPrimitive FillPrimitive
        {
            get
            {
                return this.fill;
            }
        }

        public TextPrimitive TitlePrimitive
        {
            get { return caption; }
            set { caption = value; }
        }

        public ImagePrimitive IconPrimitive
        {
            get
            {
                return this.titleBarIcon;
            }
            set
            {
                this.titleBarIcon = value;
            }
        }

        /// <summary>
        /// Determines whether the parent form can be resized by dragging the title bar's edges.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [Description("Determines whether the parent form can be resized by dragging the title bar's edges.")]
        [DefaultValue(true)]
        public bool AllowResize
        {
            get { return this.allowResize; }
            set { this.allowResize = value; }
        }

        /// <summary>
        /// Determines whether the parent form can be moved by dragging the title bar.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Determines whether the parent form can be managed by the title bar.")]
        [DefaultValue(true)]
        public bool CanManageOwnerForm
        {
            get
            {
                return this.canManageOwnerForm;
            }
            set
            {
                this.allowResize = value;
                this.canManageOwnerForm = value;
            }
        }
		
		#endregion

		#region Events

		/// <summary>
        /// Fires when a close action is performed by the user (the close button is pressed
        /// or the system menu is pressed twice). The system menu is not visible by default. 
		/// Use the Visual Style Builder to set which elements are visible and to change their
        /// appearance.
        /// </summary>
        public event TitleBarSystemEventHandler Close;
        
		/// <summary>
		/// Fires when a minimize action is performed by the user
		/// </summary>
        public event TitleBarSystemEventHandler Minimize;
        
		/// <summary>
        /// Fires when a maximize/restore action is performed by the user (maximizes button
        /// is pressed or the title bar is double clicked).
        /// </summary>
        public event TitleBarSystemEventHandler MaximizeRestore;

		#endregion

        #region Event handlers

        protected void OnClose(object sender, EventArgs args)
        {
            if (!this.canManageOwnerForm)
                return;
            if (Close != null)
            {
                Close(sender, args);
            }
            RadTitleBar titleBar = this.ElementTree.Control as RadTitleBar;
            if (titleBar != null)
            {
                titleBar.CallOnClose(sender, args);
            }
        }

        protected void OnMinimize(object sender, EventArgs args)
        {

            if (!this.canManageOwnerForm)
                return;

            if (Minimize != null)
            {
                Minimize(sender, args);
            }
            RadTitleBar titleBar = this.ElementTree.Control as RadTitleBar;
            if (titleBar != null)
            {
                titleBar.CallOnMinimize(sender, args);
            }
        }
        
		protected void OnMaximizeRestore(object sender, EventArgs args)
        {

            if (!this.canManageOwnerForm)
                return;

            if (MaximizeRestore != null)
            {
                MaximizeRestore(sender, args);
            }
            RadTitleBar titleBar = this.ElementTree.Control as RadTitleBar;
            if (titleBar != null)
            {
                titleBar.CallOnMaximizeRestore(sender, args);
            }
        }        

        /// <summary>
        /// An Icon that represents the icon for the form.
        /// </summary>
        [DefaultValue(null)]
        [Category("Window Style")]
        public Icon ImageIcon
        {
            get
            {
                return this.imageIcon;
            }
            set
            {
                this.imageIcon = value;
                if (this.ElementTree != null)
                {
                    Form form = RadTitleBar.FindParentForm(this.ElementTree.Control);
                    if (form != null)
                    {
                        form.Icon = value;                        
                    }

                    if (value == null)
                    {
                        this.titleBarIcon.Image = null;
                    }
                    else
                    {
                        this.titleBarIcon.Image = Bitmap.FromHicon(value.Handle);
                    }
                    this.imageIcon = value;
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FillPrimitive TitleBarFill
        {
            get { return fill; }
        }

        protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);
            HandleDoubleClickClick();
		}

        public virtual void HandleDoubleClickClick()
        {
            if (this.ElementTree != null)
            {
                Form form = RadTitleBar.FindParentForm(this.ElementTree.Control);
                if (form != null && form.MaximizeBox)
                    OnMaximizeRestore(this, EventArgs.Empty);
            }
        }

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
            HandleMouseDown(e);
		}

        public virtual void HandleMouseDown(MouseEventArgs e)
        {
            downPoint = e.Location;
        }

        protected override void OnMouseUp( MouseEventArgs e )
        {
            base.OnMouseUp( e );
            HandleMouseUp();
        }

        public virtual void HandleMouseUp()
        {
            Cursor.Current = Cursors.Default;
        }

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

            if (this.IsDesignMode || this.DesignMode)
            {
                return;
            }

            Form form = null;
            if (this.ElementTree != null)
            {
                form = RadTitleBar.FindParentForm(this.ElementTree.Control);
            }
            
            if (form == null)
            {
                return;
            }
           
            HandleMouseMove(e, form);
		}

        public virtual void HandleMouseMove(MouseEventArgs e, Form form)
        {
            //fix allow resize when form is not sizeble
            ShapedForm shapedForm = form as ShapedForm;
            bool parentFormAllowResize = true;
            if (shapedForm != null)
            {
                parentFormAllowResize = shapedForm.AllowResize;
            }

            if (form.WindowState == FormWindowState.Normal)
            {
                if (e.Button == MouseButtons.Left && downPoint != e.Location)
                {
                    //p.p. 28.08.07
                    //add resize support to titlebar
                    NativeMethods.ReleaseCapture();
                    if (parentFormAllowResize && this.allowResize)
                    {

                        if (e.Y >= 0 && e.Y < 7)
                        {
                            if (e.X < 7)
                            {
                                Cursor.Current = Cursors.SizeNWSE;
                                NativeMethods.SendMessage(new HandleRef(this, form.Handle), NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HTTOPLEFT, IntPtr.Zero);
                            }
                            else if (e.X > (this.Size.Width - 7))
                            {
                                Cursor.Current = Cursors.SizeNESW;
                                NativeMethods.SendMessage(new HandleRef(this, form.Handle), NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HTTOPRIGHT, IntPtr.Zero);
                            }
                            else
                            {
                                Cursor.Current = Cursors.SizeNS;
                                NativeMethods.SendMessage(new HandleRef(this, form.Handle), NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HTTOP, IntPtr.Zero);
                            }
                        }
                        else if (e.X < 7)
                        {
                            Cursor.Current = Cursors.SizeWE;
                            NativeMethods.SendMessage(new HandleRef(this, form.Handle), NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HTLEFT, IntPtr.Zero);
                        }
                        else if (e.X > (this.Size.Width - 7))
                        {
                            Cursor.Current = Cursors.SizeWE;
                            NativeMethods.SendMessage(new HandleRef(this, form.Handle), NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HTRIGHT, IntPtr.Zero);
                        }
                        //end add
                    }
                    if (this.canManageOwnerForm && e.X < this.Size.Width - this.systemButtons.Size.Width)
                    {
                        NativeMethods.SendMessage(new HandleRef(this, form.Handle), NativeMethods.WM_NCLBUTTONDOWN, 2, IntPtr.Zero);
                    }
                }
                else
                {
                    if (allowResize)
                    {
                        if (e.Y >= 0 && e.Y < 4)
                        {
                            Cursor.Current = Cursors.SizeNS;
                            if (e.X < 7)
                            {
                                Cursor.Current = Cursors.SizeNWSE;
                            }
                            else if (e.X > (this.Size.Width - 7))
                            {
                                Cursor.Current = Cursors.SizeNESW;
                            }
                            else
                            {
                                Cursor.Current = Cursors.SizeNS;
                            }
                        }
                        else if (e.X < 7)
                        {
                            Cursor.Current = Cursors.SizeWE;
                        }
                        else if (e.X > (this.Size.Width - 7))
                        {
                            Cursor.Current = Cursors.SizeWE;
                        }
                        else
                        {
                            Cursor.Current = Cursors.Default;
                        }
                    }
                }
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == RadElement.RightToLeftProperty)
            {
                this.SetDockingAlignmentUponRtlChange((bool)e.NewValue);
            }
            base.OnPropertyChanged(e);
        }

        private void SetDockingAlignmentUponRtlChange(bool rtlState)
        {
            if (rtlState)
            {
                DockLayoutPanel.SetDock(this.titleBarIcon, Dock.Right);
                DockLayoutPanel.SetDock(this.caption, Dock.Right);
                DockLayoutPanel.SetDock(this.systemButtons, Dock.Left);
            }
            else
            {
                DockLayoutPanel.SetDock(this.titleBarIcon, Dock.Left);
                DockLayoutPanel.SetDock(this.caption, Dock.Left);
                DockLayoutPanel.SetDock(this.systemButtons, Dock.Right);
            }
        }

		/// <summary>
		/// Raises the PropertyChanged event
		/// </summary>
		/// <param name="propertyName">The name of the property</param>
		protected override void OnNotifyPropertyChanged(string propertyName)
		{
			this.OnNotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		protected override void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
		{
            base.OnNotifyPropertyChanged(e);

			switch (e.PropertyName)
			{
				case "LeftImage":
				case "RightImage":
				case "MiddleImage":
					this.Invalidate();
					break;
				default:
					break;
			}
		}

		#endregion

		protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.StretchHorizontally = true;
            this.StretchVertically = true;

            this.fill = new FillPrimitive();
            this.TitleBarFill.Class = "TitleFill";
            this.Children.Add(this.TitleBarFill);

            this.border = new BorderPrimitive();
            this.border.Class = "TitleBorder";
            this.Children.Add(this.border);

            this.layout = new DockLayoutPanel();
            this.layout.StretchVertically = true;
            this.layout.StretchHorizontally = true;
            this.Children.Add(this.layout);

            this.titleBarIcon = new ImagePrimitive();
            this.titleBarIcon.Class = "TitleIcon";
            this.layout.Children.Add(this.titleBarIcon);

            this.caption = new TextPrimitive();
            this.caption.Class = "TitleCaption";
            this.caption.StretchVertically = true;
            this.layout.Children.Add(this.caption);

            this.systemButtons = new StackLayoutPanel();
            this.systemButtons.Alignment = ContentAlignment.MiddleRight;

            this.minimizeButton = new RadImageButtonElement();
            this.minimizeButton.StateManager = new TitleBarButtonStateManager().StateManagerInstance;
            this.minimizeButton.StretchHorizontally = false;
            this.minimizeButton.StretchVertically = false;
            this.minimizeButton.Class = "MinimizeButton";
            this.minimizeButton.ButtonFillElement.Name = "MinimizeButtonFill";
            this.minimizeButton.Click += new EventHandler(this.OnMinimize);
            this.minimizeButton.ThemeRole = "TitleBarMinimizeButton";
            this.systemButtons.Children.Add(minimizeButton);

            this.maximizeButton = new RadImageButtonElement();
            this.maximizeButton.StateManager = new TitleBarButtonStateManager().StateManagerInstance;
            this.maximizeButton.StretchHorizontally = false;
            this.maximizeButton.StretchVertically = false;
            this.maximizeButton.Class = "MaximizeButton";
            this.maximizeButton.ButtonFillElement.Name = "MaximizeButtonFill";
            this.maximizeButton.Click += new EventHandler(this.OnMaximizeRestore);
            this.maximizeButton.ThemeRole = "TitleBarMaximizeButton";
            this.systemButtons.Children.Add(maximizeButton);

            this.closeButton = new RadImageButtonElement();
            this.closeButton.StateManager = new TitleBarButtonStateManager().StateManagerInstance;
            this.closeButton.StretchHorizontally = false;
            this.closeButton.StretchVertically = false;
            this.closeButton.Class = "CloseButton";
            this.closeButton.ButtonFillElement.Name = "CloseButtonFill";
            this.closeButton.Click += new EventHandler(this.OnClose);
            this.closeButton.ThemeRole = "TitleBarCloseButton";
            this.systemButtons.Children.Add(closeButton);

            this.layout.Children.Add(this.systemButtons);

            this.SetDockingAlignmentUponRtlChange(this.RightToLeft);

            layout.LastChildFill = false;

            this.caption.BindProperty(TextPrimitive.TextProperty, this, RadItem.TextProperty, PropertyBindingOptions.OneWay);
		}

		protected override void PaintOverride(Telerik.WinControls.Paint.IGraphics screenRadGraphics, Rectangle clipRectangle, float angle, SizeF scale, bool useRelativeTransformation)
		{
			if (this.LeftImage != null && this.RightImage != null && this.MiddleImage != null)
			{
				using (Graphics g = screenRadGraphics.UnderlayGraphics as Graphics)
				{
					using (Brush brush = new SolidBrush(Color.FromArgb(255, this.BackColor)))
					{
						g.FillRectangle(brush, this.BoundingRectangle);
					}

					int imageMaxHeight = this.MiddleImage.Height;
					int rightWidth = this.Size.Width - this.rightImage.Width;

					Rectangle tileRect = new Rectangle(
						this.leftImage.Width,
						0,
						this.Size.Width - this.leftImage.Width - this.rightImage.Width,
						imageMaxHeight);

					Rectangle clipRect = Rectangle.Intersect(this.BoundingRectangle, tileRect);

					ImageLayout imgLayout = ImageLayout.Tile;
					if (ElementTree != null)
					{
						imgLayout = this.ElementTree.Control.BackgroundImageLayout;
					}

					DrawBackgroundImage(g, this.MiddleImage, BackColor, imgLayout, tileRect, clipRect, Point.Empty, this.RightToLeft);

					g.DrawImage(this.rightImage, new Point(rightWidth, 0));
					g.DrawImage(this.leftImage, Point.Empty);
				}
			}

			base.PaintOverride(screenRadGraphics, clipRectangle, angle, scale, useRelativeTransformation);
		}

		internal static void DrawBackgroundImage(Graphics g, Image backgroundImage, Color backColor, ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect, Point scrollOffset, bool rightToLeft)
		{
			if (backgroundImageLayout == ImageLayout.Tile)
			{
				using (TextureBrush brush1 = new TextureBrush(backgroundImage, WrapMode.Tile))
				{
					if (scrollOffset != Point.Empty)
					{
						Matrix matrix1 = brush1.Transform;
						matrix1.Translate((float)scrollOffset.X, (float)scrollOffset.Y);
						brush1.Transform = matrix1;
					}
					g.FillRectangle(brush1, clipRect);
					return;
				}
			}
			Rectangle rectangle1 = CalculateBackgroundImageRectangle(bounds, backgroundImage, backgroundImageLayout);
			if ((rightToLeft == true) && (backgroundImageLayout == ImageLayout.None))
			{
				rectangle1.X += clipRect.Width - rectangle1.Width;
			}
			using (SolidBrush brush2 = new SolidBrush(backColor))
			{
				g.FillRectangle(brush2, clipRect);
			}
			if (!clipRect.Contains(rectangle1))
			{
				if ((backgroundImageLayout == ImageLayout.Stretch) || (backgroundImageLayout == ImageLayout.Zoom))
				{
					rectangle1.Intersect(clipRect);
					g.DrawImage(backgroundImage, rectangle1);
				}
				else if (backgroundImageLayout == ImageLayout.None)
				{
					rectangle1.Offset(clipRect.Location);
					Rectangle rectangle2 = rectangle1;
					rectangle2.Intersect(clipRect);
					Rectangle rectangle3 = new Rectangle(Point.Empty, rectangle2.Size);
					g.DrawImage(backgroundImage, rectangle2, rectangle3.X, rectangle3.Y, rectangle3.Width, rectangle3.Height, GraphicsUnit.Pixel);
				}
				else
				{
					Rectangle rectangle4 = rectangle1;
					rectangle4.Intersect(clipRect);
					Rectangle rectangle5 = new Rectangle(new Point(rectangle4.X - rectangle1.X, rectangle4.Y - rectangle1.Y), rectangle4.Size);
					g.DrawImage(backgroundImage, rectangle4, rectangle5.X, rectangle5.Y, rectangle5.Width, rectangle5.Height, GraphicsUnit.Pixel);
				}
			}
			else
			{
				ImageAttributes attributes1 = new ImageAttributes();
				attributes1.SetWrapMode(WrapMode.TileFlipXY);
				g.DrawImage(backgroundImage, rectangle1, 0, 0, backgroundImage.Width, backgroundImage.Height, GraphicsUnit.Pixel, attributes1);
				attributes1.Dispose();
			}
		}

		internal static Rectangle CalculateBackgroundImageRectangle(Rectangle bounds, Image backgroundImage, ImageLayout imageLayout)
		{
			Rectangle rectangle1 = bounds;
			if (backgroundImage != null)
			{
				switch (imageLayout)
				{
					case ImageLayout.None:
						rectangle1.Size = backgroundImage.Size;
						return rectangle1;

					case ImageLayout.Tile:
						return rectangle1;

					case ImageLayout.Center:
						{
							rectangle1.Size = backgroundImage.Size;
							Size size1 = bounds.Size;
							if (size1.Width > rectangle1.Width)
							{
								rectangle1.X = (size1.Width - rectangle1.Width) / 2;
							}
							if (size1.Height > rectangle1.Height)
							{
								rectangle1.Y = (size1.Height - rectangle1.Height) / 2;
							}
							return rectangle1;
						}
					case ImageLayout.Stretch:
						rectangle1.Size = bounds.Size;
						return rectangle1;

					case ImageLayout.Zoom:
						{
							Size size2 = backgroundImage.Size;
							float single1 = ((float)bounds.Width) / ((float)size2.Width);
							float single2 = ((float)bounds.Height) / ((float)size2.Height);
							if (single1 < single2)
							{
								rectangle1.Width = bounds.Width;
								rectangle1.Height = (int)((size2.Height * single1) + 0.5);
								if (bounds.Y >= 0)
								{
									rectangle1.Y = (bounds.Height - rectangle1.Height) / 2;
								}
								return rectangle1;
							}
							rectangle1.Height = bounds.Height;
							rectangle1.Width = (int)((size2.Width * single2) + 0.5);
							if (bounds.X >= 0)
							{
								rectangle1.X = (bounds.Width - rectangle1.Width) / 2;
							}
							return rectangle1;
						}
				}
			}
			return rectangle1;
		}
    }
}