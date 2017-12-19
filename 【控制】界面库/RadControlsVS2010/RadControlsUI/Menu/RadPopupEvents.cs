using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the method that will handle the RadPopupClosing event.
    /// </summary>
    /// <param name="sender">Represents the event sender.</param>
    /// <param name="args">Represents the <see cref="RadPopupClosingEventHandler">event arguments</see>.</param>
    public delegate void RadPopupClosingEventHandler(object sender, RadPopupClosingEventArgs args);
    /// <summary>
    ///     Represents the method that will handle the
    ///     <see cref="RadComboBox.DropDownClosed">DropDownClosed</see> event.
    ///     <param name="args">Represents the <see cref="RadPopupClosedEventArgs">event arguments</see>.</param>
    /// 	<param name="sender">Represents the sender of the event.</param>
    /// </summary>
    public delegate void RadPopupClosedEventHandler(object sender, RadPopupClosedEventArgs args);

    /// <summary>
    /// Represents a method which will handle the PopupOpening event.
    /// </summary>
    /// <param name="sender">Repretents the event sender.</param>
    /// <param name="args">Represents the <see cref="CancelEventArgs"/>event arguments</param>
    public delegate void RadPopupOpeningEventHandler(object sender, CancelEventArgs args);

    /// <summary>
    /// Represents a method which will handle the FadeAnimationFinished event.
    /// </summary>
    /// <param name="sender">Repretents the event sender.</param>
    /// <param name="args">Represents the <see cref="FadeAnimationEventArgs"/>event arguments</param>
    public delegate void RadPopupFadeAnimationFinishedEventHandler(object sender, FadeAnimationEventArgs args);


    /// <summary>
    /// Represents a method which will handle the PopupOpened event.
    /// </summary>
    /// <param name="sender">Repretents the event sender.</param>
    /// <param name="args">Represents the <see cref="RadPopupOpenedEventHandler"/>event arguments</param>
    public delegate void RadPopupOpenedEventHandler(object sender, EventArgs args);

    /// <summary>Defines the closing reasons for the popup.</summary>
    public enum RadPopupCloseReason
    {
        // Summary:
        //     Specifies that the popup was closed because another application has received
        //    the focus.
        /// <summary>
        /// 	Specifies that the popup was closed because
        ///     another application has received the
        ///     focus.
        /// </summary>
        AppFocusChange = 0,
        //
        // Summary:
        //     Specifies that the popup was closed because the mouse was clicked outside
        //     the popup.
        /// <summary>
        /// 	Specifies that the popup was closed because the
        ///     mouse was clicked outside the
        ///     popup.
        /// </summary>
        Mouse = 1,
        //
        // Summary:
        //     Specifies that popup was closed because of keyboard activity, such as the
        //     ESC key being pressed.
        /// <summary>
        /// 	Specifies that popup was closed because of
        ///     keyboard activity, such as the ESC key being
        ///     pressed.
        /// </summary>
        Keyboard = 3,
        //
        // Summary:
        //     Specifies that the popup was closed because ClosePopup() method was called.
        /// <summary>
        /// 	Specifies that the popup was closed because
        ///     ClosePopup() method had been called.
        /// </summary>
        CloseCalled = 4,
        /// <summary>
        /// Specifies that the popup was closed because its parent was closed.
        /// </summary>
        ParentClosed = 5
    }

    /// <summary>
    /// Instances of this class contain information
    /// about the fade animation finished event of a popup control.
    /// </summary>
    public class FadeAnimationEventArgs : EventArgs
    {
        #region Fields

        private bool isFadingIn;

        #endregion

        #region Ctor

        public FadeAnimationEventArgs(bool isFadingIn)
        {
            this.isFadingIn = isFadingIn;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a boolean value determining the type
        /// of the fade animation.
        /// </summary>
        public bool IsFadingIn
        {
            get
            {
                return this.isFadingIn;
            }
        }

        #endregion
    }

    /// <summary>
    /// Instances of this class contain information
    /// about the opening event of a popup control.
    /// </summary>
    public class RadPopupOpeningEventArgs : CancelEventArgs
    {
        #region Fields

        private Point customLocation;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates an instance of the <see cref="Telerik.WinControls.UI.RadPopupOpeningEventArgs"/>
        /// class.
        /// </summary>
        public RadPopupOpeningEventArgs(Point location)
        {
            this.customLocation = location;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an instance of the <see cref="System.Drawing.Point"/>
        /// struct which contains the coordinates which will be used
        /// to position the popup.
        /// </summary>
        public Point CustomLocation
        {
            get
            {
                return this.customLocation;
            }
            set
            {
                this.customLocation = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Represents event data of the RadPopupClosingEvent.
    /// </summary>
    public class RadPopupClosingEventArgs : CancelEventArgs
    {
        public readonly RadPopupCloseReason CloseReason;
        /// <summary>
        /// Initializes a new instance of the RadPopupClosingEventArgs class using the close reason.
        /// </summary>
        /// <param name="CloseReason"></param>
        public RadPopupClosingEventArgs(RadPopupCloseReason CloseReason)
        {
            this.CloseReason = CloseReason;
        }

        public RadPopupClosingEventArgs(RadPopupCloseReason CloseReason, bool cancel)
            : base(cancel)
        {
            this.CloseReason = CloseReason;
        }
    }
    /// <summary>
    /// Represents event data of the RadPopupClosed event.
    /// </summary>
    public class RadPopupClosedEventArgs : EventArgs
    {
        public readonly RadPopupCloseReason CloseReason;
        /// <summary>
        /// Initializes a new instance of the RadPopupClosedEventArgs class using 
        /// the closing reason.
        /// </summary>
        /// <param name="CloseReason">
        ///  closing reason 
        /// </param>
        public RadPopupClosedEventArgs(RadPopupCloseReason CloseReason)
        {
            this.CloseReason = CloseReason;
        }
    }
}
