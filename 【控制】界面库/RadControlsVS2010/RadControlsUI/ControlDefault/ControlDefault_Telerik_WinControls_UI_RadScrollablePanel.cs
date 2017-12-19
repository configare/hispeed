using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ScrollablePanel : ControlDefaultThemeComponent
    {
        public ScrollablePanel()
        {
            InitializeComponent();
        }

        public ScrollablePanel(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
