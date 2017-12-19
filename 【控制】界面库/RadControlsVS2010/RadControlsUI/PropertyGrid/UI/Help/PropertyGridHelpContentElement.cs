
using System.Drawing;
using Telerik.WinControls.Styles;
namespace Telerik.WinControls.UI
{
    public class PropertyGridHelpContentElement : LightVisualElement
    {
        static PropertyGridHelpContentElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ItemStateManagerFactory(), typeof(PropertyGridHelpContentElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.TextAlignment = ContentAlignment.TopLeft;
            this.TextWrap = true;
            this.NotifyParentOnMouseInput = true;
        }
    }
}
