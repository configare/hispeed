using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class BaseColorEditorColorBox : LightVisualElement
    {
        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.StretchVertically = true;
            this.StretchHorizontally = false;
            this.DrawBorder = true;
            this.BorderGradientStyle = GradientStyles.Solid;
            this.BorderBoxStyle = BorderBoxStyle.SingleBorder;
            this.BorderColor = Color.Black;
            this.DrawFill = true;
            this.MinSize = new System.Drawing.Size(20, 0);
            this.GradientStyle = GradientStyles.Solid;
            this.BackColor = Color.Empty;
            this.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
        }
    }
}
