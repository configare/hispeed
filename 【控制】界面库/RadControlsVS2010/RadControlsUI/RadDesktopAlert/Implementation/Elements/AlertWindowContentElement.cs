using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layouts;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the content of a <see cref="RadDesktopAlert"/>component.
    /// The content usually is built of an image and HTML enabled text.
    /// </summary>
    public class AlertWindowContentElement : LightVisualElement
    {
        #region Methods

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.Padding = new System.Windows.Forms.Padding(5);
            this.TextWrap = true;
            this.TextAlignment = ContentAlignment.TopLeft;
            this.ImageAlignment = ContentAlignment.MiddleLeft;
            this.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.AutoEllipsis = true;
        }

        #endregion
    }
}
