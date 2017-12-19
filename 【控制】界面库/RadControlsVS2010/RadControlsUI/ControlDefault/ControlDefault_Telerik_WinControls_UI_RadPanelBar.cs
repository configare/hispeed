using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class PanelBar : ControlDefaultThemeComponent
    {
        public PanelBar()
        {
            InitializeComponent();
        }

        public PanelBar(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
