using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Design;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI;
using System.Reflection;
using System.Resources;
namespace Telerik.WinControls.UI
{


	/// <summary>
	/// Represents a RadPanelBarElement. The RadPanelBarElement class is a simple wrapper for 
	/// the RadPanelBarElement class. The RadPanelBar acts to transfer events to and from 
	/// its corresponding RadPanelBarElement instance. The RadPanelBarElement which is 
	/// essentially the RadPanelBar control may be nested in other telerik controls.
	/// </summary>
	[ToolboxItem(false)]
	public class RadPanelBarElement : LightVisualElement
	{
		private PanelBarStyleBase currentStyle;
		private RadItemOwnerCollection items;
		private RadTabStripContentPanel contentPanel;
		private RadPanelBarContentControl itemsControl;
		internal List<RadPanelBarGroupElement> hiddenGroupsList;
      	internal RadPanelBarVisualElement captionElement = new RadPanelBarVisualElement();
        private List<bool> groupStates = new List<bool>();
        private int height = 0;
        EventHandler loadedEventHandler;

        /// <summary>
        /// Occurs when the children collection is refreshed by changing the style or selection.
        /// </summary>
        [Category("Action")]
        [Description("Occurs when the children collection is refreshed by changing the style or selection.")]
        public event EventHandler Load
        {
            add
            {
                lock (loadedLock)
                {
                    loadedEventHandler += value;
                }
            }
            remove
            {
                lock (loadedLock)
                {
                    loadedEventHandler -= value;
                }
            }
        }

        readonly object loadedLock = new object();

