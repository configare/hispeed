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


namespace Telerik.WinControls.UI
{
	/// <exclude/>
	[ToolboxItem(false), ComVisible(false)]
	public class RadRibbonBarCaption : RadItem
	{
		private TextPrimitive captionText;		
		private StackLayoutPanel minMaxCloseButtonsStackLayoutPanel;
		private RadImageButtonElement closeButton;
        private RadImageButtonElement minimizeButton;
        private RadImageButtonElement maximizeButton;
		private StackLayoutPanel systemButtons;
        private RadRibbonBarElement radRibbonBarElement;

        public StackLayoutPanel SystemButtons
        {
            get { return systemButtons; }
            set { systemButtons = value; }
        }
		private RibbonBarCaptionLayoutPanel ribbonTextAndContextGroupPanel;
		private Point downPoint;
		
		public RadRibbonBarCaption(RadRibbonBarElement ribbonBarElement)
		{
            this.radRibbonBarElement = ribbonBarElement;
            //create the group panel, which needs a reference to the ribbon bar element
            this.ribbonTextAndContextGroupPanel = new RibbonBarCaptionLayoutPanel(this.radRibbonBarElement);
            this.ribbonTextAndContextGroupPanel.Children.Add(this.captionText);
            this.Children.Add(ribbonTextAndContextGroupPanel);
		}

		#region Properties

		/// <summary>
		/// Gets or sets the value of the caption
		/// </summary>
		[Localizable(true)]
		public string Caption
		{
			get
			{
				return (string)this.GetValue(RadItem.TextProperty);
			}
			set
			{
				this.SetValue(RadItem.TextProperty, value);
			}
		}

        [Browsable(false)]
        internal StackLayoutPanel SystemButtonsPanel
        {
            get
            {
                return this.systemButtons;
            }
        }

		/// <summary>
		/// Gets the caption layout
		/// </summary>
		[Browsable(false)]
		public RibbonBarCaptionLayoutPanel CaptionLayout
		{
			get
			{
				return this.ribbonTextAndContextGroupPanel;
			}
		}

		/// <summary>
		/// Gets the Minimize button
		/// </summary>
		public RadButtonElement MinimizeButton
		{
			get
			{
				return minimizeButton;
			}
		}
		/// <summary>
		/// Gets the Maxmimize button
		/// </summary>
		public RadButtonElement MaximizeButton
		{
			get
			{
				return maximizeButton;
			}
		}
		/// <summary>
		/// Gets the Close button
		/// </summary>
		public RadButtonElement CloseButton
		{
			get
			{
				return closeButton;
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Fires when the close button is clicked
		/// </summary>
		public event TitleBarSystemEventHandler Close;
		/// <summary>
		/// Fires when the minimize button is clicked
		/// </summary>
		public event TitleBarSystemEventHandler Minimize;
		/// <summary>
		/// Fires when the maximize button is clicked
		/// </summary>
		public event TitleBarSystemEventHandler MaximizeRestore;

		#endregion

		#region Event handlers

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			downPoint = e.Location;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button == MouseButtons.Left && downPoint != e.Location && this.PointFromControl(e.Location).X < this.Size.Width - this.systemButtons.Size.Width)
			{
				Form form = FindForm();
				if (form != null)
				{
					NativeMethods.ReleaseCapture();
					NativeMethods.SendMessage(new HandleRef(this, form.Handle), NativeMethods.WM_NCLBUTTONDOWN, 2, IntPtr.Zero);
				}
			}
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);
			
			if(this.MaximizeButton.Visibility == ElementVisibility.Visible)
			{
				OnMaximizeRestore(this, EventArgs.Empty);
			}
		}

		protected void OnClose(object sender, EventArgs args)
		{
			if (Close != null)
			{
				Close(sender, args);
			}

			Form form = this.FindForm();
			if (form != null)
			{
				form.Close();
			}
		}

		protected void OnMinimize(object sender, EventArgs args)
		{
			if (Minimize != null)
			{
				Minimize(sender, args);
			}

			Form form = this.FindForm();
			if (form != null)
			{
				form.WindowState = FormWindowState.Minimized;
			}
		}

		protected void OnMaximizeRestore(object sender, EventArgs args)
		{
			if (MaximizeRestore != null)
			{
				MaximizeRestore(sender, args);
			}

			Form form = this.FindForm();
			if (form != null)
			{
				if (form.WindowState == FormWindowState.Maximized)
					form.WindowState = FormWindowState.Normal;
				else
					form.WindowState = FormWindowState.Maximized;
			}
		}

		private void captionText_FontChanged(object sender, EventArgs e)
		{
			this.MinSize = new Size(0, Math.Max(26, this.captionText.FullBoundingRectangle.Height));
			this.MaxSize = new Size(this.MaxSize.Width, 28);
			if (this.MinSize.Height > this.MaxSize.Height)
				this.MinSize = new Size(0, this.MaxSize.Height);
		}

		#endregion

