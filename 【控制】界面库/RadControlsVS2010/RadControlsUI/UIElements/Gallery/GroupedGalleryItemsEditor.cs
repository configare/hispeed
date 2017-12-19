using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Security.Permissions;

namespace Telerik.WinControls.UI
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode), PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	public class GroupedGalleryItemsEditor : UITypeEditor
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
				RadGalleryGroupItem group = context.Instance as RadGalleryGroupItem;
				RadGalleryElement owner = null;

				if (collection1 == null ||
					group == null)
				{
					return value;
				}
				else
				{
                    if (group.Owner == null && selectionService != null && selectionService.PrimarySelection != null && selectionService.PrimarySelection is RadGalleryElement)
                    {
                        group.Owner = (RadGalleryElement) selectionService.PrimarySelection;
                    }
					owner = group.Owner;
					if (owner == null)
					{
                        
						return value;
					}
				}
				if (this.groupedItemsUI == null)
				{
					this.groupedItemsUI = new GroupedItemsEditorUI();
				}
				componentChangeService.OnComponentChanging(context.Instance, TypeDescriptor.GetProperties(context.Instance)["Items"]);
				this.groupedItemsUI.Start(windowsFormsEditorService, typeDiscoveryService, designerHost, collection1, group, owner); //(this, collection1, group, owner)

				if (windowsFormsEditorService.ShowDialog(this.groupedItemsUI) == DialogResult.OK)
				{
					this.groupedItemsUI.End();
					value = this.groupedItemsUI.Value;
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

		private GroupedItemsEditorUI groupedItemsUI;
	}
}
