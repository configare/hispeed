using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class PropertyGridErrorIndicatorElement: LightVisualElement
    {
        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.DrawFill = false;
            this.DrawBorder = false;
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            this.ElementTree.Control.Cursor = Cursors.Default;

            base.OnMouseMove(e);
        }
    }
}
