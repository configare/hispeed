using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using Telerik.WinControls.Design;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Telerik.WinControls.UI
{
	[RadToolboxItem(false)]
    [ToolboxItem(false)]
	public class ZoomPopup : RadControl, IMessageListener
    {
        #region Fields

        private bool animationFinished;
		private bool elementAutoSize;
		private SizeF zoomFactor;
		private bool clicked;
		private bool hasShadow = true;
		private int animationFrames = 4;
		private int animationInterval = 20;
		private bool hideMenus;
        private RadElement element;
        private Rectangle initialBounds;
        private Rectangle elementScreenBounds;
        private Point elementLocation;
        private RadElement elementParent;
        private int elementIndex;
        private SizeF elementScaleTransform;
        private Padding elementMargin;
	    private bool waitForAnimationFinished = true;
        private bool elementHandleMouseInput;

        #endregion

        #region Constructors/Initializers

        public ZoomPopup(RadElement element, SizeF zoomFactor) 
            : this(element, zoomFactor, false)
		{
		}

        public ZoomPopup(RadElement element, SizeF zoomFactor, bool waitForAnimationFinished)
            : this(element, zoomFactor, Rectangle.Empty, waitForAnimationFinished)
        {
        }

        public ZoomPopup(RadElement element, SizeF zoomFactor, Rectangle initialBounds, bool waitForAnimationFinished)
        {
            this.Visible = false;
            this.element = element;
            this.zoomFactor = zoomFactor;
            this.waitForAnimationFinished = waitForAnimationFinished;
            this.initialBounds = initialBounds;
            this.CreateChildItems(this.RootElement);
            element.InvalidateArrange(true);
            this.LoadElementTree();
        }

        protected override void CreateChildItems(RadElement parent)
        {
            if (this.element == null || element.ElementTree == null)
            {
                return;
            }

            this.AutoSize = false;

            this.elementScreenBounds = this.element.ElementTree.Control.RectangleToScreen(this.element.ControlBoundingRectangle);
            if (this.initialBounds == Rectangle.Empty)
            {
                this.initialBounds = this.elementScreenBounds;
            }

            this.elementLocation = this.element.Location;

            this.elementParent = this.element.Parent;
            this.elementIndex = this.elementParent.Children.IndexOf(this.element);
            this.elementScaleTransform = this.element.ScaleTransform;
            this.elementMargin = this.element.Margin;
            this.elementAutoSize = this.element.AutoSize;
            this.elementHandleMouseInput = this.element.ShouldHandleMouseInput;

            Point point = this.element.PointToScreen(new Point(0, 0));
            Rectangle rect = new Rectangle(point.X + this.Size.Width / 2, point.Y + this.Size.Height / 2, 1, 1);

            string themeName = this.element.ElementTree.ComponentTreeHandler.ThemeName;
            if (string.IsNullOrEmpty(themeName))
            {
                themeName = "ControlDefault";
            }

            this.ThemeName = themeName;
            this.ThemeClassName = this.element.ElementTree.ComponentTreeHandler.ThemeClassName;

            this.elementParent.SuspendLayout();
            this.elementParent.Children.RemoveAt(this.elementIndex);

            this.element.Margin = new Padding(0);
            this.element.Location = new Point(0, 0);
            this.element.AutoSize = true;
            this.element.ShouldHandleMouseInput = false;
            this.element.IsMouseOver = false;

            this.Bounds = rect;

            parent.Children.Add(this.element);

            this.element.RadPropertyChanged += new RadPropertyChangedEventHandler(this.element_RadPropertyChanged);
            if (this.element is RadItem)
            {
                (this.element as RadItem).Click += new EventHandler(this.ZoomPopup_Click);
            }
        }

        #endregion

        #region Events/Properties

        public EventHandler Closed;
		
		public EventHandler Clicked;

        /// <summary>
		/// Gets or sets the zoom popup shadow
		/// </summary>
		[Browsable(true), Category("Appearance")]
		[Description("Gets or sets the zoom popup shadow")]
		[DefaultValue(true)]
		public bool HasShadow
		{
			get { return hasShadow; }
			set { hasShadow = value; }
		}

		/// <summary>
		/// Gets or sets the animation frames count
		/// </summary>
		[Browsable(true), Category("Appearance")]
		[Description("Gets or sets the animation frames count")]
		[DefaultValue(4)]
		public int AnimationFrames
		{
			get { return animationFrames; }
			set { animationFrames = value; }
		}

		/// <summary>
		/// Gets or sets the animation interval (in miliseconds)
		/// </summary>
		[Browsable(true), Category("Appearance")]
		[Description("Gets or sets the animation interval (in miliseconds)")]
		[DefaultValue(20)]
		public int AnimationInterval
		{
			get { return animationInterval; }
			set { animationInterval = value; }
		}

        #endregion

        #region Overrides

        protected override CreateParams CreateParams
		{
			get
			{
				CreateParams params1 = base.CreateParams;

				if ((Environment.OSVersion.Version.Major > 5 ||
					(Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1)) &&
					hasShadow)
					params1.ClassStyle |= 0x20000; 
				
				params1.Style &= -79691777;
				params1.ExStyle &= -262145;
				params1.Style |= -2147483648;
				params1.ExStyle |= 0x10000 | 0x00000008;
				params1.ClassStyle |= 0x800;

				return params1;
			}
		}

		protected override void SetVisibleCore(bool value)
		{
			base.SetVisibleCore(value);
            if (value || !this.IsLoaded)
            {
                return;
            }

			RadMessageFilter.Instance.RemoveListener(this);
			RadModalFilter.Instance.MenuHierarchyClosing -= new EventHandler(Instance_MenuHierarchyClosing);

            this.ReparentElement();

			if (Closed != null)
				Closed(this, EventArgs.Empty);

            if (this.clicked && this.Clicked != null)
            {
                this.Clicked(this, EventArgs.Empty);
            }

            if (hideMenus)
            {
                RadModalFilter.Instance.UnRegisterAllMenus();
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_MOUSEACTIVATE:
                    m.Result = (IntPtr)NativeMethods.MA_NOACTIVATE;
                    return;

                case NativeMethods.WM_ACTIVATEAPP:
                    this.Hide();
                    break;

                case NativeMethods.WM_LBUTTONDOWN:
                case NativeMethods.WM_MBUTTONDOWN:
                case NativeMethods.WM_RBUTTONDOWN:
                case NativeMethods.WM_LBUTTONUP:
                case NativeMethods.WM_RBUTTONUP:
                case NativeMethods.WM_MBUTTONUP:
                case NativeMethods.WM_LBUTTONDBLCLK:
                case NativeMethods.WM_RBUTTONDBLCLK:
                case NativeMethods.WM_MBUTTONDBLCLK:
                    if (!animationFinished)
                        return;
                    break;
            }

            base.WndProc(ref m);
        }

        protected override bool ProcessFocusRequested(RadElement element)
        {
            return false;
        }

        protected override bool ProcessCaptureChangeRequested(RadElement element, bool capture)
        {
            return false;
        }

        public new void Show()
        {
            NativeMethods.ShowWindow(Handle, NativeMethods.SW_SHOWNOACTIVATE);
            ControlHelper.BringToFront(Handle, false);
            this.ShowCore();
        }

        #endregion

        #region Private Implementation

        private void ShowCore()
        {
            hideMenus = false;
            this.animationFinished = false;

            RadMessageFilter.Instance.AddListener(this);
            RadModalFilter.Instance.MenuHierarchyClosing += new EventHandler(Instance_MenuHierarchyClosing);

            SizeF originalScaleTransform = this.element.ScaleTransform;

            AnimatedPropertySetting animatedExpand = new AnimatedPropertySetting(
                RadElement.ScaleTransformProperty,
                originalScaleTransform,
                zoomFactor,
                animationFrames,
                animationInterval);

            animatedExpand.ApplyValue(this.element);
            animatedExpand.AnimationFinished += delegate
            {
                animationFinished = true;
                this.element.ShouldHandleMouseInput = this.elementHandleMouseInput;
                this.element.IsMouseOver = true;
                this.ApplyElementShape();
            };
        }

        private void ApplyElementShape()
        {
            if (this.element != null && this.element.Shape != null)
            {
                using (GraphicsPath path = this.element.Shape.CreatePath(this.ClientRectangle))
                {
                    this.Region = new Region(path);
                }
            }
        }

        private void ReparentElement()
        {
            if (this.elementParent == null || !this.elementParent.IsInValidState(false))
            {
                return;
            }

            this.element.RadPropertyChanged -= new RadPropertyChangedEventHandler(element_RadPropertyChanged);
            if (this.element is RadItem)
            {
                (this.element as RadItem).Click -= new EventHandler(ZoomPopup_Click);
            }

            this.element.ResetValue(RadElement.ScaleTransformProperty, ValueResetFlags.Animation);

            this.element.AutoSize = elementAutoSize;
            this.element.Margin = elementMargin;
            this.element.Location = elementLocation;
            this.element.ScaleTransform = elementScaleTransform;
            this.element.IsMouseOver = false;
            this.element.ShouldHandleMouseInput = this.elementHandleMouseInput;

            if (element.Parent != this.elementParent)
            {
                elementParent.Children.Insert(elementIndex, element);
            }
            elementParent.ResumeLayout(true);

            elementParent = null;
        }

        private void UpdateSize(SizeF scale)
        {
            //TODO: Try to remove this in new layouts
            this.element.InvalidateTransformations();

            float width = this.initialBounds.Width * scale.Width;
            float height = this.initialBounds.Height * scale.Height;
            Size size = new Size((int)(width + .5F), (int)(height + .5F));

            this.Size = size;
            this.Location = new Point(
                this.initialBounds.X - (this.Width - this.initialBounds.Size.Width) / 2,
                this.initialBounds.Y - (this.Height - this.initialBounds.Size.Height) / 2);

            this.ApplyElementShape();
        }

        #endregion

        #region Event Handlers

        private void element_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
        {
            if (e.Property == RadElement.ScaleTransformProperty && this.IsHandleCreated)
            {
                this.RootElement.UpdateLayout();
                this.UpdateSize((SizeF)e.NewValue);
            }
        }

        private void ZoomPopup_Click(object sender, EventArgs e)
        {
            this.clicked = true;
            this.Hide();
        }

        private void Instance_MenuHierarchyClosing(object sender, EventArgs e)
        {
            hideMenus = true;
        }

        #endregion

        #region IMessageListener Members

        InstalledHook IMessageListener.DesiredHook
        {
            get
            {
                return InstalledHook.GetMessage;
            }
        }

        MessagePreviewResult IMessageListener.PreviewMessage(ref Message msg)
        {
            Debug.Assert(this.IsHandleCreated, "Should not monitor WM_MOUSEMOVE if not displayed");
            if (msg.Msg == NativeMethods.WM_MOUSEMOVE)
            {
                Point mouse = Control.MousePosition;
                bool hide;
                if (!this.animationFinished)
                {
                    hide = !this.elementScreenBounds.Contains(mouse);
                }
                else
                {
                    hide = !this.initialBounds.Contains(mouse);
                }

                if (hide)
                {
                    this.Hide();
                    return MessagePreviewResult.Processed;
                }
            }

            return MessagePreviewResult.NotProcessed;
        }

        void IMessageListener.PreviewWndProc(Message msg)
        {
            throw new NotImplementedException();
        }

        void IMessageListener.PreviewSystemMessage(SystemMessage message, Message msg)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
