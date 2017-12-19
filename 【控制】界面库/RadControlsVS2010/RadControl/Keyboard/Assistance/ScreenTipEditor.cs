using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Security.Permissions;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;

namespace Telerik.WinControls
{
	[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode), PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	public class ScreenTipEditor : UITypeEditor
	{
		public ScreenTipEditor()
		{
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (provider != null)
			{
				IWindowsFormsEditorService service1 = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				ITypeDiscoveryService service2 = (ITypeDiscoveryService)provider.GetService(typeof(ITypeDiscoveryService));
				IDesignerHost service3 = (IDesignerHost)provider.GetService(typeof(IDesignerHost));
				IComponentChangeService service4 = (IComponentChangeService)provider.GetService(typeof(IComponentChangeService));

				if (service1 == null)
				{
					return value;
				}
				if (this.screenTipUI == null)
				{
					this.screenTipUI = new ScreenTipUI(this);
				}
				//service4.OnComponentChanging(context.Instance, TypeDescriptor.GetProperties(context.Instance)["ScreenTip"]);
                this.screenTipUI.Initialize(service1, service2, service3, service4, value);
				service1.DropDownControl(this.screenTipUI);
				if (this.screenTipUI.Value != null )
				{
					value = this.screenTipUI.Value;
				}
				this.screenTipUI.End();
				//service4.OnComponentChanged(context.Instance, TypeDescriptor.GetProperties(context.Instance)["ScreenTip"], null, null);
			}
			return value;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		// Fields
		private ScreenTipUI screenTipUI;
	}
}
