using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class Carousel : ControlDefaultThemeComponent
    {
        public Carousel()
        {
            InitializeComponent();
        }

        public Carousel(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
