using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    [ToolboxItem(false)]
    [Designer("Telerik.WinControls.UI.Design.RadSplitterDesigner, Telerik.WinControls.UI.Design, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e")]
    public class RadSplitter : RadControl
    {
        // Fields
        private const int DefaultThumbLength = 50;

        private Point anchor;
        private BorderStyle borderStyle;
        private const int defaultWidth = 3;
        private const int DRAW_END = 3;
        private const int DRAW_MOVE = 2;
        private const int DRAW_START = 1;
        private static readonly object EVENT_MOVED;
        private static readonly object EVENT_MOVING;
        private int initTargetSize;
        private int lastDrawSplit;
        private int maxSize;
        private int minExtra;
        private int minSize;
        private int splitSize;
        private Control splitTarget;
        private SplitterMessageFilter splitterMessageFilter;
        private int splitterThickness;

        private SplitterElement splitterElement;
        private SplitterManager splitterManager;
        private SplitterCollapsedState collapsedState;
        

        private int thumbLength = DefaultThumbLength;
        private int restoreSize = 0;

        private class SplitData
        {
            // Fields
            public int dockHeight;
            public int dockWidth;
            internal Control target;

            // Methods
            public SplitData()
            {
                this.dockWidth = -1;
                this.dockHeight = -1;
            }
        }

        private class SplitterMessageFilter : IMessageFilter
        {
            // Fields
            private RadSplitter owner;

            // Methods
            public SplitterMessageFilter(RadSplitter splitter)
            {
                this.owner = splitter;
            }

            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public bool PreFilterMessage(ref Message m)
            {
                if ((m.Msg < 0x100) || (m.Msg > 0x108))
                {
                    return false;
                }
                if ((m.Msg == 0x100) && (((int)((long)m.WParam)) == 0x1b))
                {
                    this.owner.SplitEnd(false);
                }

                return true;
            }
        }

        static RadSplitter()
        {
            EVENT_MOVING = new object();
            EVENT_MOVED = new object();
        }

        public RadSplitter()
        {
            this.anchor = Point.Empty;
            this.splitSize = -1;
            this.splitterThickness = 3;
            this.lastDrawSplit = -1;
            base.SetStyle(ControlStyles.Selectable, false);
            this.TabStop = false;
            this.minSize = 0;
            this.minExtra = 0;
            this.Dock = DockStyle.Left;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            this.splitterManager = SplitterManager.CreateManager(this.Parent);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SplitterElement SplitterElement
        {
            get { return splitterElement; }
        }

        public bool Collapse(SplitterCollapsedState target)
        {
            if (this.IsCollapsed)
            {
                if (target == SplitterCollapsedState.None)
                {
                    return Expand();
                }

                return false;
            }

            if (target == SplitterCollapsedState.Previous)
            {
                CollapseToPrev();
            }

            if (target == SplitterCollapsedState.Next)
            {
                CollapseToNext();
            }

            return true;
        }

        public bool Expand()
        {
            this.collapsedState = SplitterCollapsedState.None;
            this.SplitPosition = this.restoreSize;

            if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
            {
                this.splitterElement.PrevArrow.Direction = ArrowDirection.Left;
                this.splitterElement.NextArrow.Direction = ArrowDirection.Right;
            }
            else
            {
                this.splitterElement.PrevArrow.Direction = ArrowDirection.Up;
                this.splitterElement.NextArrow.Direction = ArrowDirection.Down;
            }

            this.splitterElement.PrevNavigationButton.Visibility = ElementVisibility.Visible;
            this.splitterElement.NextNavigationButton.Visibility = ElementVisibility.Visible;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsCollapsed
        {
            get
            {
                return this.collapsedState != SplitterCollapsedState.None;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SplitterCollapsedState CollapsedState
        {
            get
            {
                return this.collapsedState;
            }
        }

        private void CollapseToPrev()
        {
            this.collapsedState = SplitterCollapsedState.Previous;
            this.restoreSize = this.SplitPosition;
            this.SplitPosition = 0;

            if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
            {
                this.splitterElement.PrevArrow.Direction = ArrowDirection.Right;
            }
            else
            {
                this.splitterElement.PrevArrow.Direction = ArrowDirection.Down;
            }

            this.splitterElement.NextNavigationButton.Visibility = ElementVisibility.Collapsed;
        }

        private void CollapseToNext()
        {
            this.collapsedState = SplitterCollapsedState.Next;
            this.restoreSize = this.SplitPosition;
            this.SplitPosition = int.MaxValue;

            if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
            {
                this.splitterElement.NextArrow.Direction = ArrowDirection.Left;
            }
            else
            {
                this.splitterElement.NextArrow.Direction = ArrowDirection.Up;
            }

            this.splitterElement.PrevNavigationButton.Visibility = ElementVisibility.Collapsed;
        }

        private void RestoreArrows()
        {
            if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
            {
                this.splitterElement.PrevArrow.Direction = ArrowDirection.Left;
                this.splitterElement.NextArrow.Direction = ArrowDirection.Right;
            }
            else
            {
                this.splitterElement.PrevArrow.Direction = ArrowDirection.Up;
                this.splitterElement.NextArrow.Direction = ArrowDirection.Down;
            }
        }

        /// <summary>
        /// Get or set thumb size
        /// </summary>
        [DefaultValue(DefaultThumbLength)]
        public int ThumbLength
        {
            get { return thumbLength; }
            set { thumbLength = value; }
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);

            this.splitterElement = new SplitterElement();
            this.splitterElement.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            parent.Children.Add(this.splitterElement);

            RestoreArrows();
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(6, 200);
            }
        }

        private void ApplySplitPosition()
        {
            this.SplitPosition = this.splitSize;
        }

        private SplitData CalcSplitBounds()
        {
            SplitData data = new SplitData();
            Control control = this.FindTarget();
            data.target = control;
            if (control != null)
            {
                switch (control.Dock)
                {
                    case DockStyle.Top:
                    case DockStyle.Bottom:
                        this.initTargetSize = control.Bounds.Height;
                        break;

                    case DockStyle.Left:
                    case DockStyle.Right:
                        this.initTargetSize = control.Bounds.Width;
                        break;
                }

                Control parentInternal = this.ParentInternal;
                Control.ControlCollection controls = parentInternal.Controls;
                int count = controls.Count;
                int num2 = 0;
                int num3 = 0;
                for (int i = 0; i < count; i++)
                {
                    Control control3 = controls[i];
                    if (control3 != control)
                    {
                        switch (control3.Dock)
                        {
                            case DockStyle.Top:
                            case DockStyle.Bottom:
                                num3 += control3.Height;
                                break;

                            case DockStyle.Left:
                            case DockStyle.Right:
                                num2 += control3.Width;
                                break;
                        }
                    }
                }

                Size clientSize = parentInternal.ClientSize;
                if (this.Horizontal)
                {
                    this.maxSize = (clientSize.Width - num2) - this.minExtra;
                }
                else
                {
                    this.maxSize = (clientSize.Height - num3) - this.minExtra;
                }
                data.dockWidth = num2;
                data.dockHeight = num3;
            }
            return data;
        }

        private Rectangle CalcSplitLine(int splitSize, int minWeight)
        {
            Rectangle bounds = base.Bounds;
            Rectangle rectangle2 = this.splitTarget.Bounds;
            switch (this.Dock)
            {
                case DockStyle.Top:
                    if (bounds.Height < minWeight)
                    {
                        bounds.Height = minWeight;
                    }
                    bounds.Y = rectangle2.Y + splitSize;
                    return bounds;

                case DockStyle.Bottom:
                    if (bounds.Height < minWeight)
                    {
                        bounds.Height = minWeight;
                    }
                    bounds.Y = ((rectangle2.Y + rectangle2.Height) - splitSize) - bounds.Height;
                    return bounds;

                case DockStyle.Left:
                    if (bounds.Width < minWeight)
                    {
                        bounds.Width = minWeight;
                    }
                    bounds.X = rectangle2.X + splitSize;
                    return bounds;

                case DockStyle.Right:
                    if (bounds.Width < minWeight)
                    {
                        bounds.Width = minWeight;
                    }
                    bounds.X = ((rectangle2.X + rectangle2.Width) - splitSize) - bounds.Width;
                    return bounds;
            }
            return bounds;
        }

        private int CalcSplitSize()
        {
            Control control = this.FindTarget();
            if (control != null)
            {
                Rectangle bounds = control.Bounds;
                switch (this.Dock)
                {
                    case DockStyle.Top:
                    case DockStyle.Bottom:
                        return bounds.Height;

                    case DockStyle.Left:
                    case DockStyle.Right:
                        return bounds.Width;
                }
            }
            return -1;
        }

        private void DrawSplitBar(int mode)
        {
            if ((mode != 1) && (this.lastDrawSplit != -1))
            {
                this.DrawSplitHelper(this.lastDrawSplit);
                this.lastDrawSplit = -1;
            }
            else if ((mode != 1) && (this.lastDrawSplit == -1))
            {
                return;
            }
            if (mode != 3)
            {
                this.DrawSplitHelper(this.splitSize);
                this.lastDrawSplit = this.splitSize;
            }
            else
            {
                if (this.lastDrawSplit != -1)
                {
                    this.DrawSplitHelper(this.lastDrawSplit);
                }
                this.lastDrawSplit = -1;
            }
        }

        internal virtual Control ParentInternal
        {
            get
            {
                return base.Parent;
            }
            set
            {
                if (base.Parent != value)
                {
                    if (value != null)
                    {
                        value.Controls.Add(this);
                    }
                    else
                    {
                        base.Parent.Controls.Remove(this);
                    }
                }
            }
        }

        private void DrawSplitHelper(int splitSize)
        {
            if (this.splitTarget != null)
            {
                Rectangle rectangle = this.CalcSplitLine(splitSize, 3);
                IntPtr handle = this.ParentInternal.Handle;
                IntPtr ptr2 = NativeMethods.GetDCEx(new HandleRef(this.ParentInternal, handle), NativeMethods.NullHandleRef, 0x402);
                IntPtr ptr3 = TelerikPaintHelper.CreateHalftoneBrush();
                IntPtr ptr4 = NativeMethods.SelectObject(new HandleRef(this.ParentInternal, ptr2), new HandleRef(null, ptr3));
                NativeMethods.PatBlt(new HandleRef(this.ParentInternal, ptr2), rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, 0x5a0049);
                NativeMethods.SelectObject(new HandleRef(this.ParentInternal, ptr2), new HandleRef(null, ptr4));
                NativeMethods.DeleteObject(new HandleRef(null, ptr3));
                NativeMethods.ReleaseDC(new HandleRef(this.ParentInternal, handle), new HandleRef(null, ptr2));
            }
        }

        private Control FindTarget()
        {
            Control parentInternal = this.ParentInternal;
            if (parentInternal != null)
            {
                Control.ControlCollection controls = parentInternal.Controls;
                int count = controls.Count;
                DockStyle dock = this.Dock;
                for (int i = 0; i < count; i++)
                {
                    Control control2 = controls[i];
                    if (control2 != this)
                    {
                        switch (dock)
                        {
                            case DockStyle.Top:
                                if (control2.Bottom != base.Top)
                                {
                                    break;
                                }
                                return control2;

                            case DockStyle.Bottom:
                                if (control2.Top != base.Bottom)
                                {
                                    break;
                                }
                                return control2;

                            case DockStyle.Left:
                                if (control2.Right != base.Left)
                                {
                                    break;
                                }
                                return control2;

                            case DockStyle.Right:
                                if (control2.Left != base.Right)
                                {
                                    break;
                                }
                                return control2;
                        }
                    }
                }
            }
            return null;
        }

        private int GetSplitSize(int x, int y)
        {
            int num;
            if (this.Horizontal)
            {
                num = x - this.anchor.X;
            }
            else
            {
                num = y - this.anchor.Y;
            }
            int num2 = 0;
            switch (this.Dock)
            {
                case DockStyle.Top:
                    num2 = this.splitTarget.Height + num;
                    break;

                case DockStyle.Bottom:
                    num2 = this.splitTarget.Height - num;
                    break;

                case DockStyle.Left:
                    num2 = this.splitTarget.Width + num;
                    break;

                case DockStyle.Right:
                    num2 = this.splitTarget.Width - num;
                    break;
            }
            return Math.Max(Math.Min(num2, this.maxSize), this.minSize);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if ((this.splitTarget != null) && (e.KeyCode == Keys.Escape))
            {
                this.SplitEnd(false);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.IsCollapsed || ThumbHitTest(e.Location)) return;

            if ((e.Button == MouseButtons.Left) && (e.Clicks == 1))
            {
                this.SplitBegin(e.X, e.Y);
            }
        }

        public bool ThumbHitTest(Point pt)
        {
            Rectangle prevRect = new Rectangle(this.splitterElement.PrevNavigationButton.PointToControl(Point.Empty), this.splitterElement.PrevNavigationButton.Size);
            Rectangle nextRect = new Rectangle(this.splitterElement.NextNavigationButton.PointToControl(Point.Empty), this.splitterElement.NextNavigationButton.Size);

            if (prevRect.Contains(pt) || nextRect.Contains(pt))
            {
                return true;
            }

            return false;
        }

        private RadItem GetThumbFromPoint(Point pt)
        {
            Rectangle prevRect = new Rectangle(this.splitterElement.PrevNavigationButton.PointToControl(Point.Empty), this.splitterElement.PrevNavigationButton.Size);
            if (prevRect.Contains(pt))
            {
                return this.splitterElement.PrevNavigationButton;
            }

            Rectangle nextRect = new Rectangle(this.splitterElement.NextNavigationButton.PointToControl(Point.Empty), this.splitterElement.NextNavigationButton.Size);
            if (nextRect.Contains(pt))
            {
                return this.splitterElement.NextNavigationButton;
            }

            return null;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.IsCollapsed || ThumbHitTest(e.Location))
            {
                this.Cursor = Cursors.Arrow;
                return;
            }

            this.Cursor = this.DefaultCursor;
            if (this.splitTarget != null)
            {
                int x = e.X + base.Left;
                int y = e.Y + base.Top;
                Rectangle rectangle = this.CalcSplitLine(this.GetSplitSize(e.X, e.Y), 0);
                int splitX = rectangle.X;
                int splitY = rectangle.Y;
                this.OnSplitterMoving(new SplitterEventArgs(x, y, splitX, splitY));
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (ThumbHitTest(e.Location))
            {
                if (this.IsCollapsed)
                {
                    Expand();
                    return;
                }

                RadItem thumb = GetThumbFromPoint(e.Location);
                if (thumb == this.splitterElement.PrevNavigationButton)
                {
                    CollapseToPrev();
                }

                if (thumb == this.splitterElement.NextNavigationButton)
                {
                    CollapseToNext();
                }
            }

            if (this.splitTarget != null)
            {
                int x = e.X;
                int left = base.Left;
                int y = e.Y;
                int top = base.Top;
                Rectangle rectangle = this.CalcSplitLine(this.GetSplitSize(e.X, e.Y), 0);
                int num5 = rectangle.X;
                int num6 = rectangle.Y;
                this.SplitEnd(true);
            }
        }

        protected virtual void OnSplitterMoved(SplitterEventArgs sevent)
        {
            SplitterEventHandler handler = (SplitterEventHandler)base.Events[EVENT_MOVED];
            if (handler != null)
            {
                handler(this, sevent);
            }
            if (this.splitTarget != null)
            {
                this.SplitMove(sevent.SplitX, sevent.SplitY);
            }
        }

        protected virtual void OnSplitterMoving(SplitterEventArgs sevent)
        {
            SplitterEventHandler handler = (SplitterEventHandler)base.Events[EVENT_MOVING];
            if (handler != null)
            {
                handler(this, sevent);
            }
            if (this.splitTarget != null)
            {
                this.SplitMove(sevent.SplitX, sevent.SplitY);
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (this.Horizontal)
            {
                if (width < 1)
                {
                    width = 3;
                }
                this.splitterThickness = width;
            }
            else
            {
                if (height < 1)
                {
                    height = 3;
                }
                this.splitterThickness = height;
            }
            base.SetBoundsCore(x, y, width, height, specified);
        }

        private void SplitBegin(int x, int y)
        {
            SplitData data = this.CalcSplitBounds();
            if ((data.target != null) && (this.minSize < this.maxSize))
            {
                this.anchor = new Point(x, y);
                this.splitTarget = data.target;
                this.splitSize = this.GetSplitSize(x, y);
                try
                {
                    if (this.splitterMessageFilter != null)
                    {
                        this.splitterMessageFilter = new SplitterMessageFilter(this);
                    }
                    Application.AddMessageFilter(this.splitterMessageFilter);
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }

                base.Capture = true; //Internal
                this.DrawSplitBar(1);
            }
        }

        private void SplitEnd(bool accept)
        {
            this.DrawSplitBar(3);
            this.splitTarget = null;
            base.Capture = false; //Internal

            if (this.splitterMessageFilter != null)
            {
                Application.RemoveMessageFilter(this.splitterMessageFilter);
                this.splitterMessageFilter = null;
            }
            if (accept)
            {
                this.ApplySplitPosition();
            }
            else if (this.splitSize != this.initTargetSize)
            {
                this.SplitPosition = this.initTargetSize;
            }
            this.anchor = Point.Empty;
        }

        private void SplitMove(int x, int y)
        {
            int splitSize = this.GetSplitSize((x - base.Left) + this.anchor.X, (y - base.Top) + this.anchor.Y);
            if (this.splitSize != splitSize)
            {
                this.splitSize = splitSize;
                this.DrawSplitBar(2);
            }
        }

        public override string ToString()
        {
            string str = base.ToString();
            return (str + ", MinExtra: " + this.MinExtra.ToString(CultureInfo.CurrentCulture) + ", MinSize: " + this.MinSize.ToString(CultureInfo.CurrentCulture));
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AllowDrop
        {
            get
            {
                return base.AllowDrop;
            }
            set
            {
                base.AllowDrop = value;
            }
        }

        [DefaultValue(0), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override AnchorStyles Anchor
        {
            get
            {
                return AnchorStyles.None;
            }
            set
            {
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        [DefaultValue(0), DispId(-504)]
        public BorderStyle BorderStyle
        {
            get
            {
                return this.borderStyle;
            }
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, 0, 2))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(BorderStyle));
                }
                if (this.borderStyle != value)
                {
                    this.borderStyle = value;
                    base.UpdateStyles();
                }
            }
        }

        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle &= -513;
                createParams.Style &= -8388609;
                switch (this.borderStyle)
                {
                    case BorderStyle.FixedSingle:
                        createParams.Style |= 0x800000;
                        return createParams;

                    case BorderStyle.Fixed3D:
                        createParams.ExStyle |= 0x200;
                        return createParams;
                }
                return createParams;
            }
        }

        protected override Cursor DefaultCursor
        {
            get
            {
                switch (this.Dock)
                {
                    case DockStyle.Top:
                    case DockStyle.Bottom:
                        return Cursors.HSplit;

                    case DockStyle.Left:
                    case DockStyle.Right:
                        return Cursors.VSplit;
                }
                return base.DefaultCursor;
            }
        }

        protected override ImeMode DefaultImeMode
        {
            get
            {
                return ImeMode.Disable;
            }
        }

        [Localizable(true), DefaultValue(DockStyle.Left)]
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                if (((value != DockStyle.Top) && (value != DockStyle.Bottom)) && ((value != DockStyle.Left) && (value != DockStyle.Right)))
                {
                    throw new ArgumentException(SR.GetString("SplitterInvalidDockEnum"));
                }

                int splitterThickness = this.splitterThickness;
                this.splitterElement.Dock = base.Dock = value;

                switch (this.Dock)
                {
                    case DockStyle.Top:
                    case DockStyle.Bottom:
                        if (this.splitterThickness == -1)
                        {
                            break;
                        }
                        base.Height = splitterThickness;
                        return;

                    case DockStyle.Left:
                    case DockStyle.Right:
                        if (this.splitterThickness != -1)
                        {
                            base.Width = splitterThickness;
                        }
                        break;

                    default:
                        return;
                }

                RestoreArrows();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }

        private bool Horizontal
        {
            get
            {
                DockStyle dock = this.Dock;
                if (dock != DockStyle.Left)
                {
                    return dock == DockStyle.Right;
                }
                return true;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public new ImeMode ImeMode
        {
            get
            {
                return base.ImeMode;
            }
            set
            {
                base.ImeMode = value;
            }
        }

        [Localizable(true), DefaultValue(0)]
        public int MinExtra
        {
            get
            {
                return this.minExtra;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                this.minExtra = value;
            }
        }

        [Localizable(true), DefaultValue(0)]
        public int MinSize
        {
            get
            {
                return this.minSize;
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                this.minSize = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SplitPosition
        {
            get
            {
                if (this.splitSize == -1)
                {
                    this.splitSize = this.CalcSplitSize();
                }
                return this.splitSize;
            }
            set
            {
                SplitData data = this.CalcSplitBounds();
                if (value > this.maxSize)
                {
                    value = this.maxSize;
                }
                if (value < this.minSize)
                {
                    value = this.minSize;
                }
                this.splitSize = value;
                this.DrawSplitBar(3);
                if (data.target == null)
                {
                    this.splitSize = -1;
                }
                else
                {
                    Rectangle bounds = data.target.Bounds;
                    switch (this.Dock)
                    {
                        case DockStyle.Top:
                            bounds.Height = value;
                            break;

                        case DockStyle.Bottom:
                            bounds.Y += bounds.Height - this.splitSize;
                            bounds.Height = value;
                            break;

                        case DockStyle.Left:
                            bounds.Width = value;
                            break;

                        case DockStyle.Right:
                            bounds.X += bounds.Width - this.splitSize;
                            bounds.Width = value;
                            break;
                    }

                    data.target.Bounds = bounds;
                    Application.DoEvents();
                    this.OnSplitterMoved(new SplitterEventArgs(base.Left, base.Top, base.Left + (bounds.Width / 2), base.Top + (bounds.Height / 2)));
                }
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool TabStop
        {
            get
            {
                return base.TabStop;
            }
            set
            {
                base.TabStop = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Bindable(false)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public new event EventHandler BackgroundImageChanged
        {
            add
            {
                base.BackgroundImageChanged += value;
            }
            remove
            {
                base.BackgroundImageChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public new event EventHandler BackgroundImageLayoutChanged
        {
            add
            {
                base.BackgroundImageLayoutChanged += value;
            }
            remove
            {
                base.BackgroundImageLayoutChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler Enter
        {
            add
            {
                base.Enter += value;
            }
            remove
            {
                base.Enter -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler FontChanged
        {
            add
            {
                base.FontChanged += value;
            }
            remove
            {
                base.FontChanged -= value;
            }
        }


        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ForeColorChanged
        {
            add
            {
                base.ForeColorChanged += value;
            }
            remove
            {
                base.ForeColorChanged -= value;
            }
        }



        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ImeModeChanged
        {
            add
            {
                base.ImeModeChanged += value;
            }
            remove
            {
                base.ImeModeChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyEventHandler KeyDown
        {
            add
            {
                base.KeyDown += value;
            }
            remove
            {
                base.KeyDown -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public new event KeyPressEventHandler KeyPress
        {
            add
            {
                base.KeyPress += value;
            }
            remove
            {
                base.KeyPress -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public new event KeyEventHandler KeyUp
        {
            add
            {
                base.KeyUp += value;
            }
            remove
            {
                base.KeyUp -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler Leave
        {
            add
            {
                base.Leave += value;
            }
            remove
            {
                base.Leave -= value;
            }
        }

        public event SplitterEventHandler SplitterMoved
        {
            add
            {
                base.Events.AddHandler(EVENT_MOVED, value);
            }
            remove
            {
                base.Events.RemoveHandler(EVENT_MOVED, value);
            }
        }


        public event SplitterEventHandler SplitterMoving
        {
            add
            {
                base.Events.AddHandler(EVENT_MOVING, value);
            }
            remove
            {
                base.Events.RemoveHandler(EVENT_MOVING, value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public new event EventHandler TabStopChanged
        {
            add
            {
                base.TabStopChanged += value;
            }
            remove
            {
                base.TabStopChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler TextChanged
        {
            add
            {
                base.TextChanged += value;
            }
            remove
            {
                base.TextChanged -= value;
            }
        }
    }

    class SplitterLayout : LayoutPanel
    {
        private RadSplitter owner;

        public SplitterLayout(RadSplitter owner)
        {
            this.owner = owner;
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.Children.Count == 0)
            {
                return base.MeasureOverride(availableSize);
            }

            for (int i = 0; i < this.Children.Count; i++)
            {
                int splitterThumbHeight = this.owner.ThumbLength;
                if (this.Children[i].MinSize.Height != 0)
                {
                    splitterThumbHeight = this.Children[i].MinSize.Height;
                }

                SizeF buttonSize = (this.owner.Dock == DockStyle.Left || this.owner.Dock == DockStyle.Right) ? new SizeF(availableSize.Width, splitterThumbHeight) :
                    new SizeF(splitterThumbHeight, availableSize.Height);

                this.Children[i].Measure(buttonSize);
            }

            return availableSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            if (this.Children.Count == 0)
            {
                return base.ArrangeOverride(finalSize);
            }

            int len = 0;
            for (int i = 0; i < this.Children.Count; i++)
            {
                if (Children[i].Visibility == ElementVisibility.Visible)
                {
                    len += this.owner.ThumbLength;
                }
            }

            float start = (this.owner.Dock == DockStyle.Left || this.owner.Dock == DockStyle.Right) ? (finalSize.Height - len) / 2 : (finalSize.Width - len) / 2;
            for (int i = 0; i < this.Children.Count; i++)
            {
                if (this.Children[i].Visibility != ElementVisibility.Visible) continue;

                if (this.owner.Dock == DockStyle.Left || this.owner.Dock == DockStyle.Right)
                {
                    this.Children[i].Arrange(new RectangleF(0, start, finalSize.Width, this.owner.ThumbLength));
                }
                else
                {
                    this.Children[i].Arrange(new RectangleF(start, 0, this.owner.ThumbLength, finalSize.Height));
                }

                start += this.owner.ThumbLength;
            }

            return finalSize;
        }
    }
}


