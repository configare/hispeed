using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Windows.Forms.Design;
using System.ComponentModel;
using Telerik.WinControls.Enumerations;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using System.Collections;

namespace Telerik.WinControls.UI
{
	/// <summary>
	///     Represents a overflow manager used by the
	///     <see cref="RadToolStripItem">RadToolStripItem</see>.
	/// </summary>
	public class ToolStripItemsOverFlow
	{
		private RadItemOwnerCollection items;
		private RadToolStripItem toolStripItem;
		private RadToolStripOverFlowButtonElement dropDownButton;
		private RadToolStripElement lastParent;
		private RadToolStripManager lastManager;
		private ImageList imageList = new ImageList();
		private int visibleItemsCount;
		private Stack<RadItem> hiddenItems;
		internal ArrayList hiddenItemsList;
		internal ArrayList fakeItemsList;
		/// <summary>Gets the dropdown button of the ToolStripItem istance.</summary>
		public RadToolStripOverFlowButtonElement DropDownButton
		{
			get { return dropDownButton; }
		}

		public RadToolStripItem ToolStripItem
		{
			get
			{
				return this.toolStripItem;
			}
			set
			{
				this.toolStripItem = value;
			}
		}

		private bool managerChanged;

		public bool ManagerChanged
		{
			get
			{
				return this.managerChanged;
			}
			set
			{
				this.managerChanged = value;
			}
		}
		/// <summary>
		/// Initializes the overflow drop down button with the ToolStripItem owner of the
		/// ToolStripItemsOverFlow instance and the parent element.
		/// </summary>
        public void InitializeOverflowDropDown(RadToolStripItem toolStripItem)
		{
			this.items = toolStripItem.Items;
			this.toolStripItem = toolStripItem;
			this.dropDownButton = new RadToolStripOverFlowButtonElement();
			this.DropDownButton.Click += new EventHandler(DropDownButton_Click);
			this.DropDownButton.ZIndex = 10000;
			this.DropDownButton.SetValue(ToolStripItemLayout.IsOverFlowButtonElementProperty, true);
			this.DropDownButton.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            this.DropDownButton.Alignment = ContentAlignment.TopLeft;
            this.lastParent = this.toolStripItem.ParentToolStripElement;
			this.hiddenItems = new Stack<RadItem>();
			this.hiddenItemsList = new ArrayList();
			this.fakeItemsList = new ArrayList();
	
		}

		private void DropDownButton_Click(object sender, EventArgs e)
		{
			if (this.items.Count > 0)
			{
				PrepareDropDownItems();
			}
			this.dropDownButton.ShowDropDown();
	
		}

		private bool AnyItemVisibilityChanged()
		{
			int count = 0;
			foreach (RadToolStripItem item in this.toolStripItem.ParentToolStripElement.Items)
			{
				foreach (RadItem childItem in item.Items)
				{
					if (childItem.Visibility == ElementVisibility.Visible)
						count++;
				}
			}
			if (count != this.visibleItemsCount)
			{
				this.visibleItemsCount = 0;
				return true;
			}
			return false;

		}

		private RadItem CloneItem(RadItem item)
		{
			RadItem itemToReturn = null;

			if (item is RadButtonElement)
			{
				itemToReturn = new RadButtonElement();
				itemToReturn.Text = item.Text;

				if (this.imageList != null)
				{
					int index = (item as RadButtonElement).ImageIndex;
					if (index >= 0)
						(itemToReturn as RadButtonElement).Image = imageList.Images[index];
				}
				else
					(itemToReturn as RadButtonElement).Image = (item as RadButtonElement).Image;

			}


			if (item is RadDropDownButtonElement)
			{
				itemToReturn = new RadDropDownButtonElement();
				itemToReturn.Text = item.Text;

				if (this.imageList != null)
				{
					int index = (item as RadDropDownButtonElement).ImageIndex;
					if (index >= 0)
						(itemToReturn as RadDropDownButtonElement).Image = imageList.Images[index];
				}
				else
					(itemToReturn as RadDropDownButtonElement).Image = (item as RadDropDownButtonElement).Image;

			}

			return itemToReturn;
		}

