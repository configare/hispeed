using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.TextPrimitiveUtils;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    public class TextPart : VisualElementLayoutPart
    {
        public TextPart(LightVisualElement owner) : base(owner) { }        

        public override SizeF Measure(SizeF availableSize)
        {
            SizeF result = SizeF.Empty;
            if (this.owner.DrawText)
            {
                TextParams textParams = this.owner.CreateTextParams();
                result = this.owner.textPrimitiveImpl.MeasureOverride(availableSize, textParams);
            }

            this.desiredSize = result;
            this.desiredSize.Width = Math.Min(availableSize.Width, this.desiredSize.Width);
            this.desiredSize.Height = Math.Min(availableSize.Height, this.desiredSize.Height);

            return this.desiredSize;
        }
    }
}
