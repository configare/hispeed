using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;

namespace LdfDriverTestUnit
{
    [TestFixture]
    public class URW_SpatialRef
    {
        [SetUp]
        public void Init()
        {
            PrjStdsMapTableParser.CnfgFile = @"F:\技术研究\MAS_II\home\源代码\【控制】统一数据存取框架\RefDLL\Project\GeoDo.Project.Cnfg.xml";
        }

        [Test]
        public void Tiff()
        {
            string fname = "f:\\4_大昭寺_IMG_GE.tif";
            PrintSpatialRefOfFile(fname);
        }

        [Test]
        public void Bmp()
        {
            string fname = "f:\\1.jpg";
            PrintSpatialRefOfFile(fname);
        }

        [Test]
        public void HamPrj()
        {
            string fname = "f:\\FY3A_VIRRD_10A0_L2_LST_MLT_HAM_20110815_POAD_1000M_MS_Mosic.ldf";
            PrintSpatialRefOfFile(fname);
        }

        private void PrintSpatialRefOfFile(string fname)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            Assert.NotNull(prd);
            ISpatialReference spref = prd.SpatialRef;
            if (spref != null)
            {
                Console.Write(spref.ToWKTString() + "\n");
                Console.WriteLine(spref.ProjectionCoordSystem == null ? "Is Geographics Coordinate System" : "Is Projection Coordinate System");
            }
            else
            {
                Console.WriteLine("SpatialRef is NULL");
            }
            ICoordTransform rct = prd.CoordTransform;
            Assert.NotNull(rct);
            for (int i = -10; i < 100; i++)
            {
                for (int j = -10; j < 100; j++)
                {
                    double x = 0, y = 0;
                    rct.Raster2DataCoord(i, j, out x, out y);
                    Console.Write("(" + x.ToString("0.####").PadRight(7, ' ') + "," + y.ToString("0.####").PadRight(7, ' ') + "),");
                }
                Console.Write("\n");
            }
        }
    }
}
