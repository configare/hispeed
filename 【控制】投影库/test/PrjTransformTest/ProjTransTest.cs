using System;
using NUnit.Framework;
using GeoDo.Project;

namespace test
{
    [TestFixture]
    public class ProjTransTest
    {
        public ProjTransTest()
        {
        }

        [SetUp]
        public void SetUp()
        {
            PrjStdsMapTableParser.CnfgFile = @"D:\工作空间\MAS二期\06.开发代码\【控制】投影库\Output\GeoDo.Project.Cnfg.xml";
        }

        [Test]
        public void TestProjMap()
        {
            string proj4Str = "proj +proj=stere +lon_0=0 +lat_0=-90 +lat_ts=-71 +ellps=WGS84 +datum=WGS84";
            ISpatialReference srcSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("WGS 1984.prj");
            srcSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByProj4String(proj4Str);
            Console.WriteLine(srcSpatialRef.ToString());
        }
    }
}
