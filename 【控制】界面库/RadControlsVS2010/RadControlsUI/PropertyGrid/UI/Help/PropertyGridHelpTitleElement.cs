using System.Drawing;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class PropertyGridHelpTitleElement : LightVisualElement
    {
        static PropertyGridHelpTitleElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ItemStateManagerFactory(), typeof(PropertyGridHelpTitleElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.TextAlignment = ContentAlignment.MiddleLeft;
            this.NotifyParentOnMouseInput = true;
        }
    }
}
