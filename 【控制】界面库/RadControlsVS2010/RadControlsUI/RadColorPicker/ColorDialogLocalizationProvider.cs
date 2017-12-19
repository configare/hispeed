using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Localization;

namespace Telerik.WinControls.UI
{
    public class ColorDialogStringId
    {
        public const string ColorDialogProfessionalTab = "ColorDialogProfessionalTab";
        public const string ColorDialogWebTab = "ColorDialogWebTab";
        public const string ColorDialogSystemTab = "ColorDialogSystemTab";
        public const string ColorDialogBasicTab = "ColorDialogBasicTab";
        public const string ColorDialogAddCustomColorButton = "ColorDialogAddCustomColorButton";
        public const string ColorDialogOKButton = "ColorDialogOKButton";
        public const string ColorDialogCancelButton = "ColorDialogCancelButton";
        public const string ColorDialogCurrentColorLabel = "ColorDialogCurrentColorLabel";
        public const string ColorDialogNewColorLabel = "ColorDialogNewColorLabel";
    }

    public class ColorDialogLocalizationProvider : LocalizationProvider<ColorDialogLocalizationProvider>
    {
        public override string GetLocalizedString(string id)
        {
            switch(id)
            {
                case ColorDialogStringId.ColorDialogProfessionalTab: return "Professional";
                case ColorDialogStringId.ColorDialogWebTab: return "Web";
                case ColorDialogStringId.ColorDialogSystemTab: return "System";
                case ColorDialogStringId.ColorDialogBasicTab: return "Basic";
                case ColorDialogStringId.ColorDialogAddCustomColorButton: return "Add Custom Color";
                case ColorDialogStringId.ColorDialogOKButton: return "OK";
                case ColorDialogStringId.ColorDialogCancelButton: return "Cancel";
                case ColorDialogStringId.ColorDialogNewColorLabel: return "New";
                case ColorDialogStringId.ColorDialogCurrentColorLabel: return "Current";
            }

            return id;
        } 
    }
}
