using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.UI.Design;
using Telerik.WinControls.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using Telerik.WinControls.UI.Docking;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{

    [TelerikToolboxCategory(ToolboxGroupStrings.ContainersGroup)]
    [ToolboxItem(true)]
    [Designer("Telerik.WinControls.UI.Design.RadSplitContainerDesigner, Telerik.WinControls.UI.Design, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e")]
    [Docking(System.Windows.Forms.DockingBehavior.Ask)]
    //[ToolboxBitmap(typeof(RadDock), "RadSplitContainer.bmp")]
    [RadToolboxItem(false)]
    public class RadSplitContainer : SplitPanel
    {
        internal const int DEFAULT_SPLITTER_SLOTS = 4;
        private bool customThemeClassName;
       
        private SplitterElement currentSplitter;
        private int lastSplitterDistance = -1;
        internal int splitterDistance = -1;
        private int beginSplitterDistance;
        private bool resizing = false;
        private bool isCleanUpTarget;

        private static readonly object SplitterMovingEventKey;
        private static readonly object SplitterMovedEventKey;
        private static readonly object ChildPanelCollapsedChangedEventKey;
        private Orientation orientation;
        private SplitPanelCollection panels;
        private SplitContainerLayoutStrategy layoutStrategy;
        internal SplitContainerElement splitContainerElement;
        private SplitterCollection splitters;

        static RadSplitContainer()
        {
            SplitterMovingEventKey = new object();
            SplitterMovedEventKey = new object();
            ChildPanelCollapsedChangedEventKey = new object();

            //ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.ControlDefault.SplitContainer.xml");
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadSplitContainer().DeserializeTheme();
        }

        public RadSplitContainer()
        {
            this.panels = new SplitPanelCollection(this);
            this.layoutStrategy = new SplitContainerLayoutStrategy();
        }

		public RadSplitContainer(Orientation orientation)
			: this()
		{
			this.orientation = orientation;
		}

        protected override void Construct()
        {
            this.orientation = Orientation.Vertical;
            base.Construct();
        }

        protected override void OnLoad(Size desiredSize)
        {
            base.OnLoad(desiredSize);

            this.ApplySplitterWidth(this.SplitterWidth);
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);

            this.splitContainerElement = new SplitContainerElement();
            this.splitContainerElement.ShouldPaint = false;
            this.splitContainerElement.SetValue(SplitContainerElement.IsVerticalProperty, true);
            parent.Children.Add(splitContainerElement);
        }

        public new class ControlCollection : Control.ControlCollection
        {
            private RadSplitContainer owner;

            public ControlCollection(RadSplitContainer owner)
                : base(owner)
            {
                this.owner = owner;

            }

            public override void Add(Control value)
            {
                if (!(value is SplitPanel))
                {
                    throw new ArgumentException("RadSplitContainer.Controls can contain only controls of type SplitPanel");
                }

                SplitPanel splitPanel = (SplitPanel)value;
                //reset default layout styles which may alter our custom layout logic
                splitPanel.Dock = DockStyle.None;
                splitPanel.Anchor = AnchorStyles.Left | AnchorStyles.Top;

                base.Add(splitPanel);

                ISite site = this.owner.Site;
                if ((site != null) && (splitPanel.Site == null))
                {
                    IContainer container = site.Container;
                    if (container != null)
                    {
                        container.Add(splitPanel);
                    }
                }

                splitPanel.ThemeName = this.owner.ThemeName;
            }
        }

        protected override Control.ControlCollection CreateControlsInstance()
        {
            return new ControlCollection(this);
        }
        
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            ClearSplitterReferences(e.Control);
        }

        private void ClearSplitterReferences(Control control)
        {
            //int count = this.splitters.Length;
            for (int i = 0; i < this.splitContainerElement.Count; i++)
            {
                SplitterElement splitter = this.splitContainerElement[i];
                if (splitter == null)
                {
                    continue;
                }

                if (splitter.LeftNode == control)
                {
                    splitter.LeftNode = null;
                }
                if (splitter.RightNode == control)
                {
                    splitter.RightNode = null;
                }
            }
        }

        //TODO: Consider whether this collection should be visible at design-time
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SplitPanelCollection SplitPanels
        {
            get
            {
                return this.panels;
            }
        }

        /// <summary>
        /// Determines whether the container is a target of automatic defragment operation.
        /// This property is internally used by the framework and is not intended to be directly used in code.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsCleanUpTarget
        {
            get
            {
                return this.isCleanUpTarget;
            }
            set
            {
                this.isCleanUpTarget = value;
            }
        }

        public event SplitPanelEventHandler PanelCollapsedChanged
        {
            add
            {
                this.Events.AddHandler(ChildPanelCollapsedChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ChildPanelCollapsedChangedEventKey, value);
            }
        }

        protected internal virtual void OnChildPanelCollapsedChanged(SplitPanel child)
        {
            //this.Collapsed = !HasNonCollapsedChild;
            SplitPanelEventHandler eh = this.Events[ChildPanelCollapsedChangedEventKey] as SplitPanelEventHandler;
            if (eh != null)
            {
                SplitPanelEventArgs args = new SplitPanelEventArgs(child);
                eh(this, args);
            }
        }

        protected override bool CanEditElementAtDesignTime(RadElement element)
        {
            //splitters are dynamically added, do not allow to be modified at design-time
            if (element is SplitterElement)
            {
                return false;
            }

            return base.CanEditElementAtDesignTime(element);
        }

        [Browsable(false)]
        public bool HasNonCollapsedChild
        {
            get
            {
                int count = this.panels.Count;
                bool hasNonCollapsed = false;

                for (int i = 0; i < count; i++)
                {
                    if (!this.panels[i].Collapsed)
                    {
                        hasNonCollapsed = true;
                        break;
                    }
                }

                return hasNonCollapsed;
            }
        }

        [Browsable(false)]
        public virtual Rectangle ContentRectangle
        {
            get
            {
                return this.ClientRectangle;
            }
        }

        [Browsable(false)]
        public RadSplitContainer RootContainer
        {
            get
            {
                Control parent = this.Parent;
                while (parent != null)
                {
                    if (!(parent.Parent is RadSplitContainer))
                    {
                        break;
                    }

                    parent = parent.Parent;
                }

                if (parent is RadSplitContainer)
                {
                    return (RadSplitContainer)parent;
                }

                return this;
            }
        }

        [Browsable(false)]
        public bool HasVisibleSplitPanels
        {
            get
            {
                for (int i = 0; i < this.SplitPanels.Count; i++)
                {
                    if (!this.SplitPanels[i].Collapsed)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the horizontal or vertical orientation of
        /// the Telerik.WinControls.UI.RadSplitContainer panels.
        /// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(Orientation.Vertical), Localizable(true)]
        public Orientation Orientation
        {
            get
            {
                return this.orientation;
            }
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, 0, 1))
                {
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(Orientation));
                }

                if (this.orientation != value)
                {
                    this.orientation = value;
                    this.OnOrientationChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnOrientationChanged(EventArgs e)
        {
            this.splitContainerElement.SetValue(SplitContainerElement.IsVerticalProperty, this.orientation == Orientation.Vertical);
            this.PerformLayout();
        }

        /// <summary>
        /// Gets or sets the width of a single splitter on the container.
        /// Specify zero to prevent displaying any splitters at all.
        /// </summary>
        [RadPropertyDefaultValue("SplitterWidth", typeof(SplitContainerElement))]
        [Description("Gets or sets the length of a single splitter on the container. Specify zero to prevent displaying any splitters at all.")]
        public virtual int SplitterWidth
        {
            get
            {
                return this.splitContainerElement.SplitterWidth;
            }
            set
            {
                this.splitContainerElement.SplitterWidth = value;
            }
        }

        /// <summary>
        /// Applies the desired splitter width across all splitters and delegates the event to all descendant RadSplitContainer instances.
        /// </summary>
        /// <param name="splitterWidth"></param>
        protected internal virtual void ApplySplitterWidth(int splitterWidth)
        {
            foreach (SplitterElement splitter in this.splitContainerElement.Children)
            {
                splitter.SplitterWidth = splitterWidth;
            }

            for (int i = 0; i < this.panels.Count; i++)
            {
                RadSplitContainer container = this.panels[i] as RadSplitContainer;
                if (container != null)
                {
                    container.splitContainerElement.SetDefaultValueOverride(SplitContainerElement.SplitterWidthProperty, splitterWidth);
                }
            }

            this.PerformLayout();
        }

        /// <summary>
        /// Gets or sets the layout strategy that arranges all the visible SplitPanel children.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SplitContainerLayoutStrategy LayoutStrategy
        {
            get
            {
                return this.layoutStrategy;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("LayoutStrategy");
                }

                this.layoutStrategy = value;
                this.PerformLayout();
            }
        }

        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);

            this.LayoutInternal();
        }

        protected void LayoutInternal()
        {
            this.SuspendLayout();

            if (this.Controls.Count > 0 && this.layoutStrategy != null)
            {
                int splitterCount = this.VisiblePanelCount - 1;
                if (this.splitContainerElement.Count != splitterCount)
                {
                    this.splitContainerElement.DisposeChildren();
                    while (this.splitContainerElement.Count < splitterCount)
                    {
                        SplitterElement splitter = new SplitterElement();
                        this.splitContainerElement.Children.Add(splitter);
                        splitter.AutoSize = false;
                        splitter.Dock = (this.orientation == Orientation.Horizontal) ? DockStyle.Top : DockStyle.Left;
                        splitter.PrevNavigationButton.Visibility = ElementVisibility.Collapsed;
                        splitter.NextNavigationButton.Visibility = ElementVisibility.Collapsed;
                        splitter.SplitterWidth = this.SplitterWidth;
                    }
                }

                this.layoutStrategy.PerformLayout(this);
            }

            this.ResumeLayout(false);
        }

        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SplitterCollection Splitters
        {
            get
            {
                if (splitters == null)
                {
                    splitters = new SplitterCollection(this);
                }

                return splitters;
            }
        }

        /// <summary>
        /// Updates the splitter, associated with the specified index of a child SplitPanel.
        /// </summary>
        /// <param name="info">The layout info, containing information about the operation.</param>
        /// <param name="panelIndex">The index of the panel for which the splitter should be updated.</param>
        /// <param name="bounds">The bounding rectangle of the splitter.</param>
        protected internal virtual void UpdateSplitter(SplitContainerLayoutInfo info, int panelIndex, Rectangle bounds)
        {
            SplitterElement splitter = this.splitContainerElement[panelIndex - 1];
            splitter.LeftNode = info.LayoutTargets[panelIndex - 1];
            splitter.RightNode = info.LayoutTargets[panelIndex];
            splitter.Bounds = bounds;
        }

        /// <summary>
        /// Provides a routine which merges a container with its parent (if appropriate).
        /// The purpose of this logic is to remove internally created containers when they are not needed.
        /// </summary>
        protected internal virtual bool MergeWithParentContainer()
        {
            this.UpdateCollapsed();
            //only internally created split containers are target of automatic clean-up
            if (!this.isCleanUpTarget)
            {
                return false;
            }

            if (this.panels.Count == 0)
            {
                this.Dispose();
                return true;
            }

            RadSplitContainer parentContainer = this.Parent as RadSplitContainer;
            if (parentContainer == null)
            {
                return false;
            }

            //we have one child panel, add it to our parent
            if (this.panels.Count == 1)
            {
                //remember at which position are we and what size we have
                int index = parentContainer.SplitPanels.IndexOf(this);
                Size currSize = this.Size;

                //detach ourselves from the parent
                this.Parent = null;
                SplitPanel child = this.panels[0];

                child.Parent = null;
                child.Size = currSize;

                parentContainer.panels.Insert(index, child);

                return true;
            }

            //since we are the only child in our parent,
            //we simply remove ourselves and add our children to our parent
            if (parentContainer.SplitPanels.Count == 1)
            {
                //do not forget to get set orientation for the container we are merging with
                parentContainer.Orientation = this.Orientation;
                List<SplitPanel> children = ControlHelper.GetChildControls<SplitPanel>(this, false);
                //remove ourselves from Parent's panels
                this.Parent = null;
                this.SplitPanels.Clear();
                //add all our children to our parent
                foreach (SplitPanel child in children)
                {
                    parentContainer.SplitPanels.Add(child);
                }

                return true;
            }

            //now check for equal orientation - we do not want nested containers with equal orientation
            if (parentContainer.Orientation == this.orientation)
            {
                //get all our children and add them to the parent container
                int index = parentContainer.SplitPanels.IndexOf(this);
                List<SplitPanel> children = ControlHelper.GetChildControls<SplitPanel>(this, false);

                this.Parent = null;
                this.SplitPanels.Clear();

                foreach (SplitPanel child in children)
                {
                    parentContainer.SplitPanels.Insert(index++, child);
                }

                return true;
            }

            return false;
        }

        protected internal virtual void UpdateCollapsed()
        {
            this.Collapsed = !HasNonCollapsedChild;
        }

        private int VisiblePanelCount
        {
            get
            {
                int visibleCount = 0;
                int count = this.panels.Count;

                for (int i = 0; i < count; i++)
                {
                    if (!this.panels[i].Collapsed)
                    {
                        visibleCount++;
                    }
                }

                return visibleCount;
            }
        }

        private List<SplitPanel> VisiblePanels
        {
            get
            {
                List<SplitPanel> visiblePanels = new List<SplitPanel>(this.panels.Count);

                for (int i = 0; i < this.panels.Count; i++)
                {
                    if (!this.panels[i].Collapsed)
                    {
                        visiblePanels.Add(this.panels[i]);
                    }
                }

                return visiblePanels;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left && this.ContentRectangle.Contains(e.Location))
            {
                this.currentSplitter = GetSplitterElementAtPoint(e.Location);

                if (this.currentSplitter != null && !this.currentSplitter.Fixed)
                {
                    resizing = true;
                    if (Cursor == Cursors.VSplit)
                    {
                        beginSplitterDistance = e.X;
                        splitterDistance = e.X;

                    }
                    else if (Cursor == Cursors.HSplit)
                    {
                        beginSplitterDistance = e.Y;
                        splitterDistance = e.Y;
                    }
                }
            }
        }

        /// <summary>
        /// Gets SplitterElement which rectangle conttains the specified Point.
        /// </summary>
        /// <param name="clientPoint">Point to test, in SplitContainer client coordinates</param>
        /// <returns>SplitterElement if found, null otherwise</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual SplitterElement GetSplitterElementAtPoint(Point clientPoint)
		{
            //Splitters are not yet initialized. This mainly happens at design-time
            if (this.splitContainerElement.Children.Count != this.VisiblePanelCount - 1)
            {
                return null;
            }

			for (int i = 0; i < this.VisiblePanels.Count - 1; i++)
			{
                if (this.splitContainerElement[i].Bounds.Contains(clientPoint))
				{
					return this.splitContainerElement[i];
				}
			}

            return null;
		}

        private Padding SplitterBounds(SplitterElement splitter)
        {
            Padding bounds = new Padding();
            SplitPanel leftPanel = (SplitPanel)splitter.LeftNode;
            SplitPanel rightPanel = (SplitPanel)splitter.RightNode;

            Size leftMinSize = this.layoutStrategy == null ? leftPanel.SizeInfo.MinimumSize : this.layoutStrategy.GetMinimumSize(leftPanel);
            Size rightMinSize = this.layoutStrategy == null ? rightPanel.SizeInfo.MinimumSize : this.layoutStrategy.GetMinimumSize(rightPanel);

            if (this.Orientation == Orientation.Vertical)
            {
                bounds.Left = leftPanel.Width - leftMinSize.Width;

                int diff = rightPanel.SizeInfo.MaximumSize.Width - rightPanel.Width;
                if (rightPanel.SizeInfo.MaximumSize.Width > 0 && diff >= 0)
                {
                    bounds.Left = Math.Min(bounds.Left, diff);
                }

                bounds.Right = rightPanel.Width - rightMinSize.Width;
                diff = leftPanel.SizeInfo.MaximumSize.Width - leftPanel.Width;
                if (leftPanel.SizeInfo.MaximumSize.Width > 0 && diff >= 0)
                {
                    bounds.Right = Math.Min(bounds.Right, diff);
                }
            }
            else
            {
                bounds.Top = leftPanel.Height - leftMinSize.Height;
                int diff = rightPanel.SizeInfo.MaximumSize.Height - leftPanel.Height;
                if (rightPanel.SizeInfo.MaximumSize.Height > 0 && diff >= 0)
                {
                    bounds.Top = Math.Min(bounds.Top, diff);
                }

                bounds.Bottom = rightPanel.Height - rightMinSize.Height;
                diff = leftPanel.SizeInfo.MaximumSize.Height - leftPanel.Height;
                if (leftPanel.SizeInfo.MaximumSize.Height > 0 && diff >= 0)
                {
                    bounds.Bottom = Math.Min(bounds.Bottom, diff);
                }
            }

            return bounds;
        }
     
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!resizing)
            {
                this.UpdateCursor(e.Location);
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            Padding bounds = SplitterBounds(this.currentSplitter);
            if (this.Orientation == Orientation.Vertical)
            {
                this.splitterDistance = e.X;
                if (this.splitterDistance < beginSplitterDistance - bounds.Left)
                {
                    this.splitterDistance = beginSplitterDistance - bounds.Left;
                }
                if (this.splitterDistance > bounds.Right + beginSplitterDistance)
                {
                    this.splitterDistance = bounds.Right + beginSplitterDistance;
                }
            }
            else
            {
                this.splitterDistance = e.Y;
                if (this.splitterDistance < beginSplitterDistance - bounds.Top)
                {
                    this.splitterDistance = beginSplitterDistance - bounds.Top;
                }
                if (this.splitterDistance > bounds.Bottom + beginSplitterDistance)
                {
                    this.splitterDistance = bounds.Bottom + beginSplitterDistance;
                }
            }

            SplitterCancelEventArgs args = new SplitterCancelEventArgs(e.X, e.Y, currentSplitter.Bounds.Left, currentSplitter.Bounds.Top);
            OnSplitterMoving(args);

            if (args.Cancel)
            {
                resizing = false;
                Cursor = Cursors.Arrow;
            }

            DrawSplitBar();
        }

        private void UpdateCursor(Point mouse)
        {
            SplitterElement splitter = this.GetSplitterElementAtPoint(mouse);
            if (splitter != null)
            {
                this.Cursor = this.orientation == Orientation.Vertical ? Cursors.VSplit : Cursors.HSplit;
            }
            else
            {
                Cursor = null;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            this.UpdateCursor(e.Location);

            if (resizing)
            {
                Point pt = this.PointToClient(new Point(Control.MousePosition.X, Control.MousePosition.Y));
                OnSplitterMoved(new SplitterEventArgs(pt.X, pt.Y, currentSplitter.Bounds.X, currentSplitter.Bounds.Y));

                resizing = false;
                DrawSplitBar();

                UpdateBoundsFromSplitter();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            Cursor = null;
        }

        private void DrawSplitBar()
        {
            if (this.lastSplitterDistance != -1)    //erase the drawed splitter
            {
                TelerikPaintHelper.DrawHalftoneLine(this, this.CalcSplitRectangle(this.lastSplitterDistance));
            }
            if (resizing)   //draw current splitter
            {
                TelerikPaintHelper.DrawHalftoneLine(this, this.CalcSplitRectangle(this.splitterDistance));
                this.lastSplitterDistance = this.splitterDistance;
            }
            else
            {
                this.lastSplitterDistance = -1;
            }
        }

        private void UpdateBoundsFromSplitter()
        {
            if (splitterDistance == -1 || beginSplitterDistance == splitterDistance)
            {
                return;
            }

            if (this.layoutStrategy == null)
            {
                return;
            }

            Debug.Assert(this.currentSplitter != null, "Could not apply splitter correction with no current splitter.");

            SuspendLayout();

            int dragLength = beginSplitterDistance - splitterDistance;
            this.layoutStrategy.ApplySplitterCorrection((SplitPanel)this.currentSplitter.LeftNode, (SplitPanel)this.currentSplitter.RightNode, dragLength);

            ResumeLayout(false);
            this.LayoutInternal();

            if (!this.DesignMode)
            {
                return;
            }

            //notify designer that we have changed
            IComponentChangeService service = this.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            if (service != null)
            {
                service.OnComponentChanged(this, null, null, null);
            }
        }

        private Rectangle CalcSplitRectangle(int splitterDistance)
        {
            if (this.Orientation == Orientation.Vertical)
            {
                return new Rectangle(splitterDistance, currentSplitter.Bounds.Top, this.SplitterWidth, currentSplitter.Bounds.Height);
            }
            else
            {
                return new Rectangle(currentSplitter.Bounds.Left, splitterDistance, currentSplitter.Bounds.Width, this.SplitterWidth);
            }
        }

        /// <summary>
        ///		Occurs when any of the splitters is moving.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Occurs when any of the splitters is moving.")]
        public event SplitterCancelEventHandler SplitterMoving
        {
            add
            {
                this.Events.AddHandler(RadSplitContainer.SplitterMovingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadSplitContainer.SplitterMovingEventKey, value);
            }
        }

        /// <summary>
        ///		Occurs when any of the splitters is moved.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Occurs when any of the splitters is moved.")]
        public event SplitterEventHandler SplitterMoved
        {
            add
            {
                this.Events.AddHandler(RadSplitContainer.SplitterMovedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadSplitContainer.SplitterMovedEventKey, value);
            }
        }

        protected virtual void OnSplitterMoved(SplitterEventArgs e)
        {
            SplitterEventHandler handler1 = (SplitterEventHandler)base.Events[RadSplitContainer.SplitterMovedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }

            if (this.Parent is RadSplitContainer)
            {
                ((RadSplitContainer)this.Parent).OnSplitterMoved(e);
            }
        }

        /// <summary>
        /// Determines whether the container can be selected at design-time.
        /// </summary>
        /// <returns></returns>
        protected internal virtual bool CanSelectAtDesignTime()
        {
            return true;
        }

        protected virtual void OnSplitterMoving(SplitterCancelEventArgs e)
        {
            SplitterCancelEventHandler handler1 = (SplitterCancelEventHandler)base.Events[RadSplitContainer.SplitterMovingEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }

            if (this.Parent is RadSplitContainer)
            {
                ((RadSplitContainer)this.Parent).OnSplitterMoving(e);
            }
        }

        protected override void OnThemeChanged()
        {
            foreach (SplitPanel panel in panels)
            {
                panel.ThemeName = this.ThemeName;
            }

            base.OnThemeChanged();
        }

        [Browsable(false)]
        public override string ThemeClassName
        {
            get
            {
                if (customThemeClassName)
                {
                    return base.ThemeClassName;
                }

                return typeof(RadSplitContainer).FullName;
            }
            set
            {
                customThemeClassName = true;
                base.ThemeClassName = value;
            }
        }
    }
}
