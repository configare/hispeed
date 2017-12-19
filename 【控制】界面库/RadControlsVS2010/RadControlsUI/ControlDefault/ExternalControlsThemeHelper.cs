using System.Reflection;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public class ExternalControlsThemeHelper
    {
        public static void LoadGridThemes()
        {
            ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.ControlDefault.ControlDefault_Repository.xml");
            ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadGridView.xml");
            ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.ControlDefault.ControlDefault_Telerik_WinControls_UI_ColumnChooserControl.xml");
        }

        public static void LoadSchedulerThemes()
        {
            Assembly resourceAssembly = typeof(ExternalControlsThemeHelper).Assembly;
            ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.ControlDefault.ControlDefault_Repository.xml");
            ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, resourceAssembly, "Telerik.WinControls.UI.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadScheduler.xml");
        }
    }
}
