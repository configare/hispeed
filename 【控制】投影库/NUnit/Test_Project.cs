using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.FileProject;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;

namespace NUnit
{
    [TestFixture]
    class Test_Project
    {
        [Test]
        public void Porject_VIRR_FULL()
        {
            IFileProjector prj = FileProjector.GetFileProjectByName("FY3_VIRR");
            Assert.NotNull(prj);
            IRasterDataProvider prd = GetSrcRaster();
            Assert.NotNull(prd);
            prj.Project(prd, GetFilePrjSettings(prd), new SpatialReference(new GeographicCoordSystem()),null );
            Console.WriteLine("OK.");
        }

        private FilePrjSettings GetFilePrjSettings(IRasterDataProvider prd)
        {
            FY3_VIRR_PrjSettings settings = new FY3_VIRR_PrjSettings();
            BandMap map = new BandMap();
            map.File = prd ;
            map.DatasetName = "EV_RefSB";
            map.BandIndex =0;
            settings.BandMapTable.Add(map);
            settings.OutResolutionX = 0.01f;
            settings.OutResolutionY = 0.01f;
            settings.OutEnvelope = new GeoDo.RasterProject.PrjEnvelope(86, 136, 19, 60);
            settings.OutFormat = "LDF";
            settings.OutPathAndFileName = "f:\\Out.LDF";
            return settings;
        }

        private IRasterDataProvider GetSrcRaster()
        {
            string fname = "f:\\FY3A_VIRR_2010_06_24_11_39_1000M_L1B.HDF";
            return GeoDataDriver.Open(fname) as IRasterDataProvider;
        }
    }
}
