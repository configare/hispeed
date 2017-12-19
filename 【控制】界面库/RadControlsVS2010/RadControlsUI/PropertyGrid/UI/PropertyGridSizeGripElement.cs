
using Telerik.WinControls.Styles;
namespace Telerik.WinControls.UI
{
    public class PropertyGridSizeGripElement : LightVisualElement
    {
        static PropertyGridSizeGripElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ItemStateManagerFactory(), typeof(PropertyGridSizeGripElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            
            this.StretchHorizontally = true;
            this.StretchVertically = false;
            this.NotifyParentOnMouseInput = true;

            this.MinSize = new System.Drawing.Size(0, 5);
            this.DrawFill = true;
            this.DrawBorder = true;
        }
    }
}
