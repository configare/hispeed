using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ControlDefault_Telerik_WinControls_UI_RadTitleBar : ControlDefaultThemeComponent
    {
        public ControlDefault_Telerik_WinControls_UI_RadTitleBar()
        {
            InitializeComponent();
        }

        public ControlDefault_Telerik_WinControls_UI_RadTitleBar(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
