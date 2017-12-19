using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A helper class that exposes common methods, used across a docking framework.
    /// </summary>
    public static class DockHelper
    {
        /// <summary>
        /// Applies Maximum constraint to the specified Size structure.
        /// </summary>
        /// <param name="size">The size, which is the constraint target.</param>
        /// <param name="max">The size, which is the maximum allowed.</param>
        /// <returns></returns>
        public static Size EnsureSizeMaxBounds(Size size, Size max)
        {
            return EnsureSizeBounds(size, MinSize, max);
        }

        /// <summary>
        /// Applies Minimum constraint to the specified Size structure.
        /// </summary>
        /// <param name="size">The size, which is the constraint target.</param>
        /// <param name="min">The size, which is the minimum allowed.</param>
        /// <returns></returns>
        public static Size EnsureSizeMinBounds(Size size, Size min)
        {
            return EnsureSizeBounds(size, min, MaxSize);
        }

        /// <summary>
        /// Applies Minimum and Maximum constraints to the specified Size structure.
        /// </summary>
        /// <param name="size">The size, which is the constraint target.</param>
        /// <param name="min">The size, which is the minimum allowed.</param>
        /// <param name="max">The size, which is the maximum allowed.</param>
        /// <returns></returns>
        public static Size EnsureSizeBounds(Size size, Size min, Size max)
        {
            size.Width = Math.Max(min.Width, size.Width);
            size.Width = Math.Min(max.Width, size.Width);

            size.Height = Math.Max(min.Height, size.Height);
            size.Height = Math.Min(max.Height, size.Height);

            return size;
        }

        /// <summary>
        /// Applies Minimum and Maximum constraints to the specified SizeF structure.
        /// </summary>
        /// <param name="size">The size, which is the constraint target.</param>
        /// <param name="min">The size, which is the minimum allowed.</param>
        /// <param name="max">he size, which is the maximum allowed.</param>
        /// <returns></returns>
        public static SizeF EnsureSizeBounds(SizeF size, SizeF min, SizeF max)
        {
            size.Width = Math.Max(min.Width, size.Width);
            size.Width = Math.Min(max.Width, size.Width);

            size.Height = Math.Max(min.Height, size.Height);
            size.Height = Math.Min(max.Height, size.Height);

            return size;
        }

        /// <summary>
        /// Retrieves a Rectangle structure, that is aligned within the specified bounds, and is with the desired size.
        /// </summary>
        /// <param name="bounds">The outer Rectangle structure, used as an alignment</param>
        /// <param name="size">The size of the newly created rectangle.</param>
        /// <returns></returns>
        public static Rectangle CenterRect(Rectangle bounds, Size size)
        {
            Point location = new Point(bounds.X + (bounds.Width - size.Width) / 2, 
                                       bounds.Y + (bounds.Height - size.Height) / 2);

            return new Rectangle(location, size);
        }

        /// <summary>
        /// Determines whether a Drag operation should be started.
        /// </summary>
        /// <param name="curr">The current cursor location.</param>
        /// <param name="capture">The cursor location </param>
        /// <returns></returns>
        public static bool ShouldBeginDrag(Point curr, Point capture)
        {
            Size dragSize = SystemInformation.DragSize;
            Rectangle dragRect = new Rectangle(capture.X - dragSize.Width / 2,
                                               capture.Y - dragSize.Height / 2,
                                               dragSize.Width, dragSize.Height);
            return !dragRect.Contains(curr);
        }

        /// <summary>
        /// Retrieves an <see cref="AutoHidePosition">AutoHidePosition</see> value from the specified <see cref="DockPosition">DockPosition</see>.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static AutoHidePosition GetAutoHidePosition(DockPosition position)
        {
            switch (position)
            {
                case DockPosition.Left:
                    return AutoHidePosition.Left;
                case DockPosition.Top:
                    return AutoHidePosition.Top;
                case DockPosition.Right:
                    return AutoHidePosition.Right;
                case DockPosition.Bottom:
                    return AutoHidePosition.Bottom;
                default:
                    return AutoHidePosition.Auto;
            }
        }

        /// <summary>
        /// Retrieves an <see cref="AllowedDockState">AllowedDockState</see> value from the specified <see cref="DockState">DockState</see>.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static AllowedDockState GetAllowedDockState(DockState state)
        {
            switch (state)
            {
                case DockState.Docked:
                    return AllowedDockState.Docked;
                case DockState.TabbedDocument:
                    return AllowedDockState.TabbedDocument;
                case DockState.Hidden:
                    return AllowedDockState.Hidden;
                case DockState.AutoHide:
                    return AllowedDockState.AutoHide;
                case DockState.Floating:
                    return AllowedDockState.Floating;
                default:
                    return AllowedDockState.Hidden;
            }
        }

        /// <summary>
        /// Retrieves a <see cref="DockPosition">DockPosition</see> value from the specified <see cref="AllowedDockPosition">AllowedDockPosition</see>.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static DockPosition GetDockPosition(AllowedDockPosition position)
        {
            DockPosition dockPos = DockPosition.Fill;
            switch(position)
            {
                case AllowedDockPosition.Bottom:
                    dockPos = DockPosition.Bottom;
                    break;
                case AllowedDockPosition.Left:
                    dockPos = DockPosition.Left;
                    break;
                case AllowedDockPosition.Right:
                    dockPos = DockPosition.Right;
                    break;
                case AllowedDockPosition.Top:
                    dockPos = DockPosition.Top;
                    break;
            }

            return dockPos;
        }

        /// <summary>
        /// Finds the first RadSplitContainer instance, which contains both specified panels.
        /// </summary>
        /// <param name="child1"></param>
        /// <param name="child2"></param>
        /// <returns></returns>
        public static RadSplitContainer FindCommonAncestor(SplitPanel child1, SplitPanel child2)
        {
            if (child1 == null || child2 == null)
            {
                return null;
            }

            RadSplitContainer container1 = child1.SplitContainer;
            RadSplitContainer container2 = child2.SplitContainer;

            while (container1 != null && container2 != null)
            {
                if (ControlHelper.IsDescendant(container1, child2))
                {
                    return container1;
                }
                if (ControlHelper.IsDescendant(container2, child1))
                {
                    return container2;
                }

                container1 = container1.SplitContainer;
                container2 = container2.SplitContainer;
            }

            return null;
        }

        /// <summary>
        /// Traverses the tree of split containers and finds the panel,
        /// which is direct child of the specified container and contains the specified split panel as a descendant.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="descendant"></param>
        /// <returns></returns>
        public static SplitPanel GetDirectChildContainingPanel(RadSplitContainer container, SplitPanel descendant)
        {
            RadSplitContainer parent = container;
            SplitPanel curr = descendant;

            while (parent != null)
            {
                if (parent.SplitPanels.IndexOf(curr) != -1)
                {
                    return curr;
                }

                curr = curr.SplitContainer;
            }

            return null;
        }

        /// <summary>
        /// Performs a clean-up logic upon the specified RadSplitContainer instance, associated with the specified RadDock instance.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="dockManager"></param>
        public static void CleanupContainer(RadSplitContainer container, RadDock dockManager)
        {
            CollapseOrDisposeStrips(container, dockManager);
            MergeContainers(container);
        }

        /// <summary>
        /// Asks all <see cref="DockTabStrip">DockTabStrip</see> instances to check whether they need to be collapsed or disposed.
        /// Used in a clean-up pass of RadDock for a control tree defragmentation.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="dockManager"></param>
        public static void CollapseOrDisposeStrips(RadSplitContainer container, RadDock dockManager)
        {
            foreach (DockTabStrip strip in ControlHelper.GetChildControls<DockTabStrip>(container, true))
            {
                if (strip.DockManager == dockManager)
                {
                    strip.CheckCollapseOrDispose();
                }
            }
        }

        /// <summary>
        /// Collects all the DockWindow instances, residing on the specified parent, and associated with the provided RadDock instance.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="recursive"></param>
        /// <param name="dockManager"></param>
        /// <returns></returns>
        public static List<DockWindow> GetDockWindows(Control parent, bool recursive, RadDock dockManager)
        {
            List<DockWindow> descendants = new List<DockWindow>();
            foreach (Control child in ControlHelper.EnumChildControls(parent, recursive))
            {
                DockWindow window = child as DockWindow;
                if (window != null && window.DockManager == dockManager)
                {
                    descendants.Add(window);
                }
            }

            return descendants;
        }

        /// <summary>
        /// Collects all the RadSplitContainer instances, residing on the specified parent, and associated with the provided RadDock instance.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="recursive"></param>
        /// <param name="dockManager"></param>
        /// <returns></returns>
        public static List<RadSplitContainer> GetSplitContainers(Control parent, bool recursive, RadDock dockManager)
        {
            List<RadSplitContainer> descendants = new List<RadSplitContainer>();
            foreach (Control child in ControlHelper.EnumChildControls(parent, recursive))
            {
                RadSplitContainer container = child as RadSplitContainer;
                if (container != null && ControlHelper.FindAncestor<RadDock>(container) == dockManager)
                {
                    descendants.Add(container);
                }
            }

            return descendants;
        }

        /// <summary>
        /// Collects all the DockTabStrip instances, residing on the specified parent, and associated with the provided RadDock instance.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="recursive"></param>
        /// <param name="dockManager"></param>
        /// <returns></returns>
        public static List<T> GetDockTabStrips<T>(Control parent, bool recursive, RadDock dockManager) where T : DockTabStrip
        {
            List<T> descendants = new List<T>();
            foreach (Control child in ControlHelper.EnumChildControls(parent, recursive))
            {
                T strip = child as T;
                if (strip != null && strip.DockManager == dockManager)
                {
                    descendants.Add(strip);
                }
            }

            return descendants;
        }

        /// <summary>
        /// Defragments the tree of RadSplitContainer instances.
        /// Used by a RadDock control to clean-up unnecessary containers.
        /// </summary>
        /// <param name="parentContainer"></param>
        public static void MergeContainers(RadSplitContainer parentContainer)
        {
            if (parentContainer == null)
            {
                return;
            }

            parentContainer.SuspendLayout();

            //first delegate to child containers to ensure proper merging with parents
            RadSplitContainer childContainer;
            for (int i = 0; i < parentContainer.SplitPanels.Count; i++)
            {
                childContainer = parentContainer.SplitPanels[i] as RadSplitContainer;
                if (childContainer != null)
                {
                    MergeContainers(childContainer);
                }
            }

            //try to merge the current container with its parent
            if (parentContainer.MergeWithParentContainer())
            {
                parentContainer.Dispose();
            }
            else
            {
                parentContainer.ResumeLayout(true);
            }
        }

        /// <summary>
        /// Default maximum size.
        /// </summary>
        public static readonly Size MaxSize = new Size(Int32.MaxValue, Int32.MaxValue);
        /// <summary>
        /// Default minimum size.
        /// </summary>
        public static readonly Size MinSize = new Size(Int32.MinValue, Int32.MinValue);
    }
}
