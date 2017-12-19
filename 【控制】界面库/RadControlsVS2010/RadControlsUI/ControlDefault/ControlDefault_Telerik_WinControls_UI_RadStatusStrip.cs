using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class StatusStrip : ControlDefaultThemeComponent
    {
        public StatusStrip()
        {
            InitializeComponent();
        }

        public StatusStrip(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
