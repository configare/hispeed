using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.Project;
using System.IO;

namespace NUnit
{
    [TestFixture]
    public class Test_GetSpatialRefFromProj4String
    {
        [SetUp]
        public void LdfFileName()
        {
            PrjStdsMapTableParser.CnfgFile = @"F:\技术研究\MAS_II\源代码\【控制】统一数据存取框架\RefDLL\Project\GeoDo.Project.Cnfg.xml";
        }

        [Test]
        public void ParserTiff()
        {
            string spreftxt = File.ReadAllText("f:\\2.prj");
            ISpatialReference spref = SpatialReferenceFactory.GetSpatialReferenceByWKT(spreftxt, enumWKTSource.GDAL);
            Assert.NotNull(spref);
            Console.Write(spref.ToWKTString());
        }

        [Test]
        public void Test()
        {
            ISpatialReference srcSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("WGS 1984.prj");
            srcSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("Mercator (sphere).prj");
            string proj4 = srcSpatialRef.ToProj4String();
            Assert.AreEqual(proj4, "+proj=merc +x_0=0 +y_0=0 +lon_0=0 +lat_1=0 +datum=WGS84 +a=6371000 +b=6356863.01877305+f=正无穷大 +nodefs");
            srcSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByProj4String(proj4);
        }

        [Test]
        public void ParseProj4()
        {
            string proj4 = "+proj=merc +lon_0=0 +k0=1 +x_0=0 +y_0=0 +a=6378137 +b=6378137";
            Console.WriteLine("INPUT:");
            Console.WriteLine(proj4);
            ISpatialReference spatialRef = SpatialReferenceFactory.GetSpatialReferenceByProj4String(proj4);
            Console.WriteLine("ESRI WKT:");
            Console.WriteLine(spatialRef.ToString());
            Console.WriteLine("PROJ.4:");
            Console.WriteLine(spatialRef.ToProj4String());
            Console.WriteLine("OGC WKT:");
            Console.WriteLine(spatialRef.ToWKTString());
            Console.WriteLine("ENVI Projection Info:");
            Console.WriteLine(spatialRef.ToEnviProjectionInfoString());
        }
    }
}
