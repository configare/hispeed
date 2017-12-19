using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.FileProject;
using GeoDo.Project;
using GeoDo.RasterProject;
using GeoDo.RSS.Core.DF;

namespace GeoDo.FileProject
{
    public class FileFinder
    {
        public static IRasterDataProvider TryFind03FileFromModisImgFile(IRasterDataProvider fileRaster)
        {
            string dir = Path.GetDirectoryName(fileRaster.fileName);
            string fileName = Path.GetFileName(fileRaster.fileName);
            string retFile;
            if (fileName.Contains("MOD021KM"))
                retFile = fileName.Replace("MOD021KM", "MOD03");
            else if (fileName.Contains("MOD02HKM"))
                retFile = fileName.Replace("MOD02HKM", "MOD03");
            else if (fileName.Contains("MOD02QKM"))
                retFile = fileName.Replace("MOD02QKM", "MOD03");
            else if (fileName.Contains("MYD021KM"))
                retFile = fileName.Replace("MYD021KM", "MYD03");
            else if (fileName.Contains("MYD02HKM"))
                retFile = fileName.Replace("MYD02HKM", "MYD03");
            else if (fileName.Contains("MYD02QKM"))
                retFile = fileName.Replace("MYD02QKM", "MYD03");
            else
                throw new Exception("无法找到角度数据(如经纬度等)文件[.MOD/MYD03.hdf]");
            retFile = Path.Combine(dir, retFile);
            if (retFile == fileName || !File.Exists(retFile))
                throw new Exception("无法找到角度数据(如经纬度等)文件[.MOD/MYD03.hdf]");
            try
            {
                return Open(retFile);
            }
            catch (Exception ex)
            {
                throw new Exception("读取经纬度文件失败，无法使用角度数据(如经纬度等)文件[.MOD/MYD03.hdf]" + ex.Message, ex);
            }
        }

        public static IRasterDataProvider TryFindMODIS_1KM_L1From03(IRasterDataProvider fileRaster)
        {
            string dir = Path.GetDirectoryName(fileRaster.fileName);
            string fileName = Path.GetFileName(fileRaster.fileName);
            string retFile = null;
            retFile = fileName.Replace("MOD03", "MOD021KM").Replace("MYD03", "MYD021KM");
            retFile = Path.Combine(dir, retFile);
            if (retFile == fileName || !File.Exists(retFile))
                return null;
            return Open(retFile);
        }

        public static IRasterDataProvider TryFindMODIS_HKM_L1From03(IRasterDataProvider fileRaster)
        {
            string dir = Path.GetDirectoryName(fileRaster.fileName);
            string fileName = Path.GetFileName(fileRaster.fileName);
            string retFile = null;
            retFile = fileName.Replace("MOD03", "MOD02HKM").Replace("MYD03", "MYD02HKM");
            retFile = Path.Combine(dir, retFile);
            if (retFile == fileName || !File.Exists(retFile))
                return null;
            return Open(retFile);
        }

        public static IRasterDataProvider TryFindMODIS_QKM_L1From03(IRasterDataProvider fileRaster)
        {
            string dir = Path.GetDirectoryName(fileRaster.fileName);
            string fileName = Path.GetFileName(fileRaster.fileName);
            string retFile = null;
            retFile = fileName.Replace("MOD03", "MOD02QKM").Replace("MYD03", "MYD02QKM");
            retFile = Path.Combine(dir, retFile);
            if (retFile == fileName || !File.Exists(retFile))
                return null;
            return Open(retFile);
        }

        public static IRasterDataProvider Open(string filename)
        {
            return GeoDataDriver.Open(filename) as IRasterDataProvider;
        }

        /// <summary>
        /// 20130418添加了对以下类型文件名的支持。(尚未完成)
        /// Z_SATE_C_BAWX_20130321034403_P_FY3B_MERSI_GBAL_L1_20110220_0510_0250M_MS.HDF
        /// Z_SATE_C_BAWX_20130321034729_P_FY3B_MERSI_GBAL_L1_20110220_0510_1000M_MS.HDF
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static IRasterDataProvider TryFindMERSI_1KM_L1FromQKM(IRasterDataProvider fileRaster)
        {
            try
            {
                string dir = Path.GetDirectoryName(fileRaster.fileName);
                string fileName = Path.GetFileName(fileRaster.fileName);
                string retFile = fileName.Replace("_0250M_", "_1000M_");
                retFile = Path.Combine(dir, retFile);
                if (retFile == fileName || !File.Exists(retFile))
                {//这里再进一步扫描目录下的文件，用正则匹配，如果能找到，同样适用。
                    throw new Exception("无法找到对应1KM文件(获取太阳天顶角等角度数据使用)");
                }
                return Open(retFile);
            }
            catch (Exception ex)
            {
                throw new Exception("无法找到对应1KM文件(获取太阳天顶角等角度数据使用)", ex);
            }
        }

