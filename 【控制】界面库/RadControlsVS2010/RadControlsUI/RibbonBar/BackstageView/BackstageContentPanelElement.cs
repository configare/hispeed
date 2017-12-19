using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the area where backstage pages are arranged.
    /// </summary>
    public class BackstageContentPanelElement : BackstageVisualElement
    {
        protected override void InitializeFields()
        {
            this.MinSize = new System.Drawing.Size(340, 0);
            base.InitializeFields();
        }
    }
}
