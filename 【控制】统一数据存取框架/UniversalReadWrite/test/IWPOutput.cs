using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Windows.Forms;
using System.Drawing;
using GeoDo.RSS.DF.GDAL.HDF5Universal;

namespace WindowsFormsApplication1
{
    public class IWPOutput
    {
        private static string L2IWPFlag = "*_MWHSX_GBAL_L2_IWP_*_POAD_*.HDF";
        private static string[] L2IWPDataset = new string[] { "IWP_183_1_Ascent SDS", "IWP_183_1_Dscent SDS", "IWP_183_3_Ascent SDS", "IWP_183_3_Dscent SDS", "IWP_183_7_Ascent SDS", "IWP_183_7_Dscent SDS" };
        private static string L2IWPDatasetStr = "";

        private static string L1EarthBTFlag = "*_MWHSX_GBAL_L1*KM*.HDF";
        private static string[] L1EarthBTDataset = new string[] { "Earth_Obs_BT", "Longitude", "Latitude" };
        private static string[] L1EarthBTGroupDataset = new string[] { "Data/Earth_Obs_BT", "Geolocation/Longitude", "Geolocation/Latitude" };

        #region L2IWP

        public static bool L2ProOutput(string inputDir, string outDir, out string error)
        {
            error = string.Empty;
            if (!Directory.Exists(inputDir))
            {
                error += "文件目录不存在!\n";
                return false;
            }
            string[] files = Directory.GetFiles(inputDir, L2IWPFlag, SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
            {
                error += "没有查询到匹配的文件!\n";
                return false;
            }
            GetL2IWPDataset();
            foreach (string file in files)
            {
                if (!OutputL2FileByOne(file, outDir, ref error))
                    continue;
            }
            return true;
        }

        private static void GetL2IWPDataset()
        {
            L2IWPDatasetStr = "datasets=";
            foreach (string item in L2IWPDataset)
                L2IWPDatasetStr += item + ",";
            L2IWPDatasetStr = L2IWPDatasetStr.Substring(0, L2IWPDatasetStr.Length - 1);
        }

        private static bool OutputL2FileByOne(string hdfFile, string outDir, ref string error)
        {
            IRasterDataProvider srcPrd = HDF5Driver.Open(hdfFile, enumDataProviderAccess.ReadOnly, new string[] { L2IWPDatasetStr }) as IRasterDataProvider;
            float[] data = null;
            string outDirBase = outDir + "\\L2IWP\\";
            Size srSize = Size.Empty;

            try
            {
                for (int bandNo = 1; bandNo <= srcPrd.BandCount; bandNo++)
                {
                    IBandProvider srcbandpro = srcPrd.BandProvider as IBandProvider;
                    {
                        IRasterBand latBand = srcPrd.GetRasterBand(bandNo);
                        {
                            srSize = new Size(latBand.Width, latBand.Height);
                            data = new float[srSize.Width * srSize.Height];
                            unsafe
                            {
                                fixed (float* ptrLat = data)
                                {
                                    IntPtr bufferPtrLat = new IntPtr(ptrLat);
                                    latBand.Read(0, 0, srSize.Width, srSize.Height, bufferPtrLat, enumDataType.Float, srSize.Width, srSize.Height);
                                }
                            }
                        }
                        string outFilename = L2IWPDataset[bandNo - 1] + ".txt";
                        WriteL2IWPFile(outDirBase + Path.GetFileNameWithoutExtension(hdfFile) + "\\", outFilename, data, srSize);
                    }
                }
            }
            catch (Exception ex)
            {
                error += hdfFile + ": " + ex.Message + "\n";
            }
            finally
            {
                data = null;
                srcPrd.Dispose();
                GC.SuppressFinalize(false);
            }
            return true;
        }

        private static void WriteL2IWPFile(string outDir, string filename, float[] data, Size srSize)
        {
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            if (data == null)
                return;
            List<string> fileContext = new List<string>();
            string temp = null;
            for (int row = 0; row < srSize.Height; row++)
            {
                temp = string.Empty;
                for (int col = 0; col < srSize.Width; col++)
                    temp += data[row * srSize.Width + col] + "\t";
                fileContext.Add(temp);
            }
            File.WriteAllLines(outDir + filename, fileContext.ToArray(), Encoding.Default);
        }

        #endregion

        #region L1EarthBT

        public static bool L1GBALOutput(string inputDir, string outDir, out string error)
        {
            error = string.Empty;
            if (!Directory.Exists(inputDir))
            {
                error += "文件目录不存在!\n";
                return false;
            }
            string[] files = Directory.GetFiles(inputDir, L1EarthBTFlag, SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
            {
                error += "没有查询到匹配的文件!\n";
                return false;
            }
            string dataSets = string.Empty;
            foreach (string file in files)
            {
                if (!OutputL1FileByOne(file, outDir, ref error))
                {
                    dataSets = GetL1GBALDataset(true);

                }
            }
            return true;
        }

        private static string GetL1GBALDataset(bool group)
        {
            string GetL1GBALDatasetStr = "datasets=";
            if (!group)
                foreach (string item in L1EarthBTDataset)
                    GetL1GBALDatasetStr += item + ",";
            else
                foreach (string item in L1EarthBTGroupDataset)
                    GetL1GBALDatasetStr += item + ",";
            GetL1GBALDatasetStr = GetL1GBALDatasetStr.Substring(0, GetL1GBALDatasetStr.Length - 1);
            return GetL1GBALDatasetStr;
        }

        private static bool OutputL1FileByOne(string hdfFile, string outDir, ref string error)
        {
            string L1GBALDataSetsStr = GetL1GBALDataset(false);
            IRasterDataProvider srcPrd = HDF5Driver.Open(hdfFile, enumDataProviderAccess.ReadOnly, new string[] { L1GBALDataSetsStr }) as IRasterDataProvider;
            if (srcPrd.BandCount == 0)
            {
                srcPrd.Dispose();
                L1GBALDataSetsStr = GetL1GBALDataset(true);
                srcPrd = HDF5Driver.Open(hdfFile, enumDataProviderAccess.ReadOnly, new string[] { L1GBALDataSetsStr }) as IRasterDataProvider;
            }
            float[] data = null;
            float[] lon = null;
            float[] lat = null;
            string outDirBase = outDir + "\\L1GBAL\\";
            Size srSize = Size.Empty;
            try
            {
                GetBandData(srcPrd, srcPrd.BandCount - 1, ref lon, ref srSize);
                GetBandData(srcPrd, srcPrd.BandCount, ref lat, ref srSize);
                for (int bandNo = 1; bandNo <= srcPrd.BandCount - 2; bandNo++)
                {
                    IBandProvider srcbandpro = srcPrd.BandProvider as IBandProvider;
                    {
                        IRasterBand latBand = srcPrd.GetRasterBand(bandNo);
                        {
                            srSize = new Size(latBand.Width, latBand.Height);
                            data = new float[srSize.Width * srSize.Height];
                            unsafe
                            {
                                fixed (float* ptrLat = data)
                                {
                                    IntPtr bufferPtrLat = new IntPtr(ptrLat);
                                    latBand.Read(0, 0, srSize.Width, srSize.Height, bufferPtrLat, enumDataType.Float, srSize.Width, srSize.Height);
                                }
                            }
                        }
                        string outFilename = "band_" + bandNo + ".txt";
                        WriteL1GBALFile(outDirBase + Path.GetFileNameWithoutExtension(hdfFile) + "\\", outFilename, data, lon, lat, srSize);
                    }
                }
            }
            catch (Exception ex)
            {
                error += hdfFile + ": " + ex.Message + "\n";
                return false;
            }
            finally
            {
                data = null;
                srcPrd.Dispose();
                GC.SuppressFinalize(false);
            }
            return true;
        }

        private static void GetBandData(IRasterDataProvider srcPrd, int bandNo, ref float[] data, ref Size srSize)
        {
            IBandProvider srcbandpro = srcPrd.BandProvider as IBandProvider;
            {
                IRasterBand latBand = srcPrd.GetRasterBand(bandNo);
                {
                    srSize = new Size(latBand.Width, latBand.Height);
                    data = new float[srSize.Width * srSize.Height];
                    unsafe
                    {
                        fixed (float* ptrLat = data)
                        {
                            IntPtr bufferPtrLat = new IntPtr(ptrLat);
                            latBand.Read(0, 0, srSize.Width, srSize.Height, bufferPtrLat, enumDataType.Float, srSize.Width, srSize.Height);
                        }
                    }
                }
            }
        }

        private static void WriteL1GBALFile(string outDir, string filename, float[] data, float[] lon, float[] lat, Size srSize)
        {
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            if (data == null)
                return;
            List<string> fileContext = new List<string>();
            //string temp = null;
            //int index = 0;
            //for (int row = 0; row < srSize.Height; row++)
            //{
            //    temp = string.Empty;
            //    for (int col = 0; col < srSize.Width; col++)
            //    {
            //        index = row * srSize.Width + col;
            //        temp += lon[index] + "\t" + lat[index] + "\t" + data[index] + "\t";
            //    }
            //    fileContext.Add(temp);
            //}
            for (int index = 0; index < data.Length; index++)
                fileContext.Add(lon[index].ToString().PadRight(10, '0') + "\t" + lat[index].ToString().PadRight(10, '0') + "\t" + data[index].ToString().PadRight(10, '0'));
            File.WriteAllLines(outDir + filename, fileContext.ToArray(), Encoding.Default);
        }

        #endregion
    }

}
