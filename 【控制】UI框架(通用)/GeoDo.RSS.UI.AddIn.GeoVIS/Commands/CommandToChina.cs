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
    public class CommandToChina:Command
    {
        public CommandToChina()
        {
            _id = 8315;
            _name = "ToChina";
            _text = "中国区域";
            _toolTip = "中国区域";
        }

        public override void Execute()
        {
            PluginGeoVisWnd viewer = _smartSession.SmartWindowManager.ActiveViewer as PluginGeoVisWnd;
            if (viewer == null)
                return;
            else
            {
                UCGeoVIS vis = viewer.Controls[0] as UCGeoVIS;
                vis.Viewer.Pose = new CameraPose(Angle.FromDegrees(40), Angle.FromDegrees(105), GeoVisViewer.Radius * 1);
            }
        }
    }
}
