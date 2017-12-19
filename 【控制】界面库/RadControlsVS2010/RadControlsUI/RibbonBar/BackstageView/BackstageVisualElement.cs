using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Paint;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a base class 
    /// </summary>
    public class BackstageVisualElement : LightVisualElement
    {
        static BackstageVisualElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new BackstageItemStateManagerFactory(), typeof(BackstageVisualElement));
        }

    }
}
