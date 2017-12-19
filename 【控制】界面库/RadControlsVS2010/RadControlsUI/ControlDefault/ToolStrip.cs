using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ToolStrip : ControlDefaultThemeComponent
    {
        public ToolStrip()
        {
            InitializeComponent();
        }

        public ToolStrip(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
