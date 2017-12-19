using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Primitives;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using System.Collections;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Represents a tool strip manager. The ToolStrip manager makes sure that the items 
	/// are relocated appropriately after each drag and drop or resize operation. 
	/// </summary>
	public class RadToolStripManager : RadItem
	{
		#region Fields
		private RadItemOwnerCollection items;

		internal StripLayoutPanel verticalLayout;

		private FillPrimitive toolStripFill;

		internal bool parentAutoSize;

		private Stack<RadToolStripItem> toolStripItems;

		internal ArrayList elementList;
		internal ArrayList formList;
		internal ArrayList indexList;

		private RootRadElement rootElement;

		internal Timer resizeTimer;

		private ToolStripDialogForm DialogForm;

		private ArrayList dockingSites;
		#endregion

		#region Constructors

		protected override void InitializeFields()
        {
            base.InitializeFields();

            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] { typeof(RadToolStripElement) };
            this.toolStripItems = new Stack<RadToolStripItem>();
            this.items.ItemsChanged += new ItemChangedDelegate(items_ItemsChanged);
            this.elementList = new ArrayList();
            this.indexList = new ArrayList();
            this.formList = new ArrayList();
            this.parentAutoSize = true;
            this.dockingSites = new ArrayList();
            this.resizeTimer = new Timer();
            this.resizeTimer.Interval = 1000;
            this.DialogForm = new ToolStripDialogForm(this);
            this.DialogForm.Shown += new EventHandler(DialogForm_Shown);
        }

        private void DialogForm_Shown(object sender, EventArgs e)
        {
            Debug.Assert(this.ElementState == ElementState.Loaded, "Invalid RadToolStripManagerState - showing dialog form while not loaded.");
            if (this.ElementState != ElementState.Loaded)
            {
                this.DialogForm.Hide();
                return;
            }

            this.DialogForm.Owner = this.ElementTree.Control.FindForm();
        }

        protected override void DisposeManagedResources()
        {
            if (this.DialogForm != null)
            {
                this.DialogForm.Dispose();
            }
            if (this.resizeTimer != null)
            {
                this.resizeTimer.Stop();
                this.resizeTimer.Dispose();
            }

            base.DisposeManagedResources();
        }

		private object GetItemByKey(string key)
		{
			foreach (RadToolStripElement element in this.Items)
			{
				foreach (RadToolStripItem item in element.Items)
				{
					if (String.Compare(item.Key, key) == 0)
					{
						return item;
					}
				}
			}

			foreach (RadToolStripElement element in this.elementList)
			{
				foreach (RadToolStripItem item in element.Items)
				{
					if (String.Compare(item.Key, key) == 0)
					{
						return item;
					}
				}
			}

			foreach (FloatingForm form in this.formList)
			{
				if (form.Key == key)
				{
					return form;
				}
			}

			return null;
		}

		private int VisibleItemsOnRow(RadToolStripElement element)
		{
			int i = 0;
			foreach (RadToolStripItem item in element.Items)
			{
				if (item.Visibility == ElementVisibility.Visible)
					i++;
			}
			return i;
		}
        /// <summary>
        /// Toggles tool strip item visibility by a given key.
        /// </summary>
        /// <param name="key"></param>
		public void ToggleToolStripItemVisibilityByKey( string key )
		{
			object sender = GetItemByKey(key);

			if (sender as RadToolStripItem != null)
			{
				RadToolStripItem item = sender as RadToolStripItem;

				if (item.Visibility == ElementVisibility.Collapsed)
				{
					item.Visibility = ElementVisibility.Visible;
					item.Margin = new Padding(0, 0, 0, 0);
					item.InvalidateLayout();

					for (int i = 0; i < this.elementList.Count; i++)
					{
						RadToolStripElement element = this.elementList[i] as RadToolStripElement;
						if (element != null && element.Items.Contains(item))
						{
							element.Orientation = this.Orientation;
							this.Items.Add(element);
							this.elementList.Remove(element);
							break;
						}
					}
				}
				else
				{
					item.Visibility = ElementVisibility.Collapsed;

					item.Margin = new Padding(0, 0, 0, 0);
					item.InvalidateLayout();


					foreach (RadToolStripElement element in this.Items)
					{
						if (element.Items.Contains(item))
						{
							if (VisibleItemsOnRow(element) == 0)
							{
								RadToolStripElement myElement = new RadToolStripElement();
								foreach (RadToolStripItem currentItem in element.Items)
								{
									currentItem.ParentToolStripElement = myElement;
									currentItem.Grip.ParentToolStripElement = myElement;
									myElement.Items.Add(currentItem);
								}

								this.elementList.Add(myElement);

								this.Items.Remove(element);
							}
							break;
						}

					}
				}
			}
			else
			{

				if ((sender as FloatingForm).Visible)
				{
					(sender as FloatingForm).Visible = false;

				}
				else
				{
					(sender as FloatingForm).Visible = true;

				}
			}	
		}


	
		static RadToolStripManager()
		{
            new Themes.ControlDefault.ToolStrip().DeserializeTheme();
		}

		/// <summary>
		///		Occurs when the changes the orientation of the ToolStrip.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the orientation of the ToolStrip is changed.")]
		public event ToolStripOrientationEventHandler OrientationChanged;

		/// <summary>
		///		Occurs when the user dragged a toolstrip to another row.
		/// </summary>
		[Category(RadDesignCategory.PropertyChangedCategory)]
		[Description("Occurs when the user dragged a toolstrip to another row.")]
		public event ToolStripChangeEventHandler RowChanged;

		/// <summary>
		///		Occurs when the user starts dragging a toolstrip item. The event occurs just before the appearance of the dragged item as a floating element.
		/// </summary>
		[Category(RadDesignCategory.DragDropCategory)]
		[Description("Occurs when the user starts dragging a toolstrip item. The event occurs just before the appearance of the dragged item as a floating element.")]
		public event ToolStripDragEventHandler DragStarting;

		/// <summary>
		///		Occurs when the user starts dragging a toolstrip item. The event occurs just after the appearance of the dragged item as a floating element.
		/// </summary>
		[Category(RadDesignCategory.DragDropCategory)]
		[Description("Occurs when the user starts dragging a toolstrip item. The event occurs just after the appearance of the dragged item as a floating element.")]
		public event ToolStripDragEventHandler DragStarted;


		/// <summary>
		///		Occurs when the user ends dragging a toolstrip item. The event occurs just before the toolstrip item is positioned on its new place.
		/// </summary>
		[Category(RadDesignCategory.DragDropCategory)]
		[Description("Occurs when the user ends dragging a toolstrip item. The event occurs just before the toolstrip item is positioned on its new place.")]
		public event ToolStripDragEventHandler DragEnding;


		/// <summary>
		///		Occurs when the user ends dragging a toolstrip item. The event occurs just after the toolstrip item is positioned on its new place.
		/// </summary>
		[Category(RadDesignCategory.DragDropCategory)]
		[Description("Occurs when the user ends dragging a toolstrip item. The event occurs just after the toolstrip item is positioned on its new place.")]
		public event ToolStripDragEventHandler DragEnded;

		protected virtual void OnOrientationChanged(ToolStripOrientationEventArgs args)
		{
			if (this.OrientationChanged != null)
			{
				this.OrientationChanged(this, args);
			}
		}

		protected virtual void OnRowChanged(ToolStripChangedEventArgs args)
		{
			if (this.RowChanged != null)
			{
				this.RowChanged(this, args);
			}
		}

		protected virtual void OnDragStarting(ToolStripDragEventArgs args)
		{
			if (this.DragStarting != null)
			{
				this.DragStarting(this, args);
			}
		}

		protected virtual void OnDragStarted(ToolStripDragEventArgs args)
		{

			if (this.DragStarted != null)
			{
				this.DragStarted(this, args);
			}
		}

		protected virtual void OnDragEnding(ToolStripDragEventArgs args)
		{
			if (this.DragEnding != null)
			{
				this.DragEnding(this, args);
			}
		}

		protected virtual void OnDragEnded(ToolStripDragEventArgs args)
		{
			if (this.DragEnded != null)
			{
				this.DragEnded(this, args);
			}
		}
		/// <summary>
		/// Gets the customize dialog form. 
		/// </summary>
		[Browsable(false)]
		[Description("Get the Customize Dialog Form")]
		public ToolStripDialogForm OverFlowDialog
		{
			get
			{
				return this.DialogForm;
			}
		}

		internal RootRadElement RootElement
		{
			get
			{
				return this.rootElement;
			}
			set
			{
				this.rootElement = value;
			}
		}

        /// <summary>
        /// Gets or sets the docking sizes. 
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ArrayList DockingSites
		{
			get
			{
				return this.dockingSites;
			}
			set
			{
				this.dockingSites = value;
			}
		}

        private void items_ItemsChanged(RadItemCollection changed, RadItem tartet, ItemsChangeOperation operation)
		{
            if (operation == ItemsChangeOperation.Inserted)
            {
                RadToolStripElement element = tartet as RadToolStripElement;
                if (element != null)
                    element.Orientation = this.Orientation;
            }
		}

		private Size GetRealToolStripItemSize(RadToolStripItem toolStripItem)
		{
			Size sz = Size.Empty;

			if (toolStripItem != null)
			{
				foreach (RadItem item in toolStripItem.Items)
				{
					if (sz.Height < item.FullBoundingRectangle.Height)
						sz.Height = item.FullBoundingRectangle.Height;

					if (sz.Width < item.FullBoundingRectangle.Width)
						sz.Width = item.FullBoundingRectangle.Width;
				}
				sz.Height = Math.Max(sz.Height, toolStripItem.MinSize.Height);
				sz.Height += toolStripItem.Margin.Bottom + toolStripItem.Margin.Top;

				sz.Width = Math.Max(sz.Width, toolStripItem.MinSize.Width);
				sz.Width += toolStripItem.Margin.Left + toolStripItem.Margin.Right;
				if (toolStripItem.HasBorder())
				{
					sz.Height += 2;
					sz.Width += 2;
				}
			}


			return sz;
		}

		private int GetElementPreferredWidth(RadToolStripElement element)
		{
			int width = 0;
			int valueToReturn = 0;

			foreach (RadToolStripItem item in element.Items)
			{
				if (width < GetRealToolStripItemSize(item).Width)
					width = GetRealToolStripItemSize(item).Width;
			}

			valueToReturn = width + element.Margin.Left + element.Margin.Right;
			//		if (element.HasBorder()) valueToReturn += 2;
			return Math.Max((element.MinSize.Width + element.Margin.Left + element.Margin.Right), valueToReturn);
		}

		private int GetElementPreferredHeight(RadToolStripElement element)
		{
			int height = 0;
			int valueToReturn = 0;

			foreach (RadToolStripItem item in element.Items)
			{
				if (height < GetRealToolStripItemSize(item).Height)
					height = GetRealToolStripItemSize(item).Height;
			}

			valueToReturn = height + element.Margin.Top + element.Margin.Bottom;
			if (element.HasBorder()) valueToReturn += 2;
			return Math.Max((element.MinSize.Height + element.Margin.Top + element.Margin.Bottom), valueToReturn);
		}

		#endregion

		#region Properties

		public static RadProperty ToolStripOrientationProperty = RadProperty.Register(
		"Orientation", typeof(Orientation), typeof(RadToolStripManager), new RadElementPropertyMetadata(
		Orientation.Horizontal, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty AllowDraggingProperty = RadProperty.Register(
		"AllowDragging", typeof(bool), typeof(RadToolStripManager), new RadElementPropertyMetadata(
		false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty AllowFloatingProperty = RadProperty.Register(
		"AllowFloating", typeof(bool), typeof(RadToolStripManager), new RadElementPropertyMetadata(
		false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty FadeDelayProperty = RadProperty.Register(
		"FadeDelay", typeof(int), typeof(RadToolStripManager), new RadElementPropertyMetadata(
		0, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		public static RadProperty ShowOverflowButtonProperty = RadProperty.Register(
		"ShowOverflowButton", typeof(bool), typeof(RadToolStripManager), new RadElementPropertyMetadata(
		true, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public StripLayoutPanel VerticalLayout
		{
			get
			{
				return this.verticalLayout;
			}
		}

		/// <summary>
		/// Gets or Sets the FadeDelay of the Floating ToolStrips
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
		[RadPropertyDefaultValue("FadeDelay", typeof(RadToolStripManager))]
		[Description("Gets or Sets the FadeDelay of the Floating ToolStrips")]
		public int FadeDelay
		{
			get
			{
				return (int)this.GetValue(FadeDelayProperty);
			}
			set
			{
				this.SetValue(FadeDelayProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets the tool strip orientation.
		/// </summary>
        [Category(RadDesignCategory.LayoutCategory)]
        [RadPropertyDefaultValue("Orientation", typeof(RadToolStripManager))]
        [Description("Orientation of the element - could be horizontal or vertical")]
        public Orientation Orientation
		{
			get
			{
				return (Orientation)this.GetValue(ToolStripOrientationProperty);
			}
			set
			{
				this.SetValue(ToolStripOrientationProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets whether ToolStripItems support floating mode 
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
        [RadPropertyDefaultValue("AllowFloating", typeof(RadToolStripManager))]
        [Description("Gets or sets whether ToolStripItems support floating mode")]
        public bool AllowFloating
		{
			get
			{
				return (bool)this.GetValue(AllowFloatingProperty);
			}
			set
			{
				this.SetValue(AllowFloatingProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets whether ToolStripItems can change their location.
		/// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadPropertyDefaultValue("AllowDragging", typeof(RadToolStripManager))]
        [Description("Gets or sets whether ToolStripItems can change their location.")]
        public bool AllowDragging
		{
			get
			{
				return (bool)this.GetValue(AllowDraggingProperty);
			}
			set
			{
				this.SetValue(AllowDraggingProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets whether OverFlowButton should be visible.
		/// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadPropertyDefaultValue("ShowOverFlowButton", typeof(RadToolStripManager))]
        [Description("Gets or sets whether OverFlowButton should be visible.")]
        public bool ShowOverFlowButton
		{
			get
			{
				return (bool)this.GetValue(ShowOverflowButtonProperty);
			}
			set
			{
				this.SetValue(ShowOverflowButtonProperty, value);
			}
		}

		/// <summary>Gets the <see cref="RadToolStripElement">elements</see> in the toolstrip.</summary>
		[Browsable(true), Category(RadDesignCategory.DataCategory)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RadItemOwnerCollection Items
		{
			get
			{
				return this.items;
			}
		}
		#endregion

		#region PublicMethods
		/// <summary>Notifies that bounds is changed.</summary>
		public void CallBoundsChanged()
		{
			this.OnBoundsChanged(RadPropertyChangedEventArgs.Empty);
		}
		#endregion

		#region PrivateMethods
		private int GetTotalWidthOfItems(RadToolStripItem item)
		{
			if (item.Visibility == ElementVisibility.Collapsed)
				return 0;

			RadToolStripElement _element = null;
			int width = -1;

			foreach (RadToolStripElement element in this.Items)
			{
				if (element.Items.Contains(item))
					_element = element;
			}

			if (_element != null)
				foreach (RadToolStripItem _item in _element.Items)
				{
					if (_item.Visibility == ElementVisibility.Visible)
						width += _item.FullBoundingRectangle.Width;
				}

			return width;

		}

		private int GetTotalHeightOfItems(RadToolStripItem item)
		{
			RadToolStripElement _element = null;
			int height = -1;

			foreach (RadToolStripElement element in this.Items)
			{
				if (element.Items.Contains(item))
					_element = element;
			}

			if (_element != null)
				foreach (RadToolStripItem _item in _element.Items)
				{
					if (_item.Visibility == ElementVisibility.Visible)
						height += _item.FullBoundingRectangle.Height;
				}

			return height;

		}

		private Stack<RadElement> GetItemsFromToolStripItems(RadToolStripItem toolStripItem)
		{
			Stack<RadElement> items = new Stack<RadElement>();
			for (int i = 0; i < toolStripItem.Items.Count; i++)
			{
				items.Push(toolStripItem.Items[i]);
			}
			return items;
		}

		private Stack<RadToolStripItem> GetToolStripItems()
		{
			Stack<RadToolStripItem> items = new Stack<RadToolStripItem>();

			int i = 0;

			foreach (RadToolStripElement element in this.items)
			{
				foreach (RadToolStripItem item in element.Items)
				{
					if (item.Visibility == ElementVisibility.Visible)
						items.Push(item);
				}
				i++;
			}
			return items;
		}
		#endregion

		#region OverridenMethods

        public override bool AffectsInnerLayout
        {
            get
            {
                return true;
            }
        }

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			if (e.Property == ShowOverflowButtonProperty)
			{
				bool value = (bool)e.NewValue;

				ElementVisibility visible;
				if ( value )
					visible = ElementVisibility.Visible;
				else
					visible = ElementVisibility.Hidden;

				foreach (RadToolStripElement toolStripElement in this.Items)
				{
					foreach (RadToolStripItem item in toolStripElement.Items)
					{
						item.OverflowManager.DropDownButton.Visibility = visible;
					}
				}
			}

			if (e.Property == ToolStripOrientationProperty)
			{
                Orientation newOrientation = (Orientation)e.NewValue;
				OnOrientationChanged(new ToolStripOrientationEventArgs((Orientation)e.OldValue, newOrientation));
                
                if (newOrientation == Orientation.Horizontal)
				{
					this.verticalLayout.Orientation = Orientation.Vertical;
                    this.verticalLayout.EqualChildrenHeight = false;
                    this.verticalLayout.EqualChildrenWidth = true;
                    for (int i = 0; i < this.Items.Count; i++)
					{
                        RadToolStripElement toolStripElement = this.Items[i] as RadToolStripElement;
                        if (toolStripElement != null)
						    toolStripElement.Orientation = Orientation.Horizontal;
						foreach (RadElement element in toolStripElement.Items)
						{
							element.SetValue(ToolStripOrientationProperty, Orientation.Horizontal);
							foreach (RadElement fillElement in element.Children)
							{
								fillElement.SetValue(ToolStripOrientationProperty, Orientation.Horizontal);
							}
						
						}
					}
				}
				else
				{
                    this.verticalLayout.Orientation = Orientation.Horizontal;
					this.verticalLayout.EqualChildrenWidth = false;
					this.verticalLayout.EqualChildrenHeight = true;
                    for (int i = 0; i < this.Items.Count; i++)
                    {
                        RadToolStripElement toolStripElement = this.Items[i] as RadToolStripElement;
                        if (toolStripElement != null)
                            toolStripElement.Orientation = Orientation.Vertical;
						foreach (RadElement element in toolStripElement.Items)
						{
							element.SetValue(ToolStripOrientationProperty, Orientation.Vertical);
							foreach (RadElement fillElement in element.Children)
							{
								fillElement.SetValue(ToolStripOrientationProperty, Orientation.Vertical);
							}
						}
					}
				}
			}

			base.OnPropertyChanged(e);
		}

		public override Size GetPreferredSizeCore(Size proposedSize)
		{
			Size res = base.GetPreferredSizeCore(proposedSize);
            if (this.Orientation == Orientation.Horizontal)
                res.Width = proposedSize.Width;
            else
                res.Height = proposedSize.Height;
            return res;
		}

		/// <commentsfrom cref="ILayoutEngine.PerformLayoutCore" filter=""/>
		public override void PerformLayoutCore(RadElement affectedElement)
		{
			base.PerformLayoutCore(affectedElement);

            if (this.Orientation == Orientation.Horizontal)
                this.verticalLayout.Size = new Size(this.Parent.FieldSize.Width, this.verticalLayout.Size.Height);
            else
                this.verticalLayout.Size = new Size(this.verticalLayout.Size.Width, this.Parent.FieldSize.Height);
		}

		protected override void CreateChildElements()
		{
			this.verticalLayout = new StripLayoutPanel();
			this.verticalLayout.Orientation = Orientation.Vertical;
			this.verticalLayout.EqualChildrenWidth = true;
            this.verticalLayout.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			this.verticalLayout.Class = "ToolStripLayout";

			this.toolStripFill = new FillPrimitive();
			this.toolStripFill.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
			this.toolStripFill.Class = "Fill";

			this.items.Owner = verticalLayout;
			this.Children.Add(toolStripFill);
			this.Children.Add(verticalLayout);
		}

        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
		{
			base.OnBubbleEvent(sender, args);

			if (args.RoutedEvent == RadToolStripGripElement.DragStartingEvent)
			{
				ToolStripDragEventArgs dragArgs = (ToolStripDragEventArgs)args.OriginalEventArgs;
				OnDragStarting(dragArgs);
				args.Canceled = dragArgs.Cancel;
			}
			else if (args.RoutedEvent == RadToolStripGripElement.DragStartedEvent)
			{
				ToolStripDragEventArgs dragArgs = (ToolStripDragEventArgs)args.OriginalEventArgs;
				OnDragStarted(dragArgs);
				args.Canceled = dragArgs.Cancel;
			}
			else if (args.RoutedEvent == RadToolStripGripElement.DragEndingEvent)
			{
				ToolStripDragEventArgs dragArgs = (ToolStripDragEventArgs)args.OriginalEventArgs;
				OnDragEnding(dragArgs);
				args.Canceled = dragArgs.Cancel;
			}
			else if (args.RoutedEvent == RadToolStripGripElement.DragEndedEvent)
			{
				ToolStripDragEventArgs dragArgs = (ToolStripDragEventArgs)args.OriginalEventArgs;
				OnDragEnded(dragArgs);
				args.Canceled = dragArgs.Cancel;
			}
			else if (args.RoutedEvent == RadToolStripGripElement.RowChangedEvent)
			{
				OnRowChanged((ToolStripChangedEventArgs)args.OriginalEventArgs);
			}
		}

		#endregion
	}
}
