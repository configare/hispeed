using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Core;
using CodeCell.AgileMap.Components;
using CodeCell.Bricks.UIs;

namespace CodeCell.AgileMap.Catalog
{
    public partial class frmCatalog : Form,IMouseLocationInfoPrinter
    {
        private IApplication _application = null;

        public frmCatalog()
        {
            ResourceLoader r = new ResourceLoader("CodeCell.AgileMap.Catalog", "CodeCell.AgileMap.Catalog.ZResources.");
            InitializeComponent();
            ConstructUIs();
            AttachEvents();
            RegisterMouseTips();
        }

        private void RegisterMouseTips()
        {
            mapControl1.Register(this);
        }

        private void AttachEvents()
        {
            ucBudGISDataSource1.OnCatalogItemDoubleClicked += new OnCatalogItemDoubleClickedHandler(ucBudGISDataSource1_OnCatalogItemDoubleClicked);
        }

        void ucBudGISDataSource1_OnCatalogItemDoubleClicked(object sender, ICatalogItem catalogItem)
        {
            if (catalogItem is CatalogFile)
            {
                CatalogFile file = catalogItem as CatalogFile;
                DisplayFile(file);
            }
            else if (catalogItem is ICatalogEntityClass)
            { 
            }
            else if (catalogItem is CatalogFeatureClass)
            {
                DisplayFeatureClass(catalogItem as CatalogFeatureClass);
            }
        }

        private void DisplayFeatureClass(CatalogFeatureClass catalogFeatureClass)
        {
            SpatialFeatureClass fetclass = catalogFeatureClass.Tag as SpatialFeatureClass;
            FeatureDataSourceBase ds = new SpatialDbDataSource(fetclass.Name, fetclass.ConnString+"@"+fetclass.DataTable);
            FeatureClass fetc = new FeatureClass(ds);
            IFeatureLayer lyr = new FeatureLayer(fetclass.Name, fetc);
            IMap map = new Map(new IFeatureLayer[] { lyr });
            mapControl1.Apply(map);
            mapControl1.ReRender();
        }

        private void DisplayFile(CatalogFile file)
        {
            IFeatureLayer lyr = new FeatureLayer(Path.GetFileNameWithoutExtension(file.Tag.ToString()),
                file.Tag.ToString());
            IMap map = new Map(new IFeatureLayer[] { lyr });
            mapControl1.Apply(map);
            mapControl1.ReRender();
        }

        private void ConstructUIs()
        {
            _application = new ApplicationCatalog(mapControl1, AppDomain.CurrentDomain.BaseDirectory + "Temp");
            IUIBuilder uibuilder = new UIBuilderCatalog(this, _application);
            ICommandProvider cmdProvider = new CommandProviderCatalog();
            uibuilder.Building(cmdProvider);
            SetToolStripExsDefaultOwner();
        }

        private void SetToolStripExsDefaultOwner()
        {
            foreach (Control c in Controls)
                if (c is ToolStripEx)
                    (c as ToolStripEx).DefaultOwner = this;
        }

        public void Print(object sender, int x, int y, float prjX, float prjY, float geoX, float geoY)
        {
            string msg = "像素坐标:{" + x.ToString() + "," + y.ToString() + "}," +
                                "投影坐标:{" + prjX.ToString("0.##") + "," + prjY.ToString("0.##") + "}," +
                                "地理坐标:{" + geoX.ToString("0.####") + "," + geoY.ToString("0.####") + "}";
            statusStrip1.Items[0].Text = msg;
        }
    }
}
