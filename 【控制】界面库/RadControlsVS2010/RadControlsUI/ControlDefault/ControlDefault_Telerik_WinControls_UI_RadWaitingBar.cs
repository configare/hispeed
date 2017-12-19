using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class WaitingBar : ControlDefaultThemeComponent
    {
        public WaitingBar()
        {
            InitializeComponent();
        }

        public WaitingBar(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
