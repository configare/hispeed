//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Drawing;
//using System.ComponentModel;
//using System.Windows.Forms;
//using Telerik.WinControls.Design;
//using Telerik.WinControls.UI;
//using Telerik.WinControls.WindowAnimation;
//using System.Runtime.InteropServices;
//using System.Diagnostics;

//namespace Telerik.WinControls
//{
//#pragma warning disable 0618
	
//    [RadVisualStyleBuilderVisible(false)]
//    public class RadPopupForm : RadFormOld
//    {
//#pragma warning restore 0618
		
//        // Fields
//        private PopupEditorBaseElement owner;

//        public RadPopupForm(): this(null)
//        {
//        }

//        public RadPopupForm(PopupEditorBaseElement owner)
//        {
//            this.owner = owner;

//            this.KeyPreview = true;
//            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
//            this.StartPosition = FormStartPosition.Manual;
//            this.FormBorderStyle = FormBorderStyle.None;
//            this.ShowInTaskbar = false;
//            this.ControlBox = false;
//            this.callbackAnimating = new NotifyAnimationCallback(AnimatingCallback);
//            this.callbackAnimationFinished = new NotifyAnimationCallback(AnimationFinishedCallback);
//            this.FormElement.TitleBar.Visibility = ElementVisibility.Collapsed;
//            this.TopMost = true;

//        }

//        protected override int CaptionHeight
//        {
//            get
//            {
//                EnsureChildItems();
//                return (int)Math.Round(this.FormElement.TitleBar.DesiredSize.Height);
//            }
//        }
//        #region Layouts

//        public Size CalcFormSize()
//        {
//            return CalcFormSizeCore();
//        }

//        protected Size CalcFormSizeCore()
//        {
//            return Size.Empty;
//        } 

//        #endregion

//        #region Show, Hide & Close

//        new public void Show()
//        {
//            ShowPopup();
//        }

//        new public void Hide()
//        {
//            HidePopup();
//        }

//        new public void Close()
//        {
//            Close(RadPopupCloseReason.CloseCalled);
//        }

//        public virtual void Close(RadPopupCloseReason closeMode)
//        {
//            if (OwnerItem == null)
//            {
//                return;
//            }
//            OwnerItem.ClosePopup(closeMode);
//        }

//        protected void AutoUpdateBounds()
//        {
//            this.minimum = new Size(this.Width, 0);
//            this.maximum = this.Size;
//        }

//        public virtual void ShowPopup()
//        {
			
//            this.AutoUpdateBounds();
//            if (this.AnimationEnabled)
//            {
//                this.Animate(true);
//            }
//            else
//            {
//                this.Visible = true;
//                this.Update();
//            }
            
//        }

//        public virtual void HidePopup()
//        {
//            if (ContainsFocus)
//            {
//                Form form = OwnerItem.FindForm();
//                if (form != null && Form.ActiveForm == form)
//                {
//                    form.Activate();
//                }
//                OwnerItem.Focus();
//            }
//            this.Visible = false;
//        } 

//        #endregion

//        #region Owner Events
//        protected virtual void WireEvents()
//        {
//            Form form = OwnerItem.FindForm();
//            if (form == null)
//            {
//                return;
//            }
//            form.Resize += new EventHandler(OnOwnerResize);
//            form.Move += new EventHandler(OnOwnerMove);
//        }

//        protected virtual void UnwireEvents()
//        {
//            if (OwnerItem == null)
//            {
//                return;
//            }
//            Form form = OwnerItem.FindForm();
//            if (form == null)
//            {
//                return;
//            }
//            form.Resize -= new EventHandler(OnOwnerResize);
//            form.Move -= new EventHandler(OnOwnerMove);
//        }

//        protected void OnOwnerResize(object sender, EventArgs e)
//        {
//            if (Visible && OwnerItem != null)
//            {
//                Close(RadPopupCloseReason.CloseCalled);
//            }
//        }

//        protected void OnOwnerMove(object sender, EventArgs e)
//        {
//            if (Visible && OwnerItem != null)
//            {
//                Close(RadPopupCloseReason.CloseCalled);
//            }
//        } 
//        #endregion

