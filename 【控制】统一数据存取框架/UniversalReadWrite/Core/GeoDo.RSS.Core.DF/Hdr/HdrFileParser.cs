using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;

namespace GeoDo.RSS.Core.DF
{
    public static class HdrFileParser
    {
        public static HdrFile ParseFromHdrfile(string hdrfile)
        {
            string[] regexStrs = new string[] {@"^\s*\bdescription\b\s*=\s*{(?<Des>.*)}$",
            @"^\s*\bsamples\b\s*=\s*(?<samples>\d*)$",
            @"^\s*\blines\b\s*=\s*(?<lines>\d*)$",
            @"^\s*\bbands\b\s*=\s*(?<bands>\d*)$",
            @"^\s*\bheader\b\s*\boffset\b\s*=\s*(?<headerOffset>\d*)$",
            @"^\s*\bfile\b\s*\btype\b\s*=\s*(?<fileType>.*)$",
            @"^\s*\bdata\b\s*\btype\b\s*=\s*(?<dataType>\d*)$",
            @"^\s*\binterleave\b\s*=\s*(?<interleave>.*)$",
            @"^\s*\bsensor\b\s*\btype\b\s*=\s*(?<sensorType>.*)$",
            @"^\s*\bbyte\b\s*\border\b\s*=\s*(?<byteOder>\d*)$",
            @"^\s*\bmap\b\s*\binfo\b\s*=\s*{(?<prjName>.*),\s*(?<CX>\d+(\.\d+)?)\s*,\s*(?<CY>\d+(\.\d+)?)\s*,\s*(?<CLon>-?\d+(\.\d+)?)([E|e](?<ELon>\+?\d+))?\s*,\s*(?<CLat>-?\d+(\.\d+)?)([E|e](?<ELat>\+?\d+))?\s*,\s*(?<RLon>-?\d+(\.\d+)?)([E|e](?<ERLon>-?\d+))?\s*,\s*(?<RLat>-?\d+(\.\d+)?)([E|e](?<ERLat>-?\d+))?\s*,\s*(?<Project>\S*)\s*,\s*\bunits\b\s*=\s*(?<units>\S*)\s*}",
            @"^\s*\bband\b\s*\bnames\b\s*=\s*{\s*(?<bandNames>[^}]*)",
            @"^\s*\bgeo\b\s*\bpoints\b\s*=\s*{\s*(?<geoPoints>[^}]*)}$"};

            HdrFile _hdrFile = new HdrFile();
            string projExp = @"\bprojection\b\s*\binfo\b\s*=\s*{(?<ID>\d+),.+}";
            string fileString = File.ReadAllText(hdrfile, Encoding.Default);
            Match prjM = Regex.Match(fileString, projExp, RegexOptions.IgnoreCase);
            if (prjM.Success)
            {
                _hdrFile.HdrProjectionInfo = GetHdrProjectionInfo(prjM.Value);
            }
            //
            string[] context = File.ReadAllLines(hdrfile, Encoding.Default);
            int j = 0;
            foreach (string str in context)
            {
                for (int i = 0; i < regexStrs.Length; i++)
                {
                    Regex regex = new Regex(regexStrs[i]);
                    Match m = regex.Match(str);
                    if (m.Success)
                    {
                        switch (i)
                        {
                            case 0: _hdrFile.Description = m.Groups["Des"].Value; break;
                            case 1: _hdrFile.Samples = Convert.ToInt32(string.IsNullOrEmpty(m.Groups["samples"].Value) ? "0" : m.Groups["samples"].Value); break;
                            case 2: _hdrFile.Lines = Convert.ToInt32(string.IsNullOrEmpty(m.Groups["lines"].Value) ? "0" : m.Groups["lines"].Value); break;
                            case 3: _hdrFile.Bands = Convert.ToInt32(string.IsNullOrEmpty(m.Groups["bands"].Value) ? "0" : m.Groups["bands"].Value); break;
                            case 4: _hdrFile.HeaderOffset = Convert.ToInt32(string.IsNullOrEmpty(m.Groups["headerOffset"].Value) ? "0" : m.Groups["headerOffset"].Value); break;
                            case 5: _hdrFile.FileType = m.Groups["fileType"].Value; break;
                            case 6: _hdrFile.DataType = Convert.ToInt32(string.IsNullOrEmpty(m.Groups["dataType"].Value) ? "0" : m.Groups["dataType"].Value); break;
                            case 7: _hdrFile.Intertleave = GetEnuminterleave(m.Groups["interleave"].Value); break;
                            case 8: _hdrFile.SensorType = m.Groups["sensorType"].Value; break;
                            case 9: _hdrFile.ByteOrder = GetEnumByteOrder(m.Groups["byteOder"].Value); break;
                            case 10:
                                _hdrFile.MapInfo = GetMapInfo(m.Groups["prjName"].Value, m.Groups["CX"].Value, m.Groups["CY"].Value, m.Groups["CLon"].Value, m.Groups["CLat"].Value, m.Groups["ELon"].Value, m.Groups["ELat"].Value, m.Groups["RLon"].Value, m.Groups["ERLon"].Value, m.Groups["RLat"].Value, m.Groups["ERLat"].Value, m.Groups["Project"].Value, m.Groups["units"].Value); break;
                            case 11: _hdrFile.BandNames = GetBandNames(m.Groups["bandNames"].Value, str, context, j); break;
                            case 12: _hdrFile.GeoPoints = GetGeoPoint(m.Groups["geoPoints"].Value); break;
                        }
                        break;
                    }
                }
                j++;
            }
            ParseSatelliteSensor(ref _hdrFile);
            return _hdrFile;
        }

