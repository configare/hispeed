using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
	public enum GroupAction
	{
		Expand,
		Collapse,
		Select,
		UnSelect,
	}

	public abstract class PanelBarStyleBase
	{
		protected RadPanelBarElement panelBar = null;

		public abstract RadPanelBarElement GetRadPanelBarElement();
		public abstract void CreateChildren();
      
		protected PanelBarStyleBase(RadPanelBarElement panelBar)
		{
			this.panelBar = panelBar;
		}

		protected virtual void DoExpand(RadPanelBarGroupElement[] groups)
		{
		}

		protected virtual void DoCollapse(RadPanelBarGroupElement[] groups)
		{
		}

		protected virtual void DoSelect(RadPanelBarGroupElement[] groups)
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
			this.panelBar.EndUpdate();

			PanelBarGroupEventArgs groupArgs = new PanelBarGroupEventArgs(groups[0]);
			this.panelBar.CallPanelBarGroupSelected(groupArgs);
		}

		protected virtual void DoUnSelect(RadPanelBarGroupElement[] groups)
		{
		}

		protected internal virtual void UpdateGroupsUI()
		{
		}

		public abstract void SyncHostedPanels(RadPanelBarGroupElement[] groups, bool enableHostControlMode);

		internal protected virtual PanelBarBaseLayout GetBaseLayout()
		{
			return null;
		}


		public virtual void WireEvents()
		{
		}

		public virtual void UnWireEvents()
		{
		}

		public virtual void PerformAction(RadPanelBarGroupElement[] groups, GroupAction action)
		{
			switch (action)
			{
				case GroupAction.Expand:
					{
						this.DoExpand(groups);
						break;
					}
				case GroupAction.Collapse:
					{
						this.DoCollapse(groups);
						break;
					}
				case GroupAction.Select:
					{
						this.DoSelect(groups);
						break;
					}
				case GroupAction.UnSelect:
					{
						this.DoUnSelect(groups);
						break;
					}
			}
		}
	}
}
