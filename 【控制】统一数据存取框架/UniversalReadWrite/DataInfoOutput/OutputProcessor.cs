#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/10/10 16:05:46
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;

namespace DataInfoOutput
{
    /// <summary>
    /// 类名：OutputProcessor
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/10/10 16:05:46
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class OutputProcessor
    {
        public void Process(string inputDir, string outDir, string type, Action<int, string> progressCallback)
        {
            switch (type)
            {
                case "FY1":
                    ProcessForFY1(inputDir, outDir, progressCallback);
                    break;
                case "FY3":
                    ProcessForFY3(inputDir, outDir, progressCallback);
                    break;
            }
        }

        private unsafe void ProcessForFY3(string inputDir, string outDir, Action<int, string> progressCallback)
        {
            if (string.IsNullOrEmpty(inputDir) || !Directory.Exists(inputDir))
                return;
            string[] fileNames = Directory.GetFiles(inputDir, "*.hdf");
            if (fileNames.Length > 0)
            {
                IRasterDataProvider dataPrd = null;
                if (string.IsNullOrEmpty(outDir))
                    outDir = Path.Combine(inputDir, "辅助信息");
                if (!Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);
                if (progressCallback != null)
                    progressCallback(0, "开始进行处理...");
                int curStep = -1;
                foreach (string file in fileNames)
                {
                    try
                    {
                        curStep++;
                        int curp = (int)((curStep * 1.0f / fileNames.Length) * 100);
                        if (progressCallback != null)
                            progressCallback(curp, "处理完成" + curp + "%");
                        dataPrd = GeoDataDriver.Open(file) as IRasterDataProvider;
                        //纬度
                        ReadAndWriteOneBandRaw(dataPrd, "Latitude", outDir);
                        //经度
                        ReadAndWriteOneBandRaw(dataPrd, "Longitude", outDir);
                        //通道
                        IRasterBand band = dataPrd.GetRasterBand(5);
                        byte[] buffer = new byte[band.Width * band.Height * 2];
                        fixed (byte* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(0, 0, dataPrd.Width, dataPrd.Height, bufferPtr, enumDataType.UInt16, dataPrd.Width, dataPrd.Height);
                        }
                        //输出单波段
                        string outFileName = Path.Combine(outDir, Path.GetFileNameWithoutExtension(file) + "_ch5.raw");
                        using (FileStream fs = new FileStream(outFileName, FileMode.Create, FileAccess.Write))
                        {
                            using (BinaryWriter br = new BinaryWriter(fs))
                            {
                                br.Write(buffer);
                            }
                        }
                        string hdrFile = Path.ChangeExtension(outFileName, ".hdr");
                        SaveHdrFile(hdrFile, dataPrd.Width, dataPrd.Height, 12);
                    }
                    finally
                    {
                        if (dataPrd != null)
                            dataPrd.Dispose();
                    }
                }
                if (progressCallback != null)
                    progressCallback(100, "文件夹" + inputDir + "FY3数据处理完成!");
            }
        }

        private unsafe void ProcessForFY1(string inputDir, string outDir, Action<int, string> progressCallback)
        {
            if (string.IsNullOrEmpty(inputDir) || !Directory.Exists(inputDir))
                return;
            string[] fileNames = Directory.GetFiles(inputDir, "*.1A5");
            //string fileName = @"G:\临时文件夹\FY1D\FY1D_AVHRR_GDPT_L1_ORB_MLT_NUL_20070101_1058_4000M_PJ.1A5";
            if (fileNames.Length > 0)
            {
                IRasterDataProvider dataPrd = null;

                if (string.IsNullOrEmpty(outDir))
                    outDir = Path.Combine(inputDir, "辅助信息");
                if (!Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);
                if (progressCallback != null)
                    progressCallback(0, "开始进行处理...");
                int curStep = -1;
                foreach (string file in fileNames)
                {
                    try
                    {
                        curStep++;
                        int curp = (int)((curStep * 1.0f / fileNames.Length) * 100);
                        if (progressCallback != null)
                            progressCallback(curp, "处理完成" + curp + "%");
                        dataPrd = GeoDataDriver.Open(file) as IRasterDataProvider;
                        //IRasterBand band = dataPrd.GetRasterBand(4);
                        //byte[] buffer = new byte[band.Width * band.Height * 2];
                        //fixed (byte* ptr = buffer)
                        //{
                        //    IntPtr bufferPtr = new IntPtr(ptr);
                        //    band.Read(0, 0, dataPrd.Width, dataPrd.Height, bufferPtr, enumDataType.UInt16, dataPrd.Width, dataPrd.Height);
                        //}
                        ////输出单波段
                        //string outFileName = Path.Combine(outDir, Path.GetFileNameWithoutExtension(file) + "_ch5.raw");
                        //using (FileStream fs = new FileStream(outFileName, FileMode.Create, FileAccess.Write))
                        //{
                        //    using (BinaryWriter br = new BinaryWriter(fs))
                        //    {
                        //        br.Write(buffer);
                        //    }
                        //}
                        //string hdrFile = Path.ChangeExtension(outFileName, ".hdr");
                        //SaveHdrFile(hdrFile, band.Width, band.Height, 12);
                        //轨道根数
                        string headerFile = Path.Combine(outDir, Path.GetFileNameWithoutExtension(file) + ".HeaderInfo.txt");
                        string headerstring = (dataPrd as GeoDo.RSS.DF.FY1D.FY1_1A5DataProvider).GetHeaderInfo();
                        File.WriteAllText(headerFile, headerstring);
                        //经纬度
                        //string lonlatFile = Path.Combine(outDir, Path.GetFileNameWithoutExtension(file) + ".LonLat.txt");
                        //double[] firstLon; double[] firstLat; double[] lastLon; double[] lastLat;
                        //(dataPrd as GeoDo.RSS.DF.FY1D.FY1_1A5DataProvider).GetPositionInfo(out firstLon, out firstLat, out lastLon, out lastLat);
                        //StringBuilder lonstr1 = new StringBuilder();
                        //lonstr1.AppendLine("首行纬度、首行经度、末行纬度、末行经度如下：");
                        //lonstr1.AppendLine(string.Join(",", firstLon));
                        //lonstr1.AppendLine(string.Join(",", firstLat));
                        //lonstr1.AppendLine(string.Join(",", lastLon));
                        //lonstr1.AppendLine(string.Join(",", lastLat));
                        //lonstr1.AppendLine("采样点数：1018 ，定位点51个，从第8个点开始，每个20个点定位一个点。");
                        //File.WriteAllText(lonlatFile, lonstr1.ToString());
                    }
                    finally
                    {
                        if (dataPrd != null)
                            dataPrd.Dispose();
                    }
                }
                if (progressCallback != null)
                    progressCallback(100, "文件夹" + inputDir + "FY1数据处理完成!");
            }
        }

        private void SaveHdrFile(string hdrFile, int width, int height, int dataType)
        {
            using (StreamWriter sw = new StreamWriter(hdrFile, false, Encoding.Default))
            {
                sw.WriteLine("ENVI");
                sw.WriteLine("description = {File Imported into ENVI}");
                sw.WriteLine(string.Format("samples = {0}", width));
                sw.WriteLine(string.Format("lines = {0}", height));
                sw.WriteLine("bands = 1");
                sw.WriteLine("file type = ENVI Standard");
                sw.WriteLine(string.Format("data type = {0}", dataType));
                sw.WriteLine("interleave = bip");
                sw.WriteLine("byte order = 0");
            }
        }

        private unsafe void ReadAndWriteOneBandRaw(IRasterDataProvider dataPrd, string fieldName, string outDirName)
        {
            IRasterBand[] bands = dataPrd.BandProvider.GetBands(fieldName);
            if (bands != null)
            {
                byte[] buffer = new byte[dataPrd.Width * dataPrd.Height * 4];
                fixed (byte* ptr = buffer)
                {
                    IntPtr bufferPtr = new IntPtr(ptr);
                    bands[0].Read(0, 0, dataPrd.Width, dataPrd.Height, bufferPtr, enumDataType.Float, dataPrd.Width, dataPrd.Height);
                }
                //输出单波段
                string outFileName = Path.Combine(outDirName, Path.GetFileNameWithoutExtension(dataPrd.fileName) + "_" + fieldName + ".raw");
                using (FileStream fs = new FileStream(outFileName, FileMode.Create, FileAccess.Write))
                {
                    using (BinaryWriter br = new BinaryWriter(fs))
                    {
                        br.Write(buffer);
                    }
                }
                string hdrFile = Path.ChangeExtension(outFileName, ".hdr");
                SaveHdrFile(hdrFile, dataPrd.Width, dataPrd.Height, 4);
            }
        }
    }
}
