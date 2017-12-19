using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Telerik.WinControls.Paint;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// The RadScrollablePanel control can be used as a container for different UI elements.
    /// This control is powered by the Telerik Presentation Framework and supports
    /// gradient backgrounds, shapes and theming. This control supports also theming
    /// of the scrollbars.
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.ContainersGroup)]
    [ToolboxItem(true)]
    [Description("This control extends the behavior of RadPanel by enabling the theming of the scrollbars when in AutoScroll mode.")]
    [Docking(DockingBehavior.Ask)]
    [Designer(DesignerConsts.RadScrollablePanelDesignerString)]
    [RadThemeDesignerData(typeof(RadScrollablePanelThemeDesignerData))]
    public class RadScrollablePanel : RadControl
    {
        
        #region Fields

        private RadScrollablePanelElement panelElement;

        internal RadScrollablePanelContainer container;

        private RadVScrollBar verticalScrollbar;
        private RadHScrollBar horizontalScrollbar;

        #endregion

        #region Ctor

        public RadScrollablePanel()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.InitializeInternalControls();
            this.container.Layout += new LayoutEventHandler(OnContainer_Layout);
        }

        #endregion

        #region Properties

        /// <summary>Gets the default size of the control.</summary>
        /// <value>The default System.Drawing.Size of the control.</value>
        /// <remarks>The default Size of the control.</remarks>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(200, 100);
            }
        }

        /// <summary>
        /// Gets the current client area margin
        /// of the control.
        /// </summary>
        internal Padding NCMargin
        {
            get
            {
                return this.GetContentMargin();
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="Telerik.WinControls.UI.RadScrollablePanelElement"/>
        /// class which represents the main element of the control.
        /// </summary>
        [Browsable(false)]
        public RadScrollablePanelElement PanelElement
        {
            get
            {
                return this.panelElement;
            }
        }

        /// <summary>
        /// Gets the container panel that holds
        /// all the components added to the panel.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Panel PanelContainer
        {

            get
            {
                return this.container;
            }
        }

        /// <summary>
        /// Gets the vertical scrollbar of the control.
        /// </summary>
        [Browsable(false)]
        public RadVScrollBar VerticalScrollbar
        {
            get
            {
                return this.verticalScrollbar;
            }
        }

        /// <summary>
        /// Gets the horizontal scrollbar of the control.
        /// </summary>
        [Browsable(false)]
        public RadHScrollBar HorizontalScrollbar
        {
            get
            {
                return this.horizontalScrollbar;
            }
        }

        public override bool AutoScroll
        {
            get
            {
                return this.container.AutoScroll;
            }
            set
            {
                this.container.AutoScroll = value;
                this.OnNotifyPropertyChanged("AutoScroll");
            }
        }

        public override Point AutoScrollOffset
        {
            get
            {
                return this.container.AutoScrollOffset;
            }
            set
            {
                this.container.AutoScrollOffset = value;
            }
        }

        public new Point AutoScrollPosition
        {
            get
            {
                return this.container.AutoScrollPosition;
            }
            set
            {
                this.container.AutoScrollPosition = value;
            }
        }

        public new Size AutoScrollMargin
        {
            get
            {
                return this.container.AutoScrollMargin;
            }
            set
            {
                this.container.AutoScrollMargin = value;
            }
        }

        public new Size AutoScrollMinSize
        {
            get
            {
                return this.container.AutoScrollMinSize;
            }
            set
            {
                this.container.AutoScrollMinSize = value;
            }
        }


        #endregion

        #region Overrides

        protected override void OnThemeChanged()
        {
            base.OnThemeChanged();
            if (this.verticalScrollbar != null && this.horizontalScrollbar != null)
            {
                this.verticalScrollbar.ThemeName = this.ThemeName;
                this.horizontalScrollbar.ThemeName = this.ThemeName;
            }
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);

            if (this.RootElement.RightToLeft)
            {
                this.verticalScrollbar.Dock = DockStyle.Left;
            }
            else
            {
                this.verticalScrollbar.Dock = DockStyle.Right;
            }
        }

        protected override void OnNotifyPropertyChanged(string propertyName)
        {
            base.OnNotifyPropertyChanged(propertyName);

            if (propertyName == "AutoScroll")
            {
                this.OnAutoScrollChanged();
            }
        }
       
        protected override Control.ControlCollection CreateControlsInstance()
        {
            return new RadScrollablePanelControlCollection(this);
        }

        protected override RootRadElement CreateRootElement()
        {
            return new PanelRootElement();
        }

        #endregion

        #region Methods

        protected virtual Color GetSizingGripColor()
        {
            FillPrimitive scrollbarFill = this.horizontalScrollbar.ScrollBarElement.Children[0] as FillPrimitive;

            switch(scrollbarFill.GradientStyle)
            {
                case GradientStyles.Linear:
                    {
                        return this.GetFillPrimitiveAverageColor(scrollbarFill);
                    }
                case GradientStyles.Solid:
                default:
                    {
                        return scrollbarFill.BackColor;
                    }
            }
        }

        private Color GetFillPrimitiveAverageColor(FillPrimitive fill)
        {
            Color result = fill.BackColor;

            if (fill.NumberOfColors == 2)
            {
                result = Color.FromArgb(
                    (byte)(result.A + fill.BackColor2.A) / 2,
                    (byte)(result.R + fill.BackColor2.R) / 2,
                    (byte)(result.G + fill.BackColor2.G) / 2,
                     (byte)(result.B + fill.BackColor2.B) / 2);
            }
            else if (fill.NumberOfColors == 3)
            {
                result = Color.FromArgb(
                    (byte)(result.A + fill.BackColor2.A + fill.BackColor3.A) / 3,
                    (byte)(result.R + fill.BackColor2.R + fill.BackColor3.R) / 3,
                    (byte)(result.G + fill.BackColor2.G + fill.BackColor3.G) / 3,
                     (byte)(result.B + fill.BackColor2.B + fill.BackColor3.B) / 3);
            }
            else if (fill.NumberOfColors == 4)
            {
                result = Color.FromArgb(
                   (byte)(result.A + fill.BackColor2.A + fill.BackColor3.A + fill.BackColor3.A) / 4,
                   (byte)(result.R + fill.BackColor2.R + fill.BackColor3.R + fill.BackColor3.R) / 4,
                   (byte)(result.G + fill.BackColor2.G + fill.BackColor3.G + fill.BackColor3.G) / 4,
                   (byte)(result.B + fill.BackColor2.B + fill.BackColor3.B + fill.BackColor3.B) / 4);
            }

            return result;
        }

        /// <summary>
        /// This method inserts the scrollbars and the container
        /// in the Controls collection of this control.
        /// </summary>
        protected virtual void InsertInternalControls()
        {
            this.SuspendLayout();
            (this.Controls as RadScrollablePanelControlCollection).AddControlInternal(this.container);
            (this.Controls as RadScrollablePanelControlCollection).AddControlInternal(this.verticalScrollbar);
            (this.Controls as RadScrollablePanelControlCollection).AddControlInternal(this.horizontalScrollbar);
            this.ResumeLayout(true);
        }

        /// <summary>
        /// Calculates the non-client margin of the control
        /// based on the radius of the round rect shape.
        /// </summary>
        /// <returns>An instance of the <see cref="System.Windows.Forms.Padding"/> struct
        /// which represents the left, top, right and bottom margin.</returns>
        protected virtual Padding GetContentMargin()
        {
            if (this.panelElement.Border.Visibility == ElementVisibility.Visible)
            {
                Padding result = this.panelElement.BorderThickness;

                if (this.RootElement.Shape != null && this.RootElement.ApplyShapeToControl
                    && this.RootElement.Shape is RoundRectShape)
                {
                    RoundRectShape rRectShape = this.RootElement.Shape as RoundRectShape;
                    int ncOffset = (int)(rRectShape.Radius - Math.Sqrt(rRectShape.Radius * rRectShape.Radius / 2));
                    result = Padding.Add(new Padding(ncOffset), result);
                }

                return result;
            }

            return Padding.Empty;
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);

            this.panelElement = new RadScrollablePanelElement();
            parent.Children.Add(this.panelElement);

            this.panelElement.Border.PropertyChanged += new PropertyChangedEventHandler(OnPanelElementBorder_PropertyChanged);
        }

        //If the AutoScroll functionality is disabled, apply padding to the control so that 
        //the container control is shrinked accordingly in order for the border to display correctly.
        private void OnPanelElementBorder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == BorderPrimitive.VisibilityProperty.Name)
            {
                if (this.panelElement.Border.Visibility != ElementVisibility.Visible)
                {
                    this.Padding = Padding.Empty;
                    return;
                }
            }

            if (e.PropertyName == BorderPrimitive.WidthProperty.Name)
            {
                if (this.panelElement.Border.BoxStyle != BorderBoxStyle.FourBorders)
                {
                    this.Padding = new Padding((int)this.panelElement.Border.Width);
                }
            }

            if (e.PropertyName == BorderPrimitive.LeftWidthProperty.Name
                || e.PropertyName == BorderPrimitive.TopWidthProperty.Name
                || e.PropertyName == BorderPrimitive.RightWidthProperty.Name
                || e.PropertyName == BorderPrimitive.BottomWidthProperty.Name)
            {
                if (this.panelElement.Border.BoxStyle == BorderBoxStyle.FourBorders)
                {
                    this.Padding = new Padding(
                        (int)this.panelElement.Border.LeftWidth,
                        (int)this.panelElement.Border.TopWidth,
                        (int)this.panelElement.Border.RightWidth,
                        (int)this.panelElement.Border.BottomWidth);
                }
            }
        }

        /// <summary>
        /// This method initializes the scrollbars and the
        /// container control.
        /// </summary>
        protected virtual void InitializeInternalControls()
        {
            this.container = new RadScrollablePanelContainer(this);
            this.container.ScrollBarSynchronizationNeeded += new ScrollbarSynchronizationNeededEventHandler(OnContainer_ScrolledToControl);
            this.container.Dock = System.Windows.Forms.DockStyle.Fill;

            this.verticalScrollbar = new RadVScrollBar();

            this.verticalScrollbar.Dock = System.Windows.Forms.DockStyle.Right;
            this.verticalScrollbar.Scroll += new ScrollEventHandler(OnVerticalScrollbar_Scroll);


            this.horizontalScrollbar = new RadHScrollBar();
            this.horizontalScrollbar.Dock = DockStyle.Bottom;
            this.horizontalScrollbar.Scroll += new ScrollEventHandler(OnHorizontalScrollbar_Scroll);

            this.container.MouseWheel += new MouseEventHandler(OnInternalContainer_MouseWheel);

            this.InsertInternalControls();
            this.horizontalScrollbar.Visible = false;
            this.verticalScrollbar.Visible = false;
        }

        
        private void OnInternalContainer_MouseWheel(object sender, MouseEventArgs e)
        {
            this.SynchronizeScrollbars(this.container.AutoScrollPosition);

            if (this.verticalScrollbar.Visible)
            {
                this.container.Invalidate();
            }
        }

        private bool IsValueInRange(RadScrollBar scrollBar, int value)
        {
            if (value <= scrollBar.Maximum && value >= scrollBar.Minimum)
            {
                return true;
            }

            return false;
        }

        protected virtual void SynchronizeVScrollbar(int value)
        {
            if (!this.IsValueInRange(this.verticalScrollbar, value))
            {
                return;
            }

            this.verticalScrollbar.Minimum = this.container.VerticalScroll.Minimum;
            this.verticalScrollbar.Maximum = this.container.VerticalScroll.Maximum;
            this.verticalScrollbar.LargeChange = this.container.VerticalScroll.LargeChange;
            this.verticalScrollbar.SmallChange = this.container.VerticalScroll.SmallChange;
            this.verticalScrollbar.Value = value;
        }

        protected virtual void SynchronizeHScrollbar(int value)
        {
            if (!this.IsValueInRange(this.horizontalScrollbar, value))
            {
                return;
            }

            this.horizontalScrollbar.Minimum = this.container.HorizontalScroll.Minimum;
            this.horizontalScrollbar.Maximum = this.container.HorizontalScroll.Maximum;
            this.horizontalScrollbar.LargeChange = this.container.HorizontalScroll.LargeChange;
            this.horizontalScrollbar.SmallChange = this.container.HorizontalScroll.SmallChange;
            this.horizontalScrollbar.Value = value;
        }


        protected virtual void AdjustSizingGrip()
        {
            if (this.container.HorizontalScroll.Visible 
                && this.container.VerticalScroll.Visible)
            {
                if (!this.elementTree.RootElement.RightToLeft)
                {
                    this.horizontalScrollbar.ScrollBarElement.Margin = new Padding(0, 0, this.verticalScrollbar.ScrollBarElement.ControlBoundingRectangle.Width, 0);
                }
                else
                {
                    this.horizontalScrollbar.ScrollBarElement.Margin = new Padding(this.verticalScrollbar.ScrollBarElement.ControlBoundingRectangle.Width, 0, 0, 0);
                }
            }
            else
            {
                this.horizontalScrollbar.ScrollBarElement.Margin = Padding.Empty;
            }
        }

        protected virtual void SynchronizeScrollbars(Point scrollPoint)
        {
            if (!this.AutoScroll)
            {
                if (this.verticalScrollbar.Visible != this.container.VerticalScroll.Visible)
                {
                    this.verticalScrollbar.Visible = this.container.VerticalScroll.Visible;
                }

                if (this.horizontalScrollbar.Visible != this.container.HorizontalScroll.Visible)
                {
                    this.horizontalScrollbar.Visible = this.container.HorizontalScroll.Visible;
                }

                return;
            }

            this.verticalScrollbar.Visible = this.container.VerticalScroll.Visible;
            this.horizontalScrollbar.Visible = this.container.HorizontalScroll.Visible;

            if (this.container.VerticalScroll.Visible)
            {
                this.SynchronizeVScrollbar(-scrollPoint.Y);
            }

            if (this.container.HorizontalScroll.Visible)
            {
                this.SynchronizeHScrollbar(-scrollPoint.X);
            }

            this.AdjustSizingGrip();
        }

        private void AdjustPaddingForAutoScrollSetting()
        {
            this.Padding = this.GetContentMargin();
        }

        #endregion

        #region Event handling

        private void OnContainer_ScrolledToControl(object sender, ScrollbarSynchronizationNeededEventArgs args)
        {
            this.SynchronizeScrollbars(args.ScrolledLocation);
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            this.AdjustPaddingForAutoScrollSetting();
        }

        protected virtual void OnAutoScrollChanged()
        {
            this.AdjustPaddingForAutoScrollSetting();
            this.SynchronizeScrollbars(this.container.AutoScrollPosition);
            this.Refresh();
            this.LayoutEngine.Layout(this, new LayoutEventArgs(this, "Bounds"));
        }

        private void OnContainer_Layout(object sender, LayoutEventArgs e)
        {
            this.SynchronizeScrollbars(this.container.AutoScrollPosition);
        }

        private void OnVerticalScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            this.container.VerticalScroll.Value = e.NewValue;
            this.container.Invalidate();
        }

        private void OnHorizontalScrollbar_Scroll(object sender, ScrollEventArgs e)
        {
            if (this.RightToLeft != RightToLeft.Yes)
            {
                this.container.HorizontalScroll.Value = e.NewValue;
            }
            else
            {
                this.container.HorizontalScroll.Value = this.container.HorizontalScroll.Maximum - e.NewValue;
            }
            this.container.Invalidate();

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.SynchronizeScrollbars(this.container.AutoScrollPosition);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.container.ScrollBarSynchronizationNeeded -= new ScrollbarSynchronizationNeededEventHandler(OnContainer_ScrolledToControl);
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
