using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class TabStrip : ControlDefaultThemeComponent
    {
        public TabStrip()
        {
            InitializeComponent();
        }

        public TabStrip(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
