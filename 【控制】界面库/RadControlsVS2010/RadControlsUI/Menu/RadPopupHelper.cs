using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{

    /// <summary>
	/// Defines the direction in which the drop-down window will be shown relative to its parent.
	/// </summary>
	/// <remarks>
	///  This enumeration is used in such controls like menus, combo boxes, etc. for example.
	/// </remarks>
    public enum RadDirection
    {
        /// <summary>
        /// Indicates that the drop-down will be shown on the left side of the parent.
        /// </summary>
        Left,
        /// <summary>
        /// Indicates that the drop-down will be shown on the right side of the parent.
        /// </summary>
        Right,
        /// <summary>
        /// Indicates that the drop-down will be shown on the top side of the parent.
        /// </summary>
        Up,
        /// <summary>
        /// Indicates that the drop-down will be shown on the bottom side of the parent.
        /// </summary>
        Down
    }
    ///<exclude/>
    public class RadPopupHelper
    {
        /// <summary>
        /// Gets the screen rectangle of the provided screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        /// <param name="excludeTaskBar">Determines whether the taskbar is included in the result.</param>
        /// <returns>A Rectangle struct that contains the data about the bounds of the screen.</returns>
        private static Rectangle GetScreenBounds(Screen screen, bool excludeTaskBar)
        {
            return screen.WorkingArea;
        }

        private static Rectangle GetScreenBounds(Screen screen)
        {
            return screen.Bounds;
        }
        /// <summary>
        /// Gets the valid location for a context menu
        /// </summary>
        /// <param name="popupDirection"></param>
        /// <param name="location"></param>
        /// <param name="popupSize"></param>
        /// <param name="corected"></param>
        /// <returns></returns>
        public static Point GetValidLocationForContextMenu(RadDirection popupDirection, Point location,
            Size popupSize, ref bool corected)
		{
            Rectangle screenRect = GetScreenRect(location);
			Rectangle popupRect = new Rectangle(location, popupSize);
			Point firstLocation = location;

			switch (popupDirection)
			{
				case RadDirection.Left:
                    location = new Point(popupRect.Left - popupSize.Width, popupRect.Top);
                    break;
				case RadDirection.Up:
                    location = new Point(popupRect.Left, popupRect.Top - popupSize.Height);
                    break;
			}

			if (GetCorrectingDirection(popupRect, screenRect, ref popupDirection))
			{
				switch (popupDirection)
				{
					case RadDirection.Left:
                        location = new Point(popupRect.Left - popupSize.Width, popupRect.Top);
                        break;
					case RadDirection.Right:
                        location = new Point(popupRect.Right, popupRect.Top);
                        break;
					case RadDirection.Up:
                        location = new Point(popupRect.Left, popupRect.Top - popupSize.Height);
                        break;
					case RadDirection.Down:
                        location = new Point(popupRect.Left, popupRect.Bottom);
                        break;
				}

				// Prevent from recursive corrections
				popupRect = new Rectangle(location, popupSize);
				if (GetCorrectingDirection(popupRect, screenRect, ref popupDirection))
				{
					location = firstLocation;
				}
				corected = true;
			}
			
			popupRect.Location = location;
			popupRect = EnsureBoundsInScreen(popupRect, screenRect);

			return popupRect.Location;
		}

        /// <summary>
        /// 	<para>Gets the valid location for a drop-down (for menus, combo boxes,
        ///     etc.).</para>
        /// </summary>
        /// <remarks>
        ///     This method calculates: 
        ///     <para>1. The rectangle of the screen where the drop down should be shown</para>
        /// 	<para>2. The rectangle (in screen coordinates) of the owner element. Owner element
        ///     is the element that shows the drop-down and is connected to it - like a menu item
        ///     that shows its sub menus or a combobox element that shows its drop-down.</para>
        /// 	<para>After calculating the screen and the element rectangles this method calls the
        ///     basic method.
        ///     </para>
        /// </remarks>
        /// <param name="popupDirection"></param>
        /// <param name="popupSize"></param>
        /// <param name="owner"></param>
        /// <param name="ownerOffset">
        /// Offset in pixels from the owner element. When this is zero there is no space
        /// between the owner and the drop-down.
        /// </param>
        /// <param name="corected"></param>
		public static Point GetValidLocationForDropDown(RadDirection popupDirection,
            Size popupSize, RadElement owner, int ownerOffset, ref bool corected)
        {
            Rectangle screenRect = GetScreenRect(owner);
            Rectangle ownerRect = owner.ControlBoundingRectangle;
            if (owner.ElementTree != null)
                ownerRect = owner.ElementTree.Control.RectangleToScreen(ownerRect);

            Point location = GetValidLocationForDropDown(popupDirection, screenRect,
                popupSize, ownerRect, ownerOffset, ref corected);

            return location;
        }

        /// <summary>Gets the valid location for a drop-down (for menus, combo boxes, etc.).</summary>
        /// <remarks>
        /// The popup is not allowed to be outside the screen rectangle and to be shown over
        /// the <strong>ownerRect</strong>.
        /// </remarks>
        /// <param name="popupDirection"></param>
        /// <param name="screenRect"></param>
        /// <param name="popupSize"></param>
        /// <param name="ownerRect"></param>
        /// <param name="ownerOffset">
        /// Offset in pixels from the owner element. When this is zero there is no space
        /// between the owner and the drop-down.
        /// </param>
        /// <param name="corected"></param>
        public static Point GetValidLocationForDropDown(RadDirection popupDirection,
            Rectangle screenRect, Size popupSize, Rectangle ownerRect, int ownerOffset, ref bool corected)
        {
            Point firstLocation = CalcLocation(popupDirection, popupSize,
                ownerRect, ownerOffset);

            Point location = firstLocation;
            RadDirection correction = popupDirection;

            // Correct the location if necessary (only opposite directions are corrected)
            Rectangle popupRect = new Rectangle(location, popupSize);
            if (GetCorrectingDirection(popupRect, screenRect, ref correction))
            {
                location = CalcLocation(correction, popupSize,
                    ownerRect, ownerOffset);

                // Prevent from recursive corrections
                popupRect = new Rectangle(location, popupSize);
                if (GetCorrectingDirection(popupRect, screenRect, ref correction))
                {
                    location = firstLocation;
                }
				corected = true;
            }

            // Ensure that the whole popup rectangle is visible
            popupRect.Location = location;
            popupRect = EnsureBoundsInScreen(popupRect, screenRect);

            return popupRect.Location;
        }

        /// <summary>Gets a screen from a point on the desktop.</summary>
        /// <returns>
        /// A Screen object that contains the given point or the PrimaryScreen on
        /// error.
        /// </returns>
        /// <param name="pointInScreen">The point on the desktop that must be in the returned screen.</param>
        public static Screen GetScreen(Point pointInScreen)
        {
            Screen res = Screen.PrimaryScreen;
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                Rectangle screenBounds = GetScreenBounds(Screen.AllScreens[i]);
                if (screenBounds.Contains(pointInScreen))
                {
                    return Screen.AllScreens[i];
                }
            }
            return res;
        }

        /// <summary>
        /// Gets the rectangle of the screen that contains the biggest part of a given
        /// element.
        /// </summary>
        /// <returns>The rectangle of the primary screen on error.</returns>
        /// <param name="elementOnScreen">
        /// If the element is not added in a control or is not visible the rectangle of the
        /// primary screen is returned.
        /// </param>
        public static Rectangle GetScreenRect(RadElement elementOnScreen)
        {
            Rectangle screenRect = GetScreenBounds(Screen.PrimaryScreen);

            if (elementOnScreen != null && elementOnScreen.ElementTree != null &&
                elementOnScreen.ElementTree != null)
            {
                Point renderingCenter = new Point(elementOnScreen.Size.Width / 2, elementOnScreen.Size.Height / 2);
                Point center = elementOnScreen.PointToControl(renderingCenter);
                center = elementOnScreen.ElementTree.Control.PointToScreen(center);

                screenRect = GetScreenRect(center);
            }
            return screenRect;
        }

        /// <summary>Gets the rectangle of the screen that contains given point on the desktop.</summary>
        /// <returns>The rectangle of the primary screen on error.</returns>
        /// <param name="pointInScreen">The point on the desktop that must be in the returned screen rectangle.</param>
        public static Rectangle GetScreenRect(Point pointInScreen)
        {
            return GetScreenBounds(GetScreen(pointInScreen), true);
        }

        /// <summary>
        /// Ensures a drop-down rectangle is entirely visible in a given screen
        /// rectangle.
        /// </summary>
        public static Rectangle EnsureBoundsInScreen(Rectangle popupBounds, Rectangle screenRect)
        {
            if (!screenRect.IsEmpty)
            {
                if (popupBounds.Right > screenRect.Right)
                    popupBounds.X -= popupBounds.Right - screenRect.Right;
                if (popupBounds.Bottom > screenRect.Bottom)
                    popupBounds.Y -= popupBounds.Bottom - screenRect.Bottom;
                if (popupBounds.X < screenRect.X)
                    popupBounds.X = screenRect.X;
                if (popupBounds.Y < screenRect.Y)
                    popupBounds.Y = screenRect.Y;
            }

            return popupBounds;
        }

        private static Point CalcLocation(RadDirection popupDirection,
            Size popupSize, Rectangle ownerRect, int ownerOffset)
        {
            Point res = new Point(0, 0);
            switch (popupDirection)
            {
                case RadDirection.Left:
                    res = new Point(ownerRect.Left - ownerOffset - popupSize.Width, ownerRect.Top);
                    break;
                case RadDirection.Right:
                    res = new Point(ownerRect.Right + ownerOffset, ownerRect.Top);
                    break;
                case RadDirection.Up:
                    res = new Point(ownerRect.Left, ownerRect.Top - ownerOffset - popupSize.Height);
                    break;
                case RadDirection.Down:
                    res = new Point(ownerRect.Left, ownerRect.Bottom + ownerOffset);
                    break;
            }
            return res;
        }

        // return true if there is some correction
        private static bool GetCorrectingDirection(Rectangle popupRect, Rectangle screenRect,
            ref RadDirection correction)
        {
            bool res = true;
            if (popupRect.Left < screenRect.Left && correction == RadDirection.Left)
            {
                correction = RadDirection.Right;
            }
            else if (popupRect.Top < screenRect.Top && correction == RadDirection.Up)
            {
                correction = RadDirection.Down;
            }
            else if (popupRect.Right > screenRect.Right && correction == RadDirection.Right)
            {
                correction = RadDirection.Left;
            }
            else if (popupRect.Bottom > screenRect.Bottom && correction == RadDirection.Down)
            {
                correction = RadDirection.Up;
            }
            else
            {
                res = false;
            }
            return res;
        }

        public static void SetTopMost(IntPtr handle)
        {
            SetWindowPosition(handle, (IntPtr)NativeMethods.HWND_TOPMOST);
        }

        public static void SetWindowPosition(IntPtr handle, IntPtr parentForm)
        {
            Point location = Point.Empty;
            Control mock = Control.FromHandle(handle);
            if (mock != null)
            {
                location = mock.Location;
            }
            NativeMethods.SetWindowPos(new HandleRef(null, handle), new HandleRef(null, parentForm), location.X, location.Y, 0, 0, NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_SHOWWINDOW | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE);
        }

        //public static void SetWindowPosition(IntPtr handle, IntPtr parentForm, int x, int y)
        //{
        //    NativeMethods.SetWindowPos(handle, parentForm, x, y, 0, 0, NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_SHOWWINDOW | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE);
        //}

        public static bool SetVisibleCore(Control control)
        {
            SetVisibleCore(control, (IntPtr)NativeMethods.HWND_TOPMOST);
            return true;
        }

        public static bool SetVisibleCore(Control control, IntPtr parentForm)
        {
            SetWindowPosition(control.Handle, parentForm);
            NativeMethods.ShowWindow(control.Handle, 8);
            return true;
        }

        public static void ActivateForm(IntPtr form)
        {
            NativeMethods.SendMessage(new HandleRef(null, form), NativeMethods.WM_NCACTIVATE, 1, 0);
        }
    }
}
