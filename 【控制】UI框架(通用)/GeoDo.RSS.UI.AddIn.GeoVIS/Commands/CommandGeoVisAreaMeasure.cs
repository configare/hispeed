using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.IO;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.GeoVIS
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandGeoVisAreaMeasure:Command
    {
        public CommandGeoVisAreaMeasure()
        {
            _id = 8308;
            _name = "GeoVisAreaMeasure";
            _text = "数字地球面积测量";
            _toolTip = "数字地球面积测量";
        }

        public override void Execute()
        {
            PluginGeoVisWnd viewer = _smartSession.SmartWindowManager.ActiveViewer as PluginGeoVisWnd;
            if (viewer == null)
                return;
            else
            {
                UCGeoVIS vis = viewer.Controls[0] as UCGeoVIS;
                vis.Viewer.CurrentTool = vis.Viewer.CreateMeasureTool(GeoVisSDK.Measure.MeasureType.Area);
            }
        }
    }
}
