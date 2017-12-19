using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Telerik.WinControls.Themes.ControlDefault;

namespace Telerik.WinControls.UI.ControlDefault
{
    public partial class ControlDefault_Telerik_WinControls_UI_RadDropDownList :  ControlDefaultThemeComponent
    {
        public ControlDefault_Telerik_WinControls_UI_RadDropDownList()
        {
            InitializeComponent();
        }

        public ControlDefault_Telerik_WinControls_UI_RadDropDownList(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
