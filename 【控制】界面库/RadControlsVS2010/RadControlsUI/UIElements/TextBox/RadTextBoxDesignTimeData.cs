using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Themes.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents helper class that is used by Visual Style Builder.
    /// </summary>
    public class RadTextBoxDesignTimeData : RadControlDesignTimeData
    {
        /// <summary>
        /// Initializes a new instance of RadTextBoxDesignTimeData
        /// </summary>
        public RadTextBoxDesignTimeData()
        { }

        /// <summary>
        /// Initializes a new instance of RadTextBoxDesignTimeData
        /// </summary>
        public RadTextBoxDesignTimeData(string name)
            : base(name)
        { }


        /// <summary>
        /// Initializes the controls which will appear in the VSBuilder
        /// </summary>
        /// <param name="previewSurface"></param>
        /// <returns></returns>
        public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
        {
            RadTextBox textbox = new RadTextBox();
            textbox.Text = "RadTextBox";
            textbox.Bounds = new System.Drawing.Rectangle(0, 0, 100, 15);

            RadTextBox textboxStructure = new RadTextBox();
            textboxStructure.Bounds = new System.Drawing.Rectangle(0, 0, 100, 15);
            textboxStructure.Text = "RadTextBox";

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(textbox, textboxStructure.RootElement);
            designed.MainElementClassName = typeof(RadTextBoxElement).FullName;
            designed.Placemenet = PreviewControlPlacemenet.MiddleCenter;

            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();
            res.Add(designed);
            return res;
        }
    }
}
