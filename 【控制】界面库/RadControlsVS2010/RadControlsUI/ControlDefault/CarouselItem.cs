using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class CarouselItem : ControlDefaultThemeComponent
    {
        public CarouselItem()
        {
            InitializeComponent();
        }

        public CarouselItem(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