        private static void ParseSatelliteSensor(ref HdrFile hdr)
        {
            if (string.IsNullOrEmpty(hdr.Description) || string.IsNullOrEmpty(hdr.SensorType))
                return;
            Int16 satelliteType = -1;
            if (!Int16.TryParse(hdr.SensorType, out satelliteType))
                return;
            string regStr = @"\S*\s*\S*filedatetime:(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})\s*(?<hour>\d{2})(?<minute>\d{2})(?<second>\d{2})\S*";
            DateTime dt = DateTime.MinValue;
            Regex reg = new Regex(regStr);
            if (reg.IsMatch(hdr.Description.ToLower()))
            {
                Match m = reg.Match(hdr.Description.ToLower());
                if (!DateTime.TryParse(m.Groups["year"].Value + "-" + m.Groups["month"].Value + "-" + m.Groups["day"].Value + " " + m.Groups["hour"].Value
                    + ":" + m.Groups["minute"].Value + ":" + m.Groups["second"].Value, out dt))
                    dt = DateTime.MinValue;
            }
        }

        private static HdrProjectionInfo GetHdrProjectionInfo(string prjInfoString)
        {
            if (string.IsNullOrEmpty(prjInfoString))
                return null;
            prjInfoString = prjInfoString.ToUpper();
            prjInfoString = prjInfoString.Replace("{", string.Empty).Replace("}", string.Empty);
            string exp = @"\bPROJECTION\s*INFO\s*=\s*";
            prjInfoString = Regex.Replace(prjInfoString, exp, string.Empty);
            string[] parts = prjInfoString.Split(',');
            if (parts.Length < 3)
                return null;
            int i = 0;
            List<float> argList = new List<float>();
            foreach (string p in parts)
            {
                float v = 0;
                if (float.TryParse(p, out v))
                {
                    argList.Add(v);
                    i++;
                }
                else
                    break;
            }
            string name = string.Empty;
            string units = "Meters";
            string datum = "WGS-84";
            if (i < parts.Length)
            {
                datum = parts[i];
                if (i + 1 < parts.Length)
                {
                    name = parts[i + 1];
                }
                if (i + 2 < parts.Length)
                {
                    units = parts[i + 2];
                    exp = @"\bUNITS\s*=\s*";
                    units = Regex.Replace(units, exp, string.Empty);
                }
            }
            int ID = (int)argList[0];
            float[] args = new float[argList.Count - 1];
            for (int j = 1; j < i; j++)
                args[j - 1] = argList[j];
            HdrProjectionInfo prjInfo = new HdrProjectionInfo();
            prjInfo.ProjectionID = ID;
            prjInfo.PrjArguments = args;
            prjInfo.Units = units;
            prjInfo.Name = name;
            return prjInfo;
        }
      