		private RadItem CreateFakeItem(RadItem item)
		{

			if (item as RadButtonElement != null)
			{
				RadButtonElement element = new RadButtonElement();
				element.Text = item.Text;
				element.Font = item.Font;
				element.Image = (item as RadButtonElement).Image;
				element.ImageIndex = (item as RadButtonElement).ImageIndex;
				element.ImageKey = (item as RadButtonElement).ImageKey;

				return element;
			}

			if (item as RadComboBoxElement != null)
			{
				RadComboBoxElement element = new RadComboBoxElement();
                element.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
				element.Text = item.Text;
				element.Font = item.Font;
				return element;
			}

			if (item as RadTextBoxElement != null)
			{
				RadTextBoxElement element = new RadTextBoxElement();
                element.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
				element.Text = item.Text;
				element.Font = item.Font;
				return element;
			}

			if (item as RadDropDownButtonElement != null)
			{
				RadDropDownButtonElement element = new RadDropDownButtonElement();
				element.Text = item.Text;
				element.Image = (item as RadDropDownButtonElement).Image;
				element.ImageIndex = (item as RadDropDownButtonElement).ImageIndex;
				element.ImageKey = (item as RadDropDownButtonElement).ImageKey;
				return element;
			}

			return null;
		}

		internal void RefreshParentToolStrip()
		{		
				if (this.toolStripItem == null) return;
				ArrayList indexesToRemove = new ArrayList();

				for (int i = 0; i < this.toolStripItem.Items.Count; i++)
				{
					
					RadFakeElement element = this.toolStripItem.Items[i] as RadFakeElement;
					if ( element != null )
					if (element.Visibility == ElementVisibility.Visible)
					{
						int index = this.toolStripItem.Items.IndexOf(element as RadItem);
						
						if ( index != -1 )
							indexesToRemove.Add(index);
						
					}
				}

				for (int i = 0; i < indexesToRemove.Count; i++)
				{
					int indexToInsert = (int)indexesToRemove[i];

					RadFakeElement fake = this.toolStripItem.Items[indexToInsert] as RadFakeElement;
					if (fake == null)
						return;

					if (this.layout.Children.Contains(fake.AssociatedItem))
						this.layout.Children.Remove(fake.AssociatedItem);
					else
						return;

					if ( this.toolStripItem.Items.Count - 1 >= indexToInsert )
						this.toolStripItem.Items.RemoveAt(indexToInsert);

			
					this.toolStripItem.Items.Insert(indexToInsert, fake.AssociatedItem);
					
					this.fakeItemsList.Remove(fake);
				}
		}

        private Telerik.WinControls.Layouts.StackLayoutPanel layout;

		public class RadFakeElement : RadItem
		{
			private RadItem associatedItem;

			public RadFakeElement(RadItem associatedItem)
			{
				this.associatedItem = associatedItem;
                this.Text = this.associatedItem.Text;
			}

            protected override void InitializeFields()
            {
                base.InitializeFields();

                this.AutoSize = false;
            }

			public RadItem AssociatedItem
			{
				get
				{
					return this.associatedItem;
				}
			}
		}

		private ArrayList realItemsList;

		private void CreateStackOverflowBehavior()
		{
			this.layout = new Telerik.WinControls.Layouts.StackLayoutPanel();
			this.layout.Orientation = Orientation.Horizontal;
		
			RadMenuContentItem contentItem = new RadMenuContentItem();
			
			foreach ( RadItem item in this.items ) 
			{
				if (this.realItemsList == null)
					this.realItemsList = new ArrayList();

				if (item as RadFakeElement == null)
				{
					if ( item.Visibility != ElementVisibility.Collapsed )
					if (!this.realItemsList.Contains(item))
						this.realItemsList.Add(item);
				}
			}

			for (int i = 0; i < this.realItemsList.Count; i++)
			{
				RadItem item = (RadItem)this.realItemsList[i];
				if (item.Visibility != ElementVisibility.Collapsed)
				{
					if (item as RadFakeElement != null)
					{
						this.fakeItemsList.Add(item as RadFakeElement);
				    }


					if (item.Visibility == ElementVisibility.Hidden)
					{
						RadFakeElement element = new RadFakeElement(item);
						element.Bounds = item.Bounds;
						element.Margin = item.Margin;
						this.fakeItemsList.Add(element);

						this.toolStripItem.Items.Remove(item);
						this.toolStripItem.Items.Insert(i, element);
					}
				}
			}

			for (int i = 0; i < this.fakeItemsList.Count; i++)
			{
				RadFakeElement element = this.fakeItemsList[i] as RadFakeElement;

				if ( element.AssociatedItem.Visibility != ElementVisibility.Collapsed )
					element.AssociatedItem.Visibility = ElementVisibility.Visible;

				if (element.AssociatedItem as RadButtonElement != null)
				{
					(element.AssociatedItem as RadButtonElement).Padding = new Padding(3, 3, 3, 3);
				}

				this.layout.Children.Add(element.AssociatedItem);

			}

			if (layout.Children.Count > 0)
			{
				contentItem.ContentElement = layout;
				contentItem.ShouldHandleMouseInput = false;
				contentItem.MinSize = new Size(164, 0);
				this.DropDownButton.Items.Add(contentItem);
				contentItem.Margin = new Padding(0, 5, 5, 0);
				
				this.DropDownButton.Items.Add(new RadMenuSeparatorItem());
			}
		}


