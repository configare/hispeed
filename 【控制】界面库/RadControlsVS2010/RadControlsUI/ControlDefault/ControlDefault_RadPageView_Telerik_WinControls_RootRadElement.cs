using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ControlDefault_RadPageView_Telerik_WinControls_RootRadElement : ControlDefaultThemeComponent
    {
        public ControlDefault_RadPageView_Telerik_WinControls_RootRadElement()
        {
            InitializeComponent();
        }

        public ControlDefault_RadPageView_Telerik_WinControls_RootRadElement(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
