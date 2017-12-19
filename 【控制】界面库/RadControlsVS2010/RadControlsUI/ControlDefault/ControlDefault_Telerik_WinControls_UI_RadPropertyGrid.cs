using System.ComponentModel;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class PrpertyGrid : ControlDefaultThemeComponent
    {
        public PrpertyGrid()
        {
            InitializeComponent();
        }

        public PrpertyGrid(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
