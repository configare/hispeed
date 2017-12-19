using Telerik.WinControls.Themes.Design;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class RadTitleBarDesignTimeData : RadControlDesignTimeData
    {
        public RadTitleBarDesignTimeData() 
		{ 
		}

        public RadTitleBarDesignTimeData(string name): base(name) 
		{ 
        }

        public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
        {
			RadTitleBar titleBarPreview = new RadTitleBar();
			titleBarPreview.Text = "RadTitleBar";
			titleBarPreview.Bounds = new Rectangle(30, 30, 300, 26);

			RadTitleBar titleBarStructure = new RadTitleBar();
			titleBarStructure.Text = "RadTitleBar";

			ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(titleBarPreview, titleBarStructure.RootElement);
			ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

			res.Add(designed);

			return res;
        }
    }
}
