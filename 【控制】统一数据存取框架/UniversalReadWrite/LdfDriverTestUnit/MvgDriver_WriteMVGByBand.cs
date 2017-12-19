using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.RSS.Core.DF;
using System.IO;
using GeoDo.RSS.DF.MVG;

namespace LdfDriverTestUnit
{
    [TestFixture]
    public class MvgDriver_WriteMVGByBand
    {
        protected string _fname = @"E:\气象局项目\MAS二期\mvg文件\FOG_DBLV_FY3A_VIRR_1000M_DXX_P001_200911131354.mvg";

        [Test]
        public unsafe void WriteMvgFileNormal()
        {
            // _fname = @"C:\工作\3.13\SNW_DBLV_FY3A_VIRR_1000M_NULL_P001_200911131356.mvg";
            IGeoDataDriver drv = GeoDataDriver.GetDriverByName("MVG");
            IRasterDataProvider prd = drv.Open(_fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            Assert.NotNull(prd);
            IRasterBand band = prd.GetRasterBand(1);
            Assert.NotNull(band);
            Int16[] buffer = new Int16[prd.Width * prd.Height];
            Console.WriteLine("prd.Width = " + prd.Width);
            Console.WriteLine("prd.Height = " + prd.Height);
            Console.WriteLine("band.Width = " + band.Width);
            Console.WriteLine("band.Height= " + band.Height);
            fixed (Int16* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                band.Read(0, 0, prd.Width, prd.Height, bufferPtr, enumDataType.Int16, prd.Width, prd.Height);
            }

            string dstFname = @"e:\mvg_writer.bin";
            string mapInfo = "{1,1}:{110,35}:{0.01,0.01}";
            Int16 valueCount = 2;
            bool withHdr = true;
            string values = "{ 0, 1 }";
            string valueNames = "{120,99}";
            IMvgDataProvider prdWriter = (drv as IRasterDataDriver).Create(dstFname, prd.Width, prd.Height, 1, enumDataType.Int16, "VALUECOUNT=" + valueCount, "VALUES=" + values,
                                            "VALUENAMES=" + valueNames, "MAPINFO=" + mapInfo, "WITHHDR=" + withHdr.ToString()) as IMvgDataProvider;
            IRasterBand bandWriter = prdWriter.GetRasterBand(1);
            fixed (Int16* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                bandWriter.Write(0, 0, prd.Width, prd.Height, bufferPtr, enumDataType.Int16, prd.Width, prd.Height);
            }
            prd.Dispose();
            drv.Dispose();
            prdWriter.Dispose();
        }

        private void WriteBandToFile(Int16[] bandValues, string fname)
        {
            using (FileStream fs = new FileStream(fname, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    int len = bandValues.Length;
                    for (int i = 0; i < len; i++)
                    {
                        bw.Write(bandValues[i]);
                    }
                }
            }
        }
    }
}
