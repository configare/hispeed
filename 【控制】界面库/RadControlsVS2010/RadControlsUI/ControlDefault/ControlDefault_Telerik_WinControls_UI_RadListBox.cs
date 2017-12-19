using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ListBox : ControlDefaultThemeComponent
    {
        public ListBox()
        {
            InitializeComponent();
        }

        public ListBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
