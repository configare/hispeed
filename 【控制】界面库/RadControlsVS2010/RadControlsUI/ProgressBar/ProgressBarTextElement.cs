namespace Telerik.WinControls.UI
{
    public class ProgressBarTextElement : LightVisualElement
    {
        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.DrawText = true;
            this.DrawBorder = false;
            this.DrawFill = false;
        }
    }
}