        private static HdrMapInfo GetMapInfo(string prjName, string cX, string cY, string cLon, string cLat, string eLon, string eLat, string rLon, string erLon, string rLat, string erLat, string project, string units)
        {
            HdrMapInfo hdrMapInfo = new HdrMapInfo();
            hdrMapInfo.Name = prjName;
            hdrMapInfo.BaseRowColNumber = new System.Drawing.Point(Convert.ToInt32(string.IsNullOrEmpty(cX) ? "-1" : (cX.IndexOf('.') == -1 ? cX : cX.Substring(0, cX.IndexOf('.')))), Convert.ToInt32(string.IsNullOrEmpty(cY) ? "-1" : (cY.IndexOf('.') == -1 ? cY : cY.Substring(0, cY.IndexOf('.')))));
            hdrMapInfo.BaseMapCoordinateXY.Latitude = Convert.ToDouble(string.IsNullOrEmpty(cLat) ? "-1" : cLat) * (string.IsNullOrEmpty(eLat) ? 1 : Math.Pow(10, Convert.ToDouble(eLat))); ;
            hdrMapInfo.BaseMapCoordinateXY.Longitude = Convert.ToDouble(string.IsNullOrEmpty(cLon) ? "-1" : cLon) * (string.IsNullOrEmpty(eLon) ? 1 : Math.Pow(10, Convert.ToDouble(eLon))); ;
            hdrMapInfo.XYResolution.Latitude = Math.Round(Convert.ToDouble(string.IsNullOrEmpty(rLat) ? "-1" : rLat), 6) * (string.IsNullOrEmpty(erLat) ? 1 : Math.Pow(10, Convert.ToDouble(erLat)));
            hdrMapInfo.XYResolution.Longitude = Math.Round(Convert.ToDouble(string.IsNullOrEmpty(rLon) ? "-1" : rLon), 6) * (string.IsNullOrEmpty(erLon) ? 1 : Math.Pow(10, Convert.ToDouble(erLon)));
            hdrMapInfo.CoordinateType = project;
            hdrMapInfo.Units = units;
            return hdrMapInfo;
        }

        private static string[] GetBandNames(string bandNames, string str, string[] context, int index)
        {
            if (str.IndexOf("}") != -1)
                return string.IsNullOrEmpty(bandNames) ? null : bandNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder(bandNames);
            for (int i = index + 1; i < context.Length; i++)
            {
                if (context[i].IndexOf("}") == -1)
                    sb.Append(context[i]);
                else
                    sb.Append(context[i].Substring(0, context[i].IndexOf("}")));
            }
            return string.IsNullOrEmpty(sb.ToString()) ? null : sb.ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static HdrGeoPoint[] GetGeoPoint(string geoPoint)
        {
            List<HdrGeoPoint> geoPointsList = new List<HdrGeoPoint>();
            HdrGeoPoint _geoPoint = new HdrGeoPoint();
            string[] geoPoints = geoPoint.Split(',');
            for (int i = 0; i < geoPoints.Length; i++)
            {
                if (i == 0)
                    _geoPoint.PixelPoint.Y = Convert.ToInt32(geoPoints[i]);
                else
                {
                    switch (i % 4)
                    {
                        case 1: _geoPoint.PixelPoint.X = Convert.ToInt32(geoPoints[i]); break;
                        case 2: _geoPoint.GeoPoint.Latitude = Convert.ToDouble(geoPoints[i]); break;
                        case 3: _geoPoint.GeoPoint.Longitude = Convert.ToDouble(geoPoints[i]); break;
                        case 0: _geoPoint.PixelPoint.Y = Convert.ToInt32(geoPoints[i]); break;
                    }
                }
                if (i != 0 && i % 4 == 0)
                    geoPointsList.Add(_geoPoint);
            }
            return geoPointsList.ToArray();
        }

        private static enumHdrByteOder GetEnumByteOrder(string byteOder)
        {
            enumHdrByteOder _enumByteOder = new enumHdrByteOder();
            switch (Convert.ToInt32(byteOder))
            {
                case 0: _enumByteOder = enumHdrByteOder.Host_intel; break;
                case 1: _enumByteOder = enumHdrByteOder.Network_IEEE; break;
            }
            return _enumByteOder;
        }

        private static enumInterleave GetEnuminterleave(string interleave)
        {
            enumInterleave _enumInterleave = new enumInterleave();
            switch (interleave.ToLower().Trim())
            {
                case "bsq": _enumInterleave = enumInterleave.BSQ; break;
                case "bil": _enumInterleave = enumInterleave.BIL; break;
                case "bip": _enumInterleave = enumInterleave.BIP; break;
            }
            return _enumInterleave;
        }
    }
}
