using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ControlDefault_Telerik_WinControls_UI_RadSplitContainer : ControlDefaultThemeComponent
    {
        public ControlDefault_Telerik_WinControls_UI_RadSplitContainer()
        {
            InitializeComponent();
        }

        public ControlDefault_Telerik_WinControls_UI_RadSplitContainer(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
