
using System.Drawing;
namespace Telerik.WinControls.UI
{
    public class PropertyGridGroupTextElement : PropertyGridTextElement
    {
        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ShouldHandleMouseInput = false;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.ButtonsLayout.Visibility = ElementVisibility.Collapsed;
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            return availableSize;
        }
    }
}
