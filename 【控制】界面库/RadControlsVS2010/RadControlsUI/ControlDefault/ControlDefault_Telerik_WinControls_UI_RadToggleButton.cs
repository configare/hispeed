using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ToggleButton : ControlDefaultThemeComponent
    {
        public ToggleButton()
        {
            InitializeComponent();

        }

        public ToggleButton(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
