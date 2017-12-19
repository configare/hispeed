using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GeoDo.Project;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace LdfDriverTestUnit
{
    [TestFixture]
    public class MvgDriver_ReadMVGByProvider
    {
        protected string _fname = @"E:\气象局项目\MAS二期\mvg文件\FOG_DBLV_FY3A_VIRR_1000M_DXX_P001_200911131354.mvg";

        [Test]
        public unsafe void ReadMvgFileNormal()
        {
            // _fname = @"C:\工作\3.13\SNW_DBLV_FY3A_VIRR_1000M_NULL_P001_200911131356.mvg";
            IGeoDataDriver drv = GeoDataDriver.GetDriverByName("MVG");
            IRasterDataProvider prd = drv.Open(_fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            Assert.NotNull(prd);
            Int16[] data = new Int16[prd.Width * prd.Height];
            Console.WriteLine("prd.Width = " + prd.Width);
            Console.WriteLine("prd.Height = " + prd.Height);
            int[] bandMap = { 1 };
            fixed (Int16* ptr = data)
            {
                IntPtr buffer = new IntPtr(ptr);
                prd.Read(0, 0, prd.Width, prd.Height, buffer, enumDataType.Int16, prd.Width, prd.Height, 1, bandMap, enumInterleave.BSQ);
            }
            string dstFname = @"e:\mvg";//@"C:\工作\3.13\read";
            WriteBandToFile(data, dstFname + "_normal" + "_prdRead" + ".dat");
        }

        [Test]
        public unsafe void ReadMvgFile_offset_bigger()
        {
            // _fname = @"C:\工作\3.13\SNW_DBLV_FY3A_VIRR_1000M_NULL_P001_200911131356.mvg";
            IGeoDataDriver drv = GeoDataDriver.GetDriverByName("MVG");
            IRasterDataProvider prd = drv.Open(_fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            Assert.NotNull(prd);
            Int16[] data = new Int16[prd.Width  * prd.Height];
            Console.WriteLine("prd.Width = " + prd.Width);
            Console.WriteLine("prd.Height = " + prd.Height);
            int[] bandMap = { 1 };
            fixed (Int16* ptr = data)
            {
                IntPtr buffer = new IntPtr(ptr);
                prd.Read(10, 1, prd.Width/2, prd.Height/2, buffer, enumDataType.Int16, prd.Width , prd.Height, 1, bandMap, enumInterleave.BSQ);
            }
            string dstFname = @"e:\mvg";//@"C:\工作\3.13\read";
            WriteBandToFile(data, dstFname +"_offset"+ "_bigger" + "_prdRead" + ".dat");
        }

        [Test]
        public unsafe void ReadMvgFile_smaller()
        {
            // _fname = @"C:\工作\3.13\SNW_DBLV_FY3A_VIRR_1000M_NULL_P001_200911131356.mvg";
            IGeoDataDriver drv = GeoDataDriver.GetDriverByName("MVG");
            IRasterDataProvider prd = drv.Open(_fname, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
            Assert.NotNull(prd);
            Int16[] data = new Int16[prd.Width/2 * prd.Height];
            Console.WriteLine("prd.Width = " + prd.Width);
            Console.WriteLine("prd.Height = " + prd.Height);
            int[] bandMap = { 1 };
            fixed (Int16* ptr = data)
            {
                IntPtr buffer = new IntPtr(ptr);
                prd.Read(0, 0, prd.Width, prd.Height, buffer, enumDataType.Int16, prd.Width / 2, prd.Height, 1, bandMap, enumInterleave.BSQ);
            }
            string dstFname = @"e:\mvg";//@"C:\工作\3.13\read";
            WriteBandToFile(data, dstFname  + "_smaller" + "_prdRead" + ".dat");
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
