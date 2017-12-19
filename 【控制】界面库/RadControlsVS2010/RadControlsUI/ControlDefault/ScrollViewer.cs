using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ScrollViewer : ControlDefaultThemeComponent
    {
        public ScrollViewer()
        {
            InitializeComponent();
        }

        public ScrollViewer(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
