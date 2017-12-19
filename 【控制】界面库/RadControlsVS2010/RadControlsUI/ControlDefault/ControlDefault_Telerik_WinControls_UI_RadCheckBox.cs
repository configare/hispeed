using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class CheckBox : ControlDefaultThemeComponent
    {
        public CheckBox()
        {
            InitializeComponent();
        }

        public CheckBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
