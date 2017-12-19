using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Themes.Design;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// represent the DesignTimeData for RadStatusStrip
    /// </summary>
    public class RadStatusBarDesignTimeData : RadControlDesignTimeData
    {
        /// <summary>
        /// create instance of the RadStatusBarDesignTimeData
        /// </summary>
        public RadStatusBarDesignTimeData()
        { }

        /// <summary>
        /// create instance of the RadStatusBarDesignTimeData
        /// </summary>
        public RadStatusBarDesignTimeData(string name)
            : base(name)
        { }

        /// <summary>
        /// create the controls for VSB
        /// </summary>
        public override ControlStyleBuilderInfoList GetThemeDesignedControls(System.Windows.Forms.Control previewSurface)
        {
            RadStatusStrip statusBar = new RadStatusStrip();
            statusBar.AutoSize = false;
            statusBar.Size = new Size(200, 27);
            statusBar.Text = "RadStatusStrip";

            RadItem separatorItem = new RadToolStripSeparatorItem();
            statusBar.Items.Add(separatorItem);

            RadStatusStrip statusBarStructure = new RadStatusStrip();
            statusBarStructure.AutoSize = false;
            statusBarStructure.Size = new Size(200, 27);

            RadItem structureSeparatorItem = new RadToolStripSeparatorItem();
            statusBarStructure.Items.Add(structureSeparatorItem);

            ControlStyleBuilderInfo designed = new ControlStyleBuilderInfo(statusBar, statusBarStructure.RootElement);
            designed.MainElementClassName = typeof(RadStatusBarElement).FullName;
            ControlStyleBuilderInfoList res = new ControlStyleBuilderInfoList();

            res.Add(designed);

            return res;
        }
    }
}
