using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Diagnostics;
using System.Drawing;

namespace Telerik.WinControls
{
    /// <summary>
    /// Encapsulates common mothods related with Control Tree.
    /// </summary>
    public static class ControlHelper
    {
        #region Public Methods

        public static bool GetAnyDisposingInHierarchy(Control child)
        {
            Control current = child;
            while (current != null)
            {
                if (current.IsDisposed || current.Disposing)
                {
                    return true;
                }

                current = current.Parent;
            }

            return false;
        }

        public static Control GetControlUnderMouse()
        {
            Point mouse = Control.MousePosition;
            IntPtr hitWindow = NativeMethods.WindowFromPoint(mouse.X, mouse.Y);
            if (hitWindow == IntPtr.Zero)
            {
                return null;
            }

            return Control.FromChildHandle(hitWindow);
        }

        /// <summary>
        /// Brings the window on top of the z-order.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="activate"></param>
        public static void BringToFront(IntPtr handle, bool activate)
        {
            UpdateZOrder(new HandleRef(null, handle), NativeMethods.HWND_TOP, activate);
        }

        /// <summary>
        /// Sends the 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="activate"></param>
        public static void SendToBack(IntPtr handle, bool activate)
        {
            UpdateZOrder(new HandleRef(null, handle), NativeMethods.HWND_BOTTOM, activate);
        }

        /// <summary>
        /// Forces the non-client area of the specified Control instance to be re-evaluated.
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="activate"></param>
        public static void InvalidateNonClient(Control frame, bool activate)
        {
            if(!frame.IsHandleCreated || frame.RecreatingHandle)
            {
                return;
            }

            int flags = NativeMethods.SWP_FRAMECHANGED | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOZORDER;
            if (!activate)
            {
                flags |= NativeMethods.SWP_NOACTIVATE;
            }

            NativeMethods.SetWindowPos(new HandleRef(null, frame.Handle), new HandleRef(null, IntPtr.Zero), 0, 0, 0, 0, flags);
        }

        /// <summary>
        /// Determines whether the specified Child is contained within the specified Parent's Control Tree.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static bool IsDescendant(Control parent, Control child)
        {
            Control currParent = child.Parent;
            while (currParent != null)
            {
                if (currParent == parent)
                {
                    return true;
                }

                currParent = currParent.Parent;
            }

            return false;
        }

        /// <summary>
        /// Gets the Control instance that currently contains the Keyboard focus.
        /// </summary>
        /// <returns></returns>
        public static Control GetFocusedControl()
        {
            IntPtr focused = NativeMethods.GetFocus();
            if (focused == IntPtr.Zero)
            {
                return null;
            }

            return Control.FromChildHandle(focused);
        }

        /// <summary>
        /// Determines whether the specified ControlStyle is applied to the provided control.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static bool GetControlStyle(Control instance, ControlStyles style)
        {
            MethodInfo mi = typeof(Control).GetMethod("GetStyle", BindingFlags.Instance | BindingFlags.NonPublic);
            Debug.Assert(mi != null, "Unknown GetStyle method");
            Debug.Assert(mi.ReturnType == typeof(bool), "GetStyle method should return boolean value");
            if (mi == null || mi.ReturnType != typeof(bool))
            {
                return false;
            }

            return (bool)mi.Invoke(instance, new object[] { style });
        }

        /// <summary>
        /// Sends a WM_SETREDRAW message to the control, preventing any paint operation afterwards.
        /// </summary>
        /// <param name="control"></param>
        public static void BeginUpdate(Control control)
        {
            if (control == null || !control.IsHandleCreated || !control.Visible)
            {
                return;
            }

            NativeMethods.SendMessage(new HandleRef(null, control.Handle), NativeMethods.WM_SETREDRAW, (IntPtr)0, (IntPtr)0);
        }

        /// <summary>
        /// Resumes Control's painting, previously suspended by a BeginUpdate call.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="invalidate"></param>
        public static void EndUpdate(Control control, bool invalidate)
        {
            if (control == null || !control.IsHandleCreated || !control.Visible)
            {
                return;
            }

            NativeMethods.SendMessage(new HandleRef(null, control.Handle), NativeMethods.WM_SETREDRAW, (IntPtr)1, (IntPtr)0);
            if (invalidate)
            {
                control.Invalidate(true);
            }
        }

        /// <summary>
        /// Enumerates the Control tree, starting from the provided parent as a root,
        /// and collects all the child controls that match the specified filter.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="filter"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static List<Control> FilterChildControls(Control parent, Filter filter, bool recursive)
        {
            List<Control> children = new List<Control>();

            foreach (Control child in EnumChildControls(parent, recursive))
            {
                if (filter.Match(child))
                {
                    children.Add(child);
                }
            }

            return children;
        }