        public static IRasterDataProvider TryFindMERSI_QKM_L1FromKM(IRasterDataProvider fileRaster)
        {
            try
            {
                string dir = Path.GetDirectoryName(fileRaster.fileName);
                string fileName = Path.GetFileName(fileRaster.fileName);
                string retFile = fileName.Replace("_1000M_", "_0250M_");
                if (retFile == fileName)//文件名中不包含_1000M_
                    return null;
                retFile = Path.Combine(dir, retFile);
                if (File.Exists(retFile))
                    return Open(retFile);
                //if (kmFileName.Contains("Z_SATE_C_BAWX_"))
                //{ 
                //}
                return null;
            }
            catch
            {
                return null;
            }
        }

        public static IRasterDataProvider TryFindGeoFileFromFY3C_VIRR(IRasterDataProvider fileRaster)
        {
            string dir = Path.GetDirectoryName(fileRaster.fileName);
            string fileName = Path.GetFileName(fileRaster.fileName);
            string retFile;
            if (fileName.Contains("_1000M_"))
                retFile = fileName.Replace("_1000M_", "_GEOXX_");
            else
                throw new Exception("无法找到角度数据(如经纬度等)文件[._GEOXX_...HDF]");
            retFile = Path.Combine(dir, retFile);
            if (retFile == fileName || !File.Exists(retFile))
                throw new Exception("无法找到角度数据(如经纬度等)文件[._GEOXX_...HDF]");
            try
            {
                return Open(retFile);
            }
            catch (Exception ex)
            {
                throw new Exception("获取经纬度文件失败" + ex.Message, ex);
            }
        }
        
        public static IRasterDataProvider TryFindGeoFileFromFY3C_MERSI(IRasterDataProvider fileRaster)
        {
            try
            {
                IRasterDataProvider kmGeo = TryFindkmGeoFileFromFY3C_MERSI(fileRaster);
                if (kmGeo == null)
                {
                    kmGeo = TryFindQkmGeoFileFromFY3C_MERSI(fileRaster);
                }
                if (kmGeo == null)
                    throw new Exception("无法找到角度数据(如经纬度等)文件[._GEO1K_...HDF]或[._GEOQK_...HDF]");
                return kmGeo;
            }
            catch (Exception ex)
            {
                throw new Exception("打开经纬度文件失败" + ex.Message, ex);
            }
        }

        public static IRasterDataProvider TryFindkmGeoFileFromFY3C_MERSI(IRasterDataProvider fileRaster)
        {
            string fileName = fileRaster.fileName;
            string dir = Path.GetDirectoryName(fileName);
            fileName = Path.GetFileName(fileName);
            string retFile;
            if (fileName.Contains("_1000M_"))
                retFile = fileName.Replace("_1000M_", "_GEO1K_");
            else if (fileName.Contains("_0250M_"))
                retFile = fileName.Replace("_0250M_", "_GEO1K_");
            else
                return null;
            string fileFullPath = Path.Combine(dir, retFile);
            if (retFile == fileName || !File.Exists(fileFullPath))
                return null;
            try
            {
                return Open(fileFullPath);
            }
            catch
            {
                return null;
            }
        }

        public static IRasterDataProvider TryFindQkmGeoFileFromFY3C_MERSI(IRasterDataProvider fileRaster)
        {
            string fileName = fileRaster.fileName;
            string dir = Path.GetDirectoryName(fileName);
            fileName = Path.GetFileName(fileName);
            string retFile;
            if (fileName.Contains("_1000M_"))
                retFile = fileName.Replace("_1000M_", "_GEOQK_");
            else if (fileName.Contains("_0250M_"))
                retFile = fileName.Replace("_0250M_", "_GEOQK_");
            else
                return null;
            string fileFullPath = Path.Combine(dir, retFile);
            if (retFile == fileName || !File.Exists(fileFullPath))
                return null;
            try
            {
                return Open(fileFullPath);
            }
            catch
            {
                return null;
            }
        }

        public static IRasterDataProvider TryFindFY3C_MERSI_1KM_L1FromQKM(IRasterDataProvider fileRaster)
        {
            try
            {
                string dir = Path.GetDirectoryName(fileRaster.fileName);
                string fileName = Path.GetFileName(fileRaster.fileName);
                string retFile = fileName.Replace("_0250M_", "_1000M_");
                retFile = Path.Combine(dir, retFile);
                if (retFile == fileName || !File.Exists(retFile))
                    return null;
                return Open(retFile);
            }
            catch
            {
                return null;
            }
        }
        public static void GetModisBandmapTable(IRasterDataProvider kmRaster, IRasterDataProvider hkmRaster, IRasterDataProvider qkmRaster, int[] bandNumbers,
           out int[] qkmBandNumberMaps, out int[] hkmBandNumberMaps, out int[] kmBandNumberMaps)
        {
            qkmBandNumberMaps = null;
            hkmBandNumberMaps = null;
            kmBandNumberMaps = null;
            if (qkmRaster == null && kmRaster == null && hkmRaster == null)
                return;
            int qkmBandLength = PrjBand.MODIS_250_Orbit.Length;
            int hkmBandLength = PrjBand.MODIS_500_Orbit.Length;
            int kmBandLength = PrjBand.MODIS_1000_Orbit.Length;
            if (bandNumbers == null || bandNumbers.Length == 0)
            {
                bandNumbers = new int[kmBandLength];
                for (int i = 0; i < bandNumbers.Length; i++)
                {
                    bandNumbers[i] = i + 1;
                }
            }
            List<int> qkm = new List<int>();
            List<int> hkm = new List<int>();
            List<int> km = new List<int>();
            for (int i = 0; i < bandNumbers.Length; i++)
            {
                if (qkmRaster != null)
                {
                    if (bandNumbers[i] <= qkmBandLength)
                    {
                        qkm.Add(bandNumbers[i]);//当前通道号为bandIndexs[i]，目标的为i
                        continue;
                    }
                }
                if (hkmRaster != null)
                {
                    if (bandNumbers[i] <= hkmBandLength)
                    {
                        hkm.Add(bandNumbers[i]);//当前通道号为bandIndexs[i]，目标的为i
                        continue;
                    }
                }
                if (kmRaster != null)
                    if (bandNumbers[i] <= kmBandLength)
                        km.Add(bandNumbers[i]);
            }
            qkmBandNumberMaps = qkm.Count == 0 ? null : qkm.ToArray();
            hkmBandNumberMaps = hkm.Count == 0 ? null : hkm.ToArray();
            kmBandNumberMaps = km.Count == 0 ? null : km.ToArray();
        }

