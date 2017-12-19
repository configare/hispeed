using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class RadSeparator : ControlDefaultThemeComponent
    {
        public RadSeparator()
        {
            InitializeComponent();
        }

        public RadSeparator(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}