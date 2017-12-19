using Telerik.WinControls.Themes.Design;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI 
{
    /// <summary>
    /// Class is required by Visual Style Builder.
    /// </summary>
    public class RadGroupBoxThemeDesignerData : RadControlDesignTimeData
    {
        /// <summary>
        /// Parameterless constructor.
        /// </summary>
        public RadGroupBoxThemeDesignerData() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        public RadGroupBoxThemeDesignerData(string name)
            : base(name) 
        { }

        /// <summary>
        /// Method is required by Visual Style Builder.
        /// </summary>
        /// <param name="previewSurface"></param>
        /// <returns>theme designed controls</returns>
        public override ControlStyleBuilderInfoList GetThemeDesignedControls(Control previewSurface)
        {

            RadGroupBox groupBox = new RadGroupBox();
            
            groupBox.Size = new Size(250, 250);
            
            groupBox.Text = "Header Text";
            groupBox.FooterText = "Footer Text";
            
            RadGroupBox groupBoxStructure = new RadGroupBox();

            groupBoxStructure.Text = "Header Text";
            groupBoxStructure.FooterText = "Footer Text";

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(groupBox, groupBoxStructure.RootElement);
            designed.MainElementClassName = typeof(RadGroupBoxElement).FullName;
            ThemeDesignedControlList res = new ThemeDesignedControlList();
            res.Add(designed);
            return res;

        }
    }
}