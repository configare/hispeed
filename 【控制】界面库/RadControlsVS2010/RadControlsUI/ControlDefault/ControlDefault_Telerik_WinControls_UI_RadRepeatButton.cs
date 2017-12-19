using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class RepeatButton : ControlDefaultThemeComponent
    {
        public RepeatButton()
        {
            InitializeComponent();
        }

        public RepeatButton(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
