using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class PropertyValueButtonElement: LightVisualElement
    {
        public static RadProperty IsModifiedProperty = RadProperty.Register(
            "IsModified", typeof(bool), typeof(PropertyValueButtonElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));

        static PropertyValueButtonElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new PropertyGridValueButtonStateManager(), typeof(PropertyValueButtonElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.StretchHorizontally = false;
            this.StretchVertically = false;
            this.Alignment = ContentAlignment.MiddleRight;
            this.NotifyParentOnMouseInput = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            this.ElementTree.Control.Cursor = Cursors.Default;
        }
    }
}
