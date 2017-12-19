using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.GeoVIS
{
    [Export(typeof(ICommand)),ExportMetadata("VERSION", "1")]
    public class CommandGeoVisDistanceMeasure:Command
    {
        public CommandGeoVisDistanceMeasure()
        {
            _id = 8309;
            _name = "GeoVisDistanceMeasure";
            _text = "数字地球距离测量";
            _toolTip = "数字地球距离测量";
        }

        public override void Execute()
        {
            PluginGeoVisWnd viewer = _smartSession.SmartWindowManager.ActiveViewer as PluginGeoVisWnd;
            if (viewer == null)
                return;
            else
            {
                UCGeoVIS vis = viewer.Controls[0] as UCGeoVIS;
                vis.Viewer.CurrentTool = vis.Viewer.CreateMeasureTool(GeoVisSDK.Measure.MeasureType.Length);
            }
        }
    }
}