        private OverflowDropDownMenuItem resetItem;
        private OverflowDropDownMenuItem rootItem;
        private OverflowDropDownMenuItem CustomizeItem;

		private bool showRootItem = true;
		private bool showResetItem = true;
		private bool showCustomizeItem = true;

		[Browsable(false)]
		public bool ShowRootItem
		{
			get
			{
				return this.showRootItem;
			}
			set
			{
				this.showRootItem = value;
			}
		}

		[Browsable(false)]
		public bool ShowCustomizeItem
		{
			get
			{
				return this.showCustomizeItem;
			}
			set
			{
				this.showCustomizeItem = value;
			}
		}

		[Browsable(false)]
		public bool ShowResetItem
		{
			get
			{
				return this.showResetItem;
			}
			set
			{
				this.showResetItem = value;
			}
		}

	

			/// <summary>
		/// Initializes the drop down behavior of the drop down items.
		/// </summary>
		public void PrepareDropDownItems()
		{
            	this.lastParent = this.toolStripItem.ParentToolStripElement;
				this.lastManager = this.toolStripItem.ParentToolStripManager;

				this.DropDownButton.Items.Clear();
				if (this.toolStripItem.ElementTree != null)
                    this.imageList = this.toolStripItem.ElementTree.ComponentTreeHandler.ImageList;
				RadItem addOrRemoveItem = new RadItem();
                addOrRemoveItem.Text = RadToolStripLocalizationProvider.CurrentProvider.GetLocalizationString(
                        RadToolStripLocalizationStringId.AddorRemoveButtons);

				this.CreateStackOverflowBehavior();
				this.InvalidateAllToolStripItems();

				this.rootItem = new OverflowDropDownMenuItem(addOrRemoveItem, this.toolStripItem, this.imageList);
				rootItem.AssociatedItem = addOrRemoveItem;
				this.DropDownButton.Items.Add(rootItem);

				Graphics g = this.toolStripItem.ElementTree.Control.CreateGraphics();

				SizeF stringSize = g.MeasureString(addOrRemoveItem.Text, addOrRemoveItem.Font);
				this.layout.MinSize = new Size((int)stringSize.Width  , 0);
				this.layout.MaxSize = new Size( this.layout.MinSize.Width, 0 );
				g.Dispose();

				foreach (RadToolStripItem toolItem in this.toolStripItem.ParentToolStripElement.Items)
				{
					OverflowDropDownMenuItem corresponding = new OverflowDropDownMenuItem(toolItem, this.toolStripItem, this.imageList);
					corresponding.AssociatedItem = toolItem;

					foreach (RadItem item in toolItem.Items)
					{
						OverflowDropDownMenuItem correspondingItem = new OverflowDropDownMenuItem(item, toolItem, this.imageList);

						correspondingItem.AssociatedItem = item;
						correspondingItem.Click += new EventHandler(corresponding_Click);

						if (item.Visibility == ElementVisibility.Visible)
						{
							correspondingItem.IsChecked = true;
							this.visibleItemsCount++;
						}
						else
							correspondingItem.IsChecked = false;

						corresponding.Items.Add(correspondingItem);
					}

					corresponding.Items.Add(new RadMenuSeparatorItem());

					RadItem myItem = new RadItem();
                    myItem.Text = RadToolStripLocalizationProvider.CurrentProvider.GetLocalizationString(RadToolStripLocalizationStringId.ResetToolBar);

					resetItem = new OverflowDropDownMenuItem(myItem, this.toolStripItem, this.imageList);


					resetItem.AssociatedItem = myItem;
					resetItem.Click += new EventHandler(resetItem_Click);

					corresponding.Items.Add(resetItem);

					rootItem.Items.Add(corresponding);

				}

				RadItem customizeItem = new RadItem();
                customizeItem.Text = RadToolStripLocalizationProvider.CurrentProvider.GetLocalizationString(RadToolStripLocalizationStringId.Customize);

			    CustomizeItem = new OverflowDropDownMenuItem(customizeItem, this.toolStripItem, this.imageList);

				CustomizeItem.AssociatedItem = customizeItem;
				CustomizeItem.Click += new EventHandler(CustomizeItem_Click);

				rootItem.Items.Add(new RadMenuSeparatorItem());
				rootItem.Items.Add(CustomizeItem);

				if (!this.showRootItem)
					rootItem.Visibility = ElementVisibility.Collapsed;

				if (!this.showCustomizeItem)
					this.CustomizeItem.Visibility = ElementVisibility.Collapsed;

				if (!this.showResetItem)
					this.CustomizeItem.Visibility = ElementVisibility.Collapsed;
		}

