using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.Design;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI;
using System.Reflection;
using System.Resources;

namespace Telerik.WinControls.UI
{
	public class OverFlowAssociatedButton : RadToggleButtonElement
	{
		private RadPanelBarGroupElement group;
		private string groupCaption;

		public static RadProperty PressedProperty = RadProperty.Register(
		"Pressed", typeof(bool), typeof(OverFlowAssociatedButton),
		new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

		private RadPanelBarOverFlow panelBarOverFlow;

		public RadPanelBarOverFlow ParentPanelBarOverFlow
		{
			get
			{
				if (this.panelBarOverFlow == null)
				{
					for (RadElement res = this.Parent; res != null && panelBarOverFlow == null; res = res.Parent)
					{
						panelBarOverFlow = res as RadPanelBarOverFlow;
					}
				}
				return this.panelBarOverFlow;
			}
		}

		public bool Pressed
		{
			get
			{
				return (bool)this.GetValue(PressedProperty);
			}

			set
			{
				this.SetValue(PressedProperty, value);
			}
		}

		public Color SelectedColor1
		{
			get
			{
				return this.fillPrimitive.BackColor;
			}
			set
			{
				this.fillPrimitive.BackColor = value;
			}
		}

		public Color SelectedColor2
		{
			get
			{
				return this.fillPrimitive.BackColor2;
			}
			set
			{
				this.fillPrimitive.BackColor2 = value;
			}
		}

		public OverFlowAssociatedButton(RadPanelBarGroupElement group)
		{	
			this.group = group;
			this.groupCaption = group.Caption;
			this.SetImage();
			if (!this.group.Selected)
				PerformMutualExclusion();
			else
				this.ToggleState = Telerik.WinControls.Enumerations.ToggleState.On;
		}

		public string Caption
		{
			get
			{
				return this.groupCaption;
			}
		}

		private void SetImage()
		{
			this.Image = this.group.Image;
			this.ImageIndex = this.group.ImageIndex;
			this.ImageKey = this.group.ImageKey;

			if ((this.Image == null) && (this.ImageIndex == -1))
			{
				this.Image = new Bitmap(16, 16);

				Assembly currentAssembly = Assembly.GetExecutingAssembly();
				string[] s = currentAssembly.GetManifestResourceNames();
				string p = "";
				foreach (string str in s)
				{
					if (str.EndsWith("2Telerik.bmp"))
						p = str;
				}
				if (p != "")
					this.Image = Image.FromStream(Telerik.WinControls.TelerikHelper.GetStreamFromResource(currentAssembly, p));
			}

		}

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == PressedProperty)
			{
				foreach (RadElement child in this.ChildrenHierarchy)
				{
					child.SetValue(PressedProperty, e.NewValue);
				}
			}

		
		}

		protected override Type ThemeEffectiveType
		{
			get
			{
				return typeof(RadToggleButtonElement);
			}
		}

		public void SetInitialState()
		{
			this.ToggleState = Telerik.WinControls.Enumerations.ToggleState.Off;
		}

		public void SetSelectedState()
		{
		}

		private void PerformMutualExclusion()
		{
			if (this.ParentPanelBarOverFlow != null)
			{
				foreach (OverFlowAssociatedButton button in this.ParentPanelBarOverFlow.Items)
				{
					if (button != this)
					{
						button.SetInitialState();
					}
				}
			}
			else
				this.ToggleState = Telerik.WinControls.Enumerations.ToggleState.Off;

		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);

			PerformMutualExclusion();
			this.group.PerformSelect();
		}
	}
}
