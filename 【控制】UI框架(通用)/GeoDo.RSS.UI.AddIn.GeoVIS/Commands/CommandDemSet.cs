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
    public class CommandDemSet:Command
    {
        public CommandDemSet()
        {
            _id = 8311;
            _name = "DemSet";
            _text = "设置高程系数";
            _toolTip = "设置高程系数";

        }

        public override void  Execute(string argument)
        {
 	        PluginGeoVisWnd viewer = _smartSession.SmartWindowManager.ActiveViewer as PluginGeoVisWnd;
            if (viewer == null)
                return;
            else
            {
                float result;
                UCGeoVIS vis = viewer.Controls[0] as UCGeoVIS;
                float.TryParse(argument,out result);
                vis.Viewer.Terrain.VerticalExaggration = result;
                vis.Viewer.Terrain.TerrainMapped = true;
                viewer.Refresh();
            }
         }
    }  
}
