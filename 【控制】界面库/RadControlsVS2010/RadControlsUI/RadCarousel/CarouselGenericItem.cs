using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    [RadToolboxItem(false)]
    public class CarouselGenericItem: RadButtonElement
    {
        static CarouselGenericItem()
        {
            ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.Resources.CarouselThemes.CarouselItemDefault.xml");
        }
    }
}