		protected override void CreateChildElements()
		{
			base.CreateChildElements();

			this.minMaxCloseButtonsStackLayoutPanel = new StackLayoutPanel();
			this.minMaxCloseButtonsStackLayoutPanel.EqualChildrenHeight = true;
			this.Children.Add(this.minMaxCloseButtonsStackLayoutPanel);

			this.systemButtons = new StackLayoutPanel();
            this.systemButtons.Class = "SystemButtonsStackLayout";
			this.systemButtons.Orientation = Orientation.Horizontal;
			this.systemButtons.Alignment = ContentAlignment.MiddleRight;
			this.minMaxCloseButtonsStackLayoutPanel.Children.Add(this.systemButtons);

			this.minimizeButton = new RadImageButtonElement();
            this.minimizeButton.StateManager = new RibbonBarSystemButtonStateManager().StateManagerInstance;
            this.minimizeButton.ThemeRole = "RibbonMinimizeButton";
			this.minimizeButton.Class = "MinimizeButton";
			this.minimizeButton.Image = Properties.Resources.ribbon_minimize;
			this.minimizeButton.Click += new EventHandler(this.OnMinimize);
			
			ClassSelector selector = new ClassSelector("ButtonFill");
			ClassSelector borderSelector = new ClassSelector("ButtonBorder");
			selector.GetSelectedElements(this.minimizeButton).First.Value.Class = "CaptionButtonFill";
            borderSelector.GetSelectedElements(this.minimizeButton).First.Value.Class = "CaptionButtonBorder";
			this.systemButtons.Children.Add(minimizeButton);

            this.maximizeButton = new RadImageButtonElement();
            this.maximizeButton.StateManager = new RibbonBarSystemButtonStateManager().StateManagerInstance;
            this.maximizeButton.ThemeRole = "RibbonMaximizeButton";
			this.maximizeButton.Class = "MaximizeButton";
			this.maximizeButton.Image = Properties.Resources.ribbon_maximize;
			this.maximizeButton.Click += new EventHandler(this.OnMaximizeRestore);
            selector.GetSelectedElements(this.maximizeButton).First.Value.Class = "CaptionButtonFill";
            borderSelector.GetSelectedElements(this.maximizeButton).First.Value.Class = "CaptionButtonBorder";
			this.systemButtons.Children.Add(maximizeButton);

            this.closeButton = new RadImageButtonElement();
            this.closeButton.StateManager = new RibbonBarSystemButtonStateManager().StateManagerInstance;
            this.closeButton.ThemeRole = "RibbonCloseButton";
			this.closeButton.Class = "CloseButton";
			this.closeButton.Image = Properties.Resources.ribbon_close;
			this.closeButton.Click += new EventHandler(this.OnClose);
            selector.GetSelectedElements(this.closeButton).First.Value.Class = "CaptionButtonFill";
            borderSelector.GetSelectedElements(this.closeButton).First.Value.Class = "CaptionButtonBorder";
			this.systemButtons.Children.Add(closeButton);

			this.captionText = new TextPrimitive();
			this.captionText.Class = "TitleCaption";
			this.captionText.TextAlignment = ContentAlignment.MiddleCenter;
			captionText.SetValue(RibbonBarCaptionLayoutPanel.CaptionTextProperty, true);
			captionText.BindProperty(TextPrimitive.TextProperty, this, RadItem.TextProperty, PropertyBindingOptions.OneWay);
			this.captionText.FontChanged += new EventHandler(captionText_FontChanged);
		}

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            //base.MeasureOverride(availableSize);
            minMaxCloseButtonsStackLayoutPanel.Measure(availableSize);//new SizeF( availableSize.Width - minMaxCloseButtonsStackLayoutPanel.DesiredSize.Width, availableSize.Height ));
            SizeF textAndContextPanelSize = new SizeF( availableSize.Width - minMaxCloseButtonsStackLayoutPanel.DesiredSize.Width, availableSize.Height );
            ribbonTextAndContextGroupPanel.Measure(textAndContextPanelSize);
            return availableSize;// new SizeF(captionLayout.DesiredSize.Width + layout.DesiredSize.Width, Math.Max(captionLayout.DesiredSize.Height, layout.DesiredSize.Height));
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            //base.ArrangeOverride( finalSize);
            if (!this.RightToLeft)
            {
                minMaxCloseButtonsStackLayoutPanel.Arrange(new RectangleF(new PointF(finalSize.Width - minMaxCloseButtonsStackLayoutPanel.DesiredSize.Width, 0f), minMaxCloseButtonsStackLayoutPanel.DesiredSize));
                ribbonTextAndContextGroupPanel.Arrange(new RectangleF(PointF.Empty, new SizeF(finalSize.Width - minMaxCloseButtonsStackLayoutPanel.DesiredSize.Width, finalSize.Height)));//new RectangleF(new PointF(finalSize.Width / 2f, 0f), ribbonTextAndContextGroupPanel.DesiredSize));
            }
            else
            {
                minMaxCloseButtonsStackLayoutPanel.Arrange(new RectangleF(PointF.Empty, minMaxCloseButtonsStackLayoutPanel.DesiredSize));
                ribbonTextAndContextGroupPanel.Arrange(new RectangleF(new PointF(minMaxCloseButtonsStackLayoutPanel.DesiredSize.Width, 0f), new SizeF(finalSize.Width - minMaxCloseButtonsStackLayoutPanel.DesiredSize.Width, finalSize.Height)));//new RectangleF(new PointF(finalSize.Width / 2f, 0f), ribbonTextAndContextGroupPanel.DesiredSize));
            }
            return finalSize;
        }

		private Form FindForm()
		{
			Form result = null;

			if (this.ElementTree != null)
			{
				result = this.ElementTree.Control.FindForm();
			}

			return result;
		}
	}
}
