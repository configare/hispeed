using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Drawing;

namespace Telerik.WinControls.UI.RibbonBar
{
    public class RibbonBarCaptionFillPrimitive : FillPrimitive
    {
        #region Methods

        public override void PaintPrimitive(Telerik.WinControls.Paint.IGraphics g, float angle, System.Drawing.SizeF scale)
        {
            RadRibbonBar ribbonBar = this.ElementTree.Control as RadRibbonBar;

            if (ribbonBar != null)
            {
                RadFormControlBase form = ribbonBar.FindForm() as RadFormControlBase;

                if (form != null && form.FormBehavior is RadRibbonFormBehavior)
                {
                    float scaleAmount = form.Width / (float)ribbonBar.Width;
                    int translateAmount = (form.Width - ribbonBar.Width) / 2;
                    g.TranslateTransform(-translateAmount, 0);

                    if (scaleAmount != 0f && !float.IsInfinity(scaleAmount))
                    {
                        g.ScaleTransform(new SizeF(scaleAmount, 1));
                    }

                    base.PaintPrimitive(g, angle, scale);

                    g.TranslateTransform(translateAmount, 0);

                    scaleAmount = ribbonBar.Width / (float)form.Width;

                    if (scaleAmount != 0f && !float.IsInfinity(scaleAmount))
                    {
                        g.ScaleTransform(new SizeF(scaleAmount, 1));
                    }
                }
                else
                {
                    base.PaintPrimitive(g, angle, scale);
                }
            }
        }

        #endregion
    }
}
