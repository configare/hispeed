using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Text.RegularExpressions;

namespace GeoDo.RSS.BlockOper
{
    public class ClipCutHelper
    {
        public static int GetSize(enumDataType enumDataType)
        {
            switch (enumDataType)
            {
                case enumDataType.Atypism:
                    break;
                case enumDataType.Bits:
                    break;
                case enumDataType.Byte:
                    return 1;
                    break;
                case enumDataType.Double:
                    return 8;
                    break;
                case enumDataType.Float:
                    return 4;
                    break;
                case enumDataType.Int16:
                    return 2;
                    break;
                case enumDataType.Int32:
                    return 4;
                    break;
                case enumDataType.Int64:
                    return 8;
                    break;
                case enumDataType.UInt16:
                    return 2;
                    break;
                case enumDataType.UInt32:
                    return 4;
                    break;
                case enumDataType.UInt64:
                    return 8;
                    break;
                case enumDataType.Unknow:
                    break;
                default:
                    break;
            }
            return -1;
        }

        public static Size GetTargetSize(BlockDef blockItem, double lonResolution, double latResolution)
        {
            double fWidth = (blockItem.MaxX - blockItem.MinX) / lonResolution;
            double fHeight = (blockItem.MaxY - blockItem.MinY) / latResolution;
            return new Size(GetInteger(fWidth, lonResolution), GetInteger(fHeight, latResolution));
        }

        public static string GetBlockFilename(BlockDef blockItem, string filename, string outdir, string driver)
        {
            RasterIdentifyForClip rid = new RasterIdentifyForClip(filename);
            string exts = string.Empty;
            if (IsAngleFile(filename))
            {
                exts = filename.Substring(filename.IndexOf('.'));
            }
            else
            {
                exts = GetExtByDriver(driver);
            }
            rid.RegionIdentify = blockItem.Name;
            string outFilename = Path.Combine(outdir, rid.ToWksFileName(exts));

            if (!File.Exists(outFilename))
                return outFilename;
            else
                while (File.Exists(outFilename))
                    outFilename = Path.Combine(outdir, UpdateFilename(outFilename, blockItem.Name, rid));
            return outFilename;
        }
        //判断裁切文件是否是角度文件
        private static bool IsAngleFile( string infile)
        {
            string[] AngleNames = { "SolarZenith", "SolarAzimuth", "SensorZenith", "SensorAzimuth" };
            for(int i=0;i<AngleNames.Length;i++)
            {
                if(infile.Contains(AngleNames[i]))
                {
                    return true;
                }
            }
            return false;

        }

        private static string UpdateFilename(string outFilename, string srcRegion, RasterIdentifyForClip rid)
        {
            string regexStr = "_" + srcRegion + @"(?<num>\d+)_";
            Match m = Regex.Match(outFilename, regexStr);
            if (m.Success)
            {
                string num = m.Groups["num"].Value;
                int numtemp = int.Parse(num) + 1;
                string newNUM = srcRegion + numtemp;
                return outFilename.Replace(srcRegion + num, newNUM);
            }
            else
            {
                rid.RegionIdentify += "1";
                return Path.Combine(Path.GetDirectoryName(outFilename), rid.ToWksFileName(Path.GetExtension(outFilename)));
            }
        }

        private static string GetExtByDriver(string driver)
        {
            switch (driver)
            {
                case "MEM":
                    return ".DAT";
                default:
                    return ".LDF";
            }
        }

        public static int GetInteger(double fWidth, double resolution)
        {
            int v = (int)Math.Round(fWidth);
            if (fWidth - v > resolution)
                v++;
            return v;
        }

        //计算所取行数
        public static int ComputeRowStep(IRasterDataProvider srcRaster, int oMinBeginRow, int oMaxEndRow)
        {
            long mSize = MemoryHelper.GetAvalidPhyMemory() / 3;
            long maxLimit = mSize;
            int row = (int)(maxLimit / srcRaster.Height);
            if (row == 0)
                row = 1;
            if (row > srcRaster.Height)
                row = srcRaster.Height;
            if (row > oMaxEndRow - oMinBeginRow)
                row = oMaxEndRow - oMinBeginRow;
            return row;
        }

        public static int[] GetOffsetIndex(int[] aoi, int beginRow, int beginCol, int tWidth, int oWidth)
        {
            if (tWidth == 0 || oWidth == 0)
                return null;
            int[] aoiCh = new int[aoi.Count()];
            for (int i = 0; i < aoi.Count(); i++)
            {
                int row = aoi[i] / tWidth - beginRow;
                int col = aoi[i] % tWidth - beginCol;
                aoiCh[i] = row * oWidth + col;
            }
            return aoiCh;
        }

        public static byte[] WriteValueToBuffer(byte[] btBuffer, int[] aoi, int typeSize)
        {
            byte[] newBuffer = new byte[btBuffer.Count()];
            for (int i = 0; i < newBuffer.Count(); i++)
            {
                newBuffer[i] = 0;
            }
            foreach (int i in aoi)
            {
                int k = i * typeSize;
                if (k >= btBuffer.Count() || k < 0)
                    continue;
                for (int j = k; j < (k + typeSize); j++)
                {
                    newBuffer[j] = btBuffer[j];
                }
            }
            return newBuffer;
        }
    }
}
