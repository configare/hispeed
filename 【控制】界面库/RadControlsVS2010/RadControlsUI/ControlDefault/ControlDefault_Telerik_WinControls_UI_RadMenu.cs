using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class Menu : ControlDefaultThemeComponent
    {
        public Menu()
        {
            InitializeComponent();
        }

        public Menu(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
