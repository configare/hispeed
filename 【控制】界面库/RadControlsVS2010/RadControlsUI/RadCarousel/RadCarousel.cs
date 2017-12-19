using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.UI.Carousel;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{

    /// <summary>
    /// represent Navigation Button possition
    /// </summary>
    public enum NavigationButtonsPosition
    {
        Left, Right, Top, Bottom
    };    

    /// <summary>
    /// RadCarouses is a control that animates a group of items in Carousel-style
    /// rotation.
    /// </summary>
    /// <remarks>
    ///     You can add item to RadCarousel control using Items collection, or through binding
    ///     to data by assigning its DataSource properties. In order to manage the display of
    ///     great number of items you may need to set the <see cref="VirtualMode"/>
    ///     property to <strong>true</strong>. In this case you should specify the maximum
    ///     visible number of item, using the <see cref="VisibleItemCount"/> property.
    ///     Item path can be specified through <see cref="CarouselPath"/> property. Each
    ///     carousel path instance contains properties to adjust various aspects of the path
    ///     curve, including "start" and "end" position, selected items position. If you use a
    ///     RadCarousel bound to a data, you would need to handle the
    ///     <strong>ItemDataBound</strong> event to change each <strong>carouselItem</strong>'s
    ///     properties according to items in the data source. You may also need to handle the
    ///     <strong>CreateNewCarouselItem</strong> event, to change the default type of items
    ///     <strong>RadCarousel</strong> will produce when databinding.
    /// </remarks>
    [ToolboxItem(true)]
    [RadThemeDesignerData(typeof(RadCarouselThemeDesignerData))]
	[Description("Enables a user to select from a group of items, animated in Carousel-style rotation")]
	[DefaultProperty("Items"), DefaultEvent("SelectedItemChanged")]
    [Designer("Telerik.WinControls.UI.Design.RadCarouselDesigner, Telerik.WinControls.UI.Design, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e")]
    public class RadCarousel : RadControl
    {
        private RadCarouselElement carouselElement = null;

        #region Constructors & Initialization

        public RadCarousel()
        {
            // PATCH - for double click in design-time
            Size sz = this.DefaultSize;
            this.ElementTree.PerformInnerLayout(true, 0, 0, sz.Width, sz.Height);
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);

            this.carouselElement = new RadCarouselElement();
            parent.Children.Add(this.CarouselElement);
        }

        /// <summary>
        /// Gets a reference to the Carousel element, which incapsulates the most of the
        /// functionality of RadCarousel
        /// </summary>
        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadCarouselElement CarouselElement
        {
            get
            {
                return this.carouselElement;
            }
        }

        #endregion

        #region ExposedFromElement

        /// <summary>
        /// Gets ot sets the number of animation frames between two positions
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets ot sets the number of animation frames between two positions")]
        [DefaultValue(30)]
        public int AnimationFrames
        {
            get { return this.CarouselElement.AnimationFrames; }
            set { this.CarouselElement.AnimationFrames = value; }
        }

        /// <summary>
        /// Gets or sets the delay in ms. between two frames of animation
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets the delay in ms. between two frames of animation")]
        [DefaultValue(40)]
        public int AnimationDelay
        {
            get { return this.CarouselElement.AnimationDelay; }
            set { this.CarouselElement.AnimationDelay = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating that the Carousel will loop items automatically
        /// </summary>
        [Description("Gets or sets a value indicating that the Carousel will loop items automatically.")]
        [DefaultValue(false)]
        [Category("AutoLoopBehavior")]
        public bool EnableAutoLoop
        {
            get { return this.CarouselElement.CarouselItemContainer.EnableAutoLoop; }

            set { this.CarouselElement.CarouselItemContainer.EnableAutoLoop = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether carousel will increnment or decrement item indexes when in auto-loop mode.
        /// </summary>
        [Description("Gets or sets a value indicating whether carousel will increnment or decrement item indexes when in auto-loop mode.")]
        [DefaultValue(AutoLoopDirections.Forward)]
        [Category("AutoLoopBehavior")]
        public AutoLoopDirections AutoLoopDirection
        {
            get { return this.CarouselElement.CarouselItemContainer.AutoLoopDirection; }

            set { this.CarouselElement.CarouselItemContainer.AutoLoopDirection = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating when carousel will pause looping if in auto-loop mode.
        /// </summary>
        [Description("Gets or sets a value indicating when carousel will pause looping if in auto-loop mode.")]
        [DefaultValue(AutoLoopPauseConditions.OnMouseOverCarousel)]
        [Category("AutoLoopBehavior")]
        public AutoLoopPauseConditions AutoLoopPauseCondition
        {
            get { return this.CarouselElement.CarouselItemContainer.AutoLoopPauseCondition; }

            set { this.CarouselElement.CarouselItemContainer.AutoLoopPauseCondition = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating the interval (in seconds) after which the carousel will resume looping when in auto-loop mode.
        /// </summary>
        [Description("Gets or sets a value indicating the interval (in seconds) after which the carousel will resume looping when in auto-loop mode.")]
        [DefaultValue(3)]
        [Category("AutoLoopBehavior")]
        public int AutoLoopPauseInterval
        {
            get { return this.CarouselElement.AutoLoopPauseInterval; }

            set { this.CarouselElement.AutoLoopPauseInterval = value; }
        }

        ///// <summary>
        ///// Gets or sets the interval of automatic current item change.
        ///// </summary>
        //[Description("Gets or sets the interval of automatic current item change.")]
        //[DefaultValue(1000)]
        //public int Interval
        //{
        //    get { return this.CarouselElement.Interval; }
        //    set { this.CarouselElement.Interval = value; }
        //}

        /// <commentsfrom cref="RadCarouselElement.DataSource" filter=""/>
        [DefaultValue((string)null),
        AttributeProvider(typeof(IListSource)),
        Description("Gets or sets the data source."),
        Category(RadDesignCategory.DataCategory),
        RefreshProperties(RefreshProperties.Repaint)]
        public object DataSource
        {
            get
            {
                return this.CarouselElement.DataSource;
            }
            set { this.CarouselElement.DataSource = value; }
        }

        /// <commentsfrom cref="RadCarouselElement.Items" filter=""/>
        [AttributeProvider("Telerik.WinControls.UI.RadCarouselElement", "Items")]
		[RadEditItemsAction]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor)),
		Category(RadDesignCategory.DataCategory)]
		[RadDescription("Items", typeof(RadCarouselElement))]
        public RadItemCollection Items
        {
            get
            {
                return this.carouselElement.Items;
            }
        }

        /// <commentsfrom cref="RadCarouselElement.SelectedItem" filter=""/>
        [AttributeProvider("Telerik.WinControls.UI.RadCarouselElement", "SelectedItem")]
        public virtual Object SelectedItem
        {
            get { return this.CarouselElement.SelectedItem; }
            set { this.CarouselElement.SelectedItem = value; }
        }

        /// <summary>Gets or sets the item in the carousel that is currently selected.</summary>
        [Description("Gets or sets the currently selected item.")]
        [Category(RadDesignCategory.DataCategory)]
        public virtual int SelectedIndex
        {
            get { return this.CarouselElement.SelectedIndex; }
            set { this.CarouselElement.SelectedIndex = value; }
        }

        /// <commentsfrom cref="RadCarouselElement.SelectedValue" filter=""/>
        [AttributeProvider("Telerik.WinControls.UI.RadCarouselElement", "SelectedValue")]
        public Object SelectedValue
        {
            get { return this.CarouselElement.SelectedValue; }
            set { this.CarouselElement.SelectedValue = value; }
        }

        /// <summary>
        /// Gets or sets the field from the data source to use as the actual value for the
        /// carousel items.
        /// </summary>
        [Description("Gets or sets the property to use as the actual value for the items."),
        Category(RadDesignCategory.DataCategory),
        DefaultValue(""),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]        
        public string ValueMember
        {
            get { return this.CarouselElement.ValueMember; }
            set { this.CarouselElement.ValueMember = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether formatting is applied to the DisplayMember property.
        /// </summary>
        [Description("Gets or sets a value indicating whether formatting is applied to the DisplayMember property."),
        DefaultValue(false)]
        public bool FormattingEnabled
        {
            get { return this.CarouselElement.FormattingEnabled; }
            set { this.CarouselElement.FormattingEnabled = value; }
        }

        /// <summary>
        ///     Gets or sets the number of items that carousel displays when <see cref="VirtualMode"/> is set to <strong>true</strong>.
        /// </summary>
        [Description("Number of items that carousel displays displays when VirtualMode is set to true")]
        [DefaultValue(10)]
        [Category(RadDesignCategory.BehaviorCategory)]
        public int VisibleItemCount
        {
            get { return this.CarouselElement.CarouselItemContainer.VisibleItemCount; }
            set { this.CarouselElement.CarouselItemContainer.VisibleItemCount = value; }
        }

        /// <summary>
        ///     Get or sets value indicating the maximum number of items that will be displayed in
        ///     the carousel, even when there are more Items in the <see cref="Items"/>
        ///     collection. Virtualizing the carousel would significantly improve its performance.
        /// </summary>
        /// <remarks>
        /// False indicates that all items be displayed.
        /// It depends on SelectedIndex, which items are displayed in this case.
        /// </remarks>
        [Description("Indicates the maximum number of items that will be displayed.")]
        [DefaultValue(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
		public bool VirtualMode
		{
			get { return this.CarouselElement.ItemsContainer.Virtualized; }
            set { this.CarouselElement.ItemsContainer.Virtualized = value; }
		}

        /// <summary>
        /// Gets or sets value indicating that when item position goes beyond the carousel
        /// path, it will be displayed again in the beginning of the carousel path.
        /// </summary>
        [Description("Indicates whether items will cycle through carousel path")]
        [DefaultValue(true)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool EnableLooping
        {
            get { return this.CarouselElement.ItemsContainer.EnableLooping; }
            set { this.CarouselElement.ItemsContainer.EnableLooping = value; }
        }

        /// <commentsfrom cref="RadCarouselElement.NewCarouselItemCreating" filter=""/>
        [Browsable(true),
       Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs when new databound carousel item is created.")]
        public event NewCarouselItemCreatingEventHandler NewCarouselItemCreating
        {
            add
            {
                this.CarouselElement.AddEventHandler(RadCarouselElement.NewCarouselItemCreatingEventKey, value);
            }
            remove
            {
                this.CarouselElement.RemoveEventHandler(RadCarouselElement.NewCarouselItemCreatingEventKey, value);
            }
        }

        /// <commentsfrom cref="RadCarouselElement.ItemDataBound" filter=""/>
        [Browsable(true),
       Category(RadDesignCategory.DataCategory),
        Description("Occurs after an Item is databound.")]
        public event ItemDataBoundEventHandler ItemDataBound
        {
            add
            {
                this.CarouselElement.AddEventHandler(RadCarouselElement.ItemDataBoundEventKey, value);
            }
            remove
            {
                this.CarouselElement.RemoveEventHandler(RadCarouselElement.ItemDataBoundEventKey, value);
            }
        }

        /// <commentsfrom cref="RadCarouselElement.SelectedIndexChanged" filter=""/>
        [Browsable(true),
        Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs when the SelectedIndex property has changed.")]
        public event EventHandler SelectedIndexChanged
        {
            add
            {
                this.CarouselElement.AddEventHandler(RadCarouselElement.SelectedIndexChangedEventKey, value);
            }
            remove
            {
                this.CarouselElement.RemoveEventHandler(RadCarouselElement.SelectedIndexChangedEventKey, value);
            }
        }

        /// <commentsfrom cref="RadCarouselElement.SelectedValueChanged" filter=""/>
        [Browsable(true),
        Description("Occurs when the SelectedValue property has changed.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public event EventHandler SelectedValueChanged
        {
            add
            {
                this.CarouselElement.AddEventHandler(RadCarouselElement.SelectedValueChangedEventKey, value);
            }
            remove
            {
                this.CarouselElement.RemoveEventHandler(RadCarouselElement.SelectedValueChangedEventKey, value);
            }
        }

        /// <commentsfrom cref="RadCarouselElement.SelectedItemChanged" filter=""/>
        [Browsable(true),
       Category(RadDesignCategory.BehaviorCategory),
       Description("Occurs when the selected items is changed.")]
        public event EventHandler SelectedItemChanged
        {
            add
            {
                this.CarouselElement.AddEventHandler(RadCarouselElement.SelectedItemChangedEventKey, value);
            }
            remove
            {
                this.CarouselElement.RemoveEventHandler(RadCarouselElement.SelectedItemChangedEventKey, value);
            }
        }

        /// <commentsfrom cref="CarouselItemsContainer.CarouselPath" filter=""/>
        [TypeConverter(typeof(CarouselPathConverter))]
        [Editor(typeof(CarouselPathEditor), typeof(UITypeEditor))]
		[Description("Gets or sets the path which items will follow when animated.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public CarouselParameterPath CarouselPath
        {
            get { return (CarouselParameterPath)this.CarouselElement.ItemsContainer.CarouselPath; }
            set
            {
                bool relativePath = this.EnableRelativePath;
                this.CarouselElement.ItemsContainer.CarouselPath = value;
                this.EnableRelativePath = relativePath;
            }
        }


        [Description("Get or set using the relative point coordinate.If set to true each point should be between 0 and 100")]
        [DefaultValue(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        public bool EnableRelativePath
        {
            set
            {
                if (this.CarouselElement.ItemsContainer.CarouselPath == null)
                    return;

                CarouselParameterPath path =
                    (CarouselParameterPath) this.CarouselElement.ItemsContainer.CarouselPath;

                if (path.EnableRelativePath != value)
                {
                    path.EnableRelativePath = value;
                    this.OnNotifyPropertyChanged("EnableRelativePath");
                }
            }
            get
            {
                if (this.CarouselElement.ItemsContainer.CarouselPath == null)
                    return true;

                return ((CarouselParameterPath)this.CarouselElement.ItemsContainer.CarouselPath).EnableRelativePath;
            }
        }

        /// <summary>
        /// Sets the way opacity is applied to carousel items
        /// </summary>
        [DefaultValue(OpacityChangeConditions.ZIndex)]
        [Description("Sets the way opacity is applied to carousel items.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public OpacityChangeConditions OpacityChangeCondition
        {
            get
            {
               if (this.CarouselElement.ItemsContainer.CarouselPath == null)
                    return OpacityChangeConditions.ZIndex;

                return this.CarouselElement.ItemsContainer.OpacityChangeCondition;
            }
            set
            {
                if (this.CarouselElement.ItemsContainer.CarouselPath == null)
                    return;

                this.CarouselElement.ItemsContainer.OpacityChangeCondition = value;
            }
        }

        /// <summary>
        /// Gets or sets value indicating the minimum value of the opacity applied to items
        /// </summary>
        [Description("Indicates the minimum value of the opacity applied to items")]
        [DefaultValue(0.33)]
        [Category(RadDesignCategory.AppearanceCategory)]
        public double  MinFadeOpacity
        {
            set
            {
                if (this.CarouselElement.ItemsContainer.CarouselPath == null)
                    return;

                this.CarouselElement.ItemsContainer.MinFadeOpacity = value;
            }
            get
            {
                if (this.CarouselElement.ItemsContainer.CarouselPath == null)
                    return 0d;

                return this.CarouselElement.ItemsContainer.MinFadeOpacity;
            }
        }

        /// <commentsfrom cref="CarouselItemsContainer.EasingType" filter=""/>
        [DefaultValue(RadEasingType.OutQuad)]
		[Description("Gets or sets the easing equation to be used for the animations.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public RadEasingType EasingType
        {
            get { return this.CarouselElement.ItemsContainer.EasingType; }
            set { this.CarouselElement.ItemsContainer.EasingType = value; }
        }

        /// <summary>
        /// Gets or sets value indicating which of the predefined animations will be applied to carousel items
        /// </summary>
        [DefaultValue(Animations.All)]
        [Description("Gets or sets the type of animation to be applied to carousel items")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public Animations AnimationsToApply
        {
            get { return this.CarouselElement.ItemsContainer.AnimationsApplied; }
            set { this.CarouselElement.ItemsContainer.AnimationsApplied = value; }
        }

        /// <summary>
        /// Gets or sets value indicating which of the predefined animations will be applied to carousel items
        /// </summary>
        [DefaultValue(Animations.All)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets or sets the type of animation to be applied to carousel items")]
        [Obsolete("Please, use AnimationsToApply property")]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Animations AnimationsToAppy
        {
            get { return this.CarouselElement.ItemsContainer.AnimationsApplied; }
            set { this.CarouselElement.ItemsContainer.AnimationsApplied = value; }
        }

        /// <summary>
        ///     Gets or sets the default action when item is clicked as <see cref="CarouselItemClickAction"/> member.
        /// </summary>
        /// <value>The item click default action.</value>
        [DefaultValue(CarouselItemClickAction.SelectItem)]
		[Description("Gets or sets the action to be performed when a carousel item is clicked")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public CarouselItemClickAction ItemClickDefaultAction
        {
            get { return this.CarouselElement.ItemClickDefaultAction; }
            set { this.CarouselElement.ItemClickDefaultAction = value; }
        }

        /// <summary>
        /// Gets or sets value indicating the height (in percentage - values from 0.0. to 1.0) of reflection that will be painted bellow each carousel item.
        /// </summary>
        /// <value>The item reflection percentage.</value>
        /// <remarks>
        /// 0.0 indicates no reflection and 1.0 indicates 100% of the height of the original item
        /// </remarks>
        [DefaultValue(0.333)]
        [Category(RadDesignCategory.AppearanceCategory)]
        public double ItemReflectionPercentage
        {
            get { return this.CarouselElement.ItemReflectionPercentage; }
            set { this.CarouselElement.ItemReflectionPercentage = value; }
        }


        /// <summary>
        /// Present the Previous button
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Represents the button moving the items back one at a time")]
        [Category(RadDesignCategory.AppearanceCategory)]
		public RadRepeatButtonElement ButtonPrevious
        {
            get
            {
                return this.carouselElement.ButtonPrevious;
            }
        }

        /// <summary>
        /// Pressent the Next button
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("Represents the button moving the items forward one at a time")]
        [Category(RadDesignCategory.AppearanceCategory)]
        public RadRepeatButtonElement ButtonNext
        {
            get
            {
                return this.carouselElement.ButtonNext;
            }
        }

        /// <summary>
        /// Get or sets the minimum size to apply on an element when layout is calculated.
        /// </summary>
        [Description("Represents the navigation buttons offset")]
        [DefaultValue(typeof(Size), "0,0")]
        [Category(RadDesignCategory.AppearanceCategory)]
        public virtual Size NavigationButtonsOffset
        {
            get
            {
                return this.carouselElement.NavigationButtonsOffset;
            }
            set
            {
                this.carouselElement.NavigationButtonsOffset = value;
            }
        }


        /// <summary>
        /// Represent the Navigation buttons Possitions
        /// </summary>
        [DefaultValue(NavigationButtonsPosition.Bottom)]
		[Description("Gets or sets the location of the navigation buttons")]
        [Category(RadDesignCategory.AppearanceCategory)]
        public virtual NavigationButtonsPosition ButtonPositions
        {
            get
            {
                return this.carouselElement.ButtonPositions;
            }
            set
            {
                this.carouselElement.ButtonPositions = value;
            }
        }

        #endregion

		/// <summary>
		/// Gets the default size of the control.
		/// </summary>		
		protected override Size DefaultSize
		{
			get
			{
				return new Size(240, 150);
			}
		}

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (element == this.ButtonNext ||
                element == this.ButtonPrevious )
                return true;

            return base.ControlDefinesThemeForElement(element);
        }
    }
}
