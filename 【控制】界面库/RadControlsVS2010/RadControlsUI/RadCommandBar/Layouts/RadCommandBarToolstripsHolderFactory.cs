

namespace Telerik.WinControls.UI
{
    public sealed class RadCommandBarToolstripsHolderFactory
    {
        private RadCommandBarToolstripsHolderFactory()
        {
        }

        public static CommandBarRowElement CreateLayoutPanel(RadCommandBarElement owner)
        {
            return new CommandBarRowElement();
        }
    }
}
