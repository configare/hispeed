using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using GeoVis.GeoImage;
using GeoVisSDK.ViewNode;

namespace GeoDo.RSS.UI.AddIn.GeoVIS
{
    public partial class UCGeoVIS : UserControl
    {
        private GeoVisViewer _viewer = null;
        private OViewNode _lonLat = null;
        private OGeoImageViewNode _imageLayer = null;
        private IGeoImage _img = null;
        //private LayerManager layer = null;

        public UCGeoVIS()
        {
            InitializeComponent();
            Init();
        }

        public GeoVisViewer Viewer
        {
            get { return _viewer; }
        }

        public OViewNode LonLat
        {
            get { return _lonLat; }
        }

        public IGeoImage Img
        {
            get { return _img; }
            set { _img = value; }
        }
        private void Init()
        {     
            _viewer = new GeoVisViewer();
            _viewer.Dock = DockStyle.Fill;
            this.Controls.Add(_viewer); 
            _lonLat = _viewer.CreateViewNode(ViewNodeType.LonLatGridLayer);
            _lonLat.SetStatus(NodeStatus.Visible, false);_lonLat.SetStatus(NodeStatus.Visible, false);
            _viewer.Root.InsertNode(_viewer.Root.ChildrenCount, _lonLat);
            string terrainPath = System.Configuration.ConfigurationManager.AppSettings["GeoTerrainPath"];
            terrainPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, terrainPath);
            if (!File.Exists(terrainPath))
                return ; 
            _imageLayer = _viewer.CreateViewNode(ViewNodeType.GeoImageLayer) as OGeoImageViewNode;
            _imageLayer.Name = "基础影像层";
            _viewer.Root.InsertNode(0, _imageLayer);
            LoadConfigure();
            _viewer.UpdateView(false);
            System.Threading.Thread.Sleep(200);            
        }

        private void LoadConfigure()
        {
            double ff = _viewer.SpeedPercent;
            _viewer.SpeedPercent = 1.0f;
            EarthGoto(25, 116.3, GeoVisViewer.Radius * 2);
            _viewer.SpeedPercent = ff;
            
        }
        void EarthGoto(double lat, double lon, double alt)
        {
            try
            {
                _viewer.Pose = new CameraPose(Angle.FromDegrees(lat), Angle.FromDegrees(lon), alt);
            }
            catch(Exception)
            {
            }
        }

        public void DisposeMember()
        {
            if (_viewer != null)
                _viewer.Dispose();
            if (_lonLat != null)
                _lonLat = null;
            if (_imageLayer != null)
                _imageLayer = null;
            if (_img != null)
                _img.Dispose();
        }
    }
}
