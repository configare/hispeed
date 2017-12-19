using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Telerik.WinControls.Themes.ControlDefault;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ListView : ControlDefaultThemeComponent
    {
        public ListView()
        {
            InitializeComponent();
        }

        public ListView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
