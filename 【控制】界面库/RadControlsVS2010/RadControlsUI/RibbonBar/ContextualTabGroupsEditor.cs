using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace Telerik.WinControls.UI
{
    public class ContextualTabGroupsEditor : UITypeEditor
	{
        private IWindowsFormsEditorService editorService = null;
		//ArrayList shapes = null;
        private List<ContextualTabGroup> contextualTabGroups = null;
        private bool indexChanged;

		public ContextualTabGroupsEditor()
        {
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
			return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
			//shapes = new ArrayList();
			this.contextualTabGroups = new List<ContextualTabGroup>();
			editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			ListBox listBox = CreateListBox(context, value);
			
			indexChanged = false;

			editorService.DropDownControl(listBox);

			if (!indexChanged)
			{
				return value;
			}

			if (listBox.SelectedIndex == 0)
			{
				return null;
			}

			object result = this.contextualTabGroups[listBox.SelectedIndex-1];

			this.contextualTabGroups.Clear();
			
			return result;
        }

        private void listBox_SelectedValueChanged(object sender, EventArgs e)
        {
			indexChanged = true;
            if (editorService != null)
                editorService.CloseDropDown();
        }

		private ListBox CreateListBox(ITypeDescriptorContext context, object value)
		{		
			ListBox listBox = new ListBox();

			listBox.SelectedValueChanged += new EventHandler(listBox_SelectedValueChanged);
			listBox.Dock = DockStyle.Fill;
			listBox.BorderStyle = BorderStyle.None;
			listBox.ItemHeight = 13;

			listBox.Items.Add("(none)");

			//if (context.Container != null)
			{

				IReferenceService referenceService = (IReferenceService)context.GetService(typeof(IReferenceService));
				
				foreach(ContextualTabGroup group in referenceService.GetReferences(typeof(ContextualTabGroup)))
				{
					listBox.Items.Add(group.Text);
					this.contextualTabGroups.Add(group);

					if (value != null && value == group)
					{
						listBox.SelectedIndex = listBox.Items.Count - 1;
					}
				}
			}

			return listBox;
		}
	}
}
