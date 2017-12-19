using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Telerik.WinControls.Themes.ControlDefault;

namespace Telerik.WinControls.UI.ControlDefault
{
    public partial class ControlDefault_Telerik_WinControls_UI_RadLabel : ControlDefaultThemeComponent
    {
        public ControlDefault_Telerik_WinControls_UI_RadLabel()
        {
            InitializeComponent();
        }

        public ControlDefault_Telerik_WinControls_UI_RadLabel(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
