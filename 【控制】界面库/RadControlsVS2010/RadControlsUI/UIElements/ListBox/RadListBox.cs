using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using Telerik.WinControls.UI.Design;
using Telerik.WinControls.UI.UIElements.ListBox;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a list box. The RadListBox class is a simple wrapper for the
    ///     <see cref="RadListBoxElement">RadListBoxElement</see> class. The latter implements
    ///     all the UI and logic functionality. RadListBox act to transfer events to and from
    ///     its <see cref="RadListBoxElement">RadListBoxElement</see> instance. 
    ///     <see cref="RadListBoxElement">RadListBoxElement</see> can be nested in other
    ///     telerik controls.
    /// </summary>
	[RadThemeDesignerData(typeof(RadListBoxDesignTimeData))]
	[LookupBindingProperties("DataSource", "DisplayMember", "ValueMember", "SelectedValue")]
	[Description("Displays a collection of items")]
	[DefaultBindingProperty("Items"), DefaultEvent("SelectedIndexChanged"), DefaultProperty("Items")]
    [Docking(DockingBehavior.Ask)]
	[ToolboxItem(false)]
    [Obsolete("This control is obsolete, Use RadListControl instead.")]
	public class RadListBox : RadControl
    {
		private static readonly object SelectedIndexChangedEventKey;
        private static readonly object SelectedItemChangedEventKey;
		private static readonly object SortedChangedEventKey;
		private static readonly object SelectedValueChangedEventKey;
		private static readonly object ItemDataBoundEventKey;
        private static readonly object ItemDataBindingEventKey;

		private RadListBoxElement listBoxElement;

		static RadListBox()
		{			
			SelectedIndexChangedEventKey = new object();
			SortedChangedEventKey = new object();
			SelectedValueChangedEventKey = new object();
			ItemDataBoundEventKey = new object();
            ItemDataBindingEventKey = new object();
            SelectedItemChangedEventKey = new object();
		}

        public RadListBox()
        {
            // PATCH - for double click in design-time
            Size sz = this.DefaultSize;
            this.ElementTree.PerformInnerLayout(true, 0, 0, sz.Width, sz.Height);
        }

		#region Properties

        /// <summary>
        /// Gets the instance of RadListBoxElement wrapped by this control. RadListBoxElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadListBox.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadListBoxElement ListBoxElement
        {
            get
            {
                return this.listBoxElement;
            }
        }

        /// <summary>
        /// Enables or disables the ReadOnly mode of RadListBox. The default value is false.
        /// </summary>
        [Description("Enables or disables the ReadOnly mode of RadListBox. The default value is false.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Obsolete("This property should not be used anymore. To disable the selection, the SelectionMode property must be set to None.")] // Marked obsolete for the Q3 2009 release.
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool ReadOnly
        {
            get
            {
                return this.listBoxElement.ReadOnly;
            }
            set
            {
                this.listBoxElement.ReadOnly = value;
            }
        }
		
		/// <commentsfrom cref="RadListBoxElement.DataSource" filter=""/>
		[RadDefaultValue("DataSource", typeof(RadListBoxElement)),
		AttributeProvider(typeof(IListSource)),
		RadDescription("DataSource", typeof(RadListBoxElement)),
		Category(RadDesignCategory.DataCategory),
		RefreshProperties(RefreshProperties.Repaint)]
		public object DataSource
		{
			get
			{
				return this.listBoxElement.DataSource;
			}
			set
			{
				this.listBoxElement.DataSource = value;
			}
		}

		/// <commentsfrom cref="RadListBoxElement.DisplayMember" filter=""/>
		[RadDefaultValue("DisplayMember", typeof(RadListBoxElement)),
		TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
		RadDescription("DisplayMember", typeof(RadListBoxElement)),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
		Category(RadDesignCategory.DataCategory)]
		public string DisplayMember
		{
			get
			{
				return this.listBoxElement.DisplayMember;
			}
			set
			{
				this.listBoxElement.DisplayMember = value;
			}
		}

		/// <commentsfrom cref="RadListBoxElement.FormatInfo" filter=""/>
		[Browsable(false),
		RadDescription("FormatInfo", typeof(RadListBoxElement)),		
		EditorBrowsable(EditorBrowsableState.Advanced),
		RadDefaultValue("FormatInfo", typeof(RadListBoxElement))]
		public IFormatProvider FormatInfo
		{
			get
			{
				return this.listBoxElement.FormatInfo;
			}
			set
			{
				this.listBoxElement.FormatInfo = value;
			}
		}

		/// <commentsfrom cref="RadListBoxElement.FormatString" filter=""/>
		[RadDefaultValue("FormatString", typeof(RadListBoxElement)),
	    Editor(typeof(FormatStringEditor), typeof(UITypeEditor)),
		MergableProperty(false),
		RadDescription("FormatString", typeof(RadListBoxElement))]
		public string FormatString
		{
			get
			{
				return this.listBoxElement.FormatString;
			}
			set
			{
				this.listBoxElement.FormatString = value;
			}
		}

		/// <commentsfrom cref="RadListBoxElement.FormattingEnabled" filter=""/>
		[RadDescription("FormattingEnabled", typeof(RadListBoxElement)),
		RadDefaultValue("FormattingEnabled", typeof(RadListBoxElement))]
		public bool FormattingEnabled
		{
			get
			{
				return this.listBoxElement.FormattingEnabled;
			}
			set
			{
				this.listBoxElement.FormattingEnabled = value;
			}
		}

        /// <commentsfrom cref="RadListBoxElement.IntegralHeight" filter=""/>
        [Category(RadDesignCategory.BehaviorCategory),
        RadDescription("IntegralHeight", typeof(RadListBoxElement)),
        RadDefaultValue("IntegralHeight", typeof(RadListBoxElement))]
        public bool IntegralHeight
        {
            get
            {
                return this.listBoxElement.IntegralHeight;
            }
            set
            {
                this.listBoxElement.IntegralHeight = value;
            }
        }

		/// <commentsfrom cref="RadListBoxElement.Items" filter=""/>
        [RadEditItemsAction]
		[RadNewItem("Click to add new item", false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Category(RadDesignCategory.DataCategory)]
        [RadDescription("Items", typeof(RadListBoxElement))]
        public RadItemCollection Items
        {
            get
            {
                return listBoxElement.Items;
            }
        }
		
		/// <commentsfrom cref="RadListBoxElement.SelectedIndex" filter=""/>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		RadDescription("SelectedIndex", typeof(RadListBoxElement))]
		public int SelectedIndex
		{
			get
			{
				return this.listBoxElement.SelectedIndex;
			}
			set
			{
				this.listBoxElement.SelectedIndex = value;
			}
		}

        /// <summary>
        /// Returns the text of the currently selected item.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false),
        RadDescription("SelectedText", typeof(RadListBoxElement))]
        public string SelectedText
        {
            get
            {
                return this.listBoxElement.SelectedText;
            }
        }

        ///// <commentsfrom cref="RadListBoxElement.SelectedIndices" filter=""/>
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        //RadDescription("SelectedIndices", typeof(RadListBoxElement)),
        //Browsable(false)]
        //public RadListBoxItemIndexCollection SelectedIndices
        //{
        //    get
        //    {
        //        return this.listBoxElement.SelectedIndices;
        //    }
		//}

		/// <commentsfrom cref="RadListBoxElement.SelectedItem" filter=""/>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RadDescription("SelectedItem", typeof(RadListBoxElement)),
		Browsable(false),
		Bindable(true)]
		public Object SelectedItem
		{
			get
			{
				return this.listBoxElement.SelectedItem;
			}
			set
			{
				this.listBoxElement.SelectedItem = value;
                if (value is RadItem)
                {
                    this.listBoxElement.SetActiveItem((RadItem)value);
                }
			}
		}

		/// <commentsfrom cref="RadListBoxElement.SelectedItems" filter=""/>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RadDescription("SelectedItems", typeof(RadListBoxElement)),
		Browsable(false)]
		public RadListBoxItemCollection SelectedItems
		{
			get
			{
				return this.listBoxElement.SelectedItems;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Description("Gets or sets value specifying the currently selected item."),
		Browsable(false),
		Bindable(true)]
		public Object SelectedValue
		{
			get
			{
				return this.listBoxElement.SelectedValue;
			}
			set
			{
				this.listBoxElement.SelectedValue = value;
			}
		}

		/// <commentsfrom cref="RadListBoxElement.SelectionMode" filter=""/>
		[Category(RadDesignCategory.BehaviorCategory),
		RadDescription("SelectionMode", typeof(RadListBoxElement)),
		RadDefaultValue("SelectionMode", typeof(RadListBoxElement))]
		public SelectionMode SelectionMode
		{
			get
			{
                return this.listBoxElement.SelectionMode;
			}
			set
			{
                this.listBoxElement.SelectionMode = value;
			}
		}

		/// <commentsfrom cref="RadListBoxElement.SortItems" filter=""/>
		[Category(RadDesignCategory.BehaviorCategory),
		RadDefaultValue("SortItems", typeof(RadListBoxElement)),
	    RadDescription("SortItems", typeof(RadListBoxElement))]
		public SortStyle Sorted
		{
			get
			{
				return this.listBoxElement.SortItems;
			}
			set
			{
				this.listBoxElement.SortItems = value;
			}
		}

		/// <commentsfrom cref="RadListBoxElement.Text" filter=""/>
		public override string Text
		{
			get
			{
				return this.listBoxElement.Text;
			}
			set
			{
				this.listBoxElement.Text = value;				
			}
		}

		/// <commentsfrom cref="RadListBoxElement.ValueMember" filter=""/>
		[RadDescription("ValueMember", typeof(RadListBoxElement)),
		Category(RadDesignCategory.DataCategory),
		RadDefaultValue("ValueMember", typeof(RadListBoxElement)),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		public string ValueMember
		{
			get
			{
				return this.listBoxElement.ValueMember;
			}
			set
			{
				this.listBoxElement.ValueMember = value;
			}
		}

		/// <commentsfrom cref="RadListBoxElement.Virtualized" filter=""/>
		[Browsable(true)]
        [RadDefaultValue("Virtualized", typeof(RadListBoxElement))]
		[Category(RadDesignCategory.BehaviorCategory)]
		[Description("Gets or sets a value indicating whether RadScrollViewer uses UI virtualization.")]
		public bool Virtualized
		{
			get
			{
				return this.listBoxElement.Virtualized;
			}
			set
			{
				this.listBoxElement.Virtualized = value;				
			}
		}
		#endregion

		protected override Size DefaultSize
		{
			get
			{
				return new Size(120, 90);
			}
		}
						
		private void listBoxElement_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.OnSelectedIndexChanged(e);
		}

        private void listBoxElement_SelectedItemChanged(object sender, RadListBoxSelectionChangeEventArgs e)
        {
            this.OnSelectedItemChanged(e);
        }
		
		private void listBoxElement_SortItemsChanged(object sender, EventArgs e)
		{
			this.OnSortedChanged(e);
		}

		private void listBoxElement_SelectedValueChanged(object sender, EventArgs e)
		{
			this.OnSelectedValueChanged(e);
		}

		void listBoxElement_ItemDataBound(object sender, ItemDataBoundEventArgs e)
		{
			this.OnItemDataBound(e);
		}

        void listBoxElement_ItemDataBinding(object sender, ItemDataBindingEventArgs e)
        {
            this.OnItemDataBinding(e);
        }
				
		#region Events
		protected override bool IsInputKey(Keys keyData)
		{
			if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Left || keyData == Keys.Right)			
				return true;
			if (keyData == Keys.Shift)
				return false;
			if ((keyData == (Keys.Shift | Keys.Down)) || (keyData == (Keys.Shift | Keys.Up)))
				return true;
			return base.IsInputKey(keyData);
		}
				
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			this.listBoxElement.Focus();
		}

		/// <commentsfrom cref="RadListBoxElement.SelectedIndexChanged" filter=""/>
		[Browsable(true),
		Category(RadDesignCategory.BehaviorCategory),
		RadDescription("SelectedIndexChanged", typeof(RadListBoxElement))]
		public event EventHandler SelectedIndexChanged
		{
			add
			{
				this.Events.AddHandler(RadListBox.SelectedIndexChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadListBox.SelectedIndexChangedEventKey, value);
			}
		}

		/// <commentsfrom cref="RadListBoxElement.OnSelectedIndexChanged" filter=""/>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnSelectedIndexChanged(EventArgs e)
		{
			EventHandler handler1 = this.Events[RadListBox.SelectedIndexChangedEventKey] as EventHandler;
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <commentsfrom cref="RadListBoxElement.SelectedValueChanged" filter=""/>
		[Browsable(true),
		Category("Property Changed"),
	    RadDescription("SelectedValueChanged", typeof(RadListBoxElement))]
		public event EventHandler SelectedValueChanged
		{
			add
			{
				this.Events.AddHandler(RadListBox.SelectedValueChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadListBox.SelectedValueChangedEventKey, value);
			}
		}

        [Browsable(true)]
        [Category("Property Changed")]
        [RadDescription("SelectedItemChanged", typeof(RadListBoxElement))]
        public event RadListBoxSelectionChangeEventHandler SelectedItemChanged
        {
            add
            {
                this.Events.AddHandler(RadListBox.SelectedItemChangedEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(RadListBox.SelectedItemChangedEventKey, value);
            }
        }

        protected virtual void OnSelectedItemChanged(RadListBoxSelectionChangeEventArgs e)
        {
            RadListBoxSelectionChangeEventHandler handler = this.Events[RadListBox.SelectedItemChangedEventKey] as RadListBoxSelectionChangeEventHandler;
            if (handler != null)
            {
                handler(this, new RadListBoxSelectionChangeEventArgs(this.listBoxElement.SelectedIndex, (RadItem)this.listBoxElement.SelectedItem));
            }
        }

		/// <commentsfrom cref="RadListBoxElement.OnSelectedValueChanged" filter=""/>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnSelectedValueChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadListBox.SelectedValueChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when the Sorted property has changed.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the Sorted property has changed.")]
		public event EventHandler SortedChanged
		{
			add
			{
				this.Events.AddHandler(RadListBox.SortedChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadListBox.SortedChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the SortedChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnSortedChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadListBox.SortedChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}
				
		/// <summary>
		/// Occurs when a data record is bound to an item.
		/// </summary>
		[Browsable(true),
		Category(RadDesignCategory.DataCategory),
	    Description("Occurs when a data record is bound to an item.")]
		public event ItemDataBoundEventHandler ItemDataBound
		{
			add
			{
				this.Events.AddHandler(RadListBox.ItemDataBoundEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadListBox.ItemDataBoundEventKey, value);
			}
		}

        /// <summary>
        /// Occurs when a data record is to be bound to an item.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.DataCategory),
        Description("Occurs when a data record is to be bound to an item.")]
        public event ItemDataBindingEventHandler ItemDataBinding
        {
            add
            {
                this.Events.AddHandler(RadListBox.ItemDataBindingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadListBox.ItemDataBindingEventKey, value);
            }
        }

		/// <summary>
		/// Raises the ItemDataBound event.
		/// </summary>
		protected virtual void OnItemDataBound(ItemDataBoundEventArgs e)
		{			
			ItemDataBoundEventHandler handler1 = (ItemDataBoundEventHandler)this.Events[RadListBox.ItemDataBoundEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

        /// <summary>
        /// Raises the ItemDataBinding event.
        /// </summary>
        protected virtual void OnItemDataBinding(ItemDataBindingEventArgs e)
        {
            ItemDataBindingEventHandler handler1 = (ItemDataBindingEventHandler)this.Events[RadListBox.ItemDataBindingEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

		#endregion

		#region Public methods

		/// <commentsfrom cref="RadListBoxElement.BeginUpdate" filter=""/>
		public void BeginUpdate()
		{
			this.listBoxElement.BeginUpdate();
		}
		
		/// <commentsfrom cref="RadListBoxElement.ClearSelected" filter=""/>
		public void ClearSelected()
		{
			this.listBoxElement.ClearSelected();
		}

		/// <commentsfrom cref="RadListBoxElement.EndUpdate" filter=""/>
		public void EndUpdate()
		{
			this.listBoxElement.EndUpdate();
		}

		/// <commentsfrom cref="RadListBoxElement.FindItem" filter=""/>
		public RadListBoxItem FindItem(string startsWith)
		{
            return this.listBoxElement.FindItem(startsWith) as RadListBoxItem;
		}

		/// <commentsfrom cref="RadListBoxElement.FindItemExact" filter=""/>
		public RadListBoxItem FindItemExact(string text)
		{
            return this.listBoxElement.FindItemExact(text) as RadListBoxItem;
		}

		/// <commentsfrom cref="RadListBoxElement.GetItemHeight" filter=""/>
		public int GetItemHeight(int index)
		{
			return this.listBoxElement.GetItemHeight(index);
		}

		public string GetItemText(int index)
		{
			return this.listBoxElement.Items[index].Text;
		}

        public void SelectAll()
        {
            this.listBoxElement.SelectAll();
        }

		[Obsolete("This method is obsolete and will be removed in the next release.")] // Skarlatov 25/09/2009
		public virtual string GetItemText(object item)
		{
			return item.ToString();
		}

        /// <commentsfrom cref="RadListBoxElement.SetSelected" filter=""/>
        public void SetSelected(int index, bool value)
        {
            this.listBoxElement.SetSelected(index, value);
        }
		#endregion

		public override void EndInit()
		{
			base.EndInit();			
			this.listBoxElement.EndInit();			
		}

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.A && Control.ModifierKeys == Keys.Control)
            {
                this.listBoxElement.SelectAll();
            }
        }

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			//TODO: handle scrolling here
            if (e.Delta > 0)
            {
                //// CECO_08.04.2009: MouseWheel fails when there are collapsed items
                if (this.listBoxElement != null &&
                    this.listBoxElement.UsePhysicalScrolling == false &&
                    this.listBoxElement.Virtualized == true &&
                    this.listBoxElement.Viewport is IRadScrollViewport &&
                    this.listBoxElement.VerticalScrollBar != null &&
                    this.listBoxElement.VerticalScrollBar.Value > 0)
                {
                    Point oldValue = new Point(0, this.listBoxElement.VerticalScrollBar.Value);
                    this.listBoxElement.VerticalScrollBar.Value--;
                    while (this.listBoxElement.VerticalScrollBar.Value > 0 &&
                        this.listBoxElement.Items[this.listBoxElement.VerticalScrollBar.Value].Visibility == ElementVisibility.Collapsed)
                    {
                        this.listBoxElement.VerticalScrollBar.Value--;
                    }
                    Point newValue = new Point(0, this.listBoxElement.VerticalScrollBar.Value);
                    ((IRadScrollViewport)this.listBoxElement.Viewport).DoScroll(oldValue, newValue);
                }
                else
                {
                    this.listBoxElement.LineUp();
                }
                //// END_FIX 
            }
            else if (e.Delta < 0)
            {
                //// CECO_08.04.2009: MouseWheel fails when there are collapsed items
                if (this.listBoxElement != null &&
                    this.listBoxElement.UsePhysicalScrolling == false &&
                    this.listBoxElement.Virtualized == true &&
                    this.listBoxElement.Viewport is IRadScrollViewport &&
                    this.listBoxElement.VerticalScrollBar != null &&
                    this.listBoxElement.VerticalScrollBar.Value < this.listBoxElement.VerticalScrollBar.Maximum)
                {
                    Point oldValue = new Point(0, this.listBoxElement.VerticalScrollBar.Value);
                    this.listBoxElement.VerticalScrollBar.Value++;
                    while (this.listBoxElement.VerticalScrollBar.Value < this.listBoxElement.VerticalScrollBar.Maximum &&
                        this.listBoxElement.Items[this.listBoxElement.VerticalScrollBar.Value].Visibility == ElementVisibility.Collapsed)
                    {
                        this.listBoxElement.VerticalScrollBar.Value++;
                    }
                    Point newValue = new Point(0, this.listBoxElement.VerticalScrollBar.Value);
                    ((IRadScrollViewport)this.listBoxElement.Viewport).DoScroll(oldValue, newValue);
                }
                else
                {
                    this.listBoxElement.LineDown();
                }
                //// END_FIX                
            }

			if (e is HandledMouseEventArgs && this.listBoxElement.CanVerticalScroll)
			{
				((HandledMouseEventArgs)e).Handled = true;
			}
		}

        protected override void CreateChildItems(RadElement parent)
        {
            this.listBoxElement = new RadListBoxElement();
            this.listBoxElement.ForceViewportWidth = true;
            this.listBoxElement.SelectedIndexChanged += new EventHandler(listBoxElement_SelectedIndexChanged);
            this.listBoxElement.SelectedItemChanged += new RadListBoxSelectionChangeEventHandler(listBoxElement_SelectedItemChanged);
            this.listBoxElement.SortItemsChanged += new EventHandler(listBoxElement_SortItemsChanged);
            this.listBoxElement.SelectedValueChanged += new EventHandler(listBoxElement_SelectedValueChanged);
			this.listBoxElement.ItemDataBound += new ItemDataBoundEventHandler(listBoxElement_ItemDataBound);
            this.listBoxElement.ItemDataBinding += new ItemDataBindingEventHandler(listBoxElement_ItemDataBinding);

            RootRadElement root = this.RootElement;

            this.listBoxElement.BindProperty(RadElement.MinSizeProperty, root, RadElement.MinSizeProperty, PropertyBindingOptions.OneWay);
            this.listBoxElement.BindProperty(RadElement.MaxSizeProperty, root, RadElement.MaxSizeProperty, PropertyBindingOptions.OneWay);
            this.listBoxElement.BindProperty(RadElement.AutoSizeProperty, root, RadElement.AutoSizeProperty, PropertyBindingOptions.OneWay);
            this.listBoxElement.BindProperty(RadElement.AutoSizeModeProperty, root, RadElement.AutoSizeModeProperty, PropertyBindingOptions.OneWay);

            this.RootElement.Children.Add(listBoxElement);
        }
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (this.listBoxElement != null)
            {
                this.listBoxElement.SelectedIndexChanged -= new EventHandler(listBoxElement_SelectedIndexChanged);
                this.listBoxElement.SortItemsChanged -= new EventHandler(listBoxElement_SortItemsChanged);
                this.listBoxElement.SelectedValueChanged -= new EventHandler(listBoxElement_SelectedValueChanged);
                this.listBoxElement.SelectedItemChanged -= listBoxElement_SelectedItemChanged;
            }
        }
	}

	public class RadListBoxDesignTimeData : RadControlDesignTimeData
	{
		public RadListBoxDesignTimeData()
		{ }

		public RadListBoxDesignTimeData(string name)
			: base(name)
		{ }

		public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
		{
			RadListBox listBoxStructure = new RadListBox();
            listBoxStructure.Virtualized = false;
			listBoxStructure.Size = new Size(200, 130);
            listBoxStructure.Items.Add(CreateSampleItem());

            RadListBox listBox = new RadListBox();
            listBox.Virtualized = false;
            listBox.Size = new Size(200, 130);
            listBox.Items.Add(CreateSampleItem());

			string itemText;					
			for(int i = 1; i <= 10; i++)
			{
				itemText = string.Format("Item{0}", i);
				listBox.Items.Add(new RadListBoxItem(itemText));
			}
			
			string longItemText = "Some text to make the horizontal scroll appear";
			listBox.Items.Add(new RadListBoxItem(longItemText));
			
			ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(listBox, listBoxStructure.RootElement);
			designed.MainElementClassName = typeof(RadListBoxElement).FullName;
			ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

			res.Add(designed);

			return res;
		}

        private RadListBoxItem CreateSampleItem()
        {
            RadListBoxItem res = new RadListBoxItem("Item with description");
            res.DescriptionText = "Description text which is longer..";
            res.TextSeparatorVisibility = ElementVisibility.Visible;
            return res;
        }
	}	
}
