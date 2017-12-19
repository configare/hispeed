using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using GeoVisSDK.ViewNode;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.GeoVIS
{
    public partial class PluginGeoVisWnd : ToolWindow,ISmartViewer,ILinkableViewer
    {
        private EventHandler _onCoordEnvelopeChanged = null;
        private UCGeoVIS _geoVis;
        private ISmartSession _session;
        private EventHandler _onWindowClosed;

        public PluginGeoVisWnd()
        {
            InitializeComponent();
            this.Text = "数字地球";
            Init();
        }

        public PluginGeoVisWnd(IContainer container)
        {
            InitializeComponent();
            this.Text = "数字地球";
            Init();
        }

        private void Init()
        {
            _geoVis = new UCGeoVIS();
            Controls.Add(_geoVis);
            _geoVis.Dock = DockStyle.Fill;
        }

        public EventHandler OnWindowClosed
        {
            get { return _onWindowClosed; }
            set { _onWindowClosed = value; }
        }

        public ISmartSession Session
        {
            set { _session = value; }
        }

        public UCGeoVIS GeoVis
        {
            get { return _geoVis; }
        }

        public string Title
        {
            get { return "数字地球"; }
        }

        public object ActiveObject
        {
            get { return null; }

        }

        public bool IsPrimaryLinkWnd
        {
            get { return false; }
            set { ;}
        }

        public void To(GeoDo.RSS.Core.DrawEngine.CoordEnvelope viewport)
        {
            if (_session.SmartWindowManager.ActiveCanvasViewer != null && _session.SmartWindowManager.ActiveCanvasViewer.IsPrimaryLinkWnd)
            {
                GeoDo.RSS.Core.DrawEngine.CoordEnvelope ev = viewport.Clone();
                ICanvas c = _session.SmartWindowManager.ActiveCanvasViewer.Canvas;
                if (c != null)
                {
                    double prjX1 = ev.MinX, prjY1 = ev.MaxY, prjX2 = ev.MaxX, prjY2 = ev.MinY;
                    double geoX1, geoY1, geoX2, geoY2;
                    c.CoordTransform.Prj2Geo(prjX1, prjY1, out geoX1, out geoY1);
                    c.CoordTransform.Prj2Geo(prjX2, prjY2, out geoX2, out geoY2);
                    ev = new CoordEnvelope(geoX1, geoX2, geoY2, geoY1);
                    double lon = ev.Center.X;
                    double lat = ev.Center.Y;
                    double span = (ev.MaxX - ev.MinX) > (ev.MaxY - ev.MinY) ?
                        (ev.MaxX - ev.MinX) : (ev.MaxY - ev.MinY);
                    double alt = 6357000 * Math.Sin(span / 2 * Angle.DegreeToRadians);
                    EarthGoto(lat, lon, alt);
                }
            }
            
        }

        void EarthGoto(double lat, double lon, double alt)
        {            
            _geoVis.Viewer.Pose = new CameraPose(Angle.FromDegrees(lat), Angle.FromDegrees(lon), alt);
        }

        public EventHandler OnCoordEnvelopeChanged
        {
            get { return _onCoordEnvelopeChanged; }
            set { _onCoordEnvelopeChanged = value; }
        }

        public void DisposeViewer()
        {
            _geoVis.DisposeMember();
            _geoVis.Dispose();
        }
    }
}
