using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class stores information about a close request sent to an <see cref="IPopupControl"/>.
    /// The class stores the reason for the close request, information about the operation result,
    /// and an instance to a context.
    /// </summary>
    public class PopupCloseInfo
    {
        #region Fields

        private object context;
        private RadPopupCloseReason closeReason;
        private bool closed;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates an instance of the <see cref="PopupCloseInfo"/> class.
        /// The default value of the Closed property is true.
        /// </summary>
        /// <param name="closeReason">A value from the <see cref="RadPopupCloseReason"/> enum
        /// that determines the reason for the close request.</param>
        /// <param name="context">A request context.</param>
        public PopupCloseInfo(RadPopupCloseReason closeReason, object context)
        {
            this.closeReason = closeReason;
            this.context = context;
            this.closed = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Defines whether the request is executed or not.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Closed
        {
            get
            {
                return this.closed;
            }
            internal set
            {
                this.closed = value;
            }
        }

        /// <summary>
        /// The reason for the close request.
        /// </summary>
        public RadPopupCloseReason CloseReason
        {
            get
            {
                return this.closeReason;
            }
        }

        /// <summary>
        /// The context associated with this the close request.
        /// </summary>
        public object Context
        {
            get
            {
                return this.context;
            }
        }

        #endregion
    }
}
