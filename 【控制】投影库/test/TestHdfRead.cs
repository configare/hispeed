using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;

namespace test
{
    [TestFixture]
    public class TestHdfRead
    {
        [Test]
        public void TestReadHDF()//持续运行多次后，自动退出
        {
            IRasterDataProvider srcPrd = GetReader();
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    IBandProvider srcbandpro = srcPrd.BandProvider as IBandProvider;
                    {
                        IRasterBand[] latBands = srcbandpro.GetBands("EV_250_RefSB_b1");
                        IRasterBand latBand = latBands[0];
                        {
                            Size srSize = new Size(latBand.Width / 2, latBand.Height / 2);
                            UInt16[] lats = new UInt16[srSize.Width * srSize.Height];
                            unsafe
                            {
                                fixed (UInt16* ptrLat = lats)
                                {
                                    IntPtr bufferPtrLat = new IntPtr(ptrLat);
                                    latBand.Read(0, 0, srSize.Width, srSize.Height, bufferPtrLat, enumDataType.UInt16, srSize.Width, srSize.Height);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                srcPrd.Dispose();
                GC.SuppressFinalize(false);
            }
        }

        private IRasterDataProvider GetReader()
        {
            string srcFilename = @"D:\masData\Mersi\FY3A_MERSI_GBAL_L1_20110501_0250_0250M_MS.HDF";
            IRasterDataProvider srcPrd = GeoDataDriver.Open(srcFilename) as IRasterDataProvider;
            return srcPrd;
        }
    }
}
