using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using Telerik.WinControls.Assistance;
using System.ComponentModel.Design;
using System.Collections;
using Telerik.WinControls.Elements;

namespace Telerik.WinControls
{
	[ToolboxItem(false), ComVisible(false)]
	internal partial class ScreenTipUI : UserControl
	{
        // Fields
        private object originalValue;
        private IScreenTipContent currentValue;
        private bool updateCurrentValue;
        private ScreenTipEditor editor;
        private IWindowsFormsEditorService edSvc;
        private IComponentChangeService changeSvc;
        private IDesignerHost host;
        private ITypeDiscoveryService typeSvc;
        private ICollection templates;

		#region Constructors
		public ScreenTipUI()
			: this(null)
		{
		}

		public ScreenTipUI(ScreenTipEditor editor)
		{
			this.editor = editor;
			this.End();
			this.InitializeComponent();
		} 
		#endregion

		#region Properties
		public IWindowsFormsEditorService EditorService
		{
			get
			{
				return this.edSvc;
			}
		}

		public bool ShouldPersistValue
		{
			get
			{
				if (this.currentValue != null && (this.currentValue as RadItem).SerializeChildren)
				{
					foreach (RadItem item in this.currentValue.TipItems)
					{
						if (item.SerializeProperties)
						{
							return true;							
						}
					}
				}
				return false;
			}
		}

		public object Value
		{
			get
			{
				if (this.ShouldPersistValue)//this.currentValue != (this.originalValue as IScreenTipContent)
				{
					return this.currentValue;
				}
				return null;
			}
		}
		#endregion

        public void Initialize(IWindowsFormsEditorService edSvc, ITypeDiscoveryService typeSvc, IDesignerHost host, IComponentChangeService changeSvc ,object value)
        {
            this.host = host;
            this.edSvc = edSvc;
            this.typeSvc = typeSvc;
            this.changeSvc = changeSvc;
            this.currentValue = value as IScreenTipContent;
            this.originalValue = value;
            this.updateCurrentValue = false;
            if (templates == null)
            {
                templates = this.typeSvc.GetTypes(typeof(IScreenTipContent), false);
                if (templates == null)
                {
                    return;
                }
                foreach (Type type in templates)
                {
                    if (type.IsAbstract || type.IsInterface || type.IsAbstract || !type.IsPublic || 
                        this.templateBox.Items.Contains(type) || !type.IsVisible || typeof(Control).IsAssignableFrom(type))
                    {
                        continue;
                    }
                    this.templateBox.Items.Add(type);
                }
            }
            if (this.currentValue != null)
            {
                Type templateType = (value as IScreenTipContent).TemplateType;
                

                int typeIndex = templateType != null ?  this.templateBox.Items.IndexOf(templateType) :  -1;

                if (typeIndex > -1)
                {
                    this.templateBox.SelectedItem = this.templateBox.Items[typeIndex];
                }

                PopulateElementsBox(this.currentValue.TipItems);
            }
            this.updateCurrentValue = true;
        }

        public void End()
        {
            this.typeSvc = null;
            this.host = null;
            this.editor = null;
            this.edSvc = null;
            this.originalValue = null;
            this.currentValue = null;
			//this.chordConverter = null;
        }

		private void templateBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			//System.Diagnostics.Debug.WriteLine("SelectedIndexChanged: " + this.templateBox.SelectedIndex.ToString());

			if (!this.updateCurrentValue)
			{
				return;
			}
            //by fdc,his.templateBox.SelectedItem => this.templateBox.SelectedItem.GetType()
			if (this.currentValue != null && this.templateBox.SelectedItem.GetType() == this.currentValue.GetType())
			{
				return;
			}

            this.currentValue = null;
			if (this.templateBox.SelectedIndex != 0)
			{
				//this.currentValue = Activator.CreateInstance((this.templateBox.SelectedItem as TypeItem).TemplateType) as IScreenTipContent;// as RadScreenTipElement
				if (this.originalValue != null)
				{
					this.host.DestroyComponent(this.originalValue as IComponent);
				}
				this.currentValue = this.host.CreateComponent(this.templateBox.SelectedItem as Type) as IScreenTipContent;// as RadScreenTipElement
				if (this.currentValue is Control)
				{
					IScreenTipContent tempValue = (this.currentValue as RadScreenTip).ScreenTipElement;
					tempValue.TemplateType = (this.currentValue as RadScreenTip).TemplateType;
					this.currentValue = tempValue;
				}
				//this.persistChange = true;
				this.updateCurrentValue = true;
                //this.stylableElements.Items.Clear();
				PopulateElementsBox(this.currentValue.TipItems);
			}
		}

		private void stylableElements_SelectedIndexChanged(object sender, EventArgs e)
		{
			InstanceItem instanceItem = this.stylableElements.SelectedItem as InstanceItem;
			this.propertyGrid1.SelectedObject = (instanceItem != null) ? instanceItem.Instance : null;
		}

		protected void PopulateElementsBox(RadItemReadOnlyCollection elements)
		{
			this.stylableElements.Items.Clear();
			for (int i = 0; i < elements.Count; i++)
			{
				this.stylableElements.Items.Add(new InstanceItem(elements[i]));			
			}	
		}

		protected class InstanceItem
		{
			public InstanceItem()
			{
			}

			public InstanceItem(RadItem instance)
			{
				this.instance = instance;
			}

			// Fields
			private RadItem instance;

			public RadItem Instance
			{
				get
				{
					return instance;
				}
				set
				{
					instance = value;
				}
			}

			public override string ToString()
			{
				return instance.Class;
			}
		}
	}
}
