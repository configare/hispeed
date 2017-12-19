using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls;
using System.Windows.Forms;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI;
using System.Reflection;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
	public class OutlookStyle : PanelBarStyleBase
	{
		private OutLookStyleLayout groupLayout;
		private RadPanelBarVisualElement captionElement;
		private RadPanelBarOverFlow panelBarOverFlow;
		private OutLookGripPrimitive grip;
		private RadMenuItem showMoreButtonsMenuItem;
		private RadMenuItem showFewerButtonsMenuItem;
		private RadMenuItem navigationPaneOptions;
		private RadMenuItem AddOrRemoveButtons;
		private RadHostItem contentPanelHost;
		private RadTabStripContentPanel contentPanel;
		private List<RadPanelBarGroupElement> collapsedGroups;
		private RadPanelBarContentControl itemsControl;
		private RadPanelBarGroupElement selectedGroup;

		public OutlookStyle(RadPanelBarElement panelBar, RadTabStripContentPanel contentPanel, RadPanelBarContentControl itemsControl) : base(panelBar)
		{
			//this.panelBar = panelBar;
			this.contentPanel = contentPanel;
			this.itemsControl = itemsControl;

			if (itemsControl == null)
			{
				this.itemsControl = new RadPanelBarContentControl();
			}

			if (this.contentPanel != null)
			{
				this.contentPanelHost = new RadHostItem(this.contentPanel);
                this.contentPanel.AutoSize = false;
//                this.contentPanel.AutoScroll = false;
////				this.contentPanelHost.AutoSize = false;
//                this.contentPanel.Size = new Size(50, 50);
//                this.contentPanelHost.Size = new Size(50, 50);
				this.contentPanel.Visible = false;
				this.contentPanelHost.Visibility = ElementVisibility.Hidden;
                this.contentPanelHost.StretchHorizontally = true;
                this.contentPanelHost.StretchVertically = true;
                this.contentPanel.Location = this.contentPanelHost.LocationToControl();

			}
			else
			{
				this.contentPanelHost = new RadHostItem(this.itemsControl);
				this.contentPanelHost.AutoSize = false;
				this.contentPanelHost.Size = new Size(50, 50);
				this.contentPanelHost.Visibility = ElementVisibility.Hidden;
				this.itemsControl.MaximumSize = new Size(this.contentPanelHost.Bounds.Width, this.contentPanelHost.Size.Height);
            //    this.itemsControl.Bounds = new Rectangle(this.contentPanelHost.Bounds.X, 0, this.contentPanelHost.Bounds.Width, this.contentPanelHost.Bounds.Height); 
                this.itemsControl.Bounds = this.contentPanelHost.Bounds;
                this.contentPanelHost.StretchHorizontally = false;
                this.contentPanelHost.StretchVertically = false;
            }

			this.collapsedGroups = new List<RadPanelBarGroupElement>();             
        }

		public RadTabStripContentPanel ContentPanel
		{
			get
			{
				return this.contentPanel;
			}
		}

		protected override void DoExpand(RadPanelBarGroupElement[] groups)
		{
			PanelBarGroupCancelEventArgs groupCancelArgs = new PanelBarGroupCancelEventArgs(groups[0], false);

			this.panelBar.CallPanelBarGroupExpanding(groupCancelArgs);

			if (groupCancelArgs.Cancel)
				return;

			PanelBarGroupEventArgs groupArgs = new PanelBarGroupEventArgs(groups[0]);


			this.panelBar.CallPanelBarGroupExpanded(groupArgs);

			return;
		}

		protected internal override PanelBarBaseLayout GetBaseLayout()
		{
			return this.groupLayout;
		}

		private void SetImageToItem(RadMenuItem item, string searchPattern)
		{
			item.Image = new Bitmap(16, 16);
			Assembly currentAssembly = Assembly.GetExecutingAssembly();
			string[] s = currentAssembly.GetManifestResourceNames();
			string p = "";
			foreach (string str in s)
			{
				if (str.EndsWith(searchPattern))
					p = str;
			}
			if (p != "")
				item.Image = Image.FromStream(Telerik.WinControls.TelerikHelper.GetStreamFromResource(currentAssembly, p));
		}

		private void InitializeOverFlowDropDownButton()
		{
			this.showMoreButtonsMenuItem = new RadMenuItem();
			this.showMoreButtonsMenuItem.Text = "Show More Buttons";
			this.showMoreButtonsMenuItem.Enabled = false;
			this.showMoreButtonsMenuItem.Click += new EventHandler(item_Click);
			this.SetImageToItem(this.showMoreButtonsMenuItem, "arrowUp.gif");
			if (this.panelBarOverFlow.Items.Count > 0)
			{
				this.showMoreButtonsMenuItem.Enabled = true;
			}

			this.showFewerButtonsMenuItem = new RadMenuItem();
			this.showFewerButtonsMenuItem.Text = "Show Fewer Buttons";
			this.showFewerButtonsMenuItem.Click += new EventHandler(item2_Click);
			this.SetImageToItem(this.showFewerButtonsMenuItem, "arrowDown.gif");
			if (this.panelBarOverFlow.Items.Count == this.panelBar.Items.Count)
			{
				this.showFewerButtonsMenuItem.Enabled = false;
			}

			this.navigationPaneOptions = new RadMenuItem();
			this.navigationPaneOptions.Text = "Navigation Pane Options";
			this.navigationPaneOptions.Click += new EventHandler(navigationPaneOptions_Click);

			this.AddOrRemoveButtons = new RadMenuItem();
			this.AddOrRemoveButtons.Text = "Add or Remove Buttons";

			this.panelBarOverFlow.DropDownButton.Items.Add(this.showMoreButtonsMenuItem);
			this.panelBarOverFlow.DropDownButton.Items.Add(this.showFewerButtonsMenuItem);
			this.panelBarOverFlow.DropDownButton.Items.Add(this.navigationPaneOptions);
			this.panelBarOverFlow.DropDownButton.Items.Add(this.AddOrRemoveButtons);


			foreach (RadPanelBarGroupElement group in this.panelBar.Items)
			{
				RadAssociatedMenuItem menuItem = new RadAssociatedMenuItem(group);
				menuItem.Text = group.Caption;
				menuItem.IsChecked = true;
				menuItem.Image = (group.horizontalLayout.Children[0] as RadPanelBarVisualElement).Image;
				this.AddOrRemoveButtons.Items.Add(menuItem);
				menuItem.CheckOnClick = true;
				menuItem.Click += new EventHandler(menuItem_Click);
			}

			foreach (RadPanelBarGroupElement group in this.panelBar.hiddenGroupsList)
			{
				RadAssociatedMenuItem menuItem = new RadAssociatedMenuItem(group);
				menuItem.Text = group.Caption;
				menuItem.IsChecked = false;
				menuItem.Image = (group.horizontalLayout.Children[0] as RadPanelBarVisualElement).Image;
				this.AddOrRemoveButtons.Items.Add(menuItem);
				menuItem.CheckOnClick = true;
				menuItem.Click += new EventHandler(menuItem_Click);
			}
		}

        private void menuItem_Click(object sender, EventArgs e)
		{
            this.panelBar.BeginUpdate();
            this.panelBar.SuspendLayout();
            RadAssociatedMenuItem item = sender as RadAssociatedMenuItem;

			if (item.IsChecked)
			{
                BoxLayout layout = new BoxLayout();
              
				this.panelBar.Items.Insert(0, item.group);
                this.panelBar.Items.Owner = layout;
                
                this.panelBar.hiddenGroupsList.Remove(item.group);

				if (item.group.Visibility == ElementVisibility.Collapsed)
				{
					InsertToggleButton(item.group);
				}

			}
			else
			{
				this.panelBar.Items.Remove(item.group);
                item.group.Items.Owner = item.group.verticalLayout;
				this.panelBar.hiddenGroupsList.Add(item.group);
				RemoveToggleButton(item.group);
			}

            this.panelBar.SetStyle();
            this.panelBar.EndUpdate();
            this.panelBar.ResumeLayout(true);
		}

		private radNavigationPaneOptions navigationForm;

		public radNavigationPaneOptions NavigationForm
		{
			get
			{
				if (this.navigationForm == null)
				{
					this.navigationForm = new radNavigationPaneOptions(this.panelBar);
				}

                navigationForm.SetButtonsTheme(((RadControl)this.panelBar.ElementTree.Control).ThemeName);

				return navigationForm;
			}
		}

		protected override void DoSelect(RadPanelBarGroupElement[] groups)
		{
			this.DoSelectUnSelect(groups);
		}

		private void DoSelectUnSelect(RadPanelBarGroupElement[] groups)
		{        
			if (groups == null || groups.Length == 0)
			{
				return;
			}

			PanelBarGroupCancelEventArgs groupCancelArgs = new PanelBarGroupCancelEventArgs(groups[0], false);
			this.panelBar.CallPanelBarGroupSelecting(groupCancelArgs);
			if (groupCancelArgs.Cancel)
			{
				return;
			}

			this.panelBar.BeginUpdate();
			foreach (RadPanelBarGroupElement group in this.panelBar.Items)
			{
				
				if (group != groups[0])
				{
					group.Selected = false;
				}
			}
			groups[0].Selected = true;

            if (this.itemsControl.ThemeName != this.GetRadPanelBarElement().ElementTree.ComponentTreeHandler.ThemeName)
            {
                this.itemsControl.ThemeName = this.GetRadPanelBarElement().ElementTree.ComponentTreeHandler.ThemeName;
            }

			MoveContent(groups[0]);
			this.panelBar.EndUpdate();

			PanelBarGroupEventArgs groupArgs = new PanelBarGroupEventArgs(groups[0]);
			this.panelBar.CallPanelBarGroupSelected(groupArgs);

			UnWireEvents();
			this.panelBar.SetStyle();


     	}


		private RadPanelBarGroupElement GetSelectedGroup()
		{
			foreach (RadPanelBarGroupElement group in this.panelBar.Items)
			{
				if (group.Selected)
					return group;
			}

			return null;
		}



		private void MoveContent(RadPanelBarGroupElement group)
		{
            if (this.selectedGroup != null && this.selectedGroup.IsDisposed)
            {           
                this.itemsControl.ItemsControlElement.GetBoxLayout().Children.Clear();
                return;
            }

			if (group != null)
			{
				if (group.EnableHostControlMode)
				{
					if (this.panelBar.GetContentPanel() == null || !this.panelBar.GetContentPanel().Equals(group.ContentPanel))
					{
						this.panelBar.SetContentPanel(group.ContentPanel);
                        if (group.verticalGroupLayout.Children.Contains(group.ContentPanelHost))
                        {
                            group.verticalGroupLayout.Children.Remove(group.ContentPanelHost);
                        }
                    }

                    this.itemsControl.HideChildren();

                    if (this.selectedGroup != null && !this.selectedGroup.Equals(group))
                    {
                        this.selectedGroup.Items.Owner = this.selectedGroup.verticalGroupLayout;
                        this.selectedGroup.CollapseChildren(false);
                    }
				}
				else
				{
					this.panelBar.SetContentPanel(null);
					if (this.selectedGroup != null && !this.selectedGroup.Equals(group))
					{
						this.selectedGroup.Items.Owner = this.selectedGroup.verticalGroupLayout;
						this.selectedGroup.CollapseChildren(false);
					}

					group.Items.Owner = this.itemsControl.ItemsControlElement.GetBoxLayout();
					group.CollapseChildren(true);
					this.panelBar.SetItemsControl(this.itemsControl);
					this.itemsControl.ShowChildren();
				}
			}
		}

        private void navigationPaneOptions_Click(object sender, EventArgs e)
        {
            if (this.navigationForm == null)
            {
                this.navigationForm = new radNavigationPaneOptions(this.panelBar);
            }

            if (!this.navigationForm.IsDisposed)
            {
                navigationForm.TopMost = true;
                navigationForm.SetButtonsTheme(((RadControl)this.panelBar.ElementTree.Control).ThemeName);
                navigationForm.Show();
            }
            else
            {
                this.navigationForm = new radNavigationPaneOptions(this.panelBar);
                navigationForm.TopMost = true;
                navigationForm.SetButtonsTheme(((RadControl)this.panelBar.ElementTree.Control).ThemeName);
                navigationForm.Show();
            }
        }

		private void item2_Click(object sender, EventArgs e)
		{
			ShowFewerButtons();
		}


		/// <summary>
		/// Decreases the number of outlook panel buttons, if panelbar view style is OutlookBar
		/// </summary>
		public void ShowFewerButtons()
		{

			for (int i = this.panelBar.Items.Count - 1; i > -1; i--)
			{
				RadPanelBarGroupElement group = (RadPanelBarGroupElement)this.panelBar.Items[i];
				if (group.Visibility == ElementVisibility.Visible)
				{
					group.Visibility = ElementVisibility.Collapsed;
					this.InsertToggleButton(group);
					break;
				}
			}

			if (this.panelBarOverFlow.Items.Count == this.panelBar.Items.Count)
			{
				this.showFewerButtonsMenuItem.Enabled = false;
			}
			else
			{
				this.showFewerButtonsMenuItem.Enabled = true;
			}

			if (this.panelBarOverFlow.Items.Count == 0)
			{
				this.showMoreButtonsMenuItem.Enabled = false;
			}
			else
			{
				this.showMoreButtonsMenuItem.Enabled = true;
			}
		}

		private void item_Click(object sender, EventArgs e)
		{
			ShowMoreButtons();
		}

		/// <summary>
		/// Increases the number of outlook panel buttons, if panelbar view style is OutlookBar
		/// </summary>
		public void ShowMoreButtons()
		{
			for (int i = 0; i < this.panelBar.Items.Count; i++)
			{
				RadPanelBarGroupElement group = (RadPanelBarGroupElement)this.panelBar.Items[i];
				if (group.Visibility == ElementVisibility.Collapsed)
				{
					group.Visibility = ElementVisibility.Visible;
					this.RemoveToggleButton(group);
					break;
				}
			}

			if (this.panelBarOverFlow.Items.Count == this.panelBar.Items.Count)
			{
				this.showFewerButtonsMenuItem.Enabled = false;
			}
			else
			{
				this.showFewerButtonsMenuItem.Enabled = true;
			}

			if (this.panelBarOverFlow.Items.Count == 0)
			{
				this.showMoreButtonsMenuItem.Enabled = false;
			}
			else
			{
				this.showMoreButtonsMenuItem.Enabled = true;
			}
		}

		public override RadPanelBarElement GetRadPanelBarElement()
		{
			return this.panelBar;
		}

        public override void CreateChildren()
        {
            this.captionElement = this.panelBar.captionElement;
            this.panelBarOverFlow = new RadPanelBarOverFlow();

            this.grip = new OutLookGripPrimitive();
            this.grip.Class = "PanelBarOutlookGrip";

            this.groupLayout = new OutLookStyleLayout(this.captionElement, this.panelBarOverFlow, this.grip, this.contentPanelHost);
            this.panelBar.Children.Add(this.groupLayout);
            UpdateGroupsUI();
            this.panelBar.Items.Owner = this.groupLayout;
            this.InitializeOverFlowDropDownButton();

            foreach (RadPanelBarGroupElement group in this.panelBar.Items)
            {
                if (group.verticalGroupLayout != null)
                {
                    if (group.verticalGroupLayout.Children.Contains(group.ContentPanelHost))
                    {
                        group.verticalGroupLayout.Children.Remove(group.ContentPanelHost);
                    }
                }
            }

            this.panelBarOverFlow.DropDownButton.DropDownOpening += new System.ComponentModel.CancelEventHandler(DropDownButton_DropDownOpening);
        }

        private void DropDownButton_DropDownOpening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.panelBarOverFlow.DropDownButton.Items.Clear();
			this.InitializeOverFlowDropDownButton();
		}

        private void DropDownButton_DropDownOpened(object sender, EventArgs e)
		{
		}

		public override void WireEvents()
		{
			this.panelBar.RadPropertyChanged += new Telerik.WinControls.RadPropertyChangedEventHandler(panelBar_RadPropertyChanged);
			this.groupLayout.RadPropertyChanged += new RadPropertyChangedEventHandler(explorerBarLayout_RadPropertyChanged);
			this.panelBar.Items.ItemsChanged += new ItemChangedDelegate(Items_ItemsChanged);
		
			this.groupLayout.ArrangeModified += new EventHandler(groupLayout_ArrangeModified);

			if (this.panelBar.ElementTree != null)
			{
				this.panelBar.ElementTree.Control.MouseDown += new MouseEventHandler(Control_MouseDown);
				this.panelBar.ElementTree.Control.MouseUp += new MouseEventHandler(Control_MouseUp);
				this.panelBar.ElementTree.Control.MouseMove += new MouseEventHandler(Control_MouseMove);
                this.panelBar.ElementTree.Control.Resize += new EventHandler(Control_Resize);
            }

			foreach (RadPanelBarGroupElement group in this.panelBar.Items)
			{
				group.RadPropertyChanged += new RadPropertyChangedEventHandler(target_RadPropertyChanged);
            }
		}
		
		public override void UnWireEvents()
		{
			this.panelBar.RadPropertyChanged -= new Telerik.WinControls.RadPropertyChangedEventHandler(panelBar_RadPropertyChanged);
			this.groupLayout.RadPropertyChanged -= new RadPropertyChangedEventHandler(explorerBarLayout_RadPropertyChanged);
			this.panelBar.Items.ItemsChanged -= new ItemChangedDelegate(Items_ItemsChanged);
	
			this.groupLayout.ArrangeModified -= new EventHandler(groupLayout_ArrangeModified);

			if (this.panelBar.ElementTree != null)
			{
				this.panelBar.ElementTree.Control.MouseDown -= new MouseEventHandler(Control_MouseDown);
				this.panelBar.ElementTree.Control.MouseUp -= new MouseEventHandler(Control_MouseUp);
				this.panelBar.ElementTree.Control.MouseMove -= new MouseEventHandler(Control_MouseMove);
                this.panelBar.ElementTree.Control.Resize -= new EventHandler(Control_Resize);
            }

			foreach (RadPanelBarGroupElement group in this.panelBar.Items)
			{
				group.RadPropertyChanged -= new RadPropertyChangedEventHandler(target_RadPropertyChanged);
			}
		}

		#region ResizingLogic

        private Stack<RadPanelBarGroupElement> groupsStack = new Stack<RadPanelBarGroupElement>();
		internal List<int> collapsingSteps = new List<int>();
        private int resizingIndx;

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.panelBar.Items.Count == 0)
                return;

            if (this.grip.ControlBoundingRectangle.Contains(e.Location))
            {
                Cursor.Current = Cursors.SizeNS;
            }

            if (this.panelBar.ElementTree.Control.Capture && e.Button == MouseButtons.Left && Cursor.Current == Cursors.SizeNS)
            {
                collapsingSteps.Clear();
                int currentValue = this.panelBarOverFlow.ControlBoundingRectangle.Top;

                foreach (RadPanelBarGroupElement group in this.panelBar.Items)
                {
                    currentValue -= (int)group.CaptionHeight;
                    collapsingSteps.Insert(0, currentValue);
                }

                PerformDynamicResize(e);
            }
        }

		private void PerformDynamicResize(MouseEventArgs e)
		{
			int indx = this.resizingIndx;

			if (this.panelBar.Items.Count == 0)
				return;

			if (this.resizingIndx > this.panelBar.Items.Count - 1)
			{
				indx--;
			}

			if (indx < collapsingSteps.Count && e.Y > collapsingSteps[indx])
			{
				ShowFewerButtons();
			}
			else
			{
				indx = this.resizingIndx - 1;
				if (indx == -1)
				{
					indx = 0;
				}

				if (indx < collapsingSteps.Count && e.Y < collapsingSteps[indx])
				{
					ShowMoreButtons();
				}
			}

			this.resizingIndx = 0;
			foreach (RadPanelBarGroupElement group in this.panelBar.Items)
			{
				if (group.Visibility == ElementVisibility.Collapsed)
					resizingIndx++;
			}
		}

        private void Control_MouseUp(object sender, MouseEventArgs e)
		{
			this.panelBar.ElementTree.Control.Capture = false;
			Cursor.Current = Cursors.Default;
		}


        private void Control_MouseDown(object sender, MouseEventArgs e)
		{
			if (this.grip.ControlBoundingRectangle.Contains(e.Location))
			{
				this.panelBar.ElementTree.Control.Capture = true;
				Cursor.Current = Cursors.SizeNS;
			}
		}
		#endregion
        private void groupLayout_ArrangeModified(object sender, EventArgs e)
		{
			SetHostsBounds();
		}

		private void SetHostsBounds()
		{
			int hostHeight = this.grip.ControlBoundingRectangle.Top - this.captionElement.ControlBoundingRectangle.Bottom;

			if (this.contentPanel != null && this.contentPanelHost != null)
			{
				this.contentPanel.MaximumSize = Size.Empty;
			//	this.contentPanelHost.Bounds = new Rectangle(1, (int)this.captionElement.DesiredSize.Height, this.captionElement.ControlBoundingRectangle.Width,
			//		hostHeight);
				this.contentPanel.MinimumSize = Size.Empty;
				this.contentPanel.MaximumSize = new Size(this.contentPanelHost.Bounds.Width, this.contentPanelHost.Size.Height);
				this.contentPanel.Size = this.contentPanelHost.Size;
				this.contentPanel.MinimumSize = this.contentPanel.Bounds.Size;
                this.contentPanel.Location = this.contentPanelHost.LocationToControl();
           //     this.contentPanel.PerformLayout();
         //       this.contentPanel.Location = this.contentPanelHost.Location;
			}
			else
			{

				if (this.contentPanelHost != null)
				{
					this.itemsControl.MaximumSize = Size.Empty;
                    this.contentPanelHost.Bounds = new Rectangle(1, (int)this.captionElement.DesiredSize.Height, this.captionElement.ControlBoundingRectangle.Width,
                        hostHeight);

					this.itemsControl.MaximumSize = new Size(this.contentPanelHost.Bounds.Width, this.contentPanelHost.Size.Height);
                    //this.itemsControl.Bounds = new Rectangle(this.contentPanelHost.Bounds.X, 0, this.contentPanelHost.Bounds.Width, this.contentPanelHost.Bounds.Height);
                    this.itemsControl.Bounds = this.contentPanelHost.Bounds;
                    this.itemsControl.PerformLayout();
				}

			}
		}

        private void Control_Resize(object sender, EventArgs e)
        {
            collapsingSteps.Clear();

            RefreshItemsControlWidth();
        }

        private void RefreshItemsControlWidth()
        {
            if (this.itemsControl != null && this.panelBar != null)
            {
                RadPanelBar panelBar = this.panelBar.ElementTree.Control as RadPanelBar;

                if (panelBar != null)
                {
                    this.itemsControl.Width = panelBar.Width;
                }
            }
        }



		protected internal override void UpdateGroupsUI()
		{
			if (this.panelBar.Items.Count > 0)
			{
				bool selectedGroups = false;

				RadPanelBarGroupElement selectedGroup = null;

                

				foreach (RadPanelBarGroupElement group in this.panelBar.Items)
				{
					group.ShowCaptionButton(false);
					if (group.Selected)
					{
						selectedGroups = true;
						selectedGroup = group;
						this.selectedGroup = group;
					}

					group.CollapseChildren(false);
					group.horizontalLayout.StretchHorizontally = true;
					group.verticalLayout.StretchHorizontally = true;
					group.verticalLayout.StretchVertically = true;
					group.verticalGroupLayout.StretchHorizontally = true;
					group.verticalGroupLayout.StretchVertically = true;
					group.StretchVertically = true;
					group.StretchHorizontally = true;
					group.GetCaptionElement().captionOffset = 0;
				}

				for (int i = this.panelBar.Items.Count - 1; i > -1; i--)
				{
					RadPanelBarGroupElement group = (RadPanelBarGroupElement)this.panelBar.Items[i];

					if (group.Visibility == ElementVisibility.Collapsed)
					{
						InsertToggleButton(group);
					}
				}

				if (selectedGroups)
				{
					if (this.contentPanel != null)
					{
						this.contentPanel.Visible = true;
					}

					if (selectedGroup != null)
					{
						this.captionElement.Text = selectedGroup.Caption;
                        MoveContent(selectedGroup);
                        //           DoSelect(new RadPanelBarGroupElement[] { selectedGroup });
                    }

					this.contentPanelHost.Visibility = ElementVisibility.Visible;
					this.itemsControl.ShowChildren();


          		}


			}
		}

        private void Items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
		{
			RadPanelBarGroupElement group = target as RadPanelBarGroupElement;
			if (target != null)
			{
				group.ShowCaptionButton(false);
				group.CollapseChildren(false);
				group.horizontalLayout.StretchHorizontally = true;
				group.verticalLayout.StretchHorizontally = true;
				group.verticalLayout.StretchVertically = true;
				group.verticalGroupLayout.StretchHorizontally = true;
				group.verticalGroupLayout.StretchVertically = true;
				group.StretchVertically = true;
				group.StretchHorizontally = true;
            	//	UpdateGroupsUI();
			}
		}


		private class RadAssociatedToggleButton : RadToggleButtonElement
		{
			internal RadPanelBarGroupElement group;
			public RadAssociatedToggleButton(RadPanelBarGroupElement group)
			{
				this.group = group;
                this.Enabled = group.Enabled;
			}
		}


		private class RadAssociatedMenuItem : RadMenuItem
		{
			internal RadPanelBarGroupElement group;
			public RadAssociatedMenuItem(RadPanelBarGroupElement group)
			{
				this.group = group;
			}

            protected override Type ThemeEffectiveType
            {
                get
                {
                    return typeof(RadMenuItem);
                }
            }
		}

		private void target_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
		{

		}

		private void RemoveToggleButton(RadPanelBarGroupElement group)
		{
			foreach (RadAssociatedToggleButton button in this.panelBarOverFlow.Items)
			{
				if (button.group.Equals(group))
				{
					this.panelBarOverFlow.Items.Remove(button);
					break;
				}
			}
		}

		private void InsertToggleButton(RadPanelBarGroupElement group)
		{
			RadAssociatedToggleButton toggleButton = new RadAssociatedToggleButton(group);

			int index = this.panelBar.Items.IndexOf(group);

			if (group == null)
				return;

			if (group.horizontalLayout == null)
				return;

			if (group.horizontalLayout == null)
				return;

			if (group.horizontalLayout.Children.Count == 0)
				return;


			Bitmap bmp = null;

			if ((group.horizontalLayout.Children[0] as RadPanelBarVisualElement).Image != null)
			{
				bmp = new Bitmap((group.horizontalLayout.Children[0] as RadPanelBarVisualElement).Image, 16, 16);
			}
			toggleButton.Image = bmp;



			if (toggleButton.Image == null)
			{
				toggleButton.Image = new Bitmap(16, 16);


				Assembly currentAssembly = Assembly.GetExecutingAssembly();
				string[] s = currentAssembly.GetManifestResourceNames();
				string p = "";
				foreach (string str in s)
				{
					if (str.EndsWith("2Telerik.bmp"))
						p = str;
				}
				if (p != "")
					toggleButton.Image = Image.FromStream(Telerik.WinControls.TelerikHelper.GetStreamFromResource(currentAssembly, p));

			}

            //set ToolTip text for toggle button
            if (group.ToolTipText != null && group.ToolTipText != String.Empty)
            {
                toggleButton.ToolTipText = group.ToolTipText;
            }
            else if (group.Caption != null && group.Caption != String.Empty)
            {
                toggleButton.ToolTipText = group.Caption;
            }

			int indx = this.panelBar.Items.Count - 1 - index;

			if (indx < this.panelBarOverFlow.Items.Count)
			{
				this.panelBarOverFlow.Items.Insert(indx, toggleButton);
			}
			else
			{
				this.panelBarOverFlow.Items.Add(toggleButton);
			}

			toggleButton.ToggleStateChanged += new StateChangedEventHandler(toggleButton_ToggleStateChanged);

			if (group.Selected)
			{
				toggleButton.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
			}
		}

        private void toggleButton_ToggleStateChanged(object sender, StateChangedEventArgs args)
		{
            if (args.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On)
            {
                RadAssociatedToggleButton toggleButton = sender as RadAssociatedToggleButton;
                    for (int i = 0; i < this.panelBarOverFlow.Items.Count; i++)
                    {
                        RadAssociatedToggleButton button = this.panelBarOverFlow.Items[i] as RadAssociatedToggleButton;
                        if (!button.Equals(toggleButton))
                        {
                            button.ToggleState = Telerik.WinControls.Enumerations.ToggleState.Off;
                        }
                    }

                    toggleButton.group.Selected = true;
                
            }
		}

		public override void SyncHostedPanels(RadPanelBarGroupElement[] groups, bool enableHostControlMode)
		{
            this.SetHostsBounds();
            foreach (RadPanelBarGroupElement group in groups)
            {
                if (group.Selected)
                    continue;

                if (enableHostControlMode)
                {
                    if (!group.verticalGroupLayout.Children.Contains(group.ContentPanelHost))
                    {
                        group.verticalGroupLayout.Children.Add(group.ContentPanelHost);
                    }
                }
                else
                {
                    if (group.verticalGroupLayout.Children.Contains(group.ContentPanelHost))
                    {
                        group.verticalGroupLayout.Children.Remove(group.ContentPanelHost);
                    }
                }

                group.ContentPanelHost.StretchVertically = true;
            }
		}

        private void explorerBarLayout_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
		{
			if (e.Property == RadItem.BoundsProperty)
			{
				SetHostsBounds();
			}
		}

        private void panelBar_RadPropertyChanged(object sender, Telerik.WinControls.RadPropertyChangedEventArgs e)
		{
			if (e.Property == RadItem.BoundsProperty
			 || e.Property == RadPanelBarElement.TopOffsetProperty
			 || e.Property == RadPanelBarElement.RightOffsetProperty
			 || e.Property == RadPanelBarElement.SpacingBetweenGroupsProperty
				 || e.Property == RadPanelBarElement.SpacingBetweenColumnsProperty
				 || e.Property == RadPanelBarElement.NumberOfColumnsProperty
					|| e.Property == RadPanelBarElement.BottomOffsetProperty
					|| e.Property == RadPanelBarElement.LeftOffsetProperty
					|| e.Property == RadPanelBarElement.PanelBarStyleProperty)
			{
			}

		}


	}
}
