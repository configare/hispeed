using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace CodeCell.AgileMap.Core
{
    public class UIFeatureRendererTypeEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            try
            {
                if (!(value is IFeatureRenderer))
                    return value;
                RotateFieldDef rotateFieldDef = (value as IFeatureRenderer).RotateFieldDef;
                IFeatureRenderEnvironment env = (value as BaseFeatureRenderer)._environment;
                IFeatureRenderer retRender = null;
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                if (edSvc != null)
                {
                        using (frmFeatureRendererEditor frm = new frmFeatureRendererEditor(context.Instance as IFeatureLayer))
                        {
                            if (edSvc.ShowDialog(frm) == DialogResult.OK)
                            {
                                retRender = (frm as IFeatureRendererProvider).Renderer;
                                if (retRender == null)
                                    return value;
                                retRender.RotateFieldDef = rotateFieldDef;
                                (retRender as BaseFeatureRenderer).SetFeatureLayerEnvironment(env);
                                return retRender;
                            }
                            else
                                return value;
                        }
                }
                return value;
            }
            finally
            {
            }
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
    }
}
