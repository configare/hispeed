using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using Telerik.WinControls.Layouts;
using Telerik.WinControls;

namespace Telerik.WinControls.UI
{
	public class ListBarStyle : PanelBarStyleBase
	{
		//private RadPanelBarElement panelBar;
		private ListBarLayout listBarLayout;

		public ListBarStyle(RadPanelBarElement panelBar) : base(panelBar)
		{
			//this.panelBar = panelBar;
		}

		public override RadPanelBarElement GetRadPanelBarElement()
		{
			return this.panelBar;
		}

		internal protected override PanelBarBaseLayout GetBaseLayout()
		{
			return this.listBarLayout;
		}

     
		//protected override void DoSelect(RadPanelBarGroupElement[] groups)
		//{
		//    PanelBarGroupCancelEventArgs groupCancelArgs = new PanelBarGroupCancelEventArgs(groups[0], false);
		//    this.panelBar.CallPanelBarGroupSelecting(groupCancelArgs);
		//    if (groupCancelArgs.Cancel)
		//    {
		//        return;
		//    }

		//    this.panelBar.BeginUpdate();
		//    foreach (RadPanelBarGroupElement group in this.panelBar.Items)
		//    {
		//        group.Selected = false;
		//    }
		//    groups[0].Selected = true;
		//    this.panelBar.EndUpdate();

		//    PanelBarGroupEventArgs groupArgs = new PanelBarGroupEventArgs(groups[0]);
		//    this.panelBar.CallPanelBarGroupSelected(groupArgs);
		//}

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

		protected override void DoCollapse(RadPanelBarGroupElement[] groups)
		{
			DoExpandCollapse(groups);
		}

		protected override void DoExpand(RadPanelBarGroupElement[] groups)
		{
			DoExpandCollapse(groups);
		}


		private bool DoExpandCollapse(RadPanelBarGroupElement[] groups)
		{
			PanelBarGroupCancelEventArgs groupCancelArgs = new PanelBarGroupCancelEventArgs(groups[0], false);

			this.panelBar.CallPanelBarGroupExpanding(groupCancelArgs);

            if (groupCancelArgs.Cancel)
            {
                groups[0].Expanded = false;
                return false;
            }

			this.panelBar.BeginUpdate();
			foreach (RadPanelBarGroupElement group in this.panelBar.Items)
			{
				if (group.Expanded && group != groups[0])
				{
					PanelBarGroupCancelEventArgs groupCollapseCancelArgs = new PanelBarGroupCancelEventArgs(group, false);

					this.panelBar.CallPanelBarGroupCollapsing(groupCancelArgs);

					if (groupCollapseCancelArgs.Cancel)
					{
						this.panelBar.EndUpdate();
						return false;
					}

					group.Expanded = false;
					group.Expand(false);

					PanelBarGroupEventArgs groupCollapseArgs = new PanelBarGroupEventArgs(group);
					this.panelBar.CallPanelBarGroupCollapsed(groupCollapseArgs);
				}
			}

			groups[0].Expand(true);
			groups[0].Expanded = true;

			this.panelBar.EndUpdate();

			PanelBarGroupEventArgs groupArgs = new PanelBarGroupEventArgs(groups[0]);


			this.panelBar.CallPanelBarGroupExpanded(groupArgs);

			return true;
		}

		public override void SyncHostedPanels(RadPanelBarGroupElement[] groups, bool enableHostControlMode)
		{
			foreach (RadPanelBarGroupElement group in groups)
			{
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

		public override void CreateChildren()
		{
			this.listBarLayout = new ListBarLayout();
			this.panelBar.Children.Add(this.listBarLayout);
			this.panelBar.Items.Owner = this.listBarLayout;
			UpdateGroupsUI();
		}

		public override void WireEvents()
		{
			this.panelBar.Items.ItemsChanged += new ItemChangedDelegate(Items_ItemsChanged);
		}

		public override void UnWireEvents()
		{
			this.panelBar.Items.ItemsChanged -= new ItemChangedDelegate(Items_ItemsChanged);
		}

		protected internal override void UpdateGroupsUI()
		{
			if (this.panelBar.Items.Count > 0)
			{
				foreach (RadPanelBarGroupElement group in this.panelBar.Items)
				{
					group.ShowCaptionButton(false);
					group.GetCaptionElement().captionOffset = 0;
                    group.Visibility = ElementVisibility.Visible;
				}

				this.PerformAction(new RadPanelBarGroupElement[] { (RadPanelBarGroupElement)this.panelBar.Items[0] }, GroupAction.Expand);
			}
		}

        private void Items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
		{
			if (target != null)
			{
				if (this.panelBar.CurrentStyle is ListBarStyle)
				{
					RadPanelBarGroupElement group = (RadPanelBarGroupElement)target;

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
	}
}
