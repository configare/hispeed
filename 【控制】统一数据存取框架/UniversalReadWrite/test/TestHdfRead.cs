using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.Windows.Forms;
using OSGeo.GDAL;
using System.Threading;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public class TestHdfRead
    {
        public void Test()
        {
            //TestOnlyGDAL();
            TestGDALRead();
        }

        private void TestReadHDF()//持续运行多次后，自动退出
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

        private void TestOnlyGDAL()
        {
            Gdal.AllRegister();
            {
                try
                {
                    string dsFile = "HDF5:\"D:\\masData\\FY3A_VIRRX_GBAL_L1_20110322_0525_1000M_MS.HDF\"://EV_RefSB";
                    dsFile = "D:\\masData\\FY3A_VIRRX_GBAL_L1_20110322_0525_1000M_MS.HDF";
                    dsFile = "HDF5:\"D:\\masData中文\\FY3A_MERSI_GBAL_L1_20100429_0300_0250M_MS.HDF\"://Longitude";
                    dsFile = "HDF4_SDS:UNKNOWN:\"D:\\masData\\MODIS\\TERRA_2010_03_25_03_09_GZ.MOD03.hdf\":23";
                    Stopwatch sw = new Stopwatch();
                    for (int i = 0; i < 10; i++)
                    {
                        sw.Restart();
                        using (Dataset ds = Gdal.Open(dsFile, Access.GA_ReadOnly))
                        {
                            string[] s = ds.GetMetadata("Subdatasets");//Subdatasets
                            using (Band bd = ds.GetRasterBand(1))
                            {
                                Size srSize = new Size(bd.XSize, bd.YSize);
                                Double[] data = new Double[srSize.Width * srSize.Height];
                                unsafe
                                {
                                    fixed (Double* ptrLat = data)
                                    {
                                        IntPtr bufferPtrLat = new IntPtr(ptrLat);
                                        bd.ReadRaster(0, 0, srSize.Width, srSize.Height, bufferPtrLat, srSize.Width, srSize.Height, DataType.GDT_Float64, 0, 0);
                                    }
                                }
                                data = null;
                            }
                            sw.Stop();
                            Console.WriteLine("{0}:{1}", i, sw.ElapsedMilliseconds);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void TestGDALRead()
        {
            string dsFile = "";
            dsFile = "HDF4_SDS:UNKNOWN:\"D:\\masData\\MODIS\\TERRA_2010_03_25_03_09_GZ.MOD03.hdf\":23";
            dsFile = "D:\\masData\\MODIS\\TERRA_2010_03_25_03_09_GZ.MOD021KM.hdf";
            IRasterDataProvider srcRaster = GeoDataDriver.Open(dsFile) as IRasterDataProvider;
          
            IBandProvider srcbandpro = srcRaster.BandProvider;
            IDictionary<string, string> dsAtts = srcbandpro.GetDatasetAttributes("EV_1KM_RefSB");//reflectance_scales
            dsAtts = srcbandpro.GetAttributes();
            
        }
    }
}
