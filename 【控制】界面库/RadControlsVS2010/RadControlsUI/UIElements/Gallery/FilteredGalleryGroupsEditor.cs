using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode), PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    public class FilteredGalleryGroupsEditor : UITypeEditor
	{
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider != null)
			{
				IWindowsFormsEditorService windowsFormsEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				ITypeDiscoveryService typeDiscoveryService = (ITypeDiscoveryService)provider.GetService(typeof(ITypeDiscoveryService));
				IDesignerHost designerHost = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
				IComponentChangeService componentChangeService = (IComponentChangeService)provider.GetService(typeof(IComponentChangeService));
                ISelectionService selectionService = (ISelectionService)provider.GetService(typeof(ISelectionService));

				if (windowsFormsEditorService == null)
				{
					return value;
				}

				RadItemOwnerCollection collection1 = value as RadItemOwnerCollection;
				RadGalleryGroupFilter filter = context.Instance as RadGalleryGroupFilter;
				RadGalleryElement owner = null;

				if (collection1 == null ||
					filter == null)
				{
					return value;
				}
				else
				{
                    if (filter.Owner == null && selectionService != null && selectionService.PrimarySelection != null)
                    {
                        filter.Owner = (RadGalleryElement)selectionService.PrimarySelection;
                    }
					owner = filter.Owner;
					if (owner == null)
					{
						return value;
					}
				}
				if (this.filteredItemsUI == null)
				{
					this.filteredItemsUI = new FilteredItemsEditorUI();
				}
				componentChangeService.OnComponentChanging(context.Instance, TypeDescriptor.GetProperties(context.Instance)["Items"]);
				this.filteredItemsUI.Start(windowsFormsEditorService, typeDiscoveryService, designerHost, collection1, filter, owner);

				if (windowsFormsEditorService.ShowDialog(this.filteredItemsUI) == DialogResult.OK)
				{
					this.filteredItemsUI.End();
					value = this.filteredItemsUI.Value;
					componentChangeService.OnComponentChanged(context.Instance, TypeDescriptor.GetProperties(context.Instance)["Items"], null, null);
					return value;
				}				
			}
			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		private FilteredItemsEditorUI filteredItemsUI;
	}
}
