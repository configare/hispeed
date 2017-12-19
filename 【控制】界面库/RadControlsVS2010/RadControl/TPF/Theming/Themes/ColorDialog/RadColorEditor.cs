using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls
{

	
	/// <summary>
	/// Represents the method that will handle the ColorChanged event.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	public delegate void ColorChangedEventHandler(object sender, ColorChangedEventArgs args);

    /// <exclude/>
	public class RadColorEditor: UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		public static IColorSelector CreateColorSelectorInstance()
		{
            Type colorSelectorType;
            colorSelectorType = Type.GetType("Telerik.WinControls.UI.RadColorSelector");
            if (colorSelectorType == null)
            {
                colorSelectorType = Type.GetType(string.Format("Telerik.WinControls.UI.RadColorSelector, Telerik.WinControls.UI, Version={0}, Culture=neutral, PublicKeyToken=5bb2a467cbec794e", VersionNumber.Number));
            }

			if (colorSelectorType != null)
			{
				return (IColorSelector)Activator.CreateInstance(colorSelectorType);
			}

			throw new InvalidOperationException("Unable to locate color selector: Telerik.WinControls.UI.RadColorSelector. Please add reference to assembly Telerik.WinControls.UI.dll");
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
            IRadColorDialog dialogForm = RadColorEditor.CreateColorDialogInstance(); 

			UserControl colorSelector = dialogForm.RadColorSelector;// RadColorEditor.CreateColorSelectorInstance() as UserControl;

		    bool isHslColor = false;

            if (value != null && value.GetType() == typeof(Color))
            {
                ((IColorSelector) colorSelector).SelectedColor = (Color) value;
                ((IColorSelector) colorSelector).OldColor = (Color) value;
            }
            else if (value != null && value.GetType() == typeof(HslColor))
            {
                ((IColorSelector)colorSelector).SelectedHslColor = (HslColor)value;
                ((IColorSelector)colorSelector).OldColor = ((HslColor)value).RgbValue;

                isHslColor = true;
            }

			if (value == null)
				return null;

            if (((Form)dialogForm).ShowDialog() == DialogResult.OK)
            {
                if (isHslColor)
                {
                    return dialogForm.SelectedHslColor;
                }
                else
                {
                    return dialogForm.SelectedColor;
                }
            }

		    return value;
		}

		public override void PaintValue(PaintValueEventArgs e)
		{
		    Color color;
            if (e.Value != null && e.Value.GetType() == typeof(HslColor) )
            {
                color = ((HslColor)e.Value).RgbValue;
            }
            else
            {
                if (e.Value != null)
                {
                    color = (Color)e.Value;
                }
                else
                {
                    color = Color.Empty;
                }
            }

		    if (e.Value != null)
				using (Brush brush = new SolidBrush(color))
					e.Graphics.FillRectangle(brush, e.Bounds);
		}

		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}

        public static IRadColorDialog CreateColorDialogInstance()
        {
            Type colorDialogType;
            colorDialogType = Type.GetType("Telerik.WinControls.UI.RadColorDialogForm");
            if (colorDialogType == null)
            {
                colorDialogType = Type.GetType(string.Format("Telerik.WinControls.UI.RadColorDialogForm, Telerik.WinControls.UI, Version={0}, Culture=neutral, PublicKeyToken=5bb2a467cbec794e", VersionNumber.Number));
            }

            if (colorDialogType != null)
            {
                return (IRadColorDialog)Activator.CreateInstance(colorDialogType);
            }

            throw new InvalidOperationException("Unable to locate color selector: Telerik.WinControls.UI.RadColorDialogForm. Please add reference to assembly Telerik.WinControls.UI.dll");
        }
    }

	//events raised when the buttons are clicked
	//public delegate void OKButtonClickedEventHandler(object sender, ColorChangedEventArgs args);
	//public delegate void CancelButtonClickedEventHandler(object sender, ColorChangedEventArgs args);
}