        protected internal virtual void OnLoad()
        {
            EventHandler handler;
            lock (loadedLock)
            {
                handler = loadedEventHandler;
            }
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

		#region Events
		/// <summary>
		/// Occurs when the  Caption property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the  Caption property value changes.")]
		public event EventHandler CaptionChanged;

		/// <summary>
		/// Occurs when the SpacingBetweenItems property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the SpacingBetweenItems property value changes.")]
		public event EventHandler SpacingBetweenItemsChanged;

		/// <summary>
		/// Occurs when the PanelBarStyle property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the PanelBarStyle property value changes.")]
		public event EventHandler PanelBarStyleChanged;

		/// <summary>
		/// Occurs when the TopOffset property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the TopOffset property value changes.")]
		public event EventHandler TopOffsetChanged;

		/// <summary>
		/// Occurs when the BottomOffset property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the BottomOffset property value changes.")]
		public event EventHandler BottomOffsetChanged;

		/// <summary>
		/// Occurs when the RightOffset property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the RightOffset property value changes.")]
		public event EventHandler RightOffsetChanged;

		/// <summary>
		/// Occurs when the LeftOffset property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the LeftOffset property value changes.")]
		public event EventHandler LeftOffsetChanged;

		/// <summary>
		/// Occurs when the SpacingBetweenGroups property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the SpacingBetweenGroups property value changes.")]
		public event EventHandler SpacingBetweenGroupsChanged;

		/// <summary>
		/// Occurs when the SpacingBetweenColumns property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the SpacingBetweenColumns property value changes.")]
		public event EventHandler SpacingBetweenColumnsChanged;

		/// <summary>
		/// Occurs when the NumberOfColumns property value changes.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the NumberOfColumns property value changes.")]
		public event EventHandler NumberOfColumnsChanged;

		/// <summary>
		/// Occurs when a group has been expanded
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group has been expanded")]
		public event PanelBarGroupEventHandler PanelBarGroupExpanded;

		/// <summary>
		/// Occurs when a group is going to be expanded
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group is going to be expanded")]
		public event PanelBarGroupCancelEventHandler PanelBarGroupExpanding;

		/// <summary>
		/// Occurs when a group has been selected
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group has been selected")]
		public event PanelBarGroupEventHandler PanelBarGroupSelected;

		/// <summary>
		/// Occurs when a group is going to be selected
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group is going to be selected")]
		public event PanelBarGroupCancelEventHandler PanelBarGroupSelecting;


		/// <summary>
		/// Occurs when a group has been collapsed
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group has been collapsed")]
		public event PanelBarGroupEventHandler PanelBarGroupCollapsed;

		/// <summary>
		/// Occurs when a group is going to be collapsed
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group is going to be collapsed")]
		public event PanelBarGroupCancelEventHandler PanelBarGroupCollapsing;

		/// <summary>
		/// Occurs when a group has been unselected
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group has been unselected")]
		public event PanelBarGroupEventHandler PanelBarGroupUnSelected;

		/// <summary>
		/// Occurs when a group is going to be unselected
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when a group is going to be unselected")]
		public event PanelBarGroupCancelEventHandler PanelBarGroupUnSelecting;

		protected virtual void OnPanelBarGroupSelected(PanelBarGroupEventArgs args)
		{
			if (this.PanelBarGroupSelected != null)
			{
				this.PanelBarGroupSelected(this, args);
			}
		}

		protected virtual void OnPanelBarGroupExpanded(PanelBarGroupEventArgs args)
		{
			if (this.PanelBarGroupExpanded != null)
			{
				this.PanelBarGroupExpanded(this, args);
			}
		}

		protected virtual void OnPanelBarGroupUnSelected(PanelBarGroupEventArgs args)
		{
			if (this.PanelBarGroupUnSelected != null)
			{
				this.PanelBarGroupUnSelected(this, args);
			}
		}

		protected virtual void OnPanelBarGroupCollapsed(PanelBarGroupEventArgs args)
		{
			if (this.PanelBarGroupCollapsed != null)
			{
				this.PanelBarGroupCollapsed(this, args);
			}
		}

		protected virtual void OnPanelBarGroupUnSelecting(PanelBarGroupCancelEventArgs args)
		{
			if (this.PanelBarGroupUnSelecting != null)
			{
				this.PanelBarGroupUnSelecting(this, args);
			}
		}

		protected virtual void OnPanelBarGroupCollapsing(PanelBarGroupCancelEventArgs args)
		{
			if (this.PanelBarGroupCollapsing != null)
			{
				this.PanelBarGroupCollapsing(this, args);
			}
		}

		internal void CallPanelBarGroupSelecting(PanelBarGroupCancelEventArgs args)
		{
			OnPanelBarGroupSelecting(args);
		}

		internal void CallPanelBarGroupSelected(PanelBarGroupEventArgs args)
		{
			OnPanelBarGroupSelected(args);
		}

		internal void CallPanelBarGroupExpanding(PanelBarGroupCancelEventArgs args)
		{
			OnPanelBarGroupExpanding(args);
		}

		internal void CallPanelBarGroupExpanded(PanelBarGroupEventArgs args)
		{
			OnPanelBarGroupExpanded(args);
		}

		//
		internal void CallPanelBarGroupUnSelecting(PanelBarGroupCancelEventArgs args)
		{
			OnPanelBarGroupUnSelecting(args);
		}

		internal void CallPanelBarGroupUnSelected(PanelBarGroupEventArgs args)
		{
			OnPanelBarGroupUnSelected(args);
		}

		internal void CallPanelBarGroupCollapsing(PanelBarGroupCancelEventArgs args)
		{
			OnPanelBarGroupCollapsing(args);
		}

		internal void CallPanelBarGroupCollapsed(PanelBarGroupEventArgs args)
		{
			OnPanelBarGroupCollapsed(args);
		}

		protected virtual void OnPanelBarGroupExpanding(PanelBarGroupCancelEventArgs args)
		{
			if (this.PanelBarGroupExpanding != null)
			{
				this.PanelBarGroupExpanding(this, args);
			}
		}

		protected virtual void OnPanelBarGroupSelecting(PanelBarGroupCancelEventArgs args)
		{
			if (this.PanelBarGroupSelecting != null)
			{
				this.PanelBarGroupSelecting(this, args);
			}
		}
		/// <summary>
		///		Raises the <see cref="TopOffsetChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnTopOffsetChanged(EventArgs args)
		{
			if (this.TopOffsetChanged != null)
			{
				this.TopOffsetChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="BottomOffsetChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnBottomOffsetChanged(EventArgs args)
		{
			if (this.BottomOffsetChanged != null)
			{
				this.BottomOffsetChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="RightOffsetChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnRightOffsetChanged(EventArgs args)
		{
			if (this.RightOffsetChanged != null)
			{
				this.RightOffsetChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="LeftOffsetChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnLeftOffsetChanged(EventArgs args)
		{
			if (this.LeftOffsetChanged != null)
			{
				this.LeftOffsetChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="SpacingBetweenColumnsChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnSpacingBetweenColumnsChanged(EventArgs args)
		{
			if (this.SpacingBetweenColumnsChanged != null)
			{
				this.SpacingBetweenColumnsChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="SpacingBetweenGroupsChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnSpacingBetweenGroupsChanged(EventArgs args)
		{
			if (this.SpacingBetweenGroupsChanged != null)
			{
				this.SpacingBetweenGroupsChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="NumberOfColumnsChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnNumberOfColumnsChanged(EventArgs args)
		{
			if (this.NumberOfColumnsChanged != null)
			{
				this.NumberOfColumnsChanged(this, args);
			}

            UpdateScrollbars();
		}

        public void UpdateScrollbars()
        {
            this.groupStates.Clear();
            RadPanelBarElement_ArrangeModified(this, EventArgs.Empty);
        }

		/// <summary>
		///		Raises the <see cref="CaptionChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnCaptionChanged(EventArgs args)
		{
			if (this.CaptionChanged != null)
			{
				this.CaptionChanged(this, args);
			}
		}

		/// <summary>
		///		Raises the <see cref="SpacingBetweenItemsChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnSpacingBetweenItemsChanged(EventArgs args)
		{
			if (this.SpacingBetweenItemsChanged != null)
			{
				this.SpacingBetweenItemsChanged(this, args);
			}

            this.groupStates.Clear();
            RadPanelBarElement_ArrangeModified(this, EventArgs.Empty);
		
		}

		/// <summary>
		///		Raises the <see cref="PanelBarStyleChanged"/> event.
		/// </summary>
		/// <param name="args">An <see cref="EventArgs"/> that contains the event data.</param>
		protected virtual void OnPanelBarStyleChanged(EventArgs args)
		{
			if (this.PanelBarStyleChanged != null)
			{
				this.PanelBarStyleChanged(this, args);
			}

            this.groupStates.Clear();
        }
		#endregion
		#region Properties

		public static RadProperty InitialOffsetProperty = RadProperty.Register(
			"InitialOffset", typeof(int), typeof(RadPanelBarElement), new RadElementPropertyMetadata(
			5, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty SpacingBetweenItemsProperty = RadProperty.Register(
			"SpacingBetweenItems", typeof(int), typeof(RadPanelBarElement), new RadElementPropertyMetadata(
			5, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsArrange | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty CaptionHeightProperty = RadProperty.Register(
		"CaptionHeight", typeof(int), typeof(RadPanelBarElement), new RadElementPropertyMetadata(
			25, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty CaptionButtonRightOffsetProperty = RadProperty.Register(
			"CaptionButtonRightOffset", typeof(int), typeof(RadPanelBarElement), new RadElementPropertyMetadata(
			5, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty PanelBarStyleProperty = RadProperty.Register(
		"PanelBarStyle", typeof(PanelBarStyles), typeof(RadPanelBarElement), new RadElementPropertyMetadata(
			PanelBarStyles.ListBar, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty SpacingBetweenGroupsProperty = RadProperty.Register(
		"SpacingBetweenGroups", typeof(int), typeof(RadPanelBarElement), new RadElementPropertyMetadata(
			0, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsArrange | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty SpacingBetweenColumnsProperty = RadProperty.Register(
		"SpacingBetweenColumns", typeof(int), typeof(RadPanelBarElement), new RadElementPropertyMetadata(
			5, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsArrange | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty NumberOfColumnsProperty = RadProperty.Register(
		"NumberOfColumns", typeof(int), typeof(RadPanelBarElement), new RadElementPropertyMetadata(
			1, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsArrange | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty RightOffsetProperty = RadProperty.Register(
		"RightOffset", typeof(int), typeof(RadPanelBarElement), new RadElementPropertyMetadata(
			0, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsArrange | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty LeftOffsetProperty = RadProperty.Register(
		"LeftOffset", typeof(int), typeof(RadPanelBarElement), new RadElementPropertyMetadata(
			0, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsArrange | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty BottomOffsetProperty = RadProperty.Register(
		"BottomOffset", typeof(int), typeof(RadPanelBarElement), new RadElementPropertyMetadata(
			0, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsArrange | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty TopOffsetProperty = RadProperty.Register(
		"TopOffset", typeof(int), typeof(RadPanelBarElement), new RadElementPropertyMetadata(
			0, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsArrange | ElementPropertyOptions.AffectsMeasure));

		public static RadProperty CaptionProperty = RadProperty.Register(
			"Caption", typeof(string), typeof(RadPanelBarElement), new RadElementPropertyMetadata(
			"Default Caption", ElementPropertyOptions.None));

		public static RadProperty ShowCaptionImagesProperty = RadProperty.Register(
			"ShowCaptionImages", typeof(bool), typeof(RadPanelBarElement),
		new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public PanelBarStyleBase CurrentStyle
		{
			get
			{
				return this.currentStyle;
			}
		}

		/// <summary>
		/// Gets or sets caption's height.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets caption's height.")]
		[DefaultValue(25)]
		[Obsolete("This property will be removed in the next version")]
		public int CaptionHeight
		{
			get
			{
				return (int)this.GetValue(CaptionHeightProperty);
			}
			set
			{
				this.SetValue(CaptionHeightProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets whether groups should show images in their caption
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets whether groups should show images in their caption.")]
		[DefaultValue(true)]
		[Obsolete("This property will be removed in the next version")]
		public bool ShowCaptionImages
		{
			get
			{
				return (bool)this.GetValue(ShowCaptionImagesProperty);
			}
			set
			{
				this.SetValue(ShowCaptionImagesProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the font of the OutLookStyle's caption.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets the font of the OutLookStyle's caption.")]
		public Font CaptionTextFont
		{
			get
			{
				if (this.CurrentStyle as OutlookStyle != null && this.Children[0] != null && this.Children[0].Children.Count > 0 && this.Children[0].Children[0] != null)
				{
					RadPanelBarVisualElement caption = this.Children[0].Children[0] as RadPanelBarVisualElement;

					if (caption != null)
					{
						return caption.Font;
					}
				}

				return this.Font;
			}
			set
			{
				if (this.CurrentStyle as OutlookStyle != null && this.Children[0] != null && this.Children[0].Children.Count > 0 && this.Children[0].Children[0] != null)
				{
					RadPanelBarVisualElement caption = this.Children[0].Children[0] as RadPanelBarVisualElement;
					if (caption != null)
					{
						caption.Font = value;
					}
				}
				else
				{
					this.Font = value;
				}
			}
		}



		/// <summary>
		/// Gets or sets the Caption
		/// </summary>
		[Browsable(false), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets the caption")]
		[DefaultValue("Default Caption")]
		[Localizable(true)]
		public string Caption
		{
			get
			{
				return (string)this.GetValue(CaptionProperty);
			}
			set
			{
				this.SetValue(CaptionProperty, value);
				this.OnCaptionChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the right offset of the caption button which is part of the caption of every group
		/// </summary>
		[Browsable(false), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets the right offset of the caption button which is part of the caption of every group")]
		[DefaultValue(5)]
		[Obsolete("This property will be removed in the next version")]
		public int CaptionButtonRightOffset
		{
			get
			{
				return (int)this.GetValue(CaptionButtonRightOffsetProperty);
			}
			set
			{
				this.SetValue(CaptionButtonRightOffsetProperty, value);
			}
		}


		/// <summary>
		/// Gets or sets the Spacing between caption elements
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets the Spacing between caption elements")]
		[DefaultValue(5)]
		public int SpacingBetweenItems
		{
			get
			{
				return (int)this.GetValue(SpacingBetweenItemsProperty);
			}
			set
			{
				this.SetValue(SpacingBetweenItemsProperty, value);
				this.OnSpacingBetweenItemsChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the initial offset of caption elements
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets the initial offset of caption elements")]
		[DefaultValue(5)]
		public int InitialOffset
		{
			get
			{
				return (int)this.GetValue(InitialOffsetProperty);
			}
			set
			{

				this.SetValue(InitialOffsetProperty, value);
				this.OnNotifyPropertyChanged("InitialOffset");
			}
		}


		/// <summary>
		/// Gets or sets the style of the PanelBar
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets the style of the PanelBar")]
		[DefaultValue(typeof(PanelBarStyles), "ListBar")]
		public PanelBarStyles PanelBarStyle
		{
			get
			{
				return (PanelBarStyles)this.GetValue(PanelBarStyleProperty);
			}
			set
			{
        		this.SetValue(PanelBarStyleProperty, value);
				this.OnPanelBarStyleChanged(EventArgs.Empty);

			}
		}

		/// <summary>
		/// Gets or sets the RightOffset of the inner area
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[Description("Gets or sets the RightOffset of the inner area")]
		[DefaultValue(0)]
		public int RightOffset
		{
			get
			{
				return (int)this.GetValue(RightOffsetProperty);
			}
			set
			{
				this.SetValue(RightOffsetProperty, value);
				this.OnRightOffsetChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the BottomOffset of the inner area
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[Description("Gets or sets the BottomOffset of the inner area")]
		[DefaultValue(0)]
		public int BottomOffset
		{
			get
			{
				return (int)this.GetValue(BottomOffsetProperty);
			}
			set
			{
				this.SetValue(BottomOffsetProperty, value);
				this.OnBottomOffsetChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the TopOffset of the inner area
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[Description("Gets or sets the TopOffset of the inner area")]
		[DefaultValue(0)]
		public int TopOffset
		{
			get
			{
				return (int)this.GetValue(TopOffsetProperty);
			}
			set
			{
				this.SetValue(TopOffsetProperty, value);
				this.OnTopOffsetChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the LeftOffset of the inner area
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[Description("Gets or sets the LeftOffset of the inner area")]
		[DefaultValue(0)]
		public int LeftOffset
		{
			get
			{
				return (int)this.GetValue(LeftOffsetProperty);
			}
			set
			{
				this.SetValue(LeftOffsetProperty, value);
				this.OnLeftOffsetChanged(EventArgs.Empty);
			}
		}


		/// <summary>
		/// Gets or sets the number of columns
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[Description("Gets or sets the number of columns")]
		[DefaultValue(1)]
		public int NumberOfColumns
		{
			get
			{
				return (int)this.GetValue(NumberOfColumnsProperty);
			}
			set
			{
				if (value > this.Items.Count)
					value = this.Items.Count;
				if (value < 1) value = 1;

				this.SetValue(NumberOfColumnsProperty, value);
				this.OnNumberOfColumnsChanged(EventArgs.Empty);
			}

		}

		/// <summary>
		/// Gets or sets the spacing between columns
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[Description("Gets or sets the spacing between columns")]
		[DefaultValue(5)]
		public int SpacingBetweenColumns
		{
			get
			{
				return (int)this.GetValue(SpacingBetweenColumnsProperty);
			}
			set
			{
				this.SetValue(SpacingBetweenColumnsProperty, value);
				this.OnSpacingBetweenColumnsChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the spacing between groups
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[Description("Gets or sets the spacing between groups")]
		[DefaultValue(0)]
		public int SpacingBetweenGroups
		{
			get
			{
				return (int)this.GetValue(SpacingBetweenGroupsProperty);
			}
			set
			{
				this.SetValue(SpacingBetweenGroupsProperty, value);
				this.OnSpacingBetweenGroupsChanged(EventArgs.Empty);
			}
		}

      
      


		/// <summary>
		///		Gets a collection of items which are children of the PanelBar element.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.DataCategory)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor))]
		[Description("Gets the collection of items which are children of the PanelBar element.")]
		public RadItemOwnerCollection Items
		{
			get
			{
				return this.items;
			}
		}

		#endregion


		protected override void InitializeFields()
        {
            base.InitializeFields();

            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] { typeof(RadPanelBarGroupElement) };
            this.items.ItemsChanged += new ItemChangedDelegate(items_ItemsChanged);
            this.DrawBorder = true;
            this.hiddenGroupsList = new List<RadPanelBarGroupElement>();
            this.PanelBarGroupCollapsed += new PanelBarGroupEventHandler(RadPanelBarElement_PanelBarGroupCollapsed);
        }

        private void RadPanelBarElement_PanelBarGroupCollapsed(object sender, PanelBarGroupEventArgs args)
        {
            if (this.Children.Count > 0 && this.ElementTree != null)
            {
                RadPanelBar panelBar = this.ElementTree.Control as RadPanelBar;

                if (panelBar != null && this.PanelBarStyle != PanelBarStyles.ExplorerBarStyle && this.PanelBarStyle != PanelBarStyles.VisualStudio2005ToolBox)
                {
                    panelBar.vScrollBar.Value = 0;
                    this.Children[0].PositionOffset = new SizeF(
                    this.Children[0].PositionOffset.Width,
                    0);
                }
            }
        }


        private void items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
		{
            this.groupStates.Clear();
            if (operation == ItemsChangeOperation.Inserted)
            {
                if (target is RadPanelBarGroupElement)
                {
                    this.SetStyle();
                    this.CurrentStyle.SyncHostedPanels(new RadPanelBarGroupElement[] { target as RadPanelBarGroupElement },
                        (target as RadPanelBarGroupElement).EnableHostControlMode);
                }
            }
		}

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			if (e.Property == TopOffsetProperty || e.Property == RightOffsetProperty ||
				e.Property == SpacingBetweenGroupsProperty || e.Property == SpacingBetweenColumnsProperty
				|| e.Property == NumberOfColumnsProperty || e.Property == BottomOffsetProperty || e.Property == LeftOffsetProperty
				|| e.Property == PanelBarStyleProperty)
			{
						OnNotifyPropertyChanged(e.Property.Name);
			}



			base.OnPropertyChanged(e);
		}


		static RadPanelBarElement()
		{
			new Themes.ControlDefault.PanelBar().DeserializeTheme();
            new Themes.ControlDefault.ScrollBar().DeserializeTheme();
		}


		internal void SetStyle()
		{
			RadPanelBar panelBar = null;

			if (this.ElementTree != null)
			{
				panelBar = this.ElementTree.Control as RadPanelBar;
			}

			
			if (this.currentStyle != null)
			{
				this.currentStyle.GetBaseLayout().ArrangeModified -= new EventHandler(RadPanelBarElement_ArrangeModified);

				currentStyle.UnWireEvents();
			}

            this.Children.Clear();
          
			PanelBarStyleBase style = null;

			switch (PanelBarStyle)
			{
				case PanelBarStyles.ListBar:
					style = new ListBarStyle(this);
					break;
				case PanelBarStyles.ExplorerBarStyle:
					style = new ExplorerBarStyle(this);
					break;
				case PanelBarStyles.OutlookNavPane:
					style = new OutlookStyle(this, this.contentPanel, this.itemsControl);
					break;
				case PanelBarStyles.VisualStudio2005ToolBox:
					style = new VS2005Style(this);
					break;

			}

			bool styleChanged = false;

			if (this.currentStyle != null)
			{
				styleChanged = this.currentStyle.GetType() != style.GetType();
			}

			this.currentStyle = style;

			style.CreateChildren();

			if (panelBar != null && panelBar.CanScroll)
			{

				this.Children[0].PositionOffset = SizeF.Empty;
				panelBar.vScrollBar.Value = 0;
				panelBar.vScrollBar.Visible = false;
                this.groupStates.Clear();
			}

			style.WireEvents();

			RadPanelBarGroupElement selectedGroup = null;

			foreach (RadPanelBarGroupElement group in this.Items)
			{
				if (group.EnableHostControlMode)
				{
                    if (group.ContentPanelSize != null)
                    {
                        group.ApplyContentSize(group.ContentPanelSize.Value);
                    }

					group.ResetContentPanelProperties();
					this.currentStyle.SyncHostedPanels(new RadPanelBarGroupElement[] { group }, true);
				}
				else
				{
					this.currentStyle.SyncHostedPanels(new RadPanelBarGroupElement[] { group }, false);
				}

				if (group.Selected && this.itemsControl != null)
				{
					selectedGroup = group;
				}

         
			}

			if (styleChanged)
			{
				ResetGroupsOnStyleChanged(selectedGroup);
			}

            //this.currentStyle.UpdateGroupsUI();

            this.OnLoad();
            
            if (this.PanelBarStyle == PanelBarStyles.ListBar || this.PanelBarStyle == PanelBarStyles.OutlookNavPane)
                return;

			this.currentStyle.GetBaseLayout().ArrangeModified += new EventHandler(RadPanelBarElement_ArrangeModified);
     
		}

		private void ResetGroupsOnStyleChanged(RadPanelBarGroupElement selectedGroup)
		{
			for (int i = 0; i < this.Items.Count; i++)
			{
				RadPanelBarGroupElement group = this.Items[i] as RadPanelBarGroupElement;
				group.Selected = false;
				group.Expanded = false;
			}

			if (selectedGroup != null)
			{
				selectedGroup.Items.Owner = selectedGroup.verticalGroupLayout;
			}

			if (this.currentStyle is OutlookStyle && this.Items.Count > 0 )
			{
				RadPanelBarGroupElement group = this.Items[0] as RadPanelBarGroupElement;
				group.Selected = true;		
			}
		}

        private void RadPanelBarElement_ArrangeModified(object sender, EventArgs e)
		{
         	// ScrollBar logic             
            if (this.ElementTree != null && this.ElementTree.Control is RadPanelBar && this.Items.Count > 0)
            {
                RadPanelBar panelBar = this.ElementTree.Control as RadPanelBar;
                bool updateScrolling = this.ShouldUpdateScrollBar(panelBar);

                if (!updateScrolling)
                    return;

                groupStates.Clear();
             
                if (!panelBar.CanScroll)
                {
                    this.Children[0].MaxSize = Size.Empty;
                    return;
                }

                if (panelBar.Width <= 0 || panelBar.Height <= 0)
                    return;

                if (panelBar.GroupStyle == PanelBarStyles.OutlookNavPane || panelBar.GroupStyle == PanelBarStyles.ListBar)
                {
                    panelBar.vScrollBar.Size = Size.Empty;
                    panelBar.vScrollBar.Value = 0;
                    return;
                }

                int calculatedHeight = this.Items[this.Items.Count - 1].ControlBoundingRectangle.Bottom - (int)this.Children[0].PositionOffset.Height;

                bool isExplorerBar = this.PanelBarStyle == PanelBarStyles.ExplorerBarStyle && this.NumberOfColumns > 1;
                int currentHeight = 0;

                if (isExplorerBar)
                {
                    int column = 0;
                    int newHeight = 0;
                    foreach (RadPanelBarGroupElement group in this.Items)
                    {
                        currentHeight = Math.Max(group.ControlBoundingRectangle.Height + this.SpacingBetweenGroups, currentHeight);
                        column++;

                        if (column >= this.NumberOfColumns)
                        {
                            column = 0;
                            newHeight += currentHeight;
                            currentHeight = 0;
                        }
                    }

                    if (column > 0)
                    {
                        newHeight += currentHeight;
                    }

                    calculatedHeight = newHeight;
                }


                if (calculatedHeight > 4 + this.ElementTree.Control.Size.Height)
                {
                    int y = panelBar.vScrollBar.Value;
                    SizeF tmpSize = this.Children[0].PositionOffset;

                    if (!panelBar.vScrollBar.Visible)
                    {
                        this.Children[0].PositionOffset = SizeF.Empty;
                        panelBar.vScrollBar.Value = 0;
                        panelBar.vScrollBar.Visible = true;
                    }

                    int largeChange = panelBar.Size.Height / 2;
                    panelBar.vScrollBar.LargeChange = largeChange;

                    int calculatedItemsHeight = this.TopOffset + this.BorderThickness.Vertical;
                    foreach (RadPanelBarGroupElement group in this.Items)
                    {
                        groupStates.Add(group.Expanded);
                        calculatedItemsHeight += group.ControlBoundingRectangle.Height + this.SpacingBetweenGroups;
                    }

                    height = panelBar.Height;

                    int maximum = calculatedItemsHeight - panelBar.Size.Height + largeChange;

                    if (isExplorerBar)
                    {
                        maximum = calculatedHeight - panelBar.Size.Height + largeChange;
                    }

                    panelBar.vScrollBar.Maximum = maximum;
                 

                    panelBar.vScrollBar.Size = new Size(17, panelBar.Size.Height - 2);

                    if (!this.RightToLeft)
                    {
                        panelBar.vScrollBar.Location = new Point(panelBar.Size.Width - 17, 1);
                    }
                    else
                    {
                        panelBar.vScrollBar.Location = new Point(0, 1);
                    }

                    this.Children[0].MaxSize = new Size(panelBar.Size.Width - panelBar.vScrollBar.Size.Width, 0);
                    if (y + largeChange >= maximum + 10)
                    {
                        y = maximum - largeChange;
                    }

                    panelBar.vScrollBar.Value = y;


                    this.Children[0].PositionOffset = new SizeF(
                        this.Children[0].PositionOffset.Width,
                        -panelBar.vScrollBar.Value);
                }
                else
                {
                    this.Children[0].MaxSize = Size.Empty;
                    panelBar.vScrollBar.Visible = false;
                    this.Children[0].PositionOffset = SizeF.Empty;
                    panelBar.vScrollBar.Value = 0;

                }
            }
		}

        private bool ShouldUpdateScrollBar(RadPanelBar panelBar)
        {
            bool updateScrolling = false;

            if (!this.RightToLeft)
            {
                if (panelBar.Width - panelBar.vScrollBar.Width != panelBar.vScrollBar.Location.Y)
                {
                    return true;
                }
            }

            if (this.groupStates.Count != this.Items.Count)
                return true;

            if (groupStates.Count > 0)
            {
                for (int i = 0; i < groupStates.Count; i++)
                {
                    if (i >= this.Items.Count)
                    {
                        updateScrolling = true;
                        break;
                    }

                    RadPanelBarGroupElement group = this.Items[i] as RadPanelBarGroupElement;
                    if (groupStates[i] != group.Expanded)
                    {
                        updateScrolling = true;
                        break;
                    }
                }
            }
            else
            {
                updateScrolling = true;
            }

            if (panelBar.Height != height && height > 0)
            {
                updateScrolling = true;
            }
            return updateScrolling;
        }

		#region Update
		private bool updating;
		public void BeginUpdate()
		{
			this.updating = true;
		}

		public void EndUpdate()
		{
			this.updating = false;
		}


		/// <summary>
		/// Gets whether the panelbar is currently updating
		/// </summary>
		[Browsable(false)]
		public bool IsUpdating
		{
			get
			{
				return this.updating;
			}
		}
		#endregion

		protected override void OnNotifyPropertyChanged(string propertyName)
		{
			base.OnNotifyPropertyChanged(propertyName);

			switch (propertyName)
			{
				case "PanelBarStyle":
					foreach (RadPanelBarGroupElement group in this.hiddenGroupsList)
					{
						this.Items.Add(group);
					}

					this.hiddenGroupsList.Clear();
					SetStyle();
					break;
				case "TopOffset":
				case "BottomOffset":
				case "RightOffset":
				case "LeftOffset":
				case "SpacingBetweenColumns":
				case "SpacingBetweenGroups":
				case "NumberOfColumns":
					this.CurrentStyle.GetBaseLayout().PerformLayout();
					break;
				case "InitialOffset":
					foreach (RadPanelBarGroupElement group in this.Items)
					{
						group.InitialOffset = this.InitialOffset;
					}
					break;

			}
		}

		protected override void CreateChildElements()
		{
            this.captionElement.SetDefaultValueOverride(RadItem.TextProperty, "OutlookStyle");
            this.captionElement.SetDefaultValueOverride(LightVisualElement.DrawFillProperty, true);
            this.captionElement.SetDefaultValueOverride(LightVisualElement.DrawBorderProperty, false);
            this.captionElement.SetDefaultValueOverride(LightVisualElement.ClassProperty, "OutLookCaption");

			SetStyle();
		}

		internal RadTabStripContentPanel GetContentPanel()
		{
			return this.contentPanel;
		}


		internal void SetContentPanel(RadTabStripContentPanel contentPanel)
		{
			this.contentPanel = contentPanel;
		}


		internal void SetItemsControl(RadPanelBarContentControl itemsControl)
		{
			this.itemsControl = itemsControl;
		}
	}
}