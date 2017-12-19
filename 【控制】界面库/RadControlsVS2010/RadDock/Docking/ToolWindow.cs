
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Implements a <see cref="DockWindow">DockWindow</see> that resides within a ToolTabStrip.
    /// </summary>
    [ToolboxItem(false)]
    public class ToolWindow : DockWindow
    {
        #region Fields

        string caption;

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor. Initializes a new instance of a <see cref="DockWindow">DockWindow</see> class.
        /// </summary>
        public ToolWindow()
        {
            this.CloseAction = DockWindowCloseAction.Hide;
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="DockWindow">DockWindow</see> class setting the provided Text.
        /// </summary>
        /// <param name="text"></param>
        public ToolWindow(string text)
            :this()
        {
            this.Text = text;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Change the window state to auto-hide mode
        /// </summary>
        public void AutoHide()
        {
            if (this.DockManager != null)
            {
                this.DockManager.AutoHideWindow(this);
            }
        }

        /// <summary>
        /// Change the window to be in floating mode
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        public void Float(Point location, Size size)
        {
            Float(new Rectangle(location, size));
        }

        /// <summary>
        /// Change the window to be in floating mode
        /// </summary>
        /// <param name="bounds"></param>
        public void Float(Rectangle bounds)
        {
            if (this.DockManager != null)
            {
                this.DockManager.FloatWindow(this, bounds);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(DockWindowCloseAction.Hide)]
        public override DockWindowCloseAction CloseAction
        {
            get
            {
                return base.CloseAction;
            }
            set
            {
                base.CloseAction = value;
            }
        }

        public string Caption
        {
            get 
            {
                return this.caption; 
            }
            set 
            {
                if (this.caption != value)
                {
                    this.caption = value;
                    UpdateOnCaptionChanged();
                }

            }
        }

        #endregion

        protected override void UpdateOnTextChanged()
        {
            base.UpdateOnTextChanged();

            ToolTabStrip parentStrip = this.TabStrip as ToolTabStrip;
            if (parentStrip == null || parentStrip.ActiveWindow != this)
            {
                return;
            }

            if (!parentStrip.CaptionVisible && this.DockState == DockState.Floating)
            {
                Form floatingParent = this.FloatingParent;
                Debug.Assert(floatingParent != null, "No floating parent when in DockState.Floating");
                if (floatingParent != null)
                {
                    floatingParent.Text = this.Text;
                }
            }

            UpdateOnCaptionChanged();
        }

        protected virtual void UpdateOnCaptionChanged()
        {
            if (!string.IsNullOrEmpty(this.caption))
            {
                ToolTabStrip parentStrip = this.TabStrip as ToolTabStrip;
                if (parentStrip != null && parentStrip.ActiveWindow == this)
                {
                    parentStrip.CaptionText = this.caption;
                }
            }
        }
    }
}
