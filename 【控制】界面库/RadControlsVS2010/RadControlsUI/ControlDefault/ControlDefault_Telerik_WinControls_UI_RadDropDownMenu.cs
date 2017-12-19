using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Telerik.WinControls.Themes.ControlDefault;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ControlDefault_Telerik_WinControls_UI_RadDropDownMenu : ControlDefaultThemeComponent
    {
        public ControlDefault_Telerik_WinControls_UI_RadDropDownMenu()
        {
            InitializeComponent();
        }

        public ControlDefault_Telerik_WinControls_UI_RadDropDownMenu(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
