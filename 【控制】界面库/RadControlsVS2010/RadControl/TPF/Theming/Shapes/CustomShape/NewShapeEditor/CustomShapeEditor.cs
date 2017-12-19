using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Telerik.WinControls
{
    public class CustomShapeEditor : UITypeEditor
    {
        public CustomShapeEditor()
        {
        }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            CustomShape shape = value as CustomShape;
            //CustomShapeEditorForm editor = new CustomShapeEditorForm();

            //if (shape != null)
            {
                //editor.EditorControl.Dimension = shape.Dimension;
                //editor.EditorControl.Points.AddRange(shape.Points);
            }

            //if (editor.ShowDialog() == DialogResult.OK)
            {
                shape = new CustomShape();
                //shape.Points.AddRange(editor.EditorControl.Points);
                //shape.Dimension = editor.EditorControl.Dimension;

                return shape;
            }

            //return value;
        }
        public override void PaintValue(PaintValueEventArgs e)
        {
            //base.PaintValue(e);
            CustomShape shape = e.Value as CustomShape;
            if(shape != null)
                using (GraphicsPath path = shape.CreatePath(e.Bounds))
                    e.Graphics.DrawPath(Pens.Black, path);
        }
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
