using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class WaitingBarTextElement: LightVisualElement
    {
        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.DrawBorder = false;
            this.DrawFill = false;
            this.DrawText = false;
            this.StretchVertically = true;
            this.StretchHorizontally = true;
        }
    }
}
