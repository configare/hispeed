using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.UI.Multimedia;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This is a base class for a behavior that can be associated with a
    /// RadFormControlBase instance. The behavior defines the behavior and appearance of the form.
    /// </summary>
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    public abstract class FormControlBehavior : System.ComponentModel.Component
    {
        #region Fields

        private bool isDisposed = false;
        protected IComponentTreeHandler targetHandler;
        private WndProcInvoker baseWndProcCallback;
        private WndProcInvoker defWndProcCallback;

        private bool shouldHandleCreateChildItems = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates an instance of the RadFormBehaviorBase class.
        /// This instance has no Form associated with it.
        /// </summary>
        public FormControlBehavior()
        {
        }

        /// <summary>
        /// Creates an instance of the RadFormBehaviorBase class.
        /// </summary>
        /// <param name="targetTreeHandler">
        /// An implementation of the IComponentTreeHandler which
        /// this behavior applies to</param>
        public FormControlBehavior(IComponentTreeHandler targetTreeHandler)
        {
            this.targetHandler = targetTreeHandler;

            if (this.targetHandler != null)
            {
                RadFormControlBase form = this.targetHandler.RootElement.ElementTree.Control as RadFormControlBase;

                if (form == null)
                {
                    throw new ArgumentException("The implementation of IComponentTreeHandler is not of the correct type.");
                }

                this.OnFormAssociated();
            }
        }

        /// <summary>
        /// Creates an instance of the RadFormBehavior class.
        /// </summary>
        /// <param name="targetTreeHandler">
        /// An implementation of the IComponentTreeHandler which
        /// this behavior applies to</param>
        /// <param name="handleCreateChildItems"/>
        public FormControlBehavior(IComponentTreeHandler targetTreeHandler, bool handleCreateChildItems)
        {
            this.shouldHandleCreateChildItems = handleCreateChildItems;

            this.targetHandler = targetTreeHandler;

            if (this.targetHandler != null)
            {
                RadFormControlBase form = this.targetHandler.RootElement.ElementTree.Control as RadFormControlBase;

                if (form == null)
                {
                    throw new ArgumentException("The implementation of IComponentTreeHandler is not of the correct type.");
                }

                this.OnFormAssociated();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the RadElement instance that represents the root element
        /// containing the hierarchy that builds the visual appearance of the form.
        /// </summary>
        public abstract RadElement FormElement
        {
            get;
        }

        /// <summary>
        /// Determines whether the CreateChildItems call is 
        /// routed to the handler of the behavior.
        /// Used by the RadFormControlBase class.
        /// </summary>
        internal bool HandlesCreateChildItems
        {
            get
            {
                return this.shouldHandleCreateChildItems;
            }
        }

       #region Appearance properties

        /// <summary>
        /// Gets the width of the form border
        /// </summary>
        public abstract Padding BorderWidth
        {
            get;
        }

        /// <summary>
        /// Gets the height of the caption that is drawn
        /// by the behavior.
        /// </summary>
        public abstract int CaptionHeight
        {
            get;
        }

        /// <summary>
        /// Gets the margin that describes the size and posizition
        /// of the client area.
        /// </summary>
        public abstract Padding ClientMargin
        {
            get;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Occurs when a form is associated with the behavior.
        /// </summary>
        protected virtual void OnFormAssociated()
        {
        }

        internal void SetBaseWndProcCallback(WndProcInvoker callback)
        {
            this.baseWndProcCallback = callback;
        }

        internal void SetDefWndProcCallback(WndProcInvoker callback)
        {
            this.defWndProcCallback = callback;
        }

        protected void CallBaseWndProc(ref Message m)
        {
            if (this.baseWndProcCallback != null)
            {
                this.baseWndProcCallback(ref m);
            }
        }

        protected void CallDefWndProc(ref Message m)
        {
            if (this.defWndProcCallback != null)
            {
                this.defWndProcCallback(ref m);
            }
        }

        #region Abstract methods

        public abstract bool HandleWndProc(ref Message m);

        public abstract void InvalidateElement(RadElement element, Rectangle bounds);

        #endregion

        public virtual bool OnAssociatedFormPaint(PaintEventArgs args)
        {
            return false;
        }

        public virtual bool OnAssociatedFormPaintBackground(PaintEventArgs args)
        {
            return false;
        }

        public virtual CreateParams CreateParams(CreateParams parameters)
        {
            return parameters;
        }

        public virtual void CreateChildItems(RadElement parent)
        {
        }


        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    if (this.defWndProcCallback != null)
                    {
                        this.defWndProcCallback = null;
                    }

                    if (this.baseWndProcCallback != null)
                    {
                        this.baseWndProcCallback = null;
                    }
                }

                this.isDisposed = true;
            }
        }

        #endregion

    }
}
