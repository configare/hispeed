using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class CommandBar : ControlDefaultThemeComponent
    {
        public CommandBar()
        {
            InitializeComponent();
        }

        public CommandBar(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
