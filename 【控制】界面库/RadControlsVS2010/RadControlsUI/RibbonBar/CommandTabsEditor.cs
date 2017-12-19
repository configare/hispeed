using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel.Design;
using System.ComponentModel;

namespace Telerik.WinControls.UI.RibbonBar
{
    public class CommandTabsEditor : UITypeEditor
    {
        public CommandTabsEditor()
        {

        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            RadItemCollection commandTabCollection = value as RadItemCollection;

            if (commandTabCollection != null)
            {
                IComponentChangeService service = (IComponentChangeService)provider.GetService(typeof(IComponentChangeService));

                service.OnComponentChanging(context.Instance, TypeDescriptor.GetProperties(context.Instance)["CommandTabs"]);

                CommandTabsUITypeEditorForm editorForm = new CommandTabsUITypeEditorForm(provider.GetService(typeof(IDesignerHost)) as IDesignerHost, commandTabCollection);

                if (editorForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    service.OnComponentChanged(context.Instance, TypeDescriptor.GetProperties(context.Instance)["CommandTabs"], null, null);
                }
            }

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
