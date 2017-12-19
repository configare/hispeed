#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/27 14:04:22
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

namespace GeoDo.RSS.DF.FY1D
{
    /// <summary>
    /// 类名：FileHeaderSetterFor1A5
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/27 14:04:22
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public static class FileHeaderSetterFor1A5
    {
        public static D1A5Header Set1A5Header(string filename)
        {
            FileStream fs = null;
            BinaryReader br = null;
            D1A5Header d1a5Header = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs, Encoding.Default);
                d1a5Header = CreateFileHeader(fs, br, 0, 9744) as D1A5Header;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
                if (br != null)
                    br.Close();
            }
            return d1a5Header;
        }

        private static D1A5Header CreateFileHeader(FileStream fs, BinaryReader br, int offset, int endOffset)
        {
            D1A5Header hInfo = new D1A5Header();
            fs.Seek(offset, SeekOrigin.Begin);
            hInfo.SatelliteIdentify = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            hInfo.DataBeginYear = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            hInfo.DataBeginMilliSecond = ToLocalEndian.ToUInt32FromBig(br.ReadBytes(4));
            hInfo.DataBeginDayNums = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            hInfo.DataEndYear = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            hInfo.DataEndMilliSecond = ToLocalEndian.ToUInt32FromBig(br.ReadBytes(4));
            hInfo.DataEndDayNums = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            hInfo.RecordCount = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            hInfo.LastRecord = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            hInfo.ErrorFrameCount = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            hInfo.BitErrorRatio = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            fs.Seek(2, SeekOrigin.Current);
            hInfo.ErrorTimeOrder = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            hInfo.LostRecordCount = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            hInfo.SlopeAnalyseResult = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            fs.Seek(164, SeekOrigin.Current);
            hInfo.TrackNumber = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            hInfo.EpochTrackTime = ToLocalEndian.ToDouble64FromBig(br.ReadBytes(8));
            hInfo.Time = GetTime(hInfo.EpochTrackTime);
            hInfo.OrbitSemiMajorAxis = ToLocalEndian.ToDouble64FromBig(br.ReadBytes(8));
            hInfo.OrbitEccentricity = ToLocalEndian.ToDouble64FromBig(br.ReadBytes(8));
            hInfo.OrbitInclination = ToLocalEndian.ToDouble64FromBig(br.ReadBytes(8));
            hInfo.LongitudeAscendingNode = ToLocalEndian.ToDouble64FromBig(br.ReadBytes(8));
            hInfo.PerigeeAngle = ToLocalEndian.ToDouble64FromBig(br.ReadBytes(8));
            hInfo.MeanAnomaly = ToLocalEndian.ToDouble64FromBig(br.ReadBytes(8));
            hInfo.AscDescendTag = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            hInfo.ResurceType = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            hInfo.OrbitNumber = ToLocalEndian.ToUInt16FromBig(br.ReadBytes(2));
            fs.Seek(2, SeekOrigin.Current);
            hInfo.OrbitCycle = ToLocalEndian.ToDouble64FromBig(br.ReadBytes(8));
            //角度信息
            hInfo.Angles = GetAngles(br.ReadBytes(24));
            fs.Seek(20, SeekOrigin.Current);
            float[] lats,lons;
            GetPosition(br.ReadBytes(32),out lats,out lons);
            hInfo.Lats = lats;
            hInfo.Lons = lons;
            fs.Seek(4, SeekOrigin.Current);
            hInfo.DataBeginSecond = ToLocalEndian.ToDouble64FromBig(br.ReadBytes(8));
            hInfo.DataEndSecond = ToLocalEndian.ToDouble64FromBig(br.ReadBytes(8));
            hInfo.SatelliteName = hInfo.SatelliteIdentify == 113 ? "FY1C" : "FY1D";
            hInfo.OrbitBeginTime = DateTime.Parse(hInfo.DataBeginYear.ToString() + "-01-01").AddDays(hInfo.DataBeginDayNums - 1).AddMilliseconds(hInfo.DataBeginMilliSecond);
            return hInfo;
        }

        //计算从80年开始以秒为单位的时间
        private static DateTime GetTime(double seconds)
        {
            DateTime time = new DateTime(1980, 1, 1);
            time = time.AddSeconds(seconds);
            return time;
        }

        private static double[] GetAngles(byte[] datas)
        {
            byte[] angle = new byte[8];
            double[] angles = new double[3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    angle[j] = datas[j + 8 * i];
                }
                angles[i] = ToLocalEndian.ToDouble64FromBig(angle);
            }
            return angles;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="lats">左下 右下  左上  右上</param>
        /// <param name="lons">左下 右下  左上  右上</param>
        private static void GetPosition(byte[] positions,out float[] lats,out float[] lons)
        {
            lats=new float[4];
            lons = new float[4];
            byte[] lon = new byte[4];
            byte[] lat = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    lat[j] = positions[j + 8 * i];
                    lon[j] = positions[j + 4 + 8 * i];

                }
                lats[i] = ToLocalEndian.ToFloatFromBig(lat);
                lons[i] = ToLocalEndian.ToFloatFromBig(lon);
            }
        }
    }
}
