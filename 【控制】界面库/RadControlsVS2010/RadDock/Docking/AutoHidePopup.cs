
using System.Windows.Forms;
using System.Drawing;
using System;
using Telerik.WinControls.UI;
using Telerik.WinControls.Enumerations;
using System.Diagnostics;
using System.ComponentModel;
namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents a popup Form that is used to display auto-hidden windows.
    /// </summary>
    public class AutoHidePopup : DockPopupForm
    {
        #region Fields

        private AutoHideTabStrip toolStrip;
        private int animationDuration = 200;

        #endregion

        #region Constructor

        internal AutoHidePopup()
            :this(null)
        {
        }

        internal AutoHidePopup(RadDock dockManager)
            :base(dockManager, true)
        {
            this.toolStrip = new AutoHideTabStrip(dockManager);
            this.toolStrip.Dock = DockStyle.Fill;
            this.Controls.Add(this.toolStrip);

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.ShowInTaskbar = false;
        }

        #endregion

        /// <summary>
        /// Gets the DockStyle that determines at which edge the popup should be displayed.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockStyle AutoHideDock
        {
            get
            {
                return this.toolStrip.AutoHideDock;
            }
            internal set
            {
                this.toolStrip.AutoHideDock = value;
            }
        }

        /// <summary>
        /// Gets or sets the duration of the animation.
        /// </summary>
        /// <value>The duration of the animation.</value>
        public int AnimationDuration
        {
            get { return animationDuration; }
            set { animationDuration = value; }
        }

        private AnimateWindowFlags GetAnimationFlags(DockStyle hideStyle, AnimationType animationType, bool showing)
        {
            AnimateWindowFlags flag = 0;
            switch (animationType)
            {
                case AnimationType.Slide:
                    flag = AnimateWindowFlags.AW_SLIDE;
                    break;
                case AnimationType.ExpandCollapse:
                    flag = AnimateWindowFlags.AW_CENTER;
                    break;
                case AnimationType.Roll:
                    break;
                case AnimationType.Blend:
                    flag = AnimateWindowFlags.AW_BLEND;
                    break;
                default:
                    break;
            }

            switch (hideStyle)
            {
                case DockStyle.Left:
                    flag |= showing ? AnimateWindowFlags.AW_HOR_POSITIVE : AnimateWindowFlags.AW_HOR_NEGATIVE;
                    break;
                case DockStyle.Top:
                    flag |= showing ? AnimateWindowFlags.AW_VER_POSITIVE : AnimateWindowFlags.AW_VER_NEGATIVE;
                    break;
                case DockStyle.Right:
                    flag |= showing ? AnimateWindowFlags.AW_HOR_NEGATIVE : AnimateWindowFlags.AW_HOR_POSITIVE;
                    break;
                case DockStyle.Bottom:
                    flag |= showing ? AnimateWindowFlags.AW_VER_NEGATIVE : AnimateWindowFlags.AW_VER_POSITIVE;
                    break;
            }

            if (!showing)
            {
                flag |= AnimateWindowFlags.AW_HIDE;
            }

            return flag;
        }

        internal void ShowWindow()
        {
            Debug.Assert(this.dockManager != null, "Null RadDock reference.");
            if (this.dockManager == null)
            {
                return;
            }

            //ensure proper AutoHideButton state
            RadToggleButtonElement autoHideButton = this.ToolStrip.AutoHideButton;
            if (autoHideButton != null)
            {
                autoHideButton.ToggleState = ToggleState.Off;
            }

            bool animate = this.dockManager.AutoHideAnimation == AutoHideAnimateMode.AnimateShow ||
                            this.dockManager.AutoHideAnimation == AutoHideAnimateMode.Both;

            if (animate)
            {
                int animationFlags = (int)this.GetAnimationFlags(this.AutoHideDock, AnimationType.Slide, true);
                TelerikHelper.AnimateWindow(this.Handle, this.animationDuration, animationFlags);
                base.Show();
            }
            else
            {
                NativeMethods.ShowWindow(Handle, NativeMethods.SW_SHOWNOACTIVATE);
            }
        }

        internal void HideWindow()
        {
            Debug.Assert(this.dockManager != null, "Null RadDock reference.");
            if (this.dockManager == null)
            {
                return;
            }

            bool animate = this.dockManager.AutoHideAnimation == AutoHideAnimateMode.AnimateHide ||
                            this.dockManager.AutoHideAnimation == AutoHideAnimateMode.Both;

            if (animate)
            {
                int animationFlags = (int)this.GetAnimationFlags(this.AutoHideDock, AnimationType.Slide, false);
                TelerikHelper.AnimateWindow(this.Handle, this.animationDuration, animationFlags);
                base.Hide();
            }
            else
            {
                base.Hide();
            }
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            this.ToolStrip.RightToLeft = this.RightToLeft;
        }

        protected override void OnThemeChanged()
        {
            base.OnThemeChanged();

            if (this.toolStrip != null)
            {
                this.toolStrip.ThemeName = this.ThemeName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool FixedSplitter
        {
            get { return this.toolStrip.FixedSplitter; }
            set { this.toolStrip.FixedSplitter = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public AutoHideTabStrip ToolStrip
        {
            get { return toolStrip; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DockWindow ActiveWindow
        {
            get
            {
                return this.toolStrip.ActiveWindow;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SplitterWidth
        {
            get { return this.toolStrip.SplitterWidth; }
            set
            {
                this.toolStrip.SplitterWidth = value;
            }
        }

        #region Nested Types

        enum AnimationType
        {
            Slide,
            Roll,
            Blend,
            ExpandCollapse
        }

        [Flags]
        enum AnimateWindowFlags
        {
            AW_HOR_POSITIVE = 0x00000001,
            AW_HOR_NEGATIVE = 0x00000002,
            AW_VER_POSITIVE = 0x00000004,
            AW_VER_NEGATIVE = 0x00000008,
            AW_CENTER = 0x00000010,
            AW_HIDE = 0x00010000,
            AW_ACTIVATE = 0x00020000,
            AW_SLIDE = 0x00040000,
            AW_BLEND = 0x00080000
        }

        #endregion
    }
}