		private void CustomizeItem_Click(object sender, EventArgs e)
		{	
			if (!this.toolStripItem.ParentToolStripManager.OverFlowDialog.Loaded())
					this.toolStripItem.ParentToolStripManager.OverFlowDialog.LoadDataInPanel();

				if (this.ManagerChanged)
				{
					this.toolStripItem.ParentToolStripManager.OverFlowDialog.CleadDataFromPanel();
					this.toolStripItem.ParentToolStripManager.OverFlowDialog.LoadDataInPanel();
					this.managerChanged = false;
				}

			this.toolStripItem.ParentToolStripManager.OverFlowDialog.ShowDialog();
		}

		private void resetItem_Click(object sender, EventArgs e)
		{
			if (sender as OverflowDropDownMenuItem != null)
			{
				OverflowDropDownMenuItem item = (OverflowDropDownMenuItem)(sender as OverflowDropDownMenuItem).HierarchyParent;

				foreach (RadItem childItem in (item.AssociatedItem as RadToolStripItem).Items)
				{
					if (this.toolStripItem.ParentToolStripManager.Orientation == Orientation.Vertical)
					{
						if (!((childItem is RadTextBoxElement) || (childItem is RadComboBoxElement)))

							childItem.Visibility = ElementVisibility.Visible;
					}
					else
						childItem.Visibility = ElementVisibility.Visible;

				}
				(item.AssociatedItem as RadToolStripItem).InvalidateLayout();
				for (int i = 0; i < item.Items.Count; i++)
				{
					OverflowDropDownMenuItem myItem = item.Items[i] as OverflowDropDownMenuItem;
					if (myItem != null)
						myItem.IsChecked = true;

				}

				(sender as OverflowDropDownMenuItem).IsChecked = false;
			}

			toolStripItem.LayoutEngine.SetLayoutInvalidated(true);
			toolStripItem.LayoutEngine.PerformParentLayout();
			InvalidateAllToolStripItems();

		}

		private void RefreshItems()
		{
			foreach (RadItem item in this.items)
			{
				if (item.Visibility == ElementVisibility.Collapsed) continue;

				if (item.FullBoundingRectangle.Right < this.toolStripItem.Bounds.Right)
					item.Visibility = ElementVisibility.Visible;
			}
		}

		private void InvalidateAllToolStripItems()
		{
			foreach (RadToolStripItem toolStripItem in this.toolStripItem.ParentToolStripElement.Items)
			{
				toolStripItem.InvalidateLayout();
			}
		}

		private Size GetRealParentSize()
		{
			Size sz = Size.Empty;

			if (this.toolStripItem.Visibility == ElementVisibility.Visible)
				foreach (RadItem childItem in this.toolStripItem.Items)
				{
					sz.Width += childItem.FullBoundingRectangle.Width;
					sz.Height += childItem.FullBoundingRectangle.Height;
				}

			return sz;
		}

