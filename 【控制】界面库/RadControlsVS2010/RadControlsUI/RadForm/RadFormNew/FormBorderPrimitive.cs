using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class FormBorderPrimitive : BorderPrimitive
    {
        #region Properties

        public new BorderBoxStyle BoxStyle
        {
            get
            {
                return base.BoxStyle;
            }
            set
            {
                if (value != BorderBoxStyle.FourBorders)
                {
                    base.BoxStyle = value;
                }
            }
        }

        #endregion
    }
}
