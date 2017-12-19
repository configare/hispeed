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
using Telerik.WinControls.Layouts;
using Telerik.WinControls.UI.TabStrip;
using Telerik.WinControls;
using Telerik.WinControls.Design;
using Telerik.WinControls.UI;
using System.Collections;
//using Telerik.WinControls.UI.ToolStrip;
using Telerik.WinControls.Elements;
namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Represents a tool strip item. Tool strip items are containers for elements 
	/// such as buttons, textboxes, comboboxes, etc.  
	/// </summary>
    [Designer(DesignerConsts.RadToolStripItemDesignerString)]
    public class RadToolStripItem : RadItem, IItemsOwner
	{
		#region Fields
		private const int GripOffset = 5;
        private const int GripLength = 10;
        private const int OverflowButtonLength = 10;

		private Size floatingFormPreferredSize;

		
		private FillPrimitive fill;
		private RadItemOwnerCollection items;
		private BorderPrimitive border;
		internal ToolStripItemLayout itemsLayout;
        private RadToolStripGripElement gripElement;

        public RadToolStripGripElement Grip
        {
            get 
            {
                return gripElement; 
            }
        }

		private  Stack<int> shrinkWidths;

		private ToolStripItemsOverFlow overflowManager = new ToolStripItemsOverFlow();

		private RadToolStripElement parentToolStripElement;
		private RadToolStripManager parentToolStripManager;
		#endregion

		#region Constructors

		protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Class = "ToolStripItem";
            this.MinSize = new Size(25, 21);
            this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;

            this.shrinkWidths = new Stack<int>();
        }

		#endregion

		#region Properties
		internal Size FloatingFormPreferredSize
		{
			get
			{
				return this.floatingFormPreferredSize;
			}
			set
			{
				this.floatingFormPreferredSize = value;
			}
		}

		internal Stack<int> ShrinkWidths
		{
			get
			{
				return this.shrinkWidths;
			}
			set
			{
				this.shrinkWidths = value;
			}
		}

		private string key = "";
		/// <summary>
		/// Gets or sets the key value associated to this object
		/// </summary>
		[Browsable(false)]
		[Description("Gets or sets the key value associated to this object")]
		[DefaultValue("")]
		public string Key
		{
			get
			{
				if (String.IsNullOrEmpty(key))
				{
					if (this.ParentToolStripElement != null)
					{
						if (this.ParentToolStripElement.Items.Count > 0)
							this.key = this.ParentToolStripElement.Items.IndexOf(this).ToString();
					}
				}

				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

		/// <summary>
		/// Gets the close button of the floating toolStrip
		/// </summary>
		[Category("Behavior")]
		[Browsable(false)]
		[Description("Gets the close button of the floating toolStrip")]
		public RadButton FloatingCloseButton
		{
			get
			{
				return this.gripElement.closeButton;
			}
		}

		/// <summary>
		/// Gets the overflow button of the floating toolStrip
		/// </summary>
		[Category("Behavior")]
		[Browsable(false)]
		[Description("Gets the overflow button of the floating toolStrip")]
		public RadDropDownButton FloatingOverFlowButton
		{
			get
			{
				return this.gripElement.dropDownButton;
			}
		}


		/// <summary>
		/// Gets the inner items.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.DataCategory)]
        [RadEditItemsAction]
        [Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor))]
		[RadNewItem("  ", false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RadItemOwnerCollection Items
		{
			get
			{
				return this.items;
			}
		}

        /// <summary>
        /// Gets or sets orientation - it could be horizontal or vertical.
        /// </summary>
		[Browsable(false)]
        [Category(RadDesignCategory.LayoutCategory)]
        [RadDefaultValue("Orientation", typeof(ToolStripItemLayout))]
        [Description("Orientation of the element - could be horizontal or vertical")]
        public Orientation Orientation
        {
            get
            {
                return this.itemsLayout.Orientation;
            }
            set
            {
                bool newValue = value != this.itemsLayout.Orientation;
                this.itemsLayout.Orientation = value;
                if (newValue)
                {
                    if (value == Orientation.Horizontal)
                    {
                        this.gripElement.AngleTransform = 0;
                        this.OverflowManager.DropDownButton.AngleTransform = 0;
                    }
                    else
                    {
                        this.gripElement.AngleTransform = 90;
                        this.OverflowManager.DropDownButton.AngleTransform = 90;
                    }
                }
            }
        }

		public void InvalidateLayout()
		{
		}


        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ToolStripItemsOverFlow OverflowManager
		{
			get 
            {
                return overflowManager; 
            }
		}

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadToolStripOverFlowButtonElement OverflowButton
        {
            get 
            {
                return overflowManager.DropDownButton;  
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadDropDownMenu DropDownMenu
        {
            get 
            {
                return overflowManager.DropDownButton.DropDownMenu;  
            }
        }

		/// <summary>Gets the <see cref="RadToolStripManager">tool strip manager</see>.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadToolStripManager ParentToolStripManager
		{
			get
			{
                if (this.parentToolStripManager == null)
				{
					for (RadElement res = this.Parent; res != null && this.parentToolStripManager == null; res = res.Parent)
					{
						this.parentToolStripManager = res as RadToolStripManager;
					}
				}
				return this.parentToolStripManager;
			}
		}
		/// <summary>Gets or sets the ToolStripElement's <see cref="RadToolStripElement">parent</see>.</summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadToolStripElement ParentToolStripElement
		{
			get
			{
                if (this.parentToolStripElement == null)
				{
					for (RadElement res = this.Parent; res != null && this.parentToolStripElement == null; res = res.Parent)
					{
						this.parentToolStripElement = res as RadToolStripElement;
					}
				}
				return this.parentToolStripElement;
			}
			set
			{
				this.parentToolStripElement = value;
			}
		}
		#endregion

		#region Methods

        private void items_ItemsChanged(RadItemCollection changed, RadItem tartet, ItemsChangeOperation operation)
		{
            if (tartet != null && operation == ItemsChangeOperation.Inserted)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    tartet.Alignment = ContentAlignment.MiddleLeft;
                }
                else
                {
                    tartet.Alignment = ContentAlignment.TopLeft;
                }
            }
            if (operation == ItemsChangeOperation.Inserted)
			{
				if (this.Orientation == Orientation.Vertical)
				{
					tartet.AngleTransform = 90;
				}
				else
					tartet.AngleTransform = 0;

				if (tartet is RadToolStripSeparatorItem)
				{
					tartet.MinSize = new Size(2, 0);
				}
				
                else if (tartet is RadComboBoxElement)
				{
                    tartet.MinSize = new Size(118, 17);
                    tartet.MaxSize = new Size(118, 20);

                    foreach (RadElement element in tartet.Children)
					{
						if (element is FillPrimitive)
						{
							(element as FillPrimitive).FitToSizeMode = RadFitToSizeMode.FitToParentContent;
							element.MaxSize = new Size(0, 18);
							element.MinSize = new Size(0, 17);
						}
						if (element is DropDownEditorLayoutPanel)
						{
							element.MaxSize = new Size(0, 18);
							foreach (RadElement child in element.Children)
							{
								if (child as RadArrowButtonElement != null)
								{
									child.MaxSize = new Size(0, 18);
								}
                                else if (child as RadTextBoxElement != null)
                                {
                                    child.MaxSize = new Size(100, 0);
                                }
							}
						}
					}
					(tartet as RadComboBoxElement).TextBoxItem.MinSize = new Size(96, 0);
					(tartet as RadComboBoxElement).TextBoxItem.MaxSize = new Size(96, 0);

					//		(tartet as RadComboBoxElement).ArrowButton.SetFillClass("NoClass");
					//		(tartet as RadComboBoxElement).ComboBoxFill.ShouldPaint = t;// = RadFitToSizeMode.FitToBounds;

                    tartet.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
				}

				else if (tartet is RadTextBoxElement)
				{
                    tartet.MinSize = new Size(118, 19);
                    tartet.MaxSize = new Size(118, 19);

					(tartet as RadTextBoxElement).TextBoxItem.MinSize = new Size(114, 0);
					(tartet as RadTextBoxElement).TextBoxItem.MaxSize = new Size(114, 0);
					(tartet as RadTextBoxElement).ShowBorder = true;

                    tartet.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
				}
			}
		}


		/// <summary>Refreshes the items' Z-index in the collection of items.</summary>
		public void RefreshItems()
		{
		}

		internal bool HasBorder()
		{
			return this.border.Visibility != ElementVisibility.Collapsed;
		}

        private Size CalcOwnSize(Size itemsSize)
        {
            Size res = itemsSize;
            if (this.Orientation == Orientation.Horizontal)
                res.Width = itemsSize.Width + GripOffset + GripLength + OverflowButtonLength;
            else
                res.Height = itemsSize.Height + GripOffset + GripLength + OverflowButtonLength;
            return res;
        }

        private void ResetItemsVisibility(Size childrenSize, Size availableSize)
        {
			bool shouldShow = false;
		
            if (items.Count == 0)
                return;

            if (this.Orientation == Orientation.Horizontal)
            {
                int availableLength = childrenSize.Width;
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    RadItem item = items[i];
                    if (item.Visibility != ElementVisibility.Collapsed)
                    {
                        if (availableLength > availableSize.Width)
                        {
                            item.Visibility = ElementVisibility.Hidden;
							shouldShow = true;
							availableLength -= item.Size.Width;
                        }
                        else
                        {
                            item.Visibility = ElementVisibility.Visible;
                        }
                    }
                }
            }
            else
            {
                int availableLength = childrenSize.Height;
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    RadItem item = items[i];
                    if (item.Visibility != ElementVisibility.Collapsed)
                    {
                        if (availableLength > availableSize.Height)
                        {
                            item.Visibility = ElementVisibility.Hidden;
							shouldShow = true;
							availableLength -= item.Size.Height;
                        }
                        else
                        {
                            item.Visibility = ElementVisibility.Visible;
                        }
                    }
                }
            }

			this.OverflowManager.DropDownButton.ShowSmallArrows(shouldShow);
			
			this.overflowManager.RefreshParentToolStrip();
        }

		private Size lastAvailableSize;
        public override void PerformLayoutCore(RadElement affectedElement)
		{
			//base.PerformLayoutCore(affectedElement);
            Size paddingSize = Size.Subtract(this.Size, Size.Truncate(this.border.BorderSize));//CalcOwnSize(this.AvailableSize);
            Size availableSize = Size.Subtract(paddingSize, this.Padding.Size);

		//	if (this.lastAvailableSize != availableSize)
			{
				this.lastAvailableSize = availableSize;

				Size borderSize = this.border.GetPreferredSize(this.Size);
				Size fillSize = this.fill.GetPreferredSize(paddingSize);
				Size gripSize = this.gripElement.GetPreferredSize(availableSize);
				Size itemsSize = this.itemsLayout.GetPreferredSize(availableSize);
				Size dropDownSize = this.OverflowManager.DropDownButton.GetPreferredSize(availableSize);

				Rectangle itemsRect = Rectangle.Empty;
				Orientation currentOrientation = this.Orientation;
				if (currentOrientation == Orientation.Horizontal)
				{
					gripSize.Width = GripLength;
					gripSize.Height = availableSize.Height;

					dropDownSize.Width = OverflowButtonLength;
					dropDownSize.Height = availableSize.Height;

					itemsRect = new Rectangle(GripOffset + GripLength, 0, itemsSize.Width, availableSize.Height);
				}
				else
				{
					/*gripSize.Width = availableSize.Width;
					gripSize.Height = GripLength;

					dropDownSize.Width = availableSize.Width;
					dropDownSize.Height = OverflowButtonLength;*/

					gripSize.Width = GripLength;
					gripSize.Height = availableSize.Width;

					dropDownSize.Width = OverflowButtonLength;
					dropDownSize.Height = availableSize.Width;

					itemsRect = new Rectangle(0, GripOffset + GripLength, availableSize.Width, itemsSize.Height);
				}

				this.border.Size = this.Size;
				this.fill.Size = paddingSize;
				if (currentOrientation == Orientation.Horizontal)
				{
					this.gripElement.SetBounds(GripOffset, 0, gripSize.Width, gripSize.Height);
					this.OverflowManager.DropDownButton.SetBounds(
						availableSize.Width - OverflowButtonLength, 0, dropDownSize.Width, dropDownSize.Height);
				}
				else
				{
					this.gripElement.SetBounds(0, GripOffset, gripSize.Width, gripSize.Height);
					this.OverflowManager.DropDownButton.SetBounds(
						0, availableSize.Height - OverflowButtonLength, dropDownSize.Width, dropDownSize.Height);
				}
				this.itemsLayout.SetBounds(itemsRect);

				if (currentOrientation == Orientation.Horizontal)
					availableSize.Width = availableSize.Width - GripOffset - GripLength - OverflowButtonLength;
				else
					availableSize.Height = availableSize.Height - GripOffset - GripLength - OverflowButtonLength;
	
				ResetItemsVisibility(itemsSize, availableSize);
	
			}

		}

	    public override Size GetPreferredSizeCore(Size proposedSize)
        {
            Size res = base.GetPreferredSizeCore(this.itemsLayout.FullBoundingRectangle.Size);

			res = CalcOwnSize(res);
			
			return res;
        }

		public void CreateFloatingToolStrip(Point screenLocation)
		{
			if (this.gripElement != null)
			{
				this.gripElement.CreateFloatingToolStrip();

				this.gripElement.form.Location = screenLocation;

				this.gripElement.form.Show();
			}
		}

		protected override void CreateChildElements()
		{
			this.border = new BorderPrimitive();
            this.border.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.border.GradientStyle = GradientStyles.Solid;
            this.border.MinSize = this.MinSize;
            this.border.Visibility = ElementVisibility.Hidden;
		    this.border.Class = "ToolStripItemBorder";
            this.Children.Add(this.border);

			this.fill = new FillPrimitive();
			this.fill.Class = "ToolStripFill";
			this.fill.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
			this.fill.MinSize = this.MinSize;
			this.fill.BackColor = Color.DarkGray;
			this.fill.GradientStyle = GradientStyles.Solid;
            this.Children.Add(this.fill);

			this.gripElement = new RadToolStripGripElement();
            //this.gripElement.MinSize = new Size(10, 0);
            //this.gripElement.Margin = new Padding(5, 0, 0, 0);
            this.gripElement.SetValue(ToolStripItemLayout.IsGripElementProperty, true);
            this.Children.Add(this.gripElement);

            this.itemsLayout = new ToolStripItemLayout();
            //this.itemsLayout.EqualChildrenHeight = true;
            //this.itemsLayout.ForceElementsHeight = true;
            this.itemsLayout.MinSize = new Size(21, 21);
            this.itemsLayout.Class = "ToolStripItemLayout";
            this.itemsLayout.Alignment = ContentAlignment.TopLeft;
            this.itemsLayout.ParentToolStripManager = this.ParentToolStripManager;
			this.Children.Add(itemsLayout);

            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] { typeof(RadButtonElement), typeof(RadToggleButtonElement), typeof(RadRepeatButtonElement),
                 typeof(RadCheckBoxElement), typeof(RadImageButtonElement),
                 typeof(RadRadioButtonElement), typeof(RadTextBoxElement), typeof(RadMaskedEditBoxElement), typeof(RadComboBoxElement), 
				 typeof(RadToolStripLabelElement), typeof(RadToolStripSeparatorItem), typeof(RadDropDownButtonElement), typeof(RadSplitButtonElement), 
			};
            this.items.ItemsChanged += new ItemChangedDelegate(items_ItemsChanged);
            this.items.Owner = itemsLayout;

            this.OverflowManager.InitializeOverflowDropDown(this);
            this.Children.Add(this.OverflowManager.DropDownButton);


			if (this.ParentToolStripManager != null)
			{
				if (!this.ParentToolStripManager.ShowOverFlowButton)
					this.OverflowManager.DropDownButton.Visibility = ElementVisibility.Hidden;

				this.ParentToolStripManager.RadPropertyChanged += new RadPropertyChangedEventHandler(ParentToolStripManager_RadPropertyChanged);
			}
		}

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == RadElement.BoundsProperty)
            {
                this.PerformLayout(this);
            }
        }

		private void ParentToolStripManager_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
		{
			if (e.Property == RadToolStripManager.ShowOverflowButtonProperty)
			{
				if ((bool)e.NewValue == false)
				{
					this.OverflowManager.DropDownButton.Visibility = ElementVisibility.Hidden;
				}
				else
					this.OverflowManager.DropDownButton.Visibility = ElementVisibility.Visible;

			}
		}

		#endregion
	}
}
