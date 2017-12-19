using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class DateTimePicker : ControlDefaultThemeComponent
    {
        public DateTimePicker()
        {
            InitializeComponent();
        }

        public DateTimePicker(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