//        #region Animation

//        private int backupHeight = 0;
//        private int backupWidth = 0;
//        private Rectangle backupBounds = Rectangle.Empty;
//        private bool animationEnabled = false;
//        private int frames = 4;

//        private WindowAnimationEngine collapseAnimation = null;
//        private NotifyAnimationCallback callbackAnimating;
//        private NotifyAnimationCallback callbackAnimationFinished;
//        private RadEasingType easingType = RadEasingType.Linear;
//        private Size minimum = Size.Empty;
//        private Size maximum = Size.Empty;

//        public bool AnimationEnabled
//        {
//            get
//            {
//                return animationEnabled;
//            }
//            set
//            {
//                animationEnabled = value;
//            }
//        }

//        public RadEasingType EasingType
//        {
//            get
//            {
//                return this.easingType;
//            }
//            set
//            {
//                this.easingType = value;
//            }
//        }

//        public int AnimationFrames
//        {
//            get
//            {
//                return this.frames;
//            }
//            set
//            {
//                if (value == 0)
//                {
//                    throw new InvalidOperationException("Frames can not be zero");
//                }
//                this.frames = value;
//            }
//        }

//        /// <summary>
//        /// Get/Set minimum value allowed for size
//        /// </summary>	
//        internal Size Minimum
//        {
//            get
//            {
//                return minimum;
//            }

//            set
//            {
//                minimum = value;
//            }
//        }

//        /// <summary>
//        /// Get/Set maximum value allowed for size
//        /// </summary>
//        internal Size Maximum
//        {
//            get
//            {
//                return maximum;
//            }
//            set
//            {
//                maximum = value;
//            }
//        }

//        private void OnAnimating(object sender, AnimationEventArgs e)
//        {
//            if (!this.IsDisposed && !this.Disposing)
//            {
//                this.Invoke(callbackAnimating, e);
//            }
//        }

//        private void OnAnimationFinished(object sender, AnimationEventArgs e)
//        {
//            if (!this.IsDisposed && !this.Disposing)
//            {
//                this.Invoke(callbackAnimationFinished, e);
//            }
//        }

//        protected virtual void AnimatingCallback(AnimationEventArgs e)
//        {
//            Size animationSize = ((Rectangle)e.AnimationValue).Size;
//            int num1 = NativeMethods.SWP_NOACTIVATE;
//            num1 |= NativeMethods.SWP_NOMOVE;
//            num1 |= NativeMethods.SWP_SHOWWINDOW;
//            num1 |= NativeMethods.SWP_NOCOPYBITS;
//            num1 |= NativeMethods.SWP_NOZORDER;

//            NativeMethods.SetWindowPos(new HandleRef(null, this.Handle), NativeMethods.HWND_TOPMOST, this.Left, this.Top, animationSize.Width, animationSize.Height, num1);
//        }

//        protected virtual void AnimationFinishedCallback(AnimationEventArgs e)
//        {
//            this.Size = backupBounds.Size;
//        }

//        protected virtual void BackupBounds()
//        {
//            backupBounds = this.Bounds;
//            if (backupHeight == 0)
//            {
//                backupHeight = this.Height;
//            }
//            if (backupWidth == 0)
//            {
//                backupWidth = this.Width;
//            }
//        }

//        protected virtual void AnimationStarting()
//        {
//        }

//        protected virtual void Animate(bool animateDown)
//        {
//            if (!this.animationEnabled)
//            {
//                return;
//            }

//            this.AnimationStarting();
//            this.BackupBounds();
//            this.Size = this.Minimum;
//            this.Visible = true;
//            if (collapseAnimation == null)
//            {
//                collapseAnimation = new WindowAnimationEngine();
//                collapseAnimation.Animating += new AnimationEventHandler(OnAnimating);
//                collapseAnimation.AnimationFinished += new AnimationEventHandler(OnAnimationFinished);
//            }
//            if (animateDown)
//            {
//                collapseAnimation.Minimum = new Rectangle(this.Location, this.minimum);
//                collapseAnimation.Maximum = new Rectangle(this.Location, this.maximum);
//                collapseAnimation.AnimateMinimumToMaximum = true;
//                collapseAnimation.AnimationFrames = this.frames;
//                collapseAnimation.EasingType = this.easingType;
//            }
//            else
//            {

