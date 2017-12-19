using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.Project;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.FileProject;
using GeoDo.RasterProject;

namespace test2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TestPrj4Parser_Click(null, null);
        }

        private void TestPrj4Parser_Click(object sender, EventArgs e)
        {
            ISpatialReference srcSpatialRef = null;// SpatialReferenceFactory.GetSpatialReferenceByPrjFile("WGS 1984.prj");
            string prjFile = "f:\\32600.prj";
            srcSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile(prjFile);
            string proj4 = srcSpatialRef.ToProj4String();
            ISpatialReference inverSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByProj4String(proj4);

            bool isSame = srcSpatialRef.IsSame(inverSpatialRef);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //IRasterDataProvider mainRaster = null;
            //IRasterDataProvider locationRaster = null;
            ////string file = @"G:\hdf4\cloudsat\cloudsat\2007101034511_05065_CS_2B-CLDCLASS_GRANULE_P_R04_E02.hdf";
            //string file = @"G:\hdf4\MOD06_L2.A2011001.0350.hdf";
            //string file1 = @"G:\hdf4\MOD03.A2011001.0350.hdf";
            //string outDir = Path.GetDirectoryName(file);
            //try
            //{
            //    //Cloud_Mask_1km,
            //    string[] openArgs = new string[] { "datasets=" + "Cloud_Mask_1km"};
            //    //Cloud_Top_Temperature,Cloud_Fraction,Cloud_Phase_Infrared,Water_Path,cloud_Optical_Thickness,Effective_Particle_Radius" };
            //    //string[] openArgs = new string[] { "datasets=" + "cloud_scenario" };
            //    mainRaster = RasterDataDriver.Open(file, openArgs) as IRasterDataProvider;
            //    string[] locationArgs = new string[] { "datasets=" + "Latitude,Longitude", "geodatasets=" + "Latitude,Longitude" };
            //    //string[] locationArgs = new string[] { "datasets=" + "latitude,longitude", "geodatasets=" + "latitude,longitude" };
            //    locationRaster = RasterDataDriver.Open(file1, locationArgs) as IRasterDataProvider;
            //    if (locationRaster == null || locationRaster.BandCount == 0)
            //    {
            //        //msgInfo.AppendLine("经纬度HDF数据文件，不存在经纬度数据集[Longitude,Latitude]" + file);
            //        return;
            //    }
            //    string outFilename = Path.Combine(outDir, Path.GetFileNameWithoutExtension(file) + ".LDF");
            //    HDF4FilePrjSettings setting = new HDF4FilePrjSettings();
                
            //    setting.LocationFile = locationRaster;
            //    setting.OutFormat = "LDF";
            //    setting.OutPathAndFileName = outFilename;
            //    setting.OutResolutionX = 0.01f;
            //    setting.OutResolutionY = 0.01f;
            //    setting.OutEnvelope = new PrjEnvelope(99.5, 109.5, 20, 30);
            //    HDF4FileProjector projector = new HDF4FileProjector();
            //    projector.Project(mainRaster, setting, SpatialReference.GetDefault(), null);
            //}
            //finally
            //{
            //    if (mainRaster != null)
            //    {
            //        mainRaster.Dispose();
            //        mainRaster = null;
            //    }
            //    if (locationRaster != null)
            //    {
            //        locationRaster.Dispose();
            //        locationRaster = null;
            //    }
            //}
            string file = @"C:\Users\DongW\Documents\Tencent Files\379336658\FileRecv\MOD021KM.A2005001.0120.005.2010156230829\MOD021KM.A2005001.0120.005.2010156230829.hdf";
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(file) as IRasterDataProvider)
            {
                dataPrd.Attributes.GetAttributeDomain("RANGE BEGINNING DATE");
            }
        }
    }
}
