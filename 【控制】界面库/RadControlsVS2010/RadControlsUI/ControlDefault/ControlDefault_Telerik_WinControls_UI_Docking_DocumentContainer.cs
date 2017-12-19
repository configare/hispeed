using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ControlDefault_Telerik_WinControls_UI_Docking_DocumentContainer : ControlDefaultThemeComponent
    {
        public ControlDefault_Telerik_WinControls_UI_Docking_DocumentContainer()
        {
            InitializeComponent();
        }

        public ControlDefault_Telerik_WinControls_UI_Docking_DocumentContainer(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
