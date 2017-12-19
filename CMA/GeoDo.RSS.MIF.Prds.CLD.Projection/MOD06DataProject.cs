using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.FileProject;
using GeoDo.RSS.Core.DF;
using System.Text.RegularExpressions;
using GeoDo.RasterProject;
using GeoDo.Project;

namespace GeoDo.RSS.MIF.Prds.CLD.Projection
{
    public class MOD06DataProject
    {
        public static void Project(PrjEnvelopeItem prjItem, string dataSet, string locationFile, string dataFile, string outFilename, float outResolution)
        {
            IRasterDataProvider mainRaster = null;
            IRasterDataProvider locationRaster = null;
            try
            {
                string[] openArgs = new string[] { "datasets=" + dataSet };
                mainRaster = RasterDataDriver.Open(dataFile, openArgs) as IRasterDataProvider;
                string[] locationArgs = new string[] { "datasets=" + "Latitude,Longitude", "geodatasets=" + "Latitude,Longitude" };
                locationRaster = RasterDataDriver.Open(locationFile, locationArgs) as IRasterDataProvider;
                if (locationRaster == null || locationRaster.BandCount == 0)
                {
                    return;
                }
                HDF4FilePrjSettings setting = new HDF4FilePrjSettings();
                setting.LocationFile = locationRaster;
                setting.OutFormat = "LDF";
                setting.OutResolutionX = setting.OutResolutionY = outResolution;
                Dictionary<string, double> exargs = new Dictionary<string, double>();
                double invalidValue;
                if (double.TryParse(GetDefaultNullValue(mainRaster.DataType), out invalidValue))
                {
                    exargs.Add("FillValue", invalidValue);
                    setting.ExtArgs = new object[]{exargs};
                }
                HDF4FileProjector projector = new HDF4FileProjector();
                GeoDo.RasterProject.PrjEnvelope mainPrj;
                projector.ComputeDstEnvelope(mainRaster, SpatialReference.GetDefault(), out mainPrj, null);
                GeoDo.RasterProject.PrjEnvelope dstmainPrj = GeoDo.RasterProject.PrjEnvelope.Intersect(prjItem.PrjEnvelope, mainPrj);
                if (dstmainPrj != null)
                {
                    setting.OutEnvelope = dstmainPrj;
                    setting.OutPathAndFileName = outFilename;
                        projector.Project(mainRaster, setting, SpatialReference.GetDefault(), null);
                }
                else
                {
                    return;
                }
            }
            catch (System.Exception ex)
            {
                string fname =Path.GetFileName(dataFile);
                string label=null;
                if (fname.Contains("MOD") )
                    label="MOD06ProjectError";
                else if(fname.Contains("MYD") )
                    label="MYD06ProjectError";
                else
                    label="AIRSProjectError";
                LogFactory.WriteLine(label, "投影失败!" + ";" + dataFile + ";" + dataSet + ";" + outResolution.ToString("f2") + ";" + outFilename + ";" + ex.Message);
            }
            finally
            {
                if (mainRaster != null)
                {
                    mainRaster.Dispose();
                    mainRaster = null;
                }
                if (locationRaster != null)
                {
                    locationRaster.Dispose();
                    locationRaster = null;
                }
            }
        }

        public static DateTime GetOribitTime(string fileName,out string orbitTime)
        {
            int year;
            orbitTime = "";
            string fName = Path.GetFileName(fileName).ToUpper();
            Match match = Regex.Match(fName, @".(?<year>\d{4})(?<days>\d{3}).(?<hour>\d{2})(?<minutes>\d{2}).");
            if (match.Success)
            {
                year = Int32.Parse(match.Groups["year"].Value);
                int daysofyear = Int32.Parse(match.Groups["days"].Value);
                DateTime date = GetDateFormDaysOfYear(year, daysofyear);
                date = date.AddHours(double.Parse(match.Groups["hour"].Value)).AddMinutes(double.Parse(match.Groups["minutes"].Value));
                orbitTime = date.ToString("yyyyMMddhhmm");
                return date;
            }
            return DateTime.MinValue;
        }

        private static DateTime GetDateFormDaysOfYear(int year,int daysofyear)
        {
            int[,] tab={{0,31,28,31,30,31,30,31,31,30,31,30,31},{0,31,29,31,30,31,30,31,31,30,31,30,31}};
            int k;
            int leap = DateTime.IsLeapYear(year)?1:0;
            for (k = 1; daysofyear > tab[leap, k]; k++)
                daysofyear = daysofyear - tab[leap, k];
            DateTime date = new DateTime(year, k, daysofyear);
            return date;
        }

        private  static string GetDefaultNullValue(enumDataType datatype)
        {
            switch (datatype)
            {
                case enumDataType.Byte:
                    return "126";
                case enumDataType.Double:
                    return "-32767";
                case enumDataType.Float:
                    return "-32767";
                case enumDataType.Int16:
                    return "-32767";
                case enumDataType.Int32:
                    return "-32767";
                case enumDataType.Int64:
                    return "-32767";
                case enumDataType.UInt16:
                    return UInt16.MaxValue.ToString();
                case enumDataType.UInt32:
                    return UInt16.MaxValue.ToString();
                case enumDataType.UInt64:
                    return UInt16.MaxValue.ToString();
                case enumDataType.Unknow:
                case enumDataType.Atypism:
                case enumDataType.Bits:
                default:
                    break;
            }
            return null;
        }
    }
}