        internal static void GetBandmapTableMERSI(IRasterDataProvider qkmRaster, IRasterDataProvider kmRaster, int[] bandNumbers,
            out int[] qkmBandNoMaps, out int[] kmBandNoMaps)
        {
            qkmBandNoMaps = null;
            kmBandNoMaps = null;
            if (qkmRaster == null && kmRaster == null)
                return;
            int kmBandLength = PrjBand.MERSI_1000_Orbit.Length;
            int qkmBandLength = PrjBand.MERSI_0250_Orbit.Length;
            if (bandNumbers == null || bandNumbers.Length == 0)
            {
                bandNumbers = new int[kmBandLength];
                for (int i = 0; i < bandNumbers.Length; i++)
                {
                    bandNumbers[i] = i + 1;
                }
            }
            List<int> qkm = new List<int>();
            List<int> km = new List<int>();
            for (int i = 0; i < bandNumbers.Length; i++)
            {
                if (qkmRaster != null)
                {
                    if (bandNumbers[i] <= qkmBandLength)
                    {
                        qkm.Add(bandNumbers[i]);//当前通道号为bandIndexs[i]，目标的为i
                        continue;
                    }
                }
                if (kmRaster != null)
                    if (bandNumbers[i] <= kmBandLength)
                        km.Add(bandNumbers[i]);
            }
            qkmBandNoMaps = qkm.Count == 0 ? null : qkm.ToArray();
            kmBandNoMaps = km.Count == 0 ? null : km.ToArray();
        }

        public static void GetVIRRBandmapTable(int[] bandNumbers, out int[] kmBandNoMaps)
        {
            kmBandNoMaps = null;
            int kmBandLength = PrjBand.VIRR_1000_Orbit.Length;
            if (bandNumbers == null || bandNumbers.Length == 0)
            {
                bandNumbers = new int[kmBandLength];
                for (int i = 0; i < bandNumbers.Length; i++)
                {
                    bandNumbers[i] = i + 1;
                }
            }
            List<int> km = new List<int>();
            for (int i = 0; i < bandNumbers.Length; i++)
            {
                if (bandNumbers[i] <= kmBandLength)
                    km.Add(bandNumbers[i]);
            }
            kmBandNoMaps = km.Count == 0 ? null : km.ToArray();
        }

        public static void GetNoaaBandmapTable(int[] bandIndexs, out int[] kmBandNoMaps)
        {
            kmBandNoMaps = null;
            int kmBandLength = PrjBand.AVHRR_1000_Orbit.Length;
            if (bandIndexs == null || bandIndexs.Length == 0)
            {
                bandIndexs = new int[kmBandLength];
                for (int i = 0; i < bandIndexs.Length; i++)
                {
                    bandIndexs[i] = i + 1;
                }
            }
            List<int> km = new List<int>();
            for (int i = 0; i < bandIndexs.Length; i++)
            {
                if (bandIndexs[i] <= kmBandLength)
                    km.Add(bandIndexs[i]);
            }
            kmBandNoMaps = km.Count == 0 ? null : km.ToArray();
        }

        public static void GetVISSRBandmapTable(int[] bandIndexs, out int[] kmBandNoMaps)
        {
            kmBandNoMaps = null;
            int kmBandLength = PrjBand.VISSR_5000_Orbit.Length;
            if (bandIndexs == null || bandIndexs.Length == 0)
            {
                bandIndexs = new int[kmBandLength];
                for (int i = 0; i < bandIndexs.Length; i++)
                {
                    bandIndexs[i] = i + 1;
                }
            }
            List<int> km = new List<int>();
            for (int i = 0; i < bandIndexs.Length; i++)
            {
                if (bandIndexs[i] <= kmBandLength)
                    km.Add(bandIndexs[i]);
            }
            kmBandNoMaps = km.Count == 0 ? null : km.ToArray();
        }
    }
}
