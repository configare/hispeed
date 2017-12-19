using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Themes.Design;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class RadToolStripDesignTimeData : RadControlDesignTimeData
	{
        public RadToolStripDesignTimeData()            
        { }

        public RadToolStripDesignTimeData(string name)
            : base(name)
        {}

		public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
		{
			RadToolStripElement element = new RadToolStripElement();
            element.Size = new Size(600, element.Size.Height);
            element.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			RadToolStripItem item = new RadToolStripItem();
            item.Size = new Size(600, item.Size.Height);
            item.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			element.Items.Add(item);
			item.Items.Add(new RadButtonElement("one"));
			item.Items.Add(new RadDropDownButtonElement());
            item.Items.Add(new RadToolStripSeparatorItem());
            item.Items.Add(new RadToggleButtonElement());
            item.Items.Add(new RadRepeatButtonElement());
            item.Items.Add(new RadImageButtonElement());
            item.Items.Add(new RadRadioButtonElement());
            item.Items.Add(new RadCheckBoxElement());
            item.Items.Add(new RadTextBoxElement());
            item.Items.Add(new RadMaskedEditBoxElement());
            item.Items.Add(new RadSplitButtonElement());
            item.Items.Add(new RadApplicationMenuButtonElement());

            RadComboBoxElement combo = new RadComboBoxElement();
            combo.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			item.Items.Add(combo);

			RadTextBoxElement textBoxElement = new RadTextBoxElement();
			textBoxElement.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			item.Items.Add(textBoxElement);

			RadToolStrip toolStrip = new RadToolStrip();
            toolStrip.BeginInit();
			toolStrip.AutoSize = false;

			toolStrip.Bounds = new Rectangle(30, 30, 400, 50);
			toolStrip.Items.Add(element);
			toolStrip.AllowFloating = false;
			toolStrip.AllowDragging = false;
            toolStrip.EndInit();
		
			RadToolStrip toolStripStructure = new RadToolStrip();
            toolStripStructure.BeginInit();
			toolStripStructure.AllowDragging = false;
			toolStripStructure.AllowFloating = false;
			toolStripStructure.AutoSize = false;

			RadToolStripElement structureElement = new RadToolStripElement();
            structureElement.Size = new Size(600, structureElement.Size.Height);
            structureElement.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			RadToolStripItem structureItem = new RadToolStripItem();
            structureItem.Size = new Size(600, structureItem.Size.Height);
            structureItem.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
		
			structureElement.Items.Add(structureItem);
			structureItem.Items.Add(new RadButtonElement("one"));
			structureItem.Items.Add(new RadDropDownButtonElement());
			structureItem.Items.Add(new RadToolStripSeparatorItem());
            structureItem.Items.Add(new RadToggleButtonElement());
            structureItem.Items.Add(new RadRepeatButtonElement());
            structureItem.Items.Add(new RadImageButtonElement());
            structureItem.Items.Add(new RadRadioButtonElement());
            structureItem.Items.Add(new RadCheckBoxElement());
            structureItem.Items.Add(new RadTextBoxElement());
            structureItem.Items.Add(new RadMaskedEditBoxElement());
            structureItem.Items.Add(new RadSplitButtonElement());
            structureItem.Items.Add(new RadApplicationMenuButtonElement());
            combo = new RadComboBoxElement();
            combo.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            structureItem.Items.Add(combo);

			textBoxElement = new RadTextBoxElement();
			textBoxElement.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
			structureItem.Items.Add(textBoxElement);

			toolStripStructure.Bounds = new Rectangle(30, 30, 450, 50);
		
			toolStripStructure.Items.Add(structureElement);

            toolStripStructure.EndInit();
						
			ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(toolStrip, toolStripStructure.RootElement);
			designed.MainElementClassName = typeof(RadToolStripElement).FullName;
			ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();
			res.Add(designed);

			return res;
		}
    }
}
