using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class SpinEdit : ControlDefaultThemeComponent
    {
        public SpinEdit()
        {
            InitializeComponent();
        }

        public SpinEdit(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
