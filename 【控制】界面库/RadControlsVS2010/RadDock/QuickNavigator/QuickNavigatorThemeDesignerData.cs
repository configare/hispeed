using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Themes.Design;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A helper class that defines the appearance of the QuickNavigator control in the Visual Style Builder.
    /// </summary>
    public class QuickNavigatorThemeDesignerData : RadControlDesignTimeData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="previewSurface"></param>
        /// <returns></returns>
        public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
        {
            QuickNavigator previewNavigator = new QuickNavigator();
            Size size = previewNavigator.GetPreferredSize();
            previewNavigator.Bounds = new Rectangle(Point.Empty, size);

            QuickNavigator elementStructureNavigator = new QuickNavigator();
            size = elementStructureNavigator.GetPreferredSize();
            elementStructureNavigator.Bounds = new Rectangle(Point.Empty, size);

            ControlStyleBuilderInfo navigatorDesignData = new ControlStyleBuilderInfo(previewNavigator, elementStructureNavigator.RootElement);
            ControlStyleBuilderInfoList list = new ControlStyleBuilderInfoList();
            list.Add(navigatorDesignData);

            return list;
        }
    }
}
