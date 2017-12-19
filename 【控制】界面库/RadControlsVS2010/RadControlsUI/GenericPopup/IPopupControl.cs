using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// An interface for all Popup-forms used in RadControls for WinForms.
    /// </summary>
    public interface IPopupControl
    {
        /// <summary>
        /// Shows the IPopupControl at the specific location.
        /// </summary>
        /// <param name="alignmentRect">An instance of the Rectangle struct 
        /// which represents a portion of the screen which the IPopupControl
        /// is aligned to.</param>
        void ShowPopup(Rectangle alignmentRect);

        /// <summary>
        /// Closes the IPopupControl.
        /// </summary>
        void ClosePopup(RadPopupCloseReason reason);

        /// <summary>
        /// Tries to close the <see cref="IPopupControl"/>.
        /// </summary>
        /// <param name="closeInfo">An instance of the <see cref="PopupCloseInfo"/> class
        /// containing information about the close request.</param>
        void ClosePopup(PopupCloseInfo closeInfo);

        /// <summary>
        /// This method determines whether the IPopupControl can be closed.
        /// Used in the PopupManager class to prevent the IPopupControl from closing
        /// in specific occasions.
        /// </summary>
        /// <param name="reason">The reason why the IPopupControl is closed.</param>
        /// <returns>True if the IPopupControl can be closed, otherwise false.</returns>
        bool CanClosePopup(RadPopupCloseReason reason);

        /// <summary>
        /// Gets a <see cref="System.Collections.Generic.List&lt;T&gt;"/> instance that represents
        /// a collection of logical children of this IPopupControl.
        /// The OwnerPopup property of these children would point
        /// to this IPopupControl instance.
        /// </summary>
        List<IPopupControl> Children
        {
            get;
        }

        /// <summary>
        /// Gets the owner IPopupControl of this IPopupControl.
        /// </summary>
        IPopupControl OwnerPopup
        {
            get;
        }

        /// <summary>
        /// Gets the Bounds rectangle of the IPopupControl.
        /// </summary>
        Rectangle Bounds
        {
            get;
        }

        /// <summary>
        /// Gets the owner element of the IPopupControl.
        /// </summary>
        RadElement OwnerElement
        {
            get;
        }

        /// <summary>
        /// Executes when a key is pressed.
        /// </summary>
        /// <param name="keyData">An instance of the <see cref="System.Windows.Forms.Keys"/>
        /// struct which contains the key information.</param>
        /// <returns>A boolean value that determines whether the 
        /// IPopupControl processes the message.</returns>
        bool OnKeyDown(Keys keyData);

        /// <summary>
        /// Callback for handling the WM_MOUSEWHEEL message.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="delta"></param>
        /// <returns>True if the message is processed, false otherwise.</returns>
        bool OnMouseWheel(Control target, int delta);
    }
}
