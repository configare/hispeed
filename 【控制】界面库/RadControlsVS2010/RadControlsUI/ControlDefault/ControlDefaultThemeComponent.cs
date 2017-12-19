using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public class ControlDefaultThemeComponent : RadThemeComponentBase
    {
        public const string RepositoryName = "ControlDefault_Repository.xml";

        public override void DeserializeTheme()
        {
            //TODO: Uncomment when new ControlDefault theme is ready
            this.EnsureRepository();

            base.DeserializeTheme();
        }

        private void EnsureRepository()
        {
            if (ThemeResolutionService.GetThemeRepository(ThemeResolutionService.ControlDefaultThemeName, false, false) == null)
            {
                string themeLocation = "Telerik.WinControls.UI.ControlDefault." + RepositoryName;
                ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, themeLocation);
            }
        }
    }
}
