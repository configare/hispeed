using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Drawing;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Themes.Design;
using System.Collections;
using System.Reflection;
using Telerik.WinControls.Keyboard;
using Telerik.WinControls.Assistance;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using Telerik.WinControls.Layouts;
using System.Globalization;
using System.Runtime.InteropServices;
namespace Telerik.WinControls
{
    public abstract class ComponentLayoutElementTree : ComponentElementTree
    {
        //Fields         
		//private bool isLayoutPending = true;
        private int layoutSuspendCount = 0;
        private bool controlLayoutLocked = false;
        private bool layoutInitialized = false;

        internal bool creatingChildItems = false;

        #region Constructors
        public ComponentLayoutElementTree(IComponentTreeHandler owner)
            : base(owner)
        {
        }
        #endregion

        internal protected virtual bool ScaleChildren
        {
            get
            {
                return false;
            }
        }

        internal protected bool IsLayoutSuspended
        {
            get
            {
                return (layoutSuspendCount > 0);
            }
        }

#if DEBUG
        [Browsable(true), Category(RadDesignCategory.LayoutCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMSLayoutSuspended
        {
            get
            {
                PropertyInfo pi = typeof(Control).GetProperty("IsLayoutSuspended", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.ExactBinding);
                return (bool)pi.GetValue(this.Control, null);
            }
        }
#endif

        internal void LockControlLayout()
        {
            this.controlLayoutLocked = true;
        }

        internal void UnlockControlLayout()
        {
            this.controlLayoutLocked = false;
        }

        protected virtual void PerformLayoutCore(RadElement affectedElement)
        {
            if (this.ComponentTreeHandler.Initializing || this.RootElement.ElementState != ElementState.Loaded)
            {
                return;
            }

            if (affectedElement != null && affectedElement.ElementState != ElementState.Loaded)
            {
                return;
            }

            this.RootElement.LayoutEngine.RegisterLayoutRunning();

            if (!this.RootElement.IsLayoutSuspended && (this.RootElement.AutoSizeMode != RadAutoSizeMode.FitToAvailableSize))
            {
                this.RootElement.Size = this.RootElement.GetPreferredSize(this.Control.Size);
            }
            else
            {
                if (this.RootElement.Size != this.Control.Size)
                {
                    this.RootElement.Size = this.Control.Size;
                }
                else
                {
                    this.RootElement.PerformLayout(affectedElement);
                }
            }

            CheckExplicitLayout(this.RootElement);

            this.RootElement.LayoutEngine.UnregisterLayoutRunning();
        }

        private void CheckExplicitLayout(RadElement element)
        {
            if (element.ElementState != ElementState.Loaded)
            {
                return;
            }

            if (element.PerformLayoutAfterFinishLayout == PerformLayoutType.SelfExplicit)
            {
                element.PerformLayoutAfterFinishLayout = PerformLayoutType.None;
                element.LayoutEngine.PerformLayout(element, true);
            }
        }

        /// <summary>
        /// Forces the element and controls to apply layout logic to child elements and
        /// controls.
        /// </summary>
        internal void PerformLayoutInternal(RadElement affectedElement)
        {
            if (!this.UseNewLayoutSystem)
            {
                if (!IsLayoutSuspended)
                {
                    if (this.RootElement.UseNewLayoutSystem)
                    {
                        this.RootElement.Size = this.Control.Size;
                    }
                    else
                    {
                        PerformLayoutCore(affectedElement);
                    }
                }
            }
        }

        internal protected void SuspendRadLayout()
        {
            layoutSuspendCount++;
            this.RootElement.SuspendLayout(true);

            if (this.UseNewLayoutSystem)
            {
                RoutedEventArgs args = new RoutedEventArgs(EventArgs.Empty, RootRadElement.RootLayoutSuspendedEvent);
                this.RootElement.RaiseTunnelEvent(this.RootElement, args);
            }
        }

        public void ResumeRadLayout(bool performLayout)
        {
            layoutSuspendCount--;

            this.RootElement.ResumeLayout(true, performLayout);

            if (this.UseNewLayoutSystem)
            {
                RoutedEventArgs args = new RoutedEventArgs(EventArgs.Empty, RootRadElement.RootLayoutResumedEvent);
                this.RootElement.RaiseTunnelEvent(this.RootElement, args);
            }
        }

        private bool IsHorizontallyStretchable()
        {
            AnchorStyles anchorMask = AnchorStyles.Left | AnchorStyles.Right;
            bool anchored = (this.Control.Anchor & anchorMask) == anchorMask;
            bool docked = this.Control.Dock == DockStyle.Top ||
                this.Control.Dock == DockStyle.Bottom ||
                this.Control.Dock == DockStyle.Fill;
            return anchored || docked;
        }

        private bool IsVerticallyStretchable()
        {
            AnchorStyles anchorMask = AnchorStyles.Top | AnchorStyles.Bottom;
            bool anchored = (this.Control.Anchor & anchorMask) == anchorMask;
            bool docked = this.Control.Dock == DockStyle.Left ||
                this.Control.Dock == DockStyle.Right ||
                this.Control.Dock == DockStyle.Fill;
            return anchored || docked;
        }

        internal protected virtual void OnLayout(LayoutEventArgs e, Rectangle ownerBounds,
            bool ownerAutoSize, Control ownerParent)
        {
            if (this.UseNewLayoutSystem)
            {
                this.ComponentTreeHandler.CallOnLayout(e);
            }
            else
            {
                if (!layoutInitialized)
                {
                    this.layoutInitialized = true;
                }

                if (!this.controlLayoutLocked)
                {
                    if (this.Control.AutoSize)
                    {
                        Rectangle bounds = this.RootElement.Bounds;
                        if (this.RootElement.StretchHorizontally)
                            bounds.Width = this.Control.Width;
                        if (this.RootElement.StretchVertically)
                            bounds.Height = this.Control.Height;
                        if (this.RootElement.StretchHorizontally || this.RootElement.StretchVertically)
                        {
                            this.RootElement.Bounds = bounds;
                        }
                    }
                    else
                    {
                        this.RootElement.Bounds = this.Control.Bounds;
                    }
                }

                this.ComponentTreeHandler.CallOnLayout(e);
            }
        }

        /// <summary>
        /// Retrieves the size of a rectangular area into which a control can be
        /// fitted. This override is called only when AutoSize is true.
        /// </summary>
        public virtual Size GetPreferredSize(Size sizeConstraints)
        {
            if (this.UseNewLayoutSystem)
            {
                Size proposedSize = this.ComponentTreeHandler.CallGetPreferredSize(sizeConstraints);
                if (this.IsHorizontallyStretchable())
                {
                    if (sizeConstraints.Width > 0)
                    {
                        proposedSize.Width = sizeConstraints.Width;
                    }
                    else
                    {
                        if (!this.ComponentTreeHandler.IsDesignMode)
                        {
                            sizeConstraints.Width = 0;
                            proposedSize.Width = sizeConstraints.Width;
                        }
                    }
                }

                if (this.IsVerticallyStretchable())
                {
                    if (sizeConstraints.Height > 0)
                    {
                        proposedSize.Height = sizeConstraints.Height;
                    }
                    else
                    {
                        if (!this.ComponentTreeHandler.IsDesignMode)
                        {
                            sizeConstraints.Height = 0;
                            proposedSize.Height = sizeConstraints.Height;
                        }
                    }
                }

                if (!this.ComponentTreeHandler.LayoutManager.IsUpdating)
                {
                    if (this.RootElement.GetBitState(RadElement.NeverMeasuredStateKey) || this.RootElement.GetBitState(RadElement.NeverArrangedStateKey))
                    {
                        Rectangle rect = new Rectangle(this.Control.Location, proposedSize);
                        PerformInnerLayout(true, rect.X, rect.Y, rect.Width, rect.Height);
                    }
                    else
                    {
                        this.RootElement.BitState[RootRadElement.DisableControlSizeSetStateKey] = true;
                        this.RootElement.Measure(proposedSize);
                        this.RootElement.UpdateLayout();
                        this.RootElement.BitState[RootRadElement.DisableControlSizeSetStateKey] = false;
                    }
                }

                Size res = Size.Round(this.RootElement.DesiredSize);

                if (this.RootElement.StretchHorizontally)
                    res.Width = proposedSize.Width;
                if (this.RootElement.StretchVertically)
                    res.Height = proposedSize.Height;

                return res;
            }
            else
            {
                if (this.RootElement.Size.IsEmpty)
                    return this.Control.Size;

                Size proposedSize = this.ComponentTreeHandler.CallGetPreferredSize(sizeConstraints);
                if (this.IsHorizontallyStretchable())
                    proposedSize.Width = sizeConstraints.Width;
                if (this.IsVerticallyStretchable())
                    proposedSize.Height = sizeConstraints.Height;

                Size res = proposedSize;
                if (!this.RootElement.StretchHorizontally)
                    res.Width = this.RootElement.Size.Width;
                if (!this.RootElement.StretchVertically)
                    res.Height = this.RootElement.Size.Height;

                return res;
            }
        }

        public void SetBounds(int x, int y, int width, int height, BoundsSpecified specified)
        {
            this.SetBoundsCore(x, y, width, height, specified);
        }

        internal protected virtual void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (this.UseNewLayoutSystem)
            {
                Size desiredSize = new Size(width, height);
                bool performMeasure = true;
                desiredSize = PerformInnerLayout(performMeasure, x, y, width, height);

                this.ComponentTreeHandler.CallSetBoundsCore(x, y, desiredSize.Width, desiredSize.Height, specified);
            }
            else
            {
                this.ComponentTreeHandler.CallSetBoundsCore(x, y, width, height, specified);
            }
        }

        // Return the size desired from the inner layout
        internal Size PerformInnerLayout(bool performMeasure, int x, int y, int width, int height)
        {
            if (!this.RootElement.GetBitState(RootRadElement.DisableControlSizeSetStateKey))
            {
                this.RootElement.BitState[RootRadElement.DisableControlSizeSetStateKey] = true;

                if (performMeasure)
                {
                    SizeF availableSize = new SizeF(width, height);
                    this.RootElement.InvalidateMeasure();
                    this.RootElement.Measure(availableSize);
                }
                SizeF newContentSize = this.RootElement.DesiredSize;

                if (this.RootElement.StretchHorizontally)
                    newContentSize.Width = width;
                if (this.RootElement.StretchVertically)
                    newContentSize.Height = height;

                // Gele was here: ??? Why InvalidateArrange is necessary - see unit tests ???
                //this.RootElement.InvalidateArrange();
                this.RootElement.Arrange(new RectangleF(x, y, newContentSize.Width, newContentSize.Height));
                this.RootElement.UpdateLayout();

                if (this.Control.AutoSize)
                {
                    Size temp = Size.Round(newContentSize);
                    width = temp.Width;
                    height = temp.Height;
                }

                this.RootElement.BitState[RootRadElement.DisableControlSizeSetStateKey] = false;
            }

            return new Size(width, height);
        }

        internal void ResetSize()
        {
            Rectangle bounds = this.Control.Bounds;
            Size newSize = PerformInnerLayout(true, bounds.X, bounds.Y, bounds.Width, bounds.Height);
            this.Control.Size = newSize;
        }

        private bool childItemsCreated = false;
        protected bool ChildItemsCreated
        {
            get { return childItemsCreated; }
        
        }

        internal void SetChildItemsCreated(bool value)
        {
            this.childItemsCreated = value;
        }


        internal void SetChildItemsCreating(bool value)
        {
            this.creatingChildItems = value;
        }

        protected bool CreatingChildItems
        {
            get { return creatingChildItems; }
        }

        internal protected virtual void OnAutoSizeChanged(EventArgs e)
        {
            if (this.ComponentTreeHandler.Initializing)
            {
                return;
            }

            if (this.UseNewLayoutSystem)
            {
                if (this.Control.AutoSize && !this.ComponentTreeHandler.LayoutManager.IsUpdating)
                {
                    this.RootElement.InvalidateMeasure();
                    this.RootElement.UpdateLayout();
                }
            }

            this.RootElement.OnControlAutoSizeChanged(this.Control.AutoSize);
        }

        #region Properties
#if DEBUG
        [Browsable(true)]
#else
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UseNewLayoutSystem
        {
            get
            {
                if (this.RootElement != null)
                    return this.RootElement.UseNewLayoutSystem;
                return false;
            }
            set
            {
                if (this.RootElement != null)
                {
                    this.RootElement.UseNewLayoutSystem = value;
                }
            }
        }

        #endregion
    }
}
