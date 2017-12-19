using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the caption grip of a <see cref="RadDesktopAlert"/>window.
    /// </summary>
    public class AlertWindowCaptionGrip : LightVisualElement
    {
        #region Methods

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.MinSize = new System.Drawing.Size(0, 10);
            this.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            this.ImageAlignment = System.Drawing.ContentAlignment.MiddleCenter;
        }

        #endregion
    }
}
