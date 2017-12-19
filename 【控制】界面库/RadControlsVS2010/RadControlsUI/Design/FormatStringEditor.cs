using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI.Design
{
    public class FormatStringEditor : UITypeEditor
    {
		// Fields
        private FormatStringDialog formatStringDialog;

        // Methods
        public FormatStringEditor()
        {
        }

		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return false;
		}

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                IWindowsFormsEditorService service1 = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
                if (service1 == null)
                {
                    return value;
                }
                DataGridViewCellStyle style1 = context.Instance as DataGridViewCellStyle;
                //ListControl control1 = context.Instance as ListControl;
				RadListBoxElement control1 = null;
				if (context.Instance is RadListBox)
				{
					control1 = ((RadListBox)context.Instance).RootElement.Children[0] as RadListBoxElement;
				}
				else if (context.Instance is RadListBoxElement)
				{ 
					control1 = context.Instance as RadListBoxElement; 
				}
				else if (context.Instance is RadComboBox)
				{
					control1 = ((RadComboBox)context.Instance).ComboBoxElement.ListBoxElement as RadListBoxElement;
				}
				else if (context.Instance is RadComboBoxElement)
				{
					control1 = ((RadComboBoxElement)context.Instance).ListBoxElement as RadListBoxElement;
				}				

                if (this.formatStringDialog == null)
                {
                    this.formatStringDialog = new FormatStringDialog(context);
                }
                if (control1 != null)
                {
                    this.formatStringDialog.ListControl = control1;
                }
                else
                {
                    this.formatStringDialog.DataGridViewCellStyle = style1;
                }
                IComponentChangeService service2 = (IComponentChangeService) provider.GetService(typeof(IComponentChangeService));
                if (service2 != null)
                {
                    if (style1 != null)
                    {
                        service2.OnComponentChanging(style1, TypeDescriptor.GetProperties(style1)["Format"]);
                        service2.OnComponentChanging(style1, TypeDescriptor.GetProperties(style1)["NullValue"]);
                        service2.OnComponentChanging(style1, TypeDescriptor.GetProperties(style1)["FormatProvider"]);
                    }
                    else
                    {
                        service2.OnComponentChanging(control1, TypeDescriptor.GetProperties(control1)["FormatString"]);
                        service2.OnComponentChanging(control1, TypeDescriptor.GetProperties(control1)["FormatInfo"]);
                    }
                }
                service1.ShowDialog(this.formatStringDialog);
                this.formatStringDialog.End();
                if (!this.formatStringDialog.Dirty)
                {
                    return value;
                }
                TypeDescriptor.Refresh(context.Instance);
                if (service2 == null)
                {
                    return value;
                }
                if (style1 != null)
                {
                    service2.OnComponentChanged(style1, TypeDescriptor.GetProperties(style1)["Format"], null, null);
                    service2.OnComponentChanged(style1, TypeDescriptor.GetProperties(style1)["NullValue"], null, null);
                    service2.OnComponentChanged(style1, TypeDescriptor.GetProperties(style1)["FormatProvider"], null, null);
                    return value;
                }
                service2.OnComponentChanged(control1, TypeDescriptor.GetProperties(control1)["FormatString"], null, null);
                service2.OnComponentChanged(control1, TypeDescriptor.GetProperties(control1)["FormatInfo"], null, null);
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}

