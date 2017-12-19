using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public class DefaultStyleBuilder : StyleBuilderBase
    {
        protected override StyleSheet CreateDefaultStyle()
        {
            return new StyleSheet();
        }
    }
}
