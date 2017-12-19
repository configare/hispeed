using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Paint;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls
{   /// <exclude/>
	public class RadFillEditor: UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (value == null)
			{
				return null;
			}

			RadGradientDialog dialog = new RadGradientDialog();
			FillPrimitive original = (FillPrimitive)value;

			PropertyDescriptorCollection originalProperties = TypeDescriptor.GetProperties(original);

			dialog.Fill.BackColor = (Color) originalProperties.Find("BackColor", true).GetValue(original);
			dialog.Fill.BackColor2 = (Color) originalProperties.Find("BackColor2", true).GetValue(original);
			dialog.Fill.BackColor3 = (Color) originalProperties.Find("BackColor3", true).GetValue(original);
			dialog.Fill.BackColor4 = (Color) originalProperties.Find("BackColor4", true).GetValue(original);

			dialog.Fill.GradientAngle = (float)originalProperties.Find("GradientAngle", true).GetValue(original);
			dialog.Fill.GradientPercentage = (float)originalProperties.Find("GradientPercentage", true).GetValue(original);
			dialog.Fill.GradientPercentage2 = (float)originalProperties.Find("GradientPercentage2", true).GetValue(original);
			dialog.Fill.GradientStyle = (GradientStyles)originalProperties.Find("GradientStyle", true).GetValue(original);

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				originalProperties.Find("BackColor", false).SetValue(original, dialog.Fill.BackColor);
				originalProperties.Find("BackColor2", false).SetValue(original, dialog.Fill.BackColor2);
				originalProperties.Find("BackColor3", false).SetValue(original, dialog.Fill.BackColor3);
				originalProperties.Find("BackColor4", false).SetValue(original, dialog.Fill.BackColor4);

				originalProperties.Find("GradientAngle", false).SetValue(original, dialog.Fill.GradientAngle);
				originalProperties.Find("GradientPercentage", false).SetValue(original, dialog.Fill.GradientPercentage);
				originalProperties.Find("GradientPercentage2", false).SetValue(original, dialog.Fill.GradientPercentage2);
				originalProperties.Find("GradientStyle", false).SetValue(original, dialog.Fill.GradientStyle);
			}

			return value;
		}

		public override void PaintValue(PaintValueEventArgs e)
		{
			if (e.Value != null && e.Value is FillPrimitive)
			{
				FillPrimitive fill = (e.Value as FillPrimitive);

                using (Brush b = new SolidBrush(Color.Transparent))
                {
                    Bitmap bmp = fill.GetAsBitmap(b, 0f, new SizeF(1, 1));
					if (bmp != null)
					{
						e.Graphics.DrawImage(bmp, e.Bounds);
						bmp.Dispose();
					}
                }
			}
		}

		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
