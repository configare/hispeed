using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI.RadColorPicker
{
    public class ColorListBoxItem: RadListVisualItem
    {
        const int colorBoxSizeWidth = 30;

        LightVisualElement colorBox;
        LightVisualElement textBox;

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadListVisualItem);
            }
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.Padding = new System.Windows.Forms.Padding(2);
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            StackLayoutElement stack = new StackLayoutElement();
            stack.StretchVertically = true;
            stack.FitInAvailableSize = true;
            stack.Orientation = System.Windows.Forms.Orientation.Horizontal;
            stack.Padding = new System.Windows.Forms.Padding(2);
            stack.ElementSpacing = 5;
            stack.StretchHorizontally = false;
            this.Children.Add(stack);

            colorBox = new LightVisualElement();
            colorBox.GradientStyle = Telerik.WinControls.GradientStyles.Solid;
            colorBox.DrawFill = true;
            colorBox.NotifyParentOnMouseInput = true;
            colorBox.MinSize = new Size(colorBoxSizeWidth, 0);
            colorBox.MaxSize = new Size(colorBoxSizeWidth, 0);
            colorBox.DrawBorder = true;
            colorBox.BorderColor = Color.Black;
            colorBox.BorderGradientStyle = GradientStyles.Solid;
            colorBox.NotifyParentOnMouseInput = true;
            colorBox.StretchHorizontally = false;
            stack.Children.Add(colorBox);

            textBox = new LightVisualElement();
            textBox.NotifyParentOnMouseInput = true;
            textBox.TextAlignment = ContentAlignment.MiddleLeft;
            textBox.StretchHorizontally = false;
            stack.Children.Add(textBox);
        }

        public override void Synchronize()
        {
            base.Synchronize();
            colorBox.BackColor = (Color)this.Data.Value;
            string colorString = this.Data.Value.ToString();
            int startColorIndex = colorString.IndexOf('[');
            string finalColorString = colorString.Substring(startColorIndex + 1, colorString.Length - startColorIndex - 2);
            string localizedColor = Telerik.WinControls.UI.ColorDialogLocalizationProvider.CurrentProvider.GetLocalizedString(finalColorString);
            this.textBox.Text = localizedColor;
            this.Text = "";
        }
    }
}
