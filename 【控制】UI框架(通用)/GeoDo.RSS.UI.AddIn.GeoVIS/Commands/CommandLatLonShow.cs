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
    public class CommandLatLonShow:Command
    {
        public CommandLatLonShow()
        {
            _id = 8303;
            _name = "LatLonShow";
            _text = "经纬网显示";
            _toolTip = "经纬网显示";
        }

        public override void Execute()
        {
            PluginGeoVisWnd viewer = _smartSession.SmartWindowManager.ActiveViewer as PluginGeoVisWnd;
            if (viewer == null)
                return;
            else
            {
                UCGeoVIS vis = viewer.Controls[0] as UCGeoVIS;
                vis.LonLat.SetStatus(NodeStatus.Visible, !vis.LonLat.GetStatus(NodeStatus.Visible));
                vis.Viewer.UpdateView(false);
            }
        }

    }
}
