using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.GeoVIS
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandDemSetCancel:Command
    {
        public CommandDemSetCancel()
        {
            _id = 8307;
            _name = "DemSetCancel";
            _text = "取消高程";
            _toolTip = "取消高程";
        }

        public override void Execute()
        {
            PluginGeoVisWnd viewer = _smartSession.SmartWindowManager.ActiveViewer as PluginGeoVisWnd;
            if (viewer == null)
                return;
            else
            {
                UCGeoVIS vis = viewer.Controls[0] as UCGeoVIS;
                vis.Viewer.Terrain.TerrainMapped = false;
                
            }
        }
    }
}
