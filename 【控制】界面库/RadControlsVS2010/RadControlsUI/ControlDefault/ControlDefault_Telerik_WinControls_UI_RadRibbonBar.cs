using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class RibbonBar : ControlDefaultThemeComponent
    {
        public RibbonBar()
        {
            InitializeComponent();
        }

        public RibbonBar(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
