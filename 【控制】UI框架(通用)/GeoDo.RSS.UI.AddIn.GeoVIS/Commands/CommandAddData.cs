using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using System.IO;
using GeoDo.RSS.Core.UI;
using GeoVis.GeoImage;
using GeoVisSDK.ViewNode;

namespace GeoDo.RSS.UI.AddIn.GeoVIS
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandAddData:Command
    {
        UCGeoVIS vis = null;
        public CommandAddData()
        {
            _id = 8310;
            _name = "DataAdd";
            _text = "添加单幅影像";
            _toolTip = "添加单幅影像";
        }

        public override void Execute()
        {
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.Filter = "LDF File(*.ldf)|*.ldf|Image File (*.rst)|*.rst|(*.img)|*.img|(*.tif)|*.tif|(*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    PluginGeoVisWnd viewer = _smartSession.SmartWindowManager.ActiveViewer as PluginGeoVisWnd;
                    if (viewer == null)
                        return;
                    else
                    {
                        vis = viewer.Controls[0] as UCGeoVIS;
                        vis.Img = IGeoImage.GeoOpen(dlg.FileName);
                        if (vis.Img == null)
                            return;
                        vis.Img.SetPara(3, new int[] { 6, 2, 1 }, new float[] { 0, 0, 0 }, new float[] { 1000, 1000, 1000 });
                        OViewNode node = vis.Viewer.CreateViewNode(ViewNodeType.GeoQuadTileSet);
                        node.Name = Path.GetFileNameWithoutExtension(dlg.FileName);
                        node.Tag = vis.Img;
                        vis.Viewer.Root.InsertNode(vis.Viewer.Root.ChildrenCount, node);
                        double alt = GeoVisViewer.Radius * Math.Abs(Math.Sin(Angle.DegreeToRadians * ((vis.Img.CurGeoRegion.MaxLon - vis.Img.CurGeoRegion.MinLon))));
                        EarthGoto(vis.Img.CurGeoRegion.CenterLatitude, vis.Img.CurGeoRegion.CenterLongitude, alt);
                    }
                }
            }
            catch(Exception ex)
            {
                MsgBox.ShowInfo(ex.Message);
            }
        }

        void EarthGoto(double lat, double lon, double alt)
        {
            vis.Viewer.Pose = new CameraPose(Angle.FromDegrees(lat), Angle.FromDegrees(lon), alt);
        }
    }
}
