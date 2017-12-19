using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a Label(static) element - such as Header and Footer - within a RadPageViewElement instance.
    /// </summary>
    public class RadPageViewLabelElement : RadPageViewElementBase
    {
        #region Initialization

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.ImageAlignment = ContentAlignment.MiddleLeft;
            this.TextAlignment = ContentAlignment.MiddleLeft;
        }

        #endregion
    }
}
