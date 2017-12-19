using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents a special type of DockWindow, which is used internally by RadDock to wrap custom controls as part of the docking framework.
    /// </summary>
    public class HostWindow : DockWindow
    {
        #region Fields

        private Control content;
        private DockType dockType = DockType.ToolWindow;
        private FormBorderStyle originalBorderStyle;

        #endregion

        /// <summary>
        /// Gets the Control instance that is hosted on this window.
        /// </summary>
        [Browsable(false)]
        public Control Content
        {
            get
            {
                return content;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostWindow"/> class.
        /// </summary>
        public HostWindow()
        {

        }

        /// <summary>
        /// Initializes a new <see cref="HostWindow">HostWindow</see> instance with the specified Control as a content.
        /// </summary>
        /// <param name="control"></param>
        public HostWindow(Control control)
        {
            this.content = control;
            InitializeContent();
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        /// <param name="content">The content.</param>
        public void LoadContent(Control content)
        {
            this.VerifyContentChange();

            this.Controls.Clear();

            this.content = content;
            InitializeContent();
        }

        private void VerifyContentChange()
        {
            if (this.DockManager == null)
            {
                return;
            }

            Form currentContent = this.content as Form;
            if (currentContent == null)
            {
                return;
            }

            if (this.DockManager.MdiController.MdiChildren.Contains(currentContent))
            {
                throw new InvalidOperationException("Cannot reload content for a HostWindow, which is bound to a MDI Child.");
            }
        }

        // TO-DO: Make a Show method at DockWindow level
        /// <summary>
        /// Display the HostWindow if was previously hidden.
        /// </summary>
        public new void Show()
        {
            if (this.DockManager != null)
            {
                this.DockManager.DisplayWindow(this);
                return;
            }

            base.Show();
        }

        /// <summary>
        /// Initializes a new <see cref="HostWindow">HostWindow</see> instance with the specified Control as a content and using the desired DockType.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="dockType"></param>
        public HostWindow(Control control, DockType dockType)
            :this(control)
        {
            this.dockType = dockType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            //clear content reference
            if (e.Control == this.content)
            {
                this.content = null;
            }

            if (this.DockManager == null)
            {
                return;
            }

            //check whether a Form has been manually removed
            Form form = e.Control as Form;
            if (form != null && !form.Disposing && !form.IsDisposed)
            {
                this.DockManager.MdiController.OnFormRemoved(this, form);
                this.SetFormBorderStyle(form, this.originalBorderStyle, false);
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            if (this.content != null)
            {
                this.content.Bounds = this.DisplayRectangle;
            }
        }

        private void SetFormBorderStyle(Form form, FormBorderStyle style, bool copyOriginal)
        {
            //We need this cast since our Form shadows the base Form's FormBorderStyle property (it is not overriddable)
            RadFormControlBase radForm = form as RadFormControlBase;
            if (radForm != null)
            {
                if (copyOriginal)
                {
                    this.originalBorderStyle = radForm.FormBorderStyle;
                }
                radForm.FormBorderStyle = style;
            }
            else
            {
                if (copyOriginal)
                {
                    this.originalBorderStyle = form.FormBorderStyle;
                }
                form.FormBorderStyle = style;
            }
        }

        private void InitializeContent()
        {
            this.SuspendLayout();

            if (this.content is Form)
            {
                this.CloseAction = DockWindowCloseAction.CloseAndDispose;

                Form form = (Form)this.content;
                form.TopLevel = false;
                this.SetFormBorderStyle(form, FormBorderStyle.None, true);
            }

            this.Text = this.content.Text;
            this.Controls.Add(this.content);

            this.content.Show();

            if (this.IsHandleCreated && this.Visible)
            {
                this.content.Bounds = this.DisplayRectangle;
            }

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Gets the associated Content as a Form instance. Valid when used in standard MDI mode.
        /// </summary>
        [Browsable(false)]
        public Form MdiChild
        {
            get
            {
                if (this.DockManager == null || !this.DockManager.AutoDetectMdiChildren)
                {
                    return null;
                }

                return this.content as Form;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override DockType DockType
        {
            get
            {
                return this.dockType;
            }
        }

        protected internal override void OnClosing(DockWindowCancelEventArgs e)
        {
            base.OnClosing(e);

            if (this.MdiChild != null)
            {
                this.DockManager.MdiController.OnMdiChildClosing(e);
            }
        }
    }
}
