using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Defines the animation type used in popups
    /// </summary>
    public enum PopupAnimationTypes
    {
        /// <summary>
        /// No animation is applied.
        /// </summary>
        None = 0,

        /// <summary>
        /// The control fades in upon showing.
        /// </summary>
        Fade = 1,

        /// <summary>
        /// The control uses easing animation.
        /// </summary>
        Easing = 2,

        /// <summary>
        /// Both easing and fade animation will be applied.
        /// </summary>
        Both = Fade | Easing
    }

    /// <summary>
    /// Defines the type of fade animation.
    /// </summary>
    [Flags]
    public enum FadeAnimationType
    {
        /// <summary>
        /// No fade animation is applied.
        /// </summary>
        None = 0,
        /// <summary>
        /// The control fades in upon showing.
        /// </summary>
        FadeIn = 1,
        /// <summary>
        /// The control fades out upon closing.
        /// </summary>
        FadeOut = 2
    }

    /// <summary>
    /// Defines the horizontal alignment of the popup
    /// based on the alignment rectangle passed
    /// in the ShowPopup method.
    /// </summary>
    public enum HorizontalPopupAlignment
    {
        /// <summary>
        /// The left edge of the popup is aligned to the left edge of the alignment rectangle.
        /// </summary>
        LeftToLeft,
        /// <summary>
        /// The left edge of the popup is aligned to the right edge of the alignment rectangle.
        /// </summary>
        LeftToRight,
        /// <summary>
        /// The right edge of the popup is aligned to the left edge of the alignment rectangle.
        /// </summary>
        RightToLeft,
        /// <summary>
        /// The right edge of the popup is aligned to the right edge of the alignment rectangle.
        /// </summary>
        RightToRight
    }

    /// <summary>
    /// Defines the vertical alignment of the popup
    /// based on the alignment rectangle passed
    /// in the ShowPopup method.
    /// </summary>
    public enum VerticalPopupAlignment
    {
        /// <summary>
        /// The top edge of the popup is aligned to the top edge of the alignment rectangle.
        /// </summary>
        TopToTop,
        /// <summary>
        /// The top edge of the popup is aligned to the bottom edge of the alignment rectangle.
        /// </summary>
        TopToBottom,
        /// <summary>
        /// The bottom edge of the popup is aligned to the top edge of the alignment rectangle.
        /// </summary>
        BottomToTop,
        /// <summary>
        /// The bottom edge of the popup is aligned to the bottom edge of the alignment rectangle.
        /// </summary>
        BottomToBottom
    }

    /// <summary>
    /// Defines the popup alignment correction mode.
    /// The values of this enumerator define how the popup alignment
    /// is adjusted when the default aligning routine is not able
    /// to properly position the popup due to lack of screen space.
    /// </summary>
    public enum AlignmentCorrectionMode
    {
        /// <summary>
        /// No adjustments to the coordinates are applied.
        /// </summary>
        None,
        /// <summary>
        /// The coordinates are adjusted with the needed amount so that
        /// the popup remains visible in the current screen.
        /// </summary>
        Smooth,
        /// <summary>
        /// The coordinates are adjusted with the needed amount so that
        /// the popup remains visible in the current screen, whereby
        /// the popup edge is aligned with an edge of the alignment rectangle.
        /// </summary>
        SnapToEdges,
        /// <summary>
        /// The coordinates are adjusted with the needed amount so that
        /// the popup remains visible in the current screen, whereby
        /// the popup edge is aligned with an outer edge of the alignment rectangle.
        /// The popup does not cross the alignment rectangle bounds.
        /// </summary>
        SnapToOuterEdges
    }

    /// <summary>
    /// This enum defines how the size of a <see cref="RadPopupControlBase"/> is
    /// fitted to the screen bounds.
    /// </summary>
    [Flags]
    public enum FitToScreenModes
    {
        /// <summary>
        /// The size of the popup is not fit to the bounds of the screen.
        /// </summary>
        None = 0,
        /// <summary>
        /// The width of the popup is fit to the available space on the screen.
        /// </summary>
        FitWidth = 1,
        /// <summary>
        /// The height of the popup is fit to the available space on the screen.
        /// </summary>
        FitHeight = 2,
    }

    /// <summary>
    /// This eunm defines the possible screen space usage modes.
    /// </summary>
    public enum ScreenSpaceMode
    {
        /// <summary>
        /// The whole screen is used when positioning the popup.
        /// </summary>
        WholeScreen,
        /// <summary>
        /// The working area of the screen is used when positioning the popup.
        /// </summary>
        WorkingArea
    }

    /// <summary>
    /// An enum that defines the possible overlap modes which are
    /// used to position the popup when its location cannot be adjusted so
    /// that it meets all alignment and alignment correction requirements.
    /// </summary>
    public enum AlternativeCorrectionMode
    {
        /// <summary>
        /// The popup's bounds can overlap with the alignment rectangle.
        /// </summary>
        Overlap,
        /// <summary>
        /// The popup will be snapped to the first possible outer edge of the alignment rectangle so that it does not overlap it.
        /// The order of the considered edges depends on the popup alignment settings.
        /// </summary>
        Flip
    }
}