        public static List<T> GetChildControls<T>(Control parent) where T : Control
        {
            return GetChildControls<T>(parent, false);
        }

        /// <summary>
        /// Gets the Control of type T that is descendant of the specified parent and is anchored to the specified current T instance.
        /// </summary>
        /// <typeparam name="T">A Control of Type T.</typeparam>
        /// <param name="parent">The parent control, which descendants are to be examined.</param>
        /// <param name="curr">The current T instance to start the search from.</param>
        /// <param name="recursive">True to perform depth-first traversal of the Control Tree, false to look-up direct children only.</param>
        /// <param name="forward">True to search for a T instance that is next to the current specified one, false to search for a T instance that is previous to the current specified one.</param>
        /// <param name="wrap">True to start the search from the beginning when end of the search is reached.</param>
        /// <returns></returns>
        public static T GetNextControl<T>(Control parent, T curr, bool recursive, bool forward, bool wrap) where T : Control
        {
            if (parent == null || curr == null)
            {
                return null;
            }

            List<T> children = GetChildControls<T>(parent, recursive);

            int currIndex = children.IndexOf(curr);
            if (currIndex < 0)
            {
                return null;
            }

            int count = children.Count;

            if (forward)
            {
                currIndex++;
                if (currIndex > count - 1)
                {
                    if (!wrap)
                    {
                        return null;
                    }

                    currIndex = 0;
                }
            }
            else
            {
                currIndex--;
                if (currIndex < 0)
                {
                    if (!wrap)
                    {
                        return null;
                    }
                    currIndex = count - 1;
                }
            }

            return children[currIndex];
        }

        /// <summary>
        /// Gets the first Control of Type T, which is descendant of the specified Parent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static T GetFirstControl<T>(Control parent, bool recursive) where T : Control
        {
            foreach (Control child in EnumChildControls(parent, recursive))
            {
                if (child is T)
                {
                    return child as T;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the last Control of Type T, which is descendant of the specified Parent.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static T GetLastControl<T>(Control parent, bool recursive) where T : Control
        {
            T last = null;
            foreach (Control child in EnumChildControls(parent, recursive))
            {
                if (child is T)
                {
                    last = child as T;
                }
            }

            return last;
        }

        /// <summary>
        /// Collects all child controls of given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static List<T> GetChildControls<T>(Control parent, bool recursive) where T : Control
        {
            List<T> children = new List<T>();
            foreach (Control child in EnumChildControls(parent, recursive))
            {
                if (child is T)
                {
                    children.Add((T)child);
                }
            }

            return children;
        }

        /// <summary>
        /// Enumerates all child controls of the specified parent and optionally traverses the entire tree using Depth-first approach.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        public static IEnumerable<Control> EnumChildControls(Control parent, bool recursive)
        {
            foreach (Control c in parent.Controls)
            {
                yield return c;

                if (recursive)
                {
                    foreach (Control child in EnumChildControls(c, recursive))
                    {
                        yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// Searches up the parent chain of controls, looking for an ancestor of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns></returns>
        public static T FindAncestor<T>(Control child) where T : Control
        {
            if (child == null)
            {
                return null;
            }

            Control parent = child.Parent;
            while (parent != null)
            {
                if (parent is T)
                {
                    return (T)parent;
                }

                parent = parent.Parent;
            }

            return null;
        }

        public static Control FindDescendant(Control parent, Type descendantType)
        {
            foreach (Control control in ControlHelper.EnumChildControls(parent, true))
            {
                if (control.GetType() == descendantType)
                {
                    return control;
                }
            }

            return null;
        }

        /// <summary>
        /// Searches down the control tree, using breadth-first approach, for a descendant of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T FindDescendant<T>(Control parent) where T : Control
        {
            if (parent == null)
            {
                return null;
            }

            Queue<Control> children = new Queue<Control>();
            children.Enqueue(parent);

            while (children.Count > 0)
            {
                Control child = children.Dequeue();
                foreach (Control nestedChild in child.Controls)
                {
                    if (nestedChild is T)
                    {
                        return (T)nestedChild;
                    }

                    children.Enqueue(nestedChild);
                }
            }

            return null;
        }

        #endregion

        #region Private Implementation

        private static void UpdateZOrder(HandleRef handle, HandleRef pos, bool activate)
        {
            int flags = 0x603;
            if (!activate)
            {
                flags |= 0x10;
            }
            NativeMethods.SetWindowPos(handle, pos, 0, 0, 0, 0, flags);
        }

        #endregion
    }
}
