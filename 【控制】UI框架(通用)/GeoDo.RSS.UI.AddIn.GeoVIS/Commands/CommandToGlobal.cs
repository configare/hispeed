using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoVisSDK.ViewNode;

namespace GeoDo.RSS.UI.AddIn.GeoVIS
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandToGlobal:Command
    {
        public CommandToGlobal()
        {
            _id = 8316;
            _name = "ToGlobal";
            _text = "全球区域";
            _toolTip = "全球区域";
        }
        public override void Execute()
        {
            PluginGeoVisWnd viewer = _smartSession.SmartWindowManager.ActiveViewer as PluginGeoVisWnd;
            if (viewer == null)
                return;
            else
            {
                UCGeoVIS vis = viewer.Controls[0] as UCGeoVIS;
                vis.Viewer.Pose = new CameraPose(Angle.FromDegrees(45), Angle.FromDegrees(150), GeoVisViewer.Radius * 2);
            }
        }
    }
}
