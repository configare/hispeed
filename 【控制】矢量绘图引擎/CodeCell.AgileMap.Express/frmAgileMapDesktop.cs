using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.UIs;


namespace CodeCell.AgileMap.Express
{
    public partial class frmAgileMapDesktop : Form,IMouseLocationInfoPrinter,IQueryResultContainer
    {
        private IApplication _application = null;

        public frmAgileMapDesktop()
        {
            InitializeComponent();
            ConstructUIs();
            RegMousePrinter();
            mapControl1.Resize += new EventHandler(mapControl1_Resize);
        }

        void mapControl1_Resize(object sender, EventArgs e)
        {
            mapControl1.ReRender();
        }

        private void RegMousePrinter()
        {
            (mapControl1 as IMapControl).Register(this);
            toolStripStatusLabel1.Text = string.Empty;
        }

        public IMapControl MapControl
        {
            get { return mapControl1 as IMapControl; }
        }

        private void printEnvelope(object obj, Envelope evp)
        {
            Console.WriteLine(evp.ToString());
        }

        private void ConstructUIs()
        {
            ResourceLoader r = new ResourceLoader("CodeCell.AgileMap.Express", "CodeCell.AgileMap.Express.ZResources.");
            _application = new ApplicationAgileMap(mapControl1, AppDomain.CurrentDomain.BaseDirectory + "Temp");
            IUIBuilder uibuilder = new UIBuilderAgileMap(this, _application);
            ICommandProvider cmdProvider = new CommandProviderAgileMap();
            uibuilder.Building(cmdProvider);
            SetToolStripExsDefaultOwner();
            mapControl1.MapRuntime.QueryResultContainer = this;
            mapControl1.MapRuntime.QueryResultContainer.Init(mapControl1.MapRuntime.LocationService);
        }

        private void SetToolStripExsDefaultOwner()
        {
            foreach (Control c in Controls)
                if (c is ToolStripEx)
                    (c as ToolStripEx).DefaultOwner = this;
        }

        #region IMouseLocationInfoPrinter Members

        public void Print(object sender, int x, int y, float prjX, float prjY, float geoX, float geoY)
        {
            string msg =        "像素坐标:{" + x.ToString() + "," + y.ToString() + "}," +
                                "投影坐标:{" + prjX.ToString("0.##") + "," + prjY.ToString("0.##") + "}," +
                                "地理坐标:{" + geoX.ToString("0.####") + "," + geoY.ToString("0.####") + "}";
            statusStrip1.Items[0].Text = msg;
        }

        #endregion

        #region IQueryResultContainer 成员

        public void Init(ILocationService locationsrv)
        {
            ucQueryResultContainer1.Init(mapControl1.MapRuntime);
        }

        public void AddFeatures(Feature[] features)
        {
            ucQueryResultContainer1.AddFeatures(features);
        }

        public void Clear()
        {
            ucQueryResultContainer1.Clear();
        }

        public bool ResultContainerVisible
        {
            get { return ucQueryResultContainer1.Visible; }
            set 
            {
                if (value != ucQueryResultContainer1.Visible)
                {
                    ucQueryResultContainer1.Visible = value;
                    mapControl1.ReRender();
                }
            }
        }

        public Control BeginInvokeContiner
        {
            get { return ucQueryResultContainer1; }
        }

        #endregion
    }
}
