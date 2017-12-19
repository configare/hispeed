using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class ProgressIndicatorElement : LightVisualElement
    {
        public static RadProperty IsVerticalProperty = RadProperty.Register("IsVertical", typeof(bool),
            typeof(ProgressIndicatorElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));

        static ProgressIndicatorElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ProgressIndicatorStateManager(), typeof(ProgressIndicatorElement));
        }
    }
}
