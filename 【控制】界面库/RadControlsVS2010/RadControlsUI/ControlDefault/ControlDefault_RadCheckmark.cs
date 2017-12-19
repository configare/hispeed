using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Telerik.WinControls.Themes.ControlDefault;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ControlDefault_RadCheckmark : ControlDefaultThemeComponent
    {
        public ControlDefault_RadCheckmark()
        {
            InitializeComponent();
        }

        public ControlDefault_RadCheckmark(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
