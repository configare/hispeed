using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ControlDefault_DockSplitContainer : ControlDefaultThemeComponent
    {
        public ControlDefault_DockSplitContainer()
        {
            InitializeComponent();
        }

        public ControlDefault_DockSplitContainer(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
