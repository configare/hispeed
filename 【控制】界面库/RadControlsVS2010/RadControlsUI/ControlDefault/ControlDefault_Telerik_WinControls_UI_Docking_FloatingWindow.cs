using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ControlDefault_Telerik_WinControls_UI_Docking_FloatingWindow : ControlDefaultThemeComponent
    {
        public ControlDefault_Telerik_WinControls_UI_Docking_FloatingWindow()
        {
            InitializeComponent();
        }

        public ControlDefault_Telerik_WinControls_UI_Docking_FloatingWindow(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