//            }
//            collapseAnimation.Start();
//        }

//        #endregion

//        [Browsable(false)]
//        public virtual PopupEditorBaseElement OwnerItem
//        {
//            get
//            {
//                return owner;
//            }
//            set 
//            { 
//            }
//        }
        
//        [Browsable(false)]
//        public virtual RadComboBoxElement OwnerComboItem
//        {
//            get
//            {
//                return (RadComboBoxElement)this.OwnerItem;
//            }
//        }

//        protected override void SetVisibleCore(bool newVisible)
//        {
//            Form form = null;

//            if (OwnerItem != null && !OwnerItem.IsDisposing && !OwnerItem.IsDisposed)
//            {
//                OwnerItem.FindForm();
//            }

//            if (form != null && form.IsHandleCreated && !form.Disposing && !form.IsDisposed)
//            {
//                RadPopupHelper.SetVisibleCore(this, form == null ? IntPtr.Zero : form.Handle);
//            }

//            base.SetVisibleCore(newVisible);
//        }

//        protected override void OnKeyDown(KeyEventArgs e)
//        {
//            base.OnKeyDown(e);
//            if (!e.Handled)
//            {
//                ProcessKeyDown(e);
//            }
//        }

//        protected override void OnKeyUp(KeyEventArgs e)
//        {
//            base.OnKeyUp(e);
//            if (!e.Handled)
//            {
//                ProcessKeyUp(e);
//            }
//        }

//        protected override void OnKeyPress(KeyPressEventArgs e)
//        {
//            base.OnKeyPress(e);
//            if (!e.Handled)
//            {
//                ProcessKeyPress(e);
//            }
//        }

//        public virtual void ProcessKeyDown(KeyEventArgs e)
//        {
//            if (e.Handled)
//                return;
//            if (e.KeyCode == Keys.Tab)
//            {
//                e.Handled = true;
//                OwnerItem.ProcessPopupTabKey(e);
//                return;
//            }
//            if (e.KeyData == Keys.Escape)
//            {
//                    e.Handled = true;
//                    OwnerItem.ClosePopup();
//                    return;
//            }
//            if (e.KeyData == (Keys.Down | Keys.Alt))
//            {
//                e.Handled = true;
//                Close(RadPopupCloseReason.Keyboard);
//                return;
//            }
//        }

//        public virtual void ProcessKeyUp(KeyEventArgs e)
//        {
//        }

//        public virtual void ProcessKeyPress(KeyPressEventArgs e)
//        {
//        }

//        protected override void WndProc(ref Message msg)
//        {
//            switch (msg.Msg)
//            {
//                case NativeMethods.WM_MOUSEACTIVATE:
//                    msg.Result = (IntPtr)NativeMethods.MA_NOACTIVATE;
//                    return;
//            }
//            base.WndProc(ref msg);
//        }

//        internal Control OwnerControl
//        { 
//            get
//            {
//                if (this.OwnerItem.ElementTree.Control != null)
//                {
//                    return this.OwnerItem.ElementTree.Control;
//                }
//                return null;            
//            }
//        }

//        public override BindingContext BindingContext
//        {
//            get
//            {
//                if (base.BindingContext == null && this.OwnerControl != null)
//                {
//                    if (this.OwnerControl.BindingContext != null)
//                    {
//                        base.BindingContext = this.OwnerControl.BindingContext;
//                    }
//                    else if (this.OwnerControl.Parent != null)
//                    {
//                        base.BindingContext = this.OwnerControl.Parent.BindingContext;
//                    }
//                }
//                return base.BindingContext;
//            }
//            set
//            {
//                base.BindingContext = value;
//            }
//        }

//        protected override void OnRightToLeftChanged(EventArgs e)
//        {
//            this.EnsureChildItems();
//            base.OnRightToLeftChanged(e);
//        }

        

//        #region Events

//        public EventHandler Opened;

//        public CancelEventHandler Opening;

//        #endregion

//        public virtual bool CanClosePopup()
//        {
//            return true;
//        }
//    }
//}

