using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ControlDefault_Telerik_WinControls_UI_RadRibbonForm : ControlDefaultThemeComponent
    {
        public ControlDefault_Telerik_WinControls_UI_RadRibbonForm()
        {
            InitializeComponent();
        }

        public ControlDefault_Telerik_WinControls_UI_RadRibbonForm(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
