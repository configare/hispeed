using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using Telerik.WinControls.WindowAnimation;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
	internal class ComboBoxPopupControl : RadControl, IMessageFilter
	{
		private SizeGripElement grip;
		private bool sendingActivateMessage;
		private bool closing;
		private IntPtr activeHwnd;
		private RadListBoxElement popupListBoxElement;
		private RadPopupCloseReason closeReason;

		public ComboBoxPopupControl()
		{
            this.callbackAnimating = new NotifyAnimationCallback(AnimatingCallback);
			this.callbackAnimationFinished = new NotifyAnimationCallback(AnimationFinishedCallback);
			this.Visible = false;
		}

		#region Events

		public EventHandler Opened;

		public RadPopupClosingEventHandler Closing;

		public RadPopupClosedEventHandler Closed;

		new public MouseEventHandler MouseWheel;

		#endregion

		#region Properties

		public RadControl OwnerControl;
        public Control EditorToFocus;

		public RadElement OwnerElement;

		public RadListBoxElement PopupListBox
		{
			get
			{
				return this.popupListBoxElement;
			}
			set
			{
				this.popupListBoxElement = value;
			}
		}

		public override BindingContext BindingContext
		{
			get
			{
				if (base.BindingContext == null && this.OwnerControl != null)
				{
					if (this.OwnerControl.BindingContext != null)
						base.BindingContext = this.OwnerControl.BindingContext;
					else if (this.OwnerControl.Parent != null)
						base.BindingContext = this.OwnerControl.Parent.BindingContext;
				}
				return base.BindingContext;
			}
			set
			{
				base.BindingContext = value;
			}
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;

				// WS_POPUP | WS_VISIBLE | WS_CLIPSIBLINGS | WS_CLIPCHILDREN);
				cp.Style &= -79691777;
				//cp.Style |= NativeMethods.WS_POPUP;
				//cp.Style |= NativeMethods.WS_VISIBLE;
				//cp.Style |= NativeMethods.WS_CLIPSIBLINGS;
				//cp.Style |= NativeMethods.WS_CLIPCHILDREN;
				cp.Style |= -2147483648 | NativeMethods.WS_CLIPSIBLINGS;

				cp.ExStyle = NativeMethods.WS_EX_LEFT | NativeMethods.WS_EX_LTRREADING | NativeMethods.WS_EX_RIGHTSCROLLBAR | NativeMethods.WS_EX_TOPMOST | NativeMethods.WS_EX_CONTROLPARENT;
				cp.ClassStyle = NativeMethods.CS_DBLCLKS | NativeMethods.CS_SAVEBITS;

				if (Environment.OSVersion.Version.Major > 5 ||
					(Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1))
					cp.ClassStyle |= NativeMethods.CS_DROPSHADOW;

				return cp;
			}
		}

		public SizingMode SizingMode
		{
			get 
			{
				return this.grip.SizingMode; 
			}
			set	
			{
                this.grip.SizingMode = value;
			}
		}

		public RadElement ResizeGrip
		{
			get
			{
				return this.grip;
			}
		}

		#endregion

		#region Public methods

        public void ShowControl(RadDirection popupDirection, int ownerOffset)
		{
			if (this.Visible)
				return;

			bool corected = false;
			Point location = RadPopupHelper.GetValidLocationForDropDown(popupDirection,
				this.Size, this.OwnerElement, ownerOffset, ref corected);

			if (this.grip != null)
			{
				if (corected)
					DockLayoutPanel.SetDock(grip, Telerik.WinControls.Layouts.Dock.Top);		
				else
					DockLayoutPanel.SetDock(grip, Telerik.WinControls.Layouts.Dock.Bottom);		
			}

            this.Location = location;

			this.AutoUpdateBounds();
			if (this.AnimationEnabled)
			{
				this.AnimateDropDown(true);
			}
			else
			{
				this.Show();
			}
        }

		public void HideControl()
		{
			this.HideControl(RadPopupCloseReason.CloseCalled);
		}

		#endregion

		#region IMessageFilter Members

        internal bool wmLButtonDowown = false;
		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg == NativeMethods.WM_LBUTTONDOWN)
			{
                wmLButtonDowown = true;
                Debug.WriteLine("WM_LBUTTONDOWN");
				if (!this.ClientRectangle.Contains(PointToClient(Control.MousePosition)))
				{
                    Debug.WriteLine("WM_LBUTTONDOWN_HideControl");
					this.HideControl(RadPopupCloseReason.Mouse);
					return true;
				}
                else
                {
                    wmLButtonDowown = true;
                }
			}

            if (wmLButtonDowown)
            {
                Debug.WriteLine(m.Msg);
            }
            //if (m.Msg == NativeMethods.WM_LBUTTONUP && 
            //    this.ClientRectangle.Contains(PointToClient(Control.MousePosition)))
            //{
            //    wmLButtonDowown = false;
            //}

            //if (m.Msg == NativeMethods.WM_KILLFOCUS)//&& wmLButtonDowown &&  (m.WParam != this.Handle)
            //{
            //    this.HideControl(RadPopupCloseReason.CloseCalled);
            //    return true;
            //}

			if (m.Msg == NativeMethods.WM_KEYDOWN)
			{
				if ((int)m.WParam == NativeMethods.VK_RETURN || (int)m.WParam == NativeMethods.VK_TAB)
				{				
					this.HideControl(RadPopupCloseReason.Keyboard);
				}
				if ((int)m.WParam == NativeMethods.VK_ESCAPE)
				{
					this.HideControl(RadPopupCloseReason.Keyboard);
					return true;
				}
			}
     
			return false;			
		}

		#endregion

		#region Animation

		private int backupHeight = 0;
		private int backupWidth = 0;
		private Rectangle backupBounds = Rectangle.Empty;
		private bool animationEnabled = false;
		private int frames = 4;

		private WindowAnimationEngine collapseAnimation = null;
		private NotifyAnimationCallback callbackAnimating;
		private NotifyAnimationCallback callbackAnimationFinished;
		private RadEasingType easingType = RadEasingType.Linear;
		private Size minimum = Size.Empty;
		private Size maximum = Size.Empty;

		public bool AnimationEnabled
		{
			get 
			{ 
				return animationEnabled; 
			}
			set 
			{ 
				animationEnabled = value; 
			}
		}

		public RadEasingType EasingType
		{
			get
			{
				return this.easingType;
			}
			set
			{
				this.easingType = value;
			}
		}

		public int AnimationFrames
		{
			get
			{
				return this.frames;
			}
			set
			{
				if (value == 0)
				{
					throw new InvalidOperationException("Frames can not be zero");
				}
				this.frames = value;
			}
		}

		/// <summary>
		/// Get/Set minimum value allowed for size
		/// </summary>	
		internal Size Minimum
		{
			get
			{
				return minimum;
			}

			set
			{
				minimum = value;
			}
		}

		/// <summary>
		/// Get/Set maximum value allowed for size
		/// </summary>
		internal Size Maximum
		{
			get
			{
				return maximum;
			}
			set
			{
				maximum = value;
			}
		}

		protected void AutoUpdateBounds()
		{
			this.minimum = new Size(this.Width, 0);
			this.maximum = this.Size;
		}

		protected virtual void BackupBounds()
		{
			backupBounds = this.Bounds;
			if (backupHeight == 0)
			{
				backupHeight = this.Height;
			}
			if (backupWidth == 0)
			{
				backupWidth = this.Width;
			}
		}

		private void OnAnimating(object sender, AnimationEventArgs e)
		{
			this.Invoke(callbackAnimating, e);
		}

		private void OnAnimationFinished(object sender, AnimationEventArgs e)
		{
			this.Invoke(callbackAnimationFinished, e);
		}

		protected virtual void AnimatingCallback(AnimationEventArgs e)
        {
			Size animationSize = ((Rectangle)e.AnimationValue).Size;
			int num1 = NativeMethods.SWP_NOACTIVATE;
			num1 |= NativeMethods.SWP_NOMOVE;
			num1 |= NativeMethods.SWP_SHOWWINDOW;
			//num1 |= NativeMethods.SW;
			num1 |= NativeMethods.SWP_NOCOPYBITS;
			num1 |= NativeMethods.SWP_NOZORDER;

			NativeMethods.SetWindowPos(new HandleRef(null, this.Handle), NativeMethods.HWND_TOPMOST, this.Left, this.Top, animationSize.Width, animationSize.Height, num1);
			//this.Refresh();
			//this.Size = animationSize;
			//Debug.WriteLine("frame Size:" + (Rectangle)e.AnimationValue);
        }

		protected virtual void AnimationFinishedCallback(AnimationEventArgs e)
        {
			Size animationSize = ((Rectangle)e.AnimationValue).Size;

			int num1 = NativeMethods.SWP_NOMOVE;
			num1 |= NativeMethods.SWP_HIDEWINDOW;
			num1 |= NativeMethods.SWP_NOZORDER;
			num1 |= NativeMethods.SWP_NOREDRAW;
			num1 |= NativeMethods.SWP_NOSIZE;

			NativeMethods.SetWindowPos(new HandleRef(this, this.Handle), NativeMethods.NullHandleRef, this.Left, this.Top, 0 /*animationSize.Width*/, 0/*animationSize.Height*/, num1);

			this.Size = backupBounds.Size; //animationSize;
			//Debug.WriteLine("finish Size:" + (Rectangle)e.AnimationValue);

			//this.UpdateStyles();

			//NativeMethods.ShowWindow(new HandleRef(this, this.Handle), NativeMethods.SW_SHOW);

			this.Visible = false;
			this.Visible = true;
			//this.CreateControl();
			//this.Refresh()
			//this.Invalidate();
			this.ResumeLayout(true);
            this.RootElement.ResumeLayout(true);

			//Debug.WriteLine("animation end: Enabled=" + this.Enabled + ", Visible=" + this.Visible + ", Bounds=" + this.Bounds + ", Capture=" + this.Capture + ", Focused=" + this.Focused);
        }

		private void AnimateDropDown(bool animateDown)
		{
			if (!this.animationEnabled)
			{
				return;
			}
			this.SuspendLayout();
            this.RootElement.SuspendLayout();
			this.BackupBounds();
			//set the start animation size
			this.Size = this.Minimum;
			this.Show();
			//if (!this.IsHandleCreated)
			//{
			//    this.CreateControl();
			//}
			if (collapseAnimation == null)
			{
				if (collapseAnimation == null)
				{
					collapseAnimation = new WindowAnimationEngine();
					//set the events to be raised by the animation worker thread
					collapseAnimation.Animating += new AnimationEventHandler(OnAnimating);
					collapseAnimation.AnimationFinished += new AnimationEventHandler(OnAnimationFinished);					
				}			
			}
			if (animateDown)
			{
				collapseAnimation.Minimum = new Rectangle(this.Location, this.minimum);
				collapseAnimation.Maximum = new Rectangle(this.Location, this.maximum);
				collapseAnimation.AnimateMinimumToMaximum = true;
				collapseAnimation.AnimationFrames = this.frames;
				collapseAnimation.EasingType = this.easingType;
			}
			else
			{

			}
			//ShowControls();
			collapseAnimation.Start();
		}

		#endregion

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            this.popupListBoxElement.RightToLeft = this.RightToLeft == RightToLeft.Yes;
            this.grip.RightToLeft = this.RightToLeft == RightToLeft.Yes;
        }

		protected override void SetVisibleCore(bool value)
		{
			base.SetVisibleCore(value);

            if (value)
			{
				if (Opened != null)
				{
					Opened(this, EventArgs.Empty);
				}
				closing = false;

				activeHwnd = NativeMethods.GetActiveWindow();
				if (activeHwnd == IntPtr.Zero)
				{
					return;
				}
                RadComboBoxElement ownerComboElement = this.OwnerElement as RadComboBoxElement;
                if (ownerComboElement != null)
                   ownerComboElement.IsDropDownShown = true;
               
                this.Capture = true;

				NativeMethods.SetWindowPos(
					new HandleRef(this, this.Handle),
					NativeMethods.HWND_TOPMOST, 0, 0, 0, 0,
					NativeMethods.SWP_DEFERERASE |
					NativeMethods.SWP_NOACTIVATE |
					NativeMethods.SWP_NOMOVE |
					NativeMethods.SWP_NOSIZE);

				Application.AddMessageFilter(this);

				if (this.EditorToFocus != null && (!this.EditorToFocus.Focused || !this.EditorToFocus.ContainsFocus))
				{
					this.EditorToFocus.Focus();
				}

				if (this.OwnerControl != null && !string.IsNullOrEmpty(this.OwnerControl.ThemeName))
				{
                    ThemeName = this.OwnerControl.ThemeName;
				}
				else
				{
					ThemeName = "ControlDefault";
				}
			}
			else
			{
				closing = true;
				this.Capture = false;
				Application.RemoveMessageFilter(this);
				if (activeHwnd != IntPtr.Zero)
					NativeMethods.SetActiveWindow(new HandleRef(this, activeHwnd));
				if (Closed != null)
				{
					RadPopupClosedEventArgs args = new RadPopupClosedEventArgs(this.closeReason);
					Closed(this, args);
				}
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			if (MouseWheel != null)
				MouseWheel(this, e);
		}

		protected override void WndProc(ref Message m)
		{           
			switch (m.Msg)
			{
				case NativeMethods.WM_LBUTTONUP:
					base.WndProc(ref m);
					if (!closing)
					{
						this.Capture = true;
						if (OwnerControl != null)
						{
							if (this.EditorToFocus != null)
							{
								if (this.EditorToFocus.Focus()) m.Result = new IntPtr(1);
								else m.Result = IntPtr.Zero;
							}
							else
							{
								NativeMethods.SetFocus(new HandleRef(this, OwnerControl.Handle));
							}
						}						
					}
					return;

				case NativeMethods.WM_NCACTIVATE:
					this.WmNCActivate(ref m);
					return;

				case NativeMethods.WM_ACTIVATEAPP:
				    this.HideControl(RadPopupCloseReason.AppFocusChange);
				    break;

                case NativeMethods.WM_KILLFOCUS:
                    if (OwnerControl != null)
                    {
                        if (this.EditorToFocus != null)
                        {
                            if (this.EditorToFocus.Focus()) m.Result = new IntPtr(1);
                            else m.Result = IntPtr.Zero;
                        }
                        else
                        {
                            NativeMethods.SetFocus(new HandleRef(this, OwnerControl.Handle));
                        }
                    }
					break;

				case NativeMethods.WM_ACTIVATE:
				    m.Result = IntPtr.Zero;
				    return;
			}

			base.WndProc(ref m);
		}

		private void WmNCActivate(ref Message m)
		{
			if (m.WParam != IntPtr.Zero)
			{
				if (!this.sendingActivateMessage)
				{
					this.sendingActivateMessage = true;

					NativeMethods.SendMessage(new HandleRef(this, activeHwnd), NativeMethods.WM_NCACTIVATE, (IntPtr)1, IntPtr.Zero);
					NativeMethods.RedrawWindow(new HandleRef(this, activeHwnd), IntPtr.Zero, new HandleRef(this, IntPtr.Zero), 0x401);
					m.WParam = (IntPtr)1;
					
					this.sendingActivateMessage = false;
				}

				this.DefWndProc(ref m);
			}
			else
			{
				base.WndProc(ref m);
			}
		}

		private void HideControl(RadPopupCloseReason reason)
		{
            if (this.Visible)
			{
				if (Closing != null)
				{
					RadPopupClosingEventArgs args = new RadPopupClosingEventArgs(reason);
					Closing(this, args);
					if (args.Cancel == true)
						return;
				}
				this.closeReason = reason;
				this.Hide();
			}
		}

        protected override void CreateChildItems(RadElement parent)
        {
            DockLayoutPanel panel = new DockLayoutPanel();
            panel.Class = "PopupPanel";

            grip = new SizeGripElement();
			grip.SizingMode = SizingMode.None;
            DockLayoutPanel.SetDock(grip, Telerik.WinControls.Layouts.Dock.Bottom);
            panel.Children.Add(grip);

            if (this.popupListBoxElement != null)
                panel.Children.Add(this.popupListBoxElement);

            parent.Children.Add(panel);
        }
	}
}
