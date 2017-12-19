using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ScrollBar : ControlDefaultThemeComponent
    {
        public ScrollBar()
        {
            InitializeComponent();
        }

        public ScrollBar(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
