using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace CodeCell.AgileMap.Core
{
    public class UIFieldTypeEditor:UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value != null && value.GetType() != typeof(string))
                return value;
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                ListBox  listBox = new ListBox();
                listBox.Tag = edSvc;
                listBox.SelectedIndexChanged += new EventHandler(listBox_SelectedIndexChanged);
                if (context.Instance is IFieldNamesProvider)
                {
                    IFieldNamesProvider prv = context.Instance as IFieldNamesProvider;
                    if (prv.Fields != null)
                    {
                        foreach (string fld in prv.Fields)
                            listBox.Items.Add(fld);
                    }
                }
                if(value != null)
                    listBox.Text = value.ToString();
                edSvc.DropDownControl(listBox);
                return listBox.Text;
            }
            return value;
        }

        void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ((sender as ListBox).Tag as IWindowsFormsEditorService).CloseDropDown();
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
}
