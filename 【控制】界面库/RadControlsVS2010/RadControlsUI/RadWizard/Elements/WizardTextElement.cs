using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents an element of RadWizard which paints its text on glass.
    /// </summary>
    public class WizardTextElement : BaseWizardElement
    {
        protected override void PaintOverride(Paint.IGraphics screenRadGraphics, Rectangle clipRectangle, float angle, SizeF scale, bool useRelativeTransformation)
        {
            base.PaintOverride(screenRadGraphics, clipRectangle, angle, scale, useRelativeTransformation);
            if (base.Visibility != ElementVisibility.Visible || this.Parent == null)
            {
                return;
            }
            WizardPageHeaderElement pageHeader = this.Parent as WizardPageHeaderElement;
            if (pageHeader == null)
            {
                return;
            }

            RadWizardElement owner = pageHeader.Owner;
            if (DWMAPI.IsCompositionEnabled && !this.IsDesignMode && owner != null && owner.OwnerControl != null && owner.Mode == WizardMode.Aero && owner.EnableAeroStyle)
            {
                TelerikPaintHelper.DrawGlowingText(
                           (Graphics)screenRadGraphics.UnderlayGraphics,
                           this.Text,
                           this.Font,
                           this.ControlBoundingRectangle,
                           this.ForeColor, TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }
    }
}