using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Implements a special <see cref="DockWindow">DockWindow</see> that may reside within a document container only.
    /// </summary>
    public class DocumentWindow : DockWindow
    {
        #region Constructors

        /// <summary>
        /// Default contructor.
        /// </summary>
        public DocumentWindow()
        {
        }

        /// <summary>
        /// Initializes new DocumentWindow instance, using the provided Text.
        /// </summary>
        /// <param name="text"></param>
        public DocumentWindow(string text)
            : this()
        {
            this.Text = text;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        protected internal override DockWindowCloseAction DefaultCloseAction
        {
            get
            {
                return DockWindowCloseAction.CloseAndDispose;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal override DockState DefaultDockState
        {
            get
            {
                return DockState.TabbedDocument;
            }
        }

        /// <summary>
        /// Returns <see cref="Telerik.WinControls.UI.Docking.DockType.Document">Document</see>.
        /// </summary>
        public override DockType DockType
        {
            get
            {
                return DockType.Document;
            }
        }

        #endregion
    }
}
