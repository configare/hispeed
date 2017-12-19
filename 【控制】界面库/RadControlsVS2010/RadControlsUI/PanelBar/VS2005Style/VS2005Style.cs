using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
	public class VS2005Style : PanelBarStyleBase
	{
		//private RadPanelBarElement panelBar;
		private VS2005StyleLayout groupLayout;

		public VS2005Style(RadPanelBarElement panelBar) : base(panelBar)
		{
			//this.panelBar = panelBar;
		}

		protected internal override PanelBarBaseLayout GetBaseLayout()
		{
			return this.groupLayout;
		}

		protected override void DoUnSelect(RadPanelBarGroupElement[] groups)
		{
			PanelBarGroupCancelEventArgs groupCancelArgs = new PanelBarGroupCancelEventArgs(groups[0], false);

			this.panelBar.CallPanelBarGroupUnSelecting(groupCancelArgs);

			if (groupCancelArgs.Cancel)
				return;

			this.panelBar.BeginUpdate();
			foreach (RadPanelBarGroupElement group in this.panelBar.Items)
			{
				group.Selected = false;
			}

			this.panelBar.EndUpdate();

			PanelBarGroupEventArgs groupArgs = new PanelBarGroupEventArgs(groups[0]);

			this.panelBar.CallPanelBarGroupUnSelected(groupArgs);
		}

		public override void SyncHostedPanels(RadPanelBarGroupElement[] groups, bool enableHostControlMode)
		{
			if (this.panelBar.CurrentStyle is VS2005Style)
			{
				foreach (RadPanelBarGroupElement group in groups)
				{
					if (enableHostControlMode)
					{
						if (!group.verticalGroupLayout.Children.Contains(group.ContentPanelHost))
						{
							group.verticalGroupLayout.Children.Add(group.ContentPanelHost);
						}

						group.ContentPanelHost.StretchVertically = false;
						group.ContentPanel.MinimumSize = new Size(0, 20);
						group.ContentPanel.AutoSize = true;
                        
					}

					else
					{
						group.ResetContentPanelProperties();

						if (group.verticalGroupLayout.Children.Contains(group.ContentPanelHost))
						{
							group.verticalGroupLayout.Children.Remove(group.ContentPanelHost);
						}

					}
				}
			}
		}

		public override RadPanelBarElement GetRadPanelBarElement()
		{
			return this.panelBar;
		}

		protected override void DoExpand(RadPanelBarGroupElement[] groups)
		{
			PanelBarGroupCancelEventArgs groupCancelArgs = new PanelBarGroupCancelEventArgs(groups[0], false);

			this.panelBar.CallPanelBarGroupExpanding(groupCancelArgs);

			if (groupCancelArgs.Cancel)
				return;

			this.panelBar.BeginUpdate();
			for (int i = 0; i < groups.Length; i++)
			{
				RadPanelBarGroupElement group = groups[i];
				group.Expanded = true;
				group.Expand(true);
			}

			this.panelBar.EndUpdate();

			PanelBarGroupEventArgs groupArgs = new PanelBarGroupEventArgs(groups[0]);


			UpdateGroupsUI();
			this.panelBar.CallPanelBarGroupExpanded(groupArgs);


			return;
		}

		protected override void DoCollapse(RadPanelBarGroupElement[] groups)
		{
			PanelBarGroupCancelEventArgs groupCancelArgs = new PanelBarGroupCancelEventArgs(groups[0], false);

			this.panelBar.CallPanelBarGroupCollapsing(groupCancelArgs);

			if (groupCancelArgs.Cancel)
				return;

			this.panelBar.BeginUpdate();
			for (int i = 0; i < groups.Length; i++)
			{
				RadPanelBarGroupElement group = groups[i];
				group.Expanded = false;
				group.Expand(false);
			}

			this.panelBar.EndUpdate();

			PanelBarGroupEventArgs groupArgs = new PanelBarGroupEventArgs(groups[0]);


			UpdateGroupsUI();

			this.panelBar.CallPanelBarGroupCollapsed(groupArgs);


			return;
		}

		public override void CreateChildren()
		{
			this.groupLayout = new VS2005StyleLayout();
			this.panelBar.Children.Add(this.groupLayout);

			UpdateGroupsUI();

			this.panelBar.Items.Owner = this.groupLayout;

		}

		public override void WireEvents()
		{
			this.panelBar.RadPropertyChanged += new Telerik.WinControls.RadPropertyChangedEventHandler(panelBar_RadPropertyChanged);
			this.groupLayout.RadPropertyChanged += new RadPropertyChangedEventHandler(explorerBarLayout_RadPropertyChanged);
			this.panelBar.Items.ItemsChanged += new ItemChangedDelegate(Items_ItemsChanged);
		}

		public override void UnWireEvents()
		{
			this.panelBar.RadPropertyChanged -= new Telerik.WinControls.RadPropertyChangedEventHandler(panelBar_RadPropertyChanged);
			this.groupLayout.RadPropertyChanged -= new RadPropertyChangedEventHandler(explorerBarLayout_RadPropertyChanged);
			this.panelBar.Items.ItemsChanged -= new ItemChangedDelegate(Items_ItemsChanged);
		}

		protected internal override void UpdateGroupsUI()
		{
			if (this.panelBar.Items.Count > 0)
			{
				foreach (RadPanelBarGroupElement group in this.panelBar.Items)
				{
                   
					if (group.GetCaptionButton() != null)
					{
						group.AutoSetCaptionButtonPosition(); 
						group.ShowCaptionButton(true);

						group.GetCaptionButton().GetGroupState().PanelBarStyle = PanelBarStyles.VisualStudio2005ToolBox;

						if (group.Expanded)
						{
							group.GetCaptionButton().ChangeStyle(Telerik.WinControls.Primitives.GroupStatePrimitive.GroupState.Expanded);
						}
						else
						{
							group.GetCaptionButton().ChangeStyle(Telerik.WinControls.Primitives.GroupStatePrimitive.GroupState.Collapsed);
						}

						group.GetCaptionButton().PerformLayout();
					}

                    if (!group.Expanded)
                    {
                        group.CollapseChildren(false);
                    }

					group.horizontalLayout.StretchHorizontally = true;
					group.verticalLayout.StretchHorizontally = true;
					group.verticalLayout.StretchVertically = true;
					group.verticalGroupLayout.StretchHorizontally = true;
					group.verticalGroupLayout.StretchVertically = true;
					group.StretchVertically = true;
					group.StretchHorizontally = true;
					group.GetCaptionElement().captionOffset = 20;
                    group.Visibility = ElementVisibility.Visible;
                }


			}
		}

        private void Items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
		{
			if (this.panelBar.CurrentStyle is VS2005Style)
			{
				RadPanelBarGroupElement group = target as RadPanelBarGroupElement;
				if (target != null)
				{
					target.RadPropertyChanged += new RadPropertyChangedEventHandler(target_RadPropertyChanged);
					group.ShowCaptionButton(true);
					UpdateGroupsUI();
					group.horizontalLayout.StretchHorizontally = true;
					group.verticalLayout.StretchHorizontally = true;
					group.verticalLayout.StretchVertically = true;
					group.verticalGroupLayout.StretchHorizontally = true;
					group.verticalGroupLayout.StretchVertically = true;
					group.StretchVertically = true;
					group.StretchHorizontally = true;
				}
			}
		}


        private void target_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
		{
			if (e.Property == RadItem.BoundsProperty)
			{
				this.RestrictPanelWidths();
				SyncHostedPanels(new RadPanelBarGroupElement[] { sender as RadPanelBarGroupElement },
					(sender as RadPanelBarGroupElement).EnableHostControlMode);
			}
		}

        private void explorerBarLayout_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
		{
			if (e.Property == RadItem.BoundsProperty)
			{
				RestrictPanelWidths();
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


				RestrictPanelWidths();
		}


		private void RestrictPanelWidths()
		{
			if (this.panelBar.CurrentStyle is VS2005Style)
			{
				foreach (RadPanelBarGroupElement group in this.panelBar.Items)
				{
					if (group.EnableHostControlMode)
					{
						group.ContentPanel.MaximumSize = new Size((int)group.ControlBoundingRectangle.Width, group.ContentPanel.MaximumSize.Height);
						group.ContentPanel.MinimumSize = new Size((int)group.ControlBoundingRectangle.Width, group.ContentPanel.MaximumSize.Height);


						int height = 0;
						foreach (Control control in group.ContentPanel.Controls)
						{
							height = Math.Max(height, control.Bounds.Bottom);
						}

						height = Math.Max(group.ContentPanel.MinimumSize.Height, height);
						group.ContentPanel.MaximumSize = new Size((int)group.ControlBoundingRectangle.Width, height);


					}
				}
			}
		}
	}
}
