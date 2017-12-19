using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Primitives;
using System.ComponentModel;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// 	<para>
    ///         The only implementation of <see cref="IScrollViewer"/> and base class of
    ///         all scrollable elements.
    ///     </para>
    /// 	<para>This class contains one element called Viewport. In addition to the ordinary
    ///     property Size, Viewport has parameter called "extent size" which represents the
    ///     real size of its content. Extent size could be bigger as well as smaller than the
    ///     size of the scroll viewer.</para>
    /// 	<para>
    ///         There are two types of viewports: ordinary elements and elements that implement
    ///         <see cref="IRadScrollViewport"/>. In the first case extent size is the
    ///         size of the viewport itself. The scrolling is done on pixel basis and via
    ///         painting offset of the viewport (it is called physical scrolling). In the
    ///         second case the functions that are declared in
    ///         <see cref="IRadScrollViewport"/> are called for getting extent size and
    ///         performing the scroll operation (this is called logical scrolling).
    ///     </para>
    /// 	<para>
    ///         If the viewport implementation is of type <see cref="IRadScrollViewport"/> it
    ///         still can be physically scrolled by setting the property
    ///         <see cref="UsePhysicalScrolling"/> to true.
    ///     </para>
    /// 	<para>
    ///         Physical scrolling has one parameter that can be set -
    ///         <see cref="PixelsPerLineScroll"/> which represents the small change value
    ///         for the scrolling (i.e. the number of pixels for Line Up/Down/Left/Right). The
    ///         large change (Page Up/Down/Left/Right) is the corresponding size of the
    ///         viewable size of the viewport.
    ///     </para>
    /// 	<para>
    ///         For more information about custom viewports and logical scrolling - see
    ///         <see cref="IRadScrollViewport"/>.
    ///     </para>
    /// 	<para>
    ///         Current scroll position can be get or set via the property
    ///         <see cref="Value"/>. In addition scrolling can be performed by calling the
    ///         methods that are implemented from <see cref="IScrollViewer"/>.
    ///     </para>
    /// </summary>
    public class RadScrollViewer : RadItem, IScrollViewer
    {
        //keep consistency with BitState keys declaration
        internal const ulong RadScrollViewerLastStateKey = RadItemLastStateKey;

		private RadScrollLayoutPanel scrollPanel;
        private BorderPrimitive border;
		private FillPrimitive fillPrimitive;

        #region Events
        public event RadScrollPanelHandler Scroll;
        protected virtual void OnScroll(ScrollPanelEventArgs args)
        {
            if (Scroll != null)
            {
                Scroll(this, args);
            }
        }

        public event ScrollNeedsHandler ScrollNeedsChanged;
        protected virtual void OnScrollNeedsChanged(ScrollNeedsEventArgs args)
        {
            if (ScrollNeedsChanged != null)
            {
                ScrollNeedsChanged(this, args);
            }
        }

        public event RadPanelScrollParametersHandler ScrollParametersChanged;
        protected virtual void OnScrollParametersChanged(RadPanelScrollParametersEventArgs args)
        {
            if (ScrollParametersChanged != null)
            {
                ScrollParametersChanged(this, args);
            }
        }
        #endregion

        #region Properties

        
        public RadScrollLayoutPanel ScrollLayoutPanel
        {
            get
            {
                return this.scrollPanel;
            }
        }

		/// <commentsfrom cref="RadScrollLayoutPanel.CanHorizontalScroll" filter=""/>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool CanHorizontalScroll
		{
			get { return this.scrollPanel.CanHorizontalScroll; }
		}

		/// <commentsfrom cref="RadScrollLayoutPanel.CanVerticalScroll" filter=""/>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public bool CanVerticalScroll
		{
			get { return this.scrollPanel.CanVerticalScroll; }
		}

        /// <commentsfrom cref="RadScrollLayoutPanel.Viewport" filter=""/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public virtual RadElement Viewport
        {
            get
            {
                return this.scrollPanel.Viewport;
            }

            set
            {
                this.scrollPanel.Viewport = value;
            }
        }

        [Browsable(false)]
		public RadScrollBarElement VerticalScrollBar
		{
			get
			{
				return this.scrollPanel.VerticalScrollBar;
			}
		}

		[Browsable(false)]
		public RadScrollBarElement HorizontalScrollBar
		{
			get
			{
				return this.scrollPanel.HorizontalScrollBar;
			}
		}

        /// <commentsfrom cref="RadScrollLayoutPanel.HorizontalScrollState" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("HorizontalScrollState", typeof(RadScrollLayoutPanel))]
        [RadDefaultValue("HorizontalScrollState", typeof(RadScrollLayoutPanel))]
        public ScrollState HorizontalScrollState
        {
            get
            {
                return this.scrollPanel.HorizontalScrollState;
            }
            set
            {
                this.scrollPanel.HorizontalScrollState = value;
            }
        }

        /// <commentsfrom cref="RadScrollLayoutPanel.VerticalScrollState" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("VerticalScrollState", typeof(RadScrollLayoutPanel))]
        [RadDefaultValue("VerticalScrollState", typeof(RadScrollLayoutPanel))]
        public ScrollState VerticalScrollState
        {
            get
            {
                return this.scrollPanel.VerticalScrollState;
            }
            set
            {
                this.scrollPanel.VerticalScrollState = value;
            }
        }

        /// <commentsfrom cref="RadScrollLayoutPanel.UsePhysicalScrolling" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("UsePhysicalScrolling", typeof(RadScrollLayoutPanel))]
        [RadDefaultValue("UsePhysicalScrolling", typeof(RadScrollLayoutPanel))]
        public bool UsePhysicalScrolling
        {
            get
            {
                return this.scrollPanel.UsePhysicalScrolling;
            }
            
            set
            {
                this.scrollPanel.UsePhysicalScrolling = value;
            }
        }

        /// <commentsfrom cref="RadScrollLayoutPanel.PixelsPerLineScroll" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("PixelsPerLineScroll", typeof(RadScrollLayoutPanel))]
        [RadDefaultValue("PixelsPerLineScroll", typeof(RadScrollLayoutPanel))]
        public Point PixelsPerLineScroll
        {
            get
            {
                return this.scrollPanel.PixelsPerLineScroll;
            }
            set
            {
                this.scrollPanel.PixelsPerLineScroll = value;
            }
        }

        /// <commentsfrom cref="RadScrollLayoutPanel.MinValue" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("MinValue", typeof(RadScrollLayoutPanel))]
        [RadDefaultValue("MinValue", typeof(RadScrollLayoutPanel))]
        public Point MinValue
        {
            get
            {
                return this.scrollPanel.MinValue;
            }
        }

        /// <commentsfrom cref="RadScrollLayoutPanel.MaxValue" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("MaxValue", typeof(RadScrollLayoutPanel))]
        [RadDefaultValue("MaxValue", typeof(RadScrollLayoutPanel))]
        public Point MaxValue
        {
            get
            {
                return this.scrollPanel.MaxValue;
            }
        }

        /// <commentsfrom cref="RadScrollLayoutPanel.Value" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("Value", typeof(RadScrollLayoutPanel))]
        [RadDefaultValue("Value", typeof(RadScrollLayoutPanel))]
        public Point Value
        {
            get
            {
                return this.scrollPanel.Value;
            }
            set
            {
                this.scrollPanel.Value = value;
            }
        }

		/// <summary>Gets or sets a value indicating whether the border is shown.</summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Gets or sets a value indicating whether the border is shown.")]
		public bool ShowBorder
		{
			get
			{
				return this.border.Visibility == ElementVisibility.Visible;
			}
			set
			{
				this.border.Visibility = value ? ElementVisibility.Visible : ElementVisibility.Collapsed;
			}
		}

		/// <summary>Gets or sets a value indicating whether the fill is shown.</summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Gets or sets a value indicating whether the fill is shown.")]
		public bool ShowFill
		{
			get
			{
				return this.fillPrimitive.Visibility == ElementVisibility.Visible;
			}
			set
			{
				this.fillPrimitive.Visibility = value ? ElementVisibility.Visible : ElementVisibility.Collapsed;
			}
		}

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public RadElement FillElement
		{
			get { return this.fillPrimitive; }
		}

        /// <commentsfrom cref="RadScrollLayoutPanel.ScrollThickness" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("ScrollThickness", typeof(RadScrollLayoutPanel))]
        [RadDefaultValue("ScrollThickness", typeof(RadScrollLayoutPanel))]
		public int ScrollThickness
		{
			get
			{
				return this.scrollPanel.ScrollThickness;
			}
			set
			{
				this.scrollPanel.ScrollThickness = value;
			}
		}

        /// <commentsfrom cref="RadScrollLayoutPanel.ForceViewportWidth" filter=""/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ForceViewportWidth
        {
            get
            {
                return this.scrollPanel.ForceViewportWidth;
            }
            set
            {
                this.scrollPanel.ForceViewportWidth = value;
            }
        }

        /// <commentsfrom cref="RadScrollLayoutPanel.ForceViewportHeight" filter=""/>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ForceViewportHeight
        {
            get
            {
                return this.scrollPanel.ForceViewportHeight;
            }
            set
            {
                this.scrollPanel.ForceViewportHeight = value;
            }
        }
        #endregion

        #region Constructors

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ClipDrawing = true;
        }

        public RadScrollViewer()
        {
        }

        public RadScrollViewer(RadElement viewport)
        {
            this.Viewport = viewport;
        }

		static RadScrollViewer()
		{
			new Themes.ControlDefault.ScrollViewer().DeserializeTheme();
		}

        #endregion

        #region IScrollViewer Members
        [Browsable(true)]
        [DefaultValue(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets whether RadScrollViewer uses UI virtualization")]
        public virtual bool Virtualized
        {
            get
            {
                IVirtualViewport virtualViewport = this.Viewport as IVirtualViewport;
                if (virtualViewport != null)
                    return virtualViewport.Virtualized;
                return true;
            }
            set
            {
                throw new InvalidOperationException("There is no base implementation for setting Virtualized property of RadScrollViewer. Inherit it and override the setter.");
            }
        }

        public void LineDown()
        {
			this.scrollPanel.ScrollWith(0, this.scrollPanel.SmallVerticalChange);
        }

        public void LineLeft()
        {
			this.scrollPanel.ScrollWith(-this.scrollPanel.SmallHorizontalChange, 0);
        }

        public void LineRight()
        {
			this.scrollPanel.ScrollWith(this.scrollPanel.SmallHorizontalChange, 0);
        }

        public void LineUp()
        {
			this.scrollPanel.ScrollWith(0, -this.scrollPanel.SmallVerticalChange);
        }

        public void PageDown()
        {
			this.scrollPanel.ScrollWith(0, this.scrollPanel.LargeVerticalChange);
        }

        public void PageLeft()
        {
			this.scrollPanel.ScrollWith(0, -this.scrollPanel.LargeHorizontalChange);
        }

        public void PageRight()
        {
			this.scrollPanel.ScrollWith(0, this.scrollPanel.LargeHorizontalChange);
        }

        public void PageUp()
        {
			this.scrollPanel.ScrollWith(0, -this.scrollPanel.LargeVerticalChange);
        }

        public void ScrollToTop()
        {
            this.scrollPanel.ScrollTo(this.scrollPanel.Value.X, 0);
        }

        public void ScrollToBottom()
        {
            this.scrollPanel.ScrollTo(this.scrollPanel.Value.X, this.scrollPanel.MaxValue.Y);
        }

        public void ScrollToLeftEnd()
        {
            this.scrollPanel.ScrollTo(0, this.scrollPanel.Value.Y);
        }

        public void ScrollToRightEnd()
        {
            this.scrollPanel.ScrollTo(this.scrollPanel.MaxValue.X, this.scrollPanel.Value.Y);
        }

        public void ScrollToHome()
        {
            this.scrollPanel.ScrollTo(0, 0);
        }

        public void ScrollToEnd()
        {
            this.scrollPanel.ScrollTo(this.scrollPanel.MaxValue.X, this.scrollPanel.MaxValue.Y);
        }

        public void ScrollElementIntoView(RadElement element)
        {
            if (this.scrollPanel != null)
                this.scrollPanel.ScrollElementIntoView(element);
        }
        #endregion

        #region Overrides
        protected override void CreateChildElements()
        {
            this.scrollPanel = this.CreateScrollLayoutPanel();
            //this.scrollPanel.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.scrollPanel.Scroll += delegate(object sender, ScrollPanelEventArgs args) { OnScroll(args); };
            this.scrollPanel.ScrollNeedsChanged += delegate(object sender, ScrollNeedsEventArgs args) { OnScrollNeedsChanged(args); };
            this.scrollPanel.ScrollParametersChanged += delegate(object sender, RadPanelScrollParametersEventArgs args) { OnScrollParametersChanged(args); };

            this.border = new BorderPrimitive();
			this.border.Class = "RadScrollViewBorder";

			this.fillPrimitive = new FillPrimitive();
			this.fillPrimitive.Class = "RadScrollViewFill";
			this.fillPrimitive.GradientAngle = 45f;

			this.Children.Add(this.fillPrimitive);
            this.Children.Add(this.border);			
            this.Children.Add(this.scrollPanel);

            this.scrollPanel.AutoSize = this.AutoSize;
            this.scrollPanel.AutoSizeMode = this.AutoSizeMode;

            this.scrollPanel.BindProperty(RadElement.AutoSizeProperty, this, RadElement.AutoSizeProperty, PropertyBindingOptions.OneWay);
            this.scrollPanel.BindProperty(RadElement.AutoSizeModeProperty, this, RadElement.AutoSizeModeProperty, PropertyBindingOptions.OneWay);
        }

        protected virtual RadScrollLayoutPanel CreateScrollLayoutPanel()
        {
            return new RadScrollLayoutPanel();
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == RadElement.MaxSizeProperty)
            {
                this.scrollPanel.MaxSize = SubtractBorderSize((Size)e.NewValue);
            }
            else if (e.Property == RadElement.MinSizeProperty)
            {
                this.scrollPanel.MinSize = SubtractBorderSize((Size)e.NewValue);
            }
        }

        private Size SubtractBorderSize(Size size)
        {
            size.Width = size.Width;// -borderSize.Width;
            size.Height = size.Height;// -borderSize.Height;
            return size;
        }

        protected internal override bool ElementThemeAffectsChildren
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}
