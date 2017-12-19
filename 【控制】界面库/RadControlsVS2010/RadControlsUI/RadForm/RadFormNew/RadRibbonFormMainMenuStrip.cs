using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class serves as a dummy MenuStrip used by the RadRibbonFormBehavior
    /// to prevent the default MDI caption from being painted when a MDI child is maximized
    /// since the RadRibbonBar controls takes care to handle MDI children.
    /// </summary>
    internal class RadRibbonFormMainMenuStrip : MenuStrip
    {
    }
}
