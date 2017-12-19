using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.FileProject;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;

namespace test
{
    [TestFixture]
    public class Test_Project
    {
        [Test]
        public void Porject_VIRR_FULL()
        {
            IFileProjector prj = FileProjector.GetFileProjectByName("FY3_VIRR");
            //IFileProjector prj = new FY3_VIRRFileProjector();
            Assert.NotNull(prj);
            IRasterDataProvider prd = GetSrcRaster();
            Assert.NotNull(prd);
            prj.Project(prd, GetFilePrjSettings(prd), new SpatialReference(new GeographicCoordSystem()),null );
            Console.WriteLine("OK.");

        }

        private FilePrjSettings GetFilePrjSettings(IRasterDataProvider prd)
        {
            FY3_VIRR_PrjSettings settings = new FY3_VIRR_PrjSettings();
            //BandMap map = new BandMap();
            //map.File = prd ;//default 
            //map.DatasetName = "EV_RefSB"; //default
            //map.BandIndex =0;//default
            //settings.BandMapTable = new List<BandMap>(); //inside create
            //settings.BandMapTable.Add(map);
            settings.OutResolutionX = 0.01f; // default
            settings.OutResolutionY = 0.01f; // default
            settings.OutEnvelope = new GeoDo.RasterProject.PrjEnvelope(86, 136, 19, 60); //default
            settings.OutFormat = "LDF"; //default
            settings.OutPathAndFileName = "f:\\Out.LDF";
            return settings;
        }

        private IRasterDataProvider GetSrcRaster()
        {
            string fname = "f:\\FY3A_VIRR_2010_06_24_11_39_1000M_L1B.HDF";
            return GeoDataDriver.Open(fname) as IRasterDataProvider;
        }

        public static void Hammer()
        {
            double prjX = -12756274.0;
            double prjY = 6378137.0;
            //prjX = -12756849.0;
            //prjY = 6379251.5;
            double[] xs = new double[] { prjX };
            double[] ys = new double[] { prjY };
            Proj4Projection _srcProjection = new Proj4Projection("+proj=latlong +datum=WGS84 +a=6378137 +ellps=WGS84 +towgs84=0,0,0");
            Proj4Projection _dstProjection = new Proj4Projection("+proj=hammer +lat_0=0 +lon_0=105 +x_0=0 +y_0=0 +datum=WGS84 +a=6378137 +ellps=WGS84 +towgs84=0,0,0");
            Proj4Projection.Transform(_dstProjection, _srcProjection, xs, ys);

            //IProjectionTransform tr = ProjectionTransformFactory.GetProjectionTransform(new 
            //_projectionTransform.InverTransform(xs, ys);
            double geoX = xs[0];
            double geoY = ys[0];
        }
    }
}
