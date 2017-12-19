using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Telerik.WinControls.Themes.ControlDefault;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ListControl : ControlDefaultThemeComponent
    {
        public ListControl()
        {
            InitializeComponent();
        }

        public ListControl(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
