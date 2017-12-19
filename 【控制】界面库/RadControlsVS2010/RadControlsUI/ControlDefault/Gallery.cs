using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class Gallery : ControlDefaultThemeComponent
    {
        public Gallery()
        {
            InitializeComponent();
        }

        public Gallery(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
