using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ControlDefault_Telerik_WinControls_UI_RadApplicationMenu : ControlDefaultThemeComponent
    {
        public ControlDefault_Telerik_WinControls_UI_RadApplicationMenu()
        {
            InitializeComponent();
        }

        public ControlDefault_Telerik_WinControls_UI_RadApplicationMenu(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