		private void corresponding_Click(object sender, EventArgs e)
		{
			OverflowDropDownMenuItem dropDownItem = (OverflowDropDownMenuItem)sender;
			if (dropDownItem.AssociatedItem.Visibility == ElementVisibility.Visible)
			{
				dropDownItem.AssociatedItem.Visibility = ElementVisibility.Collapsed;
				if (dropDownItem.AssociatedItem as RadFakeElement != null)
					(dropDownItem.AssociatedItem as RadFakeElement).AssociatedItem.Visibility = ElementVisibility.Collapsed;
				dropDownItem.IsChecked = false;

			}
			else
			{
				if (this.toolStripItem.ParentToolStripManager.Orientation == Orientation.Horizontal)
				{
						dropDownItem.AssociatedItem.Visibility = ElementVisibility.Visible;
						
						if (dropDownItem.AssociatedItem as RadFakeElement != null)
							RefreshParentToolStrip();
						dropDownItem.IsChecked = true;
				}
				else
				{
					if (this.toolStripItem.ParentToolStripManager.Orientation == Orientation.Vertical)
					{
							
						if (!((dropDownItem.AssociatedItem is RadTextBoxElement) || (dropDownItem.AssociatedItem is RadComboBoxElement)))
							{

								if (dropDownItem.AssociatedItem as RadFakeElement != null)
									RefreshParentToolStrip();
					
								this.toolStripItem.InvalidateLayout();
								dropDownItem.AssociatedItem.Visibility = ElementVisibility.Visible;
								dropDownItem.IsChecked = true;
							}
						}
					}
            }
        

			RefreshItems();

			this.toolStripItem.LayoutEngine.SetLayoutInvalidated(true);
			this.toolStripItem.LayoutEngine.PerformParentLayout();
			InvalidateAllToolStripItems();
		}

		private class OverflowDropDownMenuItem : RadMenuItem
		{
            private RadItem associatedItem;

            public OverflowDropDownMenuItem() 
            {
            
            }

			public OverflowDropDownMenuItem(RadItem associatedItem, RadToolStripItem toolStripItem, ImageList imageList)
			{
				this.associatedItem = associatedItem;
			
				this.Text = associatedItem.Text;
                if (associatedItem as RadFakeElement != null)
                {
                    this.Text = (associatedItem as RadFakeElement).AssociatedItem.Text;
                }
				
				if ((associatedItem as RadButtonElement) != null) 
				{
					this.Image = (associatedItem as RadButtonElement).Image;
					this.ImageKey = (associatedItem as RadButtonElement).ImageKey;
					this.ImageIndex = (associatedItem as RadButtonElement).ImageIndex;
				}

				if ((associatedItem as RadDropDownButtonElement) != null)
				{
					this.Image = (associatedItem as RadDropDownButtonElement).Image;
					this.ImageKey = (associatedItem as RadDropDownButtonElement).ImageKey;
					this.ImageIndex = (associatedItem as RadDropDownButtonElement).ImageIndex;
				}

				if (associatedItem as RadFakeElement != null)
				{
					if ((associatedItem as RadFakeElement).AssociatedItem as RadButtonElement != null)
					{
						this.Image = ((associatedItem as RadFakeElement).AssociatedItem as RadButtonElement).Image;
						this.ImageIndex = ((associatedItem as RadFakeElement).AssociatedItem as RadButtonElement).ImageIndex;
						this.ImageKey = ((associatedItem as RadFakeElement).AssociatedItem as RadButtonElement).ImageKey;
					}

					if ((associatedItem as RadFakeElement).AssociatedItem as RadDropDownButtonElement != null)
					{
						this.Image = ((associatedItem as RadFakeElement).AssociatedItem as RadDropDownButtonElement).Image;
						this.ImageIndex = ((associatedItem as RadFakeElement).AssociatedItem as RadDropDownButtonElement).ImageIndex;
						this.ImageKey = ((associatedItem as RadFakeElement).AssociatedItem as RadDropDownButtonElement).ImageKey;
					}
				}
								
				if (associatedItem.Text == "")
				{   
                    
                    if (toolStripItem.Text != "")
                    {
                        this.Text = toolStripItem.Text;
                    }
                    else
                    {
                        this.Text = RadToolStripLocalizationProvider.CurrentProvider.GetLocalizationString(
                                RadToolStripLocalizationStringId.NoTextAssociated);
                    }
				}
			}
			/// <summary>
			/// Gets ot sets the item that is owner of the overflow manager.
			/// </summary>
			public RadItem AssociatedItem
			{
				get
				{
					return this.associatedItem;
				}
				set
				{
					this.associatedItem = value;
				}
			}

            protected override Type ThemeEffectiveType
            {
                get
                {
                    return typeof(RadMenuItem);
                }
            }
		}
	}
}
